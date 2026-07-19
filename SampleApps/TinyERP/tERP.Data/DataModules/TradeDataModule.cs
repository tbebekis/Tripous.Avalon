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
    /// Cancels the source Invoice and releases its quantities from the source Delivery Note.
    /// </summary>
    protected virtual void CancelSourceInvoice(DbTransaction Transaction, string SourceDocumentName)
    {
        string SourceId = CurrentRow.AsString("CancelsTradeId");
        if (string.IsNullOrWhiteSpace(SourceId))
            throw new TripousBusinessException($"The source {SourceDocumentName} is required.");

        DataRow SourceInvoice = Store.Provider.SelectForUpdate(Transaction, "Trade", "Id", SourceId);
        if (SourceInvoice == null)
            throw new TripousBusinessException($"The source {SourceDocumentName} does not exist.");
        if (!SourceInvoice.AsString("DocumentTypeId").IsSameText(DocumentType.CancellationTargetId))
            throw new TripousBusinessException($"The selected document is not a {SourceDocumentName}.");
        if (SourceInvoice.AsBoolean("IsCancelled") || !string.IsNullOrWhiteSpace(SourceInvoice.AsString("CancelledByTradeId")))
            throw new TripousBusinessException($"The {SourceDocumentName} is already cancelled.");
        if ((TradeStatus)SourceInvoice.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException($"Only posted {SourceDocumentName}s can be cancelled.");

        int CreditedLineCount = Store.IntegerResult(Transaction, """
                                                                  select count(*)
                                                                  from TradeLine
                                                                  where TradeId = :TradeId
                                                                    and CreditedQuantity > 0
                                                                  """, 0, new Dictionary<string, object>()
        {
            ["TradeId"] = SourceId,
        });
        if (CreditedLineCount > 0)
            throw new TripousBusinessException($"A {SourceDocumentName} with posted Credit Notes cannot be cancelled.");

        Dictionary<string, decimal> CancellationQuantities = GetTransformationSourceQuantities(SourceDocumentName, "The Cancellation document has no lines.");
        int SourceLineCount = Store.IntegerResult(Transaction, "select count(*) from TradeLine where TradeId = :TradeId", 0, new Dictionary<string, object>()
        {
            ["TradeId"] = SourceId,
        });
        MemTable CancellationLineTable = FindItemTable("TradeLine");
        DataRow[] CancellationLines = CancellationLineTable.Rows.Cast<DataRow>()
            .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .ToArray();
        if (CancellationQuantities.Count != SourceLineCount || CancellationLines.Length != SourceLineCount)
            throw new TripousBusinessException($"The Cancellation document must contain all {SourceDocumentName} lines.");

        Dictionary<string, decimal> DeliveryQuantities = new(StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, decimal> Entry in CancellationQuantities.OrderBy(Item => Item.Key))
        {
            DataRow InvoiceLine = Store.Provider.SelectForUpdate(Transaction, "TradeLine", "Id", Entry.Key);
            if (InvoiceLine == null || !InvoiceLine.AsString("TradeId").IsSameText(SourceId))
                throw new TripousBusinessException($"A source {SourceDocumentName} line does not exist.");
            if (Entry.Value != InvoiceLine.AsDecimal("Quantity"))
                throw new TripousBusinessException($"The Cancellation quantity must equal the {SourceDocumentName} quantity.");

            DataRow CancellationLine = CancellationLines.Single(Row => Row.AsString("SourceTradeLineId").IsSameText(Entry.Key));
            string[] LineStringFields = ["ProductId", "TaxProductGroupId", "WarehouseId", "UnitOfMeasureId"];
            foreach (string FieldName in LineStringFields)
            {
                if (!CancellationLine.AsString(FieldName).IsSameText(InvoiceLine.AsString(FieldName)))
                    throw new TripousBusinessException($"The Cancellation line must preserve the {SourceDocumentName} {FieldName} value.");
            }
            string[] LineDecimalFields =
            [
                "Quantity", "UnitRatio", "UnitPrice", "DiscountPercent", "DiscountAmount",
                "NetAmount", "DocumentDiscountAmount", "TaxAmount", "TotalAmount"
            ];
            foreach (string FieldName in LineDecimalFields)
            {
                if (CancellationLine.AsDecimal(FieldName) != InvoiceLine.AsDecimal(FieldName))
                    throw new TripousBusinessException($"The Cancellation line must preserve the {SourceDocumentName} {FieldName} value.");
            }

            string DeliveryLineId = InvoiceLine.AsString("SourceTradeLineId");
            if (string.IsNullOrWhiteSpace(DeliveryLineId))
                continue;

            DeliveryQuantities.TryGetValue(DeliveryLineId, out decimal Quantity);
            DeliveryQuantities[DeliveryLineId] = Quantity + InvoiceLine.AsDecimal("Quantity");
        }

        string[] HeaderStringFields =
        [
            "PersonId", "CurrencyId", "TaxBusinessGroupId",
            "OriginTaxJurisdictionId", "DestinationTaxJurisdictionId",
            "BillingName", "BillingAddressLine1", "BillingAddressLine2", "BillingCity", "BillingRegion", "BillingPostalCode", "BillingCountryId",
            "ShippingName", "ShippingAddressLine1", "ShippingAddressLine2", "ShippingCity", "ShippingRegion", "ShippingPostalCode", "ShippingCountryId"
        ];
        foreach (string FieldName in HeaderStringFields)
        {
            if (!CurrentRow.AsString(FieldName).IsSameText(SourceInvoice.AsString(FieldName)))
                throw new TripousBusinessException($"The Cancellation document must preserve the {SourceDocumentName} {FieldName} value.");
        }
        string[] HeaderDecimalFields =
        [
            "ExchangeRate", "LinesAmount", "DiscountPercent", "DiscountAmount",
            "ChargesAmount", "NetAmount", "TaxAmount", "TotalAmount"
        ];
        foreach (string FieldName in HeaderDecimalFields)
        {
            if (CurrentRow.AsDecimal(FieldName) != SourceInvoice.AsDecimal(FieldName))
                throw new TripousBusinessException($"The Cancellation document must preserve the {SourceDocumentName} {FieldName} value.");
        }

        foreach (KeyValuePair<string, decimal> Entry in DeliveryQuantities.OrderBy(Item => Item.Key))
        {
            DataRow DeliveryLine = Store.Provider.SelectForUpdate(Transaction, "TradeLine", "Id", Entry.Key);
            if (DeliveryLine == null)
                throw new TripousBusinessException("A source Delivery Note line does not exist.");

            decimal InvoicedQuantity = DeliveryLine.AsDecimal("InvoicedQuantity");
            if (Entry.Value > InvoicedQuantity)
                throw new TripousBusinessException("The source Delivery Note invoiced quantity is inconsistent.");

            Store.ExecSql(Transaction, """
                                       update TradeLine
                                       set InvoicedQuantity = :InvoicedQuantity
                                       where Id = :Id
                                       """, new Dictionary<string, object>()
            {
                ["Id"] = Entry.Key,
                ["InvoicedQuantity"] = InvoicedQuantity - Entry.Value,
            });
        }

        string UserId = Sys.GetCurrentAppUserId();
        DateTime Now = DateTime.UtcNow;
        Store.ExecSql(Transaction, """
                                   update Trade
                                   set TradeStatusId = :TradeStatusId,
                                       IsCancelled = :IsCancelled,
                                       CancelledByTradeId = :CancelledByTradeId,
                                       CancelledAt = :CancelledAt,
                                       CancelledBy = :CancelledBy,
                                       ModifiedAt = :ModifiedAt,
                                       ModifiedBy = :ModifiedBy
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = SourceId,
            ["TradeStatusId"] = (int)TradeStatus.Cancelled,
            ["IsCancelled"] = true,
            ["CancelledByTradeId"] = CurrentRow.AsString("Id"),
            ["CancelledAt"] = Now,
            ["CancelledBy"] = UserId,
            ["ModifiedAt"] = Now,
            ["ModifiedBy"] = UserId,
        });
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
    /// Returns a posting account identifier by account code.
    /// </summary>
    protected virtual string GetPostingAccountId(DbTransaction Transaction, string AccountCode)
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
    /// Returns the generated journal entry code for the current document.
    /// </summary>
    protected virtual string GetGeneratedJournalEntryCode() => $"JE-{CurrentRow.AsString("Code")}";
    /// <summary>
    /// Returns the journal entry generated by a source trade document.
    /// </summary>
    protected virtual DataRow GetSourceJournalEntry(DbTransaction Transaction)
    {
        string SourceId = CurrentRow.AsString("CancelsTradeId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return null;

        return Store.SelectResults(Transaction, """
                                                select *
                                                from JournalEntry
                                                where SourceTable = 'Trade'
                                                  and SourceId = :SourceId
                                                """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    /// <summary>
    /// Links a cancellation journal entry to the journal entry it reverses.
    /// </summary>
    protected virtual void LinkCancellationJournalEntry(DbTransaction Transaction, string JournalEntryId, DataRow SourceJournalEntry)
    {
        if (SourceJournalEntry == null)
            return;

        string SourceJournalEntryId = SourceJournalEntry.AsString("Id");
        Store.ExecSql(Transaction, """
                                   update JournalEntry
                                   set CancelledDocumentId = :CancelledDocumentId
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = JournalEntryId,
            ["CancelledDocumentId"] = SourceJournalEntryId,
        });
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
            ["Id"] = SourceJournalEntryId,
            ["StatusId"] = (int)TradeStatus.Cancelled,
            ["CancellationDocumentId"] = JournalEntryId,
            ["IsCancelled"] = true,
            ["CancelledAt"] = DateTime.UtcNow,
            ["CancelledBy"] = Sys.GetCurrentAppUserId(),
        });
    }
    /// <summary>
    /// Returns the finance movement generated by a source trade document.
    /// </summary>
    protected virtual DataRow GetSourceFinanceMovement(DbTransaction Transaction)
    {
        string SourceId = CurrentRow.AsString("CancelsTradeId");
        if (string.IsNullOrWhiteSpace(SourceId))
            return null;

        return Store.SelectResults(Transaction, """
                                                select *
                                                from FinanceMovement
                                                where SourceTable = 'Trade'
                                                  and SourceId = :SourceId
                                                """, new Dictionary<string, object>()
        {
            ["SourceId"] = SourceId,
        });
    }
    /// <summary>
    /// Links a cancellation finance movement to the finance movement it reverses.
    /// </summary>
    protected virtual void LinkCancellationFinanceMovement(DbTransaction Transaction, string FinanceMovementId, DataRow SourceFinanceMovement)
    {
        if (SourceFinanceMovement == null)
            return;

        Store.ExecSql(Transaction, """
                                   update FinanceMovement
                                   set CancellationMovementId = :CancellationMovementId
                                   where Id = :Id
                                   """, new Dictionary<string, object>()
        {
            ["Id"] = SourceFinanceMovement.AsString("Id"),
            ["CancellationMovementId"] = FinanceMovementId,
        });
    }
    /// <summary>
    /// Updates the finance balance for a person.
    /// </summary>
    protected virtual void UpdatePersonFinanceBalance(DbTransaction Transaction, string FinanceMovementId, DateTime MovementDate, int Direction, decimal Amount)
    {
        string PersonId = CurrentRow.AsString("PersonId");
        string CurrencyId = CurrentRow.AsString("CurrencyId");
        int TradeTypeId = CurrentRow.AsInteger("TradeTypeId");
        decimal SignedAmount = RoundAmount(Direction * Amount);
        DataRow BalanceKey = Store.SelectResults(Transaction, """
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
                ["PersonId"] = PersonId,
                ["CashAccountId"] = DBNull.Value,
                ["CompanyBankAccountId"] = DBNull.Value,
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
    /// Creates a finance movement and updates the related balance for the current trade document.
    /// </summary>
    protected virtual void CreateFinancialMovement(DbTransaction Transaction)
    {
        if (!DocumentType.AffectsFinancial)
            return;

        int ExistingCount = Store.IntegerResult(Transaction, """
                                                             select count(*)
                                                             from FinanceMovement
                                                             where SourceTable = 'Trade'
                                                               and SourceId = :SourceId
                                                             """, 0, new Dictionary<string, object>()
        {
            ["SourceId"] = CurrentRow.AsString("Id"),
        });
        if (ExistingCount > 0)
            throw new TripousBusinessException("A finance movement already exists for this document.");

        int Direction = DocumentType.FinancialDirection;
        if (Direction != 1 && Direction != -1)
            throw new TripousBusinessException("Invalid financial direction.");

        bool IsSales = DocumentType.TradeTypeId == (int)TradeType.Sales;
        bool IsPurchase = DocumentType.TradeTypeId == (int)TradeType.Purchases;
        if (!IsSales && !IsPurchase)
            return;

        string PersonId = CurrentRow.AsString("PersonId");
        if (string.IsNullOrWhiteSpace(PersonId))
            throw new TripousBusinessException("Finance movement person is required.");

        decimal Amount = RoundAmount(CurrentRow.AsDecimal("TotalAmount"));
        if (Amount <= 0)
            throw new TripousBusinessException("Finance movement amount must be greater than zero.");

        string FinanceMovementId = Sys.GenId();
        DateTime MovementDate = CurrentRow.AsDateTime("PostingDate", DateTime.Today);
        DataRow SourceFinanceMovement = GetSourceFinanceMovement(Transaction);
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
            ["TradeTypeId"] = CurrentRow.AsInteger("TradeTypeId"),
            ["MovementDate"] = MovementDate,
            ["PersonId"] = PersonId,
            ["CashAccountId"] = DBNull.Value,
            ["CompanyBankAccountId"] = DBNull.Value,
            ["Direction"] = Direction,
            ["Amount"] = Amount,
            ["CurrencyId"] = CurrentRow.AsString("CurrencyId"),
            ["ExchangeRate"] = CurrentRow.AsDecimal("ExchangeRate", 1),
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "Trade",
            ["SourceId"] = CurrentRow.AsString("Id"),
            ["CancelledMovementId"] = SourceFinanceMovement == null ? DBNull.Value : (object)SourceFinanceMovement.AsString("Id"),
            ["CancellationMovementId"] = DBNull.Value,
            ["DocumentTypeId"] = CurrentRow.AsString("DocumentTypeId"),
            ["DocumentCode"] = CurrentRow.AsString("Code"),
            ["DocumentDate"] = CurrentRow.AsDateTime("TradeDate", DateTime.Today),
            ["Remarks"] = $"Generated from {CurrentRow.AsString("Code")}",
            ["CreatedAt"] = DateTime.UtcNow,
            ["CreatedBy"] = Sys.GetCurrentAppUserId(),
        });
        LinkCancellationFinanceMovement(Transaction, FinanceMovementId, SourceFinanceMovement);
        UpdatePersonFinanceBalance(Transaction, FinanceMovementId, MovementDate, Direction, Amount);
    }
    /// <summary>
    /// Inserts a journal entry line.
    /// </summary>
    protected virtual void InsertJournalEntryLine(DbTransaction Transaction, string JournalEntryId, int DisplayOrder, string AccountId, decimal DebitAmount, decimal CreditAmount, string Remarks)
    {
        if (DebitAmount == 0 && CreditAmount == 0)
            return;

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
            ["SourceTable"] = "Trade",
            ["SourceId"] = CurrentRow.AsString("Id"),
        });
    }
    /// <summary>
    /// Creates a posted accounting journal entry for the current trade document.
    /// </summary>
    protected virtual void CreateAccountingJournalEntry(DbTransaction Transaction)
    {
        if (!DocumentType.AffectsAccounting)
            return;

        int ExistingCount = Store.IntegerResult(Transaction, """
                                                             select count(*)
                                                             from JournalEntry
                                                             where SourceTable = 'Trade'
                                                               and SourceId = :SourceId
                                                             """, 0, new Dictionary<string, object>()
        {
            ["SourceId"] = CurrentRow.AsString("Id"),
        });
        if (ExistingCount > 0)
            throw new TripousBusinessException("An accounting journal entry already exists for this document.");

        int Direction = DocumentType.AccountingDirection;
        if (Direction != 1 && Direction != -1)
            throw new TripousBusinessException("Invalid accounting direction.");

        bool IsSales = DocumentType.TradeTypeId == (int)TradeType.Sales;
        bool IsPurchase = DocumentType.TradeTypeId == (int)TradeType.Purchases;
        if (!IsSales && !IsPurchase)
            return;

        decimal NetAmount = RoundAmount(CurrentRow.AsDecimal("NetAmount"));
        decimal TaxAmount = RoundAmount(CurrentRow.AsDecimal("TaxAmount"));
        decimal TotalAmount = RoundAmount(CurrentRow.AsDecimal("TotalAmount"));
        if (TotalAmount <= 0)
            throw new TripousBusinessException("Accounting journal entry amount must be greater than zero.");

        string ReceivableAccountId = GetPostingAccountId(Transaction, "10-3000");
        string PayableAccountId = GetPostingAccountId(Transaction, "20-1000");
        string RevenueAccountId = GetPostingAccountId(Transaction, "70-1000");
        string PurchaseAccountId = GetPostingAccountId(Transaction, "60-1000");
        string VatPayableAccountId = GetPostingAccountId(Transaction, "20-2000");
        string VatReceivableAccountId = GetPostingAccountId(Transaction, "10-5000");
        string JournalEntryId = Sys.GenId();
        string UserId = Sys.GetCurrentAppUserId();
        DateTime Now = DateTime.UtcNow;
        DateTime EntryDate = CurrentRow.AsDateTime("PostingDate", DateTime.Today);
        DataRow SourceJournalEntry = GetSourceJournalEntry(Transaction);

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
            ["TotalDebit"] = TotalAmount,
            ["TotalCredit"] = TotalAmount,
            ["SourceModule"] = ModuleDef.Name,
            ["SourceTable"] = "Trade",
            ["SourceId"] = CurrentRow.AsString("Id"),
            ["DocumentTypeId"] = CurrentRow.AsString("DocumentTypeId"),
            ["DocumentCode"] = CurrentRow.AsString("Code"),
            ["DocumentDate"] = CurrentRow.AsDateTime("TradeDate", DateTime.Today),
            ["TradeTypeId"] = CurrentRow.AsInteger("TradeTypeId"),
            ["Remarks"] = $"Generated from {CurrentRow.AsString("Code")}",
            ["CancelledDocumentId"] = DBNull.Value,
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

        if (IsSales && Direction == 1)
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, ReceivableAccountId, TotalAmount, 0, "Customer receivable");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, RevenueAccountId, 0, NetAmount, "Sales revenue");
            InsertJournalEntryLine(Transaction, JournalEntryId, 30, VatPayableAccountId, 0, TaxAmount, "VAT payable");
        }
        else if (IsSales)
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, RevenueAccountId, NetAmount, 0, "Sales reversal");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, VatPayableAccountId, TaxAmount, 0, "VAT payable reversal");
            InsertJournalEntryLine(Transaction, JournalEntryId, 30, ReceivableAccountId, 0, TotalAmount, "Customer receivable reversal");
        }
        else if (Direction == -1)
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, PurchaseAccountId, NetAmount, 0, "Purchase expense");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, VatReceivableAccountId, TaxAmount, 0, "VAT receivable");
            InsertJournalEntryLine(Transaction, JournalEntryId, 30, PayableAccountId, 0, TotalAmount, "Supplier payable");
        }
        else
        {
            InsertJournalEntryLine(Transaction, JournalEntryId, 10, PayableAccountId, TotalAmount, 0, "Supplier payable reversal");
            InsertJournalEntryLine(Transaction, JournalEntryId, 20, PurchaseAccountId, 0, NetAmount, "Purchase reversal");
            InsertJournalEntryLine(Transaction, JournalEntryId, 30, VatReceivableAccountId, 0, TaxAmount, "VAT receivable reversal");
        }

        LinkCancellationJournalEntry(Transaction, JournalEntryId, SourceJournalEntry);
    }
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// </summary>
    protected override void SetDefaultValues(MemTable Table, DataRow Row, TableDef TableDef)
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
        else if (IsTradeLineTable(Table)
                 && CurrentRow != null
                 && tblItem.ContainsColumn("WarehouseId")
                 && Table.ContainsColumn("WarehouseId")
                 && string.IsNullOrWhiteSpace(Row.AsString("WarehouseId")))
        {
            Row.SetValue("WarehouseId", CurrentRow["WarehouseId"]);
        }
    }
    protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs ea)
    {
        base.NewRowAdded(Table, ea);

        if (Table == tblItem || IsTransforming || CurrentRow == null || !tblItem.ContainsColumn("WarehouseId") || !Table.ContainsColumn("WarehouseId"))
            return;

        ea.Row.SetValue("WarehouseId", CurrentRow["WarehouseId"]);
    }
    /// <summary>
    /// Returns true when a changed line field should resolve a new price from the current price list.
    /// </summary>
    protected virtual bool ShouldResolveLinePrice(DataRow Row, string FieldName)
    {
        if (Row != null
            && FieldName.IsSameText("Quantity")
            && Row.Table.Columns.Contains("SourceTradeLineId")
            && !string.IsNullOrWhiteSpace(Row.AsString("SourceTradeLineId")))
        {
            return false;
        }

        return true;
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
                if (IsPriceLineField && ShouldResolveLinePrice(ea.Row, FieldName))
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
    /// <summary>
    /// Applies server-side side effects for a web JSON calculation field change.
    /// </summary>
    protected virtual void ApplyJsonCalculateFieldChange(string TableName, string FieldName)
    {
        if (string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(FieldName))
            return;

        MemTable Table = FindTable(TableName);
        if (Table == null || !Table.ContainsColumn(FieldName) || Table.Rows.Count == 0)
            return;

        DataRow Row = Table == tblItem && CurrentRow != null
            ? CurrentRow
            : Table.Rows.Cast<DataRow>().FirstOrDefault(item => item.RowState != DataRowState.Deleted);

        if (Row == null || fCalculationLevel > 0 || IsTransforming || IsCopyingPersonAddresses || !State.In(DataMode.Insert | DataMode.Edit))
            return;

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
                Row.SetValue("TaxBusinessGroupId", LoadTaxBusinessGroupId(Row.AsString("PersonId")));
            if (Table == tblItem && FieldName.IsSameText("BranchId"))
                Row.SetValue("OriginTaxJurisdictionId", DBNull.Value);
            if (Table == tblItem && (FieldName.StartsWith("Billing", StringComparison.OrdinalIgnoreCase)
                                     || FieldName.StartsWith("Shipping", StringComparison.OrdinalIgnoreCase)))
                Row.SetValue("DestinationTaxJurisdictionId", DBNull.Value);

            if (IsTradeLineTable(Table) && IsLineField)
            {
                if (IsPriceLineField && ShouldResolveLinePrice(Row, FieldName))
                    ResolveLinePrice(Row);
                Calculate(Row, FieldName, "DiscountPercent");
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
    protected override void TableSet_TransactionStageCommit(object sender, TransactionEventArgs e)
    {
        base.TableSet_TransactionStageCommit(sender, e);

        if (IsPosting && e.Stage == TransactionStage.Post && e.ExecTime == ExecTime.After)
        {
            CreateAccountingJournalEntry(e.Transaction);
            CreateFinancialMovement(e.Transaction);
        }
    }
    // ● construction
    public TradeDataModule()
    {
    }

    // ● public
    /// <summary>
    /// Applies a JSON contract object, recalculates commercial values, and returns this data module as a JSON contract object.
    /// </summary>
    public virtual JsonDataModule JsonCalculate(JsonDataModule Source)
    {
        return JsonCalculate(Source, string.Empty, string.Empty);
    }
    /// <summary>
    /// Applies a JSON contract object, applies the specified field change, recalculates commercial values, and returns this data module as a JSON contract object.
    /// </summary>
    public virtual JsonDataModule JsonCalculate(JsonDataModule Source, string TableName, string FieldName)
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
        ApplyJsonCalculateFieldChange(TableName, FieldName);
        ResolvePrices();
        Calculate();

        return new JsonDataModule(this);
    }
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
    /// <summary>
    /// Returns true when the persisted document has at least one credited line.
    /// </summary>
    public virtual bool HasCreditedQuantity()
    {
        if (CurrentRow == null)
            return false;

        int Count = Store.IntegerResult("""
                                        select count(*)
                                        from TradeLine
                                        where TradeId = :TradeId
                                          and CreditedQuantity > 0
                                        """, 0, new Dictionary<string, object>()
        {
            ["TradeId"] = CurrentRow.AsString("Id"),
        });
        return Count > 0;
    }
}
