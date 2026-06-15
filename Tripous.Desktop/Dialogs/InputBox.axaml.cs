/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Dialog used to request a text value from the user.
/// </summary>
public partial class InputBox : DialogWindow
{
    // ● private fields
    /// <summary>
    /// The dialog data.
    /// </summary>
    private InputBoxData BoxData;
    
    // ● event handlers
    /// <summary>
    /// Handles OK and Cancel button clicks.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The routed event arguments.</param>
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (sender == btnCancel)
            this.ModalResult = ModalResult.Cancel;
        else if (sender == btnOK)
            await ControlsToItem();
    }
    
    // ● overridables
    /// <summary>
    /// Initializes the window.
    /// </summary>
    protected override async Task WindowInitialize()
    {
        BoxData = InputData as InputBoxData;
        ResultData = BoxData;
        lblMessage.Content = BoxData.Message;
        edtValue.Text = BoxData.Value;
 
        edtValue.Focus();
        
        await Task.CompletedTask;
    }
    /// <summary>
    /// Loads item values into the dialog controls.
    /// </summary>
    protected override async Task ItemToControls()
    {
        await Task.CompletedTask;
    }
    /// <summary>
    /// Saves dialog control values to the item.
    /// </summary>
    protected override async Task ControlsToItem()
    {
        string Value = edtValue.Text.Trim();
        if (!string.IsNullOrWhiteSpace(Value))
        {
            BoxData.Value = Value;
            this.ModalResult = ModalResult.Ok;
        }
        await Task.CompletedTask;
    }
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="InputBox"/> class.
    /// </summary>
    public InputBox()
    {
        InitializeComponent();
    }

    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    /// <param name="Message">The message displayed by the dialog.</param>
    /// <param name="Value">The initial value.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The input box data.</returns>
    static public async Task<InputBoxData> ShowModal(string Message, string Value = "", Control Caller = null)
    {
        InputBoxData BoxData = new() { Message = Message, Value = Value };
        DialogInfo Info = await  ShowModal<InputBox>(BoxData, Caller);
        BoxData.Info = Info;
        return BoxData;
    }
}

/// <summary>
/// Contains input box dialog data.
/// </summary>
public class InputBoxData
{
    // ● properties
    /// <summary>
    /// Gets or sets the message displayed by the dialog.
    /// </summary>
    public string Message { get; set; } = "Please, enter a value";
    /// <summary>
    /// Gets or sets the input value.
    /// </summary>
    public string Value { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets a value indicating whether the input value should be numeric.
    /// </summary>
    public bool IsNumeric { get; set; }
    /// <summary>
    /// Gets the dialog information.
    /// </summary>
    public DialogInfo Info { get; internal set; }
    /// <summary>
    /// Gets a value indicating whether the dialog result is OK.
    /// </summary>
    public bool Result => Info.Result;
}
