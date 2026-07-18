/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Desktop;

/// <summary>
/// Provides UI helper methods and settings.
/// </summary>
static public partial class Ui
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
    /// <summary>
    /// Returns the parent window of a control.
    /// </summary>
    /// <param name="Control">The control.</param>
    /// <returns>The parent window, if any; otherwise, null.</returns>
    static public Window GetParentWindow(this Control Control) => TopLevel.GetTopLevel(Control) as Window;
    /// <summary>
    /// Returns the owner window of a control.
    /// </summary>
    /// <param name="Control">The control.</param>
    /// <returns>The owner window.</returns>
    static public Window GetOwnerWindow(this Control Control)
    {
        if (Control is Window Window)
            return Window;
        Window Result = Control != null ? Control.GetParentWindow() : null;
        return Result ?? Ui.MainWindow;
    }
    
    /// <summary>
    /// Shows a save file dialog.
    /// </summary>
    /// <param name="Caller">The caller control.</param>
    /// <param name="Extensions">The allowed file extensions.</param>
    /// <returns>The selected file path, if any; otherwise, null.</returns>
    static public async Task<string> SaveFileDialog(Control Caller, params string[] Extensions)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;

        Window ParentWindow = Caller.GetOwnerWindow();
 
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
    /// <summary>
    /// Shows an open file dialog.
    /// </summary>
    /// <param name="Caller">The caller control.</param>
    /// <param name="Extensions">The allowed file extensions.</param>
    /// <returns>The selected file path, if any; otherwise, null.</returns>
    static public async Task<string> OpenFileDialog(Control Caller,params string[] Extensions)
    {
        if (Caller == null)
            Caller = Ui.MainWindow;

        Window ParentWindow = Caller.GetOwnerWindow();
 
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
    /// <summary>
    /// Shows an input box dialog.
    /// </summary>
    /// <param name="Message">The dialog message.</param>
    /// <param name="Value">The initial value.</param>
    /// <param name="Caller">The caller control.</param>
    /// <returns>The input box data.</returns>
    static public async Task<InputBoxData> InputBox(string Message, string Value = "", Control Caller = null)
    {
        return await Desktop.InputBox.ShowModal(Message, Value, Caller);
    }

    // ● TreeView
    /// <summary>
    /// Expands or collapses all items in a TreeView.
    /// </summary>
    /// <param name="tv">The tree view.</param>
    /// <param name="Flag">True to expand; false to collapse.</param>
    static public void ExpandAll(this TreeView tv, bool Flag) => ExpandAll(tv as ItemsControl, Flag);
    /// <summary>
    /// Expands or collapses all items in TreeViewItem.
    /// </summary>
    /// <param name="Node">The tree view item.</param>
    /// <param name="Flag">True to expand; false to collapse.</param>
    static public void ExpandAll(this TreeViewItem Node, bool Flag)=> ExpandAll(Node as ItemsControl, Flag);
    /// <summary>
    /// Expands or collapses all items in a TreeView or TreeViewItem.
    /// </summary>
    /// <param name="Control">The items control.</param>
    /// <param name="Flag">True to expand; false to collapse.</param>
    static public void ExpandAll(ItemsControl Control, bool Flag)
    {
        if (Control == null)
            return;

        foreach (object Item in Control.Items)
        {
            // ● Get the visual container for the data item
            TreeViewItem Container = Item as TreeViewItem ?? Control.ContainerFromItem(Item) as TreeViewItem;

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
    /// <param name="Caption">The node caption.</param>
    /// <param name="FontWeight">The caption font weight.</param>
    /// <param name="IconFile">The icon file name.</param>
    /// <param name="Tag">The node tag.</param>
    /// <param name="Spacing">The content spacing.</param>
    /// <param name="NegativeMargin">The negative left margin.</param>
    /// <returns>The created tree view item.</returns>
    static public TreeViewItem CreateTreeNode(string Caption, FontWeight FontWeight, string IconFile, object Tag, double Spacing = 5, int NegativeMargin = 0)
    {
        var Panel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = Spacing,  };
        if (NegativeMargin > 0)
            Panel.Margin = new Thickness(-NegativeMargin, 0, 0, 0);
        
        if (!string.IsNullOrWhiteSpace(IconFile))
        {
            Image Img = AvaloniaAssets.FindImage16(IconFile);
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
    /// <param name="Caption">The node caption.</param>
    /// <param name="Tag">The node tag.</param>
    /// <param name="IconFile">The icon file name.</param>
    /// <param name="Spacing">The content spacing.</param>
    /// <param name="NegativeMargin">The negative left margin.</param>
    /// <returns>The created tree view item.</returns>
    static public TreeViewItem CreateContainerNode(string Caption, object Tag = null, string IconFile = "folder16.png", double Spacing = 5, int NegativeMargin = -8)
    {
        TreeViewItem Result = CreateTreeNode(Caption, FontWeight.SemiBold, IconFile, Tag, Spacing: Spacing, NegativeMargin: NegativeMargin);
        return Result;
    }
    /// <summary>
    /// Creates a leaf <see cref="TreeViewItem"/> node with an image.
    /// </summary>
    /// <param name="Caption">The node caption.</param>
    /// <param name="Tag">The node tag.</param>
    /// <param name="IconFile">The icon file name.</param>
    /// <param name="Spacing">The content spacing.</param>
    /// <param name="NegativeMargin">The negative left margin.</param>
    /// <returns>The created tree view item.</returns>
    static public TreeViewItem CreateLeafNode(string Caption, object Tag = null, string IconFile = "item16.png", double Spacing = 5, int NegativeMargin = 0)
    {
        TreeViewItem Result = CreateTreeNode(Caption, FontWeight.Normal, IconFile, Tag, Spacing: Spacing, NegativeMargin: NegativeMargin);
        return Result;
    }
    
    // ● miscs
    /// <summary>
    /// Writes debug text to the UI log or debug output.
    /// </summary>
    /// <param name="Text">The text to write.</param>
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
    /// <summary>
    /// Writes an exception to the debug output.
    /// </summary>
    /// <param name="e">The exception.</param>
    static public void Debug(Exception e)
    {
        if (Sys.DebugMode)
        {
            Debug(e.ToString());
        }
    }
    /// <summary>
    /// Displays a desktop notification.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <param name="Type">The notification type.</param>
    /// <returns>The created notification.</returns>
    static public DesktopNote Note(string Message, DesktopNoteType Type) => DesktopNote.Show(Message, Type);
    /// <summary>
    /// Displays an information desktop notification.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <returns>The created notification.</returns>
    static public DesktopNote InfoNote(string Message) => Note(Message, DesktopNoteType.Information);
    /// <summary>
    /// Displays a success desktop notification.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <returns>The created notification.</returns>
    static public DesktopNote SuccessNote(string Message) => Note(Message, DesktopNoteType.Success);
    /// <summary>
    /// Displays a warning desktop notification.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <returns>The created notification.</returns>
    static public DesktopNote WarningNote(string Message) => Note(Message, DesktopNoteType.Warning);
    /// <summary>
    /// Displays an error desktop notification.
    /// </summary>
    /// <param name="Message">The notification message.</param>
    /// <returns>The created notification.</returns>
    static public DesktopNote ErrorNote(string Message) => Note(Message, DesktopNoteType.Error);

    /// <summary>
    /// Executes an action on the UI thread (fire-and-forget).
    /// <para>Supports both synchronous and asynchronous delegates.</para>
    /// <c>Ui.Post(async () => await DoSomethingAsync());</c>
    /// </summary>
    /// <param name="Proc">The action to execute.</param>
    static public void Post(Action Proc) => Post(Proc, DispatcherPriority.Background);
    /// <summary>
    /// Executes an action on the UI thread (fire-and-forget).
    /// <para>Supports both synchronous and asynchronous delegates.</para>
    /// <c>Ui.Post(async () => await DoSomethingAsync());</c>
    /// </summary>
    /// <param name="Proc">The action to execute.</param>
    /// <param name="Priority">The dispatcher priority.</param>
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
    /// <param name="Func">The function to execute.</param>
    static public void Post(Func<Task> Func) => Post(Func, DispatcherPriority.Background);
    /// <summary>
    /// Executes an action on the UI thread (fire-and-forget).
    /// <para>Supports both synchronous and asynchronous delegates.</para>
    /// <c>Ui.Post(async () => await DoSomethingAsync());</c>
    /// </summary>
    /// <param name="Func">The function to execute.</param>
    /// <param name="Priority">The dispatcher priority.</param>
    static public void Post(Func<Task> Func, DispatcherPriority Priority)
    {
        if (Func != null)
            Dispatcher.UIThread.Post(() => Func(), Priority);
    }
    
    /// <summary>
    /// Executes an action while showing the wait cursor.
    /// </summary>
    /// <param name="Proc">The action to execute.</param>
    /// <param name="Caller">The caller control.</param>
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
    /// <summary>
    /// Executes an action while showing the wait cursor.
    /// </summary>
    /// <typeparam name="T">The action argument type.</typeparam>
    /// <param name="Proc">The action to execute.</param>
    /// <param name="Info">The action argument.</param>
    /// <param name="Caller">The caller control.</param>
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
    /// The main window.
    /// </summary>
    static public Window MainWindow { get; set; }
    /// <summary>
    /// UI global settings.
    /// </summary>
    static public UiGlobalSettings Settings { get; } = new();
}
