using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Tripous.Data;

namespace Tripous.Avalon;

public class LookupItem
{
    // ● constructors
    public LookupItem(object Value, string DisplayText, Row Row, bool IsNullItem = false)
    {
        this.Value = Value;
        this.DisplayText = DisplayText;
        this.Row = Row;
        this.IsNullItem = IsNullItem;
    }

    // ● public methods
    public override string ToString()
    {
        return DisplayText ?? string.Empty;
    }

    // ● properties
    public object Value { get; }
    public string DisplayText { get; }
    public Row Row { get; }
    public bool IsNullItem { get; }
}
public class ControlBinding: IDisposable
{
    // ● private fields
    Action fDisposeAction;

    // ● constructors
    public ControlBinding(Control Control, string ColumnName = null)
    {
        this.Control = Control ?? throw new ArgumentNullException(nameof(Control));
        this.ColumnName = ColumnName;
    }

    // ● public methods
    public void Dispose()
    {
        fDisposeAction?.Invoke();
        fDisposeAction = null;
    }

    // ● properties
    public Control Control { get; }
    public string ColumnName { get; }
    public bool IsRefreshing { get; set; }
    public ILookupSource LookupSource { get; set; }
    public LookupResolver LookupResolver { get; set; }
    public List<LookupItem> LookupItems { get; } = new();
    public bool AllowNullSelection { get; set; }
    public string NullText { get; set; }
    public string DisplayMember { get; set; }
    public string ValueMember { get; set; }
    public Action DisposeAction
    {
        get => fDisposeAction;
        set => fDisposeAction = value;
    }
}
public class RowSetViewBinder: IDisposable
{
    // ● private fields
    List<ControlBinding> fBindings = new();
    RowSetView fView;
    LookupRegistry fLookups;

    // ● private methods
    void View_CurrentRowViewChanged(object Sender, EventArgs e)
    {
        Refresh();
    }
    void View_Rebuilt(object Sender, EventArgs e)
    {
        Refresh();
    }

    // ● constructors
    public RowSetViewBinder(RowSetView View, LookupRegistry Lookups = null)
    {
        fView = View ?? throw new ArgumentNullException(nameof(View));
        fLookups = Lookups;
        fView.CurrentRowViewChanged += View_CurrentRowViewChanged;
        fView.Rebuilt += View_Rebuilt;
    }

    // ● public methods
    public void Dispose()
    {
        if (fView != null)
        {
            fView.CurrentRowViewChanged -= View_CurrentRowViewChanged;
            fView.Rebuilt -= View_Rebuilt;
        }

        Clear();
    }
    public void Clear()
    {
        foreach (ControlBinding Item in fBindings)
            Item.Dispose();

        fBindings.Clear();
    }
    public void Unbind(Control Control)
    {
        if (Control == null)
            return;

        for (int i = fBindings.Count - 1; i >= 0; i--)
        {
            if (ReferenceEquals(fBindings[i].Control, Control))
            {
                fBindings[i].Dispose();
                fBindings.RemoveAt(i);
            }
        }
    }
    public void Refresh()
    {
        foreach (ControlBinding Item in fBindings)
            RowSetViewBinderControlBindingHelper.Refresh(this, Item);
    }

    public void Bind(TextBox Box, string ColumnName)
    {
        Unbind(Box);
        fBindings.Add(RowSetViewBinderControlBindingHelper.Bind(this, Box, ColumnName));
    }
    public void Bind(ComboBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        Unbind(Box);
        fBindings.Add(RowSetViewBinderControlBindingHelper.Bind(this, Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText));
    }
    public void Bind(ComboBox Box, string LookupSourceName, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        if (string.IsNullOrWhiteSpace(LookupSourceName))
            throw new ArgumentNullException(nameof(LookupSourceName));

        ILookupSource LookupSource = Lookups?.Find(LookupSourceName);
        if (LookupSource == null)
            throw new ArgumentException($"Lookup source not found: {LookupSourceName}", nameof(LookupSourceName));

        Bind(Box, LookupSource, TargetColumnName, DisplayMember, ValueMember, AllowNullSelection, NullText);
    }
    public void Bind(ListBox Box, string DisplayMember = null)
    {
        Unbind(Box);
        fBindings.Add(RowSetViewBinderControlBindingHelper.Bind(this, Box, DisplayMember));
    }

