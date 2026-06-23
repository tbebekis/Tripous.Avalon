// ● objects
/**
 * Returns an object's own and inherited property names.
 * Functions, constructor, and properties starting with __ are excluded.
 * @param {object} Value The object to inspect.
 * @param {Function|null|undefined} Predicate The optional predicate called for each property name.
 * @returns {string[]} Returns the property names.
 */
tp.GetPropertyNames = function (Value, Predicate) {
    var Result = [];
    var List;
    var Name;
    var Descriptor;
    var Index;
    while (Value && Value !== Object.prototype) {
        List = Object.getOwnPropertyNames(Value);
        for (Index = 0; Index < List.length; Index++) {
            Name = List[Index];
            Descriptor = Object.getOwnPropertyDescriptor(Value, Name);
            if (Name !== "constructor"
                && !tp.StartsWith(Name, "__", false)
                && (!Descriptor || !tp.IsFunction(Descriptor.value))
                && Result.indexOf(Name) === -1
                && (!tp.IsFunction(Predicate) || Predicate(Name))) {
                Result.push(Name);
            }
        }
        Value = Object.getPrototypeOf(Value);
    }
    return Result;
};
/**
 * Copies enumerable source properties to a target object.
 * @param {object} Target The target object.
 * @param {object|null|undefined} Source The source object.
 * @returns {object} Returns the target object.
 */
tp.Assign = function (Target, Source) {
    var Name;
    if (tp.IsNil(Target) || tp.IsNil(Source))
        return Target;
    for (Name in Source) {
        if (Object.prototype.propertyIsEnumerable.call(Source, Name))
            Target[Name] = Source[Name];
    }
    return Target;
};
/**
 * Creates an instance of a constructor using arguments from an array.
 * @param {Function} Constructor The constructor function.
 * @param {Array|null|undefined} Args The constructor arguments.
 * @returns {object} Returns the created instance.
 */
tp.CreateInstance = function (Constructor, Args) {
    Args = tp.IsArray(Args) ? Args : [];
    return new (Function.prototype.bind.apply(Constructor, [null].concat(Args)))();
};

// ● functions
/**
 * Calls a function, if specified, using a context and arguments.
 * @param {Function|null|undefined} Func The function to call.
 * @param {object|null|undefined} Context The optional context to use as this.
 * @param {...*} Args The arguments to pass to the function.
 * @returns {*} Returns whatever the called function returns, or null.
 */
tp.Call = function (Func, Context, ...Args) {
    if (!tp.IsFunction(Func))
        return null;
    return Args.length > 0 ? Func.apply(Context || null, Args) : Func.call(Context || null);
};
/**
 * Returns true when all properties of B exist in A and have the same values.
 * @param {object} A The first object.
 * @param {object} B The second object.
 * @returns {boolean} Returns true when B properties exist in A with the same values.
 */
tp.Equals = function (A, B) {
    var Key;
    if (A === B)
        return true;
    if (tp.IsNil(A) || tp.IsNil(B))
        return false;
    for (Key in B) {
        if (Object.prototype.propertyIsEnumerable.call(B, Key) && B[Key] !== A[Key])
            return false;
    }
    return true;
};
/**
 * Creates and returns a MutationObserver instance when available.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/MutationObserver|MDN MutationObserver}
 * @param {Function} Callback A callback of the form function (mutations, observer): void.
 * @returns {MutationObserver|null} Returns a MutationObserver instance or null.
 */
tp.CreateMutationObserver = function (Callback) {
    if (!tp.IsFunction(Callback))
        return null;
    if (typeof MutationObserver !== "undefined")
        return new MutationObserver(Callback);
    if (typeof window !== "undefined" && window.WebKitMutationObserver)
        return new window.WebKitMutationObserver(Callback);
    return null;
};
/**
 * Returns the name of an enum value.
 * @param {object} EnumType The enum-like object.
 * @param {number|string} Value The enum value.
 * @returns {string} Returns the enum member name or empty string.
 */
tp.EnumNameOf = function (EnumType, Value) {
    var Key;
    if (tp.IsNil(EnumType))
        return "";
    for (Key in EnumType) {
        if (Object.prototype.propertyIsEnumerable.call(EnumType, Key) && EnumType[Key] === Value)
            return Key;
    }
    return "";
};
/**
 * Returns a string with characters representing the argument types.
 * Characters used: s, b, n, a, f, d, r, o, and e for empty values.
 * @param {IArguments|Array} Args The arguments object or array.
 * @param {boolean|null|undefined} IncludeEmpty True to include null/undefined as e.
 * @returns {string} Returns the type signature.
 */
tp.Overload = function (Args, IncludeEmpty) {
    var Result = [];
    var Value;
    var Index;
    Args = tp.IsNil(Args) ? [] : Args;
    for (Index = 0; Index < Args.length; Index++) {
        Value = Args[Index];
        if (tp.IsString(Value))
            Result.push("s");
        else if (tp.IsBoolean(Value))
            Result.push("b");
        else if (tp.IsNumber(Value))
            Result.push("n");
        else if (tp.IsArray(Value))
            Result.push("a");
        else if (tp.IsFunction(Value))
            Result.push("f");
        else if (tp.IsDate(Value))
            Result.push("d");
        else if (tp.IsRegExp(Value))
            Result.push("r");
        else if (tp.IsEmpty(Value)) {
            if (IncludeEmpty === true)
                Result.push("e");
        } else if (tp.IsObject(Value)) {
            Result.push("o");
        }
    }
    return Result.join();
};
/**
 * Returns true when an instance contains all specified member names.
 * @param {object} Instance The instance to check.
 * @param {string|string[]} MemberNames The required member name or names.
 * @returns {boolean} Returns true when the instance implements all members.
 */
