namespace MiniCrm;

/// <summary>
/// Dialog used by an administrator to set a user password.
/// </summary>
public partial class SetPasswordDialog : Window
{
    // ● private fields
    bool fResult;

    // ● private
    /// <summary>
    /// Validates dialog input.
    /// </summary>
    /// <returns>True when input is valid.</returns>
    bool Validate()
    {
        lblMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            lblMessage.Text = "Password fields are required.";
            return false;
        }
        if (Password != ConfirmPassword)
        {
            lblMessage.Text = "Passwords differ.";
            return false;
        }
        return true;
    }
    /// <summary>
    /// Accepts the dialog.
    /// </summary>
    void OkClick()
    {
        if (!Validate())
            return;
        fResult = true;
        Close();
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="SetPasswordDialog"/> class.
    /// </summary>
    public SetPasswordDialog()
    {
        InitializeComponent();
        Loaded += (Sender, Args) => edtPassword.Focus();
        btnOK.Click += (Sender, Args) => OkClick();
        btnCancel.Click += (Sender, Args) => Close();
    }

    // ● static public
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    /// <param name="UserName">The user name.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The dialog.</returns>
    static public async Task<SetPasswordDialog> ShowModal(string UserName, Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        SetPasswordDialog Dialog = new();
        Dialog.lblUser.Text = $"User: {UserName}";
        await Dialog.ShowDialog(Caller.GetOwnerWindow());
        return Dialog;
    }

    // ● properties
    /// <summary>
    /// Gets the password entered by the administrator.
    /// </summary>
    public string Password => edtPassword.GetText();
    /// <summary>
    /// Gets the confirmation password entered by the administrator.
    /// </summary>
    public string ConfirmPassword => edtConfirmPassword.GetText();
    /// <summary>
    /// Gets a value indicating whether the administrator accepted the dialog.
    /// </summary>
    public bool Result => fResult;
}
