using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;

namespace Tripous.Avalon;

public enum ModalResult
{
    None,
    Ok,
    Cancel
}

public class DialogWindow: Window
{
    bool IsWindowInitialized = false;
    //protected ModalResult fModalResult;
    //protected object fInputData;

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
    protected virtual void InputDataChanged()
    {
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
    /*
    static public async Task<(bool Result, object ResultObject)> ShowModal(Window Parent, DialogWindow Dialog)
    {
        if (Dialog == null) 
            throw new Exception("DialogWindow Dialog is null");
        if (Parent == null) 
            throw new Exception("Window Parent is null");

        await Dialog.ShowDialog(Parent);
    
        return (Dialog.ModalResult == ModalResult.Ok, Dialog.ResultData);
    }
    static public async Task<(bool Result, object ResultObject)> ShowModal<T>(Window Parent, object InputData = null) where T : DialogWindow, new()
    {
        T Dialog = Activator.CreateInstance<T>();
        
        if (Dialog == null) 
            throw new Exception($"Could not create instance of {typeof(T)}");
        
        Dialog.InputData = InputData;

        return await ShowModal(Parent, Dialog);
    }
    */
    static public async Task<DialogData> ShowModal<T>(Window Parent = null, object InputData = null)
        where T : DialogWindow, new()
    {
        DialogData Data = new DialogData(typeof(T), Parent, InputData);
        await Data.ShowModal();
        return Data;
    }
    
    // ● properties
    public virtual ModalResult ModalResult
    {
        get => Data.ModalResult;
        set
        {
            Data.ModalResult = value;
            if (Data.ModalResult == ModalResult.None)
                return;
            Close();
        }
    }
    public object InputData => Data.InputData;
    public object ResultData
    {
        get => Data.ResultData;
        set => Data.ResultData = value;
    }
    public DialogData Data { get; set; }
}

public class DialogData
{
    public DialogData(Type ClassType, Window Parent, object InputData = null)
    {
        this.Parent = Parent ?? Ui.MainWindow;
        this.InputData = InputData;
        
        Dialog = Activator.CreateInstance(ClassType) as DialogWindow;
        Dialog.Data = this;
    }

    public async Task ShowModal()
    {
        await Dialog.ShowDialog(Parent);
    }

    public DialogWindow Dialog { get; }
    public Window Parent { get; }
    public object InputData { get; }

    public ModalResult ModalResult { get; set; }
    public bool Result => ModalResult == ModalResult.Ok;
    public object ResultData { get; set; }
    public object Tag { get; set; }
}