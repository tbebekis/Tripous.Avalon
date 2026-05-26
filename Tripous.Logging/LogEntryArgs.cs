/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Logging;

public class LogEntryArgs : EventArgs
{
    public LogEntryArgs(LogEntry Entry)
    {
        this.Entry = Entry;
    }

    public LogEntry Entry { get; }
}
 
