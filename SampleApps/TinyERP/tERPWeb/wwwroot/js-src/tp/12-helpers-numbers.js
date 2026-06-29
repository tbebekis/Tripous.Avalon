// ● conversion result
/**
 * Represents the result of a number conversion.
 */
tp.NumberConversionResult = class {
    // ● constructor
    /**
     * Creates a number conversion result.
     * @param {number|null|undefined} Value The converted value.
     * @param {boolean|null|undefined} Result True when conversion succeeded.
     */
    constructor(Value, Result) {
        this.Value = tp.IsNumber(Value) ? Value : 0;
        this.Result = Result === true;
    }
};
/**
 * The converted value.
 * @type {number}
 */
tp.NumberConversionResult.prototype.Value = 0;
/**
 * True when conversion succeeded.
 * @type {boolean}
 */
tp.NumberConversionResult.prototype.Result = false;
// ● numbers
/**
 * Converts a value to an integer number.
 * @param {*} Value The value to convert.
 * @returns {number} Returns an integer number.
 */
tp.ToInt = function (Value) {
    return Number.isFinite(Number(Value)) ? Math.trunc(Number(Value)) : 0;
};
/**
 * Rounds a number to specified decimal places.
 * @param {number|string} Value The number or string representation of a number.
 * @param {number|null|undefined} Decimals The decimal places. Defaults to 2.
 * @returns {number} Returns the rounded number.
 */
tp.Round = function (Value, Decimals) {
    Value = tp.StrToFloat(Value, 0);
    Decimals = tp.IsNullOrUndefined(Decimals) ? 2 : tp.ToInt(Decimals);
    return Number(Value.toFixed(Decimals));
};
/**
 * Truncates a number to an integer.
 * @param {number|string} Value The value to truncate.
 * @returns {number} Returns the truncated number.
 */
tp.Truncate = function (Value) {
    return Math.trunc(tp.StrToFloat(Value, 0));
};
/**
 * Returns a random integer inside a specified inclusive range.
 * @param {number} Min The minimum value.
 * @param {number} Max The maximum value.
 * @returns {number} Returns a random integer.
 */
tp.Random = function (Min, Max) {
    Min = tp.ToInt(Min);
    Max = tp.ToInt(Max);
    if (Max < Min) {
        var Temp = Min;
        Min = Max;
        Max = Temp;
    }
    return Math.floor(Math.random() * (Max - Min + 1)) + Min;
};
/**
 * Returns a random float inside a specified range.
 * @param {number} Min The minimum value.
 * @param {number} Max The maximum value.
 * @returns {number} Returns a random float.
 */
tp.RandomFloat = function (Min, Max) {
    Min = tp.StrToFloat(Min, 0);
    Max = tp.StrToFloat(Max, 0);
    if (Max < Min) {
        var Temp = Min;
        Min = Max;
        Max = Temp;
    }
    return Math.random() * (Max - Min) + Min;
};
/**
 * Tries to convert a value to an integer.
 * @param {*} Value The value to convert.
 * @returns {tp.NumberConversionResult} Returns the conversion result.
 */
tp.TryStrToInt = function (Value) {
    var NumberValue;
    if (tp.IsNumber(Value) && Number.isFinite(Value))
        return new tp.NumberConversionResult(Math.trunc(Value), true);
    if (!tp.IsBlankString(Value)) {
        NumberValue = Number.parseInt(String(Value), 10);
        if (Number.isFinite(NumberValue))
            return new tp.NumberConversionResult(NumberValue, true);
    }
    return new tp.NumberConversionResult(0, false);
};
/**
 * Tries to convert a value to a floating point number.
 * The decimal separator may be point or comma.
 * @param {*} Value The value to convert.
 * @returns {tp.NumberConversionResult} Returns the conversion result.
 */
tp.TryStrToFloat = function (Value) {
    var NumberValue;
    if (tp.IsNumber(Value) && Number.isFinite(Value))
        return new tp.NumberConversionResult(Value, true);
    if (!tp.IsBlankString(Value)) {
        NumberValue = Number.parseFloat(String(Value).replace(",", "."));
        if (Number.isFinite(NumberValue))
            return new tp.NumberConversionResult(NumberValue, true);
    }
    return new tp.NumberConversionResult(0, false);
};
/**
 * Converts a value to an integer, returning a default value when conversion fails.
 * @param {*} Value The value to convert.
 * @param {number|null|undefined} Default The default value.
 * @returns {number} Returns the converted integer or the default value.
 */
tp.StrToInt = function (Value, Default) {
    var Result = tp.TryStrToInt(Value);
    return Result.Result === true ? Result.Value : (tp.IsNumber(Default) ? Default : 0);
};
/**
 * Converts a value to a floating point number, returning a default value when conversion fails.
 * @param {*} Value The value to convert.
 * @param {number|null|undefined} Default The default value.
 * @returns {number} Returns the converted number or the default value.
 */
tp.StrToFloat = function (Value, Default) {
    var Result = tp.TryStrToFloat(Value);
    return Result.Result === true ? Result.Value : (tp.IsNumber(Default) ? Default : 0);
};
/**
 * Converts a value to a boolean, returning a default value when conversion fails.
 * @param {*} Value The value to convert.
 * @param {boolean|null|undefined} Default The default value.
 * @returns {boolean} Returns the converted boolean or the default value.
 */
tp.StrToBool = function (Value, Default) {
    if (tp.IsBoolean(Value))
        return Value;
    if (tp.IsSameText(Value, "true") || tp.IsSameText(Value, "yes") || tp.IsSameText(Value, "1"))
        return true;
    if (tp.IsSameText(Value, "false") || tp.IsSameText(Value, "no") || tp.IsSameText(Value, "0"))
        return false;
    return Default === true;
};
/**
 * Converts an integer value to a hexadecimal string.
 * @param {number} Value The value to convert.
 * @returns {string} Returns the hexadecimal string.
 */
tp.ToHex = function (Value) {
    Value = tp.ToInt(Value);
    if (Value < 0)
        Value = 0xFFFFFFFF + Value + 1;
    var Result = Value.toString(16).toUpperCase();
    while (Result.length % 2 !== 0)
        Result = "0" + Result;
    return Result;
};
