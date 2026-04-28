namespace Tripous.Data;

public class LookupDef: BaseDef
{
    private bool fUseNullItem;
    private string fValueField = "Id";
    private string fDisplayField = "Name";
    private string fSqlText;

    // ● properties
    public bool UseNullItem
    {
        get => fUseNullItem;
        set
        {
            if (fUseNullItem != value)
            {
                fUseNullItem = value;
                NotifyPropertyChanged(nameof(UseNullItem));
            }
        }
    }
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
    public string DisplayField
    {
        get => fDisplayField;
        set
        {
            if (fDisplayField != value)
            {
                fDisplayField = value;
                NotifyPropertyChanged(nameof(DisplayField));
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