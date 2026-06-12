/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Runtime locator service.
/// </summary>
[TypeStore]
public class Locator
{
    // ●  fields
    /// <summary>
    /// Field
    /// </summary>
    protected SqlStore fStore;
    /// <summary>
    /// Field
    /// </summary>
    protected bool fActive = true;
    /// <summary>
    /// Field
    /// </summary>
    protected LocatorDef fLocatorDef;
    /// <summary>
    /// Field
    /// </summary>
    protected DataTable ftblSchema;

    // ● protected
    /// <summary>
    /// Sql store.
    /// </summary>
    protected virtual SqlStore Store => fStore??= SqlStores.CreateSqlStore(LocatorDef.ConnectionName);
    /// <summary>
    /// Returns a <see cref="DataTable"/> with the schema of the source table.
    /// </summary>
    protected virtual DataTable tblSchema
    {
        get
        {
            if (ftblSchema == null)
            {
                SelectSql SS = GetSelectSql();
        
                string StatementName = $"{this.GetType().FullName}.{LocatorDef.Name}";
                ftblSchema = Store.GetNativeSchemaFromSelect(StatementName, SS.Text);
            }

            return ftblSchema;  
        }
    }
    /// <summary>
    /// The character that triggers a locator search.
    /// </summary>
    protected Char TriggerChar = '?';
    
    // ● engine helpers
    /// <summary>
    /// Removes all the trailing occurence of a character in a string.
    /// </summary>
    protected virtual string TrimEnd(string Text, Char C) => !string.IsNullOrWhiteSpace(Text) ? Text.TrimEnd().TrimEnd(C) : Text;
    /// <summary>
    /// Executes an event as indicated by the EventType and returns the LocatorEventArgs of the event.
    /// <para>NOTE: It returns a LocatorEventArgs even if the AnyEvent event is not linked.</para>
    /// </summary>
    protected virtual LocatorEventArgs OnAnyEvent(LocatorEventType EventType, SelectSql SelectSql = null, string UserWhere = null, string SourceTableFilter = "")
    {
        LocatorEventArgs Args = new ()
        {
            Locator = this,
            EventType = EventType,
            SelectSql = SelectSql,
            UserWhere = UserWhere,
            SourceTableFilter = SourceTableFilter,
        };

        if (AnyEvent != null)
            AnyEvent(this, Args);

        return Args;
    }
    
