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
