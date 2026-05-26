/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

public class TableSetException : ApplicationException
{
    // ● constructor
    public TableSetException() { }
    public TableSetException(string Message) : base(Message) { }
    public TableSetException(string Message, Exception InnerException) : base(Message, InnerException) { }
}