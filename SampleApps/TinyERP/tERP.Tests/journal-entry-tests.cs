namespace tERP.Tests;

[Collection(TestCollection.Name)]
public class JournalEntryTests
{
    // ● private fields
    readonly TestDatabaseFixture fFixture;

    // ● private
    string GetAccountId(string Code)
    {
        object Result = fFixture.Store.SelectResult("select Id from Account where Code = :Code", null, new Dictionary<string, object>()
        {
            ["Code"] = Code,
        });
        if (Sys.IsNull(Result))
            throw new TripousDataException($"Test account not found: {Code}");
        return Result.ToString();
    }
    DataRow GetJournalEntry(string Id)
    {
        DataRow Result = fFixture.Store.SelectResults("select * from JournalEntry where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
        if (Result == null)
            throw new TripousDataException($"Journal Entry not found: {Id}");
        return Result;
    }
    DataTable GetJournalEntryLinesBySource(string SourceId)
    {
        return fFixture.Store.Select("""
                                     select
                                       Account.Code as AccountCode,
                                       JournalEntryLine.DebitAmount,
                                       JournalEntryLine.CreditAmount
                                     from JournalEntry
                                       inner join JournalEntryLine on JournalEntryLine.JournalEntryId = JournalEntry.Id
                                       inner join Account on Account.Id = JournalEntryLine.AccountId
                                     where JournalEntry.SourceTable = 'Trade'
                                       and JournalEntry.SourceId = :SourceId
                                     order by JournalEntryLine.DisplayOrder
                                     """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    DataTable GetPaymentJournalEntryLinesBySource(string SourceId)
    {
        return fFixture.Store.Select("""
                                     select
                                       Account.Code as AccountCode,
                                       JournalEntryLine.DebitAmount,
                                       JournalEntryLine.CreditAmount
                                     from JournalEntry
                                       inner join JournalEntryLine on JournalEntryLine.JournalEntryId = JournalEntry.Id
                                       inner join Account on Account.Id = JournalEntryLine.AccountId
                                     where JournalEntry.SourceTable = 'Payment'
                                       and JournalEntry.SourceId = :SourceId
                                     order by JournalEntryLine.DisplayOrder
                                     """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    DataRow GetPaymentJournalEntryBySource(string SourceId)
    {
        DataRow Result = fFixture.Store.SelectResults("""
                                                      select *
                                                      from JournalEntry
                                                      where SourceTable = 'Payment'
                                                        and SourceId = :SourceId
                                                      """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
        if (Result == null)
            throw new TripousDataException($"Payment Journal Entry not found for source: {SourceId}");
        return Result;
    }
    DataRow GetJournalEntryBySource(string SourceId)
    {
        DataRow Result = fFixture.Store.SelectResults("""
                                                      select *
                                                      from JournalEntry
                                                      where SourceTable = 'Trade'
                                                        and SourceId = :SourceId
                                                      """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
        if (Result == null)
            throw new TripousDataException($"Journal Entry not found for source: {SourceId}");
        return Result;
    }
    DataRow GetFinanceMovementBySource(string SourceId)
    {
        DataRow Result = fFixture.Store.SelectResults("""
                                                      select *
                                                      from FinanceMovement
                                                      where SourceTable = 'Trade'
                                                        and SourceId = :SourceId
                                                      """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
        if (Result == null)
            throw new TripousDataException($"Finance movement not found for source: {SourceId}");
        return Result;
    }
    DataTable GetPaymentFinanceMovementsBySource(string SourceId)
    {
        return fFixture.Store.Select("""
                                     select *
                                     from FinanceMovement
                                     where SourceTable = 'Payment'
                                       and SourceId = :SourceId
                                     order by
                                       case when PersonId is not null then 0 else 1 end,
                                       TradeTypeId
                                     """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    DataRow GetPayment(string Id)
    {
        DataRow Result = fFixture.Store.SelectResults("select * from Payment where Id = :Id", new Dictionary<string, object>()
        {
            ["Id"] = Id,
        });
        if (Result == null)
            throw new TripousDataException($"Payment not found: {Id}");
        return Result;
    }
    DataRow GetFinanceBalance(string PersonCode, TradeType TradeType)
    {
        DataRow Result = fFixture.Store.SelectResults("""
                                                      select FinanceBalance.*
                                                      from FinanceBalance
                                                        inner join Person on Person.Id = FinanceBalance.PersonId
                                                      where Person.Code = :PersonCode
                                                        and FinanceBalance.TradeTypeId = :TradeTypeId
                                                        and FinanceBalance.CurrencyId = :CurrencyId
                                                      """, new Dictionary<string, object>()
        {
            ["PersonCode"] = PersonCode,
            ["TradeTypeId"] = (int)TradeType,
            ["CurrencyId"] = DataLib.GetDefaultCurrencyId(),
        });
        if (Result == null)
            throw new TripousDataException($"Finance balance not found for person: {PersonCode}");
        return Result;
    }
    decimal GetFinanceBalanceAmount(string PersonCode, TradeType TradeType)
    {
        object Result = fFixture.Store.SelectResult("""
                                                    select FinanceBalance.Balance
                                                    from FinanceBalance
                                                      inner join Person on Person.Id = FinanceBalance.PersonId
                                                    where Person.Code = :PersonCode
                                                      and FinanceBalance.TradeTypeId = :TradeTypeId
                                                      and FinanceBalance.CurrencyId = :CurrencyId
                                                    """, null, new Dictionary<string, object>()
        {
            ["PersonCode"] = PersonCode,
            ["TradeTypeId"] = (int)TradeType,
            ["CurrencyId"] = DataLib.GetDefaultCurrencyId(),
        });
        return Sys.IsNull(Result) ? 0 : Convert.ToDecimal(Result);
    }
    decimal GetBankFinanceBalanceAmount()
    {
        object Result = fFixture.Store.SelectResult("""
                                                    select Balance
                                                    from FinanceBalance
                                                    where TradeTypeId = :TradeTypeId
                                                      and CurrencyId = :CurrencyId
                                                      and PersonId is null
                                                      and CashAccountId is null
                                                      and CompanyBankAccountId = :CompanyBankAccountId
                                                    """, null, new Dictionary<string, object>()
        {
            ["TradeTypeId"] = (int)TradeType.Financial,
            ["CurrencyId"] = DataLib.GetDefaultCurrencyId(),
            ["CompanyBankAccountId"] = DataLib.GetDefaultCompanyBankAccountId(),
        });
        return Sys.IsNull(Result) ? 0 : Convert.ToDecimal(Result);
    }
    DataRow GetProduct(string Name)
    {
        DataRow Result = fFixture.Store.SelectResults("""
                                                      select
                                                        Product.Id,
                                                        Product.Code,
                                                        Product.Name,
                                                        Product.TaxProductGroupId,
                                                        Product.PrimaryUnitOfMeasureId as UnitOfMeasureId,
                                                        UnitOfMeasure.Name as UnitOfMeasureName
                                                      from Product
                                                        inner join UnitOfMeasure on UnitOfMeasure.Id = Product.PrimaryUnitOfMeasureId
                                                      where Product.Name = :Name
                                                      """, new Dictionary<string, object>()
        {
            ["Name"] = Name,
        });
        if (Result == null)
            throw new TripousDataException($"Product not found: {Name}");
        return Result;
    }
    string GetPersonId(string Code)
    {
        object Result = fFixture.Store.SelectResult("select Id from Person where Code = :Code", null, new Dictionary<string, object>()
        {
            ["Code"] = Code,
        });
        if (Sys.IsNull(Result))
            throw new TripousDataException($"Person not found: {Code}");
        return Result.ToString();
    }
    string GetTaxJurisdictionId()
    {
        object Result = fFixture.Store.SelectResult("select Id from TaxJurisdiction where Code = 'GR'", null);
        if (Sys.IsNull(Result))
            throw new TripousDataException("Test tax jurisdiction not found.");
        return Result.ToString();
    }
    void ConfigureTradeDocument(TradeDataModule Module, string PersonCode)
    {
        Module.CurrentRow.SetValue("PersonId", GetPersonId(PersonCode));
        Module.CurrentRow.SetValue("WarehouseId", DataLib.GetDefaultWarehouseId());
        Module.CurrentRow.SetValue("CurrencyId", DataLib.GetDefaultCurrencyId());
        Module.CurrentRow.SetValue("ExchangeRate", 1m);
        Module.CurrentRow.SetValue("TaxBusinessGroupId", DataLib.GetDefaultTaxBusinessGroupId());
        Module.CurrentRow.SetValue("OriginTaxJurisdictionId", GetTaxJurisdictionId());
        Module.CurrentRow.SetValue("DestinationTaxJurisdictionId", GetTaxJurisdictionId());
    }
    DataRow AddLine(JournalEntryDataModule Module, string AccountCode, decimal DebitAmount, decimal CreditAmount)
    {
        DataRow Result = Module.GetTable("JournalEntryLine").AddNewRow();
        Result.SetValue("AccountId", GetAccountId(AccountCode));
        Result.SetValue("DebitAmount", DebitAmount);
        Result.SetValue("CreditAmount", CreditAmount);
        return Result;
    }
    JournalEntryDataModule CreateJournalEntry()
    {
        JournalEntryDataModule Result = CreateJournalEntryModule();
        Result.Insert();
        return Result;
    }
    DataRow AddTradeLine(TradeDataModule Module, string ProductName, decimal Quantity, decimal UnitPrice)
    {
        DataRow Product = GetProduct(ProductName);
        DataRow Result = Module.GetTable("TradeLine").AddNewRow();
        Result.SetValue("ProductId", Product["Id"]);
        Result.SetValue("ProductCode", Product["Code"]);
        Result.SetValue("ProductName", Product["Name"]);
        Result.SetValue("TaxProductGroupId", Product["TaxProductGroupId"]);
        Result.SetValue("WarehouseId", DataLib.GetDefaultWarehouseId());
        Result.SetValue("UnitOfMeasureId", Product["UnitOfMeasureId"]);
        Result.SetValue("UnitOfMeasureName", Product["UnitOfMeasureName"]);
        Result.SetValue("UnitRatio", 1m);
        Result.SetValue("Quantity", Quantity);
        Result.SetValue("UnitPrice", UnitPrice);
        return Result;
    }
    JournalEntryDataModule CreateJournalEntryModule()
    {
        JournalEntryDataModule Result = DataRegistry.CreateModule("JournalEntry") as JournalEntryDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create the Journal Entry module.");
        return Result;
    }
    SalesInvoiceDataModule CreateSalesInvoice()
    {
        SalesInvoiceDataModule Result = DataRegistry.CreateModule("SalesInvoice") as SalesInvoiceDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create the Sales Invoice module.");
        Result.Insert();
        ConfigureTradeDocument(Result, "CUST-ACME");
        AddTradeLine(Result, "Laptop Computer 14", 1m, 100m);
        Result.Commit();
        return Result;
    }
    PurchaseInvoiceDataModule CreatePurchaseInvoice()
    {
        PurchaseInvoiceDataModule Result = DataRegistry.CreateModule("PurchaseInvoice") as PurchaseInvoiceDataModule;
        if (Result == null)
            throw new TripousDataException("Cannot create the Purchase Invoice module.");
        Result.Insert();
        ConfigureTradeDocument(Result, "SUP-HELIOS");
        AddTradeLine(Result, "Laptop Computer 14", 1m, 100m);
        Result.Commit();
        return Result;
    }
    PaymentDataModule CreatePayment(string ModuleName, string PersonCode, string FinanceMovementId, decimal Amount)
    {
        PaymentDataModule Result = DataRegistry.CreateModule(ModuleName) as PaymentDataModule;
        if (Result == null)
            throw new TripousDataException($"Cannot create the {ModuleName} module.");
        Result.Insert();
        Result.CurrentRow.SetValue("PersonId", GetPersonId(PersonCode));
        Result.CurrentRow.SetValue("CurrencyId", DataLib.GetDefaultCurrencyId());
        Result.CurrentRow.SetValue("ExchangeRate", 1m);
        Result.CurrentRow.SetValue("PaymentMethodId", DataLib.GetDefaultPaymentMethodId());
        Result.CurrentRow.SetValue("CompanyBankAccountId", DataLib.GetDefaultCompanyBankAccountId());
        Result.CurrentRow.SetValue("CashAccountId", DBNull.Value);
        Result.CurrentRow.SetValue("Amount", Amount);
        DataRow Settlement = Result.GetTable("PaymentSettlement").AddNewRow();
        Settlement.SetValue("FinanceMovementId", FinanceMovementId);
        Settlement.SetValue("Amount", Amount);
        Result.Commit();
        return Result;
    }

    // ● construction
    public JournalEntryTests(TestDatabaseFixture Fixture)
    {
        fFixture = Fixture;
    }

    // ● public
    [Fact]
    public void BalancedJournalEntrySavesWithCalculatedTotals()
    {
        JournalEntryDataModule Module = CreateJournalEntry();
        AddLine(Module, "10-3000", 124m, 0m);
        AddLine(Module, "70-1000", 0m, 100m);
        AddLine(Module, "20-2000", 0m, 24m);

        Module.Commit();
        DataRow Row = GetJournalEntry(Module.CurrentRow.AsString("Id"));

        Assert.Equal(124m, Row.AsDecimal("TotalDebit"));
        Assert.Equal(124m, Row.AsDecimal("TotalCredit"));
        Assert.Equal((int)TradeStatus.Draft, Row.AsInteger("StatusId"));
        Assert.Equal((int)TradeType.Accounting, Row.AsInteger("TradeTypeId"));
        Assert.False(Row.AsBoolean("IsLocked"));
        Assert.False(Row.AsBoolean("IsCancelled"));
    }
    [Fact]
    public void UnbalancedJournalEntryIsRejected()
    {
        JournalEntryDataModule Module = CreateJournalEntry();
        AddLine(Module, "10-3000", 124m, 0m);
        AddLine(Module, "70-1000", 0m, 100m);

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => Module.Commit());

        Assert.Contains("debit and credit totals must be equal", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void PostingJournalEntrySetsPostedStatusAndFinalCode()
    {
        JournalEntryDataModule Module = CreateJournalEntry();
        AddLine(Module, "10-3000", 124m, 0m);
        AddLine(Module, "70-1000", 0m, 100m);
        AddLine(Module, "20-2000", 0m, 24m);

        Module.Commit();
        string JournalEntryId = Module.CurrentRow.AsString("Id");
        Module.Post();
        DataRow Row = GetJournalEntry(JournalEntryId);

        Assert.Equal((int)TradeStatus.Posted, Row.AsInteger("StatusId"));
        Assert.True(Row.AsBoolean("IsLocked"));
        Assert.False(Sys.IsNull(Row["PostedAt"]));
        Assert.Equal(Sys.Context.CurrentUser.Id, Row.AsString("PostedBy"));
        Assert.False(Row.AsString("Code").StartsWith("DRAFT-", StringComparison.OrdinalIgnoreCase));
    }
    [Fact]
    public void PostedJournalEntryCannotBeSaved()
    {
        JournalEntryDataModule Module = CreateJournalEntry();
        AddLine(Module, "10-3000", 124m, 0m);
        AddLine(Module, "70-1000", 0m, 100m);
        AddLine(Module, "20-2000", 0m, 24m);
        Module.Commit();
        string JournalEntryId = Module.CurrentRow.AsString("Id");
        Module.Post();

        JournalEntryDataModule StaleModule = CreateJournalEntryModule();
        StaleModule.Edit(JournalEntryId);
        StaleModule.CurrentRow.SetValue("Remarks", "Changed after posting");
        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => StaleModule.Commit());

        Assert.Contains("locked document cannot be saved", Error.Message.ToLowerInvariant());
    }
    [Fact]
    public void PostingSalesInvoiceCreatesJournalEntry()
    {
        SalesInvoiceDataModule Module = CreateSalesInvoice();
        string TradeId = Module.CurrentRow.AsString("Id");

        Module.Post();
        DataTable Lines = GetJournalEntryLinesBySource(TradeId);

        Assert.Equal(3, Lines.Rows.Count);
        Assert.Equal("10-3000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("70-1000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(100m, Lines.Rows[1].AsDecimal("CreditAmount"));
        Assert.Equal("20-2000", Lines.Rows[2].AsString("AccountCode"));
        Assert.Equal(24m, Lines.Rows[2].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void PostingSalesInvoiceCreatesFinanceMovementAndBalance()
    {
        decimal PreviousBalance = GetFinanceBalanceAmount("CUST-ACME", TradeType.Sales);
        SalesInvoiceDataModule Module = CreateSalesInvoice();
        string TradeId = Module.CurrentRow.AsString("Id");

        Module.Post();
        DataRow Movement = GetFinanceMovementBySource(TradeId);
        DataRow Balance = GetFinanceBalance("CUST-ACME", TradeType.Sales);

        Assert.Equal((int)TradeType.Sales, Movement.AsInteger("TradeTypeId"));
        Assert.Equal(1, Movement.AsInteger("Direction"));
        Assert.Equal(124m, Movement.AsDecimal("Amount"));
        Assert.Equal(GetPersonId("CUST-ACME"), Movement.AsString("PersonId"));
        Assert.Equal(PreviousBalance + 124m, Balance.AsDecimal("Balance"));
        Assert.Equal(Movement.AsString("Id"), Balance.AsString("LastMovementId"));
    }
    [Fact]
    public void PostingPurchaseInvoiceCreatesJournalEntry()
    {
        PurchaseInvoiceDataModule Module = CreatePurchaseInvoice();
        string TradeId = Module.CurrentRow.AsString("Id");

        Module.Post();
        DataTable Lines = GetJournalEntryLinesBySource(TradeId);

        Assert.Equal(3, Lines.Rows.Count);
        Assert.Equal("60-1000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(100m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("10-5000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(24m, Lines.Rows[1].AsDecimal("DebitAmount"));
        Assert.Equal("20-1000", Lines.Rows[2].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[2].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void PostingPurchaseInvoiceCreatesFinanceMovementAndBalance()
    {
        decimal PreviousBalance = GetFinanceBalanceAmount("SUP-HELIOS", TradeType.Purchases);
        PurchaseInvoiceDataModule Module = CreatePurchaseInvoice();
        string TradeId = Module.CurrentRow.AsString("Id");

        Module.Post();
        DataRow Movement = GetFinanceMovementBySource(TradeId);
        DataRow Balance = GetFinanceBalance("SUP-HELIOS", TradeType.Purchases);

        Assert.Equal((int)TradeType.Purchases, Movement.AsInteger("TradeTypeId"));
        Assert.Equal(-1, Movement.AsInteger("Direction"));
        Assert.Equal(124m, Movement.AsDecimal("Amount"));
        Assert.Equal(GetPersonId("SUP-HELIOS"), Movement.AsString("PersonId"));
        Assert.Equal(PreviousBalance - 124m, Balance.AsDecimal("Balance"));
        Assert.Equal(Movement.AsString("Id"), Balance.AsString("LastMovementId"));
    }
    [Fact]
    public void PostingSalesCreditNoteCreatesReversalJournalEntry()
    {
        decimal PreviousBalance = GetFinanceBalanceAmount("CUST-ACME", TradeType.Sales);
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        SalesCreditNoteDataModule CreditNoteModule = InvoiceModule.CreateCreditNote();
        CreditNoteModule.Commit();
        string TradeId = CreditNoteModule.CurrentRow.AsString("Id");

        CreditNoteModule.Post();
        DataTable Lines = GetJournalEntryLinesBySource(TradeId);
        DataRow Movement = GetFinanceMovementBySource(TradeId);
        DataRow Balance = GetFinanceBalance("CUST-ACME", TradeType.Sales);

        Assert.Equal(3, Lines.Rows.Count);
        Assert.Equal(-1, Movement.AsInteger("Direction"));
        Assert.Equal(124m, Movement.AsDecimal("Amount"));
        Assert.Equal(PreviousBalance, Balance.AsDecimal("Balance"));
        Assert.Equal("70-1000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(100m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("20-2000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(24m, Lines.Rows[1].AsDecimal("DebitAmount"));
        Assert.Equal("10-3000", Lines.Rows[2].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[2].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void PostingSalesCancellationCreatesReversalJournalEntry()
    {
        decimal PreviousBalance = GetFinanceBalanceAmount("CUST-ACME", TradeType.Sales);
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        string InvoiceId = InvoiceModule.CurrentRow.AsString("Id");
        InvoiceModule.Post();
        DataRow InvoiceJournalEntry = GetJournalEntryBySource(InvoiceId);
        DataRow InvoiceFinanceMovement = GetFinanceMovementBySource(InvoiceId);
        SalesCancellationDataModule CancellationModule = InvoiceModule.CreateCancellation();
        CancellationModule.Commit();
        string TradeId = CancellationModule.CurrentRow.AsString("Id");

        CancellationModule.Post();
        DataRow CancellationJournalEntry = GetJournalEntryBySource(TradeId);
        DataRow CancellationFinanceMovement = GetFinanceMovementBySource(TradeId);
        DataRow Balance = GetFinanceBalance("CUST-ACME", TradeType.Sales);
        DataTable Lines = GetJournalEntryLinesBySource(TradeId);

        Assert.Equal(3, Lines.Rows.Count);
        Assert.Equal((int)TradeStatus.Cancelled, GetJournalEntry(InvoiceJournalEntry.AsString("Id")).AsInteger("StatusId"));
        Assert.True(GetJournalEntry(InvoiceJournalEntry.AsString("Id")).AsBoolean("IsCancelled"));
        Assert.Equal(CancellationJournalEntry.AsString("Id"), GetJournalEntry(InvoiceJournalEntry.AsString("Id")).AsString("CancellationDocumentId"));
        Assert.Equal(InvoiceJournalEntry.AsString("Id"), CancellationJournalEntry.AsString("CancelledDocumentId"));
        Assert.Equal(InvoiceFinanceMovement.AsString("Id"), CancellationFinanceMovement.AsString("CancelledMovementId"));
        Assert.Equal(CancellationFinanceMovement.AsString("Id"), GetFinanceMovementBySource(InvoiceId).AsString("CancellationMovementId"));
        Assert.Equal(PreviousBalance, Balance.AsDecimal("Balance"));
        Assert.Equal("70-1000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(100m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("20-2000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(24m, Lines.Rows[1].AsDecimal("DebitAmount"));
        Assert.Equal("10-3000", Lines.Rows[2].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[2].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void PostingPurchaseCreditNoteCreatesReversalJournalEntry()
    {
        decimal PreviousBalance = GetFinanceBalanceAmount("SUP-HELIOS", TradeType.Purchases);
        PurchaseInvoiceDataModule InvoiceModule = CreatePurchaseInvoice();
        InvoiceModule.Post();
        PurchaseCreditNoteDataModule CreditNoteModule = InvoiceModule.CreateCreditNote();
        CreditNoteModule.Commit();
        string TradeId = CreditNoteModule.CurrentRow.AsString("Id");

        CreditNoteModule.Post();
        DataTable Lines = GetJournalEntryLinesBySource(TradeId);
        DataRow Movement = GetFinanceMovementBySource(TradeId);
        DataRow Balance = GetFinanceBalance("SUP-HELIOS", TradeType.Purchases);

        Assert.Equal(3, Lines.Rows.Count);
        Assert.Equal(1, Movement.AsInteger("Direction"));
        Assert.Equal(124m, Movement.AsDecimal("Amount"));
        Assert.Equal(PreviousBalance, Balance.AsDecimal("Balance"));
        Assert.Equal("20-1000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("60-1000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(100m, Lines.Rows[1].AsDecimal("CreditAmount"));
        Assert.Equal("10-5000", Lines.Rows[2].AsString("AccountCode"));
        Assert.Equal(24m, Lines.Rows[2].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void PostingPurchaseCancellationCreatesReversalJournalEntry()
    {
        decimal PreviousBalance = GetFinanceBalanceAmount("SUP-HELIOS", TradeType.Purchases);
        PurchaseInvoiceDataModule InvoiceModule = CreatePurchaseInvoice();
        string InvoiceId = InvoiceModule.CurrentRow.AsString("Id");
        InvoiceModule.Post();
        DataRow InvoiceJournalEntry = GetJournalEntryBySource(InvoiceId);
        DataRow InvoiceFinanceMovement = GetFinanceMovementBySource(InvoiceId);
        PurchaseCancellationDataModule CancellationModule = InvoiceModule.CreateCancellation();
        CancellationModule.Commit();
        string TradeId = CancellationModule.CurrentRow.AsString("Id");

        CancellationModule.Post();
        DataRow CancellationJournalEntry = GetJournalEntryBySource(TradeId);
        DataRow CancellationFinanceMovement = GetFinanceMovementBySource(TradeId);
        DataRow Balance = GetFinanceBalance("SUP-HELIOS", TradeType.Purchases);
        DataTable Lines = GetJournalEntryLinesBySource(TradeId);

        Assert.Equal(3, Lines.Rows.Count);
        Assert.Equal((int)TradeStatus.Cancelled, GetJournalEntry(InvoiceJournalEntry.AsString("Id")).AsInteger("StatusId"));
        Assert.True(GetJournalEntry(InvoiceJournalEntry.AsString("Id")).AsBoolean("IsCancelled"));
        Assert.False(Sys.IsNull(GetJournalEntry(InvoiceJournalEntry.AsString("Id"))["CancelledAt"]));
        Assert.Equal(Sys.Context.CurrentUser.Id, GetJournalEntry(InvoiceJournalEntry.AsString("Id")).AsString("CancelledBy"));
        Assert.Equal(CancellationJournalEntry.AsString("Id"), GetJournalEntry(InvoiceJournalEntry.AsString("Id")).AsString("CancellationDocumentId"));
        Assert.Equal(InvoiceJournalEntry.AsString("Id"), CancellationJournalEntry.AsString("CancelledDocumentId"));
        Assert.True(CancellationJournalEntry.AsBoolean("IsLocked"));
        Assert.False(CancellationJournalEntry.AsBoolean("IsCancelled"));
        Assert.Equal(InvoiceFinanceMovement.AsString("Id"), CancellationFinanceMovement.AsString("CancelledMovementId"));
        Assert.Equal(CancellationFinanceMovement.AsString("Id"), GetFinanceMovementBySource(InvoiceId).AsString("CancellationMovementId"));
        Assert.Equal(PreviousBalance, Balance.AsDecimal("Balance"));
        Assert.Equal("20-1000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("60-1000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(100m, Lines.Rows[1].AsDecimal("CreditAmount"));
        Assert.Equal("10-5000", Lines.Rows[2].AsString("AccountCode"));
        Assert.Equal(24m, Lines.Rows[2].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void PostingCustomerReceiptCreatesFinanceMovementsAndJournalEntry()
    {
        decimal PreviousCustomerBalance = GetFinanceBalanceAmount("CUST-ACME", TradeType.Sales);
        decimal PreviousBankBalance = GetBankFinanceBalanceAmount();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = CreatePayment("CustomerReceipt", "CUST-ACME", InvoiceMovement.AsString("Id"), 124m);
        string PaymentId = PaymentModule.CurrentRow.AsString("Id");

        PaymentModule.Post();
        DataTable Movements = GetPaymentFinanceMovementsBySource(PaymentId);
        DataTable Lines = GetPaymentJournalEntryLinesBySource(PaymentId);

        Assert.Equal(2, Movements.Rows.Count);
        Assert.Equal((int)TradeType.Sales, Movements.Rows[0].AsInteger("TradeTypeId"));
        Assert.Equal(-1, Movements.Rows[0].AsInteger("Direction"));
        Assert.Equal(GetPersonId("CUST-ACME"), Movements.Rows[0].AsString("PersonId"));
        Assert.Equal((int)TradeType.Financial, Movements.Rows[1].AsInteger("TradeTypeId"));
        Assert.Equal(1, Movements.Rows[1].AsInteger("Direction"));
        Assert.Equal(DataLib.GetDefaultCompanyBankAccountId(), Movements.Rows[1].AsString("CompanyBankAccountId"));
        Assert.Equal(PreviousCustomerBalance, GetFinanceBalanceAmount("CUST-ACME", TradeType.Sales));
        Assert.Equal(PreviousBankBalance + 124m, GetBankFinanceBalanceAmount());
        Assert.Equal(2, Lines.Rows.Count);
        Assert.Equal("10-2000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("10-3000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[1].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void PostingSupplierPaymentCreatesFinanceMovementsAndJournalEntry()
    {
        decimal PreviousSupplierBalance = GetFinanceBalanceAmount("SUP-HELIOS", TradeType.Purchases);
        decimal PreviousBankBalance = GetBankFinanceBalanceAmount();
        PurchaseInvoiceDataModule InvoiceModule = CreatePurchaseInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = CreatePayment("SupplierPayment", "SUP-HELIOS", InvoiceMovement.AsString("Id"), 124m);
        string PaymentId = PaymentModule.CurrentRow.AsString("Id");

        PaymentModule.Post();
        DataTable Movements = GetPaymentFinanceMovementsBySource(PaymentId);
        DataTable Lines = GetPaymentJournalEntryLinesBySource(PaymentId);

        Assert.Equal(2, Movements.Rows.Count);
        Assert.Equal((int)TradeType.Purchases, Movements.Rows[0].AsInteger("TradeTypeId"));
        Assert.Equal(1, Movements.Rows[0].AsInteger("Direction"));
        Assert.Equal(GetPersonId("SUP-HELIOS"), Movements.Rows[0].AsString("PersonId"));
        Assert.Equal((int)TradeType.Financial, Movements.Rows[1].AsInteger("TradeTypeId"));
        Assert.Equal(-1, Movements.Rows[1].AsInteger("Direction"));
        Assert.Equal(DataLib.GetDefaultCompanyBankAccountId(), Movements.Rows[1].AsString("CompanyBankAccountId"));
        Assert.Equal(PreviousSupplierBalance, GetFinanceBalanceAmount("SUP-HELIOS", TradeType.Purchases));
        Assert.Equal(PreviousBankBalance - 124m, GetBankFinanceBalanceAmount());
        Assert.Equal(2, Lines.Rows.Count);
        Assert.Equal("20-1000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("10-2000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[1].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void CustomerReceiptRejectsSettlementForAnotherPerson()
    {
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => CreatePayment("CustomerReceipt", "SUP-HELIOS", InvoiceMovement.AsString("Id"), 124m));

        Assert.Contains("belongs to another person", Error.Message);
    }
    [Fact]
    public void CustomerReceiptRejectsOverSettlement()
    {
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = CreatePayment("CustomerReceipt", "CUST-ACME", InvoiceMovement.AsString("Id"), 124m);
        PaymentModule.Post();

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => CreatePayment("CustomerReceipt", "CUST-ACME", InvoiceMovement.AsString("Id"), 1m));

        Assert.Contains("exceeds open amount", Error.Message);
    }
    [Fact]
    public void CustomerReceiptAdjustsHeaderAmountToSettlementTotal()
    {
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = DataRegistry.CreateModule("CustomerReceipt") as PaymentDataModule;
        if (PaymentModule == null)
            throw new TripousDataException("Cannot create the Customer Receipt module.");
        PaymentModule.Insert();
        PaymentModule.CurrentRow.SetValue("PersonId", GetPersonId("CUST-ACME"));
        PaymentModule.CurrentRow.SetValue("CurrencyId", DataLib.GetDefaultCurrencyId());
        PaymentModule.CurrentRow.SetValue("ExchangeRate", 1m);
        PaymentModule.CurrentRow.SetValue("PaymentMethodId", DataLib.GetDefaultPaymentMethodId());
        PaymentModule.CurrentRow.SetValue("CompanyBankAccountId", DataLib.GetDefaultCompanyBankAccountId());
        PaymentModule.CurrentRow.SetValue("CashAccountId", DBNull.Value);
        PaymentModule.CurrentRow.SetValue("Amount", 200m);
        DataRow Settlement = PaymentModule.GetTable("PaymentSettlement").AddNewRow();
        Settlement.SetValue("FinanceMovementId", InvoiceMovement.AsString("Id"));
        Settlement.SetValue("Amount", 124m);

        PaymentModule.Commit();

        Assert.Equal(124m, PaymentModule.CurrentRow.AsDecimal("Amount"));
        Assert.Equal(124m, PaymentModule.CurrentRow.AsDecimal("SettledAmount"));
        Assert.Equal(0m, PaymentModule.CurrentRow.AsDecimal("UnappliedAmount"));
        Assert.Contains("adjusted from 200", PaymentModule.AmountAdjustmentMessage);
    }
    [Fact]
    public void CustomerReceiptRejectsOverSettlementBeforeHeaderAdjustment()
    {
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = DataRegistry.CreateModule("CustomerReceipt") as PaymentDataModule;
        if (PaymentModule == null)
            throw new TripousDataException("Cannot create the Customer Receipt module.");
        PaymentModule.Insert();
        PaymentModule.CurrentRow.SetValue("PersonId", GetPersonId("CUST-ACME"));
        PaymentModule.CurrentRow.SetValue("CurrencyId", DataLib.GetDefaultCurrencyId());
        PaymentModule.CurrentRow.SetValue("ExchangeRate", 1m);
        PaymentModule.CurrentRow.SetValue("PaymentMethodId", DataLib.GetDefaultPaymentMethodId());
        PaymentModule.CurrentRow.SetValue("CompanyBankAccountId", DataLib.GetDefaultCompanyBankAccountId());
        PaymentModule.CurrentRow.SetValue("CashAccountId", DBNull.Value);
        PaymentModule.CurrentRow.SetValue("Amount", 200m);
        DataRow Settlement = PaymentModule.GetTable("PaymentSettlement").AddNewRow();
        Settlement.SetValue("FinanceMovementId", InvoiceMovement.AsString("Id"));
        Settlement.SetValue("Amount", 125m);

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => PaymentModule.Commit());

        Assert.Contains("exceeds open amount", Error.Message);
        Assert.Equal(200m, PaymentModule.CurrentRow.AsDecimal("Amount"));
    }
    [Fact]
    public void PostingCustomerReceiptCancellationReversesPayment()
    {
        decimal PreviousCustomerBalance = GetFinanceBalanceAmount("CUST-ACME", TradeType.Sales);
        decimal PreviousBankBalance = GetBankFinanceBalanceAmount();
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = CreatePayment("CustomerReceipt", "CUST-ACME", InvoiceMovement.AsString("Id"), 124m);
        PaymentModule.Post();
        string PaymentId = PaymentModule.CurrentRow.AsString("Id");
        DataRow PaymentJournalEntry = GetPaymentJournalEntryBySource(PaymentId);
        DataTable PaymentMovements = GetPaymentFinanceMovementsBySource(PaymentId);
        PaymentDataModule CancellationModule = PaymentModule.CreateCancellation();
        CancellationModule.Commit();
        string CancellationId = CancellationModule.CurrentRow.AsString("Id");

        CancellationModule.Post();
        DataTable CancellationMovements = GetPaymentFinanceMovementsBySource(CancellationId);
        DataTable Lines = GetPaymentJournalEntryLinesBySource(CancellationId);
        DataRow CancellationJournalEntry = GetPaymentJournalEntryBySource(CancellationId);

        Assert.Equal((int)TradeStatus.Cancelled, GetPayment(PaymentId).AsInteger("StatusId"));
        Assert.True(GetPayment(PaymentId).AsBoolean("IsCancelled"));
        Assert.Equal(CancellationId, GetPayment(PaymentId).AsString("CancellationPaymentId"));
        Assert.Equal(PaymentId, GetPayment(CancellationId).AsString("CancelledPaymentId"));
        Assert.Equal(PreviousCustomerBalance + 124m, GetFinanceBalanceAmount("CUST-ACME", TradeType.Sales));
        Assert.Equal(PreviousBankBalance, GetBankFinanceBalanceAmount());
        Assert.Equal(PaymentMovements.Rows[0].AsString("Id"), CancellationMovements.Rows[0].AsString("CancelledMovementId"));
        Assert.Equal(CancellationMovements.Rows[0].AsString("Id"), GetPaymentFinanceMovementsBySource(PaymentId).Rows[0].AsString("CancellationMovementId"));
        Assert.Equal((int)TradeStatus.Cancelled, GetJournalEntry(PaymentJournalEntry.AsString("Id")).AsInteger("StatusId"));
        Assert.Equal(CancellationJournalEntry.AsString("Id"), GetJournalEntry(PaymentJournalEntry.AsString("Id")).AsString("CancellationDocumentId"));
        Assert.Equal(PaymentJournalEntry.AsString("Id"), CancellationJournalEntry.AsString("CancelledDocumentId"));
        Assert.Equal(2, Lines.Rows.Count);
        Assert.Equal("10-3000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("10-2000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[1].AsDecimal("CreditAmount"));
    }
    [Fact]
    public void CustomerReceiptCancellationRejectsSettlementRows()
    {
        SalesInvoiceDataModule InvoiceModule = CreateSalesInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = CreatePayment("CustomerReceipt", "CUST-ACME", InvoiceMovement.AsString("Id"), 124m);
        PaymentModule.Post();
        PaymentDataModule CancellationModule = PaymentModule.CreateCancellation();
        DataRow Settlement = CancellationModule.GetTable("PaymentSettlement").AddNewRow();
        Settlement.SetValue("FinanceMovementId", InvoiceMovement.AsString("Id"));
        Settlement.SetValue("Amount", 1m);

        TripousBusinessException Error = Assert.Throws<TripousBusinessException>(() => CancellationModule.Commit());

        Assert.Contains("cancellation documents cannot have settlement lines", Error.Message);
    }
    [Fact]
    public void PostingSupplierPaymentCancellationReversesPayment()
    {
        decimal PreviousSupplierBalance = GetFinanceBalanceAmount("SUP-HELIOS", TradeType.Purchases);
        decimal PreviousBankBalance = GetBankFinanceBalanceAmount();
        PurchaseInvoiceDataModule InvoiceModule = CreatePurchaseInvoice();
        InvoiceModule.Post();
        DataRow InvoiceMovement = GetFinanceMovementBySource(InvoiceModule.CurrentRow.AsString("Id"));
        PaymentDataModule PaymentModule = CreatePayment("SupplierPayment", "SUP-HELIOS", InvoiceMovement.AsString("Id"), 124m);
        PaymentModule.Post();
        string PaymentId = PaymentModule.CurrentRow.AsString("Id");
        PaymentDataModule CancellationModule = PaymentModule.CreateCancellation();
        CancellationModule.Commit();
        string CancellationId = CancellationModule.CurrentRow.AsString("Id");

        CancellationModule.Post();
        DataTable Lines = GetPaymentJournalEntryLinesBySource(CancellationId);

        Assert.Equal((int)TradeStatus.Cancelled, GetPayment(PaymentId).AsInteger("StatusId"));
        Assert.True(GetPayment(PaymentId).AsBoolean("IsCancelled"));
        Assert.Equal(CancellationId, GetPayment(PaymentId).AsString("CancellationPaymentId"));
        Assert.Equal(PreviousSupplierBalance - 124m, GetFinanceBalanceAmount("SUP-HELIOS", TradeType.Purchases));
        Assert.Equal(PreviousBankBalance, GetBankFinanceBalanceAmount());
        Assert.Equal(2, Lines.Rows.Count);
        Assert.Equal("10-2000", Lines.Rows[0].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[0].AsDecimal("DebitAmount"));
        Assert.Equal("20-1000", Lines.Rows[1].AsString("AccountCode"));
        Assert.Equal(124m, Lines.Rows[1].AsDecimal("CreditAmount"));
    }
}
