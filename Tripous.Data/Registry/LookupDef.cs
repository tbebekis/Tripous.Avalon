namespace Tripous.Data;

/*
public class LookupDef: BaseDef
{
    bool fUseNullItem;
    string fValueField = "Id";
    string fDisplayField = "Name";
    string fSqlText;
    string fConnectionName;

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
    public string ConnectionName  
    {
        get => !string.IsNullOrWhiteSpace(fConnectionName)? fConnectionName: SysConfig.DefaultConnectionName;
        set { if (fConnectionName != value) { fConnectionName = value; NotifyPropertyChanged(nameof(ConnectionName)); } }
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
*/