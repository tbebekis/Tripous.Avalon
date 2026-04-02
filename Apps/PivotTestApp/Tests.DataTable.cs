using System;
using System.Data;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Controls.DataGridPivoting;

namespace PivotTestApp;

static public partial class Tests
{
    static public void DataTableTest(DataGrid Grid, PivotDef Def, DataView DataView)
    {
        PivotTableModel Model = new PivotTableModel();
        
        Model.BeginUpdate();
        try
        {
            Model.ItemsSource = DataView;
            Model.Culture = CultureInfo.CurrentCulture;
            UpdatePivotDataTableModel(Def, Model, DataView);
             
            Grid.AutoGenerateColumns = false;
            Grid.ItemsSource = Model.Rows; 
            Grid.ColumnDefinitionsSource = Model.ColumnDefinitions;
        
            Grid.IsReadOnly = true;
            Grid.HeadersVisibility = DataGridHeadersVisibility.All;
            Grid.CanUserResizeColumns = true;
            Grid.CanUserReorderColumns = true;
        }
        finally
        {
            Model.EndUpdate();
        }

        // <<<<<<<<<<<<<< ΕΔΩ έχουν δημιουργηθεί
        // υπάρχει μόνο μία DataGridTemplateColumnDefinition
        // και όλες οι άλλες στήλες είναι DataGridNumericColumnDefinition
 
        NumberFormatInfo NFI = CultureInfo.CurrentCulture.NumberFormat;
        
        foreach (var ColDef in Model.ColumnDefinitions)
        {
            if (ColDef is DataGridNumericColumnDefinition NumCol)
            {
                NumCol.NumberFormat = NFI;
                NumCol.FormatString = "N2";
            }
        }


        var x = Grid.Columns;

    }
    static public void DataTableTest_01(DataGrid Grid, PivotDef Def, DataView DataView)
    {
        PivotTableModel Model = new PivotTableModel();

        Model.BeginUpdate();
        try
        {
            Model.ItemsSource = DataView;
            Model.Culture = CultureInfo.CurrentCulture;
            UpdatePivotDataTableModel(Def, Model, DataView);
        }
        finally
        {
            Model.EndUpdate();
        }

        NumberFormatInfo NFI = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();

        foreach (var ColDef in Model.ColumnDefinitions)
        {
            if (ColDef is DataGridNumericColumnDefinition NumCol)
            {
                NumCol.NumberFormat = NFI;
                NumCol.FormatString = "N2";
            }
        }

        Grid.AutoGenerateColumns = false;
        Grid.ItemsSource = Model.Rows;
        Grid.ColumnDefinitionsSource = Model.ColumnDefinitions;

        Grid.IsReadOnly = true;
        Grid.HeadersVisibility = DataGridHeadersVisibility.All;
        Grid.CanUserResizeColumns = true;
        Grid.CanUserReorderColumns = true;
    }
    
    static public void UpdatePivotDataTableModel(PivotDef PivotDef, PivotTableModel Model, DataView DataView)
    {
        object GetColumnValue(DataRowView RowView, string FieldName)
        {
            object Result = null;

            if (RowView != null && RowView.DataView.Table.Columns.Contains(FieldName))
            {
                Result  = RowView[FieldName];
                if (Result == DBNull.Value)
                    Result = null;
            }

            if (Result != null)
            {
                if (Result is double d)
                    Result = Math.Round(d, 2);

                else if (Result is float f)
                    Result = Math.Round((decimal)f, 2);
            }

            return Result;
        }
        
        Model.BeginUpdate();
        try
        {
            Model.RowFields.Clear();
            Model.ColumnFields.Clear();
            Model.ValueFields.Clear();
            
            foreach (string FieldName in PivotDef.RowFields)
            {
                Model.RowFields.Add(new PivotAxisField()
                {
                    Header = FieldName,
                    ValueSelector = Item => GetColumnValue(Item as DataRowView, FieldName),
      
                });
            }

            foreach (string FieldName in PivotDef.ColumnFields)
            {
                Model.ColumnFields.Add(new PivotAxisField()
                {
                    Header = FieldName,
                     ValueSelector = Item => GetColumnValue(Item as DataRowView, FieldName),
    
                });
            }

            foreach (PivotValueDef ValueDef in PivotDef.ValueFields)
            {
                string Header = !string.IsNullOrWhiteSpace(ValueDef.Caption)
                    ? ValueDef.Caption
                    : $"{ValueDef.AggregateType}({ValueDef.FieldName})";

                Model.ValueFields.Add(new PivotValueField()
                {
                    Header = Header,
                    ValueSelector = Item => GetColumnValue(Item as DataRowView, ValueDef.FieldName), 
                    AggregateType = ToPivotAggregateType(ValueDef.AggregateType),
                    StringFormat = "C2" //ValueDef.Format  
                });
            }

            Model.Layout.ShowRowSubtotals = PivotDef.ShowSubtotals;
            Model.Layout.ShowColumnSubtotals = PivotDef.ShowSubtotals;
            Model.Layout.ShowRowGrandTotals = PivotDef.ShowGrandTotals;
            Model.Layout.ShowColumnGrandTotals = PivotDef.ShowGrandTotals;
            Model.Layout.ValuesPosition = PivotDef.ShowValuesOnRows
                ? PivotValuesPosition.Rows
                : PivotValuesPosition.Columns;
        }
        finally
        {
            Model.EndUpdate();
        }
    }
}