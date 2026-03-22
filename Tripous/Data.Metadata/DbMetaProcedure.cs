namespace Tripous;

public class DbMetaProcedure : DbMetaObject
{
    public string ProcedureType { get; set; }              // procedure vs function
    
    public override string DisplayText
    {
        get
        {
            string Result = Name;
            
            if (!string.IsNullOrWhiteSpace(ProcedureType))
                Result += $" ({ProcedureType})";
 
            return Result;
        }
    }
}