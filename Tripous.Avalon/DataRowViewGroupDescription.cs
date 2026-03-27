using System;
using System.Data;
using System.Globalization;
using Avalonia.Collections;

namespace Tripous.Avalon;

public class DataRowViewGroupDescription : DataGridGroupDescription
{
    public DataRowViewGroupDescription(string ColumnName)
    {
        this.ColumnName = ColumnName;
    }

    public string ColumnName { get; }

    public override string PropertyName => ColumnName;

    public override object GroupKeyFromItem(object item, int level, CultureInfo culture)
    {
        if (item is DataRowView RowView)
        {
            if (RowView.DataView.Table.Columns.Contains(ColumnName))
            {
                object Value = RowView[ColumnName];
                return Value == DBNull.Value ? null : Value;
            }
        }

        return item;
    }
}