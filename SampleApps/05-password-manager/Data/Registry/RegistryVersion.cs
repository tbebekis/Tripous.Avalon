namespace PasswordManager.Data;

/// <summary>
/// Base class for manual registry version classes.
/// </summary>
public class RegistryVersion
{
    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="RegistryVersion"/> class.
    /// </summary>
    public RegistryVersion()
    {
    }

    // ● public
    /// <summary>
    /// Registers modules.
    /// </summary>
    public virtual void RegisterModules()
    {
    }
    /// <summary>
    /// Registers forms.
    /// </summary>
    public virtual void RegisterForms()
    {
    }
    /// <summary>
    /// Registers lookup sources.
    /// </summary>
    public virtual void RegisterLookupSources()
    {
    }
    /// <summary>
    /// Registers configuration property definitions.
    /// </summary>
    public virtual void RegisterConfigProperties()
    {
    }

    // ● properties
    /// <summary>
    /// Gets the registry version number.
    /// </summary>
    public virtual int VersionNumber => -1;
}
