/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Base class for modal dialog windows.
/// </summary>
public class DialogWindow: Window
{
    // ● private fields
    /// <summary>
    /// True when the window has been initialized.
    /// </summary>
    bool IsWindowInitialized = false;

    // ● overridables
    /// <summary>
    /// Initializes the window.
    /// </summary>
    protected virtual async Task WindowInitialize()
    {
        await Task.CompletedTask;
    }
    /// <summary>
    /// Loads item values into the dialog controls.
    /// </summary>
    protected virtual async Task ItemToControls()
    {
        await Task.CompletedTask;
    }
    /// <summary>
    /// Saves dialog control values to the item.
    /// </summary>
    protected virtual async Task ControlsToItem()
    {
        await Task.CompletedTask;
    }
 
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="DialogWindow"/> class.
    /// </summary>
    public DialogWindow()
    {
        CanMinimize = false;

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
    /// <summary>
    /// Shows a dialog window modally.
    /// </summary>
    /// <typeparam name="T">The dialog window type.</typeparam>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The dialog information.</returns>
    static public async Task<DialogInfo> ShowModal<T>(Control Caller) where T : DialogWindow, new()
    {
        return await ShowModal<T>(Caller, null);
    }
    /// <summary>
    /// Shows a dialog window modally.
    /// </summary>
    /// <typeparam name="T">The dialog window type.</typeparam>
    /// <param name="InputData">The input data passed to the dialog.</param>
    /// <returns>The dialog information.</returns>
    static public async Task<DialogInfo> ShowModal<T>(object InputData) where T : DialogWindow, new()
    {
        if (InputData == null)
            throw new TripousArgumentNullException(nameof(InputData));
        return await ShowModal<T>(InputData,null);
    }
    /// <summary>
    /// Shows a dialog window modally.
    /// </summary>
    /// <typeparam name="T">The dialog window type.</typeparam>
    /// <param name="InputData">The input data passed to the dialog.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The dialog information.</returns>
    static public async Task<DialogInfo> ShowModal<T>(object InputData = null, Control Caller = null)
        where T : DialogWindow, new()
    {
        DialogInfo Info = new DialogInfo();
        await Info.ShowModal<T>(InputData, Caller);
        return Info;
    }
    
    
    // ● properties
    /// <summary>
    /// Gets or sets the modal result.
    /// </summary>
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
    /// <summary>
    /// Gets the input data passed to the dialog.
    /// </summary>
    public object InputData => Info != null? Info.InputData: null;
    /// <summary>
    /// Gets or sets the result data returned by the dialog.
    /// </summary>
    public object ResultData
    {
        get =>  Info != null? Info.ResultData: null;
        set
        {
            if (Info != null)
                Info.ResultData = value;
        }
    }
    /// <summary>
    /// Gets or sets the dialog information.
    /// </summary>
    public DialogInfo Info { get; set; }
}
