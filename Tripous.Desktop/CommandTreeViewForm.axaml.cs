namespace Tripous.Desktop;

public partial class CommandTreeViewForm : AppForm
{
    DefList<Command> Commands; 

    // ● private
    void CreateTreeViewNodes()
    {
        // ----------------------------------------------------------------------
        TreeViewItem CreateFolderNode(string Caption, string IconFile = "folder16.png")
        {
            var Panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
            Panel.Children.Add(Assets.FindImage16(IconFile));
            Panel.Children.Add(new TextBlock { Text = Caption, FontWeight = FontWeight.SemiBold  });
            
            var Node = new TreeViewItem { Header = Panel };
            return Node;
        }
        // ----------------------------------------------------------------------
        TreeViewItem CreateLeafNode(string Caption, object Tag = null, string IconFile = "item16.png")
        {
            var Panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5}; // , Margin = new Thickness(-40, 0, 0, 0)
            Panel.Children.Add(Assets.FindImage16(IconFile));
            Panel.Children.Add(new TextBlock { Text = Caption });
            
            var Node = new TreeViewItem { Header = Panel, Tag = Tag };
            return Node;
        }
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
    
    // ● overrides
    /// <summary>
    /// Called in order to initialize the form
    /// </summary>
    protected override void FormInitialize()
    {
        TitleText = "Commands";
        CreateTreeViewNodes();
    }
 
    /// <summary>
    /// This is called just after the <see cref="Context"/> is assigned.
    /// <para>NOTE: When this method is called the form has already a parent, the <see cref="Context"/> is assigned buth the <see cref="FormInitialize"/> has not been called. </para>
    /// </summary>
    protected override void Setup()
    {
        Commands = Context.Tag as DefList<Command>;
        if (Commands == null)
            Commands = AppRegistry.MenuCommands;
    }
    
    // ● construction
    public CommandTreeViewForm()
    {
        InitializeComponent();
    }
    
    // ● static
    static public FormContext CreateFormContext(object Tag = null) => FormContext.Create(typeof(CommandTreeViewForm), FormDisplayMode.TabItem, Caller: null, Tag: Tag);
 
}