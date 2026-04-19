namespace Tripous.Data;

public class RowFilterDef
{
    private bool fCorrectingSerialization;
    private string fValueText;
    private string fValue2Text;
    private string fValueType;
    private string fValue2Type;
    private object fValue;
    private object fValue2;

    // ● overrides
    private void CorrectSerialization()
    {
        if (fCorrectingSerialization)
            return;

        try
        {
            fCorrectingSerialization = true;

            /* Runtime -> Serializable */
            if (fValue != null)
            {
                if (ConditionOp == ConditionOp.In)
                {
                    if (fValue is IEnumerable List && fValue is not string)
                    {
                        object[] Items = List.Cast<object>().ToArray();

                        Type ItemType = typeof(string);
                        object FirstNonNull = Items.FirstOrDefault(x => x != null);
                        if (FirstNonNull != null)
                            ItemType = FirstNonNull.GetType();

                        fValueType = ItemType.FullName; //ItemType.AssemblyQualifiedName;
                        fValueText = string.Join("\u001F", Items.Select(ConvertToText));

                        fValue2 = null;
                        fValue2Text = null;
                        fValue2Type = null;
                    }
                    else
                    {
                        throw new ApplicationException("IN requires an IEnumerable value.");
                    }
                }
                else
                {
                    fValueType = fValue.GetType().FullName; //fValue.GetType().AssemblyQualifiedName;
                    fValueText = ConvertToText(fValue);

                    if (fValue2 != null)
                    {
                        fValue2Type = fValue2.GetType().FullName; //fValue2.GetType().AssemblyQualifiedName;
                        fValue2Text = ConvertToText(fValue2);
                    }
                    else
                    {
                        fValue2Type = null;
                        fValue2Text = null;
                    }
                }

                return;
            }

            /* Serializable -> Runtime */
            if (!string.IsNullOrWhiteSpace(fValueType))
            {
                Type T1 = Type.GetType(fValueType, throwOnError: false);
                if (T1 != null)
                {
                    if (ConditionOp == ConditionOp.In)
                    {
                        string[] Parts = string.IsNullOrWhiteSpace(fValueText)
                            ? Array.Empty<string>()
                            : fValueText.Split('\u001F');

                        fValue = Parts.Select(x => ConvertFromText(x, T1)).ToArray();
                        fValue2 = null;
                    }
                    else
                    {
                        fValue = fValueText != null ? ConvertFromText(fValueText, T1) : null;

                        Type T2 = !string.IsNullOrWhiteSpace(fValue2Type)
                            ? Type.GetType(fValue2Type, throwOnError: false)
                            : T1;

                        fValue2 = (T2 != null && fValue2Text != null)
                            ? ConvertFromText(fValue2Text, T2)
                            : null;
                    }
                }
            }
        }
        finally
        {
            fCorrectingSerialization = false;
        }
    }
    private static string ConvertToText(object Value)
    {
        if (Value == null)
            return null;

        Type T = Value.GetType();

        if (T == typeof(DateTime))
            return ((DateTime)Value).ToString("O", CultureInfo.InvariantCulture);

        if (T == typeof(DateTimeOffset))
            return ((DateTimeOffset)Value).ToString("O", CultureInfo.InvariantCulture);

        if (T == typeof(decimal))
            return ((decimal)Value).ToString(CultureInfo.InvariantCulture);

        if (T == typeof(double))
            return ((double)Value).ToString(CultureInfo.InvariantCulture);

        if (T == typeof(float))
            return ((float)Value).ToString(CultureInfo.InvariantCulture);

        if (T == typeof(Guid))
            return Value.ToString();

        if (T.IsEnum)
            return Convert.ToInt32(Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

        return Convert.ToString(Value, CultureInfo.InvariantCulture);
    }
    private static object ConvertFromText(string Text, Type T)
    {
        if (Text == null)
            return null;

        if (T == typeof(string))
            return Text;

        if (T == typeof(int))
            return int.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(long))
            return long.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(short))
            return short.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(byte))
            return byte.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(decimal))
            return decimal.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(double))
            return double.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(float))
            return float.Parse(Text, CultureInfo.InvariantCulture);

        if (T == typeof(bool))
            return bool.Parse(Text);

        if (T == typeof(DateTime))
            return DateTime.Parse(Text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        if (T == typeof(DateTimeOffset))
            return DateTimeOffset.Parse(Text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        if (T == typeof(Guid))
            return Guid.Parse(Text);

        if (T.IsEnum)
            return Enum.ToObject(T, int.Parse(Text, CultureInfo.InvariantCulture));

        return Convert.ChangeType(Text, T, CultureInfo.InvariantCulture);
    }

    // ● construction
    public RowFilterDef()
    {
    }
    /// <summary>
    /// Constructor
    /// <para>With <see cref="ConditionOp.In"/> a value could be an <see cref="IEnumerable"/>, e.g. new[] { "Open", "Closed", "Pending" }</para>
    /// </summary>
    public RowFilterDef(BoolOp BoolOp, ConditionOp ConditionOp, string FieldName, object Value, object Value2 = null)
        : this()
    {
        this.BoolOp = BoolOp;
        this.ConditionOp = ConditionOp;
        this.FieldName = FieldName;
        this.Value = Value;
        this.Value2 = Value2;
    }

    // ● public
    public override string ToString() => this.Text;
    public void Check()
    {
        if (ConditionOp == ConditionOp.None)
            throw new ApplicationException("A WHERE item must have a condition operator.");

        if (!Enum.IsDefined(typeof(ConditionOp), ConditionOp))
            throw new ApplicationException($"Invalid condition operator: {ConditionOp}");

        if (!Enum.IsDefined(typeof(BoolOp), BoolOp))
            throw new ApplicationException($"Invalid boolean operator: {BoolOp}");

        if (string.IsNullOrWhiteSpace(FieldName))
            throw new ApplicationException("A WHERE item must have a field name");

        switch (ConditionOp)
        {
            case ConditionOp.Equal:
            case ConditionOp.NotEqual:
            case ConditionOp.Greater:
            case ConditionOp.GreaterOrEqual:
            case ConditionOp.Less:
            case ConditionOp.LessOrEqual:
            case ConditionOp.Like:
            case ConditionOp.Contains:
            case ConditionOp.StartsWith:
            case ConditionOp.EndsWith:
            case ConditionOp.In:
                if (Value == null)
                    throw new ApplicationException($"Operator {ConditionOp} requires a value: {FieldName}");
                break;

            case ConditionOp.Between:
                if (Value == null || Value2 == null)
                    throw new ApplicationException($"A BETWEEN expression requires two values: {FieldName}");
                break;
        }
    }

    public void AssignFrom(RowFilterDef Source)
    {
        BoolOp = Source.BoolOp;
        ConditionOp = Source.ConditionOp;
        FieldName = Source.FieldName;
        fValue = Source.Value;
        fValue2 = Source.Value2;
        fValueText = Source.ValueText;
        fValue2Text = Source.Value2Text;
        fValueType = Source.ValueType;
        fValue2Type = Source.Value2Type;
        ColumnDef = Source.ColumnDef;
        Tag = Source.Tag;
    }
    public RowFilterDef Clone()
    {
        RowFilterDef Result = new();
        Result.AssignFrom(this);
        return Result;
    }
    
    
    // ● properties
    public BoolOp BoolOp { get; set; }
    public ConditionOp ConditionOp { get; set; }
    public string FieldName { get; set; }
    [JsonIgnore]
    public object Value
    {
        get => fValue;
        set
        {
            fValue = value;
            CorrectSerialization();
        }
    }
    [JsonIgnore]
    public object Value2
    {
        get => fValue2;
        set
        {
            fValue2 = value;
            CorrectSerialization();
        }
    }
    public string ValueText
    {
        get => fValueText;
        set
        {
            fValueText = value;
            CorrectSerialization();
        }
    }
    public string Value2Text
    {
        get => fValue2Text;
        set
        {
            fValue2Text = value;
            CorrectSerialization();
        }
    }
    public string ValueType
    {
        get => fValueType;
        set
        {
            fValueType = value;
            CorrectSerialization();
        }
    }
    public string Value2Type
    {
        get => fValue2Type;
        set
        {
            fValue2Type = value;
            CorrectSerialization();
        }
    }
    [JsonIgnore]
    public string Text
    {
        get
        {
            Check();
            string Text = string.Empty;

            string SFieldName = RowFilterFormatter.Field(FieldName);
            string V1 = RowFilterFormatter.Value(Value);
            string V2 = RowFilterFormatter.Value(Value2);

            string S = Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;

            switch (ConditionOp)
            {
                case ConditionOp.Equal: Text += $"{SFieldName} = {V1}"; break;
                case ConditionOp.NotEqual: Text += $"{SFieldName} <> {V1}"; break;
                case ConditionOp.Greater: Text += $"{SFieldName} > {V1}"; break;
                case ConditionOp.GreaterOrEqual: Text += $"{SFieldName} >= {V1}"; break;
                case ConditionOp.Less: Text += $"{SFieldName} < {V1}"; break;
                case ConditionOp.LessOrEqual: Text += $"{SFieldName} <= {V1}"; break;
                case ConditionOp.Like: Text += $"{SFieldName} like {V1}"; break;

                case ConditionOp.Contains:
                case ConditionOp.StartsWith:
                case ConditionOp.EndsWith:
                    Text += $"{SFieldName} like {RowFilterFormatter.Value(RowFilterFormatter.LikePattern(ConditionOp, S))}";
                    break;

                case ConditionOp.Between:
                    Text += $"{SFieldName} between {V1} and {V2}";
                    break;

                case ConditionOp.In:
                    if (Value is IEnumerable List && Value is not string)
                    {
                        var Items = List.Cast<object>().Select(RowFilterFormatter.Value);
                        Text += $"{SFieldName} in ({string.Join(", ", Items)})";
                    }
                    else
                    {
                        throw new ApplicationException("IN requires a collection value.");
                    }
                    break;

                case ConditionOp.Null:
                    Text += $"{SFieldName} is null";
                    break;
            }

            switch (BoolOp)
            {
                case BoolOp.And: Text = $"and ({Text}) "; break;
                case BoolOp.Or: Text = $"or ({Text}) "; break;
                case BoolOp.AndNot: Text = $"and (not {Text}) "; break;
                case BoolOp.OrNot: Text = $"or (not {Text}) "; break;
            }

            return Text;
        }
    }
    [JsonIgnore]
    public GridViewColumnDef ColumnDef { get; set; }
    [JsonIgnore]
    public object Tag { get; set; }
}