    // ● properties
    public RowSetView View => fView;
    public LookupRegistry Lookups => fLookups;
    public Row CurrentRow => View?.CurrentRow;
    public RowView CurrentRowView
    {
        get => View?.CurrentRowView;
        set
        {
            if (View != null)
                View.CurrentRowView = value;
        }
    }
    public object this[string ColumnName]
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ColumnName))
                throw new ArgumentNullException(nameof(ColumnName));

            Row Row = CurrentRow;
            return Row != null ? Row[ColumnName] : null;
        }
        set
        {
            if (string.IsNullOrWhiteSpace(ColumnName))
                throw new ArgumentNullException(nameof(ColumnName));

            Row Row = CurrentRow;
            if (Row != null)
                Row[ColumnName] = value;
        }
    }
}


static internal class RowSetViewBinderControlBindingHelper
{
    // ● private methods
    static private void SetTextBoxValue(RowSetViewBinder Binder, ControlBinding Binding, TextBox Box)
    {
        object Value = Binder[Binding.ColumnName];
        string Text = Value == null ? string.Empty : Convert.ToString(Value);

        Binding.IsRefreshing = true;
        try
        {
            if (!string.Equals(Box.Text, Text, StringComparison.Ordinal))
                Box.Text = Text;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    static private void BuildLookupItems(ControlBinding Binding)
    {
        Binding.LookupItems.Clear();

        if (Binding.AllowNullSelection)
            Binding.LookupItems.Add(new LookupItem(null, string.IsNullOrWhiteSpace(Binding.NullText) ? "(None)" : Binding.NullText, null, true));

        RowSet LookupRowSet = Binding.LookupSource?.GetRowSet();
        if (LookupRowSet == null)
            return;

        foreach (Row Row in LookupRowSet)
        {
            object Value = Row[Binding.ValueMember];
            object DisplayValue = Row[Binding.DisplayMember];
            string DisplayText = DisplayValue == null ? string.Empty : Convert.ToString(DisplayValue);

            Binding.LookupItems.Add(new LookupItem(Value, DisplayText, Row));
        }
    }
    static private LookupItem FindLookupItemByValue(ControlBinding Binding, object Value)
    {
        if (Binding == null)
            return null;

        if (Value == null)
        {
            foreach (LookupItem Item in Binding.LookupItems)
            {
                if (Item.IsNullItem)
                    return Item;
            }

            return null;
        }

        Row Row = Binding.LookupResolver?.GetRow(Value);
        if (Row == null)
            return null;

        foreach (LookupItem Item in Binding.LookupItems)
        {
            if (ReferenceEquals(Item.Row, Row))
                return Item;
        }

        return null;
    }
    static private FuncDataTemplate<LookupItem> CreateLookupItemTemplate()
    {
        return new FuncDataTemplate<LookupItem>((Item, _) =>
        {
            return new TextBlock()
            {
                Text = Item != null ? Item.DisplayText : string.Empty
            };
        }, true);
    }
    static private FuncDataTemplate<RowView> CreateRowViewTemplate(string DisplayMember)
    {
        return new FuncDataTemplate<RowView>((Item, _) =>
        {
            string Text = string.Empty;

            if (Item?.Row != null && !string.IsNullOrWhiteSpace(DisplayMember))
            {
                object Value = Item.Row[DisplayMember];
                Text = Value == null ? string.Empty : Convert.ToString(Value);
            }

            return new TextBlock()
            {
                Text = Text
            };
        }, true);
    }
    static private void RefreshTextBox(RowSetViewBinder Binder, ControlBinding Binding)
    {
        if (Binding.Control is TextBox Box)
            SetTextBoxValue(Binder, Binding, Box);
    }
    static private void RefreshLookupSelection(RowSetViewBinder Binder, ControlBinding Binding)
    {
        if (Binding.Control is not ComboBox Box)
            return;

        LookupItem Item = FindLookupItemByValue(Binding, Binder[Binding.ColumnName]);

        Binding.IsRefreshing = true;
        try
        {
            if (!ReferenceEquals(Box.SelectedItem, Item))
                Box.SelectedItem = Item;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }
    static private void RefreshListSelection(RowSetViewBinder Binder, ControlBinding Binding)
    {
        if (Binding.Control is not ListBox Box)
            return;

        Binding.IsRefreshing = true;
        try
        {
            if (!ReferenceEquals(Box.SelectedItem, Binder.CurrentRowView))
                Box.SelectedItem = Binder.CurrentRowView;
        }
        finally
        {
            Binding.IsRefreshing = false;
        }
    }

    // ● static public methods
    static public void Refresh(RowSetViewBinder Binder, ControlBinding Binding)
    {
        if (Binder == null || Binding == null)
            return;

        if (Binding.Control is TextBox)
            RefreshTextBox(Binder, Binding);
        else if (Binding.Control is ComboBox)
            RefreshLookupSelection(Binder, Binding);
        else if (Binding.Control is ListBox)
            RefreshListSelection(Binder, Binding);
    }
    static public ControlBinding Bind(RowSetViewBinder Binder, TextBox Box, string ColumnName)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (string.IsNullOrWhiteSpace(ColumnName))
            throw new ArgumentNullException(nameof(ColumnName));

        ControlBinding Result = new(Box, ColumnName);

        EventHandler<TextChangedEventArgs> Handler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;
            if (Binder.CurrentRow == null)
                return;

            Binder[ColumnName] = Box.Text;
        };

        Box.TextChanged += Handler;
        Result.DisposeAction = () => Box.TextChanged -= Handler;

        Refresh(Binder, Result);
        return Result;
    }
    static public ControlBinding Bind(RowSetViewBinder Binder, ComboBox Box, ILookupSource LookupSource, string TargetColumnName, string DisplayMember, string ValueMember, bool? AllowNullSelection = null, string NullText = null)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));
        if (LookupSource == null)
            throw new ArgumentNullException(nameof(LookupSource));
        if (string.IsNullOrWhiteSpace(TargetColumnName))
            throw new ArgumentNullException(nameof(TargetColumnName));
        if (string.IsNullOrWhiteSpace(DisplayMember))
            throw new ArgumentNullException(nameof(DisplayMember));
        if (string.IsNullOrWhiteSpace(ValueMember))
            throw new ArgumentNullException(nameof(ValueMember));

        RowSet LookupRowSet = LookupSource.GetRowSet();
        if (LookupRowSet == null)
            throw new InvalidOperationException("Lookup source returned null RowSet.");

        ControlBinding Result = new(Box, TargetColumnName)
        {
            LookupSource = LookupSource,
            LookupResolver = new LookupResolver(LookupRowSet, ValueMember, DisplayMember),
            AllowNullSelection = AllowNullSelection ?? false,
            NullText = NullText,
            DisplayMember = DisplayMember,
            ValueMember = ValueMember,
        };

        BuildLookupItems(Result);

        Box.ItemsSource = Result.LookupItems;
        //Box.ItemTemplate = CreateLookupItemTemplate();
 

        EventHandler<SelectionChangedEventArgs> Handler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;
            if (Binder.CurrentRow == null)
                return;

            LookupItem Item = Box.SelectedItem as LookupItem;
            Binder[TargetColumnName] = Item != null ? Item.Value : null;
            //Binder.View.Refresh();
        };

        Box.SelectionChanged += Handler;
        Result.DisposeAction = () => Box.SelectionChanged -= Handler;

        Refresh(Binder, Result);
        return Result;
    }
    static public ControlBinding Bind(RowSetViewBinder Binder, ListBox Box, string DisplayMember = null)
    {
        if (Binder == null)
            throw new ArgumentNullException(nameof(Binder));
        if (Box == null)
            throw new ArgumentNullException(nameof(Box));

        ControlBinding Result = new(Box)
        {
            DisplayMember = DisplayMember,
        };

        Box.ItemsSource = Binder.View;

        if (!string.IsNullOrWhiteSpace(DisplayMember))
            Box.ItemTemplate = CreateRowViewTemplate(DisplayMember);

        EventHandler<SelectionChangedEventArgs> Handler = (Sender, Args) =>
        {
            if (Result.IsRefreshing)
                return;

            Binder.CurrentRowView = Box.SelectedItem as RowView;
        };

        Box.SelectionChanged += Handler;
        Result.DisposeAction = () => Box.SelectionChanged -= Handler;

        Refresh(Binder, Result);
        return Result;
    }
}





 