    // ● engine
    /// <summary>
    /// Executes the SELECT statement, loads the <see cref="SourceTable"/> and returns the row count of the table.
    /// <para>WhereUser is the WHERE added by the user in a locator control or column. Could be null or empty.</para>
    /// </summary>
    protected virtual int SelectSourceTable(SelectSql SelectSql, string UserWhere)
    {
        int Result = 0;
        
        if (Active)
        {
           Clear();

            // NOTE: this may load the source table, changing the IsSourceTableValid.
            // A client code should use the Select() method of this class when executing the SELECT statement.
            // The Select() method sets the appropriate flags when it is called.
            LocatorEventArgs Args = OnAnyEvent(LocatorEventType.SelectSourceTable, SelectSql: SelectSql, UserWhere: UserWhere);

            if (!IsSourceTableValid)
            {
                string SqlText = SelectSql.Text;
                Result = Select(SqlText, UserWhere);
            }
        }
        
        return Result;
    }
    /// <summary>
    /// Returns a <see cref="SelectSql"/> with the SELECT statement the locator has to select.
    /// <para>If the <see cref="LocatorDef.SelectSql"/> is not given, then this method constructs a SELECT statement based on the <see cref="LocatorDef"/> information.</para>
    /// <para>NOTE: no USER WHERE clause is included, but a WHERE clause may added by a client code through an event handler.</para>
    /// </summary>
    protected virtual SelectSql GetSelectSql()
    {
        SelectSql Result = new();

        if (!string.IsNullOrWhiteSpace(LocatorDef.SelectSql))
        {
            Result.Text = LocatorDef.SelectSql;
        }
        else
        {
            // WARNING:
            // We assume that all fields belong to the same table, the source table name of the descriptor.
            // For more complex SELECTs the user must provide the SELECT statement manually.  
            if (!string.IsNullOrEmpty(LocatorDef.SourceTableName))
            {
                List<string> SelectList = new();
                if (!LocatorDef.Fields.Any(item => item.Name.IsSameText(LocatorDef.KeyField) || item.Alias.IsSameText(LocatorDef.KeyField)))
                    SelectList.Add($"  {LocatorDef.SourceTableName}.{LocatorDef.KeyField} as {LocatorDef.KeyField}");

                foreach (LocatorFieldDef FieldDef in LocatorDef.Fields)
                {
                    string FieldLine = $"  {LocatorDef.SourceTableName}.{FieldDef.Name} as {FieldDef.Alias}";
                    SelectList.Add(FieldLine);
                }

                string S = SelectList.Count > 0 ? string.Join($", {Environment.NewLine}", SelectList.ToArray()) : "*";

                Result.Select = S;
                Result.From = LocatorDef.SourceTableName;

                if (!string.IsNullOrWhiteSpace(LocatorDef.OrderBy))
                    Result.AddToOrderBy(LocatorDef.OrderBy);
            }
        }
        
        LocatorEventArgs Args = OnAnyEvent(LocatorEventType.AddToWhere, SelectSql: Result);

        return Result;
    }
    /// <summary>
    /// Filters the <see cref="SourceTable"/> 
    /// </summary>
    protected virtual void FilterSourceTable()
    {
        if (Active && IsSourceTableValid && SourceTable.Columns.Count > 0)
        {
            string Filter = string.Empty;

            if (Master != null && Master.Active && !string.IsNullOrWhiteSpace(DetailKey))
            {
                Filter = string.Empty;
                Type MasterKeyValueType = !Sys.IsNull(Master.KeyValue)? Master.KeyValue.GetType() : null;

                if (MasterKeyValueType != null)
                {
                    if (MasterKeyValueType.IsString())
                    {
                        string V = Master.KeyValue.ToString().Replace("'", "''");
                        Filter = $"{DetailKey} = '{V}' ";
                    }
                    else if (MasterKeyValueType.IsDateTime())
                    {
                        DateTime DT = (DateTime)Master.KeyValue;
                        string V = DT.ToString("yyyy-MM-dd HH:mm:ss");
                        Filter = $"{DetailKey} = '{V}' " ;
                    }
                    else if (MasterKeyValueType.IsInteger())
                    {
                        int V = Convert.ToInt32(Master.KeyValue);
                        Filter = $"{DetailKey} = {V} " ;
                    }
                }
            }

            LocatorEventArgs Args = OnAnyEvent(LocatorEventType.FilterSourceTable, SourceTableFilter: Filter);

            SourceTable.DetailRowFilter = Args.SourceTableFilter;
        }
    }
    /// <summary>
    /// Sets up the columns of the source table, i.e. setting visibility and captions.
    /// </summary>
    protected virtual void SetupSourceTable()
    {
        // setup list table columns 
        if (Active && IsSourceTableValid && SourceTable.Columns.Count > 0)
        {
            FilterSourceTable();

            foreach (DataColumn Column in SourceTable.Columns)
            {
                LocatorFieldDef FieldDef = LocatorDef.Fields.Find(Column.ColumnName);
                Column.IsVisible(FieldDef != null && FieldDef.IsVisible);
                if (Column.IsVisible())
                {
                    Column.Caption = string.Empty;
                    if (FieldDef != null)
                        Column.Caption = FieldDef.Title;
                }
            }

            LocatorEventArgs Args = OnAnyEvent(LocatorEventType.SetupSourceTable);
        }
    }
    /// <summary>
    /// Constructs the WHERE clause which is going to be added to the SELECT statement.
    /// </summary>
    protected virtual string ConstructUserWhere(string Term)
    {
        if (string.IsNullOrWhiteSpace(Term))
            throw new TripousException($"Locator {nameof(Term)} is empty.");
        
        if (!ContainsSearchTrigger(Term))
            throw new TripousException("Locator search trigger is missing.");

        Term = TrimEnd(Term, TriggerChar).TrimEnd('%');  

        if (Term.Length < Db.Settings.LocatorMinimumSearchTextLength)
            throw new TripousException($"Locator search text must contain at least {Db.Settings.LocatorMinimumSearchTextLength} characters.");

        Term = Term.Replace("'", "''");
        List<string> UserWhereList = new();
        foreach (LocatorFieldDef FieldDef in LocatorDef.Fields)
        {
            if (FieldDef.IsSearchable)
            {
                DataColumn Column = tblSchema.FindColumn(FieldDef.Alias);
                if (Column != null && Column.DataType.IsString())
                {
                    string WhereItem = $"{Column.ColumnName} LIKE '{Term}%' " ;
                    UserWhereList.Add(WhereItem);
                }
            }
            
        }
 
        
        string Result = UserWhereList.Count > 0? string.Join(" or ", UserWhereList): string.Empty;
        return Result;
    }
    /// <summary>
    /// Finds the source row key column.
    /// </summary>
    protected virtual DataColumn FindSourceKeyColumn(DataRow SourceRow)
    {
        DataColumn Result = SourceRow.Table.FindColumn(LocatorDef.KeyField);
        if (Result != null)
            return Result;

        LocatorFieldDef KeyFieldDef = LocatorDef.Fields.FirstOrDefault(item => item.Name.IsSameText(LocatorDef.KeyField));
        return KeyFieldDef != null ? SourceRow.Table.FindColumn(KeyFieldDef.Alias) : null;
    }
    /// <summary>
    /// Returns a source row key value.
    /// </summary>
    protected virtual object GetSourceKeyValue(DataRow SourceRow)
    {
        DataColumn Column = FindSourceKeyColumn(SourceRow);
        return Column != null ? SourceRow[Column] : DBNull.Value;
    }
    /// <summary>
    /// Returns the source key field name as exposed by the locator SELECT.
    /// </summary>
    protected virtual string GetSourceKeySelectFieldName()
    {
        LocatorFieldDef KeyFieldDef = LocatorDef.Fields.FirstOrDefault(item => item.Name.IsSameText(LocatorDef.KeyField));
        return KeyFieldDef != null ? KeyFieldDef.Alias : LocatorDef.KeyField;
    }
    /// <summary>
    /// Returns a source row locator field value.
    /// </summary>
    protected virtual object GetSourceFieldValue(DataRow SourceRow, LocatorFieldDef FieldDef)
    {
        DataColumn Column = SourceRow.Table.FindColumn(FieldDef.Alias);
        if (Column == null)
            Column = SourceRow.Table.FindColumn(FieldDef.Name);
        return Column != null ? SourceRow[Column] : DBNull.Value;
    }
    /// <summary>
    /// Resolves a target field name for a locator field.
    /// </summary>
    protected virtual string ResolveTargetFieldName(LocatorFieldDef FieldDef, Dictionary<string, string> TargetFieldMap, bool UseAliasFallback)
    {
        if (FieldDef == null)
            return null;

        if (TargetFieldMap != null)
        {
            if (TargetFieldMap.TryGetValue(FieldDef.Name, out string Result))
                return Result;
            if (TargetFieldMap.TryGetValue(FieldDef.Alias, out Result))
                return Result;
        }
        if (!string.IsNullOrWhiteSpace(FieldDef.TargetField))
            return FieldDef.TargetField;
        return UseAliasFallback ? FieldDef.Alias : null;
    }
    /// <summary>
    /// Sets a target row value.
    /// </summary>
    protected virtual void SetTargetRowValue(DataRow TargetRow, string FieldName, object Value, bool ForceReadOnly)
    {
        if (TargetRow == null || string.IsNullOrWhiteSpace(FieldName))
            return;

        DataColumn Column = TargetRow.Table.FindColumn(FieldName);
        if (Column == null)
            return;
        if (Column.ReadOnly && !ForceReadOnly)
            return;

        object NewValue = Sys.IsNull(Value) ? DBNull.Value : Value;
        if (Sys.IsNull(NewValue) && !Column.AllowDBNull)
            return;

        if (object.Equals(TargetRow[Column], NewValue))
            return;

        bool WasReadOnly = Column.ReadOnly;
        try
        {
            if (WasReadOnly && ForceReadOnly)
                Column.ReadOnly = false;
            TargetRow[Column] = NewValue;
        }
        finally
        {
            if (WasReadOnly && ForceReadOnly)
                Column.ReadOnly = true;
        }
    }
    /// <summary>
    /// Assigns locator values using optional key and target map information.
    /// </summary>
    protected virtual void Assign(DataRow SourceRow, DataRow TargetRow, string KeyFieldName, Dictionary<string, string> TargetFieldMap, bool UseAliasFallback, bool ForceTargetReadOnly)
    {
        bool Clearing = SourceRow == null;

        if (Clearing)
        {
            SelectedRow = null;
            KeyValue = DBNull.Value;
        }
        else
        {
            SelectedRow = SourceRow;
            KeyValue = GetSourceKeyValue(SourceRow);
        }

        MemTable tblTarget = TargetRow.Table as MemTable;
        if (tblTarget == null)
            throw new TripousDataException($"{this.GetType().FullName} cannot assign values when the target table is not a {nameof(MemTable)} ");

        if (!string.IsNullOrWhiteSpace(KeyFieldName))
            SetTargetRowValue(TargetRow, KeyFieldName, KeyValue, ForceReadOnly: false);

        foreach (LocatorFieldDef FieldDef in LocatorDef.Fields)
        {
            if (LocatorDef.KeyField.IsSameText(FieldDef.Name))
                continue;

            string TargetFieldName = ResolveTargetFieldName(FieldDef, TargetFieldMap, UseAliasFallback);
            object Value = Clearing ? DBNull.Value : GetSourceFieldValue(SourceRow, FieldDef);
            SetTargetRowValue(TargetRow, TargetFieldName, Value, ForceTargetReadOnly);
        }
    }

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public Locator()
    {
    }

