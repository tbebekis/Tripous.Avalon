namespace Tripous.Data;

public class SqlParam
{
    // ● private
    string fName;
    object fValue;

    // ● constructor
    public SqlParam(string Name, object Value)
    {
        fName = Name;
        fValue = Value;
    }

    // ● public
    public string Name { get { return fName; } }
    public object Value { get { return fValue; } }
}