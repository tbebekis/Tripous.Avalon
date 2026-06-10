/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Base data module for commercial documents.
/// </summary>
/// <remarks>
/// Calculates commercial values, resolves line taxes, and maintains the
/// generated TradeLineTax and TradeTax snapshot rows.
/// </remarks>
public class TradeDataModule: DocumentDataModule
{
    // ● private fields
    IPriceResolver fPriceResolver;
    ITaxResolver fTaxResolver;

    // ● protected fields
    protected int fCalculationLevel;
    protected bool IsCopyingPersonAddresses = false;
    
    // ● protected
    /// <summary>
    /// Rounds a commercial or tax amount using the document calculation precision.
    /// </summary>
    protected virtual decimal RoundAmount(decimal Value) => Math.Round(Value, 4, MidpointRounding.AwayFromZero);
    /// <summary>
    /// Returns true when a table is the commercial document line table.
    /// </summary>
    protected virtual bool IsTradeLineTable(MemTable Table)
    {
        return Table != null
               && Table.TableName.IsSameText("TradeLine")
               && Table.ContainsColumn("GrossAmount")
               && Table.ContainsColumn("DiscountPercent")
               && Table.ContainsColumn("DiscountAmount")
               && Table.ContainsColumn("NetAmount")
               && Table.ContainsColumn("TaxAmount")
               && Table.ContainsColumn("TotalAmount");
    }
    /// <summary>
    /// Returns an item table by name.
    /// </summary>
    protected virtual MemTable FindItemTable(string TableName)
    {
        return ItemTables.FirstOrDefault(Table => Table.TableName.IsSameText(TableName));
    }
    /// <summary>
    /// Creates the configured price resolver.
    /// </summary>
    protected virtual IPriceResolver CreatePriceResolver()
    {
        string ClassName = AppDefaultProperties.Sales.PriceResolverClassName;
        if (string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException("SalesDefaults.PriceResolverClassName is not defined.");

        return TypeStore.CreateInstance<IPriceResolver>(ClassName);
    }
    /// <summary>
    /// Creates the configured tax resolver.
    /// </summary>
    protected virtual ITaxResolver CreateTaxResolver()
    {
        string ClassName = AppDefaultProperties.Sales.TaxResolverClassName;
        if (string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException("SalesDefaults.TaxResolverClassName is not defined.");

        return TypeStore.CreateInstance<ITaxResolver>(ClassName);
    }
    /// <summary>
    /// Returns the configured price list type or resolves the current sales default.
    /// </summary>
    protected virtual string GetPriceListTypeId()
    {
        string Result = AppDefaultProperties.Sales.PriceListTypeId;
        return !string.IsNullOrWhiteSpace(Result) ? Result : DataLib.GetDefaultPriceListTypeId();
    }
    /// <summary>
    /// Creates the complete price context for a commercial document line.
    /// </summary>
    protected virtual PriceResolveArgs CreatePriceResolveArgs(DataRow Row)
    {
        return new PriceResolveArgs
        {
            TradeType = (TradeType)CurrentRow.AsInteger("TradeTypeId"),
            PriceListTypeId = GetPriceListTypeId(),
            PersonId = CurrentRow.AsString("PersonId"),
            ProductId = Row.AsString("ProductId"),
            UnitOfMeasureId = Row.AsString("UnitOfMeasureId"),
            Quantity = Row.AsDecimal("Quantity"),
            TradeDate = CurrentRow.AsDateTime("TradeDate", DateTime.Today),
            CurrencyId = CurrentRow.AsString("CurrencyId"),
        };
    }
    /// <summary>
    /// Converts a tax-inclusive list price to the tax-exclusive line price.
    /// </summary>
    protected virtual decimal GetTaxExclusiveUnitPrice(DataRow Row, decimal UnitPrice)
    {
        if (UnitPrice == 0)
            return 0;

        TaxResolveArgs Args = CreateTaxResolveArgs(Row);
        Args.TaxableAmount = UnitPrice;
        TaxResult Result = fTaxResolver.Resolve(Args);

        return Result.TaxPercent != 0
            ? RoundAmount(UnitPrice / (1 + Result.TaxPercent / 100))
            : UnitPrice;
    }
    /// <summary>
    /// Resolves and assigns the unit price of a commercial document line.
    /// </summary>
    protected virtual void ResolveLinePrice(DataRow Row)
    {
        PriceResult Result = fPriceResolver.Resolve(CreatePriceResolveArgs(Row));
        decimal UnitPrice = Result.IsFound ? Result.UnitPrice : 0;

        if (Result.IsFound && Result.IsTaxIncluded)
            UnitPrice = GetTaxExclusiveUnitPrice(Row, UnitPrice);

        Row.SetValue("UnitPrice", UnitPrice);
    }
    /// <summary>
    /// Resolves prices for all active commercial document lines.
    /// </summary>
    protected virtual void ResolvePrices()
    {
        foreach (MemTable Table in ItemTables.Where(IsTradeLineTable))
            foreach (DataRow Row in Table.Rows)
                if (Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                    ResolveLinePrice(Row);
    }
    /// <summary>
    /// Loads the tax business group assigned to a person.
    /// </summary>
    protected virtual string LoadTaxBusinessGroupId(string PersonId)
    {
        if (string.IsNullOrWhiteSpace(PersonId))
            return "";

        string SqlText = @"
select
    TaxBusinessGroupId
from
    Person
where
    Id = :PersonId
";

        DataRow Row = Store.SelectResults(SqlText, new Dictionary<string, object>()
        {
            ["PersonId"] = PersonId,
        });

        return Row?.AsString("TaxBusinessGroupId") ?? "";
    }
    /// <summary>
    /// Loads the company branch address used as the tax origin.
    /// </summary>
    protected virtual PersonAddress LoadOriginAddress(string BranchId)
    {
        PersonAddress Result = new();
        if (string.IsNullOrWhiteSpace(BranchId))
            return Result;

        string SqlText = @"
select
    Id,
    Name,
    CountryId,
    City,
    PostalCode,
    AddressLine1,
    AddressLine2
from
    CompanyBranch
where
    Id = :BranchId
";

        DataRow Row = Store.SelectResults(SqlText, new Dictionary<string, object>()
        {
            ["BranchId"] = BranchId,
        });

        if (Row != null)
        {
            Result.Id = Row.AsString("Id");
            Result.Name = Row.AsString("Name");
            Result.CountryId = Row.AsString("CountryId");
            Result.City = Row.AsString("City");
            Result.PostalCode = Row.AsString("PostalCode");
            Result.AddressLine1 = Row.AsString("AddressLine1");
            Result.AddressLine2 = Row.AsString("AddressLine2");
        }

        return Result;
    }
    /// <summary>
    /// Creates a destination address from the shipping or billing document snapshot.
    /// </summary>
    protected virtual PersonAddress GetDestinationAddress()
    {
        if (CurrentRow == null)
            return new PersonAddress();

        bool UseShippingAddress = !string.IsNullOrWhiteSpace(CurrentRow.AsString("ShippingCountryId"));
        string Prefix = UseShippingAddress ? "Shipping" : "Billing";

        return new PersonAddress
        {
            Name = CurrentRow.AsString($"{Prefix}Name"),
            CountryId = CurrentRow.AsString($"{Prefix}CountryId"),
            Region = CurrentRow.AsString($"{Prefix}Region"),
            City = CurrentRow.AsString($"{Prefix}City"),
            PostalCode = CurrentRow.AsString($"{Prefix}PostalCode"),
            AddressLine1 = CurrentRow.AsString($"{Prefix}AddressLine1"),
            AddressLine2 = CurrentRow.AsString($"{Prefix}AddressLine2"),
        };
    }
    /// <summary>
    /// Creates the complete tax context for a commercial document line.
    /// </summary>
    protected virtual TaxResolveArgs CreateTaxResolveArgs(DataRow Row)
    {
        return new TaxResolveArgs
        {
            TradeId = CurrentRow.AsString("Id"),
            TradeLineId = Row.AsString("Id"),
            DocumentTypeId = CurrentRow.AsString("DocumentTypeId"),
            TradeType = (TradeType)CurrentRow.AsInteger("TradeTypeId"),
            TradeDate = CurrentRow.AsDateTime("TradeDate", DateTime.Today),
            PersonId = CurrentRow.AsString("PersonId"),
            TaxBusinessGroupId = CurrentRow.AsString("TaxBusinessGroupId"),
            ProductId = Row.AsString("ProductId"),
            TaxProductGroupId = Row.AsString("TaxProductGroupId"),
            OriginTaxJurisdictionId = CurrentRow.AsString("OriginTaxJurisdictionId"),
            DestinationTaxJurisdictionId = CurrentRow.AsString("DestinationTaxJurisdictionId"),
            OriginAddress = LoadOriginAddress(CurrentRow.AsString("BranchId")),
            DestinationAddress = GetDestinationAddress(),
            TaxableAmount = Row.AsDecimal("NetAmount"),
        };
    }
    /// <summary>
    /// Deletes all active rows from a generated tax table.
    /// </summary>
    protected virtual void DeleteRows(IEnumerable<DataRow> Rows)
    {
        foreach (DataRow Row in Rows.ToArray())
            if (Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                Row.Delete();
    }
    /// <summary>
    /// Replaces the stored tax components of a commercial document line.
    /// </summary>
    protected virtual void ReplaceLineTaxRows(DataRow TradeLineRow, TaxResult Result)
    {
        MemTable TradeLineTaxTable = FindItemTable("TradeLineTax");
        if (TradeLineTaxTable == null)
            return;

        DeleteRows(TradeLineTaxTable.GetChildRows(TradeLineRow));

        MemTable TradeLineTable = TradeLineRow.Table as MemTable;
        DataRow PreviousCurrentRow = TradeLineTable.CurrentRow;
        try
        {
            TradeLineTable.CurrentRow = TradeLineRow;
            foreach (TaxComponent Component in Result.Components)
            {
                DataRow Row = TradeLineTaxTable.NewRow();
                Row.SetValue("TaxRuleId", Component.TaxRuleId);
                Row.SetValue("TaxRateId", Component.TaxRateId);
                Row.SetValue("TaxJurisdictionId", Component.TaxJurisdictionId);
                Row.SetValue("TaxClauseId", string.IsNullOrWhiteSpace(Component.TaxClauseId) ? DBNull.Value : Component.TaxClauseId);
                Row.SetValue("SequenceNo", Component.SequenceNo);
                Row.SetValue("TaxCalculationTypeId", (int)Component.TaxCalculationType);
                Row.SetValue("TaxRatePercent", Component.TaxRatePercent);
                Row.SetValue("TaxableAmount", Component.TaxableAmount);
                Row.SetValue("TaxAmount", Component.TaxAmount);
                Row.SetValue("IsExempt", Component.IsExempt);
                Row.SetValue("IsReverseCharge", Component.IsReverseCharge);
                Row.SetValue("TaxClauseText", string.IsNullOrWhiteSpace(Component.TaxClauseText) ? DBNull.Value : Component.TaxClauseText);
                TradeLineTaxTable.Rows.Add(Row);
            }
        }
        finally
        {
            TradeLineTable.CurrentRow = PreviousCurrentRow;
        }
    }
    /// <summary>
    /// Resolves and stores the tax result of a commercial document line.
    /// </summary>
    protected virtual void CalculateLineTax(DataRow Row)
    {
        TaxResult Result = fTaxResolver.Resolve(CreateTaxResolveArgs(Row));

        CurrentRow.SetValue("OriginTaxJurisdictionId", string.IsNullOrWhiteSpace(Result.OriginTaxJurisdictionId) ? DBNull.Value : Result.OriginTaxJurisdictionId);
        CurrentRow.SetValue("DestinationTaxJurisdictionId", string.IsNullOrWhiteSpace(Result.DestinationTaxJurisdictionId) ? DBNull.Value : Result.DestinationTaxJurisdictionId);
        Row.SetValue("TaxPercent", Result.TaxPercent);
        Row.SetValue("IsTaxExempt", Result.IsExempt);
        Row.SetValue("IsReverseCharge", Result.IsReverseCharge);
        Row.SetValue("TaxAmount", Result.TaxAmount);
        Row.SetValue("TotalAmount", RoundAmount(Row.AsDecimal("NetAmount") + Result.TaxAmount));

        ReplaceLineTaxRows(Row, Result);
    }
    /// <summary>
    /// Removes tax component rows whose commercial line no longer exists.
    /// </summary>
    protected virtual void DeleteOrphanLineTaxRows()
    {
        MemTable TradeLineTable = ItemTables.FirstOrDefault(IsTradeLineTable);
        MemTable TradeLineTaxTable = FindItemTable("TradeLineTax");
        if (TradeLineTable == null || TradeLineTaxTable == null)
            return;

        HashSet<string> TradeLineIds = TradeLineTable.Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .Select(Row => Row.AsString("Id"))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        DeleteRows(TradeLineTaxTable.Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .Where(Row => !TradeLineIds.Contains(Row.AsString("TradeLineId"))));
    }
    /// <summary>
    /// Rebuilds document tax summaries from the stored line tax components.
    /// </summary>
    protected virtual void CalculateTaxSummary()
    {
        MemTable TradeLineTaxTable = FindItemTable("TradeLineTax");
        MemTable TradeTaxTable = FindItemTable("TradeTax");
        if (TradeLineTaxTable == null || TradeTaxTable == null || CurrentRow == null)
            return;

        DeleteOrphanLineTaxRows();
        DeleteRows(TradeTaxTable.GetChildRows(CurrentRow));

        IEnumerable<IGrouping<string, DataRow>> Groups = TradeLineTaxTable.Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .GroupBy(Row => Row.AsString("TaxRuleId"), StringComparer.OrdinalIgnoreCase);

        foreach (IGrouping<string, DataRow> Group in Groups)
        {
            DataRow ComponentRow = Group.First();
            decimal TaxableAmount = RoundAmount(Group.Sum(Row => Row.AsDecimal("TaxableAmount")));
            decimal TaxAmount = RoundAmount(Group.Sum(Row => Row.AsDecimal("TaxAmount")));
            DataRow Row = TradeTaxTable.NewRow();
            Row.SetValue("TaxRuleId", Group.Key);
            Row.SetValue("TaxRateId", ComponentRow.AsString("TaxRateId"));
            Row.SetValue("TaxRatePercent", ComponentRow.AsDecimal("TaxRatePercent"));
            Row.SetValue("TaxableAmount", TaxableAmount);
            Row.SetValue("TaxAmount", TaxAmount);
            Row.SetValue("TotalAmount", RoundAmount(TaxableAmount + TaxAmount));
            TradeTaxTable.Rows.Add(Row);
        }
    }
    /// <summary>
    /// Calculates the commercial values of a document line.
    /// </summary>
    protected virtual void CalculateLine(DataRow Row, string ChangedFieldName)
    {
        decimal Quantity = Row.AsDecimal("Quantity");
        decimal UnitRatio = Row.AsDecimal("UnitRatio", 1);
        decimal UnitPrice = Row.AsDecimal("UnitPrice");
        decimal GrossAmount = RoundAmount(Quantity * UnitPrice);
        decimal DiscountPercent = Row.AsDecimal("DiscountPercent");
        decimal DiscountAmount = Row.AsDecimal("DiscountAmount");

        if ("DiscountAmount".IsSameText(ChangedFieldName))
        {
            DiscountAmount = Math.Clamp(DiscountAmount, 0, GrossAmount);
            DiscountPercent = GrossAmount != 0 ? RoundAmount(DiscountAmount * 100 / GrossAmount) : 0;
        }
        else
        {
            DiscountPercent = Math.Clamp(DiscountPercent, 0, 100);
            DiscountAmount = RoundAmount(GrossAmount * DiscountPercent / 100);
        }

        decimal NetAmount = RoundAmount(GrossAmount - DiscountAmount);
        decimal NetUnitPrice = Quantity != 0 ? RoundAmount(NetAmount / Quantity) : 0;

        Row.SetValue("PrimaryUnitQuantity", RoundAmount(Quantity * UnitRatio));
        Row.SetValue("GrossAmount", GrossAmount);
        Row.SetValue("DiscountPercent", DiscountPercent);
        Row.SetValue("DiscountAmount", DiscountAmount);
        Row.SetValue("NetUnitPrice", NetUnitPrice);
        Row.SetValue("NetAmount", NetAmount);
        CalculateLineTax(Row);
    }
    /// <summary>
    /// Calculates commercial document totals.
    /// </summary>
    protected virtual void CalculateTotals(string ChangedFieldName)
    {
        if (CurrentRow == null)
            return;

        MemTable TradeLineTable = ItemTables.FirstOrDefault(IsTradeLineTable);
        decimal LinesAmount = TradeLineTable != null
            ? TradeLineTable.Rows.Cast<DataRow>()
                .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                .Sum(Row => Row.AsDecimal("NetAmount"))
            : 0;
        decimal DiscountPercent = CurrentRow.AsDecimal("DiscountPercent");
        decimal DiscountAmount = CurrentRow.AsDecimal("DiscountAmount");

        if ("DiscountAmount".IsSameText(ChangedFieldName))
        {
            DiscountAmount = Math.Clamp(DiscountAmount, 0, LinesAmount);
            DiscountPercent = LinesAmount != 0 ? RoundAmount(DiscountAmount * 100 / LinesAmount) : 0;
        }
        else
        {
            DiscountPercent = Math.Clamp(DiscountPercent, 0, 100);
            DiscountAmount = RoundAmount(LinesAmount * DiscountPercent / 100);
        }

        decimal TaxAmount = TradeLineTable != null
            ? TradeLineTable.Rows.Cast<DataRow>()
                .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                .Sum(Row => Row.AsDecimal("TaxAmount"))
            : 0;
        decimal NetAmount = RoundAmount(LinesAmount - DiscountAmount + CurrentRow.AsDecimal("ChargesAmount"));

        CurrentRow.SetValue("LinesAmount", RoundAmount(LinesAmount));
        CurrentRow.SetValue("DiscountPercent", DiscountPercent);
        CurrentRow.SetValue("DiscountAmount", DiscountAmount);
        CurrentRow.SetValue("NetAmount", NetAmount);
        CurrentRow.SetValue("TaxAmount", RoundAmount(TaxAmount));
        CurrentRow.SetValue("TotalAmount", RoundAmount(NetAmount + TaxAmount));
    }
    /// <summary>
    /// Recalculates all commercial document lines, taxes, and totals.
    /// </summary>
    protected virtual void Calculate()
    {
        if (fCalculationLevel > 0)
            return;

        fCalculationLevel++;
        try
        {
            foreach (MemTable Table in ItemTables.Where(IsTradeLineTable))
            {
                foreach (DataRow Row in Table.Rows)
                {
                    if (Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                        CalculateLine(Row, "DiscountPercent");
                }
            }

            CalculateTaxSummary();
            CalculateTotals("DiscountPercent");
        }
        finally
        {
            fCalculationLevel--;
        }
    }
    /// <summary>
    /// Recalculates the document after a commercial line is deleted.
    /// </summary>
    protected virtual void TradeLine_RowDeleted(object Sender, DataRowChangeEventArgs Args)
    {
        if (fCalculationLevel == 0 && State.In(DataMode.Insert | DataMode.Edit))
            Calculate();
    }
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// </summary>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("DocumentTypeId", DocumentType.Id);
            Row.SetValue("TradeStatusId", (int)TradeStatus.Draft);
            Row.SetValue("ExchangeRate", 1);
            Row.SetValue("TradeDate", DateTime.UtcNow.Date);
        }
    }
    protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs ea)
    {
        base.NewRowAdded(Table, ea);

        if (Table == tblItem || IsTransforming || CurrentRow == null || !tblItem.ContainsColumn("WarehouseId") || !Table.ContainsColumn("WarehouseId"))
            return;

        ea.Row.SetValue("WarehouseId", CurrentRow["WarehouseId"]);
    }
    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs ea)
    {
        base.ColumnChanged(Table, ea);

        if (fCalculationLevel > 0 || IsCopyingPersonAddresses || !State.In(DataMode.Insert | DataMode.Edit))
            return;

        string FieldName = ea.Column.ColumnName;
        bool IsLineField = FieldName.IsSameText("Quantity")
                           || FieldName.IsSameText("UnitOfMeasureId")
                           || FieldName.IsSameText("UnitRatio")
                           || FieldName.IsSameText("UnitPrice")
                           || FieldName.IsSameText("DiscountPercent")
                           || FieldName.IsSameText("DiscountAmount")
                           || FieldName.IsSameText("ProductId")
                           || FieldName.IsSameText("TaxProductGroupId");
        bool IsPriceLineField = FieldName.IsSameText("ProductId")
                                || FieldName.IsSameText("TaxProductGroupId")
                                || FieldName.IsSameText("UnitOfMeasureId")
                                || FieldName.IsSameText("Quantity");
        bool IsHeaderField = FieldName.IsSameText("DiscountPercent")
                             || FieldName.IsSameText("DiscountAmount")
                             || FieldName.IsSameText("ChargesAmount");
        bool IsTaxHeaderField = FieldName.IsSameText("PersonId")
                                || FieldName.IsSameText("TaxBusinessGroupId")
                                || FieldName.IsSameText("TradeDate")
                                || FieldName.IsSameText("TradeTypeId")
                                || FieldName.IsSameText("BranchId")
                                || FieldName.IsSameText("OriginTaxJurisdictionId")
                                || FieldName.IsSameText("DestinationTaxJurisdictionId")
                                || FieldName.StartsWith("Billing", StringComparison.OrdinalIgnoreCase)
                                || FieldName.StartsWith("Shipping", StringComparison.OrdinalIgnoreCase);
        bool IsPriceHeaderField = FieldName.IsSameText("PersonId")
                                  || FieldName.IsSameText("TradeDate")
                                  || FieldName.IsSameText("TradeTypeId")
                                  || FieldName.IsSameText("CurrencyId");

        fCalculationLevel++;
        try
        {
            if (Table == tblItem && FieldName.IsSameText("PersonId"))
                ea.Row.SetValue("TaxBusinessGroupId", LoadTaxBusinessGroupId(ea.Row.AsString("PersonId")));
            if (Table == tblItem && FieldName.IsSameText("BranchId"))
                ea.Row.SetValue("OriginTaxJurisdictionId", DBNull.Value);
            if (Table == tblItem && (FieldName.StartsWith("Billing", StringComparison.OrdinalIgnoreCase)
                                     || FieldName.StartsWith("Shipping", StringComparison.OrdinalIgnoreCase)))
                ea.Row.SetValue("DestinationTaxJurisdictionId", DBNull.Value);

            if (IsTradeLineTable(Table) && IsLineField)
            {
                if (IsPriceLineField)
                    ResolveLinePrice(ea.Row);
                CalculateLine(ea.Row, FieldName);
                CalculateTaxSummary();
                CalculateTotals("DiscountPercent");
            }
            else if (Table == tblItem && IsHeaderField)
            {
                CalculateTotals(FieldName);
            }
            else if (Table == tblItem && (IsTaxHeaderField || IsPriceHeaderField))
            {
                if (IsPriceHeaderField)
                    ResolvePrices();

                foreach (MemTable TradeLineTable in ItemTables.Where(IsTradeLineTable))
                    foreach (DataRow Row in TradeLineTable.Rows)
                        if (Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                            CalculateLine(Row, "DiscountPercent");

                CalculateTaxSummary();
                CalculateTotals("DiscountPercent");
            }
        }
        finally
        {
            fCalculationLevel--;
        }
    }
    protected override void Commiting(bool Reselect)
    {
        Calculate();
        base.Commiting(Reselect);
    }
    
    // ● construction
    public TradeDataModule()
    {
    }

    // ● public
    public override void Initialize(ModuleDef ModuleDef)
    {
        bool IsInitialized = this.ModuleDef != null;
        base.Initialize(ModuleDef);

        if (!IsInitialized)
        {
            fPriceResolver = CreatePriceResolver();
            fTaxResolver = CreateTaxResolver();
            foreach (MemTable Table in ItemTables.Where(IsTradeLineTable))
                Table.RowDeleted += TradeLine_RowDeleted;
        }
    }
}
