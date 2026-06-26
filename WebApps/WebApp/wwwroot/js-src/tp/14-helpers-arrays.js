// ● arrays
/**
 * Returns true when a value provides a length property.
 * @param {*} Value The value to check.
 * @returns {boolean} Returns true when the value is array-like.
 */
tp.IsArrayLike = function (Value) {
    return tp.IsValid(Value) && "length" in Value;
};
/**
 * Returns true when an index is inside an array-like object.
 * @param {Array|object} List The array-like object.
 * @param {number} Index The index to check.
 * @returns {boolean} Returns true when the index is valid.
 */
tp.InRange = function (List, Index) {
    return tp.IsArrayLike(List) && Index >= 0 && Index <= List.length - 1;
};
/**
 * Returns an array from an array-like or iterable value.
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
/**
 * Returns true when an array-like object contains an item.
 * @param {Array|object} List The array-like object.
 * @param {*} Value The item to find.
 * @returns {boolean} Returns true when the item exists.
 */
tp.ListContains = function (List, Value) {
    return tp.IsArrayLike(List) && Array.prototype.indexOf.call(List, Value) >= 0;
};
/**
 * Returns true when an array-like object contains a string value case-insensitively.
 * @param {Array|object} List The array-like object.
 * @param {string} Value The text to find.
 * @returns {boolean} Returns true when the text exists.
 */
tp.ListContainsText = function (List, Value) {
    return tp.ListIndexOfText(List, Value) >= 0;
};
/**
 * Returns the index of a string value case-insensitively.
 * @param {Array|object} List The array-like object.
 * @param {string} Value The text to find.
 * @returns {number} Returns the item index or -1.
 */
tp.ListIndexOfText = function (List, Value) {
    var Index;
    var Text;
    if (!tp.IsArrayLike(List))
        return -1;
    for (Index = 0; Index < List.length; Index++) {
        Text = !tp.IsEmpty(List[Index]) ? String(List[Index]) : "";
        if (tp.IsSameText(Text, Value))
            return Index;
    }
    return -1;
};
/**
 * Inserts an item into an array.
 * @param {Array} List The array to modify.
 * @param {number} Index The insert index.
 * @param {*} Value The item to insert.
 * @returns {void}
 */
tp.ListInsert = function (List, Index, Value) {
    if (tp.IsArray(List))
        List.splice(tp.ToInt(Index), 0, Value);
};
/**
 * Removes an item from an array.
 * @param {Array} List The array to modify.
 * @param {*} Value The item to remove.
 * @returns {void}
 */
tp.ListRemove = function (List, Value) {
    var Index;
    if (tp.IsArray(List)) {
        Index = List.indexOf(Value);
        if (Index !== -1)
            List.splice(Index, 1);
    }
};
/**
 * Removes an item at an index from an array.
 * @param {Array} List The array to modify.
 * @param {number} Index The item index.
 * @returns {void}
 */
tp.ListRemoveAt = function (List, Index) {
    if (tp.IsArray(List) && tp.InRange(List, Index))
        List.splice(Index, 1);
};
/**
 * Clears an array.
 * @param {Array} List The array to clear.
 * @returns {void}
 */
tp.ListClear = function (List) {
    if (tp.IsArray(List))
        List.length = 0;
};
/**
 * Clones an array.
 * @param {Array} List The array to clone.
 * @param {boolean|null|undefined} Deep True for deep clone using structuredClone/JSON fallback.
 * @returns {Array} Returns the cloned array.
 */
tp.ListClone = function (List, Deep) {
    if (!tp.IsArrayLike(List))
        return [];
    List = tp.ToArray(List);
    if (Deep !== true)
        return List.slice();
    if (typeof structuredClone === "function")
        return structuredClone(List);
    return JSON.parse(JSON.stringify(List));
};

// ● array sort and filter
/**
 * Information object for array sorting.
 */
