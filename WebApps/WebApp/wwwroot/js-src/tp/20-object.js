// ● base classes
/**
 * Represents Tripous event arguments.
 */
tp.EventArgs = class {
    // ● constructor
    /**
     * Creates a new event arguments instance.
     * @param {string|Event|object|null|undefined} EventName The event name, DOM event, or source object.
     * @param {object|null|undefined} Sender The optional sender.
     * @param {Event|null|undefined} DomEvent The optional DOM event.
     */
    constructor(EventName, Sender, DomEvent) {
        var IsDomEventValue = function (Value) {
            return typeof Event !== "undefined" && Value instanceof Event;
        };
        this.EventName = "";
        this.Sender = null;
        this.e = null;
        this.el = null;
        this.Handled = false;
        this.Cancel = false;
        this.Command = "";
        if (arguments.length === 1) {
            if (IsDomEventValue(EventName))
                this.e = EventName;
            else if (tp.IsObject(EventName))
                tp.Assign(this, EventName);
            else if (tp.IsString(EventName))
                this.EventName = EventName;
        } else {
            this.EventName = tp.IsNil(EventName) ? "" : String(EventName);
            this.Sender = tp.IsNil(Sender) ? null : Sender;
            this.e = IsDomEventValue(DomEvent) ? DomEvent : null;
        }
        if (IsDomEventValue(this.e) && tp.IsBlank(this.EventName))
            this.EventName = this.e.type;
        if (IsDomEventValue(this.e) && tp.IsNil(this.el))
            this.el = this.e.target;
    }

    // ● properties
    /**
     * Returns true when this instance wraps a DOM event.
     * @returns {boolean} Returns true when this instance wraps a DOM event.
     */
    get IsDomEvent() {
        return typeof Event !== "undefined" && this.e instanceof Event;
    }
    /**
     * Returns true when this instance is a Tripous event.
     * @returns {boolean} Returns true when this instance is a Tripous event.
     */
    get IsTripousEvent() {
        return !this.IsDomEvent;
    }
};

/**
 * Stores a Tripous event listener callback.
 */
tp.Listener = class {
    // ● constructor
    /**
     * Creates a new listener.
     * @param {Function|null|undefined} Func The callback function.
     * @param {object|null|undefined} Context The callback context.
     * @param {boolean|null|undefined} Once True to remove the listener after the first call.
     */
    constructor(Func, Context, Once) {
        this.Func = tp.IsFunction(Func) ? Func : null;
        this.Context = tp.IsNil(Context) ? null : Context;
        this.Once = Once === true;
    }
};

/**
 * The ultimate Tripous base class.
 */
