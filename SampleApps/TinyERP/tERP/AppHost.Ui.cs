namespace tERP;

static public partial class AppHost
{
    static public void ShowSideBarPages()
    {
        SideBarHandler.ShowAppForm(CommandTreeViewForm.CreateFormContext());
    }
}