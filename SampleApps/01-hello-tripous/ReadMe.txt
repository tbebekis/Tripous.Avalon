# Hello Tripous

This is the first Tripous sample application.

Purpose

- Show the smallest useful desktop application structure.
- Use a main window.
- Add a menu.
- Add a Tripous.Desktop toolbar.
- Show a Tripous.Desktop dialog window.
- Show a Tripous.Desktop message box.
- Use the Tripous.Desktop LogBox.
- Avoid database access.
- Avoid RegBuilder.

What this sample teaches

- How an Avalonia application starts.
- How the main window is assigned.
- How Tripous.Desktop.Ui.MainWindow is assigned.
- How a command can be exposed from both menu and toolbar.
- How a single click handler can dispatch to private methods.
- How a modal dialog is owned by the main window.
- How a very small application can use Tripous.Desktop helpers without any schema or database.

Files

- HelloTripous.csproj
- Program.cs
- App.axaml
- App.axaml.cs
- MainWindow.axaml
- MainWindow.axaml.cs
- AboutDialog.axaml
- AboutDialog.axaml.cs
- ReadMe.txt

Important points

- Program.cs creates the Avalonia application builder.
- App.axaml.cs creates MainWindow during desktop startup.
- App.axaml.cs assigns Ui.MainWindow so Tripous.Desktop dialogs have an owner window.
- MainWindow.axaml defines the menu, toolbar host, status bar, body, and log box.
- MainWindow.axaml.cs contains the window initialization and command handlers.
- AboutDialog.axaml and AboutDialog.axaml.cs define a Tripous.Desktop dialog.

Manual test

- Start the application.
- Check that the main window is visible and centered.
- Open File / Say Hello.
- Check that a message box appears.
- Close the message box.
- Check that a log line was added.
- Press the Say Hello toolbar button.
- Check that the same command runs.
- Open File / Open Dialog.
- Check that the About dialog appears in front of the main window.
- Close the dialog.
- Open Help / About.
- Check that the same dialog appears.
- Press the Clear toolbar button.
- Check that the log is cleared.
- Open Edit / Clear Log.
- Check that the command still works when the log is already empty.
- Open File / Exit.
- Check that the application closes.

Next sample

- The next sample should add SQLite, one table, one module, one form, and a DataModule.
- The next sample should define TableDef, ModuleDef, FormDef, and registration by hand.