public class RowFilterDefs : ObservableCollection<RowFilterDef>
{
    public RowFilterDefs()
    {
    }

    public override string ToString() => this.Text;

    public void Check() => this.ToList().ForEach(x => x.Check());
    
    public RowFilterDef Add(BoolOp BoolOp, ConditionOp ConditionOp, string FieldName, object Value, object Value2 = null)
    {
        RowFilterDef Result = new RowFilterDef(BoolOp, ConditionOp, FieldName, Value, Value2);
        this.Add(Result);
        return Result;
    }

    public RowFilterDef Find(string FieldName) => this.FirstOrDefault(x => FieldName.IsSameText(x.FieldName));
    public bool Contains(string FieldName) => this.Any(x => x.FieldName.IsSameText(FieldName));
    public RowFilterDef Get(string FieldName)
    {
        RowFilterDef Result = Find(FieldName);
        if (Result == null)
            throw new ApplicationException($"Filter Item not found: {FieldName}");
        return Result;
    }
    
    [JsonIgnore]
    public string Text
    {
        get
        {
            Check();
            StringBuilder SB = new();

            string Text;

            for (int i = 0; i < Count; i++)
            {
                RowFilterDef def = this[i];
                Text = def.Text;

                if (i == 0)
                {
                    if (Text.StartsWith("and "))
                        Text = Text.Remove(0, "and ".Length);
                    else if (Text.StartsWith("or "))
                        Text = Text.Remove(0, "or ".Length);
                }

                SB.Append(Text);
            }

            return SB.ToString();
        }
    }
}

