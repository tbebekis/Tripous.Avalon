namespace PasswordManager;

/// <summary>
/// Dialog used to unlock the vault.
/// </summary>
public partial class UnlockVaultDialog : Window
{
    // ● private
    /// <summary>
    /// Handles the OK button click.
    /// </summary>
    private void OK()
    {
        if (!VaultService.Unlock(edtPassword.Text))
        {
            lblMessage.Text = "Invalid master password.";
            return;
        }

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
    /// Initializes a new instance of the <see cref="UnlockVaultDialog"/> class.
    /// </summary>
    public UnlockVaultDialog()
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
        UnlockVaultDialog Dialog = new();
        return await Dialog.ShowDialog<bool>(Owner);
    }
}
