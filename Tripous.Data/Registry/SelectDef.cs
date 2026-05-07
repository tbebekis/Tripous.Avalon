namespace Tripous.Data;

/// <summary>
/// Describes a SELECT statement along with its possible WHERE filters.
/// </summary>
public class SelectDef: BaseDef
{
    string fSqlText;
    Dictionary<string, string> fDisplayLabels;
    SqlFilterDefs fFilterDefs;
    bool fUseFilters = true;

    // ● construction
    public SelectDef()
    {
    }

    // ● public
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(this.SqlText))
            Sys.Throw(Texts.GS($"E_{typeof(SelectDef)}_NoSql", $"{typeof(SelectDef)} must have an SQL statement"));
    }
    /// <summary>
    /// Adds a filter definition
    /// </summary>
    public SqlFilterDef AddFilter(string Name, string FieldName = null, DataFieldType FilterDataType = DataFieldType.String, BoolOp BoolOp = BoolOp.And, ConditionOp ConditionOp = ConditionOp.Equal, string TitleKey = null)
        => FilterDefs.Add(Name, FieldName, FilterDataType, BoolOp, ConditionOp, TitleKey);
    public SqlFilterDef AddFilter(FieldDef FieldDef)
        => FilterDefs.Add(FieldDef.Name, FieldName: FieldDef.Name, FilterDataType: FieldDef.DataType, TitleKey: FieldDef.TitleKey);

    /// <summary>
    /// Creates filter entries in the <see cref="FilterDefs"/> when no filters exist.
    /// <para><b>WARNING:</b> The <see cref="ModuleName"/> and a TableName are used in constructing a unique StatementName.</para>
    /// <para>The StatementName is used with the <see cref="SqlStore.GetNativeSchemaFromTableName"/>
    /// so the <c>ModuleName.TableName</c> must construct a unique name because schema DataTables are stored in the <see cref="SqlCache"/> under that unique name. </para>
    /// </summary>
    public SqlFilterDefs DefineFilters(string ModuleName, SqlStore Store)
    {
        SqlFilterDefs Result = new();
        
        string StatementName = $"{ModuleName}.{Name}";
        DataTable tblSchema = Store.GetNativeSchemaFromSelect(StatementName, SqlText);
        
        string[] FieldNames = { "Code", "Name", "LastName", "FirstName", "Product", "Customer", "Country", "City", "Date", "Amount", "Price" };
        List<DataColumn> Columns = new();
        foreach (DataColumn Column in tblSchema.Columns)
        {
            foreach (string FieldName in FieldNames)
            {
                if (FieldName.IsSameText(Column.ColumnName))
                {
                    DataFieldType FilterDataType = Column.DataType.GetDataFieldType();
                    if (FilterDataType.IsValidFilterType())
                    {
                        Result.Add(FieldName, FieldName: FieldName, FilterDataType: FilterDataType, TitleKey: Column.Caption);
                    }
                }
            }
        }

        return Result;
    }
    
    // ● properties
    public string SqlText
    {
        get => fSqlText;
        set
        {
            if (fSqlText != value)
            {
                fSqlText = value;
                NotifyPropertyChanged(nameof(SqlText));
            }
        }
    }
    public Dictionary<string, string> DisplayLabels
    {
        get => fDisplayLabels ??= new();
        set
        {
            if (fDisplayLabels != value)
            {
                fDisplayLabels = value;
                NotifyPropertyChanged(nameof(DisplayLabels));
            }
        }
    }
    public SqlFilterDefs FilterDefs
    {
        get => fFilterDefs ??= new();
        set
        {
            if (fFilterDefs != value)
            {
                fFilterDefs = value;
                NotifyPropertyChanged(nameof(FilterDefs));
            }
        }
    }
    /// <summary>
    /// Enables/Disables the use of the specified filters.
    /// </summary>
    public bool UseFilters
    {
        get => fUseFilters;
        set
        {
            if (fUseFilters != value)
            {
                fUseFilters = value;
                NotifyPropertyChanged(nameof(UseFilters));
            }
        }
    }
    [JsonIgnore]
    public object Owner { get; set; }
}