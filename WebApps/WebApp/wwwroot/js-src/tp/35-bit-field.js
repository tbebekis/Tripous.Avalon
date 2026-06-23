// ● bit fields
/**
 * Bit-field helper for enum-like sets backed by integer flags.
 * @type {object}
 */
tp.Bf = {
    /**
     * Returns the union of two bit-field values.
     * @param {number} A The first bit-field value.
     * @param {number} B The second bit-field value.
     * @returns {number} Returns a bit-field containing all flags from both values.
     */
    Union: function (A, B) {
        return A | B;
    },
    /**
     * Returns the intersection of two bit-field values.
     * @param {number} A The first bit-field value.
     * @param {number} B The second bit-field value.
     * @returns {number} Returns a bit-field containing only common flags.
     */
    Intersection: function (A, B) {
        return A & B;
    },
    /**
     * Returns the intersection of two bit-field values.
     * @param {number} A The first bit-field value.
     * @param {number} B The second bit-field value.
     * @returns {number} Returns a bit-field containing only common flags.
     */
    Junction: function (A, B) {
        return A & B;
    },
    /**
     * Returns the symmetric difference of two bit-field values.
     * @param {number} A The first bit-field value.
     * @param {number} B The second bit-field value.
     * @returns {number} Returns a bit-field containing flags not common to both values.
     */
    Dif: function (A, B) {
        return A ^ B;
    },
    /**
     * Returns the subtraction of B from A.
     * @param {number} A The source bit-field value.
     * @param {number} B The flags to remove.
     * @returns {number} Returns A without the flags contained in B.
     */
    Subtract: function (A, B) {
        return A & ~B;
    },
    /**
     * Returns true when all flags in A exist in B.
     * @param {number} A The member flag or bit-field to test.
     * @param {number} B The containing bit-field.
     * @returns {boolean} Returns true when A is contained in B.
     */
    Member: function (A, B) {
        return A !== 0 && (A & B) === A;
    },
    /**
     * Returns true when all flags in A exist in B.
     * @param {number} A The member flag or bit-field to test.
     * @param {number} B The containing bit-field.
     * @returns {boolean} Returns true when A is contained in B.
     */
    In: function (A, B) {
        return A !== 0 && (A & B) === A;
    },
    /**
     * Returns true when a bit-field value is null, undefined, or zero.
     * @param {number|null|undefined} A The bit-field value to test.
     * @returns {boolean} Returns true when the value is empty.
     */
    IsEmpty: function (A) {
        return tp.IsNil(A) || Number(A) === 0;
    },
    /**
     * Converts a bit-field value to a comma-delimited name list.
     * @param {object} SetType The enum-like object containing integer flags.
     * @param {number} Value The bit-field value.
     * @returns {string} Returns a comma-delimited flag name list.
     */
    SetToString: function (SetType, Value) {
        var Result = [];
        var Prop;
        if (!tp.IsObject(SetType))
            return "";
        for (Prop in SetType) {
            if (Object.prototype.propertyIsEnumerable.call(SetType, Prop) && Number.isInteger(SetType[Prop]) && tp.Bf.Member(SetType[Prop], Value))
                Result.push(Prop);
        }
        return Result.join(", ");
    },
    /**
     * Returns an integer array with the flags found in a bit-field value.
     * @param {object} SetType The enum-like object containing integer flags.
     * @param {number} SetValue The bit-field value.
     * @returns {number[]} Returns the flag values found in SetValue.
     */
    SetValueToIntegerArray: function (SetType, SetValue) {
        var Result = [];
        var Prop;
        var Value;
        if (!tp.IsObject(SetType))
            return Result;
        for (Prop in SetType) {
            if (Object.prototype.propertyIsEnumerable.call(SetType, Prop)) {
                Value = SetType[Prop];
                if (Number.isInteger(Value) && tp.Bf.In(Value, SetValue))
                    Result.push(Value);
            }
        }
        return Result;
    },
    /**
     * Returns a bit-field value from an integer flag array.
     * @param {number[]} FieldFlagsArray The integer flags to combine.
     * @returns {number} Returns the combined bit-field value.
     */
    IntegerArrayToSetValue: function (FieldFlagsArray) {
        var Result = 0;
        if (tp.IsArray(FieldFlagsArray)) {
            FieldFlagsArray.forEach(function (Item) {
                if (Number.isInteger(Item))
                    Result |= Item;
            });
        }
        return Result;
    }
};
Object.freeze(tp.Bf);
