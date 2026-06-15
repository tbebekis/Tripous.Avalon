/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;
 
/// <summary>
/// Provides context and result data for a dialog window.
/// </summary>
public class DialogInfo
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogInfo"/> class.
    /// </summary>
    public DialogInfo()
    {
    }

    // ● public
    /// <summary>
    /// Shows a dialog window modally.
    /// </summary>
    /// <typeparam name="T">The dialog window type.</typeparam>
    /// <param name="InputData">The input data passed to the dialog.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>This dialog information instance.</returns>
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

    // ● properties
    /// <summary>
    /// Gets the dialog window.
    /// </summary>
    public DialogWindow Dialog { get; private set; }
    /// <summary>
    /// Gets the parent window.
    /// </summary>
    public Window Parent { get; private set; }
    /// <summary>
    /// Gets the caller control.
    /// </summary>
    public Control Caller  { get; private set; }
    /// <summary>
    /// Gets the input data passed to the dialog.
    /// </summary>
    public object InputData { get; private set; }
    /// <summary>
    /// Gets the modal result.
    /// </summary>
    public ModalResult ModalResult { get; internal set; }
    /// <summary>
    /// Gets a value indicating whether the dialog result is OK.
    /// </summary>
    public bool Result => ModalResult == ModalResult.Ok;
    /// <summary>
    /// Gets the result data returned by the dialog.
    /// </summary>
    public object ResultData { get; internal set; }
    /// <summary>
    /// Gets or sets user-defined data.
    /// </summary>
    public object Tag { get; set; }

    // ● public fields
    /// <summary>
    /// User-defined dialog parameters.
    /// </summary>
    public Dictionary<string, object> Params = new();
}
