using DocumentFormat.OpenXml.Office2010.CustomUI;

namespace Tripous.Models;

/// <summary>
/// A table definition
/// </summary>
public class TableDef: BaseDef
{
    /// <summary>
    /// Constant
    /// </summary>
    public const string ITEM = "_ITEM_";
    /// <summary>
    /// Constant
    /// </summary>
    public const string LINES = "_LINES_";
    /// <summary>
    /// Constant
    /// </summary>
    public const string SUBLINES = "_SUBLINES_";
 
    string fAlias;
    private string fKeyField = "Id";
    private string fMasterField;
    private string fDetailField;
    private List<TableDef> fDetails;
    
    // ● construction  
    /// <summary>
    /// Constructor
    /// </summary>
    public TableDef()
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public TableDef(ModuleDef ModuleDef)
    {
        this.ModuleDef = ModuleDef;
    }

    // ● public  
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(this.Alias))
            Sys.Throw(Texts.GS($"E_{typeof(TableDef)}_AliasIsEmpty", $"{typeof(TableDef)} Alias is empty."));

        if (string.IsNullOrWhiteSpace(this.KeyField))
            Sys.Throw(Texts.GS($"E_{typeof(TableDef)}_PrimaryKeyFieldIsEmpty", $"{typeof(TableDef)} Primary KeyField is empty."));

        if (this.Fields == null || this.Fields.Count == 0)
            Sys.Throw(Texts.GS($"E_{typeof(TableDef)}_NoFieldsDefined", $"{typeof(TableDef)} Fields not defined."));
    }
    public string GetTopTableErrors()
    {
        StringBuilder SB = new();

        List<TableDef> Tables = new();

        void AddErrors(TableDef Table)
        {
            if (Tables.Contains(Table))
                SB.AppendLine($"Table '{Table.Name}' is already in the table tree.");
            
            if (string.IsNullOrWhiteSpace(Table.Name))
                SB.AppendLine($"Table '{Table.Name}' has no table name.");
            
            if (string.IsNullOrWhiteSpace(Table.KeyField))
                SB.AppendLine($"Table '{Table.Name}' has no key field.");

            /*
            if (Master != null)
            {
                if (string.IsNullOrWhiteSpace(Table.MasterField))
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no master field.");
                
                if (string.IsNullOrWhiteSpace(Table.DetailField))
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no detail field.");
                
                if (Master.FindColumn(Table.MasterField) == null)
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no matching master field.");
                
                if (Table.FindColumn(Table.DetailField) == null)
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no matching detail field.");
            }
            */

            if (Details.Count > 0)
            {
                foreach (var tblDetail in Details)
                    AddErrors(tblDetail);
            }
        }

        AddErrors(this);
            
        return SB.ToString();
    }
    public void CheckTopTableErrors()
    {
        string ErrorText = GetTopTableErrors();
        if (!string.IsNullOrWhiteSpace(ErrorText))
            throw new ApplicationException(ErrorText);
    }
    /// <summary>
    /// Updates references such as when an instance has references to other instances, e.g. tables of a module definition.
    /// </summary>
    public override void UpdateReferences()
    {
        foreach (SelectDef SelectDef in Stocks)
            SelectDef.Owner = this;
        
        foreach (FieldDef FieldDef in Fields)
            FieldDef.TableDef = this;

        foreach (TableDef TableDef in Details)
        {
            TableDef.Master = this;
            TableDef.UpdateReferences();
        }
    }
    
    // ● find 
    /// <summary>
    /// Searces the whole joined tree for a table by a Name or Alias and returns
    /// a JoinTableDescriptor, if any, else null.
    /// </summary>
    public TableDef FindAnyJoinTable(string NameOrAlias)
    {
        var Result = this.Joins.Find(item => item.Name.IsSameText(NameOrAlias) || item.Alias.IsSameText(NameOrAlias)); // base.Find(NameOrAlias);
        if (Result == null)
        {
            foreach (var JoinTable in this.Joins)
            {
                Result = JoinTable.FindAnyJoinTable(NameOrAlias);
                if (Result != null)
                    return Result;
            }
        }

        return Result;
    }
    /// <summary>
    /// Finds a join table descriptor by MasterKeyField, if any, else null.
    /// </summary>
    public TableDef FindJoinTableByMasterKeyField(string MasterKeyField)
    {
        return this.Joins.Find(item => item.MasterField.IsSameText(MasterKeyField));
    }
    /// <summary>
    /// Searces the whole joined tree for a join table descriptor by MasterKeyField
    /// and returns that table, if any, else null.
    /// </summary>
    public TableDef FindAnyJoinTableByMasterKeyField(string MasterKeyField)
    {
        var Result = FindJoinTableByMasterKeyField(MasterKeyField);
        if (Result == null)
        {
            foreach (var JoinTable in this.Joins)
            {
                Result = JoinTable.FindAnyJoinTableByMasterKeyField(MasterKeyField);
                if (Result != null)
                    return Result;
            }
        }

        return Result;
    }

    /// <summary>
    /// Finds and returns, if exists, a field that has NameOrAlias Name or Alias. 
    /// It searches this table descriptor and its joined tables in the full tree.
    /// Returns null if a field not found.
    /// </summary>
    public Tuple<TableDef, FieldDef> FindAnyField(string NameOrAlias)
    {
        FieldDef FieldDef = Fields.Find(item => item.Name.IsSameText(NameOrAlias) || item.Alias.IsSameText(NameOrAlias));
        if (FieldDef != null)
            return Tuple.Create(this, FieldDef);
        return FindAnyField(NameOrAlias, this.Joins);
    }
    /// <summary>
    /// Finds a field by Name or Alias by searching the whole tree of JoinTables tables.
    /// Returns null if a field not found.
    /// </summary>
    Tuple<TableDef, FieldDef> FindAnyField(string NameOrAlias, List<TableDef> JoinTables)
    {

        Tuple<TableDef, FieldDef> Result = null;
        FieldDef FieldDef = null;

        foreach (var JoinTable in JoinTables)
        {
            FieldDef = JoinTable.Fields.Find(item => item.Name.IsSameText(NameOrAlias) || item.Alias.IsSameText(NameOrAlias));
            if (FieldDef != null)
            {
                Result = Tuple.Create(JoinTable, FieldDef);
                break;
            }

            if (JoinTable.Joins != null)
            {
                Result = FindAnyField(NameOrAlias, JoinTable.Joins);
                if (Result != null)
                    break;
            }

        }

        return Result;
    }

    /// <summary>
    /// Finds a field title by searching the whole tree of fields.
    /// </summary>
    public string FindAnyFieldTitle(string NameOrAlias)
    {
        Tuple<TableDef, FieldDef> Pair = FindAnyField(NameOrAlias);
        FieldDef Field = Pair != null ? Pair.Item2 : null;
        return (Field == null) ? NameOrAlias : Field.Title;
    }

    // ● sql generation  
    public TableSqls BuildSql(BuildSqlFlags Flags)
    {
        TableSqls Statements = new TableSqls();

        bool GuidOid = BuildSqlFlags.GuidOids.In(Flags) || this.Fields.Find(item => item.Name.IsSameText(this.KeyField)).DataType == DataFieldType.String;
        bool OidModeIsBefore = !GuidOid && ((Flags & BuildSqlFlags.OidModeIsBefore) == BuildSqlFlags.OidModeIsBefore);

        List<string> InsertList = new List<string>();
        List<string> InsertParamsList = new List<string>();
        List<string> UpdateList = new List<string>();

        // string S = string.Join(", " + Environment.NewLine, InsertList.ToArray());

        string FieldName;

        foreach (var FieldDes in this.Fields)
        {
            if (FieldDes.IsNativeField && !FieldDes.IsNoInsertOrUpdate)
            {
                FieldName = FieldDes.Name;

                if (!Sys.IsSameText(FieldName, this.KeyField))
                {
                    InsertList.Add($"  {FieldName}");
                    InsertParamsList.Add($"  :{FieldName}");
                    UpdateList.Add($"  {FieldName} = :{FieldName}");
                }
                else if (GuidOid || OidModeIsBefore)
                {
                    InsertList.Add($"  {FieldName}");
                    InsertParamsList.Add($"  :{FieldName}");
                }
            }
        }

        string sInsertList       = SqlHelper.TransformToFieldList(InsertList);           // string.Join(", " + Environment.NewLine, InsertList.ToArray()).TrimEnd();
        string sInsertParamsList = SqlHelper.TransformToFieldList(InsertParamsList);     // string.Join(", " + Environment.NewLine, InsertParamsList.ToArray()).TrimEnd();
        string sUpdateList       = SqlHelper.TransformToFieldList(UpdateList);           // string.Join(", " + Environment.NewLine, UpdateList.ToArray()).TrimEnd();



        /* Insert */
        Statements.InsertRowSql = $@"insert into {this.Name} (
{sInsertList}
) values (
{sInsertParamsList}
)
";

        /* Update */
        Statements.UpdateRowSql = $@"update {this.Name} 
