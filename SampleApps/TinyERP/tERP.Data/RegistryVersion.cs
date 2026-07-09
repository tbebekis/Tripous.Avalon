namespace tERP.Data;

/// <summary>
/// Base class for generated registry version classes.
/// </summary>
public class RegistryVersion
{
    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public RegistryVersion()
    {
    }

    // ● public
    /// <summary>
    /// Registers data modules.
    /// </summary>
    public virtual void RegisterModules()
    {
    }
    /// <summary>
    /// Registers module FactBoxes.
    /// </summary>
    public virtual void RegisterFactBoxes()
    {
    }
    /// <summary>
    /// Registers data forms.
    /// </summary>
    public virtual void RegisterForms()
    {
    }
    /// <summary>
    /// Registers lookups.
    /// </summary>
    public virtual void RegisterLookups()
    {
    }
    /// <summary>
    /// Registers lookup sources.
    /// </summary>
    public virtual void RegisterLookupSources()
    {
    }
    /// <summary>
    /// Registers locators.
    /// </summary>
    public virtual void RegisterLocators()
    {
    }
    /// <summary>
    /// Registers code providers.
    /// </summary>
    public virtual void RegisterCodeProviders()
    {
    }
 

    // ● properties
    /// <summary>
    /// Registry version number.
    /// </summary>
    public virtual int VersionNumber { get;  } = -1;
}
