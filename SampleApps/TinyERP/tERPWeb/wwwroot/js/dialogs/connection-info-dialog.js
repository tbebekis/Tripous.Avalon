/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● connection info dialog
/**
 * Displays the database connection dialog.
 */
app.ConnectionInfoDialog = class {
    // ● constructor
    /**
     * Creates the database connection dialog helper.
     * @param {object|null|undefined} Params Optional dialog params.
     */
    constructor(Params) {
        Params = Params || {};
        /**
         * Dialog create params.
         * @type {object}
         */
        this.Params = Params;
        /**
         * Provider metadata map.
         * @type {object}
         */
        this.ProviderMap = {};
        /**
         * Dialog controls keyed by logical name.
         * @type {object}
         */
        this.Controls = {};
        /**
         * Dialog rows keyed by logical name.
         * @type {object}
         */
        this.Rows = {};
        /**
         * Ordered connection property names.
         * @type {string[]}
         */
        this.PropNames = ["Server", "Port", "Database", "UserId", "Password", "IntegratedSecurity", "TrustServerCertificate", "SslMode", "Charset"];
        /**
         * Components created by this dialog.
         * @type {tp.Component[]}
         */
        this.Components = [];
    }

    // ● protected
    /**
     * Finds a dialog element by selector.
     * @param {tp.Window} Window The dialog window.
     * @param {string} Selector The CSS selector.
     * @returns {HTMLElement|null} Returns the element, if any.
     */
    Find(Window, Selector) {
        return Window && Window.Handle ? Window.Handle.querySelector(Selector) : null;
    }
    /**
     * Sets the message text of the dialog.
     * @param {tp.Window} Window The dialog window.
     * @param {string} Text The message text.
     * @returns {void}
     */
    SetMessage(Window, Text) {
        var Element = this.Find(Window, "[data-role='message']");
        if (Element)
            Element.textContent = Text || "";
    }
    /**
     * Returns provider metadata.
     * @param {string} Name The provider name.
     * @returns {object|null} Returns provider metadata or null.
     */
    GetProvider(Name) {
        return this.ProviderMap[Name] || null;
    }
    /**
     * Returns a property definition for a provider.
     * @param {object|null} Provider The provider metadata.
     * @param {string} PropName The property name.
     * @returns {object|null} Returns the property definition or null.
     */
    GetPropDef(Provider, PropName) {
        var Index;
        var Props = Provider && tp.IsArray(Provider.Props) ? Provider.Props : [];
        for (Index = 0; Index < Props.length; Index++) {
            if (tp.IsSameText(Props[Index].PropType, PropName))
                return Props[Index];
        }
        return null;
    }
    /**
     * Returns a property value.
     * @param {object|null|undefined} ConnectionInfo The connection information packet.
     * @param {string} PropName The property name.
     * @returns {string} Returns the property value.
     */
    GetPropValue(ConnectionInfo, PropName) {
        var Index;
        var Props = ConnectionInfo && tp.IsArray(ConnectionInfo.Props) ? ConnectionInfo.Props : [];
        for (Index = 0; Index < Props.length; Index++) {
            if (tp.IsSameText(Props[Index].PropType, PropName))
                return Props[Index].Value || "";
        }
        return "";
    }
    /**
     * Returns a control value.
     * @param {tp.Component|null|undefined} Control The control.
     * @returns {string} Returns the control value.
     */
    GetControlValue(Control) {
        if (Control instanceof tp.CheckBox)
            return Control.Checked ? "True" : "False";
        if (Control instanceof tp.ComboBox)
            return Control.SelectedValue || "";
        return Control ? Control.Text || "" : "";
    }
    /**
     * Sets a control value.
     * @param {tp.Component|null|undefined} Control The control.
     * @param {string} Value The value.
     * @returns {void}
     */
    SetControlValue(Control, Value) {
        if (Control instanceof tp.CheckBox) {
            Control.Checked = tp.IsSameText(Value, "true");
        } else if (Control instanceof tp.ComboBox) {
            Control.SelectedValue = Value || "";
        } else if (Control) {
            Control.Text = Value || "";
        }
    }
    /**
     * Sets a component enabled state.
     * @param {tp.Component|null|undefined} Control The control.
     * @param {boolean} Flag True to enable.
     * @returns {void}
     */
    SetControlEnabled(Control, Flag) {
        if (!Control)
            return;
        Control.Enabled = Flag === true;
        if (Flag === true)
            tp.RemoveClass(Control.Handle, tp.Classes.Disabled);
        else
            tp.AddClass(Control.Handle, tp.Classes.Disabled);
        if (Control instanceof tp.ComboBox && Control.fTextBox instanceof HTMLInputElement)
            Control.fTextBox.disabled = Flag !== true;
        if (Control instanceof tp.CheckBox && Control.fCheckBox instanceof HTMLInputElement)
            Control.fCheckBox.disabled = Flag !== true;
    }
    /**
     * Resolves control references from a created component list.
     * @param {tp.Component[]} List The created component list.
     * @returns {void}
     */
    ResolveControls(List) {
        var Index;
        var Component;
        var FieldName;
        for (Index = 0; Index < List.length; Index++) {
            Component = List[Index];
            if (!(Component instanceof tp.Component) || !Component.Handle)
                continue;
            if (Component instanceof tp.TabControl) {
                this.Controls.TabControl = Component;
            } else if (Component instanceof tp.CtrlRow || Component instanceof tp.CheckBoxRow) {
                FieldName = Component.Handle.dataset.field || "";
                if (!tp.IsBlankString(FieldName)) {
                    this.Rows[FieldName] = Component;
                    this.Controls[FieldName] = Component.Control;
                }
            } else if (Component instanceof tp.Memo && Component.Handle.dataset.role === "connection-string") {
                this.Controls.ConnectionString = Component;
            }
        }
    }
    /**
     * Creates all Tripous controls.
     * @param {tp.Window} Window The dialog window.
     * @returns {void}
     */
    CreateControls(Window) {
        var List;
        var TabControl;
        tp.Ui.CreateContainerControls(Window.Handle);
        List = tp.Ui.GetContainerControls(Window.Handle);
        this.Components = List;
        this.ResolveControls(List);
        this.Controls.Password.Handle.type = "password";
        this.Controls.Port.TextAlign = "right";
        this.Controls.CommandTimeoutSeconds.TextAlign = "right";
        TabControl = this.Controls.TabControl;
        TabControl.On("SelectedIndexChanged", function () {
            if (TabControl.SelectedIndex === 1) {
                this.UpdatePreviewAsync(Window).catch(function (e) {
                    this.SetMessage(Window, tp.ExceptionText(e));
                }.bind(this));
            }
        }, this);
        this.Controls.DbServerType.On("SelectedIndexChanged", function () {
            this.ApplyProvider(Window, this.Controls.DbServerType.SelectedValue, null);
        }, this);
        TabControl.SetSelectedIndex(0);
    }
    /**
     * Loads provider metadata into the dialog.
     * @param {object|null|undefined} Packet The server packet.
     * @returns {void}
     */
    LoadProviders(Packet) {
        var Providers = Packet && tp.IsArray(Packet.Providers) ? Packet.Providers : [];
        var ProviderNames = [];
        var Index;
        for (Index = 0; Index < Providers.length; Index++) {
            this.ProviderMap[Providers[Index].Name] = Providers[Index];
            ProviderNames.push(Providers[Index].Name);
        }
        this.Controls.DbServerType.Items = ProviderNames;
    }
    /**
     * Applies provider metadata to controls.
     * @param {tp.Window} Window The dialog window.
     * @param {string} ProviderName The provider name.
     * @param {object|null|undefined} ConnectionInfo Optional connection information.
     * @returns {void}
     */
    ApplyProvider(Window, ProviderName, ConnectionInfo) {
        var Provider = this.GetProvider(ProviderName);
        var Index;
        var PropName;
        var Def;
        var Value;
        var Control;
        var Row;
        for (Index = 0; Index < this.PropNames.length; Index++) {
            PropName = this.PropNames[Index];
            Def = this.GetPropDef(Provider, PropName);
            Value = this.GetPropValue(ConnectionInfo, PropName);
            Control = this.Controls[PropName];
            Row = this.Rows[PropName];
            if (Row) {
                Row.Text = Def ? Def.Label : Row.Handle.dataset.defaultText || Row.Text;
                if (Def)
                    tp.RemoveClass(Row.Handle, tp.Classes.Disabled);
                else
                    tp.AddClass(Row.Handle, tp.Classes.Disabled);
            }
            this.SetControlEnabled(Control, !!Def);
            if (PropName === "SslMode")
                Control.Items = Def && tp.IsArray(Def.ValidValues) ? Def.ValidValues : [];
            this.SetControlValue(Control, Def ? (Value || Def.DefaultValue || "") : "");
        }
        this.SetMessage(Window, "");
    }
    /**
     * Loads connection information into the dialog.
     * @param {tp.Window} Window The dialog window.
     * @param {object|null|undefined} Packet The server packet.
     * @returns {void}
     */
    LoadConnectionInfo(Window, Packet) {
        var ConnectionInfo = Packet && Packet.ConnectionInfo ? Packet.ConnectionInfo : {};
        this.SetControlValue(this.Controls.Name, ConnectionInfo.Name || "");
        this.SetControlValue(this.Controls.CommandTimeoutSeconds, ConnectionInfo.CommandTimeoutSeconds || "");
        this.SetControlValue(this.Controls.DbServerType, ConnectionInfo.DbServerType || "Sqlite");
        this.ApplyProvider(Window, this.Controls.DbServerType.SelectedValue || "Sqlite", ConnectionInfo);
    }
    /**
     * Collects values from the dialog.
     * @returns {object} Returns the connection info values.
     */
    CollectData() {
        var Result = {
            Name: this.GetControlValue(this.Controls.Name),
            DbServerType: this.GetControlValue(this.Controls.DbServerType),
            CommandTimeoutSeconds: this.GetControlValue(this.Controls.CommandTimeoutSeconds),
            Values: {}
        };
        var Index;
        var PropName;
        var Control;
        for (Index = 0; Index < this.PropNames.length; Index++) {
            PropName = this.PropNames[Index];
            Control = this.Controls[PropName];
            if (!Control || Control.Enabled !== true)
                continue;
            Result.Values[PropName] = this.GetControlValue(Control);
        }
        return Result;
    }
    /**
     * Updates the connection string preview.
     * @param {tp.Window} Window The dialog window.
     * @returns {Promise<void>} Returns a Promise.
     */
    async UpdatePreviewAsync(Window) {
        var Packet = await app.App.GetConnectionInfoPreviewAsync(this.CollectData());
        this.SetControlValue(this.Controls.ConnectionString, Packet && Packet.ConnectionString ? Packet.ConnectionString : "");
        if (Packet && Packet.Success !== true)
            this.SetMessage(Window, Packet.Message || "");
        else
            this.SetMessage(Window, "");
    }
    /**
     * Tests the database connection.
     * @param {tp.Window} Window The dialog window.
     * @returns {Promise<void>} Returns a Promise.
     */
    async TestConnectionAsync(Window) {
        var Packet = await app.App.TestConnectionInfoAsync(this.CollectData());
        this.SetMessage(Window, Packet && Packet.Message ? Packet.Message : "");
        if (Packet && Packet.Success === true && tp.IsFunction(tp.SuccessNote))
            tp.SuccessNote(Packet.Message || tp._L("ConnectionSucceeded", "Connection succeeded."));
        else if (tp.IsFunction(tp.ErrorNote))
            tp.ErrorNote(Packet && Packet.Message ? Packet.Message : tp._L("ConnectionFailed", "Connection failed."));
    }
    /**
     * Handles dialog key presses.
     * @param {KeyboardEvent} e The keyboard event.
     * @param {tp.Window} Window The dialog window.
     * @returns {void}
     */
    HandleKeyDown(e, Window) {
        if (tp.IsKey(e, tp.Keys.Enter) && e.target instanceof HTMLInputElement && e.target.type !== "checkbox") {
            e.preventDefault();
            Window.DialogResult = tp.DialogResult.OK;
        }
    }
    /**
     * Shows the modal dialog once.
     * @param {object|null|undefined} Packet The server packet.
     * @param {string} Message The message text.
     * @returns {Promise<object|null>} Returns dialog data or null when cancelled.
     */
    async ShowDialogAsync(Packet, Message) {
        Packet = Packet || {};
        var Self = this;
        var Args = {
            Text: tp._L("DatabaseConnection", "Database Connection"),
            Width: 500,
            Height: 700,
            ResizeEdges: tp.Edge.None,
            InitialFocusSelector: ".tp-TextBox, input",
            ShowFunc: function (Window) {
                var TestButton;
                tp.AddClass(Window.Handle, "app-connection-info-window");
                TestButton = Window.CreateFooterButton("TestConnection", tp._L("TestConnection", "Test Connection"), tp.DialogResult.None, true);
                TestButton.On(tp.Events.Click, function () {
                    Self.TestConnectionAsync(Window).catch(function (e) {
                        Self.SetMessage(Window, tp.ExceptionText(e));
                    });
                });
                Self.CreateControls(Window);
                Self.LoadProviders(Packet);
                Self.LoadConnectionInfo(Window, Packet);
                Self.SetMessage(Window, Message);
                Window.ConnectionInfoDialogKeyDownHandler = function (e) {
                    Self.HandleKeyDown(e, Window);
                };
                Window.Handle.addEventListener("keydown", Window.ConnectionInfoDialogKeyDownHandler);
            },
            CloseFunc: function (Window) {
                if (Window.ConnectionInfoDialogKeyDownHandler)
                    Window.Handle.removeEventListener("keydown", Window.ConnectionInfoDialogKeyDownHandler);
                if (Window.DialogResult === tp.DialogResult.OK)
                    Window.ResultData = Self.CollectData();
                Self.DisposeComponents();
            }
        };
        var Window = await tp.ContentWindow.ShowModalAsync(Packet.Html || "", Args);
        return Window.DialogResult === tp.DialogResult.OK ? Window.ResultData : null;
    }

    // ● protected
    /**
     * Disposes controls created by the dialog.
     * @returns {void}
     */
    DisposeComponents() {
        var Index;
        for (Index = this.Components.length - 1; Index >= 0; Index--) {
            if (this.Components[Index] && this.Components[Index].IsDisposed !== true && tp.IsFunction(this.Components[Index].Dispose))
                this.Components[Index].Dispose();
        }
        this.Components.length = 0;
        this.Controls = {};
        this.Rows = {};
    }

    // ● public
    /**
     * Shows the dialog and saves accepted values.
     * @param {object|null|undefined} Packet The server packet.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ShowAsync(Packet) {
        var Message = "";
        var Data;
        var Result;
        while (true) {
            Data = await this.ShowDialogAsync(Packet, Message);
            if (Data === null)
                return;
            Result = await app.App.SaveConnectionInfoAsync(Data);
            Message = Result && Result.Message ? Result.Message : "";
            if (Result && Result.Success === true) {
                if (tp.LogBox)
                    tp.LogBox.AppendLine(Message || tp._L("ConnectionInformationSaved", "Connection information saved."));
                if (app.App.MainPage && app.App.MainPage.StatusBar)
                    app.App.MainPage.StatusBar.Message = Message || tp._L("ConnectionInformationSaved", "Connection information saved.");
                if (tp.IsFunction(tp.SuccessNote))
                    tp.SuccessNote(Message || tp._L("ConnectionInformationSaved", "Connection information saved."));
                return;
            }
        }
    }
};
