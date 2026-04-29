namespace Tripous.Desktop;

public partial class DbConnectionEditDialog : Window
{
    // ● private fields
    private bool fLoading;
    bool DialogResult;

    // ● private methods
    private DbConAdapter GetAdapter()
    {
        return DbConAdapters.Get((DbServerType)cboDbServerType.SelectedItem);
    }
    private DbConPropDef GetPropDef(DbConPropType PropType)
    {
        return GetAdapter().PropDefs.FirstOrDefault(item => item.PropType == PropType);
    }
    private bool IsValidProp(DbConPropType PropType)
    {
        return GetPropDef(PropType) != null;
    }
    private void SetTextBox(TextBox TextBox, TextBlock Label, DbConPropType PropType)
    {
        var def = GetPropDef(PropType);
        var isValid = def != null;
        TextBox.IsEnabled = isValid;
        TextBox.Text = "";
        Label.IsEnabled = isValid;
        if (isValid)
        {
            Label.Text = def.Label;
            TextBox.Text = def.DefaultValue;
        }
    }
    private void SetCheckBox(CheckBox CheckBox, DbConPropType PropType)
    {
        var def = GetPropDef(PropType);
        CheckBox.IsEnabled = def != null;
        CheckBox.IsChecked = false;
    }
    private void SetSslMode()
    {
        var def = GetPropDef(DbConPropType.SslMode);
        var isValid = def != null;
        lblSslMode.IsEnabled = isValid;
        cboSslMode.IsEnabled = isValid;
        cboSslMode.ItemsSource = isValid ? def.ValidValues : Array.Empty<string>();
        cboSslMode.SelectedIndex = -1;
        if (isValid && def.ValidValues.Length > 0)
            cboSslMode.SelectedIndex = 0;
    }
    private void ApplyProvider()
    {
        if (fLoading)
            return;
        SetTextBox(edtServer, lblServer, DbConPropType.Server);
        SetTextBox(edtPort, lblPort, DbConPropType.Port);
        SetTextBox(edtDatabase, lblDatabase, DbConPropType.Database);
        SetTextBox(edtUserId, lblUserId, DbConPropType.UserId);
        SetTextBox(edtPassword, lblPassword, DbConPropType.Password);
        SetCheckBox(chIntegratedSecurity, DbConPropType.IntegratedSecurity);
        SetCheckBox(chTrustServerCertificate, DbConPropType.TrustServerCertificate);
        SetSslMode();
        SetTextBox(edtCharset, lblCharset, DbConPropType.Charset);
        edtConnectionString.Text = "";
        txtMessage.Text = "";
    }
    private string GetText(TextBox TextBox)
    {
        return TextBox.Text == null ? "" : TextBox.Text.Trim();
    }
    private void AddProp(List<DbConProp> Props, DbConPropType PropType, string Value)
    {
        if (IsValidProp(PropType) && !string.IsNullOrWhiteSpace(Value))
            Props.Add(new DbConProp { PropType = PropType, Value = Value });
    }
    private List<DbConProp> GetPropsFromControls()
    {
        var result = new List<DbConProp>();
        AddProp(result, DbConPropType.Server, GetText(edtServer));
        AddProp(result, DbConPropType.Port, GetText(edtPort));
        AddProp(result, DbConPropType.Database, GetText(edtDatabase));
        AddProp(result, DbConPropType.UserId, GetText(edtUserId));
        AddProp(result, DbConPropType.Password, GetText(edtPassword));
        if (IsValidProp(DbConPropType.IntegratedSecurity))
            AddProp(result, DbConPropType.IntegratedSecurity, chIntegratedSecurity.IsChecked == true ? "True" : "False");
        if (IsValidProp(DbConPropType.TrustServerCertificate))
            AddProp(result, DbConPropType.TrustServerCertificate, chTrustServerCertificate.IsChecked == true ? "True" : "False");
        if (IsValidProp(DbConPropType.SslMode) && cboSslMode.SelectedItem != null)
            AddProp(result, DbConPropType.SslMode, cboSslMode.SelectedItem.ToString());
        AddProp(result, DbConPropType.Charset, GetText(edtCharset));
        return result;
    }
    private void SetPropToControls(DbConProp Prop)
    {
        if (Prop.PropType == DbConPropType.Server)
            edtServer.Text = Prop.Value;
        else if (Prop.PropType == DbConPropType.Port)
            edtPort.Text = Prop.Value;
        else if (Prop.PropType == DbConPropType.Database)
            edtDatabase.Text = Prop.Value;
        else if (Prop.PropType == DbConPropType.UserId)
            edtUserId.Text = Prop.Value;
        else if (Prop.PropType == DbConPropType.Password)
            edtPassword.Text = Prop.Value;
        else if (Prop.PropType == DbConPropType.IntegratedSecurity)
            chIntegratedSecurity.IsChecked = string.Equals(Prop.Value, "True", StringComparison.OrdinalIgnoreCase);
        else if (Prop.PropType == DbConPropType.TrustServerCertificate)
            chTrustServerCertificate.IsChecked = string.Equals(Prop.Value, "True", StringComparison.OrdinalIgnoreCase);
        else if (Prop.PropType == DbConPropType.SslMode)
            cboSslMode.SelectedItem = Prop.Value;
        else if (Prop.PropType == DbConPropType.Charset)
            edtCharset.Text = Prop.Value;
    }
    private void LoadConnectionInfo()
    {
        fLoading = true;
        edtName.Text = ConnectionInfo.Name;
        cboDbServerType.ItemsSource = Enum.GetValues(typeof(DbServerType));
        cboDbServerType.SelectedItem = ConnectionInfo.DbServerType;
        edtCommandTimeoutSeconds.Text = ConnectionInfo.CommandTimeoutSeconds.ToString();
        fLoading = false;
        ApplyProvider();
        if (!string.IsNullOrWhiteSpace(ConnectionInfo.ConnectionString))
        {
            var props = GetAdapter().Parse(ConnectionInfo.ConnectionString);
            foreach (var prop in props)
                SetPropToControls(prop);
        }
    }
    private bool Validate()
    {
        txtMessage.Text = "";
        if (string.IsNullOrWhiteSpace(GetText(edtName)))
        {
            txtMessage.Text = "Name is required.";
            return false;
        }
        foreach (var def in GetAdapter().PropDefs.Where(item => item.IsRequired))
        {
            var value = GetPropsFromControls().FirstOrDefault(item => item.PropType == def.PropType);
            if (value == null || string.IsNullOrWhiteSpace(value.Value))
            {
                txtMessage.Text = def.Label + " is required.";
                return false;
            }
        }
        return true;
    }
    private string ConstructConnectionString()
    {
        return GetAdapter().Construct(GetPropsFromControls());
    }
    private void UpdateConnectionStringPreview()
    {
        try
        {
            edtConnectionString.Text = ConstructConnectionString();
        }
        catch (Exception ex)
        {
            edtConnectionString.Text = ex.Message;
        }
    }
    private void SaveConnectionInfo()
    {
        ConnectionInfo.Name = GetText(edtName);
        ConnectionInfo.DbServerType = (DbServerType)cboDbServerType.SelectedItem;
        ConnectionInfo.CommandTimeoutSeconds = edtCommandTimeoutSeconds.AsInt(ConnectionInfo.CommandTimeoutSeconds);
        ConnectionInfo.ConnectionString = ConstructConnectionString();
    }
    private async Task<bool> TestConnection()
    {
        if (!Validate())
            return false;
        var connectionString = ConstructConnectionString();
        var factory = DbProviderFactories.GetFactory(ConnectionInfo.GetProviderInvariantName());
        using var connection = factory.CreateConnection();
        connection.ConnectionString = connectionString;
        await connection.OpenAsync();
        await connection.CloseAsync();
        txtMessage.Text = "Connection succeeded.";
        return true;
    }
    
