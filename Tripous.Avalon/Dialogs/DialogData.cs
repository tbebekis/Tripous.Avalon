namespace Tripous.Avalon;
 
public class DialogData
{
    public DialogData()
    {
    }

    public async Task<DialogData> ShowModal<T>(Window Parent, object InputData = null) where T : DialogWindow, new()
    {
        this.Parent = Parent ?? Ui.MainWindow;
        this.InputData = InputData?? this;
    
        Dialog = Activator.CreateInstance<T>() as DialogWindow;
        Dialog.Data = this;
        
        await Dialog.ShowDialog(this.Parent);
        return this;
    }

    public DialogWindow Dialog { get; private set; }
    public Window Parent { get; private set; }
    public object InputData { get; private set; }

    public ModalResult ModalResult { get; internal set; }
    public bool Result => ModalResult == ModalResult.Ok;
    public object ResultData { get; internal set; }
    public object Tag { get; set; }
    public Dictionary<string, object> Params = new();
}