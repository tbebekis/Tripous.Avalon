namespace TestApp;

static public partial class AppHost
{
    static public void Initialize(AppFormPagerHandler SideBarHandler, AppFormPagerHandler ContentHandler)
    {
        if (AppHost.SideBarHandler == null)
        {
            //TestData.Initialize(500);
            
            AppHost.SideBarHandler = SideBarHandler;
            AppHost.ContentHandler = ContentHandler;

            //RegisterDescriptors();

            //Links.Data = new TestDataLink();
        }
    }

    static public AppFormPagerHandler SideBarHandler { get; private set; } // pagerSideBar
    static public AppFormPagerHandler ContentHandler { get; private set; } // pagerContent
    
     
}