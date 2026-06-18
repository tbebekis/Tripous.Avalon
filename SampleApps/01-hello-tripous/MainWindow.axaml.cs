using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading.Tasks;
using Tripous.Desktop;

namespace HelloTripous;

/// <summary>
/// Represents the main application window.
/// </summary>
public partial class MainWindow : Window
{
    // ● private fields
    private bool fIsWindowInitialized;
    private ToolBar fToolBar;

    // ● private
    /// <summary>
    /// Initializes the window after it is opened.
    /// </summary>
    private void WindowInitialize()
    {
        // ● Connect the Tripous log helper to this window text box.
        LogBox.Initialize(edtLog);
        // ● Create toolbar buttons by using the Tripous.Desktop toolbar helper.
        CreateToolBar();
        // ● Initialize the visible status line.
        UpdateStatusBar("Ready");
    }
    /// <summary>
    /// Creates the toolbar buttons.
    /// </summary>
    private void CreateToolBar()
    {
        fToolBar = new();
        fToolBar.Panel = pnlToolBar;
        Button Button = fToolBar.AddButton("information.png", "Say Hello", AnyClick);
        Button.Tag = "SayHello";
        Button = fToolBar.AddButton("application_go.png", "Open Dialog", AnyClick);
        Button.Tag = "OpenDialog";
        Button = fToolBar.AddButton("draw_eraser.png", "Clear Log", AnyClick);
        Button.Tag = "ClearLog";
        fToolBar.AddSeparator();
        Button = fToolBar.AddButton("emotion_question.png", "About", AnyClick);
        Button.Tag = "About";
        Button = fToolBar.AddButton("door_out.png", "Exit", AnyClick);
        Button.Tag = "Exit";
    }
    /// <summary>
    /// Updates the status bar.
    /// </summary>
    /// <param name="Message">The status message.</param>
    private void UpdateStatusBar(string Message)
    {
        lblStatus.Text = Message;
        lblDetails.Text = "Hello Tripous v1.0";
    }
    /// <summary>
    /// Handles all command clicks.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The event arguments.</param>
    private async void AnyClick(object Sender, RoutedEventArgs Args)
    {
        Control Control = Sender as Control;
        string CommandName = Control?.Tag as string;
        await ExecuteCommand(CommandName);
    }
    /// <summary>
    /// Executes a named command.
    /// </summary>
    /// <param name="CommandName">The command name.</param>
    private async Task ExecuteCommand(string CommandName)
    {
        switch (CommandName)
        {
            case "SayHello":
                await SayHello();
                break;
            case "OpenDialog":
            case "About":
                await OpenAboutDialog();
                break;
            case "ClearLog":
                ClearLog();
                break;
            case "Exit":
                Exit();
                break;
        }
    }
    /// <summary>
    /// Shows a hello message.
    /// </summary>
    private async Task SayHello()
    {
        // ● Write to the Tripous log box first so the user sees immediate feedback.
        Log("Hello from Tripous.Desktop.");
        // ● Show a Tripous.Desktop modal message box owned by this window.
        await MessageBox.Info("Hello from Tripous.Desktop.", this);
        UpdateStatusBar("Hello command executed.");
    }
    /// <summary>
    /// Opens the About dialog.
    /// </summary>
    private async Task OpenAboutDialog()
    {
        // ● Use the Tripous.Desktop dialog infrastructure.
        await DialogWindow.ShowModal<AboutDialog>(this);
        Log("The About dialog was closed.");
        UpdateStatusBar("About dialog closed.");
    }
    /// <summary>
    /// Clears the log.
    /// </summary>
    private void ClearLog()
    {
        // ● Clear the Tripous log box.
        LogBox.Clear();
        UpdateStatusBar("Log cleared.");
    }
    /// <summary>
    /// Closes the application.
    /// </summary>
    private void Exit()
    {
        // ● Close the main window and let the desktop lifetime end.
        Close();
    }
    /// <summary>
    /// Adds a line to the log box.
    /// </summary>
    /// <param name="Text">The log text.</param>
    private void Log(string Text)
    {
        if (string.IsNullOrWhiteSpace(Text))
            return;

        LogBox.AppendLine(Text);
    }

    // ● protected
    /// <summary>
    /// Handles the window opened event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (fIsWindowInitialized)
            return;

        WindowInitialize();
        fIsWindowInitialized = true;

        LogBox.AppendLine("Application Started.");
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
    }
}
