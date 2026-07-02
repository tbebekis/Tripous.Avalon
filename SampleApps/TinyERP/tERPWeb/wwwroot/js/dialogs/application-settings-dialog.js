/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● application settings dialog
/**
 * Displays and saves application settings.
 */
app.ApplicationSettingsDialog = class {
    // ● constructor
    /**
     * Creates the application settings dialog helper.
     * @param {object|null|undefined} Params Optional dialog params.
     */
    constructor(Params) {
        Params = Params || {};
        /**
         * Dialog create params.
         * @type {object}
         */
        this.Params = Params;
    }

    // ● protected
    /**
     * Sets the message text of the dialog.
     * @param {tp.Window} Window The dialog window.
     * @param {string} Text The message text.
     * @returns {void}
     */
    SetMessage(Window, Text) {
        var Element = Window && Window.Handle ? Window.Handle.querySelector("[data-role='message']") : null;
        if (Element)
            Element.textContent = Text || "";
    }
    /**
     * Applies a value to a setting control.
     * @param {tp.Component} Control The setting control.
     * @param {string} Kind The config value kind.
     * @param {string} Value The config value.
     * @returns {void}
     */
    SetControlValue(Control, Kind, Value) {
        if (!(Control instanceof tp.Component))
            return;
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (Kind === "Boolean" && "Checked" in Control)
            Control.Checked = Value === "true" || Value === "1";
        else if ("Text" in Control)
            Control.Text = Value;
        else if ("Value" in Control)
            Control.Value = Value;
    }
    /**
     * Returns a value from a setting control.
     * @param {tp.Component} Control The setting control.
     * @param {string} Kind The config value kind.
     * @returns {string} Returns the config value.
     */
    GetControlValue(Control, Kind) {
        if (!(Control instanceof tp.Component))
            return "";
        if (Kind === "Boolean" && "Checked" in Control)
            return Control.Checked === true ? "true" : "false";
        if ("Text" in Control)
            return Control.Text || "";
        if ("Value" in Control)
            return tp.IsNil(Control.Value) ? "" : String(Control.Value);
        return "";
    }
    /**
     * Applies values to created setting controls.
     * @param {HTMLElement} Root The dialog root element.
     * @returns {void}
     */
    ApplyValues(Root) {
        var Rows = Root instanceof HTMLElement ? Root.querySelectorAll(".app-settings-row[data-config-name]") : [];
        var Index;
        var Row;
        var RowControl;
        var Control;

        for (Index = 0; Index < Rows.length; Index++) {
            Row = Rows[Index];
            RowControl = tp.GetComponent(Row);
            Control = RowControl && RowControl.Control instanceof tp.Component ? RowControl.Control : null;
            this.SetControlValue(Control, Row.dataset.configKind || "", Row.dataset.configValue || "");
        }
    }
    /**
     * Collects scalar application settings from the dialog.
     * @param {tp.Window} Window The dialog window.
     * @returns {object} Returns the settings data.
     */
    CollectData(Window) {
        var Root = Window && Window.Handle ? Window.Handle.querySelector(".app-settings-dialog") : null;
        var ScopeElement = Root ? Root.querySelector("select[name='Scope']") : null;
        var Rows = Root ? Root.querySelectorAll(".app-settings-row[data-config-name]") : [];
        var Result = { Scope: ScopeElement ? ScopeElement.value : "User", Values: {} };
        var Index;
        var Row;
        var RowControl;
        var Control;

        for (Index = 0; Index < Rows.length; Index++) {
            Row = Rows[Index];
            RowControl = tp.GetComponent(Row);
            Control = RowControl && RowControl.Control instanceof tp.Component ? RowControl.Control : null;
            Result.Values[Row.dataset.configName] = this.GetControlValue(Control, Row.dataset.configKind || "");
        }

        return Result;
    }
    /**
     * Attaches dialog handlers.
     * @param {tp.Window} Window The dialog window.
     * @returns {void}
     */
    Attach(Window) {
        var Self = this;
        var Root = Window && Window.Handle ? Window.Handle.querySelector(".app-settings-dialog") : null;
        var ScopeElement = Root ? Root.querySelector("select[name='Scope']") : null;
        var Buttons = Window && Window.Footer ? Window.Footer.querySelectorAll("button") : [];

        if (Buttons.length > 0)
            Buttons[0].textContent = "Save";
        if (Buttons.length > 1)
            Buttons[1].textContent = "Close";

        if (Root instanceof HTMLElement) {
            tp.Ui.CreateContainerControls(Root);
            this.ApplyValues(Root);
        }

        if (ScopeElement) {
            Window.ApplicationSettingsScopeChangedHandler = async function () {
                var Packet;
                try {
                    Packet = await app.App.GetApplicationSettingsDialogAsync(ScopeElement.value);
                    if (Window.Content && Window.Content.Handle) {
                        Window.Content.DisposeChildComponents();
                        Window.Content.Handle.innerHTML = Packet.Html || "";
                        Self.Attach(Window);
                    }
                } catch (e) {
                    Self.SetMessage(Window, tp.ExceptionText(e));
                }
            };
            ScopeElement.addEventListener("change", Window.ApplicationSettingsScopeChangedHandler);
        }
    }

    // ● public
    /**
     * Shows the application settings dialog.
     * @param {object|null|undefined} Packet Optional server packet.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ShowAsync(Packet) {
        Packet = Packet || await app.App.GetApplicationSettingsDialogAsync("User");
        var Self = this;
        var Args = {
            Text: "Application Settings",
            Width: 900,
            Height: 640,
            InitialFocusSelector: "select[name='Scope']",
            ShowFunc: function (Window) {
                Self.Attach(Window);
            },
            CloseFunc: function (Window) {
                if (Window.DialogResult === tp.DialogResult.OK)
                    Window.ResultData = Self.CollectData(Window);
            }
        };
        var Window = await tp.ContentWindow.ShowModalAsync(Packet.Html || "", Args);
        var Result;
        if (Window.DialogResult === tp.DialogResult.OK) {
            Result = await app.App.SaveApplicationSettingsAsync(Window.ResultData.Scope, Window.ResultData.Values);
            if (tp.LogBox)
                tp.LogBox.AppendLine(Result && Result.Message ? Result.Message : "Settings saved.");
            if (app.App.MainPage && app.App.MainPage.StatusBar)
                app.App.MainPage.StatusBar.Message = Result && Result.Message ? Result.Message : "Settings saved.";
        }
    }
};
