namespace tERP.Data;

public class RegistryVersion
{
    public RegistryVersion()
    {
    }

    public virtual void RegisterModules()
    {
    }
    public virtual void RegisterForms()
    {
    }
    public virtual void RegisterLookups()
    {
    }
    public virtual void RegisterLookupSources()
    {
    }
    public virtual void RegisterLocators()
    {
    }
    public virtual void RegisterCodeProviders()
    {
    }
 

    public virtual int VersionNumber { get;  } = -1;
}