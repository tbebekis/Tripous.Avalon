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
            Row.SetValue("TaxTreatmentId", (int)TaxTreatment.Normal);
            Row.SetValue("ExchangeRate", 1);
            Row.SetValue("TradeDate", DateTime.UtcNow.Date);
        }
    }
    
    // ● construction
    public TradeDataModule()
    {
    }
}
