namespace Tripous.Data;

public class ModuleDef: BaseDef, IJsonLoadable
{
    private string fClassName = typeof(DataModule).FullName;
    private string fDescription;
    private List<SelectDef> fSelectList = new();
    private TableDef fTable = new();

    // ● public
    public DataModule Create()
    {
        DataModule Result = TypeResolver.CreateInstance<DataModule>(ClassName);
        Result.ModuleDef = this;
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

        Table.ModuleDef = this;
        Table.UpdateReferences();
    }
    
    // ● properties
    public string ClassName
    {
        get => fClassName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
    public string Description
    {
        get => fDescription;
        set { if (fDescription != value) { fDescription = value; NotifyPropertyChanged(nameof(Description)); } }
    }
    public List<SelectDef> SelectList
    {
        get => fSelectList;
        set { if (fSelectList != value) { fSelectList = value; NotifyPropertyChanged(nameof(SelectList)); } }
    }
    public TableDef Table
    {
        get => fTable;
        set { if (fTable != value) { fTable = value; NotifyPropertyChanged(nameof(Table)); } }
    }
}