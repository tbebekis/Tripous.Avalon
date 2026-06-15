/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Defines the result of a modal dialog.
/// </summary>
public enum ModalResult
{
    /// <summary>
    /// No modal result has been assigned.
    /// </summary>
    None,
    /// <summary>
    /// The dialog was accepted.
    /// </summary>
    Ok,
    /// <summary>
    /// The dialog was cancelled.
    /// </summary>
    Cancel
}
