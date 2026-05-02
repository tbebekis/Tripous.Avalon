namespace Tripous.Desktop;

/// <summary>
/// Helper for the Ui
/// </summary>
static public class Ui
{
    static Stretch fFormImageStretch = Stretch.Uniform;
    static int fFormMemoRowCount;
    static double fFormColumnWidth;
    static int fFormColumnCount;
    static int fFormImageHeight;
    
    static double GetCurrentScreenWidth()
    {
        if (MainWindow != null)
        {
            var screen = MainWindow.Screens.ScreenFromVisual(MainWindow);
            if (screen != null)
                return screen.WorkingArea.Width;
        }
            
        return 1024;  
    }
    static double GetAvailableScreenWidth()
    {
        double ScreenWidth = GetCurrentScreenWidth();
        double SideBarWidth = 200; 
        double Result = ScreenWidth - SideBarWidth;
        return Result;
    }
    static int GetFormDefaultColumnCount()
    {
        double AvailableWidth = GetAvailableScreenWidth();
        int Result = AvailableWidth > 1100 ? 3 : 2;
        return Result;
    }
    static double GetFormDefaultColumnWidth()
    {
        double AvailableWidth = GetAvailableScreenWidth();
        int ColumnCount = GetFormDefaultColumnCount();
        double Result = AvailableWidth / ColumnCount;
        return Result;
    }
    
    // ● images
    
    /// <summary>
    /// Returns true if an image resource path exists, e.g. <c>avares://Tripous.Desktop/Images/MyImage.png</c>
    /// </summary>
    static public Uri FindImageUriByPath(string ImageResourcePath)
    {
        if (System.Uri.TryCreate(ImageResourcePath, UriKind.Absolute, out Uri Uri))
            if (AssetLoader.Exists(Uri))
                return Uri;
        return null;
    }
    /// <summary>
    /// Returns true if if finds an image resource along with its full path.
    /// </summary>
    static public Uri FindImageUri(Assembly Assembly, string FileName)
    {
        Uri Result = null;
        
        if (!string.IsNullOrWhiteSpace(FileName) && (Assembly != null))
        {
            string AssemblyName = Assembly.GetName().Name;
            string S = $"avares://{AssemblyName}/Images/{FileName}";
            Result = FindImageUriByPath(S);
        }

        return Result;
    }
    /// <summary>
    /// Finds and returns the full path of an image resource, if any, else null.
    /// </summary>
    static public Uri FindImageUri(string FileName)
    {
        if (string.IsNullOrWhiteSpace(FileName))
            return null;
        
        Uri Result = FindImageUriByPath(FileName);
        
        if (Result == null)
            Result = FindImageUri(Assembly.GetEntryAssembly(), FileName);
        
        if (Result == null)
            Result = FindImageUri(Assembly.GetCallingAssembly(), FileName);

        if (Result == null)
            Result = FindImageUri(typeof(Ui).Assembly, FileName);

        return Result;
    }

    static public bool SetImage(Image Image, string FileName)
    {
        if (Image != null)
        {
            Uri Uri = FindImageUri(FileName);
            if (Uri != null)
            {
                Image.Source = new Bitmap(AssetLoader.Open(Uri));
                return true;
            }
        }
        return false;
    }
    static public bool SetImage(this Button Button, string FileName)
    {
        if (Button != null)
        {
            Uri Uri = FindImageUri(FileName);
            if (Uri != null)
            {
                Button.Content = new Image() { Source = new Bitmap(AssetLoader.Open(Uri)) };
                return true;
            }
        }

        return false;
    }
    static public bool SetImage(this MenuItem MenuItem, string FileName)
    {
        if (MenuItem != null)
        {
            Uri Uri = FindImageUri(FileName);
            if (Uri != null)
            {
                MenuItem.Icon = new Image() { Source = new Bitmap(AssetLoader.Open(Uri)) };
                return true;
            }
        }

        return false;
    }
 
    // ● dialogs
    static public Window GetParentWindow(this Control Control) => TopLevel.GetTopLevel(Control) as Window;
    
    static public async Task<string> SaveFileDialog(Control Caller, params string[] Extensions)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;

        Window ParentWindow = Caller is Window? Caller as Window: Caller.GetParentWindow(); 
 
        if (ParentWindow == null)
            return null;

        var topLevel = TopLevel.GetTopLevel(ParentWindow);
        if (topLevel?.StorageProvider == null)
            return null;

        Extensions ??= Array.Empty<string>();

        var fileTypes = new List<FilePickerFileType>();

