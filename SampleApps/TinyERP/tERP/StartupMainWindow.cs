/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

/// <summary>
/// Displays the visible desktop startup window while the application initializes.
/// </summary>
public class StartupMainWindow: Window
{
    // ● private fields
    TextBlock lblTitle;
    TextBlock lblMessage;

    // ● private
    Control CreateContent()
    {
        Grid Panel = new();
        Panel.Classes.Add("StartupSurface");
        Panel.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Panel.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        Panel.RowDefinitions.Add(new RowDefinition(GridLength.Star));
        Panel.Margin = new Thickness(24, 48, 24, 24);

        lblTitle = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            FontSize = 34,
            FontWeight = FontWeight.SemiBold,
            Text = "tERP"
        };
        lblTitle.Classes.Add("StartupTitle");
        lblMessage = new TextBlock
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 16, 0, 0),
            FontSize = 18,
            FontWeight = FontWeight.Medium,
            Text = Texts.L("Starting", "Starting..."),
            TextAlignment = TextAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
        lblMessage.Classes.Add("StartupMessage");

        Grid.SetRow(lblTitle, 0);
        Grid.SetRow(lblMessage, 1);
        Panel.Children.Add(lblTitle);
        Panel.Children.Add(lblMessage);
        return Panel;
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public StartupMainWindow()
    {
        Title = "tERP";
        WindowState = WindowState.Maximized;
        Classes.Add("StartupMainWindow");
        ShowInTaskbar = true;
        Content = CreateContent();
    }

    // ● public
    /// <summary>
    /// Sets the application title displayed by the startup window.
    /// </summary>
    public void SetApplicationTitle(string Text)
    {
        Text = string.IsNullOrWhiteSpace(Text) ? "tERP" : Text;
        Title = Text;
        if (lblTitle != null)
            lblTitle.Text = Text;
    }
    /// <summary>
    /// Sets the startup status message.
    /// </summary>
    public void SetMessage(string Text)
    {
        if (lblMessage != null)
            lblMessage.Text = string.IsNullOrWhiteSpace(Text) ? string.Empty : Text;
    }
}
