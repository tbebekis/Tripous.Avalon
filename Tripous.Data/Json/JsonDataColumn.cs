/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// JSON contract for a data column and its field metadata used by Tripous Web.
/// </summary>
public class JsonDataColumn
{
    // ● private
    static FieldDef GetFieldDef(DataColumn Source)
    {
        if (Source != null && Source.ExtendedProperties.ContainsKey("Descriptor"))
            return Source.ExtendedProperties["Descriptor"] as FieldDef;
        return null;
    }
    static DataFieldType GetDataType(DataColumn Source, FieldDef FieldDef)
    {
        if (FieldDef != null)
            return FieldDef.DataType;
        return Source != null ? Source.DataType.GetDataFieldType() : DataFieldType.String;
    }
    static DataColumnType GetColumnType(DataFieldType DataType, FieldFlags Flags)
    {
        if (Flags.HasFlag(FieldFlags.Boolean) || DataType == DataFieldType.Boolean)
            return DataColumnType.Boolean;
        if (Flags.HasFlag(FieldFlags.Image) || Flags.HasFlag(FieldFlags.ImagePath))
            return DataColumnType.Image;
        if (Flags.HasFlag(FieldFlags.Memo) || Flags.HasFlag(FieldFlags.LargeMemo) || DataType == DataFieldType.TextBlob)
            return DataColumnType.Memo;
        if (DataType == DataFieldType.Date)
            return DataColumnType.Date;
        if (DataType == DataFieldType.DateTime)
            return DataColumnType.DateTime;
        if (DataType == DataFieldType.Integer)
            return DataColumnType.Integer;
        if (DataType.IsFloat())
            return DataColumnType.Decimal;
        if (DataType == DataFieldType.String)
            return DataColumnType.Text;
        return DataColumnType.None;
    }
    static object GetDefaultValue(DataColumn Source, FieldDef FieldDef)
    {
        if (FieldDef != null)
            return FieldDef.DefaultValue == Sys.NULL ? null : FieldDef.DefaultValue;
        if (Source != null && !Sys.IsNull(Source.DefaultValue))
            return Source.DefaultValue;
        return null;
    }

    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataColumn()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataColumn(DataColumn Source)
        : this(Source, GetFieldDef(Source))
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    public JsonDataColumn(DataColumn Source, FieldDef FieldDef)
    {
        DataFieldType DataFieldType = GetDataType(Source, FieldDef);
        FieldFlags FieldFlagsValue = FieldDef != null ? FieldDef.Flags : FieldFlags.None;

        Name = Source != null ? Source.ColumnName : FieldDef != null ? FieldDef.Name : string.Empty;
        Alias = FieldDef != null ? FieldDef.Alias : string.Empty;
        Title = FieldDef != null ? FieldDef.Title : Source != null ? Source.Caption : Name;
        TitleKey = FieldDef != null ? FieldDef.TitleKey : string.Empty;
        DataType = (int)DataFieldType;
        Expression = FieldDef != null ? FieldDef.Expression : Source != null ? Source.Expression : string.Empty;
        DefaultValue = GetDefaultValue(Source, FieldDef);
        MaxLength = FieldDef != null ? FieldDef.MaxLength : Source != null ? Source.MaxLength : -1;
        Decimals = FieldDef != null ? FieldDef.Decimals : -1;
        Unique = Source != null && Source.Unique;
        Flags = (int)FieldFlagsValue;
        ColumnType = (int)GetColumnType(DataFieldType, FieldFlagsValue);
        DisplayFormat = FieldDef != null ? FieldDef.DisplayFormat : string.Empty;
        EditFormat = FieldDef != null ? FieldDef.EditFormat : string.Empty;
        DisplayWidth = FieldDef != null ? FieldDef.DisplayWidth : 0;
        LookupSource = FieldDef != null ? FieldDef.LookupSource : string.Empty;
        Locator = FieldDef != null ? FieldDef.Locator : string.Empty;
        CodeProvider = FieldDef != null ? FieldDef.CodeProvider : string.Empty;
        SnapshotOf = FieldDef != null ? FieldDef.SnapshotOf : string.Empty;
        Group = FieldDef != null ? FieldDef.Group : Sys.GENERAL;
        ToolTip = FieldDef != null ? FieldDef.ToolTip : string.Empty;
    }

    // ● public
    /// <summary>
    /// Returns a string representation of this instance.
    /// </summary>
    public override string ToString() => !string.IsNullOrWhiteSpace(Name)? Name: base.ToString();
    
    // ● properties
    /// <summary>
    /// The column name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The column alias.
    /// </summary>
    public string Alias { get; set; } = string.Empty;
    /// <summary>
    /// The column title.
    /// </summary>
    public string Title { get; set; }
    /// <summary>
    /// The column title localization key.
    /// </summary>
    public string TitleKey { get; set; } = string.Empty;
    /// <summary>
    /// The data type as a <see cref="DataFieldType"/> integer value.
    /// </summary>
    public int DataType { get; set; } = (int)DataFieldType.String;
    /// <summary>
    /// The column expression.
    /// </summary>
    public string Expression { get; set; } = string.Empty;
    /// <summary>
    /// The default value.
    /// </summary>
    public object DefaultValue { get; set; }
    /// <summary>
    /// The maximum string length. -1 means not set.
    /// </summary>
    public int MaxLength { get; set; } = -1;
    /// <summary>
    /// The decimal digits count. -1 means not set.
    /// </summary>
    public int Decimals { get; set; } = -1;
    /// <summary>
    /// True when values must be unique.
    /// </summary>
    public bool Unique { get; set; }
    /// <summary>
    /// The field flags as a <see cref="FieldFlags"/> integer value.
    /// </summary>
    public int Flags { get; set; }
    /// <summary>
    /// The column type as a <see cref="DataColumnType"/> integer value.
    /// </summary>
    public int ColumnType { get; set; }
    /// <summary>
    /// The display format.
    /// </summary>
    public string DisplayFormat { get; set; } = string.Empty;
    /// <summary>
    /// The edit format.
    /// </summary>
    public string EditFormat { get; set; } = string.Empty;
    /// <summary>
    /// The display width in pixels. 0 means not set.
    /// </summary>
    public int DisplayWidth { get; set; }
    /// <summary>
    /// The lookup source name.
    /// </summary>
    public string LookupSource { get; set; } = string.Empty;
    /// <summary>
    /// The locator name.
    /// </summary>
    public string Locator { get; set; } = string.Empty;
    /// <summary>
    /// The code provider name.
    /// </summary>
    public string CodeProvider { get; set; } = string.Empty;
    /// <summary>
    /// The source field whose value is stored as a snapshot in this column.
    /// </summary>
    public string SnapshotOf { get; set; } = string.Empty;
    /// <summary>
    /// The display group.
    /// </summary>
    public string Group { get; set; } = Sys.GENERAL;
    /// <summary>
    /// The tooltip text.
    /// </summary>
    public string ToolTip { get; set; } = string.Empty;
}
