// ● data source listener interface
/**
 * Interface-like base for objects receiving data source notifications.
 */
tp.IDataSourceListener = class {
    // ● public
    /**
     * Notification called when a row is created.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowCreated(Table, Row) {
    }
    /**
     * Notification called when a row is added.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowAdded(Table, Row) {
    }
    /**
     * Notification called when a row is modified.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @param {tp.DataColumn} Column The data column.
     * @param {*} OldValue The old value.
     * @param {*} NewValue The new value.
     * @returns {void}
     */
    DataSourceRowModified(Table, Row, Column, OldValue, NewValue) {
    }
    /**
     * Notification called when a row is removed.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowRemoved(Table, Row) {
    }
    /**
     * Notification called when position changes.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The current row.
     * @param {number} Position The new position.
     * @returns {void}
     */
    DataSourcePositionChanged(Table, Row, Position) {
    }
    /**
     * Notification called after sorting.
     * @returns {void}
     */
    DataSourceSorted() {
    }
    /**
     * Notification called after filtering.
     * @returns {void}
     */
    DataSourceFiltered() {
    }
    /**
     * Notification called after datasource update.
     * @returns {void}
     */
    DataSourceUpdated() {
    }
};

// ● data source listener
/**
 * Base implementation of a data source listener.
 */
tp.DataSourceListener = class extends tp.Object {
    // ● constructor
    /**
     * Creates a data source listener.
     * @param {object|null|undefined} Owner The owner object.
     */
    constructor(Owner) {
        super();
        this.Owner = Owner || null;
    }

    // ● public
    /**
     * Notification called when a row is created.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowCreated(Table, Row) {
    }
    /**
     * Notification called when a row is added.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowAdded(Table, Row) {
    }
    /**
     * Notification called when a row is modified.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @param {tp.DataColumn} Column The data column.
     * @param {*} OldValue The old value.
     * @param {*} NewValue The new value.
     * @returns {void}
     */
    DataSourceRowModified(Table, Row, Column, OldValue, NewValue) {
    }
    /**
     * Notification called when a row is removed.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The data row.
     * @returns {void}
     */
    DataSourceRowRemoved(Table, Row) {
    }
    /**
     * Notification called when position changes.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataRow} Row The current row.
     * @param {number} Position The new position.
     * @returns {void}
     */
    DataSourcePositionChanged(Table, Row, Position) {
    }
    /**
     * Notification called after sorting.
     * @returns {void}
     */
    DataSourceSorted() {
    }
    /**
     * Notification called after filtering.
     * @returns {void}
     */
    DataSourceFiltered() {
    }
    /**
     * Notification called after datasource update.
     * @returns {void}
     */
    DataSourceUpdated() {
    }
};

// ● data source
/**
 * Data-binding source between a data table and UI controls.
 * Events:
 * - RowCreated
 * - RowAdded
 * - RowModified
 * - RowRemoved
 * - PositionChanged
 * - Sorted
 * - Filtered
 * - Updated
 */
