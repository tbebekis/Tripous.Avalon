/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Dialog used to edit database connection information.
/// </summary>
public partial class DbConnectionEditDialog : Window
{
    // ● private fields
    /// <summary>
    /// True while connection information is being loaded.
    /// </summary>
    private bool fLoading;
    /// <summary>
    /// The dialog result.
    /// </summary>
    bool DialogResult;

    // ● private methods
    /// <summary>
    /// Returns the selected database connection adapter.
    /// </summary>
    /// <returns>The selected database connection adapter.</returns>
    private DbConAdapter GetAdapter()
    {
        return DbConAdapters.Get((DbServerType)cboDbServerType.SelectedItem);
    }
    /// <summary>
    /// Returns a connection property definition.
    /// </summary>
    /// <param name="PropType">The connection property type.</param>
    /// <returns>The connection property definition, if any; otherwise, null.</returns>
    private DbConPropDef GetPropDef(DbConPropType PropType)
    {
        return GetAdapter().PropDefs.FirstOrDefault(item => item.PropType == PropType);
    }
    /// <summary>
    /// Returns true when a connection property type is supported by the selected adapter.
    /// </summary>
    /// <param name="PropType">The connection property type.</param>
    /// <returns>True if the property is supported; otherwise, false.</returns>
    private bool IsValidProp(DbConPropType PropType)
    {
        return GetPropDef(PropType) != null;
    }
    /// <summary>
    /// Configures a text box for a connection property.
    /// </summary>
    /// <param name="TextBox">The text box.</param>
    /// <param name="Label">The label.</param>
    /// <param name="PropType">The connection property type.</param>
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
    /// <summary>
    /// Configures a check box for a connection property.
    /// </summary>
    /// <param name="CheckBox">The check box.</param>
    /// <param name="PropType">The connection property type.</param>
    private void SetCheckBox(CheckBox CheckBox, DbConPropType PropType)
    {
        var def = GetPropDef(PropType);
        CheckBox.IsEnabled = def != null;
        CheckBox.IsChecked = false;
    }
    /// <summary>
    /// Configures the SSL mode combo box.
    /// </summary>
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
    /// <summary>
    /// Applies the selected database provider to the UI controls.
    /// </summary>
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
    /// <summary>
    /// Returns trimmed text from a text box.
    /// </summary>
    /// <param name="TextBox">The text box.</param>
    /// <returns>The trimmed text.</returns>
    private string GetText(TextBox TextBox)
    {
        return TextBox.Text == null ? "" : TextBox.Text.Trim();
    }
    /// <summary>
    /// Adds a connection property to a list when valid.
    /// </summary>
    /// <param name="Props">The property list.</param>
    /// <param name="PropType">The connection property type.</param>
    /// <param name="Value">The property value.</param>
    private void AddProp(List<DbConProp> Props, DbConPropType PropType, string Value)
    {
        if (IsValidProp(PropType) && !string.IsNullOrWhiteSpace(Value))
            Props.Add(new DbConProp { PropType = PropType, Value = Value });
    }
    /// <summary>
    /// Returns connection properties from the UI controls.
    /// </summary>
    /// <returns>The connection properties.</returns>
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
    /// <summary>
    /// Applies a connection property to the UI controls.
    /// </summary>
    /// <param name="Prop">The connection property.</param>
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
    /// <summary>
    /// Loads connection information to the UI controls.
    /// </summary>
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
    /// <summary>
    /// Validates the connection information.
    /// </summary>
    /// <returns>True if the connection information is valid; otherwise, false.</returns>
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
    /// <summary>
    /// Constructs the connection string from the UI controls.
    /// </summary>
    /// <returns>The constructed connection string.</returns>
    private string ConstructConnectionString()
    {
        return GetAdapter().Construct(GetPropsFromControls());
    }
    /// <summary>
    /// Updates the connection string preview.
    /// </summary>
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
    /// <summary>
    /// Saves the UI values to the connection information.
    /// </summary>
    private void SaveConnectionInfo()
    {
        ConnectionInfo.Name = GetText(edtName);
        ConnectionInfo.DbServerType = (DbServerType)cboDbServerType.SelectedItem;
        ConnectionInfo.CommandTimeoutSeconds = edtCommandTimeoutSeconds.AsInt(ConnectionInfo.CommandTimeoutSeconds);
        ConnectionInfo.ConnectionString = ConstructConnectionString();
    }
    /// <summary>
    /// Tests the current connection information.
    /// </summary>
    /// <returns>True if the connection succeeds; otherwise, false.</returns>
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
    
    /// <summary>
    /// Handles the Test Connection button click.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The routed event arguments.</param>
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
    /// <summary>
    /// Handles the OK button click.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The routed event arguments.</param>
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
    /// <summary>
    /// Handles the Cancel button click.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The routed event arguments.</param>
    private void btnCancel_Click(object Sender, RoutedEventArgs Args)
    {
        DialogResult = false;
        Close();
    }
    /// <summary>
    /// Handles database server type selection changes.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The selection changed event arguments.</param>
    private void cboDbServerType_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        ApplyProvider();
    }
    /// <summary>
    /// Handles tab selection changes.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The selection changed event arguments.</param>
    private void tabControl_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        if (tabControl.SelectedItem == tabConnectionString)
            UpdateConnectionStringPreview();
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="DbConnectionEditDialog"/> class.
    /// </summary>
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
        
        this.Loaded += async (s, e) =>
        {
            btnCancel.Focus();
        };
    }

    // ● static public methods
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    /// <param name="ConnectionInfo">The connection information to edit.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>True if the dialog was accepted; otherwise, false.</returns>
    static public async Task<bool> ShowModal(DbConnectionInfo ConnectionInfo, Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        Window parent = Caller.GetOwnerWindow();
        var dialog = new DbConnectionEditDialog();
        dialog.ConnectionInfo = ConnectionInfo;
        dialog.LoadConnectionInfo();
        await dialog.ShowDialog(parent);
        return dialog.DialogResult;
    }

    // ● properties
    /// <summary>
    /// Gets or sets the connection information being edited.
    /// </summary>
    public DbConnectionInfo ConnectionInfo { get; set; }
}
