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
/// Provides static data discovery methods to populate a <see cref="DbSchema"/> container using embedded SQL resources.
/// </summary>
static public class DbSchemaLoader
{
    // ● private methods
    /// <summary>
    /// Determines the embedded resource namespace base folder path corresponding to the given database server provider.
    /// </summary>
    static string GetSqlResourceBasePath(DbServerType DbServerType)
    {
        string folder = DbServerType switch
        {
            DbServerType.MsSql      => "MsSql",
            DbServerType.MySql      => "MySql",
            DbServerType.PostgreSql => "PostgreSql",
            DbServerType.Firebird   => "FirebirdSql",
            DbServerType.Oracle     => "Oracle",
            DbServerType.Sqlite     => "Sqlite",
   
            _ => throw new Exception($"Unsupported DbServerType: {DbServerType}")
        };

        string ResourceBasePath = $"Tripous.Data.Metadata.Resources.Sql.{folder}";

        return ResourceBasePath;
    }
    /// <summary>
    /// Clears all existing operational metadata lists from the specified schema model.
    /// </summary>
    static void Clear(DbSchema Schema)
    {
        Schema.Tables.Clear();
        Schema.Views.Clear();
        Schema.Sequences.Clear();
        Schema.Procedures.Clear();
    }
    /// <summary>
    /// Extracts the raw SQL statement script content from the embedded assembly manifest stream resource.
    /// </summary>
    static string ReadEmbeddedSql(string baseResourcePath, string fileName)
    {
        Assembly assembly = typeof(DbSchema).Assembly;
        string fileStem = Path.GetFileNameWithoutExtension(fileName);
        string exactName = $"{baseResourcePath}.{fileStem}.sql";

        Stream stream = assembly.GetManifestResourceStream(exactName);
        if (stream != null)
        {
            using (stream)
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        string foundName = string.Empty;
        string[] ManifestResourceNames = assembly.GetManifestResourceNames();
        foreach (string resourceName in ManifestResourceNames)
        {
            if (resourceName.Equals(exactName, StringComparison.OrdinalIgnoreCase))
            {
                foundName = resourceName;
                break;
            }
        }

        if (string.IsNullOrWhiteSpace(foundName))
            return string.Empty;

        Stream foundStream = assembly.GetManifestResourceStream(foundName);
        if (foundStream == null)
            return string.Empty;

        string SqlText = string.Empty;
        using (foundStream)
        using (StreamReader foundReader = new StreamReader(foundStream))
        {
            SqlText = foundReader.ReadToEnd();
        }

        return SqlText;
    }
    /// <summary>
    /// Performs target database provider specific adjustments and literal dynamic tokens updates on schema queries.
    /// </summary>
    static string ProcessReplacementsInMetadataSql(DbConnectionInfo ConInfo, string SqlText)
    {
        ConnectionStringBuilder CSB = new ConnectionStringBuilder(ConInfo.ConnectionString);
 
        switch (ConInfo.DbServerType)
        {
            case DbServerType.MsSql: break;
            case DbServerType.MySql: SqlText = SqlText.Replace("@SCHEMA_NAME", CSB.Database); break;
            case DbServerType.PostgreSql:break;
            case DbServerType.Firebird:break;
            case DbServerType.Oracle:break;
            case DbServerType.Sqlite:break;
 
        }        
        return SqlText;       
    }
    /// <summary>
    /// Fetches the processed embedded DDL command query statement text and returns the executed table results matrix.
    /// </summary>
    static DataTable ExecuteResourceSql(DbConnectionInfo ConInfo, string baseResourcePath, string fileName)
    {
        string SqlText = ReadEmbeddedSql(baseResourcePath, fileName);
        SqlText = ProcessReplacementsInMetadataSql(ConInfo, SqlText);

        if (string.IsNullOrWhiteSpace(SqlText))
            return null;

        SqlProvider Provider = ConInfo.GetSqlProvider();
        return Provider.Select(ConInfo, SqlText);
    }
    /// <summary>
    /// Locates an existing schema table definition row layout entry match, or spawns and appends a new model configuration.
    /// </summary>
    static DbMetaTable FindOrAddTable(DbSchema Schema, string SchemaName, string TableName)
    {
        DbMetaTable MetaTable = Schema.Tables.FirstOrDefault(x =>
            SchemaName.IsSameText(x.SchemaName) && TableName.IsSameText(x.Name));

        if (MetaTable == null)
        {
            MetaTable = new() { Name = TableName, SchemaName = SchemaName };
            Schema.Tables.Add(MetaTable);
        }

        return MetaTable;
    }
    /// <summary>
    /// Locates an existing structural metadata view context layer, or inserts a new instance record context.
    /// </summary>
    static DbMetaView FindOrAddView(DbSchema Schema, string SchemaName, string ViewName)
    {
        DbMetaView MetaView = Schema.Views.FirstOrDefault(x =>
            SchemaName.IsSameText(x.SchemaName) && ViewName.IsSameText(x.Name));

        if (MetaView == null)
        {
            MetaView = new() { Name = ViewName, SchemaName = SchemaName };
            Schema.Views.Add(MetaView);
        }

        return MetaView;
    }
    /// <summary>
    /// Populates table definitions inside the root schema context from data table rows.
    /// </summary>
    static void LoadTables(DbSchema Schema, DataTable tblSql)
    {
        string SchemaName;
        string TableName;
        
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            TableName = Row.AsString("TableName");
            
            FindOrAddTable(Schema, SchemaName, TableName);
        }
    }
    /// <summary>
    /// Extracts column specifications schema attributes matrix into target structural table components container.
    /// </summary>
    static void LoadTableFields(DbSchema Schema, DataTable tblSql)
    {
        DbMetaTable MetaTable;
        DbMetaColumn Column;
        string SchemaName;
        string TableName;
        string FieldName;
        
        foreach (DataRow Row in tblSql.Rows)
        {
            TableName = Row.AsString("TableName");
            SchemaName = Row.AsString("SchemaName");

            MetaTable = FindOrAddTable(Schema, SchemaName, TableName);
 
            FieldName = Row.AsString("FieldName");
            Column = new DbMetaColumn()  { Name = FieldName, SchemaName = SchemaName };
            MetaTable.Columns.Add(Column);

            LoadField(Row, Column);
        }
    }
    /// <summary>
    /// Processes view description statement definition blocks and updates global relational view collections layout.
    /// </summary>
    static void LoadViews(DbSchema Schema, DataTable tblSql)
    {
        if (tblSql == null)
            return;
        
        DbMetaView MetaView;
        string SchemaName;
        string TableName;
 
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            TableName = Row.AsString("TableName");
            
            MetaView = FindOrAddView(Schema, SchemaName, TableName);
            MetaView.SourceCode  = Row.AsString("Definition");
        }
    }
    /// <summary>
    /// Synchronizes structural projection column expressions details mapping to respective relational view layouts.
    /// </summary>
    static void LoadViewFields(DbSchema Schema, DataTable tblSql)
    {
        if (tblSql == null)
            return;
        
        DbMetaView MetaView;
        DbMetaColumn Column;
        string SchemaName;
        string ViewName;
        string FieldName;
 
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            ViewName = Row.AsString("TableName");

            MetaView = FindOrAddView(Schema, SchemaName, ViewName);
 
            FieldName = Row.AsString("FieldName");
            Column = new DbMetaColumn()  { Name = FieldName, SchemaName = SchemaName };
            MetaView.Columns.Add(Column);

            LoadField(Row, Column);
        }
    }
    /// <summary>
    /// Collects index key constraints optimization definitions and assigns key groups to underlying table targets.
    /// </summary>
    static void LoadIndexes(DbSchema Schema, DataTable tblSql)
    {
        if (tblSql == null)
            return;
        
        DbMetaTable MetaTable;
        DbMetaIndex MetaIndex;
        string SchemaName;
        string TableName;
        string IndexName;
        string FieldName;
        
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            TableName = Row.AsString("TableName");
            
            MetaTable = FindOrAddTable(Schema, SchemaName, TableName);

            IndexName = Row.AsString("IndexName");
            FieldName = Row.AsString("FieldName");
            
            MetaIndex = MetaTable.Indexes.FirstOrDefault(x =>
                SchemaName.IsSameText(x.SchemaName) && IndexName.IsSameText(x.Name));

            if (MetaIndex == null)
            {
                MetaIndex = new DbMetaIndex() { SchemaName = SchemaName, Name = IndexName, Columns = FieldName };
                MetaTable.Indexes.Add(MetaIndex);
                MetaIndex.IndexType = Row.AsString("IndexType");
                MetaIndex.IsUnique = Row.AsBoolean("IsUnique");
            }
            else
            {
                MetaIndex.Columns += $";{FieldName}";
            }
        }
    }
    /// <summary>
    /// Processes integrity check constraint layouts and handles multi-column reference key groups formatting.
    /// </summary>
    static void LoadConstraints(DbSchema Schema, DataTable tblSql)
    {
        if (tblSql == null)
            return;
        
        DbMetaTable MetaTable = null;
 
        DbMetaConstraint MetaConstraint;
        DbMetaForeignKey MetaForeignKey;
        string SchemaName;
        string TableName;
        string ConstraintName;
        string FieldName;
        string ForeignField;
        ConstraintType ConstraintType;
 
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            TableName = Row.AsString("TableName").Trim();

            MetaTable = FindOrAddTable(Schema, SchemaName, TableName);
            
            ConstraintName  = Row.AsString("ConstraintName");
            ConstraintType = (ConstraintType)Row.AsInteger("ConstraintType");
            FieldName = Row.AsString("FieldName");
            ForeignField = Row.AsString("ForeignField");

            if (ConstraintType == ConstraintType.PrimaryKey)
            {
                MetaConstraint = MetaTable.Constraints.FirstOrDefault(x => SchemaName.IsSameText(x.SchemaName) && ConstraintName.IsSameText(x.Name));
            }    
            else if (ConstraintType == ConstraintType.ForeignKey)
                MetaConstraint = MetaTable.ForeignKeys.FirstOrDefault(x => SchemaName.IsSameText(x.SchemaName) && ConstraintName.IsSameText(x.Name));
            else
                MetaConstraint = MetaTable.Constraints.FirstOrDefault(x => SchemaName.IsSameText(x.SchemaName) && ConstraintName.IsSameText(x.Name));

            if (MetaConstraint == null)
            {
                if (ConstraintType == ConstraintType.PrimaryKey)
                {
                    MetaConstraint = new DbMetaConstraint() { Name = ConstraintName, SchemaName = SchemaName };
                    MetaTable.Constraints.Add(MetaConstraint);
                }
                else if (ConstraintType == ConstraintType.ForeignKey)
                {
                    MetaForeignKey = new DbMetaForeignKey() { Name = ConstraintName, SchemaName = SchemaName };
                    MetaTable.ForeignKeys.Add(MetaForeignKey);
                    
                    MetaForeignKey.ForeignTable = Row.AsString("ForeignTable");
                    MetaForeignKey.ForeignFields = ForeignField;
 
                    MetaConstraint = MetaForeignKey;
                }
                else
                {
                    MetaConstraint = new DbMetaConstraint() { Name = ConstraintName, SchemaName = SchemaName };
                    MetaTable.Constraints.Add(MetaConstraint);
                }

                MetaConstraint.ConstraintTypeText = Row.AsString("ConstraintTypeText");
                MetaConstraint.ConstraintType = ConstraintType;
                MetaConstraint.Columns = FieldName;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(FieldName))
                    MetaConstraint.Columns = string.IsNullOrWhiteSpace(MetaConstraint.Columns)
                        ? FieldName
                        : MetaConstraint.Columns + $";{FieldName}";

                if (ConstraintType == ConstraintType.ForeignKey)
                {
                    MetaForeignKey = MetaConstraint as DbMetaForeignKey;
                    if (!string.IsNullOrWhiteSpace(ForeignField))
                        MetaForeignKey.ForeignFields = string.IsNullOrWhiteSpace(MetaForeignKey.ForeignFields)
                            ? ForeignField
                            : MetaForeignKey.ForeignFields + $";{ForeignField}";
                }
            }
        }
        
        List<DbMetaConstraint> List = MetaTable.Constraints.OrderBy(x => x.ConstraintType == ConstraintType.PrimaryKey).ToList();
        MetaTable.Constraints.Clear();
        MetaTable.Constraints.AddRange(List);
    }
    /// <summary>
    /// Parses tracking context rows to construct and assign intercept automation trigger actions to entities.
    /// </summary>
    static void LoadTriggers(DbSchema Schema, DataTable tblSql)
    {
        if (tblSql == null)
            return;
 
        DbMetaTable MetaTable;
        DbMetaTrigger MetaTrigger;
        string SchemaName;
        string TableName;
        string TriggerName;
 
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            TableName = Row.AsString("TableName").Trim();

            if (!string.IsNullOrWhiteSpace(TableName))
            {
                MetaTable = FindOrAddTable(Schema, SchemaName, TableName);

                TriggerName = Row.AsString("TriggerName");
                MetaTrigger = new  DbMetaTrigger() {  Name = TriggerName, SchemaName = SchemaName };
                MetaTable.Triggers.Add(MetaTrigger);
                MetaTrigger.TableName = Row.AsString("TableName");    
                MetaTrigger.TriggerType = Row.AsString("TriggerType");  
            }
        }
    }
    /// <summary>
    /// Loads procedural action modules logic specifications rows directly into target storage execution routines.
    /// </summary>
    static void LoadProcedures(DbSchema Schema, DataTable tblSql)
    {
        if (tblSql == null)
            return;
        
        DbMetaProcedure MetaProcedure;
        string SchemaName;
        string ProcedureName;
 
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            ProcedureName = Row.AsString("ProcedureName");

            MetaProcedure = new DbMetaProcedure() { Name = ProcedureName, SchemaName = SchemaName };
            Schema.Procedures.Add(MetaProcedure);
            MetaProcedure.ProcedureType = Row.AsString("ProcedureType");
            MetaProcedure.SourceCode = Row.AsString("Definition");
        }
    }
    /// <summary>
    /// Populates numeric system sequence identity generation fields structure entries inside root context.
    /// </summary>
    static void LoadSequences(DbSchema Schema, DataTable tblSql)
    {
        if (tblSql == null)
            return;
        
        DbMetaSequence MetaSequence;
        string SchemaName;
        string SequenceName;
 
        foreach (DataRow Row in tblSql.Rows)
        {
            SchemaName = Row.AsString("SchemaName");
            SequenceName = Row.AsString("SequenceName");

            MetaSequence = new DbMetaSequence() { Name = SequenceName, SchemaName = SchemaName };
            Schema.Sequences.Add(MetaSequence);
            MetaSequence.CurrentValue = Row.AsInteger("CurrentValue");
            MetaSequence.InitialValue = Row.AsInteger("InitialValue");
            MetaSequence.IncrementBy = Row.AsInteger("IncrementBy");
        }
    }

    // ● static public methods
    /// <summary>
    /// Maps dynamic relational data record column types into core typed property descriptors.
    /// </summary>
    static public void LoadField(DataRow Row, DbMetaColumn MetaField)
    {
        MetaField.DataType        = Row.AsString("DataType");
        MetaField.DataSubType     = Row.AsString("DataSubType");
        MetaField.IsNullable      = Row.AsInteger("IsNullable") == 1;
        MetaField.SizeInChars     = Row.AsInteger("SizeInChars");
        MetaField.SizeInBytes     = Row.AsInteger("SizeInBytes");
        MetaField.Precision       = Row.AsInteger("DecimalPrecision");
        MetaField.Scale           = Row.AsInteger("DecimalScale");
        MetaField.DefaultValue    = Row.AsString("DefaultValue");
        MetaField.Expression      = Row.AsString("Expression");
        MetaField.OrdinalPosition = Row.AsInteger("OrdinalPosition");
    }
    /// <summary>
    /// Triggers discovery parsing queries pipeline steps sequentially to hydrate entire target database tracking context schema elements.
    /// </summary>
    static public void Load(DbSchema Schema)
    {
        Clear(Schema);
        string BaseResourcePath = GetSqlResourceBasePath(Schema.DbServerType);
        
        DataTable tblTables          = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "Tables.sql");
        DataTable tblTableFields     = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "TableFields.sql");
        DataTable tblViews           = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "Views.sql");
        DataTable tblViewFields      = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "ViewFields.sql");
        DataTable tblIndexes         = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "Indexes.sql");
        DataTable tblConstraints     = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "Constraints.sql");
        DataTable tblTriggers        = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "Triggers.sql");
        DataTable tblProcedures      = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "Procedures.sql");
        DataTable tblSequences       = ExecuteResourceSql(Schema.ConnectionInfo, BaseResourcePath, "Sequences.sql"); 
        
        LoadTables(Schema, tblTables);
        LoadTableFields(Schema, tblTableFields);
        LoadViews(Schema, tblViews);
        LoadViewFields(Schema, tblViewFields);
        LoadIndexes(Schema, tblIndexes);
        LoadConstraints(Schema, tblConstraints);
        LoadTriggers(Schema, tblTriggers);
        LoadProcedures(Schema, tblProcedures);
        LoadSequences(Schema, tblSequences);
    }
    /// <summary>
    /// Performs teardown operations to release resources associated with schema state loading tracking definitions.
    /// </summary>
    static public void UnLoad(DbSchema Schema)
    {
 
    }
    /// <summary>
    /// Forces complete clear cycles and re-runs internal discovery loader sequences to refresh relational structure maps.
    /// </summary>
    static public void ReLoad(DbSchema Schema)
    {
        UnLoad(Schema);
        Load(Schema);
    }
}