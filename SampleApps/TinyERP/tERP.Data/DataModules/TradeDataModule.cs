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
    protected virtual bool IsPurchaseTrade() => DocumentType.TradeTypeId == (int)TradeType.Purchases;
    protected virtual bool RequiresShippingAddress()
    {
        string[] ModuleNames =
        [
            "SalesOrder", "SalesDeliveryNote", "SalesReturn",
            "PurchaseOrder", "PurchaseDeliveryNote", "PurchaseReturn"
        ];
        return ModuleNames.Contains(ModuleDef.Name, StringComparer.OrdinalIgnoreCase);
    }
    /// <summary>
    /// Returns true when the persisted document has remaining quantity for the specified counter.
    /// </summary>
    protected virtual bool HasRemainingQuantity(string QuantityFieldName)
    {
        if (CurrentRow == null)
            return false;

        string SqlText = $"""
                          select count(*)
                          from TradeLine
                          where TradeId = :TradeId
                            and Quantity > {QuantityFieldName}
                          """;
        int Count = Store.IntegerResult(SqlText, 0, new Dictionary<string, object>()
        {
            ["TradeId"] = CurrentRow.AsString("Id"),
        });
        return Count > 0;
    }
    protected virtual string GetPriceResolverClassName() => IsPurchaseTrade()
        ? AppDefaultProperties.Purchase.PriceResolverClassName
        : AppDefaultProperties.Sales.PriceResolverClassName;
    protected virtual string GetTaxResolverClassName() => IsPurchaseTrade()
        ? AppDefaultProperties.Purchase.TaxResolverClassName
        : AppDefaultProperties.Sales.TaxResolverClassName;
    /// <summary>
    /// Creates the configured price resolver.
    /// </summary>
    protected virtual IPriceResolver CreatePriceResolver()
    {
        string ClassName = GetPriceResolverClassName();
        if (string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException("PriceResolverClassName is not defined.");

        return TypeStore.CreateInstance<IPriceResolver>(ClassName);
    }
    /// <summary>
    /// Creates the configured tax resolver.
    /// </summary>
    protected virtual ITaxResolver CreateTaxResolver()
    {
        string ClassName = GetTaxResolverClassName();
        if (string.IsNullOrWhiteSpace(ClassName))
            throw new TripousException("TaxResolverClassName is not defined.");

        return TypeStore.CreateInstance<ITaxResolver>(ClassName);
    }
    /// <summary>
    /// Returns the configured price list type for the current trade type.
    /// </summary>
    protected virtual string GetPriceListTypeId()
    {
        string Result = IsPurchaseTrade()
            ? AppDefaultProperties.Purchase.PriceListTypeId
            : AppDefaultProperties.Sales.PriceListTypeId;
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
            PriceListTypeId = CurrentRow.AsString("PriceListTypeId"),
            PersonId = CurrentRow.AsString("PersonId"),
            ProductId = Row.AsString("ProductId"),
            UnitOfMeasureId = Row.AsString("UnitOfMeasureId"),
            Quantity = Row.AsDecimal("Quantity"),
            TradeDate = CurrentRow.AsDateTime("TradeDate", DateTime.Today),
            CurrencyId = CurrentRow.AsString("CurrencyId"),
        };
    }
    /// <summary>
    /// Restores the document tax context after copying address snapshots.
    /// </summary>
    protected virtual void CopyTaxContext(DataRow Source, DataRow Dest)
    {
        string[] Fields =
        [
            "TaxBusinessGroupId",
            "OriginTaxJurisdictionId",
            "DestinationTaxJurisdictionId"
        ];

        foreach (string FieldName in Fields)
        {
            if (!Source.Table.Columns.Contains(FieldName) || !Dest.Table.Columns.Contains(FieldName))
                continue;

            Dest.SetValue(FieldName, Source[FieldName]);
        }
    }
    /// <summary>
    /// Collects quantities grouped by their source document line.
    /// </summary>
    protected virtual Dictionary<string, decimal> GetTransformationSourceQuantities(string SourceLineLabel, string EmptyDocumentMessage)
    {
        MemTable LineTable = FindItemTable("TradeLine");
        if (LineTable == null)
            throw new TripousDataException("TradeLine table is not available.");

        Dictionary<string, decimal> Result = new(StringComparer.OrdinalIgnoreCase);
        foreach (DataRow Row in LineTable.Rows)
        {
            if (Row.RowState == DataRowState.Deleted || Row.RowState == DataRowState.Detached)
                continue;

            string SourceTradeLineId = Row.AsString("SourceTradeLineId");
            if (string.IsNullOrWhiteSpace(SourceTradeLineId))
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Source {SourceLineLabel} line is required.");

            decimal Quantity = Row.AsDecimal("Quantity");
            if (Quantity <= 0)
                throw new TripousBusinessException($"{GetLineLabel(Row)}: Quantity must be greater than zero.");

            Result.TryGetValue(SourceTradeLineId, out decimal TotalQuantity);
            Result[SourceTradeLineId] = TotalQuantity + Quantity;
        }

        if (Result.Count == 0)
            throw new TripousBusinessException(EmptyDocumentMessage);

        return Result;
    }
    /// <summary>
    /// Updates a quantity counter on source document lines inside the posting transaction.
    /// </summary>
    protected virtual void UpdateSourceTransformationQuantities(DbTransaction Transaction, string SourceDocumentName, string QuantityFieldName, string ActionLabel, string EmptyDocumentMessage)
    {
        string SourceId = CurrentRow.AsString("SourceId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return;

        DataRow SourceDocument = Store.Provider.SelectForUpdate(Transaction, "Trade", "Id", SourceId);
        if (SourceDocument == null)
            throw new TripousBusinessException($"The source {SourceDocumentName} does not exist.");
        if ((TradeStatus)SourceDocument.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException($"Only posted {SourceDocumentName}s can be transformed.");
        if (SourceDocument.AsBoolean("IsCancelled"))
            throw new TripousBusinessException($"A cancelled {SourceDocumentName} cannot be transformed.");

        Dictionary<string, decimal> Quantities = GetTransformationSourceQuantities(SourceDocumentName, EmptyDocumentMessage);
        foreach (KeyValuePair<string, decimal> Entry in Quantities.OrderBy(Item => Item.Key))
        {
            DataRow SourceLine = Store.Provider.SelectForUpdate(Transaction, "TradeLine", "Id", Entry.Key);
            if (SourceLine == null || !SourceLine.AsString("TradeId").IsSameText(SourceId))
                throw new TripousBusinessException($"A source {SourceDocumentName} line does not exist.");

            decimal SourceQuantity = SourceLine.AsDecimal("Quantity");
            decimal TransformedQuantity = SourceLine.AsDecimal(QuantityFieldName);
            decimal RemainingQuantity = SourceQuantity - TransformedQuantity;
            if (Entry.Value > RemainingQuantity)
                throw new TripousBusinessException($"{ActionLabel} quantity {Entry.Value} exceeds remaining quantity {RemainingQuantity}.");

            string SqlText = $"""
                              update TradeLine
                              set {QuantityFieldName} = :TransformedQuantity
                              where Id = :Id
                              """;
            Store.ExecSql(Transaction, SqlText, new Dictionary<string, object>()
            {
                ["Id"] = Entry.Key,
                ["TransformedQuantity"] = TransformedQuantity + Entry.Value,
            });
        }
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
    /// Resolves the applicable price result of a commercial document line.
    /// </summary>
    protected virtual PriceResult ResolveLinePriceResult(DataRow Row) => fPriceResolver.Resolve(CreatePriceResolveArgs(Row));
    /// <summary>
    /// Resolves and assigns the unit price of a commercial document line.
    /// </summary>
    protected virtual void ResolveLinePrice(DataRow Row)
    {
        PriceResult Result = ResolveLinePriceResult(Row);
        if (!Result.IsFound)
            return;

        decimal UnitPrice = Result.UnitPrice;
        if (Result.IsTaxIncluded)
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
    /// Returns a display label for a commercial document line.
    /// </summary>
    protected virtual string GetLineLabel(DataRow Row)
    {
        string ProductCode = Row.AsString("ProductCode");
        string ProductName = Row.AsString("ProductName");
        string ProductText = !string.IsNullOrWhiteSpace(ProductCode) ? ProductCode : ProductName;
        int DisplayOrder = Row.AsInteger("DisplayOrder");

        return !string.IsNullOrWhiteSpace(ProductText)
            ? $"Line {DisplayOrder}: {ProductText}"
            : $"Line {DisplayOrder}";
    }
    /// <summary>
    /// Validates a commercial document line before commit.
    /// </summary>
    protected virtual void ValidateLine(DataRow Row, List<string> Errors)
    {
        string LineLabel = GetLineLabel(Row);

        bool HasTaxBusinessGroup = !string.IsNullOrWhiteSpace(CurrentRow.AsString("TaxBusinessGroupId"));
        bool HasTaxProductGroup = !string.IsNullOrWhiteSpace(Row.AsString("TaxProductGroupId"));
        if (!HasTaxProductGroup)
            Errors.Add($"{LineLabel}: Tax product group is required.");
        if (!HasTaxBusinessGroup
            || !HasTaxProductGroup
            || Row.AsDecimal("NetAmount") - Row.AsDecimal("DocumentDiscountAmount") == 0)
            return;

        TaxResult TaxResult = fTaxResolver.Resolve(CreateTaxResolveArgs(Row));
        if (string.IsNullOrWhiteSpace(TaxResult.OriginTaxJurisdictionId))
            Errors.Add($"{LineLabel}: Origin tax jurisdiction could not be resolved.");
        if (string.IsNullOrWhiteSpace(TaxResult.DestinationTaxJurisdictionId))
            Errors.Add($"{LineLabel}: Destination tax jurisdiction could not be resolved.");
        if (TaxResult.Components.Count == 0)
            Errors.Add($"{LineLabel}: No applicable tax rule was found.");
    }
    /// <summary>
    /// Validates the commercial document before commit.
    /// </summary>
    protected virtual void Validate()
    {
        List<string> Errors = [];

        if (string.IsNullOrWhiteSpace(CurrentRow.AsString("TaxBusinessGroupId")))
            Errors.Add("Tax business group is required.");
        ValidateAddress("Billing", "Billing", Errors);
        if (RequiresShippingAddress())
            ValidateAddress("Shipping", "Shipping", Errors);

        foreach (MemTable Table in ItemTables.Where(IsTradeLineTable))
            foreach (DataRow Row in Table.Rows)
                if (Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                    ValidateLine(Row, Errors);

        if (Errors.Count > 0)
            throw new TripousBusinessException(string.Join(Environment.NewLine, Errors));
    }
    /// <summary>
    /// Validates a document address snapshot.
    /// </summary>
    protected virtual void ValidateAddress(string FieldPrefix, string AddressLabel, List<string> Errors)
    {
        (string FieldSuffix, string FieldLabel)[] Fields =
        [
            ("Name", "name"),
            ("AddressLine1", "address line 1"),
            ("City", "city"),
            ("PostalCode", "postal code"),
            ("CountryId", "country")
        ];

        foreach ((string FieldSuffix, string FieldLabel) in Fields)
        {
            if (string.IsNullOrWhiteSpace(CurrentRow.AsString($"{FieldPrefix}{FieldSuffix}")))
                Errors.Add($"{AddressLabel} {FieldLabel} is required.");
        }
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
            TaxableAmount = RoundAmount(Row.AsDecimal("NetAmount") - Row.AsDecimal("DocumentDiscountAmount")),
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

        if (!string.IsNullOrWhiteSpace(Result.OriginTaxJurisdictionId))
            CurrentRow.SetValue("OriginTaxJurisdictionId", Result.OriginTaxJurisdictionId);
        if (!string.IsNullOrWhiteSpace(Result.DestinationTaxJurisdictionId))
            CurrentRow.SetValue("DestinationTaxJurisdictionId", Result.DestinationTaxJurisdictionId);
        Row.SetValue("TaxPercent", Result.TaxPercent);
        Row.SetValue("IsTaxExempt", Result.IsExempt);
        Row.SetValue("IsReverseCharge", Result.IsReverseCharge);
        Row.SetValue("TaxAmount", Result.TaxAmount);
        Row.SetValue("TotalAmount", RoundAmount(Row.AsDecimal("NetAmount") - Row.AsDecimal("DocumentDiscountAmount") + Result.TaxAmount));

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
    }
    /// <summary>
    /// Allocates the document discount proportionally to the commercial document lines.
    /// </summary>
    protected virtual void CalculateDocumentDiscount(List<DataRow> Rows, string ChangedFieldName)
    {
        if (CurrentRow == null)
            return;

        decimal LinesAmount = RoundAmount(Rows.Sum(Row => Row.AsDecimal("NetAmount")));
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

        CurrentRow.SetValue("LinesAmount", LinesAmount);
        CurrentRow.SetValue("DiscountPercent", DiscountPercent);
        CurrentRow.SetValue("DiscountAmount", DiscountAmount);

        foreach (DataRow Row in Rows)
            Row.SetValue("DocumentDiscountAmount", 0);

        List<DataRow> EligibleRows = Rows.Where(Row => Row.AsDecimal("NetAmount") > 0).ToList();
        decimal EligibleAmount = EligibleRows.Sum(Row => Row.AsDecimal("NetAmount"));
        decimal AllocatedAmount = 0;

        for (int Index = 0; Index < EligibleRows.Count; Index++)
        {
            DataRow Row = EligibleRows[Index];
            decimal LineDiscountAmount = Index == EligibleRows.Count - 1
                ? RoundAmount(DiscountAmount - AllocatedAmount)
                : RoundAmount(DiscountAmount * Row.AsDecimal("NetAmount") / EligibleAmount);

            Row.SetValue("DocumentDiscountAmount", LineDiscountAmount);
            AllocatedAmount += LineDiscountAmount;
        }
    }
    /// <summary>
    /// Calculates commercial document totals.
    /// </summary>
    protected virtual void CalculateTotals(List<DataRow> Rows)
    {
        if (CurrentRow == null)
            return;

        decimal LinesAmount = RoundAmount(Rows.Sum(Row => Row.AsDecimal("NetAmount")));
        decimal DiscountAmount = CurrentRow.AsDecimal("DiscountAmount");
        decimal TaxAmount = RoundAmount(Rows.Sum(Row => Row.AsDecimal("TaxAmount")));
        decimal NetAmount = RoundAmount(LinesAmount - DiscountAmount + CurrentRow.AsDecimal("ChargesAmount"));

        CurrentRow.SetValue("LinesAmount", LinesAmount);
        CurrentRow.SetValue("NetAmount", NetAmount);
        CurrentRow.SetValue("TaxAmount", TaxAmount);
        CurrentRow.SetValue("TotalAmount", RoundAmount(NetAmount + TaxAmount));
    }
    /// <summary>
    /// Recalculates commercial document lines, discount allocation, taxes, and totals.
    /// </summary>
    protected virtual void Calculate(DataRow ChangedRow, string ChangedLineFieldName, string ChangedHeaderFieldName)
    {
        List<DataRow> Rows = ItemTables
            .Where(IsTradeLineTable)
            .SelectMany(Table => Table.Rows.Cast<DataRow>())
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .ToList();

        foreach (DataRow Row in Rows)
            CalculateLine(Row, Row == ChangedRow ? ChangedLineFieldName : "DiscountPercent");

        CalculateDocumentDiscount(Rows, ChangedHeaderFieldName);

        foreach (DataRow Row in Rows)
            CalculateLineTax(Row);

        CalculateTaxSummary();
        CalculateTotals(Rows);
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
            Calculate(null, "DiscountAmount", "DiscountAmount");
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

        if (fCalculationLevel > 0 || IsTransforming || IsCopyingPersonAddresses || !State.In(DataMode.Insert | DataMode.Edit))
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
                                  || FieldName.IsSameText("PriceListTypeId")
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
                Calculate(ea.Row, FieldName, "DiscountPercent");
            }
            else if (Table == tblItem && IsHeaderField)
            {
                Calculate(null, "DiscountPercent", FieldName);
            }
            else if (Table == tblItem && (IsTaxHeaderField || IsPriceHeaderField))
            {
                if (IsPriceHeaderField)
                    ResolvePrices();

                Calculate(null, "DiscountPercent", "DiscountPercent");
            }
        }
        finally
        {
            fCalculationLevel--;
        }
    }
    // ● construction
    public TradeDataModule()
    {
    }

    // ● public
    public override void CheckCanCommit(bool Reselect)
    {
        base.CheckCanCommit(Reselect);
        Calculate();
        Validate();
    }
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
    /// <summary>
    /// Returns true when the persisted document has at least one line with remaining transformable quantity.
    /// </summary>
    public virtual bool HasRemainingTransformQuantity()
    {
        return HasRemainingQuantity("ExecutedQuantity");
    }
    /// <summary>
    /// Returns true when the persisted document has at least one line with remaining invoice quantity.
    /// </summary>
    public virtual bool HasRemainingInvoiceQuantity()
    {
        return HasRemainingQuantity("InvoicedQuantity");
    }
    /// <summary>
    /// Returns true when the persisted document has at least one line with remaining credit quantity.
    /// </summary>
    public virtual bool HasRemainingCreditQuantity()
    {
        return HasRemainingQuantity("CreditedQuantity");
    }
}
