namespace Tripous.Data;

/// <summary>
/// Parser message type.
/// </summary>
public enum ParsingErrorType
{
    None = 0,
    Error = 1,
    Warning = 2
}

/// <summary>
/// Defines generated duplicate registry checks.
/// </summary>
[Flags]
public enum DuplicateCheck
{
    None = 0,
    Lookup = 1,
    Enum = 2,
    Form = 4,
    Module = 8,
    Locator = 16
}

/// <summary>
/// A parser validation or informational message.
/// </summary>
public class ParsingMessage
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ParsingMessage()
    {
    }

    public override string ToString()
    {
        string Result = @$"Type: {MessageType}
Code: {Code}
ErrorText: {Text}
";
        return Result;
    }
    
    // ● properties
    /// <summary>
    /// Message type.
    /// </summary>
    public ParsingErrorType MessageType { get; set; }
    /// <summary>
    /// Message code.
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// Message text.
    /// </summary>
    public string Text { get; set; }
}

/// <summary>
/// Result of parsing a Tripous schema registration script.
/// </summary>
public class SchemaParserResult
{
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public SchemaParserResult()
    {
    }

    public string GetErrors()
    {
        StringBuilder SB = new();
        foreach (var Msg in Messages)
            if (Msg.MessageType == ParsingErrorType.Error)
                SB.AppendLine(Msg.ToString());
        return SB.ToString();
    }

    public string GetWarnings()
    {
        StringBuilder SB = new();
        foreach (var Msg in Messages)
        if (Msg.MessageType == ParsingErrorType.Warning)
        SB.AppendLine(Msg.ToString());
        return SB.ToString();
    }
    
    // ● properties
    /// <summary>
    /// Ordered schema SQL text.
    /// </summary>
    public string SchemaSql { get; set; }
    /// <summary>
    /// Source code that registers create table SQL statements.
    /// </summary>
    public string CreateTablesSourceCode { get; set; }
    /// <summary>
    /// Source code that registers module definitions.
    /// </summary>
    public string ModuleDefsSourceCode { get; set; }
    /// <summary>
    /// Source code that registers form definitions.
    /// </summary>
    public string FormDefsSourceCode { get; set; }
    /// <summary>
    /// Parser messages.
    /// </summary>
    public List<ParsingMessage> Messages { get; set; } = new();
    /// <summary>
    /// True when parser contains errors.
    /// </summary>
    public bool HasErrors => Messages.Any(x => x.MessageType == ParsingErrorType.Error);
    /// <summary>
    /// True when parser contains warnings.
    /// </summary>
    public bool HasWarnings => Messages.Any(x => x.MessageType == ParsingErrorType.Warning);
}

/// <summary>
/// Parses a Tripous schema script and generates schema, module and form registration source code.
/// </summary>
static public class SchemaRegistrationBuilder
{
    // ● static public
    /// <summary>
    /// Parses a Tripous schema script and generates schema, module and form registration source code.
    /// </summary>
    static public SchemaParserResult Parse(string SchemaSql, int SchemaVersion)
        => Parse(SchemaSql, SchemaVersion, DuplicateCheck.None);
    /// <summary>
    /// Parses a Tripous schema script and generates schema, module and form registration source code.
    /// </summary>
    static public SchemaParserResult Parse(string SchemaSql, int SchemaVersion, DuplicateCheck DuplicateChecks)
    {
        if (string.IsNullOrWhiteSpace(SchemaSql))
            throw new TripousArgumentNullException(nameof(SchemaSql));

        SchemaParserResult Result = new();
        SchemaScript Script = SchemaScript.Parse(SchemaSql);

        ValidateScript(Result, Script);

        if (Result.HasErrors)
            return Result;

        Script.Validate();

        Result.SchemaSql = BuildOrderedSchemaSql(Script);
        Result.CreateTablesSourceCode = BuildCreateTablesSourceCode(Script, SchemaVersion);
        Result.ModuleDefsSourceCode = BuildModuleDefsSourceCode(Script, DuplicateChecks);
        Result.FormDefsSourceCode = BuildFormDefsSourceCode(Script, DuplicateChecks);

        return Result;
    }

