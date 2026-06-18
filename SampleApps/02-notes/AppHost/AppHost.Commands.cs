namespace Notes;

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
    /// Shows the Notes form in a modal dialog.
    /// </summary>
    static async Task<object> ShowNotesModal()
    {
        await DataFormContext.ShowFormModal("Note", Caller: MainWindow);
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
    /// Registers toolbar commands.
    /// </summary>
    static void RegisterCommands()
    {
        Command cmdNotes = Command.Create("Notes", "book_open.png", Cmd => ContentHandler.ShowDataForm("Note"));
        Command cmdNotesModal = Command.CreateAsync("Notes Modal", "application_go.png", async Cmd => await ShowNotesModal());
        Command cmdAppFolder = Command.Create("ShowAppFolder", "folder.png", Cmd => { ShowAppFolder(); return null; });
        Command cmdToggleLog = Command.Create("ToggleLog", "file_extension_log.png", Cmd => { MainWindow.ToggleLog(); return null; });
        Command cmdClearLog = Command.Create("ClearLog", "draw_eraser.png", Cmd => { LogBox.Clear(); return null; });
        Command cmdExit = Command.Create("Exit", "door_out.png", Cmd => { MainWindow.Close(); return null; });

        Command cmdGeneral = new("General");
        cmdGeneral.Commands.AddRange([cmdAppFolder, cmdToggleLog, cmdClearLog, cmdExit]);

        Command cmdModules = new("Modules");
        foreach (FormDef FormDef in DesktopRegistry.Forms)
        {
            Command Cmd = FormDef.CreateShowCommand(ShowForm, ImageFileName: "book_open.png");
            cmdModules.Commands.Add(Cmd);
        }

        AppRegistry.MenuCommands.AddRange([cmdGeneral, cmdModules]);
        AppRegistry.ToolBarCommands.AddRange([cmdNotes, cmdNotesModal, cmdAppFolder, cmdToggleLog, cmdClearLog, cmdExit]);
    }
}
