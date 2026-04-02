using System;
using System.Collections;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.DataGridPivoting;

namespace PivotTestApp;

static public partial class Tests
{
    static public void PocoTest(DataGrid Grid, PivotDef Def, Type PocoType, IEnumerable ItemsSource)
    {
        PivotTableModel Model = new PivotTableModel();
        
        Model.BeginUpdate();
        try
        {
            Model.ItemsSource = ItemsSource;
            UpdatePivotPocoModel(Def, Model, PocoType);
            
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
    }
    static public void UpdatePivotPocoModel(PivotDef PivotDef, PivotTableModel Model, Type PocoType)
    {
        object GetPropertyValue(object Instance, PropertyInfo Prop)
        {
            if (Instance == null || Prop == null)
                return null;
 
            Type type = Instance.GetType();
            return Prop?.GetValue(Instance);
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
                    ValueSelector = Item => GetPropertyValue(Item, PocoType.GetProperty(FieldName)) 
                });
            }

            foreach (string FieldName in PivotDef.ColumnFields)
            {
                Model.ColumnFields.Add(new PivotAxisField()
                {
                    Header = FieldName,
                     ValueSelector = Item => GetPropertyValue(Item, PocoType.GetProperty(FieldName))  
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
                    ValueSelector = Item => GetPropertyValue(Item, PocoType.GetProperty(ValueDef.FieldName)), //((DataRowView)Item)[ValueDef.FieldName],
                    AggregateType = ToPivotAggregateType(ValueDef.AggregateType),
                    StringFormat = "C" //ValueDef.Format
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