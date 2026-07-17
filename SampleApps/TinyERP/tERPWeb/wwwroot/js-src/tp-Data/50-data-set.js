// ● data set
/**
 * Represents a data set, a named collection of data tables.
 */
tp.DataSet = class extends tp.Object {
    // ● constructor
    /**
     * Creates a data set.
     * @param {string|object|null|undefined} NameOrSource The data set name or a source object.
     */
    constructor(NameOrSource) {
        super();
        this.Tables = [];
        this.Name = "";
        if (tp.IsObject(NameOrSource))
            this.Assign(NameOrSource);
        else
            this.Name = tp.IsBlank(NameOrSource) ? tp.NextName("DataSet") : String(NameOrSource);
    }

    // ● properties
    /**
     * Gets the number of tables.
     * @returns {number} Returns the number of tables.
     */
    get TableCount() {
        return this.Tables.length;
    }

    // ● public
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns the data set name.
     */
    toString() {
        return this.Name;
    }
    /**
     * Clears this data set.
     * @returns {void}
     */
    Clear() {
        this.Tables.forEach(function (Table) {
            Table.DataSet = null;
        });
        this.Tables.length = 0;
        this.Name = "";
    }
    /**
     * Returns a plain object used by JSON.stringify().
     * @returns {object} Returns a plain object.
     */
    toJSON() {
        return {
            Name: this.Name,
            Tables: this.Tables.map(function (Table) { return Table.toJSON(); })
        };
    }
    /**
     * Assigns schema and row values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.Name = Source.Name || this.Name;
        this.AssignSchema(Source);
        this.AssignRows(Source.Tables, false);
    }
    /**
     * Assigns only schema values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    AssignSchema(Source) {
        var Index;
        var Table;
        var Name;
        if (!tp.IsObject(Source))
            return;
        Name = Source.Name || this.Name;
        this.Clear();
        this.Name = Name;
        if (tp.IsArray(Source.Tables)) {
            for (Index = 0; Index < Source.Tables.length; Index++) {
                Table = new tp.DataTable(Source.Tables[Index].Name);
                this.AddTable(Table);
                Table.AssignSchema(Source.Tables[Index]);
            }
        }
    }
    /**
     * Assigns row values from source tables to existing tables with the same name.
     * @param {object[]} SourceTables The source tables.
     * @param {boolean} UpdateExisting True to update existing rows by key field.
     * @returns {void}
     */
    AssignRows(SourceTables, UpdateExisting) {
        var Index;
        var SourceTable;
        var Table;
        if (!tp.IsArray(SourceTables))
            return;
        for (Index = 0; Index < SourceTables.length; Index++) {
            SourceTable = SourceTables[Index];
            Table = this.FindTable(SourceTable.Name);
            if (Table) {
                Table.AssignRows(SourceTable.Rows, UpdateExisting);
                Table.AssignDeletedRows(SourceTable.Deleted);
            }
        }
    }
    /**
     * Sets all rows to Unchanged state and clears deleted row lists.
     * @returns {void}
     */
    AcceptChanges() {
        this.Tables.forEach(function (Table) {
            Table.AcceptChanges();
        });
    }
    /**
     * Returns true when any table has added, modified, or deleted rows.
     * @returns {boolean} Returns true when this data set has changes.
     */
    HasChanges() {
        var Index;
        for (Index = 0; Index < this.Tables.length; Index++) {
            if (this.Tables[Index].HasChanges())
                return true;
        }
        return false;
    }
    /**
     * Returns the index of a table or -1.
     * @param {number|string|tp.DataTable} Table The table index, name, or instance.
     * @returns {number} Returns the table index or -1.
     */
    IndexOfTable(Table) {
        var Index;
        if (tp.IsNumber(Table))
            return tp.InRange(this.Tables, Table) ? Table : -1;
        if (tp.IsString(Table)) {
            for (Index = 0; Index < this.Tables.length; Index++) {
                if (tp.IsSameText(this.Tables[Index].Name, Table))
                    return Index;
            }
        } else if (Table instanceof tp.DataTable) {
            return this.Tables.indexOf(Table);
        }
        return -1;
    }
    /**
     * Returns true if a table exists.
     * @param {number|string|tp.DataTable} Table The table index, name, or instance.
     * @returns {boolean} Returns true if the table exists.
     */
    ContainsTable(Table) {
        return this.IndexOfTable(Table) >= 0;
    }
    /**
     * Finds a table.
     * @param {number|string|tp.DataTable} Table The table index, name, or instance.
     * @returns {tp.DataTable|null} Returns the table or null.
     */
    FindTable(Table) {
        var Index = this.IndexOfTable(Table);
        return Index >= 0 ? this.Tables[Index] : null;
    }
    /**
     * Adds a table.
     * @param {string|tp.DataTable|object} NameOrTable The table name, instance, or source object.
     * @returns {tp.DataTable|null} Returns the added table or null.
     */
    AddTable(NameOrTable) {
        var Table;
        Table = NameOrTable instanceof tp.DataTable ? NameOrTable : new tp.DataTable(NameOrTable);
        if (tp.IsBlank(Table.Name))
            return null;
        if (this.ContainsTable(Table.Name))
            return this.FindTable(Table.Name);
        Table.DataSet = this;
        this.Tables.push(Table);
        return Table;
    }
    /**
     * Removes a table.
     * @param {number|string|tp.DataTable} Table The table index, name, or instance.
     * @returns {void}
     */
    RemoveTable(Table) {
        var Item = this.FindTable(Table);
        var Index;
        if (!Item)
            return;
        Index = this.Tables.indexOf(Item);
        this.Tables.splice(Index, 1);
        Item.DataSet = null;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.DataSet.prototype.tpClass = "tp.DataSet";
