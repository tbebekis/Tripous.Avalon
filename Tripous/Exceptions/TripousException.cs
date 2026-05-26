/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

public class TripousException : ApplicationException
{
    // ● constructor
    public TripousException() { }
    public TripousException(string Message) : base(Message) { }
    public TripousException(string Message, Exception InnerException) : base(Message, InnerException) { }
}

