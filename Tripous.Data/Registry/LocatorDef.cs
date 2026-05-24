namespace Tripous.Data;

/// <summary>
/// Describes a <see cref="Locator"/>, i.e. a searchable selector for large reference tables.
/// <para>A locator represents (returns) a single value from the source <see cref="SourceTableName"/>, but it can handle and display multiple values
/// in order to help the end user in identifying and locating that single value.</para>
/// <para>For example, a TRADES target data table has a CUSTOMER_ID column, representing that single value, but the user interface
/// has to display information from the CUSTOMERS source table, specifically, the ID, CODE and NAME columns.</para>
/// <para>The TRADES table is the <b>target data table</b> and the CUSTOMER_ID is the <b>target field name</b>.</para>
/// <para>The CUSTOMERS source table is the <see cref="SourceTableName"/> and the ID is the source <see cref="KeyField"/> field name.</para>
/// <para>The fields, ID, CODE and NAME, may be described by individual <see cref="LocatorFieldDef"/> field items.</para>
/// <para>A locator can be used either as a single-row control, i.e. as a locator control, or as a group of
/// related columns in a Grid.</para>
/// <para>NOTE: A locator of a locator control type, may or may not define the <see cref="LocatorFieldDef.TargetField"/> 
/// field names. Usually in a case like that, the target data table contains just the target key field, the field the locator control is bound to.  </para>
/// <para>A locator of a grid-type must define the names of those target fields always and the target data table must contain DataColumn columns
/// of those fields.</para>
/// </summary>
public class LocatorDef: BaseDef
{
    // ● private fields
    string fSourceTableName;
    string fKeyField;
    string fClassName;
    string fForm;
    string fConnectionName;
    string fSelectSql;
    string fOrderBy;
    bool fIsReadOnly;
    
    DefList<LocatorFieldDef> fFields;

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public LocatorDef()
    {
    }

    // ● public methods
    /// <summary>
    /// Creates and returns a <see cref="Locator"/>
    /// </summary>
    public Locator Create()
    {
        Locator Result = TypeStore.CreateInstance<Locator>(ClassName);
        Result.Initialize(this);
        return Result;
    }
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(SourceTableName))
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no {nameof(SourceTableName)}.");

        if (string.IsNullOrWhiteSpace(KeyField))
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no KeyField.");

        if (Fields.Count == 0)
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no Fields.");
    }
    /// <summary>
    /// Updates references such as when an instance has references to other instances, e.g. tables of a module definition.
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
    public LocatorFieldDef Add(string Name, DataFieldType DataType, string TargetField, string Alias, string TitleKey, bool IsVisible, bool IsSearchable)
    {
        LocatorFieldDef Result = Fields.FindOrdAdd(Name);
        Result.DataType = DataType;
        Result.TargetField = TargetField;
        Result.Alias = Alias;
        Result.TitleKey = TitleKey;
        Result.IsVisible = IsVisible;
        Result.IsSearchable = IsSearchable;
        return Result;
    }
    /// <summary>
    /// Adds a field to the locator.
    /// </summary>
    public LocatorFieldDef Add(string Name, DataFieldType DataType, string TargetField)
    {
        return Add(Name, DataType, TargetField, Alias: null, TitleKey: null, IsVisible: true, IsSearchable: true);
    }
    /// <summary>
    /// Adds a field to the locator.
    /// </summary>
    public LocatorFieldDef Add(string Name, DataFieldType DataType)
    {
        return Add(Name, DataType, TargetField: null, Alias: null, TitleKey: null, IsVisible: true, IsSearchable: true);
    }
    /// <summary>
    /// Adds a string field to the locator.
    /// </summary>
    public LocatorFieldDef Add(string Name)
    {
        return Add(Name, DataType: DataFieldType.String, TargetField: null, Alias: null, TitleKey: null, IsVisible: true, IsSearchable: true);
    }

    // ● properties
    /// <summary>
    /// The class name of the <see cref="System.Type"/> this descriptor describes.
    /// <para>NOTE: The value of this property may be a string returned by the <see cref="Type.AssemblyQualifiedName"/> property of the type. </para>
    /// <para>In that case, it consists of the type name, including its namespace, followed by a comma, followed by the display name of the assembly
    /// the type belongs to. It might looks like the following</para>
    /// <para><c>Tripous.Data.DataModule, Tripous, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</c></para>
    /// <para>Otherwise it can be a full type name <see cref="Type.FullName"/>, e.g. </para>
    /// <para><c>Tripous.Data.DataModule</c></para>
    /// </summary>
    public string ClassName
    {
        get => !string.IsNullOrWhiteSpace(fClassName)? fClassName: typeof(Locator).FullName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
    /// <summary>
    /// The source table name
    /// </summary>
    public string SourceTableName
    {
        get => fSourceTableName;
        set { if (fSourceTableName != value) { fSourceTableName = value; NotifyPropertyChanged(nameof(SourceTableName)); } }
    }
    /// <summary>
    /// The primary key field of the locator source table, named "Id" in most cases.
    /// </summary>
    public string KeyField
    {
        get => !string.IsNullOrWhiteSpace(fKeyField)? fKeyField: "Id";
        set { if (fKeyField != value) { fKeyField = value; NotifyPropertyChanged(nameof(KeyField)); } }
    }
    
    /// <summary>
    /// Gets or sets the connection name (database)
    /// </summary>
    public string ConnectionName  
    {
        get => !string.IsNullOrWhiteSpace(fConnectionName)? fConnectionName: DbConfig.DefaultConnectionName;
        set { if (fConnectionName != value) { fConnectionName = value; NotifyPropertyChanged(nameof(ConnectionName)); } }
    }
    /// <summary>
    /// The name of a form that displays the table.
    /// </summary>
    public string Form
    {
        get => !string.IsNullOrWhiteSpace(fForm)? fForm: Name;
        set { if (fForm != value) { fForm = value; NotifyPropertyChanged(nameof(Form)); } }
    }
    /// <summary>
    /// The SELECT statement to execute for returning the data.
    /// <para>WARNING: The statement should NOT have a WHERE clause.</para>
    /// </summary>
    public string SelectSql
    {
        get => fSelectSql;
        set { if (fSelectSql != value) { fSelectSql = value; NotifyPropertyChanged(nameof(SelectSql)); } }
    }
    /// <summary>
    /// The ORDER BY clause. Used only when the SELECT Sql is constructed by the Locator.
    /// </summary>
    public string OrderBy
    {
        get => fOrderBy;
        set { if (fOrderBy != value) { fOrderBy = value; NotifyPropertyChanged(nameof(OrderBy)); } }
    }
    /// <summary>
    /// When true then the locator is read-only. It affects the UI only by making locator controls or columns readonly.
    /// </summary>
    public bool IsReadOnly
    {
        get => fIsReadOnly;
        set { if (fIsReadOnly != value) { fIsReadOnly = value; NotifyPropertyChanged(nameof(IsReadOnly)); } }
    }
    /// <summary>
    /// The list of fields
    /// </summary>
    public DefList<LocatorFieldDef> Fields
    {
        get => fFields ??= new();
        set { if (fFields != value) { fFields = value; NotifyPropertyChanged(nameof(Fields)); } }
    }

}
 