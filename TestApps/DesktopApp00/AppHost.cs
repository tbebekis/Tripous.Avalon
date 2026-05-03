namespace DesktopApp;

static public partial class AppHost
{
    static AppHost()
    {
#if DEBUG
        Sys.DebugMode = true;
#endif
        AppHost.HiddenMainWindow = new();
    }
    // ● miscs
    static public void Log(string Text)
    {
        if (LogBox.IsInitialized)
            LogBox.AppendLine(Text);
    }
    
    // ● properties
    static public HiddenMainWindow HiddenMainWindow { get; private set; }
    static public MainWindow MainWindow { get; private set; }
    static public IClassicDesktopStyleApplicationLifetime AvaloniaDesktop { get; private set; }
    static public AppFormPagerHandler SideBarHandler { get; private set; } // pagerSideBar
    static public AppFormPagerHandler ContentHandler { get; private set; } // pagerContent

    static public SqlStore Store { get; private set; }
}