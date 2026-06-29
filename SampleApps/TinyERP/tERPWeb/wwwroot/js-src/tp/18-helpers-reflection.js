// ● property definition
/**
 * Sets BaseClass as the base class of Class.
 * This helper is kept for legacy function-constructor code.
 * @see {@link https://stackoverflow.com/questions/9959727/proto-vs-prototype-in-javascript|__proto__ vs prototype}
 * @param {Function} Class The derived constructor function.
 * @param {Function} BaseClass The base constructor function.
 * @returns {object} Returns the base prototype.
 */
tp.SetBaseClass = function (Class, BaseClass) {
    Class.prototype = Object.create(BaseClass.prototype);
    Class.prototype.constructor = Class;
    return BaseClass.prototype;
};
/**
 * Defines a named or accessor property on a prototype or object.
 * @param {string} Name The property name.
 * @param {object} Prototype The prototype or object.
 * @param {*|Function} GetFunc The value or getter function.
 * @param {Function|null|undefined} SetFunc The optional setter function.
 * @returns {void}
 */
tp.Property = function (Name, Prototype, GetFunc, SetFunc) {
    var Descriptor = {};
    if (tp.IsFunction(GetFunc)) {
        Descriptor.get = GetFunc;
        if (tp.IsFunction(SetFunc))
            Descriptor.set = SetFunc;
    } else {
        Descriptor.value = GetFunc;
        Descriptor.writable = true;
    }
    Descriptor.enumerable = true;
    Descriptor.configurable = true;
    Object.defineProperty(Prototype, Name, Descriptor);
};
/**
 * Defines a constant property on a prototype or object.
 * @param {string} Name The property name.
 * @param {object} Prototype The prototype or object.
 * @param {*} Value The constant value.
 * @returns {void}
 */
tp.Constant = function (Name, Prototype, Value) {
    Object.defineProperty(Prototype, Name, {
        value: Value,
        writable: false,
        enumerable: false,
        configurable: false
    });
};

// ● reflection
/**
 * Contains information about a property or function.
 */
tp.PropertyInfo = class {
    // ● constructor
    /**
     * Creates a property information object.
     */
    constructor() {
        this.Name = "";
        this.Signature = "";
        this.Type = "";
        this.Args = 0;
        this.HasGetter = false;
        this.HasSetter = false;
        this.IsConstructor = false;
        this.IsFunction = false;
        this.IsProperty = false;
        this.IsConfigurable = false;
        this.IsEnumerable = false;
        this.IsWritable = false;
        this.Pointer = null;
    }
};
/**
 * The member name.
 * @type {string}
 */
tp.PropertyInfo.prototype.Name = "";
/**
 * The member signature.
 * @type {string}
 */
tp.PropertyInfo.prototype.Signature = "";
/**
 * The member type marker.
 * @type {string}
 */
tp.PropertyInfo.prototype.Type = "";
/**
 * The function argument count.
 * @type {number}
 */
tp.PropertyInfo.prototype.Args = 0;
/**
 * True when the member has a getter.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.HasGetter = false;
/**
 * True when the member has a setter.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.HasSetter = false;
/**
 * True when the member is a constructor.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.IsConstructor = false;
/**
 * True when the member is a function.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.IsFunction = false;
/**
 * True when the member is a property.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.IsProperty = false;
/**
 * True when the member is configurable.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.IsConfigurable = false;
/**
 * True when the member is enumerable.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.IsEnumerable = false;
/**
 * True when the member is writable.
 * @type {boolean}
 */
tp.PropertyInfo.prototype.IsWritable = false;
/**
 * The member value or function pointer.
 * @type {*}
 */
tp.PropertyInfo.prototype.Pointer = null;

/**
 * Returns a property descriptor from an object or its prototype chain.
 * Can also be used for calling inherited property getters and setters.
 * @example
 * return tp.GetPropertyDescriptor(base, "Name").get.call(this);
 * tp.GetPropertyDescriptor(base, "Name").set.call(this, Value);
 * @param {object|null|undefined} Value The object or prototype to inspect.
 * @param {string} PropName The property name.
 * @returns {PropertyDescriptor|null} Returns the property descriptor or null.
 */
tp.GetPropertyDescriptor = function (Value, PropName) {
    var Descriptor;
    while (Value && Value !== Object.prototype) {
        Descriptor = Object.getOwnPropertyDescriptor(Value, PropName);
        if (Descriptor)
            return Descriptor;
        Value = Object.getPrototypeOf(Value);
    }
    return null;
};
/**
 * Returns information about a property or function.
 * @param {object} Value The object or prototype to inspect.
 * @param {string} Key The member name.
 * @returns {tp.PropertyInfo} Returns an information object.
 */
tp.GetPropertyInfo = function (Value, Key) {
    var Descriptor = tp.GetPropertyDescriptor(Value, Key);
    var Result = new tp.PropertyInfo();
    var Pointer;
    var ParamList;
    Result.Name = Key;
    Result.Signature = Key;
    if (!Descriptor)
        return Result;
    Pointer = "value" in Descriptor ? Descriptor.value : null;
    Result.HasGetter = tp.IsFunction(Descriptor.get);
    Result.HasSetter = tp.IsFunction(Descriptor.set);
    Result.IsConstructor = tp.IsSameText("constructor", Key);
    if (tp.IsFunction(Pointer)) {
        Result.Type = "f";
        Result.IsFunction = true;
        Result.Args = Pointer.length || 0;
        ParamList = Result.Args > 0 ? tp.GetFunctionParams(Pointer) : [];
        Result.Signature = "function " + Key + "(" + ParamList.join(",") + ")";
    } else if (tp.IsArray(Pointer)) {
        Result.Type = "a";
    } else {
        Result.Type = "o";
    }
    Result.IsProperty = !Result.IsFunction && !Result.IsConstructor;
    Result.IsConfigurable = Descriptor.configurable === true;
    Result.IsEnumerable = Descriptor.enumerable === true;
    Result.IsWritable = Descriptor.writable === true || Result.HasSetter === true;
    Result.Pointer = Pointer;
    return Result;
};
/**
 * Returns true when a specified property is writable.
 * @param {object} Value The object or prototype to inspect.
 * @param {string} Key The member name.
 * @returns {boolean} Returns true when the property is writable.
 */
