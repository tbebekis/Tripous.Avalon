namespace Tripous.Data;

public class MemTable : DataTable
{
    static private int TableNameCounter = 0;
    protected int EventsDisableCounter = 0;
    private bool hasExpressionsChecked;

    private bool hasExpressions;
    private string[] fDetailFields;
    private DataRow fCurrentRow;
    private MemTable fMaster;
    private string fDetailRowFilter;
    private string fUserRowFilter;

    //  ● private
    private bool IsValidRow(DataRow row)
    {
        return row != null
               && ReferenceEquals(row.Table, this)
               && row.RowState != DataRowState.Deleted
               && row.RowState != DataRowState.Detached;
    }
    private DataRow FirstRowOrNull() => Rows.Count > 0 ? Rows[0] : null;
    private void EnsureCurrentRow()
    {
        if (IsValidRow(fCurrentRow))
            return;

        fCurrentRow = Rows.Count > 0 ? Rows[0] : null;
        OnCurrentRowChanged();
    }

    protected virtual void OnCurrentRowChanged()
    {
        CurrentRowChanged?.Invoke(this, EventArgs.Empty);
    }
    private static string QuoteColumnName(string ColumnName)
    {
        // return $"[{ColumnName.Replace("]", "]]")}]";
        return ColumnName;
    }
    private static bool IsIntegerType(Type DataType)
    {
        return DataType == typeof(byte)
               || DataType == typeof(sbyte)
               || DataType == typeof(short)
               || DataType == typeof(ushort)
               || DataType == typeof(int)
               || DataType == typeof(uint)
               || DataType == typeof(long)
               || DataType == typeof(ulong);
    }
    private static bool IsSupportedRelationFieldType(Type DataType)
    {
        return DataType == typeof(string) || IsIntegerType(DataType);
    }

    private static string FormatRowFilterValue(object Value, Type DataType)
    {
        if (Value == null || Value == DBNull.Value)
            return "NULL";

        if (DataType == typeof(string))
            return $"'{Value.ToString().Replace("'", "''")}'";

        if (IsIntegerType(DataType))
            return Convert.ToString(Value, CultureInfo.InvariantCulture);

        throw new ApplicationException($"Unsupported relation field type '{DataType.FullName}'.");
    }
    private static string CreateRowFilter(DataRow MasterRow, string[] MasterFields, string[] DetailFields, DataColumn[] MasterColumns, DataColumn[] DetailColumns)
    {
        var SB = new StringBuilder();

        for (int i = 0; i < MasterFields.Length; i++)
        {
            if (i > 0)
                SB.Append(" AND ");

            object Value = MasterRow[MasterColumns[i]];

            if (Value == DBNull.Value)
                SB.Append($"{QuoteColumnName(DetailFields[i])} IS NULL");
            else
                SB.Append(
                    $"{QuoteColumnName(DetailFields[i])} = {FormatRowFilterValue(Value, DetailColumns[i].DataType)}");
        }

        string Result = SB.ToString();
        return Result;
    }

    internal void ValidateRelationSchema()
    {
        if (Master == null)
            throw new ApplicationException($"Table '{TableName}' has no master.");

        if (MasterFields == null || MasterFields.Length == 0)
            throw new ApplicationException($"Table '{TableName}': MasterFields not defined.");

        if (DetailFields == null || DetailFields.Length == 0)
            throw new ApplicationException($"Table '{TableName}': DetailFields not defined.");

        if (MasterFields.Length != DetailFields.Length)
            throw new ApplicationException($"Table '{TableName}': MasterFields and DetailFields count mismatch.");

        DataColumn[] ParentColumns = Master.GetColumns(MasterFields);
        DataColumn[] ChildColumns = GetColumns(DetailFields);

        for (int i = 0; i < ParentColumns.Length; i++)
        {
            Type ParentType = ParentColumns[i].DataType;
            Type ChildType = ChildColumns[i].DataType;

            if (!IsSupportedRelationFieldType(ParentType))
                throw new ApplicationException(
                    $"Table '{Master.TableName}': Column '{ParentColumns[i].ColumnName}' has unsupported relation type '{ParentType.Name}'.");

            if (!IsSupportedRelationFieldType(ChildType))
                throw new ApplicationException(
                    $"Table '{TableName}': Column '{ChildColumns[i].ColumnName}' has unsupported relation type '{ChildType.Name}'.");

            if (ParentType != ChildType)
                throw new ApplicationException(
                    $"Table '{Master.TableName}' -> '{TableName}': Field type mismatch between '{ParentColumns[i].ColumnName}' and '{ChildColumns[i].ColumnName}'.");
        }
    }

