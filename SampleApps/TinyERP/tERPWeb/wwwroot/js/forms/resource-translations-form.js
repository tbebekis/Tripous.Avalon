/**
 * TinyERP Web application namespace.
 * @type {object}
 */
var app = app || {};

// ● resource translations form
/**
 * Displays an admin editor for system resource translations.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 * - CloseRequested
 */
app.ResourceTranslationsForm = class extends tp.WebForm {
    // ● constructor
    /**
     * Creates a resource translations form.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        /**
         * Translation grid.
         * @type {tp.Grid|null}
         */
        this.Grid = null;
        /**
         * Translation table.
         * @type {tp.DataTable|null}
         */
        this.Table = null;
        /**
         * Language metadata keyed by grid column name.
         * @type {object}
         */
        this.LanguageMap = {};
        /**
         * True while loading data.
         * @type {boolean}
         */
        this.Loading = false;
        /**
         * True when a reload is waiting for the current load to finish.
         * @type {boolean}
         */
        this.ReloadPending = false;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, "app-resource-translations");
    }
    /**
     * Notification called after field initialization.
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateControls();
    }
    /**
     * Creates form controls.
     * @returns {void}
     */
    CreateControls() {
        this.CreateGrid();
    }
    /**
     * Creates the grid.
     * @returns {void}
     */
    CreateGrid() {
        var Element = this.Handle.querySelector("[data-role='grid']");
        if (!(Element instanceof HTMLElement))
            return;
        this.Grid = new tp.Grid({
            ElementOrSelector: Element,
            AutoGenerateColumns: true
        });
        this.Grid.ReadOnly = false;
        this.Grid.ToolBarVisible = true;
        this.Grid.GroupsVisible = false;
        this.Grid.FilterVisible = true;
        this.Grid.FooterVisible = false;
        this.Grid.AllowUserToAddRows = false;
        this.Grid.AllowUserToDeleteRows = false;
        this.Grid.ButtonInsertVisible = false;
        this.Grid.ButtonDeleteVisible = true;
        this.Grid.ButtonEditVisible = false;
        this.Grid.ButtonFindVisible = false;
        this.Grid.On("ToolBarButtonClick", this.HandleGridToolBarButtonClick, this);
    }
    /**
     * Executes an async operation while showing the global spinner.
     * @param {Function} Func The async function to execute.
     * @returns {Promise<*>} Returns the function result.
     */
    async ExecuteWithSpinner(Func) {
        var ShowSpinner = tp.IsFunction(tp.ShowSpinner);
        if (ShowSpinner)
            tp.ShowSpinner(true);
        try {
            return await Func.call(this);
        } finally {
            if (ShowSpinner)
                tp.ShowSpinner(false);
        }
    }
    /**
     * Handles grid toolbar button clicks.
     * @param {tp.ToolBarItemClickEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleGridToolBarButtonClick(Args) {
        if (!Args)
            return;
        if (Args.Command === "GridRowDelete") {
            Args.Handled = true;
            this.DeleteFocusedKeyAsync();
        }
    }
    /**
     * Handles translation table row modifications.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleTableRowModified(Args) {
        if (this.Loading === true)
            return;
        this.SaveModifiedTranslationAsync(Args);
    }
    /**
     * Handles translation table sorting.
     * @param {tp.DataSourceEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleDataSourceSorted(Args) {
        this.ApplyLanguageSortBlanksFirst();
    }
    /**
     * Handles broadcaster events.
     * @param {string} EventName The broadcaster event name.
     * @param {object} Args The broadcaster event arguments.
     * @returns {void}
     */
    HandleBroadcasterEvent(EventName, Args) {
        var ModuleName;
        super.HandleBroadcasterEvent(EventName, Args);
        if (tp.IsSameText(EventName, "SysStrRes.Changed")) {
            this.LoadTranslationsAsync();
            return;
        }
        if (!tp.IsSameText(EventName, "DataModule.Saved") || !Args)
            return;
        ModuleName = Args.ModuleName || "";
        if (tp.IsSameText(ModuleName, "SYS_LANG") || tp.IsSameText(ModuleName, "SysLang"))
            this.LoadTranslationsAsync();
    }
    /**
     * Applies column settings after binding.
     * @param {object[]} Languages The language metadata.
     * @returns {void}
     */
    ApplyGridColumns(Languages) {
        var Index;
        var Language;
        var Column;
        this.LanguageMap = {};
        if (!(this.Grid instanceof tp.Grid))
            return;
        this.Grid.SetColumnReadOnly("ResKey", true);
        for (Index = 0; Index < Languages.length; Index++) {
            Language = Languages[Index];
            this.LanguageMap[Language.ColumnName] = Language;
            Column = this.Grid.ColumnByName(Language.ColumnName);
            if (Column) {
                Column.ReadOnly = Language.IsEnglish === true;
                Column.Width = 220;
            }
        }
        Column = this.Grid.ColumnByName("ResKey");
        if (Column)
            Column.Width = 260;
        this.Grid.BestFitColumns();
    }
    /**
     * Returns the active language sort column, if any.
     * @returns {tp.GridColumn|null} Returns the sorted language column or null.
     */
    GetActiveLanguageSortColumn() {
        var Index;
        var Column;
        if (!(this.Grid instanceof tp.Grid))
            return null;
        for (Index = 0; Index < this.Grid.ValueColumns.length; Index++) {
            Column = this.Grid.ValueColumns[Index];
            if (Column && !tp.IsBlankString(Column.SortMode) && this.LanguageMap[Column.Name])
                return Column;
        }
        return null;
    }
    /**
     * Sorts active language column rows with blank translations first.
     * @returns {void}
     */
    ApplyLanguageSortBlanksFirst() {
        var Column = this.GetActiveLanguageSortColumn();
        var Rows;
        var Reverse;
        var ColumnIndex;
        if (!(Column instanceof tp.GridColumn) || !(this.Grid instanceof tp.Grid) || !this.Grid.DataSource)
            return;
        Rows = this.Grid.DataSource.Rows;
        if (!tp.IsArray(Rows) || Rows.length < 2)
            return;
        Reverse = tp.IsSameText(Column.SortMode, "desc");
        ColumnIndex = Column.DataIndex;
        Rows.sort(function (A, B) {
            var ValueA = A instanceof tp.DataRow ? A.GetByIndex(ColumnIndex) : null;
            var ValueB = B instanceof tp.DataRow ? B.GetByIndex(ColumnIndex) : null;
            var BlankA = tp.IsNil(ValueA) || tp.IsBlankString(String(ValueA));
            var BlankB = tp.IsNil(ValueB) || tp.IsBlankString(String(ValueB));
            var Result;
            if (BlankA !== BlankB)
                return BlankA ? -1 : 1;
            if (BlankA === true)
                return 0;
            Result = String(ValueA).localeCompare(String(ValueB));
            return Reverse ? -Result : Result;
        });
        this.Grid.BuildGroups();
    }

    // ● public
    /**
     * Loads form data.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadDataAsync() {
        await this.LoadTranslationsAsync();
    }
    /**
     * Loads translations.
     * @returns {Promise<void>} Returns a Promise.
     */
    async LoadTranslationsAsync() {
        var Packet;
        var Languages;
        if (this.Loading === true) {
            this.ReloadPending = true;
            return;
        }
        this.Loading = true;
        try {
            Packet = await this.ExecuteWithSpinner(async function () {
                return await tp.AjaxRequest.ExecuteAsync("App.ResourceTranslations.Load");
            });
            Languages = Packet && tp.IsArray(Packet.Languages) ? Packet.Languages : [];
            if (Packet && tp.IsObject(Packet.Table) && this.Grid instanceof tp.Grid) {
                this.Table = new tp.DataTable(Packet.Table);
                this.Table.On("RowModified", this.HandleTableRowModified, this);
                this.Grid.DataSource = this.Table;
                this.Grid.DataSource.On("Sorted", this.HandleDataSourceSorted, this);
                this.ApplyGridColumns(Languages);
            }
        } finally {
            this.Loading = false;
            if (this.ReloadPending === true) {
                this.ReloadPending = false;
                this.LoadTranslationsAsync();
            }
        }
    }
    /**
     * Saves a modified translation cell.
     * @param {tp.DataTableEventArgs} Args The event arguments.
     * @returns {Promise<void>} Returns a Promise.
     */
    async SaveModifiedTranslationAsync(Args) {
        var Column;
        var Language;
        var Row;
        var ResKey;
        var ResValue;
        var ColumnIndex;
        if (!Args || !(Args.Column instanceof tp.DataColumn) || !(Args.Row instanceof tp.DataRow))
            return;
        Column = Args.Column;
        Language = this.LanguageMap[Column.Name];
        if (!Language || Language.IsEnglish === true)
            return;
        Row = Args.Row;
        ResKey = Row.Get("ResKey", "");
        if (tp.IsBlankString(ResKey))
            return;
        ResValue = tp.IsNil(Args.NewValue) ? "" : String(Args.NewValue);
        try {
            await tp.AjaxRequest.ExecuteAsync("App.ResourceTranslations.Save", {
                LanguageId: Language.Id,
                ResKey: ResKey,
                ResValue: ResValue
            });
            Row.AcceptChanges();
            if (tp.LogBox)
                tp.LogBox.AppendLine(tp._L("ResourceTranslationSaved", "Resource translation saved") + ": " + ResKey + " [" + Language.Code + "]");
        } catch (e) {
            ColumnIndex = this.Table ? this.Table.IndexOfColumn(Column) : -1;
            if (ColumnIndex >= 0)
                Row.Data[ColumnIndex] = Args.OldValue;
            if (this.Grid instanceof tp.Grid)
                this.Grid.Render();
            tp.ErrorNote(tp.ExceptionText(e));
        }
    }
    /**
     * Deletes the focused resource key.
     * @returns {Promise<void>} Returns a Promise.
     */
    async DeleteFocusedKeyAsync() {
        var Row = this.Grid instanceof tp.Grid ? this.Grid.FocusedRow : null;
        var ResKey = Row instanceof tp.DataRow ? Row.Get("ResKey", "") : "";
        var Message;
        var Confirmed;
        if (tp.IsBlankString(ResKey))
            return;
        Message = tp._L("ConfirmDeleteResourceTranslations", "Delete all translations for this resource key?") + "\n\n" + ResKey;
        Confirmed = await tp.YesNoBoxAsync(Message);
        if (Confirmed !== true)
            return;
        await this.ExecuteWithSpinner(async function () {
            return await tp.AjaxRequest.ExecuteAsync("App.ResourceTranslations.Delete", {
                ResKey: ResKey
            });
        });
        Row.Remove();
        if (tp.Broadcaster)
            tp.Broadcaster.Send("SysStrRes.Changed", this, { ResKey: ResKey });
        if (tp.LogBox)
            tp.LogBox.AppendLine(tp._L("ResourceTranslationsDeleted", "Resource translations deleted") + ": " + ResKey);
    }
};

// ● prototype
/**
 * Translation grid.
 * @type {tp.Grid|null}
 */
app.ResourceTranslationsForm.prototype.Grid = null;
/**
 * Translation table.
 * @type {tp.DataTable|null}
 */
app.ResourceTranslationsForm.prototype.Table = null;
/**
 * Language metadata keyed by grid column name.
 * @type {object|null}
 */
app.ResourceTranslationsForm.prototype.LanguageMap = null;
/**
 * True while loading data.
 * @type {boolean}
 */
app.ResourceTranslationsForm.prototype.Loading = false;
/**
 * True when a reload is waiting for the current load to finish.
 * @type {boolean}
 */
app.ResourceTranslationsForm.prototype.ReloadPending = false;
