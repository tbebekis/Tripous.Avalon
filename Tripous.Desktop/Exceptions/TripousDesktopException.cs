/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Represents errors that occur in Tripous desktop services and UI.
/// </summary>
public class TripousDesktopException : ApplicationException
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="TripousDesktopException"/> class.
    /// </summary>
    public TripousDesktopException() { }
    /// <summary>
    /// Initializes a new instance of the <see cref="TripousDesktopException"/> class.
    /// </summary>
    /// <param name="Message">The error message.</param>
    public TripousDesktopException(string Message) : base(Message) { }
    /// <summary>
    /// Initializes a new instance of the <see cref="TripousDesktopException"/> class.
    /// </summary>
    /// <param name="Message">The error message.</param>
    /// <param name="InnerException">The inner exception.</param>
    public TripousDesktopException(string Message, Exception InnerException) : base(Message, InnerException) { }
}
