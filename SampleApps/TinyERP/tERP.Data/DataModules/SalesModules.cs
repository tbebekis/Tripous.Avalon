/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;
 
public class SalesDataModule: TradeDataModule
{
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
            Row.SetValue("WarehouseId", AppDefaultProperties.Sales.WarehouseId);
            Row.SetValue("CostCenterId", AppDefaultProperties.Sales.CostCenterId);
            Row.SetValue("BranchId", AppDefaultProperties.Sales.BranchId);
            Row.SetValue("CurrencyId", AppDefaultProperties.Sales.CurrencyId);
            Row.SetValue("PaymentMethodId", AppDefaultProperties.Sales.PaymentMethodId);
            Row.SetValue("PaymentTermId", AppDefaultProperties.Sales.PaymentTermId);
        }
    }

    protected override void ColumnChanged(MemTable Table, DataColumnChangeEventArgs ea)
    {
        base.ColumnChanged(Table, ea);
        if (!IsCopyingPersonAddresses 
            &&Table == tblItem 
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
            Row.SetValue("BillingPostalCode", BillingAddress != null ? BillingAddress.PostalCode : DBNull.Value);
            Row.SetValue("BillingCountryId", BillingAddress != null ? BillingAddress.CountryId : DBNull.Value);

            Row.SetValue("ShippingName", ShippingAddress != null ? ShippingAddress.Name : DBNull.Value);
            Row.SetValue("ShippingAddressLine1", ShippingAddress != null ? ShippingAddress.AddressLine1 : DBNull.Value);
            Row.SetValue("ShippingAddressLine2", ShippingAddress != null ? ShippingAddress.AddressLine2 : DBNull.Value);
            Row.SetValue("ShippingCity", ShippingAddress != null ? ShippingAddress.City : DBNull.Value);
            Row.SetValue("ShippingPostalCode", ShippingAddress != null ? ShippingAddress.PostalCode : DBNull.Value);
            Row.SetValue("ShippingCountryId", ShippingAddress != null ? ShippingAddress.CountryId : DBNull.Value);
        }
        finally
        {
            IsCopyingPersonAddresses = false;
        }
    }

    // ● construction
    public SalesDataModule()
    {
    }
}

public class SalesOrderDataModule: SalesDataModule
{
    // ● construction
    public SalesOrderDataModule()
    {
    }
}

public class SalesDeliveryNoteDataModule: SalesDataModule
{
    // ● construction
    public SalesDeliveryNoteDataModule()
    {
    }
}

public class SalesInvoiceDataModule: SalesDataModule
{
    // ● construction
    public SalesInvoiceDataModule()
    {
    }
}

public class SalesCreditNoteDataModule: SalesDataModule
{
    // ● construction
    public SalesCreditNoteDataModule()
    {
    }
}

public class SalesReturnDataModule: SalesDataModule
{
    // ● construction
    public SalesReturnDataModule()
    {
    }
}

public class SalesCancellationDataModule: SalesDataModule
{
    // ● construction
    public SalesCancellationDataModule()
    {
    }
}
