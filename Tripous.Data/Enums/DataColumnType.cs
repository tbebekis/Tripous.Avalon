namespace Tripous.Data;

[Flags]
public enum DataColumnType
{
    None        = 0x0000,
    Text        = 0x0001,
    Boolean     = 0x0002,
    Date        = 0x0004,
    DateTime    = 0x0008,
    Integer     = 0x0010,
    Decimal     = 0x0020,
    Currency    = 0x0040,
    Image       = 0x0080,
    Memo        = 0x0100,
    Lookup      = 0x0200,
}