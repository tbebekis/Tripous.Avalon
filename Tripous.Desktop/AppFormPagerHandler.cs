/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Handles a tab control which displays forms (UserControls) embedded in TabItems.
/// </summary>
public class AppFormPagerHandler
{
    // ● private
    /// <summary>
    /// Returns the form assigned to a tab item.
    /// </summary>
    /// <param name="TabPage">The tab item.</param>
    /// <returns>The assigned form, if any; otherwise, null.</returns>
    AppForm GetForm(TabItem TabPage)
    {
        return (TabPage.Tag is AppForm)? TabPage.Tag as AppForm : null;
    }
    /// <summary>
    /// Handles middle-click close requests on tab items.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The pointer event arguments.</param>
    void TabItem_PointerPressed(object sender, PointerPressedEventArgs e)
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
    /// <summary>
    /// Initializes a new instance of the <see cref="AppFormPagerHandler"/> class.
    /// </summary>
    /// <param name="Pager">The tab control handled by this instance.</param>
    public AppFormPagerHandler(TabControl Pager)
    {
        this.Pager = Pager;
    }
    
    // ● public
    /// <summary>
    /// Finds a tab item by form identifier.
    /// </summary>
    /// <param name="FormId">The form identifier.</param>
    /// <returns>The matching tab item, if any; otherwise, null.</returns>
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
    /// <summary>
    /// Finds an application form by form identifier.
    /// </summary>
    /// <param name="FormId">The form identifier.</param>
    /// <returns>The matching form, if any; otherwise, null.</returns>
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
 
    /// <summary>
    /// Shows an application form in the pager.
    /// </summary>
    /// <param name="Context">The form context.</param>
    /// <returns>The shown form.</returns>
    public AppForm ShowAppForm(FormContext Context)
    {
        if (Context == null)
            throw new TripousArgumentNullException(nameof(Context));

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
    /// <summary>
    /// Shows a data form in the pager.
    /// </summary>
    /// <param name="Context">The data form context.</param>
    /// <returns>The shown data form.</returns>
    public DataForm ShowDataForm(DataFormContext Context) => ShowAppForm(Context) as DataForm;
    /// <summary>
    /// Shows a data form in the pager.
    /// </summary>
    /// <param name="RegistryName">The registry name of the data form.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The shown data form.</returns>
    public DataForm ShowDataForm(string RegistryName, Control Caller = null) => ShowDataForm(DataFormContext.Create(RegistryName, Caller)); 
 
    /// <summary>
    /// Closes a form by form identifier.
    /// </summary>
    /// <param name="FormId">The form identifier.</param>
    public void CloseForm(string FormId)
    {
        AppForm Form = FindAppForm(FormId);
        if (Form != null)
            Form.CloseForm();
    }

    // ● properties
    /// <summary>
    /// Gets the tab control handled by this instance.
    /// </summary>
    public TabControl Pager { get; private set; }
}