tp.IsWritableProperty = function (Value, Key) {
    var Info = tp.GetPropertyInfo(Value, Key);
    return Info.HasSetter === true || Info.IsWritable === true;
};
/**
 * Returns property and function information about an object.
 * @param {object} Value The object or prototype to inspect.
 * @returns {tp.PropertyInfo[]} Returns a list of information objects.
 */
tp.GetPropertyInfoList = function (Value) {
    var Result = [];
    var Names;
    var Name;
    var Descriptor;
    var Index;
    while (Value) {
        Names = Object.getOwnPropertyNames(Value);
        for (Index = 0; Index < Names.length; Index++) {
            Name = Names[Index];
            if (Result.some(function (Item) { return Item.Name === Name; }))
                continue;
            Descriptor = Object.getOwnPropertyDescriptor(Value, Name);
            if (Descriptor)
                Result.push(tp.GetPropertyInfo(Value, Name));
        }
        Value = Object.getPrototypeOf(Value);
    }
    return Result;
};
/**
 * Returns a descriptive reflection text for an object.
 * @param {object} Value The object or prototype to inspect.
 * @returns {string} Returns a descriptive text.
 */
tp.GetReflectionText = function (Value) {
    var List = tp.GetPropertyInfoList(Value);
    var Builder = new tp.StringBuilder();
    var Format = "{0} {1} {2} {3} {4} {5} {6} {7} {8}";
    var Text;
    List.forEach(function (Info) {
        Text = tp.Format(Format,
            Info.Type,
            Info.Args,
            Info.IsConstructor ? "c" : "_",
            Info.HasGetter ? "g" : "_",
            Info.HasSetter ? "s" : "_",
            Info.IsConfigurable ? "c" : "_",
            Info.IsEnumerable ? "e" : "_",
            Info.IsWritable ? "w" : "_",
            Info.Signature);
        Builder.AppendLine(Text);
    });
    return Builder.ToString();
};
/**
 * Returns definition text for an object, using property and function signatures.
 * @param {object} Value The object or prototype to inspect.
 * @returns {string} Returns the definition text.
 */
tp.GetObjectDefText = function (Value) {
    var List = tp.GetPropertyInfoList(Value);
    var Constructor = "";
    var Props = [];
    var Funcs = [];
    var Builder = new tp.StringBuilder();
    var Info;
    var Index;
    for (Index = 0; Index < List.length; Index++) {
        Info = List[Index];
        if (Info.IsConstructor)
            Constructor = Info.Signature;
        else if (Info.IsFunction)
            Funcs.push(Info.Signature);
        else if (Info.IsProperty)
            Props.push(Info.Signature);
    }
    Props.sort();
    Funcs.sort();
    if (!tp.IsBlank(Constructor))
        Builder.AppendLine("constructor " + Constructor);
    if (Props.length > 0) {
        Builder.AppendLine();
        Builder.AppendLine("// properties");
        for (Index = 0; Index < Props.length; Index++)
            Builder.AppendLine(Props[Index]);
    }
    if (Funcs.length > 0) {
        Builder.AppendLine();
        Builder.AppendLine("// methods");
        for (Index = 0; Index < Funcs.length; Index++)
            Builder.AppendLine(Funcs[Index]);
    }
    return Builder.ToString();
};
/**
 * Returns the parameter names of a function.
 * @see {@link https://stackoverflow.com/questions/1007981/how-to-get-function-parameter-names-values-dynamically-from-javascript|Function parameter names}
 * @param {Function} Func The function to inspect.
 * @returns {string[]} Returns the parameter names.
 */
tp.GetFunctionParams = function (Func) {
    var Text;
    var Start;
    var End;
    if (!tp.IsFunction(Func))
        return [];
    Text = Func.toString()
        .replace(/\/\*[\s\S]*?\*\//g, "")
        .replace(/\/\/.*$/gm, "")
        .trim();
    if (Text.indexOf("=>") !== -1) {
        Text = Text.substring(0, Text.indexOf("=>")).trim();
        Text = Text.replace(/^\(/, "").replace(/\)$/, "");
    } else {
        Start = Text.indexOf("(");
        End = Text.indexOf(")");
        Text = Start !== -1 && End !== -1 && End > Start ? Text.substring(Start + 1, End) : "";
    }
    return Text.split(",").map(function (Item) {
        return Item.replace(/=.*/g, "").trim();
    }).filter(Boolean);
};
/**
 * Returns true when a specified object has a specified property.
 * @param {object} Value The object or prototype to inspect.
 * @param {string} Key The member name.
 * @returns {boolean} Returns true when the member is a property.
 */
tp.HasProperty = function (Value, Key) {
    var Info = tp.GetPropertyInfo(Value, Key);
    return Info.IsProperty === true;
};
/**
 * Returns true when a specified object has a specified writable property.
 * @param {object} Value The object or prototype to inspect.
 * @param {string} Key The member name.
 * @returns {boolean} Returns true when the member is a writable property.
 */
tp.HasWritableProperty = function (Value, Key) {
    var Info = tp.GetPropertyInfo(Value, Key);
    return Info.IsProperty === true && (Info.IsWritable === true || Info.HasSetter === true);
};
