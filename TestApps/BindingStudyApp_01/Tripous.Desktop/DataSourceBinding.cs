namespace Tripous.Desktop;

/// <summary>
/// Stores information and lifecycle for a DataSource UI binding.
/// </summary>
public class DataSourceBinding: IDisposable
{
    // ● private fields
    IDisposable fSubscription;
    bool fDisposed;

    // ● constructors
    /// <summary>
    /// Initializes a new control binding.
    /// </summary>
    public DataSourceBinding(DataSource Source, Control Control, string FieldName, AvaloniaProperty TargetProperty, IDisposable Subscription)
    {
        this.Source = Source;
        this.Control = Control;
        this.FieldName = FieldName;
        this.TargetProperty = TargetProperty;
        this.Kind = Control is DataGrid ? DataSourceBindingKind.Grid : DataSourceBindingKind.Control;
        fSubscription = Subscription;
    }
    /// <summary>
    /// Initializes a new grid column binding.
    /// </summary>
    public DataSourceBinding(DataSource Source, DataGridColumn GridColumn, string FieldName, IDisposable Subscription)
    {
        this.Source = Source;
        this.GridColumn = GridColumn;
        this.FieldName = FieldName;
        this.Kind = DataSourceBindingKind.GridColumn;
        fSubscription = Subscription;
    }

    // ● public
    /// <summary>
    /// Disposes the binding subscription.
    /// </summary>
    public void Dispose()
    {
        if (fDisposed)
            return;

        fSubscription?.Dispose();
        fDisposed = true;
    }

    // ● properties
    /// <summary>
    /// Gets the bound DataSource.
    /// </summary>
    public DataSource Source { get; }
    /// <summary>
    /// Gets the bound control.
    /// </summary>
    public Control Control { get; }
    /// <summary>
    /// Gets the bound grid column.
    /// </summary>
    public DataGridColumn GridColumn { get; }
    /// <summary>
    /// Gets the bound field name.
    /// </summary>
    public string FieldName { get; }
    /// <summary>
    /// Gets the target Avalonia property.
    /// </summary>
    public AvaloniaProperty TargetProperty { get; }
    /// <summary>
    /// Gets the binding kind.
    /// </summary>
    public DataSourceBindingKind Kind { get; }
    /// <summary>
    /// Gets a value indicating whether this is a control binding.
    /// </summary>
    public bool IsControlBinding => Control != null;
    /// <summary>
    /// Gets a value indicating whether this is a grid column binding.
    /// </summary>
    public bool IsGridColumnBinding => GridColumn != null;
    /// <summary>
    /// Gets a value indicating whether this binding is disposed.
    /// </summary>
    public bool IsDisposed => fDisposed;
}

/// <summary>
/// Wraps a custom dispose action as an IDisposable subscription.
/// </summary>
public class DataSourceBindingSubscription: IDisposable
{
    // ● private fields
    Action fDisposeProc;
    bool fDisposed;

    // ● constructors
    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public DataSourceBindingSubscription(Action DisposeProc)
    {
        fDisposeProc = DisposeProc;
    }

    // ● public
    /// <summary>
    /// Executes the stored dispose action.
    /// </summary>
    public void Dispose()
    {
        if (fDisposed)
            return;

        fDisposeProc?.Invoke();
        fDisposed = true;
    }
}
