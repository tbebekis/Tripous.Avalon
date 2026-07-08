namespace Tripous.Data;

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
    List<string> fListVisibleFields;
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
    void CheckFieldsInResultFields(IEnumerable<string> FieldNames, string ListName)
    {
        List<string> ResultFields = GetResultFields();

        foreach (string FieldName in FieldNames)
        {
            if (!ResultFields.Any(item => item.IsSameText(FieldName)))
                throw new TripousDataException($"{nameof(LocatorDef)} {Name} {ListName} field is not in {nameof(ResultFields)}: {FieldName}");
        }
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

        if (ResultFields.Count == 0)
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no {nameof(ResultFields)}.");

        CheckFieldExists(KeyField, nameof(KeyField));
        CheckFieldsExist(GetSearchFields(IsMultiRow: false), nameof(SingleRowSearchFields));
        CheckFieldsExist(GetSearchFields(IsMultiRow: true), nameof(MultiRowSearchFields));
        CheckFieldsExist(GetResultFields(), nameof(ResultFields));
        CheckFieldsInResultFields(GetListVisibleFields(), nameof(ListVisibleFields));
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
    /// Adds fields to the list-visible field list.
    /// </summary>
    public void AddListVisibleFields(params string[] Names)
    {
        AddNames(ListVisibleFields, Names);
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
        return [.. ResultFields];
    }
    /// <summary>
    /// Returns the fields displayed by locator list UIs.
    /// </summary>
    public List<string> GetListVisibleFields()
    {
        return ListVisibleFields.Count > 0 ? [.. ListVisibleFields] : GetResultFields();
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
    /// </summary>
    public List<string> ResultFields
    {
        get => fResultFields ??= [];
        set { if (fResultFields != value) { fResultFields = value; NotifyPropertyChanged(nameof(ResultFields)); } }
    }
    /// <summary>
    /// Gets or sets the fields displayed by locator list UIs.
    /// </summary>
    public List<string> ListVisibleFields
    {
        get => fListVisibleFields ??= [];
        set { if (fListVisibleFields != value) { fListVisibleFields = value; NotifyPropertyChanged(nameof(ListVisibleFields)); } }
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
