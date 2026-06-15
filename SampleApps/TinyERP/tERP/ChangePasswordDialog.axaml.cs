/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Dialog used by the current user to change password.
/// </summary>
public partial class ChangePasswordDialog : Window
{
    // ● private fields
    bool fResult;

    // ● private methods
    bool Validate()
    {
        lblMessage.Text = string.Empty;
        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword) || string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            lblMessage.Text = "All password fields are required.";
            return false;
        }
        if (NewPassword != ConfirmPassword)
        {
            lblMessage.Text = "Passwords differ.";
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

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public ChangePasswordDialog()
    {
        InitializeComponent();
        Loaded += (Sender, Args) => edtCurrentPassword.Focus();
        btnOK.Click += (Sender, Args) => OkClick();
        btnCancel.Click += (Sender, Args) => Close();
    }

    // ● static public methods
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    static public async Task<ChangePasswordDialog> ShowModal(string UserName, Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        ChangePasswordDialog Dialog = new();
        Dialog.lblUser.Text = $"User: {UserName}";
        await Dialog.ShowDialog(Caller.GetOwnerWindow());
        return Dialog;
    }

    // ● properties
    /// <summary>
    /// Gets the current password entered by the user.
    /// </summary>
    public string CurrentPassword => edtCurrentPassword.GetText();
    /// <summary>
    /// Gets the new password entered by the user.
    /// </summary>
    public string NewPassword => edtNewPassword.GetText();
    /// <summary>
    /// Gets the confirmation password entered by the user.
    /// </summary>
    public string ConfirmPassword => edtConfirmPassword.GetText();
    /// <summary>
    /// Gets a value indicating whether the user accepted the dialog.
    /// </summary>
    public bool Result => fResult;
}
