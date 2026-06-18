using MiniCrm.Data;

namespace MiniCrm;

/// <summary>
/// Registers application commands.
/// </summary>
static public partial class AppHost
{
    // ● private
    /// <summary>
    /// Shows a registered data form in the content area.
    /// </summary>
    /// <param name="Cmd">The command.</param>
    /// <returns>The shown form.</returns>
    static object ShowForm(Command Cmd)
    {
        return ContentHandler.ShowDataForm(Cmd.Form);
    }
    /// <summary>
    /// Shows the customer form in a modal dialog.
    /// </summary>
    static async Task<object> ShowCustomerModal()
    {
        await DataFormContext.ShowFormModal("Customer", Caller: MainWindow);
        return null;
    }
    /// <summary>
    /// Opens the application folder.
    /// </summary>
    static void ShowAppFolder()
    {
        Directory.CreateDirectory(SysConfig.AppFolderPath);
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
    }
    /// <summary>
    /// Changes the current user password.
    /// </summary>
    static async Task ChangePassword()
    {
        AppUser User = Sys.Context.CurrentUser;
        if (User == null)
            return;
        ChangePasswordDialog Dialog = await ChangePasswordDialog.ShowModal(User.UserName, MainWindow);
        if (!Dialog.Result)
            return;
        try
        {
            AppUserDataModule Module = DataRegistry.CreateModule("AppUser") as AppUserDataModule;
            Module.ChangePassword(User.UserName, Dialog.CurrentPassword, Dialog.NewPassword);
            await MessageBox.Info("Password changed.", MainWindow);
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, MainWindow);
        }
    }
    /// <summary>
    /// Registers toolbar commands.
    /// </summary>
    static void RegisterCommands()
    {
        Command cmdCustomers = Command.Create("Customers", "book_open.png", Cmd => ContentHandler.ShowDataForm("Customer"));
        Command cmdCustomerModal = Command.CreateAsync("Customer Modal", "application_go.png", async Cmd => await ShowCustomerModal());
        Command cmdAppFolder = Command.Create("ShowAppFolder", "folder.png", Cmd => { ShowAppFolder(); return null; });
        Command cmdApplicationSettings = Command.CreateAsync("Application Settings", "setting_tools.png", async Cmd => { await ConfigDialog.ShowModal(MainWindow); return null; });
        Command cmdChangePassword = Command.CreateAsync("Change Password", "change_password.png", async Cmd => { await ChangePassword(); return null; });
        Command cmdConnectionInfo = Command.CreateAsync("ConnectionInfo", "database_edit.png", async Cmd => { await ShowDbConnectionEditDialog(Db.GetDefaultConnectionInfo()); return null; });
        Command cmdToggleLog = Command.Create("ToggleLog", "file_extension_log.png", Cmd => { MainWindow.ToggleLog(); return null; });
        Command cmdClearLog = Command.Create("ClearLog", "draw_eraser.png", Cmd => { LogBox.Clear(); return null; });
        Command cmdToggleLogSqlStatements = Command.Create("Log Sql", "file_extension_log.png", Cmd => { MainWindow.ToggleLogSqlStatements(); return null; });
        cmdToggleLogSqlStatements.IsToggle = true;
        Command cmdExit = Command.Create("Exit", "door_out.png", Cmd => { MainWindow.Close(); return null; });

        Command cmdGeneral = new("General");
        cmdGeneral.Commands.AddRange([cmdAppFolder, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdExit]);

        Command cmdModules = new("Modules");
        foreach (FormDef FormDef in DesktopRegistry.Forms)
        {
            Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
            cmdModules.Commands.Add(Cmd);
        }

        AppRegistry.MenuCommands.AddRange([cmdGeneral, cmdModules]);
        AppRegistry.ToolBarCommands.AddRange([cmdCustomers, cmdCustomerModal, cmdAppFolder, cmdApplicationSettings, cmdChangePassword, cmdConnectionInfo, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdExit]);
    }
}