set 
{sUpdateList}
where
{this.KeyField} = :{this.KeyField}
";

        /* Delete */
        Statements.DeleteRowSql = $@"delete from {this.Name} where {this.KeyField} = :{this.KeyField}";



        /* RowSelect */
        SelectSql SS = BuildSqlSelect(Flags, false);
        SS.Where = $"  {this.Name}.{this.KeyField} = :{this.KeyField}"; 
        Statements.SelectRowSql = SS.Text;


        /* Browse */
        SS = BuildSqlSelect(Flags, true);

        // it is a detail table 
        bool IsDetailTable = IsDetail
                            && !string.IsNullOrWhiteSpace(this.MasterField)
                            && !string.IsNullOrWhiteSpace(this.DetailField);

        if (IsDetailTable)
        {
            SS.Where = $"{this.Alias}.{this.DetailField} = :{Sys.MASTER_KEY_FIELD_NAME}";
        }

        Statements.SelectSql = SS.Text;

        return Statements;
    }
    SelectSql BuildSqlSelect(BuildSqlFlags Flags, bool IsBrowserSelect)
    {
        SelectSql SelectSql = new SelectSql();

        // native fields
        string S = string.Empty;
        List<string> FieldList = new List<string>();

        foreach (var FieldDes in this.Fields)
        {
            if (FieldDes.IsNativeField) //  && !FieldDes.IsNoInsertOrUpdate -- NO, auto-inc fields are Native
            {
                if (IsBrowserSelect)
                {
                    if (FieldDes.DataType.IsBlob() && ((Flags & BuildSqlFlags.IncludeBlobFields) == BuildSqlFlags.None))
                        continue;
                }

                string FieldName = $"  {this.Name}.{FieldDes.Name}".PadRight(SqlHelper.StatementDefaultSpaces, ' ');
                FieldName = $"{FieldName} as {FieldDes.Name}";
                FieldList.Add(FieldName);
            }
        }

        // add it to SELECT
        string sFieldList = SqlHelper.TransformToFieldList(FieldList);
        SelectSql.Select = sFieldList;

        // native from
        SelectSql.From = $"{this.Name} {this.Alias} " + Environment.NewLine;

        // joined tables and fields
        List<string> JoinTableNamesList = new();
        foreach (var JoinTableDes in this.Joins)
            BuildSqlAddJoin(JoinTableNamesList, SelectSql, this.Alias, JoinTableDes);

        // remove the last comma
        S = SelectSql.Select.TrimEnd();
        if ((S.Length > 1) && (S[S.Length - 1] == ','))
            S = S.Remove(S.Length - 1, 1);

        SelectSql.Select = S;
        SelectSql.From = SelectSql.From.TrimEnd();

        return SelectSql;
    }
    void BuildSqlAddJoin(List<string> JoinTableNamesList, SelectSql SelectSql, string MasterAlias, TableDef JoinTableDes)
    {
        string JoinTableName = SqlHelper.FormatTableNameAlias(JoinTableDes.Name, JoinTableDes.Alias);

        if (JoinTableNamesList.IndexOf(JoinTableName) == -1)
        {
            JoinTableNamesList.Add(JoinTableName);
            SelectSql.From += $"    left join {JoinTableName} on {JoinTableDes.Alias}.{JoinTableDes.KeyField} = {MasterAlias}.{JoinTableDes.MasterField} " + Environment.NewLine;
        }

        // joined field list
        List<string> FieldList = new List<string>();
        foreach (var JoinFieldDes in JoinTableDes.Fields)
        {
            if (!Sys.IsSameText(JoinFieldDes.Name, JoinTableDes.KeyField))
            {
                string FieldName = $"  {JoinTableDes.Alias}.{JoinFieldDes.Name}".PadRight(SqlHelper.StatementDefaultSpaces, ' '); 
                FieldName = $"{FieldName} as {JoinFieldDes.Alias}";
                FieldList.Add(FieldName);
            }
        }

        // add it to SELECT
        string sFieldList = SqlHelper.TransformToFieldList(FieldList);
        SelectSql.Select = SelectSql.Select.TrimEnd() + ", " + Environment.NewLine;
        SelectSql.Select += sFieldList;

        // joined tables to this join table
        foreach (var JoinTableDescriptor in JoinTableDes.Joins)
            BuildSqlAddJoin(JoinTableNamesList, SelectSql, JoinTableDes.Alias, JoinTableDescriptor);
    }
 
    /// <summary>
    /// Updates this descriptor information using a specified DataTable schema.
    /// </summary>
    public void UpdateFrom(DataTable Table)
    {
        FieldFlags Flags;
        string Title;

        foreach (DataColumn Field in Table.Columns)
        {
            var FieldDes = Fields.Find(item => item.Name.IsSameText(Field.ColumnName));

            if (FieldDes == null)
            {
                Flags = FieldFlags.None;
                Title = Texts.GS(Field.ColumnName);
                if (!Field.ColumnName.EndsWithText("Id"))
                    Flags |= FieldFlags.Visible;

                if (Simple.IsString(Field.DataType) || Simple.IsDateTime(Field.DataType))
                    Flags |= FieldFlags.Searchable;

                Fields.Add(new FieldDef()
                {
                    Name = Field.ColumnName,
                    DataType = DataFieldTypeHelper.DataFieldTypeOf(Field.DataType),
                    MaxLength = Field.MaxLength,
                    TitleKey = Title,
                    Flags = Flags
                });
            }
            else
            {

                if (FieldDes.DataType.IsDateTime() && Field.DataType == typeof(DateTime))
                {
                    // let FieldDes.DataType keep its original value
                }
                else if (FieldDes.DataType == DataFieldType.Boolean && (Field.DataType == typeof(int) || Field.DataType == typeof(System.Int64)))
                {
                    // let FieldDes.DataType keep its original value
                }
                else if (FieldDes.DataType != DataFieldTypeHelper.DataFieldTypeOf(Field.DataType))
                {
                    FieldDes.DataType = DataFieldTypeHelper.DataFieldTypeOf(Field.DataType);
                }

                if (FieldDes.DataType == DataFieldType.String && (Field.MaxLength != -1) && (FieldDes.MaxLength != Field.MaxLength))
                    FieldDes.MaxLength = Field.MaxLength;
            }

        }
    }

    // ● create DataTable  
    /// <summary>
    /// Creates a DataTable based on a descriptor. 
    /// <para>Creates the look-up tables too if a flag is specified.</para>
    /// <para>The table may added to a list using a specified delegate.</para>
    /// </summary>
    public void CreateDescriptorTable(SqlStore Store, Action<MemTable> AddTableFunc)
    {
        MemTable Table = new MemTable() { TableName = this.Name };
        AddTableFunc(Table);
        Table.ExtendedProperties["Descriptor"] = this;

        Table.KeyFields = [this.KeyField];
        Table.MasterFields = [this.MasterField];
        Table.DetailFields = [this.DetailField];
        DataColumn Column;

        // native fields and lookups
        foreach (var FieldDes in this.Fields)
        {
            Column = new DataColumn(FieldDes.Name);
            Column.ExtendedProperties["Descriptor"] = FieldDes;
            Column.DataType = FieldDes.DataType.GetNetType();
            if (Sys.IsSameText(this.KeyField, FieldDes.Name) && (FieldDes.DataType == DataFieldType.Integer))
            {
                Column.AutoIncrement = true;
                Column.AutoIncrementSeed = -1;
                Column.AutoIncrementStep = -1;
            }
            if (Column.DataType == typeof(System.String))
                Column.MaxLength = FieldDes.MaxLength;
            Column.Caption = FieldDes.Title;

            SetupDefaultValue(Store, Column, FieldDes);

            Table.Columns.Add(Column); 

            // joined table to TableDescriptor on this FieldDes
            TableDef JoinTableDes = this.FindAnyJoinTableByMasterKeyField(FieldDes.Name);
            if (JoinTableDes != null)
                CreateDescriptorTables_AddJoinTableFields(JoinTableDes, Table);
        }
    }
    /// <summary>
    /// Sets up the <see cref="DataColumn.DefaultValue"/> of a specified column based on a specified field descriptor settings.
    /// </summary>
    void SetupDefaultValue(SqlStore Store, DataColumn Column, FieldDef FieldDes)
    {
        if ((FieldDes.DefaultValue != null) && !Sys.IsSameText(Sys.NULL, FieldDes.DefaultValue))
        {
            try
            {
                if (Sys.IsSameText("EmptyString", FieldDes.DefaultValue))
                    Column.DefaultValue = string.Empty;
                else if (!Db.StandardDefaultValues.ContainsText(FieldDes.DefaultValue))
                {
                    SimpleType ColumnDataType = Simple.SimpleTypeOf(Column.DataType);

                    if (Simple.IsInteger(ColumnDataType))
                        Column.DefaultValue = int.Parse(FieldDes.DefaultValue);
                    else if (Simple.IsFloat(ColumnDataType))
                        Column.DefaultValue = double.Parse(FieldDes.DefaultValue);
                    else if (Simple.IsString(ColumnDataType))
                        Column.DefaultValue = FieldDes.DefaultValue;
                }
            }
            catch
            {
            }
        }
    }
 
    /// <summary>
    /// Adds join fields to the Table
    /// </summary>
    void CreateDescriptorTables_AddJoinTableFields(TableDef JoinTable, MemTable Table)
    {
        DataColumn Column;
        foreach (var JoinFieldDes in JoinTable.Fields)
        {
            if (!Sys.IsSameText(JoinTable.KeyField, JoinFieldDes.Name))
            {
                Column = new DataColumn(JoinFieldDes.Alias);
                Column.ExtendedProperties["Descriptor"] = JoinFieldDes;
                Column.DataType = JoinFieldDes.DataType.GetNetType();
                Column.MaxLength = JoinFieldDes.MaxLength;
                Column.Caption = JoinFieldDes.Title;

                Table.Columns.Add(Column);

                // joined table to JoinTable on this JoinFieldDes
                var JoinTableDes2 = JoinTable.FindAnyJoinTableByMasterKeyField(JoinFieldDes.Name);
                if (JoinTableDes2 != null)
                    CreateDescriptorTables_AddJoinTableFields(JoinTableDes2, Table);
            }
        }
    }
 
    // ● display labels  
    /// <summary>
    /// Setups column titles for Table columns using the DisplayLabes dictionary and the TableDes TableDescriptor.
    /// </summary>
    public void SetupFieldsDisplayLabelsFor(DataTable Table, Dictionary<string, string> DisplayLabels)
    {
        foreach (DataColumn Field in Table.Columns)
        {
            // if Column.Caption is not defined in some way
            if (string.IsNullOrWhiteSpace(Field.Caption) || Sys.IsSameText(Field.ColumnName, Field.Caption))
            {
                // first look to the DisplayLabels 
                if ((DisplayLabels.ContainsKey(Field.ColumnName)) && !string.IsNullOrWhiteSpace(DisplayLabels[Field.ColumnName]))
                {
                    Field.Caption = DisplayLabels[Field.ColumnName];
                }
                // and then look to ANY field (joins included) of the TableDes
                else if (!string.IsNullOrWhiteSpace(this.FindAnyFieldTitle(Field.ColumnName)))
                {
                    Field.Caption = this.FindAnyFieldTitle(Field.ColumnName);
                }
            }
        }
    }
 
    // ● fields 
    /// <summary>
    /// Adds and returns a field.
    /// </summary>
    public FieldDef AddField(string Name, DataFieldType DataType, string TitleKey, FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = Fields.Find(item => item.Name.IsSameText(Name));

        if (Result == null)
        {
            Result = new FieldDef(this) 
            { 
                Name = Name,
                TitleKey = !string.IsNullOrWhiteSpace(TitleKey)? TitleKey: Name,
                DataType = DataType, 
                Flags = Flags 
            };

            Fields.Add(Result);
        }

        return Result;
    }

    /// <summary>
    /// Adds and returns an Id field.
    /// </summary>
    public FieldDef AddId(string Name, DataFieldType DataType, int MaxLength = 40)
    {
        if (!DataType.In(DataFieldType.String | DataFieldType.Integer))
            Sys.Throw($"DataType not supported for a table Primary Key. {DataType}");

        var Result = AddField(Name, DataType, "");
        FieldFlags.Visible.Remove(Result.Flags);
        if (DataType == DataFieldType.String)
            Result.MaxLength = MaxLength;
        return Result;
    }
    /// <summary>
    /// Adds and returns an Id field based on settings on <see cref="SysConfig.OidDataType"/> and <see cref="SysConfig.OidSize"/>.
    /// </summary>
    public FieldDef AddId(string Name = "Id") => AddId(Name, SysConfig.OidDataType, SysConfig.OidSize);
    /// <summary>
    /// Adds and returns a string Id field
    /// </summary>
    public FieldDef AddStringId(string Name = "Id", int MaxLength = 40) => AddId(Name, DataFieldType.String, MaxLength);
    /// <summary>
    /// Adds and returns an integer Id field
    /// </summary>
    public FieldDef AddIntegerId(string Name = "Id") => AddId(Name, DataFieldType.Integer, 0);

    /// <summary>
    /// Adds and returns a string field.
    /// </summary>
    public FieldDef Add(string Name, int MaxLength, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.String, TitleKey, Flags);
        Result.MaxLength = MaxLength;
        return Result;
    }

    /// <summary>
    /// Adds and returns a string field.
    /// </summary>
    public FieldDef AddString(string Name, int MaxLength = 96, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.String, TitleKey, Flags);
        Result.MaxLength = MaxLength;
        return Result;
    }
    /// <summary>
    /// Adds and returns an integer field.
    /// </summary>
    public FieldDef AddInteger(string Name, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.Integer, TitleKey, Flags); 
        return Result;
    }
    /// <summary>
    /// Adds and returns an double field.
    /// </summary>
    public FieldDef AddDouble(string Name, int Decimals = 4, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.Double, TitleKey, Flags);
        Result.Decimals = Decimals;
        return Result;
    }
    /// <summary>
    /// Adds and returns an decimal field.
    /// </summary>
    public FieldDef AddDecimal(string Name, int Decimals = 4, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.Decimal, TitleKey, Flags);
        Result.Decimals = Decimals;
        return Result;
    }
    /// <summary>
    /// Adds and returns an date field.
    /// </summary>
    public FieldDef AddDate(string Name, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.Date, TitleKey, Flags);
        return Result;
    }
    /// <summary>
    /// Adds and returns an date-time field.
    /// </summary>
    public FieldDef AddDateTime(string Name, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.DateTime, TitleKey, Flags);
        return Result;
    }
    /// <summary>
    /// Adds and returns an integer-boolean field.
    /// </summary>
    public FieldDef AddBoolean(string Name, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.Boolean, TitleKey, Flags | FieldFlags.Boolean);
        return Result;
    }
    /// <summary>
    /// Adds and returns a blob field.
    /// </summary>
    public FieldDef AddBlob(string Name, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
    {
        FieldDef Result = AddField(Name, DataFieldType.Blob, TitleKey, Flags);
        return Result;
    }
    /// <summary>
    /// Adds and returns a text blob field.
    /// </summary>
    public FieldDef AddTextBlob(string Name, string TitleKey = "", FieldFlags Flags = FieldFlags.Memo)
    {
        FieldDef Result = AddField(Name, DataFieldType.TextBlob, TitleKey, Flags);
        return Result;
    }
 
    public TableDef AddDetail(string DetailTableName, string MasterField, string DetailField)
    {
        TableDef Result = new();
        Result.Name = DetailTableName;
        Result.MasterField = MasterField;
        Result.DetailField = DetailField;
        Result.IsDetail = true;
        this.Details.Add(Result);
        return Result;
    }
    
    /// <summary>
    /// Adds a table join in this table and returns the joined table.
    /// <para>If this is CUSTOMER table then the COUNTRY table can be joined as</para>
    /// <para>TableName: COUNTRY</para>
    /// <para>Alias: emtpy string or null or an alias</para>
    /// <para>OwnKeyField: COUNTRY_ID</para>
    /// <para>The above settings produce the following:</para>
    /// <para><c>left join COUNTRY on COUNTRY.ID = CUSTOMER.COUNTRY_ID</c></para>
    /// </summary>
    /// <param name="OwnKeyField">A field that belongs to this table. Is used in the join SQL statement</param>
    /// <param name="ForeignTable">The name of the table to join, e.g. COUNTRY to CUSTOMER</param>
    /// <param name="ForeignAlias">The alias of the table to join, e.g. <c>co</c></param>
    /// <param name="ForeignPrimaryKey">The primary key field name of the table to join, e.g. <c>Id</c></param>
    public TableDef AddJoin(string OwnKeyField, string ForeignTable, string ForeignAlias = "", string ForeignPrimaryKey = "Id")
    {
        TableDef Result = new TableDef();
        Result.Name = ForeignTable;
        Result.Alias = ForeignAlias;
        Result.KeyField = ForeignPrimaryKey;
        Result.MasterField = OwnKeyField;
        Joins.Add(Result);
        return Result;
    }

    // ● properties */
    /// <summary>
    /// The master definition this instance belongs to.
    /// </summary>
    [JsonIgnore]
    public ModuleDef ModuleDef { get; set; }
    [JsonIgnore]
    public TableDef Master { get; set; }
 
    /// <summary>
    /// An alias of this table
    /// </summary>
    public string Alias
    {
        get => !string.IsNullOrWhiteSpace(fAlias)? fAlias: Name;
        set { if (fAlias != value) { fAlias = value; NotifyPropertyChanged(nameof(Alias)); } }
    }
 
    /// <summary>
    /// Gets or sets the name of the primary key field of this table.
    /// </summary>
    public string KeyField
    {
        get => fKeyField;
        set { if (fKeyField != value) { fKeyField = value; NotifyPropertyChanged(nameof(KeyField)); } }
    }
    /// <summary>
    /// Gets or sets the field name of a field belonging to a master table.
    /// <para>Used when this table is a detail table in a master-detail relation or when this is a join table.</para>
    /// </summary>
    public string MasterField
    {
        get => fMasterField;
        set { if (fMasterField != value) { fMasterField = value; NotifyPropertyChanged(nameof(MasterField)); } }
    }
    /// <summary>
    /// Gets or sets the detail key field. A field that belongs to this table and mathes the <see cref="MasterTableName"/> primary key field.
    /// <para>It is used when this table is a detail table in a master-detail relation.</para>
    /// </summary>
    public string DetailField
    {
        get => fDetailField != null ? fDetailField : KeyField;
        set { if (fDetailField != value) { fDetailField = value; NotifyPropertyChanged(nameof(DetailField)); } }
    }

    public bool IsDetail { get; set; }

    /// <summary>
    /// The fields of this table
    /// </summary>
    public List<FieldDef> Fields { get; set; } = new List<FieldDef>();
    /// <summary>
    /// The list of join tables. 
    /// </summary>
    public List<TableDef> Joins { get; set; } = new List<TableDef>();
    /// <summary>
    /// The main table (Item) is selected as 
    /// <para>  <c>select * from TABLE_NAME where ID = :ID</c></para>
    /// <para>
    /// If the table contains foreign keys, for instance CUSTOMER_ID etc, then those foreign tables are NOT joined. 
    /// The programmer who designs the UI just creates a Locator where needed.
    /// </para>
    /// <para>
    /// But there is always the need to have data from those foreign tables in many situations, i.e. in reports.
    /// </para>
    /// <para>
    /// StockTables are used for that. StockTables are selected each time after the select of the main table (Item)          
    /// </para>
    /// </summary>
    public List<SelectDef> Stocks { get; set; } = new List<SelectDef>();
    public List<TableDef> Details
    {
        get => fDetails;
        set { if (fDetails != value) { fDetails = value; NotifyPropertyChanged(nameof(Details)); } }
    }

}