public enum BlobType
{
    None,
    Text,
    Image,
    Binary,
}

public interface ILookupSource
{
    string Name { get; }
    IEnumerable Items { get; }
}

public class GridViewColumnDef
{
    // ● private fields
    private string fDisplayFormat;
    private string fEditFormat;
    private string fCaption;
    private Type fDataType;
    private bool fSourceAllowsNull;

    protected void DataTypeChanged()
    {
        UnderlyingType = DataType != null ? Nullable.GetUnderlyingType(DataType) ?? DataType : null;
        IsString = UnderlyingType != null && UnderlyingType.IsString();
        IsDateTime = UnderlyingType != null && UnderlyingType.IsDateTime();
        IsNumeric = UnderlyingType != null && UnderlyingType.IsNumeric();
        IsBool = UnderlyingType != null && (UnderlyingType == typeof(bool));
        IsBlob = UnderlyingType == typeof(byte[]) || UnderlyingType.InheritsFrom(typeof(Stream));
        IsNullableType = (DataType != null && Nullable.GetUnderlyingType(DataType) != null);
        IsNullable = IsNullableType || SourceAllowsNull;
        IsRowFilterSupportedColumn = IsString || IsNumeric || IsDateTime;
        ValidAggregates = UnderlyingType != null ? UnderlyingType.GetValidAggregates() : Array.Empty<AggregateType>();
    }

