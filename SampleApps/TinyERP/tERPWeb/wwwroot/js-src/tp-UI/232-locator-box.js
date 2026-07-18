// ● locator box
/**
 * Composite locator editor with one or more search text boxes and an ellipsis button.
 *
 * Events:
 * - Cleared
 * - Located
 */
tp.LocatorBox = class extends tp.Control {
    // ● constructor
    /**
     * Creates a locator box.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.LocatorBox.CreateLocatorBoxParams(CreateParams));
    }

    // ● static
    /**
     * Creates locator-box create params.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create params, handle, or selector.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateLocatorBoxParams(CreateParams) {
        var Args;
        if (CreateParams instanceof tp.CreateParams) {
            Args = new tp.CreateParams(CreateParams);
            if (tp.IsNil(Args.ElementOrSelector))
                Args.ElementOrSelector = "div";
            return Args;
        }
        Args = tp.IsObject(CreateParams) && !tp.IsHTMLElement(CreateParams) ? tp.Assign({}, CreateParams) : {};
        if (tp.IsHTMLElement(CreateParams) || tp.IsString(CreateParams))
            Args.ElementOrSelector = CreateParams;
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "div";
        return Args;
    }

    // ● protected
    /**
     * Initializes class metadata.
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Value";
    }
    /**
     * Initializes instance fields.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.LocatorName = "";
        this.ModuleName = "";
        this.TableName = "";
        this.ReferenceField = "";
        this.Fields = [];
        this.SearchFields = [];
        this.MinimumSearchLength = 2;
        this.IsMultiRow = true;
        this.Value = null;
        this.Result = null;
        this.TargetRow = null;
        this.Info = null;
        this.ReferenceContextMenu = null;
        this.fInputs = [];
        this.fInputMap = {};
        this.fButton = null;
        this.fDropDownBox = null;
        this.fTable = null;
        this.fInfoPromise = null;
        this.fMinimumSearchLengthAssigned = false;
        this.fSelectedIndex = -1;
        this.fSearchToken = 0;
        this.fActiveInput = null;
        this.fInputHandler = this.FuncBind(this.HandleInput);
        this.fInputKeyDownHandler = this.FuncBind(this.HandleInputKeyDown);
        this.fButtonClickHandler = this.FuncBind(this.HandleButtonClick);
        this.fTableClickHandler = this.FuncBind(this.HandleTableClick);
        this.fTableDblClickHandler = this.FuncBind(this.HandleTableDoubleClick);
        this.fDropDownKeyDownHandler = this.FuncBind(this.HandleDropDownKeyDown);
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.LocatorBox);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateControls();
    }
    /**
     * Applies explicit create params.
     * @param {tp.CreateParams|object|null|undefined} Params The create params.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.LocatorName))
            this.LocatorName = String(Params.LocatorName);
        if (!tp.IsNil(Params.ModuleName))
            this.ModuleName = String(Params.ModuleName);
        if (!tp.IsNil(Params.Module))
            this.ModuleName = String(Params.Module);
        if (!tp.IsNil(Params.TableName))
            this.TableName = String(Params.TableName);
        if (!tp.IsNil(Params.ReferenceField))
            this.ReferenceField = String(Params.ReferenceField);
        if (tp.IsArray(Params.Fields))
            this.Fields = Params.Fields.slice();
        if (tp.IsArray(Params.SearchFields))
            this.SearchFields = Params.SearchFields.slice();
        if (!tp.IsNil(Params.MinimumSearchLength)) {
            this.MinimumSearchLength = tp.ToInt(Params.MinimumSearchLength);
            this.fMinimumSearchLengthAssigned = true;
        }
        if (!tp.IsNil(Params.TargetRow))
            this.TargetRow = Params.TargetRow;
        if (!tp.IsNil(Params.ReferenceContextMenu))
            this.ReferenceContextMenu = Params.ReferenceContextMenu;
        if (!tp.IsNil(Params.IsMultiRow))
            this.IsMultiRow = Params.IsMultiRow === true;
        this.RebuildInputs();
    }
    /**
     * Releases owned controls and handlers.
     * @returns {void}
     */
    DoDispose() {
        this.DestroyInputs();
        if (this.fButton)
            this.fButton.removeEventListener("click", this.fButtonClickHandler, false);
        if (this.fTable) {
            this.fTable.removeEventListener("click", this.fTableClickHandler, false);
            this.fTable.removeEventListener("dblclick", this.fTableDblClickHandler, false);
        }
        if (this.fDropDownBox) {
            this.fDropDownBox.Handle.removeEventListener("keydown", this.fDropDownKeyDownHandler, false);
            this.fDropDownBox.Dispose();
            this.fDropDownBox = null;
        }
        this.fButton = null;
        this.fTable = null;
        super.DoDispose();
    }
    /**
     * Creates child controls.
     * @returns {void}
     */
    CreateControls() {
        this.fButton = this.Document.createElement("button");
        this.fButton.type = "button";
        this.fButton.tabIndex = -1;
        this.fButton.className = tp.Classes.Btn + " " + tp.Classes.LocatorBoxButton;
        this.fButton.textContent = "...";
        this.fButton.addEventListener("click", this.fButtonClickHandler, false);
        this.Handle.appendChild(this.fButton);
        this.fDropDownBox = new tp.DropDownBox(null, {
            Associate: this.Handle,
            Owner: this
        });
        tp.AddClass(this.fDropDownBox.Handle, tp.Classes.LocatorBoxDropDown);
        this.fTable = this.Document.createElement("table");
        this.fTable.className = tp.Classes.LocatorBoxTable;
        this.fTable.tabIndex = -1;
        this.fTable.addEventListener("click", this.fTableClickHandler, false);
        this.fTable.addEventListener("dblclick", this.fTableDblClickHandler, false);
        this.fDropDownBox.Handle.addEventListener("keydown", this.fDropDownKeyDownHandler, false);
        this.fDropDownBox.Handle.appendChild(this.fTable);
        this.RebuildInputs();
    }
    /**
     * Destroys inner input elements.
     * @returns {void}
     */
    DestroyInputs() {
        var Index;
        var Input;
        for (Index = 0; Index < this.fInputs.length; Index++) {
            Input = this.fInputs[Index];
            Input.removeEventListener("input", this.fInputHandler, false);
            Input.removeEventListener("keydown", this.fInputKeyDownHandler, false);
            if (Input.parentNode === this.Handle)
                this.Handle.removeChild(Input);
        }
        this.fInputs = [];
        this.fInputMap = {};
    }
    /**
     * Rebuilds inner input elements.
     * @returns {void}
     */
    RebuildInputs() {
        var Fields = this.GetInputFields();
        var Index;
        var Input;
        this.DestroyInputs();
        for (Index = 0; Index < Fields.length; Index++) {
            Input = this.CreateInput(Fields[Index], Index, Fields.length);
            this.Handle.insertBefore(Input, this.fButton);
            this.fInputs.push(Input);
            this.fInputMap[Fields[Index]] = Input;
        }
        this.ApplyReadOnly();
    }
    /**
     * Creates an inner input element.
     * @param {string} FieldName The locator field name.
     * @param {number} Index The field index.
     * @param {number} Count The field count.
     * @returns {HTMLInputElement} Returns the created input.
     */
    CreateInput(FieldName, Index, Count) {
        var Result = this.Document.createElement("input");
        Result.type = "text";
        Result.spellcheck = false;
        Result.autocomplete = "off";
        Result.placeholder = FieldName;
        Result.dataset.field = FieldName;
        Result.className = tp.Classes.LocatorBoxInput;
        Result.style.flex = Index === Count - 1 ? "1 1 auto" : "0 0 120px";
        Result.addEventListener("input", this.fInputHandler, false);
        Result.addEventListener("keydown", this.fInputKeyDownHandler, false);
        return Result;
    }
    /**
     * Applies read-only state to child inputs.
     * @returns {void}
     */
    ApplyReadOnly() {
        var Index;
        for (Index = 0; Index < this.fInputs.length; Index++)
            this.fInputs[Index].readOnly = this.ReadOnly === true || !this.IsSearchField(this.fInputs[Index].dataset.field);
    }
    /**
     * Called after ReadOnly changes.
     * @returns {void}
     */
    OnReadOnlyChanged() {
        this.ApplyReadOnly();
        super.OnReadOnlyChanged();
    }
    /**
     * Binds the control to its data source.
     * @returns {void}
     */
    Bind() {
        super.Bind();
        this.ReadDataValue();
    }
    /**
     * Called when the control should clear its data display.
     * @returns {void}
     */
    OnClearDataDisplay() {
        super.OnClearDataDisplay();
        this.ClearInputValues();
        this.Value = null;
        this.Result = null;
        this.CloseDropDown();
    }
    /**
     * Returns the input fields.
     * @returns {string[]} Returns field names.
     */
    GetInputFields() {
        if (tp.IsArray(this.Fields) && this.Fields.length > 0)
            return this.Fields.slice();
        if (tp.IsArray(this.SearchFields) && this.SearchFields.length > 0)
            return this.SearchFields.slice();
        return ["Code", "Name"];
    }
    /**
     * Returns true when a field can be used for searching.
     * @param {string} FieldName The field name.
     * @returns {boolean} Returns true when searchable.
     */
    IsSearchField(FieldName) {
        if (!tp.IsArray(this.SearchFields) || this.SearchFields.length === 0)
            return true;
        return this.SearchFields.some(function (Item) { return tp.IsSameText(Item, FieldName); });
    }
    /**
     * Returns the current target row.
     * @returns {tp.DataRow|object|null} Returns the current target row.
     */
    GetTargetRow() {
        if (this.TargetRow)
            return this.TargetRow;
        return this.DataSource ? this.DataSource.Current : null;
    }
    /**
     * Returns true when a term contains the locator search trigger.
     * @param {string} Term The search text.
     * @returns {boolean} Returns true when the term ends with question mark.
     */
    ContainsSearchTrigger(Term) {
        return !tp.IsBlank(Term) && String(Term).trimEnd().endsWith("?");
    }
    /**
     * Returns a normalized search term without the trigger.
     * @param {string} Term The search text.
     * @returns {string} Returns the normalized search text.
     */
    GetSearchTerm(Term) {
        return !tp.IsBlank(Term) ? String(Term).trim().replace(/\?+$/g, "").trim() : "";
    }
    /**
     * Returns the locator name.
     * @returns {string} Returns the locator name.
     */
    GetLocatorName() {
        return this.LocatorName || (this.DataColumn ? this.DataColumn.Locator : "");
    }
    /**
     * Creates a metadata request.
     * @returns {object} Returns request parameters.
     */
    CreateInfoRequest() {
        return {
            LocatorName: this.GetLocatorName(),
            ModuleName: this.ModuleName,
            TableName: this.TableName,
            ReferenceField: this.ReferenceField || this.DataField
        };
    }
    /**
     * Ensures locator metadata is loaded.
     * @returns {Promise<tp.LocatorInfo|null>} Returns locator metadata.
     */
    async EnsureInfoAsync() {
        if (this.Info instanceof tp.LocatorInfo)
            return this.Info;
        if (tp.IsBlank(this.GetLocatorName()))
            return null;
        if (!this.fInfoPromise)
            this.fInfoPromise = tp.Locator.GetInfoAsync(this.CreateInfoRequest());
        this.Info = await this.fInfoPromise;
        this.ApplyInfo(this.Info);
        return this.Info;
    }
    /**
     * Applies locator metadata.
     * @param {tp.LocatorInfo|null} Info The locator metadata.
     * @returns {void}
     */
    ApplyInfo(Info) {
        var LocatorDef = Info instanceof tp.LocatorInfo ? Info.Locator : null;
        if (!(LocatorDef instanceof tp.LocatorDef))
            return;
        if (tp.IsArray(this.Fields) && this.Fields.length === 0)
            this.Fields = LocatorDef.GetInputFields(this.IsMultiRow);
        if (tp.IsArray(this.SearchFields) && this.SearchFields.length === 0)
            this.SearchFields = LocatorDef.GetSearchFields(this.IsMultiRow);
        if (this.fMinimumSearchLengthAssigned !== true)
            this.MinimumSearchLength = LocatorDef.MinimumSearchLength > 0 ? LocatorDef.MinimumSearchLength : 2;
        this.RebuildInputs();
    }
    /**
     * Returns the current mapping plan.
     * @returns {tp.LocatorMapPlan|null} Returns the mapping plan.
     */
    GetMapPlan() {
        if (this.Info instanceof tp.LocatorInfo && this.Info.MapPlan instanceof tp.LocatorMapPlan)
            return this.Info.MapPlan;
        if (this.Result instanceof tp.LocatorResult && this.Result.MapPlan instanceof tp.LocatorMapPlan)
            return this.Result.MapPlan;
        return null;
    }
    /**
     * Finds a mapping item by locator source field name.
     * @param {tp.LocatorMapPlan|null} Plan The mapping plan.
     * @param {string} SourceField The source field name.
     * @returns {tp.LocatorMapItem|null} Returns the mapping item or null.
     */
    FindMapItemBySourceField(Plan, SourceField) {
        var Index;
        var Item;
        if (!(Plan instanceof tp.LocatorMapPlan) || tp.IsBlank(SourceField))
            return null;
        for (Index = 0; Index < Plan.Items.length; Index++) {
            Item = Plan.Items[Index];
            if (Item instanceof tp.LocatorMapItem && tp.IsSameText(Item.SourceField, SourceField))
                return Item;
        }
        return null;
    }
    /**
     * Returns fields displayed by the popup table.
     * @param {tp.DataTable} Table The result table.
     * @returns {string[]} Returns display field names.
     */
    GetPopupFields(Table) {
        var LocatorDef = this.Info instanceof tp.LocatorInfo ? this.Info.Locator : null;
        var Result = LocatorDef instanceof tp.LocatorDef ? LocatorDef.GetListVisibleFields() : [];
        var Index;
        if (Result.length === 0 && Table instanceof tp.DataTable) {
            for (Index = 0; Index < Table.Columns.length; Index++)
                Result.push(Table.Columns[Index].Name);
        }
        return Result;
    }
    /**
     * Sets a target row field value.
     * @param {tp.DataRow|object|null} TargetRow The target row.
     * @param {string} FieldName The target field name.
     * @param {*} Value The field value.
     * @returns {void}
     */
    SetTargetValue(TargetRow, FieldName, Value) {
        if (tp.IsEmpty(TargetRow) || tp.IsBlank(FieldName))
            return;
        if (TargetRow instanceof tp.DataRow)
            TargetRow.Set(FieldName, Value);
        else
            TargetRow[FieldName] = Value;
    }
    /**
     * Gets a target row field value.
     * @param {tp.DataRow|object|null} TargetRow The target row.
     * @param {string} FieldName The target field name.
     * @param {*} Default The default value.
     * @returns {*} Returns the field value.
     */
    GetTargetValue(TargetRow, FieldName, Default) {
        if (tp.IsEmpty(TargetRow) || tp.IsBlank(FieldName))
            return Default;
        if (TargetRow instanceof tp.DataRow)
            return TargetRow.Get(FieldName, Default);
        return FieldName in TargetRow ? TargetRow[FieldName] : Default;
    }
    /**
     * Formats an input display value.
     * @param {*} Value The value to format.
     * @returns {string} Returns display text.
     */
    FormatInputValue(Value) {
        if (tp.IsNil(Value))
            return "";
        if (tp.IsString(Value) || tp.IsNumber(Value) || tp.IsBoolean(Value))
            return String(Value);
        if (tp.IsDate(Value))
            return tp.FormatDateTime(Value, tp.DateFormatISO);
        return "";
    }
    /**
     * Clears inner input values.
     * @returns {void}
     */
    ClearInputValues() {
        var Index;
        for (Index = 0; Index < this.fInputs.length; Index++)
            this.fInputs[Index].value = "";
    }
    /**
     * Refreshes input display values from the target row and map plan.
     * @returns {void}
     */
    RefreshInputValuesFromTargetRow() {
        var Plan = this.GetMapPlan();
        var TargetRow = this.GetTargetRow();
        var Index;
        var Input;
        var Item;
        var Value;
        if (!(Plan instanceof tp.LocatorMapPlan) || tp.IsEmpty(TargetRow)) {
            this.ClearInputValues();
            return;
        }
        for (Index = 0; Index < this.fInputs.length; Index++) {
            Input = this.fInputs[Index];
            Item = this.FindMapItemBySourceField(Plan, Input.dataset.field || "");
            Value = Item instanceof tp.LocatorMapItem ? this.GetTargetValue(TargetRow, Item.TargetField, "") : "";
            Input.value = this.FormatInputValue(Value);
        }
    }
    /**
     * Refreshes input display values after ensuring locator metadata is available.
     * @returns {Promise<void>} Returns a promise.
     */
    async RefreshInputValuesFromTargetRowAsync() {
        await this.EnsureInfoAsync();
        this.RefreshInputValuesFromTargetRow();
    }
    /**
     * Returns true when all input values are blank.
     * @returns {boolean} Returns true when all input values are blank.
     */
    AreInputsBlank() {
        var Index;
        for (Index = 0; Index < this.fInputs.length; Index++) {
            if (!tp.IsBlank(this.fInputs[Index].value))
                return false;
        }
        return true;
    }
    /**
     * Clears mapped target values and input values.
     * @returns {void}
     */
    Clear() {
        var Plan = this.GetMapPlan();
        var TargetRow = this.GetTargetRow();
        var Index;
        this.fSearchToken++;
        this.ClearInputValues();
        if (Plan instanceof tp.LocatorMapPlan) {
            for (Index = 0; Index < Plan.Items.length; Index++)
                this.SetTargetValue(TargetRow, Plan.Items[Index].TargetField, null);
        }
        this.Value = null;
        this.Result = null;
        this.CloseDropDown();
        this.Trigger("Cleared", { TargetRow: TargetRow });
    }
    /**
     * Shows locator result feedback when needed.
     * @param {tp.LocatorResult} Result The locator result.
     * @returns {void}
     */
    NotifyResultIssue(Result) {
        var Message;
        if (!(Result instanceof tp.LocatorResult))
            return;
        if (Result.HasTooManyResults)
            Message = !tp.IsBlank(Result.Message) ? Result.Message : "Too many results. Type more characters.";
        else if (Result.Status === tp.LocatorResultStatus.NoResult)
            Message = !tp.IsBlank(Result.Message) ? Result.Message : "No rows found.";
        if (!tp.IsBlank(Message) && tp.IsFunction(tp.InfoNote))
            tp.InfoNote(Message);
    }
    /**
     * Creates the locator request.
     * @param {HTMLInputElement|null} Input The active input.
     * @returns {object} Returns request parameters.
     */
    CreateRequest(Input) {
        var FieldName = Input ? Input.dataset.field || "" : "";
        return {
            LocatorName: this.GetLocatorName(),
            SearchTerm: Input ? this.GetSearchTerm(Input.value || "") : "",
            SearchField: FieldName,
            IsMultiRow: this.IsMultiRow === true,
            ModuleName: this.ModuleName,
            TableName: this.TableName,
            ReferenceField: this.ReferenceField || this.DataField
        };
    }
    /**
     * Executes a locator search.
     * @param {HTMLInputElement|null} Input The active input.
     * @param {boolean} ForceOpen True to open even with empty text.
     * @returns {Promise<void>} Returns a promise.
     */
    async SearchAsync(Input, ForceOpen) {
        var Result;
        var TargetRow;
        var Token = ++this.fSearchToken;
        var SearchText = Input ? this.GetSearchTerm(Input.value || "") : "";
        var SearchField = Input ? Input.dataset.field || "" : "";
        if (this.ReadOnly === true)
            return;
        await this.EnsureInfoAsync();
        if (Token !== this.fSearchToken)
            return;
        if (Input && this.fInputs.indexOf(Input) < 0) {
            Input = !tp.IsBlank(SearchField) && this.fInputMap[SearchField] ? this.fInputMap[SearchField] : this.fInputs[0] || null;
            if (Input && tp.IsBlank(Input.value))
                Input.value = SearchText;
        }
        this.fActiveInput = Input || this.fActiveInput || this.fInputs[0] || null;
        if (!this.fActiveInput)
            return;
        if (ForceOpen !== true && this.GetSearchTerm(this.fActiveInput.value || "").length < this.MinimumSearchLength)
            return;
        try {
            Result = await tp.Locator.ExecuteAsync(this.CreateRequest(this.fActiveInput));
            if (Token !== this.fSearchToken)
                return;
            this.Result = Result;
            if (Result.HasSingleResult && ForceOpen !== true) {
                TargetRow = this.GetTargetRow();
                Result.Apply(TargetRow);
                this.SetInputValuesFromRow(Result.FirstRow);
                this.OnLocated(Result, Result.FirstRow, TargetRow);
            } else if (Result.Table instanceof tp.DataTable && Result.Table.RowCount > 0) {
                this.RenderDropDown(Result.Table);
                this.OpenDropDown();
            } else {
                this.CloseDropDown();
                this.NotifyResultIssue(Result);
            }
        } catch (e) {
            this.CloseDropDown();
            if (tp.ErrorNote)
                tp.ErrorNote(tp.ExceptionText(e));
            else
                throw e;
        }
    }
    /**
     * Cancels the current locator operation.
     * @returns {void}
     */
    CancelOperation() {
        this.fSearchToken++;
        this.CloseDropDown();
    }
    /**
     * Renders the result drop-down table.
     * @param {tp.DataTable} Table The result table.
     * @returns {void}
     */
    RenderDropDown(Table) {
        var Html = [];
        var Fields = this.GetPopupFields(Table);
        var RowIndex;
        var ColIndex;
        var Row;
        var Column;
        var Value;
        Html.push("<thead><tr>");
        for (ColIndex = 0; ColIndex < Fields.length; ColIndex++) {
            Column = Table.FindColumn(Fields[ColIndex]);
            if (Column)
                Html.push("<th>", tp.EncodeHtml(Column.DisplayTitle || Column.Name), "</th>");
        }
        Html.push("</tr></thead><tbody>");
        for (RowIndex = 0; RowIndex < Table.Rows.length; RowIndex++) {
            Row = Table.Rows[RowIndex];
            Html.push("<tr data-index='", RowIndex.toString(), "'>");
            for (ColIndex = 0; ColIndex < Fields.length; ColIndex++) {
                Column = Table.FindColumn(Fields[ColIndex]);
                if (Column) {
                    Value = Column.Format(Row.Get(Column), true);
                    Html.push("<td>", tp.EncodeHtml(tp.IsNil(Value) ? "" : Value), "</td>");
                }
            }
            Html.push("</tr>");
        }
        Html.push("</tbody>");
        this.fTable.innerHTML = Html.join("");
        this.SetSelectedIndex(0);
    }
    /**
     * Opens the drop-down.
     * @returns {void}
     */
    OpenDropDown() {
        var HostWidth = this.Handle.getBoundingClientRect().width;
        var TableWidth;
        var ViewWidth = Math.max(320, tp.Viewport.Width - 24);
        this.fDropDownBox.Width = Math.min(Math.max(HostWidth, 320), ViewWidth);
        this.fDropDownBox.Open();
        TableWidth = this.fTable ? this.fTable.scrollWidth + 20 : 0;
        this.fDropDownBox.Width = Math.min(Math.max(HostWidth, TableWidth, 320), ViewWidth);
        this.fDropDownBox.KeepInsideViewport();
        this.FocusDropDown();
    }
    /**
     * Focuses the drop-down content.
     * @returns {void}
     */
    FocusDropDown() {
        var Table = this.fTable;
        if (Table) {
            setTimeout(function () {
                if (tp.IsFunction(Table.focus))
                    Table.focus({ preventScroll: true });
            }, 0);
        }
    }
    /**
     * Closes the drop-down.
     * @returns {void}
     */
    CloseDropDown() {
        if (this.fDropDownBox)
            this.fDropDownBox.Close();
    }
    /**
     * Sets the selected result row index.
     * @param {number} Index The selected index.
     * @returns {void}
     */
    SetSelectedIndex(Index) {
        var Rows = this.fTable ? this.fTable.querySelectorAll("tbody tr") : [];
        var OldRow;
        var NewRow;
        if (Rows.length === 0) {
            this.fSelectedIndex = -1;
            return;
        }
        Index = Math.max(0, Math.min(Index, Rows.length - 1));
        OldRow = this.fTable.querySelector("tbody tr." + tp.Classes.Selected);
        if (OldRow)
            tp.RemoveClass(OldRow, tp.Classes.Selected);
        NewRow = Rows[Index];
        tp.AddClass(NewRow, tp.Classes.Selected);
        this.fSelectedIndex = Index;
        this.ScrollSelectedRowIntoView(NewRow);
    }
    /**
     * Scrolls a selected drop-down row into view.
     * @param {HTMLTableRowElement} Row The selected row.
     * @returns {void}
     */
    ScrollSelectedRowIntoView(Row) {
        var Box = this.fDropDownBox ? this.fDropDownBox.Handle : null;
        var RowTop;
        var RowBottom;
        if (!Row || !Box)
            return;
        RowTop = Row.offsetTop;
        RowBottom = RowTop + Row.offsetHeight;
        if (RowTop < Box.scrollTop)
            Box.scrollTop = RowTop;
        else if (RowBottom > Box.scrollTop + Box.clientHeight)
            Box.scrollTop = RowBottom - Box.clientHeight;
    }
    /**
     * Selects the current popup row.
     * @returns {void}
     */
    SelectCurrentRow() {
        var Row = this.Result && this.Result.Table instanceof tp.DataTable && this.fSelectedIndex >= 0 ? this.Result.Table.Rows[this.fSelectedIndex] : null;
        var TargetRow = this.GetTargetRow();
        if (this.Result instanceof tp.LocatorResult && Row) {
            this.Result.Apply(TargetRow, Row);
            this.SetInputValuesFromRow(Row);
            this.OnLocated(this.Result, Row, TargetRow);
        }
        this.CloseDropDown();
    }
    /**
     * Sets input display values from a locator result row.
     * @param {tp.DataRow|null} Row The result row.
     * @returns {void}
     */
    SetInputValuesFromRow(Row) {
        var Index;
        var Input;
        if (!(Row instanceof tp.DataRow))
            return;
        for (Index = 0; Index < this.fInputs.length; Index++) {
            Input = this.fInputs[Index];
            Input.value = this.FormatInputValue(Row.Get(Input.dataset.field, ""));
        }
    }
    /**
     * Reads the bound row value and refreshes locator display values.
     * @returns {void}
     */
    ReadDataValue() {
        var Value;
        if (this.ReadingDataValue === true || this.WritingDataValue === true)
            return;
        if (this.IsDataBound && this.DataSource.Position >= 0) {
            this.ReadingDataValue = true;
            try {
                Value = this.DataSource.Get(this.DataField);
                this.Value = this.DataValueToDataProperty(Value);
                this.RefreshInputValuesFromTargetRowAsync().catch(function (e) {
                    if (tp.LogBox && tp.LogBox.AppendLine)
                        tp.LogBox.AppendLine("LocatorBox refresh failed: " + tp.ExceptionText(e));
                });
            } finally {
                this.ReadingDataValue = false;
            }
        }
    }
    /**
     * Handles input.
     * @param {InputEvent} e The DOM event.
     * @returns {void}
     */
    HandleInput(e) {
        this.fActiveInput = e.target;
        if (this.ContainsSearchTrigger(e.target.value)) {
            e.target.value = this.GetSearchTerm(e.target.value);
            this.SearchAsync(e.target, false);
        } else if (this.AreInputsBlank()) {
            this.Clear();
        }
    }
    /**
     * Handles input keydown.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleInputKeyDown(e) {
        if (tp.IsKey(e, tp.Keys.Enter)) {
            tp.CancelEvent(e, true);
            if (this.fDropDownBox.IsOpen)
                this.SelectCurrentRow();
        } else if (tp.IsKey(e, tp.Keys.Down)) {
            tp.CancelEvent(e, true);
            if (this.fDropDownBox.IsOpen)
                this.SetSelectedIndex(this.fSelectedIndex + 1);
        } else if (tp.IsKey(e, tp.Keys.Up) && this.fDropDownBox.IsOpen) {
            tp.CancelEvent(e, true);
            this.SetSelectedIndex(this.fSelectedIndex - 1);
        } else if (tp.IsKey(e, tp.Keys.Escape)) {
            tp.CancelEvent(e, true);
            this.CloseDropDown();
        }
    }
    /**
     * Handles ellipsis button click.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleButtonClick(e) {
        tp.CancelEvent(e, true);
        if (this.ReferenceContextMenu && tp.IsFunction(this.ReferenceContextMenu.OpenAsync)) {
            this.ReferenceContextMenu.OpenAsync(e).catch(function (Error) {
                if (tp.ErrorNote)
                    tp.ErrorNote(tp.ExceptionText(Error));
            });
            return;
        }
        if (this.fDropDownBox.IsOpen) {
            this.CloseDropDown();
            return;
        }
        this.SearchAsync(this.fActiveInput || this.fInputs[0] || null, true);
    }
    /**
     * Handles drop-down row click.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleTableClick(e) {
        var Row = e.target;
        while (Row && Row !== this.fTable && Row.tagName !== "TR")
            Row = Row.parentNode;
        if (Row && Row.dataset && !tp.IsBlank(Row.dataset.index))
            this.SetSelectedIndex(tp.ToInt(Row.dataset.index));
    }
    /**
     * Handles drop-down row double click.
     * @param {MouseEvent} e The DOM event.
     * @returns {void}
     */
    HandleTableDoubleClick(e) {
        tp.CancelEvent(e);
        this.HandleTableClick(e);
        this.SelectCurrentRow();
    }
    /**
     * Handles drop-down keydown.
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {void}
     */
    HandleDropDownKeyDown(e) {
        if (tp.IsKey(e, tp.Keys.Enter)) {
            tp.CancelEvent(e, true);
            this.SelectCurrentRow();
        } else if (tp.IsKey(e, tp.Keys.Escape)) {
            tp.CancelEvent(e, true);
            this.CloseDropDown();
            if (this.fActiveInput)
                this.fActiveInput.focus();
        } else if (tp.IsKey(e, tp.Keys.Down)) {
            tp.CancelEvent(e, true);
            this.SetSelectedIndex(this.fSelectedIndex + 1);
        } else if (tp.IsKey(e, tp.Keys.Up)) {
            tp.CancelEvent(e, true);
            this.SetSelectedIndex(this.fSelectedIndex - 1);
        }
    }
    /**
     * Triggers the Located event.
     * @param {tp.LocatorResult} Result The locator result.
     * @param {tp.DataRow} SourceRow The selected source row.
     * @param {tp.DataRow|object|null} TargetRow The target row.
     * @returns {void}
     */
    OnLocated(Result, SourceRow, TargetRow) {
        this.Value = TargetRow instanceof tp.DataRow && !tp.IsBlank(this.DataField) ? TargetRow.Get(this.DataField, null) : this.Value;
        this.Trigger("Located", { Result: Result, SourceRow: SourceRow, TargetRow: TargetRow });
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorBox.prototype.tpClass = "tp.LocatorBox";
/**
 * Optional reference context menu.
 * @type {*}
 */
tp.LocatorBox.prototype.ReferenceContextMenu = null;

tp.Ui.RegisterType(["LocatorBox", "tp-LocatorBox"], tp.LocatorBox);
