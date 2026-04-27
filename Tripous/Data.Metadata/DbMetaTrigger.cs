namespace Tripous.Data;

public class DbMetaTrigger : DbMetaObject
{
    public string TableName { get; set; }                 
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