/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

public class TripousDesktopException : ApplicationException
{
    // ● constructor
    public TripousDesktopException() { }
    public TripousDesktopException(string Message) : base(Message) { }
    public TripousDesktopException(string Message, Exception InnerException) : base(Message, InnerException) { }
}