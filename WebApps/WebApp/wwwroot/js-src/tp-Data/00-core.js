// ● data row state
/**
 * Indicates the state of a data row.
 * Numeric values match the .NET System.Data.DataRowState enum.
 * @enum {number}
 */
tp.DataRowState = {
    Detached: 1,
    Unchanged: 2,
    Added: 4,
    Deleted: 8,
    Modified: 16
};
Object.freeze(tp.DataRowState);

// ● data type
/**
 * The data-type of a data field.
 * Numeric values match the C# Tripous.Data.DataFieldType enum.
 * @enum {number}
 */
tp.DataType = {
    /** None. */
    None: 0,
    /** String (nvarchar, varchar). */
    String: 1,
    /** Integer. */
    Integer: 2,
    /** Double (float, double precision, etc). */
    Double: 4,
    /** Decimal (decimal(18, 4)). */
    Decimal: 8,
    /** Decimal (decimal(?, ?)). */
    Decimal_: 0x10,
    /** Date (date). */
    Date: 0x20,
    /** DateTime (datetime, timestamp, etc). */
    DateTime: 0x40,
    /** Boolean (integer always, 1 = true, else false). */
    Boolean: 0x80,
    /** Blob. */
    Blob: 0x100,
    /** Text Blob. */
    TextBlob: 0x200
};
/**
 * Returns the name of a data type value.
 * @param {number} Value The data type value.
 * @returns {string} Returns the data type name, or "None".
 */
tp.DataType.TypeName = function (Value) {
    var Prop;
    for (Prop in tp.DataType) {
        if (Object.prototype.propertyIsEnumerable.call(tp.DataType, Prop) && tp.DataType[Prop] === Value)
            return Prop;
    }
    return "None";
};
/**
 * Returns true if a specified value is a valid data type.
 * @param {number} Value The data type value.
 * @returns {boolean} Returns true if the value is a valid data type.
 */
tp.DataType.IsValid = function (Value) {
    return Value === tp.DataType.String
        || Value === tp.DataType.Integer
        || Value === tp.DataType.Double
        || Value === tp.DataType.Decimal
        || Value === tp.DataType.Decimal_
        || Value === tp.DataType.Date
        || Value === tp.DataType.DateTime
        || Value === tp.DataType.Boolean
        || Value === tp.DataType.Blob
        || Value === tp.DataType.TextBlob;
};
/**
 * Returns true if a specified data type is numeric.
 * @param {number} Value The data type value.
 * @returns {boolean} Returns true if the value is numeric.
 */
tp.DataType.IsNumeric = function (Value) {
    return Value === tp.DataType.Integer || tp.DataType.IsFloat(Value);
};
/**
 * Returns true if a specified data type is float.
 * @param {number} Value The data type value.
 * @returns {boolean} Returns true if the value is float.
 */
tp.DataType.IsFloat = function (Value) {
    return Value === tp.DataType.Double || Value === tp.DataType.Decimal || Value === tp.DataType.Decimal_;
};
/**
 * Returns true if a specified data type is date-time or date.
 * @param {number} Value The data type value.
 * @returns {boolean} Returns true if the value is date-time or date.
 */
tp.DataType.IsDateTime = function (Value) {
    return Value === tp.DataType.DateTime || Value === tp.DataType.Date;
};
/**
 * Returns true if a specified data type is strictly date-time.
 * @param {number} Value The data type value.
 * @returns {boolean} Returns true if the value is date-time.
 */
tp.DataType.IsDateTimeStrict = function (Value) {
    return Value === tp.DataType.DateTime;
};
/**
 * Returns true if a specified data type is strictly date.
 * @param {number} Value The data type value.
 * @returns {boolean} Returns true if the value is date.
 */
tp.DataType.IsDateStrict = function (Value) {
    return Value === tp.DataType.Date;
};
/**
 * Returns true if a specified data type is blob or text blob.
 * @param {number} Value The data type value.
 * @returns {boolean} Returns true if the value is blob or text blob.
 */
tp.DataType.IsBlob = function (Value) {
    return Value === tp.DataType.Blob || Value === tp.DataType.TextBlob;
};
Object.freeze(tp.DataType);
/**
 * Alias for tp.DataType, matching the C# enum name.
 * @type {object}
 */
tp.DataFieldType = tp.DataType;

// ● aggregate type
/**
 * Specifies the aggregate operation to be applied to a data column.
 * Numeric values match the C# Tripous.Data.AggregateType enum.
 * @enum {number}
 */
tp.AggregateType = {
    /** No aggregate operation. */
    None: 0,
    /** Sum of values. */
    Sum: 1,
    /** Average of values. */
    Avg: 2,
    /** Number of values. */
    Count: 3,
    /** Minimum value. */
    Min: 4,
    /** Maximum value. */
    Max: 5
};
Object.freeze(tp.AggregateType);

// ● field flags
/**
 * A list of possible field flags.
 * Numeric values match the C# Tripous.Data.FieldFlags enum.
 * @enum {number}
 */
tp.FieldFlags = {
    /** Container of the flags is set. */
    None: 0,
    /** Must be hidden. */
    Hidden: 1,
    /** Determines whether the field can be modified. */
    ReadOnly: 2,
    /** Concerns controls that display the field. */
    ReadOnlyUI: 4,
    /** The field is editable when inserting only. */
    ReadOnlyEdit: 8,
    /** Can not be null. */
    Required: 0x10,
    /** It is an integer field that must be displayed in a check box control. 0 = false, 1 = true. */
    Boolean: 0x20,
    /** Field is a multiline text. */
    Memo: 0x40,
    /** Field is a large multiline text. */
    LargeMemo: 0x80,
    /** Field is an image, i.e. png, jpg, etc. */
    Image: 0x100,
    /** Field is a path to an image. */
    ImagePath: 0x200,
    /** The field is not used with INSERT or UPDATE statements. */
    NoInsertUpdate: 0x400,
    /** A foreign key field. */
    ForeignKey: 0x800,
    /** The field does NOT exist in the database. It just added to the DataTable schema for some reason. */
    Extra: 0x1000,
    /** Field is searchable and can be a part of a filter. */
    Searchable: 0x2000
};
Object.freeze(tp.FieldFlags);

// ● data mode
/**
 * Indicates the mode of a form or business object.
 * Numeric values match the C# Tripous.Data.DataMode enum.
 * @enum {number}
 */
tp.DataMode = {
    /** None. */
    None: 0,
    /** List. */
    List: 1,
    /** Insert. */
    Insert: 2,
    /** Edit. */
    Edit: 4,
    /** Delete. */
    Delete: 8,
    /** Save. */
    Save: 0x10,
    /** Cancel. */
    Cancel: 0x20
};
Object.freeze(tp.DataMode);

// ● data column type
/**
 * Indicates the data type of a column.
 * Numeric values match the C# Tripous.Data.DataColumnType enum.
 * @enum {number}
 */
tp.DataColumnType = {
    /** No type specified. */
    None: 0x0000,
    /** A text (string) column. */
    Text: 0x0001,
    /** A boolean column. */
    Boolean: 0x0002,
    /** A date-only column. */
    Date: 0x0004,
    /** A date and time column. */
    DateTime: 0x0008,
    /** An integer column. */
    Integer: 0x0010,
    /** A decimal (floating-point) column. */
    Decimal: 0x0020,
    /** A currency column. */
    Currency: 0x0040,
    /** An image column. */
    Image: 0x0080,
    /** A memo (long text) column. */
    Memo: 0x0100,
    /** A lookup column, referring to a value in another table. */
    Lookup: 0x0200
};
Object.freeze(tp.DataColumnType);
