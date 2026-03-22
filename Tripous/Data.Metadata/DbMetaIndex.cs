using System.Linq;

namespace Tripous;

public class DbMetaIndex : DbMetaObject
{
    public bool IsUnique { get; set; }
    public string IndexType { get; set; }                 // BTREE, HASH, κλπ
    public string Columns { get; set; } 
  
    public override string DisplayText
    {
        get
        {
            string Result = Name + $" ({Columns})";
            if (!string.IsNullOrWhiteSpace(IndexType))
                Result += $" ({IndexType})";
            else if (IsUnique)
                Result += $" Unique";
 
            return Result;
        }
    }
 
    
    /*

    */
}