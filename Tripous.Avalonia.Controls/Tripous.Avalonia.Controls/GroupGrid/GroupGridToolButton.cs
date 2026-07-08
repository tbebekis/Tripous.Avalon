// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Describes a compact button displayed in the group grid toolbar band.
/// </summary>
public class GroupGridToolButton: INotifyPropertyChanged
{
    // ● private fields
    string fName = string.Empty;
    string fText = string.Empty;
    string fToolTip = string.Empty;
    bool fIsVisible = true;
    bool fIsEnabled = true;
    GroupGridToolButtonAlignment fAlignment;
    object fTag;

    // ● private methods
    bool SetValue<T>(ref T Field, T Value, string PropertyName)
    {
        if (EqualityComparer<T>.Default.Equals(Field, Value))
            return false;

        Field = Value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        return true;
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridToolButton"/> class.
    /// </summary>
    public GroupGridToolButton()
    {
    }

    // ● properties
    /// <summary>
    /// Gets or sets the button name.
    /// </summary>
    public string Name
    {
        get => fName;
        set => SetValue(ref fName, value ?? string.Empty, nameof(Name));
    }
    /// <summary>
    /// Gets or sets the compact display text.
    /// </summary>
    public string Text
    {
        get => fText;
        set => SetValue(ref fText, value ?? string.Empty, nameof(Text));
    }
    /// <summary>
    /// Gets or sets the tooltip text.
    /// </summary>
    public string ToolTip
    {
        get => fToolTip;
        set => SetValue(ref fToolTip, value ?? string.Empty, nameof(ToolTip));
    }
    /// <summary>
    /// Gets or sets a value indicating whether the button is visible.
    /// </summary>
    public bool IsVisible
    {
        get => fIsVisible;
        set => SetValue(ref fIsVisible, value, nameof(IsVisible));
    }
    /// <summary>
    /// Gets or sets a value indicating whether the button is enabled.
    /// </summary>
    public bool IsEnabled
    {
        get => fIsEnabled;
        set => SetValue(ref fIsEnabled, value, nameof(IsEnabled));
    }
    /// <summary>
    /// Gets or sets the button alignment.
    /// </summary>
    public GroupGridToolButtonAlignment Alignment
    {
        get => fAlignment;
        set => SetValue(ref fAlignment, value, nameof(Alignment));
    }
    /// <summary>
    /// Gets or sets custom user data.
    /// </summary>
    public object Tag
    {
        get => fTag;
        set => SetValue(ref fTag, value, nameof(Tag));
    }

    // ● events
    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}
