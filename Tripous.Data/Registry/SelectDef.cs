/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Describes a SELECT statement along with its possible WHERE filters.
/// </summary>
public class SelectDef : BaseDef
{
    // ● private fields
    string fSqlText;
    Dictionary<string, string> fDisplayLabels;
    Dictionary<string, DataColumnType> fColumnTypes;
    SqlFilterDefs fFilterDefs;
    bool fUseFilters = true;

    // ● private
    /// <summary>
    /// Returns true when the specified type is an integer-compatible type.
    /// </summary>
    /// <param name="T">The type to check.</param>
    /// <returns>True when the type is integer-compatible.</returns>
    static bool IsIntegerCompatibleType(Type T)
    {
        return T == typeof(byte)
            || T == typeof(short)
            || T == typeof(int)
            || T == typeof(long)
            || T == typeof(sbyte)
            || T == typeof(ushort)
            || T == typeof(uint)
            || T == typeof(ulong);
    }
    /// <summary>
    /// Returns the schema column name for a filter definition.
    /// </summary>
    /// <param name="FilterDef">The filter definition.</param>
    /// <returns>The schema column name.</returns>
    static string GetFilterSchemaColumnName(SqlFilterDef FilterDef)
    {
        string Result = FilterDef.Name;
        if (string.IsNullOrWhiteSpace(Result))
            Result = FilterDef.FieldName;
        if (!string.IsNullOrWhiteSpace(Result) && Result.Contains('.'))
            Result = Result.Split('.').Last();
        return Result;
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance of the SelectDef class.
    /// </summary>
    public SelectDef()
    {
    }

    // ● public methods
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined.
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(this.SqlText))
            Sys.Throw(Texts.GS($"E_{typeof(SelectDef)}_NoSql", $"{typeof(SelectDef)} must have an SQL statement"));
    }
    /// <summary>
    /// Adds a filter definition to the internal collection using explicit criteria options.
    /// </summary>
    public SqlFilterDef AddFilter(string Name, string FieldName = null, DataFieldType FilterDataType = DataFieldType.String, BoolOp BoolOp = BoolOp.And, ConditionOp ConditionOp = ConditionOp.Equal, string TitleKey = null)
        => FilterDefs.Add(Name, FieldName, FilterDataType, BoolOp, ConditionOp, TitleKey);
    /// <summary>
    /// Adds a filter definition inferred directly from a field definition object metadata layout.
    /// </summary>
    public SqlFilterDef AddFilter(FieldDef FieldDef)
        => FilterDefs.Add(FieldDef.Name, FieldName: FieldDef.Name, FilterDataType: FieldDef.DataType, TitleKey: FieldDef.TitleKey);
    /// <summary>
    /// Validates that boolean filters are backed by integer-compatible schema columns.
    /// </summary>
    /// <param name="ModuleName">The module name.</param>
    /// <param name="Store">The SQL store.</param>
    /// <param name="FilterDefs">The filter definitions to validate.</param>
    public void ValidateBooleanFilterTypes(string ModuleName, SqlStore Store, SqlFilterDefs FilterDefs)
    {
        if (Store == null || FilterDefs == null || !FilterDefs.Any(item => item.FilterDataType == DataFieldType.Boolean))
            return;

        string StatementName = $"{ModuleName}.{Name}";
        DataTable tblSchema = Store.GetNativeSchemaFromSelect(StatementName, SqlText);
        foreach (SqlFilterDef FilterDef in FilterDefs.Where(item => item.FilterDataType == DataFieldType.Boolean))
        {
            string ColumnName = GetFilterSchemaColumnName(FilterDef);
            if (string.IsNullOrWhiteSpace(ColumnName) || !tblSchema.Columns.Contains(ColumnName))
                throw new TripousDataException($"Boolean filter '{FilterDef.Name}' column '{ColumnName}' not found in SELECT schema.");

            Type DataType = tblSchema.Columns[ColumnName].DataType;
            if (!IsIntegerCompatibleType(DataType))
                throw new TripousDataException($"Boolean filter '{FilterDef.Name}' must be backed by an integer-compatible column. Actual type is {DataType.FullName}.");
        }
    }
    /// <summary>
    /// Creates filter entries in the <see cref="FilterDefs"/> when no filters exist.
    /// <para><b>WARNING:</b> The module name and a table name are used in constructing a unique StatementName.</para>
    /// <para>The StatementName is used with the <see cref="SqlStore.GetNativeSchemaFromTableName"/>
    /// so the <c>ModuleName.TableName</c> must construct a unique name because schema DataTables are stored in the <see cref="SqlCache"/> under that unique name. </para>
    /// </summary>
    public SqlFilterDefs DefineFilters(string ModuleName, SqlStore Store)
    {
        string StatementName = $"{ModuleName}.{Name}";
        DataTable tblSchema = Store.GetNativeSchemaFromSelect(StatementName, SqlText);
        
        string[] FieldNames = { "Code", "Name", "Description", "LastName", "FirstName", "Product", "Customer", "Country", "City", "Date", "Amount", "Price" };
        List<DataColumn> Columns = new();
        foreach (DataColumn Column in tblSchema.Columns)
        {
            foreach (string FieldName in FieldNames)
            {
                if (FieldName.IsSameText(Column.ColumnName))
                {
                    if (Column.DataType == typeof(string) && Column.MaxLength > 256)
                        continue;
                    
                    DataFieldType FilterDataType = Column.DataType.GetDataFieldType();
                    if (FilterDataType.IsValidFilterType())
                    {
                        Columns.Add(Column);
                    }
                }
            }
        }

        Columns = Columns.OrderBy(x => x.Caption).ToList();
 
        Columns.Sort((A, B) => 
        {
            bool IsAName = A.Caption.Equals("Name", StringComparison.OrdinalIgnoreCase);
            bool IsBName = B.Caption.Equals("Name", StringComparison.OrdinalIgnoreCase);

            if (IsAName && !IsBName) return -1;
            if (!IsAName && IsBName) return 1;
            return 0;
        });
        
        SqlFilterDefs Result = new();
        foreach (DataColumn Column in Columns)
            Result.Add(Column.ColumnName, FieldName: Column.ColumnName, FilterDataType: Column.DataType.GetDataFieldType(), TitleKey: Column.Caption);

        return Result;
    }
    
    // ● properties
    /// <summary>
    /// Gets or sets the raw SQL query statement text template configuration block.
    /// </summary>
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
    /// <summary>
    /// Gets or sets the translation mapping table used for custom localized display titles.
    /// </summary>
    public Dictionary<string, string> DisplayLabels
    {
        get => fDisplayLabels ??= new(); 
        set { if (fDisplayLabels != value) { fDisplayLabels = value; NotifyPropertyChanged(nameof(DisplayLabels)); } }
    }
    /// <summary>
    /// Gets or sets the explicit structural data column database field type classification rules.
    /// </summary>
    public Dictionary<string, DataColumnType> ColumnTypes 
    {
        get => fColumnTypes ??= new(); 
        set { if (fColumnTypes != value) { fColumnTypes = value; NotifyPropertyChanged(nameof(ColumnTypes)); } }
    }
    /// <summary>
    /// Gets or sets the complete filter specification models collection belonging to this data query execution path.
    /// </summary>
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
    /// Gets or sets a value indicating whether runtime filter processing operations are active.
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
    /// <summary>
    /// Gets or sets the optional metadata context owner or master view model reference binding layer.
    /// </summary>
    [JsonIgnore]
    public object Owner { get; set; }
}
