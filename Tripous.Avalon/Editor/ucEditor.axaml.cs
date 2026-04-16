using Avalonia.LogicalTree;

namespace Tripous.Avalon;

public partial class ucEditor : UserControl
{
    private HighlightMode fHighlightMode = HighlightMode.None;
    
    // ● private
    void AfterCreate()
    {
        ToolBar = new();
        ToolBar.Panel = pnlToolBar;
    }
    
    // ● constructor
    public ucEditor()
    {
        InitializeComponent();
        
        if (!Design.IsDesignMode)
            AfterCreate();
    }
    
    // ● public
    public void Clear() => Editor.Clear();
    public void SetFocus()
    {
        Editor.Focus();
        Editor.TextArea.Focus();
    }
    
    public void CloseParentTabPage()
    {
        var Page = this.FindLogicalAncestorOfType<TabItem>();
        if (Page != null)
        {
            var Pager = Page.FindLogicalAncestorOfType<TabControl>();
            if (Pager != null)
                Pager.Items.Remove(Page);
        }
    }
    
    // ● properties
    public TextEditor Editor => TextEditor;
    public string EditorText
    {
        get => Editor.Text != null? Editor.Text.Trim() : string.Empty;
        set => Editor.Text = value;
    }
    public HighlightMode HighlightMode
    {
        get => fHighlightMode;
        set
        {
            if (fHighlightMode != value)
            {
                fHighlightMode = value;
                Editor.SyntaxHighlighting = Highlighters.Find(value);
            }
        }
    }
    public ToolBar ToolBar { get; private set; }
 
}