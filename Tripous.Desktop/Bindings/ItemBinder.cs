namespace Tripous.Desktop;

public class ItemBinder
{
    IRowProvider fRowProvider;

    void RowProvider_CurrentRowChanged(object sender, EventArgs ea)
    {
        Refresh();
    }

    // ● construction
    public ItemBinder()
    {
    }

    // ● public
    public void Clear() => Bindings.Clear();
    public void Refresh()
    {
        Dispatcher.UIThread.Post(() => 
        {  
            DataRow Row = RowProvider.CurrentRow;
            LogBox.AppendLine("BINDER");
            LogBox.AppendLine(Row);
            
            foreach (var Binding in Bindings)
                ControlBindingHelper.Refresh(RowProvider, Binding);
            
        }, DispatcherPriority.Background);  
    }
    
    public ControlBinding Bind(TextBox Box, FieldDef FieldDef)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding Bind(TextBox Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding BindMemo(TextBox Box, string FieldName, FieldDef FieldDef = null)
    {       
        ControlBinding Result = ControlBindingHelper.BindMemo(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding Bind(CheckBox Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding Bind(DatePicker Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding Bind(ComboBox Box, string FieldName, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, Items, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding Bind(ListBox Box, string FieldName, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, Items, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding Bind(NumericUpDown Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding BindLookup(ComboBox Box, string FieldName, FieldDef FieldDef)
    {
        ControlBinding Result = ControlBindingHelper.BindLookup(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    public ControlBinding BindLookup(ComboBox Box, string FieldName, string LookupSourceName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.BindLookup(RowProvider, Box, FieldName, LookupSourceName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }

    public IRowProvider RowProvider
    {
        get => fRowProvider;
        set
        {
            if (fRowProvider != null)
                fRowProvider.CurrentRowChanged -= RowProvider_CurrentRowChanged;
            fRowProvider = value;
            if (fRowProvider != null)
                fRowProvider.CurrentRowChanged += RowProvider_CurrentRowChanged;
        }
    }
    public List<ControlBinding> Bindings { get; private set; } = new();

}