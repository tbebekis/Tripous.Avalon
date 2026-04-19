using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
 

namespace Tripous.Avalon;
using Tripous.Data;


/// <summary>
/// Holds binding metadata for a control
/// </summary>
public class ControlBinding
{
    // ● private
    Control fControl;
    string fColumnName;
    ILookupSource fLookupSource;
    string fLookupSourceName;
    string fDisplayMember;
    string fValueMember;
    EventHandler<SelectionChangedEventArgs> fSelectionChangedHandler;
    bool fIsRefreshing;
    bool? fAllowNullSelection;
    string fNullText;
    object fLookupNullItem;
    List<object> fLookupItems = new();

    // ● constructor
    public ControlBinding(Control Control, string ColumnName)
    {
        fControl = Control;
        fColumnName = ColumnName;
    }

    // ● public methods
    public override string ToString()
    {
        return $"{Control?.GetType().Name}: {ColumnName}";
    }
    public void ClearLookupItems()
    {
        fLookupItems.Clear();
    }
    public void AddLookupItem(object Item)
    {
        fLookupItems.Add(Item);
    }
    public IEnumerable GetLookupItems()
    {
        return fLookupItems != null && fLookupItems.Count > 0 ? fLookupItems : null;
    }
    
    // ● properties
    public Control Control => fControl;
    public string ColumnName => fColumnName;
    public ILookupSource LookupSource
    {
        get => fLookupSource;
        set => fLookupSource = value;
    }
    public string LookupSourceName
    {
        get => fLookupSourceName;
        set => fLookupSourceName = value;
    }
    public string DisplayMember
    {
        get => fDisplayMember;
        set => fDisplayMember = value;
    }
    public string ValueMember
    {
        get => fValueMember;
        set => fValueMember = value;
    }
    public EventHandler<SelectionChangedEventArgs> SelectionChangedHandler
    {
        get => fSelectionChangedHandler;
        set => fSelectionChangedHandler = value;
    }
    public bool IsRefreshing
    {
        get => fIsRefreshing;
        set => fIsRefreshing = value;
    }
    public bool IsLookup => LookupSource != null;
    public bool? AllowNullSelection
    {
        get => fAllowNullSelection;
        set => fAllowNullSelection = value;
    }
    public string NullText
    {
        get => fNullText;
        set => fNullText = value;
    }
    public object LookupNullItem
    {
        get => fLookupNullItem;
        set => fLookupNullItem = value;
    }
}

/// <summary>
/// Synthetic lookup item representing a null value
/// </summary>
sealed public class LookupNullItem
{
    // ● constructor
    public LookupNullItem(string Text)
    {
        fText = Text;
    }

    // ● private
    string fText;

    // ● public methods
    public override string ToString()
    {
        return fText;
    }

    // ● properties
    public string Text => fText;
}


