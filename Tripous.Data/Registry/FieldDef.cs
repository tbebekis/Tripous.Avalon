namespace Tripous.Data;
 
/// <summary>
/// A field definition
/// </summary>
public class FieldDef: BaseDef
{
    string fDisplayFormat;
    string fEditFormat;
    int fSize;
    int fDisplayWidth;
    string fAlias;
    string fExpression;
    DataFieldType fDataType = DataFieldType.String;
    FieldFlags fFlags;
    string fLookupSource;
    string fLocator;
    string fDefaultValue = Sys.NULL;
    string fGroup;
    int fDecimals = -1;
    string fCodeProviderName;

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public FieldDef()
    {
    }
    /// <summary>
    /// Constructor
    /// </summary>
    public FieldDef(TableDef TableDef)
    {
        this.TableDef = TableDef;
    }

    // ● public
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString()
    {
        return Name;
    }
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(this.Alias))
            Sys.Throw(Texts.GS($"E_{typeof(FieldDef)}_TextIsEmpty", $"{typeof(FieldDef)} Alias  is empty. "));

        if (this.DataType == DataFieldType.None)
            Sys.Throw(Texts.GS($"E_{typeof(FieldDef)}_DataTypeIsEmpty", $"{typeof(FieldDef)} DataType is Unknown. "));
         
    }

    // ● for fluent syntax 
    /// <summary>
    /// Sets the <see cref="Alias"/> and returns this instance.
    /// </summary>
    public FieldDef SetAlias(string Value)
    {
        this.Alias = Value;
        return this;
    }

    /// <summary>
    /// Sets the <see cref="BaseDef.TitleKey"/> and returns this instance.
    /// </summary>
    public FieldDef SetTitleKey(string Value)
    {
        this.TitleKey = Value;
        return this;
    }
    /// <summary>
    /// Sets the <see cref="MaxLength"/> and returns this instance.
    /// </summary>
    public FieldDef SetMaxLength(int Value)
    {
        this.MaxLength = Value;
        return this;
    }
    /// <summary>
    /// Sets the <see cref="Decimals"/> and returns this instance.
    /// </summary>
    public FieldDef SetDecimals(int Value)
    {
        this.Decimals = Value;
        return this;
    }
    /// <summary>
    /// Sets the <see cref="Flags"/> and returns this instance.
    /// </summary>
    public FieldDef SetFlags(FieldFlags Value)
    { 
        this.Flags = Value;
        return this;
    }
    /// <summary>
    /// Adds the <see cref="Flags"/> and returns this instance.
    /// </summary>
    public FieldDef AddFlags(FieldFlags Value)
    { 
        this.Flags |= Value;
        return this;
    }
    /// <summary>
    /// Sets the <see cref="CodeProviderName"/> and returns this instance.
    /// </summary>
    public FieldDef SetCodeProviderName(string Value)
    {
        this.CodeProviderName = Value;
        return this;
    }
    /// <summary>
    /// Sets the <see cref="DefaultValue"/> and returns this instance.
    /// </summary>
    public FieldDef SetDefaultValue(string Value)
    {
        this.DefaultValue = Value;
        return this;
    }
    /// <summary>
    /// Sets the <see cref="Expression"/> and returns this instance.
    /// </summary>
    public FieldDef SetExpression(string Value)
    {
        this.Expression = Value;
        return this;
    }
 
    
    // ● properties
    /// <summary>
    /// The master definition this instance belongs to.
    /// </summary>
    [JsonIgnore]
    public TableDef TableDef { get; set; }
    /// <summary>
    /// An alias of this field
    /// </summary>
    public string Alias
    {
        get => !string.IsNullOrWhiteSpace(fAlias)? fAlias: Name;
        set { if (fAlias != value) { fAlias = value; NotifyPropertyChanged(nameof(Alias)); } }
    }
    public DataFieldType DataType
    {
        get => fDataType;
        set { if (fDataType != value) { fDataType = value; NotifyPropertyChanged(nameof(DataType)); } }
    }
    /// <summary>
    /// The group the field should be displayed into, e.g. General, Address, Billing, etc.
    /// </summary>
    public string Group
    {
        get => !string.IsNullOrWhiteSpace(fGroup)? fGroup: Sys.GENERAL;
        set { if (fGroup != value) { fGroup = value; NotifyPropertyChanged(nameof(Group)); } }
    }
    public int MaxLength
    {
        get => fSize > 0 ? fSize : -1;
        set { if (fSize != value) { fSize = value; NotifyPropertyChanged(nameof(MaxLength)); } }
    }
    public FieldFlags Flags
    {
        get => fFlags;
        set { if (fFlags != value) { fFlags = value; NotifyPropertyChanged(nameof(Flags)); } }
    }
    public string DefaultValue
    {
        get => string.IsNullOrEmpty(fDefaultValue) ? Sys.NULL : fDefaultValue;
        set { if (fDefaultValue != value) { fDefaultValue = value; NotifyPropertyChanged(nameof(DefaultValue)); } }
    }
    public string Expression
    {
        get => string.IsNullOrEmpty(fExpression) ? string.Empty : fExpression;
        set { if (fExpression != value) { fExpression = value; NotifyPropertyChanged(nameof(Expression)); } }
    }
    public string DisplayFormat
    {
        get => !string.IsNullOrWhiteSpace(fDisplayFormat) ? fDisplayFormat :
            IsNumeric ? "N2" : IsDateTime ? "yyyy-MM-dd HH:mm" : string.Empty;
        set { if (fDisplayFormat != value) { fDisplayFormat = value; NotifyPropertyChanged(nameof(DisplayFormat)); } }
    }
    public string EditFormat
    {
        get => !string.IsNullOrWhiteSpace(fEditFormat) ? fEditFormat :
               IsDateTime ? "yyyy-MM-dd HH:mm" : string.Empty;
        set { if (fEditFormat != value) { fEditFormat = value; NotifyPropertyChanged(nameof(EditFormat)); } }
    }
    public int DisplayWidth
    {
        get => fDisplayWidth >= 0 ? fDisplayWidth : 0;
        set { if (fDisplayWidth != value) { fDisplayWidth = value; NotifyPropertyChanged(nameof(DisplayWidth)); } }
    }
    public string LookupSource
    {
        get => fLookupSource;
        set { if (fLookupSource != value) { fLookupSource = value; NotifyPropertyChanged(nameof(LookupSource)); } }
    }
    public string Locator
    {
        get => fLocator;
        set { if (fLocator != value) { fLocator = value; NotifyPropertyChanged(nameof(Locator)); } }
    }
    /// <summary>
    /// Gets or sets the decimals of the field. Used when is a float field. -1 means is not set.
    /// </summary>
    public int Decimals
    {
        get => fDecimals;
        set { if (fDecimals != value) { fDecimals = value; NotifyPropertyChanged(nameof(Decimals)); } }
    }
    /// <summary>
    /// Gets or sets the Name of the code producer descriptor associated to this field.
    /// </summary>
    public string CodeProviderName 
    {
        get => fCodeProviderName;
        set { if (fCodeProviderName != value) { fCodeProviderName = value; NotifyPropertyChanged(nameof(CodeProviderName)); } }
    }

    /// <summary>
    /// Returns true when the Required flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsRequired => (FieldFlags.Required & Flags) == FieldFlags.Required;
    /// <summary>
    /// Returns true when the IsHidden flag is NOT set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsVisible => (FieldFlags.Visible & Flags) == FieldFlags.Visible;
    /// <summary>
    /// Returns true when the IsHidden flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsHidden => !IsVisible;
    /// <summary>
    /// Returns true when the ReadOnly flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsReadOnly => (FieldFlags.ReadOnly & Flags) == FieldFlags.ReadOnly;
    /// <summary>
    /// Returns true when the ReadOnlyUI flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsReadOnlyUI => (FieldFlags.ReadOnlyUI & Flags) == FieldFlags.ReadOnlyUI;
    /// <summary>
    /// Returns true when the ReadOnlyEdit flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsReadOnlyEdit => (FieldFlags.ReadOnlyEdit & Flags) == FieldFlags.ReadOnlyEdit;
    [JsonIgnore] public bool IsNumeric => DataType.IsNumeric();
    [JsonIgnore] public bool IsInteger => DataType == DataFieldType.Integer;
    [JsonIgnore] public bool IsFloat => DataType.IsFloat();
    [JsonIgnore] public bool IsDateTime => DataType.IsDateTime();
    /// <summary>
    /// Returns true when the Boolean flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsBoolean => Flags.HasFlag(FieldFlags.Boolean) || DataType == DataFieldType.Boolean;
    /// <summary>
    /// Returns true when the Memo flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsMemo => (FieldFlags.Memo & Flags) == FieldFlags.Memo;
    [JsonIgnore] public bool IsBlob => DataType.IsBlob();
    /// <summary>
    /// Returns true when the Image flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsImage => (FieldFlags.Image & Flags) == FieldFlags.Image;
    /// <summary>
    /// Returns true when the ImagePath flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsImagePath => (FieldFlags.ImagePath & Flags) == FieldFlags.ImagePath;
    /// <summary>
    /// Returns true when the Searchable flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsSearchable => (FieldFlags.Searchable & Flags) == FieldFlags.Searchable;
    /// <summary>
    /// Returns true when the Extra flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsExtraField => (FieldFlags.Extra & Flags) == FieldFlags.Extra;
    /// <summary>
    /// Returns true when the Extra flag is NOT set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsNativeField => !IsExtraField;
    /// <summary>
    /// Returns true when the ForeignKey flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsForeignKeyField => (FieldFlags.ForeignKey & Flags) == FieldFlags.ForeignKey;
    /// <summary>
    /// Returns true when the NoInsertUpdate flag is set in Flags.
    /// </summary>
    [JsonIgnore] public bool IsNoInsertOrUpdate => (FieldFlags.NoInsertUpdate & Flags) == FieldFlags.NoInsertUpdate;
    
    [JsonIgnore] public bool IsBindable => Flags.HasFlag(FieldFlags.Visible) && !DataType.In(DataFieldType.None | DataFieldType.Blob);
    [JsonIgnore] public bool IsLookup => !string.IsNullOrWhiteSpace(LookupSource);
}