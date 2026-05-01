namespace Tripous.Desktop;

public class TripousDesktopException : ApplicationException
{
    // ● constructor
    public TripousDesktopException() { }
    public TripousDesktopException(string Message) : base(Message) { }
    public TripousDesktopException(string Message, Exception InnerException) : base(Message, InnerException) { }
}