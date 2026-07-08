/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Used with group grids, i.e. grids with groups, summaries, etc.
/// </summary>
public class GridColumnDef: BaseDef
{
    // ● private fields
    /// <summary>
    /// The column data type.
    /// </summary>
    private Type fDataType;
    /// <summary>
    /// True when the source column allows null values.
    /// </summary>
    private bool fSourceAllowsNull;
    /// <summary>
    /// The display format.
    /// </summary>
    private string fDisplayFormat;
    /// <summary>
    /// The edit format.
    /// </summary>
    private string fEditFormat;
    /// <summary>
    /// True when the column is read-only.
    /// </summary>
    private bool fIsReadOnly;
    /// <summary>
    /// The visible index.
    /// </summary>
    private int fVisibleIndex = 0;
    /// <summary>
    /// The group index.
    /// </summary>
    private int fGroupIndex = -1;
    /// <summary>
    /// The sort index.
    /// </summary>
    private int fSortIndex = -1;
    /// <summary>
    /// The sort direction.
    /// </summary>
    private ListSortDirection fSortDirection;
    /// <summary>
    /// The aggregate type.
    /// </summary>
    private AggregateType fAggregate = AggregateType.None;
    /// <summary>
    /// The horizontal alignment.
    /// </summary>
    private HorizontalAlignment fAlignment = HorizontalAlignment.Left;

    // ● private
    /// <summary>
    /// Updates derived data type flags.
    /// </summary>
    void DataTypeChanged()
    {
        UnderlyingType = fDataType != null ? Nullable.GetUnderlyingType(fDataType) ?? fDataType : null;
        IsString = UnderlyingType == typeof(string);
        IsDateTime = UnderlyingType == typeof(DateTime);
        IsBool = UnderlyingType == typeof(bool);
        IsNumeric = UnderlyingType != null && (
            UnderlyingType == typeof(byte) ||
            UnderlyingType == typeof(short) ||
            UnderlyingType == typeof(int) ||
            UnderlyingType == typeof(long) ||
            UnderlyingType == typeof(float) ||
            UnderlyingType == typeof(double) ||
            UnderlyingType == typeof(decimal));
        IsNullable = (Nullable.GetUnderlyingType(fDataType) != null) || fSourceAllowsNull;
    }

    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="GridColumnDef"/> class.
    /// </summary>
    public GridColumnDef()
    {
    }

    // ● static public
    /// <summary>
    /// Creates a grid column definition from a data column and optional field definition.
    /// </summary>
    /// <param name="Column">The data column.</param>
    /// <param name="Field">The field definition.</param>
    /// <returns>The created grid column definition.</returns>
    static public GridColumnDef From(DataColumn Column, FieldDef Field = null)
    {
        GridColumnDef Result = new();

        Result.Name = Column.ColumnName;
        Result.DataType = Column.DataType;
        Result.SourceAllowsNull = Column.AllowDBNull;

        if (Field != null)
        {
            Result.TitleKey = Field.TitleKey;
            Result.DisplayFormat = Field.DisplayFormat;
            Result.EditFormat = Field.EditFormat;
            Result.IsReadOnly = Field.Flags.HasFlag(FieldFlags.ReadOnly);
        }

        return Result;
    }
 
    // ● properties
    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string FieldName => Name;
   
