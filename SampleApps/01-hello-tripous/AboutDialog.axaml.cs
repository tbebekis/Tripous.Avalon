using Avalonia.Interactivity;
using Avalonia.Input;
using Avalonia.Threading;
using Tripous.Desktop;

namespace HelloTripous;

/// <summary>
/// Displays basic information about the sample application.
/// </summary>
public partial class AboutDialog : DialogWindow
{
    // ● private
    /// <summary>
    /// Handles the Close button click.
    /// </summary>
    /// <param name="Sender">The event sender.</param>
    /// <param name="Args">The event arguments.</param>
    private void Close_Click(object Sender, RoutedEventArgs Args)
    {
        // ● Close the dialog through the Tripous.Desktop modal result.
        ModalResult = ModalResult.Ok;
    }

    // ● protected
    /// <summary>
    /// Handles the window opened event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // ● Focus the close button so Enter and Escape work immediately.
        Dispatcher.UIThread.Post(() => btnClose.Focus(NavigationMethod.Tab, KeyModifiers.None), DispatcherPriority.Input);
    }

    // ● constructor
    /// <summary>
    /// Initializes a new instance of the <see cref="AboutDialog"/> class.
    /// </summary>
    public AboutDialog()
    {
        InitializeComponent();
    }
}