    // ● public methods
    /// <summary>
    /// Initializes this instance.
    /// </summary>
    public virtual void Initialize(LocatorDef LocatorDef)
    {
        if (!IsInitialized)
            this.LocatorDef = LocatorDef;
    }
    /// <summary>
    /// Returns true if the specified text requests a locator search.
    /// </summary>
    public virtual bool ContainsSearchTrigger(string Term) => !string.IsNullOrWhiteSpace(Term) && Term.TrimEnd().EndsWith("?");
 
    /// <summary>
    /// Returns true when exactly one row is found.
    /// </summary>
    public virtual bool TryLocate(string Term, out LocatorSearchResult Result)
    {
        Result = Execute(Term);
        return Result.IsSingleRow;
    }
    
    /// <summary>
    /// Locates a single source row by the locator key value.
    /// </summary>
    public virtual bool LocateByKey(object Value)
    {
        if (Sys.IsNull(Value))
            return false;

        string FieldName = GetSourceKeySelectFieldName();
        string ValueText;
        if (Value is string)
        {
            string V = Value.ToString().Replace("'", "''");
            ValueText = $"'{V}'";
        }
        else if (Value is DateTime Date)
        {
            ValueText = $"'{Date:yyyy-MM-dd HH:mm:ss}'";
        }
        else
        {
            ValueText = Value.ToString();
        }

        SelectSql SS = GetSelectSql();
        int RowCount = SelectSourceTable(SS, $"{FieldName} = {ValueText}");
        if (RowCount != 1)
            return false;

        SelectedRow = SourceTable.Rows[0];
        this.KeyValue = GetSourceKeyValue(SelectedRow);
        return true;
    }
    /// <summary>
    /// Assigns the locator key and field values from a source row to a target row.
    /// </summary>
    /// <param name="SourceRow">The row to get values from. When is null, then the key field and mapped target fields are cleared. </param>
    /// <param name="TargetRow">The row to set values to.</param>
    /// <param name="KeyFieldName">Identifies the target field that receives the locator key.</param>
    /// <param name="TargetFieldMap">maps locator source field names or aliases to fields in the target row.
    /// <para>The mapping is supplied by the binding layer because only the binding knows the target table, join and aliases used in its current context. This allows the same locator definition to be reused safely by different controls, grids and modules.</para>
    /// <para>The target field map takes precedence over <see cref="LocatorFieldDef.TargetField"/>.</para></param>
    public virtual void Assign(DataRow SourceRow, DataRow TargetRow, string KeyFieldName, Dictionary<string, string> TargetFieldMap)
    {
        Assign(SourceRow, TargetRow, KeyFieldName, TargetFieldMap, UseAliasFallback: true, ForceTargetReadOnly: true);
    }