    // ● construction
    public GridViewColumnDef()
    {
    }

    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(FieldName) ? FieldName : base.ToString();
    public void SetAllowsNullFromSource(bool Value)
    {
        fSourceAllowsNull = Value;
        DataTypeChanged();
    }

    public void AssignFrom(GridViewColumnDef Source)
    {
        FieldName = Source.FieldName;
        fCaption = Source.fCaption;
        VisibleIndex = Source.VisibleIndex;
        GroupIndex = Source.GroupIndex;
        SortIndex = Source.SortIndex;
        SortDirection = Source.SortDirection;
        Aggregate = Source.Aggregate;
        BlobType = Source.BlobType;
        
        fDisplayFormat = Source.fDisplayFormat;
        fEditFormat = Source.fEditFormat;
        
        IsReadOnly = Source.IsReadOnly;
        IsIntAsBool = Source.IsIntAsBool;
        fSourceAllowsNull = Source.fSourceAllowsNull;

        DisplayMember = Source.DisplayMember;
        ValueMember = Source.ValueMember;
        LookupSourceName = Source.LookupSourceName;
        LookupSql = Source.LookupSql;

        LookupSource = Source.LookupSource;

        DataType = Source.DataType;
    }
    public GridViewColumnDef Clone()
    {
        GridViewColumnDef Result = new();
        Result.AssignFrom(this);
        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the field name.
    /// </summary>
    public string FieldName { get; set; }
    /// <summary>
    /// The caption, header, of the column.
    /// </summary>
    public string Caption
    {
        get => !string.IsNullOrWhiteSpace(fCaption) ? fCaption : FieldName;
        set => fCaption = value;
    }
    /// <summary>
    /// When below zero the column is not visible. Else this value is the order of the column in the visible columns.
    /// </summary>
    public int VisibleIndex { get; set; } = 0;
    /// <summary>
    /// When below zero the column is not part of the group. Else this value is the column index in the group.
    /// </summary>
    public int GroupIndex { get; set; } = -1;
    public int SortIndex { get; set; } = -1;     
    public ListSortDirection SortDirection { get; set; } // Asc / Desc
    /// <summary>
    /// The aggregate type of the column. <see cref="AggregateType.None"/> means no aggregate is applied.
    /// </summary>
    public AggregateType Aggregate { get; set; } = AggregateType.None;
    /// <summary>
    /// Valid only when is a blob
    /// </summary>
    public BlobType BlobType { get; set; }

    /// <summary>
    /// The display format used by the grid cell when no explicit value is assigned.
    /// </summary>
    public string DisplayFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fDisplayFormat))
                return fDisplayFormat;

            if (UnderlyingType != null && UnderlyingType.IsNumeric() && !UnderlyingType.IsInteger())
                return "N2";

            if (UnderlyingType != null && UnderlyingType.IsDateTime())
                return "yyyy-MM-dd HH:mm";

            return string.Empty;
        }
        set => fDisplayFormat = value;
    }
    /// <summary>
    /// The edit format used by inplace editors when no explicit value is assigned.
    /// </summary>
    public string EditFormat
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(fEditFormat))
                return fEditFormat;

            if (UnderlyingType != null && UnderlyingType.IsDateTime())
                return "yyyy-MM-dd HH:mm";

            return string.Empty;
        }
        set => fEditFormat = value;
    }
    
    /// <summary>
    /// When true the column is not editable.
    /// </summary>
    public bool IsReadOnly { get; set; }
    /// <summary>
    /// An integer field acting as a bool field, i.e. 0 = false, else is true
    /// </summary>
    public bool IsIntAsBool{ get; set; }
    /// <summary>
    /// True when the source column is nullable
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
            }
        }
    }
 
    public string DisplayMember { get; set; }
    public string ValueMember { get; set; }
    public string LookupSourceName { get; set; }
    public string LookupSql { get; set; }

    [JsonIgnore]
    public ILookupSource LookupSource { get; set; }
    [JsonIgnore]
    public bool IsLookup => LookupSource != null;

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
            }
        }
    }

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
    public bool IsBlob { get; private set; }
    [JsonIgnore]
    public bool IsNullable { get; private set; }
    [JsonIgnore]
    public bool IsNullableType { get; private set; }
    [JsonIgnore]
    public bool IsRowFilterSupportedColumn { get; private set; }
    [JsonIgnore]
    public AggregateType[] ValidAggregates { get; private set; }
}
 
