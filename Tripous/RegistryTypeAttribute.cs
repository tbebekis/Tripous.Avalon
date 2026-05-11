namespace Tripous;
 

/// <summary>
/// Marks a type as discoverable by the <see cref="TypeRegistry"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public class RegistryTypeAttribute : Attribute
{
    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public RegistryTypeAttribute()
    {
    }
}