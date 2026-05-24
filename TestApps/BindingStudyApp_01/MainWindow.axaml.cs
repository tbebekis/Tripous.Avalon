namespace BindingStudyApp_01;

public partial class MainWindow : Window
{
    bool IsWindowInitialized = false;

    DataSource dsCustomer;
    
    // ● private
    void WindowInitialize()
    {
        btnFirst.Click += (sender, args) => MoveFirst(); 
        btnNext.Click += (sender, args) => MoveNext(); 
        btnPrior.Click += (sender, args) => MovePrior(); 
        btnLast.Click += (sender, args) => MoveLast(); 
        btnAdd.Click += (sender, args) => AddCustomer();
        btnDelete.Click += (sender, args) => DeleteCustomer();
        btnAddOrder.Click += (sender, args) => AddOrder();
        btnDeleteOrder.Click += (sender, args) => DeleteOrder();
        
        btnChangeSourceRow.Click += (sender, args) => DataSourceTest.Execute_ChangeByDataSourceRow();
        btnChangeRowView.Click += (sender, args) => DataSourceTest.Execute_ChangeByDataRowView();
        btnChangeRow.Click += (sender, args) => DataSourceTest.Execute_ChangeByDataRow();
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

    void MoveFirst()
    {
        dsCustomer.MoveFirst();
        EnableCommands();
    }
    void MoveNext()
    {
        dsCustomer.MoveNext();
        EnableCommands();
    }
    void MovePrior()
    {
        dsCustomer.MovePrevious();
        EnableCommands();
    }
    void MoveLast()
    {
        dsCustomer.MoveLast();
        EnableCommands();
    }
    void AddCustomer()
    {
        DataSourceTest.Execute_AddCustomer();
        EnableCommands();
    }
    void DeleteCustomer()
    {
        DataSourceTest.Execute_DeleteCustomer();
        EnableCommands();
    }
    void AddOrder()
    {
        DataSourceTest.Execute_AddOrder();
    }
    void DeleteOrder()
    {
        DataSourceTest.Execute_DeleteOrder();
    }

    void EnableCommands()
    {
        btnFirst.IsEnabled = dsCustomer.HasRows && !dsCustomer.IsBof;
        btnPrior.IsEnabled = dsCustomer.HasRows && !dsCustomer.IsBof;
        btnNext.IsEnabled = dsCustomer.HasRows && !dsCustomer.IsEof;
        btnLast.IsEnabled = dsCustomer.HasRows && !dsCustomer.IsEof;
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
        
        DataSourceTest.Execute();
        dsCustomer = DataSourceTest.dsCustomer;
        EnableCommands();
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