    // ● source table specific
    /// <summary>
    /// Clears the data from source table
    /// </summary>
    public virtual void Clear()
    {
        SourceTable.DeleteAll(AcceptChangesToo: true);
        IsSourceTableValid = false;
        KeyValue = DBNull.Value;
        SelectedRow = null;
    }
    /// <summary>
    /// Executes a SELECT statement and loads the source table.
    /// <para>NOTE: To be used by client code.</para>
    /// </summary>
    public virtual int Select(string SqlText, string UserWhere)
    {
        int Result = 0;
        if (IsSourceTableValid)
            Clear();
        
        if (!string.IsNullOrWhiteSpace(UserWhere))
            SqlText = $"select * from ({SqlText}) X where {UserWhere}";
        
        Result = Store.SelectTo(SourceTable, SqlText);
        IsSourceTableValid = true;
        
        FilterSourceTable();
        SetupSourceTable();

        return Result;
    }
    /// <summary>
    /// Executes the locator SELECT SQL and assigns the <see cref="SourceTable"/>
    /// </summary>
    public virtual LocatorSearchResult Execute(string Term)
    {
        SelectSql SS = GetSelectSql();
        string UserWhere = ConstructUserWhere(Term);

        int RowCount = SelectSourceTable(SS, UserWhere);
        
        bool TooManyRows = RowCount > Db.Settings.LocatorMaximumDropDownRows;
        string Message = TooManyRows ? "Too many rows. Type more characters." : string.Empty;

        if (TooManyRows || RowCount == 0)
            Clear();

        LocatorSearchResult Result = new()
        {
            Locator = this,
            SearchTerm = Term,
            SelectSql = SS,
            TooManyRows = TooManyRows,
            Message = Message
        };

        return Result;
    }
    
