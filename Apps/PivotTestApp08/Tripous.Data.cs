using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Tripous.Data;

/// <summary>
/// Specifies pivot value aggregate type.
/// </summary>
public enum PivotValueAggregateType
{
    None,
    Sum,
    Avg,
    Count,
    Min,
    Max,
    StdDev,
    StdDevP,
    Variance,
    VarianceP,
    CountDistinct,
    Product
}

/// <summary>
/// Specifies pivot axis.
/// </summary>
public enum PivotAxis
{
    None,
    Row,
    Column
}

/// <summary>
/// Specifies pivot data column kind.
/// </summary>
public enum PivotDataColumnKind
{
    None,
    RowHeader,
    Value
}

/// <summary>
/// Specifies pivot data row type.
/// </summary>
public enum PivotDataRowType
{
    Normal,
    Subtotal,
    GrandTotal
}

/// <summary>
/// Pivot column definition.
/// </summary>
public class PivotColumnDef
{
    private string fCaption;

    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// Gets or sets the axis.
    /// </summary>
    public PivotAxis Axis { get; set; }
    /// <summary>
    /// Gets or sets the is value.
    /// </summary>
    public bool IsValue { get; set; }
    /// <summary>
    /// Gets or sets the value aggregate type.
    /// </summary>
    public PivotValueAggregateType ValueAggregateType { get; set; }

    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    public string Caption
    {
        get => !string.IsNullOrWhiteSpace(fCaption) ? fCaption : FieldName;
        set => fCaption = value;
    }

    /// <summary>
    /// Gets or sets the format.
    /// </summary>
    public string Format { get; set; }
    /// <summary>
    /// Gets or sets the sort descending.
    /// </summary>
    public bool SortDescending { get; set; }

    /// <summary>
    /// Gets or sets the sort by value.
    /// </summary>
    public bool SortByValue { get; set; } = true;

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}

/// <summary>
/// Pivot definition.
/// </summary>
public class PivotDef
{
    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    public ObservableCollection<PivotColumnDef> Columns { get; set; } = new();
    /// <summary>
    /// Gets or sets the show subtotals.
    /// </summary>
    public bool ShowSubtotals { get; set; } = true;
    /// <summary>
    /// Gets or sets the show grand totals.
    /// </summary>
    public bool ShowGrandTotals { get; set; } = true;

    /// <summary>
    /// Still ignored by the current engine. Values are rendered on columns.
    /// </summary>
    public bool ShowValuesOnRows { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }

    /// <summary>
    /// Executes get rows.
    /// </summary>
    public IEnumerable<PivotColumnDef> GetRows()
        => Columns.Where(x => x.Axis == PivotAxis.Row);

    /// <summary>
    /// Executes get columns.
    /// </summary>
    public IEnumerable<PivotColumnDef> GetColumns()
        => Columns.Where(x => x.Axis == PivotAxis.Column);

    /// <summary>
    /// Executes get values.
    /// </summary>
    public IEnumerable<PivotColumnDef> GetValues()
        => Columns.Where(x => x.IsValue);
}

/// <summary>
/// A pivot output column.
/// </summary>
public class PivotDataColumn
{
    /// <summary>
    /// Gets or sets the key.
    /// </summary>
    public string Key { get; set; }
    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    public string Caption { get; set; }
    /// <summary>
    /// Gets or sets the data type.
    /// </summary>
    public Type DataType { get; set; }
    /// <summary>
    /// Gets or sets the format.
    /// </summary>
    public string Format { get; set; }
    /// <summary>
    /// Gets or sets the kind.
    /// </summary>
    public PivotDataColumnKind Kind { get; set; }
    /// <summary>
    /// Gets or sets the row level.
    /// </summary>
    public int RowLevel { get; set; } = -1;
    /// <summary>
    /// Gets or sets the source column.
    /// </summary>
    public PivotColumnDef SourceColumn { get; set; }

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}

/// <summary>
/// A pivot output row.
/// </summary>
public class PivotDataRow
{
    /// <summary>
    /// Gets or sets the values.
    /// </summary>
    public object[] Values { get; set; }
    /// <summary>
    /// Gets or sets the row type.
    /// </summary>
    public PivotDataRowType RowType { get; set; } = PivotDataRowType.Normal;
    /// <summary>
    /// Gets or sets the level.
    /// </summary>
    public int Level { get; set; } = -1;

    /// <summary>
    /// Gets or sets the tag.
    /// </summary>
    [JsonIgnore]
    public object Tag { get; set; }
}

/// <summary>
/// Pivot output data.
/// </summary>
public class PivotData
{
    /// <summary>
    /// Gets or sets the columns.
    /// </summary>
    public List<PivotDataColumn> Columns { get; } = new();
    /// <summary>
    /// Gets or sets the rows.
    /// </summary>
    public List<PivotDataRow> Rows { get; } = new();
}

/// <summary>
/// Pivot engine with:
/// - multi-level row subtotals
/// - multi-level column subtotals
/// - grand total row
/// - grand total columns
/// - aggregate-aware totals everywhere
/// Current limitation:
/// - values are still rendered on columns only
/// </summary>
static public class PivotEngine
{
    private const string KeySeparator = "\u001F";
    private const string NullToken = "\u2400";
    private const string GrandRowKey = "__GRAND_TOTAL__";

    /// <summary>
    /// Represents aggregate bucket.
    /// </summary>
    private sealed class AggregateBucket
    {
        private readonly PivotValueAggregateType fAggregateType;
        private int fCount;
        private decimal fSum;
        private decimal fSumSquares;
        private decimal fProduct = 1m;
        private decimal? fMin;
        private decimal? fMax;
        private HashSet<object> fDistinctSet;

