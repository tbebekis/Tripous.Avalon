// ● constants
/**
 * Represents the JavaScript undefined value.
 * @type {undefined}
 */
tp.Undefined = void 0;

// ● type checks
/**
 * Returns true when a value is null or undefined.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is null or undefined.
 */
tp.IsNil = function (Value) {
    return Value === null || Value === undefined;
};
/**
 * Returns true when a value is null or undefined.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is null or undefined.
 */
tp.IsNullOrUndefined = function (Value) {
    return tp.IsNil(Value);
};
/**
 * Returns true when a value is null or undefined.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is null or undefined.
 */
tp.IsEmpty = function (Value) {
    return tp.IsNil(Value);
};
/**
 * Returns true when a value is not null or undefined.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is not null or undefined.
 */
tp.IsValid = function (Value) {
    return !tp.IsNil(Value);
};
/**
 * Returns true when a value is a string.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a string.
 */
tp.IsString = function (Value) {
    return typeof Value === "string" || Value instanceof String;
};
/**
 * Returns true when a value is null, undefined, or an empty trimmed string.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is blank.
 */
tp.IsBlank = function (Value) {
    return tp.IsNil(Value) || (tp.IsString(Value) && Value.trim().length === 0);
};
/**
 * Returns true when a value is null, undefined, or an empty trimmed string.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is null or whitespace.
 */
tp.IsNullOrWhiteSpace = function (Value) {
    return tp.IsBlank(Value);
};
/**
 * Returns true when a value is a function.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a function.
 */
tp.IsFunction = function (Value) {
    return typeof Value === "function";
};
/**
 * Returns true when a value is an array.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is an array.
 */
tp.IsArray = function (Value) {
    return Array.isArray(Value);
};
/**
 * Returns true when a value is an arguments object.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is an arguments object.
 */
tp.IsArguments = function (Value) {
    return Object.prototype.toString.call(Value) === "[object Arguments]";
};
/**
 * Returns true when a value is an object.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is an object.
 */
tp.IsObject = function (Value) {
    return Value !== null && typeof Value === "object";
};
/**
 * Returns true when a value is a plain object.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a plain object.
 */
tp.IsPlainObject = function (Value) {
    if (!tp.IsObject(Value))
        return false;
    return Object.getPrototypeOf(Value) === Object.prototype || Object.getPrototypeOf(Value) === null;
};
/**
 * Returns true when a value is a number and is not NaN.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a number and is not NaN.
 */
tp.IsNumber = function (Value) {
    return typeof Value === "number" && !Number.isNaN(Value);
};
/**
 * Returns true when a value is an integer number.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is an integer number.
 */
tp.IsInteger = function (Value) {
    return Number.isInteger(Value);
};
/**
 * Returns true when a value is a non-integer number.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a non-integer number.
 */
tp.IsFloat = function (Value) {
    return tp.IsNumber(Value) && !Number.isInteger(Value);
};
/**
 * Returns true when a value is a boolean.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a boolean.
 */
tp.IsBoolean = function (Value) {
    return typeof Value === "boolean";
};
/**
 * Returns true when a value is a Date object.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a Date object.
 */
tp.IsDate = function (Value) {
    return Value instanceof Date;
};
/**
 * Returns true when a value is a string, number, or boolean.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is primitive for Tripous purposes.
 */
tp.IsPrimitive = function (Value) {
    return tp.IsString(Value) || tp.IsNumber(Value) || tp.IsBoolean(Value);
};
/**
 * Returns true when a value is a string, number, boolean, or Date.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is simple for Tripous purposes.
 */
tp.IsSimple = function (Value) {
    return tp.IsPrimitive(Value) || tp.IsDate(Value);
};
/**
 * Returns true when a value is a Promise object or thenable.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a Promise object or thenable.
 */
tp.IsPromise = function (Value) {
    return Value instanceof Promise || (!tp.IsNil(Value) && tp.IsFunction(Value.then));
};
/**
 * Returns true when a value is a RegExp object.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a RegExp object.
 */
tp.IsRegExp = function (Value) {
    return Object.prototype.toString.call(Value) === "[object RegExp]";
};

// ● json
/**
 * Parses JSON text and returns an object, array, string, number, or boolean on success; otherwise, null.
 * @param {string} JsonText The JSON text to parse.
 * @returns {*} Returns the parsed value or null.
 */
