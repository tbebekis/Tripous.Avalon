# DynamicClass

`DynamicClass` is a small helper for objects whose properties are not known at compile time.
It combines three ideas in one type:

- A dictionary-backed property bag.
- C# `dynamic` member access.
- Runtime property metadata through `ICustomTypeDescriptor`.

It is useful when a strongly typed class would be too rigid, but the object still has to behave like a normal object in serializers, data binding, or designer-style code.

## What It Provides

`DynamicClass` stores values in its `Properties` dictionary.
Each entry in that dictionary becomes a runtime property.

```csharp
DynamicClass Item = new();

Item["Name"] = "Customer";
Item["Enabled"] = true;

string Name = Item["Name"] as string;
```

The same object may also be used through the C# `dynamic` keyword.

```csharp
dynamic Item = new DynamicClass();

Item.Name = "Customer";
Item.Enabled = true;

string Name = Item.Name;
```

Internally both styles use the same dictionary.

## JSON Support

`DynamicClass` can be serialized and restored with JSON.
This makes it useful for small flexible payloads, simple metadata objects, or user-defined values.

```csharp
DynamicClass Item = new();

Item["Name"] = "Customer";
Item["Enabled"] = true;

string JsonText = Item.ToJson();
DynamicClass Loaded = new(JsonText);
```

The constructor that accepts JSON calls `FromJson()`, which fills the internal property dictionary.
Tripous code uses verbatim strings when a multi-line string is needed.

```csharp
DynamicClass Item = new();

Item.FromJson(@"
{
  ""Name"": ""Customer"",
  ""Enabled"": true
}");
```

## Change Notifications

`DynamicClass` implements `INotifyPropertyChanged`.
Changing a value through the indexer or through dynamic member assignment raises `PropertyChanged`.

```csharp
DynamicClass Item = new();

Item.PropertyChanged += (Sender, Args) =>
{
    string PropertyName = Args.PropertyName;
};

Item["Name"] = "Supplier";
```

This allows the class to participate in binding scenarios where the consumer listens for property changes.

## Runtime Property Metadata

The class also implements `ICustomTypeDescriptor`.
That means consumers can ask it for its current properties at runtime.

```csharp
DynamicClass Item = new();

Item["Name"] = "Customer";
Item["Enabled"] = true;

PropertyDescriptorCollection Properties = TypeDescriptor.GetProperties(Item);
```

This is useful for code that works with `PropertyDescriptor` objects instead of direct C# properties.
For example, a property-grid style UI can inspect the object and discover the currently available dynamic fields.

## When To Use It

Use `DynamicClass` when the shape of the object is intentionally flexible.

- Small metadata objects.
- Optional values that are not worth a dedicated class.
- Runtime-defined property bags.
- JSON-backed dynamic payloads.
- UI or tooling code that needs `PropertyDescriptor` support.

Avoid it when the object has a stable contract.
In that case a normal class is clearer, safer, and easier to document.

```csharp
/// <summary>
/// Represents basic customer information.
/// </summary>
public class CustomerInfo
{
    public string Code { get; set; }
    public string Name { get; set; }
}
```

Use a real type when the rest of the application depends on specific properties being present.

## Clearing Values

All dynamic properties can be removed with `RemoveAllProperties()`.

```csharp
DynamicClass Item = new();

Item["Name"] = "Customer";
Item["Enabled"] = true;

Item.RemoveAllProperties();
```

After that call the object is still valid, but its property dictionary is empty.

## Notes

`DynamicClass` is deliberately lightweight.
It does not validate property names, enforce a schema, or convert values to target types.
The caller owns those rules.

That is the main trade-off:

- It is flexible and easy to fill dynamically.
- It is less safe than a normal strongly typed class.

For stable domain objects, prefer explicit classes.
For flexible runtime data, `DynamicClass` is a practical Tripous Core utility.
