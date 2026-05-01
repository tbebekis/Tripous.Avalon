namespace Tripous.Data;

public class TripousDataException : ApplicationException
{
    // ● constructor
    public TripousDataException() { }
    public TripousDataException(string Message) : base(Message) { }
    public TripousDataException(string Message, Exception InnerException) : base(Message, InnerException) { }
}