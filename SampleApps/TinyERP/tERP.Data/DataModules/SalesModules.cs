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