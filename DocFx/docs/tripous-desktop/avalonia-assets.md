# AvaloniaAssets

`AvaloniaAssets` is the Tripous.Desktop helper for locating Avalonia embedded resources and creating image controls from them.
It is used mostly for toolbar icons, message box icons, tree nodes, and command images.

## Resource Folders

Tripous.Desktop embeds icon folders as Avalonia resources.

```xml
<AvaloniaResource Include="Images32\**" />
<AvaloniaResource Include="Images16\**" />
```

The default asset search checks these folders:

- `Images32`
- `Images16`
- `Images`
- `Assets`
- `Binaries`
- `Files`

The search is not limited to `Tripous.Desktop`.
It also scans the application assemblies, with the Tripous.Desktop assembly and the calling assembly prioritized.

## Finding Asset URIs

Use `FindUri()` when code needs the Avalonia asset URI.

```csharp
Uri Uri = AvaloniaAssets.FindUri("table.png");
```

You can also search a specific folder.

```csharp
Uri Uri = AvaloniaAssets.FindUri("Images16", "table16.png");
```

The resulting URI is an `avares://` URI, such as:

```text
avares://Tripous.Desktop/Images32/table.png
```

Use `FindUriByPath()` when you already have a full asset path.

## Creating Images

Use `FindImage()` when an Avalonia `Image` control is needed.

```csharp
Image Image = AvaloniaAssets.FindImage("table.png");
```

For fixed icon sizes:

```csharp
Image Icon16 = AvaloniaAssets.FindImage16("table16.png");
Image Icon32 = AvaloniaAssets.FindImage32("table.png");
```

For a custom size:

```csharp
Image Image = AvaloniaAssets.FindImage(
    "table.png",
    new Size(24, 24));
```

The size behavior is controlled by `ImageSizeType`.

- `Undefined` creates an image without fixed width or height.
- `Icon16` creates a 16x16 image.
- `Icon32` creates a 32x32 image.
- `Defined` creates an image with the supplied size.

## Setting Image Source

Use `SetImage()` when the `Image` control already exists.

```csharp
Image Image = new();
bool Found = AvaloniaAssets.SetImage(Image, "information.png");
```

`SetImage()` opens the asset stream and assigns a `Bitmap` to `Image.Source`.
It returns false when the image or URI cannot be found.

## Finding Raw Assets

Use `FindAsset()` when code needs a stream.

```csharp
using Stream Stream = AvaloniaAssets.FindAsset("script.sql");
```

The returned stream must be disposed by the caller.

## Toolbars And Commands

`ToolBar` uses `AvaloniaAssets.FindImage()` when creating buttons.

```csharp
ToolBar.AddButton(
    "table_refresh.png",
    "Refresh",
    Refresh);
```

For `Command` objects, `Command.ImageFileName` is passed to the same toolbar image lookup.

```csharp
Command Cmd = Command.CreateForm(
    "CustomerList",
    "Customer",
    ImageFileName: "table.png");
```

If the image is not found, the toolbar can fall back to text content based on the tooltip.

## Message Boxes

`MessageBox` uses `AvaloniaAssets.SetImage()` for its mode icon.

```csharp
AvaloniaAssets.SetImage(imgIcon, "information.png");
```

Standard message box icon file names include:

- `information.png`
- `error.png`
- `emotion_question.png`

## Tree Nodes

`Ui.CreateTreeNode()` uses 16x16 icons.

```csharp
TreeViewItem Node = Ui.CreateTreeNode(
    "Customers",
    FontWeight.Normal,
    "table16.png",
    Tag: null);
```

If the icon is not found, the node is still created with text only.

## Practical Notes

- Use file names, not full paths, for normal icon lookup.
- Put common 32x32 icons under `Images32`.
- Put small tree icons under `Images16`.
- Use `FindImage16()` and `FindImage32()` when the UI surface expects a stable icon size.
- Dispose streams returned by `FindAsset()`.
