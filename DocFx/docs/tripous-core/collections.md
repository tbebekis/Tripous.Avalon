# Collections

`TripousList<T>` is the base observable collection used by Tripous descriptor lists.
It wraps a normal `List<T>`, but adds collection change notifications, property change notifications, and overridable validation hooks.

The most important derived class is `DefList<T>`.

## Basic Use

`TripousList<T>` implements both generic and non-generic collection interfaces.

```csharp
TripousList<string> Items = new();

Items.Add("One");
Items.Add("Two");

string First = Items[0];
int Count = Items.Count;
```

It supports the normal list operations:

- `Add()`.
- `Insert()`.
- `Remove()`.
- `RemoveAt()`.
- `AddRange()`.
- `Clear()`.
- `IndexOf()`.
- `Contains()`.
- Indexer access.

## Change Notifications

`TripousList<T>` implements `INotifyCollectionChanged`.
This allows UI code or other observers to react when items are added, removed, replaced, or when the list is reset.

```csharp
TripousList<string> Items = new();

Items.CollectionChanged += (Sender, Args) =>
{
    NotifyCollectionChangedAction Action = Args.Action;
};

Items.Add("One");
```

It also implements `INotifyPropertyChanged`.
The `Count` property is raised when items are added, removed, or the list is cleared.

```csharp
Items.PropertyChanged += (Sender, Args) =>
{
    if (Args.PropertyName == nameof(Items.Count))
    {
        int Count = Items.Count;
    }
};
```

## Duplicate Instance Protection

By default, `TripousList<T>` does not allow the same item instance to be added twice.

```csharp
TripousList<object> Items = new();
object Item = new();

Items.Add(Item);
Items.Add(Item);
```

The second `Add()` throws.
This protects collection state and keeps notifications easier to reason about.

## Validation Hooks

Derived classes can override `CheckAdding()` and `CheckRemoving()`.

```csharp
/// <summary>
/// Represents a list of non-empty names.
/// </summary>
public class NameList : TripousList<string>
{
    // ● overridables
    /// <summary>
    /// Checks whether an item can be added.
    /// </summary>
    protected override void CheckAdding(string Item)
    {
        base.CheckAdding(Item);

        if (string.IsNullOrWhiteSpace(Item))
            throw new TripousException("Name is required.");
    }
}
```

`DefList<T>` uses this pattern to reject descriptors without a name and to prevent duplicate descriptor names.

```csharp
protected override void CheckAdding(T Def)
{
    base.CheckAdding(Def);

    if (string.IsNullOrWhiteSpace(Def.Name))
        throw new TripousArgumentNullException(nameof(Def.Name));
}
```

Use `CheckRemoving()` when a derived list needs to prevent removing protected items.

## Replacing Items

The indexer can replace an item.
When the value changes, `TripousList<T>` raises a replace notification.

```csharp
TripousList<string> Items = new();

Items.Add("One");
Items[0] = "Two";
```

The replacement still passes through `CheckAdding()`.

## Clear And Reset

`Clear()` removes all items and raises a reset notification.

```csharp
TripousList<string> Items = new();

Items.Add("One");
Items.Add("Two");
Items.Clear();
```

If the list is already empty, `Clear()` does nothing.

## Where Tripous Uses It

Tripous uses `TripousList<T>` as a base class rather than directly in many places.
The main use is `DefList<T>`, which adds descriptor-specific name lookup and validation.

```csharp
public class DefList<T> : TripousList<T>, IJsonLoadable where T : IDef
{
}
```

That means descriptor collections get the common collection behavior from `TripousList<T>`:

- Add/remove notifications.
- `Count` notifications.
- Duplicate instance protection.
- Validation hooks.
- Generic and non-generic list support.

## When To Use It

Use `TripousList<T>` when a custom list needs observable behavior and validation hooks.

Use `DefList<T>` instead when the items are descriptors.
`DefList<T>` adds the descriptor-specific rules that `TripousList<T>` intentionally does not know about.

Use `List<T>` when none of these features are needed.
