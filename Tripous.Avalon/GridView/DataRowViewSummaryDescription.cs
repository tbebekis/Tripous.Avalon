namespace Tripous.Avalon;

public class DataRowViewSummaryDescription : DataGridSummaryDescription
{
    public DataRowViewSummaryDescription(DataGridAggregateType AggregateType, string ColumnName)
    {
        this.MyAggregateType = AggregateType;
        this.ColumnName = ColumnName;
    }

    public string ColumnName { get; }
    public DataGridAggregateType MyAggregateType { get; }

    public override DataGridAggregateType AggregateType => MyAggregateType;

    public override object Calculate(IEnumerable items, DataGridColumn column)
    {
        List<object> Values = new();

        foreach (object Item in items)
        {
            if (Item is DataRowView RowView)
            {
                object V = RowView[ColumnName];
                if (V != DBNull.Value)
                    Values.Add(V);
            }
        }

        switch (MyAggregateType)
        {
            case DataGridAggregateType.Count:
                return Values.Count;

            case DataGridAggregateType.Sum:
                return Values.Count > 0
                    ? Values.Cast<IConvertible>().Sum(x => Convert.ToDecimal(x))
                    : null;

            case DataGridAggregateType.Min:
                return Values.Count > 0
                    ? Values.Cast<IComparable>().Min()
                    : null;

            case DataGridAggregateType.Max:
                return Values.Count > 0
                    ? Values.Cast<IComparable>().Max()
                    : null;

            case DataGridAggregateType.Average:
                return Values.Count > 0
                    ? Values.Cast<IConvertible>().Average(x => Convert.ToDecimal(x))
                    : null;
        }

        return null;
    }
}