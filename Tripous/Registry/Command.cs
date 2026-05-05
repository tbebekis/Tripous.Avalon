namespace Tripous;

public class Command: BaseDef
{
    private string fImageFileName;
    private DefList<Command> fCommands = new();
    
    // ● construction
    public Command()
    {
    }
    public Command(string Name, string TitleKey, string ImageFileName)
    {
        this.Name = Name;
        this.TitleKey = TitleKey;
        this.ImageFileName = ImageFileName;
    }
    
    public Command(string Name, Func<Command, object> ExecuteFunc)
        : this(Name, "", "")
    {
        this.ExecuteFunc = ExecuteFunc;
    }
    public Command(string Name, string ImageFileName, Func<Command, object> ExecuteFunc)
        : this(Name, "", ImageFileName)
    {
        this.ExecuteFunc = ExecuteFunc;
    }
    public  Command(string Name, string TitleKey, string ImageFileName, Func<Command, object> ExecuteFunc)
        : this(Name, TitleKey, ImageFileName)
    {
        this.ExecuteFunc = ExecuteFunc;
    }
    
    public Command(string Name, Func<Command, Task<object>> ExecuteAsyncFunc)
        : this(Name, "", "")
    {
        this.ExecuteAsyncFunc = ExecuteAsyncFunc;
    }
    public Command(string Name, string ImageFileName, Func<Command, Task<object>> ExecuteAsyncFunc)
        : this(Name, "", ImageFileName)
    {
        this.ExecuteAsyncFunc = ExecuteAsyncFunc;
    }
    public Command(string Name, string TitleKey, string ImageFileName, Func<Command, Task<object>> ExecuteAsyncFunc)
        : this(Name, TitleKey, ImageFileName)
    {
        this.ExecuteAsyncFunc = ExecuteAsyncFunc;
    }
    
    // ● public
    public bool CanExecute() => CanExecuteFunc != null ? CanExecuteFunc(this) : true;
    public object Execute() => ExecuteFunc != null ? ExecuteFunc(this) : null;
    public async Task<object> ExecuteAsync() => ExecuteAsyncFunc != null? await ExecuteAsyncFunc(this) : null;

    // ● properties
    public string ImageFileName
    {
        get => fImageFileName;
        init { if (fImageFileName != value) { fImageFileName = value; NotifyPropertyChanged(nameof(ImageFileName)); } }
    }
    public DefList<Command> Commands
    {
        get => fCommands;
        init { if (fCommands != value) { fCommands = value; NotifyPropertyChanged(nameof(Commands)); } }
    }

    public bool IsSync => !IsAsync;
    public bool IsAsync => ExecuteAsyncFunc != null;

    public Func<Command, bool> CanExecuteFunc { get; set; }
    public Func<Command, object> ExecuteFunc { get; set; }
    public Func<Command, Task<object>> ExecuteAsyncFunc { get; set; }

    public bool HasChildren => Commands != null && Commands.Count > 0;
}