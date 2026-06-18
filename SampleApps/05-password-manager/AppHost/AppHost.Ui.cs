namespace PasswordManager;

/// <summary>
/// Contains application UI helpers.
/// </summary>
static public partial class AppHost
{
    // ● static public
    /// <summary>
    /// Shows sidebar pages.
    /// </summary>
    static public void ShowSideBarPages()
    {
        SideBarHandler.ShowAppForm(CommandTreeViewForm.CreateFormContext());
    }
}
