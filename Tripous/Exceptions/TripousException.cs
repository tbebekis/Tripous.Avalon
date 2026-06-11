/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Tripous exception
/// </summary>
public class TripousException : ApplicationException
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousException() { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousException(string Message) : base(Message) { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousException(string Message, Exception InnerException) : base(Message, InnerException) { }
}

