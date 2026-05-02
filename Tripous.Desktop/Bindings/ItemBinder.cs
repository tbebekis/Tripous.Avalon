namespace Tripous.Desktop;

/// <summary>
/// Binds to single (Item) controls, no grids.
/// <para>It watches for <see cref="CurrentRow"/> changes and notifies its controls.</para>
/// </summary>
public class ItemBinder
{
    IRowProvider fRowProvider;
    DataRow fCurrentRow;

    void RowProvider_CurrentRowChanged(object sender, EventArgs ea) => this.CurrentRow = RowProvider.CurrentRow;
 
    // ● construction
    /// <summary>
    /// Constructor.
    /// </summary>
    public ItemBinder()
    {
    }

    // ● public
    /// <summary>
    /// Clears all bindings.
    /// <para>NOTE: Dangerous.</para>
    /// </summary>
    public void Clear() => Bindings.Clear();
    /// <summary>
    /// Sends a notification to its controls that the <see cref="CurrentRow"/> has changed.
    /// </summary>
    public void Refresh()
    {
        Dispatcher.UIThread.Post(() => 
        { 
            foreach (var Binding in Bindings)
                ControlBindingHelper.Refresh(RowProvider, Binding);
            
        }, DispatcherPriority.Background);  
    }
    

    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding Bind(TextBox Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding BindMemo(TextBox Box, string FieldName, FieldDef FieldDef = null)
    {       
        ControlBinding Result = ControlBindingHelper.BindMemo(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding Bind(CheckBox Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding Bind(DatePicker Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding Bind(ComboBox Box, string FieldName, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, Items, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding Bind(ListBox Box, string FieldName, IEnumerable Items, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, Items, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding Bind(NumericUpDown Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.Bind(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding BindLookup(ComboBox Box, string FieldName, FieldDef FieldDef)
    {
        ControlBinding Result = ControlBindingHelper.BindLookup(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding BindLookup(ComboBox Box, string FieldName, string LookupSourceName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.BindLookup(RowProvider, Box, FieldName, LookupSourceName, FieldDef);
        Bindings.Add(Result);
        return  Result;
    }
    /// <summary>
    /// Bind utility.
    /// </summary>
    public ControlBinding BindImage(Image Box, string FieldName, FieldDef FieldDef = null)
    {
        ControlBinding Result = ControlBindingHelper.BindImage(RowProvider, Box, FieldName, FieldDef);
        Bindings.Add(Result);
        return Result;
    }

    // ● properties
    /// <summary>
    /// Provides the <see cref="CurrentRow"/> and change notifications.
    /// </summary>
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
    /// <summary>
    /// The current row.
    /// </summary>
    public DataRow CurrentRow
    {
        get => fCurrentRow;
        protected set
        {
            if (fCurrentRow != value)
            {
                CurrentRowChanging?.Invoke(this, EventArgs.Empty);
                fCurrentRow = value;
                Refresh();
                CurrentRowChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
    /// <summary>
    /// The list of bindings.
    /// </summary>
    public List<ControlBinding> Bindings { get; private set; } = new();
    
    // ● events
    /// <summary>
    /// Occurs when the <see cref="CurrentRow"/> is about to change.
    /// </summary>
    public event EventHandler CurrentRowChanging;
    /// <summary>
    /// Occurs when the <see cref="CurrentRow"/> is changed.
    /// </summary>
    public event EventHandler CurrentRowChanged;
}