/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Handles customer receipt and supplier payment documents.
/// </summary>
public class PaymentDataModule : DocumentDataModule
{
    // ● private
    /// <summary>
    /// Rounds finance amounts to four decimal places.
    /// </summary>
    decimal RoundAmount(decimal Value) => Math.Round(Value, 4, MidpointRounding.AwayFromZero);
    /// <summary>
    /// Returns the payment settlement table.
    /// </summary>
    MemTable GetSettlementTable()
    {
        MemTable Result = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("PaymentSettlement"));
        if (Result == null)
            throw new TripousDataException("PaymentSettlement table is not available.");
        return Result;
    }
    /// <summary>
    /// Returns active settlement rows.
    /// </summary>
    List<DataRow> GetActiveSettlements()
    {
        return GetSettlementTable().Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .ToList();
    }
    /// <summary>
    /// Recalculates payment settlement totals.
    /// </summary>
    void CalculateTotals()
    {
        if (CurrentRow == null)
            return;
        decimal SettledAmount = RoundAmount(GetActiveSettlements().Sum(Row => Row.AsDecimal("Amount")));
        CurrentRow.SetValue("SettledAmount", SettledAmount);
        CurrentRow.SetValue("UnappliedAmount", RoundAmount(CurrentRow.AsDecimal("Amount") - SettledAmount));
    }
    /// <summary>
    /// Adjusts the payment amount to match the settlement total when settlement lines exist.
    /// </summary>
    void AdjustAmountToSettlementTotal()
    {
        AmountAdjustmentMessage = "";
        if (CurrentRow == null || IsPaymentCancellation())
            return;
        List<DataRow> Settlements = GetActiveSettlements();
        if (Settlements.Count == 0)
            return;
        decimal SettledAmount = RoundAmount(Settlements.Sum(Row => Row.AsDecimal("Amount")));
        decimal Amount = RoundAmount(CurrentRow.AsDecimal("Amount"));
        if (Amount == SettledAmount)
            return;
        CurrentRow.SetValue("Amount", SettledAmount);
        AmountAdjustmentMessage = $"Payment amount was adjusted from {Amount} to {SettledAmount} to match settlement total.";
    }
    /// <summary>
    /// Returns true when this module handles customer receipts.
    /// </summary>
    bool IsCustomerReceipt() => ModuleDef != null
                                && (ModuleDef.Name.IsSameText("CustomerReceipt")
                                    || ModuleDef.Name.IsSameText("CustomerReceiptCancellation"));
    /// <summary>
    /// Returns true when this module is a payment cancellation document.
    /// </summary>
    bool IsPaymentCancellation() => DocumentType != null && DocumentType.IsCancellation;
    /// <summary>
    /// Returns a database value for an optional string.
    /// </summary>
    object DbString(string Value)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return DBNull.Value;
        return Value;
    }
    /// <summary>
    /// Sets a row field value when the field exists.
    /// </summary>
    void SetValueIfColumnExists(DataRow Row, string FieldName, object Value)
    {
        if (Row.Table.Columns.Contains(FieldName))
            Row.SetValue(FieldName, Value);
    }
    /// <summary>
    /// Sets person display fields on a payment row.
    /// </summary>
    void SetPersonDisplayFields(DataRow PaymentRow)
    {
        string PersonId = PaymentRow.AsString("PersonId");
        if (string.IsNullOrWhiteSpace(PersonId))
            return;
        DataRow Person = Store.SelectResults("""
                                             select Code, Name
                                             from Person
                                             where Id = :Id
                                             """, new Dictionary<string, object>()
        {
            ["Id"] = PersonId,
        });
        if (Person == null)
            return;
        SetValueIfColumnExists(PaymentRow, "Person__Code", Person.AsString("Code"));
        SetValueIfColumnExists(PaymentRow, "Person__Name", Person.AsString("Name"));
    }
    /// <summary>
    /// Sets payment display fields for cancellation links.
    /// </summary>
    void SetPaymentDisplayFields(DataRow PaymentRow, string FieldPrefix, DataRow SourcePayment)
    {
        SetValueIfColumnExists(PaymentRow, $"{FieldPrefix}__Code", SourcePayment.AsString("Code"));
    }
    /// <summary>
    /// Applies a JSON contract object to this module before creating another document.
    /// </summary>
    void ApplyJsonSource(JsonDataModule Source)
    {
        if (Source == null)
            throw new TripousArgumentNullException(nameof(Source));

        State = (DataMode)Source.State;

        tblItem.EventsDisabled = true;
        try
        {
            JsonApplyTableRows(tblItem, Source);
        }
        finally
        {
            tblItem.EventsDisabled = false;
        }
    }
    /// <summary>
    /// Returns the generated journal entry code for the current payment.
    /// </summary>
    string GetGeneratedJournalEntryCode() => $"JE-{CurrentRow.AsString("Code")}";
    /// <summary>
    /// Returns the posting account with the specified code.
    /// </summary>
    string GetPostingAccountId(DbTransaction Transaction, string AccountCode)
    {
        DataRow Row = Store.SelectResults(Transaction, """
                                                       select Id, IsPosting, IsActive
                                                       from Account
                                                       where Code = :Code
                                                       """, new Dictionary<string, object>()
        {
            ["Code"] = AccountCode,
        });
        if (Row == null)
            throw new TripousBusinessException($"Accounting account was not found: {AccountCode}");
        if (!Row.AsBoolean("IsActive"))
            throw new TripousBusinessException($"Accounting account is not active: {AccountCode}");
        if (!Row.AsBoolean("IsPosting"))
            throw new TripousBusinessException($"Accounting account is not a posting account: {AccountCode}");
        return Row.AsString("Id");
    }
    /// <summary>
    /// Returns the finance balance row key for the specified owner.
    /// </summary>
    DataRow FindFinanceBalance(DbTransaction Transaction, int TradeTypeId, string CurrencyId, string PersonId, string CashAccountId, string CompanyBankAccountId)
    {
        if (!string.IsNullOrWhiteSpace(PersonId))
        {
            return Store.SelectResults(Transaction, """
                                                    select Id
                                                    from FinanceBalance
                                                    where TradeTypeId = :TradeTypeId
                                                      and CurrencyId = :CurrencyId
                                                      and PersonId = :PersonId
                                                      and CashAccountId is null
                                                      and CompanyBankAccountId is null
                                                    """, new Dictionary<string, object>()
            {
                ["TradeTypeId"] = TradeTypeId,
                ["CurrencyId"] = CurrencyId,
                ["PersonId"] = PersonId,
            });
        }
        if (!string.IsNullOrWhiteSpace(CashAccountId))
        {
            return Store.SelectResults(Transaction, """
                                                    select Id
                                                    from FinanceBalance
                                                    where TradeTypeId = :TradeTypeId
                                                      and CurrencyId = :CurrencyId
                                                      and PersonId is null
                                                      and CashAccountId = :CashAccountId
                                                      and CompanyBankAccountId is null
                                                    """, new Dictionary<string, object>()
            {
                ["TradeTypeId"] = TradeTypeId,
                ["CurrencyId"] = CurrencyId,
                ["CashAccountId"] = CashAccountId,
            });
        }
        return Store.SelectResults(Transaction, """
                                                select Id
                                                from FinanceBalance
                                                where TradeTypeId = :TradeTypeId
                                                  and CurrencyId = :CurrencyId
                                                  and PersonId is null
                                                  and CashAccountId is null
                                                  and CompanyBankAccountId = :CompanyBankAccountId
                                                """, new Dictionary<string, object>()
        {
            ["TradeTypeId"] = TradeTypeId,
            ["CurrencyId"] = CurrencyId,
            ["CompanyBankAccountId"] = CompanyBankAccountId,
        });
    }
    /// <summary>
    /// Updates a finance balance for a person, cash account, or bank account.
    /// </summary>
    void UpdateFinanceBalance(DbTransaction Transaction, string FinanceMovementId, DateTime MovementDate, int TradeTypeId, string CurrencyId, string PersonId, string CashAccountId, string CompanyBankAccountId, int Direction, decimal Amount)
    {
        decimal SignedAmount = RoundAmount(Direction * Amount);
        DataRow BalanceKey = FindFinanceBalance(Transaction, TradeTypeId, CurrencyId, PersonId, CashAccountId, CompanyBankAccountId);
        if (BalanceKey == null)
        {
            Store.ExecSql(Transaction, """
                                       insert into FinanceBalance
                                       (
                                         Id, TradeTypeId, CurrencyId, PersonId, CashAccountId, CompanyBankAccountId,
                                         Balance, LastMovementDate, LastMovementId
                                       )
                                       values
                                       (
                                         :Id, :TradeTypeId, :CurrencyId, :PersonId, :CashAccountId, :CompanyBankAccountId,
                                         :Balance, :LastMovementDate, :LastMovementId
                                       )
                                       """, new Dictionary<string, object>()
            {
                ["Id"] = Sys.GenId(),
                ["TradeTypeId"] = TradeTypeId,
                ["CurrencyId"] = CurrencyId,
                ["PersonId"] = DbString(PersonId),
                ["CashAccountId"] = DbString(CashAccountId),
                ["CompanyBankAccountId"] = DbString(CompanyBankAccountId),
                ["Balance"] = SignedAmount,
                ["LastMovementDate"] = MovementDate,
                ["LastMovementId"] = FinanceMovementId,
            });
            return;
        }
        DataRow Balance = Store.Provider.SelectForUpdate(Transaction, "FinanceBalance", "Id", BalanceKey["Id"]);
        Store.ExecSql(Transaction, """
                                   update FinanceBalance
                                   set Balance = :Balance,
                                       LastMovementDate = :LastMovementDate,
                                       LastMovementId = :LastMovementId
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = Balance["Id"],
            ["Balance"] = RoundAmount(Balance.AsDecimal("Balance") + SignedAmount),
            ["LastMovementDate"] = MovementDate,
            ["LastMovementId"] = FinanceMovementId,
        });
    }
    /// <summary>
    /// Inserts a finance movement for the current payment.
    /// </summary>
    string InsertFinanceMovement(DbTransaction Transaction, int TradeTypeId, DateTime MovementDate, string PersonId, string CashAccountId, string CompanyBankAccountId, int Direction, decimal Amount, string CancelledMovementId, string Remarks)
    {
        string FinanceMovementId = Sys.GenId();
        Store.ExecSql(Transaction, """
                                   insert into FinanceMovement
                                   (
                                     Id, TradeTypeId, MovementDate,
                                     PersonId, CashAccountId, CompanyBankAccountId,
                                     Direction, Amount, CurrencyId, ExchangeRate,
                                     SourceModule, SourceTable, SourceId,
                                     CancelledMovementId, CancellationMovementId,
                                     DocumentTypeId, DocumentCode, DocumentDate,
                                     Remarks, CreatedAt, CreatedBy
                                   )
                                   values
                                   (
                                     :Id, :TradeTypeId, :MovementDate,
                                     :PersonId, :CashAccountId, :CompanyBankAccountId,
                                     :Direction, :Amount, :CurrencyId, :ExchangeRate,
                                     :SourceModule, :SourceTable, :SourceId,
                                     :CancelledMovementId, :CancellationMovementId,
                                     :DocumentTypeId, :DocumentCode, :DocumentDate,
                                     :Remarks, :CreatedAt, :CreatedBy
                                   )
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = FinanceMovementId,
            ["TradeTypeId"] = TradeTypeId,
            ["MovementDate"] = MovementDate,
            ["PersonId"] = DbString(PersonId),
            ["CashAccountId"] = DbString(CashAccountId),
            ["CompanyBankAccountId"] = DbString(CompanyBankAccountId),
            ["Direction"] = Direction,
            ["Amount"] = Amount,
            ["CurrencyId"] = CurrentRow.AsString("CurrencyId"),
            ["ExchangeRate"] = CurrentRow.AsDecimal("ExchangeRate", 1),
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "Payment",
            ["SourceId"] = CurrentRow.AsString("Id"),
            ["CancelledMovementId"] = DbString(CancelledMovementId),
            ["CancellationMovementId"] = DBNull.Value,
            ["DocumentTypeId"] = CurrentRow.AsString("DocumentTypeId"),
            ["DocumentCode"] = CurrentRow.AsString("Code"),
            ["DocumentDate"] = CurrentRow.AsDateTime("PaymentDate", DateTime.Today),
            ["Remarks"] = Remarks,
            ["CreatedAt"] = DateTime.UtcNow,
            ["CreatedBy"] = Sys.GetCurrentAppUserId(),
        });
        UpdateFinanceBalance(Transaction, FinanceMovementId, MovementDate, TradeTypeId, CurrentRow.AsString("CurrencyId"), PersonId, CashAccountId, CompanyBankAccountId, Direction, Amount);
        return FinanceMovementId;
    }
    /// <summary>
    /// Links a source finance movement to its cancellation movement.
    /// </summary>
    void LinkCancellationFinanceMovement(DbTransaction Transaction, string SourceFinanceMovementId, string CancellationFinanceMovementId)
    {
        if (string.IsNullOrWhiteSpace(SourceFinanceMovementId))
            return;
        Store.ExecSql(Transaction, """
                                   update FinanceMovement
                                   set CancellationMovementId = :CancellationMovementId
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = SourceFinanceMovementId,
            ["CancellationMovementId"] = CancellationFinanceMovementId,
        });
    }
    /// <summary>
    /// Returns the finance movement from the cancelled payment that has the specified owner kind.
    /// </summary>
    DataRow GetCancelledPaymentMovement(DbTransaction Transaction, bool PersonMovement)
    {
        if (!IsPaymentCancellation())
            return null;
        string SourcePaymentId = CurrentRow.AsString("CancelledPaymentId");
        if (string.IsNullOrWhiteSpace(SourcePaymentId))
            return null;
        string OwnerSql = PersonMovement
            ? "and PersonId is not null"
            : "and PersonId is null";
        return Store.SelectResults(Transaction, $"""
                                                select *
                                                from FinanceMovement
                                                where SourceTable = 'Payment'
                                                  and SourceId = :SourceId
                                                  {OwnerSql}
                                                """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourcePaymentId,
        });
    }
    /// <summary>
    /// Creates finance movements for the current payment.
    /// </summary>
    void CreateFinancialMovements(DbTransaction Transaction)
    {
        if (!DocumentType.AffectsFinancial)
            return;
        int ExistingCount = Store.IntegerResult(Transaction, """
                                                             select count(*)
                                                             from FinanceMovement
                                                             where SourceTable = 'Payment'
                                                               and SourceId = :SourceId
                                                             """, 0, new Dictionary<string, object>()
        {
            ["SourceId"] = CurrentRow.AsString("Id"),
        });
        if (ExistingCount > 0)
            throw new TripousBusinessException("Finance movements already exist for this payment.");
        int CashDirection = DocumentType.FinancialDirection;
        if (CashDirection != 1 && CashDirection != -1)
            throw new TripousBusinessException("Invalid financial direction.");
        string PersonId = CurrentRow.AsString("PersonId");
        string CashAccountId = CurrentRow.AsString("CashAccountId");
        string CompanyBankAccountId = CurrentRow.AsString("CompanyBankAccountId");
        decimal Amount = RoundAmount(CurrentRow.AsDecimal("Amount"));
        DateTime MovementDate = CurrentRow.AsDateTime("PostingDate", DateTime.Today);
        DataRow SourcePartnerMovement = GetCancelledPaymentMovement(Transaction, true);
        DataRow SourceCashMovement = GetCancelledPaymentMovement(Transaction, false);
        string PartnerMovementId = InsertFinanceMovement(Transaction, CurrentRow.AsInteger("PartnerTradeTypeId"), MovementDate, PersonId, "", "", -CashDirection, Amount, SourcePartnerMovement == null ? "" : SourcePartnerMovement.AsString("Id"), $"Generated from {CurrentRow.AsString("Code")}");
        string CashMovementId = InsertFinanceMovement(Transaction, (int)TradeType.Financial, MovementDate, "", CashAccountId, CompanyBankAccountId, CashDirection, Amount, SourceCashMovement == null ? "" : SourceCashMovement.AsString("Id"), $"Generated from {CurrentRow.AsString("Code")}");
        LinkCancellationFinanceMovement(Transaction, SourcePartnerMovement == null ? "" : SourcePartnerMovement.AsString("Id"), PartnerMovementId);
        LinkCancellationFinanceMovement(Transaction, SourceCashMovement == null ? "" : SourceCashMovement.AsString("Id"), CashMovementId);
    }
    /// <summary>
    /// Inserts a journal entry line.
    /// </summary>
    void InsertJournalEntryLine(DbTransaction Transaction, string JournalEntryId, int DisplayOrder, string AccountId, decimal DebitAmount, decimal CreditAmount, string Remarks)
    {
        Store.ExecSql(Transaction, """
                                   insert into JournalEntryLine
                                   (
                                     Id, JournalEntryId, DisplayOrder, AccountId,
                                     DebitAmount, CreditAmount, CurrencyId, ExchangeRate,
                                     ReferenceNo, Remarks, SourceModule, SourceTable, SourceId
                                   )
                                   values
                                   (
                                     :Id, :JournalEntryId, :DisplayOrder, :AccountId,
                                     :DebitAmount, :CreditAmount, :CurrencyId, :ExchangeRate,
                                     :ReferenceNo, :Remarks, :SourceModule, :SourceTable, :SourceId
                                   )
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = Sys.GenId(),
            ["JournalEntryId"] = JournalEntryId,
            ["DisplayOrder"] = DisplayOrder,
            ["AccountId"] = AccountId,
            ["DebitAmount"] = DebitAmount,
            ["CreditAmount"] = CreditAmount,
            ["CurrencyId"] = CurrentRow.AsString("CurrencyId"),
            ["ExchangeRate"] = CurrentRow.AsDecimal("ExchangeRate", 1),
            ["ReferenceNo"] = CurrentRow.AsString("Code"),
            ["Remarks"] = Remarks,
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "Payment",
            ["SourceId"] = CurrentRow.AsString("Id"),
        });
    }
    /// <summary>
    /// Creates the accounting journal entry for the current payment.
    /// </summary>
    void CreateAccountingJournalEntry(DbTransaction Transaction)
    {
        if (!DocumentType.AffectsAccounting)
            return;
        int ExistingCount = Store.IntegerResult(Transaction, """
                                                             select count(*)
                                                             from JournalEntry
                                                             where SourceTable = 'Payment'
                                                               and SourceId = :SourceId
                                                             """, 0, new Dictionary<string, object>()
        {
            ["SourceId"] = CurrentRow.AsString("Id"),
        });
        if (ExistingCount > 0)
            throw new TripousBusinessException("An accounting journal entry already exists for this payment.");
        decimal Amount = RoundAmount(CurrentRow.AsDecimal("Amount"));
        if (Amount <= 0)
            throw new TripousBusinessException("Accounting journal entry amount must be greater than zero.");
        string CashBankAccountId = GetPostingAccountId(Transaction, string.IsNullOrWhiteSpace(CurrentRow.AsString("CashAccountId")) ? "10-2000" : "10-1000");
        string PartnerAccountId = GetPostingAccountId(Transaction, IsCustomerReceipt() ? "10-3000" : "20-1000");
        string JournalEntryId = Sys.GenId();
        string UserId = Sys.GetCurrentAppUserId();
        DateTime Now = DateTime.UtcNow;
        DateTime EntryDate = CurrentRow.AsDateTime("PostingDate", DateTime.Today);
        DataRow SourceJournalEntry = GetCancelledPaymentJournalEntry(Transaction);
        Store.ExecSql(Transaction, """
                                   insert into JournalEntry
                                   (
                                     Id, Code, EntryDate, StatusId, TotalDebit, TotalCredit,
                                     SourceModule, SourceTable, SourceId,
                                     DocumentTypeId, DocumentCode, DocumentDate, TradeTypeId,
                                     Remarks, CancelledDocumentId, CancellationDocumentId,
                                     IsLocked, IsCancelled,
                                     CreatedAt, CreatedBy, ModifiedAt, ModifiedBy,
                                     PostedAt, PostedBy, CancelledAt, CancelledBy
                                   )
                                   values
                                   (
                                     :Id, :Code, :EntryDate, :StatusId, :TotalDebit, :TotalCredit,
                                     :SourceModule, :SourceTable, :SourceId,
                                     :DocumentTypeId, :DocumentCode, :DocumentDate, :TradeTypeId,
                                     :Remarks, :CancelledDocumentId, :CancellationDocumentId,
                                     :IsLocked, :IsCancelled,
                                     :CreatedAt, :CreatedBy, :ModifiedAt, :ModifiedBy,
                                     :PostedAt, :PostedBy, :CancelledAt, :CancelledBy
                                   )
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = JournalEntryId,
            ["Code"] = GetGeneratedJournalEntryCode(),
            ["EntryDate"] = EntryDate,
            ["StatusId"] = (int)TradeStatus.Posted,
            ["TotalDebit"] = Amount,
            ["TotalCredit"] = Amount,
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "Payment",
            ["SourceId"] = CurrentRow.AsString("Id"),
            ["DocumentTypeId"] = CurrentRow.AsString("DocumentTypeId"),
            ["DocumentCode"] = CurrentRow.AsString("Code"),
            ["DocumentDate"] = CurrentRow.AsDateTime("PaymentDate", DateTime.Today),
            ["TradeTypeId"] = (int)TradeType.Financial,
            ["Remarks"] = $"Generated from {CurrentRow.AsString("Code")}",
            ["CancelledDocumentId"] = SourceJournalEntry == null ? DBNull.Value : (object)SourceJournalEntry.AsString("Id"),
            ["CancellationDocumentId"] = DBNull.Value,
            ["IsLocked"] = true,
            ["IsCancelled"] = false,
            ["CreatedAt"] = Now,
            ["CreatedBy"] = UserId,
            ["ModifiedAt"] = DBNull.Value,
            ["ModifiedBy"] = DBNull.Value,
            ["PostedAt"] = Now,
            ["PostedBy"] = UserId,
            ["CancelledAt"] = DBNull.Value,
            ["CancelledBy"] = DBNull.Value,
        });
        if (IsCustomerReceipt() && DocumentType.AccountingDirection == 1)
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, CashBankAccountId, Amount, 0, "Payment received");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, PartnerAccountId, 0, Amount, "Customer receivable settlement");
        }
        else if (IsCustomerReceipt())
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, PartnerAccountId, Amount, 0, "Customer receipt cancellation");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, CashBankAccountId, 0, Amount, "Cash or bank reversal");
        }
        else if (DocumentType.AccountingDirection == -1)
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, PartnerAccountId, Amount, 0, "Supplier payable settlement");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, CashBankAccountId, 0, Amount, "Payment sent");
        }
        else
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, CashBankAccountId, Amount, 0, "Cash or bank reversal");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, PartnerAccountId, 0, Amount, "Supplier payment cancellation");
        }
        LinkCancellationJournalEntry(Transaction, SourceJournalEntry, JournalEntryId);
    }
    /// <summary>
    /// Returns the journal entry generated by the cancelled payment.
    /// </summary>
    DataRow GetCancelledPaymentJournalEntry(DbTransaction Transaction)
    {
        if (!IsPaymentCancellation())
            return null;
        string SourcePaymentId = CurrentRow.AsString("CancelledPaymentId");
        if (string.IsNullOrWhiteSpace(SourcePaymentId))
            return null;
        return Store.SelectResults(Transaction, """
                                                select *
                                                from JournalEntry
                                                where SourceTable = 'Payment'
                                                  and SourceId = :SourceId
                                                """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourcePaymentId,
        });
    }
    /// <summary>
    /// Links a cancellation journal entry to the journal entry it reverses.
    /// </summary>
    void LinkCancellationJournalEntry(DbTransaction Transaction, DataRow SourceJournalEntry, string CancellationJournalEntryId)
    {
        if (SourceJournalEntry == null)
            return;
        string UserId = Sys.GetCurrentAppUserId();
        Store.ExecSql(Transaction, """
                                   update JournalEntry
                                   set StatusId = :StatusId,
                                       CancellationDocumentId = :CancellationDocumentId,
                                       IsCancelled = :IsCancelled,
                                       CancelledAt = :CancelledAt,
                                       CancelledBy = :CancelledBy
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = SourceJournalEntry.AsString("Id"),
            ["StatusId"] = (int)TradeStatus.Cancelled,
            ["CancellationDocumentId"] = CancellationJournalEntryId,
            ["IsCancelled"] = true,
            ["CancelledAt"] = DateTime.UtcNow,
            ["CancelledBy"] = UserId,
        });
    }
    /// <summary>
    /// Returns the source payment row for a cancellation document.
    /// </summary>
    DataRow GetCancelledPayment(DbTransaction Transaction)
    {
        string SourcePaymentId = CurrentRow.AsString("CancelledPaymentId");
        if (string.IsNullOrWhiteSpace(SourcePaymentId))
            throw new TripousBusinessException("The cancelled payment is required.");
        DataRow Result = Store.Provider.SelectForUpdate(Transaction, "Payment", "Id", SourcePaymentId);
        if (Result == null)
            throw new TripousBusinessException("The cancelled payment does not exist.");
        return Result;
    }
    /// <summary>
    /// Validates the source payment before posting a cancellation document.
    /// </summary>
    void ValidateCancelledPayment(DbTransaction Transaction)
    {
        if (!IsPaymentCancellation())
            return;
        DataRow SourcePayment = GetCancelledPayment(Transaction);
        if ((TradeStatus)SourcePayment.AsInteger("StatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted payments can be cancelled.");
        if (SourcePayment.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("The payment is already cancelled.");
        if (!string.IsNullOrWhiteSpace(SourcePayment.AsString("CancelledPaymentId")))
            throw new TripousBusinessException("A payment cancellation cannot be cancelled.");
        if (!string.IsNullOrWhiteSpace(SourcePayment.AsString("CancellationPaymentId")))
            throw new TripousBusinessException("The payment already has a cancellation document.");
        if (!SourcePayment.AsString("DocumentTypeId").IsSameText(DocumentType.CancellationTargetId))
            throw new TripousBusinessException("The selected payment is not valid for this cancellation document.");
    }
    /// <summary>
    /// Marks the source payment as cancelled.
    /// </summary>
    void CancelSourcePayment(DbTransaction Transaction)
    {
        if (!IsPaymentCancellation())
            return;
        DataRow SourcePayment = GetCancelledPayment(Transaction);
        Store.ExecSql(Transaction, """
                                   update Payment
                                   set StatusId = :StatusId,
                                       CancellationPaymentId = :CancellationPaymentId,
                                       IsCancelled = :IsCancelled,
                                       CancelledAt = :CancelledAt,
                                       CancelledBy = :CancelledBy
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = SourcePayment.AsString("Id"),
            ["StatusId"] = (int)TradeStatus.Cancelled,
            ["CancellationPaymentId"] = CurrentRow.AsString("Id"),
            ["IsCancelled"] = true,
            ["CancelledAt"] = DateTime.UtcNow,
            ["CancelledBy"] = Sys.GetCurrentAppUserId(),
        });
    }
    /// <summary>
    /// Returns the target finance movement for a settlement.
    /// </summary>
    DataRow GetSettlementFinanceMovement(string FinanceMovementId)
    {
        return Store.SelectResults("""
                                   select *
                                   from FinanceMovement
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = FinanceMovementId,
        });
    }
    /// <summary>
    /// Returns the amount already posted against a finance movement.
    /// </summary>
    decimal GetPostedSettlementAmount(string FinanceMovementId)
    {
        object Result = Store.SelectResult("""
                                           select sum(PaymentSettlement.Amount)
                                           from PaymentSettlement
                                             inner join Payment on Payment.Id = PaymentSettlement.PaymentId
                                           where PaymentSettlement.FinanceMovementId = :FinanceMovementId
                                             and Payment.Id <> :PaymentId
                                             and Payment.StatusId = :StatusId
                                             and Payment.IsCancelled = 0
                                           """, null, new Dictionary<string, object>()
        {
            ["FinanceMovementId"] = FinanceMovementId,
            ["PaymentId"] = CurrentRow.AsString("Id"),
            ["StatusId"] = (int)TradeStatus.Posted,
        });
        return Sys.IsNull(Result) ? 0 : RoundAmount(Convert.ToDecimal(Result));
    }
    /// <summary>
    /// Validates a settlement line.
    /// </summary>
    void ValidateSettlement(DataRow Row, List<string> Errors)
    {
        int DisplayOrder = Row.AsInteger("DisplayOrder");
        string LineLabel = DisplayOrder > 0 ? $"Line {DisplayOrder}" : "Settlement line";
        if (string.IsNullOrWhiteSpace(Row.AsString("FinanceMovementId")))
            Errors.Add($"{LineLabel}: Finance movement is required.");
        if (Row.AsDecimal("Amount") <= 0)
            Errors.Add($"{LineLabel}: Amount must be greater than zero.");
    }
    /// <summary>
    /// Validates a settlement target finance movement.
    /// </summary>
    void ValidateSettlementTarget(string FinanceMovementId, decimal Amount, List<string> Errors)
    {
        DataRow Movement = GetSettlementFinanceMovement(FinanceMovementId);
        if (Movement == null)
        {
            Errors.Add("Settlement finance movement does not exist.");
            return;
        }
        if (!Movement.AsString("SourceTable").IsSameText("Trade"))
            Errors.Add("Only trade finance movements can be settled.");
        if (!Movement.AsString("PersonId").IsSameText(CurrentRow.AsString("PersonId")))
            Errors.Add("Settlement finance movement belongs to another person.");
        if (!Movement.AsString("CurrencyId").IsSameText(CurrentRow.AsString("CurrencyId")))
            Errors.Add("Settlement finance movement has another currency.");
        if (Movement.AsInteger("TradeTypeId") != CurrentRow.AsInteger("PartnerTradeTypeId"))
            Errors.Add("Settlement finance movement has another trade type.");
        if (Movement.AsInteger("Direction") != DocumentType.FinancialDirection)
            Errors.Add("Settlement finance movement has an incompatible direction.");
        if (!string.IsNullOrWhiteSpace(Movement.AsString("CancellationMovementId")))
            Errors.Add("A cancelled finance movement cannot be settled.");
        decimal PostedAmount = GetPostedSettlementAmount(FinanceMovementId);
        decimal OpenAmount = RoundAmount(Movement.AsDecimal("Amount") - PostedAmount);
        if (Amount > OpenAmount)
            Errors.Add($"Settlement amount {Amount} exceeds open amount {OpenAmount}.");
    }
    /// <summary>
    /// Validates the payment header and settlement lines.
    /// </summary>
    void ValidatePayment(bool ValidateHeaderAmount = true)
    {
        List<string> Errors = [];
        if (string.IsNullOrWhiteSpace(CurrentRow.AsString("PersonId")))
            Errors.Add("Person is required.");
        if (string.IsNullOrWhiteSpace(CurrentRow.AsString("CurrencyId")))
            Errors.Add("Currency is required.");
        if (CurrentRow.AsDecimal("ExchangeRate") <= 0)
            Errors.Add("Exchange rate must be greater than zero.");
        if (ValidateHeaderAmount && CurrentRow.AsDecimal("Amount") <= 0)
            Errors.Add("Amount must be greater than zero.");
        bool HasCashAccount = !string.IsNullOrWhiteSpace(CurrentRow.AsString("CashAccountId"));
        bool HasBankAccount = !string.IsNullOrWhiteSpace(CurrentRow.AsString("CompanyBankAccountId"));
        if (HasCashAccount == HasBankAccount)
            Errors.Add("Exactly one cash or bank account is required.");
        if (ValidateHeaderAmount && CurrentRow.AsDecimal("UnappliedAmount") < 0)
            Errors.Add("Settled amount cannot exceed payment amount.");
        if (!IsPaymentCancellation())
        {
            foreach (DataRow Row in GetActiveSettlements())
                ValidateSettlement(Row, Errors);
            foreach (IGrouping<string, DataRow> Group in GetActiveSettlements().Where(Row => !string.IsNullOrWhiteSpace(Row.AsString("FinanceMovementId"))).GroupBy(Row => Row.AsString("FinanceMovementId")))
                ValidateSettlementTarget(Group.Key, RoundAmount(Group.Sum(Row => Row.AsDecimal("Amount"))), Errors);
        }
        else if (GetActiveSettlements().Count > 0)
        {
            Errors.Add("Payment cancellation documents cannot have settlement lines.");
        }
        if (Errors.Count > 0)
            throw new TripousBusinessException(string.Join(Environment.NewLine, Errors));
    }

    // ● protected
    /// <summary>
    /// Sets defaults for payment headers and settlement lines.
    /// </summary>
    protected override void SetDefaultValues(MemTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);
        if (Row.RowState == DataRowState.Deleted)
            return;
        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("DocumentTypeId", DocumentType.Id);
            Row.SetValue("StatusId", (int)TradeStatus.Draft);
            Row.SetValue("PartnerTradeTypeId", IsCustomerReceipt() ? (int)TradeType.Sales : (int)TradeType.Purchases);
            Row.SetValue("PaymentDate", DateTime.UtcNow.Date);
            Row.SetValue("ExchangeRate", 1);
            Row.SetValue("CurrencyId", DataLib.GetDefaultCurrencyId());
            Row.SetValue("PaymentMethodId", DataLib.GetDefaultPaymentMethodId());
            Row.SetValue("CompanyBankAccountId", DataLib.GetDefaultCompanyBankAccountId());
        }
    }
    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs ea)
    {
        base.ColumnChanged(Table, ea);
        if (!State.In(DataMode.Insert | DataMode.Edit))
            return;
        string FieldName = ea.Column.ColumnName;
        if ((Table == tblItem && FieldName.IsSameText("Amount"))
            || (Table.TableName.IsSameText("PaymentSettlement") && FieldName.IsSameText("Amount")))
            CalculateTotals();
    }
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);
        if (e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.Before)
        {
            CalculateTotals();
            ValidateCancelledPayment(e.Transaction);
        }
        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
        {
            CreateAccountingJournalEntry(e.Transaction);
            CreateFinancialMovements(e.Transaction);
            CancelSourcePayment(e.Transaction);
        }
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public PaymentDataModule()
    {
    }

    // ● public
    public override void CheckCanCommit(bool Reselect)
    {
        base.CheckCanCommit(Reselect);
        CalculateTotals();
        ValidatePayment(false);
        AdjustAmountToSettlementTotal();
        CalculateTotals();
        ValidatePayment();
    }
    /// <summary>
    /// Creates an unsaved cancellation document from the current posted payment.
    /// </summary>
    public virtual PaymentDataModule CreateCancellation()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No payment is selected.");
        if (HasChanges())
            throw new TripousBusinessException("Save or cancel the payment changes before creating a cancellation.");
        if ((TradeStatus)CurrentRow.AsInteger("StatusId") != TradeStatus.Posted)
            throw new TripousBusinessException("Only posted payments can be cancelled.");
        if (CurrentRow.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("The payment is already cancelled.");
        if (!string.IsNullOrWhiteSpace(CurrentRow.AsString("CancelledPaymentId")))
            throw new TripousBusinessException("A payment cancellation cannot be cancelled.");
        if (!string.IsNullOrWhiteSpace(CurrentRow.AsString("CancellationPaymentId")))
            throw new TripousBusinessException("The payment already has a cancellation document.");
        string ModuleName = IsCustomerReceipt() ? "CustomerReceiptCancellation" : "SupplierPaymentCancellation";
        PaymentDataModule Result = DataRegistry.CreateModule(ModuleName) as PaymentDataModule;
        if (Result == null)
            throw new TripousDataException($"Cannot create the {ModuleName} module.");
        Result.Insert();
        Result.CurrentRow.SetValue("CancelledPaymentId", CurrentRow["Id"]);
        Result.CurrentRow.SetValue("PersonId", CurrentRow["PersonId"]);
        Result.CurrentRow.SetValue("PaymentMethodId", CurrentRow["PaymentMethodId"]);
        Result.CurrentRow.SetValue("CashAccountId", CurrentRow["CashAccountId"]);
        Result.CurrentRow.SetValue("CompanyBankAccountId", CurrentRow["CompanyBankAccountId"]);
        Result.CurrentRow.SetValue("CurrencyId", CurrentRow["CurrencyId"]);
        Result.CurrentRow.SetValue("ExchangeRate", CurrentRow["ExchangeRate"]);
        Result.CurrentRow.SetValue("Amount", CurrentRow["Amount"]);
        Result.CurrentRow.SetValue("SettledAmount", 0m);
        Result.CurrentRow.SetValue("UnappliedAmount", CurrentRow["Amount"]);
        Result.CurrentRow.SetValue("ExternalRef", CurrentRow["Code"]);
        SetPersonDisplayFields(Result.CurrentRow);
        SetPaymentDisplayFields(Result.CurrentRow, "CancelledPayment", CurrentRow);
        return Result;
    }
    /// <summary>
    /// Applies a JSON contract object and creates a payment cancellation data module.
    /// </summary>
    public virtual JsonDataModule JsonCreateCancellation(JsonDataModule Source)
    {
        ApplyJsonSource(Source);
        PaymentDataModule CancellationModule = CreateCancellation();
        JsonDataModule Result = new(CancellationModule);
        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets the latest payment amount adjustment message.
    /// </summary>
    public string AmountAdjustmentMessage { get; protected set; } = "";
}
