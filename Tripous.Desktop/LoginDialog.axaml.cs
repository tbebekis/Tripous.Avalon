/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Dialog used to collect login credentials.
/// </summary>
public partial class LoginDialog : DialogWindow
{
    // ● private fields
    /// <summary>
    /// The dialog data.
    /// </summary>
    private LoginBoxData BoxData;
    
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
        btnOK.Click += AnyClick;
        btnCancel.Click += AnyClick;
        
        btnOK.IsDefault = true;
        btnCancel.IsCancel = true;
 
        BoxData = InputData as LoginBoxData;
        ResultData = BoxData;
 
        edtUserName.Focus();
        edtUserName.SelectAll();
        
        await Task.CompletedTask;
    }
    /// <summary>
    /// Loads login data into the dialog controls.
    /// </summary>
    protected override async Task ItemToControls()
    {
        cboLanguage.ItemsSource = BoxData.SupportedCultures;
        cboLanguage.SelectedIndex = 0;
        lblMessage.Text = BoxData.Message;
        edtUserName.Text = !string.IsNullOrWhiteSpace(BoxData.UserName) ? BoxData.UserName : string.Empty;
        
        await Task.CompletedTask;
    }
    /// <summary>
    /// Saves dialog control values to the login data.
    /// </summary>
    protected override async Task ControlsToItem()
    {
        await Task.CompletedTask;
        
        if ( string.IsNullOrWhiteSpace(edtUserName.Text) 
            || string.IsNullOrWhiteSpace(edtPassword.Text) )
        {
            lblMessage.Text = "Incomplete input";
            return;
        }
 
        BoxData.UserName = edtUserName.GetText();
        BoxData.Password = edtPassword.GetText();
        BoxData.CultureCode = cboLanguage.SelectedItem.ToString();
 
        this.ModalResult = ModalResult.Ok;
    }
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="LoginDialog"/> class.
    /// </summary>
    public LoginDialog()
    {
        InitializeComponent();
    }
    
    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    /// <param name="BoxData">The login dialog data.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The login dialog data.</returns>
    static public async Task<LoginBoxData> ShowModal(LoginBoxData BoxData, Control Caller = null)
    {
        DialogInfo Info = await ShowModal<LoginDialog>(BoxData, Caller);
        BoxData.Info = Info;
        return BoxData;
    }
}

/// <summary>
/// Contains login dialog data.
/// </summary>
public class LoginBoxData
{
    // ● properties
    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the selected culture code.
    /// </summary>
    public string CultureCode { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the supported culture codes.
    /// </summary>
    public string[] SupportedCultures { get; set; } = ["en-US"];
    /// <summary>
    /// Gets or sets the dialog message.
    /// </summary>
    public string Message { get; set; } = string.Empty;
    /// <summary>
    /// Gets the dialog information.
    /// </summary>
    public DialogInfo Info { get; internal set; }
    /// <summary>
    /// Gets a value indicating whether the dialog result is OK.
    /// </summary>
    public bool Result => Info.Result;
}
