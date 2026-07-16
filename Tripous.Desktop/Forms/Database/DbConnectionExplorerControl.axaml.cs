/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Displays database connections and their schema metadata in a tree view.
/// </summary>
public partial class DbConnectionExplorerControl : UserControl
{
    // ● private fields
    /// <summary>
    /// The connection list displayed by the control.
    /// </summary>
    ObservableCollection<DbConnectionInfo> fConnections = new();
    /// <summary>
    /// The explorer options.
    /// </summary>
    DbConnectionExplorerOptions fOptions = new();

    // ● private
    async void AnyClick(object Sender, RoutedEventArgs Args)
    {
        if (Sender == btnSqlEditor)
            await ShowSqlEditor();
        else if (Sender == btnInsert)
            await Insert();
        else if (Sender == btnEdit)
            await Edit();
        else if (Sender == btnDelete)
            await Delete();
        else if (Sender == btnToggleConnect)
            await ToggleConnect();
        else if (Sender == btnRefreshSchema)
            await RefreshSchema();
        else if (Sender == mnuExpand)
            Expand();
        else if (Sender == mnuCollapse)
            Collapse();
        else if (Sender == mnuShowSourceCode)
            ShowCode(ShowCodeMode.SourceCode);
        else if (Sender == mnuShowFieldList)
            ShowCode(ShowCodeMode.FieldList);
        else if (Sender == mnuSelectTableOrView)
            ShowCode(ShowCodeMode.Select);
    }
    void Tv_SelectionChanged(object Sender, SelectionChangedEventArgs Args)
    {
        SelectedNodeChanged();
    }
    async void Tv_DoubleTapped(object Sender, RoutedEventArgs Args)
    {
        await ConnectSchema();
    }
    void Tv_OnKeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.KeyModifiers == KeyModifiers.Control)
        {
            if (Args.Key == Key.OemPlus || Args.Key == Key.Add)
            {
                Expand();
                Args.Handled = true;
            }
            else if (Args.Key == Key.OemMinus || Args.Key == Key.Subtract)
            {
                Collapse();
                Args.Handled = true;
            }
        }
    }
    void TvMenu_OnOpening(object Sender, CancelEventArgs Args)
    {
        if (tv.SelectedItem == null)
        {
            Args.Cancel = true;
            return;
        }
        TreeViewItem Node = tv.SelectedItem as TreeViewItem;
        bool Flag = Node != null && (Node.Tag is DbMetaTable || Node.Tag is DbMetaView);
        mnuShowSourceCode.IsEnabled = Flag;
        mnuShowFieldList.IsEnabled = Flag;
        mnuSelectTableOrView.IsEnabled = Flag;
    }
    void ApplyOptions()
    {
        ToolBarContainer.IsVisible = Options.ShowToolBar;
        btnInsert.IsVisible = Options.AllowAddConnections;
        btnEdit.IsVisible = Options.AllowEditConnections;
        btnDelete.IsVisible = Options.AllowDeleteConnections;
    }
    void Log(string Text)
    {
        if (!string.IsNullOrWhiteSpace(Text))
            LogMessage?.Invoke(this, Text);
    }
    StackPanel CreateHeader(string Text, string ImageFileName, bool Bold = false)
    {
        StackPanel Result = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
        Image Image = AvaloniaAssets.FindImage16(ImageFileName);
        if (Image != null)
            Result.Children.Add(Image);
        Result.Children.Add(new TextBlock { Text = Text, FontWeight = Bold ? FontWeight.SemiBold : FontWeight.Normal });
        return Result;
    }
    TreeViewItem CreateNode(string Text, string ImageFileName, object Tag = null, bool Bold = false)
    {
        return new TreeViewItem { Header = CreateHeader(Text, ImageFileName, Bold), Tag = Tag };
    }
    TreeViewItem FindSchemaNode()
    {
        TreeViewItem Current = tv.SelectedItem as TreeViewItem;
        while (Current != null)
        {
            if (Current.Tag is DbSchema)
                return Current;
            Current = Current.Parent as TreeViewItem;
        }
        return null;
    }
    DbSchema FindSchema()
    {
        TreeViewItem Node = FindSchemaNode();
        return Node?.Tag as DbSchema;
    }
    void SelectedNodeChanged()
    {
        TreeViewItem SchemaNode = FindSchemaNode();
        if (SchemaNode == null)
        {
            SelectedConnection = null;
            ConnectionSelected?.Invoke(this, new DbConnectionExplorerEventArgs(null));
            return;
        }
        DbSchema Schema = SchemaNode.Tag as DbSchema;
        SelectedConnection = Schema.ConnectionInfo;
        ToolTip.SetTip(btnToggleConnect, Schema.IsLoaded ? "Disconnect" : "Connect");
        AvaloniaAssets.SetImage(imgToggleConnect, Schema.IsLoaded ? "database.png" : "database_green.png");
        ConnectionSelected?.Invoke(this, new DbConnectionExplorerEventArgs(SelectedConnection));
    }
    void AddSchemaNodes()
    {
        tv.Items.Clear();
        foreach (DbConnectionInfo ConnectionInfo in Connections)
            AddSchemaNode(ConnectionInfo.Schema);
        if (tv.Items.Count > 0)
        {
            tv.SelectedItem = tv.Items[0];
            SelectedNodeChanged();
        }
    }
    TreeViewItem AddSchemaNode(DbSchema Schema)
    {
        TreeViewItem Result = CreateNode(Schema.Name, "database.png", Schema);
        tv.Items.Add(Result);
        return Result;
    }
    void UpdateSchemaNodeAfterLoad(TreeViewItem RootNode, DbSchema Schema)
    {
        if (RootNode.Header is StackPanel Stack && Stack.Children.OfType<Image>().FirstOrDefault() is Image Image)
            AvaloniaAssets.SetImage(Image, "database_green.png");
        RootNode.Items.Clear();
        if (Schema.Tables.Any())
        {
            TreeViewItem TablesFolder = CreateNode("Tables", "folder.png", Bold: true);
            foreach (DbMetaTable Table in Schema.Tables)
            {
                TreeViewItem TableNode = CreateNode(Table.Name, "table.png", Table);
                TreeViewItem ColumnsFolder = CreateNode("Columns", "folder.png", Bold: true);
                foreach (DbMetaColumn Column in Table.Columns)
                    ColumnsFolder.Items.Add(CreateNode(Column.DisplayText, "item.png", Column));
                TableNode.Items.Add(ColumnsFolder);
                TreeViewItem IndexesFolder = CreateNode("Indexes", "folder.png", Bold: true);
                foreach (DbMetaIndex Index in Table.Indexes)
                    IndexesFolder.Items.Add(CreateNode(Index.DisplayText, "item.png", Index));
                TableNode.Items.Add(IndexesFolder);
                TreeViewItem ConstraintsFolder = CreateNode("Constraints", "folder.png", Bold: true);
                foreach (DbMetaConstraint Constraint in Table.Constraints)
                    ConstraintsFolder.Items.Add(CreateNode(Constraint.DisplayText, "item.png", Constraint));
                TableNode.Items.Add(ConstraintsFolder);
                TreeViewItem TriggersFolder = CreateNode("Triggers", "folder.png", Bold: true);
                foreach (DbMetaTrigger Trigger in Table.Triggers)
                    TriggersFolder.Items.Add(CreateNode(Trigger.DisplayText, "item.png", Trigger));
                TableNode.Items.Add(TriggersFolder);
                TablesFolder.Items.Add(TableNode);
            }
            RootNode.Items.Add(TablesFolder);
        }
        if (Schema.Views.Any())
        {
            TreeViewItem ViewsFolder = CreateNode("Views", "folder.png", Bold: true);
            foreach (DbMetaView View in Schema.Views)
            {
                TreeViewItem ViewNode = CreateNode(View.Name, "table.png", View);
                TreeViewItem ColumnsFolder = CreateNode("Columns", "folder.png", Bold: true);
                foreach (DbMetaColumn Column in View.Columns)
                    ColumnsFolder.Items.Add(CreateNode(Column.DisplayText, "item.png", Column));
                ViewNode.Items.Add(ColumnsFolder);
                ViewsFolder.Items.Add(ViewNode);
            }
            RootNode.Items.Add(ViewsFolder);
        }
        RootNode.IsExpanded = true;
    }
    async Task ShowSqlEditor()
    {
        TreeViewItem Node = FindSchemaNode();
        if (Node == null)
        {
            Log("Cannot open SQL editor. No connection selected.");
            return;
        }
        DbSchema Schema = Node.Tag as DbSchema;
        if (!Schema.IsLoaded)
            await ConnectSchema(Schema, Node);
        OpenSqlRequested?.Invoke(this, new DbConnectionExplorerEventArgs(Schema.ConnectionInfo));
    }
    async Task Insert()
    {
        if (!Options.AllowAddConnections)
            return;
        DbConnectionInfo ConnectionInfo = new DbConnectionInfo();
        bool Flag = await DbConnectionEditDialog.ShowModal(ConnectionInfo, this);
        if (!Flag)
            return;
        if (Connections.Any(Item => Item.Name.IsSameText(ConnectionInfo.Name)))
        {
            await MessageBox.Error($"Connection '{ConnectionInfo.Name}' already exists.", this);
            return;
        }
        Connections.Add(ConnectionInfo);
        if (Options.PersistConnectionChanges)
            Db.Connections.Save();
        TreeViewItem Node = AddSchemaNode(ConnectionInfo.Schema);
        tv.SelectedItem = Node;
        await CreateDatabaseIfNeeded(ConnectionInfo);
        SelectedNodeChanged();
    }
    async Task Edit()
    {
        if (!Options.AllowEditConnections)
            return;
        TreeViewItem SchemaNode = FindSchemaNode();
        if (SchemaNode == null)
            return;
        DbSchema Schema = SchemaNode.Tag as DbSchema;
        DisconnectSchema(Schema, SchemaNode);
        bool Flag = await DbConnectionEditDialog.ShowModal(Schema.ConnectionInfo, this);
        if (Flag && Options.PersistConnectionChanges)
            Db.Connections.Save();
        if (SchemaNode.Header is StackPanel Stack && Stack.Children.OfType<TextBlock>().FirstOrDefault() is TextBlock Text)
            Text.Text = Schema.ConnectionInfo.Name;
        tv.SelectedItem = SchemaNode;
        SelectedNodeChanged();
    }
    async Task Delete()
    {
        if (!Options.AllowDeleteConnections)
            return;
        TreeViewItem SchemaNode = FindSchemaNode();
        if (SchemaNode == null)
            return;
        DbSchema Schema = SchemaNode.Tag as DbSchema;
        DisconnectSchema(Schema, SchemaNode);
        bool Flag = await MessageBox.YesNo($"Delete connection '{Schema.ConnectionInfo.Name}'?", this);
        if (!Flag)
            return;
        Connections.Remove(Schema.ConnectionInfo);
        tv.Items.Remove(SchemaNode);
        if (Options.PersistConnectionChanges)
            Db.Connections.Save();
        SelectedNodeChanged();
    }
    async Task CreateDatabaseIfNeeded(DbConnectionInfo ConnectionInfo)
    {
        if (!Options.AllowCreateDatabases)
            return;
        SqlProvider Provider = ConnectionInfo.GetSqlProvider();
        if (!Provider.CanCreateDatabases || Provider.DatabaseExists(ConnectionInfo.ConnectionString))
            return;
        bool Flag = await MessageBox.YesNo($"Create database for connection '{ConnectionInfo.Name}'?", this);
        if (Flag)
            Provider.CreateDatabase(ConnectionInfo.ConnectionString);
    }
    async Task RefreshSchema()
    {
        DisconnectSchema();
        await ConnectSchema();
        SelectedNodeChanged();
    }
    async Task ToggleConnect()
    {
        TreeViewItem SchemaNode = FindSchemaNode();
        if (SchemaNode == null)
            return;
        DbSchema Schema = SchemaNode.Tag as DbSchema;
        if (Schema.IsLoaded)
            DisconnectSchema(Schema, SchemaNode);
        else
            await ConnectSchema(Schema, SchemaNode);
        SelectedNodeChanged();
    }
    async Task ConnectSchema()
    {
        TreeViewItem SchemaNode = FindSchemaNode();
        if (SchemaNode == null)
            return;
        await ConnectSchema(SchemaNode.Tag as DbSchema, SchemaNode);
    }
    async Task ConnectSchema(DbSchema Schema, TreeViewItem SchemaNode)
    {
        if (Schema == null || Schema.IsLoaded)
            return;
        try
        {
            Log($"Loading schema: {Schema.Name}...");
            await Task.Run(() => Schema.Load());
            UpdateSchemaNodeAfterLoad(SchemaNode, Schema);
            Log($"Schema {Schema.Name} loaded.");
        }
        catch (Exception ex)
        {
            Log(ex.Message);
            await MessageBox.Error(ex.Message, this);
        }
    }
    void DisconnectSchema()
    {
        TreeViewItem SchemaNode = FindSchemaNode();
        if (SchemaNode == null)
            return;
        DisconnectSchema(SchemaNode.Tag as DbSchema, SchemaNode);
    }
    void DisconnectSchema(DbSchema Schema, TreeViewItem SchemaNode)
    {
        if (Schema == null || !Schema.IsLoaded)
            return;
        Schema.UnLoad();
        SchemaNode.Items.Clear();
        if (SchemaNode.Header is StackPanel Stack && Stack.Children.OfType<Image>().FirstOrDefault() is Image Image)
            AvaloniaAssets.SetImage(Image, "database.png");
        Log($"Connection {Schema.Name} unloaded.");
    }
    void Expand()
    {
        if (tv.SelectedItem is TreeViewItem Node)
            SetExpansionRecursive(Node, true);
    }
    void Collapse()
    {
        if (tv.SelectedItem is TreeViewItem Node)
            SetExpansionRecursive(Node, false);
    }
    void SetExpansionRecursive(TreeViewItem Node, bool IsExpanded)
    {
        Node.IsExpanded = IsExpanded;
        foreach (object Item in Node.Items)
            if (Item is TreeViewItem ChildNode)
                SetExpansionRecursive(ChildNode, IsExpanded);
    }
    void ShowCode(ShowCodeMode Mode)
    {
        TreeViewItem Node = tv.SelectedItem as TreeViewItem;
        if (Node == null || !(Node.Tag is DbMetaTable || Node.Tag is DbMetaView))
            return;
        string SqlText = string.Empty;
        if (Node.Tag is DbMetaTable Table)
        {
            if (Mode == ShowCodeMode.SourceCode)
                SqlText = Table.GetCreateTable();
            else if (Mode == ShowCodeMode.FieldList)
                SqlText = Table.GetFieldNameList();
            else if (Mode == ShowCodeMode.Select)
                SqlText = $"select * from {Table.Name}";
        }
        else if (Node.Tag is DbMetaView View)
        {
            if (Mode == ShowCodeMode.SourceCode)
                SqlText = View.SourceCode;
            else if (Mode == ShowCodeMode.FieldList)
                SqlText = View.GetFieldNameList();
            else if (Mode == ShowCodeMode.Select)
                SqlText = $"select * from {View.Name}";
        }
        if (!string.IsNullOrWhiteSpace(SqlText))
            SqlTextRequested?.Invoke(this, new DbConnectionExplorerSqlTextEventArgs(FindSchema()?.ConnectionInfo, SqlText));
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public DbConnectionExplorerControl()
    {
        InitializeComponent();
        ApplyOptions();
    }

    // ● public
    /// <summary>
    /// Assigns the connections displayed by the explorer.
    /// </summary>
    /// <param name="Connections">The connection list.</param>
    public void SetConnections(IEnumerable<DbConnectionInfo> Connections)
    {
        if (Connections is ObservableCollection<DbConnectionInfo> Observable)
            fConnections = Observable;
        else
            fConnections = new ObservableCollection<DbConnectionInfo>(Connections ?? []);
        AddSchemaNodes();
    }
    /// <summary>
    /// Reloads the tree from the assigned connection list.
    /// </summary>
    public void Reload()
    {
        AddSchemaNodes();
    }

    // ● properties
    /// <summary>
    /// Gets the displayed connections.
    /// </summary>
    public ObservableCollection<DbConnectionInfo> Connections => fConnections;
    /// <summary>
    /// Gets or sets explorer options.
    /// </summary>
    public DbConnectionExplorerOptions Options
    {
        get => fOptions;
        set
        {
            fOptions = value ?? new DbConnectionExplorerOptions();
            ApplyOptions();
        }
    }
    /// <summary>
    /// Gets the selected connection.
    /// </summary>
    public DbConnectionInfo SelectedConnection { get; private set; }

    // ● events
    /// <summary>
    /// Occurs when the selected connection changes.
    /// </summary>
    public event EventHandler<DbConnectionExplorerEventArgs> ConnectionSelected;
    /// <summary>
    /// Occurs when the user requests opening an SQL editor.
    /// </summary>
    public event EventHandler<DbConnectionExplorerEventArgs> OpenSqlRequested;
    /// <summary>
    /// Occurs when SQL text is requested from a table or view node.
    /// </summary>
    public event EventHandler<DbConnectionExplorerSqlTextEventArgs> SqlTextRequested;
    /// <summary>
    /// Occurs when the control writes a log message.
    /// </summary>
    public event EventHandler<string> LogMessage;

    // ● private types
    /// <summary>
    /// The SQL text generation mode.
    /// </summary>
    enum ShowCodeMode
    {
        /// <summary>
        /// Show source code.
        /// </summary>
        SourceCode,
        /// <summary>
        /// Show a field list.
        /// </summary>
        FieldList,
        /// <summary>
        /// Show a SELECT statement.
        /// </summary>
        Select
    }
}
