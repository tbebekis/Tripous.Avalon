namespace Tripous.Avalon;

public class PivotColumnDefRow : INotifyPropertyChanged
{
    // ● private fields
    private readonly PivotColumnDef fSource;
    private string fCaption;
    private PivotAxis fAxis;
    private bool fIsValue;
    private PivotValueAggregateType fValueAggregateType;
    private bool fSortByValue;
    private bool fSortDescending;
    private string fFormat;

    // ● private methods
    /// <summary>
    /// Applies internal rules after a property change.
    /// </summary>
    private void ApplyRules()
    {
        if (IsValue)
            fAxis = PivotAxis.None;

        if (Axis != PivotAxis.None)
            fIsValue = false;

        if (!IsValue)
            fValueAggregateType = PivotValueAggregateType.None;

        if (!IsNumeric && !IsDate)
            fFormat = null;

        if (Axis == PivotAxis.None)
        {
            fSortByValue = false;
            fSortDescending = false;
        }

        if (!IsNumeric && IsValue)
            fValueAggregateType = PivotValueAggregateType.Count;
    }
    /// <summary>
    /// Raises the property changed event.
    /// </summary>
    private void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
    /// <summary>
    /// Raises all dependent property notifications.
    /// </summary>
    private void OnStateChanged()
    {
        OnPropertyChanged(nameof(Caption));
        OnPropertyChanged(nameof(Axis));
        OnPropertyChanged(nameof(IsValue));
        OnPropertyChanged(nameof(ValueAggregateType));
        OnPropertyChanged(nameof(SortByValue));
        OnPropertyChanged(nameof(SortDescending));
        OnPropertyChanged(nameof(Format));
        OnPropertyChanged(nameof(IsPivotSupportedColumn));
        OnPropertyChanged(nameof(ValidAggregates));
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public PivotColumnDefRow(PivotColumnDef Source, DataColumn Column)
    {
        fSource = Source ?? throw new ArgumentNullException(nameof(Source));
        if (Column == null)
            throw new ArgumentNullException(nameof(Column));

        FieldName = Source.FieldName;
        DataType = Column.DataType;
        UnderlyingType = Nullable.GetUnderlyingType(DataType) ?? DataType;

        fCaption = Source.Caption;
        fAxis = Source.Axis;
        fIsValue = Source.IsValue;
        fValueAggregateType = Source.ValueAggregateType;
        fSortByValue = Source.SortByValue;
        fSortDescending = Source.SortDescending;
        fFormat = Source.Format;

        ApplyRules();
    }

    // ● public methods
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => !string.IsNullOrWhiteSpace(FieldName)? FieldName: base.ToString();
    /// <summary>
    /// Applies the row state to the source column definition.
    /// </summary>
    public void ApplyToColumnDef()
    {
        fSource.Caption = Caption;
        fSource.Axis = Axis;
        fSource.IsValue = IsValue;
        fSource.ValueAggregateType = ValueAggregateType;
        fSource.SortByValue = SortByValue;
        fSource.SortDescending = SortDescending;
        fSource.Format = Format;
    }

    // ● properties
    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string FieldName { get; }
    /// <summary>
    /// Gets the data type.
    /// </summary>
    public Type DataType { get; }
    /// <summary>
    /// Gets the underlying type.
    /// </summary>
    public Type UnderlyingType { get; }
    /// <summary>
    /// Gets a value indicating whether the type is string.
    /// </summary>
    public bool IsString => UnderlyingType.IsString();
    /// <summary>
    /// Gets a value indicating whether the type is date.
    /// </summary>
    public bool IsDate => UnderlyingType.IsDateTime();
    /// <summary>
    /// Gets a value indicating whether the type is numeric.
    /// </summary>
    public bool IsNumeric => UnderlyingType.IsNumeric();
    /// <summary>
    /// Gets a value indicating whether the column is supported by pivot.
    /// </summary>
    public bool IsPivotSupportedColumn => IsString || IsNumeric || IsDate;
    /// <summary>
    /// Gets the valid aggregates.
    /// </summary>
    public PivotValueAggregateType[] ValidAggregates => UnderlyingType.GetValidPivotAggregates();
    /// <summary>
    /// Gets or sets the caption.
    /// </summary>
    public string Caption
    {
        get => fCaption;
        set
        {
            if (fCaption == value)
                return;

            fCaption = value;
            OnPropertyChanged(nameof(Caption));
        }
    }
    /// <summary>
    /// Gets or sets the axis.
    /// </summary>
    public PivotAxis Axis
    {
        get => fAxis;
        set
        {
            if (fAxis == value)
                return;

            fAxis = value;
            ApplyRules();
            OnStateChanged();
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether this is a value column.
    /// </summary>
    public bool IsValue
    {
        get => fIsValue;
        set
        {
            if (fIsValue == value)
                return;

            fIsValue = value;
            ApplyRules();
            OnStateChanged();
        }
    }
    /// <summary>
    /// Gets or sets the aggregate type.
    /// </summary>
    public PivotValueAggregateType ValueAggregateType
    {
        get => fValueAggregateType;
        set
        {
            if (fValueAggregateType == value)
                return;

            fValueAggregateType = value;
            ApplyRules();
            OnPropertyChanged(nameof(ValueAggregateType));
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether sorting is by value.
    /// </summary>
    public bool SortByValue
    {
        get => fSortByValue;
        set
        {
            if (fSortByValue == value)
                return;

            fSortByValue = value;
            ApplyRules();
            OnPropertyChanged(nameof(SortByValue));
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether sorting is descending.
    /// </summary>
    public bool SortDescending
    {
        get => fSortDescending;
        set
        {
            if (fSortDescending == value)
                return;

            fSortDescending = value;
            ApplyRules();
            OnPropertyChanged(nameof(SortDescending));
        }
    }
    /// <summary>
    /// Gets or sets the format.
    /// </summary>
    public string Format
    {
        get => fFormat;
        set
        {
            if (fFormat == value)
                return;

            fFormat = value;
            ApplyRules();
            OnPropertyChanged(nameof(Format));
        }
    }

    // ● events
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}