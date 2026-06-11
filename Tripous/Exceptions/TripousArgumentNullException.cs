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
public class TripousArgumentNullException : ArgumentNullException
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousArgumentNullException() { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousArgumentNullException(string ParamName) : base(ParamName) { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousArgumentNullException(string ParamName, string Message): base(ParamName, Message) { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TripousArgumentNullException(string Message, Exception InnerException) : base(Message, InnerException) { }
}