    //  ● private
    /// <summary>
    /// The virtual OnTableNewRow() is not called if the invocation list of 
    /// the TableNewRow event is empty. Microsoft says this is by design
    /// see: http://connect.microsoft.com/VisualStudio/feedback/details/184473/ontablenewrow-is-called-only-when-the-event-delegate-list-for-tablenewrow-event-is-not-empty
    /// Anyway, just adding an empty event handler, forces the OnTableNewRow() to be called.
    /// </summary>
    void Table_TableNewRow(object sender, DataTableNewRowEventArgs e)
    {
    }

    //  ● overrides - event activation  
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnColumnChanging(DataColumnChangeEventArgs e)
    {
        if (!EventsDisabled)
            base.OnColumnChanging(e);
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnColumnChanged(DataColumnChangeEventArgs e)
    {
        if (!EventsDisabled)
        {
            base.OnColumnChanged(e);

            if (!hasExpressionsChecked)
            {
                foreach (DataColumn Field in Columns)
                {
                    if (!string.IsNullOrEmpty(Field.Expression))
                    {
                        hasExpressions = true;
                        break;
                    }
                }

                hasExpressionsChecked = true;
            }

            if (hasExpressions)
            {
                e.Row.EndEdit();
            }
        }
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnRowChanging(DataRowChangeEventArgs e)
    {
        if (!EventsDisabled)
            base.OnRowChanging(e);
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnRowChanged(DataRowChangeEventArgs e)
    {
        //if (!ReferenceEquals(CurrentRow, e.Row) && Rows.Count == 1)
        //    CurrentRow = e.Row;

        if (e.Action == DataRowAction.Add && e.Row.RowState != DataRowState.Deleted)
            CurrentRow = e.Row;

        if (!EventsDisabled)
        {
            base.OnRowChanged(e);

            // our own event on new row added
            if (e.Action == DataRowAction.Add && e.Row.RowState != DataRowState.Deleted)
            {
                DataTableNewRowEventArgs ea = new DataTableNewRowEventArgs(e.Row);
                NewRowAdded?.Invoke(this, ea);
            }
        }
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnRowDeleting(DataRowChangeEventArgs e)
    {
        if (!EventsDisabled)
            base.OnRowDeleting(e);
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnRowDeleted(DataRowChangeEventArgs e)
    {
        if (!EventsDisabled)
            base.OnRowDeleted(e);

        if (!IsValidRow(fCurrentRow) && !IsDetail)
            CurrentRow = FirstRowOrNull();
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnTableClearing(DataTableClearEventArgs e)
    {
        if (!EventsDisabled)
            base.OnTableClearing(e);
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnTableCleared(DataTableClearEventArgs e)
    {
        if (!EventsDisabled)
            base.OnTableCleared(e);

        if (fCurrentRow != null)
        {
            fCurrentRow = null;
            if (!EventsDisabled)
                OnCurrentRowChanged();
        }
    }
    /// <summary>
    /// Calls the base method, only if <see cref="EventsDisabled"/> is false, eventually
    /// deactivating the method.
    /// </summary>
    protected override void OnTableNewRow(DataTableNewRowEventArgs e)
    {
        if (e.Row == null)
            throw new ArgumentNullException(nameof(e.Row));

        // auto-generated Guid Key
        if (AutoGenerateGuidKeys && KeyFields != null && KeyFields.Length == 1)
        {
            int Index = Columns.IndexOf(KeyFields[0]);

            if ((Index >= 0) && (Columns[Index].DataType == typeof(System.String)))
            {
                if (e.Row[Index] == DBNull.Value || string.IsNullOrWhiteSpace(e.Row[Index].ToString()))
                    if ((Columns[Index].MaxLength == -1) || (Columns[Index].MaxLength >= 40))
                        e.Row[Index] = GenId(); // GUID string without brackets
            }
        }

        if (IsDetail)
        {
            if (Master == null)
                throw new ApplicationException($"Table '{TableName}' has no master.");

            if (Master.CurrentRow == null)
                throw new ApplicationException(
                    $"Table '{TableName}': master table '{Master.TableName}' has no current row.");

            ValidateRelationSchema();

            var parentCols = Master.GetColumns(MasterFields);
            var childCols = GetColumns(DetailFields);

            for (int i = 0; i < parentCols.Length; i++)
                e.Row[childCols[i]] = Master.CurrentRow[parentCols[i]];
        }

        CurrentRow = null;

        if (!EventsDisabled)
        {
            base.OnTableNewRow(e);

            // our own event on new row adding, 
            // WARNING: the new row is not in rows yet
            NewRowAdding?.Invoke(this, e);
        }
    }


    //  ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public MemTable()
        : this(NextTableName())
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public MemTable(string tableName)
        : this(tableName, string.Empty)
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public MemTable(string tableName, string tableNamespace)
        : base(tableName, tableNamespace)
    {
        AutoGenerateGuidKeys = true;
        Details = new DetailList(this);
        DataView = new DataView(this);
        CaseSensitive = false;
        Locale = System.Globalization.CultureInfo.InvariantCulture;

        /* The virtual OnTableNewRow() is not called if the invocation list of
           the TableNewRow event is empty. Microsoft says this is by design
            see: http://connect.microsoft.com/VisualStudio/feedback/details/184473/ontablenewrow-is-called-only-when-the-event-delegate-list-for-tablenewrow-event-is-not-empty
           Anyway, just adding an empty event handler, forces the OnTableNewRow() to be called.
         */
        this.TableNewRow += new DataTableNewRowEventHandler(Table_TableNewRow);
    }

    // ● static 
    /// <summary>
    /// Constructs and returns a valid and unique <see cref="DataTable.TableName"/>
    /// </summary>
    static public string NextTableName()
    {
        int Value = Interlocked.Increment(ref TableNameCounter);
        return "Table_" + Value.ToString();
    }
    /// <summary>
    /// Adds Table to the DS.Tables
    /// </summary>
    static public void AddToDataSet(DataSet DS, DataTable Table)
    {
        if (Table.DataSet != null)
            Table.DataSet.Tables.Remove(Table);

        DS.Tables.Add(Table);
    }
    /// <summary>
    /// Creates and returns a new Guid.
    /// <para>If UseBrackets is true, the new guid is surrounded by {}</para>
    /// </summary>
    static public string GenId(bool UseBrackets = false)
    {
        string format = UseBrackets ? "B" : "D";
        return Guid.NewGuid().ToString(format).ToUpper();
    }
    /// <summary>
    /// Sets all data rows to <see cref="DBNull.Value"/>
    /// </summary>
    static public void SetAllRowsToNull(DataRow Row)
    {
        for (int i = 0; i < Row.Table.Columns.Count; i++)
            Row[i] = DBNull.Value;
    }
    static public DataRowView GetDataRowView(DataRow Row, DataView DataView) =>
        DataView.Cast<DataRowView>().FirstOrDefault(drv => drv.Row == Row);

    // ● public 
    public DataColumn[] GetColumns(string[] ColumnNames)
    {
        if (ColumnNames == null || ColumnNames.Length == 0)
            throw new ApplicationException($"Table {TableName}: No column names specified.");

        return ColumnNames
            .Select(name => Columns.Contains(name)
                ? Columns[name]
                : throw new ApplicationException($"Table {TableName}: Column '{name}' not found."))
            .ToArray();
    }
    public void AddDetail(MemTable tblDetail)
    {
        if (this.DataSet == null)
            throw new ApplicationException("Cannot be a master table without belonging to a DataSet.");

        if (tblDetail.DataSet != this.DataSet)
            AddToDataSet(this.DataSet, tblDetail);

        this.Details.Add(tblDetail);
    }
    public void RemoveDetail(MemTable tblDetail)
    {
        if (tblDetail == null)
            throw new ArgumentNullException(nameof(tblDetail));

        if (!Details.Contains(tblDetail))
            throw new ApplicationException($"Table '{tblDetail.TableName}' is not a detail of '{TableName}'.");

        while (tblDetail.Details.Active)
            tblDetail.Details.Active = false;

        Details.Remove(tblDetail);
    }
    public MemTable[] GetDetails() => Details.ToArray();
    /// <summary>
    /// Returns the child rows (belonging to this table) of a specified master row, belonging to the master table.
    /// </summary>
    public DataRow[] GetChildRows(DataRow MasterRow)
    {
        if (MasterRow == null)
            throw new ArgumentNullException(nameof(MasterRow));

        if (Master == null)
            throw new ApplicationException($"Table '{TableName}' has no master table.");

        if (!ReferenceEquals(MasterRow.Table, Master))
            throw new ApplicationException("Master row does not belong to the master table.");

        ValidateRelationSchema();

        DataColumn[] ParentColumns = Master.GetColumns(MasterFields);
        DataColumn[] ChildColumns = GetColumns(DetailFields);
        string Filter = CreateRowFilter(MasterRow, MasterFields, DetailFields, ParentColumns, ChildColumns);

        return Select(Filter);
    }
    /// <summary>
    /// If Table has a single field as Key and that DataColumn is of type System.Int32,
    /// then initializes the properties  of the DataColumn,
    /// so the column to be an autoincrement one (negative).
    /// </summary>
    public void InitializeAutoInc()
    {
        if (KeyFields != null && KeyFields.Length == 1)
        {
            int Index = Columns.IndexOf(KeyFields[0]);
            DataColumn Column = Columns[Index];

            if (Column.DataType == typeof(System.Int32))
            {
                Column.AutoIncrement = true;
                Column.AutoIncrementSeed = -1;
                Column.AutoIncrementStep = -1;
            }
        }
    }
    public void MasterRowChanged()
    {
        if (IsDetail)
        {
            if (!Details.Active)
            {
                DataView.RowFilter = string.Empty;
                return;
            }

            ValidateRelationSchema();

            if (Master.CurrentRow == null)
            {
                DataView.RowFilter = "1 = 0";
                return;
            }

            DataColumn[] ParentColumns = Master.GetColumns(MasterFields);
            DataColumn[] ChildColumns = GetColumns(DetailFields);

            string Filter = CreateRowFilter(Master.CurrentRow, MasterFields, DetailFields, ParentColumns, ChildColumns);
            DataView.Sort = "";
            DataView.RowFilter = Filter;
        }
    }
    /// <summary>
    /// Clears this instance and all of its details of all the data. 
    /// </summary>
    public void ClearAll()
    {
        foreach (MemTable Detail in Details)
            Detail.ClearAll();

        Clear();
        AcceptChanges();
    }
    public void SetCurrentRowToNull()
    {
        CurrentRow = null;
    }

    /// <summary>
    /// Adds and returns a new row. The new row is added to rows.
    /// </summary>
    public DataRow AddNewRow()
    {
        DataRow Row = NewRow();
        Rows.Add(Row);

        CurrentRow = Row;
        return Row;
    }
    public void CopyFrom(MemTable Source, bool IncludeDetails = true)
    {
        this.EventsDisabled = true;

        try
        {
            this.DetailsActive = false;

            foreach (MemTable Detail in this.GetDetails())
                this.RemoveDetail(Detail);

            this.CopyStructureAndRowsFrom(Source);

            this.KeyFields = Source.KeyFields?.ToArray();
            this.MasterFields = Source.MasterFields?.ToArray();
            this.DetailFields = Source.DetailFields?.ToArray();
            this.AutoGenerateGuidKeys = Source.AutoGenerateGuidKeys;

            if (IncludeDetails)
            {
                foreach (MemTable SourceDetail in Source.GetDetails())
                {
                    MemTable thisDetail = new MemTable(SourceDetail.TableName);
                    thisDetail.CopyFrom(SourceDetail, true);
                    this.AddDetail(thisDetail);
                }
            }

            this.CurrentRow = this.Rows.Count > 0 ? this.Rows[0] : null;
            this.DetailsActive = Source.DetailsActive;
        }
        finally
        {
            this.EventsDisabled = false;
        }
    }

    public string GetTopTableErrors()
    {
        StringBuilder SB = new();

        List<MemTable> Tables = new();

        void AddErrors(MemTable Table)
        {
            if (Tables.Contains(Table))
                SB.AppendLine($"Table '{Table.TableName}' is already in the table tree.");
            
            if (string.IsNullOrWhiteSpace(Table.TableName))
                SB.AppendLine($"Table '{Table.TableName}' has no table name.");
            
            if (Table.KeyFields == null || Table.KeyFields.Length == 0 || string.IsNullOrWhiteSpace(Table.KeyField))
                SB.AppendLine($"Table '{Table.TableName}' has no key fields.");

            if (Table.Master != null)
            {
                if (string.IsNullOrWhiteSpace(Table.MasterField))
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no master field.");
                
                if (string.IsNullOrWhiteSpace(Table.DetailField))
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no detail field.");
 
                if (Table.Master.FindColumn(Table.MasterField) == null)
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no matching master field.");
                
                if (Table.FindColumn(Table.DetailField) == null)
                    SB.AppendLine($"Detail Table '{Table.TableName}' has no matching detail field.");
            }

            if (Table.Details.Count > 0)
            {
                foreach (var tblDetail in Table.Details)
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

    // ● properties 
    /// <summary>
    /// Primary key fields
    /// </summary>
    public string[] KeyFields { get; set; } = new[] { "Id" }; 
    /// <summary>
    /// When this is a detail table, these are the fields from its master
    /// </summary>
    public string[] MasterFields { get; set; } = new[] { "Id" }; 
    /// <summary>
    /// When this is a detail table, these fields from this table are mapped to master fields.
    /// </summary>
    public string[] DetailFields
    {
        get => fDetailFields != null ? fDetailFields : KeyFields;
        set => fDetailFields = value;
    }

    public string KeyField
    {
        get => KeyFields != null && KeyFields.Length > 0? KeyFields[0] : string.Empty;
        set
        {
            if (KeyFields != null && KeyFields.Length > 0) KeyFields[0] = value;
        }
    }
    public string MasterField
    {
        get => MasterFields != null && MasterFields.Length > 0? MasterFields[0] : string.Empty;
        set
        {
            if (MasterFields != null && MasterFields.Length > 0) MasterFields[0] = value;
        }
    }
    public string DetailField
    {
        get => DetailFields != null && DetailFields.Length > 0? DetailFields[0] : string.Empty;
        set
        {
            if (DetailFields != null && DetailFields.Length > 0) DetailFields[0] = value;
        }
    }
    
    public DataView DataView { get; }
 
    void RowFilterChanged()
    {
        string Normalize(string filter) => string.IsNullOrWhiteSpace(filter) ? null : filter.Trim();
 
        if (DataView == null)
            return;

        string DetailFilter = Normalize(DetailRowFilter);
        string UserFilter = Normalize(UserRowFilter);

        string Filter;

 
        if (!string.IsNullOrWhiteSpace(DetailFilter) && !string.IsNullOrWhiteSpace(UserFilter))
            Filter = $"({DetailFilter}) AND ({UserFilter})";
        else if (!string.IsNullOrWhiteSpace(DetailFilter))
            Filter = DetailFilter;
        else if (!string.IsNullOrWhiteSpace(UserFilter))
            Filter = UserFilter;
        else
            Filter = string.Empty;

        DataView.RowFilter = Filter;
    }
    
    public string DetailRowFilter
    {
        get => fDetailRowFilter;
        set
        {
            if (fDetailRowFilter != value)
            {
                fDetailRowFilter = value;
                RowFilterChanged();
            }
        }
    }
    public string UserRowFilter
    {
        get => fUserRowFilter;
        set
        {
            if (fUserRowFilter != value)
            {
                fUserRowFilter = value;
                RowFilterChanged();
            }
        }    
        
    }
    /// <summary>
    /// The master table when this is a detail
    /// </summary>
    public MemTable Master
    {
        get => fMaster;
        internal set
        {
            if (!ReferenceEquals(fMaster, value))
            {
                MasterChanging?.Invoke(this, EventArgs.Empty);
                fMaster = value;
                MasterChanged?.Invoke(this, EventArgs.Empty);
                MasterRowChanged();
            }
        }
    }

    internal DetailList Details { get; set; }
    /// <summary>
    /// The Sql statements for the table
    /// </summary>
    public TableSqls SqlStatements { get; set; } = new TableSqls();
    /// <summary>
    /// Gets the StockTables of this instance.
    /// </summary>
    public List<MemTable> Stocks { get; } = new();

    /// <summary>
    /// Returns true if this is a detail table.
    /// </summary>
    public bool IsDetail => Master != null;
    /// <summary>
    /// Returns the "level" of this table. The level is the position
    /// of this table in a larger master-detail-subdetail tree which
    /// is automatically constructed.
    /// </summary>
    public int Level => Master == null ? 0 : Master.Level + 1;
    /// <summary>
    /// Activates or deactivates the master detail relationships.
    /// <para>NOTE: It propagates the activation/de-activation down the tree.</para>
    /// </summary>
    public bool DetailsActive
    {
        get => Details.Active;
        set
        {
            Details.Active = value;
            foreach (var tblDetail in Details)
                tblDetail.DetailsActive = value;
        }
    }

    public bool IsEmpty => Rows.Count == 0;
 
    /// <summary>
    /// Enables or disables the OnTableXXX, OnRowXXXX and OnColumnXXXX methods,
    /// to call or not the base version of the method.
    /// </summary>
    public bool EventsDisabled
    {
        get => EventsDisableCounter > 0;
        set
        {
            if (value)
                EventsDisableCounter++;
            else
                EventsDisableCounter--;

            if (EventsDisableCounter < 0)
                EventsDisableCounter = 0;

            foreach (var tblDetail in Details)
                tblDetail.EventsDisabled = value;
        }
    }
    /// <summary>
    /// When true it automatically generates Guid keys for the primary key column, when a new row is added
    /// </summary>
    public bool AutoGenerateGuidKeys { get; set; }

    public DataRow CurrentRow
    {
        get
        {
            if (!IsValidRow(fCurrentRow) && !IsDetail)
                fCurrentRow = FirstRowOrNull();

            return fCurrentRow;
        }
        set
        {
            DataRow newValue = IsValidRow(value) ? value : null;

            if (ReferenceEquals(fCurrentRow, newValue))
                return;

            fCurrentRow = newValue;
            OnCurrentRowChanged();
        }
    }

    public DataRowView CurrentRowView
    {
        get => CurrentRow == null? null: GetDataRowView(CurrentRow, DataView);
        set => CurrentRow = value == null ? null : value.Row;
    }
    
    // ● events 
    public event EventHandler CurrentRowChanged;
    public event EventHandler MasterChanging;
    public event EventHandler MasterChanged;
    public event EventHandler<DataTableNewRowEventArgs> NewRowAdding;
    public event EventHandler<DataTableNewRowEventArgs> NewRowAdded;
}