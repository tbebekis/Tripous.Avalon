namespace Tripous;

public class TripousException : ApplicationException
{
    // ● constructor
    public TripousException() { }
    public TripousException(string Message) : base(Message) { }
    public TripousException(string Message, Exception InnerException) : base(Message, InnerException) { }
}

