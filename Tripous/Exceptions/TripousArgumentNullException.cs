/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

public class TripousArgumentNullException : ArgumentNullException
{
    // ● constructor
    public TripousArgumentNullException() { }
    public TripousArgumentNullException(string ParamName) : base(ParamName) { }
    public TripousArgumentNullException(string ParamName, string Message): base(ParamName, Message) { }
    public TripousArgumentNullException(string Message, Exception InnerException) : base(Message, InnerException) { }
}