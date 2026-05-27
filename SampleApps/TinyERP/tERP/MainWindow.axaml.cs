/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    private ToolBar ToolBar;

    private AppFormPagerHandler SideBarHandler; // pagerSideBar
    private AppFormPagerHandler ContentHandler; // pagerContent
 
    // ● private
    void WindowInitialize()
    {
        LogBox.Initialize(edtLog);
        
        SideBarHandler = new AppFormPagerHandler(pagerSideBar);
        ContentHandler = new AppFormPagerHandler(pagerContent);

        Ui.Post(() =>
        {
            CreateMenu();
            CreateToolBar();
            
            AppHost.InitializeUi(SideBarHandler, ContentHandler);  
            //Sys.LogInfo("Hi there");

            // a command for just calling the Test() method
            Command cmdTest = AppRegistry.ToolBarCommands.Find("Test");
            cmdTest.ExecuteCommand += (sender, args) => Test();
            
            Ui.Post(async () => await CheckForSampleData());
        });

    }

    async Task CheckForSampleData()
    {
        if (!Db.MainIni.ReadBool("AreSampleDataAdded", false))
        {
            bool Flag = await MessageBox.YesNo("Do you want to add sample data?", this);
            if (Flag)
            {
                LogBox.AppendLine("Adding sample data. Please wait...");
                try
                {
                    await AppHost.AddSampleDataAsync();
                    Db.MainIni.WriteBool("AreSampleDataAdded", true);
                    LogBox.Append("DONE.");
                    await MessageBox.Info("DONE", this);
                }
                catch (Exception e)
                {
                    LogBox.AppendLine(e.ToString());
                    await MessageBox.Error(e.Message, this);
                }
            }
        }
         
    }
    
    void ToggleLog()
    {
        if (edtLog.IsVisible)
        {
            edtLog.IsVisible = false;
            Splitter2.IsVisible = false;
        }
        else
        {
            Splitter2.IsVisible = true;
            edtLog.IsVisible = true;
        }
    }
    void ShowApplicationFolder()
    {
        Sys.OpenFileExplorer(SysConfig.AppFolderPath);
    }

    void CreateMenu()
    {
    }
    void CreateToolBar()
    {
        ToolBar = new();
        ToolBar.Panel = pnlToolBar;
        ToolBar.AddRange(AppRegistry.ToolBarCommands);
    }
 
    void Log(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        LogBox.AppendLine(Text);
    }


    void Test()
    {
 
    }
    
    // ● overrides
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (IsWindowInitialized)
            return;
 
        WindowInitialize();
        IsWindowInitialized = true;
    
        LogBox.AppendLine("Application Started.");
    }
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        // TODO:
    }
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        
        Dispatcher.UIThread.Post(() => 
        {  
           AppHost.HiddenMainWindow.Close();  
        }, DispatcherPriority.Background);  
    }


    // ● construction
    public MainWindow()
    {
        InitializeComponent();
    }
}
