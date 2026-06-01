using DocumentFormat.OpenXml.Math;

namespace Tripous.Desktop;

public partial class LoginDialog : DialogWindow
{
    
    private LoginBoxData BoxData;
    
    // ● event handlers
    async void AnyClick(object sender, RoutedEventArgs e)
    {
        if (sender == btnCancel)
            this.ModalResult = ModalResult.Cancel;
        else if (sender == btnOK)
            await ControlsToItem();
    }
    
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
    protected override async Task ItemToControls()
    {
        cboLanguage.ItemsSource = BoxData.SupportedCultures;
        cboLanguage.SelectedIndex = 0;
        lblMessage.Text = BoxData.Message;
        edtUserName.Text = !string.IsNullOrWhiteSpace(BoxData.UserName) ? BoxData.UserName : string.Empty;
        
        await Task.CompletedTask;
    }
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
    public LoginDialog()
    {
        InitializeComponent();
    }
    
        
    static public async Task<LoginBoxData> ShowModal(LoginBoxData BoxData, Control Caller = null)
    {
        DialogInfo Info = await ShowModal<LoginDialog>(BoxData, Caller);
        BoxData.Info = Info;
        return BoxData;
    }
}

public class LoginBoxData
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string CultureCode { get; set; } = string.Empty;
    public string[] SupportedCultures { get; set; } = ["en-US"];
    public string Message { get; set; } = string.Empty;
    public DialogInfo Info { get; internal set; }
    public bool Result => Info.Result;
}