tp.SortInfo = class {
    // ● constructor
    /**
     * Creates sort information.
     * @param {string|number|null|undefined} Prop The property name or array index.
     * @param {boolean|null|undefined} Reverse True to sort in descending order.
     * @param {Function|null|undefined} GetValueFunc Optional callback returning the sortable value.
     */
    constructor(Prop, Reverse, GetValueFunc) {
        this.Prop = tp.IsNullOrUndefined(Prop) ? "" : Prop;
        this.Reverse = Reverse === true;
        this.GetValueFunc = tp.IsFunction(GetValueFunc) ? GetValueFunc : null;
    }
};
/**
 * The property name or array index.
 * @type {string|number}
 */
tp.SortInfo.prototype.Prop = "";
/**
 * True to sort in descending order.
 * @type {boolean}
 */
tp.SortInfo.prototype.Reverse = false;
/**
 * Optional callback returning the sortable value.
 * @type {Function|null}
 */
tp.SortInfo.prototype.GetValueFunc = null;
/**
 * Sorts an array in place by multiple properties.
 * @param {object[]} List A collection of plain objects or arrays.
 * @param {Array<string|number|tp.SortInfo|object>} SortInfos The sort information items.
 * @returns {void}
 */
tp.ListSort = function (List, SortInfos) {
    var InfoList = [];
    var Index;
    var Info;
    /**
     * Returns the value to sort by.
     * @param {object|Array} Row The row being sorted.
     * @param {tp.SortInfo|object} Item The sort information item.
     * @returns {*} Returns the value.
     */
    function GetValueFunc(Row, Item) {
        return Row ? Row[Item.Prop] : null;
    }
    /**
     * Compares two rows.
     * @param {*} A The first row.
     * @param {*} B The second row.
     * @returns {number} Returns the comparison result.
     */
    function CompareFunc(A, B) {
        var ItemIndex;
        var Item;
        var ValueA;
        var ValueB;
        var Result = 0;
        for (ItemIndex = 0; ItemIndex < InfoList.length; ItemIndex++) {
            Item = InfoList[ItemIndex];
            ValueA = Item.GetValueFunc(A, Item);
            ValueB = Item.GetValueFunc(B, Item);
            Result = ValueA === ValueB ? 0 : (Item.Reverse ? (ValueA > ValueB ? -1 : 1) : (ValueA < ValueB ? -1 : 1));
            if (Result !== 0)
                break;
        }
        return Result;
    }
    if (!tp.IsArray(List) || !tp.IsArray(SortInfos) || SortInfos.length === 0)
        return;
    for (Index = 0; Index < SortInfos.length; Index++) {
        if (tp.IsNumber(SortInfos[Index]) || tp.IsString(SortInfos[Index])) {
            Info = new tp.SortInfo(SortInfos[Index], false, GetValueFunc);
        } else {
            Info = SortInfos[Index];
            if (!tp.IsFunction(Info.GetValueFunc))
                Info.GetValueFunc = GetValueFunc;
        }
        InfoList.push(Info);
    }
    List.sort(CompareFunc);
};
/**
 * Filter comparison operators.
 * @enum {number}
 */
tp.FilterOp = {
    None: 0,
    /** Greater than. */
    GT: 1,
    /** Greater than or equal. */
    GE: 2,
    /** Equal. */
    EQ: 4,
    /** Not equal. */
    NE: 8,
    /** Less than. */
    LT: 0x10,
    /** Less than or equal. */
    LE: 0x20,
    /** Contains. */
    CO: 0x40,
    /** Starts with. */
    SW: 0x80,
    /** Ends with. */
    EW: 0x100
};
/**
 * Greater than.
 * @type {number}
 */
tp.FilterOp.Greater = tp.FilterOp.GT;
/**
 * Greater than or equal.
 * @type {number}
 */
tp.FilterOp.GreaterOrEqual = tp.FilterOp.GE;
/**
 * Equal.
 * @type {number}
 */
tp.FilterOp.Equal = tp.FilterOp.EQ;
/**
 * Not equal.
 * @type {number}
 */
tp.FilterOp.NotEqual = tp.FilterOp.NE;
/**
 * Less than.
 * @type {number}
 */
tp.FilterOp.Less = tp.FilterOp.LT;
/**
 * Less than or equal.
 * @type {number}
 */
tp.FilterOp.LessOrEqual = tp.FilterOp.LE;
/**
 * Contains.
 * @type {number}
 */
