/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */


namespace Tripous.Data;
 
public class TripousBusinessException : ApplicationException
{
    // ● constructor
    public TripousBusinessException() { }
    public TripousBusinessException(string Message) : base(Message) { }
    public TripousBusinessException(string Message, Exception InnerException) : base(Message, InnerException) { }
}