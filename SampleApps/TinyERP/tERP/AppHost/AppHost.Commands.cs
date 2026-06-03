/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class AppHost
{
    static object ShowFormFunc(Command Cmd)
    {            
        //FormDef FormDef = DesktopRegistry.Forms.Get(Cmd.Form);
        return AppHost.ContentHandler.ShowDataForm(Cmd.Form);
    }
 
    static public void RegisterCommands()
    {
        // NOTE: ToolBar commands should define an ImageFileName.
        
        // ● commands  
        Command cmdExit = Command.Create("Exit", "door_out.png", (c) => { AppHost.MainWindow.Close(); return 0; });
        Command cmdAppFolder = Command.Create("ShowAppFolder", "folder.png", (c) => { Sys.OpenFileExplorer(SysConfig.AppFolderPath); return 0; });
        Command cmdConnectionInfo = Command.CreateAsync("ConnectionInfo", "database_edit.png", async (c) => {  await DbConnectionEditDialog.ShowModal(Db.GetDefaultConnectionInfo()); return 0; });
        Command cmdClearLog = Command.Create("ClearLog", "bin.png", (c) => { LogBox.Clear(); return 0; });
        Command cmdLog = Command.Create("Error Log", "error_log.png", (c) => { AppHost.ContentHandler.ShowDataForm("Log"); return 0; });
        Command cmdTest = Command.Create("Test", "lightning.png");
        
        // ● General commands  
        Command cmdGeneral = new ("General");
        cmdGeneral.Commands.AddRange([cmdAppFolder, cmdConnectionInfo, cmdLog, cmdExit]);

        // ● form commands  
        foreach (FormDef FormDef in DesktopRegistry.Forms)
        {
            Command cmdGroup = AppRegistry.FindCommand(FormDef.Group);
            if (cmdGroup == null)
            {
                cmdGroup = new Command(FormDef.Group);
                AppRegistry.MenuCommands.Add(cmdGroup);
            }

            Command Cmd = FormDef.CreateShowCommand(ShowFormFunc);
            cmdGroup.Commands.Add(Cmd);
        }
        AppRegistry.MenuCommands.Sort();
        AppRegistry.MenuCommands.Insert(0, cmdGeneral);
        
        // ● split commands to toolbar and menu commands
        AppRegistry.ToolBarCommands.AddRange([cmdAppFolder, cmdConnectionInfo, cmdClearLog, cmdTest, cmdExit]);
        //AppRegistry.MenuCommands.AddRange(MasterCommandGroups);
    }
}