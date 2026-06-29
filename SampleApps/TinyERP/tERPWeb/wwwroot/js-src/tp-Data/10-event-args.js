// ● data event args
/**
 * Event arguments for data events.
 */
tp.DataEventArgs = class extends tp.EventArgs {
    // ● constructor
    /**
     * Creates event arguments for data events.
     * @param {tp.DataColumn|null|undefined} Column The data column, if applicable.
     * @param {tp.DataRow|null|undefined} Row The data row, if applicable.
     * @param {*} OldValue The old value, if applicable.
     * @param {*} NewValue The new value, if applicable.
     */
    constructor(Column, Row, OldValue, NewValue) {
        super("", null);
        this.Column = Column || null;
        this.Row = Row || null;
        this.OldValue = OldValue;
        this.NewValue = NewValue;
    }
};

// ● prototype
/**
 * The data column, if applicable, else null.
 * @type {tp.DataColumn|null}
 */
tp.DataEventArgs.prototype.Column = null;
/**
 * The data row, if applicable, else null.
 * @type {tp.DataRow|null}
 */
tp.DataEventArgs.prototype.Row = null;
/**
 * The column old value, if applicable, else null.
 * @type {*}
 */
tp.DataEventArgs.prototype.OldValue = null;
/**
 * The column new value, if applicable, else null.
 * @type {*}
 */
tp.DataEventArgs.prototype.NewValue = null;

// ● data table event args
/**
 * Event arguments for data table events.
 */
tp.DataTableEventArgs = class extends tp.DataEventArgs {
    // ● constructor
    /**
     * Creates event arguments for data table events.
     * @param {tp.DataColumn|null|undefined} Column The data column, if applicable.
     * @param {tp.DataRow|null|undefined} Row The data row, if applicable.
     * @param {*} OldValue The old value, if applicable.
     * @param {*} NewValue The new value, if applicable.
     */
    constructor(Column, Row, OldValue, NewValue) {
        super(Column, Row, OldValue, NewValue);
    }

    // ● properties
    /**
     * Gets the sender data table.
     * @returns {tp.DataTable|null} Returns the sender data table.
     */
    get Table() {
        return this.Sender;
    }
};

// ● data source event args
/**
 * Event arguments for data source events.
 */
tp.DataSourceEventArgs = class extends tp.DataEventArgs {
    // ● constructor
    /**
     * Creates event arguments for data source events.
     * @param {tp.DataColumn|tp.DataTableEventArgs|null|undefined} ColumnOrSource The data column or a data table event source.
     * @param {tp.DataRow|null|undefined} Row The data row, if applicable.
     * @param {*} OldValue The old value, if applicable.
     * @param {*} NewValue The new value, if applicable.
     */
    constructor(ColumnOrSource, Row, OldValue, NewValue) {
        super(null, null, null, null);
        if (ColumnOrSource instanceof tp.DataTableEventArgs) {
            this.Column = ColumnOrSource.Column;
            this.Row = ColumnOrSource.Row;
            this.OldValue = ColumnOrSource.OldValue;
            this.NewValue = ColumnOrSource.NewValue;
        } else {
            this.Column = ColumnOrSource || null;
            this.Row = Row || null;
            this.OldValue = OldValue;
            this.NewValue = NewValue;
        }
    }

    // ● static
    /**
     * Creates data source event arguments from data table event arguments.
     * @param {tp.DataTableEventArgs} Source The source data table event arguments.
     * @returns {tp.DataSourceEventArgs} Returns the created event arguments.
     */
    static Create(Source) {
        return new tp.DataSourceEventArgs(Source);
    }

    // ● properties
    /**
     * Gets the sender data source.
     * @returns {tp.DataSource|null} Returns the sender data source.
     */
    get DataSource() {
        return this.Sender;
    }
    /**
     * Gets the bound data table.
     * @returns {tp.DataTable|null} Returns the bound data table.
     */
    get Table() {
        return this.DataSource ? this.DataSource.Table : null;
    }
    /**
     * Gets the data source position.
     * @returns {number} Returns the data source position, or -1.
     */
    get Position() {
        return this.DataSource ? this.DataSource.Position : -1;
    }
};
