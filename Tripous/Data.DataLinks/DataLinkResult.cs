namespace Tripous.Data;

public class DataLinkResult
{
    static public DataLinkResult OK(DataLinkOp Op, object Data, string Message = "OK")
    {
        return new() {OpId = Op.OpId, Data = Data, Result = true, Message =  Message};
    }
    static public DataLinkResult Error(DataLinkOp Op, string Message = "Unknown error.") 
    {
        return new DataLinkResult() { OpId = Op.OpId, Message =  Message };
    }
    
    public string OpId { get; set; }
    public bool Result { get; set; } = false;
    public string Message { get; set; } = "";
    public object Data { get; set; }
}