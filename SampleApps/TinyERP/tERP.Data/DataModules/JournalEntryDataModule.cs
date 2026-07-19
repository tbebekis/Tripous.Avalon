/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Handles manual accounting journal entries.
/// </summary>
public class JournalEntryDataModule: DocumentDataModule
{
    // ● private
    /// <summary>
    /// Rounds accounting amounts to four decimal places.
    /// </summary>
    decimal RoundAmount(decimal Value) => Math.Round(Value, 4, MidpointRounding.AwayFromZero);
    /// <summary>
    /// Applies a JSON contract object to this module before calculating values.
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
    /// Returns the journal entry line table.
    /// </summary>
    MemTable GetLineTable()
    {
        MemTable Result = ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText("JournalEntryLine"));
        if (Result == null)
            throw new TripousDataException("JournalEntryLine table is not available.");
        return Result;
    }
    /// <summary>
    /// Returns active journal entry lines.
    /// </summary>
    List<DataRow> GetActiveLines()
    {
        return GetLineTable().Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .ToList();
    }
    /// <summary>
    /// Recalculates journal entry totals from active lines.
    /// </summary>
    void CalculateTotals()
    {
        if (CurrentRow == null)
            return;

        List<DataRow> Rows = GetActiveLines();
        CurrentRow.SetValue("TotalDebit", RoundAmount(Rows.Sum(Row => Row.AsDecimal("DebitAmount"))));
        CurrentRow.SetValue("TotalCredit", RoundAmount(Rows.Sum(Row => Row.AsDecimal("CreditAmount"))));
    }
    /// <summary>
    /// Returns a display label for a journal entry line.
    /// </summary>
    string GetLineLabel(DataRow Row)
    {
        int DisplayOrder = Row.AsInteger("DisplayOrder");
        return DisplayOrder > 0 ? $"Line {DisplayOrder}" : "Journal entry line";
    }
    /// <summary>
    /// Validates that the selected account can be used in a journal entry line.
    /// </summary>
    void ValidateAccount(DataRow Row, List<string> Errors)
    {
        string AccountId = Row.AsString("AccountId");
        if (string.IsNullOrWhiteSpace(AccountId))
        {
            Errors.Add($"{GetLineLabel(Row)}: Account is required.");
            return;
        }

        DataRow Account = Store.SelectResults("""
                                              select IsPosting, IsActive
                                              from Account
                                              where Id = :Id
                                              """, new Dictionary<string, object>()
        {
            ["Id"] = AccountId,
        });
        if (Account == null)
        {
            Errors.Add($"{GetLineLabel(Row)}: Account does not exist.");
            return;
        }
        if (!Account.AsBoolean("IsActive"))
            Errors.Add($"{GetLineLabel(Row)}: Account is not active.");
        if (!Account.AsBoolean("IsPosting"))
            Errors.Add($"{GetLineLabel(Row)}: Account is not a posting account.");
    }
    /// <summary>
    /// Validates a journal entry line.
    /// </summary>
    void ValidateLine(DataRow Row, List<string> Errors)
    {
        decimal DebitAmount = Row.AsDecimal("DebitAmount");
        decimal CreditAmount = Row.AsDecimal("CreditAmount");
        if (DebitAmount < 0)
            Errors.Add($"{GetLineLabel(Row)}: Debit amount cannot be negative.");
        if (CreditAmount < 0)
            Errors.Add($"{GetLineLabel(Row)}: Credit amount cannot be negative.");
        if (DebitAmount > 0 && CreditAmount > 0)
            Errors.Add($"{GetLineLabel(Row)}: Only one of debit or credit amount may be greater than zero.");
        if (DebitAmount == 0 && CreditAmount == 0)
            Errors.Add($"{GetLineLabel(Row)}: Debit or credit amount is required.");

        ValidateAccount(Row, Errors);
    }
    /// <summary>
    /// Validates journal entry header and lines.
    /// </summary>
    void ValidateJournalEntry()
    {
        List<string> Errors = [];
        List<DataRow> Rows = GetActiveLines();
        if (Rows.Count < 2)
            Errors.Add("A journal entry requires at least two lines.");

        foreach (DataRow Row in Rows)
            ValidateLine(Row, Errors);

        decimal TotalDebit = CurrentRow.AsDecimal("TotalDebit");
        decimal TotalCredit = CurrentRow.AsDecimal("TotalCredit");
        if (TotalDebit <= 0 || TotalCredit <= 0)
            Errors.Add("Journal entry totals must be greater than zero.");
        if (TotalDebit != TotalCredit)
            Errors.Add("Journal entry debit and credit totals must be equal.");

        if (Errors.Count > 0)
            throw new TripousBusinessException(string.Join(Environment.NewLine, Errors));
    }
    /// <summary>
    /// Applies server-side side effects for a web JSON calculation field change.
    /// </summary>
    void ApplyJsonCalculateFieldChange(string TableName, string FieldName)
    {
        if (string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(FieldName) || !State.In(DataMode.Insert | DataMode.Edit))
            return;
        if (TableName.IsSameText("JournalEntryLine")
            && (FieldName.IsSameText("DebitAmount") || FieldName.IsSameText("CreditAmount")))
            CalculateTotals();
    }

    // ● protected
    /// <summary>
    /// Sets defaults for journal entry headers and lines.
    /// </summary>
    protected override void SetDefaultValues(MemTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;
        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("DocumentTypeId", DocumentType.Id);
            Row.SetValue("EntryDate", DateTime.UtcNow.Date);
            Row.SetValue("StatusId", (int)TradeStatus.Draft);
            Row.SetValue("TradeTypeId", (int)TradeType.Accounting);
            Row.SetValue("IsLocked", false);
            Row.SetValue("IsCancelled", false);
        }
        else if (Table.TableName.IsSameText("JournalEntryLine"))
        {
            if (Sys.IsNull(Row["CurrencyId"]))
                Row.SetValue("CurrencyId", DataLib.GetDefaultCurrencyId());
            if (Row.AsDecimal("ExchangeRate") == 0)
                Row.SetValue("ExchangeRate", 1m);
        }
    }
    /// <summary>
    /// Recalculates totals when line amounts change.
    /// </summary>
    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs Args)
    {
        base.ColumnChanged(Table, Args);

        if (!State.In(DataMode.Insert | DataMode.Edit))
            return;
        if (Table.TableName.IsSameText("JournalEntryLine")
            && (Args.Column.ColumnName.IsSameText("DebitAmount") || Args.Column.ColumnName.IsSameText("CreditAmount")))
            CalculateTotals();
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JournalEntryDataModule()
    {
    }

    // ● public
    /// <summary>
    /// Validates and recalculates the journal entry before saving or posting.
    /// </summary>
    public override void CheckCanCommit(bool Reselect)
    {
        base.CheckCanCommit(Reselect);
        if (!IsPosting && CurrentRow != null && CurrentRow.AsBoolean("IsCancelled"))
            throw new TripousBusinessException("A cancelled journal entry cannot be saved.");

        CalculateTotals();
        ValidateJournalEntry();
    }
    /// <summary>
    /// Applies a JSON contract object, recalculates journal entry values, and returns this data module as a JSON contract object.
    /// </summary>
    public virtual JsonDataModule JsonCalculate(JsonDataModule Source, string TableName, string FieldName, string RowKey)
    {
        ApplyJsonSource(Source);
        ApplyJsonCalculateFieldChange(TableName, FieldName);
        CalculateTotals();
        return new JsonDataModule(this);
    }
}
