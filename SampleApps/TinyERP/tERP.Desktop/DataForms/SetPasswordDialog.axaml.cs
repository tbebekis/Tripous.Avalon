/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Dialog used by an administrator to set a user password.
/// </summary>
public partial class SetPasswordDialog : Window
{
    // ● private fields
    bool fResult;

    // ● private methods
    bool Validate()
    {
        lblMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            lblMessage.Text = Texts.L("PasswordFieldsAreRequired", "Password fields are required.");
            return false;
        }
        if (Password != ConfirmPassword)
        {
            lblMessage.Text = Texts.L("PasswordsDiffer", "Passwords differ.");
            return false;
        }
        return true;
    }
    void OkClick()
    {
        if (!Validate())
            return;
        fResult = true;
        Close();
    }
    void ApplyTexts()
    {
        Title = Texts.L("SetPassword", "Set Password");
        lblTitle.Text = Texts.L("SetPassword", "Set Password");
        lblPassword.Text = Texts.L("Password", "Password");
        lblConfirmPassword.Text = Texts.L("ConfirmPassword", "Confirm Password");
        btnOK.Content = Texts.L("OK", "OK");
        btnCancel.Content = Texts.L("Cancel", "Cancel");
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public SetPasswordDialog()
    {
        InitializeComponent();
        ApplyTexts();
        Loaded += (Sender, Args) => edtPassword.Focus();
        btnOK.Click += (Sender, Args) => OkClick();
        btnCancel.Click += (Sender, Args) => Close();
    }

    // ● static public methods
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    static public async Task<SetPasswordDialog> ShowModal(string UserName, Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        SetPasswordDialog Dialog = new();
        Dialog.lblUser.Text = $"{Texts.L("User", "User")}: {UserName}";
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
