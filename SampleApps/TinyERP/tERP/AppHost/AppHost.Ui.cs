namespace tERP;

static internal partial class AppHost
{
    static public void ShowSideBarPages()
    {
        SideBarHandler.ShowAppForm(CommandTreeViewForm.CreateFormContext());
    }
}