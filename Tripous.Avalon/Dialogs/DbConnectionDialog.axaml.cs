using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Tripous.Avalon;

using Tripous;
using Tripous.Data;

namespace DbPark;

public partial class DbConnectionDialog : DialogWindow
{
    DbConnectionInfo ConInfo;
    private bool IsInsert = false;
    
    // ● event handlers
    async void AnyClick(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender == btnCancel)
            this.ModalResult = ModalResult.Cancel;
        else if (sender == btnOK)
            await ControlsToItem();
        else if (sender == btnCheckIsOk)
            await CheckIsOk(true);
    }

    async Task<bool> CheckIsOk(bool ShowMessage)
    {
        string Message = string.Empty;
        bool Flag = false;
        string ConName = edtName.GetText();
        string CS = edtConnectionString.GetText();

        // check if all information is given
        if (string.IsNullOrWhiteSpace(ConName) || string.IsNullOrWhiteSpace(CS))
        {
            Message = $"Connection requires a Name and a ConnectionString to be specified";
            LogBox.AppendLine(Message);
            await  MessageBox.Error(Message, this);
            return false;
        }
        
        // check if connection name can be used as a folder name
        Flag = Sys.IsValidFileName(ConName);
        if (!Flag)
        {
            Message = $"Connection Name '{ConName}' is not a valid folder name.";
            LogBox.AppendLine(Message);
            await  MessageBox.Error(Message, this);
            return false;
        }
        
        // check if the connection name already is used
        if (IsInsert && Db.Connections.Contains(ConName))
        {
            Message = $"Connection {ConName} already exists";
            LogBox.AppendLine(Message);
            await  MessageBox.Error(Message, this);
            return false;
        }
        
        // check if can connect
        DbServerType dbServerType = (DbServerType)cboDbType.SelectedItem;
        Flag = Db.CanConnect(dbServerType, CS);
        
        Message = Flag? "Connection Successful" : "Connection Failed";
        LogBox.AppendLine(Message);
        
        if (ShowMessage)
            await MessageBox.Info(Message, this);

        return Flag;
    }
 
    void SetTemplateConnectionString()
    {
        if (!IsInsert)
            return;
 
        DbServerType dbServerType = (DbServerType)cboDbType.SelectedItem;
        string Template = dbServerType.GetTemplateConnectionString();

        edtConnectionString.Text = Template;
    }
    
    // ● overrides
    protected override async Task WindowInitialize()
    {
        cboDbType.ItemsSource = Enum.GetValues(typeof(DbServerType));
        cboDbType.SelectionChanged += (sender, args) => SetTemplateConnectionString();
        edtName.IsEnabled = IsInsert;
        cboDbType.IsEnabled = IsInsert;
        if (IsInsert)
            edtName.Focus();
        else
            edtConnectionString.Focus();

        await Task.CompletedTask;
    }
    protected override async Task ItemToControls()
    {
        ConInfo = InputData as DbConnectionInfo;
        edtName.Text = ConInfo.Name;
        cboDbType.SelectedItem = ConInfo.DbServerType;
        numTimeout.Value = ConInfo.CommandTimeoutSeconds;
        edtConnectionString.Text = ConInfo.ConnectionString;

        ResultData = ConInfo;

        IsInsert = string.IsNullOrWhiteSpace(edtName.Text);
        edtName.IsReadOnly = !IsInsert;
        if (!IsInsert)
            edtConnectionString.Focus();

        SetTemplateConnectionString();
        
        await Task.CompletedTask;
    }
    protected override async Task ControlsToItem()
    {
        string Message = string.Empty;
        bool Flag = await CheckIsOk(false);
        if (!Flag)
            return;

        ConInfo.Name = edtName.GetText();
        ConInfo.ConnectionString = edtConnectionString.GetText();
        ConInfo.CommandTimeoutSeconds = (int)numTimeout.Value;
        ConInfo.DbServerType = (DbServerType)cboDbType.SelectedItem;
        
        this.ModalResult = ModalResult.Ok;
    }

    // ● construction
    public DbConnectionDialog()
    {
        InitializeComponent();
    }
}