namespace Tripous.Desktop;

/// <summary>
/// A composite locator editor.
/// </summary>
public class LocatorBox: UserControl
{
    // ● private fields
    protected Grid fRoot;
    protected Popup fPopup;
    protected DataGrid fGrid;
    protected List<TextBox> fTextBoxes = [];
    protected Dictionary<LocatorFieldDef, TextBox> fTextBoxMap = [];

    // ● protected methods
    /// <summary>
    /// Builds the control UI.
    /// </summary>
    protected virtual void Build()
    {
        fRoot = new Grid();
        fTextBoxes.Clear();
        fTextBoxMap.Clear();
        if (Locator == null || Locator.LocatorDef == null)
        {
            Content = fRoot;
            return;
        }
        List<LocatorFieldDef> Fields = Locator.LocatorDef.Fields.Where(item => item.IsVisible).ToList();
        for (int Index = 0; Index < Fields.Count; Index++)
        {
            LocatorFieldDef FieldDef = Fields[Index];
            bool IsLast = Index == Fields.Count - 1;
            ColumnDefinition Column = new();
            Column.Width = IsLast || FieldDef.DisplayWidth <= 0 ? new GridLength(1, GridUnitType.Star) : new GridLength(FieldDef.DisplayWidth);
            fRoot.ColumnDefinitions.Add(Column);
            TextBox TextBox = CreateTextBox(FieldDef);
            Grid.SetColumn(TextBox, Index);
            fRoot.Children.Add(TextBox);
            fTextBoxes.Add(TextBox);
            fTextBoxMap[FieldDef] = TextBox;
        }
        CreatePopup();
        Content = fRoot;
    }
    /// <summary>
    /// Creates a textbox for a locator field.
    /// </summary>
    protected virtual TextBox CreateTextBox(LocatorFieldDef FieldDef)
    {
        TextBox Result = new();
        Result.Tag = FieldDef;
        Result.IsReadOnly = IsReadOnly || !FieldDef.IsSearchable;
        Result.ContextMenu = CreateContextMenu();
        Result.KeyDown += TextBox_KeyDown;
        return Result;
    }
    /// <summary>
    /// Creates the popup.
    /// </summary>
    protected virtual void CreatePopup()
    {
        fGrid = new DataGrid();
        fGrid.AutoGenerateColumns = true;
        fGrid.IsReadOnly = true;
        fGrid.MinWidth = 300;
        fGrid.MaxHeight = 260;
        fGrid.DoubleTapped += Grid_DoubleTapped;
        fGrid.KeyDown += Grid_KeyDown;
        fPopup = new Popup();
        fPopup.PlacementTarget = this;
        fPopup.Placement = PlacementMode.Bottom;
        fPopup.IsLightDismissEnabled = true;
        fPopup.Child = fGrid;
    }
    /// <summary>
    /// Creates the context menu.
    /// </summary>
    protected virtual ContextMenu CreateContextMenu()
    {
        ContextMenu Result = new();
        MenuItem ItemClear = new() { Header = "Clear", IsEnabled = !IsReadOnly };
        ItemClear.Click += ItemClear_Click;
        Result.Items.Add(ItemClear);
        MenuItem ItemSearch = new() { Header = "Search", IsEnabled = !IsReadOnly };
        ItemSearch.Click += ItemSearch_Click;
        Result.Items.Add(ItemSearch);
        if (Locator != null && Locator.LocatorDef != null && !string.IsNullOrWhiteSpace(Locator.LocatorDef.ZoomCommand))
        {
            MenuItem ItemZoom = new() { Header = "Zoom" };
            ItemZoom.Click += ItemZoom_Click;
            Result.Items.Add(ItemZoom);
        }
        return Result;
    }
    /// <summary>
    /// Opens the popup.
    /// </summary>
    protected virtual void OpenPopup()
    {
        if (fPopup != null && fGrid != null && Locator != null)
        {
            fGrid.ItemsSource = Locator.SourceTable.DataView;
            fPopup.IsOpen = true;
        }
    }
    /// <summary>
    /// Closes the popup.
    /// </summary>
    protected virtual void ClosePopup()
    {
        if (fPopup != null)
            fPopup.IsOpen = false;
    }
    /// <summary>
    /// Performs search using the specified textbox text.
    /// </summary>
    protected virtual void Search(TextBox TextBox)
    {
        // TODO: Locator.Execute(), single row assign, multi-row popup.
    }
    /// <summary>
    /// Selects the current popup row.
    /// </summary>
    protected virtual void SelectCurrentRow()
    {
        // TODO: assign selected DataRowView.Row.
        ClosePopup();
    }
    /// <summary>
    /// Clears the current value.
    /// </summary>
    protected virtual void ClearValue()
    {
        KeyValue = DBNull.Value;
        ClearTargetBoxes();
    }
    protected virtual void TextBox_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter && Sender is TextBox TextBox)
        {
            Search(TextBox);
            Args.Handled = true;
        }
        else if (Args.Key == Key.Escape)
        {
            ClosePopup();
            Args.Handled = true;
        }
    }
    protected virtual void Grid_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Enter)
        {
            SelectCurrentRow();
            Args.Handled = true;
        }
        else if (Args.Key == Key.Escape)
        {
            ClosePopup();
            Args.Handled = true;
        }
    }
    protected virtual void Grid_DoubleTapped(object Sender, TappedEventArgs Args)
    {
        SelectCurrentRow();
    }
    protected virtual void ItemClear_Click(object Sender, RoutedEventArgs Args)
    {
        ClearValue();
    }
    protected virtual void ItemSearch_Click(object Sender, RoutedEventArgs Args)
    {
        TextBox TextBox = fTextBoxes.FirstOrDefault(item => item.IsFocused);
        if (TextBox != null)
            Search(TextBox);
    }
    protected virtual void ItemZoom_Click(object Sender, RoutedEventArgs Args)
    {
        ZoomRequested?.Invoke(this, EventArgs.Empty);
    }

    // ● constructors
    /// <summary>
    /// Constructor.
    /// </summary>
    public LocatorBox()
    {
        Build();
    }

    // ● static public methods
    /// <summary>
    /// Locator property.
    /// </summary>
    static public readonly StyledProperty<Locator> LocatorProperty = AvaloniaProperty.Register<LocatorBox, Locator>(nameof(Locator));
    /// <summary>
    /// IsReadOnly property.
    /// </summary>
    static public readonly StyledProperty<bool> IsReadOnlyProperty = AvaloniaProperty.Register<LocatorBox, bool>(nameof(IsReadOnly));
    /// <summary>
    /// KeyValue property.
    /// </summary>
    static public readonly StyledProperty<object> KeyValueProperty = AvaloniaProperty.Register<LocatorBox, object>(nameof(KeyValue));

    // ● public methods
    /// <summary>
    /// Rebuilds the control.
    /// </summary>
    public virtual void Rebuild()
    {
        Build();
    }
    /// <summary>
    /// Sets a textbox value.
    /// </summary>
    public virtual void SetTargetBoxValue(LocatorFieldDef FieldDef, object Value)
    {
        if (FieldDef != null && fTextBoxMap.TryGetValue(FieldDef, out TextBox TextBox))
            TextBox.Text = Sys.IsNull(Value) ? string.Empty : Value.ToString();
    }
    /// <summary>
    /// Clears all target textboxes.
    /// </summary>
    public virtual void ClearTargetBoxes()
    {
        foreach (TextBox TextBox in fTextBoxes)
            TextBox.Text = string.Empty;
    }
    /// <summary>
    /// Refreshes all target textboxes from a row.
    /// </summary>
    public virtual void RefreshTargetBoxes(DataRow Row)
    {
        if (Row == null || Locator == null || Locator.LocatorDef == null)
        {
            ClearTargetBoxes();
            return;
        }
        foreach (LocatorFieldDef FieldDef in Locator.LocatorDef.Fields)
        {
            if (FieldDef.IsVisible && !string.IsNullOrWhiteSpace(FieldDef.TargetField))
            {
                DataColumn Column = Row.Table.FindColumn(FieldDef.TargetField);
                object Value = Column != null ? Row[Column] : DBNull.Value;
                SetTargetBoxValue(FieldDef, Value);
            }
        }
    }

    // ● properties
    /// <summary>
    /// The locator.
    /// </summary>
    public Locator Locator
    {
        get => GetValue(LocatorProperty);
        set => SetValue(LocatorProperty, value);
    }
    /// <summary>
    /// True when this control is read-only.
    /// </summary>
    public bool IsReadOnly
    {
        get => GetValue(IsReadOnlyProperty);
        set => SetValue(IsReadOnlyProperty, value);
    }
    /// <summary>
    /// The selected key value.
    /// </summary>
    public object KeyValue
    {
        get => GetValue(KeyValueProperty);
        set => SetValue(KeyValueProperty, value);
    }

    // ● events
    /// <summary>
    /// Occurs when zoom is requested.
    /// </summary>
    public event EventHandler ZoomRequested;

    // ● static constructor
    /// <summary>
    /// Static constructor.
    /// </summary>
    static LocatorBox()
    {
        LocatorProperty.Changed.AddClassHandler<LocatorBox>((Sender, Args) => Sender.Rebuild());
        IsReadOnlyProperty.Changed.AddClassHandler<LocatorBox>((Sender, Args) => Sender.Rebuild());
    }
}