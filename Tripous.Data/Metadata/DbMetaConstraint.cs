namespace Tripous.Data;

public enum ConstraintType
{
    Unknown,
    PrimaryKey,
    ForeignKey,
    Unique,
    Check,
    NotNull,
}

public class DbMetaConstraint : DbMetaObject
{
    public string ConstraintTypeText  { get; set; }
    public ConstraintType ConstraintType { get; set; }          // enum: PrimaryKey, ForeignKey, Unique, Check
    public string Columns { get; set; }  
    
    public override string DisplayText
    {
        get
        {
            string Result = Name;
            
            if (!string.IsNullOrWhiteSpace(Columns))
                Result += $" ({Columns})";
            
            if (!string.IsNullOrWhiteSpace(ConstraintTypeText))
                Result += $" {ConstraintTypeText}";
 
            return Result;
        }
    }
    
}