        /// <summary>
        /// Executes aggregate bucket.
        /// </summary>
        public AggregateBucket(PivotValueAggregateType aggregateType)
        {
            fAggregateType = aggregateType;
        }

        /// <summary>
        /// Executes add.
        /// </summary>
        public void Add(object Value)
        {
            switch (fAggregateType)
            {
                case PivotValueAggregateType.Count:
                    fCount++;
                    return;

                case PivotValueAggregateType.CountDistinct:
                    if (Value != null && Value != DBNull.Value)
                    {
                        fDistinctSet ??= new HashSet<object>();
                        fDistinctSet.Add(Value);
                    }
                    return;
            }

            if (Value == null || Value == DBNull.Value)
                return;

            decimal number = Convert.ToDecimal(Value, CultureInfo.InvariantCulture);

            fCount++;

            switch (fAggregateType)
            {
                case PivotValueAggregateType.None:
                case PivotValueAggregateType.Sum:
                case PivotValueAggregateType.Avg:
                    fSum += number;
                    break;

                case PivotValueAggregateType.Min:
                    if (!fMin.HasValue || number < fMin.Value)
                        fMin = number;
                    break;

                case PivotValueAggregateType.Max:
                    if (!fMax.HasValue || number > fMax.Value)
                        fMax = number;
                    break;

                case PivotValueAggregateType.Product:
                    fProduct *= number;
                    break;

                case PivotValueAggregateType.StdDev:
                case PivotValueAggregateType.StdDevP:
                case PivotValueAggregateType.Variance:
                case PivotValueAggregateType.VarianceP:
                    fSum += number;
                    fSumSquares += number * number;
                    break;
            }
        }

        /// <summary>
        /// Executes get result.
        /// </summary>
        public object GetResult()
        {
            return fAggregateType switch
            {
                PivotValueAggregateType.None => fCount > 0 ? fSum : null,
                PivotValueAggregateType.Sum => fCount > 0 ? fSum : null,
                PivotValueAggregateType.Avg => fCount > 0 ? fSum / fCount : null,
                PivotValueAggregateType.Count => fCount,
                PivotValueAggregateType.Min => fMin,
                PivotValueAggregateType.Max => fMax,
                PivotValueAggregateType.Product => fCount > 0 ? fProduct : null,
                PivotValueAggregateType.CountDistinct => fDistinctSet?.Count ?? 0,
                PivotValueAggregateType.Variance => fCount > 1 ? ComputeVarianceSample() : null,
                PivotValueAggregateType.VarianceP => fCount > 0 ? ComputeVariancePopulation() : null,
                PivotValueAggregateType.StdDev => fCount > 1 ? SqrtDecimal(ComputeVarianceSample()) : null,
                PivotValueAggregateType.StdDevP => fCount > 0 ? SqrtDecimal(ComputeVariancePopulation()) : null,
                _ => throw new NotSupportedException($"Unsupported aggregate type: {fAggregateType}")
            };
        }

        /// <summary>
        /// Executes compute variance population.
        /// </summary>
        private decimal ComputeVariancePopulation()
        {
            decimal count = fCount;
            decimal mean = fSum / count;
            decimal variance = (fSumSquares / count) - (mean * mean);
            return variance < 0m ? 0m : variance;
        }

        /// <summary>
        /// Executes compute variance sample.
        /// </summary>
        private decimal ComputeVarianceSample()
        {
            if (fCount <= 1)
                return 0m;

            decimal count = fCount;
            decimal numerator = fSumSquares - ((fSum * fSum) / count);
            decimal variance = numerator / (count - 1m);
            return variance < 0m ? 0m : variance;
        }
    }

    /// <summary>
    /// Represents row source entry.
    /// </summary>
    private sealed class RowSourceEntry
    {
        /// <summary>
        /// Gets or sets the values.
        /// </summary>
        public Dictionary<string, object> Values { get; } = new(StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Represents row output entry.
    /// </summary>
    private sealed class RowOutputEntry
    {
        /// <summary>
        /// Gets or sets the row key.
        /// </summary>
        public string RowKey { get; set; }
        /// <summary>
        /// Gets or sets the row values.
        /// </summary>
        public object[] RowValues { get; set; }
        /// <summary>
        /// Gets or sets the row type.
        /// </summary>
        public PivotDataRowType RowType { get; set; }
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; }
    }

    /// <summary>
    /// Represents value column entry.
    /// </summary>
    private sealed class ValueColumnEntry
    {
        /// <summary>
        /// Gets or sets the column key.
        /// </summary>
        public string ColumnKey { get; set; }
        /// <summary>
        /// Gets or sets the source field name.
        /// </summary>
        public string SourceFieldName { get; set; }
        /// <summary>
        /// Gets or sets the result field key.
        /// </summary>
        public string ResultFieldKey { get; set; }
        /// <summary>
        /// Gets or sets the aggregate type.
        /// </summary>
        public PivotValueAggregateType AggregateType { get; set; }
    }

    /// <summary>
    /// Represents column output entry.
    /// </summary>
    private sealed class ColumnOutputEntry
    {
        /// <summary>
        /// Gets or sets the column key.
        /// </summary>
        public string ColumnKey { get; set; }
        /// <summary>
        /// Gets or sets the column values.
        /// </summary>
        public object[] ColumnValues { get; set; }
        /// <summary>
        /// Gets or sets the is subtotal.
        /// </summary>
        public bool IsSubtotal { get; set; }
        /// <summary>
        /// Gets or sets the is grand total.
        /// </summary>
        public bool IsGrandTotal { get; set; }
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        public int Level { get; set; } = -1;
    }

