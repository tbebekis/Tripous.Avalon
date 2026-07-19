/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Desktop;

/// <summary>
/// Application user data form.
/// </summary>
public class AppUserForm : AppDataForm
{
    // ● protected fields
    /// <summary>
    /// Button used by administrators to set the selected user password.
    /// </summary>
    protected Button btnSetPassword;

    // ● protected methods
    /// <summary>
    /// Returns true when the current row can have its password set by an administrator.
    /// </summary>
    protected virtual bool CanSetPassword()
    {
        return IsEditableForm && FormState == DataFormState.Edit && CurrentRow != null && !HasChanges();
    }
    /// <summary>
    /// Sets the password of the current user row.
    /// </summary>
    protected virtual async Task SetPassword()
    {
        if (!CanSetPassword())
            return;
        string UserName = CurrentRow.AsString("UserName");
        SetPasswordDialog Dialog = await SetPasswordDialog.ShowModal(UserName, this);
        if (!Dialog.Result)
            return;
        try
        {
            ((AppUserDataModule)Module).SetPassword(CurrentRow.AsString("Id"), Dialog.Password);
            UiLog($"{Texts.L("PasswordChangedForUser", "Password changed for user")} {UserName}.");
            Refresh();
            UpdateUi();
            await MessageBox.Info(Texts.L("PasswordChanged", "Password changed."), this);
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, this);
        }
    }
    /// <summary>
    /// Enables or disables form commands.
    /// </summary>
    protected override void EnableCommands()
    {
        base.EnableCommands();
        btnSetPassword.IsVisible = IsEditableForm;
        btnSetPassword.IsEnabled = CanSetPassword();
    }
    /// <summary>
    /// Creates the form toolbar.
    /// </summary>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;
        btnSetPassword = ToolBar.AddButton("change_password.png", Texts.L("SetPassword", "Set Password"), async () => await SetPassword());
        ToolBar.PlaceControlAfter(btnSave, btnSetPassword);
        return true;
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public AppUserForm()
    {
    }
}
