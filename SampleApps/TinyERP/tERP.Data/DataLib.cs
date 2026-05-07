namespace tERP.Data;

static public partial class DataLib
{
    static DbLogListener_tERP LogListener;
    
    static public void Initialize()
    {
        // fake, must be called for the assembly to be loaded in the domain.
        LogListener = new();
    }
}