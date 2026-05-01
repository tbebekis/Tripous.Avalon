namespace Tripous;

public class TripousArgumentNullException : ArgumentNullException
{
    // ● constructor
    public TripousArgumentNullException() { }
    public TripousArgumentNullException(string ParamName) : base(ParamName) { }
    public TripousArgumentNullException(string ParamName, string Message): base(ParamName, Message) { }
    public TripousArgumentNullException(string Message, Exception InnerException) : base(Message, InnerException) { }
}