using Avalonia.Controls;

namespace TestBindingApp;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;
    
    // ● private
    void WindowInitialize()
    {
        btnDefaultBinding.Click += (sender, args) => Test_DefaultBinding.Execute();
        btnDataRowViewIndexerBinding.Click += (sender, args) => Test_DataRowViewIndexerBinding.Execute();
        btnDataRowViewTemplateColumn.Click += (sender, args) => Test_DataRowViewTemplateColumn.Execute();
        
        btnDataRowViewTemplateColumnRowView.Click += (sender, args) => Test_DataRowViewTemplateColumn.Execute_ChangeByDataRowView();
        btnDataRowViewTemplateColumnRow.Click += (sender, args) => Test_DataRowViewTemplateColumn.Execute_ChangeByDataRow();

        btnDataRowViewEventsRowView.Click += (sender, args) => Test_DataRowViewEvents.Execute_ChangeByDataRowView();
        btnDataRowViewEventsRow.Click += (sender, args) => Test_DataRowViewEvents.Execute_ChangeByDataRow();
           
        btnRowItemBinding.Click += (sender, args) => Test_RowItemBinding.Execute();
        btnRowItemBindingChangeByRowItem.Click += (sender, args) => Test_RowItemBinding.Execute_ChangeByRowItem();
        btnRowItemBindingChangeByRowView.Click += (sender, args) => Test_RowItemBinding.Execute_ChangeByDataRowView();
        btnRowItemBindingChangeByDataRow.Click += (sender, args) => Test_RowItemBinding.Execute_ChangeByDataRow();
        
            
            
        
        
    }
    
 
 
    void Log(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        Dispatcher.UIThread.Post(() =>
        {
            edtLog.Text += Text + Environment.NewLine;
            edtLog.CaretIndex = edtLog.Text?.Length ?? 0;
        });
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
        Tests.Initialize(this, Log);
        IsWindowInitialized = true;
    
        Log("Application Started.");
        
         
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
            
        }, DispatcherPriority.Background);  
    }


    // ● construction
    public MainWindow()
    {
        InitializeComponent();
    }
}