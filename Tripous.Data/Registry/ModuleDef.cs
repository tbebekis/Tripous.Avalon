namespace Tripous.Data;

public class ModuleDef: BaseDef, IJsonLoadable
{
   string fClassName = typeof(DataModule).FullName;
   string fDescription;
   List<SelectDef> fSelectList;
   List<SelectDef> fStocks;
   TableDef fTable = new();
   string fConnectionName = SysConfig.DefaultConnectionName;
   bool fIsListModule;
   bool fGuidOids = true;
   bool fCascadeDeletes = true;

    // ● public
    public DataModule Create()
    {
        DataModule Result = TypeResolver.CreateInstance<DataModule>(ClassName);
        Result.Initialize(this);
        return Result;
    }
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(this.ClassName))
            Sys.Throw(Texts.GS($"E_{typeof(ModuleDef)}_ClassNameIsEmpty", $"{typeof(ModuleDef)} ClassName is empty."));

        if (Table == null)
            Sys.Throw(Texts.GS($"E_{typeof(ModuleDef)}_NoTopTable", $"{typeof(ModuleDef)} No Top Table is defined."));
    }
    public void JsonLoaded()
    {
        UpdateReferences();
    }
    /// <summary>
    /// Updates references such as when an instance has references to other instances, e.g. tables of a module definition.
    /// </summary>
    public override void UpdateReferences()
    {
        foreach (SelectDef SelectDef in SelectList)
            SelectDef.Owner = this;
        
        foreach (SelectDef StockDef in Stocks)
            StockDef.Owner = this;

        Table.ModuleDef = this;
        Table.UpdateReferences();
    }
 
    /// <summary>
    /// Returns all tables in a flat list
    /// </summary>
    public List<TableDef> GetTables()
    {
        UpdateReferences();
        
        List<TableDef> List = new();
        
        void AddTable(TableDef T)
        {
            if (T == null)
                return;

            List.Add(T);

            if (T.Details != null)
                foreach (var Item in T.Details)
                    AddTable(Item);
        }

        AddTable(Table);
        return List;
    }
    /// <summary>
    /// Ensures that any TableDef is updated with the actual table schema from the database.
    /// </summary>
    public void UpdateTableSchema(SqlStore Store)
    {
        UpdateReferences();
        
        DataTable SchemaTable;
        void UpdateSchema(TableDef T)
        {
            SchemaTable = Store.GetNativeSchemaFromTableName($"{Name}.{T.Name}", T.Name);
            T.UpdateFrom(SchemaTable);
            
            if (T.Details != null)
                foreach (var Item in T.Details)
                    UpdateSchema(Item);
        }

        UpdateSchema(Table);
    }
    
    // ● properties
    /// <summary>
    /// Gets or sets the class name of the <see cref="System.Type"/> this descriptor describes.
    /// <para>NOTE: The value of this property may be a string returned by the <see cref="Type.AssemblyQualifiedName"/> property of the type. </para>
    /// <para>In that case, it consists of the type name, including its namespace, followed by a comma, followed by the display name of the assembly
    /// the type belongs to. It might looks like the following</para>
    /// <para><c>Tripous.Data.DataModule, Tripous, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</c></para>
    /// <para>Otherwise it can be a full type name, e.g. </para>
    /// <para><c>Tripous.Data.DataModule</c></para>
    /// </summary>
    public string ClassName
    {
        get => fClassName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
    /// <summary>
    /// When true then this is a list module, i.e. has not a table with the single data row. All editing is done in the list table. 
    /// </summary>
    public bool IsListModule
    {
        get => fIsListModule;
        set { if (fIsListModule != value) { fIsListModule = value; NotifyPropertyChanged(nameof(IsListModule)); } }
    }
    /// <summary>
    /// Gets or sets the connection name (database)
    /// </summary>
    public string ConnectionName  
    {
        get => !string.IsNullOrWhiteSpace(fConnectionName)? fConnectionName: SysConfig.DefaultConnectionName;
        set { if (fConnectionName != value) { fConnectionName = value; NotifyPropertyChanged(nameof(ConnectionName)); } }
    }
    /// <summary>
    /// An optional description
    /// </summary>
    public string Description
    {
        get => fDescription;
        set { if (fDescription != value) { fDescription = value; NotifyPropertyChanged(nameof(Description)); } }
    }
    /// <summary>
    /// When is true indicates that the OID is a Guid string.  
    /// </summary>
    public bool GuidOids 
    {
        get => fGuidOids;
        set { if (fGuidOids != value) { fGuidOids = value; NotifyPropertyChanged(nameof(GuidOids)); } }
    }
    /// <summary>
    /// When true indicates that deletes should happen bottom to top, i.e. starting from the bottom table.
    /// When false indicates that deletes should happen top to bottom, so if any database foreign constraint exists, then let
    /// an exception to be thrown. Defaults to true.
    /// </summary>
    public bool CascadeDeletes {
        get => fCascadeDeletes;
        set { if (fCascadeDeletes != value) { fCascadeDeletes = value; NotifyPropertyChanged(nameof(CascadeDeletes)); } }
    }
    /// <summary>
    /// The top table of the module, the one with the single data row.
    /// </summary>
    public TableDef Table
    {
        get => fTable ??= new();
        set { if (fTable != value) { fTable = value; NotifyPropertyChanged(nameof(Table)); } }
    }
    /// <summary>
    /// A list of named SELECT Sql statements. Used in the List part and List SELECTs of the module.
    /// </summary>
    public List<SelectDef> SelectList
    {
        get => fSelectList ??= new();
        set { if (fSelectList != value) { fSelectList = value; NotifyPropertyChanged(nameof(SelectList)); } }
    }
    /// <summary>
    /// A list of named SELECT Sql statements that executed once at the initialization of the module and may be used
    /// in various situations, i.e. Locators
    /// </summary>
    public List<SelectDef> Stocks
    {
        get => fStocks ??= new();
        set { if (fStocks != value) { fStocks = value; NotifyPropertyChanged(nameof(Stocks)); } }
    }
}