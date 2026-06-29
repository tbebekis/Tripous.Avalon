// ● data source info item
/**
 * Base information item for data source sorting and filtering.
 */
tp.DataSourceInfoItem = class {
    // ● constructor
    /**
     * Creates an information item.
     */
    constructor() {
        this.Prop = -1;
        this.DataType = tp.DataType.None;
        this.LookUpTable = null;
        this.LookUpValue = null;
        this.ListValueField = null;
        this.ListDisplayField = null;
    }
};
// ● data table sort item
/**
 * Information item for sorting a data table.
 */
tp.DataTableSortItem = class extends tp.DataSourceInfoItem {
    // ● constructor
    /**
     * Creates a data table sort item.
     */
    constructor() {
        super();
        this.Reverse = false;
        this.GetValueFunc = null;
    }
};
// ● data table filter item
/**
 * Information item for filtering a data table.
 */
tp.DataTableFilterItem = class extends tp.DataSourceInfoItem {
    // ● constructor
    /**
     * Creates a data table filter item.
     */
    constructor() {
        super();
        this.Value = null;
        this.Operator = tp.FilterOp.Equal;
        this.FilterFunc = null;
    }
};
// ● data source info list
/**
 * Base information list for sorting and filtering a data table.
 */
tp.DataSourceInfoList = class {
    // ● constructor
    /**
     * Creates an information list.
     * @param {tp.DataTable} Table The table to operate on.
     */
    constructor(Table) {
        this.Table = Table;
        this.List = [];
        this.fGetFieldValueFuncBind = this.GetFieldValueFunc.bind(this);
    }

    // ● protected
    /**
     * Returns a row field value.
     * @protected
     * @param {tp.DataRow} Row The row.
     * @param {tp.DataSourceInfoItem} Info The information item.
     * @returns {*} Returns the field value.
     */
    GetFieldValueFunc(Row, Info) {
        return Row ? Row.Data[Info.Prop] : null;
    }
    /**
     * Returns a data column index.
     * @protected
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {number} Returns the column index.
     */
    IndexOfColumn(Column) {
        return this.Table ? this.Table.IndexOfColumn(Column) : -1;
    }
    /**
     * Throws when a column does not exist.
     * @protected
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {number} Returns the column index.
     */
    RequireColumnIndex(Column) {
        var Index = this.IndexOfColumn(Column);
        if (Index < 0)
            tp.Throw("Data column not found.");
        return Index;
    }

    // ● properties
    /**
     * Gets the number of information items.
     * @returns {number} Returns the item count.
     */
    get Count() {
        return this.List ? this.List.length : 0;
    }

    // ● public
    /**
     * Clears the list.
     * @returns {void}
     */
    Clear() {
        this.List.length = 0;
    }
    /**
     * Returns the index of an information item associated with a column.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {number} Returns the item index or -1.
     */
    IndexOf(Column) {
        var DataIndex = this.IndexOfColumn(Column);
        var Index;
        for (Index = 0; Index < this.List.length; Index++) {
            if (this.List[Index].Prop === DataIndex)
                return Index;
        }
        return -1;
    }
    /**
     * Returns true when an information item exists for a column.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {boolean} Returns true when an item exists.
     */
    Contains(Column) {
        return this.IndexOf(Column) !== -1;
    }
    /**
     * Finds an information item associated with a column.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {tp.DataSourceInfoItem|null} Returns the item or null.
     */
    Find(Column) {
        var Index = this.IndexOf(Column);
        return Index !== -1 ? this.List[Index] : null;
    }
    /**
     * Removes an information item associated with a column.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @returns {void}
     */
    Remove(Column) {
        var Index = this.IndexOf(Column);
        if (Index !== -1)
            tp.ListRemoveAt(this.List, Index);
    }
};

// ● sort info list
/**
 * A list of information items for sorting a data table.
 */
