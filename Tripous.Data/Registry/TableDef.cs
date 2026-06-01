/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

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
    string fKeyField = "Id";
    string fMasterField;
    string fDetailField;
    bool fIsDetail;
    bool fIsOneToOne;
    bool fIsUiVisible = true;

    DefList<FieldDef> fFields;
    DefList<TableDef> fJoins;
    DefList<SelectDef> fStocks;
    DefList<TableDef> fDetails;
    List<string> fFieldGroups;
    
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
            
            if (Table.Master != null)
            {
                if (string.IsNullOrWhiteSpace(Table.MasterField))
                    SB.AppendLine($"Detail Table '{Table.Name}' has no master field.");
                
                if (string.IsNullOrWhiteSpace(Table.DetailField))
                    SB.AppendLine($"Detail Table '{Table.Name}' has no detail field.");
                
                if (Table.Master.FindField(Table.MasterField) == null)
                    SB.AppendLine($"Detail Table '{Table.Name}' has no matching master field.");
                
                if (Table.FindField(Table.DetailField) == null)
                    SB.AppendLine($"Detail Table '{Table.Name}' has no matching detail field.");
            }

            if (Table.Details.Count > 0)
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
            throw new TripousDataException(ErrorText);
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
        
        if (HasStocks)
        {
            foreach (SelectDef Stock in Stocks)
            {
                Stock.Owner = this;
                Stock.UpdateReferences();
            }
        }

        if (HasDetails)
        {
            foreach (TableDef tblDetail in Details)
            {
                tblDetail.Master = this;
                tblDetail.UpdateReferences();
            }
        }

        if (HasJoins)
        {
            foreach (TableDef tblJoin in Joins)
            {
                tblJoin.Owner = this;
                tblJoin.UpdateReferences();
            }
        }

    }
 
    // ● find 
    /// <summary>
    /// Searces the whole joined tree for a table by a Name or Alias and returns
    /// a JoinTableDescriptor, if any, else null.
    /// </summary>
    public TableDef FindAnyJoinTable(string NameOrAlias)
    {
        var Result = this.Joins.FirstOrDefault(x => x.Name.IsSameText(NameOrAlias) || x.Alias.IsSameText(NameOrAlias));  
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
        return this.Joins.FirstOrDefault(item => item.MasterField.IsSameText(MasterKeyField));
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
        FieldDef FieldDef = Fields.FirstOrDefault(item => item.Name.IsSameText(NameOrAlias) || item.Alias.IsSameText(NameOrAlias));
        if (FieldDef != null)
            return Tuple.Create(this, FieldDef);
        return FindAnyField(NameOrAlias, this.Joins.ToList());
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
            FieldDef = JoinTable.Fields.FirstOrDefault(item => item.Name.IsSameText(NameOrAlias) || item.Alias.IsSameText(NameOrAlias));
            if (FieldDef != null)
            {
                Result = Tuple.Create(JoinTable, FieldDef);
                break;
            }

            if (JoinTable.Joins != null)
            {
                Result = FindAnyField(NameOrAlias, JoinTable.Joins.ToList());
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

        FieldDef KeyFieldDef = this.Fields.FirstOrDefault(item => item.Name.IsSameText(this.KeyField));
        bool GuidOid = BuildSqlFlags.GuidOids.In(Flags) || (KeyFieldDef != null && KeyFieldDef.DataType == DataFieldType.String);
        bool OidModeIsBefore = !GuidOid && ((Flags & BuildSqlFlags.OidModeIsBefore) == BuildSqlFlags.OidModeIsBefore);

        List<string> InsertList = new List<string>();
        List<string> InsertParamsList = new List<string>();
        List<string> UpdateList = new List<string>();

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

        foreach (DataColumn Column in Table.Columns)
        {
            FieldDef FieldDef = Fields.FirstOrDefault(item => item.Name.IsSameText(Column.ColumnName));

            // ● missing field
            if (FieldDef == null)
            {
                FieldDef = new FieldDef();
                FieldDef.Name = Column.ColumnName;
                FieldDef.DataType = DataFieldTypeHelper.GetDataFieldType(Column.DataType);
                
                Flags = !Db.Settings.IdFieldsVisible && Column.ColumnName.EndsWithText("Id")? FieldFlags.Hidden: FieldFlags.None;
                if (Simple.IsString(Column.DataType) || Simple.IsDateTime(Column.DataType))
                    Flags |= FieldFlags.Searchable;
                
                FieldDef.Flags = Flags;
                Fields.Add(FieldDef);
            }
            
            // ● DataType
            if (FieldDef.DataType.IsDateTime() && Column.DataType == typeof(DateTime))
            {
                // let FieldDef.DataType keep its original value
            }
            else if (FieldDef.DataType == DataFieldType.Boolean && (Column.DataType == typeof(int) || Column.DataType == typeof(System.Int64)))
            {
                // let FieldDef.DataType keep its original value
            }
            else if (FieldDef.DataType != DataFieldTypeHelper.GetDataFieldType(Column.DataType))
            {
                FieldDef.DataType = DataFieldTypeHelper.GetDataFieldType(Column.DataType);
            }

            // ● MaxLength
            if (FieldDef.DataType == DataFieldType.String && FieldDef.MaxLength != Column.MaxLength)
                FieldDef.MaxLength = Column.MaxLength;

            // ● TitleKey
            if (FieldDef.IsTitleKeyEmpty && !FieldDef.Name.IsSameText(Column.Caption))
                FieldDef.TitleKey = Texts.GS(Column.Caption);

            // ● IsNullable
            FieldDef.IsNullable = Column.AllowDBNull;
            
            // ● DefaultValue
            if (!Sys.IsNull(Column.DefaultValue) && string.IsNullOrWhiteSpace(FieldDef.DefaultValue))
                FieldDef.DefaultValue = Column.DefaultValue.ToString();

            // ● Expression
            if (!string.IsNullOrWhiteSpace(Column.Expression) && string.IsNullOrWhiteSpace(FieldDef.Expression))
                FieldDef.Expression = Column.Expression;

        }
    }

    // ● create DataTable  
    /// <summary>
    /// Creates a DataTable based on a descriptor. 
    /// <para>Creates the look-up tables too if a flag is specified.</para>
    /// <para>The table may added to a list using a specified delegate.</para>
    /// </summary>
    public MemTable CreateDescriptorTable(SqlStore Store)  
    {
        MemTable Table = new MemTable() { TableName = this.Name };
        //AddTableFunc(Table);
        Table.ExtendedProperties["Descriptor"] = this;

        Table.KeyFields = [this.KeyField];
        Table.MasterFields = [this.MasterField];
        Table.DetailFields = [this.DetailField];
        DataColumn Column;

        // native fields and lookups
        foreach (var FieldDef in this.Fields)
        {
            Column = new DataColumn(FieldDef.Name);
            Column.ExtendedProperties["Descriptor"] = FieldDef;
            Column.DataType = FieldDef.DataType.GetNetType();
            if (Sys.IsSameText(this.KeyField, FieldDef.Name) && (FieldDef.DataType == DataFieldType.Integer))
            {
                Column.AutoIncrement = true;
                Column.AutoIncrementSeed = -1;
                Column.AutoIncrementStep = -1;
            }
            if (Column.DataType == typeof(System.String))
                Column.MaxLength = FieldDef.MaxLength;
            Column.Caption = FieldDef.Title;

            SetupDefaultValue(Store, Column, FieldDef);

            Table.Columns.Add(Column); 

            // joined table to TableDescriptor on this FieldDes
            TableDef JoinTableDef = this.FindAnyJoinTableByMasterKeyField(FieldDef.Name);
            if (JoinTableDef != null)
                CreateDescriptorTables_AddJoinTableFields(JoinTableDef, Table);
        }

        return Table;
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
    public bool FieldExists(string FieldName) => FindField(FieldName) != null;
    /// <summary>
    /// Finds a field by name, if any, else null.
    /// </summary>
    public FieldDef FindField(string FieldName) => Fields.FirstOrDefault(x => FieldName.IsSameText(x.Name));
    /// <summary>
    /// Gets a field by name, if any, else exception.
    /// </summary>
    public FieldDef GetField(string FieldName)
    {
        FieldDef Result = FindField(FieldName);
        if (Result == null)
            throw new TripousDataException($"{nameof(FieldDef)} not found: {FieldName})");
        return Result;
    }

    /// <summary>
    /// Adds and returns a field.
    /// </summary>
    public FieldDef AddField(string Name, DataFieldType DataType, string Group = null, string TitleKey = null, string Alias = null, int MaxLength = -1, int Decimals = -1, string LookupSource = null, string Locator = null, FieldFlags Flags = FieldFlags.None)
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new TripousArgumentNullException(nameof(Name));
        if (FieldExists(Name))
            throw new TripousException($"{nameof(FieldDef)} '{Name}' is already registered in table {nameof(TableDef)} {this.Name}.");
        if (DataType == DataFieldType.None)
            throw new TripousException($"{nameof(FieldDef)} '{Name}' has no data-type in table {nameof(TableDef)} {this.Name}.");
        
        FieldDef Result = new();
        Result.Name = Name;
        Result.DataType = DataType;
        Result.Group = Group;
        Result.TitleKey = TitleKey;
        Result.Alias = Alias;
        Result.MaxLength = DataType == DataFieldType.String ? MaxLength : -1;
        Result.Decimals = Result.IsFloat ? Decimals : -1;
        Result.LookupSource = LookupSource;
        Result.Locator = Locator;
        Result.Flags = Flags;
        Fields.Add(Result);

        return Result;
    }
    
    // ● fields - Ids
    /// <summary>
    /// Adds and returns an Id field.
    /// </summary>
    public FieldDef AddId(string Name, DataFieldType DataType, FieldFlags Flags = FieldFlags.Required | FieldFlags.Hidden, int MaxLength = 40)
    {
        if (!DataType.In(DataFieldType.String | DataFieldType.Integer))
            throw new TripousDataException($"DataType not supported for a table Primary Key. {DataType}");

        Flags |= FieldFlags.ReadOnlyUI;
        var Result = AddField(Name, DataType, Group: Sys.GENERAL, MaxLength: MaxLength, Flags: Flags);
        return Result;
    }
    /// <summary>
    /// Adds and returns an Id field based on settings on <see cref="DbConfig.OidDataType"/> and <see cref="DbConfig.OidSize"/>.
    /// </summary>
    public FieldDef AddId(string Name = "Id", FieldFlags Flags = FieldFlags.Required | FieldFlags.Hidden) => AddId(Name, DbConfig.OidDataType, Flags: Flags, MaxLength: DbConfig.OidSize);
    /// <summary>
    /// Adds and returns a string Id field
    /// </summary>
    public FieldDef AddStringId(string Name = "Id", FieldFlags Flags = FieldFlags.Required | FieldFlags.Hidden, int MaxLength = 40) => AddId(Name, DataFieldType.String, Flags: Flags, MaxLength: MaxLength);
    /// <summary>
    /// Adds and returns an integer Id field
    /// </summary>
    public FieldDef AddIntegerId(string Name = "Id", FieldFlags Flags = FieldFlags.Required | FieldFlags.Hidden) => AddId(Name, DataFieldType.Integer,  Flags: Flags);

    // ● fields - Lookup Ids
    /// <summary>
    /// Adds a fields, such as <c>CountryId</c> which needs a <see cref="LookupDef"/> in order to be displayed correctly in the Ui.
    /// <para>The <see cref="LookupDef"/> should be registered in the registry.</para>
    /// </summary>
    public FieldDef AddLookupId(string Name, DataFieldType DataType, string LookupSource, string Group = null, string TitleKey = null, FieldFlags Flags = FieldFlags.Hidden)
    {
        if (string.IsNullOrWhiteSpace(TitleKey))
            TitleKey = LookupSource;
        
        FieldDef Result = AddField(Name, DataType, Group: Group, TitleKey: TitleKey, Flags: Flags);
        Result.LookupSource = LookupSource;
        return Result;
    }
    /// <summary>
    /// Adds a fields, such as <c>CountryId</c> which needs a <see cref="LookupDef"/> in order to be displayed correctly in the Ui.
    /// <para>The <see cref="LookupDef"/> should be registered in the registry.</para>
    /// </summary>
    public FieldDef AddStringLookupId(string Name, string LookupSource, string Group = null, string TitleKey = null, FieldFlags Flags = FieldFlags.Hidden)
        => AddLookupId(Name, DataFieldType.String, LookupSource, Group: Group, TitleKey: TitleKey, Flags: Flags);
    /// <summary>
    /// Adds a fields, such as <c>CountryId</c> which needs a <see cref="LookupDef"/> in order to be displayed correctly in the Ui.
    /// <para>The <see cref="LookupDef"/> should be registered in the registry.</para>
    /// </summary>
    public FieldDef AddIntegerLookupId(string Name, string LookupSource, string Group = null, string TitleKey = null, FieldFlags Flags = FieldFlags.Hidden)
        => AddLookupId(Name, DataFieldType.Integer, LookupSource, Group: Group, TitleKey: TitleKey, Flags: Flags);
    /// <summary>
    /// Adds a fields, such as <c>AggregateId</c> which needs a <see cref="LookupDef"/> of an enum type, such as the <see cref="AggregateType"/>,
    /// in order to be displayed correctly in the Ui.
    /// <para><b>NOTE</b>: This method creates and registers the required <see cref="LookupDef"/> to the registry.</para>
    /// </summary>
    public FieldDef AddEnumLookupId(string Name, string LookupSource, Type EnumType, string Group = null, bool UseNullItem = false, string TitleKey = null, FieldFlags Flags = FieldFlags.Hidden)
    {
        if (!EnumType.IsEnum)
            throw new TripousDataException($"Type {EnumType.FullName} is not an enum type");
        
        FieldDef Result = AddLookupId(Name, DataFieldType.Integer, LookupSource, Group: Group, TitleKey: TitleKey, Flags: Flags);
        if (!DataRegistry.Lookups.Contains(LookupSource))
            DataRegistry.AddLookupSource(LookupSource, EnumType, UseNullItem);
        
        return Result;
    }

    // ● fields - Strings
    /// <summary>
    /// Adds and returns a string field.
    /// </summary>
    public FieldDef Add(string Name, int MaxLength, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        => AddField(Name, DataFieldType.String, MaxLength: MaxLength, Group: Group, TitleKey: TitleKey, Flags: Flags);
    /// <summary>
    /// Adds and returns a string field.
    /// </summary>
    public FieldDef AddString(string Name, int MaxLength = 96, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        => AddField(Name, DataFieldType.String, MaxLength: MaxLength, Group: Group, TitleKey: TitleKey, Flags: Flags);
    
    // ● fields - Other types
    /// <summary>
    /// Adds and returns an integer field.
    /// </summary>
    public FieldDef AddInteger(string Name, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        => AddField(Name, DataFieldType.Integer, Group: Group, TitleKey: TitleKey, Flags: Flags); 
    /// <summary>
    /// Adds and returns an double field.
    /// </summary>
    public FieldDef AddDouble(string Name, string Group = null, int Decimals = 4, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        => AddField(Name, DataFieldType.Double, Group: Group, Decimals: Decimals, TitleKey: TitleKey, Flags: Flags); 
    /// <summary>
    /// Adds and returns an decimal field.
    /// </summary>
    public FieldDef AddDecimal(string Name, string Group = null, int Decimals = 4, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        => AddField(Name, DataFieldType.Decimal, Group: Group, Decimals: Decimals, TitleKey: TitleKey, Flags: Flags); 
    /// <summary>
    /// Adds and returns an date field.
    /// </summary>
    public FieldDef AddDate(string Name, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        =>  AddField(Name, DataFieldType.Date, Group: Group, TitleKey: TitleKey, Flags: Flags); 
    /// <summary>
    /// Adds and returns an date-time field.
    /// </summary>
    public FieldDef AddDateTime(string Name, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        =>  AddField(Name, DataFieldType.DateTime, Group: Group, TitleKey: TitleKey, Flags: Flags); 
    /// <summary>
    /// Adds and returns an integer-boolean field.
    /// </summary>
    public FieldDef AddBoolean(string Name, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        =>  AddField(Name, DataFieldType.Boolean, Group: Group, TitleKey: TitleKey, Flags: Flags| FieldFlags.Boolean); 
    /// <summary>
    /// Adds and returns a blob field.
    /// </summary>
    public FieldDef AddBlob(string Name, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        =>  AddField(Name, DataFieldType.Blob, Group: Group, TitleKey: TitleKey, Flags: Flags); 
    /// <summary>
    /// Adds and returns a text blob field.
    /// </summary>
    public FieldDef AddTextBlob(string Name, string Group = null, string TitleKey = "", FieldFlags Flags = FieldFlags.None)
        =>  AddField(Name, DataFieldType.TextBlob, Group: Group, TitleKey: TitleKey, Flags: Flags | FieldFlags.Memo); 
 
    // ● miscs
    /// <summary>
    /// Adds a detail table to this table.
    /// </summary>
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
    /// <para>Alias: empty string or null or an alias</para>
    /// <para>OwnKeyField: COUNTRY_ID</para>
    /// <para>The above settings produce the following:</para>
    /// <para><c>left join COUNTRY on COUNTRY.ID = CUSTOMER.COUNTRY_ID</c></para>
    /// </summary>
    /// <param name="OwnKeyField">A field that belongs to this table. Is used in the join SQL statement</param>
    /// <param name="ForeignTable">The name of the table to join, e.g. COUNTRY to CUSTOMER</param>
    /// <param name="ForeignAlias">The alias of the table to join, e.g. <c>co</c></param>
    /// <param name="ForeignPrimaryKey">The primary key field name of the table to join, e.g. <c>Id</c></param>
    public TableDef AddJoin(string OwnKeyField, string ForeignTable, string ForeignAlias = "", string ForeignPrimaryKey = "Id") => AddJoin(OwnKeyField, Locator: null, ForeignTable, ForeignAlias, ForeignPrimaryKey);
    /// <summary>
    /// Adds a table join in this table and returns the joined table.
    /// <para>If this is CUSTOMER table then the COUNTRY table can be joined as</para>
    /// <para>TableName: COUNTRY</para>
    /// <para>Alias: empty string or null or an alias</para>
    /// <para>OwnKeyField: COUNTRY_ID</para>
    /// <para>The above settings produce the following:</para>
    /// <para><c>left join COUNTRY on COUNTRY.ID = CUSTOMER.COUNTRY_ID</c></para>
    /// </summary>
    /// <param name="OwnKeyField">A field that belongs to this table. Is used in the join SQL statement</param>
    /// <param name="Locator">The locator name to be used in the <see cref="OwnKeyField"/>.</param>
    /// <param name="ForeignTable">The name of the table to join, e.g. COUNTRY to CUSTOMER</param>
    /// <param name="ForeignAlias">The alias of the table to join, e.g. <c>co</c></param>
    /// <param name="ForeignPrimaryKey">The primary key field name of the table to join, e.g. <c>Id</c></param>
    public TableDef AddJoin(string OwnKeyField, string Locator, string ForeignTable, string ForeignAlias = "", string ForeignPrimaryKey = "Id")
    {
        FieldDef FieldDef = this.Fields.Get(OwnKeyField);
        FieldDef.Locator = Locator;
        
        TableDef Result = new TableDef();
       
        Result.Name = ForeignTable;
        Result.Owner = this;
        Result.Alias = ForeignAlias;
        Result.KeyField = ForeignPrimaryKey;
        Result.MasterField = OwnKeyField;
        Joins.Add(Result);
        return Result;
    }
    
    public List<FieldDef> GetBindableFields() => Fields.Where(f => f.IsBindable).ToList();

    public Dictionary<string, List<FieldDef>> GetBindableGroups()
    {
        List<FieldDef> BindableFields = GetBindableFields();
        List<string> GroupNames = new();
        void AddGroupName(string GroupName)
        {
            GroupName = string.IsNullOrWhiteSpace(GroupName) ? Sys.GENERAL : GroupName.Trim();
            if (!GroupNames.Any(x => x.IsSameText(GroupName)))
                GroupNames.Add(GroupName);
        }

        AddGroupName(Sys.GENERAL);

        foreach (string GroupName in FieldGroups)
            AddGroupName(GroupName);

        foreach (FieldDef Field in BindableFields)
            AddGroupName(Field.Group);

        Dictionary<string, List<FieldDef>> Result = new();
        foreach (string GroupName in GroupNames)
        {
            List<FieldDef> Fields = BindableFields
                .Where(Field => Field.Group.IsSameText(GroupName))
                .ToList();
            if (Fields.Count > 0)
                Result[GroupName] = Fields;
        }
        return Result;
    }
        
    // ● properties */
    /// <summary>
    /// The master definition this instance belongs to.
    /// </summary>
    [JsonIgnore]
    public ModuleDef ModuleDef { get; set; }
    /// <summary>
    /// The master table when this is a detail table.
    /// </summary>
    [JsonIgnore]
    public TableDef Master { get; set; }
    /// <summary>
    /// When this is a join table, the owner is the table that joins this table.
    /// </summary>
    [JsonIgnore]
    public TableDef Owner { get; set; }

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
    /// The detail key field. A field that belongs to this table and matches the primary key field of the master table.
    /// <para>It is used when this table is a detail table in a master-detail relation.</para>
    /// </summary>
    public string DetailField
    {
        get => fDetailField != null ? fDetailField : KeyField;
        set { if (fDetailField != value) { fDetailField = value; NotifyPropertyChanged(nameof(DetailField)); } }
    }
    /// <summary>
    /// When true then this is a detail table, e.g. a TradeLines under a Trade table.
    /// </summary>
    public bool IsDetail
    {
        get => fIsDetail;
        set { if (fIsDetail != value) { fIsDetail = value; NotifyPropertyChanged(nameof(IsDetail)); } }
    }
    /// <summary>
    /// Sometimes there is an one-to-one relationship between the top table, i.e. <see cref="ModuleDef.Table"/> and one or more details, but single-row tables.
    /// <para>For example: Trade and StoreTrade and FinTrade, where Trade is the top table and StoreTrade and FinTrade are <see cref="IsOneToOne"/> table.</para>
    /// </summary>
    public bool IsOneToOne 
    {
        get => fIsOneToOne;
        set { if (fIsOneToOne != value) { fIsOneToOne = value; NotifyPropertyChanged(nameof(IsOneToOne)); } }
    }
    /// <summary>
    /// When false then the table is not visible in the UI.
    /// <para>Useful with <see cref="IsOneToOne"/> details, for example when a Trade table has one-to-one details tables such as the StoreTrade and FinTrade.</para>
    /// <para>It is also useful with multi-row detail tables when the table must remain hidden, without a grid in the UI.</para>
    /// </summary>
    public bool IsUiVisible 
    {
        get => fIsUiVisible;
        set { if (fIsUiVisible != value) { fIsUiVisible = value; NotifyPropertyChanged(nameof(IsUiVisible)); } }
    }

    public TableSqls Sqls { get; set; } = new();
    /// <summary>
    /// Preferred field group display order.
    /// </summary>
    public List<string> FieldGroups
    {
        get => fFieldGroups ??= new();
        set => fFieldGroups = value;
    }
    /// <summary>
    /// The fields of this table
    /// </summary>
    public DefList<FieldDef> Fields 
    {
        get => fFields ??= new();
        set { if (fFields != value) { fFields = value; NotifyPropertyChanged(nameof(Fields)); } }
    }
    /// <summary>
    /// The list of join tables. 
    /// </summary>
    public DefList<TableDef> Joins 
    {
        get => fJoins ??= new() { AllowDuplicateNames = true };
        set { if (fJoins != value) { fJoins = value; NotifyPropertyChanged(nameof(Joins)); } }
    }
    /// <summary>
    /// The main table (Item) is selected as 
    /// <para> <c>select * from TABLE_NAME where ID = :ID</c></para>
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
    public DefList<SelectDef> Stocks 
    {
        get => fStocks ??= new();
        set { if (fStocks != value) { fStocks = value; NotifyPropertyChanged(nameof(Stocks)); } }
    }
    /// <summary>
    /// The detail tables of this table.
    /// </summary>
    public DefList<TableDef> Details
    {
        get => fDetails ??= new();
        set { if (fDetails != value) { fDetails = value; NotifyPropertyChanged(nameof(Details)); } }
    }

    [JsonIgnore] public bool HasDetails => fDetails != null && fDetails.Count > 0;
    [JsonIgnore] public bool HasJoins => fJoins != null && fJoins.Count > 0;
    [JsonIgnore] public bool HasStocks => fStocks != null && fStocks.Count > 0;
    [JsonIgnore] public bool IsJoin => Owner != null;
}
