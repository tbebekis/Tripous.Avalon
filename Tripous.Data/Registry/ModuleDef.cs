/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;


/// <summary>
/// Describes a <see cref="DataMode"/>
/// </summary>
public class ModuleDef: BaseDef
{
    string fGroup; 
    string fClassName;
    string fDescription;
    SelectDefs fSelectList;
    SelectDefs fStocks;
    TableDef fTable = new();
    string fConnectionName;
    bool fIsSingleSelect = true;
    bool fGuidOids = true;
    bool fCascadeDeletes = true;
    string fItemCaptionField;
    bool fUseFilters = true;
    int fIsDocument = -1;
    bool fIsDocumentSnapshot;

    string GetItemCaptionField()
   {
       string[] CaptionFields = { "Name", "Code", "Description", "Id" };
       
       foreach (string FieldName in CaptionFields)
       {
           FieldDef FieldDef = Table.FindField(FieldName);
           if (FieldDef != null)
               return FieldName;
       }
   
       foreach (FieldDef FieldDef in Table.Fields)
           if (FieldDef.DataType == DataFieldType.String)
               return FieldDef.Name;

       throw new TripousDataException($"Cannot find an item caption/title field for {nameof(ModuleDef)} {Name}");
   }

    // ● public
    /// <summary>
    /// Creates, initializes and returns a <see cref="DataModule"/> associated with this definition.
    /// </summary>
    public DataModule Create(bool InitializeToo = true)
    {
        DataModule Result = TypeStore.CreateInstance<DataModule>(ClassName);
        if (InitializeToo)
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
 
    // ● properties
    /// <summary>
    /// The group this module belongs to, e.g. Sales, Purchaces, etc.
    /// </summary>
    public string Group  
    {
        get => !string.IsNullOrWhiteSpace(fGroup)? fGroup: Name;
        set { if (fGroup != value) { fGroup = value; NotifyPropertyChanged(nameof(Group)); } }
    }
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
        get => !string.IsNullOrWhiteSpace(fClassName)? fClassName: typeof(DataModule).FullName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
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
    /// An optional description
    /// </summary>
    public string Description
    {
        get => fDescription;
        set { if (fDescription != value) { fDescription = value; NotifyPropertyChanged(nameof(Description)); } }
    }
    /// <summary>
    /// When true then this is a module with a fixed single select.
    /// </summary>
    public bool IsSingleSelect
    {
        get => fIsSingleSelect;
        set { if (fIsSingleSelect != value) { fIsSingleSelect = value; NotifyPropertyChanged(nameof(IsSingleSelect)); } }
    }
    /// <summary>
    /// When true, the default, the filter panel in UI forms should be visible.
    /// </summary>
    public bool UseFilters 
    {
        get => fUseFilters;
        set { if (fUseFilters != value) { fUseFilters = value; NotifyPropertyChanged(nameof(UseFilters)); } }
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
    /// A field name of a field used in providing the item caption/title.
    /// </summary>
    public string ItemCaptionField
    {
        get => !string.IsNullOrWhiteSpace(fItemCaptionField)? fItemCaptionField: GetItemCaptionField();
        set { if (fItemCaptionField != value) { fItemCaptionField = value; NotifyPropertyChanged(nameof(ItemCaptionField)); } }
    }
    /// <summary>
    /// When true then this is a document data module.
    /// </summary>
    public bool IsDocument
    {
        get
        {
            if (fIsDocument < 0)
                return Table.Fields.Contains("DocumentTypeId");

            return fIsDocument == 1; 
        }
        set
        {
            int V = value? 1: 0;
            if (fIsDocument != V) { fIsDocument = V; NotifyPropertyChanged(nameof(IsDocument)); } 
        }
    }
    /// <summary>
    /// When true then this is a document snapshot data module,
    /// i.e. it may contain a DocumentTypeId field, but just as a snapshot of an actual document module.
    /// </summary>
    public bool IsDocumentSnapshot
    {
        get => fIsDocumentSnapshot;
        set { if (fIsDocumentSnapshot != value) { fIsDocumentSnapshot = value; NotifyPropertyChanged(nameof(IsDocumentSnapshot)); } }
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
    public SelectDefs SelectList
    {
        get => fSelectList ??= new();
        set { if (fSelectList != value) { fSelectList = value; NotifyPropertyChanged(nameof(SelectList)); } }
    }
    /// <summary>
    /// A list of named SELECT Sql statements that executed once at the initialization of the module and may be used
    /// in various situations, i.e. Locators
    /// </summary>
    public SelectDefs Stocks
    {
        get => fStocks ??= new();
        set { if (fStocks != value) { fStocks = value; NotifyPropertyChanged(nameof(Stocks)); } }
    }
}