public class GridViewDef
{
    private string fName;
    
    // ● construction
    public GridViewDef()
    {
    }

    // ● static
    /// <summary>
    /// Creates and returns a default definition based on a specified source.
    /// </summary>
    static public GridViewDef Create(DataView Source)
    {
        GridViewDef Def = new();
        Def.SetColumnsFrom(Source);
        return Def;
    }
    /// <summary>
    /// Creates and returns a default definition based on a specified source.
    /// </summary>
    static public GridViewDef Create(Type ItemType)
    {
        GridViewDef Def = new();
        Def.SetColumnsFrom(ItemType);
        return Def;
    }
 
    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(Name) ? Name: base.ToString();
    
    public string GetDescription()
    {
        // --------------------------------------------
        string SortDescription(GridViewColumnDef Column) => 
            Column.FieldName + " " +
            (Column.SortDirection == ListSortDirection.Ascending ? "" : "desc");
        // --------------------------------------------
        
        StringBuilder SB = new();
        
        // group
        string S = string.Join(", ", GetGroupColumns() );
        SB.AppendLine($"Group: [{S}]");
        
        // summaries
        S = string.Join(", ", GetAggregateColumns()
            .Select(x => $"{x.FieldName} = {x.Aggregate}"));
        SB.AppendLine($"Summaries: {S}");
        
        // row filters
        S = RowFilters.Text;
        SB.AppendLine($"RowFilter: {S}");
            
        // hidden columns
        S = string.Join(", ", GetHiddenColumns());
        SB.AppendLine($"Hidden: {S}");

        // sorting
        S = string.Join(", ", GetSortedColumns()
            .Select(x => SortDescription(x)));
        SB.AppendLine($"Sorting: {S}");
            
        S = SB.ToString();
        return S;
    }
    public void ClearLists()
    {
        Columns.Clear();
        RowFilters.Clear();
    }

