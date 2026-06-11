/// <summary>
/// Defines a small set of generic data types used throughout the framework.
/// </summary>
[Flags]
public enum SimpleType
{
    /// <summary>
    /// None
    /// </summary>
    None = 0,           // N
    /// <summary>
    /// String
    /// </summary>
    String = 1,         // S
    /// <summary>
    /// Integer
    /// </summary>
    Integer = 2,        // I
    /// <summary>
    /// Boolean
    /// </summary>
    Boolean = 4,        // L - Logical
    /// <summary>
    /// Double
    /// </summary>
    Double = 8,         // F - Float
    /// <summary>
    /// Decimal
    /// </summary>
    Decimal = 0x10,     // C - Currency
    /// <summary>
    /// DateTime
    /// </summary>
    DateTime = 0x20,    // D
    /// <summary>
    /// Text
    /// </summary>
    Text = 0x40,        // T
    /// <summary>
    /// Graphic
    /// </summary>
    Graphic = 0x80,     // G
    /// <summary>
    /// Blob
    /// </summary>
    Blob = 0x100        // B
}

/// <summary>
/// Provides helper methods for converting, inspecting and working
/// with <see cref="SimpleType"/> values.
/// </summary>
static public class Simple
{
    /// <summary>
    /// Converts a <see cref="SimpleType"/> value to the corresponding .NET type.
    /// </summary>
    static public Type ToType(this SimpleType SimpleType)
    {
        switch (SimpleType)
        {
            case SimpleType.String : return typeof(string);
            case SimpleType.Integer: return  typeof(int);
            case SimpleType.Boolean: return typeof(bool);
            case SimpleType.Double: return typeof(double);
            case SimpleType.Decimal: return typeof(decimal);
            case SimpleType.DateTime: return typeof(DateTime);
            case SimpleType.Text: return typeof(string);
            case SimpleType.Graphic: return typeof(byte[]);
            case SimpleType.Blob: return typeof(byte[]);
        }
        
        return  null;
    }
    
    // ● type conversion
    /// <summary>
    /// Converts a <see cref="SimpleType"/> value to the corresponding .NET type.
    /// </summary>
    static public Type GetNetType(this SimpleType Value)
    {
        switch (Value)
        {
            case SimpleType.String: return typeof(System.String);
            case SimpleType.Integer: return typeof(System.Int32);
            case SimpleType.Boolean: return typeof(System.Boolean);
            case SimpleType.Double: return typeof(System.Double);
            case SimpleType.Decimal: return typeof(System.Decimal);
            case SimpleType.DateTime: return typeof(System.DateTime);
            case SimpleType.Text: return typeof(System.String);
            case SimpleType.Graphic: return typeof(byte[]);
            case SimpleType.Blob: return typeof(byte[]);
        }

        return null;  
    }
    /// <summary>
    /// Returns the corresponding <see cref="SimpleType"/> value
    /// for a .NET type.
    /// </summary>
    static public SimpleType SimpleTypeOf(Type Value)
    {
        if (Value != null)
        {
            TypeCode Code = System.Type.GetTypeCode(Value);

            switch (Code)
            {
                case TypeCode.Empty: return SimpleType.None;
                case TypeCode.Object: return SimpleType.None;
                case TypeCode.DBNull: return SimpleType.None;
                case TypeCode.Boolean: return SimpleType.Boolean;
                case TypeCode.Char: return SimpleType.None;
                case TypeCode.SByte: return SimpleType.Integer;
                case TypeCode.Byte: return SimpleType.Integer;
                case TypeCode.Int16: return SimpleType.Integer;
                case TypeCode.UInt16: return SimpleType.Integer;
                case TypeCode.Int32: return SimpleType.Integer;
                case TypeCode.UInt32: return SimpleType.Integer;
                case TypeCode.Int64: return SimpleType.Integer;
                case TypeCode.UInt64: return SimpleType.Integer;
                case TypeCode.Single: return SimpleType.Double;
                case TypeCode.Double: return SimpleType.Double;
                case TypeCode.Decimal: return SimpleType.Decimal;
                case TypeCode.DateTime: return SimpleType.DateTime;
                case TypeCode.String: return SimpleType.String;
            }
        }

        return SimpleType.None;
    }
    /// <summary>
    /// Returns the corresponding <see cref="SimpleType"/> value
    /// for a database type.
    /// </summary>
    static public SimpleType SimpleTypeOf(DbType Value)
    {
        switch (Value)
        {
            case DbType.AnsiString: return SimpleType.String;
            case DbType.Binary: return SimpleType.Blob;
            case DbType.Byte: return SimpleType.Integer;
            case DbType.Boolean: return SimpleType.Boolean;
            case DbType.Currency: return SimpleType.Decimal;
            case DbType.Date: return SimpleType.DateTime;
            case DbType.DateTime: return SimpleType.DateTime;
            case DbType.Decimal: return SimpleType.Decimal;
            case DbType.Double: return SimpleType.Double;
            case DbType.Guid: return SimpleType.String;
            case DbType.Int16: return SimpleType.Integer;
            case DbType.Int32: return SimpleType.Integer;
            case DbType.Int64: return SimpleType.Integer;
            case DbType.Object: return SimpleType.Blob;
            case DbType.SByte: return SimpleType.Integer;
            case DbType.Single: return SimpleType.Double;
            case DbType.String: return SimpleType.String;
            case DbType.Time: return SimpleType.DateTime;
            case DbType.UInt16: return SimpleType.Integer;
            case DbType.UInt32: return SimpleType.Integer;
            case DbType.UInt64: return SimpleType.Integer;
            case DbType.VarNumeric: return SimpleType.Blob;
            case DbType.AnsiStringFixedLength: return SimpleType.String;
            case DbType.StringFixedLength: return SimpleType.String;
            case DbType.Xml: return SimpleType.Blob;
            case DbType.DateTime2: return SimpleType.DateTime;
            case DbType.DateTimeOffset: return SimpleType.DateTime;
        }

        return SimpleType.None;
    }
 
