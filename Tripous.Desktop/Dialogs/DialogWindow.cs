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
    static public async Task<DialogData> ShowModal<T>(Control Caller) where T : DialogWindow, new()
    {
        return await ShowModal<T>(Caller, null);
    }
    static public async Task<DialogData> ShowModal<T>(object InputData) where T : DialogWindow, new()
    {
        if (InputData == null)
            throw new ArgumentNullException(nameof(InputData));
        return await ShowModal<T>(InputData,null);
    }
    static public async Task<DialogData> ShowModal<T>(object InputData = null, Control Caller = null)
        where T : DialogWindow, new()
    {
        DialogData Data = new DialogData();
        await Data.ShowModal<T>(InputData, Caller);
        return Data;
    }
    
    
    // ● properties
    public virtual ModalResult ModalResult
    {
        get => Data != null? Data.ModalResult: ModalResult.None;
        set
        {
            if (Data != null)
            {
                Data.ModalResult = value;
                if (Data.ModalResult == ModalResult.None)
                    return;
                Close();            
            }

        }
    }
    public object InputData => Data != null? Data.InputData: null;
    public object ResultData
    {
        get =>  Data != null? Data.ResultData: null;
        set
        {
            if (Data != null)
                Data.ResultData = value;
        }
    }
    public DialogData Data { get; set; }
}