tp.FilterOp.Contains = tp.FilterOp.CO;
/**
 * Starts with.
 * @type {number}
 */
tp.FilterOp.StartsWith = tp.FilterOp.SW;
/**
 * Ends with.
 * @type {number}
 */
tp.FilterOp.EndsWith = tp.FilterOp.EW;
/**
 * Compares two values using a filter operator.
 * @param {number} Operator The comparison operator.
 * @param {*} A The first value.
 * @param {*} B The second value.
 * @returns {boolean} Returns true when the comparison passes.
 */
tp.FilterOp.Compare = function (Operator, A, B) {
    if (A === tp.Undefined)
        A = null;
    if (B === tp.Undefined)
        B = null;
    if (A instanceof Date)
        A = A.valueOf();
    if (B instanceof Date)
        B = B.valueOf();
    switch (Operator) {
        case tp.FilterOp.Greater: return A > B;
        case tp.FilterOp.GreaterOrEqual: return A >= B;
        case tp.FilterOp.Equal: return A === B;
        case tp.FilterOp.NotEqual: return A !== B;
        case tp.FilterOp.Less: return A < B;
        case tp.FilterOp.LessOrEqual: return A <= B;
        case tp.FilterOp.Contains: return tp.ContainsText(A, B, true);
        case tp.FilterOp.StartsWith: return tp.StartsWith(A, B, true);
        case tp.FilterOp.EndsWith: return tp.EndsWith(A, B, true);
    }
    return false;
};
Object.freeze(tp.FilterOp);
/**
 * Information object for array filtering.
 */
tp.FilterInfo = class {
    // ● constructor
    /**
     * Creates filter information.
     * @param {string|number|null|undefined} Prop The property name or array index.
     * @param {*} Value The filter value.
     * @param {number|null|undefined} Operator The filter operator.
     * @param {Function|null|undefined} FilterFunc Optional callback returning whether the row passes.
     */
    constructor(Prop, Value, Operator, FilterFunc) {
        this.Prop = tp.IsNullOrUndefined(Prop) ? "" : Prop;
        this.Value = Value;
        this.Operator = Operator || tp.FilterOp.Equal;
        this.FilterFunc = tp.IsFunction(FilterFunc) ? FilterFunc : null;
    }
};
/**
 * The property name or array index.
 * @type {string|number}
 */
tp.FilterInfo.prototype.Prop = "";
/**
 * The filter value.
 * @type {*}
 */
tp.FilterInfo.prototype.Value = null;
/**
 * The filter operator.
 * @type {number}
 */
tp.FilterInfo.prototype.Operator = tp.FilterOp.Equal;
/**
 * Optional callback returning whether the row passes.
 * @type {Function|null}
 */
tp.FilterInfo.prototype.FilterFunc = null;
/**
 * Filters an array by multiple properties.
 * @param {object[]} List A collection of plain objects or arrays.
 * @param {Array<tp.FilterInfo|object>} FilterInfos The filter information items.
 * @param {boolean|null|undefined} OrLogic True to apply OR logic; false to apply AND logic.
 * @returns {object[]} Returns a new filtered array.
 */
tp.ListFilter = function (List, FilterInfos, OrLogic) {
    var InfoList = [];
    var Index;
    var Info;
    /**
     * Tests whether a row passes a filter item.
     * @param {object|Array} Row The row being filtered.
     * @param {tp.FilterInfo|object} Item The filter information item.
     * @returns {boolean} Returns true when the row passes.
     */
    function FilterFunc(Row, Item) {
        var Value = Row ? Row[Item.Prop] : null;
        return tp.FilterOp.Compare(Item.Operator, Value, Item.Value);
    }
    /**
     * Tests whether a row passes all filter items.
     * @param {object|Array} Row The row being filtered.
     * @returns {boolean} Returns true when the row passes.
     */
    function ArrayFilterFunc(Row) {
        var ItemIndex;
        var Item;
        var Result = OrLogic === true ? false : true;
        for (ItemIndex = 0; ItemIndex < InfoList.length; ItemIndex++) {
            Item = InfoList[ItemIndex];
            Result = Item.FilterFunc(Row, Item);
            if (OrLogic !== true && Result !== true)
                break;
            if (OrLogic === true && Result === true)
                break;
        }
        return Result;
    }
    if (!tp.IsArray(List))
        return [];
    if (!tp.IsArray(FilterInfos) || FilterInfos.length === 0)
        return List.slice();
    for (Index = 0; Index < FilterInfos.length; Index++) {
        Info = FilterInfos[Index];
        Info.Operator = Info.Operator || tp.FilterOp.Equal;
        if (!tp.IsFunction(Info.FilterFunc))
            Info.FilterFunc = FilterFunc;
        InfoList.push(Info);
    }
    return List.filter(ArrayFilterFunc);
};

