using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Avalonia.Controls.DataGridPivoting;

namespace PivotTestApp;

static public partial class Tests
{
    /// <summary>
    /// Converts a Tripous <see cref="PivotValueAggregateType"/> to a ProDataGrid <see cref="PivotAggregateType"/>
    /// </summary>
    static public PivotAggregateType ToPivotAggregateType(this PivotValueAggregateType AggregateType)
    {
        return AggregateType switch
        {
            PivotValueAggregateType.Sum => PivotAggregateType.Sum,
            PivotValueAggregateType.Avg => PivotAggregateType.Average,
            PivotValueAggregateType.Count => PivotAggregateType.Count,
            PivotValueAggregateType.Min => PivotAggregateType.Min,
            PivotValueAggregateType.Max => PivotAggregateType.Max,
            PivotValueAggregateType.StdDev => PivotAggregateType.StdDev,
            PivotValueAggregateType.StdDevP => PivotAggregateType.StdDevP,
            PivotValueAggregateType.Variance => PivotAggregateType.Variance,
            PivotValueAggregateType.VarianceP => PivotAggregateType.VarianceP,
            PivotValueAggregateType.CountDistinct => PivotAggregateType.CountDistinct,
            PivotValueAggregateType.Product => PivotAggregateType.Product,
            _ => PivotAggregateType.Count,
        };
    }
    
   
    
    static public PivotDef CreateDefaultPivotDef()
    {
        PivotDef Def = new();

        Def.RowFields.AddRange(new string[] {"Region", "Category"});
        Def.ColumnFields.AddRange(new string[] {"Year", "Segment"});
        Def.ValueFields.AddRange(new PivotValueDef[]
        {
            new PivotValueDef() { FieldName = "Sales", AggregateType = PivotValueAggregateType.Sum, Format = "C0"},
            new PivotValueDef() { FieldName = "Profit", AggregateType = PivotValueAggregateType.Sum, Format = "C0"},
            new PivotValueDef() { FieldName = "Quantity", AggregateType = PivotValueAggregateType.Sum, Format = "C0"},
        });
        return Def;
    }
    
    /*
    static public object GetPropertyValue(object Instance, PropertyInfo Prop)
    {
        if (Instance == null || Prop == null)
            return null;
 
        Type type = Instance.GetType();
        return Prop?.GetValue(Instance);
    }

    static public object GetColumnValue(DataRowView RowView, string FieldName)
    {
        object Result = null;

        if (RowView != null && RowView.DataView.Table.Columns.Contains(FieldName))
        {
            Result  = RowView[FieldName];
            if (Result == DBNull.Value)
                Result = null;
        }

        return Result;
    }
    */
}