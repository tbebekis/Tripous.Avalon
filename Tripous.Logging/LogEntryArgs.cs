/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Logging;

/// <summary>
/// Provides data for an event that reports a log entry.
/// </summary>
public class LogEntryArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntryArgs"/> class.
    /// </summary>
    /// <param name="Entry">The log entry associated with the event.</param>
    public LogEntryArgs(LogEntry Entry)
    {
        this.Entry = Entry;
    }

    /// <summary>
    /// Gets the log entry associated with the event.
    /// </summary>
    public LogEntry Entry { get; }
}
 
