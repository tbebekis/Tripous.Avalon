/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;
 
public class DialogInfo
{
    public DialogInfo()
    {
    }

    public async Task<DialogInfo> ShowModal<T>(object InputData = null, Control Caller = null) where T : DialogWindow, new()
    {
        if (Caller == null)
            Caller = Ui.MainWindow;

        this.Caller = Caller;
        this.Parent = Caller.GetOwnerWindow();
        this.InputData = InputData?? this;
    
        Dialog = Activator.CreateInstance<T>() as DialogWindow;
        Dialog.Info = this;
        
        await Dialog.ShowDialog(this.Parent);
        return this;
    }

    public DialogWindow Dialog { get; private set; }
    public Window Parent { get; private set; }
    public Control Caller  { get; private set; }
    public object InputData { get; private set; }

    public ModalResult ModalResult { get; internal set; }
    public bool Result => ModalResult == ModalResult.Ok;
    public object ResultData { get; internal set; }
    public object Tag { get; set; }
    public Dictionary<string, object> Params = new();
}
