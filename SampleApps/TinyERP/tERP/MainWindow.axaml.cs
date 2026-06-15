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
    ToolBar ToolBar;

    AppFormPagerHandler SideBarHandler; 
    AppFormPagerHandler ContentHandler; 
 
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
            
            Command cmdTest = AppRegistry.ToolBarCommands.Find("Test"); // a command for just calling the Test() method
            cmdTest.ExecuteCommand += (sender, args) => Test();
            
            Ui.Post(async () => await CheckForSampleData());

            UpdateStatusBar();
        });

        Config.SetUserValue(Ui.SShowDataFormLog, true.ToString());
    }
    async Task CheckForSampleData()
    {
        SampleData[] NotAddedSampleData = SampleData.GetNotAdded();
        if (NotAddedSampleData.Length > 0)
        {
            StringBuilder SB = new();
            SB.AppendLine("The following versions of sample data are not added to the database yet.");
            SB.AppendLine();
            foreach (SampleData SD in NotAddedSampleData)
                SB.AppendLine($"{SD.VersionNumber}");
            SB.AppendLine();
            SB.AppendLine("Do you want to add that versions of sample data to the database?");
            
            bool Flag = await MessageBox.YesNo(SB.ToString(), this);
            if (Flag)
            {
                LogBox.AppendLine("Adding sample data. Please wait...");
                BusyDialog BusyDialog = new("Adding sample data. Please wait...");
                Task BusyDialogTask = BusyDialog.ShowDialog(this);
                bool IsDone = false;
                Exception Error = null;
                try
                {
                    await Dispatcher.UIThread.InvokeAsync(() => { }, DispatcherPriority.Background);
                    await SampleData.AddSampleDataAsync(NotAddedSampleData);
                    IsDone = true;
                }
                catch (Exception e)
                {
                    Error = e;
                    LogBox.AppendLine(e.ToString());
                }
                finally
                {
                    BusyDialog.CloseDialog();
                    await BusyDialogTask;
                }

                if (IsDone)
                {
                    string Message = @"DONE.

The application will now terminate.
Please restart the application.
";
                    LogBox.Append(Message);
                    await MessageBox.Info(Message, this);
                    
                    this.Close();
                }
                else if (Error != null)
                {
                    await MessageBox.Error(Error.Message, this);
                }
            }
        }
    }
    internal void ToggleLog()
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
    internal void ToggleLogSqlStatements()
    {
        bool Flag = !Db.Settings.LogSqlStatements;
        string Text = Flag ? "ON" : "OFF";
        string Message = $"SQL Statements Logging is now: {Text}.";
        LogBox.AppendLine(Message);
        Db.Settings.LogSqlStatements = Flag;
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
    void UpdateStatusBar()
    {
        lblStatus.Text = $"tERP v1.0 - {Sys.Context.CultureCode}.";
        lblMessage.Text = "Ready";
        lblUser.Text = $"User: {Sys.Context.CurrentUser.UserName}";
        lblUserRole.Text = $"User Role: {Sys.Context.CurrentUser.UserLevel}";
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
