// ● data table
/**
 * Represents a data table.
 * Events:
 * - BatchModified
 * - RowsClearing
 * - RowsCleared
 * - ColumnAdded
 * - ColumnRemoved
 * - RowCreated
 * - RowAdding
 * - RowAdded
 * - RowRemoving
 * - RowRemoved
 * - RowModifying
 * - RowModified
 */
tp.DataTable = class extends tp.Object {
    // ● constructor
    /**
     * Creates a data table.
     * @param {string|object|null|undefined} NameOrSource The table name or a source object.
     */
    constructor(NameOrSource) {
        super();
        this.DataSet = null;
        this.Columns = [];
        this.Rows = [];
        this.Deleted = [];
        this.Details = [];
        this.StockTables = [];
        this.fName = "";
        this.fBatchCounter = 0;
        this.fKeyField = "Id";
        this.fMasterField = "Id";
        this.fDetailField = "Id";
        this.fKeyFieldIndex = -1;
        this.MasterTableName = "";
        this.AutoGenerateGuidKeys = true;
        if (tp.IsObject(NameOrSource))
            this.Assign(NameOrSource);
        else
            this.Name = tp.IsBlank(NameOrSource) ? tp.NextName("DataTable") : String(NameOrSource);
    }

    // ● static
    /**
     * Normalizes a field name.
     * @param {*} Value The source value.
     * @param {string} DefaultField The default field name.
     * @returns {string} Returns a field name.
     */
    static NormalizeFieldName(Value, DefaultField) {
        DefaultField = tp.IsBlank(DefaultField) ? "Id" : String(DefaultField);
        if (tp.IsArray(Value))
            Value = Value.length > 0 ? Value[0] : null;
        return tp.IsBlank(Value) ? DefaultField : String(Value);
    }

    // ● properties
    /**
     * Gets or sets the table name.
     * @returns {string} Returns the table name.
     */
    get Name() {
        return this.fName;
    }
    /**
     * Gets or sets the table name.
     * @param {string} Value The table name.
     * @returns {void}
     */
    set Name(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (Value !== this.fName) {
            this.Trigger("NameChanging");
            this.fName = Value;
            this.Trigger("NameChanged");
        }
    }
    /**
     * Gets the number of columns.
     * @returns {number} Returns the number of columns.
     */
    get ColumnCount() {
        return this.Columns.length;
    }
    /**
     * Gets the number of rows.
     * @returns {number} Returns the number of rows.
     */
    get RowCount() {
        return this.Rows.length;
    }
    /**
     * Gets or sets the first key field name.
     * @returns {string} Returns the first key field name.
     */
    get KeyField() {
        return !tp.IsBlank(this.fKeyField) ? this.fKeyField : "Id";
    }
    /**
     * Gets or sets the first key field name.
     * @param {string} Value The first key field name.
     * @returns {void}
     */
    set KeyField(Value) {
        this.fKeyField = tp.DataTable.NormalizeFieldName(Value, "Id");
        this.fKeyFieldIndex = -1;
    }
    /**
     * Gets or sets the first master field name.
     * @returns {string} Returns the first master field name.
     */
    get MasterField() {
        return !tp.IsBlank(this.fMasterField) ? this.fMasterField : "Id";
    }
    /**
     * Gets or sets the first master field name.
     * @param {string} Value The first master field name.
     * @returns {void}
     */
    set MasterField(Value) {
        this.fMasterField = tp.DataTable.NormalizeFieldName(Value, "Id");
    }
    /**
     * Gets or sets the first detail field name.
     * @returns {string} Returns the first detail field name.
     */
    get DetailField() {
        return !tp.IsBlank(this.fDetailField) ? this.fDetailField : this.KeyField;
    }
    /**
     * Gets or sets the first detail field name.
     * @param {string} Value The first detail field name.
     * @returns {void}
     */
    set DetailField(Value) {
        this.fDetailField = tp.DataTable.NormalizeFieldName(Value, this.KeyField);
    }
    /**
     * Gets the first key field column index.
     * @returns {number} Returns the key field column index, or -1.
     */
    get KeyFieldIndex() {
        if (this.fKeyFieldIndex < 0)
            this.fKeyFieldIndex = this.IndexOfColumn(this.KeyField);
        return this.fKeyFieldIndex;
    }
    /**
     * Gets the first key field column.
     * @returns {tp.DataColumn|null} Returns the key field column or null.
     */
    get KeyFieldColumn() {
        return this.KeyFieldIndex >= 0 ? this.Columns[this.KeyFieldIndex] : null;
    }
    /**
     * Gets the master data table.
     * @returns {tp.DataTable|null} Returns the master table or null.
     */
    get MasterTable() {
        return this.DataSet && tp.IsFunction(this.DataSet.FindTable) ? this.DataSet.FindTable(this.MasterTableName) : null;
    }
    /**
     * Gets or sets a value indicating whether the table is in batch mode.
     * @returns {boolean} Returns true while in batch mode.
     */
    get Batch() {
        return this.fBatchCounter > 0;
    }
    /**
     * Gets or sets a value indicating whether the table is in batch mode.
     * @param {boolean} Value True to enter batch mode; false to leave it.
     * @returns {void}
     */
    set Batch(Value) {
        if (Value === true)
            this.fBatchCounter++;
        else if (this.fBatchCounter > 0)
            this.fBatchCounter--;
        if (this.fBatchCounter === 0)
            this.OnBatchModified();
    }

    // ● public
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns the table name.
     */
    toString() {
        return this.Name;
    }
    /**
     * Clears this table.
     * @returns {void}
     */
    Clear() {
        this.Columns.length = 0;
        this.Rows.length = 0;
        this.Deleted.length = 0;
        this.Details.length = 0;
        this.StockTables.length = 0;
        this.fKeyFieldIndex = -1;
    }
    /**
     * Returns a plain object used by JSON.stringify().
     * @returns {object} Returns a plain object.
     */
    toJSON() {
        return {
            Name: this.Name,
            KeyField: this.KeyField,
            MasterField: this.MasterField,
            DetailField: this.DetailField,
            MasterTableName: this.MasterTableName,
            AutoGenerateGuidKeys: this.AutoGenerateGuidKeys,
            Details: this.Details.slice(),
            StockTables: this.StockTables.slice(),
            Columns: this.Columns.map(function (Column) { return Column.toJSON(); }),
            Rows: this.Rows.map(function (Row) { return Row.toJSON(); }),
            Deleted: this.Deleted.map(function (Row) { return Row.toJSON(); })
        };
    }
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.Name = Source.Name || this.Name;
        this.AssignSchema(Source);
        this.AssignRows(Source.Rows, false);
        this.AssignDeletedRows(Source.Deleted);
    }
    /**
     * Assigns schema values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    AssignSchema(Source) {
        var Index;
        var Column;
        if (!tp.IsObject(Source))
            return;
        this.Columns.length = 0;
        this.Deleted.length = 0;
        this.Details = tp.IsArray(Source.Details) ? Source.Details.slice() : [];
        this.StockTables = tp.IsArray(Source.StockTables) ? Source.StockTables.slice() : [];
        this.Name = Source.Name || this.Name;
        this.KeyField = tp.DataTable.NormalizeFieldName(Source.KeyField || Source.KeyFields || Source.PrimaryKeyField, this.KeyField);
        this.MasterField = tp.DataTable.NormalizeFieldName(Source.MasterField || Source.MasterFields || Source.MasterKeyField, this.MasterField);
        this.DetailField = tp.DataTable.NormalizeFieldName(Source.DetailField || Source.DetailFields || Source.DetailKeyField, this.DetailField);
        this.fKeyFieldIndex = -1;
        this.MasterTableName = tp.IsNil(Source.MasterTableName) ? this.MasterTableName : String(Source.MasterTableName);
        this.AutoGenerateGuidKeys = Source.AutoGenerateGuidKeys !== false;
        if (tp.IsArray(Source.Columns)) {
            for (Index = 0; Index < Source.Columns.length; Index++) {
                Column = new tp.DataColumn(Source.Columns[Index]);
                this.AddColumn(Column);
            }
        }
    }
    /**
     * Assigns row values from a source row array.
     * @param {object[]} SourceRows The source rows.
     * @param {boolean} UpdateExisting True to update existing rows by key field.
     * @returns {void}
     */
    AssignRows(SourceRows, UpdateExisting) {
        var Index;
        var SourceRow;
        var Row;
        if (!tp.IsArray(SourceRows))
            return;
        this.Rows.length = 0;
        for (Index = 0; Index < SourceRows.length; Index++) {
            SourceRow = SourceRows[Index];
            Row = UpdateExisting === true ? this.FindRowByKey(SourceRow) : null;
            if (Row)
                Row.Assign(SourceRow);
            else {
                Row = new tp.DataRow(this, SourceRow);
                Row.State = tp.DataRow.NormalizeState(SourceRow ? SourceRow.State : tp.DataRowState.Unchanged);
                this.Rows.push(Row);
            }
        }
    }
    /**
     * Assigns deleted row values from a source row array.
     * @param {object[]} SourceRows The source rows.
     * @returns {void}
     */
    AssignDeletedRows(SourceRows) {
        var Index;
        var Row;
        this.Deleted.length = 0;
        if (!tp.IsArray(SourceRows))
            return;
        for (Index = 0; Index < SourceRows.length; Index++) {
            Row = new tp.DataRow(this, SourceRows[Index]);
            Row.State = tp.DataRowState.Deleted;
            this.Deleted.push(Row);
        }
    }
    /**
     * Finds a row by key field using the data in a source row.
     * @param {object} SourceRow The source row.
     * @returns {tp.DataRow|null} Returns a row or null.
     */
    FindRowByKey(SourceRow) {
        var KeyValue;
        if (!SourceRow || this.KeyFieldIndex < 0 || !tp.IsArray(SourceRow.Data))
            return null;
        KeyValue = SourceRow.Data[this.KeyFieldIndex];
        return this.FindRow(this.KeyFieldIndex, KeyValue);
    }
    /**
     * Creates a new table with the same schema and no rows.
     * @returns {tp.DataTable} Returns the cloned table.
     */
    Clone() {
        var Result = new tp.DataTable();
        Result.AssignSchema(this);
        return Result;
    }
    /**
     * Creates a new table with the same schema and data.
     * @returns {tp.DataTable} Returns the copied table.
     */
    Copy() {
        var Result = new tp.DataTable();
        Result.Assign(this);
        return Result;
    }
    /**
     * Returns the index of a column or -1.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {number} Returns the column index or -1.
     */
    IndexOfColumn(Column) {
        var Index;
        if (tp.IsNumber(Column))
            return tp.InRange(this.Columns, Column) ? Column : -1;
        if (tp.IsString(Column)) {
            for (Index = 0; Index < this.Columns.length; Index++) {
                if (tp.IsSameText(this.Columns[Index].Name, Column))
                    return Index;
            }
        } else if (Column instanceof tp.DataColumn) {
            return this.Columns.indexOf(Column);
        }
        return -1;
    }
    /**
     * Returns true if a column exists.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {boolean} Returns true if the column exists.
     */
    ContainsColumn(Column) {
        return this.IndexOfColumn(Column) >= 0;
    }
    /**
     * Finds a column.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {tp.DataColumn|null} Returns the column or null.
     */
    FindColumn(Column) {
        var Index = this.IndexOfColumn(Column);
        return Index >= 0 ? this.Columns[Index] : null;
    }
    /**
     * Adds a column.
     * @param {string|tp.DataColumn|object} NameOrColumn The column name, instance, or source object.
     * @param {number|string|null|undefined} DataType The data type.
     * @param {number|null|undefined} MaxLength The maximum length.
     * @param {number|null|undefined} ColumnIndex The insertion index.
     * @returns {tp.DataColumn|null} Returns the added column or null.
     */
    AddColumn(NameOrColumn, DataType, MaxLength, ColumnIndex) {
        var Column;
        var Index;
        Column = NameOrColumn instanceof tp.DataColumn ? NameOrColumn : new tp.DataColumn(NameOrColumn, DataType, MaxLength);
        if (tp.IsBlank(Column.Name))
            return null;
        if (this.ContainsColumn(Column.Name))
            return this.FindColumn(Column.Name);
        Column.Table = this;
        if (tp.IsNumber(ColumnIndex) && ColumnIndex >= 0) {
            if (!tp.InRange(this.Columns, ColumnIndex) && ColumnIndex !== this.Columns.length)
                tp.Throw("Cannot insert table column. Invalid column index: " + ColumnIndex);
            this.Columns.splice(ColumnIndex, 0, Column);
        } else {
            this.Columns.push(Column);
        }
        Index = this.Columns.indexOf(Column);
        this.Rows.forEach(function (Row) {
            Row.Data.splice(Index, 0, null);
        });
        this.fKeyFieldIndex = -1;
        this.OnColumnAdded(Column);
        return Column;
    }
    /**
     * Removes a column.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {void}
     */
    RemoveColumn(Column) {
        var Col = this.FindColumn(Column);
        var Index;
        if (!Col)
            return;
        Index = this.Columns.indexOf(Col);
        this.Columns.splice(Index, 1);
        this.Rows.forEach(function (Row) {
            Row.Data.splice(Index, 1);
        });
        Col.Table = null;
        this.fKeyFieldIndex = -1;
        this.OnColumnRemoved(Col);
    }
    /**
     * Sets all rows to Unchanged state and clears the deleted row list.
     * @returns {void}
     */
    AcceptChanges() {
        this.Rows.forEach(function (Row) {
            Row.AcceptChanges();
        });
        this.Deleted.length = 0;
    }
    /**
     * Removes all rows and clears deleted rows.
     * @returns {void}
     */
    ClearRows() {
        var List;
        if (this.Rows.length === 0)
            return;
        this.OnRowsClearing();
        List = this.Rows.slice();
        List.forEach(function (Row) {
            Row.State = tp.DataRowState.Deleted;
        });
        this.Deleted = this.Deleted.concat(List);
        this.Rows.length = 0;
        this.OnRowsCleared();
    }
    /**
     * Creates a new row without adding it.
     * @param {object[]|object|null|undefined} Source The row source.
     * @returns {tp.DataRow} Returns the new row.
     */
    NewRow(Source) {
        return new tp.DataRow(this, Source);
    }
    /**
     * Adds a row to this table.
     * @param {...*} Data The row, data array, source object, or value arguments.
     * @returns {tp.DataRow|null} Returns the added row or null.
     */
    AddRow(...Data) {
        var Row;
        if (Data.length === 0)
            Row = this.NewRow();
        else if (Data[0] instanceof tp.DataRow)
            Row = Data[0];
        else if (tp.IsArray(Data[0]) || tp.IsObject(Data[0]))
            Row = this.NewRow(Data[0]);
        else
            Row = this.NewRow(Data);
        if (Row && Row.Table === this && Row.State === tp.DataRowState.Detached) {
            if (!this.Batch)
                this.OnRowAdding(Row);
            this.Rows.push(Row);
            Row.State = tp.DataRowState.Added;
            if (!this.Batch)
                this.OnRowAdded(Row);
            return Row;
        }
        return null;
    }
    /**
     * Creates and adds an empty row.
     * @returns {tp.DataRow|null} Returns the added row or null.
     */
    AddEmptyRow() {
        return this.AddRow();
    }
    /**
     * Removes a row.
     * @param {number|tp.DataRow} IndexOrRow The row index or row instance.
     * @returns {void}
     */
    RemoveRow(IndexOrRow) {
        var Index = -1;
        var Row = null;
        if (IndexOrRow instanceof tp.DataRow) {
            Row = IndexOrRow;
            Index = this.Rows.indexOf(Row);
        } else if (tp.IsNumber(IndexOrRow) && tp.InRange(this.Rows, IndexOrRow)) {
            Index = IndexOrRow;
            Row = this.Rows[Index];
        }
        if (Index < 0 || !Row)
            return;
        if (!this.Batch)
            this.OnRowRemoving(Row);
        this.Rows.splice(Index, 1);
        this.Deleted.push(Row);
        Row.State = tp.DataRowState.Deleted;
        if (!this.Batch)
            this.OnRowRemoved(Row);
    }
    /**
     * Finds a row by column value.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Value The value to find.
     * @returns {tp.DataRow|null} Returns a row or null.
     */
    FindRow(Column, Value) {
        var Index = this.IndexOfColumn(Column);
        var RowIndex;
        if (Index < 0)
            return null;
        for (RowIndex = 0; RowIndex < this.Rows.length; RowIndex++) {
            if (this.Rows[RowIndex].Data[Index] === Value)
                return this.Rows[RowIndex];
        }
        return null;
    }
    /**
     * Finds a row by column value.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Value The value to find.
     * @returns {tp.DataRow|null} Returns a row or null.
     */
    Locate(Column, Value) {
        return this.FindRow(Column, Value);
    }
    /**
     * Performs a lookup and returns a value.
     * @param {number|string|tp.DataColumn} KeyColumn The key column.
     * @param {*} KeyValue The key value.
     * @param {number|string|tp.DataColumn} ResultColumn The result column.
     * @returns {*} Returns the result value or null.
     */
    LookUp(KeyColumn, KeyValue, ResultColumn) {
        var Row = this.FindRow(KeyColumn, KeyValue);
        return Row ? Row.Get(ResultColumn, null) : null;
    }
    /**
     * Selects rows matching a column value.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Value The value to match.
     * @returns {tp.DataRow[]} Returns matching rows.
     */
    SelectRows(Column, Value) {
        var Result = [];
        var Index = this.IndexOfColumn(Column);
        if (Index < 0)
            return Result;
        this.Rows.forEach(function (Row) {
            if (Row.GetByIndex(Index) === Value)
                Result.push(Row);
        });
        return Result;
    }
    /**
     * Returns rows as plain objects for UI code.
     * @returns {object[]} Returns row object projections.
     */
    RowsToObjectArray() {
        return this.Rows.map(function (Row) {
            return Row.ToObject();
        });
    }
    /**
     * Returns a copy of the row list.
     * @returns {tp.DataRow[]} Returns the rows.
     */
    RowsToList() {
        return this.Rows.slice();
    }
    /**
     * Creates rows from a list of plain objects.
     * @param {object[]} ObjectList The source object list.
     * @param {string[]} ExcludeFieldList Optional field names to exclude.
     * @returns {void}
     */
    FromObjectList(ObjectList, ExcludeFieldList) {
        var Row;
        if (!tp.IsArray(ObjectList))
            return;
        ObjectList.forEach(function (SourceObject) {
            Row = this.AddEmptyRow();
            Row.FromObject(SourceObject, ExcludeFieldList);
        }, this);
    }
    /**
     * Creates a list of plain objects.
     * @returns {object[]} Returns row object projections.
     */
    ToObjectList() {
        return this.RowsToObjectArray();
    }

    // ● event triggers
    /**
     * Triggers the BatchModified event.
     * @returns {void}
     */
    OnBatchModified() {
        this.Trigger("BatchModified", new tp.DataTableEventArgs());
    }
    /**
     * Triggers the RowsClearing event.
     * @returns {void}
     */
    OnRowsClearing() {
        this.Trigger("RowsClearing", new tp.DataTableEventArgs());
    }
    /**
     * Triggers the RowsCleared event.
     * @returns {void}
     */
    OnRowsCleared() {
        this.Trigger("RowsCleared", new tp.DataTableEventArgs());
    }
    /**
     * Triggers the ColumnAdded event.
     * @param {tp.DataColumn} Column The column.
     * @returns {void}
     */
    OnColumnAdded(Column) {
        this.Trigger("ColumnAdded", new tp.DataTableEventArgs(Column));
    }
    /**
     * Triggers the ColumnRemoved event.
     * @param {tp.DataColumn} Column The column.
     * @returns {void}
     */
    OnColumnRemoved(Column) {
        this.Trigger("ColumnRemoved", new tp.DataTableEventArgs(Column));
    }
    /**
     * Triggers the RowCreated event.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    OnRowCreated(Row) {
        if (!this.Batch)
            this.Trigger("RowCreated", new tp.DataTableEventArgs(null, Row));
    }
    /**
     * Triggers the RowAdding event.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    OnRowAdding(Row) {
        if (!this.Batch)
            this.Trigger("RowAdding", new tp.DataTableEventArgs(null, Row));
    }
    /**
     * Triggers the RowAdded event.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    OnRowAdded(Row) {
        if (!this.Batch)
            this.Trigger("RowAdded", new tp.DataTableEventArgs(null, Row));
    }
    /**
     * Triggers the RowRemoving event.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    OnRowRemoving(Row) {
        if (!this.Batch)
            this.Trigger("RowRemoving", new tp.DataTableEventArgs(null, Row));
    }
    /**
     * Triggers the RowRemoved event.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    OnRowRemoved(Row) {
        if (!this.Batch)
            this.Trigger("RowRemoved", new tp.DataTableEventArgs(null, Row));
    }
    /**
     * Triggers the RowModifying event.
     * @param {tp.DataRow} Row The row.
     * @param {tp.DataColumn} Column The column.
     * @param {*} OldValue The old value.
     * @param {*} NewValue The new value.
     * @returns {void}
     */
    OnRowModifying(Row, Column, OldValue, NewValue) {
        if (!this.Batch && Row.State !== tp.DataRowState.Detached)
            this.Trigger("RowModifying", new tp.DataTableEventArgs(Column, Row, OldValue, NewValue));
    }
    /**
     * Triggers the RowModified event.
     * @param {tp.DataRow} Row The row.
     * @param {tp.DataColumn} Column The column.
     * @param {*} OldValue The old value.
     * @param {*} NewValue The new value.
     * @returns {void}
     */
    OnRowModified(Row, Column, OldValue, NewValue) {
        if (!this.Batch && Row.State !== tp.DataRowState.Detached)
            this.Trigger("RowModified", new tp.DataTableEventArgs(Column, Row, OldValue, NewValue));
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.DataTable.prototype.tpClass = "tp.DataTable";
