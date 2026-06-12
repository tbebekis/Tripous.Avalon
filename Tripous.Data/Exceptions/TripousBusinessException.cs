/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */


namespace Tripous.Data;
 
/// <summary>
/// Base class for all exceptions thrown by the business layer.
/// </summary>
public class TripousBusinessException : ApplicationException
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousBusinessException() { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousBusinessException(string Message) : base(Message) { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousBusinessException(string Message, Exception InnerException) : base(Message, InnerException) { }
}