    // ● private - validation
    /// <summary>
    /// Adds a parser message.
    /// </summary>
    static void AddMessage(SchemaParserResult Result, ParsingErrorType MessageType, string Code, string Text)
    {
        ParsingMessage Message = new();
        Message.MessageType = MessageType;
        Message.Code = Code;
        Message.Text = Text;

        Result.Messages.Add(Message);
    }
    /// <summary>
    /// Adds an error message.
    /// </summary>
    static void AddError(SchemaParserResult Result, string Code, string Text)
    {
        AddMessage(Result, ParsingErrorType.Error, Code, Text);
    }
    /// <summary>
    /// Adds a warning message.
    /// </summary>
    static void AddWarning(SchemaParserResult Result, string Code, string Text)
    {
        AddMessage(Result, ParsingErrorType.Warning, Code, Text);
    }
    /// <summary>
    /// Validates parsed schema.
    /// </summary>
    static void ValidateScript(SchemaParserResult Result, SchemaScript Script)
    {
        ValidateDuplicateTableNames(Result, Script);
        ValidateDuplicateModuleNames(Result, Script);
        ValidateDuplicateCreationOrders(Result, Script);
        ValidateDuplicateGeneratedMethodNames(Result, Script);
        ValidateSuspiciousUniqueConstraints(Result, Script);
        ValidateLocatorFields(Result, Script);
    }
    /// <summary>
    /// Validates locator fields.
    /// </summary>
    static void ValidateLocatorFields(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.Tables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                if (!IsLocatorField(Field))
                    continue;

                if (Field.ForeignKey == null)
                {
                    AddError(Result, "LOCATOR_NO_REFERENCE", "Locator field has no foreign key reference: " + Table.Name + "." + Field.Name);
                    continue;
                }

                SchemaTable ReferenceTable = Script.FindTable(Field.ForeignKey.ReferenceTable);
                if (ReferenceTable == null)
                {
                    AddError(Result, "LOCATOR_REFERENCE_TABLE_NOT_FOUND", "Locator reference table not found: " + Table.Name + "." + Field.Name + " -> " + Field.ForeignKey.ReferenceTable);
                    continue;
                }

                LocatorInfo Locator = ResolveLocatorInfo(Script, Table, Field);
                if (Locator == null)
                    continue;

                bool HasExplicitReturnFields = ParseLocatorReturnFields(Field.MetadataText).Count > 0;
                if (!HasExplicitReturnFields && Locator.ReturnFields.Count <= 1)
                    AddError(Result, "LOCATOR_RETURN_FIELDS_NOT_FOUND", "Locator field has no default return fields: " + Table.Name + "." + Field.Name + " -> " + ReferenceTable.Name);

                foreach (string FieldName in Locator.ReturnFields)
                {
                    if (ReferenceTable.FindField(FieldName) == null)
                        AddError(Result, "LOCATOR_RETURN_FIELD_NOT_FOUND", "Locator return field not found: " + Table.Name + "." + Field.Name + " -> " + ReferenceTable.Name + "." + FieldName);
                }
            }
        }
    }
    /// <summary>
    /// Validates duplicate table names.
    /// </summary>
    static void ValidateDuplicateTableNames(SchemaParserResult Result, SchemaScript Script)
    {
        var Items = Script.Tables
            .GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in Items)
            AddError(Result, "DUPLICATE_TABLE", $"Duplicate table name: {Item.Key}");
    }
    /// <summary>
    /// Validates duplicate module names.
    /// </summary>
    static void ValidateDuplicateModuleNames(SchemaParserResult Result, SchemaScript Script)
    {
        var Items = Script.TopTables
            .Where(x => !string.IsNullOrWhiteSpace(x.ModuleName))
            .GroupBy(x => x.ModuleName, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in Items)
            AddError(Result, "DUPLICATE_MODULE", $"Duplicate module name: {Item.Key}");
    }
    /// <summary>
    /// Validates duplicate creation orders.
    /// </summary>
    static void ValidateDuplicateCreationOrders(SchemaParserResult Result, SchemaScript Script)
    {
        var Items = Script.Tables
            .Where(x => x.CreationOrder > 0)
            .GroupBy(x => x.CreationOrder)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in Items)
        {
            string TableNames = string.Join(", ", Item.Select(x => x.Name));
            AddError(Result, "DUPLICATE_CREATION_ORDER", $"Duplicate CreationOrder {Item.Key}: {TableNames}");
        }
    }
    /// <summary>
    /// Validates duplicate generated method names.
    /// </summary>
    static void ValidateDuplicateGeneratedMethodNames(SchemaParserResult Result, SchemaScript Script)
    {
        var TableMethodNames = Script.Tables
            .GroupBy(x => "RegisterTable_" + SafeIdentifier(x.Name), StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in TableMethodNames)
        {
            string TableNames = string.Join(", ", Item.Select(x => x.Name));
            AddError(Result, "DUPLICATE_TABLE_METHOD", $"Duplicate generated table method {Item.Key}: {TableNames}");
        }

        var ModuleMethodNames = Script.TopTables
            .GroupBy(x => "RegisterModule_" + SafeIdentifier(x.ModuleName), StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in ModuleMethodNames)
        {
            string ModuleNames = string.Join(", ", Item.Select(x => x.ModuleName));
            AddError(Result, "DUPLICATE_MODULE_METHOD", $"Duplicate generated module method {Item.Key}: {ModuleNames}");
        }
    }
    /// <summary>
    /// Validates suspicious unique constraints.
    /// </summary>
    static void ValidateSuspiciousUniqueConstraints(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.Tables)
            ValidateSuspiciousUniqueConstraints(Result, Table);
    }
    /// <summary>
    /// Validates suspicious unique constraints.
    /// </summary>
    static void ValidateSuspiciousUniqueConstraints(SchemaParserResult Result, SchemaTable Table)
    {
        MatchCollection Matches = Regex.Matches(
            Table.CreateSqlText,
            @"unique\s*\((?<fields>[^\)]*)\)",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        foreach (Match Match in Matches)
        {
            List<string> Fields = Match.Groups["fields"].Value
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();

            var Duplicates = Fields
                .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToList();

            if (Duplicates.Count > 0)
                AddWarning(Result, "SUSPICIOUS_UNIQUE", $"Table {Table.Name} has suspicious UNIQUE constraint with duplicate fields: {string.Join(", ", Duplicates)}");
        }
    }

    // ● private - result builders
    static bool IsBooleanColumnName(string ColumnName)
    {
        return ColumnName.StartsWith("Is", StringComparison.OrdinalIgnoreCase)
               || ColumnName.StartsWith("Has", StringComparison.OrdinalIgnoreCase)
               || ColumnName.StartsWith("Can", StringComparison.OrdinalIgnoreCase);
    }

    static bool IsCurrencyColumnName(string ColumnName)
    {
        return ColumnName.Contains("Amount", StringComparison.OrdinalIgnoreCase)
               || ColumnName.Contains("Price", StringComparison.OrdinalIgnoreCase)
               || ColumnName.Contains("Total", StringComparison.OrdinalIgnoreCase)
               || ColumnName.Contains("Balance", StringComparison.OrdinalIgnoreCase);
    }
    /// <summary>
    /// Adds a column type entry to a select build result.
    /// </summary>
    static void AddColumnType(SelectBuildResult Result, string ColumnName, SchemaField Field)
    {
        if (Result == null || Field == null || string.IsNullOrWhiteSpace(ColumnName))
            return;

        DataColumnType ColumnType = DataColumnType.None;

        switch (Field.DataType)
        {
            case DataFieldType.Date:
                ColumnType = DataColumnType.Date;
                break;
            case DataFieldType.DateTime:
                ColumnType = DataColumnType.DateTime;
                break;
            case DataFieldType.Boolean:
                ColumnType = DataColumnType.Boolean;
                break;
            case DataFieldType.Integer:
                ColumnType = IsBooleanColumnName(ColumnName) ? DataColumnType.Boolean : DataColumnType.Integer;
                break;
            case DataFieldType.Double:
            case DataFieldType.Decimal_:
            case DataFieldType.Decimal:
                ColumnType = IsCurrencyColumnName(ColumnName) ? DataColumnType.Currency : DataColumnType.Decimal;
                break;
            default:
                ColumnType = DataColumnType.Text;
                break;
        }

        Result.ColumnTypes[ColumnName] = ColumnType;
    }
    
    /// <summary>
    /// Builds ordered schema SQL.
    /// </summary>
    static string BuildOrderedSchemaSql(SchemaScript Script)
    {
        StringBuilder SB = new();

        foreach (SchemaTable Table in Script.Tables.OrderBy(x => x.CreationOrder))
        {
            if (SB.Length > 0)
                SB.AppendLine().AppendLine();
            SB.AppendLine(Table.FullSqlText.Trim());
        }

        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code for schema version registration.
    /// </summary>
    static string BuildCreateTablesSourceCode(SchemaScript Script, int SchemaVersion)
    {
        StringBuilder SB = new();
        List<SchemaTable> Tables = Script.Tables.OrderBy(x => x.CreationOrder).ToList();

        SB.AppendLine("public partial class SchemaVersion" + SchemaVersion + ": SchemaVersionDef");
        SB.AppendLine("{");
        SB.AppendLine("    // ● private");
        foreach (SchemaTable Table in Tables)
        {
            SB.AppendLine("    void RegisterTable_" + SafeIdentifier(Table.Name) + "()");
            SB.AppendLine("    {");
            SB.AppendLine("        string TableName = \"" + EscapeString(Table.Name) + "\";");
            SB.AppendLine("        string SqlText = $@\"");
            SB.AppendLine(EscapeVerbatim(Table.CreateSqlText.Trim()));
            SB.AppendLine("\";");
            SB.AppendLine("        Version.AddTable(SqlText);");
            SB.AppendLine("    }");
        }
        SB.AppendLine();
        SB.AppendLine("    // ● protected");
        SB.AppendLine("    protected override void RegisterInternal()");
        SB.AppendLine("    {");
        foreach (SchemaTable Table in Tables)
            SB.AppendLine("        RegisterTable_" + SafeIdentifier(Table.Name) + "();");
        SB.AppendLine("    }");
        SB.AppendLine();
        SB.AppendLine("    // ● construction");
        SB.AppendLine("    public SchemaVersion" + SchemaVersion + "()");
        SB.AppendLine("    {");
        SB.AppendLine("    }");
        SB.AppendLine();
        SB.AppendLine("    // ● properties");
        SB.AppendLine("    public override int VersionNumber { get; } = " + SchemaVersion + ";");
        SB.AppendLine("}");

        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code for module registration.
    /// </summary>
    static string BuildModuleDefsSourceCode(SchemaScript Script, DuplicateCheck DuplicateChecks)
    {
        StringBuilder SB = new();
        List<SchemaTable> TopTables = Script.TopTables.OrderBy(x => x.ModuleName).ToList();

        SB.AppendLine("static internal partial class Registry");
        SB.AppendLine("{");
        SB.AppendLine("    // ● private");
        BuildRegisterLookupSourcesMethod(SB, Script, DuplicateChecks);
        BuildRegisterLocatorsMethod(SB, Script, DuplicateChecks);
        foreach (SchemaTable TopTable in TopTables)
            BuildRegisterModuleMethod(SB, Script, TopTable, DuplicateChecks);
        SB.AppendLine();
        SB.AppendLine("    // ● static public");
        SB.AppendLine("    static public void RegisterModules()");
        SB.AppendLine("    {");
        SB.AppendLine("        RegisterLookupSources_FromModules();");
        SB.AppendLine("        RegisterLocators_FromModules();");
        foreach (SchemaTable TopTable in TopTables)
            SB.AppendLine("        RegisterModule_" + SafeIdentifier(TopTable.ModuleName) + "();");
        SB.AppendLine("    }");
        SB.AppendLine("}");

        return SB.ToString().TrimEnd();
    }

    /// <summary>
    /// Builds source code for form registration.
    /// </summary>
    static string BuildFormDefsSourceCode(SchemaScript Script, DuplicateCheck DuplicateChecks)
    {
        StringBuilder SB = new();
        List<SchemaTable> TopTables = Script.TopTables.OrderBy(x => x.ModuleName).ToList();

        SB.AppendLine("static internal partial class Registry");
        SB.AppendLine("{");
        SB.AppendLine("    // ● static public");
        SB.AppendLine("    static public void RegisterForms()");
        SB.AppendLine("    {");

        foreach (SchemaTable TopTable in TopTables)
        {
            string AddFormSource = BuildAddFormSource(TopTable);

            if (DuplicateChecks.HasFlag(DuplicateCheck.Form))
            {
                SB.AppendLine("        if (!DesktopRegistry.Forms.Contains(\"" + EscapeString(TopTable.FormName) + "\"))");
                SB.AppendLine("            " + AddFormSource);
            }
            else
            {
                SB.AppendLine("        " + AddFormSource);
            }
        }

        SB.AppendLine("    }");
        SB.AppendLine("}");

        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code that adds a form definition.
    /// </summary>
    static string BuildAddFormSource(SchemaTable TopTable)
    {
        List<string> Args = [];
        Args.Add("\"" + EscapeString(TopTable.FormName) + "\"");
        Args.Add("TitleKey: \"" + EscapeString(TopTable.FormName) + "\"");
        Args.Add("Module: \"" + EscapeString(TopTable.ModuleName) + "\"");

        if (!string.IsNullOrWhiteSpace(TopTable.FormClassName))
            Args.Add("ClassName: \"" + EscapeString(TopTable.FormClassName) + "\"");
        if (!string.IsNullOrWhiteSpace(TopTable.GroupName))
            Args.Add("Group: \"" + EscapeString(TopTable.GroupName) + "\"");
        if (!string.IsNullOrWhiteSpace(TopTable.ItemPageClassName))
            Args.Add("ItemClassName: \"" + EscapeString(TopTable.ItemPageClassName) + "\"");
        if (TopTable.IsReadOnly)
            Args.Add("IsReadOnly: true");

        return "DesktopRegistry.AddForm(" + string.Join(", ", Args) + ");";
    }

    // ● private - module source
    /// <summary>
    /// Collects lookup source names and table names.
    /// </summary>
    static Dictionary<string, string> CollectLookupSourceTables(SchemaScript Script)
    {
        Dictionary<string, string> Result = new(StringComparer.OrdinalIgnoreCase);

        foreach (SchemaTable Table in Script.Tables.Where(x => x.IsLookup))
            Result[Table.Name] = Table.Name;

        foreach (SchemaTable Table in Script.Tables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                if (Field.MetadataKind != FieldMetadataKind.Lookup && Field.MetadataKind != FieldMetadataKind.CorrelationLookup)
                    continue;

                string LookupSourceName = GetLookupSourceName(Script, Field);
                string LookupTableName = Field.ForeignKey != null ? Field.ForeignKey.ReferenceTable : LookupSourceName;

                if (!string.IsNullOrWhiteSpace(LookupSourceName) && !string.IsNullOrWhiteSpace(LookupTableName))
                    Result[LookupSourceName] = LookupTableName;
            }
        }

        return Result;
    }
    /// <summary>
    /// Builds lookup source registration method.
    /// </summary>
    static void BuildRegisterLookupSourcesMethod(StringBuilder SB, SchemaScript Script, DuplicateCheck DuplicateChecks)
    {
        Dictionary<string, string> LookupSources = CollectLookupSourceTables(Script);

        SB.AppendLine("    static void RegisterLookupSources_FromModules()");
        SB.AppendLine("    {");

        foreach (var Entry in LookupSources.OrderBy(x => x.Key))
        {
            if (DuplicateChecks.HasFlag(DuplicateCheck.Lookup))
            {
                SB.AppendLine("        if (!DataRegistry.LookupSources.Contains(\"" + EscapeString(Entry.Key) + "\"))");
                SB.AppendLine("            DataRegistry.AddLookupSourceWithTableName(\"" + EscapeString(Entry.Key) + "\", \"" + EscapeString(Entry.Value) + "\");");
            }
            else
            {
                SB.AppendLine("        DataRegistry.AddLookupSourceWithTableName(\"" + EscapeString(Entry.Key) + "\", \"" + EscapeString(Entry.Value) + "\");");
            }
        }

        SB.AppendLine("    }");

    }


    /// <summary>
    /// Builds the method that registers locator definitions.
    /// </summary>
    static void BuildRegisterLocatorsMethod(StringBuilder SB, SchemaScript Script, DuplicateCheck DuplicateChecks)
    {
        Dictionary<string, LocatorInfo> Locators = CollectLocators(Script);

        SB.AppendLine("    static void RegisterLocators_FromModules()");
        SB.AppendLine("    {");

        foreach (LocatorInfo Locator in Locators.Values.OrderBy(x => x.Name))
        {
            string Source = "DataRegistry.AddLocator(\"" + EscapeString(Locator.Name) + "\", \"" + EscapeString(Locator.TableName) + "\", \"" + EscapeString(Locator.KeyField) + "\"" + BuildOptionalClassNameArgument(Locator.ClassName) + ");";
            if (DuplicateChecks.HasFlag(DuplicateCheck.Locator))
            {
                SB.AppendLine("        if (!DataRegistry.Locators.Contains(\"" + EscapeString(Locator.Name) + "\"))");
                SB.AppendLine("            " + Source);
            }
            else
            {
                SB.AppendLine("        " + Source);
            }
        }

        SB.AppendLine("    }");
    }
    /// <summary>
    /// Collects locator definitions.
    /// </summary>
    static Dictionary<string, LocatorInfo> CollectLocators(SchemaScript Script)
    {
        Dictionary<string, LocatorInfo> Result = new(StringComparer.OrdinalIgnoreCase);

        foreach (SchemaTable Table in Script.Tables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                if (!IsLocatorField(Field))
                    continue;

                LocatorInfo Locator = ResolveLocatorInfo(Script, Table, Field);
                if (Locator == null)
                    continue;

                if (!Result.ContainsKey(Locator.Name))
                    Result[Locator.Name] = Locator;
            }
        }

        return Result;
    }
    /// <summary>
    /// Builds source code that adds a module definition.
    /// </summary>
    static string BuildAddModuleSource(SchemaTable TopTable)
    {
        List<string> Args = [];
        Args.Add("\"" + EscapeString(TopTable.ModuleName) + "\"");

        if (!string.IsNullOrWhiteSpace(TopTable.ModuleClassName))
            Args.Add("ClassName: \"" + EscapeString(TopTable.ModuleClassName) + "\"");

        Args.Add("ListSelectSql: SqlText");

        if (TopTable.IsSingleSelect)
            Args.Add("IsSingleSelect: true");

        return "Module = DataRegistry.AddModule(" + string.Join(", ", Args) + ");";
    }
    /// <summary>
    /// Builds module option assignments.
    /// </summary>
    static void BuildModuleOptionAssignments(StringBuilder SB, SchemaTable TopTable)
    {
        if (!TopTable.UseFilters)
            SB.AppendLine("        Module.UseFilters = false;");
        if (!TopTable.CascadeDeletes)
            SB.AppendLine("        Module.CascadeDeletes = false;");
        if (!TopTable.GuidOids)
            SB.AppendLine("        Module.GuidOids = false;");
    }

    /// <summary>
    /// Builds a module registration method.
    /// </summary>
    static void BuildRegisterModuleMethod(StringBuilder SB, SchemaScript Script, SchemaTable TopTable, DuplicateCheck DuplicateChecks)
    {
        SelectBuildResult SelectResult = BuildListSelectSql(Script, TopTable);

        SB.AppendLine("    static void RegisterModule_" + SafeIdentifier(TopTable.ModuleName) + "()");
        SB.AppendLine("    {");
        if (DuplicateChecks.HasFlag(DuplicateCheck.Module))
        {
            SB.AppendLine("        if (DataRegistry.Modules.Contains(\"" + EscapeString(TopTable.ModuleName) + "\"))");
            SB.AppendLine("            return;");
        }
        SB.AppendLine("        ModuleDef Module;");
        SB.AppendLine("        TableDef tblTop;");
        SB.AppendLine("        SelectDef SelectDef;");
        SB.AppendLine("        string SqlText;");

        SB.AppendLine("        SqlText = @\"");
        SB.AppendLine(EscapeVerbatim(SelectResult.SqlText));
        SB.AppendLine("\";");
        SB.AppendLine("        " + BuildAddModuleSource(TopTable));
        BuildModuleOptionAssignments(SB, TopTable);
        SB.AppendLine("        tblTop = Module.Table;");
        SB.AppendLine("        tblTop.Name = \"" + EscapeString(TopTable.Name) + "\";");
        SB.AppendLine("        tblTop.KeyField = \"" + EscapeString(TopTable.PrimaryKeyField.Name) + "\";");

        if (!TopTable.UiVisible)
            SB.AppendLine("        tblTop.IsUiVisible = false;");

        BuildTableFieldsSource(SB, Script, TopTable, "tblTop", "        ");
        if (TopTable.UseFilters)
            BuildFiltersSource(SB, SelectResult.FilterFields);

        BuildSelectColumnTypesSource(SB, SelectResult);

        foreach (SchemaTable Detail in Script.GetDetailsOf(TopTable))
            BuildDetailSource(SB, Script, Detail, "tblTop", "        ");

        SB.AppendLine("    }");
    }

    /// <summary>
    /// Builds source code for a detail table and its children.
    /// </summary>
    static void BuildDetailSource(StringBuilder SB, SchemaScript Script, SchemaTable DetailTable, string ParentVarName, string Indent)
    {
        SchemaField MasterField = DetailTable.MasterField;
        SchemaTable MasterTable = Script.FindTable(DetailTable.MasterName);
        string VarName = "tbl" + SafeIdentifier(DetailTable.Name);
        string MasterKeyField = MasterTable != null ? MasterTable.PrimaryKeyField.Name : "Id";
        string DetailField = MasterField != null ? MasterField.Name : FindMasterFieldName(DetailTable);

        SB.AppendLine(Indent + "TableDef " + VarName + " = " + ParentVarName + ".AddDetail(\"" + EscapeString(DetailTable.Name) + "\", \"" + EscapeString(MasterKeyField) + "\", \"" + EscapeString(DetailField) + "\");");
        SB.AppendLine(Indent + VarName + ".KeyField = \"" + EscapeString(DetailTable.PrimaryKeyField.Name) + "\";");
        if (DetailTable.IsOneToOne)
            SB.AppendLine(Indent + VarName + ".IsOneToOne = true;");
        if (!DetailTable.UiVisible)
            SB.AppendLine(Indent + VarName + ".IsUiVisible = false;");

        BuildTableFieldsSource(SB, Script, DetailTable, VarName, Indent);
        foreach (SchemaTable ChildDetail in Script.GetDetailsOf(DetailTable))
            BuildDetailSource(SB, Script, ChildDetail, VarName, Indent);
    }
    /// <summary>
    /// Builds source code for a table field list.
    /// </summary>
    static void BuildTableFieldsSource(StringBuilder SB, SchemaScript Script, SchemaTable Table, string TableVarName, string Indent)
    {
        foreach (SchemaField Field in Table.Fields)
            SB.AppendLine(Indent + BuildAddFieldSource(Script, Table, Field, TableVarName));

        BuildLocatorJoinsSource(SB, Script, Table, TableVarName, Indent);
    }
    /// <summary>
    /// Builds source code for locator joins.
    /// </summary>
    static void BuildLocatorJoinsSource(StringBuilder SB, SchemaScript Script, SchemaTable Table, string TableVarName, string Indent)
    {
        foreach (SchemaField Field in Table.Fields)
        {
            if (!IsLocatorField(Field))
                continue;

            LocatorInfo Locator = ResolveLocatorInfo(Script, Table, Field);
            if (Locator == null)
                continue;

            SchemaTable JoinTable = Script.FindTable(Locator.TableName);
            if (JoinTable == null)
                continue;

            string JoinVarName = "tbl" + SafeIdentifier(Locator.Alias);
            SB.AppendLine(Indent + "TableDef " + JoinVarName + " = " + TableVarName + ".AddJoin(\"" + EscapeString(Field.Name) + "\", \"" + EscapeString(Locator.TableName) + "\", \"" + EscapeString(Locator.Alias) + "\", \"" + EscapeString(Locator.KeyField) + "\");");
            SB.AppendLine(Indent + TableVarName + ".Fields.Get(\"" + EscapeString(Field.Name) + "\").Locator = \"" + EscapeString(Locator.Name) + "\";");

            SchemaField KeyField = JoinTable.FindField(Locator.KeyField);
            if (KeyField != null)
                SB.AppendLine(Indent + BuildAddFieldSource(Script, JoinTable, KeyField, JoinVarName));

            foreach (string FieldName in Locator.ReturnFields)
            {
                if (FieldName.IsSameText(Locator.KeyField))
                    continue;

                SchemaField JoinField = JoinTable.FindField(FieldName);
                if (JoinField == null)
                    continue;

                SB.AppendLine(Indent + BuildAddFieldSource(Script, JoinTable, JoinField, JoinVarName));
            }
        }
    }
    /// <summary>
    /// Builds source code for filters.
    /// </summary>
    static void BuildFiltersSource(StringBuilder SB, List<SelectField> FilterFields)
    {
        if (FilterFields.Count == 0)
            return;
        
        FilterFields = FilterFields
            .OrderByDescending(x => x.Alias.IsSameText("Name"))
            .ThenBy(x => x.Alias)
            .ToList();

        SB.AppendLine("        string[] FilterFields = [" + string.Join(", ", FilterFields.Select(x => "\"" + EscapeString(x.Alias) + "\"")) + "];");
        SB.AppendLine("        SelectDef = Module.SelectList[0];");
        SB.AppendLine("        foreach (string FieldName in FilterFields)");
        SB.AppendLine("            SelectDef.AddFilter(FieldName, FieldName: FieldName);");
    }

    static void BuildSelectColumnTypesSource(StringBuilder SB, SelectBuildResult SelectResult)
    {
        foreach (var Pair in SelectResult.ColumnTypes)
            SB.AppendLine($"        SelectDef.ColumnTypes[\"{EscapeString(Pair.Key)}\"] = DataColumnType.{Pair.Value};");
    }
    /// <summary>
    /// Builds source code that adds a field definition.
    /// </summary>
    static string BuildAddFieldSource(SchemaScript Script, SchemaTable Table, SchemaField Field, string TableVarName)
    {
        string NullSuffix = ".SetNullable(" + BoolLiteral(Field.IsNullable) + ")";
        string DefaultSuffix = !string.IsNullOrWhiteSpace(Field.DefaultValue) ? ".SetDefaultValue(\"" + EscapeString(Field.DefaultValue) + "\")" : "";
        string Flags = BuildFlags(Field);

        if (Field.IsPrimaryKey)
            return TableVarName + ".AddId(\"" + EscapeString(Field.Name) + "\").SetNullable(false);";

        if (Field.MetadataKind == FieldMetadataKind.Enum)
        {
            string EnumName = GetEnumName(Script, Field);
            string EnumTypeName = GetEnumTypeName(Script, EnumName);
            return TableVarName + ".AddEnumLookupId(\"" + EscapeString(Field.Name) + "\", \"" + EscapeString(EnumName) + "\", typeof(" + SafeIdentifier(EnumTypeName) + "), Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
        }

        if (IsLookupField(Script, Field))
        {
            string LookupSource = GetLookupSourceName(Script, Field);
            if (Field.DataType == DataFieldType.Integer)
                return TableVarName + ".AddIntegerLookupId(\"" + EscapeString(Field.Name) + "\", \"" + EscapeString(LookupSource) + "\", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            return TableVarName + ".AddStringLookupId(\"" + EscapeString(Field.Name) + "\", \"" + EscapeString(LookupSource) + "\", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
        }

        if (Field.MetadataKind == FieldMetadataKind.LargeMemo)
            return TableVarName + ".AddTextBlob(\"" + EscapeString(Field.Name) + "\", Flags: " + BuildFlags(Field, FieldFlagsText.LargeMemo) + ")" + NullSuffix + DefaultSuffix + ";";

        switch (Field.DataType)
        {
            case DataFieldType.String:
                return TableVarName + ".AddString(\"" + EscapeString(Field.Name) + "\", MaxLength: " + Field.MaxLength + ", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.Integer:
                return TableVarName + ".AddInteger(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.Double:
                return TableVarName + ".AddDouble(\"" + EscapeString(Field.Name) + "\", Decimals: " + Field.Decimals + ", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.Decimal:
            case DataFieldType.Decimal_:
                return TableVarName + ".AddDecimal(\"" + EscapeString(Field.Name) + "\", Decimals: " + Field.Decimals + ", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.Date:
                return TableVarName + ".AddDate(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.DateTime:
                return TableVarName + ".AddDateTime(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.Boolean:
                return TableVarName + ".AddBoolean(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.Blob:
                return TableVarName + ".AddBlob(\"" + EscapeString(Field.Name) + "\", Flags: " + BuildFlags(Field, FieldFlagsText.None) + ")" + NullSuffix + DefaultSuffix + ";";
            case DataFieldType.TextBlob:
                return TableVarName + ".AddTextBlob(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
        }

        return TableVarName + ".AddString(\"" + EscapeString(Field.Name) + "\", MaxLength: " + Field.MaxLength + ", Flags: " + Flags + ")" + NullSuffix + DefaultSuffix + ";";
    }

    // ● private - select source
    /// <summary>
    /// Builds list select SQL and filter field information.
    /// </summary>
    static SelectBuildResult BuildListSelectSql(SchemaScript Script, SchemaTable TopTable)
    {
        SelectBuildResult Result = new();
        List<string> SelectLines = [];
        List<string> JoinLines = [];
        HashSet<string> Aliases = new(StringComparer.OrdinalIgnoreCase);

        foreach (SchemaField Field in TopTable.Fields)
        {
            if (!Field.DataType.IsBlob())
            {
                SelectLines.Add("   " + TopTable.Name + "." + Field.Name);
                AddColumnType(Result, Field.Name, Field);

                if (IsFilterableField(Field, Field.Name))
                    Result.FilterFields.Add(new SelectField(Field.Name, Field.DataType));
            }
        }

        foreach (SchemaForeignKey ForeignKey in TopTable.ForeignKeys)
        {
            SchemaTable JoinTable = Script.FindTable(ForeignKey.ReferenceTable);

            if (JoinTable == null || JoinTable.Name.IsSameText(TopTable.Name))
                continue;

            string Alias = UniqueAlias(RemoveIdSuffix(ForeignKey.FieldName), Aliases);

            JoinLines.Add("    left join " + JoinTable.Name + " " + Alias + " on " + Alias + "." + ForeignKey.ReferenceField + " = " + TopTable.Name + "." + ForeignKey.FieldName);

            foreach (SchemaField JoinField in JoinTable.Fields)
            {
                if (JoinField.Name.IsSameText("Id"))
                    continue;

                if (JoinField.DataType != DataFieldType.String)
                    continue;

                if (!JoinField.Name.IsSameText("Name") &&
                    !JoinField.Name.IsSameText("Code") &&
                    !JoinField.Name.IsSameText("Title"))
                    continue;

                string DisplayAlias = UniqueAlias(Alias + "__" + JoinField.Name, Aliases);

                SelectLines.Add("   COALESCE(" + Alias + "." + JoinField.Name + ", '') as " + DisplayAlias);
                AddColumnType(Result, DisplayAlias, JoinField);

                if (IsFilterableField(JoinField, DisplayAlias))
                    Result.FilterFields.Add(new SelectField(DisplayAlias, JoinField.DataType));
            }
        }

        StringBuilder SB = new();

        SB.AppendLine("select");
        SB.AppendLine(string.Join("," + Environment.NewLine, SelectLines));
        SB.AppendLine("from");
        SB.AppendLine("  " + TopTable.Name);

        foreach (string JoinLine in JoinLines)
            SB.AppendLine(JoinLine);

        Result.SqlText = SB.ToString().TrimEnd();

        Result.FilterFields = Result.FilterFields
            .GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .OrderBy(x => x.Alias)
            .ToList();

        return Result;
    }
    /// <summary>
    /// Adds display fields from a joined table.
    /// </summary>
    static void AddJoinDisplayFields(List<string> SelectLines, List<SelectField> FilterFields, SchemaTable JoinTable, string Alias)
    {
        foreach (string FieldName in new[] { "Code", "Name", "Title", "Description", "IsActive" })
        {
            SchemaField Field = JoinTable.FindField(FieldName);
            if (Field == null || Field.DataType.IsBlob())
                continue;

            string OutputAlias = Field.Name.IsSameText("Name") ? Alias : Alias + Field.Name;
            SelectLines.Add("   " + Alias + "." + Field.Name + " as " + OutputAlias);

            if (IsFilterableField(Field, OutputAlias))
                FilterFields.Add(new SelectField(OutputAlias, Field.DataType));
        }
    }

    // ● private - parser helpers
    /// <summary>
    /// Extracts a CREATE TABLE statement from text.
    /// </summary>
    static string ExtractCreateTableSql(string Text, int StartIndex)
    {
        Match M = Regex.Match(Text.Substring(StartIndex), @"CREATE\s+TABLE\s+\{TableName\}\s*\(", RegexOptions.IgnoreCase);
        if (!M.Success)
            throw new TripousDataException("CREATE TABLE {TableName} not found.");

        int Start = StartIndex + M.Index;
        int Pos = Start + M.Length - 1;
        int Level = 0;

        for (int i = Pos; i < Text.Length; i++)
        {
            if (Text[i] == '(')
                Level++;
            else if (Text[i] == ')')
            {
                Level--;
                if (Level == 0)
                    return Text.Substring(Start, i - Start + 1).Trim();
            }
        }

        throw new TripousDataException("CREATE TABLE closing parenthesis not found.");
    }
    /// <summary>
    /// Extracts a header comment before a CREATE TABLE statement.
    /// </summary>
    static string ExtractHeaderText(string Text, int CreateTableIndex)
    {
        int HeaderStart = Text.LastIndexOf("/*", CreateTableIndex, StringComparison.Ordinal);
        int HeaderEnd = Text.LastIndexOf("*/", CreateTableIndex, StringComparison.Ordinal);

        if (HeaderStart < 0 || HeaderEnd < HeaderStart)
            throw new TripousDataException("Schema table header not found.");

        return Text.Substring(HeaderStart, HeaderEnd - HeaderStart + 2).Trim();
    }
    /// <summary>
    /// Returns a header value.
    /// </summary>
    static string GetHeaderValue(string HeaderText, string Name)
    {
        Match M = Regex.Match(HeaderText, @"^\s*" + Regex.Escape(Name) + @"\s*:\s*(.+?)\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
        return M.Success ? M.Groups[1].Value.Trim() : string.Empty;
    }
    /// <summary>
    /// Returns true if a header key exists.
    /// </summary>
    static bool HeaderKeyExists(string HeaderText, string Name)
    {
        return Regex.IsMatch(HeaderText, @"^\s*" + Regex.Escape(Name) + @"\s*(?::.*)?$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
    }
    /// <summary>
    /// Returns true if a header flag exists.
    /// </summary>
    static bool GetHeaderFlag(string HeaderText, string Name) => HeaderKeyExists(HeaderText, Name);
    /// <summary>
    /// Returns a boolean header value.
    /// </summary>
    static bool GetHeaderBool(string HeaderText, string Name, bool DefaultValue)
    {
        if (!HeaderKeyExists(HeaderText, Name))
            return DefaultValue;

        string Value = GetHeaderValue(HeaderText, Name);
        if (string.IsNullOrWhiteSpace(Value))
            return true;

        return Value.Equals("true", StringComparison.OrdinalIgnoreCase) || Value == "1" || Value.Equals("yes", StringComparison.OrdinalIgnoreCase);
    }
    /// <summary>
    /// Splits a header value into tokens.
    /// </summary>
    static List<string> SplitHeaderTokens(string Value)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return [];
        return Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
    }
    /// <summary>
    /// Returns an integer header value.
    /// </summary>
    static int GetHeaderInt(string HeaderText, string Name, int DefaultValue)
    {
        string Value = GetHeaderValue(HeaderText, Name);
        return int.TryParse(Value, out int Result) ? Result : DefaultValue;
    }
    /// <summary>
    /// Returns the CREATE TABLE body.
    /// </summary>
    static string GetCreateTableBody(string SqlText)
    {
        int Start = SqlText.IndexOf('(');
        int End = SqlText.LastIndexOf(')');
        if (Start < 0 || End < Start)
            throw new TripousDataException("Invalid CREATE TABLE statement.");
        return SqlText.Substring(Start + 1, End - Start - 1);
    }
    /// <summary>
    /// Splits SQL items by top-level commas.
    /// </summary>
    static List<string> SplitSqlItems(string Text)
    {
        List<string> Result = [];
        StringBuilder SB = new();
        int Level = 0;

        foreach (char C in Text)
        {
            if (C == '(')
                Level++;
            else if (C == ')')
                Level--;

            if (C == ',' && Level == 0)
            {
                Result.Add(SB.ToString().Trim());
                SB.Clear();
            }
            else
            {
                SB.Append(C);
            }
        }

        if (SB.Length > 0)
            Result.Add(SB.ToString().Trim());

        return Result;
    }
    /// <summary>
    /// Separates SQL text from inline comment text.
    /// </summary>
    static void SplitInlineComment(string Line, out string SqlPart, out string CommentPart)
    {
        int Index = Line.IndexOf("--", StringComparison.Ordinal);
        if (Index < 0)
        {
            SqlPart = Line.Trim();
            CommentPart = string.Empty;
            return;
        }

        SqlPart = Line.Substring(0, Index).Trim();
        CommentPart = Line.Substring(Index + 2).Trim();
    }
    /// <summary>
    /// Parses field metadata from an inline comment.
    /// </summary>
    static FieldMetadata ParseFieldMetadata(string CommentPart)
    {
        FieldMetadata Result = new();

        if (string.IsNullOrWhiteSpace(CommentPart))
            return Result;

        string[] Parts = CommentPart.Split("--", 2, StringSplitOptions.TrimEntries);
        string MetadataText = Parts.Length > 0 ? Parts[0].Trim() : string.Empty;
        Result.CommentText = Parts.Length > 1 ? Parts[1].Trim() : string.Empty;
        Result.MetadataText = MetadataText;

        if (MetadataText.StartsWith("Correlation Lookup", StringComparison.OrdinalIgnoreCase))
            Result.Kind = FieldMetadataKind.CorrelationLookup;
        else if (MetadataText.StartsWith("Correlation Locator", StringComparison.OrdinalIgnoreCase))
            Result.Kind = FieldMetadataKind.CorrelationLocator;
        else if (MetadataText.StartsWith("Master", StringComparison.OrdinalIgnoreCase))
            Result.Kind = FieldMetadataKind.Master;
        else if (MetadataText.StartsWith("Lookup", StringComparison.OrdinalIgnoreCase))
            Result.Kind = FieldMetadataKind.Lookup;
        else if (MetadataText.StartsWith("Enum", StringComparison.OrdinalIgnoreCase))
            Result.Kind = FieldMetadataKind.Enum;
        else if (MetadataText.StartsWith("Locator", StringComparison.OrdinalIgnoreCase))
            Result.Kind = FieldMetadataKind.Locator;
        else if (MetadataText.StartsWith("LargeMemo", StringComparison.OrdinalIgnoreCase))
            Result.Kind = FieldMetadataKind.LargeMemo;

        Result.MetadataName = ParseMetadataName(MetadataText, Result.Kind);
        Result.IsOneToOne = MetadataText.IndexOf("OneToOne", StringComparison.OrdinalIgnoreCase) >= 0;

        return Result;
    }
    /// <summary>
    /// Parses an optional metadata name.
    /// </summary>
    static string ParseMetadataName(string MetadataText, FieldMetadataKind Kind)
    {
        if (string.IsNullOrWhiteSpace(MetadataText))
            return string.Empty;

        string Text = MetadataText.Trim();

        if (Kind == FieldMetadataKind.CorrelationLocator && Text.StartsWith("Correlation Locator", StringComparison.OrdinalIgnoreCase))
            Text = Text.Substring("Correlation Locator".Length).Trim();
        else if (Kind == FieldMetadataKind.Locator && Text.StartsWith("Locator", StringComparison.OrdinalIgnoreCase))
            Text = Text.Substring("Locator".Length).Trim();
        else if (Kind == FieldMetadataKind.Enum && Text.StartsWith("Enum", StringComparison.OrdinalIgnoreCase))
            Text = Text.Substring("Enum".Length).Trim();
        else
            return string.Empty;

        int OpenIndex = Text.IndexOf('(');
        if (OpenIndex >= 0)
            Text = Text.Substring(0, OpenIndex).Trim();

        return SplitHeaderTokens(Text).FirstOrDefault() ?? string.Empty;
    }
    /// <summary>
    /// Returns true if a line can be parsed as a field line.
    /// </summary>
    static bool IsFieldLine(string Line)
    {
        if (string.IsNullOrWhiteSpace(Line))
            return false;
        if (!Regex.IsMatch(Line, @"^\s*\w+\s+", RegexOptions.IgnoreCase))
            return false;
        return Line.Contains("@")
               || Regex.IsMatch(Line, @"\b(INT|INTEGER|SMALLINT|BIGINT|DATE|DATETIME|DECIMAL|FLOAT|DOUBLE|VARCHAR|NVARCHAR|TEXT|BLOB)\b", RegexOptions.IgnoreCase);
    }
    /// <summary>
    /// Parses field data type.
    /// </summary>
    static void ParseFieldDataType(SchemaField Field, string SqlPart)
    {
        if (SqlPart.ContainsText("@NVARCHAR") || SqlPart.ContainsText("@VARCHAR"))
        {
            Field.DataType = DataFieldType.String;
            Field.MaxLength = ParseFirstNumber(SqlPart, 96);
        }
        else if (SqlPart.ContainsText("@DECIMAL_"))
        {
            Field.DataType = DataFieldType.Decimal_;
            Field.Decimals = ParseDecimalScale(SqlPart, 4);
        }
        else if (SqlPart.ContainsText("@DECIMAL"))
        {
            Field.DataType = DataFieldType.Decimal;
            Field.Decimals = 4;
        }
        else if (SqlPart.ContainsText("@FLOAT"))
        {
            Field.DataType = DataFieldType.Double;
            Field.Decimals = 4;
        }
        else if (SqlPart.ContainsText("@DATE_TIME"))
        {
            Field.DataType = DataFieldType.DateTime;
        }
        else if (SqlPart.ContainsText("@DATE"))
        {
            Field.DataType = DataFieldType.Date;
        }
        else if (SqlPart.ContainsText("@BOOL"))
        {
            Field.DataType = DataFieldType.Boolean;
        }
        else if (SqlPart.ContainsText("@BLOB_TEXT") || SqlPart.ContainsText("@NBLOB_TEXT"))
        {
            Field.DataType = DataFieldType.TextBlob;
        }
        else if (SqlPart.ContainsText("@BLOB"))
        {
            Field.DataType = DataFieldType.Blob;
        }
        else if (Regex.IsMatch(SqlPart, @"\b(INT|INTEGER|SMALLINT|BIGINT)\b", RegexOptions.IgnoreCase))
        {
            Field.DataType = Field.Name.EndsWithText("IsActive") || Field.Name.StartsWithText("Is") ? DataFieldType.Boolean : DataFieldType.Integer;
        }
        else if (Regex.IsMatch(SqlPart, @"\b(DATE|DATETIME)\b", RegexOptions.IgnoreCase))
        {
            Field.DataType = SqlPart.ContainsText("DATETIME") ? DataFieldType.DateTime : DataFieldType.Date;
        }
        else
        {
            Field.DataType = DataFieldType.String;
            Field.MaxLength = 96;
        }

        if (Field.MaxLength <= 0 && Field.DataType == DataFieldType.String)
            Field.MaxLength = 96;
        if (Field.Decimals < 0 && Field.DataType.IsFloat())
            Field.Decimals = 4;
    }
    /// <summary>
    /// Parses the first number in parentheses.
    /// </summary>
    static int ParseFirstNumber(string Text, int DefaultValue)
    {
        Match M = Regex.Match(Text, @"\((\d+)");
        return M.Success ? int.Parse(M.Groups[1].Value) : DefaultValue;
    }
    /// <summary>
    /// Parses a decimal scale.
    /// </summary>
    static int ParseDecimalScale(string Text, int DefaultValue)
    {
        Match M = Regex.Match(Text, @"@DECIMAL_\s*\(\s*\d+\s*,\s*(\d+)\s*\)", RegexOptions.IgnoreCase);
        return M.Success ? int.Parse(M.Groups[1].Value) : DefaultValue;
    }
    /// <summary>
    /// Parses a default value.
    /// </summary>
    static string ParseDefaultValue(string Text)
    {
        Match M = Regex.Match(Text, @"\bdefault\s+([^\s,]+)", RegexOptions.IgnoreCase);
        return M.Success ? M.Groups[1].Value.Trim('\'', '"') : string.Empty;
    }
    /// <summary>
    /// Parses a foreign key.
    /// </summary>
    static SchemaForeignKey ParseForeignKey(string Line)
    {
        Match M = Regex.Match(Line, @"FOREIGN\s+KEY\s*\(\s*(\w+)\s*\)\s+REFERENCES\s+(\w+)\s*\(\s*(\w+)\s*\)", RegexOptions.IgnoreCase);
        if (!M.Success)
            throw new TripousDataException("Invalid foreign key line: " + Line);

        SchemaForeignKey Result = new();
        Result.FieldName = M.Groups[1].Value.Trim();
        Result.ReferenceTable = M.Groups[2].Value.Trim();
        Result.ReferenceField = M.Groups[3].Value.Trim();
        return Result;
    }

    // ● private - generation helpers
    /// <summary>
    /// Returns true when a field must be generated as lookup field.
    /// </summary>
    static bool IsLookupField(SchemaScript Script, SchemaField Field)
    {
        if (Field.MetadataKind == FieldMetadataKind.Lookup || Field.MetadataKind == FieldMetadataKind.CorrelationLookup)
            return true;

        if (Field.ForeignKey != null)
        {
            SchemaTable RefTable = Script.FindTable(Field.ForeignKey.ReferenceTable);
            return RefTable != null && RefTable.IsLookup;
        }

        return false;
    }
    /// <summary>
    /// Returns lookup source name for a field.
    /// </summary>
    static string GetLookupSourceName(SchemaScript Script, SchemaField Field)
    {
        if (Field.ForeignKey != null)
            return Field.ForeignKey.ReferenceTable;

        if (Field.MetadataKind == FieldMetadataKind.Lookup || Field.MetadataKind == FieldMetadataKind.CorrelationLookup)
            return RemoveIdSuffix(Field.Name);

        return RemoveIdSuffix(Field.Name);
    }

    /// <summary>
    /// Returns enum lookup source name for a field.
    /// </summary>
    static string GetEnumName(SchemaScript Script, SchemaField Field)
    {
        string Result = !string.IsNullOrWhiteSpace(Field.MetadataName) ? Field.MetadataName : RemoveIdSuffix(Field.Name);
        EnumInfo Info = Script.FindEnum(Result);
        return Info != null && !string.IsNullOrWhiteSpace(Info.Name) ? Info.Name : Result;
    }
    /// <summary>
    /// Returns enum type name.
    /// </summary>
    static string GetEnumTypeName(SchemaScript Script, string EnumName)
    {
        EnumInfo Info = Script.FindEnum(EnumName);
        if (Info != null && !string.IsNullOrWhiteSpace(Info.TypeName))
            return Info.TypeName;
        return EnumName;
    }

    /// <summary>
    /// Returns true when a field must be generated as locator field.
    /// </summary>
    static bool IsLocatorField(SchemaField Field)
    {
        return Field.MetadataKind == FieldMetadataKind.Locator || Field.MetadataKind == FieldMetadataKind.CorrelationLocator;
    }
    /// <summary>
    /// Resolves locator metadata for a field.
    /// </summary>
    static LocatorInfo ResolveLocatorInfo(SchemaScript Script, SchemaTable Table, SchemaField Field)
    {
        if (Field.ForeignKey == null)
            return null;

        string ReferenceTableName = Field.ForeignKey.ReferenceTable;
        SchemaTable ReferenceTable = Script.FindTable(ReferenceTableName);
        if (ReferenceTable == null)
            return null;

        string LocatorName = !string.IsNullOrWhiteSpace(Field.MetadataName) ? Field.MetadataName : ReferenceTableName;

        LocatorInfo Result = new();
        Result.Name = LocatorName;
        Result.TableName = ReferenceTableName;
        Result.Alias = RemoveIdSuffix(Field.Name);
        Result.KeyField = !string.IsNullOrWhiteSpace(Field.ForeignKey.ReferenceField) ? Field.ForeignKey.ReferenceField : "Id";
        Result.ReturnFields.Add(Result.KeyField);

        List<string> ReturnFields = ParseLocatorReturnFields(Field.MetadataText);
        if (ReturnFields.Count == 0)
            ReturnFields = GetDefaultLocatorReturnFields(ReferenceTable, Result.KeyField);

        Result.ReturnFields.AddRange(ReturnFields);
        Result.ReturnFields = DistinctFieldList(Result.ReturnFields);

        return Result;
    }
    /// <summary>
    /// Parses locator return fields from field metadata text.
    /// </summary>
    static List<string> ParseLocatorReturnFields(string MetadataText)
    {
        if (string.IsNullOrWhiteSpace(MetadataText))
            return [];

        int OpenIndex = MetadataText.IndexOf('(');
        int CloseIndex = MetadataText.LastIndexOf(')');

        if (OpenIndex < 0 || CloseIndex <= OpenIndex)
            return [];

        string FieldText = MetadataText.Substring(OpenIndex + 1, CloseIndex - OpenIndex - 1).Trim();
        return ParseFieldNameList(FieldText);
    }
    /// <summary>
    /// Parses a comma separated field name list.
    /// </summary>
    static List<string> ParseFieldNameList(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return [];

        return Text
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
    }
    /// <summary>
    /// Returns default locator return fields.
    /// </summary>
    static List<string> GetDefaultLocatorReturnFields(SchemaTable Table, string KeyField)
    {
        List<string> Result = [];

        if (Table == null)
            return Result;

        AddDefaultLocatorReturnField(Result, Table, KeyField, "Code");
        AddDefaultLocatorReturnField(Result, Table, KeyField, "Name");

        foreach (SchemaField Field in Table.Fields)
        {
            if (Field.Name.IsSameText(KeyField))
                continue;
            if (Result.Any(x => x.IsSameText(Field.Name)))
                continue;
            if (!Field.Name.EndsWithText("Code") && !Field.Name.EndsWithText("Name"))
                continue;

            Result.Add(Field.Name);
        }

        return Result;
    }
    /// <summary>
    /// Adds a default locator return field.
    /// </summary>
    static void AddDefaultLocatorReturnField(List<string> Fields, SchemaTable Table, string KeyField, string FieldName)
    {
        SchemaField Field = Table.FindField(FieldName);
        if (Field == null)
            return;
        if (Field.Name.IsSameText(KeyField))
            return;
        if (Fields.Any(x => x.IsSameText(Field.Name)))
            return;

        Fields.Add(Field.Name);
    }
    /// <summary>
    /// Returns a distinct field list.
    /// </summary>
    static List<string> DistinctFieldList(List<string> Fields)
    {
        List<string> Result = [];

        foreach (string Field in Fields)
        {
            if (string.IsNullOrWhiteSpace(Field))
                continue;
            if (Result.Any(x => x.IsSameText(Field)))
                continue;

            Result.Add(Field);
        }

        return Result;
    }
    /// <summary>
    /// Builds optional locator class name argument.
    /// </summary>
    static string BuildOptionalClassNameArgument(string ClassName)
    {
        if (string.IsNullOrWhiteSpace(ClassName))
            return string.Empty;

        return ", \"" + EscapeString(ClassName) + "\"";
    }
    /// <summary>
    /// Builds C# source for a string array.
    /// </summary>
    static string BuildStringArray(List<string> Items)
    {
        if (Items == null || Items.Count == 0)
            return "null";

        return "new string[] { " + string.Join(", ", Items.Select(x => "\"" + EscapeString(x) + "\"")) + " }";
    }
    /// <summary>
    /// Returns a field name without Id suffix.
    /// </summary>
    static string RemoveIdSuffix(string Value)
    {
        if (!string.IsNullOrWhiteSpace(Value) && Value.EndsWithText("Id"))
            return Value.Substring(0, Value.Length - 2);
        return Value;
    }
    /// <summary>
    /// Finds the master field name of a detail table.
    /// </summary>
    static string FindMasterFieldName(SchemaTable DetailTable)
    {
        SchemaField Field = DetailTable.Fields.FirstOrDefault(x => x.MetadataKind == FieldMetadataKind.Master);
        if (Field != null)
            return Field.Name;

        SchemaForeignKey ForeignKey = DetailTable.ForeignKeys.FirstOrDefault(x => x.ReferenceTable.IsSameText(DetailTable.MasterName));
        if (ForeignKey != null)
            return ForeignKey.FieldName;

        return DetailTable.MasterName + "Id";
    }
    /// <summary>
    /// Returns true if a field is filterable.
    /// </summary>
    static bool IsFilterableField(SchemaField Field, string Alias)
    {
        if (string.IsNullOrWhiteSpace(Alias))
            return false;
        if (Alias.IsSameText("Id") || Alias.EndsWithText("Id"))
            return false;
        if (Field.DataType.IsBlob())
            return false;
        return Field.DataType == DataFieldType.String || Field.DataType.IsNumeric() || Field.DataType.IsDateTime() || Field.DataType == DataFieldType.Boolean;
    }
    /// <summary>
    /// Returns an unique alias.
    /// </summary>
    static string UniqueAlias(string BaseAlias, HashSet<string> Aliases)
    {
        string Result = !string.IsNullOrWhiteSpace(BaseAlias) ? BaseAlias : "X";
        string Original = Result;
        int Counter = 2;

        while (Aliases.Contains(Result))
            Result = Original + Counter++;

        Aliases.Add(Result);
        return Result;
    }
    /// <summary>
    /// Builds field flags text.
    /// </summary>
    static string BuildFlags(SchemaField Field, FieldFlagsText Extra = FieldFlagsText.Default)
    {
        List<string> Parts = [];

        if (Extra != FieldFlagsText.None)
            Parts.Add("FieldFlags.Visible");
        if (!Field.IsNullable)
            Parts.Add("FieldFlags.Required");
        if (Extra == FieldFlagsText.LargeMemo)
            Parts.Add("FieldFlags.LargeMemo");

        if (Parts.Count == 0)
            return "FieldFlags.None";

        return string.Join(" | ", Parts);
    }
    /// <summary>
    /// Escapes a C# string.
    /// </summary>
    static string EscapeString(string Value)
    {
        return (Value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
    /// <summary>
    /// Escapes a C# verbatim string.
    /// </summary>
    static string EscapeVerbatim(string Value)
    {
        return (Value ?? string.Empty).Replace("\"", "\"\"");
    }
    /// <summary>
    /// Returns a safe C# identifier.
    /// </summary>
    static string SafeIdentifier(string Value)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return "X";

        StringBuilder SB = new();

        foreach (char C in Value)
            SB.Append(char.IsLetterOrDigit(C) || C == '_' ? C : '_');

        if (SB.Length == 0)
            SB.Append("X");

        if (char.IsDigit(SB[0]))
            SB.Insert(0, "X");

        return SB.ToString();
    }
    /// <summary>
    /// Returns a C# boolean literal.
    /// </summary>
    static string BoolLiteral(bool Value) => Value ? "true" : "false";

    // ● private classes
    /// <summary>
    /// Parsed schema script.
    /// </summary>
    private class SchemaScript
    {
        // ● private fields
        List<SchemaTable> fTables = [];
        readonly Dictionary<string, EnumInfo> fEnums = new(StringComparer.OrdinalIgnoreCase);
        
        void ResolveCreationOrders()
        {
            int Order = 0;

            foreach (SchemaTable Table in Tables)
            {
                if (Table.CreationOrder > Order)
                    Order = Table.CreationOrder;
            }

            foreach (SchemaTable Table in Tables)
            {
                if (Table.CreationOrder <= 0)
                    Table.CreationOrder = ++Order;
            }
        }

        // ● static public
        /// <summary>
        /// Parses a schema script.
        /// </summary>
        static public SchemaScript Parse(string SchemaSql)
        {
            SchemaScript Result = new();
            Result.ParseEnumBlock(SchemaSql);
            MatchCollection Matches = Regex.Matches(SchemaSql, @"CREATE\s+TABLE\s+\{TableName\}\s*\(", RegexOptions.IgnoreCase);

            foreach (Match Match in Matches)
            {
                string HeaderText = ExtractHeaderText(SchemaSql, Match.Index);
                string CreateSqlText = ExtractCreateTableSql(SchemaSql, Match.Index);
                SchemaTable Table = SchemaTable.Parse(HeaderText, CreateSqlText);
                Result.Tables.Add(Table);
            }

            Result.ResolveReferences();
            Result.ResolveLookupHeuristics();
            Result.ResolveDetails();
            
            Result.ResolveCreationOrders();

            return Result;
        }

        // ● public
        /// <summary>
        /// Finds a table by name.
        /// </summary>
        public SchemaTable FindTable(string Name) => Tables.FirstOrDefault(x => x.Name.IsSameText(Name));
        /// <summary>
        /// Finds an enum definition by name.
        /// </summary>
        public EnumInfo FindEnum(string Name) => !string.IsNullOrWhiteSpace(Name) && fEnums.TryGetValue(Name, out EnumInfo Result) ? Result : null;
        /// <summary>
        /// Returns detail tables of a master table.
        /// </summary>
        public List<SchemaTable> GetDetailsOf(SchemaTable MasterTable)
        {
            return Tables
                .Where(x => x.MasterName.IsSameText(MasterTable.Name))
                .OrderBy(x => x.CreationOrder)
                .ToList();
        }
        /// <summary>
        /// Validates the schema.
        /// </summary>
        public void Validate()
        {
            foreach (SchemaTable Table in Tables)
            {
                if (string.IsNullOrWhiteSpace(Table.Name))
                    throw new TripousDataException("Schema table has no name.");
                //if (Table.CreationOrder <= 0)
                //    throw new TripousDataException("Schema table has no valid CreationOrder: " + Table.Name);
                if (Table.IsTopTable && string.IsNullOrWhiteSpace(Table.ModuleName))
                    throw new TripousDataException("Top table has no Module: " + Table.Name);
                if (Table.IsTopTable && string.IsNullOrWhiteSpace(Table.GroupName))
                    throw new TripousDataException("Top table has no Group: " + Table.Name);
                if (!Table.IsTopTable && string.IsNullOrWhiteSpace(Table.MasterName))
                    throw new TripousDataException("Detail table has no Master: " + Table.Name);
            }

            CheckDuplicateCreationOrders();
            CheckCircularReferences();
        }

        // ● private
        /// <summary>
        /// Parses the enums block.
        /// </summary>
        void ParseEnumBlock(string SchemaSql)
        {
            foreach (string Line in ExtractNamedBlockLines(SchemaSql, "Enums"))
            {
                EnumInfo Info = ParseEnumInfo(Line);
                if (!string.IsNullOrWhiteSpace(Info.Name))
                    fEnums[Info.Name] = Info;
            }
        }
        /// <summary>
        /// Extracts lines from a named global block.
        /// </summary>
        static List<string> ExtractNamedBlockLines(string SchemaSql, string Name)
        {
            List<string> Result = [];
            Match M = Regex.Match(SchemaSql, @"^\s*" + Regex.Escape(Name) + @"\s+begin\s*$([\s\S]*?)^\s*" + Regex.Escape(Name) + @"\s+end\s*$", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if (!M.Success)
                return Result;

            string Text = M.Groups[1].Value;
            string[] Lines = Text.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach (string Line in Lines)
            {
                string S = Line.Trim();
                if (string.IsNullOrWhiteSpace(S))
                    continue;
                if (S.StartsWith("--"))
                    continue;

                Result.Add(S);
            }

            return Result;
        }
        /// <summary>
        /// Parses an enum definition line.
        /// </summary>
        static EnumInfo ParseEnumInfo(string Text)
        {
            EnumInfo Result = new();
            if (string.IsNullOrWhiteSpace(Text))
                return Result;

            Text = Text.Trim();
            string TypeName = string.Empty;
            int OpenIndex = Text.IndexOf('(');
            int CloseIndex = Text.LastIndexOf(')');
            if (OpenIndex >= 0 && CloseIndex > OpenIndex)
            {
                TypeName = Text.Substring(OpenIndex + 1, CloseIndex - OpenIndex - 1).Trim();
                Text = Text.Substring(0, OpenIndex).Trim();
            }

            List<string> Parts = SplitHeaderTokens(Text);
            if (Parts.Count > 0)
                Result.Name = Parts[0];
            if (!string.IsNullOrWhiteSpace(TypeName))
                Result.TypeName = TypeName;
            else if (Parts.Count > 1)
                Result.TypeName = Parts[1];
            else
                Result.TypeName = Result.Name;

            return Result;
        }
        /// <summary>
        /// Resolves foreign key references.
        /// </summary>
        void ResolveReferences()
        {
            foreach (SchemaTable Table in Tables)
            {
                foreach (SchemaForeignKey ForeignKey in Table.ForeignKeys)
                {
                    SchemaField Field = Table.FindField(ForeignKey.FieldName);
                    if (Field != null)
                    {
                        Field.ForeignKey = ForeignKey;
                        Field.IsForeignKey = true;
                    }
                }
            }
        }
        /// <summary>
        /// Resolves lookup tables using explicit metadata and heuristics.
        /// </summary>
        void ResolveLookupHeuristics()
        {
            foreach (SchemaTable Table in Tables)
            {
                if (Table.IsLookupSpecified)
                    continue;

                if (!Table.IsTopTable)
                    continue;

                string[] Names = Table.Fields.Select(x => x.Name).ToArray();

                Table.IsLookup =
                    FieldListEquals(Names, "Id", "Name")
                    || FieldListEquals(Names, "Id", "Code", "Name")
                    || FieldListEquals(Names, "Id", "Name", "IsActive")
                    || FieldListEquals(Names, "Id", "Code", "Name", "IsActive");
            }
        }
        /// <summary>
        /// Resolves detail metadata.
        /// </summary>
        void ResolveDetails()
        {
            foreach (SchemaTable Table in Tables.Where(x => !x.IsTopTable))
            {
                // infer master table from field metadata
                if (string.IsNullOrWhiteSpace(Table.MasterName))
                {
                    SchemaField Field = Table.Fields.FirstOrDefault(x => x.MetadataKind == FieldMetadataKind.Master);

                    if (Field != null)
                    {
                        SchemaForeignKey FK = Table.ForeignKeys.FirstOrDefault(x => x.FieldName.IsSameText(Field.Name));

                        if (FK != null)
                            Table.MasterName = FK.ReferenceTable;
                    }
                }

                // no master found, skip
                if (string.IsNullOrWhiteSpace(Table.MasterName))
                    continue;

                // locate master field
                SchemaField MasterField = Table.Fields.FirstOrDefault(x => x.MetadataKind == FieldMetadataKind.Master);

                if (MasterField == null)
                {
                    MasterField = Table.Fields.FirstOrDefault(x =>
                        x.ForeignKey != null &&
                        x.ForeignKey.ReferenceTable.IsSameText(Table.MasterName));
                }

                if (MasterField != null)
                {
                    Table.MasterField = MasterField;
                    Table.IsOneToOne = MasterField.IsOneToOne;
                }
            }
        }
        /// <summary>
        /// Checks duplicate creation orders.
        /// </summary>
        void CheckDuplicateCreationOrders()
        {
            var Group = Tables.GroupBy(x => x.CreationOrder).FirstOrDefault(x => x.Count() > 1);
            if (Group != null)
                throw new TripousDataException("Duplicate CreationOrder: " + Group.Key);
        }
        /// <summary>
        /// Checks circular references.
        /// </summary>
        void CheckCircularReferences()
        {
            HashSet<string> Done = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> Visiting = new(StringComparer.OrdinalIgnoreCase);

            foreach (SchemaTable Table in Tables)
                Visit(Table, Done, Visiting);
        }
        /// <summary>
        /// Visits a table for circular reference detection.
        /// </summary>
        void Visit(SchemaTable Table, HashSet<string> Done, HashSet<string> Visiting)
        {
            if (Done.Contains(Table.Name))
                return;

            if (Visiting.Contains(Table.Name))
                throw new TripousDataException("Circular schema reference detected: " + Table.Name);

            Visiting.Add(Table.Name);

            foreach (SchemaForeignKey ForeignKey in Table.ForeignKeys)
            {
                if (ForeignKey.ReferenceTable.IsSameText(Table.Name))
                    continue;

                SchemaTable RefTable = FindTable(ForeignKey.ReferenceTable);
                if (RefTable != null)
                    Visit(RefTable, Done, Visiting);
            }

            Visiting.Remove(Table.Name);
            Done.Add(Table.Name);
        }
        /// <summary>
        /// Returns true if two field lists are equal.
        /// </summary>
        static bool FieldListEquals(string[] Names, params string[] Expected)
        {
            if (Names.Length != Expected.Length)
                return false;

            for (int i = 0; i < Names.Length; i++)
                if (!Names[i].IsSameText(Expected[i]))
                    return false;

            return true;
        }

        // ● properties
        /// <summary>
        /// Parsed tables.
        /// </summary>
        public List<SchemaTable> Tables { get => fTables; set => fTables = value; }
        /// <summary>
        /// Top module tables.
        /// </summary>
        public List<SchemaTable> TopTables => Tables.Where(x => x.IsTopTable).ToList();
    }

    /// <summary>
    /// Parsed schema table.
    /// </summary>
    private class SchemaTable
    {
        // ● private fields
        List<SchemaField> fFields = [];
        List<SchemaForeignKey> fForeignKeys = [];

        // ● static public
        /// <summary>
        /// Parses a schema table.
        /// </summary>
        static public SchemaTable Parse(string HeaderText, string CreateSqlText)
        {
            SchemaTable Result = new();
            Result.HeaderText = HeaderText;
            Result.CreateSqlText = CreateSqlText;
            Result.FullSqlText = HeaderText.Trim() + Environment.NewLine + CreateSqlText.Trim();

            Result.Name = GetHeaderValue(HeaderText, "Table");
            Result.ParseModuleHeader(GetHeaderValue(HeaderText, "Module"));
            Result.GroupName = GetHeaderValue(HeaderText, "Group");
            Result.ParseFormHeader(GetHeaderValue(HeaderText, "Form"));
            Result.MasterName = GetHeaderValue(HeaderText, "Master");
            Result.UiVisible = !GetHeaderFlag(HeaderText, "NotUiVisible");
            Result.IsReadOnly = GetHeaderFlag(HeaderText, "IsReadOnly");
            Result.IsSingleSelect = GetHeaderFlag(HeaderText, "IsSingleSelect");
            Result.UseFilters = !GetHeaderFlag(HeaderText, "NoFilters");
            Result.CascadeDeletes = !GetHeaderFlag(HeaderText, "NoCascadeDeletes");
            Result.GuidOids = !GetHeaderFlag(HeaderText, "NoGuidOids");
            Result.CreationOrder = GetHeaderInt(HeaderText, "CreationOrder", 0);
            Result.IsLookupSpecified = HeaderKeyExists(HeaderText, "IsLookup");
            Result.IsLookup = GetHeaderBool(HeaderText, "IsLookup", false);

            if (string.IsNullOrWhiteSpace(Result.Name))
                throw new TripousDataException("Schema table header has no Table value.");

            Result.ResolveFormDefaults();
            Result.ParseBody();
            return Result;
        }

        // ● public
        /// <summary>
        /// Finds a field by name.
        /// </summary>
        public SchemaField FindField(string Name) => Fields.FirstOrDefault(x => x.Name.IsSameText(Name));

        // ● private
        /// <summary>
        /// Parses module header text.
        /// </summary>
        void ParseModuleHeader(string Text)
        {
            List<string> Parts = SplitHeaderTokens(Text);
            if (Parts.Count == 0)
                return;

            ModuleName = Parts[0].IsSameText("Default") ? Name : Parts[0];

            if (Parts.Count > 1)
                ModuleClassName = Parts[1];
        }
        /// <summary>
        /// Parses form header text.
        /// </summary>
        void ParseFormHeader(string Text)
        {
            IsFormSpecified = !string.IsNullOrWhiteSpace(Text);
            List<string> Parts = SplitHeaderTokens(Text);
            if (Parts.Count == 0)
                return;

            FormName = Parts[0];

            if (Parts.Count > 1)
                FormClassName = Parts[1];
            if (Parts.Count > 2)
                ItemPageClassName = Parts[2];
        }
        /// <summary>
        /// Resolves default form values after module parsing.
        /// </summary>
        void ResolveFormDefaults()
        {
            if (string.IsNullOrWhiteSpace(ModuleName))
                return;

            if (string.IsNullOrWhiteSpace(FormName))
                FormName = ModuleName;
            else if (FormName.IsSameText("Default"))
                FormName = ModuleName;
        }
        /// <summary>
        /// Moves inline comments before comma.
        /// </summary>
        static string MoveInlineCommentsBeforeComma(string Text)
        {
            return Regex.Replace(
                Text,
                @",\s*--(.*?)$",
                " --$1,",
                RegexOptions.Multiline);
        }
        /// <summary>
        /// Parses table body.
        /// </summary>
        void ParseBody()
        {
           
            string Body = GetCreateTableBody(CreateSqlText);
            Body = MoveInlineCommentsBeforeComma(Body);
            List<string> Lines = SplitSqlItems(Body);

            foreach (string Line in Lines)
            {
                SplitInlineComment(Line, out string SqlPart, out string CommentPart);
                string CleanLine = SqlPart.Trim();

                if (string.IsNullOrWhiteSpace(CleanLine))
                    continue;

                if (CleanLine.StartsWithText("FOREIGN KEY"))
                {
                    ForeignKeys.Add(ParseForeignKey(CleanLine));
                    continue;
                }

                if (CleanLine.StartsWithText("CONSTRAINT") || CleanLine.StartsWithText("UNIQUE") || CleanLine.StartsWithText("PRIMARY KEY"))
                    continue;

                if (!IsFieldLine(CleanLine))
                    continue;

                SchemaField Field = SchemaField.Parse(CleanLine, CommentPart);
                Fields.Add(Field);
            }

            SchemaField PrimaryKey = Fields.FirstOrDefault(x => x.IsPrimaryKey);
            PrimaryKey = PrimaryKey ?? Fields.FirstOrDefault(x => x.Name.IsSameText("Id"));
            PrimaryKey = PrimaryKey ?? Fields.FirstOrDefault();

            if (PrimaryKey == null)
                throw new TripousDataException("Schema table has no fields: " + Name);

            PrimaryKey.IsPrimaryKey = true;
            PrimaryKey.IsNullable = false;
            PrimaryKeyField = PrimaryKey;
        }

        // ● properties
        /// <summary>
        /// Table name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Module name.
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// Module class name.
        /// </summary>
        public string ModuleClassName { get; set; }
        /// <summary>
        /// Form name.
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// Form class name.
        /// </summary>
        public string FormClassName { get; set; }
        /// <summary>
        /// Item page class name.
        /// </summary>
        public string ItemPageClassName { get; set; }
        /// <summary>
        /// True when Form was explicitly defined.
        /// </summary>
        public bool IsFormSpecified { get; set; }
        /// <summary>
        /// Module group name.
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Master table name.
        /// </summary>
        public string MasterName { get; set; }
        /// <summary>
        /// True when this table is lookup.
        /// </summary>
        public bool IsLookup { get; set; }
        /// <summary>
        /// True when IsLookup was explicitly defined.
        /// </summary>
        public bool IsLookupSpecified { get; set; }
        /// <summary>
        /// True when table is visible in UI.
        /// </summary>
        public bool UiVisible { get; set; } = true;
        /// <summary>
        /// True when form is read-only.
        /// </summary>
        public bool IsReadOnly { get; set; }
        /// <summary>
        /// True when module uses single-select mode.
        /// </summary>
        public bool IsSingleSelect { get; set; }
        /// <summary>
        /// True when module uses filters.
        /// </summary>
        public bool UseFilters { get; set; } = true;
        /// <summary>
        /// True when module uses cascade deletes.
        /// </summary>
        public bool CascadeDeletes { get; set; } = true;
        /// <summary>
        /// True when module uses GUID OIDs.
        /// </summary>
        public bool GuidOids { get; set; } = true;
        /// <summary>
        /// True when this is an one-to-one detail table.
        /// </summary>
        public bool IsOneToOne { get; set; }
        /// <summary>
        /// Creation order.
        /// </summary>
        public int CreationOrder { get; set; }
        /// <summary>
        /// Header text.
        /// </summary>
        public string HeaderText { get; set; }
        /// <summary>
        /// CREATE TABLE text.
        /// </summary>
        public string CreateSqlText { get; set; }
        /// <summary>
        /// Full SQL text with header.
        /// </summary>
        public string FullSqlText { get; set; }
        /// <summary>
        /// Primary key field.
        /// </summary>
        public SchemaField PrimaryKeyField { get; set; }
        /// <summary>
        /// Master field.
        /// </summary>
        public SchemaField MasterField { get; set; }
        /// <summary>
        /// True when this table is top table of a module.
        /// </summary>
        public bool IsTopTable => !string.IsNullOrWhiteSpace(ModuleName) && !string.IsNullOrWhiteSpace(GroupName);
        /// <summary>
        /// Parsed fields.
        /// </summary>
        public List<SchemaField> Fields { get => fFields; set => fFields = value; }
        /// <summary>
        /// Parsed foreign keys.
        /// </summary>
        public List<SchemaForeignKey> ForeignKeys { get => fForeignKeys; set => fForeignKeys = value; }
    }

    /// <summary>
    /// Parsed schema field.
    /// </summary>
    private class SchemaField
    {
        // ● static public
        /// <summary>
        /// Parses a schema field.
        /// </summary>
        static public SchemaField Parse(string SqlPart, string CommentPart)
        {
            string[] Parts = SqlPart.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (Parts.Length < 2)
                throw new TripousDataException("Invalid field line: " + SqlPart);

            FieldMetadata Metadata = ParseFieldMetadata(CommentPart);

            SchemaField Result = new();
            Result.Name = Parts[0];
            Result.OriginalSqlText = SqlPart;
            Result.OriginalCommentText = CommentPart;
            Result.MetadataKind = Metadata.Kind;
            Result.MetadataText = Metadata.MetadataText;
            Result.MetadataName = Metadata.MetadataName;
            Result.CommentText = Metadata.CommentText;
            Result.IsOneToOne = Metadata.IsOneToOne;
            Result.IsPrimaryKey = SqlPart.ContainsText("primary key");
            Result.IsNullable = !SqlPart.ContainsText("@NOT_NULL") && !SqlPart.ContainsText("not null");
            Result.DefaultValue = ParseDefaultValue(SqlPart);

            ParseFieldDataType(Result, SqlPart);

            return Result;
        }

        // ● properties
        /// <summary>
        /// Field name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Original SQL text.
        /// </summary>
        public string OriginalSqlText { get; set; }
        /// <summary>
        /// Original comment text.
        /// </summary>
        public string OriginalCommentText { get; set; }
        /// <summary>
        /// Metadata text.
        /// </summary>
        public string MetadataText { get; set; }
        /// <summary>
        /// Plain comment text.
        /// </summary>
        public string CommentText { get; set; }
        /// <summary>
        /// Optional metadata name.
        /// </summary>
        public string MetadataName { get; set; }
        /// <summary>
        /// Field data type.
        /// </summary>
        public DataFieldType DataType { get; set; }
        /// <summary>
        /// Field max length.
        /// </summary>
        public int MaxLength { get; set; } = -1;
        /// <summary>
        /// Field decimals.
        /// </summary>
        public int Decimals { get; set; } = -1;
        /// <summary>
        /// True when nullable.
        /// </summary>
        public bool IsNullable { get; set; } = true;
        /// <summary>
        /// True when primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// True when foreign key.
        /// </summary>
        public bool IsForeignKey { get; set; }
        /// <summary>
        /// True when one-to-one relation marker exists.
        /// </summary>
        public bool IsOneToOne { get; set; }
        /// <summary>
        /// Default value.
        /// </summary>
        public string DefaultValue { get; set; }
        /// <summary>
        /// Metadata kind.
        /// </summary>
        public FieldMetadataKind MetadataKind { get; set; }
        /// <summary>
        /// Foreign key.
        /// </summary>
        public SchemaForeignKey ForeignKey { get; set; }
    }

    /// <summary>
    /// Parsed foreign key.
    /// </summary>
    private class SchemaForeignKey
    {
        // ● properties
        /// <summary>
        /// Local field name.
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Referenced table name.
        /// </summary>
        public string ReferenceTable { get; set; }
        /// <summary>
        /// Referenced field name.
        /// </summary>
        public string ReferenceField { get; set; }
    }


    /// <summary>
    /// Parsed locator metadata.
    /// </summary>
    private class LocatorInfo
    {
        // ● properties
        /// <summary>
        /// Locator name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Locator table name.
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Join alias.
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Locator key field.
        /// </summary>
        public string KeyField { get; set; }
        /// <summary>
        /// Locator class name.
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// Return fields.
        /// </summary>
        public List<string> ReturnFields { get; set; } = [];
    }

    /// <summary>
    /// Parsed enum metadata.
    /// </summary>
    private class EnumInfo
    {
        // ● properties
        /// <summary>
        /// Enum lookup source name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Enum type name.
        /// </summary>
        public string TypeName { get; set; }
    }

    /// <summary>
    /// Field metadata.
    /// </summary>
    private class FieldMetadata
    {
        // ● properties
        /// <summary>
        /// Metadata kind.
        /// </summary>
        public FieldMetadataKind Kind { get; set; }
        /// <summary>
        /// Metadata text.
        /// </summary>
        public string MetadataText { get; set; }
        /// <summary>
        /// Plain comment text.
        /// </summary>
        public string CommentText { get; set; }
        /// <summary>
        /// Optional metadata name.
        /// </summary>
        public string MetadataName { get; set; }
        /// <summary>
        /// True when one-to-one.
        /// </summary>
        public bool IsOneToOne { get; set; }
    }

    /// <summary>
    /// Field metadata kind.
    /// </summary>
    private enum FieldMetadataKind
    {
        None = 0,
        Master = 1,
        Lookup = 2,
        Enum = 3,
        Locator = 4,
        CorrelationLookup = 5,
        CorrelationLocator = 6,
        LargeMemo = 7,
    }

    /// <summary>
    /// Select build result.
    /// </summary>
    private class SelectBuildResult
    {
        // ● properties
        /// <summary>
        /// SQL text.
        /// </summary>
        public string SqlText { get; set; }
        /// <summary>
        /// Filter fields.
        /// </summary>
        public List<SelectField> FilterFields { get; set; } = [];
        public Dictionary<string, DataColumnType> ColumnTypes  { get; set; } = [];
    }

    /// <summary>
    /// Select field.
    /// </summary>
    private class SelectField
    {
        // ● construction
        /// <summary>
        /// Constructor.
        /// </summary>
        public SelectField(string Alias, DataFieldType DataType)
        {
            this.Alias = Alias;
            this.DataType = DataType;
        }

        // ● properties
        /// <summary>
        /// Field alias.
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// Data type.
        /// </summary>
        public DataFieldType DataType { get; set; }
    }

    /// <summary>
    /// Field flags text helper.
    /// </summary>
    private enum FieldFlagsText
    {
        Default = 0,
        None = 1,
        LargeMemo = 2,
    }
}
