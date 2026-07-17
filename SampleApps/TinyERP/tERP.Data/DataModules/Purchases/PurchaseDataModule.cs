/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Base data module for purchase documents.
/// </summary>
public class PurchaseDataModule: TradeDataModule
{
    // ● protected
    /// <summary>
    /// Returns a configured default identifier, or a fallback default identifier.
    /// </summary>
    protected virtual string GetDefaultId(string ConfigValue, Func<string> DefaultProvider) => !string.IsNullOrWhiteSpace(ConfigValue) ? ConfigValue : DefaultProvider();
    /// <summary>
    /// Validates a purchase document line.
    /// </summary>
    protected override void ValidateLine(DataRow Row, List<string> Errors)
    {
        base.ValidateLine(Row, Errors);

        string LineLabel = GetLineLabel(Row);
        if (!AppDefaultProperties.Purchase.AllowZeroUnitPrice && Row.AsDecimal("UnitPrice") == 0)
            Errors.Add($"{LineLabel}: Unit price must be greater than zero.");
    }
    /// <summary>
    /// Copies common column values except for the specified fields.
    /// </summary>
    protected virtual void CopyCommonValues(DataRow Source, DataRow Dest, IEnumerable<string> ExcludedFields)
    {
        HashSet<string> Excluded = new(ExcludedFields, StringComparer.OrdinalIgnoreCase);

        foreach (DataColumn Column in Dest.Table.Columns)
        {
            if (!Excluded.Contains(Column.ColumnName) && Source.Table.Columns.Contains(Column.ColumnName))
                Dest.SetValue(Column.ColumnName, Source[Column.ColumnName]);
        }
    }
    /// <summary>
    /// Creates an unsaved purchase document from the current document.
    /// </summary>
    protected virtual PurchaseDataModule CreateTransformedDocument(string TargetModuleName, string SourceDocumentName, string SourceQuantityFieldName = "ExecutedQuantity")
    {
        if (CurrentRow == null)
            throw new TripousBusinessException($"No {SourceDocumentName} is selected.");

        PurchaseDataModule SourceModule = DataRegistry.CreateModule(ModuleDef.Name) as PurchaseDataModule;
        if (SourceModule == null)
            throw new TripousDataException($"Cannot create source module '{ModuleDef.Name}'.");
        SourceModule.Edit(CurrentRow["Id"]);

        DataRow SourceDocument = SourceModule.CurrentRow;
        if ((TradeStatus)SourceDocument.AsInteger("TradeStatusId") != TradeStatus.Posted)
            throw new TripousBusinessException($"Only posted {SourceDocumentName}s can be transformed.");
        if (SourceDocument.AsBoolean("IsCancelled"))
            throw new TripousBusinessException($"A cancelled {SourceDocumentName} cannot be transformed.");

        MemTable SourceLineTable = SourceModule.GetTable("TradeLine");
        PurchaseDataModule Result = DataRegistry.CreateModule(TargetModuleName) as PurchaseDataModule;
        if (Result == null)
            throw new TripousDataException($"Module '{TargetModuleName}' is not a purchase document module.");

        Result.IsTransforming = true;
        try
        {
            Result.Insert();

            string[] HeaderExcludedFields =
            [
                "Id", "DocumentTypeId", "Code", "TradeStatusId", "TradeDate", "PostingDate",
                "SourceId", "CancelsTradeId", "CancelledByTradeId",
                "LinesAmount", "NetAmount", "TaxAmount", "TotalAmount",
                "IsLocked", "IsCancelled",
                "CreatedAt", "CreatedBy", "ModifiedAt", "ModifiedBy",
                "PostedAt", "PostedBy", "CancelledAt", "CancelledBy"
            ];
            CopyCommonValues(SourceDocument, Result.CurrentRow, HeaderExcludedFields);
            Result.CurrentRow.SetValue("SourceId", SourceDocument["Id"]);

            MemTable TargetLineTable = Result.FindItemTable("TradeLine");
            if (TargetLineTable == null)
                throw new TripousDataException("TradeLine table is not available.");

            string[] LineExcludedFields =
            [
                "Id", "TradeId", "Quantity", "PrimaryUnitQuantity",
                "ReservedQuantity", "ExecutedQuantity", "InvoicedQuantity", "CreditedQuantity",
                "GrossAmount", "DiscountAmount", "NetUnitPrice", "NetAmount",
                "DocumentDiscountAmount", "TaxAmount", "TotalAmount",
                "TaxPercent", "IsTaxExempt", "IsReverseCharge",
                "SourceTradeLineId"
            ];

            foreach (DataRow SourceLine in SourceLineTable.Rows)
            {
                if (SourceLine.RowState == DataRowState.Deleted || SourceLine.RowState == DataRowState.Detached)
                    continue;

                decimal Quantity = SourceLine.AsDecimal("Quantity") - SourceLine.AsDecimal(SourceQuantityFieldName);
                if (Quantity <= 0)
                    continue;

                DataRow TargetLine = TargetLineTable.AddNewRow();
                CopyCommonValues(SourceLine, TargetLine, LineExcludedFields);
                TargetLine.SetValue("Quantity", Quantity);
                TargetLine.SetValue("DiscountPercent", SourceLine.AsDecimal("DiscountPercent"));
                TargetLine.SetValue("SourceTradeLineId", SourceLine["Id"]);
            }

            if (TargetLineTable.Rows.Count == 0)
                throw new TripousBusinessException("The source document has no remaining quantity to transform.");

            Result.Calculate(null, "DiscountPercent", "DiscountPercent");
            Result.CopyTaxContext(SourceDocument, Result.CurrentRow);
            Result.Validate();
            return Result;
        }
        catch
        {
            Result.Cancel();
            throw;
        }
        finally
        {
            Result.IsTransforming = false;
        }
    }
    protected override void SetDefaultValues(MemTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("WarehouseId", GetDefaultId(AppDefaultProperties.Purchase.WarehouseId, DataLib.GetDefaultWarehouseId));
            Row.SetValue("CostCenterId", GetDefaultId(AppDefaultProperties.Purchase.CostCenterId, DataLib.GetDefaultPurchaseCostCenterId));
            Row.SetValue("BranchId", GetDefaultId(AppDefaultProperties.Purchase.BranchId, DataLib.GetDefaultBranchId));
            Row.SetValue("PriceListTypeId", GetDefaultId(AppDefaultProperties.Purchase.PriceListTypeId, DataLib.GetDefaultPriceListTypeId));
            Row.SetValue("CurrencyId", GetDefaultId(AppDefaultProperties.Purchase.CurrencyId, DataLib.GetDefaultCurrencyId));
            Row.SetValue("PaymentMethodId", GetDefaultId(AppDefaultProperties.Purchase.PaymentMethodId, DataLib.GetDefaultPaymentMethodId));
            Row.SetValue("PaymentTermId", GetDefaultId(AppDefaultProperties.Purchase.PaymentTermId, DataLib.GetDefaultPaymentTermId));
            Row.SetValue("TaxBusinessGroupId", GetDefaultId(AppDefaultProperties.Purchase.TaxBusinessGroupId, DataLib.GetDefaultTaxBusinessGroupId));
            Row.SetValue("OriginTaxJurisdictionId", GetDefaultId(AppDefaultProperties.Purchase.OriginTaxJurisdictionId, DataLib.GetDefaultTaxJurisdictionId));
            Row.SetValue("DestinationTaxJurisdictionId", GetDefaultId(AppDefaultProperties.Purchase.DestinationTaxJurisdictionId, DataLib.GetDefaultTaxJurisdictionId));
        }
    }
    protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs ea)
    {
        base.NewRowAdded(Table, ea);

        if (!IsTransforming && IsTradeLineTable(Table))
            ea.Row.SetValue("Quantity", AppDefaultProperties.Purchase.DefaultQuantity);
    }
    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs ea)
    {
        base.ColumnChanged(Table, ea);
        if (!IsCopyingPersonAddresses
            && Table == tblItem
            && !IsTransforming
            && State.In(DataMode.Insert | DataMode.Edit)
            && "PersonId".IsSameText(ea.Column.ColumnName))
        {
            CopyPersonAddresses(ea.Row);
        }
    }
    protected virtual void CopyPersonAddresses(DataRow Row)
    {
        List<PersonAddress> AddressList = DataLib.LoadPersonAddressList(Row.AsString("PersonId"));

        PersonAddress BillingAddress = AddressList.FirstOrDefault(x => x.AddressType == AddressType.Billing && x.IsDefault)
                                       ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Billing)
                                       ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main && x.IsDefault)
                                       ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main)
                                       ?? AddressList.FirstOrDefault(x => x.IsDefault)
                                       ?? AddressList.FirstOrDefault();

        PersonAddress ShippingAddress = AddressList.FirstOrDefault(x => x.AddressType == AddressType.Shipping && x.IsDefault)
                                        ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Shipping)
                                        ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main && x.IsDefault)
                                        ?? AddressList.FirstOrDefault(x => x.AddressType == AddressType.Main)
                                        ?? AddressList.FirstOrDefault(x => x.IsDefault)
                                        ?? AddressList.FirstOrDefault();

        IsCopyingPersonAddresses = true;
        try
        {
            Row.SetValue("BillingName", BillingAddress != null ? BillingAddress.Name : DBNull.Value);
            Row.SetValue("BillingAddressLine1", BillingAddress != null ? BillingAddress.AddressLine1 : DBNull.Value);
            Row.SetValue("BillingAddressLine2", BillingAddress != null ? BillingAddress.AddressLine2 : DBNull.Value);
            Row.SetValue("BillingCity", BillingAddress != null ? BillingAddress.City : DBNull.Value);
            Row.SetValue("BillingRegion", BillingAddress != null ? BillingAddress.Region : DBNull.Value);
            Row.SetValue("BillingPostalCode", BillingAddress != null ? BillingAddress.PostalCode : DBNull.Value);
            Row.SetValue("BillingCountryId", BillingAddress != null ? BillingAddress.CountryId : DBNull.Value);

            Row.SetValue("ShippingName", ShippingAddress != null ? ShippingAddress.Name : DBNull.Value);
            Row.SetValue("ShippingAddressLine1", ShippingAddress != null ? ShippingAddress.AddressLine1 : DBNull.Value);
            Row.SetValue("ShippingAddressLine2", ShippingAddress != null ? ShippingAddress.AddressLine2 : DBNull.Value);
            Row.SetValue("ShippingCity", ShippingAddress != null ? ShippingAddress.City : DBNull.Value);
            Row.SetValue("ShippingRegion", ShippingAddress != null ? ShippingAddress.Region : DBNull.Value);
            Row.SetValue("ShippingPostalCode", ShippingAddress != null ? ShippingAddress.PostalCode : DBNull.Value);
            Row.SetValue("ShippingCountryId", ShippingAddress != null ? ShippingAddress.CountryId : DBNull.Value);
        }
        finally
        {
            IsCopyingPersonAddresses = false;
        }

        ResolvePrices();
        Calculate();
    }

    // ● construction
    public PurchaseDataModule()
    {
    }
}
