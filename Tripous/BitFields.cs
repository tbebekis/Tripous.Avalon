namespace Tripous;

static public class BitFields
{
    static public bool In<T>(this T Value, T Mask) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long m = Convert.ToInt64(Mask);
        return (v & m) == v;
    }
    static public bool Has<T>(this T Value, T Flag) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flag);
        return (v & f) == f;
    }
    static public bool HasAny<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (v & f) != 0;
    }
    static public bool HasNone<T>(this T Value, T Flags) where T : struct, Enum
    {
        return !Value.HasAny(Flags);
    }
    static public T Add<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (T)Enum.ToObject(typeof(T), v | f);
    }
    static public T Remove<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (T)Enum.ToObject(typeof(T), v & ~f);
    }
    static public T Toggle<T>(this T Value, T Flags) where T : struct, Enum
    {
        long v = Convert.ToInt64(Value);
        long f = Convert.ToInt64(Flags);
        return (T)Enum.ToObject(typeof(T), v ^ f);
    }
    static public T Set<T>(this T Value, T Flags, bool Enabled) where T : struct, Enum
    {
        return Enabled ? Value.Add(Flags) : Value.Remove(Flags);
    }
    static public bool IsZero<T>(this T Value) where T : struct, Enum
    {
        return Convert.ToInt64(Value) == 0;
    }
    static public bool IsExactly<T>(this T Value, T Flags) where T : struct, Enum
    {
        return Convert.ToInt64(Value) == Convert.ToInt64(Flags);
    }
 
}