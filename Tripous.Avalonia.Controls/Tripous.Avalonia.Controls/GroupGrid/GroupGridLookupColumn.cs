// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Represents a lookup column in a group grid.
/// </summary>
public class GroupGridLookupColumn: GroupGridColumn
{
    // ● private methods
    object GetMemberValue(object Item, string Member)
    {
        if (Item == null || string.IsNullOrWhiteSpace(Member))
            return null;
        if (Item is DataRowView RowView)
            return RowView.Row.Table.Columns.Contains(Member) ? RowView[Member] : null;
        if (Item is DataRow Row)
            return Row.Table.Columns.Contains(Member) ? Row[Member] : null;
        if (Item is IDictionary<string, object> Dictionary)
            return Dictionary.TryGetValue(Member, out object Value) ? Value : null;

        PropertyInfo Property = Item.GetType().GetProperty(Member, BindingFlags.Public | BindingFlags.Instance);
        return Property == null ? null : Property.GetValue(Item);
    }
    object FindLookupItem(object Value)
    {
        if (LookupItemsSource == null)
            return null;

        foreach (object Item in LookupItemsSource)
        {
            object ItemValue = GetMemberValue(Item, ValueMember);
            if (Equals(ItemValue, Value))
                return Item;
        }

        return null;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridLookupColumn"/> class.
    /// </summary>
    public GroupGridLookupColumn()
    {
    }

    // ● public methods
    /// <inheritdoc />
    public override string FormatValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return string.Empty;

        object Item = FindLookupItem(Value);
        object DisplayValue = Item == null ? null : GetMemberValue(Item, DisplayMember);
        return DisplayValue == null || DisplayValue == DBNull.Value
            ? base.FormatValue(Value)
            : Convert.ToString(DisplayValue, CultureInfo.CurrentCulture) ?? string.Empty;
    }
    /// <inheritdoc />
    public override object ParseValue(object Value)
    {
        if (Value == null || Value == DBNull.Value)
            return null;

        object Item = Value;
        object ItemValue = GetMemberValue(Item, ValueMember);
        if (ItemValue != null)
            return ItemValue;

        return Convert.ChangeType(Value, ValueType, CultureInfo.CurrentCulture);
    }

    // ● properties
    /// <summary>
    /// Gets or sets the lookup items source.
    /// </summary>
    public IEnumerable LookupItemsSource { get; set; }
    /// <summary>
    /// Gets or sets the lookup display member.
    /// </summary>
    public string DisplayMember { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the lookup value member.
    /// </summary>
    public string ValueMember { get; set; } = string.Empty;
}
