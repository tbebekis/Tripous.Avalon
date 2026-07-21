// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents the non-visual state and projection of a chart.
/// </summary>
public class ChartEngine
{
    // ● private fields
    readonly List<ChartSeries> fSeries = new();
    readonly List<string> fCategoryKeys = new();
    readonly List<string> fCategoryTexts = new();
    IChartDataAdapter fDataAdapter;
    ChartSettings fSettings = new();

    // ● private methods
    void DataAdapter_Changed(object Sender, ChartDataChangedEventArgs Args)
    {
        Rebuild();
    }
    void SetDataAdapter(IChartDataAdapter Value)
    {
        if (ReferenceEquals(fDataAdapter, Value))
            return;

        if (fDataAdapter != null)
            fDataAdapter.Changed -= DataAdapter_Changed;

        fDataAdapter = Value;

        if (fDataAdapter != null)
            fDataAdapter.Changed += DataAdapter_Changed;

        Rebuild();
        DataAdapterChanged?.Invoke(this, EventArgs.Empty);
    }
    string CreateValueKey(object Value)
    {
        return Value == null || Value == DBNull.Value ? string.Empty : Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
    string CreateValueText(object Value)
    {
        return Value == null || Value == DBNull.Value ? string.Empty : Convert.ToString(Value, CultureInfo.CurrentCulture) ?? string.Empty;
    }
    string CreateBucketKey(string CategoryKey, string SeriesKey)
    {
        return CategoryKey + "\u001E" + SeriesKey;
    }
    decimal ConvertToDecimal(object Value)
    {
        return Convert.ToDecimal(Value, CultureInfo.CurrentCulture);
    }
    decimal SumValues(IEnumerable<object> Values)
    {
        decimal Result = 0;
        foreach (object Value in Values)
            Result += ConvertToDecimal(Value);

        return Result;
    }
    decimal ProductValues(IEnumerable<object> Values)
    {
        decimal Result = 1m;
        bool HasValue = false;
        foreach (object Value in Values)
        {
            Result *= ConvertToDecimal(Value);
            HasValue = true;
        }

        return HasValue ? Result : 0m;
    }
    object AverageValues(IReadOnlyList<object> Values)
    {
        if (Values.Count == 0)
            return null;

        return SumValues(Values) / Values.Count;
    }
    object VarianceValues(IReadOnlyList<object> Values, bool IsSample)
    {
        if (Values.Count == 0 || (IsSample && Values.Count <= 1))
            return null;

        decimal Sum = 0m;
        decimal SumSquares = 0m;
        foreach (object Value in Values)
        {
            decimal Number = ConvertToDecimal(Value);
            Sum += Number;
            SumSquares += Number * Number;
        }

        decimal Count = Values.Count;
        decimal Numerator = SumSquares - ((Sum * Sum) / Count);
        decimal Variance = IsSample
            ? Numerator / (Count - 1m)
            : (SumSquares / Count) - ((Sum / Count) * (Sum / Count));
        return Variance < 0m ? 0m : Variance;
    }
    object StandardDeviationValues(IReadOnlyList<object> Values, bool IsSample)
    {
        object Variance = VarianceValues(Values, IsSample);
        if (Variance == null)
            return null;

        return Convert.ToDecimal(Math.Sqrt(Convert.ToDouble(Variance, CultureInfo.CurrentCulture)), CultureInfo.CurrentCulture);
    }
    object AggregateValues(List<object> Values)
    {
        if (fSettings.AggregateKind == ChartAggregateKind.Count)
            return Values.Count;

        List<object> NonEmptyValues = Values
            .Where(Value => Value != null && Value != DBNull.Value)
            .ToList();
        if (NonEmptyValues.Count == 0)
            return null;

        switch (fSettings.AggregateKind)
        {
            case ChartAggregateKind.Sum:
                return SumValues(NonEmptyValues);
            case ChartAggregateKind.Min:
                return NonEmptyValues.OfType<IComparable>().OrderBy(Value => Value).FirstOrDefault();
            case ChartAggregateKind.Max:
                return NonEmptyValues.OfType<IComparable>().OrderByDescending(Value => Value).FirstOrDefault();
            case ChartAggregateKind.Average:
                return AverageValues(NonEmptyValues);
            case ChartAggregateKind.StdDev:
                return StandardDeviationValues(NonEmptyValues, true);
            case ChartAggregateKind.StdDevP:
                return StandardDeviationValues(NonEmptyValues, false);
            case ChartAggregateKind.Variance:
                return VarianceValues(NonEmptyValues, true);
            case ChartAggregateKind.VarianceP:
                return VarianceValues(NonEmptyValues, false);
            case ChartAggregateKind.CountDistinct:
                return NonEmptyValues.Distinct().Count();
            case ChartAggregateKind.Product:
                return ProductValues(NonEmptyValues);
        }

        return null;
    }
    decimal ToNumericValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return 0m;

        try
        {
            return ConvertToDecimal(Value);
        }
        catch
        {
            return 0m;
        }
    }
    string FormatValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;
        if (!string.IsNullOrWhiteSpace(fSettings.ValueFormat) && Value is IFormattable Formattable)
            return Formattable.ToString(fSettings.ValueFormat, CultureInfo.CurrentCulture);

