// ● data column
/**
 * Represents a data column and its field metadata.
 * It carries information coming from both System.Data.DataColumn and Tripous.Data.FieldDef.
 */
tp.DataColumn = class extends tp.Object {
    // ● constructor
    /**
     * Creates a data column.
     * @param {string|object|null|undefined} NameOrSource The column name or a source object.
     * @param {number|string|null|undefined} DataType The data type.
     * @param {number|null|undefined} MaxLength The maximum string length.
     */
    constructor(NameOrSource, DataType, MaxLength) {
        super();
        this.Table = null;
        this.Name = "";
        this.Alias = "";
        this.Title = "";
        this.TitleKey = "";
        this.DataType = tp.DataType.String;
        this.Expression = "";
        this.DefaultValue = null;
        this.MaxLength = -1;
        this.Decimals = -1;
        this.Unique = false;
        this.Flags = tp.FieldFlags.None;
        this.ColumnType = tp.DataColumnType.None;
        this.DisplayFormat = "";
        this.EditFormat = "";
        this.DisplayWidth = 0;
        this.LocalDate = true;
        this.DisplaySeconds = false;
        this.LookupSource = "";
        this.Locator = "";
        this.CodeProvider = "";
        this.SnapshotOf = "";
        this.Group = "General";
        this.ToolTip = "";
        if (tp.IsObject(NameOrSource))
            this.Assign(NameOrSource);
        else {
            this.Name = tp.IsNil(NameOrSource) ? "" : String(NameOrSource);
            this.DataType = tp.DataColumn.NormalizeDataType(DataType);
            this.MaxLength = tp.IsNumber(MaxLength) ? MaxLength : this.MaxLength;
        }
    }

    // ● static
    /**
     * Normalizes a data type value.
     * @param {number|string|null|undefined} Value The value to normalize.
     * @returns {number} Returns a tp.DataType value.
     */
    static NormalizeDataType(Value) {
        if (tp.DataType.IsValid(Value))
            return Value;
        if (tp.IsString(Value)) {
            if (Object.prototype.propertyIsEnumerable.call(tp.DataType, Value) && tp.DataType.IsValid(tp.DataType[Value]))
                return tp.DataType[Value];
            if (Value === "Float")
                return tp.DataType.Double;
            if (Value === "Memo")
                return tp.DataType.TextBlob;
            if (Value === "Unknown")
                return tp.DataType.None;
        }
        return tp.DataType.String;
    }
    /**
     * Normalizes an integer value.
     * @param {*} Value The value to normalize.
     * @param {number} Default The default value.
     * @returns {number} Returns an integer.
     */
    static NormalizeInteger(Value, Default) {
        return tp.IsNumber(Value) ? tp.ToInt(Value) : Default;
    }
    /**
     * Normalizes a string value.
     * @param {*} Value The value to normalize.
     * @param {string} Default The default value.
     * @returns {string} Returns a string.
     */
    static NormalizeString(Value, Default) {
        return tp.IsNil(Value) ? Default : String(Value);
    }
    /**
     * Sets or clears a flag.
     * @param {number} Flags The current flags.
     * @param {number} Flag The flag to set or clear.
     * @param {boolean} Value True to set the flag; false to clear it.
     * @returns {number} Returns the new flags.
     */
    static SetFlag(Flags, Flag, Value) {
        return Value === true ? tp.Bf.Union(Flags, Flag) : tp.Bf.Subtract(Flags, Flag);
    }

    // ● properties
    /**
     * Gets the display title.
     * @returns {string} Returns the display title.
     */
    get DisplayTitle() {
        return !tp.IsBlank(this.Title) ? this.Title : tp.SplitOnUpperCase(this.Alias || this.Name);
    }
    /**
     * Gets the tooltip text.
     * @returns {string} Returns the tooltip text.
     */
    get DisplayToolTip() {
        return !tp.IsBlank(this.ToolTip) ? this.ToolTip : this.DisplayTitle;
    }
    /**
     * Gets or sets a value indicating whether the Required flag is set.
     * @returns {boolean} Returns true when the column is required.
     */
    get IsRequired() {
        return tp.Bf.In(tp.FieldFlags.Required, this.Flags);
    }
    /**
     * Gets or sets a value indicating whether the Required flag is set.
     * @param {boolean} Value True to set the Required flag; false to clear it.
     * @returns {void}
     */
    set IsRequired(Value) {
        this.Flags = tp.DataColumn.SetFlag(this.Flags, tp.FieldFlags.Required, Value);
    }
    /**
     * Gets or sets a value indicating whether the Hidden flag is not set.
     * @returns {boolean} Returns true when the column is visible.
     */
    get IsVisible() {
        return !tp.Bf.In(tp.FieldFlags.Hidden, this.Flags);
    }
    /**
     * Gets or sets a value indicating whether the Hidden flag is not set.
     * @param {boolean} Value True to clear the Hidden flag; false to set it.
     * @returns {void}
     */
    set IsVisible(Value) {
        this.Flags = tp.DataColumn.SetFlag(this.Flags, tp.FieldFlags.Hidden, Value !== true);
    }
    /**
     * Gets or sets a value indicating whether the Hidden flag is set.
     * @returns {boolean} Returns true when the column is hidden.
     */
    get IsHidden() {
        return tp.Bf.In(tp.FieldFlags.Hidden, this.Flags);
    }
    /**
     * Gets or sets a value indicating whether the Hidden flag is set.
     * @param {boolean} Value True to set the Hidden flag; false to clear it.
     * @returns {void}
     */
    set IsHidden(Value) {
        this.Flags = tp.DataColumn.SetFlag(this.Flags, tp.FieldFlags.Hidden, Value);
    }
    /**
     * Gets or sets a value indicating whether the ReadOnly flag is set.
     * @returns {boolean} Returns true when the column is read-only.
     */
    get IsReadOnly() {
        return tp.Bf.In(tp.FieldFlags.ReadOnly, this.Flags);
    }
    /**
     * Gets or sets a value indicating whether the ReadOnly flag is set.
     * @param {boolean} Value True to set the ReadOnly flag; false to clear it.
     * @returns {void}
     */
    set IsReadOnly(Value) {
        this.Flags = tp.DataColumn.SetFlag(this.Flags, tp.FieldFlags.ReadOnly, Value);
    }
    /**
     * Returns true when the ReadOnlyUI flag is set in Flags.
     * @returns {boolean} Returns true when the column is read-only in UI.
     */
    get IsReadOnlyUI() {
        return tp.Bf.In(tp.FieldFlags.ReadOnlyUI, this.Flags);
    }
    /**
     * Returns true when the ReadOnlyEdit flag is set in Flags.
     * @returns {boolean} Returns true when the column is read-only while editing.
     */
    get IsReadOnlyEdit() {
        return tp.Bf.In(tp.FieldFlags.ReadOnlyEdit, this.Flags);
    }
    /**
     * Returns true when the data type is numeric.
     * @returns {boolean} Returns true when the column is numeric.
     */
    get IsNumeric() {
        return tp.DataType.IsNumeric(this.DataType);
    }
    /**
     * Returns true when the data type is integer.
     * @returns {boolean} Returns true when the column is integer.
     */
    get IsInteger() {
        return this.DataType === tp.DataType.Integer;
    }
    /**
     * Returns true when the data type is a float type.
     * @returns {boolean} Returns true when the column is a float type.
     */
    get IsFloat() {
        return tp.DataType.IsFloat(this.DataType);
    }
    /**
     * Returns true when the data type is date or date-time.
     * @returns {boolean} Returns true when the column is date or date-time.
     */
    get IsDateTime() {
        return tp.DataType.IsDateTime(this.DataType);
    }
    /**
     * Returns true when the Boolean flag is set in Flags or DataType is Boolean.
     * @returns {boolean} Returns true when the column is boolean.
     */
    get IsBoolean() {
        return tp.Bf.In(tp.FieldFlags.Boolean, this.Flags) || this.DataType === tp.DataType.Boolean;
    }
    /**
     * Returns true when the Memo flag is set in Flags.
     * @returns {boolean} Returns true when the column is memo.
     */
    get IsMemo() {
        return tp.Bf.In(tp.FieldFlags.Memo, this.Flags);
    }
    /**
     * Returns true when the LargeMemo flag is set in Flags.
     * @returns {boolean} Returns true when the column is large memo.
     */
    get IsLargeMemo() {
        return tp.Bf.In(tp.FieldFlags.LargeMemo, this.Flags);
    }
    /**
     * Returns true when the data type is blob or text blob.
     * @returns {boolean} Returns true when the column is blob or text blob.
     */
    get IsBlob() {
        return tp.DataType.IsBlob(this.DataType);
    }
    /**
     * Returns true when the Image flag is set in Flags.
     * @returns {boolean} Returns true when the column is image.
     */
    get IsImage() {
        return tp.Bf.In(tp.FieldFlags.Image, this.Flags);
    }
    /**
     * Returns true when the ImagePath flag is set in Flags.
     * @returns {boolean} Returns true when the column is image path.
     */
    get IsImagePath() {
        return tp.Bf.In(tp.FieldFlags.ImagePath, this.Flags);
    }
    /**
     * Returns true when the Searchable flag is set in Flags.
     * @returns {boolean} Returns true when the column is searchable.
     */
    get IsSearchable() {
        return tp.Bf.In(tp.FieldFlags.Searchable, this.Flags);
    }
    /**
     * Returns true when the Extra flag is set in Flags.
     * @returns {boolean} Returns true when the column is extra.
     */
    get IsExtraField() {
        return tp.Bf.In(tp.FieldFlags.Extra, this.Flags);
    }
    /**
     * Returns true when the Extra flag is not set in Flags.
     * @returns {boolean} Returns true when the column is native.
     */
    get IsNativeField() {
        return !this.IsExtraField;
    }
    /**
     * Returns true when the ForeignKey flag is set in Flags.
     * @returns {boolean} Returns true when the column is a foreign key.
     */
    get IsForeignKeyField() {
        return tp.Bf.In(tp.FieldFlags.ForeignKey, this.Flags);
    }
    /**
     * Returns true when the NoInsertUpdate flag is set in Flags.
     * @returns {boolean} Returns true when the column is not used with insert or update.
     */
    get IsNoInsertOrUpdate() {
        return tp.Bf.In(tp.FieldFlags.NoInsertUpdate, this.Flags);
    }
    /**
     * Returns true when this column is visible and is not a blob.
     * @returns {boolean} Returns true when the column is bindable.
     */
    get IsBindable() {
        return this.IsVisible && this.DataType !== tp.DataType.None && this.DataType !== tp.DataType.Blob;
    }
    /**
     * Returns true when this column is a lookup field.
     * @returns {boolean} Returns true when the column is a lookup field.
     */
    get IsLookup() {
        return !tp.IsBlank(this.LookupSource);
    }
    /**
     * Returns true when this column is a locator field.
     * @returns {boolean} Returns true when the column is a locator field.
     */
    get IsLocator() {
        return !tp.IsBlank(this.Locator);
    }

    // ● public
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns the column name.
     */
    toString() {
        return this.Name;
    }
    /**
     * Clears this instance.
     * @returns {void}
     */
    Clear() {
        this.Table = null;
    }
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.Name = tp.DataColumn.NormalizeString(Source.Name, this.Name);
        this.Alias = tp.DataColumn.NormalizeString(Source.Alias, this.Alias);
        this.Title = tp.DataColumn.NormalizeString(Source.Title, this.Title);
        this.TitleKey = tp.DataColumn.NormalizeString(Source.TitleKey, this.TitleKey);
        this.DataType = tp.DataColumn.NormalizeDataType(Source.DataType);
        this.Expression = tp.DataColumn.NormalizeString(Source.Expression, this.Expression);
        this.DefaultValue = "DefaultValue" in Source ? Source.DefaultValue : this.DefaultValue;
        this.MaxLength = tp.DataColumn.NormalizeInteger(Source.MaxLength, this.MaxLength);
        this.Decimals = tp.DataColumn.NormalizeInteger(Source.Decimals, this.Decimals);
        this.Unique = Source.Unique === true;
        this.Flags = tp.DataColumn.NormalizeInteger(Source.Flags, this.Flags);
        if (!("Flags" in Source)) {
            if (Source.ReadOnly === true)
                this.IsReadOnly = true;
            if (Source.Visible === false)
                this.IsVisible = false;
            if (Source.Required === true)
                this.IsRequired = true;
        }
        this.ColumnType = tp.DataColumn.NormalizeInteger(Source.ColumnType, this.ColumnType);
        this.DisplayFormat = tp.DataColumn.NormalizeString(Source.DisplayFormat, this.DisplayFormat);
        this.EditFormat = tp.DataColumn.NormalizeString(Source.EditFormat, this.EditFormat);
        this.DisplayWidth = tp.DataColumn.NormalizeInteger(Source.DisplayWidth, this.DisplayWidth);
        this.LocalDate = "LocalDate" in Source ? Source.LocalDate === true : this.LocalDate;
        this.DisplaySeconds = "DisplaySeconds" in Source ? Source.DisplaySeconds === true : this.DisplaySeconds;
        this.LookupSource = tp.DataColumn.NormalizeString(Source.LookupSource, this.LookupSource);
        this.Locator = tp.DataColumn.NormalizeString(Source.Locator, this.Locator);
        this.CodeProvider = tp.DataColumn.NormalizeString(Source.CodeProvider, this.CodeProvider);
        this.SnapshotOf = tp.DataColumn.NormalizeString(Source.SnapshotOf, this.SnapshotOf);
        this.Group = tp.DataColumn.NormalizeString(Source.Group, this.Group);
        this.ToolTip = tp.DataColumn.NormalizeString(Source.ToolTip, this.ToolTip);
    }
    /**
     * Returns a plain object used by JSON.stringify().
     * @returns {object} Returns a plain object.
     */
    toJSON() {
        return {
            Name: this.Name,
            Alias: this.Alias,
            Title: this.Title,
            TitleKey: this.TitleKey,
            DataType: this.DataType,
            Expression: this.Expression,
            DefaultValue: this.DefaultValue,
            MaxLength: this.MaxLength,
            Decimals: this.Decimals,
            Unique: this.Unique,
            Flags: this.Flags,
            ColumnType: this.ColumnType,
            DisplayFormat: this.DisplayFormat,
            EditFormat: this.EditFormat,
            DisplayWidth: this.DisplayWidth,
            LocalDate: this.LocalDate,
            DisplaySeconds: this.DisplaySeconds,
            LookupSource: this.LookupSource,
            Locator: this.Locator,
            CodeProvider: this.CodeProvider,
            SnapshotOf: this.SnapshotOf,
            Group: this.Group,
            ToolTip: this.ToolTip
        };
    }
    /**
     * Returns a specified value of this column formatted as text.
     * @param {*} Value The value to format.
     * @param {boolean|null|undefined} ForList True when formatting for a list or grid.
     * @returns {string|*} Returns the formatted value.
     */
    Format(Value, ForList) {
        return tp.Db.Format(Value, this.DataType, this.ColumnType, ForList, this.Decimals, this.LocalDate, this.DisplaySeconds);
    }
    /**
     * Converts text into a value suitable for this column.
     * @param {*} Value The value to parse.
     * @returns {*} Returns the parsed value.
     */
    Parse(Value) {
        return tp.Db.Parse(Value, this.DataType);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.DataColumn.prototype.tpClass = "tp.DataColumn";
