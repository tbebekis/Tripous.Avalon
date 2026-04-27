namespace Tripous.Desktop;

public class ControlBinding
{
    // ● private
    private Control fControl;
    private string fColumnName;

    // ● constructor
    public ControlBinding(Control Control, string ColumnName)
    {
        fControl = Control ?? throw new ArgumentNullException(nameof(Control));
        fColumnName = ColumnName ?? throw new ArgumentNullException(nameof(ColumnName));
    }

    // ● public
    public Control Control => fControl;
    public string ColumnName => fColumnName;
    public FieldDef FieldDef { get; set; }
    public ILookupSource LookupSource { get; set; }
    public bool IsRefreshing { get; set; }
    public Action DisposeAction { get; set; }

    public void Dispose()
    {
        DisposeAction?.Invoke();
    }
}