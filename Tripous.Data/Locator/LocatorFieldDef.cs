namespace Tripous.Data;

/// <summary>
/// Describes a field that participates in a <see cref="Data.LocatorDef"/>.
/// </summary>
public class LocatorFieldDef : BaseDef
{
    // ● private
    DataFieldType fDataType = DataFieldType.String;

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorFieldDef()
    {
    }

    // ● properties
    /// <summary>
    /// The locator definition this field belongs to.
    /// </summary>
    [JsonIgnore]
    public LocatorDef LocatorDef { get; set; }
    /// <summary>
    /// The data type of the field.
    /// </summary>
    public DataFieldType DataType
    {
        get => fDataType;
        set { if (fDataType != value) { fDataType = value; NotifyPropertyChanged(nameof(DataType)); } }
    }
}
