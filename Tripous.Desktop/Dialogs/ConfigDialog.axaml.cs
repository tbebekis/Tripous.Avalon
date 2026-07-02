/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Displays and edits registered application configuration properties.
/// </summary>
public partial class ConfigDialog : Window
{
    // ● private fields
    readonly Dictionary<ConfigPropertyDef, Control> fControls = new();
    readonly Dictionary<ConfigPropertyDef, IConfigEditor> fEditors = new();
    bool fLoading;

    // ● private methods
    bool CanAccess(ConfigPropertyDef Def)
    {
        UserLevel Current = Sys.Context.CurrentUser != null ? Sys.Context.CurrentUser.UserLevel : UserLevel.God;
        if ((Current & UserLevel.God) == UserLevel.God)
            return true;
        if (Def.SecurityLevel == UserLevel.None)
            return true;
        if ((Current & UserLevel.Admin) == UserLevel.Admin)
            return Def.SecurityLevel == UserLevel.Admin || Def.SecurityLevel == UserLevel.User || Def.SecurityLevel == UserLevel.Guest;
        if ((Current & UserLevel.User) == UserLevel.User)
            return Def.SecurityLevel == UserLevel.User || Def.SecurityLevel == UserLevel.Guest;
        if ((Current & UserLevel.Guest) == UserLevel.Guest)
            return Def.SecurityLevel == UserLevel.Guest;
        return (Current & Def.SecurityLevel) == Def.SecurityLevel;
    }
    ConfigScope GetScope()
    {
        if (cboScope.SelectedItem is ConfigScope Scope)
            return Scope;
        return ConfigScope.User;
    }
    string GetOwnerKey(ConfigScope Scope)
    {
        if (Scope == ConfigScope.System)
            return string.Empty;
        if (Scope == ConfigScope.Company)
            return DbConfig.CompanyId.ToString();
        return Sys.GetCurrentAppUserName();
    }
    string GetEffectiveValue(ConfigPropertyDef Def)
    {
        string Value = Config.GetValue(Def.Name, GetScope(), GetOwnerKey(GetScope()));
        if (Value == null)
            Value = Config.GetValue(Def.Name);
        return Value;
    }
    string GetTitle(ConfigPropertyDef Def)
    {
        if (!string.IsNullOrWhiteSpace(Def.TitleKey))
            return Def.TitleKey;
        return Def.Name;
    }
    TextBlock CreateLabel(ConfigPropertyDef Def)
    {
        TextBlock Result = new();
        Result.Text = GetTitle(Def);
        Result.VerticalAlignment = VerticalAlignment.Center;
        ToolTip.SetTip(Result, Def.Name);
        return Result;
    }
    TextBox CreateTextBox(string Text, bool IsReadOnly = false)
    {
        TextBox Result = new();
        Result.Text = Text ?? string.Empty;
        Result.IsReadOnly = IsReadOnly;
        return Result;
    }
    Control CreateScalarControl(ConfigPropertyDef Def)
    {
        string Value = GetEffectiveValue(Def);
        if (Def.Kind == ConfigValueKind.Boolean)
        {
            CheckBox Result = new();
            Result.IsChecked = Value != null && Convert.ToBoolean(Value, CultureInfo.InvariantCulture);
            return Result;
        }
        if (Def.Kind == ConfigValueKind.Decimal || Def.Kind == ConfigValueKind.Double)
        {
            NumericUpDown Result = new();
            if (decimal.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal DecimalValue))
                Result.Value = DecimalValue;
            return Result;
        }
        if (Def.Kind == ConfigValueKind.Integer)
        {
            NumericUpDown Result = new();
            Result.Increment = 1;
            if (decimal.TryParse(Value, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal DecimalValue))
                Result.Value = DecimalValue;
            return Result;
        }
        if (Def.Kind == ConfigValueKind.Memo)
        {
            TextBox Result = CreateTextBox(Value);
            Result.AcceptsReturn = true;
            Result.MinHeight = 120;
            return Result;
        }
        return CreateTextBox(Value);
    }
    string GetScalarValue(ConfigPropertyDef Def, Control Control)
    {
        if (Control is CheckBox CheckBox)
            return CheckBox.IsChecked == true ? "true" : "false";
        if (Control is NumericUpDown NumericUpDown)
            return NumericUpDown.Value.HasValue ? NumericUpDown.Value.Value.ToString(CultureInfo.InvariantCulture) : string.Empty;
        if (Control is TextBox TextBox)
            return TextBox.Text ?? string.Empty;
        return string.Empty;
    }
    Control CreateEditorControl(ConfigPropertyDef Def)
    {
        if (string.IsNullOrWhiteSpace(Def.EditorClassName))
        {
            TextBox Result = CreateTextBox(GetEffectiveValue(Def), IsReadOnly: true);
            Result.AcceptsReturn = true;
            return Result;
        }
        IConfigEditor Editor = TypeStore.CreateInstance<IConfigEditor>(Def.EditorClassName);
        if (Editor is not Control Control)
            throw new TripousException($"Config editor '{Def.EditorClassName}' is not an Avalonia control.");
        Editor.LoadValue(Def, GetEffectiveValue(Def));
        fEditors[Def] = Editor;
        return Control;
    }
    IEnumerable<ConfigPropertyDef> GetVisibleDefs()
    {
        ConfigScope Scope = GetScope();
        return DataRegistry.ConfigProperties
            .Where(CanAccess)
            .Where(Def => Def.SupportsScope(Scope))
            .OrderBy(Def => Def.GroupName)
            .ThenBy(GetTitle);
    }
    void AddScalarRows(StackPanel Panel, IEnumerable<ConfigPropertyDef> Defs)
    {
        foreach (ConfigPropertyDef Def in Defs)
        {
            Grid Row = new();
            Row.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(220)));
            Row.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(1, GridUnitType.Star)));
            Row.Margin = new Thickness(0, 0, 0, 8);
            Row.Children.Add(CreateLabel(Def));
            Control Control = CreateScalarControl(Def);
            Grid.SetColumn(Control, 1);
            Row.Children.Add(Control);
            Panel.Children.Add(Row);
            fControls[Def] = Control;
        }
    }
    void AddScalarTab(List<ConfigPropertyDef> Defs)
    {
        StackPanel Panel = new();
        Panel.Margin = new Thickness(10);
        Panel.Spacing = 0;
        foreach (IGrouping<string, ConfigPropertyDef> Group in Defs.GroupBy(Def => Def.GroupName ?? string.Empty))
        {
            if (!string.IsNullOrWhiteSpace(Group.Key))
            {
                TextBlock Header = new();
                Header.Text = Group.Key;
                Header.FontWeight = FontWeight.SemiBold;
                Header.Margin = new Thickness(0, 6, 0, 8);
                Panel.Children.Add(Header);
            }
            AddScalarRows(Panel, Group);
        }
        ScrollViewer ScrollViewer = new();
        ScrollViewer.Content = Panel;
        tabControl.Items.Add(new TabItem() { Header = "Settings", Content = ScrollViewer });
    }
    void AddEditorTabs(IEnumerable<ConfigPropertyDef> Defs)
    {
        foreach (ConfigPropertyDef Def in Defs)
        {
            Control Control = CreateEditorControl(Def);
            tabControl.Items.Add(new TabItem() { Header = GetTitle(Def), Content = Control });
        }
    }
    void LoadSettings()
    {
        fControls.Clear();
        fEditors.Clear();
        tabControl.Items.Clear();
        List<ConfigPropertyDef> Defs = GetVisibleDefs().ToList();
        List<ConfigPropertyDef> ScalarDefs = Defs.Where(Def => Def.Kind != ConfigValueKind.Object).ToList();
        List<ConfigPropertyDef> ObjectDefs = Defs.Where(Def => Def.Kind == ConfigValueKind.Object).ToList();
        AddScalarTab(ScalarDefs);
        AddEditorTabs(ObjectDefs);
        if (tabControl.Items.Count > 0)
            tabControl.SelectedIndex = 0;
    }
    void RefreshOwnerText()
    {
        ConfigScope Scope = GetScope();
        string OwnerKey = GetOwnerKey(Scope);
        txtOwner.Text = string.IsNullOrWhiteSpace(OwnerKey) ? "Owner: System" : $"Owner: {OwnerKey}";
    }
    void SaveSettings()
    {
        ConfigScope Scope = GetScope();
        string OwnerKey = GetOwnerKey(Scope);
        foreach (var Pair in fControls)
        {
            if (!Pair.Key.SupportsScope(Scope))
                continue;
            if (!CanAccess(Pair.Key))
                throw new TripousException($"Access denied to setting '{Pair.Key.Name}'.");
            Config.SetValue(Pair.Key.Name, GetScalarValue(Pair.Key, Pair.Value), Scope, OwnerKey);
        }
        foreach (var Pair in fEditors)
        {
            if (!Pair.Key.SupportsScope(Scope))
                continue;
            if (!CanAccess(Pair.Key))
                throw new TripousException($"Access denied to setting '{Pair.Key.Name}'.");
            Config.SetValue(Pair.Key.Name, Pair.Value.SaveValue(), Scope, OwnerKey);
        }
        txtMessage.Text = "Settings saved.";
    }
    void SaveButtonClick()
    {
        try
        {
            SaveSettings();
        }
        catch (Exception e)
        {
            txtMessage.Text = e.Message;
        }
    }
    void ScopeChanged()
    {
        if (fLoading)
            return;
        RefreshOwnerText();
        LoadSettings();
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public ConfigDialog()
    {
        InitializeComponent();
        fLoading = true;
        cboScope.ItemsSource = new ConfigScope[] { ConfigScope.User, ConfigScope.Company, ConfigScope.System };
        cboScope.SelectedItem = ConfigScope.User;
        fLoading = false;
        RefreshOwnerText();
        LoadSettings();
        cboScope.SelectionChanged += (Sender, Args) => ScopeChanged();
        btnSave.Click += (Sender, Args) => SaveButtonClick();
        btnClose.Click += (Sender, Args) => Close();
        Loaded += (Sender, Args) => btnClose.Focus(NavigationMethod.Tab, KeyModifiers.None);
    }

    // ● static public methods
    /// <summary>
    /// Shows the dialog modally.
    /// </summary>
    static public async Task ShowModal(Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        Window Parent = Caller.GetOwnerWindow();
        ConfigDialog Dialog = new();
        await Dialog.ShowDialog(Parent);
    }
}
