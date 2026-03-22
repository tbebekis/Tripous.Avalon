namespace Tripous;

public class DbMetaTrigger : DbMetaObject
{
    public string TableName { get; set; }            // σε ποια table (ή null αν database-level)
    public string TriggerType { get; set; }                // INSERT, UPDATE, DELETE BEFORE, AFTER
 
    public override string DisplayText
    {
        get
        {
            string Result = Name;
            if (!string.IsNullOrWhiteSpace(TriggerType))
                Result += $" {TriggerType}";
            return Result;
        }
    }
}