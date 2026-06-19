# Json

`Json` is the Tripous helper around `System.Text.Json`.

It centralizes the JSON options used by the framework and adds convenience methods for common framework tasks:

- serializing and deserializing objects
- loading JSON into an existing instance
- saving and loading JSON files
- cloning and assigning objects through JSON
- formatting JSON text
- converting JSON to dictionaries, dynamic values, nodes and streams

The helper is used by settings classes, descriptors, configuration values, logging and dynamic objects.

## Default Options

By default `Json` uses formatted output, case-insensitive property names, string enum conversion and invariant number handling.

```csharp
string JsonText = Json.Serialize(Instance);
MyOptions Options = Json.Deserialize<MyOptions>(JsonText);
```

Custom options may be created for one operation.

```csharp
JsonSerializerOptions Options = Json.CreateJsonOptions(
    CameCase: true,
    Formatted: false,
    Decimals: 4);

string JsonText = Json.Serialize(Instance, Options);
```

Applications may also replace the global options used by the helper.

```csharp
Json.SerializerOptions = Json.CreateJsonOptions(
    CameCase: false,
    Formatted: true,
    Decimals: 2);
```

## Loading Existing Instances

`Json.PopulateObject()` updates an existing instance instead of creating a new one.

This is important for settings and descriptor objects, because the object identity stays the same while its properties are reloaded from JSON.

```csharp
string JsonText = File.ReadAllText(SettingsFilePath);
Json.PopulateObject(this, JsonText);
```

This is the pattern used by `SettingsBase`.

```csharp
public virtual void Load()
{
    if (!File.Exists(SettingsFilePath))
        return;

    string JsonText = File.ReadAllText(SettingsFilePath);
    Json.PopulateObject(this, JsonText);
}
```

## Saving And Loading Files

Small tools and settings classes can use `Json.SaveToFile()` and `Json.LoadFromFile()` directly.

```csharp
public void Load() => Json.LoadFromFile(this, FilePath);
public void Save() => Json.SaveToFile(this, FilePath);
```

`SaveToFile()` creates the target folder when needed.

```csharp
Json.SaveToFile(ProjectSettings, FilePath);
```

To create an object from a JSON file, pass the target type.

```csharp
RegBuilderSettings Settings =
    Json.LoadFromFile(typeof(RegBuilderSettings), FilePath) as RegBuilderSettings;
```

## Descriptors And Copying

Tripous descriptors use JSON as a simple, consistent way to assign and clone descriptor objects.

```csharp
public virtual void Assign(IDef Source)
{
    Json.AssignObject(Source, this);
}
```

`AssignObject()` serializes the source and populates the destination.

```csharp
TableSqls Target = new();
Json.AssignObject(SourceSqls, Target);
```

`CloneObject()` creates a new instance by round-tripping through JSON.

```csharp
ModuleDef Clone = Json.CloneObject(SourceModuleDef);
```

## JsonLoaded Callback

Types that need to rebuild internal references after JSON loading may implement `IJsonLoadable`.

```csharp
public class MyDescriptor: IJsonLoadable
{
    public void JsonLoaded()
    {
        UpdateReferences();
    }
}
```

`Json.Deserialize()` and `Json.PopulateObject()` call `JsonLoaded()` automatically when the target object implements `IJsonLoadable`.

Tripous descriptor lists use this to restore owner/reference relationships after deserialization.

```csharp
public virtual void JsonLoaded() => UpdateReferences();
```

## Stored Configuration Objects

`Json` is also used when application configuration stores a complex object as text.

```csharp
public void LoadValue(ConfigPropertyDef Def, string Value)
{
    fDefaults = string.IsNullOrWhiteSpace(Value)
        ? new AppDefaultProperties()
        : Json.Deserialize<AppDefaultProperties>(Value);
}

public string SaveValue()
{
    return Json.Serialize(fDefaults);
}
```

This keeps the configuration system scalar at the database level while still allowing structured application settings.

## Formatting And Dynamic JSON

`Json.Format()` is useful when a stored JSON value must be displayed to a developer or written to a readable file.

```csharp
string Pretty = Json.Format(JsonText);
```

For light inspection, JSON can be converted to a dictionary, a `JsonNode`, or a dynamic object.

```csharp
Dictionary<string, string> Values = Json.ToDictionary(JsonText);
JsonNode Node = Json.ObjectToJsonNode(Instance);
dynamic DynamicValue = Json.ToDynamic(JsonText);
```

## Typical Usage

- Use `Json.Serialize()` and `Json.Deserialize<T>()` for normal object round-trips.
- Use `Json.PopulateObject()` when the existing instance must be preserved.
- Use `Json.SaveToFile()` and `Json.LoadFromFile()` for small settings files and tool state.
- Use `IJsonLoadable` when an object must rebuild references after loading.
- Use `Json.AssignObject()` for descriptor-style copying.
- Avoid using JSON copying for performance-sensitive paths or database row processing.
