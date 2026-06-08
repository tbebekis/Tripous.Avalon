/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;


/// <summary>
/// A command is actually a named callback function.
/// <para>It can be used in menus, toolbars and treeviews.</para>
/// </summary>
public class Command: BaseDef
{
    string fForm;
    string fImageFileName;
    DefList<Command> fCommands;
    
    // ● construction
    static public Command Create(string Name, string ImageFileName, string TitleKey = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName };
        return Result;
    }
    
    static public Command Create(string Name, string ImageFileName, Func<Command, object> ExecuteFunc, string TitleKey = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, ExecuteFunc = ExecuteFunc };
        return Result;
    }
    static public Command CreateAsync(string Name, string ImageFileName, Func<Command, Task<object>> ExecuteAsyncFunc, string TitleKey = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, ExecuteAsyncFunc = ExecuteAsyncFunc };
        return Result;
    }
    
    static public Command Create(string Name, Func<Command, object> ExecuteFunc, string Form = null, string TitleKey = null, string ImageFileName = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, Form =  Form, ExecuteFunc = ExecuteFunc };
        return Result;
    }
    static public Command CreateAsync(string Name, Func<Command, Task<object>> ExecuteAsyncFunc, string Form = null, string TitleKey = null, string ImageFileName = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, Form =  Form, ExecuteAsyncFunc = ExecuteAsyncFunc };
        return Result;
    }
    static public Command CreateForm(string Name, string Form, string TitleKey = null, string ImageFileName = null)
    {
        Command Result = new() { Name = Name, TitleKey =  TitleKey, ImageFileName = ImageFileName, Form =  Form };
        return Result;
    }
    
    /// <summary>
    /// Constructor
    /// </summary>
    public Command()
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public Command(string Name)
    {
        this.Name = Name;
    }
 
    
    // ● public
    /// <summary>
    /// True if this is an executable command.
    /// </summary>
    public bool CanExecute() => CanExecuteFunc != null ? CanExecuteFunc(this) : true;

    /// <summary>
    /// Executes this command.
    /// </summary>
    public object Execute()
    {
       ExecuteCommand?.Invoke(this, EventArgs.Empty);
       object Result = ExecuteFunc != null && CanExecute() ? ExecuteFunc(this) : null;
       return Result;
    }

    /// <summary>
    /// Executes this command.
    /// </summary>
    public async Task<object> ExecuteAsync()
    {
        ExecuteCommand?.Invoke(this, EventArgs.Empty);
        object Result = ExecuteAsyncFunc != null && CanExecute()? await ExecuteAsyncFunc(this) : null;
        return Result;
    }

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
    /// The form to show when the command is executed.
    /// </summary>
    public string Form
    {
        get => fForm;
        init { if (fForm != value) { fForm = value; NotifyPropertyChanged(nameof(Form)); } }
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
    /// True if this is a toggle command, a command that toggles a boolean value.
    /// </summary>
    public bool IsToggle { get; set; }

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
    public bool HasChildren => fCommands != null && fCommands.Count > 0;
    [JsonIgnore] public override bool IsSerializable => false;
    
    // ● events
    public event EventHandler ExecuteCommand;

}