    /// <summary>
    /// Returns the corresponding <see cref="SimpleType"/> value
    /// for an object instance.
    /// </summary>
    static public SimpleType SimpleTypeOf(object Value)
    {
        if ((Value == null) || (DBNull.Value == Value))
            return SimpleType.None;

        return Simple.SimpleTypeOf(Value.GetType());
    }
    /// <summary>
    /// Converts a type code character to a <see cref="SimpleType"/> value.
    /// </summary>
    static public SimpleType SimpleTypeOf(char Value)
    {
        switch (char.ToUpper(Value))
        {
            case 'S': return SimpleType.String;
            case 'I': return SimpleType.Integer;
            case 'L': return SimpleType.Boolean;
            case 'F': return SimpleType.Double;
            case 'C': return SimpleType.Decimal;
            case 'D': return SimpleType.DateTime;
            case 'T': return SimpleType.Text;
            case 'G': return SimpleType.Graphic;
            case 'B': return SimpleType.Blob;
        }

        return SimpleType.None;
    }
    /// <summary>
    /// Converts a <see cref="SimpleType"/> value to its type code character.
    /// </summary>
    static public char ToChar(this SimpleType Value)
    {
        switch (Value)
        {
            case SimpleType.String: return 'S';
            case SimpleType.Integer: return 'I';
            case SimpleType.Boolean: return 'L';
            case SimpleType.Double: return 'F';
            case SimpleType.Decimal: return 'C';
            case SimpleType.DateTime: return 'D';
            case SimpleType.Text: return 'T';
            case SimpleType.Graphic: return 'G';
            case SimpleType.Blob: return 'B';
        }

        return 'N';
    }

    // ● IsXXXXX methods
    /// <summary>
    /// Returns true when the value represents a string type.
    /// </summary>
    static public bool IsString(this SimpleType Value)
    {
        return (Value & SimpleType.String) != SimpleType.None;
    }
    /// <summary>
    /// Returns true when the value represents a Boolean type.
    /// </summary>
    static public bool IsBoolean(this SimpleType Value)
    {
        return Value == SimpleType.Boolean;
    }
    /// <summary>
    /// Returns true when the value represents a date/time type.
    /// </summary>
    static public bool IsDateTime(this SimpleType Value)
    {
        return (Value & SimpleType.DateTime) != SimpleType.None;
    }
    /// <summary>
    /// Returns true when the value represents an integer type.
    /// </summary>
    static public bool IsInteger(this SimpleType Value)
    {
        return (Value == SimpleType.Integer);
    }
    /// <summary>
    /// Returns true when the value represents a floating-point
    /// or decimal type.
    /// </summary>
    static public bool IsFloat(this SimpleType Value)
    {
        return (Value & (SimpleType.Double | SimpleType.Decimal)) != SimpleType.None;
    }
    /// <summary>
    /// Returns true when the value represents a numeric type.
    /// </summary>
    static public bool IsNumeric(this SimpleType Value)
    {
        return (Value.IsFloat()) || (Value == SimpleType.Integer);
    }
    /// <summary>
    /// Returns true when the value represents a text, graphic
    /// or binary large object type.
    /// </summary>
    static public bool IsBlob(this SimpleType Value)
    {
        return (Value & (SimpleType.Text | SimpleType.Graphic | SimpleType.Blob)) != SimpleType.None;
    }

    /// <summary>
    /// Returns true when the specified .NET type is a string type.
    /// </summary>
    static public bool IsString(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsString();
    }
    /// <summary>
    /// Returns true when the specified .NET type is a Boolean type.
    /// </summary>
    static public bool IsBoolean(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsBoolean();
    }
    /// <summary>
    /// Returns true when the specified .NET type is a date/time type.
    /// </summary>
    static public bool IsDateTime(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsDateTime();
    }
    /// <summary>
    /// Returns true when the specified .NET type is an integer type.
    /// </summary>
    static public bool IsInteger(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsInteger();
    }
    /// <summary>
    /// Returns true when the specified .NET type is a floating-point
    /// or decimal type.
    /// </summary>
    static public bool IsFloat(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsFloat();
    }
    /// <summary>
    /// Returns true when the specified .NET type is a numeric type.
    /// </summary>
    static public bool IsNumeric(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsNumeric();
    }
    /// <summary>
    /// Returns true when the specified .NET type is a text, graphic
    /// or binary large object type.
    /// </summary>
    static public bool IsBlob(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsBlob();
    }
}