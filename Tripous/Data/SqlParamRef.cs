namespace Tripous.Data;

public class SqlParamRef
{
    // ● private
    readonly string fName;
    readonly int fIndex;

    // ● constructor
    public SqlParamRef(string Name, int Index)
    {
        fName = Name;
        fIndex = Index;
    }

    public override string ToString() => Name;
 

    // ● properties
    public string Name { get { return fName; } }
    public int Index { get { return fIndex; } }
}