// ● array predicates and transforms
/**
 * Returns true when any item passes a predicate.
 * @param {Array|object} List The array-like object.
 * @param {Function} Func The predicate function.
 * @param {object|null|undefined} Context The callback context.
 * @returns {boolean} Returns true when any item passes.
 */
tp.Any = function (List, Func, Context) {
    return tp.IsArrayLike(List) && tp.IsFunction(Func) ? Array.prototype.some.call(List, Func, Context || null) : false;
};
/**
 * Returns true when all items pass a predicate.
 * @param {Array|object} List The array-like object.
 * @param {Function} Func The predicate function.
 * @param {object|null|undefined} Context The callback context.
 * @returns {boolean} Returns true when all items pass.
 */
tp.All = function (List, Func, Context) {
    return tp.IsArrayLike(List) && tp.IsFunction(Func) ? Array.prototype.every.call(List, Func, Context || null) : false;
};
/**
 * Maps an array-like object to a new array.
 * @param {Array|object} List The array-like object.
 * @param {Function} Func The mapping function.
 * @param {object|null|undefined} Context The callback context.
 * @returns {Array} Returns the mapped array.
 */
tp.Transform = function (List, Func, Context) {
    return tp.IsArrayLike(List) && tp.IsFunction(Func) ? Array.prototype.map.call(List, Func, Context || null) : [];
};
/**
 * Returns a new array with distinct values of a specified property.
 * @param {Array|object} List The array-like object.
 * @param {string} Prop The property name.
 * @returns {Array} Returns the distinct items.
 */
tp.Distinct = function (List, Prop) {
    var Seen = new Set();
    var Result = [];
    var Items = tp.ToArray(List);
    var Index;
    var Item;
    var Value;
    for (Index = 0; Index < Items.length; Index++) {
        Item = Items[Index];
        Value = Item ? Item[Prop] : undefined;
        if (!Seen.has(Value)) {
            Seen.add(Value);
            Result.push(Item);
        }
    }
    return Result;
};
/**
 * Returns a new array containing items with a specified property value.
 * @param {Array|object} List The array-like object.
 * @param {string} Prop The property name.
 * @param {*} Value The property value.
 * @returns {Array} Returns the matching items.
 */
tp.Where = function (List, Prop, Value) {
    return tp.ToArray(List).filter(function (Item) {
        return Item && Item[Prop] === Value;
    });
};
/**
 * Returns a new array containing items that match all properties of an object.
 * @param {Array|object} List The array-like object.
 * @param {object} Props The properties to match.
 * @returns {Array} Returns the matching items.
 */
tp.WhereAll = function (List, Props) {
    var Names = tp.GetPropertyNames(Props);
    return tp.ToArray(List).filter(function (Item) {
        var Index;
        if (!Item)
            return false;
        for (Index = 0; Index < Names.length; Index++) {
            if (Item[Names[Index]] !== Props[Names[Index]])
                return false;
        }
        return true;
    });
};
/**
 * Returns the first item that passes a predicate, or null.
 * @param {Array|object} List The array-like object.
 * @param {Function} Func The predicate function.
 * @param {object|null|undefined} Context The callback context.
 * @returns {*} Returns the first matching item or null.
 */
tp.FirstOrDefault = function (List, Func, Context) {
    var Items = tp.ToArray(List);
    var Index;
    if (!tp.IsFunction(Func))
        return Items.length > 0 ? Items[0] : null;
    for (Index = 0; Index < Items.length; Index++) {
        if (Func.call(Context || null, Items[Index], Index, Items) === true)
            return Items[Index];
    }
    return null;
};