    public void UpdateDataTypes(DataView DataView)
    {
        var All = DataView.Table.Columns.Cast<DataColumn>().ToList();
        DataColumn Column;
        foreach (var Item in Columns)
        {
            Column = All.FirstOrDefault(x => x.ColumnName.IsSameText(Item.FieldName));
            if (Column != null)
                Item.DataType = Column.DataType;
        }
    }
    public void UpdateDataTypes(Type T)
    {
        var All = T.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToList();
        PropertyInfo PropInfo;
        foreach (var Item in Columns)
        {
            PropInfo = All.FirstOrDefault(x => x.Name.IsSameText(Item.FieldName));
            if (PropInfo != null)
                Item.DataType = PropInfo.PropertyType;
        }
    }
    
    public void SetColumnsFrom(DataView Source)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));
        
        this.Columns.Clear();
        
        foreach (DataColumn DataColumn in Source.Table.Columns)
        {
            GridViewColumnDef Column = new()
            {
                FieldName = DataColumn.ColumnName,
                Caption = DataColumn.ColumnName,
                DataType = DataColumn.DataType,
            };
            
            Column.SetAllowsNullFromSource(DataColumn.AllowDBNull);
                
            this.Columns.Add(Column);
            Column.VisibleIndex = this.Columns.IndexOf(Column);
        }
    }
    public void SetColumnsFrom(Type ItemType)
    {
        if (ItemType == null)
            throw new ArgumentNullException(nameof(ItemType));
        
        this.Columns.Clear();
 
        PropertyInfo[] Properties = ItemType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

        foreach (PropertyInfo Prop in Properties)
        {
            GridViewColumnDef Column = new()
            {
                FieldName = Prop.Name,
                Caption = Prop.Name,
                DataType = Prop.PropertyType,
            };

            this.Columns.Add(Column);
            Column.VisibleIndex = this.Columns.IndexOf(Column);
        }
    }
    
    public void AssignFrom(GridViewDef Source)
    {
        ClearLists();
        
        fName = Source.fName;
        ShowGroupColumnsAsDataColumns = Source.ShowGroupColumnsAsDataColumns;
        IsNameReadOnly = Source.IsNameReadOnly;
        Tag = Source.Tag;
        
        foreach (var SourceColumn in Source.Columns)
            Columns.Add(SourceColumn.Clone());

        foreach (var SourceFilterItem in Source.RowFilters)
            RowFilters.Add(SourceFilterItem.Clone());
    }
    public GridViewDef Clone()
    {
        GridViewDef Result = new();
        Result.AssignFrom(this);
        return Result;
    }
    
    /// <summary>
    /// Returns columns that participate in the group
    /// </summary>
    public List<GridViewColumnDef> GetGroupColumns() 
        => Columns.Where(x => x.GroupIndex >= 0).OrderBy(x => x.GroupIndex).ToList();
    /// <summary>
    /// Executes get columns.
    /// </summary>
    public List<GridViewColumnDef> GetAggregateColumns() => Columns.Where(x => x.Aggregate != AggregateType.None).ToList();
    /// <summary>
    /// Executes get values.
    /// </summary>
    public List<GridViewColumnDef> GetVisibleColumns() 
        => Columns.Where(x => x.VisibleIndex >= 0).OrderBy(x => x.VisibleIndex).ToList();
    /// <summary>
    /// Executes get values.
    /// </summary>
    public List<GridViewColumnDef> GetHiddenColumns() => Columns.Where(x => x.VisibleIndex < 0).ToList();
    /// <summary>
    /// Returns the list of sorted columns, if any, else empty list.
    /// </summary>
    /// <returns></returns>
    public List<GridViewColumnDef> GetSortedColumns() 
        => Columns.Where(x => x.SortIndex >= 0).OrderBy(x => x.SortIndex).ToList();
    
    public GridViewColumnDef Find(string FieldName) => Columns.FirstOrDefault(x => FieldName.IsSameText(x.FieldName));
    public bool Contains(string FieldName) => Columns.Any(x => x.FieldName.IsSameText(FieldName));
    public GridViewColumnDef Get(string FieldName)
    {
        GridViewColumnDef Result = Find(FieldName);
        if (Result == null)
            throw new ApplicationException($"Column not found: {FieldName}");
        return Result;
    }

    public string GetErrors()
    {
        StringBuilder SB = new();
        
        if (string.IsNullOrWhiteSpace(Name))
            SB.AppendLine($"{nameof(GridViewDef)} name is empty.");

        if (Columns == null || Columns.Count == 0)
            SB.AppendLine($"{nameof(GridViewDef)} contains no columns.");

        if (!GetVisibleColumns().Any())
            SB.AppendLine($"{nameof(GridViewDef)} contains no visible columns.");
        
        if (Columns.Any(x => x == null))
            SB.AppendLine($"{nameof(GridViewDef)} contains null GridViewDef.");
        
        if (Columns.Any(x => string.IsNullOrWhiteSpace(x.FieldName)))
            SB.AppendLine($"{nameof(GridViewDef)} contains columns with empty FieldName.");
        
        if (Columns.Any(x => x.DataType == null))
            SB.AppendLine($"{nameof(GridViewDef)} contains columns with null DataTypes.");

        HashSet<string> FieldNames = new(StringComparer.OrdinalIgnoreCase);

        foreach (GridViewColumnDef Column in Columns)
        {
            if (!FieldNames.Add(Column.FieldName))
                SB.AppendLine($"Duplicate field: '{Column.FieldName}'.");
        }

        return SB.ToString();
    }
    public bool HasErrors() => !string.IsNullOrWhiteSpace(GetErrors());
    public void Check()
    {
        string Errors = GetErrors();
        if (!string.IsNullOrWhiteSpace(Errors))
            throw new ApplicationException(Errors);
    }

    public void ResetColumns()
    {
        foreach (var Column in Columns)
        {
            Column.VisibleIndex = 0;
            Column.GroupIndex = -1;
            Column.SortIndex = -1;
            Column.SortDirection = ListSortDirection.Ascending;
            Column.Aggregate = AggregateType.None;
        }
    }
    
    // ● properties
    /// <summary>
    /// The name of this grid view
    /// </summary>
    public string Name
    {
        get => !string.IsNullOrWhiteSpace(fName) ? fName : Sys.GenId();
        set => fName = value;
    }
    /// <summary>
    /// Returns all columns
    /// </summary>
    public ObservableCollection<GridViewColumnDef> Columns { get; set; } = new();
    /// <summary>
    /// RowFilter definitions for columns participating in the DataView filtering.
    /// </summary>
    public RowFilterDefs RowFilters { get; set; } = new();
    /// <summary>
    /// When true then grouped columns are displayed as data columns too.
    /// </summary>
    public bool ShowGroupColumnsAsDataColumns { get; set; } = false;

    [JsonIgnore] 
    public GridViewColumnDef this[string FieldName] => Get(FieldName);
    [JsonIgnore]
    public object Tag { get; set; }
    [JsonIgnore]
    public bool IsNameReadOnly { get; set; }
}

