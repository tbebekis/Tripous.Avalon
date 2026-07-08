// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides a custom drop-down in-place editor for lookup group grid cells.
/// </summary>
public class GroupGridLookupInplaceEditor: GroupGridDropDownInplaceEditorBase
{
    // ● private fields
    readonly GroupGridLookupColumn fColumn;
    readonly List<GroupGridLookupEditorItem> fItems = new();
    GroupGridLookupEditorItem fSelectedItem;

    // ● private methods
    object GetMemberValue(object Item, string Member)
    {
        if (Item == null || string.IsNullOrWhiteSpace(Member))
            return null;
        if (Item is DataRowView RowView)
            return RowView.Row.Table.Columns.Contains(Member) ? RowView[Member] : null;
        if (Item is DataRow Row)
            return Row.Table.Columns.Contains(Member) ? Row[Member] : null;

        PropertyInfo Property = Item.GetType().GetProperty(Member, BindingFlags.Public | BindingFlags.Instance);
        return Property == null ? null : Property.GetValue(Item);
    }
    void LoadItems()
    {
        fItems.Clear();
        if (fColumn.LookupItemsSource == null)
            return;

        foreach (object Item in fColumn.LookupItemsSource)
        {
            object DisplayValue = GetMemberValue(Item, fColumn.DisplayMember);
            fItems.Add(new GroupGridLookupEditorItem
            {
                Item = Item,
                Text = Convert.ToString(DisplayValue, CultureInfo.CurrentCulture) ?? string.Empty,
            });
        }
    }
    void SelectValue(object Value)
    {
        foreach (GroupGridLookupEditorItem Item in fItems)
        {
            object ItemValue = GetMemberValue(Item.Item, fColumn.ValueMember);
            if (Equals(ItemValue, Value))
            {
                SetSelectedItem(Item);
                return;
            }
        }

        SetSelectedItem(null);
    }
    void SetSelectedItem(GroupGridLookupEditorItem Item)
    {
        fSelectedItem = Item;
        TextBox.Text = Item?.Text ?? string.Empty;
        RaiseValueChanged();
    }
    void MoveSelection(int Delta)
    {
        if (fItems.Count == 0)
            return;

        int Index = fSelectedItem == null ? -1 : fItems.IndexOf(fSelectedItem);
        int NewIndex = Math.Clamp(Index + Delta, 0, fItems.Count - 1);
        SetSelectedItem(fItems[NewIndex]);
    }
    void Editor_KeyDown(object Sender, KeyEventArgs Args)
    {
        if (Args.Key == Key.Down && !Args.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            MoveSelection(1);
            Args.Handled = true;
            return;
        }
        if (Args.Key == Key.Up && !Args.KeyModifiers.HasFlag(KeyModifiers.Alt))
        {
            MoveSelection(-1);
            Args.Handled = true;
            return;
        }
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridLookupInplaceEditor"/> class.
    /// </summary>
    /// <param name="Column">The lookup column.</param>
    public GroupGridLookupInplaceEditor(GroupGridLookupColumn Column)
    {
        fColumn = Column ?? throw new ArgumentNullException(nameof(Column));
        LoadItems();
        TextBox.KeyDown += Editor_KeyDown;
    }

    // ● public methods
    /// <inheritdoc />
    public override void Cleanup()
    {
        base.Cleanup();
        TextBox.KeyDown -= Editor_KeyDown;
    }

    // ● properties
    /// <inheritdoc />
    public override object Value
    {
        get => fSelectedItem?.Item;
        set => SelectValue(value);
    }
    /// <inheritdoc />
    public override bool HasDropDown => true;
    /// <inheritdoc />
    public override IEnumerable DropDownItems => fItems;
    /// <inheritdoc />
    public override object SelectedDropDownItem => fSelectedItem;
    /// <inheritdoc />
    public override void SelectDropDownItem(object Item)
    {
        SetSelectedItem(Item as GroupGridLookupEditorItem);
    }

    // ● protected properties
    /// <inheritdoc />
    protected override string DropDownButtonText => "▾";
    /// <inheritdoc />
    protected override bool IsTextReadOnly => true;

    // ● protected methods
    /// <inheritdoc />
    protected override object GetDropDownSelectedValue()
    {
        return fSelectedItem;
    }

    // ● private types
    /// <summary>
    /// Represents a display item for the lookup drop-down list.
    /// </summary>
    class GroupGridLookupEditorItem
    {
        // ● properties
        /// <summary>
        /// Gets or sets the source lookup item.
        /// </summary>
        public object Item { get; set; }
        /// <summary>
        /// Gets or sets the display text.
        /// </summary>
        public string Text { get; set; } = string.Empty;
        /// <inheritdoc />
        public override string ToString()
        {
            return Text;
        }
    }
}
