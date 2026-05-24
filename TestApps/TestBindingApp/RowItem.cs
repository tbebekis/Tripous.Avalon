namespace TestBindingApp;

public class RowItem: INotifyPropertyChanged
{
    // ● private
    DataRowView fRowView;

    object GetRowValue(string FieldName)
    {
        object Result = RowView[FieldName];
        return Result == DBNull.Value ? null : Result;
    }
    void SetRowValue(string FieldName, object Value)
    {
        RowView[FieldName] = Value ?? DBNull.Value;
    }
    void RowView_PropertyChanged(object Sender, PropertyChangedEventArgs e)
    {
        Tests.Log("RowItem.RowView.PropertyChanged: " + e.PropertyName);
        OnPropertyChanged(e.PropertyName);
        OnPropertyChanged("Item[]");
        OnPropertyChanged("Item[" + e.PropertyName + "]");
    }
    void OnPropertyChanged(string PropertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    // ● construction
    public RowItem(DataRowView RowView)
    {
        fRowView = RowView;
        fRowView.PropertyChanged += RowView_PropertyChanged;
    }

    // ● public
    public object GetValue(string FieldName)
    {
        return GetRowValue(FieldName);
    }
    public void SetValue(string FieldName, object Value)
    {
        SetRowValue(FieldName, Value);
    }

    // ● properties
    public DataRowView RowView => fRowView;
    public DataRow Row => fRowView.Row;
    public object Id
    {
        get => GetRowValue("Id");
        set => SetRowValue("Id", value);
    }
    public object Code
    {
        get => GetRowValue("Code");
        set => SetRowValue("Code", value);
    }
    public object Name
    {
        get => GetRowValue("Name");
        set => SetRowValue("Name", value);
    }
    public object CountryId
    {
        get => GetRowValue("CountryId");
        set => SetRowValue("CountryId", value);
    }
    public object Country__Code
    {
        get => GetRowValue("Country__Code");
        set => SetRowValue("Country__Code", value);
    }
    public object Country__Name
    {
        get => GetRowValue("Country__Name");
        set => SetRowValue("Country__Name", value);
    }
    public object IsActive
    {
        get => GetRowValue("IsActive");
        set => SetRowValue("IsActive", value);
    }
    public object this[string FieldName]
    {
        get => GetRowValue(FieldName);
        set => SetRowValue(FieldName, value);
    }

    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
}