tp.Object = class {
    // ● constructor
    /**
     * Creates a new Tripous object.
     */
    constructor() {
        this.InitClass();
    }

    // ● protected
    /**
     * Returns the normalized event name used as event map key.
     * @param {string} EventName The event name.
     * @returns {string} Returns the normalized event name.
     */
    NormalizeEventName(EventName) {
        return tp.IsBlank(EventName) ? "" : String(EventName).toUpperCase();
    }
    /**
     * Initializes class metadata.
     * @returns {void}
     */
    InitClass() {
    }
    /**
     * Returns true when a property should be included in JSON serialization.
     * @param {string} Prop The property name.
     * @returns {boolean} Returns true when the property should be serialized.
     */
    CanSerialize(Prop) {
        return !tp.IsFunction(this[Prop]) && (!this.fJsonExcludes || this.fJsonExcludes.indexOf(Prop) === -1) && Prop.charAt(0) !== "f";
    }

    // ● properties
    /**
     * Returns true when events are enabled.
     * @returns {boolean} Returns true when events are enabled.
     */
    get EventsEnabled() {
        return this.fEventsEnabledCounter >= 0;
    }
    /**
     * Enables or disables events using a counter.
     * @param {boolean} Value True enables events; false disables events.
     * @returns {void}
     */
    set EventsEnabled(Value) {
        this.fEventsEnabledCounter += Value === true ? 1 : -1;
    }

    // ● events
    /**
     * Returns the listener list for an event.
     * @param {string} EventName The event name.
     * @returns {tp.Listener[]} Returns the listener list.
     */
    GetInvocationList(EventName) {
        EventName = this.NormalizeEventName(EventName);
        if (!this.fEvents || tp.IsBlank(EventName))
            return [];
        return this.fEvents[EventName] || [];
    }
    /**
     * Returns true when an event has registered listeners.
     * @param {string} EventName The event name.
     * @returns {boolean} Returns true when the event has listeners.
     */
    HasListeners(EventName) {
        return this.GetInvocationList(EventName).length > 0;
    }
    /**
     * Adds a listener to an event of this instance and returns the listener object.
     * @example
     * MyObject.On("OnSomething", function (Args) {
     *     // handle event
     * });
     * @example
     * MyObject.On("OnSomething", this.HandlerFunc, this);
     * @example
     * MyObject.On("OnSomething", this.FuncBind(this.HandlerFunc));
     * @param {string} EventName The event name.
     * @param {Function} Func The callback function. Signature: function (Args: tp.EventArgs): void.
     * @param {object|null|undefined} Context The optional callback context.
     * @returns {tp.Listener|null} Returns the created listener or null.
     */
    On(EventName, Func, Context) {
        EventName = this.NormalizeEventName(EventName);
        if (tp.IsBlank(EventName) || !tp.IsFunction(Func))
            return null;
        if (!this.fEvents)
            this.fEvents = {};
        if (!(EventName in this.fEvents))
            this.fEvents[EventName] = [];
        var Listener = new tp.Listener(Func, Context, false);
        this.fEvents[EventName].push(Listener);
        return Listener;
    }
    /**
     * Adds a listener that is removed after the first call.
     * @param {string} EventName The event name.
     * @param {Function} Func The callback function.
     * @param {object|null|undefined} Context The optional callback context.
     * @returns {tp.Listener|null} Returns the created listener or null.
     */
    Once(EventName, Func, Context) {
        EventName = this.NormalizeEventName(EventName);
        if (tp.IsBlank(EventName) || !tp.IsFunction(Func))
            return null;
        if (!this.fEvents)
            this.fEvents = {};
        if (!(EventName in this.fEvents))
            this.fEvents[EventName] = [];
        var Listener = new tp.Listener(Func, Context, true);
        this.fEvents[EventName].push(Listener);
        return Listener;
    }
    /**
     * Removes a listener from an event.
     * @param {string} EventName The event name.
     * @param {tp.Listener|Function} ListenerOrFunc The listener object or callback function.
     * @returns {void}
     */
    Off(EventName, ListenerOrFunc) {
        var InvocationList;
        var Index;
        EventName = this.NormalizeEventName(EventName);
        if (tp.IsBlank(EventName) || tp.IsNil(ListenerOrFunc))
            return;
        InvocationList = this.GetInvocationList(EventName);
        for (Index = InvocationList.length - 1; Index >= 0; Index--) {
            if (InvocationList[Index] === ListenerOrFunc || InvocationList[Index].Func === ListenerOrFunc)
                InvocationList.splice(Index, 1);
        }
    }
    /**
     * Triggers an event. If EventsEnabled is false, no listener is called.
     * @param {string} EventName The event name.
     * @param {tp.EventArgs|object|null|undefined} Args The optional event arguments.
     * @returns {tp.EventArgs|null} Returns the event arguments or null.
     */
    Trigger(EventName, Args) {
        var InvocationList;
        var Listener;
        var Index;
        var EventKey = this.NormalizeEventName(EventName);
        if (!this.EventsEnabled || !this.fEvents || tp.IsBlank(EventKey))
            return null;
        InvocationList = this.GetInvocationList(EventKey).slice();
        if (InvocationList.length === 0)
            return null;
        Args = Args instanceof tp.EventArgs ? Args : new tp.EventArgs(Args || {});
        Args.EventName = String(EventName);
        Args.Sender = tp.IsNil(Args.Sender) ? this : Args.Sender;
        for (Index = 0; Index < InvocationList.length; Index++) {
            Listener = InvocationList[Index];
            Listener.Func.call(Listener.Context || this, Args);
            if (Listener.Once)
                this.Off(EventKey, Listener);
        }
        return Args;
    }

    // ● event handler
    /**
     * Implements the DOM EventListener interface.
     * @see {@link http://www.w3.org/TR/DOM-Level-2-Events/events.html#Events-EventListener|DOM Level 2 Events}
     * @see {@link https://medium.com/@WebReflection/dom-handleevent-a-cross-platform-standard-since-year-2000-5bf17287fd38|handleEvent}
     * @param {Event} e The DOM event.
     * @returns {void}
     */
    handleEvent(e) {
    }

    // ● public
    /**
     * Binds a function to this instance and caches the bound function.
     * @example
     * var Handler = this.FuncBind(this.MyFunction);
     * @param {Function} Func The function to bind.
     * @returns {Function|null} Returns the bound function or null.
     */
    FuncBind(Func) {
        var Index;
        var Item;
        if (!tp.IsFunction(Func))
            return null;
        if (!this.fBinds)
            this.fBinds = [];
        for (Index = 0; Index < this.fBinds.length; Index++) {
            if (this.fBinds[Index].Func === Func)
                return this.fBinds[Index].Bind;
        }
        Item = {
            Func: Func,
            Bind: Func.bind(this)
        };
        this.fBinds.push(Item);
        return Item.Bind;
    }
    /**
     * Returns true when an object is an instance of this object's class.
     * @param {object} Value The value to check.
     * @returns {boolean} Returns true when the value has the same class.
     */
    IsSameClass(Value) {
        return Value instanceof this.constructor;
    }
    /**
     * Creates a new instance of this object's class.
     * @param {...*} Args The constructor arguments.
     * @returns {tp.Object} Returns the created instance.
     */
    CreateInstance(...Args) {
        return tp.CreateInstance(this.constructor, Args);
    }
    /**
     * Clears this instance.
     * @returns {void}
     */
    Clear() {
    }
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        this.Clear();
        tp.Assign(this, Source);
    }
    /**
     * Clones this instance.
     * @returns {tp.Object} Returns the cloned object.
     */
    Clone() {
        var Result = this.CreateInstance();
        Result.Assign(this);
        return Result;
    }
    /**
     * Returns a plain object used by JSON.stringify().
     * If an object being stringified has a function property named toJSON, JSON.stringify() uses the value returned by that function.
     * @see {@link http://www.ecma-international.org/ecma-262/5.1/#sec-15.12.3|ECMAScript specification}
     * @see {@link https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON/stringify|MDN JSON.stringify}
     * @returns {object} Returns a plain object having all serializable properties and values of this instance.
     */
    toJSON() {
        var Result = {};
        var PropNames = tp.GetPropertyNames(this, this.CanSerialize.bind(this));
        var Index;
        var Prop;
        var Value;
        for (Index = 0; Index < PropNames.length; Index++) {
            Prop = PropNames[Index];
            Value = this[Prop];
            Result[Prop] = Value && tp.IsFunction(Value.toJSON) ? Value.toJSON() : Value;
        }
        return Result;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * Treat this as a read-only class field.
 * @type {string}
 */
tp.Object.prototype.tpClass = "tp.Object";
/**
 * Gets the events enabled counter.
 * @type {number}
 */
tp.Object.prototype.fEventsEnabledCounter = 0;
/**
 * Gets the property names excluded from JSON serialization.
 * @type {string[]|null}
 */
tp.Object.prototype.fJsonExcludes = null;
/**
 * Gets the event listener map.
 * @type {object|null}
 */
tp.Object.prototype.fEvents = null;
/**
 * Gets the cached bound functions.
 * @type {object[]|null}
 */
tp.Object.prototype.fBinds = null;
