// ● events
/**
 * Common Tripous event names and DOM event name mappings.
 * @type {object}
 */
tp.Events = {
    Unknown: "Unknown",
    Click: "Click",
    AuxClick: "AuxClick",
    DoubleClick: "DoubleClick",
    MouseDown: "MouseDown",
    MouseUp: "MouseUp",
    MouseEnter: "MouseEnter",
    MouseMove: "MouseMove",
    MouseLeave: "MouseLeave",
    KeyDown: "KeyDown",
    KeyPress: "KeyPress",
    KeyUp: "KeyUp",
    MouseWheel: "MouseWheel",
    Scroll: "Scroll",
    ContextMenu: "ContextMenu",
    Load: "Load",
    Resize: "Resize",
    Activate: "Activate",
    Focus: "Focus",
    LostFocus: "LostFocus",
    InputChanged: "InputChanged",
    TextSelected: "TextSelected",
    Change: "Change",
    DragStart: "DragStart",
    Drag: "Drag",
    DragEnd: "DragEnd",
    DragEnter: "DragEnter",
    DragOver: "DragOver",
    DragLeave: "DragLeave",
    DragDrop: "DragDrop",
    Cut: "Cut",
    Copy: "Copy",
    Paste: "Paste",
    Custom: "Custom",

    /**
     * Maps DOM event names to Tripous event names.
     * @type {object[]}
     */
    Map: [],

    /**
     * Returns the index of a DOM event name in the event map.
     * @param {string} DomEventName The DOM event name.
     * @returns {number} Returns the event map index or -1.
     */
    DomIndex: function (DomEventName) {
        var Index;
        for (Index = 0; Index < tp.Events.Map.length; Index++) {
            if (tp.IsSameText(tp.Events.Map[Index].dom, DomEventName))
                return Index;
        }
        return -1;
    },
    /**
     * Returns the index of a Tripous event name in the event map.
     * @param {string} TripousEventName The Tripous event name.
     * @returns {number} Returns the event map index or -1.
     */
    TripousIndex: function (TripousEventName) {
        var Index;
        for (Index = 0; Index < tp.Events.Map.length; Index++) {
            if (tp.IsSameText(tp.Events.Map[Index].tp, TripousEventName))
                return Index;
        }
        return -1;
    },
    /**
     * Converts a Tripous event name to a DOM event name.
     * @param {string} TripousEventName The Tripous event name.
     * @returns {string} Returns the DOM event name, or tp.Events.Unknown.
     */
    ToDom: function (TripousEventName) {
        var Index = tp.Events.TripousIndex(TripousEventName);
        return Index > -1 ? tp.Events.Map[Index].dom : tp.Events.Unknown;
    },
    /**
     * Converts a DOM event name to a Tripous event name.
     * @param {string} DomEventName The DOM event name.
     * @returns {string} Returns the Tripous event name, or tp.Events.Unknown.
     */
    ToTripous: function (DomEventName) {
        var Index = tp.Events.DomIndex(DomEventName);
        return Index > -1 ? tp.Events.Map[Index].tp : tp.Events.Unknown;
    }
};

tp.Events.Map = [
    { dom: "click", tp: tp.Events.Click },
    { dom: "auxclick", tp: tp.Events.AuxClick },
    { dom: "dblclick", tp: tp.Events.DoubleClick },
    { dom: "mousedown", tp: tp.Events.MouseDown },
    { dom: "mouseup", tp: tp.Events.MouseUp },
    { dom: "mouseover", tp: tp.Events.MouseEnter },
    { dom: "mousemove", tp: tp.Events.MouseMove },
    { dom: "mouseout", tp: tp.Events.MouseLeave },
    { dom: "keydown", tp: tp.Events.KeyDown },
    { dom: "keypress", tp: tp.Events.KeyPress },
    { dom: "keyup", tp: tp.Events.KeyUp },
    { dom: "scroll", tp: tp.Events.Scroll },
    { dom: "mousewheel", tp: tp.Events.MouseWheel },
    { dom: "DOMMouseScroll", tp: tp.Events.MouseWheel },
    { dom: "contextmenu", tp: tp.Events.ContextMenu },
    { dom: "load", tp: tp.Events.Load },
    { dom: "resize", tp: tp.Events.Resize },
    { dom: "activate", tp: tp.Events.Activate },
    { dom: "DOMActivate", tp: tp.Events.Activate },
    { dom: "focus", tp: tp.Events.Focus },
    { dom: "blur", tp: tp.Events.LostFocus },
    { dom: "change", tp: tp.Events.Change },
    { dom: "input", tp: tp.Events.InputChanged },
    { dom: "select", tp: tp.Events.TextSelected },
    { dom: "dragstart", tp: tp.Events.DragStart },
    { dom: "drag", tp: tp.Events.Drag },
    { dom: "dragend", tp: tp.Events.DragEnd },
    { dom: "dragenter", tp: tp.Events.DragEnter },
    { dom: "dragover", tp: tp.Events.DragOver },
    { dom: "dragleave", tp: tp.Events.DragLeave },
    { dom: "drop", tp: tp.Events.DragDrop },
    { dom: "copy", tp: tp.Events.Copy },
    { dom: "cut", tp: tp.Events.Cut },
    { dom: "paste", tp: tp.Events.Paste }
];

Object.freeze(tp.Events.Map);
Object.freeze(tp.Events);
