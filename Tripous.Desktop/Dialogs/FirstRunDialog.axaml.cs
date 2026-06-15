namespace Tripous.Desktop;

/// <summary>
/// Dialog used to collect first run user information.
/// </summary>
public partial class FirstRunDialog : DialogWindow
{
    // ● private fields
    /// <summary>
    /// The dialog data.
    /// </summary>
    private FirstRunBoxData BoxData;
    
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
        
        lblMessage.Text = string.Empty;
        
        BoxData = InputData as FirstRunBoxData;
        ResultData = BoxData;
 
        edtFullName.Focus();
        
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
        await Task.CompletedTask;
        
        if (string.IsNullOrWhiteSpace(edtFullName.Text) 
            || string.IsNullOrWhiteSpace(edtUserName.Text) 
            || string.IsNullOrWhiteSpace(edtPassword.Text) 
            || string.IsNullOrWhiteSpace(edtConfirmPassword.Text))
        {
            lblMessage.Text = "Incomplete input";
            return;
        }

        string Password = edtPassword.GetText();
        string ConfirmPassword = edtConfirmPassword.GetText();
        if (Password != ConfirmPassword)
        {
            lblMessage.Text = "Passwords differ";
            return;
        }

        BoxData.FullName = edtFullName.GetText();
        BoxData.UserName = edtUserName.GetText();
        BoxData.Password = Password;
 
        this.ModalResult = ModalResult.Ok;
    }
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="FirstRunDialog"/> class.
    /// </summary>
    public FirstRunDialog()
    {
        InitializeComponent();
    }
    
    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The first run dialog data.</returns>
    static public async Task<FirstRunBoxData> ShowModal(Control Caller = null)
    {
        FirstRunBoxData BoxData = new() ;
        DialogInfo Info = await  ShowModal<FirstRunDialog>(BoxData, Caller);
        BoxData.Info = Info;
        return BoxData;
    }
}

/// <summary>
/// Contains first run dialog data.
/// </summary>
public class FirstRunBoxData
{
    // ● properties
    /// <summary>
    /// Gets or sets the full name.
    /// </summary>
    public string FullName { get; set; }  
    /// <summary>
    /// Gets or sets the user name.
    /// </summary>
    public string UserName { get; set; }  
    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; }  
    /// <summary>
    /// Gets the dialog information.
    /// </summary>
    public DialogInfo Info { get; internal set; }
    /// <summary>
    /// Gets a value indicating whether the dialog result is OK.
    /// </summary>
    public bool Result => Info.Result;
}
