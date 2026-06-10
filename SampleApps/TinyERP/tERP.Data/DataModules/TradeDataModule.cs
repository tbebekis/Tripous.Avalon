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
    protected int fCalculationLevel;
    protected bool IsCopyingPersonAddresses = false;
    
    // ● protected
    protected virtual decimal RoundAmount(decimal Value) => Math.Round(Value, 4, MidpointRounding.AwayFromZero);
    protected virtual bool IsTradeLineTable(MemTable Table)
    {
        return Table != null
               && Table.TableName.IsSameText("TradeLine")
               && Table.ContainsColumn("GrossAmount")
               && Table.ContainsColumn("DiscountPercent")
               && Table.ContainsColumn("DiscountAmount")
               && Table.ContainsColumn("NetAmount")
               && Table.ContainsColumn("VatAmount")
               && Table.ContainsColumn("TotalAmount");
    }
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
        decimal VatAmount = RoundAmount(NetAmount * Row.AsDecimal("VatRatePercent") / 100);

        Row.SetValue("PrimaryUnitQuantity", RoundAmount(Quantity * UnitRatio));
        Row.SetValue("GrossAmount", GrossAmount);
        Row.SetValue("DiscountPercent", DiscountPercent);
        Row.SetValue("DiscountAmount", DiscountAmount);
        Row.SetValue("NetUnitPrice", NetUnitPrice);
        Row.SetValue("NetAmount", NetAmount);
        Row.SetValue("VatAmount", VatAmount);
        Row.SetValue("TotalAmount", RoundAmount(NetAmount + VatAmount));
    }
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

        decimal VatAmount = TradeLineTable != null
            ? TradeLineTable.Rows.Cast<DataRow>()
                .Where(Row => Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                .Sum(Row => Row.AsDecimal("VatAmount"))
            : 0;
        decimal NetAmount = RoundAmount(LinesAmount - DiscountAmount + CurrentRow.AsDecimal("ChargesAmount"));

        CurrentRow.SetValue("LinesAmount", RoundAmount(LinesAmount));
        CurrentRow.SetValue("DiscountPercent", DiscountPercent);
        CurrentRow.SetValue("DiscountAmount", DiscountAmount);
        CurrentRow.SetValue("NetAmount", NetAmount);
        CurrentRow.SetValue("VatAmount", RoundAmount(VatAmount));
        CurrentRow.SetValue("TotalAmount", RoundAmount(NetAmount + VatAmount));
    }
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

            CalculateTotals("DiscountPercent");
        }
        finally
        {
            fCalculationLevel--;
        }
    }
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
            Row.SetValue("TaxTreatmentId", (int)TaxTreatment.Normal);
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

        if (fCalculationLevel > 0 || !State.In(DataMode.Insert | DataMode.Edit))
            return;

        string FieldName = ea.Column.ColumnName;
        bool IsLineField = FieldName.IsSameText("Quantity")
                           || FieldName.IsSameText("UnitRatio")
                           || FieldName.IsSameText("UnitPrice")
                           || FieldName.IsSameText("DiscountPercent")
                           || FieldName.IsSameText("DiscountAmount")
                           || FieldName.IsSameText("VatRatePercent");
        bool IsHeaderField = FieldName.IsSameText("DiscountPercent")
                             || FieldName.IsSameText("DiscountAmount")
                             || FieldName.IsSameText("ChargesAmount");

        fCalculationLevel++;
        try
        {
            if (IsTradeLineTable(Table) && IsLineField)
            {
                CalculateLine(ea.Row, FieldName);
                CalculateTotals("DiscountPercent");
            }
            else if (Table == tblItem && IsHeaderField)
            {
                CalculateTotals(FieldName);
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
            foreach (MemTable Table in ItemTables.Where(IsTradeLineTable))
                Table.RowDeleted += TradeLine_RowDeleted;
        }
    }
}
