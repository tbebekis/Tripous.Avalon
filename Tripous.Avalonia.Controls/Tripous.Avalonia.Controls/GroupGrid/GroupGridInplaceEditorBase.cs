// Copyright (c) 2026 Theodoros Bebekis
// Licensed under the MIT License.

namespace Avalonia.Controls;

/// <summary>
/// Provides the base host for a real in-place editor displayed over a group grid cell.
/// </summary>
public abstract class GroupGridInplaceEditorBase: Border
{
    // ● protected fields
    /// <summary>
    /// The hosted editor control.
    /// </summary>
    protected Control fEditorControl;

    // ● protected methods
    /// <summary>
    /// Sets the hosted editor control.
    /// </summary>
    /// <param name="EditorControl">The editor control.</param>
    protected void SetEditorControl(Control EditorControl)
    {
        fEditorControl = EditorControl ?? throw new ArgumentNullException(nameof(EditorControl));
        Child = fEditorControl;
    }
    /// <summary>
    /// Raises the <see cref="ValueChanged"/> event.
    /// </summary>
    protected void RaiseValueChanged()
    {
        ValueChanged?.Invoke(this, EventArgs.Empty);
    }
    /// <summary>
    /// Raises the <see cref="DropDownRequested"/> event.
    /// </summary>
    protected void RaiseDropDownRequested()
    {
        DropDownRequested?.Invoke(this, EventArgs.Empty);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="GroupGridInplaceEditorBase"/> class.
    /// </summary>
    protected GroupGridInplaceEditorBase()
    {
        Background = Brushes.Transparent;
        BorderThickness = new Thickness(0);
        Padding = new Thickness(0);
        Focusable = false;
    }

    // ● public methods
    /// <summary>
    /// Gives focus to the hosted editor control.
    /// </summary>
    public virtual void FocusEditor()
    {
        fEditorControl?.Focus();
    }
    /// <summary>
    /// Selects the editor content when supported.
    /// </summary>
    public virtual void SelectAll()
    {
    }
    /// <summary>
    /// Releases editor-specific event handlers and resources.
    /// </summary>
    public virtual void Cleanup()
    {
    }
    /// <summary>
    /// Selects a drop-down item.
    /// </summary>
    /// <param name="Item">The drop-down item.</param>
    public virtual void SelectDropDownItem(object Item)
    {
    }
    /// <summary>
    /// Creates a drop-down control for this editor.
    /// </summary>
    /// <returns>The drop-down control, or null when not supported.</returns>
    public virtual Control CreateDropDownControl()
    {
        return null;
    }
    /// <summary>
    /// Called when the editor drop-down opens.
    /// </summary>
    /// <param name="DropDownControl">The drop-down control.</param>
    public virtual void DropDownOpened(Control DropDownControl)
    {
    }
    /// <summary>
    /// Called when the editor drop-down closes.
    /// </summary>
    public virtual void DropDownClosed()
    {
    }

    // ● properties
    /// <summary>
    /// Gets the hosted editor control.
    /// </summary>
    public Control EditorControl => fEditorControl;
    /// <summary>
    /// Gets or sets the edited value.
    /// </summary>
    public abstract object Value { get; set; }
    /// <summary>
    /// Gets a value indicating whether the editor has a drop-down list.
    /// </summary>
    public virtual bool HasDropDown => false;
    /// <summary>
    /// Gets the drop-down items.
    /// </summary>
    public virtual IEnumerable DropDownItems => null;
    /// <summary>
    /// Gets the selected drop-down item.
    /// </summary>
    public virtual object SelectedDropDownItem => null;
    /// <summary>
    /// Gets the preferred drop-down height.
    /// </summary>
    public virtual double DropDownHeight => 0;
    /// <summary>
    /// Gets the preferred drop-down width.
    /// </summary>
    public virtual double DropDownWidth => 0;

    // ● events
    /// <summary>
    /// Occurs when the edited value changes.
    /// </summary>
    public event EventHandler ValueChanged;
    /// <summary>
    /// Occurs when the editor requests opening or closing its drop-down list.
    /// </summary>
    public event EventHandler DropDownRequested;
}
