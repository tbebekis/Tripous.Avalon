namespace Tripous.Data;

/// <summary>
/// Describes a field that participates in a <see cref="Data.LocatorDef"/>.
/// </summary>
public class LocatorFieldDef : BaseDef
{
    // ● private
    DataFieldType fDataType = DataFieldType.String;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorFieldDef()
    {
    }

    // ● properties
    /// <summary>
    /// The locator definition this field belongs to.
    /// </summary>
    [JsonIgnore]
    public LocatorDef LocatorDef { get; set; }
    /// <summary>
    /// The data type of the field.
    /// </summary>
    public DataFieldType DataType
    {
        get => fDataType;
        set { if (fDataType != value) { fDataType = value; NotifyPropertyChanged(nameof(DataType)); } }
    }
}

/// <summary>
/// Declarative definition of a locator resolution process.
/// </summary>
public class LocatorDef : BaseDef
{
    // ● private
    string fClassName;
    string fSource;
    string fKeyField;
    string fForm;
    string fWebForm;
    string fOrderBy;
    DefList<LocatorFieldDef> fFields;
    List<string> fSingleRowSearchFields;
    List<string> fMultiRowSearchFields;
    List<string> fResultFields;
    int fMinimumSearchLength;
    int fMaximumResultCount;

    // ● private methods
    void AddNames(List<string> List, params string[] Names)
    {
        foreach (string Name in Names)
        {
            if (!string.IsNullOrWhiteSpace(Name) && !List.Any(x => x.IsSameText(Name)))
                List.Add(Name);
        }
    }
    List<string> GetStringFieldNames()
    {
        List<string> Result = [];

        foreach (LocatorFieldDef FieldDef in Fields)
        {
            if (!FieldDef.Name.IsSameText(KeyField) && FieldDef.DataType == DataFieldType.String)
                Result.Add(FieldDef.Name);
        }

        return Result;
    }
    void CheckFieldExists(string FieldName, string ListName)
    {
        if (!Fields.Contains(FieldName))
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} {ListName} field not found: {FieldName}");
    }
    void CheckFieldsExist(IEnumerable<string> FieldNames, string ListName)
    {
        foreach (string FieldName in FieldNames)
            CheckFieldExists(FieldName, ListName);
    }

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorDef()
    {
    }

    // ● public
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined.
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(Source))
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no {nameof(Source)}.");

        if (string.IsNullOrWhiteSpace(KeyField))
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no {nameof(KeyField)}.");

        if (Fields.Count == 0)
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no {nameof(Fields)}.");

        CheckFieldExists(KeyField, nameof(KeyField));
        CheckFieldsExist(GetSearchFields(IsMultiRow: false), nameof(SingleRowSearchFields));
        CheckFieldsExist(GetSearchFields(IsMultiRow: true), nameof(MultiRowSearchFields));
        CheckFieldsExist(GetResultFields(), nameof(ResultFields));
    }
    /// <summary>
    /// Updates references such as when an instance has references to other instances.
    /// </summary>
    public override void UpdateReferences()
    {
        base.UpdateReferences();

        foreach (LocatorFieldDef FieldDef in Fields)
            FieldDef.LocatorDef = this;
    }
    /// <summary>
    /// Adds a field to the locator.
    /// </summary>
    public LocatorFieldDef Add(string Name, DataFieldType DataType)
    {
        LocatorFieldDef Result = Fields.FindOrdAdd(Name);
        Result.DataType = DataType;
        Result.LocatorDef = this;
        return Result;
    }
    /// <summary>
    /// Adds a string field to the locator.
    /// </summary>
    public LocatorFieldDef Add(string Name)
    {
        return Add(Name, DataFieldType.String);
    }
    /// <summary>
    /// Adds string fields to the locator.
    /// </summary>
    public void AddFields(params string[] Names)
    {
        foreach (string Name in Names)
            Add(Name);
    }
    /// <summary>
    /// Adds fields to both single-row and multi-row search field lists.
    /// </summary>
    public void AddSearchFields(params string[] Names)
    {
        AddSingleRowSearchFields(Names);
        AddMultiRowSearchFields(Names);
    }
    /// <summary>
    /// Adds fields to the single-row search field list.
    /// </summary>
    public void AddSingleRowSearchFields(params string[] Names)
    {
        AddNames(SingleRowSearchFields, Names);
    }
    /// <summary>
    /// Adds fields to the multi-row search field list.
    /// </summary>
    public void AddMultiRowSearchFields(params string[] Names)
    {
        AddNames(MultiRowSearchFields, Names);
    }
    /// <summary>
    /// Adds fields to the result field list.
    /// </summary>
    public void AddResultFields(params string[] Names)
    {
        AddNames(ResultFields, Names);
    }
    /// <summary>
    /// Returns the search fields to use for single-row or multi-row locator resolution.
    /// </summary>
    public List<string> GetSearchFields(bool IsMultiRow)
    {
        if (IsMultiRow)
            return MultiRowSearchFields.Count > 0 ? [.. MultiRowSearchFields] : GetSearchFields(IsMultiRow: false);

        return SingleRowSearchFields.Count > 0 ? [.. SingleRowSearchFields] : GetStringFieldNames();
    }
    /// <summary>
    /// Returns the union of all search fields.
    /// </summary>
    public List<string> GetAllSearchFields()
    {
        List<string> Result = [];
        AddNames(Result, [.. GetSearchFields(IsMultiRow: false)]);
        AddNames(Result, [.. GetSearchFields(IsMultiRow: true)]);
        return Result;
    }
    /// <summary>
    /// Returns the result fields.
    /// </summary>
    public List<string> GetResultFields()
    {
        if (ResultFields.Count > 0)
            return [.. ResultFields];

        List<string> Result = [];
        AddNames(Result, KeyField);

        foreach (LocatorFieldDef FieldDef in Fields)
            AddNames(Result, FieldDef.Name);

        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the class name of the locator implementation.
    /// </summary>
    public string ClassName
    {
        get => !string.IsNullOrWhiteSpace(fClassName) ? fClassName : typeof(Locator).FullName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
    /// <summary>
    /// Gets or sets the provider-neutral source reference used by the locator.
    /// <para>Depending on the locator implementation it may be a table name, SELECT statement, service URL, object list name, or any other source reference.</para>
    /// </summary>
    public string Source
    {
        get => !string.IsNullOrWhiteSpace(fSource) ? fSource : Name;
        set { if (fSource != value) { fSource = value; NotifyPropertyChanged(nameof(Source)); } }
    }
    /// <summary>
    /// Gets or sets the field that provides the canonical identity returned by this locator.
    /// </summary>
    public string KeyField
    {
        get => !string.IsNullOrWhiteSpace(fKeyField) ? fKeyField : "Id";
        set { if (fKeyField != value) { fKeyField = value; NotifyPropertyChanged(nameof(KeyField)); } }
    }
    /// <summary>
    /// Gets or sets the desktop form name used by reference menus.
    /// </summary>
    public string Form
    {
        get => !string.IsNullOrWhiteSpace(fForm) ? fForm : Name;
        set { if (fForm != value) { fForm = value; NotifyPropertyChanged(nameof(Form)); } }
    }
    /// <summary>
    /// Gets or sets the web form name used by web reference menus.
    /// </summary>
    public string WebForm
    {
        get => !string.IsNullOrWhiteSpace(fWebForm) ? fWebForm : Form;
        set { if (fWebForm != value) { fWebForm = value; NotifyPropertyChanged(nameof(WebForm)); } }
    }
    /// <summary>
    /// Gets or sets the ORDER BY clause.
    /// </summary>
    public string OrderBy
    {
        get => fOrderBy;
        set { if (fOrderBy != value) { fOrderBy = value; NotifyPropertyChanged(nameof(OrderBy)); } }
    }
    /// <summary>
    /// Gets or sets the fields that may participate in locator input, output or display.
    /// </summary>
    public DefList<LocatorFieldDef> Fields
    {
        get => fFields ??= new();
        set { if (fFields != value) { fFields = value; NotifyPropertyChanged(nameof(Fields)); } }
    }
    /// <summary>
    /// Gets or sets the business reference fields used for resolution by a single-row locator UI.
    /// </summary>
    public List<string> SingleRowSearchFields
    {
        get => fSingleRowSearchFields ??= [];
        set { if (fSingleRowSearchFields != value) { fSingleRowSearchFields = value; NotifyPropertyChanged(nameof(SingleRowSearchFields)); } }
    }
    /// <summary>
    /// Gets or sets the business reference fields used for resolution by a multi-row locator UI.
    /// </summary>
    public List<string> MultiRowSearchFields
    {
        get => fMultiRowSearchFields ??= [];
        set { if (fMultiRowSearchFields != value) { fMultiRowSearchFields = value; NotifyPropertyChanged(nameof(MultiRowSearchFields)); } }
    }
    /// <summary>
    /// Gets or sets the fields returned as locator output.
    /// <para>These fields serve both as result display fields when resolution returns multiple results and as projection fields when a result is selected or resolved.</para>
    /// </summary>
    public List<string> ResultFields
    {
        get => fResultFields ??= [];
        set { if (fResultFields != value) { fResultFields = value; NotifyPropertyChanged(nameof(ResultFields)); } }
    }
    /// <summary>
    /// Gets or sets the minimum search text length required before resolution may run.
    /// </summary>
    public int MinimumSearchLength
    {
        get => fMinimumSearchLength > 0 ? fMinimumSearchLength : Db.Settings.LocatorMinimumSearchTextLength;
        set { if (fMinimumSearchLength != value) { fMinimumSearchLength = value; NotifyPropertyChanged(nameof(MinimumSearchLength)); } }
    }
    /// <summary>
    /// Gets or sets the maximum number of result rows returned before a result is considered too broad.
    /// </summary>
    public int MaximumResultCount
    {
        get => fMaximumResultCount > 0 ? fMaximumResultCount : Db.Settings.LocatorMaximumDropDownRows;
        set { if (fMaximumResultCount != value) { fMaximumResultCount = value; NotifyPropertyChanged(nameof(MaximumResultCount)); } }
    }
}

/// <summary>
/// Status of a locator resolution operation.
/// </summary>
public enum LocatorResultStatus
{
    /// <summary>
    /// No status has been assigned.
    /// </summary>
    None,
    /// <summary>
    /// The locator request is invalid.
    /// </summary>
    InvalidRequest,
    /// <summary>
    /// The locator context is invalid.
    /// </summary>
    InvalidContext,
    /// <summary>
    /// No result was found.
    /// </summary>
    NoResult,
    /// <summary>
    /// A single result was found.
    /// </summary>
    SingleResult,
    /// <summary>
    /// Multiple results were found.
    /// </summary>
    MultipleResults,
    /// <summary>
    /// Too many results were found.
    /// </summary>
    TooManyResults,
    /// <summary>
    /// An error occurred.
    /// </summary>
    Error,
}

/// <summary>
/// The kind of result list returned by a locator resolution operation.
/// </summary>
public enum LocatorResultListKind
{
    /// <summary>
    /// No result list is assigned.
    /// </summary>
    None,
    /// <summary>
    /// The result list is a <see cref="MemTable"/>.
    /// </summary>
    MemTable,
    /// <summary>
    /// The result list is an object list.
    /// </summary>
    ObjectList,
}

/// <summary>
/// Context of a locator resolution operation.
/// </summary>
public class LocatorContext
{
    // ● private
    string fLocatorName;
    Dictionary<string, object> fParams;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorContext()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorContext(string LocatorName)
    {
        this.LocatorName = LocatorName;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the locator name.
    /// </summary>
    public string LocatorName
    {
        get => fLocatorName;
        set => fLocatorName = value;
    }
    /// <summary>
    /// Gets the context parameters.
    /// <para>Use this dictionary for runtime execution hints that are not part of the locator descriptor identity, e.g. a <c>ConnectionName</c> override.</para>
    /// </summary>
    public Dictionary<string, object> Params => fParams ??= [];
}

/// <summary>
/// Request for a locator resolution operation.
/// </summary>
public class LocatorRequest
{
    // ● private
    object fKeyValue;
    string fSearchTerm;
    string fSearchField;
    LocatorContext fContext;
    bool fIsMultiRow;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorRequest()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the key value to resolve.
    /// <para>When specified, the locator resolves by exact key and ignores <see cref="SearchTerm"/>.</para>
    /// </summary>
    public object KeyValue
    {
        get => fKeyValue;
        set => fKeyValue = value;
    }
    /// <summary>
    /// Gets or sets the search term.
    /// </summary>
    public string SearchTerm
    {
        get => fSearchTerm;
        set => fSearchTerm = value;
    }
    /// <summary>
    /// Gets or sets the search field.
    /// </summary>
    public string SearchField
    {
        get => fSearchField;
        set => fSearchField = value;
    }
    /// <summary>
    /// Gets or sets a value indicating whether the request is for a multi-row locator.
    /// </summary>
    public bool IsMultiRow
    {
        get => fIsMultiRow;
        set => fIsMultiRow = value;
    }
    /// <summary>
    /// Gets or sets the locator context.
    /// </summary>
    public LocatorContext Context
    {
        get => fContext ??= new();
        set => fContext = value;
    }
}

/// <summary>
/// Result of a locator resolution operation.
/// </summary>
public class LocatorResult
{
    // ● private
    LocatorResultStatus fStatus;
    string fMessage;
    MemTable fTable;
    IList fObjectList;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorResult()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the result status.
    /// </summary>
    public LocatorResultStatus Status
    {
        get => fStatus;
        set => fStatus = value;
    }
    /// <summary>
    /// Gets or sets a message related to the result.
    /// </summary>
    public string Message
    {
        get => fMessage;
        set => fMessage = value;
    }
    /// <summary>
    /// Gets or sets the result table.
    /// </summary>
    public MemTable Table
    {
        get => fTable;
        set => fTable = value;
    }
    /// <summary>
    /// Gets the result table view.
    /// </summary>
    public DataView View => Table != null ? Table.DataView : null;
    /// <summary>
    /// Gets or sets the object result list.
    /// </summary>
    public IList ObjectList
    {
        get => fObjectList;
        set => fObjectList = value;
    }
    /// <summary>
    /// Gets the kind of result list returned by the operation.
    /// </summary>
    public LocatorResultListKind ListKind => Table != null ? LocatorResultListKind.MemTable : ObjectList != null ? LocatorResultListKind.ObjectList : LocatorResultListKind.None;
    /// <summary>
    /// Gets the result count.
    /// </summary>
    public int Count => Table != null ? Table.Rows.Count : ObjectList != null ? ObjectList.Count : 0;
    /// <summary>
    /// Gets true when the result has a single row.
    /// </summary>
    public bool HasSingleResult => Status == LocatorResultStatus.SingleResult && Count == 1;
    /// <summary>
    /// Gets true when the result has multiple rows.
    /// </summary>
    public bool HasMultipleResults => Status == LocatorResultStatus.MultipleResults && Count > 1;
    /// <summary>
    /// Gets true when the result is too broad.
    /// </summary>
    public bool HasTooManyResults => Status == LocatorResultStatus.TooManyResults;
    /// <summary>
    /// Gets true when the result is an error.
    /// </summary>
    public bool HasError => Status == LocatorResultStatus.Error;
}

/// <summary>
/// A locator mapping item.
/// </summary>
public class LocatorMapItem
{
    // ● private
    string fSourceField;
    string fTargetField;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorMapItem()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorMapItem(string SourceField, string TargetField)
    {
        this.SourceField = SourceField;
        this.TargetField = TargetField;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the source field name.
    /// </summary>
    public string SourceField
    {
        get => fSourceField;
        set => fSourceField = value;
    }
    /// <summary>
    /// Gets or sets the target field name.
    /// </summary>
    public string TargetField
    {
        get => fTargetField;
        set => fTargetField = value;
    }
}

/// <summary>
/// A locator mapping plan.
/// </summary>
public class LocatorMapPlan
{
    // ● private
    string fLocatorName;
    string fReferenceField;
    List<LocatorMapItem> fItems;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorMapPlan()
    {
    }

    // ● public
    /// <summary>
    /// Adds a mapping item.
    /// </summary>
    public LocatorMapItem Add(string SourceField, string TargetField)
    {
        LocatorMapItem Result = new(SourceField, TargetField);
        Items.Add(Result);
        return Result;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the locator name.
    /// </summary>
    public string LocatorName
    {
        get => fLocatorName;
        set => fLocatorName = value;
    }
    /// <summary>
    /// Gets or sets the reference field name.
    /// </summary>
    public string ReferenceField
    {
        get => fReferenceField;
        set => fReferenceField = value;
    }
    /// <summary>
    /// Gets the mapping items.
    /// </summary>
    public List<LocatorMapItem> Items => fItems ??= [];
}

/// <summary>
/// Creates locator mapping plans.
/// </summary>
public class LocatorMapper
{
    // ● protected methods
    /// <summary>
    /// Sets a target row value.
    /// </summary>
    protected virtual void SetTargetRowValue(DataRow TargetRow, string TargetField, object Value)
    {
        if (TargetRow == null || string.IsNullOrWhiteSpace(TargetField))
            return;

        DataColumn Column = TargetRow.Table.FindColumn(TargetField);
        if (Column == null || Column.ReadOnly)
            return;

        object NewValue = Sys.IsNull(Value) ? DBNull.Value : Value;
        if (Sys.IsNull(NewValue) && !Column.AllowDBNull)
            return;

        TargetRow[Column] = NewValue;
    }
    /// <summary>
    /// Returns a source row value.
    /// </summary>
    protected virtual object GetSourceRowValue(DataRow SourceRow, string SourceField)
    {
        if (SourceRow == null || string.IsNullOrWhiteSpace(SourceField))
            return DBNull.Value;

        DataColumn Column = SourceRow.Table.FindColumn(SourceField);
        return Column != null ? SourceRow[Column] : DBNull.Value;
    }
    /// <summary>
    /// Finds a snapshot field for a join field.
    /// </summary>
    protected virtual FieldDef FindSnapshotField(TableDef TargetTable, TableDef JoinTable, FieldDef JoinField)
    {
        if (TargetTable == null || JoinTable == null || JoinField == null)
            return null;

        foreach (FieldDef Field in TargetTable.Fields)
        {
            if (string.IsNullOrWhiteSpace(Field.SnapshotOf))
                continue;

            string[] Parts = Field.SnapshotOf.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (Parts.Length == 2 && Parts[0].IsSameText(JoinTable.Alias) && Parts[1].IsSameText(JoinField.Name))
                return Field;
        }

        return null;
    }
    /// <summary>
    /// Finds a target field for a locator result field.
    /// </summary>
    protected virtual FieldDef FindTargetField(TableDef TargetTable, FieldDef ReferenceField, string SourceField)
    {
        if (TargetTable == null || ReferenceField == null || string.IsNullOrWhiteSpace(SourceField))
            return null;

        TableDef JoinTable = TargetTable.FindJoinTableByMasterKeyField(ReferenceField.Name);
        FieldDef JoinField = JoinTable?.Fields.FirstOrDefault(item => item.Name.IsSameText(SourceField) || item.Alias.IsSameText(SourceField));
        if (JoinField != null)
            return FindSnapshotField(TargetTable, JoinTable, JoinField) ?? JoinField;

        return TargetTable.Fields.FirstOrDefault(item => item.Name.IsSameText(SourceField) || item.Alias.IsSameText(SourceField));
    }

    // ● public
    /// <summary>
    /// Creates a locator mapping plan.
    /// </summary>
    public virtual LocatorMapPlan CreatePlan(LocatorDef LocatorDef, TableDef TargetTable, FieldDef ReferenceField)
    {
        LocatorMapPlan Result = new()
        {
            LocatorName = LocatorDef?.Name,
            ReferenceField = ReferenceField?.Name,
        };

        if (LocatorDef == null || TargetTable == null || ReferenceField == null)
            return Result;

        Result.Add(LocatorDef.KeyField, ReferenceField.Name);

        foreach (string SourceField in LocatorDef.GetResultFields())
        {
            if (SourceField.IsSameText(LocatorDef.KeyField))
                continue;

            FieldDef TargetField = FindTargetField(TargetTable, ReferenceField, SourceField);
            if (TargetField != null)
                Result.Add(SourceField, TargetField.Alias);
        }

        return Result;
    }
    /// <summary>
    /// Applies a locator mapping plan to a target row.
    /// </summary>
    public virtual void Apply(LocatorMapPlan Plan, DataRow SourceRow, DataRow TargetRow)
    {
        if (Plan == null || TargetRow == null)
            return;

        foreach (LocatorMapItem Item in Plan.Items)
        {
            object Value = GetSourceRowValue(SourceRow, Item.SourceField);
            SetTargetRowValue(TargetRow, Item.TargetField, Value);
        }
    }
}

/// <summary>
/// Runtime locator.
/// </summary>
[TypeStore]
public class Locator
{
    // ● protected methods
    /// <summary>
    /// Returns true when the specified source is a SELECT statement.
    /// </summary>
    protected virtual bool IsSelectSource(string Source) => !string.IsNullOrWhiteSpace(Source) && Source.TrimStart().StartsWith("select", StringComparison.OrdinalIgnoreCase);
    /// <summary>
    /// Returns the result field list as SQL text.
    /// </summary>
    protected virtual string GetSqlResultFields(LocatorDef LocatorDef)
    {
        List<string> FieldNames = LocatorDef.GetResultFields();
        return FieldNames.Count > 0 ? string.Join(", ", FieldNames) : "*";
    }
    /// <summary>
    /// Returns the base SELECT statement.
    /// </summary>
    protected virtual string GetBaseSql(LocatorDef LocatorDef)
    {
        string ResultFields = GetSqlResultFields(LocatorDef);
        return IsSelectSource(LocatorDef.Source) ? $"select {ResultFields} from ({LocatorDef.Source}) X" : $"select {ResultFields} from {LocatorDef.Source}";
    }
    /// <summary>
    /// Returns the search field names to use.
    /// </summary>
    protected virtual List<string> GetSearchFieldNames(LocatorDef LocatorDef, LocatorRequest Request)
    {
        if (!string.IsNullOrWhiteSpace(Request.SearchField))
            return [Request.SearchField];

        return LocatorDef.GetSearchFields(Request.IsMultiRow);
    }
    /// <summary>
    /// Returns the key field definition.
    /// </summary>
    protected virtual LocatorFieldDef GetKeyFieldDef(LocatorDef LocatorDef)
    {
        return LocatorDef.Fields.Find(LocatorDef.KeyField);
    }
    /// <summary>
    /// Returns a SQL value literal.
    /// </summary>
    protected virtual string GetSqlValue(object Value, DataFieldType DataType)
    {
        if (Sys.IsNull(Value))
            return "NULL";

        switch (DataType)
        {
            case DataFieldType.Integer:
            case DataFieldType.Double:
            case DataFieldType.Decimal:
            case DataFieldType.Decimal_:
                return Convert.ToString(Value, CultureInfo.InvariantCulture);
            case DataFieldType.Boolean:
                return Convert.ToBoolean(Value) ? "1" : "0";
            case DataFieldType.Date:
                return Convert.ToDateTime(Value).ToString("yyyy-MM-dd").QS();
            case DataFieldType.DateTime:
                return Convert.ToDateTime(Value).ToString("yyyy-MM-dd HH:mm:ss").QS();
            default:
                return Value.ToString().Replace("'", "''").QS();
        }
    }
    /// <summary>
    /// Returns the SQL WHERE clause for a key value.
    /// </summary>
    protected virtual string GetKeyWhereSql(LocatorDef LocatorDef, LocatorRequest Request)
    {
        LocatorFieldDef FieldDef = GetKeyFieldDef(LocatorDef);
        DataFieldType DataType = FieldDef != null ? FieldDef.DataType : DataFieldType.String;
        return $"{LocatorDef.KeyField} = {GetSqlValue(Request.KeyValue, DataType)}";
    }
    /// <summary>
    /// Returns the SQL WHERE clause for a search term.
    /// </summary>
    protected virtual string GetWhereSql(LocatorDef LocatorDef, LocatorRequest Request)
    {
        if (!Sys.IsNull(Request.KeyValue))
            return GetKeyWhereSql(LocatorDef, Request);

        if (string.IsNullOrWhiteSpace(Request.SearchTerm))
            return string.Empty;

        string Term = Request.SearchTerm.Replace("'", "''");
        List<string> Items = [];

        foreach (string FieldName in GetSearchFieldNames(LocatorDef, Request))
        {
            LocatorFieldDef FieldDef = LocatorDef.Fields.Find(FieldName);
            if (FieldDef != null && FieldDef.DataType == DataFieldType.String)
                Items.Add($"{FieldDef.Name} like '%{Term}%'");
        }

        return Items.Count > 0 ? string.Join(" or ", Items) : string.Empty;
    }
    /// <summary>
    /// Returns the SELECT statement to execute.
    /// </summary>
    protected virtual string GetSql(LocatorDef LocatorDef, LocatorRequest Request)
    {
        string Result = GetBaseSql(LocatorDef);
        string Where = GetWhereSql(LocatorDef, Request);

        if (!string.IsNullOrWhiteSpace(Where))
            Result = $"select * from ({Result}) X where {Where}";

        if (!string.IsNullOrWhiteSpace(LocatorDef.OrderBy))
            Result += $" order by {LocatorDef.OrderBy}";

        return Store.Provider.ApplyRowLimit(Result, LocatorDef.MaximumResultCount + 1);
    }
    /// <summary>
    /// Returns an invalid request result.
    /// </summary>
    protected virtual LocatorResult InvalidRequest(string Message)
    {
        return new LocatorResult()
        {
            Status = LocatorResultStatus.InvalidRequest,
            Message = Message,
        };
    }
    /// <summary>
    /// Returns the status for a row count.
    /// </summary>
    protected virtual LocatorResultStatus GetStatus(LocatorDef LocatorDef, int RowCount)
    {
        if (RowCount == 0)
            return LocatorResultStatus.NoResult;
        if (RowCount == 1)
            return LocatorResultStatus.SingleResult;
        if (RowCount > LocatorDef.MaximumResultCount)
            return LocatorResultStatus.TooManyResults;
        return LocatorResultStatus.MultipleResults;
    }
    /// <summary>
    /// Checks the specified request and returns an error result if it is invalid.
    /// </summary>
    protected virtual LocatorResult CheckRequest(LocatorDef LocatorDef, LocatorRequest Request)
    {
        if (Sys.IsNull(Request.KeyValue) && !string.IsNullOrWhiteSpace(Request.SearchTerm) && Request.SearchTerm.Length < LocatorDef.MinimumSearchLength)
            return InvalidRequest($"Locator search term must contain at least {LocatorDef.MinimumSearchLength} characters.");

        if (!string.IsNullOrWhiteSpace(Request.SearchField) && !LocatorDef.GetAllSearchFields().Any(x => x.IsSameText(Request.SearchField)))
            return InvalidRequest($"Locator search field not found: {Request.SearchField}");

        return null;
    }

    // ● public
    /// <summary>
    /// Executes a locator request.
    /// </summary>
    public virtual LocatorResult Execute(LocatorDef LocatorDef, LocatorRequest Request)
    {
        LocatorResult Result = CheckRequest(LocatorDef, Request);
        if (Result != null)
            return Result;

        MemTable Table = new(LocatorDef.Name);
        int RowCount = Store.SelectTo(Table, GetSql(LocatorDef, Request));
        LocatorResultStatus Status = GetStatus(LocatorDef, RowCount);

        return new LocatorResult()
        {
            Status = Status,
            Message = Status == LocatorResultStatus.TooManyResults ? "Too many results. Type more characters." : string.Empty,
            Table = Status == LocatorResultStatus.TooManyResults ? null : Table,
        };
    }

    // ● properties
    /// <summary>
    /// Returns the SQL store to use.
    /// </summary>
    protected virtual SqlStore Store => Db.DefaultStore;
}

/// <summary>
/// Static locator service.
/// </summary>
static public class Locators
{
    // ● public
    /// <summary>
    /// Executes a locator request.
    /// </summary>
    static public LocatorResult Execute(LocatorRequest Request)
    {
        if (Request == null)
            throw new TripousArgumentNullException(nameof(Request));
        if (Request.Context == null)
            throw new TripousDataException($"{nameof(LocatorRequest)} has no {nameof(LocatorRequest.Context)}.");
        if (string.IsNullOrWhiteSpace(Request.Context.LocatorName))
            throw new TripousDataException($"{nameof(LocatorRequest)} has no {nameof(LocatorContext.LocatorName)}.");

        LocatorDef LocatorDef = DataRegistry.GetLocator(Request.Context.LocatorName);
        LocatorDef.CheckDescriptor();

        Locator Locator = TypeStore.CreateInstance<Locator>(LocatorDef.ClassName);
        return Locator.Execute(LocatorDef, Request);
    }
}
