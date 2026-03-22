namespace Tripous;

public class DbMetaView : DbMetaObject
{
    public string GetFieldNameList() => string.Join($", {Environment.NewLine}", Columns.Select(x => x.Name));
    
    public List<DbMetaColumn> Columns { get; } = new();
    
    
    
}