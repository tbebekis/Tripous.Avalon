/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Displays application commands in a tree view.
/// </summary>
public partial class CommandTreeViewForm : AppForm
{
    // ● private fields
    /// <summary>
    /// The commands displayed by the form.
    /// </summary>
    DefList<Command> Commands;
    /// <summary>
    /// The form toolbar.
    /// </summary>
    ToolBar ToolBar;
    
    // ● private
    /// <summary>
    /// Handles a tree view double-tap and executes the selected command.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The routed event arguments.</param>
    async void tv_DoubleTapped(object sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await ExecuteCommand();
    }
 
    /// <summary>
    /// Creates the tree view nodes.
    /// </summary>
    void CreateTreeViewNodes()
    {
        // ----------------------------------------------------------------------
        TreeViewItem CreateFolderNode(string Caption) => Ui.CreateContainerNode(Caption, Tag: null, IconFile: "folder16.png", NegativeMargin: 10);
        // ----------------------------------------------------------------------
        TreeViewItem CreateLeafNode(string Caption, object Tag, string IconFile = "item16.png") => Ui.CreateLeafNode(Caption, Tag: Tag, IconFile: IconFile, NegativeMargin: 10);
        // ----------------------------------------------------------------------
        void AddCommandNode(IList Items, Command Command)
        {
            TreeViewItem Node;
            if (Command.HasChildren)
            {
                if (Command.Commands.Count > 0)
                {
                    Node = CreateFolderNode(Command.Title);
                    foreach (Command ChildCommand in Command.Commands)
                        AddCommandNode(Node.Items, ChildCommand);
                    Items.Add(Node);
                }
            }
            else
            {
                Node  = CreateLeafNode(Command.Title, Command);
                Items.Add(Node);
            }
        }
        // ----------------------------------------------------------------------
        
        tv.Items.Clear();

        if (Commands == null)
            return;

        foreach (Command Command in Commands)
            AddCommandNode(tv.Items, Command);
    }
    /// <summary>
    /// Executes the selected command.
    /// </summary>
    async Task ExecuteCommand()
    {
        if (tv.SelectedItem is not TreeViewItem Node) 
            return;

        if (Node.Tag is not Command Cmd) 
            return;

        if (Cmd.IsAsync)
            await Cmd.ExecuteAsync();

        if (Cmd.IsSync)
            Cmd.Execute();
    }

    /// <summary>
    /// Creates the toolbar.
    /// </summary>
    void CreateToolBar()
    {
        if (ToolBar == null)
        {
            ToolBar = new();
            ToolBar.Panel = pnlToolBar;

            ToolBar.AddButton("arrow_out.png", "Expand", () => tv.ExpandAll(Flag: true) );
            ToolBar.AddButton("arrow_in.png", "Collapse", () => tv.ExpandAll(Flag: false) );
            ToolBar.AddButton("lightning.png", "Execute", async () => await ExecuteCommand());
        }
    }
    
    
    // ● overrides
    /// <summary>
    /// Called in order to initialize the form
    /// </summary>
    protected override void FormInitialize()
    {
        TitleText = "Commands";
        ClosableByUser = false;
        CreateTreeViewNodes();
        CreateToolBar();
        tv.DoubleTapped += tv_DoubleTapped;
    }
 
    /// <summary>
    /// This is called just after the <see cref="AppForm.Context"/> is assigned.
    /// <para>NOTE: When this method is called the form has already a parent, the <see cref="AppForm.Context"/> is assigned buth the <see cref="FormInitialize"/> has not been called. </para>
    /// </summary>
    protected override void Setup()
    {
        Commands = Context.Tag as DefList<Command>;
        if (Commands == null)
            Commands = AppRegistry.MenuCommands;
    }
    /// <summary>
    /// It is called by the OnKeyDown() method. 
    /// <para>Returns true if processes the key</para>
    /// </summary>
    /// <param name="e">The key event arguments.</param>
    /// <returns>True if the key was processed; otherwise, false.</returns>
    protected override bool ProcessKeyDown(KeyEventArgs e)
    {
        return false;
    }
    
    
    // ● construction
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandTreeViewForm"/> class.
    /// </summary>
    public CommandTreeViewForm()
    {
        InitializeComponent();
    }
    
    // ● static
    /// <summary>
    /// Creates a form context for this form.
    /// </summary>
    /// <param name="Tag">The context tag.</param>
    /// <returns>The created form context.</returns>
    static public FormContext CreateFormContext(object Tag = null) => FormContext.Create(typeof(CommandTreeViewForm), FormDisplayMode.TabItem, Caller: null, Tag: Tag);
 
}
