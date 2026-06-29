/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.RegBuilder;

/// <summary>
/// Parses a Tripous schema script and generates schema, module and form registration source code.
/// </summary>
static public class SchemaRegistrationBuilder
{
    // ● static public
    /// <summary>
    /// Returns the generated C# source file names for a schema version.
    /// </summary>
    static public string[] GetGeneratedSourceFileNames(int SchemaVersion)
    {
        string Prefix = $"RegistryVersion{SchemaVersion}";
        return
        [
            $"SchemaVersion{SchemaVersion}.cs",
            $"{Prefix}.cs",
            $"{Prefix}.Modules.cs",
            $"{Prefix}.Forms.cs",
            $"{Prefix}.Lookups.cs",
            $"{Prefix}.Locators.cs",
            $"{Prefix}.CodeProviders.cs"
        ];
    }
    /// <summary>
    /// Parses a Tripous schema script and generates schema, module and form registration source code.
    /// </summary>
    static public SchemaParserResult Parse(string SchemaSql, int SchemaVersion)
        => Parse(SchemaSql, SchemaVersion, DuplicateCheck.None);
    /// <summary>
    /// Parses a Tripous schema project and writes generated files to an output folder.
    /// </summary>
    static public SchemaParserResult Parse(RegBuilderProject Project, string OutputFolderPath)
    {
        if (Project == null)
            throw new TripousArgumentNullException(nameof(Project));
        if (string.IsNullOrWhiteSpace(Project.SchemaFilePath))
            throw new TripousArgumentNullException(nameof(Project.SchemaFilePath));
        if (string.IsNullOrWhiteSpace(OutputFolderPath))
            throw new TripousArgumentNullException(nameof(OutputFolderPath));
        if (string.IsNullOrWhiteSpace(Project.NamespaceName))
            throw new TripousArgumentNullException(nameof(Project.NamespaceName));

        string SchemaSql = File.ReadAllText(Project.SchemaFilePath);
        List<string> ReferenceSchemaSqls = [];
        List<string> MissingReferenceFilePaths = [];
        foreach (string ReferenceFilePath in Project.ReferenceFilePaths)
        {
            if (!string.IsNullOrWhiteSpace(ReferenceFilePath) && File.Exists(ReferenceFilePath))
                ReferenceSchemaSqls.Add(File.ReadAllText(ReferenceFilePath));
            else if (!string.IsNullOrWhiteSpace(ReferenceFilePath))
                MissingReferenceFilePaths.Add(ReferenceFilePath);
        }

        SchemaParserResult Result = Parse(SchemaSql, Project.SchemaVersion, Project.DuplicateChecks, Project.NamespaceName, ReferenceSchemaSqls);
        foreach (string ReferenceFilePath in MissingReferenceFilePaths)
            AddWarning(Result, "REFERENCE_SCHEMA_FILE_NOT_FOUND", "Reference schema file not found: " + ReferenceFilePath);

        if (!Result.Messages.Any(x => x.Code == "SCHEMA_PARSE_ERROR"))
            WriteOutputFiles(Result, Project.SchemaVersion, OutputFolderPath);

        return Result;
    }
    /// <summary>
    /// Parses a Tripous schema script and generates schema, module and form registration source code.
    /// </summary>
    static public SchemaParserResult Parse(string SchemaSql, int SchemaVersion, DuplicateCheck DuplicateChecks)
        => Parse(SchemaSql, SchemaVersion, DuplicateChecks, string.Empty);
    /// <summary>
    /// Parses a Tripous schema script and generates schema, module and form registration source code.
    /// </summary>
    static SchemaParserResult Parse(string SchemaSql, int SchemaVersion, DuplicateCheck DuplicateChecks, string NamespaceName)
        => Parse(SchemaSql, SchemaVersion, DuplicateChecks, NamespaceName, []);
    /// <summary>
    /// Parses a Tripous schema script and generates schema, module and form registration source code.
    /// </summary>
    static SchemaParserResult Parse(string SchemaSql, int SchemaVersion, DuplicateCheck DuplicateChecks, string NamespaceName, List<string> ReferenceSchemaSqls)
    {
        if (string.IsNullOrWhiteSpace(SchemaSql))
            throw new TripousArgumentNullException(nameof(SchemaSql));

        SchemaParserResult Result = new();
        SchemaScript Script;

        try
        {
            Script = SchemaScript.Parse(SchemaSql, ReferenceSchemaSqls);
        }
        catch (Exception ex)
        {
            AddError(Result, "SCHEMA_PARSE_ERROR", ex.Message);
            return Result;
        }

        ValidateScript(Result, Script);
        CollectCodeProviderPatterns(Result, Script);
        Script.ResolveCreationOrders(Result);

        try
        {
            Result.SchemaSql = BuildOrderedSchemaSql(Script);
        }
        catch (Exception ex)
        {
            AddError(Result, "SCHEMA_SQL_BUILD_FAILED", ex.Message);
        }
        try
        {
            Result.CreateTablesSourceCode = BuildCreateTablesSourceCode(Script, Result, SchemaVersion, NamespaceName);
        }
        catch (Exception ex)
        {
            AddError(Result, "CREATE_TABLES_SOURCE_BUILD_FAILED", ex.Message);
        }
        try
        {
            Result.RegistryVersionSourceCode = BuildRegistryVersionSourceCode(SchemaVersion, NamespaceName);
        }
        catch (Exception ex)
        {
            AddError(Result, "REGISTRY_VERSION_SOURCE_BUILD_FAILED", ex.Message);
        }
        try
        {
            Result.ModuleDefsSourceCode = BuildModuleDefsSourceCode(Script, SchemaVersion, NamespaceName);
        }
        catch (Exception ex)
        {
            AddError(Result, "MODULE_DEFS_SOURCE_BUILD_FAILED", ex.Message);
        }
        try
        {
            Result.FormDefsSourceCode = BuildFormDefsSourceCode(Script, SchemaVersion, NamespaceName);
        }
        catch (Exception ex)
        {
            AddError(Result, "FORM_DEFS_SOURCE_BUILD_FAILED", ex.Message);
        }
        try
        {
            Result.LookupDefsSourceCode = BuildLookupDefsSourceCode(Script, SchemaVersion, NamespaceName);
        }
        catch (Exception ex)
        {
            AddError(Result, "LOOKUP_DEFS_SOURCE_BUILD_FAILED", ex.Message);
        }
        try
        {
            Result.LocatorDefsSourceCode = BuildLocatorDefsSourceCode(Script, SchemaVersion, NamespaceName);
        }
        catch (Exception ex)
        {
            AddError(Result, "LOCATOR_DEFS_SOURCE_BUILD_FAILED", ex.Message);
        }
        try
        {
            Result.CodeProviderDefsSourceCode = BuildCodeProviderDefsSourceCode(Script, Result, SchemaVersion, NamespaceName);
        }
        catch (Exception ex)
        {
            AddError(Result, "CODE_PROVIDER_DEFS_SOURCE_BUILD_FAILED", ex.Message);
        }

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
        ValidateDuplicateGeneratedMethodNames(Result, Script);
        ValidateTableHeaders(Result, Script);
        ValidateForeignKeys(Result, Script);
        ValidateCircularReferences(Result, Script);
        ValidateSuspiciousUniqueConstraints(Result, Script);
        ValidateFieldMetadata(Result, Script);
        ValidateSnapshotFields(Result, Script);
        ValidateDetailOrder(Result, Script);
        ValidateLookupFields(Result, Script);
        ValidateEnumFields(Result, Script);
        ValidateLocatorFields(Result, Script);
        ValidateFilterFields(Result, Script);
    }
    /// <summary>
    /// Validates field metadata.
    /// </summary>
    static void ValidateFieldMetadata(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                foreach (string Error in Field.MetadataErrors)
                    AddError(Result, "FIELD_METADATA_INVALID", Table.Name + "." + Field.Name + ": " + Error);
            }
        }
    }
    /// <summary>
    /// Validates snapshot fields.
    /// </summary>
    static void ValidateSnapshotFields(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaField Field in Table.Fields.Where(item => !string.IsNullOrWhiteSpace(item.SnapshotOf)))
            {
                string[] Parts = Field.SnapshotOf.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (Parts.Length != 2)
                {
                    AddError(Result, "SNAPSHOT_SOURCE_INVALID", "Snapshot source must use Table.Field format: " + Table.Name + "." + Field.Name + " -> " + Field.SnapshotOf);
                    continue;
                }

                SchemaTable SourceTable = Script.FindTable(Parts[0]);
                if (SourceTable == null)
                {
                    AddError(Result, "SNAPSHOT_TABLE_NOT_FOUND", "Snapshot source table not found: " + Table.Name + "." + Field.Name + " -> " + Field.SnapshotOf);
                    continue;
                }
                if (SourceTable.FindField(Parts[1]) == null)
                    AddError(Result, "SNAPSHOT_FIELD_NOT_FOUND", "Snapshot source field not found: " + Table.Name + "." + Field.Name + " -> " + Field.SnapshotOf);
            }
        }
    }
    /// <summary>
    /// Validates module detail order entries.
    /// </summary>
    static void ValidateDetailOrder(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable TopTable in Script.TopTables)
        {
            foreach (SchemaModuleBlock ModuleBlock in TopTable.ModuleBlocks)
            {
                foreach (KeyValuePair<string, List<string>> Pair in ModuleBlock.DetailOrder)
                {
                    SchemaTable ParentTable = Script.FindTable(Pair.Key);
                    if (ParentTable == null || (!ParentTable.Name.IsSameText(TopTable.Name) && !IsDetailOf(ParentTable, TopTable, Script)))
                    {
                        AddError(Result, "DETAIL_ORDER_PARENT_NOT_FOUND", "DetailOrder parent table not found in module: " + ModuleBlock.ModuleName + " -> " + Pair.Key);
                        continue;
                    }

                    HashSet<string> DirectChildren = Script.OutputTables
                        .Where(Table => !Table.IsOneToOne && Table.MasterName.IsSameText(ParentTable.Name))
                        .Select(Table => Table.Name)
                        .ToHashSet(StringComparer.OrdinalIgnoreCase);
                    HashSet<string> Names = new(StringComparer.OrdinalIgnoreCase);
                    foreach (string DetailName in Pair.Value)
                    {
                        if (!Names.Add(DetailName))
                            AddError(Result, "DETAIL_ORDER_DUPLICATE", "Duplicate DetailOrder child: " + ModuleBlock.ModuleName + " -> " + Pair.Key + "=" + DetailName);
                        else if (!DirectChildren.Contains(DetailName))
                            AddError(Result, "DETAIL_ORDER_CHILD_NOT_FOUND", "Direct child detail table not found: " + ModuleBlock.ModuleName + " -> " + Pair.Key + "=" + DetailName);
                    }
                }
            }
        }
    }
    /// <summary>
    /// Validates module filter fields against the generated list SELECT columns.
    /// </summary>
    static void ValidateFilterFields(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable TopTable in Script.TopTables)
        {
            foreach (SchemaModuleBlock ModuleBlock in TopTable.ModuleBlocks.Where(Item => Item.FilterFields.Count > 0))
            {
                SelectBuildResult SelectResult = BuildListSelectSql(Script, TopTable, ModuleBlock);
                HashSet<string> SelectFieldNames = SelectResult.SelectFields
                    .Select(Item => Item.Alias)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                foreach (string FieldName in ModuleBlock.FilterFields)
                {
                    if (!SelectFieldNames.Contains(FieldName))
                        AddError(Result, "FILTER_FIELD_NOT_FOUND", "Filter field not found in module list SELECT: " + ModuleBlock.ModuleName + " -> " + FieldName);
                }
            }
        }
    }
    /// <summary>
    /// Returns true when a table is a descendant detail of a top table.
    /// </summary>
    static bool IsDetailOf(SchemaTable Table, SchemaTable TopTable, SchemaScript Script)
    {
        SchemaTable Current = Table;
        while (Current != null && !string.IsNullOrWhiteSpace(Current.MasterName))
        {
            if (Current.MasterName.IsSameText(TopTable.Name))
                return true;
            Current = Script.FindTable(Current.MasterName);
        }
        return false;
    }
    /// <summary>
    /// Collects discovered code provider patterns and validates duplicate definitions.
    /// </summary>
    static void CollectCodeProviderPatterns(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaModuleBlock ModuleBlock in Table.ModuleBlocks.Where(x => x.CodeProvider != null))
                AddCodeProviderPatterns(Result, GetModuleCodeProviderName(ModuleBlock), ModuleBlock.CodeProvider);

            if (!TableUsesFieldCodeProvider(Table))
                continue;

            foreach (SchemaField Field in Table.Fields)
            {
                if (Field.MetadataKind != FieldMetadataKind.Code)
                    continue;

                string CodeProviderName = GetCodeProviderName(Table, Field);
                string Pattern = Field.CodeProviderPattern;
                AddCodeProviderPatterns(Result, CodeProviderName, Pattern, Field.IsDraftCodeProvider);
            }
        }
    }
    /// <summary>
    /// Adds code provider patterns.
    /// </summary>
    static void AddCodeProviderPatterns(SchemaParserResult Result, string CodeProviderName, CodeProviderMetadata CodeProvider)
    {
        AddCodeProviderPatterns(Result, CodeProviderName, CodeProvider.Pattern, CodeProvider.IsDraft);
    }
    /// <summary>
    /// Adds code provider patterns.
    /// </summary>
    static void AddCodeProviderPatterns(SchemaParserResult Result, string CodeProviderName, string Pattern, bool IsDraft)
    {
        AddCodeProviderPattern(Result, CodeProviderName, Pattern);
        if (IsDraft)
            AddCodeProviderPattern(Result, GetDraftCodeProviderName(CodeProviderName), GetDraftCodeProviderPattern(Pattern));
    }
    /// <summary>
    /// Adds or validates a code provider pattern.
    /// </summary>
    static void AddCodeProviderPattern(SchemaParserResult Result, string CodeProviderName, string Pattern)
    {
        if (Result.CodeProviderPatterns.TryGetValue(CodeProviderName, out string ExistingPattern))
        {
            if (!ExistingPattern.IsSameText(Pattern))
                AddError(Result, "CODE_PROVIDER_PATTERN_CONFLICT", "Code provider has conflicting patterns: " + CodeProviderName);
            return;
        }

        Result.CodeProviderPatterns[CodeProviderName] = Pattern;
    }
    /// <summary>
    /// Validates lookup fields.
    /// </summary>
    static void ValidateLookupFields(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                if (!IsLookupField(Script, Field) || Field.MetadataKind == FieldMetadataKind.Enum)
                    continue;
                if (Field.ForeignKey == null && string.IsNullOrWhiteSpace(Field.LookupTableName) && string.IsNullOrWhiteSpace(Field.LookupEnumTypeName) && string.IsNullOrWhiteSpace(Field.LookupClassName))
                    AddError(Result, "LOOKUP_NO_REFERENCE", "Lookup field has no foreign key reference: " + Table.Name + "." + Field.Name);
            }
        }
    }
    /// <summary>
    /// Validates enum fields.
    /// </summary>
    static void ValidateEnumFields(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                if (Field.MetadataKind != FieldMetadataKind.Enum)
                    continue;

                string EnumName = GetEnumName(Script, Field);

                if (EnumName.IsSameText(Field.Name))
                {
                    AddError(Result, "ENUM_FIELD_INVALID_NAME", "Enum field name must end with Id: " + Table.Name + "." + Field.Name);
                    continue;
                }

                Type EnumType = TypeStore.Find(EnumName);
                if (EnumType == null)
                {
                    AddError(Result, "ENUM_TYPE_NOT_FOUND", "Enum type not found in TypeStore: " + Table.Name + "." + Field.Name + " -> " + EnumName);
                    continue;
                }

                if (!EnumType.IsEnum)
                    AddError(Result, "ENUM_TYPE_INVALID", "TypeStore type is not an enum: " + Table.Name + "." + Field.Name + " -> " + EnumName);
            }
        }
    }
    /// <summary>
    /// Validates locator fields.
    /// </summary>
    static void ValidateLocatorFields(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
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
        var Items = GetModuleRegistrations(Script)
            .Where(x => !string.IsNullOrWhiteSpace(x.Module.ModuleName))
            .GroupBy(x => x.Module.ModuleName, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in Items)
            AddError(Result, "DUPLICATE_MODULE", $"Duplicate module name: {Item.Key}");
    }
    /// <summary>
    /// Validates table header metadata.
    /// </summary>
    static void ValidateTableHeaders(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
        {
            bool HasModule = Table.ModuleBlocks.Count > 0;
            bool HasMaster = !string.IsNullOrWhiteSpace(Table.MasterName);

            if (string.IsNullOrWhiteSpace(Table.Name))
                AddError(Result, "TABLE_NO_NAME", "Schema table has no name.");
            foreach (SchemaModuleBlock ModuleBlock in Table.ModuleBlocks)
            {
                if (string.IsNullOrWhiteSpace(ModuleBlock.ModuleName))
                    AddError(Result, "TOP_TABLE_NO_MODULE", "Top table has empty Module: " + Table.Name);
                if (string.IsNullOrWhiteSpace(ModuleBlock.GroupName))
                    AddError(Result, "TOP_TABLE_NO_GROUP", "Top table module has no Group: " + Table.Name + " -> " + ModuleBlock.ModuleName);
            }
            if (!HasModule && !HasMaster)
                AddError(Result, "DETAIL_TABLE_NO_MASTER", "Detail table has no Master: " + Table.Name);
            if (HasMaster && Script.FindTable(Table.MasterName) == null)
                AddError(Result, "MASTER_TABLE_NOT_FOUND", "Master table not found: " + Table.Name + " -> " + Table.MasterName);
        }
    }
    /// <summary>
    /// Validates foreign key declarations.
    /// </summary>
    static void ValidateForeignKeys(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaForeignKey ForeignKey in Table.ForeignKeys)
            {
                SchemaField Field = Table.FindField(ForeignKey.FieldName);
                if (Field == null)
                {
                    AddError(Result, "FOREIGN_KEY_FIELD_NOT_FOUND", "Foreign key field not found: " + Table.Name + "." + ForeignKey.FieldName);
                    continue;
                }

                SchemaTable ReferenceTable = Script.FindTable(ForeignKey.ReferenceTable);
                if (ReferenceTable == null)
                {
                    AddError(Result, "FOREIGN_KEY_TABLE_NOT_FOUND", "Foreign key reference table not found: " + Table.Name + "." + ForeignKey.FieldName + " -> " + ForeignKey.ReferenceTable);
                    continue;
                }

                if (ReferenceTable.FindField(ForeignKey.ReferenceField) == null)
                    AddError(Result, "FOREIGN_KEY_REFERENCE_FIELD_NOT_FOUND", "Foreign key reference field not found: " + Table.Name + "." + ForeignKey.FieldName + " -> " + ForeignKey.ReferenceTable + "." + ForeignKey.ReferenceField);
            }
        }
    }
    /// <summary>
    /// Validates circular foreign key references.
    /// </summary>
    static void ValidateCircularReferences(SchemaParserResult Result, SchemaScript Script)
    {
        Dictionary<string, VisitState> States = new(StringComparer.OrdinalIgnoreCase);
        List<SchemaTable> Stack = [];

        foreach (SchemaTable Table in Script.OutputTables)
            VisitCircularReference(Result, Script, Table, States, Stack);
    }
    /// <summary>
    /// Visits a table for circular reference validation.
    /// </summary>
    static void VisitCircularReference(SchemaParserResult Result, SchemaScript Script, SchemaTable Table, Dictionary<string, VisitState> States, List<SchemaTable> Stack)
    {
        if (States.ContainsKey(Table.Name))
            return;

        States[Table.Name] = VisitState.Visiting;
        Stack.Add(Table);

        foreach (SchemaForeignKey ForeignKey in Table.ForeignKeys)
        {
            if (ForeignKey.ReferenceTable.IsSameText(Table.Name))
                continue;

            SchemaTable ReferenceTable = Script.FindTable(ForeignKey.ReferenceTable);
            if (ReferenceTable == null)
                continue;

            if (States.TryGetValue(ReferenceTable.Name, out VisitState ReferenceState) && ReferenceState == VisitState.Visiting)
            {
                int Index = Stack.FindIndex(x => x.Name.IsSameText(ReferenceTable.Name));
                List<string> Cycle = Stack.Skip(Index).Select(x => x.Name).ToList();
                Cycle.Add(ReferenceTable.Name);
                AddError(Result, "CIRCULAR_SCHEMA_REFERENCE", "Circular schema reference detected: " + string.Join(" -> ", Cycle));
                continue;
            }

            VisitCircularReference(Result, Script, ReferenceTable, States, Stack);
        }

        Stack.RemoveAt(Stack.Count - 1);
        States[Table.Name] = VisitState.Done;
    }
    /// <summary>
    /// Validates duplicate creation orders.
    /// </summary>
    static void ValidateDuplicateCreationOrders(SchemaParserResult Result, SchemaScript Script)
    {
        var Items = Script.OutputTables
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
        var TableMethodNames = Script.OutputTables
            .GroupBy(x => "RegisterTable_" + SafeIdentifier(x.Name), StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in TableMethodNames)
        {
            string TableNames = string.Join(", ", Item.Select(x => x.Name));
            AddError(Result, "DUPLICATE_TABLE_METHOD", $"Duplicate generated table method {Item.Key}: {TableNames}");
        }

        var ModuleMethodNames = GetModuleRegistrations(Script)
            .GroupBy(x => "RegisterModule_" + SafeIdentifier(x.Module.ModuleName), StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() > 1)
            .ToList();

        foreach (var Item in ModuleMethodNames)
        {
            string ModuleNames = string.Join(", ", Item.Select(x => x.Module.ModuleName));
            AddError(Result, "DUPLICATE_MODULE_METHOD", $"Duplicate generated module method {Item.Key}: {ModuleNames}");
        }
    }
    /// <summary>
    /// Validates suspicious unique constraints.
    /// </summary>
    static void ValidateSuspiciousUniqueConstraints(SchemaParserResult Result, SchemaScript Script)
    {
        foreach (SchemaTable Table in Script.OutputTables)
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
    /// Returns a SQL string literal.
    /// </summary>
    static string SqlString(string Text)
    {
        return "'" + (Text ?? string.Empty).Replace("'", "''") + "'";
    }
    /// <summary>
    /// Builds a display CASE expression for an enum field.
    /// </summary>
    static string BuildEnumCaseExpression(SchemaTable Table, SchemaField Field, string EnumName)
    {
        Type EnumType = TypeStore.Find(EnumName);
        if (EnumType == null || !EnumType.IsEnum)
            return string.Empty;

        StringBuilder SB = new();
        SB.Append("   case").AppendLine();

        foreach (object Value in Enum.GetValues(EnumType))
        {
            int Number = Convert.ToInt32(Value, CultureInfo.InvariantCulture);
            string Name = Enum.GetName(EnumType, Value);
            SB.Append("      when ")
                .Append(Table.Name)
                .Append(".")
                .Append(Field.Name)
                .Append(" = ")
                .Append(Number.ToString(CultureInfo.InvariantCulture))
                .Append(" then ")
                .Append(SqlString(Name))
                .AppendLine();
        }

        SB.Append("      else ''").AppendLine();
        SB.Append("   end as ").Append(EnumName);
        return SB.ToString();
    }
    
    /// <summary>
    /// Builds ordered schema SQL.
    /// </summary>
    static string BuildOrderedSchemaSql(SchemaScript Script)
    {
        StringBuilder SB = new();

        foreach (SchemaTable Table in Script.OutputTables.OrderBy(x => x.CreationOrder))
        {
            if (SB.Length > 0)
                SB.AppendLine().AppendLine();
            SB.AppendLine(BuildOrderedTableSql(Table));
        }

        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds table SQL text with generated creation order metadata.
    /// </summary>
    static string BuildOrderedTableSql(SchemaTable Table)
    {
        string HeaderText = BuildOrderedHeaderText(Table);
        return HeaderText.Trim() + Environment.NewLine + Table.CreateSqlText.Trim();
    }
    /// <summary>
    /// Builds table header text with generated creation order metadata.
    /// </summary>
    static string BuildOrderedHeaderText(SchemaTable Table)
    {
        string HeaderText = Regex.Replace(
            Table.HeaderText,
            @"^\s*CreationOrder\s*:\s*.*(?:\r?\n)?",
            string.Empty,
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        int InsertIndex = HeaderText.LastIndexOf("----------------------------------------------------*/", StringComparison.Ordinal);
        if (InsertIndex < 0)
            return HeaderText.TrimEnd() + Environment.NewLine + "CreationOrder: " + Table.CreationOrder;

        string Prefix = HeaderText.Substring(0, InsertIndex).TrimEnd();
        string Suffix = HeaderText.Substring(InsertIndex);
        return Prefix + Environment.NewLine + "CreationOrder: " + Table.CreationOrder + Environment.NewLine + Suffix;
    }
    /// <summary>
    /// Writes generated output files.
    /// </summary>
    static void WriteOutputFiles(SchemaParserResult Result, int SchemaVersion, string OutputFolderPath)
    {
        if (!Directory.Exists(OutputFolderPath))
            Directory.CreateDirectory(OutputFolderPath);

        WriteOutputFile(OutputFolderPath, "SchemaVersion" + SchemaVersion + ".cs", Result.CreateTablesSourceCode);
        WriteOutputFile(OutputFolderPath, "RegistryVersion" + SchemaVersion + ".cs", Result.RegistryVersionSourceCode);
        WriteOutputFile(OutputFolderPath, "RegistryVersion" + SchemaVersion + ".Modules.cs", Result.ModuleDefsSourceCode);
        WriteOutputFile(OutputFolderPath, "RegistryVersion" + SchemaVersion + ".Forms.cs", Result.FormDefsSourceCode);
        WriteOutputFile(OutputFolderPath, "RegistryVersion" + SchemaVersion + ".Lookups.cs", Result.LookupDefsSourceCode);
        WriteOutputFile(OutputFolderPath, "RegistryVersion" + SchemaVersion + ".Locators.cs", Result.LocatorDefsSourceCode);
        WriteOutputFile(OutputFolderPath, "RegistryVersion" + SchemaVersion + ".CodeProviders.cs", Result.CodeProviderDefsSourceCode);
        WriteOutputFile(OutputFolderPath, "Schema.sql", Result.SchemaSql);
    }
    /// <summary>
    /// Writes a generated output file.
    /// </summary>
    static void WriteOutputFile(string OutputFolderPath, string FileName, string SourceCode)
    {
        string FilePath = Path.Combine(OutputFolderPath, FileName);
        SourceCode = AddGeneratedHeader(FileName, SourceCode);
        File.WriteAllText(FilePath, SourceCode ?? string.Empty);
    }
    /// <summary>
    /// Adds the generated file header.
    /// </summary>
    static string AddGeneratedHeader(string FileName, string SourceCode)
    {
        SourceCode ??= string.Empty;
        string Header = FileName.EndsWithText(".sql") ? GetSqlGeneratedHeader() : GetCSharpGeneratedHeader();
        return Header + SourceCode.TrimStart();
    }
    /// <summary>
    /// Returns the generated C# file header.
    /// </summary>
    static string GetCSharpGeneratedHeader()
    {
        return @"/*
 * <auto-generated>
 * This file was generated by Tripous RegBuilder.
 * Do not edit this file manually.
 * </auto-generated>
 *
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

";
    }
    /// <summary>
    /// Returns the generated SQL file header.
    /// </summary>
    static string GetSqlGeneratedHeader() => GetCSharpGeneratedHeader();
    /// <summary>
    /// Appends a namespace declaration.
    /// </summary>
    static void AppendNamespace(StringBuilder SB, string NamespaceName)
    {
        if (string.IsNullOrWhiteSpace(NamespaceName))
            return;

        SB.AppendLine("namespace " + NamespaceName + ";");
        SB.AppendLine();
    }
    /// <summary>
    /// Builds source code for the registry version root partial class.
    /// </summary>
    static string BuildRegistryVersionSourceCode(int SchemaVersion, string NamespaceName)
    {
        StringBuilder SB = new();
        AppendNamespace(SB, NamespaceName);
        SB.AppendLine("public partial class RegistryVersion" + SchemaVersion + ": RegistryVersion");
        SB.AppendLine("{");
        SB.AppendLine("    // ● construction");
        SB.AppendLine("    public RegistryVersion" + SchemaVersion + "()");
        SB.AppendLine("    {");
        SB.AppendLine("    }");
        SB.AppendLine();
        SB.AppendLine("    // ● properties");
        SB.AppendLine("    public override int VersionNumber { get; } = " + SchemaVersion + ";");
        SB.AppendLine("}");
        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code for schema version registration.
    /// </summary>
    static string BuildCreateTablesSourceCode(SchemaScript Script, int SchemaVersion)
        => BuildCreateTablesSourceCode(Script, new SchemaParserResult(), SchemaVersion, string.Empty);
    /// <summary>
    /// Builds source code for schema version registration.
    /// </summary>
    static string BuildCreateTablesSourceCode(SchemaScript Script, SchemaParserResult ParserResult, int SchemaVersion, string NamespaceName)
    {
        StringBuilder SB = new();
        List<SchemaTable> Tables = Script.OutputTables.OrderBy(x => x.CreationOrder).ToList();

        AppendNamespace(SB, NamespaceName);
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
        if (ParserResult.CodeProviderPatterns.Count > 0)
        {
            SB.AppendLine("    void AddCodeProviderPatternStatements()");
            SB.AppendLine("    {");
            bool IsFirst = true;
            foreach (var Pair in ParserResult.CodeProviderPatterns.OrderBy(item => item.Key))
            {
                BuildAddCodeProviderPatternStatement(SB, Pair.Key, Pair.Value, IsFirst);
                IsFirst = false;
            }
            SB.AppendLine("    }");
        }
        SB.AppendLine();
        SB.AppendLine("    // ● protected");
        SB.AppendLine("    protected override void RegisterInternal()");
        SB.AppendLine("    {");
        foreach (SchemaTable Table in Tables)
            SB.AppendLine("        RegisterTable_" + SafeIdentifier(Table.Name) + "();");
        if (ParserResult.CodeProviderPatterns.Count > 0)
            SB.AppendLine("        AddCodeProviderPatternStatements();");
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
    /// Builds a schema-after statement for a code provider pattern.
    /// </summary>
    static void BuildAddCodeProviderPatternStatement(StringBuilder SB, string CodeProviderName, string Pattern, bool DeclareVariable)
    {
        string Code = EscapeVerbatim(EscapeSqlString(CodeProviderName));
        string SqlPattern = EscapeVerbatim(EscapeSqlString(Pattern));

        string Assignment = DeclareVariable ? "string SqlText" : "SqlText";
        SB.AppendLine("        " + Assignment + " = $@\"");
        SB.AppendLine("INSERT INTO {DbConfig.SysNumberSeriesTableName}");
        SB.AppendLine("(Id, Code, Name, Pattern, ResetPeriodId, NextNumber, LastResetValue, IsActive)");
        SB.AppendLine("VALUES");
        SB.AppendLine("('{MemTable.GenId()}', '" + Code + "', '" + Code + "', '" + SqlPattern + "', 0, 1, NULL, 1)");
        SB.AppendLine("\";");
        SB.AppendLine("        Version.AddStatementAfter(SqlText);");
    }
    /// <summary>
    /// Returns module registrations.
    /// </summary>
    static List<SchemaModuleRegistration> GetModuleRegistrations(SchemaScript Script)
    {
        List<SchemaModuleRegistration> Result = [];

        foreach (SchemaTable Table in Script.TopTables)
        {
            foreach (SchemaModuleBlock Module in Table.ModuleBlocks)
            {
                SchemaModuleRegistration Registration = new();
                Registration.Table = Table;
                Registration.Module = Module;
                Result.Add(Registration);
            }
        }

        return Result;
    }
    /// <summary>
    /// Builds source code for module registration.
    /// </summary>
    static string BuildModuleDefsSourceCode(SchemaScript Script)
        => BuildModuleDefsSourceCode(Script, 0, string.Empty);
    /// <summary>
    /// Builds source code for module registration.
    /// </summary>
    static string BuildModuleDefsSourceCode(SchemaScript Script, int SchemaVersion, string NamespaceName)
    {
        StringBuilder SB = new();
        List<SchemaModuleRegistration> Registrations = GetModuleRegistrations(Script)
            .OrderBy(x => x.Module.ModuleName)
            .ToList();

        if (SchemaVersion <= 0)
            SB.AppendLine("static internal partial class Registry");
        else
        {
            AppendNamespace(SB, NamespaceName);
            SB.AppendLine("public partial class RegistryVersion" + SchemaVersion + ": RegistryVersion");
        }
        SB.AppendLine("{");
        SB.AppendLine("    // ● private");
        if (SchemaVersion <= 0)
        {
            BuildRegisterCodeProvidersMethod(SB, Script);
            BuildRegisterLookupSourcesMethod(SB, Script);
            BuildRegisterLocatorsMethod(SB, Script);
        }
        foreach (SchemaModuleRegistration Registration in Registrations)
            BuildRegisterModuleMethod(SB, Script, Registration.Table, Registration.Module);
        SB.AppendLine();
        SB.AppendLine("    // ● public");
        SB.AppendLine(SchemaVersion <= 0 ? "    static public void RegisterModules()" : "    public override void RegisterModules()");
        SB.AppendLine("    {");
        if (SchemaVersion <= 0)
        {
            SB.AppendLine("        RegisterCodeProviders_FromModules();");
            SB.AppendLine("        RegisterLookups_FromModules();");
            SB.AppendLine("        RegisterLocators_FromModules();");
        }
        foreach (SchemaModuleRegistration Registration in Registrations)
            SB.AppendLine("        RegisterModule_" + SafeIdentifier(Registration.Module.ModuleName) + "();");
        SB.AppendLine("    }");
        SB.AppendLine("}");

        return SB.ToString().TrimEnd();
    }

    /// <summary>
    /// Builds source code for form registration.
    /// </summary>
    static string BuildFormDefsSourceCode(SchemaScript Script)
        => BuildFormDefsSourceCode(Script, 0, string.Empty);
    /// <summary>
    /// Builds source code for form registration.
    /// </summary>
    static string BuildFormDefsSourceCode(SchemaScript Script, int SchemaVersion, string NamespaceName)
    {
        StringBuilder SB = new();
        List<SchemaModuleRegistration> Registrations = GetModuleRegistrations(Script)
            .OrderBy(x => x.Module.ModuleName)
            .ToList();

        if (SchemaVersion <= 0)
            SB.AppendLine("static internal partial class Registry");
        else
        {
            AppendNamespace(SB, NamespaceName);
            SB.AppendLine("public partial class RegistryVersion" + SchemaVersion + ": RegistryVersion");
        }
        SB.AppendLine("{");
        SB.AppendLine("    // ● public");
        SB.AppendLine(SchemaVersion <= 0 ? "    static public void RegisterForms()" : "    public override void RegisterForms()");
        SB.AppendLine("    {");

        foreach (SchemaModuleRegistration Registration in Registrations)
        {
            SB.AppendLine("        " + BuildAddFormSource(Registration.Table, Registration.Module));
        }

        SB.AppendLine("    }");
        SB.AppendLine("}");

        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code for lookup registration.
    /// </summary>
    static string BuildLookupDefsSourceCode(SchemaScript Script, int SchemaVersion, string NamespaceName)
    {
        StringBuilder SB = new();
        AppendNamespace(SB, NamespaceName);
        SB.AppendLine("public partial class RegistryVersion" + SchemaVersion + ": RegistryVersion");
        SB.AppendLine("{");
        SB.AppendLine("    // ● public");
        BuildRegisterLookupSourcesMethod(SB, Script, "public override void RegisterLookups");
        SB.AppendLine("}");
        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code for locator registration.
    /// </summary>
    static string BuildLocatorDefsSourceCode(SchemaScript Script, int SchemaVersion, string NamespaceName)
    {
        StringBuilder SB = new();
        AppendNamespace(SB, NamespaceName);
        SB.AppendLine("public partial class RegistryVersion" + SchemaVersion + ": RegistryVersion");
        SB.AppendLine("{");
        SB.AppendLine("    // ● public");
        BuildRegisterLocatorsMethod(SB, Script, "public override void RegisterLocators");
        SB.AppendLine("}");
        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code for code provider registration.
    /// </summary>
    static string BuildCodeProviderDefsSourceCode(SchemaScript Script, SchemaParserResult ParserResult, int SchemaVersion, string NamespaceName)
    {
        StringBuilder SB = new();
        AppendNamespace(SB, NamespaceName);
        SB.AppendLine("public partial class RegistryVersion" + SchemaVersion + ": RegistryVersion");
        SB.AppendLine("{");
        SB.AppendLine("    // ● public");
        BuildRegisterCodeProvidersMethod(SB, Script, "public override void RegisterCodeProviders");
        SB.AppendLine("}");
        return SB.ToString().TrimEnd();
    }
    /// <summary>
    /// Builds source code that adds a form definition.
    /// </summary>
    static string BuildAddFormSource(SchemaTable TopTable, SchemaModuleBlock ModuleBlock)
    {
        List<string> Args = [];
        Args.Add("\"" + EscapeString(ModuleBlock.FormName) + "\"");
        Args.Add("TitleKey: \"" + EscapeString(ModuleBlock.FormName) + "\"");
        Args.Add("Module: \"" + EscapeString(ModuleBlock.ModuleName) + "\"");

        if (!string.IsNullOrWhiteSpace(ModuleBlock.FormClassName))
            Args.Add("ClassName: \"" + EscapeString(ModuleBlock.FormClassName) + "\"");
        if (!string.IsNullOrWhiteSpace(ModuleBlock.GroupName))
            Args.Add("Group: \"" + EscapeString(ModuleBlock.GroupName) + "\"");
        if (!string.IsNullOrWhiteSpace(ModuleBlock.ItemPageClassName))
            Args.Add("ItemClassName: \"" + EscapeString(ModuleBlock.ItemPageClassName) + "\"");
        if (TopTable.IsReadOnly)
            Args.Add("IsReadOnly: true");
        if (ModuleBlock.SecurityLevel != UserLevel.None)
            Args.Add("SecurityLevel: UserLevel." + ModuleBlock.SecurityLevel);

        return "DesktopRegistry.AddOrUpdateForm(" + string.Join(", ", Args) + ");";
    }

    // ● private - module source
    /// <summary>
    /// Collects lookup source names and table names.
    /// </summary>
    static Dictionary<string, LookupSourceInfo> CollectLookupSources(SchemaScript Script)
    {
        Dictionary<string, LookupSourceInfo> Result = new(StringComparer.OrdinalIgnoreCase);

        foreach (SchemaTable Table in Script.OutputTables.Where(x => x.IsLookup))
        {
            LookupSourceInfo Info = new();
            Info.Name = Table.Name;
            Info.TableName = Table.Name;
            Info.FormName = Table.FormName;
            Result[Info.Name] = Info;
        }

        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                if (Field.MetadataKind != FieldMetadataKind.Lookup)
                    continue;

                string LookupSourceName = GetLookupSourceName(Script, Field);
                string LookupTableName = !string.IsNullOrWhiteSpace(Field.LookupTableName) ? Field.LookupTableName : Field.ForeignKey != null ? Field.ForeignKey.ReferenceTable : LookupSourceName;
                SchemaTable LookupTable = Script.FindTable(LookupTableName);

                if (!string.IsNullOrWhiteSpace(LookupSourceName) && (!string.IsNullOrWhiteSpace(LookupTableName) || !string.IsNullOrWhiteSpace(Field.LookupEnumTypeName) || !string.IsNullOrWhiteSpace(Field.LookupClassName)))
                {
                    LookupSourceInfo Info = new();
                    Info.Name = LookupSourceName;
                    Info.TableName = LookupTableName;
                    Info.EnumTypeName = Field.LookupEnumTypeName;
                    Info.ClassName = Field.LookupClassName;
                    Info.FormName = LookupTable != null ? LookupTable.FormName : string.Empty;
                    Result[Info.Name] = Info;
                }
            }
        }

        return Result;
    }
    /// <summary>
    /// Builds lookup source registration method.
    /// </summary>
    static void BuildRegisterLookupSourcesMethod(StringBuilder SB, SchemaScript Script)
        => BuildRegisterLookupSourcesMethod(SB, Script, "static void RegisterLookups_FromModules");
    /// <summary>
    /// Builds lookup source registration method.
    /// </summary>
    static void BuildRegisterLookupSourcesMethod(StringBuilder SB, SchemaScript Script, string MethodDeclaration)
    {
        Dictionary<string, LookupSourceInfo> LookupSources = CollectLookupSources(Script);

        SB.AppendLine("    " + MethodDeclaration + "()");
        SB.AppendLine("    {");

        foreach (LookupSourceInfo LookupSource in LookupSources.Values.OrderBy(x => x.Name))
        {
            if (!string.IsNullOrWhiteSpace(LookupSource.ClassName))
                SB.AppendLine("        DataRegistry.AddOrUpdateLookupWithClassName(\"" + EscapeString(LookupSource.Name) + "\", \"" + EscapeString(LookupSource.ClassName) + "\"" + BuildOptionalFormNameArgument(LookupSource.FormName) + ");");
            else if (!string.IsNullOrWhiteSpace(LookupSource.EnumTypeName))
                SB.AppendLine("        DataRegistry.AddOrUpdateLookupSource(\"" + EscapeString(LookupSource.Name) + "\", TypeStore.Get(\"" + EscapeString(LookupSource.EnumTypeName) + "\"));");
            else
                SB.AppendLine("        DataRegistry.AddOrUpdateLookupWithTableName(\"" + EscapeString(LookupSource.Name) + "\", \"" + EscapeString(LookupSource.TableName) + "\"" + BuildOptionalFormNameArgument(LookupSource.FormName) + ");");
        }

        SB.AppendLine("    }");

    }
    /// <summary>
    /// Builds code provider registration method.
    /// </summary>
    static void BuildRegisterCodeProvidersMethod(StringBuilder SB, SchemaScript Script)
        => BuildRegisterCodeProvidersMethod(SB, Script, "static void RegisterCodeProviders_FromModules");
    /// <summary>
    /// Builds code provider registration method.
    /// </summary>
    static void BuildRegisterCodeProvidersMethod(StringBuilder SB, SchemaScript Script, string MethodDeclaration)
    {
        List<string> CodeProviderNames = CollectCodeProviderNames(Script);

        SB.AppendLine("    " + MethodDeclaration + "()");
        SB.AppendLine("    {");

        foreach (string CodeProviderName in CodeProviderNames)
            SB.AppendLine("        DataRegistry.AddOrUpdateCodeProvider(\"" + EscapeString(CodeProviderName) + "\");");

        SB.AppendLine("    }");
    }
    /// <summary>
    /// Collects code provider names.
    /// </summary>
    static List<string> CollectCodeProviderNames(SchemaScript Script)
    {
        List<string> Result = [];

        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaModuleBlock ModuleBlock in Table.ModuleBlocks.Where(x => x.CodeProvider != null))
                Result.AddRange(GetModuleCodeProviderNames(ModuleBlock));

            if (TableUsesFieldCodeProvider(Table))
            {
                foreach (SchemaField Field in Table.Fields.Where(x => x.MetadataKind == FieldMetadataKind.Code))
                    Result.AddRange(GetCodeProviderNames(Table, Field));
            }
        }

        return Result
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x)
            .ToList();
    }


    /// <summary>
    /// Builds the method that registers locator definitions.
    /// </summary>
    static void BuildRegisterLocatorsMethod(StringBuilder SB, SchemaScript Script)
        => BuildRegisterLocatorsMethod(SB, Script, "static void RegisterLocators_FromModules");
    /// <summary>
    /// Builds the method that registers locator definitions.
    /// </summary>
    static void BuildRegisterLocatorsMethod(StringBuilder SB, SchemaScript Script, string MethodDeclaration)
    {
        Dictionary<string, LocatorInfo> Locators = CollectLocators(Script);

        SB.AppendLine("    " + MethodDeclaration + "()");
        SB.AppendLine("    {");

        foreach (LocatorInfo Locator in Locators.Values.OrderBy(x => x.Name))
        {
            string Source = "DataRegistry.AddOrUpdateLocator(\"" + EscapeString(Locator.Name) + "\", \"" + EscapeString(Locator.TableName) + "\", \"" + EscapeString(Locator.KeyField) + "\"" + BuildOptionalClassNameArgument(Locator.ClassName) + BuildOptionalFormNameArgument(Locator.FormName) + ")";
            SB.AppendLine("        " + Source + ";");
        }

        SB.AppendLine("    }");
    }
    /// <summary>
    /// Collects locator definitions.
    /// </summary>
    static Dictionary<string, LocatorInfo> CollectLocators(SchemaScript Script)
    {
        Dictionary<string, LocatorInfo> Result = new(StringComparer.OrdinalIgnoreCase);

        foreach (SchemaTable Table in Script.OutputTables)
        {
            foreach (SchemaField Field in Table.Fields)
            {
                if (!IsLocatorField(Field))
                    continue;

                LocatorInfo Locator = ResolveLocatorInfo(Script, Table, Field);
                if (Locator == null)
                    continue;

                if (!Result.TryGetValue(Locator.Name, out LocatorInfo Existing))
                    Result[Locator.Name] = Locator;
                else if (string.IsNullOrWhiteSpace(Existing.ClassName) && !string.IsNullOrWhiteSpace(Locator.ClassName))
                    Existing.ClassName = Locator.ClassName;
            }
        }

        return Result;
    }
    /// <summary>
    /// Builds source code that adds a module definition.
    /// </summary>
    static string BuildAddModuleSource(SchemaTable TopTable, SchemaModuleBlock ModuleBlock)
    {
        List<string> Args = [];
        Args.Add("\"" + EscapeString(ModuleBlock.ModuleName) + "\"");

        if (!string.IsNullOrWhiteSpace(ModuleBlock.ModuleClassName))
            Args.Add("ClassName: \"" + EscapeString(ModuleBlock.ModuleClassName) + "\"");

        Args.Add("ListSelectSql: SqlText");

        if (TopTable.IsSingleSelect)
            Args.Add("IsSingleSelect: true");
        if (ModuleBlock.SecurityLevel != UserLevel.None)
            Args.Add("SecurityLevel: UserLevel." + ModuleBlock.SecurityLevel);

        return "Module = DataRegistry.AddOrUpdateModule(" + string.Join(", ", Args) + ");";
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
    static void BuildRegisterModuleMethod(StringBuilder SB, SchemaScript Script, SchemaTable TopTable, SchemaModuleBlock ModuleBlock)
    {
        SelectBuildResult SelectResult = BuildListSelectSql(Script, TopTable, ModuleBlock);

        SB.AppendLine("    static void RegisterModule_" + SafeIdentifier(ModuleBlock.ModuleName) + "()");
        SB.AppendLine("    {");
        SB.AppendLine("        ModuleDef Module;");
        SB.AppendLine("        TableDef tblTop;");
        SB.AppendLine("        SelectDef SelectDef;");
        SB.AppendLine("        string SqlText;");

        SB.AppendLine("        SqlText = @\"");
        SB.AppendLine(EscapeVerbatim(SelectResult.SqlText));
        SB.AppendLine("\";");
        SB.AppendLine("        " + BuildAddModuleSource(TopTable, ModuleBlock));
        foreach (KeyValuePair<string, List<string>> Pair in ModuleBlock.DetailOrder)
        {
            string DetailOrder = string.Join(", ", Pair.Value.Select(Name => "\"" + EscapeString(Name) + "\""));
            SB.AppendLine("        Module.DetailOrder[\"" + EscapeString(Pair.Key) + "\"] = [" + DetailOrder + "];");
        }
        SB.AppendLine("        if (Module.Table.Fields.Count > 0)");
        SB.AppendLine("            return;");
        BuildModuleOptionAssignments(SB, TopTable);
        SB.AppendLine("        tblTop = Module.Table;");
        SB.AppendLine("        tblTop.Name = \"" + EscapeString(TopTable.Name) + "\";");
        SB.AppendLine("        tblTop.KeyField = \"" + EscapeString(TopTable.PrimaryKeyField.Name) + "\";");

        if (!TopTable.UiVisible)
            SB.AppendLine("        tblTop.IsUiVisible = false;");

        BuildTableFieldsSource(SB, Script, TopTable, "tblTop", "        ", ModuleBlock);
        if (TopTable.UseFilters)
            BuildFiltersSource(SB, SelectResult.FilterFields, ModuleBlock.FilterFields.Count == 0);

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
    static void BuildTableFieldsSource(StringBuilder SB, SchemaScript Script, SchemaTable Table, string TableVarName, string Indent, SchemaModuleBlock ModuleBlock = null)
    {
        if (Table.FieldGroups.Count > 0)
            SB.AppendLine(Indent + TableVarName + ".FieldGroups.AddRange([" + string.Join(", ", Table.FieldGroups.Select(x => "\"" + EscapeString(x) + "\"")) + "]);");

        foreach (SchemaField Field in Table.Fields)
            SB.AppendLine(Indent + BuildAddFieldSource(Script, Table, Field, TableVarName, ModuleBlock));

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
    static void BuildFiltersSource(StringBuilder SB, List<SelectField> FilterFields, bool SortFields)
    {
        if (FilterFields.Count == 0)
            return;

        if (SortFields)
        {
            FilterFields = FilterFields
                .OrderByDescending(x => x.Alias.IsSameText("Name"))
                .ThenBy(x => x.Alias)
                .ToList();
        }

        SB.AppendLine("        SelectDef = Module.SelectList[0];");
        foreach (SelectField Field in FilterFields)
            SB.AppendLine("        SelectDef.AddFilter(\"" + EscapeString(Field.Alias) + "\", FieldName: \"" + EscapeString(Field.Alias) + "\", FilterDataType: DataFieldType." + Field.DataType + ");");
    }

    static void BuildSelectColumnTypesSource(StringBuilder SB, SelectBuildResult SelectResult)
    {
        foreach (var Pair in SelectResult.ColumnTypes)
            SB.AppendLine($"        SelectDef.ColumnTypes[\"{EscapeString(Pair.Key)}\"] = DataColumnType.{Pair.Value};");
    }
    /// <summary>
    /// Builds source code that adds a field definition.
    /// </summary>
    static string BuildAddFieldSource(SchemaScript Script, SchemaTable Table, SchemaField Field, string TableVarName, SchemaModuleBlock ModuleBlock = null)
    {
        string NullSuffix = ".SetNullable(" + BoolLiteral(Field.IsNullable) + ")";
        string DefaultSuffix = !string.IsNullOrWhiteSpace(Field.DefaultValue) ? ".SetDefaultValue(\"" + EscapeString(Field.DefaultValue) + "\")" : "";
        string CodeProviderSuffix = Field.MetadataKind == FieldMetadataKind.Code ? ".SetCodeProviderName(\"" + EscapeString(GetFieldCodeProviderName(Table, Field, ModuleBlock)) + "\")" : "";
        string TitleKeySuffix = !string.IsNullOrWhiteSpace(Field.TitleKey) ? ".SetTitleKey(\"" + EscapeString(Field.TitleKey) + "\")" : "";
        string SnapshotSuffix = !string.IsNullOrWhiteSpace(Field.SnapshotOf) ? ".SetSnapshotOf(\"" + EscapeString(Field.SnapshotOf) + "\")" : "";
        string MemoSuffix = Field.IsMemo ? ".SetMemo()" : "";
        string LargeMemoSuffix = Field.IsLargeMemo ? ".SetLargeMemo()" : "";
        string GroupSuffix = !string.IsNullOrWhiteSpace(Field.GroupName) ? ".SetGroup(\"" + EscapeString(Field.GroupName) + "\")" : "";
        string MetadataSuffix = NullSuffix + DefaultSuffix + CodeProviderSuffix + TitleKeySuffix + SnapshotSuffix + MemoSuffix + LargeMemoSuffix + GroupSuffix;
        string Flags = BuildFlags(Field);

        if (Field.IsPrimaryKey)
            return TableVarName + ".AddId(\"" + EscapeString(Field.Name) + "\").SetNullable(false);";

        if (Field.MetadataKind == FieldMetadataKind.Enum)
        {
            string EnumName = GetEnumName(Script, Field);
            return TableVarName + ".AddEnumLookupId(\"" + EscapeString(Field.Name) + "\", \"" + EscapeString(EnumName) + "\", TypeStore.Get(\"" + EscapeString(EnumName) + "\"), Flags: " + Flags + ")" + MetadataSuffix + ";";
        }

        if (IsLookupField(Script, Field))
        {
            string LookupSource = GetLookupSourceName(Script, Field);
            if (Field.DataType == DataFieldType.Integer)
                return TableVarName + ".AddIntegerLookupId(\"" + EscapeString(Field.Name) + "\", \"" + EscapeString(LookupSource) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";
            return TableVarName + ".AddStringLookupId(\"" + EscapeString(Field.Name) + "\", \"" + EscapeString(LookupSource) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";
        }

        if (Field.IsLargeMemo)
            return TableVarName + ".AddTextBlob(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";

        switch (Field.DataType)
        {
            case DataFieldType.String:
                return TableVarName + ".AddString(\"" + EscapeString(Field.Name) + "\", MaxLength: " + Field.MaxLength + ", Flags: " + Flags + ")" + MetadataSuffix + ";";
            case DataFieldType.Integer:
                return TableVarName + ".AddInteger(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";
            case DataFieldType.Double:
                return TableVarName + ".AddDouble(\"" + EscapeString(Field.Name) + "\", Decimals: " + Field.Decimals + ", Flags: " + Flags + ")" + MetadataSuffix + ";";
            case DataFieldType.Decimal:
            case DataFieldType.Decimal_:
                return TableVarName + ".AddDecimal(\"" + EscapeString(Field.Name) + "\", Decimals: " + Field.Decimals + ", Flags: " + Flags + ")" + MetadataSuffix + ";";
            case DataFieldType.Date:
                return TableVarName + ".AddDate(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";
            case DataFieldType.DateTime:
                return TableVarName + ".AddDateTime(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";
            case DataFieldType.Boolean:
                return TableVarName + ".AddBoolean(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";
            case DataFieldType.Blob:
                return TableVarName + ".AddBlob(\"" + EscapeString(Field.Name) + "\", Flags: " + BuildFlags(Field, FieldFlagsText.None) + ")" + MetadataSuffix + ";";
            case DataFieldType.TextBlob:
                return TableVarName + ".AddTextBlob(\"" + EscapeString(Field.Name) + "\", Flags: " + Flags + ")" + MetadataSuffix + ";";
        }

        return TableVarName + ".AddString(\"" + EscapeString(Field.Name) + "\", MaxLength: " + Field.MaxLength + ", Flags: " + Flags + ")" + MetadataSuffix + ";";
    }

    // ● private - select source
    /// <summary>
    /// Builds list select SQL and filter field information.
    /// </summary>
    static SelectBuildResult BuildListSelectSql(SchemaScript Script, SchemaTable TopTable, SchemaModuleBlock ModuleBlock)
    {
        SelectBuildResult Result = new();
        List<string> SelectLines = [];
        List<string> JoinLines = [];
        HashSet<string> Aliases = new(StringComparer.OrdinalIgnoreCase);

        foreach (SchemaField Field in TopTable.Fields)
        {
            if (!Field.DataType.IsBlob() && !Field.FieldFlags.HasFlag(FieldFlags.Hidden))
            {
                SelectLines.Add("   " + TopTable.Name + "." + Field.Name);
                AddColumnType(Result, Field.Name, Field);
                Result.SelectFields.Add(new SelectField(Field.Name, Field.DataType));

                if (IsFilterableField(Field, Field.Name))
                    Result.FilterFields.Add(new SelectField(Field.Name, Field.DataType));

                if (Field.MetadataKind == FieldMetadataKind.Enum)
                {
                    string EnumName = GetEnumName(Script, Field);
                    string EnumCaseExpression = BuildEnumCaseExpression(TopTable, Field, EnumName);
                    if (!string.IsNullOrWhiteSpace(EnumCaseExpression))
                    {
                        SelectLines.Add(EnumCaseExpression);
                        Result.ColumnTypes[EnumName] = DataColumnType.Text;
                        Result.SelectFields.Add(new SelectField(EnumName, DataFieldType.String));
                        Result.FilterFields.Add(new SelectField(EnumName, DataFieldType.String));
                    }
                }
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
                Result.SelectFields.Add(new SelectField(DisplayAlias, JoinField.DataType));

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

        if (!string.IsNullOrWhiteSpace(ModuleBlock.ListWhere))
            SB.AppendLine("where " + ModuleBlock.ListWhere);

        Result.SqlText = SB.ToString().TrimEnd();

        Result.SelectFields = Result.SelectFields
            .GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .OrderBy(x => x.Alias)
            .ToList();

        Result.FilterFields = ModuleBlock.FilterFields.Count > 0
            ? ModuleBlock.FilterFields
                .Select(Name => Result.SelectFields.FirstOrDefault(Field => Field.Alias.IsSameText(Name)))
                .Where(Field => Field != null)
                .ToList()
            : Result.FilterFields
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
        int HeaderStart = FindHeaderStartIndex(Text, CreateTableIndex);
        int HeaderEnd = Text.LastIndexOf("*/", CreateTableIndex, StringComparison.Ordinal);

        if (HeaderStart < 0 || HeaderEnd < HeaderStart)
            throw new TripousDataException("Schema table header not found.");

        return Text.Substring(HeaderStart, HeaderEnd - HeaderStart + 2).Trim();
    }
    /// <summary>
    /// Returns the header start index before a CREATE TABLE statement.
    /// </summary>
    static int FindHeaderStartIndex(string Text, int CreateTableIndex)
    {
        return Text.LastIndexOf("/*", CreateTableIndex, StringComparison.Ordinal);
    }
    /// <summary>
    /// Returns a one-based line number.
    /// </summary>
    static int GetLineNumber(string Text, int Index)
    {
        if (Index < 0)
            return 0;

        int Result = 1;
        int MaxIndex = Math.Min(Index, Text.Length);
        for (int i = 0; i < MaxIndex; i++)
        {
            if (Text[i] == '\n')
                Result++;
        }

        return Result;
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
    /// Returns ordered header entries.
    /// </summary>
    static List<HeaderEntry> GetHeaderEntries(string HeaderText)
    {
        List<HeaderEntry> Result = [];
        string[] Lines = HeaderText.Split(["\r\n", "\n"], StringSplitOptions.None);

        foreach (string Line in Lines)
        {
            string Text = Line.Trim();
            if (string.IsNullOrWhiteSpace(Text))
                continue;
            if (Text.StartsWith("/*") || Text.StartsWith("*/") || Text.All(x => x == '-'))
                continue;

            Match Match = Regex.Match(Text, @"^(?<name>[A-Za-z][A-Za-z0-9]*)\s*(?::\s*(?<value>.*))?$");
            if (!Match.Success)
                continue;

            HeaderEntry Entry = new();
            Entry.Name = Match.Groups["name"].Value.Trim();
            Entry.Value = Match.Groups["value"].Success ? Match.Groups["value"].Value.Trim() : string.Empty;
            Result.Add(Entry);
        }

        return Result;
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
    /// Splits a comma separated header value.
    /// </summary>
    static List<string> SplitHeaderList(string Value)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return [];

        return Value
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .GroupBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .ToList();
    }
    /// <summary>
    /// Parses a single-token header value.
    /// </summary>
    static string ParseSingleHeaderToken(string Value, string HeaderName)
    {
        List<string> Parts = SplitHeaderTokens(Value);
        if (Parts.Count == 0)
            return string.Empty;
        if (Parts.Count > 1)
            throw new TripousDataException("Invalid " + HeaderName + " header syntax: \"" + Value + "\". Expected: " + HeaderName + ": " + HeaderName.ToUpperInvariant() + "_NAME");
        return Parts[0];
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
    /// Returns true when text is a valid identifier.
    /// </summary>
    static bool IsIdentifier(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text) || (!char.IsLetter(Text[0]) && Text[0] != '_'))
            return false;
        for (int Index = 1; Index < Text.Length; Index++)
        {
            if (!char.IsLetterOrDigit(Text[Index]) && Text[Index] != '_')
                return false;
        }
        return true;
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

        string[] Entries = MetadataText.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        foreach (string Entry in Entries)
            ParseFieldMetadataEntry(Result, Entry, Entries.Length > 1);

        if (Result.IsMemo && Result.IsLargeMemo)
            AddFieldMetadataError(Result, "Field metadata cannot contain both Memo and LargeMemo: " + MetadataText);

        return Result;
    }
    /// <summary>
    /// Adds a field metadata error.
    /// </summary>
    static void AddFieldMetadataError(FieldMetadata Metadata, string Text)
    {
        Metadata.Errors.Add(Text);
    }
    /// <summary>
    /// Parses a single field metadata entry.
    /// </summary>
    static void ParseFieldMetadataEntry(FieldMetadata Metadata, string Entry, bool Strict)
    {
        if (string.IsNullOrWhiteSpace(Entry))
            return;
        if (Entry.StartsWith("[") && Entry.EndsWith("]"))
        {
            ParseFieldFlagsMetadata(Metadata, Entry);
            return;
        }
        if (Entry.Contains("[") || Entry.Contains("]"))
        {
            AddFieldMetadataError(Metadata, "Invalid FieldFlags metadata syntax: " + Entry);
            return;
        }

        FieldMetadataKind Kind = GetFieldMetadataKind(Entry);

        if (Kind != FieldMetadataKind.None)
        {
            if (Metadata.Kind != FieldMetadataKind.None)
            {
                AddFieldMetadataError(Metadata, "Field metadata contains multiple primary entries: " + Metadata.MetadataText);
                return;
            }

            Metadata.Kind = Kind;
            Metadata.IsOneToOne = Entry.IndexOf("OneToOne", StringComparison.OrdinalIgnoreCase) >= 0;

            if (Kind == FieldMetadataKind.Lookup)
                ParseLookupMetadata(Metadata, Entry);
            else if (Kind == FieldMetadataKind.Locator || Kind == FieldMetadataKind.CorrelationLocator)
                ParseLocatorMetadata(Metadata, Entry);
            else
                Metadata.MetadataName = ParseMetadataName(Entry, Kind);

            if (Kind == FieldMetadataKind.Code)
                ParseCodeMetadata(Metadata, Entry);
            return;
        }

        if (Entry.IsSameText("Memo"))
        {
            Metadata.IsMemo = true;
            return;
        }
        if (Entry.StartsWith("Memo ", StringComparison.OrdinalIgnoreCase))
        {
            AddFieldMetadataError(Metadata, "Invalid Memo metadata syntax: " + Entry);
            return;
        }
        if (Entry.IsSameText("LargeMemo"))
        {
            Metadata.IsLargeMemo = true;
            return;
        }
        if (Entry.StartsWith("LargeMemo ", StringComparison.OrdinalIgnoreCase))
        {
            AddFieldMetadataError(Metadata, "Invalid LargeMemo metadata syntax: " + Entry);
            return;
        }
        if (Entry.IsSameText("Group"))
        {
            AddFieldMetadataError(Metadata, "Invalid Group metadata syntax: " + Entry);
            return;
        }
        if (Entry.StartsWith("Group ", StringComparison.OrdinalIgnoreCase))
        {
            ParseGroupMetadata(Metadata, Entry);
            return;
        }
        if (Entry.StartsWith("Snapshot ", StringComparison.OrdinalIgnoreCase))
        {
            ParseSnapshotMetadata(Metadata, Entry);
            return;
        }
        if (Entry.IsSameText("TitleKey") || Entry.StartsWith("TitleKey ", StringComparison.OrdinalIgnoreCase))
        {
            ParseTitleKeyMetadata(Metadata, Entry);
            return;
        }

        if (Strict)
            AddFieldMetadataError(Metadata, "Unknown field metadata: " + Entry);
    }
    /// <summary>
    /// Parses field title key metadata.
    /// </summary>
    static void ParseTitleKeyMetadata(FieldMetadata Metadata, string MetadataText)
    {
        string Value = MetadataText.Substring("TitleKey".Length).Trim();
        if (string.IsNullOrWhiteSpace(Value))
        {
            AddFieldMetadataError(Metadata, "Invalid TitleKey metadata syntax: " + MetadataText);
            return;
        }
        Metadata.TitleKey = Value;
    }
    /// <summary>
    /// Parses snapshot metadata.
    /// </summary>
    static void ParseSnapshotMetadata(FieldMetadata Metadata, string MetadataText)
    {
        string Value = MetadataText.Substring("Snapshot".Length).Trim();
        string[] Parts = Value.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (Parts.Length != 2 || !IsIdentifier(Parts[0]) || !IsIdentifier(Parts[1]))
        {
            if (MetadataText.StartsWith("Snapshot of ", StringComparison.OrdinalIgnoreCase))
                return;
            AddFieldMetadataError(Metadata, "Invalid Snapshot metadata syntax: " + MetadataText);
            return;
        }
        Metadata.SnapshotOf = Parts[0] + "." + Parts[1];
    }
    /// <summary>
    /// Parses FieldFlags metadata.
    /// </summary>
    static void ParseFieldFlagsMetadata(FieldMetadata Metadata, string MetadataText)
    {
        string Text = MetadataText.Substring(1, MetadataText.Length - 2).Trim();
        if (string.IsNullOrWhiteSpace(Text))
        {
            AddFieldMetadataError(Metadata, "FieldFlags metadata is empty: " + MetadataText);
            return;
        }

        string[] Parts = Text.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (Parts.Length == 0)
        {
            AddFieldMetadataError(Metadata, "FieldFlags metadata is empty: " + MetadataText);
            return;
        }

        foreach (string Part in Parts)
        {
            if (!Enum.TryParse(Part, ignoreCase: true, out FieldFlags Flag))
            {
                AddFieldMetadataError(Metadata, "Unknown FieldFlags value: " + Part);
                continue;
            }

            Metadata.FieldFlags |= Flag;
        }
    }
    /// <summary>
    /// Returns the field metadata kind of an entry.
    /// </summary>
    static FieldMetadataKind GetFieldMetadataKind(string Entry)
    {
        if (Entry.StartsWith("Correlation Lookup", StringComparison.OrdinalIgnoreCase))
            return FieldMetadataKind.CorrelationLookup;
        if (Entry.StartsWith("Correlation Locator", StringComparison.OrdinalIgnoreCase))
            return FieldMetadataKind.CorrelationLocator;
        if (Entry.StartsWith("Master", StringComparison.OrdinalIgnoreCase))
            return FieldMetadataKind.Master;
        if (Entry.StartsWith("Lookup", StringComparison.OrdinalIgnoreCase))
            return FieldMetadataKind.Lookup;
        if (Entry.StartsWith("Enum", StringComparison.OrdinalIgnoreCase))
            return FieldMetadataKind.Enum;
        if (Entry.StartsWith("Locator", StringComparison.OrdinalIgnoreCase))
            return FieldMetadataKind.Locator;
        if (Entry.IsSameText("Code") || Entry.StartsWith("Code ", StringComparison.OrdinalIgnoreCase))
            return FieldMetadataKind.Code;
        return FieldMetadataKind.None;
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
        else if (Kind == FieldMetadataKind.Lookup && Text.StartsWith("Lookup", StringComparison.OrdinalIgnoreCase))
            Text = Text.Substring("Lookup".Length).Trim();
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
    /// Parses lookup metadata.
    /// </summary>
    static void ParseLookupMetadata(FieldMetadata Metadata, string MetadataText)
    {
        int OpenIndex = MetadataText.IndexOf('(');
        if (OpenIndex >= 0)
            MetadataText = MetadataText.Substring(0, OpenIndex).Trim();

        List<string> Parts = SplitHeaderTokens(MetadataText);
        if (Parts.Count == 0)
            return;

        int SourceIndex = -1;
        int SourceValueIndex = -1;
        string SourceKey = string.Empty;
        string SourceValue = string.Empty;

        for (int i = 1; i < Parts.Count; i++)
        {
            string Part = Parts[i];
            int Index = Part.IndexOf(':');
            string Key = Index >= 0 ? Part.Substring(0, Index).Trim() : Part.EndsWith(":") ? Part.Substring(0, Part.Length - 1).Trim() : string.Empty;
            if (!Key.IsSameText("TableName") && !Key.IsSameText("EnumName") && !Key.IsSameText("EnumType") && !Key.IsSameText("ClassName"))
                continue;

            SourceIndex = i;
            SourceKey = Key;
            SourceValue = Index >= 0 ? Part.Substring(Index + 1).Trim() : string.Empty;
            if (string.IsNullOrWhiteSpace(SourceValue) && i + 1 < Parts.Count)
            {
                SourceValueIndex = i + 1;
                SourceValue = Parts[i + 1];
            }
            break;
        }

        if (SourceIndex < 0)
        {
            if (Parts.Count > 2)
                AddFieldMetadataError(Metadata, "Invalid Lookup metadata syntax: " + MetadataText);
            Metadata.MetadataName = Parts.Count > 1 ? Parts[1] : string.Empty;
            return;
        }

        if (SourceIndex != 2 || string.IsNullOrWhiteSpace(SourceValue))
        {
            AddFieldMetadataError(Metadata, "Invalid Lookup metadata syntax: " + MetadataText);
            return;
        }

        int LastExpectedIndex = SourceValueIndex > 0 ? SourceValueIndex : SourceIndex;
        if (Parts.Count - 1 > LastExpectedIndex)
        {
            AddFieldMetadataError(Metadata, "Invalid Lookup metadata syntax: " + MetadataText);
            return;
        }

        Metadata.MetadataName = Parts[1];
        if (SourceKey.IsSameText("TableName"))
            Metadata.LookupTableName = SourceValue;
        else if (SourceKey.IsSameText("EnumName") || SourceKey.IsSameText("EnumType"))
            Metadata.LookupEnumTypeName = SourceValue;
        else if (SourceKey.IsSameText("ClassName"))
            Metadata.LookupClassName = SourceValue;
    }
    /// <summary>
    /// Parses locator metadata.
    /// </summary>
    static void ParseLocatorMetadata(FieldMetadata Metadata, string MetadataText)
    {
        int OpenIndex = MetadataText.IndexOf('(');
        if (OpenIndex >= 0)
            MetadataText = MetadataText.Substring(0, OpenIndex).Trim();

        string Prefix = Metadata.Kind == FieldMetadataKind.CorrelationLocator ? "Correlation Locator" : "Locator";
        string Text = MetadataText.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase)
            ? MetadataText.Substring(Prefix.Length).Trim()
            : string.Empty;

        List<string> Parts = SplitHeaderTokens(Text);
        if (Parts.Count == 0)
            return;

        if (Parts[0].StartsWith("ClassName:", StringComparison.OrdinalIgnoreCase) || Parts[0].IsSameText("ClassName:"))
        {
            AddFieldMetadataError(Metadata, "Locator name is required when ClassName is specified: " + MetadataText);
            return;
        }

        Metadata.MetadataName = Parts[0];

        if (Parts.Count == 1)
            return;

        string Part = Parts[1];
        int Index = Part.IndexOf(':');
        string Key = Index >= 0 ? Part.Substring(0, Index).Trim() : Part.EndsWith(":") ? Part.Substring(0, Part.Length - 1).Trim() : string.Empty;
        if (!Key.IsSameText("ClassName"))
        {
            AddFieldMetadataError(Metadata, "Invalid Locator metadata syntax: " + MetadataText);
            return;
        }

        string ClassName = Index >= 0 ? Part.Substring(Index + 1).Trim() : string.Empty;
        int LastExpectedIndex = 1;
        if (string.IsNullOrWhiteSpace(ClassName) && Parts.Count > 2)
        {
            ClassName = Parts[2];
            LastExpectedIndex = 2;
        }

        if (string.IsNullOrWhiteSpace(ClassName) || Parts.Count - 1 > LastExpectedIndex)
        {
            AddFieldMetadataError(Metadata, "Invalid Locator metadata syntax: " + MetadataText);
            return;
        }

        Metadata.LocatorClassName = ClassName;
    }
    /// <summary>
    /// Parses group metadata.
    /// </summary>
    static void ParseGroupMetadata(FieldMetadata Metadata, string MetadataText)
    {
        List<string> Parts = SplitHeaderTokens(MetadataText);
        if (Parts.Count != 2)
        {
            AddFieldMetadataError(Metadata, "Invalid Group metadata syntax: " + MetadataText);
            return;
        }

        Metadata.GroupName = Parts[1];
    }
    /// <summary>
    /// Parses code metadata.
    /// </summary>
    static void ParseCodeMetadata(FieldMetadata Metadata, string MetadataText)
    {
        List<string> Parts = SplitHeaderTokens(MetadataText);
        bool IsDraft = Parts.Count > 1 && Parts[1].IsSameText("Draft");
        int PatternIndex = IsDraft ? 2 : 1;
        int NameIndex = IsDraft ? 3 : 2;

        if (Parts.Count > NameIndex + 1)
        {
            AddFieldMetadataError(Metadata, "Invalid Code metadata syntax: " + MetadataText);
            return;
        }

        Metadata.IsDraftCodeProvider = IsDraft;
        Metadata.CodeProviderPattern = Parts.Count > PatternIndex ? Parts[PatternIndex] : "XXXXXX";
        Metadata.CodeProviderName = Parts.Count > NameIndex ? Parts[NameIndex] : string.Empty;
    }
    /// <summary>
    /// Parses code provider metadata.
    /// </summary>
    static CodeProviderMetadata ParseCodeProviderMetadata(string MetadataText)
    {
        FieldMetadata Metadata = new();
        ParseCodeMetadata(Metadata, "Code " + MetadataText);

        if (Metadata.Errors.Count > 0)
            throw new TripousDataException(Metadata.Errors[0]);

        CodeProviderMetadata Result = new();
        Result.Pattern = Metadata.CodeProviderPattern;
        Result.ProviderName = Metadata.CodeProviderName;
        Result.IsDraft = Metadata.IsDraftCodeProvider;
        return Result;
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
        if (IsLocatorField(Field))
            return false;

        if (Field.MetadataKind == FieldMetadataKind.Lookup)
            return true;

        return false;
    }
    /// <summary>
    /// Returns lookup source name for a field.
    /// </summary>
    static string GetLookupSourceName(SchemaScript Script, SchemaField Field)
    {
        if (!string.IsNullOrWhiteSpace(Field.MetadataName))
            return Field.MetadataName;
        if (Field.ForeignKey != null)
            return Field.ForeignKey.ReferenceTable;

        return RemoveIdSuffix(Field.Name);
    }

    /// <summary>
    /// Returns enum lookup source name for a field.
    /// </summary>
    static string GetEnumName(SchemaScript Script, SchemaField Field)
    {
        if (!string.IsNullOrWhiteSpace(Field.MetadataName))
            return Field.MetadataName;

        return RemoveIdSuffix(Field.Name);
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
        Result.ClassName = Field.LocatorClassName;
        Result.FormName = ReferenceTable.FormName;
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

        return ", ClassName: \"" + EscapeString(ClassName) + "\"";
    }
    /// <summary>
    /// Builds optional form name argument.
    /// </summary>
    static string BuildOptionalFormNameArgument(string FormName)
    {
        if (string.IsNullOrWhiteSpace(FormName))
            return string.Empty;

        return ", FormName: \"" + EscapeString(FormName) + "\"";
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
    /// Returns the resolved code provider name for a field.
    /// </summary>
    static string GetCodeProviderName(SchemaTable Table, SchemaField Field)
    {
        return !string.IsNullOrWhiteSpace(Field.CodeProviderName) ? Field.CodeProviderName : Table.Name;
    }
    /// <summary>
    /// Returns the draft code provider name.
    /// </summary>
    static string GetDraftCodeProviderName(string CodeProviderName) => "DRAFT-" + CodeProviderName;
    /// <summary>
    /// Returns the draft code provider pattern.
    /// </summary>
    static string GetDraftCodeProviderPattern(string Pattern) => "DRAFT-" + Pattern;
    /// <summary>
    /// Returns the code provider name assigned to a field.
    /// </summary>
    static string GetFieldCodeProviderName(SchemaTable Table, SchemaField Field, SchemaModuleBlock ModuleBlock = null)
    {
        if (ModuleBlock != null && ModuleBlock.CodeProvider != null)
        {
            string ModuleCodeProviderName = GetModuleCodeProviderName(ModuleBlock);
            return ModuleBlock.CodeProvider.IsDraft ? GetDraftCodeProviderName(ModuleCodeProviderName) : ModuleCodeProviderName;
        }

        string CodeProviderName = GetCodeProviderName(Table, Field);
        return Field.IsDraftCodeProvider ? GetDraftCodeProviderName(CodeProviderName) : CodeProviderName;
    }
    /// <summary>
    /// Returns the code provider name assigned to a module block.
    /// </summary>
    static string GetModuleCodeProviderName(SchemaModuleBlock ModuleBlock)
    {
        return !string.IsNullOrWhiteSpace(ModuleBlock.CodeProvider.ProviderName) ? ModuleBlock.CodeProvider.ProviderName : ModuleBlock.ModuleName;
    }
    /// <summary>
    /// Returns all code provider names generated by a module block.
    /// </summary>
    static List<string> GetModuleCodeProviderNames(SchemaModuleBlock ModuleBlock)
    {
        string CodeProviderName = GetModuleCodeProviderName(ModuleBlock);
        List<string> Result = [CodeProviderName];
        if (ModuleBlock.CodeProvider.IsDraft)
            Result.Add(GetDraftCodeProviderName(CodeProviderName));
        return Result;
    }
    /// <summary>
    /// Returns true when a table should use field-level code provider metadata.
    /// </summary>
    static bool TableUsesFieldCodeProvider(SchemaTable Table)
    {
        return Table.ModuleBlocks.Count == 0 || Table.ModuleBlocks.Any(x => x.CodeProvider == null);
    }
    /// <summary>
    /// Returns all code provider names generated by a field.
    /// </summary>
    static List<string> GetCodeProviderNames(SchemaTable Table, SchemaField Field)
    {
        string CodeProviderName = GetCodeProviderName(Table, Field);
        List<string> Result = [CodeProviderName];
        if (Field.IsDraftCodeProvider)
            Result.Add(GetDraftCodeProviderName(CodeProviderName));
        return Result;
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
        return Field.DataType.IsValidFilterType();
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
        void AddPart(FieldFlags Flag)
        {
            string Text = "FieldFlags." + Flag;
            if (!Parts.Contains(Text))
                Parts.Add(Text);
        }

        if (!Field.IsNullable)
            AddPart(FieldFlags.Required);
        if (Field.Name.IsSameText("Code"))
            AddPart(FieldFlags.ReadOnlyEdit);
        if (Field.Name.IsSameText("Code") && Field.MetadataKind == FieldMetadataKind.Code)
            AddPart(FieldFlags.ReadOnlyUI);
        foreach (FieldFlags Flag in Enum.GetValues(typeof(FieldFlags)))
            if (Flag != FieldFlags.None && Field.FieldFlags.HasFlag(Flag))
                AddPart(Flag);

        if (Parts.Count == 0)
            return "FieldFlags.None";

        return string.Join(" | ", Parts);
    }
    /// <summary>
    /// Returns true when a field is an Id field.
    /// </summary>
    static bool IsIdField(SchemaField Field)
    {
        return Field.Name.IsSameText("Id") || Field.Name.EndsWithText("Id");
    }
    /// <summary>
    /// Escapes a C# string.
    /// </summary>
    static string EscapeString(string Value)
    {
        return (Value ?? string.Empty).Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
    /// <summary>
    /// Escapes a SQL string literal.
    /// </summary>
    static string EscapeSqlString(string Value)
    {
        return (Value ?? string.Empty).Replace("'", "''");
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
        
        public void ResolveCreationOrders(SchemaParserResult Result)
        {
            Dictionary<string, List<string>> Dependencies = new(StringComparer.OrdinalIgnoreCase);
            HashSet<string> Done = new(StringComparer.OrdinalIgnoreCase);
            int Order = 0;

            List<SchemaTable> TablesToOrder = OutputTables;

            foreach (SchemaTable Table in TablesToOrder)
                Dependencies[Table.Name] = GetTableDependencies(Table);

            while (Done.Count < TablesToOrder.Count)
            {
                List<SchemaTable> ReadyTables = TablesToOrder
                    .Where(x => !Done.Contains(x.Name) && Dependencies[x.Name].All(Dependency => Done.Contains(Dependency)))
                    .ToList();

                if (ReadyTables.Count == 0)
                {
                    AddError(Result, "SCHEMA_CREATION_ORDER_FAILED", "Could not resolve schema creation order.");
                    return;
                }

                foreach (SchemaTable Table in ReadyTables)
                {
                    Table.CreationOrder = ++Order;
                    Done.Add(Table.Name);
                }
            }
        }
        /// <summary>
        /// Returns dependency table names for a table.
        /// </summary>
        List<string> GetTableDependencies(SchemaTable Table)
        {
            List<string> Result = [];

            foreach (SchemaForeignKey ForeignKey in Table.ForeignKeys)
            {
                if (ForeignKey.ReferenceTable.IsSameText(Table.Name))
                    continue;
                SchemaTable DependencyTable = FindTable(ForeignKey.ReferenceTable);
                if (DependencyTable == null || DependencyTable.IsReference)
                    continue;
                if (!Result.Any(x => x.IsSameText(ForeignKey.ReferenceTable)))
                    Result.Add(ForeignKey.ReferenceTable);
            }

            SchemaTable MasterTable = FindTable(Table.MasterName);
            if (!string.IsNullOrWhiteSpace(Table.MasterName) && !Table.MasterName.IsSameText(Table.Name) && MasterTable != null && !MasterTable.IsReference)
            {
                if (!Result.Any(x => x.IsSameText(Table.MasterName)))
                    Result.Add(Table.MasterName);
            }

            return Result;
        }

        // ● static public
        /// <summary>
        /// Parses a schema script.
        /// </summary>
        static public SchemaScript Parse(string SchemaSql)
            => Parse(SchemaSql, []);
        /// <summary>
        /// Parses a schema script using reference schema scripts as metadata context.
        /// </summary>
        static public SchemaScript Parse(string SchemaSql, List<string> ReferenceSchemaSqls)
        {
            SchemaScript Result = ParseCore(SchemaSql, false);

            foreach (string ReferenceSchemaSql in ReferenceSchemaSqls)
            {
                if (string.IsNullOrWhiteSpace(ReferenceSchemaSql))
                    continue;

                SchemaScript ReferenceScript = ParseCore(ReferenceSchemaSql, true);
                Result.Tables.AddRange(ReferenceScript.Tables);
            }

            Result.ResolveReferences();
            Result.ResolveLookupHeuristics();
            Result.ResolveDetails();

            return Result;
        }
        /// <summary>
        /// Parses a schema script without resolving references.
        /// </summary>
        static SchemaScript ParseCore(string SchemaSql, bool IsReference)
        {
            SchemaScript Result = new();
            MatchCollection Matches = Regex.Matches(SchemaSql, @"CREATE\s+TABLE\s+\{TableName\}\s*\(", RegexOptions.IgnoreCase);

            foreach (Match Match in Matches)
            {
                int HeaderStartIndex = FindHeaderStartIndex(SchemaSql, Match.Index);
                int HeaderLine = GetLineNumber(SchemaSql, HeaderStartIndex);
                int CreateTableLine = GetLineNumber(SchemaSql, Match.Index);

                try
                {
                    string HeaderText = ExtractHeaderText(SchemaSql, Match.Index);
                    string CreateSqlText = ExtractCreateTableSql(SchemaSql, Match.Index);
                    SchemaTable Table = SchemaTable.Parse(HeaderText, CreateSqlText);
                    Table.IsReference = IsReference;
                    Result.Tables.Add(Table);
                }
                catch (Exception ex)
                {
                    throw new TripousDataException("Schema table parse failed near header line " + HeaderLine + ", CREATE TABLE line " + CreateTableLine + ". " + ex.Message);
                }
            }
            
            return Result;
        }

        // ● public
        /// <summary>
        /// Finds a table by name.
        /// </summary>
        public SchemaTable FindTable(string Name) => Tables.FirstOrDefault(x => x.Name.IsSameText(Name));
        /// <summary>
        /// Returns detail tables of a master table.
        /// </summary>
        public List<SchemaTable> GetDetailsOf(SchemaTable MasterTable)
        {
            return Tables
                .Where(x => !x.IsReference && x.MasterName.IsSameText(MasterTable.Name))
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
                if (Table.IsTopTable && Table.ModuleBlocks.Any(x => string.IsNullOrWhiteSpace(x.ModuleName)))
                    throw new TripousDataException("Top table has no Module: " + Table.Name);
                if (Table.IsTopTable && Table.ModuleBlocks.Any(x => string.IsNullOrWhiteSpace(x.GroupName)))
                    throw new TripousDataException("Top table has no Group: " + Table.Name);
                if (!Table.IsTopTable && string.IsNullOrWhiteSpace(Table.MasterName))
                    throw new TripousDataException("Detail table has no Master: " + Table.Name);
            }

            CheckDuplicateCreationOrders();
            CheckCircularReferences();
        }

        // ● private
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
        /// Tables that belong to the output schema.
        /// </summary>
        public List<SchemaTable> OutputTables => Tables.Where(x => !x.IsReference).ToList();
        /// <summary>
        /// Top module tables.
        /// </summary>
        public List<SchemaTable> TopTables => OutputTables.Where(x => x.IsTopTable).ToList();
    }

    /// <summary>
    /// Header entry.
    /// </summary>
    private class HeaderEntry
    {
        // ● properties
        /// <summary>
        /// Header entry name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Header entry value.
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// Parsed schema table.
    /// </summary>
    private class SchemaTable
    {
        // ● private fields
        List<SchemaField> fFields = [];
        List<SchemaForeignKey> fForeignKeys = [];
        List<SchemaModuleBlock> fModuleBlocks = [];

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

            Result.Name = ParseSingleHeaderToken(GetHeaderValue(HeaderText, "Table"), "Table");
            Result.ParseModuleBlocks(HeaderText);
            Result.FieldGroups = SplitHeaderList(GetHeaderValue(HeaderText, "FieldGroups"));
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
        void ParseModuleHeader(SchemaModuleBlock ModuleBlock, string Text)
        {
            List<string> Parts = SplitHeaderTokens(Text);
            if (Parts.Count == 0)
                return;
            if (Parts.Count > 2)
                throw new TripousDataException("Invalid Module header syntax: \"" + Text + "\". Expected: Module: Default | MODULE_NAME [MODULE_CLASS_NAME]");

            ModuleBlock.ModuleName = Parts[0].IsSameText("Default") ? Name : Parts[0];

            if (Parts.Count > 1)
                ModuleBlock.ModuleClassName = Parts[1];
        }
        /// <summary>
        /// Parses form header text.
        /// </summary>
        void ParseFormHeader(SchemaModuleBlock ModuleBlock, string Text)
        {
            ModuleBlock.IsFormSpecified = !string.IsNullOrWhiteSpace(Text);
            List<string> Parts = SplitHeaderTokens(Text);
            if (Parts.Count == 0)
                return;
            if (Parts.Count > 3)
                throw new TripousDataException("Invalid Form header syntax: \"" + Text + "\". Expected: Form: DataForm | FORM_NAME [FORM_CLASS_NAME]");

            ModuleBlock.FormName = Parts[0];

            if (Parts.Count > 1)
                ModuleBlock.FormClassName = Parts[1];
            if (Parts.Count > 2)
            {
                if (!string.IsNullOrWhiteSpace(ModuleBlock.ItemPageClassName))
                    throw new TripousDataException("Module block contains duplicate ItemPage: " + ModuleBlock.ModuleName);
                ModuleBlock.ItemPageClassName = Parts[2];
            }
        }
        /// <summary>
        /// Parses item page header text.
        /// </summary>
        void ParseItemPageHeader(SchemaModuleBlock ModuleBlock, string Text)
        {
            List<string> Parts = SplitHeaderTokens(Text);
            if (Parts.Count != 1)
                throw new TripousDataException("Invalid ItemPage header syntax: \"" + Text + "\". Expected: ItemPage: ItemPage | ITEM_PAGE_CLASS_NAME");

            ModuleBlock.ItemPageClassName = Parts[0];
        }
        /// <summary>
        /// Parses a detail order header.
        /// </summary>
        void ParseDetailOrderHeader(SchemaModuleBlock ModuleBlock, string Text)
        {
            int SeparatorIndex = Text.IndexOf('=');
            if (SeparatorIndex <= 0 || SeparatorIndex == Text.Length - 1)
                throw new TripousDataException("Invalid DetailOrder header syntax: \"" + Text + "\". Expected: DetailOrder: PARENT_TABLE=CHILD_TABLE, CHILD_TABLE");

            string ParentName = Text.Substring(0, SeparatorIndex).Trim();
            List<string> DetailNames = SplitHeaderList(Text.Substring(SeparatorIndex + 1));
            if (!IsIdentifier(ParentName) || DetailNames.Count == 0 || DetailNames.Any(Name => !IsIdentifier(Name)))
                throw new TripousDataException("Invalid DetailOrder header syntax: \"" + Text + "\". Expected: DetailOrder: PARENT_TABLE=CHILD_TABLE, CHILD_TABLE");
            if (ModuleBlock.DetailOrder.ContainsKey(ParentName))
                throw new TripousDataException("Module block contains duplicate DetailOrder parent: " + ModuleBlock.ModuleName + " -> " + ParentName);

            ModuleBlock.DetailOrder[ParentName] = DetailNames;
        }
        /// <summary>
        /// Parses module blocks.
        /// </summary>
        void ParseModuleBlocks(string HeaderText)
        {
            SchemaModuleBlock Current = null;
            bool ModuleMetadataClosed = false;
            List<HeaderEntry> Entries = GetHeaderEntries(HeaderText);

            if (Entries.Count > 0 && !Entries[0].Name.IsSameText("Table"))
                throw new TripousDataException("Invalid header order. Table must be the first metadata entry.");

            foreach (HeaderEntry Entry in Entries)
            {
                if (Entry.Name.IsSameText("Table"))
                    continue;

                if (Entry.Name.IsSameText("Module"))
                {
                    if (ModuleMetadataClosed)
                        throw new TripousDataException("Invalid Module header order. Module blocks must appear before table-level metadata.");

                    Current = new SchemaModuleBlock();
                    ParseModuleHeader(Current, Entry.Value);
                    ModuleBlocks.Add(Current);
                    continue;
                }

                if (Entry.Name.IsSameText("Group"))
                {
                    if (Current == null)
                        throw new TripousDataException("Group metadata requires a preceding Module line.");
                    if (!string.IsNullOrWhiteSpace(Current.GroupName))
                        throw new TripousDataException("Module block contains duplicate Group: " + Current.ModuleName);
                    Current.GroupName = Entry.Value;
                    continue;
                }

                if (Entry.Name.IsSameText("Form"))
                {
                    if (Current == null)
                        throw new TripousDataException("Form metadata requires a preceding Module line.");
                    if (Current.IsFormSpecified)
                        throw new TripousDataException("Module block contains duplicate Form: " + Current.ModuleName);
                    ParseFormHeader(Current, Entry.Value);
                    continue;
                }

                if (Entry.Name.IsSameText("ItemPage"))
                {
                    if (Current == null)
                        throw new TripousDataException("ItemPage metadata requires a preceding Module line.");
                    if (!string.IsNullOrWhiteSpace(Current.ItemPageClassName))
                        throw new TripousDataException("Module block contains duplicate ItemPage: " + Current.ModuleName);
                    ParseItemPageHeader(Current, Entry.Value);
                    continue;
                }

                if (Entry.Name.IsSameText("SecurityLevel"))
                {
                    if (Current == null)
                        throw new TripousDataException("SecurityLevel metadata requires a preceding Module line.");
                    if (Current.SecurityLevel != UserLevel.None)
                        throw new TripousDataException("Module block contains duplicate SecurityLevel: " + Current.ModuleName);
                    if (!Enum.TryParse(Entry.Value, ignoreCase: true, out UserLevel SecurityLevel))
                        throw new TripousDataException("Invalid SecurityLevel metadata value: " + Entry.Value);
                    Current.SecurityLevel = SecurityLevel;
                    continue;
                }

                if (Entry.Name.IsSameText("DetailOrder"))
                {
                    if (Current == null)
                        throw new TripousDataException("DetailOrder metadata requires a preceding Module line.");
                    ParseDetailOrderHeader(Current, Entry.Value);
                    continue;
                }

                if (Entry.Name.IsSameText("ListWhere"))
                {
                    if (Current == null)
                        throw new TripousDataException("ListWhere metadata requires a preceding Module line.");
                    if (!string.IsNullOrWhiteSpace(Current.ListWhere))
                        throw new TripousDataException("Module block contains duplicate ListWhere: " + Current.ModuleName);
                    if (string.IsNullOrWhiteSpace(Entry.Value))
                        throw new TripousDataException("Invalid ListWhere header syntax. Expected: ListWhere: SQL_CONDITION");
                    Current.ListWhere = Entry.Value;
                    continue;
                }

                if (Entry.Name.IsSameText("FilterFields"))
                {
                    if (Current == null)
                        throw new TripousDataException("FilterFields metadata requires a preceding Module line.");
                    if (Current.FilterFields.Count > 0)
                        throw new TripousDataException("Module block contains duplicate FilterFields: " + Current.ModuleName);

                    List<string> FieldNames = SplitHeaderList(Entry.Value);
                    if (FieldNames.Count == 0 || FieldNames.Any(Name => !IsIdentifier(Name)))
                        throw new TripousDataException("Invalid FilterFields header syntax. Expected: FilterFields: FIELD_NAME, FIELD_NAME");

                    List<string> Duplicates = FieldNames
                        .GroupBy(Name => Name, StringComparer.OrdinalIgnoreCase)
                        .Where(Group => Group.Count() > 1)
                        .Select(Group => Group.Key)
                        .ToList();
                    if (Duplicates.Count > 0)
                        throw new TripousDataException("Module block contains duplicate FilterFields entries: " + Current.ModuleName + " -> " + string.Join(", ", Duplicates));

                    Current.FilterFields = FieldNames;
                    continue;
                }

                if (Entry.Name.IsSameText("Code"))
                {
                    if (Current == null)
                        throw new TripousDataException("Code metadata requires a preceding Module line.");
                    if (Current.CodeProvider != null)
                        throw new TripousDataException("Module block contains duplicate Code: " + Current.ModuleName);
                    Current.CodeProvider = ParseCodeProviderMetadata(Entry.Value);
                    continue;
                }

                if (ModuleBlocks.Count > 0)
                {
                    Current = null;
                    ModuleMetadataClosed = true;
                }
            }

            foreach (SchemaModuleBlock ModuleBlock in ModuleBlocks)
            {
                if (string.IsNullOrWhiteSpace(ModuleBlock.GroupName))
                    throw new TripousDataException("Module block has no Group: " + ModuleBlock.ModuleName);

                ResolveFormDefaults(ModuleBlock);
            }

            SyncPrimaryModuleProperties();
        }
        /// <summary>
        /// Resolves default form values after module parsing.
        /// </summary>
        void ResolveFormDefaults(SchemaModuleBlock ModuleBlock)
        {
            if (string.IsNullOrWhiteSpace(ModuleBlock.ModuleName))
                return;

            if (string.IsNullOrWhiteSpace(ModuleBlock.FormName))
                ModuleBlock.FormName = ModuleBlock.ModuleName;
            else if (ModuleBlock.FormName.IsSameText("Default") || ModuleBlock.FormName.IsSameText("DataForm"))
                ModuleBlock.FormName = ModuleBlock.ModuleName;
        }
        /// <summary>
        /// Synchronizes compatibility properties from the first module block.
        /// </summary>
        void SyncPrimaryModuleProperties()
        {
            SchemaModuleBlock ModuleBlock = ModuleBlocks.FirstOrDefault();
            if (ModuleBlock == null)
                return;

            ModuleName = ModuleBlock.ModuleName;
            ModuleClassName = ModuleBlock.ModuleClassName;
            FormName = ModuleBlock.FormName;
            FormClassName = ModuleBlock.FormClassName;
            ItemPageClassName = ModuleBlock.ItemPageClassName;
            IsFormSpecified = ModuleBlock.IsFormSpecified;
            GroupName = ModuleBlock.GroupName;
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
        /// True when the table belongs to a reference schema.
        /// </summary>
        public bool IsReference { get; set; }
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
        /// Preferred field group display order.
        /// </summary>
        public List<string> FieldGroups { get; set; } = [];
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
        public bool IsTopTable => ModuleBlocks.Count > 0;
        /// <summary>
        /// Parsed fields.
        /// </summary>
        public List<SchemaField> Fields { get => fFields; set => fFields = value; }
        /// <summary>
        /// Parsed foreign keys.
        /// </summary>
        public List<SchemaForeignKey> ForeignKeys { get => fForeignKeys; set => fForeignKeys = value; }
        /// <summary>
        /// Module blocks.
        /// </summary>
        public List<SchemaModuleBlock> ModuleBlocks { get => fModuleBlocks; set => fModuleBlocks = value; }
    }

    /// <summary>
    /// Parsed module block.
    /// </summary>
    private class SchemaModuleBlock
    {
        // ● properties
        /// <summary>
        /// Module name.
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// Module class name.
        /// </summary>
        public string ModuleClassName { get; set; }
        /// <summary>
        /// Module group name.
        /// </summary>
        public string GroupName { get; set; }
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
        /// Minimum user level required for this module and form.
        /// </summary>
        public UserLevel SecurityLevel { get; set; }
        /// <summary>
        /// True when Form was explicitly defined.
        /// </summary>
        public bool IsFormSpecified { get; set; }
        /// <summary>
        /// Preferred direct child detail display order, keyed by parent table name.
        /// </summary>
        public Dictionary<string, List<string>> DetailOrder { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        /// <summary>
        /// List SELECT column names used for generated filters.
        /// </summary>
        public List<string> FilterFields { get; set; } = [];
        /// <summary>
        /// Optional SQL condition appended to the generated list SELECT.
        /// </summary>
        public string ListWhere { get; set; }
        /// <summary>
        /// Code provider metadata.
        /// </summary>
        public CodeProviderMetadata CodeProvider { get; set; }
    }

    /// <summary>
    /// Parsed code provider metadata.
    /// </summary>
    private class CodeProviderMetadata
    {
        // ● properties
        /// <summary>
        /// Code provider pattern.
        /// </summary>
        public string Pattern { get; set; }
        /// <summary>
        /// Code provider name.
        /// </summary>
        public string ProviderName { get; set; }
        /// <summary>
        /// True when the code provider is draft-enabled.
        /// </summary>
        public bool IsDraft { get; set; }
    }

    /// <summary>
    /// Module registration.
    /// </summary>
    private class SchemaModuleRegistration
    {
        // ● properties
        /// <summary>
        /// Top table.
        /// </summary>
        public SchemaTable Table { get; set; }
        /// <summary>
        /// Module block.
        /// </summary>
        public SchemaModuleBlock Module { get; set; }
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
            Result.LookupTableName = Metadata.LookupTableName;
            Result.LookupEnumTypeName = Metadata.LookupEnumTypeName;
            Result.LookupClassName = Metadata.LookupClassName;
            Result.LocatorClassName = Metadata.LocatorClassName;
            Result.CommentText = Metadata.CommentText;
            Result.IsOneToOne = Metadata.IsOneToOne;
            Result.IsMemo = Metadata.IsMemo;
            Result.IsLargeMemo = Metadata.IsLargeMemo;
            Result.GroupName = Metadata.GroupName;
            Result.FieldFlags = Metadata.FieldFlags;
            Result.MetadataErrors = Metadata.Errors;
            Result.CodeProviderPattern = Metadata.CodeProviderPattern;
            Result.CodeProviderName = Metadata.CodeProviderName;
            Result.IsDraftCodeProvider = Metadata.IsDraftCodeProvider;
            Result.TitleKey = Metadata.TitleKey;
            Result.SnapshotOf = Metadata.SnapshotOf;
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
        /// Lookup source table name.
        /// </summary>
        public string LookupTableName { get; set; }
        /// <summary>
        /// Lookup source enum type name.
        /// </summary>
        public string LookupEnumTypeName { get; set; }
        /// <summary>
        /// Lookup source class name.
        /// </summary>
        public string LookupClassName { get; set; }
        /// <summary>
        /// Locator class name.
        /// </summary>
        public string LocatorClassName { get; set; }
        /// <summary>
        /// Code provider pattern.
        /// </summary>
        public string CodeProviderPattern { get; set; }
        /// <summary>
        /// Code provider name.
        /// </summary>
        public string CodeProviderName { get; set; }
        /// <summary>
        /// True when the code provider is draft-enabled.
        /// </summary>
        public bool IsDraftCodeProvider { get; set; }
        /// <summary>
        /// Field title resource key.
        /// </summary>
        public string TitleKey { get; set; }
        /// <summary>
        /// Source field stored as a snapshot.
        /// </summary>
        public string SnapshotOf { get; set; }
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
        /// True when Memo marker exists.
        /// </summary>
        public bool IsMemo { get; set; }
        /// <summary>
        /// True when LargeMemo marker exists.
        /// </summary>
        public bool IsLargeMemo { get; set; }
        /// <summary>
        /// Field UI group name.
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Field flags.
        /// </summary>
        public FieldFlags FieldFlags { get; set; }
        /// <summary>
        /// Metadata errors.
        /// </summary>
        public List<string> MetadataErrors { get; set; } = [];
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
        /// Locator associated form name.
        /// </summary>
        public string FormName { get; set; }
        /// <summary>
        /// Return fields.
        /// </summary>
        public List<string> ReturnFields { get; set; } = [];
    }
    /// <summary>
    /// Parsed lookup source metadata.
    /// </summary>
    private class LookupSourceInfo
    {
        // ● properties
        /// <summary>
        /// Lookup source name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Lookup source table name.
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// Lookup source enum type name.
        /// </summary>
        public string EnumTypeName { get; set; }
        /// <summary>
        /// Lookup source class name.
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// Lookup source associated form name.
        /// </summary>
        public string FormName { get; set; }
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
        /// Lookup source table name.
        /// </summary>
        public string LookupTableName { get; set; }
        /// <summary>
        /// Lookup source enum type name.
        /// </summary>
        public string LookupEnumTypeName { get; set; }
        /// <summary>
        /// Lookup source class name.
        /// </summary>
        public string LookupClassName { get; set; }
        /// <summary>
        /// Locator class name.
        /// </summary>
        public string LocatorClassName { get; set; }
        /// <summary>
        /// Code provider pattern.
        /// </summary>
        public string CodeProviderPattern { get; set; }
        /// <summary>
        /// Code provider name.
        /// </summary>
        public string CodeProviderName { get; set; }
        /// <summary>
        /// True when the code provider is draft-enabled.
        /// </summary>
        public bool IsDraftCodeProvider { get; set; }
        /// <summary>
        /// Field title resource key.
        /// </summary>
        public string TitleKey { get; set; }
        /// <summary>
        /// Source field stored as a snapshot.
        /// </summary>
        public string SnapshotOf { get; set; }
        /// <summary>
        /// True when one-to-one.
        /// </summary>
        public bool IsOneToOne { get; set; }
        /// <summary>
        /// True when Memo marker exists.
        /// </summary>
        public bool IsMemo { get; set; }
        /// <summary>
        /// True when LargeMemo marker exists.
        /// </summary>
        public bool IsLargeMemo { get; set; }
        /// <summary>
        /// Field UI group name.
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Field flags.
        /// </summary>
        public FieldFlags FieldFlags { get; set; }
        /// <summary>
        /// Metadata errors.
        /// </summary>
        public List<string> Errors { get; set; } = [];
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
        Code = 7,
    }
    private enum VisitState
    {
        None = 0,
        Visiting = 1,
        Done = 2,
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
        /// Fields included in the generated list SELECT.
        /// </summary>
        public List<SelectField> SelectFields { get; set; } = [];
        /// <summary>
        /// Filter fields.
        /// </summary>
        public List<SelectField> FilterFields { get; set; } = [];
        /// <summary>
        /// Generated List SELECT column types.
        /// </summary>
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
    }
}
