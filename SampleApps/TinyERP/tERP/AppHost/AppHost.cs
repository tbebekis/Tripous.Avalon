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
    static AppHost()
    {
#if DEBUG
        Sys.DebugMode = true;
#endif
        AppHost.StartupMainWindow = new();
        Sys.UiLogProc = Log;
    }
    // ● miscs
    static public void Log(string Text)
    {
        if (LogBox.IsInitialized)
            LogBox.AppendLine(Text);
    }
    
    // ● properties
    static public StartupMainWindow StartupMainWindow { get; private set; }
    static public MainWindow MainWindow { get; private set; }
    static public IClassicDesktopStyleApplicationLifetime AvaloniaDesktop { get; private set; }
    static public AppFormPagerHandler SideBarHandler { get; private set; } // pagerSideBar
    static public AppFormPagerHandler ContentHandler { get; private set; } // pagerContent

    static public SqlStore Store { get; private set; }
}