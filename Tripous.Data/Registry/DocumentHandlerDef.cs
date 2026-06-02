namespace Tripous.Data;


/// <summary>
/// A document handler definition.
/// </summary>
public class DocumentHandlerDef: BaseDef
{
    string fClassName;
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public DocumentHandlerDef()
    {
    }
    
    // ● properties
    /// <summary>
    /// The class name of the <see cref="System.Type"/> this descriptor describes.
    /// <para>NOTE: The value of this property may be a string returned by the <see cref="Type.AssemblyQualifiedName"/> property of the type. </para>
    /// <para>In that case, it consists of the type name, including its namespace, followed by a comma, followed by the display name of the assembly
    /// the type belongs to. It might looks like the following</para>
    /// <para><c>Tripous.Data.DataModule, Tripous, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</c></para>
    /// <para>Otherwise it can be a full type name <see cref="Type.FullName"/>, e.g. </para>
    /// <para><c>Tripous.Data.DataModule</c></para>
    /// </summary>
    public string ClassName
    {
        get => !string.IsNullOrWhiteSpace(fClassName)? fClassName: typeof(DataModule).FullName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
}