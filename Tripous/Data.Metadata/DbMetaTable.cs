using System.Text;

namespace Tripous;

public class DbMetaTable : DbMetaObject
{
    public string GetFieldNameList() => string.Join($", {Environment.NewLine}", Columns.Select(x => x.Name));

    public string GetCreateTable()
    {
        StringBuilder SB = new();
        
        SB.AppendLine($"create table {Name} ( ");
        string FieldList = string.Join($", {Environment.NewLine}", Columns.Select(x => "  " + x.DisplayText));
        SB.AppendLine(FieldList);
        SB.AppendLine(")");
        
        return SB.ToString();
    }
    
    public List<DbMetaColumn>     Columns     { get; } = new();
    //public DbMetaConstraint       PrimaryKey  { get; set; }
    public List<DbMetaForeignKey> ForeignKeys { get; } = new();
    public List<DbMetaConstraint> Constraints { get; } = new();
    public List<DbMetaIndex>      Indexes     { get; } = new();
    public List<DbMetaTrigger>    Triggers    { get; } = new();   
}