    // ● properties
    /// <summary>
    /// True when this instance is initialized
    /// </summary>
    public virtual bool IsInitialized => this.LocatorDef != null;
    /// <summary>
    /// Activates or deactivates the locator
    /// </summary>
    public virtual bool Active
    {
        get => fActive && IsInitialized;
        set => fActive = value;
    }
    
    /// <summary>
    /// Locator definition.
    /// </summary>
    public LocatorDef LocatorDef { get; private set; }
    /// <summary>
    /// The table that results in after the execution of the SELECT statement.
    /// </summary>
    public MemTable SourceTable { get; } = new();
    /// <summary>
    /// True when <see cref="SourceTable"/> contains valid data.
    /// </summary>
    public bool IsSourceTableValid { get; protected set; }

    /// <summary>
    /// Gets or sets the key value.
    /// <para>Is the value returned by the Locator. </para>
    /// <para>This value comes from an Id field of the source table and goes the target table.</para>
    /// </summary>
    public virtual object KeyValue { get; protected set; } = DBNull.Value;
    /// <summary>
    /// Gets or sets the selected row.
    /// </summary>
    public virtual DataRow SelectedRow { get; protected set; }
    
    /// <summary>
    /// For creating cascade lookups.
    /// <para>The master locator this locator uses to get the value of the DetailKey.</para>
    /// </summary>
    public Locator Master { get; set; }
    /// <summary>
    /// For creating cascade lookups.
    /// <para>A field name of the source table (i.e. CountryId).</para>
    /// <para>Used in filtering the source table's <see cref="MemTable.DataView"/>, using a value that comes from the <see cref="Master"/>.<see cref="KeyValue"/></para>
    /// </summary>
    public string DetailKey { get; set; }

 
    // ● events
    /// <summary>
    /// Occurs in circumstances as indicated by the <see cref="LocatorEventType"/> enum.
    /// </summary>
    public event EventHandler<LocatorEventArgs> AnyEvent;
}
