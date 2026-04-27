namespace Tripous.Data;

public class DataLinkOp
{
    public DataLinkOp()
    {
    }

    public DataLinkOp(string Module, DataLinkOpType Type, object Params = null)
    {
        this.Module = Module;
        this.Type = Type;
        this.Params = Params;
    }
    public DataLinkOp(ModuleDef ModuleDef, DataLinkOpType Type, object Params = null)
        : this(ModuleDef.Name, Type, Params)
    {
    }

    public string OpId { get; } = Sys.GenId(UseBrackets: false);
    public DataLinkOpType Type { get; set; }
    public string ProcName { get; set; }
    public string Module { get; set; }
    public object Params { get; set; }
    public string Culture { get; set; } = "en-US";
}