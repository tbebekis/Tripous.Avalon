// ● keyboard
/**
 * Keyboard key names based on KeyboardEvent.key.
 * See: https://developer.mozilla.org/en-US/docs/Web/API/KeyboardEvent/key
 * @type {object}
 */
tp.Keys = {
    F1: "F1",
    F2: "F2",
    F3: "F3",
    F4: "F4",
    F5: "F5",
    F6: "F6",
    F7: "F7",
    F8: "F8",
    F9: "F9",
    F10: "F10",
    F11: "F11",
    F12: "F12",
    Ctrl: "Control",
    Shift: "Shift",
    Alt: "Alt",
    Enter: "Enter",
    Escape: "Escape",
    Space: " ",
    Tab: "Tab",
    Backspace: "Backspace",
    Delete: "Delete",
    Insert: "Insert",
    Home: "Home",
    End: "End",
    Left: "ArrowLeft",
    Right: "ArrowRight",
    Up: "ArrowUp",
    Down: "ArrowDown",
    PageUp: "PageUp",
    PageDown: "PageDown",
    ContextMenu: "ContextMenu",
    Windows: "Meta",
    Decimal: "Decimal"
};
Object.freeze(tp.Keys);
/**
 * Returns true if a keyboard event matches a key.
 * @param {KeyboardEvent} e The keyboard event.
 * @param {string} Key The key name to check.
 * @returns {boolean} Returns true if the event matches the key.
 */
tp.IsKey = function (e, Key) {
    return e instanceof KeyboardEvent && e.key === Key;
};
/**
 * Returns true if a keyboard event represents a printable character.
 * @param {KeyboardEvent} e The keyboard event.
 * @returns {boolean} Returns true if the event represents a printable character.
 */
tp.IsPrintableKey = function (e) {
    return e instanceof KeyboardEvent && tp.IsString(e.key) && e.key.length === 1;
};