        foreach (string ext in Extensions.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            string cleanExt = ext.Trim().TrimStart('.').ToLowerInvariant();

            fileTypes.Add(new FilePickerFileType($"{cleanExt.ToUpper()} files")
            {
                Patterns = new[] { $"*.{cleanExt}" }
            });
        }

        // All files *.*
        fileTypes.Add(new FilePickerFileType("All files")
        {
            Patterns = new[] { "*.*" }
        });

        var options = new FilePickerSaveOptions
        {
            Title = "Save file",
            SuggestedFileName = Extensions.Length > 0 ? $"file.{Extensions[0].TrimStart('.')}" : "file",
            DefaultExtension = Extensions.Length > 0 ? Extensions[0].TrimStart('.') : null,
            FileTypeChoices = fileTypes
        };

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(options);

        return file?.Path?.LocalPath;
    }
    static public async Task<string> OpenFileDialog(Control Caller,params string[] Extensions)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;

        Window ParentWindow = Caller is Window? Caller as Window: Caller.GetParentWindow(); 
 
        if (ParentWindow == null)
            return null;

        var topLevel = TopLevel.GetTopLevel(ParentWindow);
        if (topLevel?.StorageProvider == null)
            return null;

        Extensions ??= Array.Empty<string>();

        var fileTypes = new List<FilePickerFileType>();

        foreach (string ext in Extensions.Where(x => !string.IsNullOrWhiteSpace(x)))
        {
            string cleanExt = ext.Trim().TrimStart('.').ToLowerInvariant();

            fileTypes.Add(new FilePickerFileType($"{cleanExt.ToUpper()} files")
            {
                Patterns = new[] { $"*.{cleanExt}" }
            });
        }

        // All files *.*
        fileTypes.Add(new FilePickerFileType("All files")
        {
            Patterns = new[] { "*.*" }
        });

        var options = new FilePickerOpenOptions
        {
            Title = "Open file",
            AllowMultiple = false,
            FileTypeFilter = fileTypes
        };

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(options);

        if (files == null || files.Count == 0)
            return null;

        return files[0]?.Path?.LocalPath;
    }
    static public async Task<DialogData> InputBox(string Message, string Value = "", Control Caller = null)
    {
        return await Desktop.InputBox.ShowModal(Message, Value, Caller);
    }

    // ● miscs
    static public void ShowWaitCursor(Action Proc, Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        
        var top = TopLevel.GetTopLevel(Caller);
        if (top == null)
        {
            Proc();
        }
        else
        {
            top.Cursor = new Cursor(StandardCursorType.Wait);
            try
            {
                Proc();
            }
            finally
            {
                top.Cursor = new Cursor(StandardCursorType.Arrow);
            }
        }

    }
    static public void ShowWaitCursor<T>(Action<T> Proc, T Info, Control Caller = null)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;
        
        var top = TopLevel.GetTopLevel(Caller);   
        if (top == null)
        {
            Proc(Info);
        }
        else
        {
            top.Cursor = new Cursor(StandardCursorType.Wait);
            try
            {
                Proc(Info);
            }
            finally
            {
                top.Cursor = new Cursor(StandardCursorType.Arrow);
            }
        }

    }
    
    // ● properties
    /// <summary>
    /// The main windows
    /// </summary>
    static public Window MainWindow { get; set; }
  
    /// <summary>
    /// How many columns a form layout may have in an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    static public int FormColumnCount
    {
        get => fFormColumnCount >= 250 && fFormColumnCount <= 600 ? fFormColumnCount : GetFormDefaultColumnCount();
        set => fFormColumnCount = value;
    }
    /// <summary>
    /// Column width of a form column in an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    static public double FormColumnWidth
    {
        get => fFormColumnWidth >= 250 && fFormColumnWidth <= 600 ? fFormColumnWidth : GetFormDefaultColumnWidth();
        set => fFormColumnWidth = value;
    }
    /// <summary>
    /// How many rows a control, for a <see cref="FieldDef.IsMemo"/> field, occupies in a column of an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    static public int FormMemoRowCount
    {
        get => fFormMemoRowCount >= 3 && fFormMemoRowCount <= 5 ? fFormMemoRowCount : 3;
        set => fFormMemoRowCount = value;
    }
    /// <summary>
    /// The height of an image control, for a <see cref="FieldDef.IsImage"/> field,  of an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    static public int FormImageHeight
    {
        get => fFormImageHeight >= 80 && fFormImageHeight <= 300 ? fFormImageHeight : 160;
        set => fFormImageHeight = value;
    }
    /// <summary>
    /// The stretch mode of an image control in an <see cref="ItemPage"/> of a <see cref="DataForm"/>
    /// </summary>
    static public Stretch FormImageStretch
    {
        get => fFormImageStretch;
        set => fFormImageStretch = value;
    }
 
}


