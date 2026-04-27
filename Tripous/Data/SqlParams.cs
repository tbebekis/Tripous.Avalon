namespace Tripous.Data;

public class SqlParams
{
    // ● private
    readonly List<SqlParam> fItems = new List<SqlParam>();

    // ● public
    public SqlParams Add(string Name, object Value)
    {
        fItems.Add(new SqlParam(Name, Value));
        return this;
    }
    public int Count { get { return fItems.Count; } }
    public SqlParam this[int Index] { get { return fItems[Index]; } }
    public List<SqlParam> Items { get { return fItems; } }
}