/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Provides helper extension methods for working with enum bit fields.
///
/// Supports testing, adding, removing and toggling flag values
/// in enums marked with the <see cref="FlagsAttribute"/> attribute.
/// </summary>
static public class BitFields
{
    /// <summary>
    /// Returns true when the specified value is fully contained
    /// in the specified mask.
    /// </summary>
    static public bool In<T>(this T Value, T Mask) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long m = Convert.ToInt64(Mask);
        return (v & m) == v;
    }
    /// <summary>
    /// Returns true when the specified flag or flags are present.
    /// </summary>
    static public bool Has<T>(this T Value, T Flag) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flag);
        return (v & f) == f;
    }
    /// <summary>
    /// Returns true when at least one of the specified flags is present.
    /// </summary>
    static public bool HasAny<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (v & f) != 0;
    }
    /// <summary>
    /// Returns true when none of the specified flags is present.
    /// </summary>
    static public bool HasNone<T>(this T Value, T Flags) where T : struct, Enum
    {
        return !Value.HasAny(Flags);
    }
    /// <summary>
    /// Adds the specified flags and returns the resulting value.
    /// </summary>
    static public T Add<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (T)Enum.ToObject(typeof(T), v | f);
    }
    /// <summary>
    /// Removes the specified flags and returns the resulting value.
    /// </summary>
    static public T Remove<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (T)Enum.ToObject(typeof(T), v & ~f);
    }
    /// <summary>
    /// Toggles the specified flags and returns the resulting value.
    /// </summary>
    static public T Toggle<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (T)Enum.ToObject(typeof(T), v ^ f);
    }
    /// <summary>
    /// Adds or removes the specified flags depending on the
    /// value of the Enabled argument.
    /// </summary>
    static public T Set<T>(this T Value, T Flags, bool Enabled) where T : struct, Enum
    {
        return Enabled ? Value.Add(Flags) : Value.Remove(Flags);
    }
    /// <summary>
    /// Returns true when the value is zero.
    /// </summary>
    static public bool IsZero<T>(this T Value) where T : struct, Enum
    {
        return Convert.ToInt64(Value) == 0;
    }
    /// <summary>
    /// Returns true when the value exactly matches the specified flags.
    /// </summary>
    static public bool IsExactly<T>(this T Value, T Flags) where T : struct, Enum
    {
        return Convert.ToInt64(Value) == Convert.ToInt64(Flags);
    }
}