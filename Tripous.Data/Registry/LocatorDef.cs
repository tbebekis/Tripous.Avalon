namespace Tripous.Data;

public class LocatorDef: BaseDef
{
    private string fValueField;
    private string[] fDisplayFields;
    private string[] fSearchableFields;
    private string fSqlText;

    // ● properties
    public string ValueField
    {
        get => fValueField;
        set
        {
            if (fValueField != value)
            {
                fValueField = value;
                NotifyPropertyChanged(nameof(ValueField));
            }
        }
    }
    public string[] DisplayFields
    {
        get => fDisplayFields;
        set
        {
            if (fDisplayFields != value)
            {
                fDisplayFields = value;
                NotifyPropertyChanged(nameof(DisplayFields));
                NotifyPropertyChanged(nameof(SearchableFields));
            }
        }
    }
    public string[] SearchableFields
    {
        get => fSearchableFields != null ? fSearchableFields : DisplayFields;
        set
        {
            if (fSearchableFields != value)
            {
                fSearchableFields = value;
                NotifyPropertyChanged(nameof(SearchableFields));
            }
        }
    }
    public string SqlText
    {
        get => fSqlText;
        set
        {
            if (fSqlText != value)
            {
                fSqlText = value;
                NotifyPropertyChanged(nameof(SqlText));
            }
        }
    }
}