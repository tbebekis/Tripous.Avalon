/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

public class TradeDataModule: DocumentDataModule
{
    // ● protected
    protected virtual int GetDefaultTaxTreatmentId() => (int)TaxTreatment.Normal;
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// </summary>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table.TableName.IsSameText("Trade"))
        {
            if (Sys.IsNull(Row["DocumentTypeId"]))
                Row["DocumentTypeId"] = DocumentType.Id;
            if (Sys.IsNull(Row["TradeStatusId"]))
                Row["TradeStatusId"] = (int)TradeStatus.Draft;
            if (Sys.IsNull(Row["TaxTreatmentId"]))
                Row["TaxTreatmentId"] = GetDefaultTaxTreatmentId();
            if (Sys.IsNull(Row["TradeDate"]))
                Row["TradeDate"] = DateTime.Today;

            if (Sys.IsNull(Row["ExchangeRate"]))
                Row["ExchangeRate"] = 1;
        }
    }
    
    // ● construction
    public TradeDataModule()
    {
    }
}