    /// <summary>
    /// Gets or sets the data type.
    /// </summary>
    [JsonIgnore]
    public Type DataType
    {
        get => fDataType;
        set
        {
            if (fDataType != value)
            {
                fDataType = value;
                DataTypeChanged();
                NotifyPropertyChanged(nameof(DataType));
            }
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the source column allows null values.
    /// </summary>
    public bool SourceAllowsNull
    {
        get => fSourceAllowsNull;
        set
        {
            if (fSourceAllowsNull != value)
            {
                fSourceAllowsNull = value;
                DataTypeChanged();
                NotifyPropertyChanged(nameof(SourceAllowsNull));
            }
        }
    }
    /// <summary>
    /// Gets or sets a value indicating whether the column is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => fIsReadOnly;
        set
        {
            if (fIsReadOnly != value)
            {
                fIsReadOnly = value;
                NotifyPropertyChanged(nameof(IsReadOnly));
            }
        }
    }
    /// <summary>
    /// Gets or sets the horizontal alignment.
    /// </summary>
    public HorizontalAlignment Alignment
    {
        get => fAlignment;
        set
        {
            if (fAlignment != value)
            {
                fAlignment = value;
                NotifyPropertyChanged(nameof(Alignment));
            }
        }
    }
    /// <summary>
    /// Gets or sets the display format.
    /// </summary>
    public string DisplayFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fDisplayFormat))
                return fDisplayFormat;

            if (IsNumeric)
                return Sys.Settings.NumericFormat;

            if (IsDateTime)
                return Sys.Settings.DateTimeFormat;

            return string.Empty;
        }
        set
        {
            if (fDisplayFormat != value)
            {
                fDisplayFormat = value;
                NotifyPropertyChanged(nameof(DisplayFormat));
            }
        }
    }
    /// <summary>
    /// Gets or sets the edit format.
    /// </summary>
    public string EditFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fEditFormat))
                return fEditFormat;

            if (IsDateTime)
                return Sys.Settings.DateTimeFormat;

            return string.Empty;
        }
        set
        {
            if (fEditFormat != value)
            {
                fEditFormat = value;
                NotifyPropertyChanged(nameof(EditFormat));
            }
        }
    }
    /// <summary>
    /// Gets or sets the visible index.
    /// </summary>
    public int VisibleIndex
    {
        get => fVisibleIndex;
        set
        {
            if (fVisibleIndex != value)
            {
                fVisibleIndex = value;
                NotifyPropertyChanged(nameof(VisibleIndex));
            }
        }
    }
    /// <summary>
    /// Gets or sets the group index.
    /// </summary>
    public int GroupIndex
    {
        get => fGroupIndex;
        set
        {
            if (fGroupIndex != value)
            {
                fGroupIndex = value;
                NotifyPropertyChanged(nameof(GroupIndex));
            }
        }
    }
    /// <summary>
    /// Gets or sets the sort index.
    /// </summary>
    public int SortIndex
    {
        get => fSortIndex;
        set
        {
            if (fSortIndex != value)
            {
                fSortIndex = value;
                NotifyPropertyChanged(nameof(SortIndex));
            }
        }
    }
    /// <summary>
    /// Gets or sets the sort direction.
    /// </summary>
    public ListSortDirection SortDirection
    {
        get => fSortDirection;
        set
        {
            if (fSortDirection != value)
            {
                fSortDirection = value;
                NotifyPropertyChanged(nameof(SortDirection));
            }
        }
    }
    /// <summary>
    /// Gets or sets the aggregate type.
    /// </summary>
    public AggregateType Aggregate
    {
        get => fAggregate;
        set
        {
            if (fAggregate != value)
            {
                fAggregate = value;
                NotifyPropertyChanged(nameof(Aggregate));
            }
        }
    }
 
    /// <summary>
    /// Gets or sets the lookup source name.
    /// </summary>
    public string LookupSource { get; set; }
 
    // ● derived
    /// <summary>
    /// Gets the underlying data type.
    /// </summary>
    [JsonIgnore]
    public Type UnderlyingType { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the column data type is string.
    /// </summary>
    [JsonIgnore]
    public bool IsString { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the column data type is date/time.
    /// </summary>
    [JsonIgnore]
    public bool IsDateTime { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the column data type is numeric.
    /// </summary>
    [JsonIgnore]
    public bool IsNumeric { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the column data type is boolean.
    /// </summary>
    [JsonIgnore]
    public bool IsBool { get; private set; }
    /// <summary>
    /// Gets a value indicating whether the column accepts null values.
    /// </summary>
    [JsonIgnore]
    public bool IsNullable { get; private set; }
    /// <summary>
    /// Gets a value indicating whether this column has a lookup source.
    /// </summary>
    [JsonIgnore]
    public bool HasLookup => !string.IsNullOrWhiteSpace(LookupSource);
}
