// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Tripous.Avalonia.Controls.Grid.Tests;

/// <summary>
/// Provides a simple row model for group grid tests.
/// </summary>
public class GridTestRow: INotifyPropertyChanged
{
    // ● private fields
    string fCategory;
    string fName;
    int fQuantity;
    decimal fAmount;

    // ● private methods
    void SetValue<T>(ref T Field, T Value, string PropertyName)
    {
        if (EqualityComparer<T>.Default.Equals(Field, Value))
            return;

        Field = Value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    }

    // ● properties
    /// <summary>
    /// Gets or sets the category.
    /// </summary>
    public string Category
    {
        get => fCategory;
        set => SetValue(ref fCategory, value, nameof(Category));
    }
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name
    {
        get => fName;
        set => SetValue(ref fName, value, nameof(Name));
    }
    /// <summary>
    /// Gets or sets the quantity.
    /// </summary>
    public int Quantity
    {
        get => fQuantity;
        set => SetValue(ref fQuantity, value, nameof(Quantity));
    }
    /// <summary>
    /// Gets or sets the amount.
    /// </summary>
    public decimal Amount
    {
        get => fAmount;
        set => SetValue(ref fAmount, value, nameof(Amount));
    }

    // ● events
    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;
}
