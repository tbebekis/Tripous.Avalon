namespace Tripous;

/// <summary>
/// The data-type of a data field
/// </summary>
[Flags]
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DataFieldType
{
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// String (nvarchar, varchar)
    /// </summary>
    String = 1,
    /// <summary>
    /// Integer
    /// </summary>
    Integer = 2,
    /// <summary>
    /// Double (float, double precision, etc)
    /// </summary>
    Double = 4,
    /// <summary>
    /// Decimal (decimal(18, 4))
    /// <para>Implied Precision and Scale <c>(18, 4)</c></para>
    /// <para>Example: <c>@DECIMAL</c> becomes <c>decimal(18, 4)</c></para>
    /// </summary>
    Decimal = 8,
    /// <summary>
    /// Decimal (decimal(?, ?))
    /// <para>The user provides the Precision and Scale explicitly.</para>
    /// <para>Example: <c>@DECIMAL_(10, 2)</c> becomes <c>decimal(10, 2)</c></para>
    /// </summary>
    Decimal_ = 0x10,
    /// <summary>
    /// Date (date)
    /// </summary>
    Date = 0x20,
    /// <summary>
    /// DateTime (datetime, timestamp, etc)
    /// </summary>
    DateTime = 0x40,
    /// <summary>
    /// Boolean (integer always, 1 = true, else false)
    /// </summary>
    Boolean = 0x80,
    /// <summary>
    /// Blob
    /// </summary>
    Blob = 0x100,
    /// <summary>
    /// Text Blob
    /// </summary>
    TextBlob = 0x200,
}


/// <summary>
/// Helper
/// </summary>
static public class DataFieldTypeHelper
{

    /// <summary>
    /// Returns the <see cref="DataFieldType"/> corresponding to a <see cref="Type"/>.
    /// </summary>
    static public DataFieldType DataFieldTypeOf(this Type T)
    {
        if (T != null)
        {
            TypeCode Code = System.Type.GetTypeCode(T);

            if ((T == typeof(byte[]) || (T == typeof(object))))
            {
                return DataFieldType.Blob;
            }
            else
            {
                switch (Code)
                {
                    case TypeCode.Boolean: return DataFieldType.Boolean;
                    case TypeCode.Char: return DataFieldType.String;
                    case TypeCode.SByte: return DataFieldType.Integer;
                    case TypeCode.Byte: return DataFieldType.Integer;
                    case TypeCode.Int16: return DataFieldType.Integer;
                    case TypeCode.UInt16: return DataFieldType.Integer;
                    case TypeCode.Int32: return DataFieldType.Integer;
                    case TypeCode.UInt32: return DataFieldType.Integer;
                    case TypeCode.Int64: return DataFieldType.Integer;
                    case TypeCode.UInt64: return DataFieldType.Integer;
                    case TypeCode.Single: return DataFieldType.Double;
                    case TypeCode.Double: return DataFieldType.Double;
                    case TypeCode.Decimal: return DataFieldType.Decimal;
                    case TypeCode.DateTime: return DataFieldType.DateTime;
                    case TypeCode.String: return DataFieldType.String;
                }
            }
        }

        return DataFieldType.Unknown;
    }

    static public bool IsNumeric(this DataFieldType Value) => Value.In(DataFieldType.Integer | DataFieldType.Double | DataFieldType.Decimal | DataFieldType.Decimal_);  
    /// <summary>
    /// True if Value is DateTime or Date or Time
    /// </summary>
    static public bool IsDateTime(this DataFieldType Value) => Value == DataFieldType.DateTime || Value == DataFieldType.Date;  
    /// <summary>
    /// Returns true if Value is strictly DateTime
    /// </summary>
    static public bool IsDateTimeStrict(this DataFieldType Value) => Value == DataFieldType.DateTime;
    /// <summary>
    /// Returns true if Value is strictly Date
    /// </summary>
    static public bool IsDateStrict(this DataFieldType Value) => Value == DataFieldType.Date;
    /// <summary>
    /// True if Value is Memo or Graphic or Blob
    /// </summary>
    static public bool IsBlob(this DataFieldType Value) => Value == DataFieldType.Blob || Value == DataFieldType.TextBlob;  

    /// <summary>
    /// Gets the Type value of Value
    /// </summary>
    static public Type GetNetType(this DataFieldType Value)
    {
        switch (Value)
        {
            case DataFieldType.String: return typeof(System.String);
            case DataFieldType.Integer: return typeof(System.Int32);
            case DataFieldType.Double: return typeof(System.Double);
            case DataFieldType.Decimal: return typeof(System.Decimal);
            case DataFieldType.Date: return typeof(System.DateTime);
            case DataFieldType.DateTime: return typeof(System.DateTime);
            case DataFieldType.Boolean: return typeof(System.Int32);
            case DataFieldType.Blob: return typeof(byte[]);
            case DataFieldType.TextBlob: return typeof(System.String);
        }

        return null; 

    }

}