namespace Tripous.Desktop;

/// <summary>
/// Handles a tab control which displays forms (UserControls) embedded in TabItems.
/// </summary>
public class AppFormPagerHandler
{
    private AppForm GetForm(TabItem TabPage)
    {
        return (TabPage.Tag is AppForm)? TabPage.Tag as AppForm : null;
    }
    private void TabItem_PointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(null).Properties.PointerUpdateKind == PointerUpdateKind.MiddleButtonPressed)
        {
            if (sender is TabItem tabItem)
            {
                AppForm Form = GetForm(tabItem);
                if ((Form != null) && Form.ClosableByUser)
                {
                    Form.CloseForm();
                }
            }
        }
    }
    
    // ● construction
    public AppFormPagerHandler(TabControl Pager)
    {
        this.Pager = Pager;
    }
    
    // ● public
    public TabItem FindTabItem(string FormId)
    {
        var TabItems =  Pager.Items.Cast<TabItem>();
        foreach (TabItem Item in TabItems)
        {
            if (Item.Tag is AppForm)
            {
                AppForm  Form = (AppForm)Item.Tag;
                if (Sys.IsSameText(FormId, Form.FormId))
                {
                    return Item;
                }
            }
        }
      
        return null;
    }
    public AppForm FindAppForm(string FormId)
    {
        var TabItems =  Pager.Items.Cast<TabItem>();
        foreach (TabItem Item in TabItems)
        {
            if (Item.Tag is AppForm)
            {
                AppForm Form = (AppForm)Item.Tag;
                if (Sys.IsSameText(FormId, Form.FormId))
                {
                    return Form;
                }
            }
        }
      
        return null;
    }
 
    public AppForm ShowAppForm(FormContext Context)
    {
        if (Context == null)
            throw new ArgumentNullException(nameof(Context));

        AppForm Form = FindAppForm(Context.FormId);
        if (Form == null)
        {
            Context.DisplayMode = FormDisplayMode.TabItem;
            Form = Context.CreateForm();
            
            TabItem TabPage = new TabItem();
            TabPage.Tag = Form;
            TabPage.PointerPressed += TabItem_PointerPressed;
            Pager.Items.Add(TabPage);
            
            Context.ParentControl = TabPage;
            Form.Setup(Context);
        }

        Form.ParentTabControl.SelectedItem = Form.ParentTabPage;
        return Form;
    }
    public DataForm ShowDataForm(DataFormContext Context) => ShowAppForm(Context) as DataForm;
    public DataForm ShowDataForm(string RegistryName, Control Caller = null) => ShowDataForm(DataFormContext.Create(RegistryName, Caller)); 
 
    public void CloseForm(string FormId)
    {
        AppForm Form = FindAppForm(FormId);
        if (Form != null)
            Form.CloseForm();
    }

    public TabControl Pager { get; private set; }
}