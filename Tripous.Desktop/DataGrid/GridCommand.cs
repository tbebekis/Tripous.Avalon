/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Defines the standard command action types a grid may support.
/// </summary>
public enum GridActionType
{
    /// <summary>
    /// Adds a row.
    /// </summary>
    Add,
    /// <summary>
    /// Deletes a row.
    /// </summary>
    Delete,
    /// <summary>
    /// Edits a row.
    /// </summary>
    Edit,
    /// <summary>
    /// Executes a custom action.
    /// </summary>
    Custom,
}

/// <summary>
/// Describes a command exposed by a grid toolbar, shortcut, or context menu.
/// </summary>
public class GridCommand
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GridCommand"/> class.
    /// </summary>
    public GridCommand()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the command action type.
    /// </summary>
    public GridActionType ActionType { get; set; }
    /// <summary>
    /// Gets or sets the command name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the command title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// Gets or sets the command tooltip.
    /// </summary>
    public string ToolTip { get; set; }
    /// <summary>
    /// Gets or sets the image file name.
    /// </summary>
    public string ImageFileName { get; set; }
    /// <summary>
    /// Gets or sets the command key gesture.
    /// </summary>
    public KeyGesture KeyGesture { get; set; }
    /// <summary>
    /// Gets or sets a value indicating whether the command is enabled.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
    /// <summary>
    /// Gets or sets a value indicating whether the command is visible.
    /// </summary>
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// Provides the runtime context used when a grid command is checked or executed.
/// </summary>
public class GridCommandContext
{
    // ● properties
    /// <summary>
    /// Gets or sets the grid command.
    /// </summary>
    public GridCommand Command { get; set; }
    /// <summary>
    /// Gets or sets the data grid.
    /// </summary>
    public DataGrid Grid { get; set; }
    /// <summary>
    /// Gets or sets the table bound to the grid.
    /// </summary>
    public MemTable Table { get; set; }
}

/// <summary>
/// Provides the runtime context used by detail table grid commands.
/// </summary>
public class DetailGridCommandContext: GridCommandContext
{
    // ● properties
    /// <summary>
    /// Gets or sets the detail table information.
    /// </summary>
    public UiDetailTableInfo DetailInfo { get; set; }
    /// <summary>
    /// Gets or sets the item context.
    /// </summary>
    public UiItemContext ItemContext { get; set; }
}

/// <summary>
/// Provides commands and executes them for a grid.
/// </summary>
public interface IGridHandler
{
    /// <summary>
    /// Returns the grid commands provided by this handler.
    /// </summary>
    /// <returns>The grid commands.</returns>
    GridCommand[] GetGridCommands();
    /// <summary>
    /// Returns true when a grid command can execute.
    /// </summary>
    /// <param name="Context">The grid command context.</param>
    /// <returns>True if the command can execute; otherwise, false.</returns>
    bool CanExecute(GridCommandContext Context);
    /// <summary>
    /// Executes a grid command.
    /// </summary>
    /// <param name="Context">The grid command context.</param>
    /// <returns>The command result.</returns>
    object Execute(GridCommandContext Context);
}
