/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;

/// <summary>
/// Represents a named application command.
/// 
/// A command may execute a synchronous or asynchronous callback,
/// open a form, or act as a container for child commands.
/// </summary>
public class Command: BaseDef
{
    string fForm;
    string fImageFileName;
    DefList<Command> fCommands;

    // ● private methods
    bool IsAllowed(UserLevel UserLevel)
    {
        if (SecurityLevel == UserLevel.None)
            return true;
        if ((UserLevel & UserLevel.God) == UserLevel.God)
            return true;
        if ((UserLevel & UserLevel.Admin) == UserLevel.Admin)
            return SecurityLevel == UserLevel.Admin || SecurityLevel == UserLevel.User || SecurityLevel == UserLevel.Guest;
        if ((UserLevel & UserLevel.User) == UserLevel.User)
            return SecurityLevel == UserLevel.User || SecurityLevel == UserLevel.Guest;
        if ((UserLevel & UserLevel.Guest) == UserLevel.Guest)
            return SecurityLevel == UserLevel.Guest;
        return (UserLevel & SecurityLevel) == SecurityLevel;
    }
    
    // ● construction
    /// <summary>
    /// Creates a command with a name, image file name and optional title key.
    /// </summary>
    static public Command Create(string Name, string ImageFileName, string TitleKey = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName };
        return Result;
    }
    /// <summary>
    /// Creates a synchronous command with a name, image file name,
    /// callback and optional title key.
    /// </summary>
    static public Command Create(string Name, string ImageFileName, Func<Command, object> ExecuteFunc, string TitleKey = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, ExecuteFunc = ExecuteFunc };
        return Result;
    }
    /// <summary>
    /// Creates an asynchronous command with a name, image file name,
    /// callback and optional title key.
    /// </summary>
    static public Command CreateAsync(string Name, string ImageFileName, Func<Command, Task<object>> ExecuteAsyncFunc, string TitleKey = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, ExecuteAsyncFunc = ExecuteAsyncFunc };
        return Result;
    }
    /// <summary>
    /// Creates a synchronous command with optional form, title key and image file name.
    /// </summary>
    static public Command Create(string Name, Func<Command, object> ExecuteFunc, string Form = null, string TitleKey = null, string ImageFileName = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, Form =  Form, ExecuteFunc = ExecuteFunc };
        return Result;
    }
    /// <summary>
    /// Creates an asynchronous command with optional form, title key and image file name.
    /// </summary>
    static public Command CreateAsync(string Name, Func<Command, Task<object>> ExecuteAsyncFunc, string Form = null, string TitleKey = null, string ImageFileName = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, Form =  Form, ExecuteAsyncFunc = ExecuteAsyncFunc };
        return Result;
    }
    /// <summary>
    /// Creates a command that opens a form.
    /// </summary>
    static public Command CreateForm(string Name, string Form, string TitleKey = null, string ImageFileName = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, Form =  Form };
        return Result;
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public Command()
    {
    }
    /// <summary>
    /// Constructs a command with the specified name.
    /// </summary>
    public Command(string Name)
    {
        this.Name = Name;
    }
 
    // ● public
    /// <summary>
    /// Returns true when this command can execute.
    /// </summary>
    public bool CanExecute() => CanExecuteFunc != null ? CanExecuteFunc(this) : true;
    /// <summary>
    /// Executes this command synchronously.
    /// </summary>
    public object Execute()
    {
        if (!CanExecute())
            return null;

        return ExecuteFunc != null ? ExecuteFunc(this) : null;
    }
    /// <summary>
    /// Executes this command asynchronously.
    /// </summary>
    public async Task<object> ExecuteAsync()
    {
        if (!CanExecute())
            return null;

        if (ExecuteAsyncFunc != null)
            return await ExecuteAsyncFunc(this);

        return ExecuteFunc != null ? ExecuteFunc(this) : null;
    }
    /// <summary>
    /// Returns true when the specified user may see or execute this command.
    /// </summary>
    public bool CanAccess(AppUser User)
    {
        UserLevel UserLevel = User != null ? User.UserLevel : UserLevel.None;
        return IsAllowed(UserLevel);
    }

    // ● properties
    /// <summary>
    /// Gets the file name of the image used when this command
    /// is displayed in menus, toolbars or tree views.
    /// </summary>
    public string ImageFileName
    {
        get => fImageFileName;
        init { if (fImageFileName != value) { fImageFileName = value; NotifyPropertyChanged(nameof(ImageFileName)); } }
    }
    /// <summary>
    /// Gets the form name opened by this command.
    /// </summary>
    public string Form
    {
        get => fForm;
        init { if (fForm != value) { fForm = value; NotifyPropertyChanged(nameof(Form)); } }
    }
    /// <summary>
    /// Gets the child commands of this command.
    /// </summary>
    public DefList<Command> Commands
    {
        get => fCommands ??= [];
        init { if (fCommands != value) { fCommands = value; NotifyPropertyChanged(nameof(Commands)); } }
    }
    /// <summary>
    /// Gets a value indicating whether this command has a synchronous callback.
    /// </summary>
    public bool IsSync => ExecuteFunc != null;
    /// <summary>
    /// Gets a value indicating whether this command has an asynchronous callback.
    /// </summary>
    public bool IsAsync => ExecuteAsyncFunc != null;
    /// <summary>
    /// Gets or sets a value indicating whether this command toggles a Boolean value.
    /// </summary>
    public bool IsToggle { get; set; }
    /// <summary>
    /// Gets or sets the minimum user level required to view or execute this command.
    /// </summary>
    public UserLevel SecurityLevel { get; set; }
    /// <summary>
    /// Gets or sets the callback that determines whether this command can execute.
    /// </summary>
    public Func<Command, bool> CanExecuteFunc { get; set; }
    /// <summary>
    /// Gets or sets the synchronous callback that executes this command.
    /// </summary>
    public Func<Command, object> ExecuteFunc { get; set; }
    /// <summary>
    /// Gets or sets the asynchronous callback that executes this command.
    /// </summary>
    public Func<Command, Task<object>> ExecuteAsyncFunc { get; set; }
    /// <summary>
    /// Gets a value indicating whether this command contains child commands.
    /// </summary>
    public bool HasChildren => fCommands != null && fCommands.Count > 0;
    /// <summary>
    /// Gets a value indicating whether this command should be serialized.
    /// </summary>
    [JsonIgnore] public override bool IsSerializable => false;
}
