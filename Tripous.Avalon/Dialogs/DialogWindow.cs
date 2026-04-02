namespace Tripous.Avalon;

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
    static public async Task<DialogData> ShowModal<T>(Window Parent) where T : DialogWindow, new()
    {
        if (Parent == null)
            throw new ArgumentNullException(nameof(Parent));
        return await ShowModal<T>(Parent, null);
    }
    static public async Task<DialogData> ShowModal<T>(object InputData) where T : DialogWindow, new()
    {
        if (InputData == null)
            throw new ArgumentNullException(nameof(InputData));
        return await ShowModal<T>(null, InputData);
    }
    static public async Task<DialogData> ShowModal<T>(Window Parent = null, object InputData = null)
        where T : DialogWindow, new()
    {
       
        DialogData Data = new DialogData();
        await Data.ShowModal<T>(Parent, InputData);
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
