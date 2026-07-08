// ● inplace editor
/**
 * Base class for grid inplace editors.
 */
tp.GridInplaceEditor = class extends tp.Object {
    // ● constructor
    /**
     * Creates an inplace editor.
     * @param {tp.GridColumn} Column The associated grid column.
     */
    constructor(Column) {
        super();
        this.fColumn = Column;
        this.CreateControl();
    }

    // ● protected
    /**
     * Creates the editor control.
     * @protected
     * @returns {void}
     */
    CreateControl() {
    }
    /**
     * Shows the editor control.
     * @protected
     * @returns {void}
     */
    ShowControl() {
    }
    /**
     * Hides the editor control.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    HideControl(PostChanges) {
    }
    /**
     * Renders the edited value to the cell.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    RenderCell(PostChanges) {
    }
    /**
     * Called after cell, row, and value are assigned.
     * @protected
     * @returns {void}
     */
    CellAssigned() {
    }

    // ● public
    /**
     * Shows the editor in a cell.
     * @param {HTMLElement} Cell The cell.
     * @returns {void}
     */
    Show(Cell) {
        if (this.Column.DataType === tp.DataType.None
            || this.Column.DataType === tp.DataType.TextBlob
            || this.Column.DataType === tp.DataType.Blob)
            return;
        this.fCell = Cell;
        this.fRow = this.Column.Grid.GetElementInfo(Cell).Node.Row;
        this.fValue = this.Column.GetValue(this.fRow);
        if (tp.IsEmpty(this.Control)) {
            this.CreateControl();
            if (!tp.IsEmpty(this.Control))
                this.Control.Handle.style.top = "-10000px";
        }
        this.CellAssigned();
        if (!tp.IsEmpty(this.Control)) {
            if (this.Control.ParentHandle !== Cell)
                Cell.appendChild(this.Control.Handle);
            this.Control.Position = "absolute";
            this.Control.Handle.style.top = "0";
            this.Control.Handle.style.left = "0";
            this.Control.Handle.style.height = "100%";
            this.Control.Handle.style.width = "100%";
            this.Control.Handle.style.display = "";
            if (this.Control instanceof tp.Control && tp.IsEmpty(this.Control.DataSource))
                this.Control.DataSource = this.Column.Grid.DataSource;
            this.ShowControl();
        }
    }
    /**
     * Hides the editor.
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    Hide(PostChanges) {
        this.Column.Grid.AlteringData = true;
        try {
            this.HideControl(PostChanges);
            this.RenderCell(PostChanges);
            if (!tp.IsEmpty(this.Control))
                this.Control.Handle.style.display = "none";
            if (this.Column.IsAggregateColumn)
                this.Column.Grid.Render();
        } finally {
            this.Column.Grid.AlteringData = false;
        }
    }
    /**
     * Returns true when an element is inside this editor control.
     * @param {HTMLElement} Element The element.
     * @returns {boolean} Returns true when contained.
     */
    ContainsHandle(Element) {
        return !tp.IsEmpty(this.Control)
            && tp.IsElement(this.Control.Handle)
            && tp.ContainsElement(this.Control.Handle, Element);
    }

    // ● properties
    /**
     * Gets the associated column.
     * @returns {tp.GridColumn} Returns the column.
     */
    get Column() {
        return this.fColumn;
    }
    /**
     * Gets the editor control.
     * @returns {tp.Component|null} Returns the control.
     */
    get Control() {
        return this.fControl;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.GridInplaceEditor.prototype.tpClass = "tp.GridInplaceEditor";

// ● text editor
/**
 * Text-box inplace editor for the grid.
 */
tp.GridInplaceEditorTextBox = class extends tp.GridInplaceEditor {
    // ● constructor
    /**
     * Creates a text-box inplace editor.
     * @param {tp.GridColumn} Column The associated grid column.
     */
    constructor(Column) {
        super(Column);
        this.tpClass = "tp.GridInplaceEditorTextBox";
    }

    // ● protected
    /**
     * Creates the control.
     * @protected
     * @returns {void}
     */
    CreateControl() {
        this.fTextBox = new tp.TextBox();
        tp.AddClass(this.fTextBox.Handle, tp.Classes.GridInplaceEditor);
        tp.AddClass(this.fTextBox.Handle, tp.Classes.NoBrowserAppearance);
        this.fTextBox.TextAlign = tp.Alignment.ToText(this.Column.Alignment);
        this.fControl = this.fTextBox;
    }
    /**
     * Shows the control.
     * @protected
     * @returns {void}
     */
    ShowControl() {
        var Self = this;
        this.fTextBox.Text = this.Column.Format(this.fValue);
        this.fTextBox.Focus();
        setTimeout(function () {
            Self.fTextBox.Select();
        }, 0);
    }
    /**
     * Hides the control.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    HideControl(PostChanges) {
        var OldText;
        var Text;
        var Value;
        if (PostChanges === true) {
            OldText = this.Column.Format(this.fValue);
            Text = this.fTextBox.Text;
            if (Text !== OldText) {
                try {
                    Value = this.Column.Parse(Text);
                    this.Column.SetValue(this.fRow, Value);
                } catch (e) {
                    tp.ErrorNote(tp.ExceptionText(e));
                }
            }
        }
    }
    /**
     * Renders the cell.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    RenderCell(PostChanges) {
        if (PostChanges === true)
            this.Column.Render(this.fCell, this.fRow);
    }

    // ● properties
    /**
     * Gets the text box.
     * @returns {tp.TextBox} Returns the text box.
     */
    get TextBox() {
        return this.fTextBox;
    }
};

// ● checkbox editor
/**
 * Check-box inplace editor for the grid.
 */
tp.GridInplaceEditorCheckBox = class extends tp.GridInplaceEditor {
    // ● constructor
    /**
     * Creates a check-box inplace editor.
     * @param {tp.GridColumn} Column The associated grid column.
     */
    constructor(Column) {
        super(Column);
        this.tpClass = "tp.GridInplaceEditorCheckBox";
    }

    // ● protected
    /**
     * Creates the control.
     * @protected
     * @returns {void}
     */
    CreateControl() {
        this.fControl = new tp.Component();
        tp.AddClass(this.fControl.Handle, tp.Classes.GridInplaceEditorCheckBox);
        this.fCheckBox = this.Column.Handle.ownerDocument.createElement("input");
        this.fCheckBox.type = "checkbox";
        this.fControl.Handle.appendChild(this.fCheckBox);
    }
    /**
     * Shows the control.
     * @protected
     * @returns {void}
     */
    ShowControl() {
        this.fCheckBox.checked = Boolean(this.fValue);
        this.fCheckBox.focus();
    }
    /**
     * Hides the control.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    HideControl(PostChanges) {
        if (PostChanges === true && this.fCheckBox.checked !== Boolean(this.fValue))
            this.Column.SetValue(this.fRow, this.fCheckBox.checked);
    }
    /**
     * Renders the cell.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    RenderCell(PostChanges) {
        if (PostChanges === true)
            this.Column.Render(this.fCell, this.fRow);
    }

    // ● properties
    /**
     * Gets the native check box.
     * @returns {HTMLInputElement} Returns the check box.
     */
    get CheckBox() {
        return this.fCheckBox;
    }
};

// ● combo-box editor
/**
 * Lookup combo-box inplace editor for the grid.
 */
tp.GridInplaceEditorComboBox = class extends tp.GridInplaceEditor {
    // ● constructor
    /**
     * Creates a combo-box inplace editor.
     * @param {tp.GridColumn} Column The associated grid column.
     */
    constructor(Column) {
        super(Column);
        this.tpClass = "tp.GridInplaceEditorComboBox";
    }

    // ● protected
    /**
     * Creates the control.
     * @protected
     * @returns {void}
     */
    CreateControl() {
        this.fComboBox = new tp.ComboBox();
        tp.AddClass(this.fComboBox.Handle, tp.Classes.GridInplaceEditor);
        this.fComboBox.ListOnly = true;
        this.fControl = this.fComboBox;
    }
    /**
     * Shows the control.
     * @protected
     * @returns {void}
     */
    ShowControl() {
        if (tp.IsBlank(this.fComboBox.DataField)) {
            this.fComboBox.DataField = this.Column.Name;
            this.fComboBox.DataSource = this.Column.Grid.DataSource;
        }
        this.fComboBox.Handle.style.zIndex = String(tp.ZIndex(this.fCell) + 100);
    }
    /**
     * Hides the control.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    HideControl(PostChanges) {
        this.fComboBox.Close();
    }

    // ● properties
    /**
     * Gets the combo-box.
     * @returns {tp.ComboBox} Returns the combo-box.
     */
    get ComboBox() {
        return this.fComboBox;
    }
};

// ● locator editor
/**
 * Locator inplace editor for the grid.
 *
 * Events:
 * - Located
 * - Cleared
 */
tp.GridInplaceEditorLocator = class extends tp.GridInplaceEditor {
    // ● constructor
    /**
     * Creates a locator inplace editor.
     * @param {tp.GridColumn} Column The associated grid column.
     */
    constructor(Column) {
        super(Column);
        this.tpClass = "tp.GridInplaceEditorLocator";
    }

    // ● protected
    /**
     * Creates the control.
     * @protected
     * @returns {void}
     */
    CreateControl() {
        this.fLocatorBox = new tp.LocatorBox();
        tp.AddClass(this.fLocatorBox.Handle, tp.Classes.GridInplaceEditor);
        tp.AddClass(this.fLocatorBox.Handle, tp.Classes.GridInplaceEditorLocator);
        this.fLocatorBox.IsMultiRow = true;
        this.fLocatorBox.On("Located", this.LocatorBox_Located, this);
        this.fLocatorBox.On("Cleared", this.LocatorBox_Cleared, this);
        this.fControl = this.fLocatorBox;
    }
    /**
     * Called after cell, row, and value are assigned.
     * @protected
     * @returns {void}
     */
    CellAssigned() {
        this.ApplyColumnLocatorParams();
        this.fLocatorBox.TargetRow = this.fRow;
        this.fLocatorBox.DataSource = this.Column.Grid.DataSource;
        this.fLocatorBox.DataField = this.Column.LocatorReferenceField;
    }
    /**
     * Shows the control.
     * @protected
     * @returns {void}
     */
    ShowControl() {
        var Self = this;
        this.fLocatorBox.ReadOnly = this.Column.ReadOnly === true;
        this.fLocatorBox.EnsureInfoAsync().then(function () {
            var Input;
            Self.fLocatorBox.RefreshInputValuesFromTargetRow();
            Input = Self.GetInput();
            if (Input) {
                setTimeout(function () {
                    Input.focus();
                    Input.select();
                }, 0);
            }
        }).catch(function (e) {
            if (tp.LogBox && tp.LogBox.AppendLine)
                tp.LogBox.AppendLine("Grid locator refresh failed: " + tp.ExceptionText(e));
        });
    }
    /**
     * Hides the control.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    HideControl(PostChanges) {
        this.fLocatorBox.CloseDropDown();
    }
    /**
     * Cancels the current locator operation.
     * @returns {void}
     */
    CancelOperation() {
        this.fLocatorBox.CancelOperation();
    }
    /**
     * Returns true when an element is inside this editor control or its drop-down.
     * @param {HTMLElement} Element The element.
     * @returns {boolean} Returns true when contained.
     */
    ContainsHandle(Element) {
        return super.ContainsHandle(Element)
            || this.fLocatorBox
            && this.fLocatorBox.fDropDownBox
            && tp.IsElement(this.fLocatorBox.fDropDownBox.Handle)
            && tp.ContainsElement(this.fLocatorBox.fDropDownBox.Handle, Element);
    }
    /**
     * Renders the cell.
     * @protected
     * @param {boolean} PostChanges True to post changes.
     * @returns {void}
     */
    RenderCell(PostChanges) {
        if (PostChanges === true)
            this.Column.Render(this.fCell, this.fRow);
    }
    /**
     * Applies column locator settings to the hosted locator box.
     * @protected
     * @returns {void}
     */
    ApplyColumnLocatorParams() {
        var Fields = this.Column.LocatorFields.length > 0 ? this.Column.LocatorFields.slice() : [this.Column.LocatorSearchField];
        var SearchFields = this.Column.LocatorSearchFields.length > 0 ? this.Column.LocatorSearchFields.slice() : [this.Column.LocatorSearchField];
        this.fLocatorBox.LocatorName = this.Column.LocatorName;
        this.fLocatorBox.ModuleName = this.Column.LocatorModuleName;
        this.fLocatorBox.TableName = this.Column.LocatorTableName;
        this.fLocatorBox.ReferenceField = this.Column.LocatorReferenceField;
        this.fLocatorBox.Fields = Fields;
        this.fLocatorBox.SearchFields = SearchFields;
        if (!tp.IsNil(this.Column.LocatorMinimumSearchLength)) {
            this.fLocatorBox.MinimumSearchLength = this.Column.LocatorMinimumSearchLength;
            this.fLocatorBox.fMinimumSearchLengthAssigned = true;
        }
        this.fLocatorBox.RebuildInputs();
    }
    /**
     * Returns the first hosted input.
     * @protected
     * @returns {HTMLInputElement|null} Returns the input.
     */
    GetInput() {
        return this.fLocatorBox && this.fLocatorBox.fInputs && this.fLocatorBox.fInputs.length > 0 ? this.fLocatorBox.fInputs[0] : null;
    }
    /**
     * Handles the locator box Located event.
     * @protected
     * @param {object} Args Event arguments.
     * @returns {void}
     */
    LocatorBox_Located(Args) {
        if (this.fCell && this.fRow)
            this.Column.Render(this.fCell, this.fRow);
    }
    /**
     * Handles the locator box Cleared event.
     * @protected
     * @param {object} Args Event arguments.
     * @returns {void}
     */
    LocatorBox_Cleared(Args) {
        if (this.fCell && this.fRow)
            this.Column.Render(this.fCell, this.fRow);
    }

    // ● properties
    /**
     * Gets the hosted locator box.
     * @returns {tp.LocatorBox} Returns the locator box.
     */
    get LocatorBox() {
        return this.fLocatorBox;
    }
};
