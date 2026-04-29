namespace Tripous.Data;

// ● public
public class DataModuleException : ApplicationException
{
    // ● constructor
    public DataModuleException() { }
    public DataModuleException(string Message) : base(Message) { }
    public DataModuleException(string Message, Exception InnerException) : base(Message, InnerException) { }
}

 
