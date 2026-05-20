namespace Tripous.Desktop;

/// <summary>
/// Adapts a <see cref="DataView"/> to an observable item source suitable for Avalonia grids.
/// </summary>
public class DataViewItemsSource: ObservableCollection<DataRowView>, IDisposable
{
    private bool fDisposed;

    // ● private
    private void DataView_ListChanged(object sender, ListChangedEventArgs e)
    {
        Reload();
    }
    private void Reload()
    {
        if (fDisposed)
            return;

        Clear();

        foreach (DataRowView RowView in DataView)
            Add(RowView);
    }

    // ● constructor
    /// <summary>
    /// Constructor.
    /// </summary>
    public DataViewItemsSource(DataView DataView)
    {
        this.DataView = DataView ?? throw new TripousArgumentNullException(nameof(DataView));
        this.DataView.ListChanged += DataView_ListChanged;
        Reload();
    }

    // ● public
    /// <summary>
    /// Releases the subscription to the underlying <see cref="DataView"/>.
    /// </summary>
    public void Dispose()
    {
        if (fDisposed)
            return;

        DataView.ListChanged -= DataView_ListChanged;
        fDisposed = true;
    }

    // ● properties
    /// <summary>
    /// The adapted <see cref="DataView"/>.
    /// </summary>
    public DataView DataView { get; }
}
