/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;


/// <summary>
/// Describes the "field" (text box or grid column) of a <see cref="Locator"/>.
/// <para>A field such that associates a column in the data table (the target) to
/// a column in the list table (the source), the table returned by a <see cref="Locator"/> search.</para>
/// <para><b>NOTE: </b> The inherited <see cref="BaseDef.Name"/> is a field name from the source table.</para>
/// </summary>
public class LocatorFieldDef: BaseDef
{
    int fDisplayWidth;
    string fAlias;
    string fTargetField;
    DataFieldType fDataType = DataFieldType.String;
    bool fIsSearchable;
    bool fIsVisible;

    protected override string GetTitleKey() => !string.IsNullOrWhiteSpace(fTitleKey) ? fTitleKey : Alias;
 

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public LocatorFieldDef()
    {
    }

    // ● properties
    /// <summary>
    /// The master definition this instance belongs to.
    /// </summary>
    [JsonIgnore]
    public LocatorDef LocatorDef { get; set; }
    /// <summary>
    /// An alias of this field
    /// </summary>
    public string Alias
    {
        get => !string.IsNullOrWhiteSpace(fAlias) ? fAlias : (LocatorDef != null? SqlHelper.FieldAlias(LocatorDef.SourceTableName, Name): Name);
        set { if (fAlias != value) { fAlias = value; NotifyPropertyChanged(nameof(Alias)); } }
    }
    /// <summary>
    /// The data type of the field. A <see cref="IsSearchable"/> field can only be of type <see cref="DataFieldType.String"/>.
    /// </summary>
    public DataFieldType DataType
    {
        get => fDataType;
        set { if (fDataType != value) { fDataType = value; NotifyPropertyChanged(nameof(DataType)); } }
    }
    /// <summary>
    /// The <see cref="DataColumn.ColumnName"/> of the target data table. It can not be empty for grid-type locators.
    /// </summary>
    public string TargetField 
    {
        get => fTargetField;
        set { if (fTargetField != value) { fTargetField = value; NotifyPropertyChanged(nameof(TargetField)); } }
    }
    /// <summary>
    /// When true the field is visible in the UI, i.e. in a locator control or in a grid.
    /// </summary>
    public bool IsVisible
    {
        get => fIsVisible;
        set { if (fIsVisible != value) { fIsVisible = value; NotifyPropertyChanged(nameof(IsVisible)); } }
    }
    /// <summary>
    /// When true the field can be part in a WHERE clause in a select statement.
    /// </summary>
    public bool IsSearchable
    {
        get => fIsSearchable;
        set { if (fIsSearchable != value) { fIsSearchable = value; NotifyPropertyChanged(nameof(IsSearchable)); } }
    }
    /// <summary>
    /// The width of a box in a locator control
    /// </summary>
    public int DisplayWidth
    {
        get => fDisplayWidth >= 0 ? fDisplayWidth : 0;
        set { if (fDisplayWidth != value) { fDisplayWidth = value; NotifyPropertyChanged(nameof(DisplayWidth)); } }
    }
}