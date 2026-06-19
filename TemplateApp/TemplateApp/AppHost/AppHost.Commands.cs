namespace TemplateApp;

/// <summary>
/// Registers application commands.
/// </summary>
static public partial class AppHost
{
    // ● private methods
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
    /// Opens the application folder.
    /// </summary>
    static void ShowAppFolder()
    {
        Directory.CreateDirectory(SysConfig.AppFolderPath);
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
    }
    /// <summary>
    /// Registers application commands.
    /// </summary>
    static void RegisterCommands()
    {
        Command CmdAppFolder = Command.Create("ShowAppFolder", "folder.png", Cmd => { ShowAppFolder(); return null; });
        Command CmdApplicationSettings = Command.CreateAsync("Application Settings", "setting_tools.png", async Cmd => { await ConfigDialog.ShowModal(MainWindow); return null; });
        Command CmdConnectionInfo = Command.CreateAsync("ConnectionInfo", "database_edit.png", async Cmd => { await ShowDbConnectionEditDialog(Db.GetDefaultConnectionInfo()); return null; });
        Command CmdToggleLog = Command.Create("ToggleLog", "file_extension_log.png", Cmd => { MainWindow.ToggleLog(); return null; });
        Command CmdClearLog = Command.Create("ClearLog", "draw_eraser.png", Cmd => { LogBox.Clear(); return null; });
        Command CmdToggleLogSqlStatements = Command.Create("Log Sql", "file_extension_log.png", Cmd => { MainWindow.ToggleLogSqlStatements(); return null; });
        CmdToggleLogSqlStatements.IsToggle = true;
        Command CmdExit = Command.Create("Exit", "door_out.png", Cmd => { MainWindow.Close(); return null; });

        Command CmdGeneral = new("General");
        CmdGeneral.Commands.AddRange([CmdAppFolder, CmdApplicationSettings, CmdConnectionInfo, CmdToggleLog, CmdClearLog, CmdToggleLogSqlStatements, CmdExit]);

        Command CmdModules = new("Modules");
        foreach (FormDef FormDef in DesktopRegistry.Forms)
        {
            Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
            CmdModules.Commands.Add(Cmd);
        }

        AppRegistry.MenuCommands.AddRange([CmdGeneral, CmdModules]);
        AppRegistry.ToolBarCommands.AddRange([CmdAppFolder, CmdApplicationSettings, CmdConnectionInfo, CmdToggleLog, CmdClearLog, CmdToggleLogSqlStatements, CmdExit]);
    }
}