/// <summary>
/// Lookup helper methods
/// </summary>
static internal class DataBinderLookupHelper
{
    // ● private methods
    static private bool AreEqual(object A, object B)
    {
        if (A == null && B == null)
            return true;
        if (A == null || B == null)
            return false;
        if (Equals(A, B))
            return true;

        try
        {
            string SA = Convert.ToString(A, CultureInfo.InvariantCulture);
            string SB = Convert.ToString(B, CultureInfo.InvariantCulture);
            return string.Equals(SA, SB, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
        }

        return false;
    }
    static private object GetMemberValue(object Item, string MemberName)
    {
        if (Item == null)
            return null;
        if (string.IsNullOrWhiteSpace(MemberName))
            return Item;
        if (Item is DataSourceRow Row)
            return Row[MemberName];
        if (Item is DataRowView RowView)
            return RowView[MemberName];
        if (Item is DataRow DataRow)
            return DataRow[MemberName];

        PropertyInfo Prop = Item.GetType().GetProperty(MemberName, BindingFlags.Instance | BindingFlags.Public);
        if (Prop != null)
            return Prop.GetValue(Item);

        return null;
    }

    // ● static public methods
    static public IEnumerable GetItems(ControlBinding Binding)
    {
        if (Binding == null)
            return null;

        IEnumerable Items = Binding.GetLookupItems();
        if (Items != null)
            return Items;

        if (Binding.LookupSource == null)
            return null;

        return Binding.LookupSource.Items;
    }
    static public object GetItemValue(object Item, ControlBinding Binding)
    {
        if (Binding == null)
            return null;
        if (Item == null)
            return null;
        if (ReferenceEquals(Item, Binding.LookupNullItem))
            return null;

        return GetMemberValue(Item, Binding.ValueMember);
    }
    static public string GetItemDisplayText(object Item, ControlBinding Binding)
    {
        if (Binding == null)
            return string.Empty;
        if (Item == null)
            return string.Empty;
        if (ReferenceEquals(Item, Binding.LookupNullItem) && Item is LookupNullItem NullItem)
            return NullItem.Text;

        object Value = GetMemberValue(Item, Binding.DisplayMember);
        return Convert.ToString(Value, CultureInfo.InvariantCulture) ?? string.Empty;
    }
    static public object FindItemByValue(ControlBinding Binding, object Value)
    {
        IEnumerable Items = GetItems(Binding);
        if (Items == null)
            return null;

        if (Value == null || Value == DBNull.Value)
        {
            if (Binding.LookupNullItem != null)
                return Binding.LookupNullItem;
        }

        foreach (object Item in Items)
        {
            if (ReferenceEquals(Item, Binding.LookupNullItem))
                continue;

            object ItemValue = GetItemValue(Item, Binding);
            if (AreEqual(ItemValue, Value))
                return Item;
        }

        return null;
    }
}
/// <summary>
/// Avalonia helper methods for control binding
/// </summary>
static internal class DataBinderControlBindingHelper
{
    // ● private methods
    static private bool GetAllowNullSelection(DataBinder Binder, string ColumnName, bool? AllowNullSelection)
    {
        if (AllowNullSelection != null)
            return AllowNullSelection.Value;

        if (Binder?.Source != null)
        {
            DataSourceColumn Column = Binder.Source.Columns.FirstOrDefault(Item => string.Equals(Item.Name, ColumnName, StringComparison.OrdinalIgnoreCase));
            if (Column != null)
                return Column.AllowsNull;
        }

        return false;
    }
    static private string GetNullText(string NullText)
    {
        return !string.IsNullOrWhiteSpace(NullText) ? NullText : "(None)";
    }
    static private void BuildLookupItems(ControlBinding Binding)
    {
        if (Binding == null || Binding.LookupSource == null)
            return;

        Binding.ClearLookupItems();

        if (Binding.LookupNullItem != null)
            Binding.AddLookupItem(Binding.LookupNullItem);

        if (Binding.LookupSource.Items != null)
        {
            foreach (object Item in Binding.LookupSource.Items)
                Binding.AddLookupItem(Item);
        }
    }
    static private Binding CreateTwoWayBinding(string ColumnName)
    {
        return new Binding($"[{ColumnName}]")
        {
            Mode = BindingMode.TwoWay,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
    }
    static private void RefreshLookupSelection(DataBinder Binder, ControlBinding Binding)
    {
        if (Binder == null || Binding == null || !Binding.IsLookup)
            return;
        if (Binding.Control is not ComboBox Box)
            return;

        Binding.IsRefreshing = true;
        try
        {
            object Value = Binder[Binding.ColumnName];
            object Item = DataBinderLookupHelper.FindItemByValue(Binding, Value);
            Box.SelectedItem = Item;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }

    // ● static public methods
    static public ControlBinding Bind(DataBinder Binder, TextBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(TextBox.TextProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, CheckBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(ToggleButton.IsCheckedProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, ComboBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(SelectingItemsControl.SelectedItemProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, ComboBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (LookupSource == null)
            throw new ArgumentNullException(nameof(LookupSource));
        if (string.IsNullOrWhiteSpace(TargetColumnName))
            throw new ArgumentNullException(nameof(TargetColumnName));

        ControlBinding Result = new(Box, TargetColumnName);
        Result.LookupSource = LookupSource;
        Result.LookupSourceName = LookupSource.Name;
        Result.DisplayMember = DisplayMember;
        Result.ValueMember = ValueMember;
        Result.AllowNullSelection = AllowNullSelection;
        Result.NullText = GetNullText(NullText);

        if (GetAllowNullSelection(Binder, TargetColumnName, AllowNullSelection))
            Result.LookupNullItem = new LookupNullItem(Result.NullText);

        BuildLookupItems(Result);

        Box.DataContext = null;
        Box.ItemsSource = Result.GetLookupItems();
        Box.ItemTemplate = new FuncDataTemplate<object>((Item, _) =>
        {
            return new TextBlock
            {
                Text = DataBinderLookupHelper.GetItemDisplayText(Item, Result)
            };
        });

        EventHandler<SelectionChangedEventArgs> Handler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;

            object Item = Box.SelectedItem;
            object Value = DataBinderLookupHelper.GetItemValue(Item, Result);
            Binder[TargetColumnName] = Value;
        };

        Result.SelectionChangedHandler = Handler;
        Box.SelectionChanged += Handler;

        RefreshLookupSelection(Binder, Result);
        return Result;
    }
    
    static public ControlBinding Bind(DataBinder Binder, DatePicker Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(DatePicker.SelectedDateProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, NumericUpDown Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(NumericUpDown.ValueProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    static public ControlBinding Bind(DataBinder Binder, ToggleSwitch Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        Box.DataContext = Binder;
        Box.Bind(ToggleSwitch.IsCheckedProperty, CreateTwoWayBinding(ColumnName));

        return Result;
    }
    
    static public void Refresh(DataBinder Binder)
    {
        if (Binder == null)
            return;

        Binder.NotifyAll();

        foreach (ControlBinding Binding in Binder.Bindings)
        {
            if (Binding.IsLookup)
                Refresh(Binder, Binding);
        }
    }
    static public void Refresh(DataBinder Binder, ControlBinding Binding)
    {
        if (Binder == null || Binding == null)
            return;

        if (Binding.IsLookup)
            RefreshLookupSelection(Binder, Binding);
        else
            Binder.Notify(Binding.ColumnName);
    }
    
    
}
/// <summary>
/// Thin UI facade over a DataSource
/// </summary>
public class DataBinder: INotifyPropertyChanged
{
    // ● private
    DataSource fSource;
    List<ControlBinding> fBindings = new();

    // ● private methods
    void Subscribe()
    {
        if (fSource == null)
            return;

        fSource.CurrentRowChanged += OnSourceChanged;
        fSource.DataChanged += OnSourceChanged;
        fSource.RowStateChanged += OnSourceChanged;
    }
    void Unsubscribe()
    {
        if (fSource == null)
            return;

        fSource.CurrentRowChanged -= OnSourceChanged;
        fSource.DataChanged -= OnSourceChanged;
        fSource.RowStateChanged -= OnSourceChanged;
    }
    void OnSourceChanged(object Sender, EventArgs Args)
    {
        RefreshBindings();
    }
    ControlBinding FindBinding(Control Control)
    {
        return fBindings.Find(Item => ReferenceEquals(Item.Control, Control));
    }

    // ● constructor
    public DataBinder(DataSource Source)
    {
        fSource = Source ?? throw new ArgumentNullException(nameof(Source));
        Subscribe();
    }

    // ● public methods
    public void Bind(TextBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(CheckBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(ComboBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    
    /// <summary>
    /// Default from source metadata: Binder.Bind(cboCustomer, "Customers", "CustomerId", "Name", "Id");
    /// Forced nullable: Binder.Bind(cboCustomer, "Customers", "CustomerId", "Name", "Id", true, "(None)");
    /// Forced non-nullable: Binder.Bind(cboCustomer, "Customers", "CustomerId", "Name", "Id", false);
    /// </summary>
    public void Bind(ComboBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText));
    }
    public void Bind(ComboBox Box, string LookupSourceName, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        if (string.IsNullOrWhiteSpace(LookupSourceName))
            throw new ArgumentNullException(nameof(LookupSourceName));

        ILookupSource LookupSource = Source.Lookups.Get(LookupSourceName);
        Bind(Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText);
    }
    
    public void Bind(DatePicker Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(NumericUpDown Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(ToggleSwitch Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(DataBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    
    public void Unbind(Control Control)
    {
        ControlBinding Binding = FindBinding(Control);
        if (Binding == null)
            return;

        if (Binding.Control is ComboBox Box && Binding.SelectionChangedHandler != null)
            Box.SelectionChanged -= Binding.SelectionChangedHandler;

        Binding.Control.DataContext = null;
        fBindings.Remove(Binding);
    }
    public void UnbindAll()
    {
        List<ControlBinding> List = new(fBindings);

        foreach (ControlBinding Binding in List)
            Unbind(Binding.Control);
    }
    public void RefreshBindings()
    {
        DataBinderControlBindingHelper.Refresh(this);
    }
    public void RefreshControl(Control Control)
    {
        ControlBinding Binding = FindBinding(Control);
        if (Binding != null)
            DataBinderControlBindingHelper.Refresh(this, Binding);
    }
    public DataSourceRow CreateNew()
    {
        return Source.CreateNew();
    }
    public void Add(DataSourceRow Row)
    {
        Source.Add(Row);
    }
    public void DeleteCurrent()
    {
        Source.DeleteCurrent();
    }
    public void Refresh()
    {
        Source.Refresh();
    }
    public void Notify(string ColumnName)
    {
        OnPropertyChanged("Item");
        OnPropertyChanged(nameof(CurrentRow));
        OnPropertyChanged(nameof(CurrentItem));
    }
    public void NotifyAll()
    {
        OnPropertyChanged("Item");
        OnPropertyChanged(nameof(CurrentRow));
        OnPropertyChanged(nameof(CurrentItem));
    }
    public override string ToString()
    {
        return Source?.ToString() ?? base.ToString();
    }

    // ● properties
    public DataSource Source => fSource;
    public DataSourceRow CurrentRow => Source.CurrentRow;
    public object CurrentItem => Source.CurrentItem;
    public IReadOnlyList<DataSourceColumn> Columns => Source.Columns;
    public IReadOnlyList<ControlBinding> Bindings => fBindings;
    public object this[string ColumnName]
    {
        get => Source[ColumnName];
        set
        {
            Source[ColumnName] = value;
            Notify(ColumnName);
        }
    }

    // ● events
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }
}