tp.SortInfoList = class extends tp.DataSourceInfoList {
    // ● protected
    /**
     * Returns a row field value, using lookup display value when configured.
     * @protected
     * @param {tp.DataRow} Row The row.
     * @param {tp.DataTableSortItem} Info The sort item.
     * @returns {*} Returns the field value.
     */
    GetFieldValueFunc(Row, Info) {
        var Result = Row ? Row.Data[Info.Prop] : null;
        var LookUpRow;
        if (Info.LookUpTable) {
            LookUpRow = Info.LookUpTable.FindRow(Info.ListValueField, Result);
            if (LookUpRow)
                Result = LookUpRow.Get(Info.ListDisplayField);
        }
        return Result;
    }

    // ● public
    /**
     * Adds a sort item.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {boolean|null|undefined} Reverse True to sort in descending order.
     * @param {Function|null|undefined} GetValueFunc Optional callback returning the sortable value.
     * @returns {tp.DataTableSortItem} Returns the sort item.
     */
    Add(Column, Reverse, GetValueFunc) {
        return this.Insert(this.List.length, Column, Reverse, GetValueFunc);
    }
    /**
     * Inserts a sort item.
     * @param {number} Index The insert index.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {boolean|null|undefined} Reverse True to sort in descending order.
     * @param {Function|null|undefined} GetValueFunc Optional callback returning the sortable value.
     * @returns {tp.DataTableSortItem} Returns the sort item.
     */
    Insert(Index, Column, Reverse, GetValueFunc) {
        var Item = this.Find(Column);
        var ColumnIndex;
        if (!Item) {
            ColumnIndex = this.RequireColumnIndex(Column);
            Item = new tp.DataTableSortItem();
            Item.Prop = ColumnIndex;
            Item.DataType = this.Table.Columns[ColumnIndex].DataType;
            Item.Reverse = Reverse === true;
            Item.GetValueFunc = GetValueFunc || this.fGetFieldValueFuncBind;
            tp.ListInsert(this.List, Index, Item);
        }
        return Item;
    }
};

// ● filter info list
/**
 * A list of information items for filtering a data table.
 */
tp.FilterInfoList = class extends tp.DataSourceInfoList {
    // ● constructor
    /**
     * Creates a filter information list.
     * @param {tp.DataTable} Table The table to operate on.
     * @param {boolean|null|undefined} OrLogic True to apply OR logic; false to apply AND logic.
     */
    constructor(Table, OrLogic) {
        super(Table);
        this.OrLogic = OrLogic === true;
        this.fFilterFuncBind = this.FilterFunc.bind(this);
    }

    // ● protected
    /**
     * Tests whether a row passes a filter item.
     * @protected
     * @param {tp.DataRow} Row The row.
     * @param {tp.DataTableFilterItem} Info The filter item.
     * @returns {boolean} Returns true when the row passes.
     */
    FilterFunc(Row, Info) {
        var Value = this.GetFieldValueFunc(Row, Info);
        var LookUpRow;
        if (!tp.IsEmpty(Value)) {
            if (Info.DataType === tp.DataType.Date)
                Value = tp.ClearTime(Value);
            if (Info.LookUpTable) {
                LookUpRow = Info.LookUpTable.FindRow(Info.ListValueField, Value);
                if (LookUpRow)
                    Value = LookUpRow.Get(Info.ListDisplayField);
            }
        }
        return tp.FilterOp.Compare(Info.Operator, Value, Info.Value);
    }

    // ● public
    /**
     * Adds a filter item.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {number|null|undefined} Operator The filter operator.
     * @param {*} Value The filter value.
     * @param {Function|null|undefined} FilterFunc Optional callback returning whether the row passes.
     * @returns {tp.DataTableFilterItem} Returns the filter item.
     */
    Add(Column, Operator, Value, FilterFunc) {
        return this.Insert(this.List.length, Column, Operator, Value, FilterFunc);
    }
    /**
     * Inserts a filter item.
     * @param {number} Index The insert index.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {number|null|undefined} Operator The filter operator.
     * @param {*} Value The filter value.
     * @param {Function|null|undefined} FilterFunc Optional callback returning whether the row passes.
     * @returns {tp.DataTableFilterItem} Returns the filter item.
     */
    Insert(Index, Column, Operator, Value, FilterFunc) {
        var Item = this.Find(Column);
        var ColumnIndex;
        if (!Item) {
            ColumnIndex = this.RequireColumnIndex(Column);
            Item = new tp.DataTableFilterItem();
            Item.Prop = ColumnIndex;
            Item.DataType = this.Table.Columns[ColumnIndex].DataType;
            Item.Operator = Operator || tp.FilterOp.Equal;
            Item.FilterFunc = FilterFunc || this.fFilterFuncBind;
            Item.Value = Value;
            tp.ListInsert(this.List, Index, Item);
        }
        return Item;
    }
    /**
     * Finds or adds a filter item.
     * @param {number|string|tp.DataColumn} Column The column index, name, or instance.
     * @param {number|null|undefined} Operator The filter operator.
     * @param {*} Value The filter value.
     * @returns {tp.DataTableFilterItem} Returns the filter item.
     */
    FindOrAdd(Column, Operator, Value) {
        var Item = this.Find(Column);
        if (!Item)
            Item = this.Add(Column, Operator, Value, null);
        else {
            Item.Operator = Operator;
            Item.Value = Value;
        }
        return Item;
    }
};
