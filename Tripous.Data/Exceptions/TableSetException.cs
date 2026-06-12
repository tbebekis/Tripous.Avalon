/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Base class for all exceptions thrown by the TableSet class.
/// </summary>
public class TableSetException : ApplicationException
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public TableSetException() { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TableSetException(string Message) : base(Message) { }
    /// <summary>
    /// Constructor
    /// </summary>
    public TableSetException(string Message, Exception InnerException) : base(Message, InnerException) { }
}