namespace PasswordManager;

/// <summary>
/// Dialog used to create the initial master password.
/// </summary>
public partial class CreateMasterPasswordDialog : Window
{
    // ● private
    /// <summary>
    /// Handles the OK button click.
    /// </summary>
    private void OK()
    {
        if (edtPassword.Text != edtConfirm.Text)
        {
            lblMessage.Text = "The two passwords do not match.";
            return;
        }
        if (!VaultService.ValidateMasterPassword(edtPassword.Text, out string Message))
        {
            lblMessage.Text = Message;
            return;
        }

        VaultService.CreateMasterPassword(edtPassword.Text);
        Close(true);
    }

    // ● protected
    /// <summary>
    /// Handles the opened event.
    /// </summary>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        edtPassword.Focus();
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateMasterPasswordDialog"/> class.
    /// </summary>
    public CreateMasterPasswordDialog()
    {
        InitializeComponent();
        btnOK.Click += (Sender, Args) => OK();
        btnCancel.Click += (Sender, Args) => Close(false);
    }

    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    static public async Task<bool> ShowModal(Window Owner)
    {
        CreateMasterPasswordDialog Dialog = new();
        return await Dialog.ShowDialog<bool>(Owner);
    }
}
