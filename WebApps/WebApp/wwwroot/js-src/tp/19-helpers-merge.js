// ● merging
/**
 * Returns true when a destination member can receive a value.
 * Missing properties are considered writable.
 * @param {object} Dest The destination object.
 * @param {string} PropName The property name.
 * @returns {boolean} Returns true when the property can be assigned.
 */
tp.CanAssignProperty = function (Dest, PropName) {
    var Descriptor;
    if (tp.IsNil(Dest) || tp.IsBlank(PropName))
        return false;
    Descriptor = tp.GetPropertyDescriptor(Dest, PropName);
    if (!Descriptor)
        return true;
    return Descriptor.writable === true || tp.IsFunction(Descriptor.set);
};
/**
 * Clones a value for deep merge purposes.
 * Primitive values are returned as-is.
 * @param {*} Value The value to clone.
 * @returns {*} Returns the cloned value.
 */
tp.CloneValue = function (Value) {
    var Result;
    var Index;
    if (tp.IsDate(Value))
        return tp.DateClone(Value);
    if (tp.IsSimple(Value))
        return Value;
    if (tp.IsArray(Value)) {
        Result = [];
        for (Index = 0; Index < Value.length; Index++)
            Result[Index] = tp.CloneValue(Value[Index]);
        return Result;
    }
    if (tp.IsCloneable(Value))
        return Value.Clone();
    if (tp.IsPlainObject(Value))
        return tp.MergeProps({}, Value, true);
    return Value;
};
/**
 * Merges properties of source objects to a destination object.
 * When DeepMerge is true, arrays and plain objects are deeply copied.
 * @param {object|Array} Dest The destination object. It is returned as the result.
 * @param {object|object[]} Sources The source object or an array of source objects.
 * @param {boolean|null|undefined} DeepMerge True for deep merge; false for shallow merge.
 * @returns {object|Array|null} Returns the destination object, or null when Dest is null or undefined.
 */
tp.MergeProps = function (Dest, Sources, DeepMerge) {
    var SourceList;
    var Source;
    var PropNames;
    var PropName;
    var SourceValue;
    var DestValue;
    var Index;
    var PropIndex;
    if (tp.IsNil(Dest))
        return null;
    if (tp.IsNil(Sources))
        return Dest;
    DeepMerge = DeepMerge !== false;
    SourceList = tp.IsArray(Sources) ? Sources : [Sources];
    for (Index = 0; Index < SourceList.length; Index++) {
        Source = SourceList[Index];
        if (tp.IsNil(Source))
            continue;
        PropNames = tp.GetPropertyNames(Source);
        for (PropIndex = 0; PropIndex < PropNames.length; PropIndex++) {
            PropName = PropNames[PropIndex];
            if (!tp.CanAssignProperty(Dest, PropName))
                continue;
            SourceValue = Source[PropName];
            if (SourceValue === Dest)
                continue;
            if (DeepMerge !== true) {
                Dest[PropName] = SourceValue;
            } else if (tp.IsDate(SourceValue)) {
                Dest[PropName] = tp.DateClone(SourceValue);
            } else if (tp.IsSimple(SourceValue)) {
                Dest[PropName] = SourceValue;
            } else if (tp.IsArray(SourceValue)) {
                Dest[PropName] = tp.CloneValue(SourceValue);
            } else if (tp.IsCloneable(SourceValue)) {
                Dest[PropName] = SourceValue.Clone();
            } else if (tp.IsPlainObject(SourceValue)) {
                DestValue = tp.IsPlainObject(Dest[PropName]) ? Dest[PropName] : {};
                Dest[PropName] = tp.MergeProps(DestValue, SourceValue, true);
            } else {
                Dest[PropName] = SourceValue;
            }
        }
    }
    return Dest;
};
/**
 * Deep-merges properties of source objects to a destination object.
 * @param {object|Array} Dest The destination object. It is returned as the result.
 * @param {object|object[]} Sources The source object or an array of source objects.
 * @returns {object|Array|null} Returns the destination object, or null when Dest is null or undefined.
 */
tp.MergePropsDeep = function (Dest, Sources) {
    return tp.MergeProps(Dest, Sources, true);
};
/**
 * Shallow-merges properties of source objects to a destination object.
 * @param {object|Array} Dest The destination object. It is returned as the result.
 * @param {object|object[]} Sources The source object or an array of source objects.
 * @returns {object|Array|null} Returns the destination object, or null when Dest is null or undefined.
 */
tp.MergePropsShallow = function (Dest, Sources) {
    return tp.MergeProps(Dest, Sources, false);
};
