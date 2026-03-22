namespace Tripous;

public class DbMetaForeignKey: DbMetaConstraint
{
    public string ForeignTable { get; set; }
    public string ForeignFields { get; set; }
    public string UpdateRule { get; set; }
    public string DeleteRule { get; set; }
 
    public override string DisplayText
    {
        get
        {
            string Result = Name;
            
            if (!string.IsNullOrWhiteSpace(Columns))
                Result += $" ({Columns})";

            if (!string.IsNullOrWhiteSpace(ForeignTable))
            {
                Result += $" references ({ForeignTable})";
                if (!string.IsNullOrWhiteSpace(ForeignFields))
                    Result += $" ({ForeignFields})";
            }
 
            return Result;
        }
    }
   
}