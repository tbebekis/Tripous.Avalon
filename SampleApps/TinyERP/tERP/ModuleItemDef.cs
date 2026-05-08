namespace Tripous.Data;

 

public class ModuleItemGroupDefs: DefList<ModuleItemGroupDef>
{
    // ● construction
    public ModuleItemGroupDefs()
    {
    }

    public string GetCreateSqlText() => GetText(GetTableList(), (x) => x.CreateTableSql);
    public string GetRegisterCreateTableSqlSourceCode() => GetText(GetTableList(), (x) => x.RegisterCreateTableSqlSourceCode);
    public string GetRegisterModuleSourceCode() => GetText(GetModuleList(), (x) => x.RegisterModuleSourceCode);
 
    public string GetCreateSqlTextByProvider(DbServerType DbServerType)
    {
        string Result = GetCreateSqlText();
        SqlProvider Provider = SqlProviders.GetSqlProvider(DbServerType);
        Result = Provider.ReplaceDataTypePlaceholders(Result);
        return Result;
    }
    
    static public string GetText(List<TableItemDef> Tables, Func<TableItemDef, string> GetPropValueFunc)
    {
        StringBuilder SB = new();

        string PropValue;
        Tables = Tables.OrderBy(Table => Table.CreationOrder).ToList();
        foreach (TableItemDef Item in Tables)
        {
            PropValue = GetPropValueFunc(Item);
            if (!string.IsNullOrWhiteSpace(PropValue))
            {
                SB.AppendLine(PropValue);
                SB.AppendLine();
            }
        }
        
        string Result = SB.ToString();
        return Result;
    }
    static public string GetText(List<ModuleItemDef> Modules, Func<ModuleItemDef, string> GetPropValueFunc)
    {
        StringBuilder SB = new();

        string PropValue;
        foreach (ModuleItemDef Item in Modules)
        {
            PropValue = GetPropValueFunc(Item);
            if (!string.IsNullOrWhiteSpace(PropValue))
            {
                SB.AppendLine(PropValue);
                SB.AppendLine();
            }
        }
        
        string Result = SB.ToString();
        return Result;
    }

    public string RegisterFormsProcSourceCode { get; set; }
    public string RegisterModulesProcSourceCode { get; set; }

    public List<ModuleItemDef> GetModuleList()
    {
        List<ModuleItemDef> Result = new();

        foreach (ModuleItemGroupDef GroupItem in Items)
        {
            List<ModuleItemDef> List = GroupItem.Items.ToList();
            foreach (ModuleItemDef ModuleItem in List)
                if (!Result.Contains(ModuleItem))
                    Result.Add(ModuleItem);
        }
        
        return Result;
    }
    public List<TableItemDef> GetTableList()
    {
        List<TableItemDef> Result = new();

        foreach (ModuleItemGroupDef GroupItem in Items)
        {
            List<TableItemDef> List = GroupItem.GetTableList();
            foreach (TableItemDef TableItem in List)
                if (!Result.Contains(TableItem))
                    Result.Add(TableItem);
        }
        
        return Result;
    }
}

public class ModuleItemGroupDef: BaseDef
{
    // ● construction
    public ModuleItemGroupDef()
    {
    }


    public List<TableItemDef> GetTableList()
    {
        List<TableItemDef> Result = new();

        foreach (ModuleItemDef ModuleItem in Items)
        {
            List<TableItemDef> List = ModuleItem.GetTableList();
            foreach (TableItemDef TableItem in List)
                if (!Result.Contains(TableItem))
                    Result.Add(TableItem);
        }

        return Result;
    }
    
    // ● properties
    /// <summary>
    /// The list of items
    /// </summary>
    public DefList<ModuleItemDef> Items { get; set; } = new();
}

public class ModuleItemDef: BaseDef
{
    public ModuleItemDef()
    {
    }

    public List<TableItemDef> GetTableList()
    {
        List<TableItemDef> Result = new();

        void Add(TableItemDef Table)
        {
            Result.Add(Table);
            if (Table.Details != null && Table.Details.Count > 0)
                foreach (TableItemDef Detail in Table.Details)
                    Add(Detail);
        }
        
        Add(TopTable);

        return Result;
    }
    
    public string Group { get; set; }
    public TableItemDef TopTable { get; set; }
    
    public string ListSelectSql { get; set; }
    public string RegisterModuleSourceCode { get; set; }
    public string RegisterFormSourceCode { get; set; }    
}

public class TableItemDef : BaseDef
{
    /// <summary>
    /// Constructor
    /// </summary>
    public TableItemDef()
    {
    }

    public TableDef TableDef { get; set; }
    public string Master { get; set; }
    public string CreateTableSql { get; set; }
    public string RegisterCreateTableSqlSourceCode { get; set; }
    public int CreationOrder { get; set; }
    public bool IsLookup { get; set; }
    public DefList<TableItemDef> Details { get; set; } = new(); 
}
