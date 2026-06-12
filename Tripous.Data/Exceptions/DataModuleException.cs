/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

// ● public
/// <summary>
/// Base class for all exceptions thrown by the Data module.
/// </summary>
public class DataModuleException : ApplicationException
{
    // ● constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public DataModuleException() { }
    /// <summary>
    /// Constructor
    /// </summary>
    public DataModuleException(string Message) : base(Message) { }
    /// <summary>
    /// Constructor
    /// </summary>
    public DataModuleException(string Message, Exception InnerException) : base(Message, InnerException) { }
}

 
