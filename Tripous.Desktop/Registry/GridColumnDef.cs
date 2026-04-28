namespace Tripous.Desktop;

public class GridColumnDef: BaseDef
{
    // ● private fields
    private Type fDataType;
    private bool fSourceAllowsNull;
    private string fDisplayFormat;
    private string fEditFormat;
    private bool fIsReadOnly;
    private int fVisibleIndex = 0;
    private int fGroupIndex = -1;
    private int fSortIndex = -1;
    private ListSortDirection fSortDirection;
    private AggregateType fAggregate = AggregateType.None;
    private HorizontalAlignment fAlignment = HorizontalAlignment.Left;

    // ● private
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
    public GridColumnDef()
    {
    }

    // ● static public
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
    public string FieldName => Name;
   
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
    public string DisplayFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fDisplayFormat))
                return fDisplayFormat;

            if (IsNumeric)
                return "N2";

            if (IsDateTime)
                return "yyyy-MM-dd HH:mm";

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
    public string EditFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fEditFormat))
                return fEditFormat;

            if (IsDateTime)
                return "yyyy-MM-dd HH:mm";

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
 
    public string LookupSource { get; set; }
 
    // ● derived
    [JsonIgnore]
    public DataGridColumn GridColumn { get; set; }
    [JsonIgnore]
    public Type UnderlyingType { get; private set; }
    [JsonIgnore]
    public bool IsString { get; private set; }
    [JsonIgnore]
    public bool IsDateTime { get; private set; }
    [JsonIgnore]
    public bool IsNumeric { get; private set; }
    [JsonIgnore]
    public bool IsBool { get; private set; }
    [JsonIgnore]
    public bool IsNullable { get; private set; }
    [JsonIgnore]
    public bool HasLookup => !string.IsNullOrWhiteSpace(LookupSource);
}