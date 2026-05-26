/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Binding information for simple controls (single-line controls, lookup combo-boxes and locator boxes).
/// </summary>
public class ControlBinding: TripousBinding
{
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public ControlBinding()
    {
    }

    // ● public
    /// <summary>
    /// The control of this binding
    /// </summary>
    public Control Control { get; set; }
}