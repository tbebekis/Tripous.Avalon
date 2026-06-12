/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Base class for all exceptions thrown by the data layer.
/// </summary>
public class TripousDataException : ApplicationException
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousDataException() { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousDataException(string Message) : base(Message) { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousDataException(string Message, Exception InnerException) : base(Message, InnerException) { }
}