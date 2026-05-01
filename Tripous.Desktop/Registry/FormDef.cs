namespace Tripous.Desktop;


/// <summary>
/// Describes a form
/// </summary>
public class FormDef: BaseDef
{
    string fClassName = typeof(DataForm).FullName;
    string fItemClassName = typeof(ItemPage).FullName;
    string fModule;
    bool fIsReadOnly;
    
    // ● public
    /// <summary>
    /// Creates a form instance as described by this instance.
    /// </summary>
    /// <returns></returns>
    public DataForm Create() => TypeResolver.CreateInstance<DataForm>(ClassName);

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
        get => !string.IsNullOrWhiteSpace(fClassName)? fClassName: typeof(DataForm).FullName;
        set { if (fClassName != value) { fClassName = value; NotifyPropertyChanged(nameof(ClassName)); } }
    }
    /// <summary>
    /// The class name of the <see cref="System.Type"/> of the item part user control.
    /// <para>NOTE: The value of this property may be a string returned by the <see cref="Type.AssemblyQualifiedName"/> property of the type. </para>
    /// <para>In that case, it consists of the type name, including its namespace, followed by a comma, followed by the display name of the assembly
    /// the type belongs to. It might looks like the following</para>
    /// <para><c>Tripous.Data.DataModule, Tripous, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null</c></para>
    /// <para>Otherwise it can be a full type name <see cref="Type.FullName"/>, e.g. </para>
    /// <para><c>Tripous.Data.DataModule</c></para>
    /// </summary>
    public string ItemClassName
    {
        get => !string.IsNullOrWhiteSpace(fItemClassName)? fItemClassName: typeof(ItemPage).FullName;
        set { if (fItemClassName != value) { fItemClassName = value; NotifyPropertyChanged(nameof(ItemClassName)); } }
    }
    /// <summary>
    /// The registration name of the module this form uses.
    /// </summary>
    public string Module
    {
        get => !string.IsNullOrWhiteSpace(fModule) ? fModule : Name;
        set { if (fModule != value) { fModule = value; NotifyPropertyChanged(nameof(Module)); } }
    }
    /// <summary>
    /// When true then no edits are allowed.
    /// </summary>
    public bool IsReadOnly 
    {
        get => fIsReadOnly;
        set { if (fIsReadOnly != value) { fIsReadOnly = value; NotifyPropertyChanged(nameof(IsReadOnly)); } }
    }
}

 