namespace Tripous
{
    using System.Diagnostics.CodeAnalysis;
    using System.Text.Json;
    using System.Text.Json.Serialization;

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
            if (obj == null && this.IsNull())
                return true;

            if (obj == null && !this.IsNull())
                return false;

            if (obj != null && this.IsNull())
                return false;

            if (obj.GetType() == typeof(string))
                return this.IsString() ? this.AsString().Equals(obj) : false;


            if ((obj.GetType() == typeof(int) || obj.GetType() == typeof(short)))
                return this.IsInt() ? this.AsInt().Equals(Convert.ToInt32(obj)) : false;

            if (obj is ID)
            {
                ID Other = (ID)obj;

                if (this.IsNull() && Other.IsNull())
                    return true;

                if (this.IsString() && Other.IsString())
                    return this.AsString().Equals(Other.AsString());

                if (this.IsInt() && Other.IsInt())
                    return this.AsInt().Equals(Other.AsInt());

                return false;
            }

            return base.Equals(obj);
        }
        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        public override int GetHashCode()
        {
            if (this.IsInt())
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
        /// Returns true if a specified object Id is null, empty string or equals to a value, such as the <see cref="Lib.EmptyValue"/>, i.e. <c>-</c>
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
        public static bool operator ==(ID left, ID right) => left.Equals(right);
        public static bool operator !=(ID left, ID right) => !(left == right);

        static public implicit operator ID(string Source) => new ID(Source);
        static public implicit operator ID(short Source) => new ID(Source);
        static public implicit operator ID(int Source) => new ID(Source);

        static public implicit operator string(ID Source) => Source.value != null ? Source.value.ToString() : string.Empty;
        static public implicit operator short(ID Source) => (Source.value != null && Source.value.GetType() == typeof(short)) ? Convert.ToInt16(Source.value) : (short)0;
        static public implicit operator int(ID Source) => Source.value != null && Source.value.GetType() == typeof(int) ? Convert.ToInt32(Source.value) : (int)0;
    }

    /// <summary>
    /// A json converter for the <see cref="ID"/> type.
    /// </summary>
    public class IDJsonConverter : JsonConverter<ID>
    {
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

        public override void Write(Utf8JsonWriter writer, ID value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.AsString());
        }
    }
}
