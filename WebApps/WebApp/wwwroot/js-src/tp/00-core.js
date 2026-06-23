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
 * Gets a value indicating whether the Tripous JavaScript runtime is ready.
 * @type {boolean}
 */
tp.IsReady = false;
/**
 * Gets the listeners to execute when the Tripous JavaScript runtime is ready.
 * @type {tp.Listener[]}
 */
tp.ReadyListeners = [];
/**
 * Adds a listener to be executed when the Tripous JavaScript runtime is ready.
 * @param {Function} Callback The callback to execute.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {tp.Listener} Returns the created listener.
 */
tp.AddReadyListener = function (Callback, Context) {
    var Listener;
    if (!tp.IsFunction(Callback))
        tp.Throw("Callback is not a function.");
    Listener = new tp.Listener(Callback, Context, false);
    tp.ReadyListeners.push(Listener);
    return Listener;
};
/**
 * Executes a callback when the Tripous JavaScript runtime is ready.
 * @param {Function} Callback The callback to execute.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {void}
 */
tp.Ready = function (Callback, Context) {
    if (tp.IsReady === true) {
        if (!tp.IsFunction(Callback))
            tp.Throw("Callback is not a function.");
        Callback.call(Context || null);
    } else {
        tp.AddReadyListener(Callback, Context);
    }
};
/**
 * Called before ready listeners during Tripous JavaScript runtime initialization.
 * @returns {void}
 */
tp.AppInitializeBefore = function () {
};
/**
 * Called after ready listeners during Tripous JavaScript runtime initialization.
 * @returns {void}
 */
tp.AppInitializeAfter = function () {
};
/**
 * Called after AppInitializeAfter() during Tripous JavaScript runtime initialization.
 * @returns {void}
 */
tp.Main = function () {
};
