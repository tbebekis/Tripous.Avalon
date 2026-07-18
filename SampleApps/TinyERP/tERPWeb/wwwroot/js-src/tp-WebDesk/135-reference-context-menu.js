// ● reference menu action type
/**
 * Defines standard actions of a WebDesk reference context menu.
 * @enum {string}
 */
tp.ReferenceMenuActionType = {
    ShowList: "ShowList",
    Reload: "Reload",
    Edit: "Edit",
    Add: "Add",
    Clear: "Clear"
};
Object.freeze(tp.ReferenceMenuActionType);

// ● reference context menu
/**
 * Common context menu for controls that edit reference values.
 */
tp.ReferenceContextMenu = class extends tp.Object {
    // ● constructor
    /**
     * Creates a reference context menu.
     * @param {object|null|undefined} Params Optional parameters.
     */
    constructor(Params) {
        super();
        Params = Params || {};
        /**
         * The command host.
         * @type {*}
         */
        this.Host = Params.Host || null;
        /**
         * The locator control served by this menu.
         * @type {tp.LocatorBox|null}
         */
        this.LocatorBox = Params.LocatorBox instanceof tp.LocatorBox ? Params.LocatorBox : null;
        /**
         * The actual context menu.
         * @type {tp.ContextMenu}
         */
        this.Menu = new tp.ContextMenu();
        this.mnuShowList = this.Menu.AddMenuItem("Show List", tp.ReferenceMenuActionType.ShowList);
        this.mnuReload = this.Menu.AddMenuItem("Reload", tp.ReferenceMenuActionType.Reload);
        this.mnuEdit = this.Menu.AddMenuItem("Edit", tp.ReferenceMenuActionType.Edit);
        this.mnuAdd = this.Menu.AddMenuItem("Add", tp.ReferenceMenuActionType.Add);
        this.mnuClear = this.Menu.AddMenuItem("Clear", tp.ReferenceMenuActionType.Clear);
        this.Menu.On("ItemClick", this.HandleItemClick, this);
    }

    // ● protected
    /**
     * Returns the reference form name.
     * @returns {string} Returns the reference web form name.
     */
    GetFormName() {
        var Locator = this.GetLocatorDef();
        return Locator instanceof tp.LocatorDef ? Locator.WebForm || Locator.Form || "" : "";
    }
    /**
     * Returns the locator definition.
     * @returns {tp.LocatorDef|null} Returns the locator definition or null.
     */
    GetLocatorDef() {
        return this.LocatorBox instanceof tp.LocatorBox
            && this.LocatorBox.Info instanceof tp.LocatorInfo
            && this.LocatorBox.Info.Locator instanceof tp.LocatorDef
            ? this.LocatorBox.Info.Locator
            : null;
    }
    /**
     * Returns the selected reference row id.
     * @returns {*} Returns the selected row id or null.
     */
    GetRowId() {
        var Row = this.LocatorBox instanceof tp.LocatorBox ? this.LocatorBox.GetTargetRow() : null;
        var FieldName = this.LocatorBox instanceof tp.LocatorBox ? this.LocatorBox.ReferenceField || this.LocatorBox.DataField : "";
        if (Row instanceof tp.DataRow)
            return Row.Get(FieldName, null);
        return Row && !tp.IsBlank(FieldName) && FieldName in Row ? Row[FieldName] : null;
    }
    /**
     * Creates a command context.
     * @param {string} ActionType The action type.
     * @returns {object} Returns the command context.
     */
    CreateContext(ActionType) {
        return {
            ActionType: ActionType,
            Menu: this,
            LocatorBox: this.LocatorBox,
            FormName: this.GetFormName(),
            RowId: ActionType === tp.ReferenceMenuActionType.Edit ? this.GetRowId() : null,
            Caller: this.LocatorBox instanceof tp.LocatorBox ? this.LocatorBox.fButton || this.LocatorBox.Handle : null,
            Result: null
        };
    }
    /**
     * Returns true when this menu can open.
     * @returns {boolean} Returns true when the menu can open.
     */
    CanOpen() {
        if (!(this.LocatorBox instanceof tp.LocatorBox))
            return false;
        if (this.LocatorBox.ReadOnly === true)
            return false;
        return !this.Host || !tp.IsFunction(this.Host.CanOpenRefContextMenu) || this.Host.CanOpenRefContextMenu(this) === true;
    }
    /**
     * Returns true when an action can execute.
     * @param {string} ActionType The action type.
     * @returns {boolean} Returns true when the action can execute.
     */
    CanExecute(ActionType) {
        var Context;
        if (this.CanOpen() !== true)
            return false;
        Context = this.CreateContext(ActionType);
        if (this.Host && tp.IsFunction(this.Host.CanExecuteReferenceMenu))
            return this.Host.CanExecuteReferenceMenu(Context) === true;
        if (ActionType === tp.ReferenceMenuActionType.ShowList)
            return !tp.IsBlank(Context.FormName) && tp.IsFunction(tp.ReferenceContextMenu.ShowDataFormModalAsync);
        if (ActionType === tp.ReferenceMenuActionType.Edit)
            return !tp.IsBlank(Context.FormName) && !tp.IsEmpty(Context.RowId) && tp.IsFunction(tp.ReferenceContextMenu.ShowDataFormModalAsync);
        if (ActionType === tp.ReferenceMenuActionType.Add)
            return !tp.IsBlank(Context.FormName) && tp.IsFunction(tp.ReferenceContextMenu.ShowDataFormModalAsync);
        if (ActionType === tp.ReferenceMenuActionType.Clear)
            return this.LocatorBox instanceof tp.LocatorBox && this.LocatorBox.ReadOnly !== true;
        return false;
    }
    /**
     * Enables or disables menu items.
     * @returns {void}
     */
    EnableMenuItems() {
        this.mnuReload.Visible = false;
        this.mnuShowList.Enabled = this.CanExecute(tp.ReferenceMenuActionType.ShowList);
        this.mnuReload.Enabled = false;
        this.mnuEdit.Enabled = this.CanExecute(tp.ReferenceMenuActionType.Edit);
        this.mnuAdd.Enabled = this.CanExecute(tp.ReferenceMenuActionType.Add);
        this.mnuClear.Enabled = this.CanExecute(tp.ReferenceMenuActionType.Clear);
    }
    /**
     * Shows the reference list.
     * @param {object} Context The command context.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ExecuteShowList(Context) {
        var Dialog = await tp.ReferenceContextMenu.ShowDataFormModalAsync(Context.FormName, {
            InitialAction: "List"
        });
        if (Dialog && Dialog.DialogResult === tp.DialogResult.OK)
            await this.SetReferenceValueAsync(Dialog.ResultData);
    }
    /**
     * Edits the selected reference item.
     * @param {object} Context The command context.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ExecuteEdit(Context) {
        var Dialog = await tp.ReferenceContextMenu.ShowDataFormModalAsync(Context.FormName, {
            InitialAction: "Edit",
            InitialKeyValue: Context.RowId
        });
        if (Dialog && Dialog.DialogResult === tp.DialogResult.OK)
            await this.SetReferenceValueAsync(Context.RowId);
    }
    /**
     * Adds a new reference item.
     * @param {object} Context The command context.
     * @returns {Promise<void>} Returns a Promise.
     */
    async ExecuteAdd(Context) {
        var Dialog = await tp.ReferenceContextMenu.ShowDataFormModalAsync(Context.FormName, {
            InitialAction: "Insert"
        });
        if (Dialog && Dialog.DialogResult === tp.DialogResult.OK)
            await this.SetReferenceValueAsync(Dialog.ResultData);
    }
    /**
     * Clears the reference value.
     * @returns {void}
     */
    ExecuteClear() {
        if (this.LocatorBox instanceof tp.LocatorBox)
            this.LocatorBox.Clear();
    }
    /**
     * Sets the locator reference value and refreshes display fields.
     * @param {*} Value The reference id.
     * @returns {Promise<void>} Returns a Promise.
     */
    async SetReferenceValueAsync(Value) {
        var Request;
        var Result;
        var TargetRow;
        if (!(this.LocatorBox instanceof tp.LocatorBox))
            return;
        if (tp.IsEmpty(Value)) {
            this.LocatorBox.Clear();
            return;
        }
        await this.LocatorBox.EnsureInfoAsync();
        TargetRow = this.LocatorBox.GetTargetRow();
        Request = this.LocatorBox.CreateRequest(null);
        Request.KeyValue = Value;
        Request.IsMultiRow = false;
        Result = await tp.Locator.ExecuteAsync(Request);
        if (Result instanceof tp.LocatorResult && Result.FirstRow instanceof tp.DataRow) {
            Result.Apply(TargetRow);
            this.LocatorBox.SetInputValuesFromRow(Result.FirstRow);
            this.LocatorBox.OnLocated(Result, Result.FirstRow, TargetRow);
        }
    }
    /**
     * Handles context menu item click.
     * @param {tp.MenuEventArgs} Args The event arguments.
     * @returns {void}
     */
    async HandleItemClick(Args) {
        var Command = Args ? Args.Command : "";
        var Context = this.CreateContext(Command);
        try {
            if (!this.CanExecute(Command))
                return;
            if (this.Host && tp.IsFunction(this.Host.ExecuteReferenceMenu)) {
                await this.Host.ExecuteReferenceMenu(Context);
                return;
            }
            if (Command === tp.ReferenceMenuActionType.ShowList)
                await this.ExecuteShowList(Context);
            else if (Command === tp.ReferenceMenuActionType.Edit)
                await this.ExecuteEdit(Context);
            else if (Command === tp.ReferenceMenuActionType.Add)
                await this.ExecuteAdd(Context);
            else if (Command === tp.ReferenceMenuActionType.Clear)
                this.ExecuteClear();
        } catch (e) {
            if (tp.ErrorNote)
                tp.ErrorNote(tp.ExceptionText(e));
            if (tp.LogBox)
                tp.LogBox.AppendLine("Reference menu command failed: " + tp.ExceptionText(e));
        }
    }

    // ● public
    /**
     * Opens the menu.
     * @param {MouseEvent|null|undefined} e The source mouse event.
     * @returns {Promise<boolean>} Returns true when the menu opened.
     */
    async OpenAsync(e) {
        var Rect;
        if (this.LocatorBox instanceof tp.LocatorBox)
            await this.LocatorBox.EnsureInfoAsync();
        if (this.CanOpen() !== true)
            return false;
        this.EnableMenuItems();
        Rect = this.LocatorBox.fButton instanceof HTMLElement ? this.LocatorBox.fButton.getBoundingClientRect() : null;
        if (Rect)
            this.Menu.ShowAt(Rect.left, Rect.bottom + 1);
        else if (e instanceof MouseEvent)
            this.Menu.Show(e);
        return true;
    }
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        if (this.Menu instanceof tp.ContextMenu) {
            this.Menu.Off("ItemClick", this.HandleItemClick, this);
            this.Menu.Dispose();
        }
        if (this.LocatorBox instanceof tp.LocatorBox && this.LocatorBox.ReferenceContextMenu === this)
            this.LocatorBox.ReferenceContextMenu = null;
        this.Menu = null;
        this.Host = null;
        this.LocatorBox = null;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.ReferenceContextMenu.prototype.tpClass = "tp.ReferenceContextMenu";
/**
 * Optional callback that opens a data form modal dialog.
 * @type {Function|null}
 */
tp.ReferenceContextMenu.ShowDataFormModalAsync = null;