tp.DataSource = class extends tp.Object {
    // ● constructor
    /**
     * Creates a data source.
     * @param {tp.DataTable} Table The source data table.
     */
    constructor(Table) {
        super();
        if (!(Table instanceof tp.DataTable))
            tp.Throw("DataSource requires a DataTable.");
        this.Table = Table;
        this.fPosition = -1;
        this.fForcePosition = false;
        this.fPropagating = false;
        this.fRows = Table.Rows.slice();
        this.fListeners = [];
        this.fSuspendBindingCounter = 0;
        this.fSortInfoList = new tp.SortInfoList(Table);
        this.fFilterInfoList = new tp.FilterInfoList(Table, false);
        this.fMasterSource = null;
        this.fDetails = [];
        this.fMasterKeyField = null;
        this.fDetailKeyField = null;
        this.Table.On("BatchModified", this.Table_BatchModified, this);
        this.Table.On("RowCreated", this.Table_RowCreated, this);
        this.Table.On("RowAdded", this.Table_RowAdded, this);
        this.Table.On("RowModified", this.Table_RowModified, this);
        this.Table.On("RowRemoved", this.Table_RowRemoved, this);
        if (this.fRows.length > 0)
            this.fPosition = 0;
    }

    // ● protected
    /**
     * Returns the current working row list.
     * @protected
     * @returns {tp.DataRow[]} Returns the working rows.
     */
    GetWorkingRows() {
        var MasterKeyIndex;
        var DetailKeyIndex;
        var MasterValue;
        var Result;
        var Index;
        var RowValue;
        if (this.fMasterSource instanceof tp.DataSource) {
            MasterKeyIndex = this.fMasterSource.Table.IndexOfColumn(this.MasterKeyField);
            DetailKeyIndex = this.Table.IndexOfColumn(this.DetailKeyField);
            if (MasterKeyIndex >= 0 && DetailKeyIndex >= 0) {
                MasterValue = this.fMasterSource.Get(MasterKeyIndex);
                if (!tp.IsEmpty(MasterValue)) {
                    Result = [];
                    for (Index = 0; Index < this.Table.Rows.length; Index++) {
                        RowValue = this.Table.Rows[Index].Get(DetailKeyIndex);
                        if (MasterValue === RowValue)
                            Result.push(this.Table.Rows[Index]);
                    }
                    return Result;
                }
            }
        }
        return this.Table.Rows.slice();
    }
    /**
     * Handles master source position changes.
     * @protected
     * @returns {void}
     */
    MasterSource_PositionChanged() {
        this.Update();
        this.Filter();
        this.Sort();
    }

    // ● event handlers
    /**
     * Handles table batch modifications.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {void}
     */
    Table_BatchModified(Args) {
        this.Update();
    }
    /**
     * Handles row creation.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {void}
     */
    Table_RowCreated(Args) {
        var Index;
        if (!this.BindingSuspended && !this.fPropagating) {
            this.fPropagating = true;
            try {
                for (Index = 0; Index < this.fListeners.length; Index++)
                    this.fListeners[Index].DataSourceRowCreated(Args.Table, Args.Row);
                this.OnRowCreated(Args);
            } finally {
                this.fPropagating = false;
            }
        }
    }
    /**
     * Handles row addition.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {void}
     */
    Table_RowAdded(Args) {
        var Index;
        this.fRows.push(Args.Row);
        if (!this.BindingSuspended && !this.fPropagating) {
            this.fPropagating = true;
            try {
                for (Index = 0; Index < this.fListeners.length; Index++)
                    this.fListeners[Index].DataSourceRowAdded(Args.Table, Args.Row);
                this.OnRowAdded(Args);
            } finally {
                this.fPropagating = false;
            }
        }
        if (this.fPosition === -1)
            this.Position = 0;
    }
    /**
     * Handles row modification.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {void}
     */
    Table_RowModified(Args) {
        var Index;
        if (!this.BindingSuspended && !this.fPropagating) {
            this.fPropagating = true;
            try {
                for (Index = 0; Index < this.fListeners.length; Index++)
                    this.fListeners[Index].DataSourceRowModified(Args.Table, Args.Row, Args.Column, Args.OldValue, Args.NewValue);
                this.OnRowModified(Args);
            } finally {
                this.fPropagating = false;
            }
        }
    }
    /**
     * Handles row removal.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {void}
     */
    Table_RowRemoved(Args) {
        var NewPosition = -2;
        var Index;
        if (Args.Row === this.Current && (this.fPosition === 0 || this.fPosition === this.fRows.length - 1))
            NewPosition = this.fRows.length === 1 ? -1 : this.fPosition - 1;
        tp.ListRemove(this.fRows, Args.Row);
        if (!this.BindingSuspended && !this.fPropagating) {
            this.fPropagating = true;
            try {
                for (Index = 0; Index < this.fListeners.length; Index++)
                    this.fListeners[Index].DataSourceRowRemoved(Args.Table, Args.Row);
                this.OnRowRemoved(Args);
            } finally {
                this.fPropagating = false;
            }
        }
        if (NewPosition !== -2)
            this.Position = NewPosition;
    }

    // ● properties
    /**
     * Gets the data source name.
     * @returns {string} Returns the table name.
     */
    get Name() {
        return this.Table.Name;
    }
    /**
     * Gets or sets the current row.
     * @returns {tp.DataRow|null} Returns the current row.
     */
    get Current() {
        return tp.InRange(this.fRows, this.fPosition) ? this.fRows[this.fPosition] : null;
    }
    /**
     * Gets or sets the current row.
     * @param {tp.DataRow} Value The current row.
     * @returns {void}
     */
    set Current(Value) {
        var Index = this.fRows.indexOf(Value);
        if (Index !== -1)
            this.Position = Index;
    }
    /**
     * Gets or sets the current position.
     * @returns {number} Returns the position.
     */
    get Position() {
        return this.fPosition;
    }
    /**
     * Gets or sets the current position.
     * @param {number} Value The position.
     * @returns {void}
     */
    set Position(Value) {
        var CanSet = (Value === -1 && this.fRows.length === 0) || (Value >= 0 && Value <= this.fRows.length - 1);
        var Index;
        if (CanSet && (Value !== this.fPosition || this.fForcePosition)) {
            this.fForcePosition = false;
            this.fPosition = Value;
            if (!this.BindingSuspended && !this.fPropagating) {
                this.fPropagating = true;
                try {
                    for (Index = 0; Index < this.fListeners.length; Index++)
                        this.fListeners[Index].DataSourcePositionChanged(this.Table, this.Current, Value);
                    for (Index = 0; Index < this.fDetails.length; Index++)
                        this.fDetails[Index].MasterSource_PositionChanged();
                    this.OnPositionChanged();
                } finally {
                    this.fPropagating = false;
                }
            }
        }
    }
    /**
     * Gets the current row count.
     * @returns {number} Returns the row count.
     */
    get Count() {
        return this.fRows.length;
    }
    /**
     * Gets the current rows.
     * @returns {tp.DataRow[]} Returns the current rows.
     */
    get Rows() {
        return this.fRows;
    }
    /**
     * Gets or sets whether binding notifications are suspended.
     * @returns {boolean} Returns true while binding is suspended.
     */
    get BindingSuspended() {
        return this.fSuspendBindingCounter > 0;
    }
    /**
     * Gets or sets whether binding notifications are suspended.
     * @param {boolean} Value True to suspend; false to resume.
     * @returns {void}
     */
    set BindingSuspended(Value) {
        this.fSuspendBindingCounter += Value === true ? 1 : -1;
        if (this.fSuspendBindingCounter < 0)
            this.fSuspendBindingCounter = 0;
        if (this.BindingSuspended === false) {
            this.fForcePosition = true;
            this.Position = this.Position;
        }
    }
    /**
     * Returns true when positioned on the first row.
     * @returns {boolean} Returns true when first.
     */
    get IsFirst() {
        return this.Count === 0 || this.Position <= 0;
    }
    /**
     * Returns true when positioned on the last row.
     * @returns {boolean} Returns true when last.
     */
    get IsLast() {
        return this.Count === 0 || this.Position === this.Count - 1;
    }
    /**
     * Gets the sort information list.
     * @returns {tp.SortInfoList} Returns the sort information list.
     */
    get SortInfoList() {
        return this.fSortInfoList;
    }
    /**
     * Gets the filter information list.
     * @returns {tp.FilterInfoList} Returns the filter information list.
     */
    get FilterInfoList() {
        return this.fFilterInfoList;
    }
    /**
     * Gets or sets the master data source.
     * @returns {tp.DataSource|null} Returns the master data source.
     */
    get MasterSource() {
        return this.fMasterSource;
    }
    /**
     * Gets or sets the master data source.
     * @param {tp.DataSource|null} Value The master data source.
     * @returns {void}
     */
    set MasterSource(Value) {
        if (this.fMasterSource instanceof tp.DataSource)
            tp.ListRemove(this.fMasterSource.fDetails, this);
        this.fMasterSource = Value instanceof tp.DataSource ? Value : null;
        if (this.fMasterSource instanceof tp.DataSource)
            this.fMasterSource.fDetails.push(this);
        this.MasterSource_PositionChanged();
    }
    /**
     * Gets or sets the master key field.
     * @returns {string} Returns the master key field.
     */
    get MasterKeyField() {
        return !tp.IsBlank(this.fMasterKeyField) ? this.fMasterKeyField : "Id";
    }
    /**
     * Gets or sets the master key field.
     * @param {string} Value The master key field.
     * @returns {void}
     */
    set MasterKeyField(Value) {
        this.fMasterKeyField = Value;
        this.MasterSource_PositionChanged();
    }
    /**
     * Gets or sets the detail key field.
     * @returns {string} Returns the detail key field.
     */
    get DetailKeyField() {
        return this.fDetailKeyField;
    }
    /**
     * Gets or sets the detail key field.
     * @param {string} Value The detail key field.
     * @returns {void}
     */
    set DetailKeyField(Value) {
        this.fDetailKeyField = Value;
        this.MasterSource_PositionChanged();
    }

    // ● public
    /**
     * Registers a listener.
     * @param {tp.IDataSourceListener} Listener The listener.
     * @returns {void}
     */
    AddDataListener(Listener) {
        if (!tp.ListContains(this.fListeners, Listener))
            this.fListeners.push(Listener);
    }
    /**
     * Removes a listener.
     * @param {tp.IDataSourceListener} Listener The listener.
     * @returns {void}
     */
    RemoveDataListener(Listener) {
        tp.ListRemove(this.fListeners, Listener);
    }
    /**
     * Moves to first row.
     * @returns {void}
     */
    First() {
        if (this.CanFirst())
            this.Position = 0;
    }
    /**
     * Moves to prior row.
     * @returns {void}
     */
    Prior() {
        if (this.CanPrior())
            this.Position = this.fPosition - 1;
    }
    /**
     * Moves to next row.
     * @returns {void}
     */
    Next() {
        if (this.CanNext())
            this.Position = this.fPosition + 1;
    }
    /**
     * Moves to last row.
     * @returns {void}
     */
    Last() {
        if (this.CanLast())
            this.Position = this.fRows.length - 1;
    }
    /**
     * Moves to a specified position.
     * @param {number} NewPosition The new position.
     * @returns {void}
     */
    Move(NewPosition) {
        if (this.CanMoveTo(NewPosition))
            this.Position = NewPosition;
    }
    /**
     * Returns true when a position can be selected.
     * @param {number} NewPosition The new position.
     * @returns {boolean} Returns true when movement is possible.
     */
    CanMoveTo(NewPosition) {
        return NewPosition >= 0 && NewPosition <= this.fRows.length - 1;
    }
    /**
     * Returns true when first row movement is possible.
     * @returns {boolean} Returns true when movement is possible.
     */
    CanFirst() {
        return !this.IsFirst && this.CanMoveTo(0);
    }
    /**
     * Returns true when next row movement is possible.
     * @returns {boolean} Returns true when movement is possible.
     */
    CanNext() {
        return !this.IsLast && this.CanMoveTo(this.fPosition + 1);
    }
    /**
     * Returns true when prior row movement is possible.
     * @returns {boolean} Returns true when movement is possible.
     */
    CanPrior() {
        return !this.IsFirst && this.CanMoveTo(this.fPosition - 1);
    }
    /**
     * Returns true when last row movement is possible.
     * @returns {boolean} Returns true when movement is possible.
     */
    CanLast() {
        return !this.IsLast && this.CanMoveTo(this.fRows.length - 1);
    }
    /**
     * Clears table rows.
     * @returns {void}
     */
    ClearRows() {
        this.Table.ClearRows();
    }
    /**
     * Adds an empty row.
     * @returns {tp.DataRow|null} Returns the added row.
     */
    AddEmptyRow() {
        return this.Table.AddEmptyRow();
    }
    /**
     * Adds a row.
     * @param {...*} Data The row data.
     * @returns {tp.DataRow|null} Returns the added row.
     */
    AddNew(...Data) {
        return this.Table.AddRow(...Data);
    }
    /**
     * Creates a new row without adding it.
     * @param {object[]|object|null|undefined} Data The row data.
     * @returns {tp.DataRow} Returns the new row.
     */
    NewRow(Data) {
        return this.Table.NewRow(Data);
    }
    /**
     * Returns a row value.
     * @param {tp.DataRow} Row The row.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Default The default value.
     * @returns {*} Returns the value.
     */
    GetValue(Row, Column, Default) {
        return Row ? Row.Get(Column, Default) : Default;
    }
    /**
     * Sets a row value.
     * @param {tp.DataRow} Row The row.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Value The value.
     * @returns {void}
     */
    SetValue(Row, Column, Value) {
        if (Row)
            Row.Set(Column, Value);
    }
    /**
     * Returns a current row value.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Default The default value.
     * @returns {*} Returns the value.
     */
    Get(Column, Default) {
        return this.GetValue(this.Current, Column, Default);
    }
    /**
     * Sets a current row value.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Value The value.
     * @returns {void}
     */
    Set(Column, Value) {
        this.SetValue(this.Current, Column, Value);
    }
    /**
     * Sorts current rows.
     * @returns {void}
     */
    Sort() {
        var Index;
        if (this.SortInfoList && this.SortInfoList.Count > 0) {
            tp.ListSort(this.fRows, this.SortInfoList.List);
            if (!this.BindingSuspended) {
                for (Index = 0; Index < this.fListeners.length; Index++)
                    this.fListeners[Index].DataSourceSorted();
                this.OnSorted();
            }
        }
    }
    /**
     * Filters current rows.
     * @returns {void}
     */
    Filter() {
        var Index;
        var Rows;
        if (this.FilterInfoList) {
            Rows = this.GetWorkingRows();
            this.fRows = this.FilterInfoList.Count > 0 ? tp.ListFilter(Rows, this.FilterInfoList.List, this.FilterInfoList.OrLogic) : Rows;
            if (this.SortInfoList && this.SortInfoList.Count > 0)
                tp.ListSort(this.fRows, this.SortInfoList.List);
            if (this.fRows.length === 0)
                this.fPosition = -1;
            else if (!tp.InRange(this.fRows, this.Position))
                this.Position = 0;
            if (!this.BindingSuspended) {
                for (Index = 0; Index < this.fListeners.length; Index++)
                    this.fListeners[Index].DataSourceFiltered();
                this.OnFiltered();
            }
        }
    }
    /**
     * Cancels any active filter.
     * @returns {void}
     */
    CancelFilter() {
        var Index;
        this.fRows = this.GetWorkingRows();
        if (this.SortInfoList && this.SortInfoList.Count > 0)
            tp.ListSort(this.fRows, this.SortInfoList.List);
        if (this.fRows.length === 0)
            this.fPosition = -1;
        else if (!tp.InRange(this.fRows, this.Position))
            this.Position = 0;
        if (!this.BindingSuspended) {
            for (Index = 0; Index < this.fListeners.length; Index++)
                this.fListeners[Index].DataSourceFiltered();
        }
    }
    /**
     * Updates the current rows from the table.
     * @returns {void}
     */
    Update() {
        var Index;
        var Position = this.Position;
        this.fRows = this.GetWorkingRows();
        this.fPosition = -1;
        if (this.fRows.length > 0)
            this.Position = 0;
        if (!this.BindingSuspended && !this.fPropagating) {
            this.fPropagating = true;
            try {
                for (Index = 0; Index < this.fListeners.length; Index++)
                    this.fListeners[Index].DataSourceUpdated();
                this.OnUpdated();
            } finally {
                this.fPropagating = false;
            }
        }
        if (Position >= 0 && tp.InRange(this.fRows, Position))
            this.Position = Position;
    }
    /**
     * Sorts on a column.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {boolean|null|undefined} Reverse True to sort in descending order.
     * @returns {void}
     */
    SortOn(Column, Reverse) {
        var ColumnIndex = this.Table.IndexOfColumn(Column);
        if (ColumnIndex >= 0) {
            this.SortInfoList.Clear();
            this.SortInfoList.Add(ColumnIndex, Reverse === true);
            this.Sort();
        }
    }
    /**
     * Filters on a column using equality.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {*} Value The filter value.
     * @returns {void}
     */
    FilterOn(Column, Value) {
        var ColumnIndex = this.Table.IndexOfColumn(Column);
        if (ColumnIndex >= 0) {
            this.FilterInfoList.Clear();
            this.FilterInfoList.FindOrAdd(ColumnIndex, tp.FilterOp.Equal, Value);
            this.Filter();
        }
    }

    // ● event triggers
    /**
     * Triggers RowCreated.
     * @param {tp.DataTableEventArgs} Source The source event arguments.
     * @returns {void}
     */
    OnRowCreated(Source) {
        this.Trigger("RowCreated", new tp.DataSourceEventArgs(Source));
    }
    /**
     * Triggers RowAdded.
     * @param {tp.DataTableEventArgs} Source The source event arguments.
     * @returns {void}
     */
    OnRowAdded(Source) {
        this.Trigger("RowAdded", new tp.DataSourceEventArgs(Source));
    }
    /**
     * Triggers RowModified.
     * @param {tp.DataTableEventArgs} Source The source event arguments.
     * @returns {void}
     */
    OnRowModified(Source) {
        this.Trigger("RowModified", new tp.DataSourceEventArgs(Source));
    }
    /**
     * Triggers RowRemoved.
     * @param {tp.DataTableEventArgs} Source The source event arguments.
     * @returns {void}
     */
    OnRowRemoved(Source) {
        this.Trigger("RowRemoved", new tp.DataSourceEventArgs(Source));
    }
    /**
     * Triggers PositionChanged.
     * @returns {void}
     */
    OnPositionChanged() {
        this.Trigger("PositionChanged", new tp.DataSourceEventArgs());
    }
    /**
     * Triggers Sorted.
     * @returns {void}
     */
    OnSorted() {
        this.Trigger("Sorted", new tp.DataSourceEventArgs());
    }
    /**
     * Triggers Filtered.
     * @returns {void}
     */
    OnFiltered() {
        this.Trigger("Filtered", new tp.DataSourceEventArgs());
    }
    /**
     * Triggers Updated.
     * @returns {void}
     */
    OnUpdated() {
        this.Trigger("Updated", new tp.DataSourceEventArgs());
    }
};
