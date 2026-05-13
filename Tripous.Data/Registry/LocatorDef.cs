namespace Tripous.Data;

/// <summary>
/// Defines a locator, i.e. a searchable selector for large reference tables.
/// </summary>
public class LocatorDef: BaseDef
{
    // ● private fields
    string fTableName;
    string fKeyField = "Id";
    string[] fDisplayFields = [];
    string[] fSearchFields = [];
    string[] fReturnFields = [];

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public LocatorDef()
    {
    }

    // ● public methods
    /// <summary>
    /// Throws an exception if this descriptor is not fully defined
    /// </summary>
    public override void CheckDescriptor()
    {
        base.CheckDescriptor();

        if (string.IsNullOrWhiteSpace(TableName))
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no TableName.");

        if (string.IsNullOrWhiteSpace(KeyField))
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no KeyField.");

        if (DisplayFields.Length == 0)
            throw new TripousDataException($"{nameof(LocatorDef)} {Name} has no DisplayFields.");
    }
    /// <summary>
    /// Generates the SELECT statement for the locator.
    /// </summary>
    public string GenerateLocatorSelectSql(string TableAlias = "")
    {
        string Alias = !string.IsNullOrWhiteSpace(TableAlias)
            ? TableAlias
            : TableName;

        List<string> Lines = [];

        foreach (string FieldName in ReturnFields)
            Lines.Add($"   {Alias}.{FieldName}");

        StringBuilder SB = new();

        SB.AppendLine("select");

        for (int i = 0; i < Lines.Count; i++)
        {
            string Line = Lines[i];

            if (i < Lines.Count - 1)
                Line += ",";

            SB.AppendLine(Line);
        }

        SB.AppendLine("from");
        SB.AppendLine($"   {TableName} {Alias}");

        return SB.ToString();
    }

    // ● properties
    /// <summary>
    /// The locator table
    /// </summary>
    public string TableName
    {
        get => fTableName;
        set
        {
            if (fTableName != value)
            {
                fTableName = value;
                NotifyPropertyChanged(nameof(TableName));
            }
        }
    }
    /// <summary>
    /// The primary key field of the locator table
    /// </summary>
    public string KeyField
    {
        get => fKeyField;
        set
        {
            if (fKeyField != value)
            {
                fKeyField = value;
                NotifyPropertyChanged(nameof(KeyField));
            }
        }
    }
    /// <summary>
    /// Fields should be displayed by the UI.
    /// </summary>
    public string[] DisplayFields
    {
        get => fDisplayFields;
        set
        {
            if (fDisplayFields != value)
            {
                fDisplayFields = value ?? [];
                NotifyPropertyChanged(nameof(DisplayFields));
            }
        }
    }
    /// <summary>
    /// <see cref="DisplayFields"/> that are searchable.
    /// </summary>
    public string[] SearchFields
    {
        get => fSearchFields.Length > 0
            ? fSearchFields
            : DisplayFields;

        set
        {
            if (fSearchFields != value)
            {
                fSearchFields = value ?? [];
                NotifyPropertyChanged(nameof(SearchFields));
            }
        }
    }
    /// <summary>
    /// Fields that the locator should return.
    /// </summary>
    public string[] ReturnFields
    {
        get => fReturnFields.Length > 0
            ? fReturnFields
            : DisplayFields;

        set
        {
            if (fReturnFields != value)
            {
                fReturnFields = value ?? [];
                NotifyPropertyChanged(nameof(ReturnFields));
            }
        }
    }
}
 