tp.ParseJson = function (JsonText) {
    var Result = null;
    var Value;
    if (tp.IsString(JsonText) && !tp.IsBlank(JsonText)) {
        try {
            Value = JSON.parse(JsonText);
            Result = tp.IsValid(Value) ? Value : null;
        } catch (e) {
            Result = null;
        }
    }
    return Result;
};
/**
 * Tries to parse JSON text.
 * @param {string} JsonText The JSON text to parse.
 * @returns {{Value: *, Result: boolean}} Returns the parsed value and success flag.
 */
tp.TryParseJson = function (JsonText) {
    var Value = tp.ParseJson(JsonText);
    return {
        Value: Value,
        Result: tp.IsValid(Value)
    };
};
/**
 * Returns true when a specified string is JSON text.
 * @param {string} Text The text to check.
 * @returns {boolean} Returns true when the text is JSON.
 */
tp.IsJsonText = function (Text) {
    return tp.IsValid(tp.ParseJson(Text));
};

// ● dom type checks
/**
 * Returns true when a value is a DOM Node.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a DOM Node.
 */
tp.IsNode = function (Value) {
    return typeof Node !== "undefined" && Value instanceof Node;
};
/**
 * Returns true when a value is a DOM attribute Node.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a DOM attribute Node.
 */
tp.IsAttribute = function (Value) {
    return typeof Node !== "undefined" && !!(Value && Value.nodeType === Node.ATTRIBUTE_NODE);
};
/**
 * Returns true when a value is a DOM Element.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a DOM Element.
 */
tp.IsElement = function (Value) {
    return typeof Element !== "undefined" && Value instanceof Element;
};
/**
 * Returns true when a value is an HTMLElement.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is an HTMLElement.
 */
tp.IsHTMLElement = function (Value) {
    return typeof HTMLElement !== "undefined" && Value instanceof HTMLElement;
};
/**
 * Returns true when a value is a DOM text node.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a DOM text node.
 */
tp.IsText = function (Value) {
    return typeof Node !== "undefined" && !!(Value && Value.nodeType === Node.TEXT_NODE);
};
/**
 * Returns true when a value is an HTMLElement with a name property.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a named HTMLElement.
 */
tp.IsNamedHtmlElement = function (Value) {
    return tp.IsHTMLElement(Value) && "name" in Value;
};
/**
 * Returns true when a value provides querySelector() and querySelectorAll().
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a node selector.
 */
tp.IsNodeSelector = function (Value) {
    return tp.IsValid(Value) && tp.IsFunction(Value.querySelector) && tp.IsFunction(Value.querySelectorAll);
};
/**
 * Returns true when a value provides HTML constraint validation methods.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/HTML/Guides/Constraint_validation|MDN Constraint validation}
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is validatable.
 */
tp.IsValidatableElement = function (Value) {
    return tp.IsValid(Value) && tp.IsFunction(Value.checkValidity) && tp.IsFunction(Value.setCustomValidity);
};
/**
 * Returns true when a value is an input, select, or textarea element.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a form element.
 */
tp.IsFormElement = function (Value) {
    return (typeof HTMLInputElement !== "undefined" && Value instanceof HTMLInputElement)
        || (typeof HTMLSelectElement !== "undefined" && Value instanceof HTMLSelectElement)
        || (typeof HTMLTextAreaElement !== "undefined" && Value instanceof HTMLTextAreaElement);
};
/**
 * Returns true when a value is an element of a specified node name.
 * @param {*} Value The value to check.
 * @param {string} NodeName The node name, e.g. div or span.
 * @returns {boolean} Returns true when the value is an element of the specified node name.
 */
tp.ElementIs = function (Value, NodeName) {
    return tp.IsElement(Value) && tp.IsString(NodeName) && Value.nodeName.toUpperCase() === NodeName.toUpperCase();
};

// ● interface type checks
/**
 * Returns true when a value provides a Clone() method.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is cloneable.
 */
tp.IsCloneable = function (Value) {
    return tp.IsValid(Value) && tp.IsFunction(Value.Clone);
};
/**
 * Returns true when a value provides an Assign() method.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is assignable.
 */
tp.IsAssignable = function (Value) {
    return tp.IsValid(Value) && tp.IsFunction(Value.Assign);
};