public class GridViewDefs
{
    private string fName;
    
    // ● construction
    public GridViewDefs()
    {
    }
 
    // ● public
    public override string ToString() => !string.IsNullOrWhiteSpace(Name) ? Name: base.ToString();

    public void AssignFrom(IList<GridViewDef> SourceList)
    {
        DefList.Clear();
        foreach (var Def in SourceList)
            Add(Def);
    }
    public void AssignFrom(GridViewDefs Source)
    {
        fName = Source.fName;
        FilePath = Source.FilePath;

        AssignFrom(Source.DefList);
    }
    public GridViewDefs Clone()
    {
        GridViewDefs Result = new();
        Result.AssignFrom(this);
        return Result;
    }
    
    public void LoadFromFile()
    {
        if (string.IsNullOrWhiteSpace(FilePath) || !File.Exists(FilePath))
            throw new ApplicationException($"Cannot load {nameof(GridViewDefs)}. Invalid file path");
        DefList.Clear();
        Json.LoadFromFile(this, FilePath);
    }
    public void SaveToFile()
    {
        Json.SaveToFile(this, FilePath);
    }
    
    public GridViewDef Find(string Name) => DefList.FirstOrDefault(x => Name.IsSameText(x.Name));
    public bool Contains(string Name) => DefList.Any(x => x.Name.IsSameText(Name));
    public bool Contains(GridViewDef Def) => DefList.IndexOf(Def) >= 0;
    public GridViewDef Get(string Name)
    {
        GridViewDef Result = Find(Name);
        if (Result == null)
            throw new ApplicationException($"{nameof(GridViewDef)} not found: {Name}");
        return Result;
    }

