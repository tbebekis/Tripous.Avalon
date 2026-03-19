using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tripous.Avalon.Data;

/// <summary>
/// A specialized ObservableCollection that supports batch loading operations 
/// by temporarily suspending collection change notifications.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
public class BindingSourceRowCollection<T> : ObservableCollection<T>
{
    private bool fIsSuspended;

    /// <summary>
    /// Initializes a new instance of the DataSourceRowCollection class.
    /// </summary>
    public BindingSourceRowCollection() : base()
    {
        this.fIsSuspended = false;
    }

    /// <summary>
    /// Clears the collection and loads a new range of items efficiently.
    /// Only a single Reset notification is sent after the operation is complete.
    /// </summary>
    /// <param name="Items">The sequence of items to load into the collection.</param>
    public void LoadRange(IEnumerable<T> Items)
    {
        if (Items == null) return;

        this.fIsSuspended = true;
        try
        {
            this.Clear();
            foreach (var Item in Items)
            {
                this.Items.Add(Item);
            }
        }
        finally
        {
            this.fIsSuspended = false;
            // Send a SINGLE event indicating that the entire list has changed (Reset)
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
    
    /// <summary>
    /// Raises the CollectionChanged event unless notifications are currently suspended.
    /// </summary>
    /// <param name="E">The event arguments.</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs E)
    {
        // If we are in a "Suspended" state, do not let the event propagate
        if (!this.fIsSuspended)
        {
            base.OnCollectionChanged(E);
        }
    }
}