    private async void btnTestConnection_Click(object Sender, RoutedEventArgs Args)
    {
        try
        {
            SaveConnectionInfo();
            await TestConnection();
        }
        catch (Exception ex)
        {
            txtMessage.Text = ex.Message;
        }
    }
    private async void btnOK_Click(object Sender, RoutedEventArgs Args)
    {
        try
        {
            if (!Validate())
                return;
            SaveConnectionInfo();
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            txtMessage.Text = ex.Message;
        }
    }
    private void btnCancel_Click(object Sender, RoutedEventArgs Args)
    {
        DialogResult = false;
        Close();
    }
    private void cboDbServerType_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        ApplyProvider();
    }
    private void tabControl_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (tabControl.SelectedItem == tabConnectionString)
            UpdateConnectionStringPreview();
    }

    // ● constructor
    public DbConnectionEditDialog()
    {
        InitializeComponent();
        cboDbServerType.SelectionChanged += cboDbServerType_SelectionChanged;
        tabControl.SelectionChanged += tabControl_SelectionChanged;
        btnTestConnection.Click += btnTestConnection_Click;
        btnOK.IsDefault = true;
        btnCancel.IsCancel = true;
        btnOK.Click += btnOK_Click;
        btnCancel.Click += btnCancel_Click;
    }

    // ● static public methods
    static public async Task<bool> ShowModal(DbConnectionInfo ConnectionInfo, Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        Window parent = Caller is Window ? Caller as Window : Caller.GetParentWindow();
        var dialog = new DbConnectionEditDialog();
        dialog.ConnectionInfo = ConnectionInfo;
        dialog.LoadConnectionInfo();
        await dialog.ShowDialog(parent);
        return dialog.DialogResult;
    }

    // ● properties
    public DbConnectionInfo ConnectionInfo { get; set; }
}