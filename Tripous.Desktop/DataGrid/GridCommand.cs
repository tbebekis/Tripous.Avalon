namespace Tripous.Desktop;

/// <summary>
/// Defines the standard command action types a grid may support.
/// </summary>
public enum GridActionType
{
    Add,
    Delete,
    Edit,
    Custom,
}

/// <summary>
/// Describes a command exposed by a grid toolbar, shortcut, or context menu.
/// </summary>
public class GridCommand
{
    // ● constructor
    public GridCommand()
    {
    }

    // ● properties
    public GridActionType ActionType { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string ToolTip { get; set; }
    public string ImageFileName { get; set; }
    public KeyGesture KeyGesture { get; set; }
    public bool IsEnabled { get; set; } = true;
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// Provides the runtime context used when a grid command is checked or executed.
/// </summary>
public class GridCommandContext
{
    // ● properties
    public GridCommand Command { get; set; }
    public DataGrid Grid { get; set; }
    public MemTable Table { get; set; }
}

/// <summary>
/// Provides the runtime context used by detail table grid commands.
/// </summary>
public class DetailGridCommandContext: GridCommandContext
{
    // ● properties
    public UiDetailTableInfo DetailInfo { get; set; }
    public UiItemContext ItemContext { get; set; }
}

/// <summary>
/// Provides commands and executes them for a grid.
/// </summary>
public interface IGridHandler
{
    GridCommand[] GetCommands();
    bool CanExecute(GridCommandContext Context);
    object Execute(GridCommandContext Context);
}
