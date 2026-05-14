namespace Tripous;
 

/// <summary>
/// Marks a type as discoverable by the <see cref="TypeStore"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
public class TypeStoreAttribute : Attribute
{
    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public TypeStoreAttribute()
    {
    }
}