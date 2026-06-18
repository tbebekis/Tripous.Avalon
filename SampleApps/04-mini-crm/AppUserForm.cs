using MiniCrm.Data;

namespace MiniCrm;

/// <summary>
/// Application user data form.
/// </summary>
public class AppUserForm : DataForm
{
    // ● protected fields
    /// <summary>
    /// Button used by administrators to set the selected user password.
    /// </summary>
    protected Button btnSetPassword;

    // ● protected
    /// <summary>
    /// Returns true when the current row can have its password set by an administrator.
    /// </summary>
    /// <returns>True when the password can be set.</returns>
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
            UiLog($"Password changed for user {UserName}.");
            Refresh();
            UpdateUi();
            await MessageBox.Info("Password changed.", this);
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, this);
        }
    }
    /// <summary>
    /// Enables or disables form commands.
    /// </summary>
    public override void UpdateUi()
    {
        base.UpdateUi();

        if (btnSetPassword != null)
        {
            btnSetPassword.IsVisible = IsEditableForm;
            btnSetPassword.IsEnabled = CanSetPassword();
        }
    }
    /// <summary>
    /// Creates the form toolbar.
    /// </summary>
    /// <returns>True if the toolbar is created.</returns>
    protected override bool CreateToolBar()
    {
        if (!base.CreateToolBar())
            return false;
        btnSetPassword = ToolBar.AddButton("change_password.png", "Set Password", async () => await SetPassword());
        ToolBar.PlaceControlAfter(btnSave, btnSetPassword);
        return true;
    }

    // ● constructors
    /// <summary>
    /// Initializes a new instance of the <see cref="AppUserForm"/> class.
    /// </summary>
    public AppUserForm()
    {
    }
}