        return Convert.ToString(Value, CultureInfo.CurrentCulture) ?? string.Empty;
    }
    List<string> GetOrderedCategoryKeys(Dictionary<string, string> CategoryTexts, Dictionary<string, decimal> CategoryTotals)
    {
        IEnumerable<string> Keys = CategoryTexts.Keys;
        if (fSettings.TopN > 0)
            Keys = Keys.OrderByDescending(Key => CategoryTotals.TryGetValue(Key, out decimal Value) ? Value : 0m).Take(fSettings.TopN).ToList();

        if (fSettings.SortDirection == ChartSortDirection.Ascending)
            Keys = Keys.OrderBy(Key => CategoryTexts[Key], StringComparer.CurrentCulture).ToList();
        else if (fSettings.SortDirection == ChartSortDirection.Descending)
            Keys = Keys.OrderByDescending(Key => CategoryTexts[Key], StringComparer.CurrentCulture).ToList();

        return Keys.ToList();
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ChartEngine"/> class.
    /// </summary>
    public ChartEngine()
    {
    }

    // ● public methods
    /// <summary>
    /// Rebuilds the chart projection.
    /// </summary>
    public void Rebuild()
    {
        fSeries.Clear();
        fCategoryKeys.Clear();
        fCategoryTexts.Clear();

        if (fDataAdapter == null || string.IsNullOrWhiteSpace(fSettings.CategoryFieldName))
        {
            ProjectionChanged?.Invoke(this, EventArgs.Empty);
            return;
        }

        Dictionary<string, string> CategoryTexts = new();
        Dictionary<string, string> SeriesTexts = new();
        Dictionary<string, List<object>> Buckets = new();
        Dictionary<string, decimal> CategoryTotals = new();
        string DefaultSeriesKey = string.Empty;
        string DefaultSeriesText = string.IsNullOrWhiteSpace(fSettings.ValueFieldName) ? "Value" : fSettings.ValueFieldName;

        for (int RowIndex = 0; RowIndex < fDataAdapter.RowCount; RowIndex++)
        {
            object CategoryValue = fDataAdapter.GetValue(RowIndex, fSettings.CategoryFieldName);
            object SeriesValue = string.IsNullOrWhiteSpace(fSettings.SeriesFieldName) ? null : fDataAdapter.GetValue(RowIndex, fSettings.SeriesFieldName);
            object Value = string.IsNullOrWhiteSpace(fSettings.ValueFieldName) ? null : fDataAdapter.GetValue(RowIndex, fSettings.ValueFieldName);
            string CategoryKey = CreateValueKey(CategoryValue);
            string SeriesKey = string.IsNullOrWhiteSpace(fSettings.SeriesFieldName) ? DefaultSeriesKey : CreateValueKey(SeriesValue);
            string BucketKey = CreateBucketKey(CategoryKey, SeriesKey);

            if (!CategoryTexts.ContainsKey(CategoryKey))
                CategoryTexts.Add(CategoryKey, CreateValueText(CategoryValue));
            if (!SeriesTexts.ContainsKey(SeriesKey))
                SeriesTexts.Add(SeriesKey, string.IsNullOrWhiteSpace(fSettings.SeriesFieldName) ? DefaultSeriesText : CreateValueText(SeriesValue));
            if (!Buckets.TryGetValue(BucketKey, out List<object> Values))
            {
                Values = new List<object>();
                Buckets.Add(BucketKey, Values);
            }

            Values.Add(Value);
        }

        Dictionary<string, object> Aggregates = new();
        foreach (KeyValuePair<string, List<object>> Entry in Buckets)
        {
            object Aggregate = AggregateValues(Entry.Value);
            Aggregates.Add(Entry.Key, Aggregate);
            string CategoryKey = Entry.Key.Split('\u001E')[0];
            decimal NumericValue = ToNumericValue(Aggregate);
            CategoryTotals[CategoryKey] = CategoryTotals.TryGetValue(CategoryKey, out decimal Current) ? Current + NumericValue : NumericValue;
        }

        List<string> OrderedCategoryKeys = GetOrderedCategoryKeys(CategoryTexts, CategoryTotals);
        HashSet<string> AcceptedCategories = OrderedCategoryKeys.ToHashSet(StringComparer.Ordinal);
        foreach (string CategoryKey in OrderedCategoryKeys)
        {
            fCategoryKeys.Add(CategoryKey);
            fCategoryTexts.Add(CategoryTexts[CategoryKey]);
        }

        foreach (KeyValuePair<string, string> SeriesEntry in SeriesTexts)
        {
            ChartSeries Series = new()
            {
                Key = SeriesEntry.Key,
                Text = string.IsNullOrWhiteSpace(SeriesEntry.Value) ? "(Blank)" : SeriesEntry.Value,
            };

            foreach (string CategoryKey in OrderedCategoryKeys)
            {
                string BucketKey = CreateBucketKey(CategoryKey, SeriesEntry.Key);
                Aggregates.TryGetValue(BucketKey, out object Aggregate);
                Series.Points.Add(new ChartDataPoint
                {
                    CategoryKey = CategoryKey,
                    CategoryText = CategoryTexts[CategoryKey],
                    SeriesKey = SeriesEntry.Key,
                    SeriesText = Series.Text,
                    Value = Aggregate,
                    NumericValue = ToNumericValue(Aggregate),
                    Text = FormatValue(Aggregate),
                });
            }

            if (Series.Points.Any(Point => AcceptedCategories.Contains(Point.CategoryKey)))
                fSeries.Add(Series);
        }

        ProjectionChanged?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Applies chart settings.
    /// </summary>
    /// <param name="Settings">The settings.</param>
    public void ApplySettings(ChartSettings Settings)
    {
        fSettings = Settings ?? new ChartSettings();
        Rebuild();
    }

    // ● properties
    /// <summary>
    /// Gets or sets the data adapter.
    /// </summary>
    public IChartDataAdapter DataAdapter
    {
        get => fDataAdapter;
        set => SetDataAdapter(value);
    }
    /// <summary>
    /// Gets the chart settings.
    /// </summary>
    public ChartSettings Settings => fSettings;
    /// <summary>
    /// Gets the chart series.
    /// </summary>
    public IReadOnlyList<ChartSeries> Series => fSeries;
    /// <summary>
    /// Gets the ordered category keys.
    /// </summary>
    public IReadOnlyList<string> CategoryKeys => fCategoryKeys;
    /// <summary>
    /// Gets the ordered category display texts.
    /// </summary>
    public IReadOnlyList<string> CategoryTexts => fCategoryTexts;
    /// <summary>
    /// Gets a value indicating whether the projection has at least one non-zero data point.
    /// </summary>
    public bool HasData => fSeries.Any(Series => Series.Points.Any(Point => Point.NumericValue != 0m));

    // ● events
    /// <summary>
    /// Occurs when the data adapter changes.
    /// </summary>
    public event EventHandler DataAdapterChanged;
    /// <summary>
    /// Occurs when the chart projection changes.
    /// </summary>
    public event EventHandler ProjectionChanged;
}
