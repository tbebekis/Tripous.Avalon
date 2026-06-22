/*
 * Tripous.Avalon JavaScript Runtime
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

// ● core
/**
 * Selects an element specified by a selector.
 * @param {Element|Document|Window|string|null|undefined} Selector The selector, element, document, or window to return.
 * @returns {Element|Document|Window|null} Returns the selected object, if any; otherwise, null.
 */
var tp = function (Selector) {
    if (tp.IsString(Selector))
        return document.querySelector(Selector);
    if (tp.IsElement(Selector) || Selector === document || Selector === window)
        return Selector;
    return null;
};
/**
 * Gets the Tripous JavaScript runtime version.
 * @type {string}
 */
tp.Version = "1.0.0";
/**
 * Throws a Tripous error.
 * @param {string} Message The error message.
 * @returns {void}
 */
tp.Throw = function (Message) {
    var Ex = new Error(Message);
    Ex.name = "Tripous Error";
    throw Ex;
};
/**
 * Executes a callback when the document is ready.
 * @param {Function} Callback The callback to execute.
 * @returns {void}
 */
tp.Ready = function (Callback) {
    if (!tp.IsFunction(Callback))
        tp.Throw("Callback is not a function.");
    if (document.readyState === "loading")
        document.addEventListener("DOMContentLoaded", Callback, { once: true });
    else
        Callback();
};

// ● type checks
/**
 * Returns true when a value is a string.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a string.
 */
tp.IsString = function (Value) {
    return typeof Value === "string" || Value instanceof String;
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
 * Returns true when a value is an object.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is an object.
 */
tp.IsObject = function (Value) {
    return Value !== null && typeof Value === "object" && !tp.IsArray(Value);
};
/**
 * Returns true when a value is a DOM element.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is a DOM element.
 */
tp.IsElement = function (Value) {
    return Value instanceof Element;
};
/**
 * Returns true when a value is null or undefined.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is null or undefined.
 */
tp.IsNil = function (Value) {
    return Value === null || Value === undefined;
};
/**
 * Returns true when a value is null, undefined, or an empty trimmed string.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is blank.
 */
tp.IsBlank = function (Value) {
    return tp.IsNil(Value) || (tp.IsString(Value) && Value.trim().length === 0);
};

// ● arrays
/**
 * Returns an array from an array-like value.
 * @param {Array|NodeList|HTMLCollection|Iterable|null|undefined} Value The value to convert.
 * @returns {Array} Returns an array.
 */
tp.ToArray = function (Value) {
    if (tp.IsNil(Value))
        return [];
    if (tp.IsArray(Value))
        return Value;
    return Array.from(Value);
};

// ● dom
/**
 * Selects all elements specified by a selector.
 * @param {string} Selector The selector to use.
 * @param {Element|Document|null|undefined} Parent The optional parent element or document.
 * @returns {Element[]} Returns the selected elements.
 */
tp.SelectAll = function (Selector, Parent) {
    Parent = Parent || document;
    return tp.ToArray(Parent.querySelectorAll(Selector));
};
/**
 * Adds an event listener to an element.
 * @param {Element|Document|Window|string} Selector The target selector, element, document, or window.
 * @param {string} EventName The event name.
 * @param {Function} Handler The event handler.
 * @param {object|boolean} Options The optional event listener options.
 * @returns {void}
 */
tp.On = function (Selector, EventName, Handler, Options) {
    var Element = tp(Selector);
    if (Element)
        Element.addEventListener(EventName, Handler, Options);
};
/**
 * Removes an event listener from an element.
 * @param {Element|Document|Window|string} Selector The target selector, element, document, or window.
 * @param {string} EventName The event name.
 * @param {Function} Handler The event handler.
 * @param {object|boolean} Options The optional event listener options.
 * @returns {void}
 */
tp.Off = function (Selector, EventName, Handler, Options) {
    var Element = tp(Selector);
    if (Element)
        Element.removeEventListener(EventName, Handler, Options);
};
/**
 * Sets or gets the text content of an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {*} Value The optional value to set.
 * @returns {string|void} Returns text when no value is specified; otherwise, returns void.
 */
tp.Text = function (Selector, Value) {
    var Element = tp(Selector);
    if (!Element)
        return "";
    if (arguments.length === 1)
        return Element.textContent;
    Element.textContent = tp.IsNil(Value) ? "" : String(Value);
};
/**
 * Sets or gets the value of an element.
 * @param {Element|string} Selector The target selector or element.
 * @param {*} Value The optional value to set.
 * @returns {string|void} Returns the element value when no value is specified; otherwise, returns void.
 */
tp.Value = function (Selector, Value) {
    var Element = tp(Selector);
    if (!Element)
        return "";
    if (arguments.length === 1)
        return Element.value;
    Element.value = tp.IsNil(Value) ? "" : Value;
};
