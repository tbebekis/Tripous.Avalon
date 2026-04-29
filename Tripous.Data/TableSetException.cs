namespace Tripous.Data;

public class TableSetException : ApplicationException
{
    // ● constructor
    public TableSetException() { }
    public TableSetException(string Message) : base(Message) { }
    public TableSetException(string Message, Exception InnerException) : base(Message, InnerException) { }
}