    public GridViewDef Add(GridViewDef ViewDef, string Name = "")
    {
        if (!string.IsNullOrWhiteSpace(Name))
            ViewDef.Name = Name;
        
        if (string.IsNullOrWhiteSpace(ViewDef.Name)) 
            ViewDef.Name = Sys.GenId();
        
        if (DefList.Count == 0)
            ViewDef.Name = "Default";

        if (Contains(ViewDef.Name))
            throw new ApplicationException($"{nameof(GridViewDef)} already exists in list: {ViewDef.Name}");

        DefList.Add(ViewDef);
        return ViewDef;
    }
    public GridViewDef Add(DataView Source, string Name = "")
    {
        GridViewDef ViewDef = GridViewDef.Create(Source);
        return Add(ViewDef, Name);
    }
    public GridViewDef Add(Type ItemType, string Name = "")
    {
        GridViewDef ViewDef = GridViewDef.Create(ItemType);
        return Add(ViewDef, Name);
    }
    public void Remove(GridViewDef ViewDef)
    {
        if (DefList.Contains(ViewDef))
        {
            if (DefList.Count == 1)
                throw new ApplicationException($"Cannot delete the last {nameof(GridViewDef)} from list.");
            if (DefList.Count > 1)
                DefList.Remove(ViewDef);
        }
    }
    
    // ● properties
    /// <summary>
    /// The name of this grid view
    /// </summary>
    public string Name
    {
        get => !string.IsNullOrWhiteSpace(fName) ? fName : Sys.GenId();
        set => fName = value;
    }
    public ObservableCollection<GridViewDef> DefList { get; set; } = new();
    [JsonIgnore] 
    public string FilePath { get; set; }
}

public class LookupRegistry
{
    // ● private fields
    private readonly List<ILookupSource> fItems = new();

    // ● private methods
    private static void CheckName(string Name)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new ArgumentException("Lookup source name is required.", nameof(Name));
    }
    private ILookupSource FindInternal(string Name)
    {
        return fItems.FirstOrDefault(x => string.Equals(x.Name, Name, StringComparison.OrdinalIgnoreCase));
    }

    // ● constructors
    public LookupRegistry()
    {
    }

    // ● public methods
    public void Add(ILookupSource Source)
    {
        if (Source == null)
            throw new ArgumentNullException(nameof(Source));

        CheckName(Source.Name);

        if (FindInternal(Source.Name) != null)
            throw new ApplicationException($"Lookup source already exists: {Source.Name}");

        
        fItems.Add(Source);
    }

    public void AddRange(IEnumerable<ILookupSource> Sources)
    {
        foreach (var Source in Sources)
            Add(Source);
    }
    public bool Remove(string Name)
    {
        CheckName(Name);

        ILookupSource Item = FindInternal(Name);
        if (Item != null)
            return fItems.Remove(Item);

        return false;
    }
    public bool Contains(string Name)
    {
        CheckName(Name);
        return FindInternal(Name) != null;
    }
    public ILookupSource Find(string Name)
    {
        CheckName(Name);
        return FindInternal(Name);
    }
    public ILookupSource Get(string Name)
    {
        CheckName(Name);

        ILookupSource Result = FindInternal(Name);
        if (Result == null)
            throw new ApplicationException($"Lookup source not found: {Name}");

        return Result;
    }
    public IEnumerable<ILookupSource> GetAll()
    {
        return fItems;
    }
    public void Clear()
    {
        fItems.Clear();
    }
}

public class GridViewContext
{
    // ● construction
    public GridViewContext()
    {
    }

    // ● properties
    public LookupRegistry LookupRegistry { get; } = new();
}