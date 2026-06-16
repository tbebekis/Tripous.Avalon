/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// A type to be used with Ids when the exact type is not known.
/// </summary>
[JsonConverter(typeof(IDJsonConverter))]
public struct ID
{
    object value = "";

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ID()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public ID(object Source)
    {
        Set(Source);
    }

    // ● public 
    /// <summary>
    /// Determines whether a specified object is equal to this object.
    /// </summary>
    public override bool Equals([NotNullWhen(true)] object obj)
    {
        if (obj == null)
            return this.IsNull();

        ID Other;
        if (obj is ID)
        {
            Other = (ID)obj;
        }
        else if (obj.GetType() == typeof(string) || obj.GetType() == typeof(int) || obj.GetType() == typeof(short))
        {
            Other = new ID(obj);
        }
        else
        {
            return base.Equals(obj);
        }

        if (this.IsNull() || Other.IsNull())
            return this.IsNull() && Other.IsNull();

        if (IsNumber(this.AsString()) && IsNumber(Other.AsString()))
            return this.AsInt().Equals(Other.AsInt());

        return this.AsString().Equals(Other.AsString());
    }
    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    public override int GetHashCode()
    {
        if (IsNumber(this.AsString()))
            return this.AsInt().GetHashCode();
        if (this.IsString())
            return this.AsString().GetHashCode();
        return base.GetHashCode();
    }
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString()
    {
        return AsString();
    }

    /// <summary>
    /// Sets the internal value. The source should be string, integer or short integer.
    /// </summary>
    public void Set(object Source)
    {
        if (Source == null)
        {
            value = null;
        }
        else if (Source.GetType() == typeof(string))
        {
            value = Source as string;
        }
        else if (Source.GetType() == typeof(int))
        {
            value = Convert.ToInt32(Source);
        }
        else if (Source.GetType() == typeof(short))
        {
            value = Convert.ToInt16(Source);
        }
        else if (Source is ID)
        {
            value = ((ID)Source).value;
        }
        else
        {
            throw new Exception($"Cannot assign an ID from an illegal value: {Source}");
        }
    }

    /// <summary>
    /// Returns the internal value.
    /// </summary>
    public object Get() => value;
    /// <summary>
    /// Returns the internal value as string.
    /// </summary>
    public string AsString() => !IsNull() ? value.ToString() : "";
    /// <summary>
    /// Returns the internal value as integer, if the internal value is a number or looks like a number.
    /// </summary>
    public int AsInt() => !IsNull() && IsNumber(value.ToString()) ? Convert.ToInt32(value) : 0;

    /// <summary>
    /// True when the internal value is null.
    /// </summary>
    public bool IsNull() => value == null;
    /// <summary>
    /// True when the internal value is int or short.
    /// </summary>
    public bool IsInt() => !IsNull() && (value.GetType() == typeof(int) || value.GetType() == typeof(short));
    /// <summary>
    /// True when the internal value is string.
    /// </summary>
    public bool IsString() => !IsNull() && value.GetType() == typeof(string);

    /// <summary>
    /// True when is null or empty string.
    /// </summary>
    public bool IsEmpty() => value == null || (IsString() && string.IsNullOrWhiteSpace(AsString()));

    // ● static
    /// <summary>
    /// True when a specified string value consists of digits.
    /// </summary>
    static public bool IsNumber(string Value)
    {
        if (string.IsNullOrWhiteSpace(Value))
            return false;

        foreach (char C in Value)
            if (!char.IsDigit(C))
                return false;

        return true;
    }

    /// <summary>
    /// Returns true if two specified Ids are equal.
    /// <para>The specified Ids can be of any integer type or string.</para>
    /// <para>String Ids are compared case-sensitively.</para>
    /// </summary>
    static public bool AreEqual(object A, object B)
    {
        ID Id1 = new ID(A);
        ID Id2 = new ID(B);
        return Id1.Equals(Id2);
    }

    /// <summary>
    /// Returns true when a specified Id is null or empty string.
    /// </summary>
    static public bool IsEmpty(object Id)
    {
        ID Id1 = new ID(Id);
        return Id1.IsEmpty();
    }
    /// <summary>
    /// Returns true if a specified object Id is null, empty string, or equals to a value, such as the  <c>-</c>
    /// </summary>
    static public bool IsEmptyOrValue(object Id, object Value)
    {
        ID Id1 = new ID(Id);
        if (Id1.IsEmpty())
            return true;

        ID Id2 = new ID(Value);
        return Id1.Equals(Id2);
    }

    /// <summary>
    /// Creates and returns a new Guid string.
    /// <para>If UseBrackets is true, the new guid is surrounded by {}</para>
    /// </summary>
    static public string GenId(bool UseBrackets)
    {
        string format = UseBrackets ? "B" : "D";
        return Guid.NewGuid().ToString(format).ToUpper();
    }
    /// <summary>
    /// Creates and returns a new Guid string WITHOUT surrounding brackets, i.e. {}
    /// </summary>
    static public string GenId()
    {
        return GenId(false);
    }

    // ● operators
    /// <summary>
    /// Operator ==
    /// </summary>
    public static bool operator ==(ID left, ID right) => left.Equals(right);
    /// <summary>
    /// Operator !=
    /// </summary>
    public static bool operator !=(ID left, ID right) => !(left == right);

    /// <summary>
    /// Implicit conversion from string, short and int to ID.
    /// </summary>
    static public implicit operator ID(string Source) => new ID(Source);
    /// <summary>
    /// Implicit conversion from short and int to ID.
    /// </summary>
    static public implicit operator ID(short Source) => new ID(Source);
    /// <summary>
    /// Implicit conversion from short and int to ID.
    /// </summary>
    static public implicit operator ID(int Source) => new ID(Source);

    /// <summary>
    /// Implicit conversion from ID to string, short and int.
    /// </summary>
    static public implicit operator string(ID Source) => Source.value != null ? Source.value.ToString() : string.Empty;
    /// <summary>
    /// Implicit conversion from ID to short and int.
    /// </summary>
    static public implicit operator short(ID Source) => Convert.ToInt16(Source.AsInt());
    /// <summary>
    /// Implicit conversion from ID to short and int.
    /// </summary>
    static public implicit operator int(ID Source) => Source.AsInt();
}

/// <summary>
/// A json converter for the <see cref="ID"/> type.
/// </summary>
public class IDJsonConverter : JsonConverter<ID>
{
    /// <summary>
    /// Overrides the default behavior of reading a value.
    /// </summary>
    public override ID Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        string S = reader.GetString();

        if (ID.IsNumber(S))
        {
            int Source = Convert.ToInt32(S);
            return new ID(Source);
        }
        return new ID(S);
    }
    /// <summary>
    /// Overrides the default behavior of writing a value.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, ID value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.AsString());
    }
}