    /// <summary>
    /// Represents group info.
    /// </summary>
    private sealed class GroupInfo
    {
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value { get; set; }
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public List<KeyValuePair<string, object[]>> Items { get; } = new();
        /// <summary>
        /// Gets or sets the sort value.
        /// </summary>
        public decimal? SortValue { get; set; }
    }

    static public PivotData Execute<T>(IEnumerable<T> source, PivotDef pivotDef)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (pivotDef == null)
            throw new ArgumentNullException(nameof(pivotDef));

        List<PivotColumnDef> rowDefs = pivotDef.GetRows().ToList();
        List<PivotColumnDef> columnDefs = pivotDef.GetColumns().ToList();
        List<PivotColumnDef> valueDefs = pivotDef.GetValues().ToList();

        if (valueDefs.Count == 0)
            throw new ApplicationException("PivotDef contains no value columns.");

        List<RowSourceEntry> rowEntries = source.Select(CreateRowSourceEntry).ToList();

        Dictionary<string, object[]> detailRowKeyMap = new();
        Dictionary<int, Dictionary<string, object[]>> subtotalRowKeyMaps = new();
        Dictionary<string, object[]> detailColumnKeyMap = new();
        Dictionary<int, Dictionary<string, object[]>> subtotalColumnKeyMaps = new();

        Dictionary<string, AggregateBucket> detailBuckets = new();
        Dictionary<int, Dictionary<string, AggregateBucket>> subtotalBucketsByRowLevel = new();
        Dictionary<int, Dictionary<string, AggregateBucket>> rowSortBucketsByLevel = new();
        Dictionary<int, Dictionary<string, AggregateBucket>> columnSortBucketsByLevel = new();
        Dictionary<string, AggregateBucket> grandBuckets = new();

        foreach (RowSourceEntry entry in rowEntries)
        {
            object[] rowValues = rowDefs.Select(x => GetValue(entry, x.FieldName)).ToArray();
            object[] columnValues = columnDefs.Select(x => GetValue(entry, x.FieldName)).ToArray();

            string detailRowKey = ComposeKey(rowValues);
            string detailColumnKey = ComposeKey(columnValues);

            if (!detailRowKeyMap.ContainsKey(detailRowKey))
                detailRowKeyMap[detailRowKey] = rowValues;

            if (!detailColumnKeyMap.ContainsKey(detailColumnKey))
                detailColumnKeyMap[detailColumnKey] = columnValues;

            if (pivotDef.ShowSubtotals)
            {
                for (int rowLevel = 0; rowLevel < rowDefs.Count; rowLevel++)
                {
                    Dictionary<string, object[]> rowKeyMap = GetOrCreateSubtotalObjectMap(subtotalRowKeyMaps, rowLevel);
                    object[] subtotalGroupValues = GetSubtotalGroupValues(rowValues, rowLevel);
                    string subtotalGroupKey = ComposeKey(subtotalGroupValues);

                    if (!rowKeyMap.ContainsKey(subtotalGroupKey))
                        rowKeyMap[subtotalGroupKey] = GetSubtotalDisplayValues(rowValues, rowLevel);
                }

                for (int columnLevel = 0; columnLevel < columnDefs.Count; columnLevel++)
                {
                    Dictionary<string, object[]> columnKeyMap = GetOrCreateSubtotalObjectMap(subtotalColumnKeyMaps, columnLevel);
                    object[] subtotalColumnGroupValues = GetSubtotalGroupValues(columnValues, columnLevel);
                    string subtotalColumnKey = ComposeKey(subtotalColumnGroupValues);

                    if (!columnKeyMap.ContainsKey(subtotalColumnKey))
                        columnKeyMap[subtotalColumnKey] = GetSubtotalDisplayValues(columnValues, columnLevel);
                }
            }

            foreach (PivotColumnDef valueDef in valueDefs)
            {
                object value = GetValue(entry, valueDef.FieldName);

                UpdateBucket(detailBuckets, detailRowKey, detailColumnKey, valueDef, value);

                if (rowDefs.Count > 0)
                {
                    for (int rowLevel = 0; rowLevel < rowDefs.Count; rowLevel++)
                    {
                        Dictionary<string, AggregateBucket> sortBucketMap = GetOrCreateSubtotalBucketMap(rowSortBucketsByLevel, rowLevel);
                        object[] subtotalGroupValues = GetSubtotalGroupValues(rowValues, rowLevel);
                        string subtotalGroupKey = ComposeKey(subtotalGroupValues);

                        UpdateBucket(sortBucketMap, subtotalGroupKey, string.Empty, valueDef, value);
                    }
                }

                if (columnDefs.Count > 0)
                {
                    for (int columnLevel = 0; columnLevel < columnDefs.Count; columnLevel++)
                    {
                        Dictionary<string, AggregateBucket> sortBucketMap = GetOrCreateSubtotalBucketMap(columnSortBucketsByLevel, columnLevel);
                        object[] subtotalColumnGroupValues = GetSubtotalGroupValues(columnValues, columnLevel);
                        string subtotalColumnKey = ComposeKey(subtotalColumnGroupValues);

                        UpdateBucket(sortBucketMap, GrandRowKey, subtotalColumnKey, valueDef, value);
                    }
                }

                if (pivotDef.ShowGrandTotals)
                    UpdateBucket(detailBuckets, detailRowKey, string.Empty, valueDef, value);

                if (pivotDef.ShowSubtotals && rowDefs.Count > 0)
                {
                    for (int rowLevel = 0; rowLevel < rowDefs.Count; rowLevel++)
                    {
                        Dictionary<string, AggregateBucket> bucketMap = GetOrCreateSubtotalBucketMap(subtotalBucketsByRowLevel, rowLevel);
                        object[] subtotalGroupValues = GetSubtotalGroupValues(rowValues, rowLevel);
                        string subtotalGroupKey = ComposeKey(subtotalGroupValues);

                        UpdateBucket(bucketMap, subtotalGroupKey, detailColumnKey, valueDef, value);

                        if (pivotDef.ShowGrandTotals)
                            UpdateBucket(bucketMap, subtotalGroupKey, string.Empty, valueDef, value);
                    }
                }

                if (pivotDef.ShowSubtotals && columnDefs.Count > 0)
                {
                    for (int columnLevel = 0; columnLevel < columnDefs.Count; columnLevel++)
                    {
                        object[] subtotalColumnGroupValues = GetSubtotalGroupValues(columnValues, columnLevel);
                        string subtotalColumnKey = ComposeKey(subtotalColumnGroupValues);

                        UpdateBucket(detailBuckets, detailRowKey, subtotalColumnKey, valueDef, value);

                        if (pivotDef.ShowSubtotals && rowDefs.Count > 0)
                        {
                            for (int rowLevel = 0; rowLevel < rowDefs.Count; rowLevel++)
                            {
                                Dictionary<string, AggregateBucket> bucketMap = GetOrCreateSubtotalBucketMap(subtotalBucketsByRowLevel, rowLevel);
                                object[] subtotalGroupValues = GetSubtotalGroupValues(rowValues, rowLevel);
                                string subtotalGroupKey = ComposeKey(subtotalGroupValues);

                                UpdateBucket(bucketMap, subtotalGroupKey, subtotalColumnKey, valueDef, value);
                            }
                        }

                        if (pivotDef.ShowGrandTotals)
                            UpdateBucket(grandBuckets, GrandRowKey, subtotalColumnKey, valueDef, value);
                    }
                }

                if (pivotDef.ShowGrandTotals)
                    UpdateBucket(grandBuckets, GrandRowKey, detailColumnKey, valueDef, value);

                if (pivotDef.ShowGrandTotals)
                    UpdateBucket(grandBuckets, GrandRowKey, string.Empty, valueDef, value);
            }
        }

