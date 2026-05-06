namespace Tripous.Desktop;

/// <summary>
/// Helper for the Ui
/// </summary>
static public class Ui
{
    // ● construction
    /// <summary>
    /// Static constructor
    /// </summary>
    static Ui()
    {
        Sys.DebugProc = Ui.Debug;
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

    // ● TreeView
    /// <summary>
    /// Expands or collapses all items in a TreeView.
    /// </summary>
    static public void ExpandAll(this TreeView tv, bool Flag) => ExpandAll(tv as ItemsControl, Flag);
    /// <summary>
    /// Expands or collapses all items in TreeViewItem.
    /// </summary>
    static public void ExpandAll(this TreeViewItem Node, bool Flag)=> ExpandAll(Node as ItemsControl, Flag);
    /// <summary>
    /// Expands or collapses all items in a TreeView or TreeViewItem.
    /// </summary>
    static public void ExpandAll(ItemsControl Control, bool Flag)
    {
        if (Control == null)
            return;

        foreach (object Item in Control.Items)
        {
            // ● Get the visual container for the data item
            TreeViewItem Container = Control.ContainerFromItem(Item) as TreeViewItem;

            if (Container != null)
            {
                // ● Set the expansion flag
                Container.IsExpanded = Flag;

                // ● Recursive call to handle children
                ExpandAll(Container, Flag);
            }
        }
    }
    
    /// <summary>
    /// Creates a <see cref="TreeViewItem"/> node with an image.
    /// </summary>
    static public TreeViewItem CreateTreeNode(string Caption, FontWeight FontWeight, string IconFile, object Tag, double Spacing = 5, int NegativeMargin = 0)
    {
        var Panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = Spacing,  };
        if (NegativeMargin > 0)
            Panel.Margin = new Thickness(-NegativeMargin, 0, 0, 0);
        
        if (!string.IsNullOrWhiteSpace(IconFile))
        {
            Image Img = Assets.FindImage16(IconFile);
            if (Img != null) 
                Panel.Children.Add(Img);
        }
        
        Panel.Children.Add(new TextBlock { Text = Caption, FontWeight = FontWeight  });
            
        var Node = new TreeViewItem { Header = Panel, Tag = Tag };
        return Node;
    }
    /// <summary>
    /// Creates a container <see cref="TreeViewItem"/> node with an image.
    /// </summary>
    static public TreeViewItem CreateContainerNode(string Caption, object Tag = null, string IconFile = "folder16.png", double Spacing = 5, int NegativeMargin = -8)
    {
        TreeViewItem Result = CreateTreeNode(Caption, FontWeight.SemiBold, IconFile, Tag, Spacing: Spacing, NegativeMargin: NegativeMargin);
        return Result;
    }
    /// <summary>
    /// Creates a leaf <see cref="TreeViewItem"/> node with an image.
    /// </summary>
    static public TreeViewItem CreateLeafNode(string Caption, object Tag = null, string IconFile = "item16.png", double Spacing = 5, int NegativeMargin = 0)
    {
        TreeViewItem Result = CreateTreeNode(Caption, FontWeight.Normal, IconFile, Tag, Spacing: Spacing, NegativeMargin: NegativeMargin);
        return Result;
    }
    
    // ● miscs
    static public void Debug(string Text)
    {
        if (Sys.DebugMode)
        {
            if (LogBox.IsInitialized)
                LogBox.AppendLine(Text);
            else
                System.Diagnostics.Debug.WriteLine(Text);
        }
    }
    static public void Debug(Exception e)
    {
        if (Sys.DebugMode)
        {
            Debug(e.ToString());
        }
    }

    /// <summary>
    /// Executes an action on the UI thread (fire-and-forget).
    /// <para>Supports both synchronous and asynchronous delegates.</para>
    /// <c>Ui.Post(async () => await DoSomethingAsync());</c>
    /// </summary>
    static public void Post(Action Proc) => Post(Proc, DispatcherPriority.Background);
    /// <summary>
    /// Executes an action on the UI thread (fire-and-forget).
    /// <para>Supports both synchronous and asynchronous delegates.</para>
    /// <c>Ui.Post(async () => await DoSomethingAsync());</c>
    /// </summary>
    static public void Post(Action Proc, DispatcherPriority Priority)
    {
        if (Proc != null)
            Dispatcher.UIThread.Post(() => Proc(), Priority);
    }

    /// <summary>
    /// Executes an action on the UI thread (fire-and-forget).
    /// <para>Supports both synchronous and asynchronous delegates.</para>
    /// <c>Ui.Post(async () => await DoSomethingAsync());</c>
    /// </summary>
    static public void Post(Func<Task> Func) => Post(Func, DispatcherPriority.Background);
    /// <summary>
    /// Executes an action on the UI thread (fire-and-forget).
    /// <para>Supports both synchronous and asynchronous delegates.</para>
    /// <c>Ui.Post(async () => await DoSomethingAsync());</c>
    /// </summary>
    static public void Post(Func<Task> Func, DispatcherPriority Priority)
    {
        if (Func != null)
            Dispatcher.UIThread.Post(() => Func(), Priority);
    }
    
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
    /// Ui global settings
    /// </summary>
    static public UiGlobalSettings Settings { get; } = new();
}


