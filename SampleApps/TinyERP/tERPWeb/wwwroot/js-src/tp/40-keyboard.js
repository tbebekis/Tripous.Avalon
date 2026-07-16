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
 * Keyboard key codes for compatibility with older KeyboardEvent APIs.
 * @type {object}
 */
tp.KeyCodes = {
    F1: 112,
    F2: 113,
    F3: 114,
    F4: 115,
    F5: 116,
    F6: 117,
    F7: 118,
    F8: 119,
    F9: 120,
    F10: 121,
    F11: 122,
    F12: 123,
    Enter: 13,
    Escape: 27,
    Space: 32,
    Tab: 9,
    Backspace: 8,
    Delete: 46,
    Insert: 45,
    Home: 36,
    End: 35,
    Left: 37,
    Up: 38,
    Right: 39,
    Down: 40,
    PageUp: 33,
    PageDown: 34
};
Object.freeze(tp.KeyCodes);
/**
 * Returns true if a keyboard event matches a key.
 * @param {KeyboardEvent} e The keyboard event.
 * @param {string} Key The key name to check.
 * @returns {boolean} Returns true if the event matches the key.
 */
tp.IsKey = function (e, Key) {
    var Code;
    if (!e)
        return false;
    if (e.key === Key)
        return true;
    Code = tp.KeyCodes[Key];
    return tp.IsNumber(Code) && (e.keyCode === Code || e.which === Code);
};
/**
 * Returns true if a keyboard event represents a printable character.
 * @param {KeyboardEvent} e The keyboard event.
 * @returns {boolean} Returns true if the event represents a printable character.
 */
tp.IsPrintableKey = function (e) {
    return !!e && tp.IsString(e.key) && e.key.length === 1;
};
