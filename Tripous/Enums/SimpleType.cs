namespace Tripous;

[Flags]
public enum SimpleType
{
    None = 0,           // N
    String = 1,         // S
    Integer = 2,        // I
    Boolean = 4,        // L    - Logical
    Double = 8,         // F    - Float
    Decimal = 0x10,     // C    - Currency
    DateTime = 0x20,    // D
    Text = 0x40,        // T
    Graphic = 0x80,     // G
    Blob = 0x100        // B
}

 
/// <summary>
/// Extensions
/// </summary>
static public class Simple
{
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
    
    /* type convertion */
    /// <summary>
    /// Gets the Type value of Value
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
    /// Gets the simple type of Value
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
                case TypeCode.Decimal: return SimpleType.Double;
                case TypeCode.DateTime: return SimpleType.DateTime;
                case TypeCode.String: return SimpleType.String;
            }
        }

        return SimpleType.None;
    }
    /// <summary>
    /// Gets the simple type of Value
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
    /// Gets the simple type of Value. If it is null or DbNull, then SimpleType.None is returned.
    /// </summary>
    static public SimpleType SimpleTypeOf(object Value)
    {
        if ((Value == null) || (DBNull.Value == Value))
            return SimpleType.None;

        return Simple.SimpleTypeOf(Value.GetType());
    }
    /// <summary>
    /// Gets the simple type of the character Value
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
    /// Gets the char value of Value
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



    /* IsXXXXX methods */
    /// <summary>
    /// True if Value is String or WideString string
    /// </summary>
    static public bool IsString(this SimpleType Value)
    {
        return (Value & SimpleType.String) != SimpleType.None;
    }
    /// <summary>
    /// True if Value is Boolean
    /// </summary>
    static public bool IsBoolean(this SimpleType Value)
    {
        return Value == SimpleType.Boolean;
    }

    /// <summary>
    /// True if Value is DateTime or Date or Time
    /// </summary>
    static public bool IsDateTime(this SimpleType Value)
    {
        return (Value & SimpleType.DateTime) != SimpleType.None;
    }


    /// <summary>
    /// Returns true if Value is Integer
    /// </summary>
    static public bool IsInteger(this SimpleType Value)
    {
        return (Value == SimpleType.Integer);
    }
    /// <summary>
    /// True if Value is Float or Currency 
    /// </summary>
    static public bool IsFloat(this SimpleType Value)
    {
        return (Value & (SimpleType.Double | SimpleType.Decimal)) != SimpleType.None;
    }
    /// <summary>
    /// Returns true if Value is Float, Currency or Integer
    /// </summary>
    static public bool IsNumeric(this SimpleType Value)
    {
        return (Value.IsFloat()) || (Value == SimpleType.Integer);
    }
    /// <summary>
    /// True if Value is Memo or Graphic or Blob
    /// </summary>
    static public bool IsBlob(this SimpleType Value)
    {
        return (Value & (SimpleType.Text | SimpleType.Graphic | SimpleType.Blob)) != SimpleType.None;
    }

    /// <summary>
    /// True if Value is String or WideString string
    /// </summary>
    static public bool IsString(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsString();
    }
    /// <summary>
    /// True if Value is Boolean
    /// </summary>
    static public bool IsBoolean(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsBoolean();
    }
    /// <summary>
    /// True if Value is DateTime or Date or Time
    /// </summary>
    static public bool IsDateTime(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsDateTime();
    }
    /// <summary>
    /// Returns true if Value is Integer
    /// </summary>
    static public bool IsInteger(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsInteger();
    }
    /// <summary>
    /// True if Value is Float or Currency 
    /// </summary>
    static public bool IsFloat(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsFloat();
    }
    /// <summary>
    /// Returns true if Value is Float, Currency or Integer
    /// </summary>
    static public bool IsNumeric(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsNumeric();
    }
    /// <summary>
    /// True if Value is Memo or Graphic or Blob
    /// </summary>
    static public bool IsBlob(Type Value)
    {
        return Simple.SimpleTypeOf(Value).IsBlob();
    }

 
}