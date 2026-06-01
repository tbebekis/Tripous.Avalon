namespace Tripous.Desktop;

public partial class FirstRunDialog : DialogWindow
{
 
    private FirstRunBoxData BoxData;
    
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
        
        lblMessage.Text = string.Empty;
        
        BoxData = InputData as FirstRunBoxData;
        ResultData = BoxData;
 
        edtFullName.Focus();
        
        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        await Task.CompletedTask;
    }
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
    public FirstRunDialog()
    {
        InitializeComponent();
    }
    
    
    
    static public async Task<FirstRunBoxData> ShowModal(Control Caller = null)
    {
        FirstRunBoxData BoxData = new() ;
        DialogInfo Info = await  ShowModal<FirstRunDialog>(BoxData, Caller);
        BoxData.Info = Info;
        return BoxData;
    }
}

public class FirstRunBoxData
{
    public string FullName { get; set; }  
    public string UserName { get; set; }  
    public string Password { get; set; }  
    public DialogInfo Info { get; internal set; }
    public bool Result => Info.Result;
}