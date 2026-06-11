/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Adapts a <see cref="DataView"/> to an observable item source suitable for Avalonia grids.
/// </summary>
public class DataViewItemsSource: ObservableCollection<DataRowView>, IDisposable
{
    // ● private fields
    private bool fDisposed;
    private bool fReloading;
    private bool fReloadPending;

    // ● private
    private void DataView_ListChanged(object sender, ListChangedEventArgs e)
    {
        if (fReloading)
        {
            fReloadPending = true;
            return;
        }

        switch (e.ListChangedType)
        {
            case ListChangedType.ItemAdded:
                if (e.NewIndex >= 0 && e.NewIndex <= Count && e.NewIndex < DataView.Count)
                    Insert(e.NewIndex, DataView[e.NewIndex]);
                else
                    Reload();
                break;
            case ListChangedType.ItemDeleted:
                if (e.NewIndex >= 0 && e.NewIndex < Count)
                    RemoveAt(e.NewIndex);
                else
                    Reload();
                break;
            case ListChangedType.ItemMoved:
                if (e.OldIndex >= 0 && e.OldIndex < Count && e.NewIndex >= 0 && e.NewIndex < Count)
                    Move(e.OldIndex, e.NewIndex);
                else
                    Reload();
                break;
            case ListChangedType.ItemChanged:
                break;
            default:
                Reload();
                break;
        }
    }
    private void Reload()
    {
        if (fDisposed)
            return;

        List<DataRowView> Items = DataView.Cast<DataRowView>().ToList();
        fReloading = true;
        try
        {
            Clear();

            foreach (DataRowView RowView in Items)
                Add(RowView);
        }
        finally
        {
            fReloading = false;
        }

        if (fReloadPending)
        {
            fReloadPending = false;
            Ui.Post(Reload);
        }
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
