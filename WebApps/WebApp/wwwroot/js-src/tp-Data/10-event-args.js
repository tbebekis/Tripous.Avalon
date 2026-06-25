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
