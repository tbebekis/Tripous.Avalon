/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

public class TripousDataException : ApplicationException
{
    // ● constructor
    public TripousDataException() { }
    public TripousDataException(string Message) : base(Message) { }
    public TripousDataException(string Message, Exception InnerException) : base(Message, InnerException) { }
}