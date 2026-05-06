namespace Tripous;


/// <summary>
/// A command is actually a named callback function.
/// <para>It can be used in menus, toolbars and treeviews.</para>
/// </summary>
public class Command: BaseDef
{
    private string fImageFileName;
    DefList<Command> fCommands;
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public Command()
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public Command(string Name, string TitleKey, string ImageFileName)
    {
        this.Name = Name;
        this.TitleKey = TitleKey;
        this.ImageFileName = ImageFileName;
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public Command(string Name)
        : this(Name, "", "", null)
    {
    }

    // ● construction - sync Execute()
    /// <summary>
    /// Constructor with sync <see cref="Execute"/>
    /// </summary>
    public Command(string Name, Func<Command, object> ExecuteFunc)
        : this(Name, "", "")
    {
        this.ExecuteFunc = ExecuteFunc;
    }
    /// <summary>
    /// Constructor with sync <see cref="Execute"/>
    /// </summary>
    public Command(string Name, string ImageFileName, Func<Command, object> ExecuteFunc)
        : this(Name, "", ImageFileName)
    {
        this.ExecuteFunc = ExecuteFunc;
    }
    /// <summary>
    /// Constructor with sync <see cref="Execute"/>
    /// </summary>
    public  Command(string Name, string TitleKey, string ImageFileName, Func<Command, object> ExecuteFunc)
        : this(Name, TitleKey, ImageFileName)
    {
        this.ExecuteFunc = ExecuteFunc;
    }
    
    // ● construction - async ExecuteAsync()
    /// <summary>
    /// Constructor with async <see cref="ExecuteAsync"/>
    /// </summary>
    public Command(string Name, Func<Command, Task<object>> ExecuteAsyncFunc)
        : this(Name, "", "")
    {
        this.ExecuteAsyncFunc = ExecuteAsyncFunc;
    }
    /// <summary>
    /// Constructor with async <see cref="ExecuteAsync"/>
    /// </summary>
    public Command(string Name, string ImageFileName, Func<Command, Task<object>> ExecuteAsyncFunc)
        : this(Name, "", ImageFileName)
    {
        this.ExecuteAsyncFunc = ExecuteAsyncFunc;
    }
    /// <summary>
    /// Constructor with async <see cref="ExecuteAsync"/>
    /// </summary>
    public Command(string Name, string TitleKey, string ImageFileName, Func<Command, Task<object>> ExecuteAsyncFunc)
        : this(Name, TitleKey, ImageFileName)
    {
        this.ExecuteAsyncFunc = ExecuteAsyncFunc;
    }
    
    // ● public
    /// <summary>
    /// True if this is an executable command.
    /// </summary>
    public bool CanExecute() => CanExecuteFunc != null ? CanExecuteFunc(this) : true;
    /// <summary>
    /// Executes this command.
    /// </summary>
    public object Execute() => ExecuteFunc != null && CanExecute() ? ExecuteFunc(this) : null;
    /// <summary>
    /// Executes this command.
    /// </summary>
    public async Task<object> ExecuteAsync() => ExecuteAsyncFunc != null && CanExecute()? await ExecuteAsyncFunc(this) : null;

    // ● properties
    /// <summary>
    /// The file name of an image. Used when a command is displayed in toolbars or treeviews.
    /// </summary>
    public string ImageFileName
    {
        get => fImageFileName;
        init { if (fImageFileName != value) { fImageFileName = value; NotifyPropertyChanged(nameof(ImageFileName)); } }
    }
    /// <summary>
    /// A list of child commands. Could be empty.
    /// </summary>
    public DefList<Command> Commands
    {
        get => fCommands ??= [];
        init { if (fCommands != value) { fCommands = value; NotifyPropertyChanged(nameof(Commands)); } }
    }

    /// <summary>
    /// Returns true if this a sync command.
    /// <para>A command has no idea of what to execute.</para>
    /// <para>The caller code should assign a callback function to <see cref="ExecuteFunc"/>.</para>
    /// </summary>
    public bool IsSync => ExecuteFunc != null;
    /// <summary>
    /// Returns true if this an async command.
    /// <para>A command has no idea of what to execute.</para>
    /// <para>The caller code should assign a callback function  to <see cref="ExecuteAsyncFunc"/>.</para>
    /// </summary>
    public bool IsAsync => ExecuteAsyncFunc != null;

    /// <summary>
    /// A callback. It is called just before command execution. Returning false, cancels the execution.
    /// </summary>
    public Func<Command, bool> CanExecuteFunc { get; set; }
    /// <summary>
    /// A callback that executes the command.
    /// </summary>
    public Func<Command, object> ExecuteFunc { get; set; }
    /// <summary>
    /// A callback that executes the command.
    /// </summary>
    public Func<Command, Task<object>> ExecuteAsyncFunc { get; set; }

    /// <summary>
    /// True when this is a container command.
    /// </summary>
    public bool HasChildren => Commands != null && Commands.Count > 0;
    [JsonIgnore] public override bool IsSerializable => false;
 
}