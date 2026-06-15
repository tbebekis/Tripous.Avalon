/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Defines the standard actions of a reference context menu.
/// </summary>
public enum ReferenceMenuActionType
{
    /// <summary>
    /// Shows the reference list.
    /// </summary>
    ShowList,
    /// <summary>
    /// Reloads reference data.
    /// </summary>
    Reload,
    /// <summary>
    /// Edits the selected reference item.
    /// </summary>
    Edit,
    /// <summary>
    /// Adds a new reference item.
    /// </summary>
    Add,
    /// <summary>
    /// Clears the reference value.
    /// </summary>
    Clear,
}

/// <summary>
/// Provides the runtime context used by a reference context menu action.
/// </summary>
public class ReferenceMenuCommandContext
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceMenuCommandContext"/> class.
    /// </summary>
    public ReferenceMenuCommandContext()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the action type.
    /// </summary>
    public ReferenceMenuActionType ActionType { get; set; }
    /// <summary>
    /// Gets or sets the reference context menu.
    /// </summary>
    public ReferenceContextMenu Menu { get; set; }
    /// <summary>
    /// Gets or sets the binding this command serves.
    /// </summary>
    public TripousBinding Binding { get; set; }
    /// <summary>
    /// Gets or sets the reference form name.
    /// </summary>
    public string FormName { get; set; }
    /// <summary>
    /// Gets or sets the reference row identifier.
    /// </summary>
    public object RowId { get; set; }
    /// <summary>
    /// Gets or sets the caller control.
    /// </summary>
    public Control Caller { get; set; }
    /// <summary>
    /// Gets or sets the form context produced by the command.
    /// </summary>
    public DataFormContext FormContext { get; set; }
    /// <summary>
    /// Gets or sets the command result.
    /// </summary>
    public object Result { get; set; }
}