tp.ImplementsInterface = function (Instance, MemberNames) {
    var Index;
    if (tp.IsNil(Instance))
        return false;
    if (tp.IsString(MemberNames))
        MemberNames = [MemberNames];
    if (!tp.IsArray(MemberNames))
        return false;
    for (Index = 0; Index < MemberNames.length; Index++) {
        if (!(MemberNames[Index] in Instance))
            return false;
    }
    return true;
};
/**
 * Returns a random color in hexadecimal format.
 * @returns {string} Returns a random color.
 */
tp.RandomColor = function () {
    return "#" + tp.ToHex(tp.Random(0, 0xFF)) + tp.ToHex(tp.Random(0, 0xFF)) + tp.ToHex(tp.Random(0, 0xFF));
};
/**
 * Waits for a specified number of milliseconds and then calls a function, if specified.
 * @param {number} MSecsToWait Milliseconds to wait.
 * @param {Function|null|undefined} FuncToCall The optional function to call.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {Promise<void>} Returns a promise resolved after the wait.
 */
tp.WaitAsync = async function (MSecsToWait, FuncToCall, Context) {
    return new Promise(function (Resolve, Reject) {
        setTimeout(function () {
            try {
                tp.Call(FuncToCall, Context);
                Resolve();
            } catch (e) {
                Reject(e);
            }
        }, tp.ToInt(MSecsToWait, 0));
    });
};
/**
 * Serializes a value to JSON text.
 * @param {*} Value The value to serialize.
 * @param {boolean|null|undefined} Formatted True to format the JSON text.
 * @returns {string} Returns the JSON text.
 */
tp.ToJson = function (Value, Formatted) {
    return Formatted === false ? JSON.stringify(Value) : JSON.stringify(Value, null, " ");
};

// ● names
/**
 * Gets the default Tripous id/name prefix.
 * @type {string}
 */
tp.Prefix = "tp-";
/**
 * Name generator for generated element names and ids.
 * @type {object}
 */
tp.Names = (function () {
    var Items = {};
    var Counter = 2000;
    return {
        /**
         * Constructs a name from a prefix and an auto-increment counter associated with that prefix.
         * The prefix lookup is case-insensitive.
         * @param {string} Prefix The optional prefix to prepend.
         * @returns {string} Returns the generated name.
         */
        Next: function (Prefix) {
            var Key;
            var Value;
            if (!tp.IsBlank(Prefix)) {
                Key = Prefix.toUpperCase();
                if (!(Key in Items))
                    Items[Key] = 2000;
                Value = Items[Key]++;
                return Prefix + Value.toString();
            }
            Counter++;
            return Counter.toString();
        }
    };
})();
/**
 * Constructs a name from a prefix and an internal auto-increment counter.
 * @param {string} Prefix The optional prefix to prepend.
 * @returns {string} Returns the generated name.
 */
tp.NextName = function (Prefix) {
    return tp.Names.Next(Prefix);
};
/**
 * Constructs a safe id from a prefix.
 * @param {string} Prefix The optional prefix to prepend.
 * @returns {string} Returns the generated id.
 */
tp.SafeId = function (Prefix) {
    var Value = tp.IsBlank(Prefix) ? tp.Prefix : Prefix;
    return tp.NextName(Value).replace(/\./g, "-");
};

// ● environment
/**
 * Provides browser environment information.
 * @type {object}
 */
tp.Environment = {
    /**
     * Initializes this object.
     * @returns {void}
     */
    Initialize: function () {
    },
    /**
     * Gets the native scrollbar size.
     * @returns {tp.Size} Returns the native scrollbar size.
     */
    get ScrollbarSize() {
        var Outer;
        var Inner;
        var WidthNoScroll;
        var HeightNoScroll;
        var WidthWithScroll;
        var HeightWithScroll;
        if (!tp.Environment.fScrollbarSize) {
            tp.Environment.fScrollbarSize = new tp.Size();
            Outer = document.createElement("div");
            Outer.style.visibility = "hidden";
            Outer.style.width = "100px";
            Outer.style.height = "100px";
            document.body.appendChild(Outer);
            WidthNoScroll = Outer.offsetWidth;
            HeightNoScroll = Outer.offsetHeight;
            Outer.style.overflow = "scroll";
            Inner = document.createElement("div");
            Inner.style.width = "100%";
            Inner.style.height = "100%";
            Outer.appendChild(Inner);
            WidthWithScroll = Inner.offsetWidth;
            HeightWithScroll = Inner.offsetHeight;
            Outer.parentNode.removeChild(Outer);
            tp.Environment.fScrollbarSize.Width = WidthNoScroll - WidthWithScroll;
            tp.Environment.fScrollbarSize.Height = HeightNoScroll - HeightWithScroll;
        }
        return tp.Environment.fScrollbarSize;
    }
};
/**
 * The cached native scrollbar size.
 * @type {tp.Size|null}
 */
tp.Environment.fScrollbarSize = null;