        return BuildPivotData(
            pivotDef,
            rowDefs,
            columnDefs,
            valueDefs,
            detailRowKeyMap,
            subtotalRowKeyMaps,
            detailColumnKeyMap,
            subtotalColumnKeyMaps,
            detailBuckets,
            subtotalBucketsByRowLevel,
            rowSortBucketsByLevel,
            columnSortBucketsByLevel,
            grandBuckets);
    }

    /// <summary>
    /// Executes execute.
    /// </summary>
    static public PivotData Execute(DataView Source, PivotDef PivotDef)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        return Execute(Source.Cast<DataRowView>(), PivotDef);
    }

    static private PivotData BuildPivotData(
        PivotDef pivotDef,
        List<PivotColumnDef> rowDefs,
        List<PivotColumnDef> columnDefs,
        List<PivotColumnDef> valueDefs,
        Dictionary<string, object[]> detailRowKeyMap,
        Dictionary<int, Dictionary<string, object[]>> subtotalRowKeyMaps,
        Dictionary<string, object[]> detailColumnKeyMap,
        Dictionary<int, Dictionary<string, object[]>> subtotalColumnKeyMaps,
        Dictionary<string, AggregateBucket> detailBuckets,
        Dictionary<int, Dictionary<string, AggregateBucket>> subtotalBucketsByRowLevel,
        Dictionary<int, Dictionary<string, AggregateBucket>> rowSortBucketsByLevel,
        Dictionary<int, Dictionary<string, AggregateBucket>> columnSortBucketsByLevel,
        Dictionary<string, AggregateBucket> grandBuckets)
    {
        PivotData result = new();

        for (int i = 0; i < rowDefs.Count; i++)
        {
            PivotColumnDef rowDef = rowDefs[i];
            result.Columns.Add(new PivotDataColumn
            {
                Key = rowDef.FieldName,
                Caption = rowDef.Caption,
                DataType = typeof(string),
                Format = rowDef.Format,
                Kind = PivotDataColumnKind.RowHeader,
                RowLevel = i,
                SourceColumn = rowDef
            });
        }

        List<ColumnOutputEntry> outputColumns = BuildOutputColumns(
            pivotDef,
            columnDefs,
            valueDefs,
            detailColumnKeyMap,
            subtotalColumnKeyMaps,
            columnSortBucketsByLevel);

        List<ValueColumnEntry> valueColumnEntries = new();
        int valueColumnIndex = 0;

        foreach (ColumnOutputEntry outputColumn in outputColumns)
        {
            foreach (PivotColumnDef valueDef in valueDefs)
            {
                string resultFieldKey = $"C{valueColumnIndex}";
                valueColumnIndex++;

                string caption = ComposeColumnCaption(outputColumn, valueDef, columnDefs);

                result.Columns.Add(new PivotDataColumn
                {
                    Key = resultFieldKey,
                    Caption = caption,
                    DataType = GetAggregateResultType(valueDef.ValueAggregateType),
                    Format = valueDef.Format,
                    Kind = PivotDataColumnKind.Value,
                    SourceColumn = valueDef,
                    Tag = outputColumn
                });

                valueColumnEntries.Add(new ValueColumnEntry
                {
                    ColumnKey = outputColumn.ColumnKey,
                    SourceFieldName = valueDef.FieldName,
                    ResultFieldKey = resultFieldKey,
                    AggregateType = valueDef.ValueAggregateType
                });
            }
        }

        List<RowOutputEntry> outputRows = BuildOutputRows(
            pivotDef,
            rowDefs,
            valueDefs,
            detailRowKeyMap,
            subtotalRowKeyMaps,
            rowSortBucketsByLevel);

        foreach (RowOutputEntry outputRow in outputRows)
        {
            Dictionary<string, AggregateBucket> bucketSource = outputRow.RowType switch
            {
                PivotDataRowType.Normal => detailBuckets,
                PivotDataRowType.Subtotal => subtotalBucketsByRowLevel[outputRow.Level],
                PivotDataRowType.GrandTotal => grandBuckets,
                _ => detailBuckets
            };

            object[] values = new object[result.Columns.Count];

            for (int i = 0; i < rowDefs.Count; i++)
                values[i] = outputRow.RowValues[i];

            for (int i = 0; i < valueColumnEntries.Count; i++)
            {
                ValueColumnEntry valueEntry = valueColumnEntries[i];
                string bucketKey = ComposeBucketKey(outputRow.RowKey, valueEntry.ColumnKey, valueEntry.SourceFieldName);
                int resultIndex = rowDefs.Count + i;

                if (bucketSource.TryGetValue(bucketKey, out AggregateBucket bucket))
                    values[resultIndex] = bucket.GetResult();
                else
                    values[resultIndex] = null;
            }

            result.Rows.Add(new PivotDataRow
            {
                Values = values,
                RowType = outputRow.RowType,
                Level = outputRow.Level
            });
        }

        return result;
    }

    static private List<RowOutputEntry> BuildOutputRows(
        PivotDef pivotDef,
        List<PivotColumnDef> rowDefs,
        List<PivotColumnDef> valueDefs,
        Dictionary<string, object[]> detailRowKeyMap,
        Dictionary<int, Dictionary<string, object[]>> subtotalRowKeyMaps,
        Dictionary<int, Dictionary<string, AggregateBucket>> rowSortBucketsByLevel)
    {
        List<RowOutputEntry> result = new();

        if (rowDefs.Count == 0)
        {
            result.Add(new RowOutputEntry
            {
                RowKey = string.Empty,
                RowValues = Array.Empty<object>(),
                RowType = PivotDataRowType.Normal,
                Level = -1
            });

            if (pivotDef.ShowGrandTotals)
            {
                result.Add(new RowOutputEntry
                {
                    RowKey = GrandRowKey,
                    RowValues = Array.Empty<object>(),
                    RowType = PivotDataRowType.GrandTotal,
                    Level = -1
                });
            }

            return result;
        }

        List<KeyValuePair<string, object[]>> orderedDetailRows = SortAxisEntries(
            detailRowKeyMap,
            rowDefs,
            valueDefs,
            true,
            rowSortBucketsByLevel);

        if (orderedDetailRows.Count == 0)
            return result;

        AppendRowLevel(result, pivotDef, rowDefs, subtotalRowKeyMaps, orderedDetailRows, 0);

        if (pivotDef.ShowGrandTotals)
        {
            object[] grandValues = new object[rowDefs.Count];
            grandValues[0] = "Grand Total";

            result.Add(new RowOutputEntry
            {
                RowKey = GrandRowKey,
                RowValues = grandValues,
                RowType = PivotDataRowType.GrandTotal,
                Level = -1
            });
        }

        return result;
    }

    static private void AppendRowLevel(
        List<RowOutputEntry> output,
        PivotDef pivotDef,
        List<PivotColumnDef> rowDefs,
        Dictionary<int, Dictionary<string, object[]>> subtotalRowKeyMaps,
        List<KeyValuePair<string, object[]>> rows,
        int level)
    {
        int index = 0;

        while (index < rows.Count)
        {
            object valueAtLevel = rows[index].Value[level];
            int groupStart = index;
            index++;

            while (index < rows.Count && AreEqual(rows[index].Value[level], valueAtLevel))
                index++;

            List<KeyValuePair<string, object[]>> groupRows = rows.GetRange(groupStart, index - groupStart);

            if (level < rowDefs.Count - 1)
                AppendRowLevel(output, pivotDef, rowDefs, subtotalRowKeyMaps, groupRows, level + 1);
            else
                AppendDetailRows(output, groupRows);

            if (pivotDef.ShowSubtotals && groupRows.Count > 1)
            {
                object[] subtotalGroupValues = GetSubtotalGroupValues(groupRows[0].Value, level);
                string subtotalGroupKey = ComposeKey(subtotalGroupValues);

                output.Add(new RowOutputEntry
                {
                    RowKey = subtotalGroupKey,
                    RowValues = subtotalRowKeyMaps[level][subtotalGroupKey],
                    RowType = PivotDataRowType.Subtotal,
                    Level = level
                });
            }
        }
    }

    static private List<ColumnOutputEntry> BuildOutputColumns(
        PivotDef pivotDef,
        List<PivotColumnDef> columnDefs,
        List<PivotColumnDef> valueDefs,
        Dictionary<string, object[]> detailColumnKeyMap,
        Dictionary<int, Dictionary<string, object[]>> subtotalColumnKeyMaps,
        Dictionary<int, Dictionary<string, AggregateBucket>> columnSortBucketsByLevel)
    {
        List<ColumnOutputEntry> result = new();

        if (columnDefs.Count == 0)
        {
            result.Add(new ColumnOutputEntry
            {
                ColumnKey = string.Empty,
                ColumnValues = Array.Empty<object>(),
                IsGrandTotal = true,
                Level = -1
            });

            return result;
        }

        List<KeyValuePair<string, object[]>> orderedDetailColumns = SortAxisEntries(
            detailColumnKeyMap,
            columnDefs,
            valueDefs,
            false,
            columnSortBucketsByLevel);

        if (orderedDetailColumns.Count == 0)
        {
            if (pivotDef.ShowGrandTotals)
            {
                result.Add(new ColumnOutputEntry
                {
                    ColumnKey = string.Empty,
                    ColumnValues = Array.Empty<object>(),
                    IsGrandTotal = true,
                    Level = -1
                });
            }

            return result;
        }

        AppendColumnLevel(result, pivotDef, columnDefs, subtotalColumnKeyMaps, orderedDetailColumns, 0);

        if (pivotDef.ShowGrandTotals)
        {
            result.Add(new ColumnOutputEntry
            {
                ColumnKey = string.Empty,
                ColumnValues = Array.Empty<object>(),
                IsGrandTotal = true,
                Level = -1
            });
        }

        return result;
    }

    static private void AppendColumnLevel(
        List<ColumnOutputEntry> output,
        PivotDef pivotDef,
        List<PivotColumnDef> columnDefs,
        Dictionary<int, Dictionary<string, object[]>> subtotalColumnKeyMaps,
        List<KeyValuePair<string, object[]>> columns,
        int level)
    {
        int index = 0;

        while (index < columns.Count)
        {
            object valueAtLevel = columns[index].Value[level];
            int groupStart = index;
            index++;

            while (index < columns.Count && AreEqual(columns[index].Value[level], valueAtLevel))
                index++;

            List<KeyValuePair<string, object[]>> groupColumns = columns.GetRange(groupStart, index - groupStart);

            if (level < columnDefs.Count - 1)
                AppendColumnLevel(output, pivotDef, columnDefs, subtotalColumnKeyMaps, groupColumns, level + 1);
            else
                AppendDetailColumns(output, groupColumns);

            if (pivotDef.ShowSubtotals && groupColumns.Count > 1)
            {
                object[] subtotalGroupValues = GetSubtotalGroupValues(groupColumns[0].Value, level);
                string subtotalGroupKey = ComposeKey(subtotalGroupValues);

                output.Add(new ColumnOutputEntry
                {
                    ColumnKey = subtotalGroupKey,
                    ColumnValues = subtotalColumnKeyMaps[level][subtotalGroupKey],
                    IsSubtotal = true,
                    Level = level
                });
            }
        }
    }

    static private List<KeyValuePair<string, object[]>> SortAxisEntries(
        Dictionary<string, object[]> sourceMap,
        List<PivotColumnDef> axisDefs,
        List<PivotColumnDef> valueDefs,
        bool isRowAxis,
        Dictionary<int, Dictionary<string, AggregateBucket>> sortBucketsByLevel)
    {
        List<KeyValuePair<string, object[]>> items = sourceMap
            .OrderBy(x => x.Value, new ObjectArrayComparer())
            .ToList();

        if (items.Count <= 1 || axisDefs.Count == 0)
            return items;

        PivotColumnDef firstValueDef = valueDefs.FirstOrDefault();
        return SortAxisEntriesLevel(items, axisDefs, firstValueDef, isRowAxis, sortBucketsByLevel, 0);
    }

    static private List<KeyValuePair<string, object[]>> SortAxisEntriesLevel(
        List<KeyValuePair<string, object[]>> items,
        List<PivotColumnDef> axisDefs,
        PivotColumnDef valueDef,
        bool isRowAxis,
        Dictionary<int, Dictionary<string, AggregateBucket>> sortBucketsByLevel,
        int level)
    {
        if (items.Count <= 1 || level >= axisDefs.Count)
            return items;

        PivotColumnDef axisDef = axisDefs[level];
        List<GroupInfo> groups = GroupByLevel(items, level);

        foreach (GroupInfo group in groups)
        {
            if (group.Items.Count > 1)
            {
                List<KeyValuePair<string, object[]>> sortedChildren = SortAxisEntriesLevel(
                    group.Items,
                    axisDefs,
                    valueDef,
                    isRowAxis,
                    sortBucketsByLevel,
                    level + 1);

                group.Items.Clear();
                group.Items.AddRange(sortedChildren);
            }

            if (axisDef.SortByValue)
                group.SortValue = GetGroupSortValue(group.Items[0].Value, level, valueDef, isRowAxis, sortBucketsByLevel);
        }

        groups.Sort((a, b) => CompareGroups(a, b, axisDef.SortByValue, axisDef.SortDescending));

        List<KeyValuePair<string, object[]>> result = new();
        foreach (GroupInfo group in groups)
            result.AddRange(group.Items);

        return result;
    }

    /// <summary>
    /// Executes group by level.
    /// </summary>
    static private List<GroupInfo> GroupByLevel(List<KeyValuePair<string, object[]>> Items, int Level)
    {
        List<GroupInfo> result = new();

        foreach (KeyValuePair<string, object[]> item in Items)
        {
            object value = item.Value[Level];
            GroupInfo group = result.FirstOrDefault(x => AreEqual(x.Value, value));

            if (group == null)
            {
                group = new GroupInfo
                {
                    Value = value
                };
                result.Add(group);
            }

            group.Items.Add(item);
        }

        return result;
    }
    static private decimal? GetGroupSortValue(
        object[] axisValues,
        int level,
        PivotColumnDef valueDef,
        bool isRowAxis,
        Dictionary<int, Dictionary<string, AggregateBucket>> sortBucketsByLevel)
    {
        if (valueDef == null || sortBucketsByLevel == null)
            return null;

        if (!sortBucketsByLevel.TryGetValue(level, out Dictionary<string, AggregateBucket> bucketMap))
            return null;

        string groupKey = ComposeKey(GetSubtotalGroupValues(axisValues, level));

        decimal sum = 0m;
        bool found = false;
        string prefix = isRowAxis
            ? groupKey + KeySeparator
            : GrandRowKey + KeySeparator;

        string suffix = KeySeparator + valueDef.FieldName;

        foreach (var pair in bucketMap)
        {
            if (!pair.Key.StartsWith(prefix, StringComparison.Ordinal))
                continue;

            if (!pair.Key.EndsWith(suffix, StringComparison.Ordinal))
                continue;

            object result = pair.Value.GetResult();
            if (result == null || result == DBNull.Value)
                continue;

            try
            {
                sum += Convert.ToDecimal(result, CultureInfo.InvariantCulture);
                found = true;
            }
            catch
            {
            }
        }

        return found ? sum : null;
    }
    
    /// <summary>
    /// Executes compare groups.
    /// </summary>
    static private int CompareGroups(GroupInfo A, GroupInfo B, bool SortByValue, bool SortDescending)
    {
        int result;

        if (SortByValue)
        {
            result = CompareNullableDecimal(A.SortValue, B.SortValue);

            if (result == 0)
                result = ObjectArrayComparer.CompareObject(A.Value, B.Value);
        }
        else
        {
            result = ObjectArrayComparer.CompareObject(A.Value, B.Value);
        }

        return SortDescending ? -result : result;
    }

    /// <summary>
    /// Executes compare nullable decimal.
    /// </summary>
    static private int CompareNullableDecimal(decimal? A, decimal? B)
    {
        if (A.HasValue && B.HasValue)
            return A.Value.CompareTo(B.Value);

        if (A.HasValue)
            return 1;

        if (B.HasValue)
            return -1;

        return 0;
    }

    /// <summary>
    /// Executes append detail rows.
    /// </summary>
    static private void AppendDetailRows(List<RowOutputEntry> Output, List<KeyValuePair<string, object[]>> Rows)
    {
        foreach (KeyValuePair<string, object[]> pair in Rows)
        {
            Output.Add(new RowOutputEntry
            {
                RowKey = pair.Key,
                RowValues = pair.Value,
                RowType = PivotDataRowType.Normal,
                Level = -1
            });
        }
    }

    /// <summary>
    /// Executes append detail columns.
    /// </summary>
    static private void AppendDetailColumns(List<ColumnOutputEntry> Output, List<KeyValuePair<string, object[]>> Columns)
    {
        foreach (KeyValuePair<string, object[]> pair in Columns)
        {
            Output.Add(new ColumnOutputEntry
            {
                ColumnKey = pair.Key,
                ColumnValues = pair.Value,
                IsSubtotal = false,
                IsGrandTotal = false,
                Level = -1
            });
        }
    }


    /// <summary>
    /// Executes compose column caption.
    /// </summary>
    static private string ComposeColumnCaption(ColumnOutputEntry OutputColumn, PivotColumnDef ValueDef, List<PivotColumnDef> ColumnDefs)
    {
        if (OutputColumn.IsGrandTotal)
            return $"Total{Environment.NewLine}{ValueDef.Caption}";

        if (!OutputColumn.IsSubtotal)
        {
            string detailCaption = ComposeCaption(OutputColumn.ColumnValues);

            return string.IsNullOrWhiteSpace(detailCaption)
                ? ValueDef.Caption
                : $"{detailCaption}{Environment.NewLine}{ValueDef.Caption}";
        }

        int level = OutputColumn.Level;
        List<string> parts = new();

        for (int i = 0; i < level; i++)
        {
            object value = OutputColumn.ColumnValues[i];
            if (value != null)
                parts.Add(value.ToString());
        }

        object subtotalValue = OutputColumn.ColumnValues[level];
        if (subtotalValue != null)
            parts.Add(subtotalValue.ToString());

        parts.Add(ValueDef.Caption);

        return string.Join(Environment.NewLine, parts.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
    static private Dictionary<string, object[]> GetOrCreateSubtotalObjectMap(
        Dictionary<int, Dictionary<string, object[]>> maps,
        int level)
    {
        if (!maps.TryGetValue(level, out Dictionary<string, object[]> map))
        {
            map = new Dictionary<string, object[]>();
            maps[level] = map;
        }

        return map;
    }

    static private Dictionary<string, AggregateBucket> GetOrCreateSubtotalBucketMap(
        Dictionary<int, Dictionary<string, AggregateBucket>> maps,
        int level)
    {
        if (!maps.TryGetValue(level, out Dictionary<string, AggregateBucket> map))
        {
            map = new Dictionary<string, AggregateBucket>();
            maps[level] = map;
        }

        return map;
    }

    /// <summary>
    /// Executes get subtotal group values.
    /// </summary>
    static private object[] GetSubtotalGroupValues(object[] DetailValues, int Level)
    {
        object[] result = new object[DetailValues.Length];

        for (int i = 0; i <= Level; i++)
            result[i] = DetailValues[i];

        return result;
    }

    /// <summary>
    /// Executes get subtotal display values.
    /// </summary>
    static private object[] GetSubtotalDisplayValues(object[] DetailValues, int Level)
    {
        object[] result = new object[DetailValues.Length];

        for (int i = 0; i < Level; i++)
            result[i] = DetailValues[i];

        result[Level] = $"{DetailValues[Level]} Total";
        return result;
    }

    static private void UpdateBucket(
        Dictionary<string, AggregateBucket> bucketMap,
        string rowKey,
        string columnKey,
        PivotColumnDef valueDef,
        object value)
    {
        string bucketKey = ComposeBucketKey(rowKey, columnKey, valueDef.FieldName);

        if (!bucketMap.TryGetValue(bucketKey, out AggregateBucket bucket))
        {
            bucket = new AggregateBucket(valueDef.ValueAggregateType);
            bucketMap[bucketKey] = bucket;
        }

        bucket.Add(value);
    }

    /// <summary>
    /// Executes compose bucket key.
    /// </summary>
    static private string ComposeBucketKey(string RowKey, string ColumnKey, string ValueFieldName)
        => $"{RowKey}{KeySeparator}{ColumnKey}{KeySeparator}{ValueFieldName}";

    /// <summary>
    /// Executes compose key.
    /// </summary>
    static private string ComposeKey(object[] Values)
    {
        if (Values == null || Values.Length == 0)
            return string.Empty;

        return string.Join(KeySeparator, Values.Select(x => x == null ? NullToken : x.ToString()));
    }

    /// <summary>
    /// Executes compose caption.
    /// </summary>
    static private string ComposeCaption(object[] Values)
    {
        if (Values == null || Values.Length == 0)
            return string.Empty;
 
        return string.Join(Environment.NewLine, Values.Select(x => x?.ToString() ?? string.Empty));
    }
 
    static private RowSourceEntry CreateRowSourceEntry<T>(T item)
    {
        RowSourceEntry entry = new();

        if (item is DataRowView rowView)
        {
            foreach (DataColumn column in rowView.DataView.Table.Columns)
            {
                object value = rowView[column.ColumnName];
                entry.Values[column.ColumnName] = value == DBNull.Value ? null : value;
            }
        }
        else
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
                entry.Values[property.Name] = property.GetValue(item);
        }

        return entry;
    }

    /// <summary>
    /// Executes get value.
    /// </summary>
    static private object GetValue(RowSourceEntry Entry, string FieldName)
    {
        if (Entry.Values.TryGetValue(FieldName, out object value))
            return value;

        throw new ApplicationException($"Field or property '{FieldName}' was not found.");
    }

    /// <summary>
    /// Executes are equal.
    /// </summary>
    static private bool AreEqual(object A, object B)
    {
        if (A == null && B == null)
            return true;

        if (A == null || B == null)
            return false;

        return Equals(A, B);
    }

    /// <summary>
    /// Executes get aggregate result type.
    /// </summary>
    static private Type GetAggregateResultType(PivotValueAggregateType AggregateType)
    {
        return AggregateType switch
        {
            PivotValueAggregateType.Count => typeof(int),
            PivotValueAggregateType.CountDistinct => typeof(int),
            PivotValueAggregateType.StdDev => typeof(decimal),
            PivotValueAggregateType.StdDevP => typeof(decimal),
            PivotValueAggregateType.Variance => typeof(decimal),
            PivotValueAggregateType.VarianceP => typeof(decimal),
            PivotValueAggregateType.Sum => typeof(decimal),
            PivotValueAggregateType.Avg => typeof(decimal),
            PivotValueAggregateType.Min => typeof(decimal),
            PivotValueAggregateType.Max => typeof(decimal),
            PivotValueAggregateType.Product => typeof(decimal),
            PivotValueAggregateType.None => typeof(decimal),
            _ => typeof(object)
        };
    }

    /// <summary>
    /// Executes sqrt decimal.
    /// </summary>
    static private decimal SqrtDecimal(decimal Value)
    {
        if (Value < 0m)
            throw new ArgumentOutOfRangeException(nameof(Value));

        if (Value == 0m)
            return 0m;

        decimal current = (decimal)Math.Sqrt((double)Value);

        for (int i = 0; i < 10; i++)
            current = (current + Value / current) / 2m;

        return current;
    }

    /// <summary>
    /// Represents object array comparer.
    /// </summary>
    private sealed class ObjectArrayComparer : IComparer<object[]>
    {
        /// <summary>
        /// Executes compare.
        /// </summary>
        public int Compare(object[] X, object[] Y)
        {
            if (ReferenceEquals(X, Y))
                return 0;

            if (X == null)
                return -1;

            if (Y == null)
                return 1;

            int length = Math.Min(X.Length, Y.Length);

            for (int i = 0; i < length; i++)
            {
                int c = CompareObject(X[i], Y[i]);
                if (c != 0)
                    return c;
            }

            return X.Length.CompareTo(Y.Length);
        }

        /// <summary>
        /// Executes compare object.
        /// </summary>
        static public int CompareObject(object A, object B)
        {
            if (A == null && B == null)
                return 0;

            if (A == null)
                return -1;

            if (B == null)
                return 1;

            if (A is IComparable ca && A.GetType() == B.GetType())
                return ca.CompareTo(B);

            return StringComparer.CurrentCultureIgnoreCase.Compare(A.ToString(), B.ToString());
        }
    }
}