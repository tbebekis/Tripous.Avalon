using Avalonia.Input.Platform;

namespace PasswordManager;

/// <summary>
/// Registers application commands.
/// </summary>
static public partial class AppHost
{
    // ● private
    /// <summary>
    /// Shows a registered data form in the content area.
    /// </summary>
    static object ShowForm(Command Cmd)
    {
        return ContentHandler.ShowDataForm(Cmd.Form);
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
    /// Exports encrypted credential rows.
    /// </summary>
    static async Task<object> ExportCredentials()
    {
        try
        {
            string FilePath = CredentialTransferService.Export(Store);
            await MessageBox.Info($"Encrypted credentials exported to:{Environment.NewLine}{FilePath}", MainWindow);
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, MainWindow);
        }
        return null;
    }
    /// <summary>
    /// Imports encrypted credential rows.
    /// </summary>
    static async Task<object> ImportCredentials()
    {
        try
        {
            bool Flag = await MessageBox.YesNo("Import replaces all current credentials. Continue?", MainWindow);
            if (!Flag)
                return null;
            int Count = CredentialTransferService.Import(Store);
            await MessageBox.Info($"Encrypted credentials imported: {Count}", MainWindow);
        }
        catch (Exception e)
        {
            await MessageBox.Error(e.Message, MainWindow);
        }
        return null;
    }
    /// <summary>
    /// Generates a password and copies it to the clipboard when possible.
    /// </summary>
    static async Task<object> GeneratePassword()
    {
        string Password = VaultService.GeneratePassword();
        var Clipboard = TopLevel.GetTopLevel(MainWindow)?.Clipboard;
        if (Clipboard != null)
            await Clipboard.SetTextAsync(Password);
        await MessageBox.Info($"Generated password copied to clipboard:{Environment.NewLine}{Password}", MainWindow);
        return null;
    }
    /// <summary>
    /// Locks the vault and returns to the unlock dialog.
    /// </summary>
    static async Task<object> LockVault()
    {
        VaultService.Lock();
        MainWindow.CloseHiddenHostOnClosed = false;
        MainWindow.Close();
        MainWindow = null;
        SideBarHandler = null;
        ContentHandler = null;
        Ui.MainWindow = HiddenMainWindow;

        bool Flag = await UnlockVault();
        if (!Flag)
        {
            HiddenMainWindow.Close();
            return null;
        }

        MainWindow = new MainWindow();
        Ui.MainWindow = MainWindow;
        MainWindow.Show();
        return null;
    }
    /// <summary>
    /// Registers toolbar commands.
    /// </summary>
    static void RegisterCommands()
    {
        Command cmdCredentials = Command.Create("Credentials", "book_open.png", Cmd => ContentHandler.ShowDataForm("Credential"));
        Command cmdCategories = Command.Create("Categories", "folder.png", Cmd => ContentHandler.ShowDataForm("Category"));
        Command cmdGeneratePassword = Command.CreateAsync("Generate Password", "generate_ssl_certificate.png", async Cmd => await GeneratePassword());
        Command cmdExport = Command.CreateAsync("Export", "table_export.png", async Cmd => await ExportCredentials());
        Command cmdImport = Command.CreateAsync("Import", "table_import.png", async Cmd => await ImportCredentials());
        Command cmdAppFolder = Command.Create("ShowAppFolder", "folder.png", Cmd => { ShowAppFolder(); return null; });
        Command cmdApplicationSettings = Command.CreateAsync("Application Settings", "setting_tools.png", async Cmd => { await ConfigDialog.ShowModal(MainWindow); return null; });
        Command cmdConnectionInfo = Command.CreateAsync("ConnectionInfo", "database_edit.png", async Cmd => { await ShowDbConnectionEditDialog(Db.GetDefaultConnectionInfo()); return null; });
        Command cmdToggleLog = Command.Create("ToggleLog", "file_extension_log.png", Cmd => { MainWindow.ToggleLog(); return null; });
        Command cmdClearLog = Command.Create("ClearLog", "draw_eraser.png", Cmd => { LogBox.Clear(); return null; });
        Command cmdToggleLogSqlStatements = Command.Create("Log Sql", "file_extension_log.png", Cmd => { MainWindow.ToggleLogSqlStatements(); return null; });
        cmdToggleLogSqlStatements.IsToggle = true;
        Command cmdLock = Command.CreateAsync("Lock", "lock.png", async Cmd => await LockVault());
        Command cmdExit = Command.Create("Exit", "door_out.png", Cmd => { MainWindow.Close(); return null; });

        Command cmdGeneral = new("General");
        cmdGeneral.Commands.AddRange([cmdAppFolder, cmdApplicationSettings, cmdConnectionInfo, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdLock, cmdExit]);
        Command cmdVault = new("Vault");
        cmdVault.Commands.AddRange([cmdCredentials, cmdCategories, cmdGeneratePassword, cmdExport, cmdImport]);
        Command cmdModules = new("Modules");
        foreach (FormDef FormDef in DesktopRegistry.Forms)
        {
            Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
            cmdModules.Commands.Add(Cmd);
        }

        AppRegistry.MenuCommands.AddRange([cmdGeneral, cmdVault, cmdModules]);
        AppRegistry.ToolBarCommands.AddRange([cmdCredentials, cmdCategories, cmdGeneratePassword, cmdExport, cmdImport, cmdAppFolder, cmdApplicationSettings, cmdConnectionInfo, cmdToggleLog, cmdClearLog, cmdToggleLogSqlStatements, cmdLock, cmdExit]);
    }
}
