namespace Tripous.Desktop;

public class DialogWindow: Window
{
    bool IsWindowInitialized = false;

    // ● overridables
    protected virtual async Task WindowInitialize()
    {
        await Task.CompletedTask;
    }
    protected virtual async Task ItemToControls()
    {
        await Task.CompletedTask;
    }
    protected virtual async Task ControlsToItem()
    {
        await Task.CompletedTask;
    }
 
    // ● construction
    public DialogWindow()
    {        
        this.Loaded += async (s, e) =>
        {
            if (IsWindowInitialized)
                return;
            await WindowInitialize();
            await ItemToControls();
            IsWindowInitialized = true;
        };
    }
 
    // ● static
    static public async Task<DialogInfo> ShowModal<T>(Control Caller) where T : DialogWindow, new()
    {
        return await ShowModal<T>(Caller, null);
    }
    static public async Task<DialogInfo> ShowModal<T>(object InputData) where T : DialogWindow, new()
    {
        if (InputData == null)
            throw new TripousArgumentNullException(nameof(InputData));
        return await ShowModal<T>(InputData,null);
    }
    static public async Task<DialogInfo> ShowModal<T>(object InputData = null, Control Caller = null)
        where T : DialogWindow, new()
    {
        DialogInfo Info = new DialogInfo();
        await Info.ShowModal<T>(InputData, Caller);
        return Info;
    }
    
    
    // ● properties
    public virtual ModalResult ModalResult
    {
        get => Info != null? Info.ModalResult: ModalResult.None;
        set
        {
            if (Info != null)
            {
                Info.ModalResult = value;
                if (Info.ModalResult == ModalResult.None)
                    return;
                Close();            
            }

        }
    }
    public object InputData => Info != null? Info.InputData: null;
    public object ResultData
    {
        get =>  Info != null? Info.ResultData: null;
        set
        {
            if (Info != null)
                Info.ResultData = value;
        }
    }
    public DialogInfo Info { get; set; }
}