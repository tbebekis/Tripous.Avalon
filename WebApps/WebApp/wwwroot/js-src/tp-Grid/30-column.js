// ● grid column
/**
 * A grid column.
 */
tp.GridColumn = class extends tp.Object {
    // ● constructor
    /**
     * Creates a grid column.
     * @param {tp.Grid} Grid The owner grid.
     * @param {string|tp.DataColumn} NameOrDataColumn The data column name or data column.
     * @param {string|null|undefined} Text The display title.
     */
    constructor(Grid, NameOrDataColumn, Text) {
        super();
        this.fGrid = Grid;
        this.CreateHandle();
        this.InitializeFields();
        this.CreateControls();
        if (NameOrDataColumn instanceof tp.DataColumn) {
            this.fName = NameOrDataColumn.Name;
            this.fDataColumn = NameOrDataColumn;
            this.Text = NameOrDataColumn.DisplayTitle;
            this.ToolTip = NameOrDataColumn.DisplayToolTip;
            this.Decimals = NameOrDataColumn.Decimals;
        } else {
            this.Name = NameOrDataColumn;
            this.Text = Text || this.Name;
            this.ToolTip = this.Text;
            this.Bind();
        }
    }

    // ● protected
    /**
     * Creates the handle.
     * @protected
     * @returns {void}
     */
    CreateHandle() {
        this.fHandle = this.Grid.Document.createElement("div");
        this.Handle.className = tp.Classes.GridColumn;
        tp.GridColumn.SetInfo(this.Handle, this);
        tp.On(this.Handle, tp.Events.MouseDown, this);
        tp.On(this.Handle, tp.Events.MouseUp, this);
        tp.On(this.Handle, tp.Events.Click, this);
    }
    /**
     * Initializes fields.
     * @protected
     * @returns {void}
     */
    InitializeFields() {
        this.fLookUpTable = null;
        this.fLookUpDisplayDataColumn = null;
        this.fDecimals = 0;
        this.fAlignment = tp.Alignment.Left;
        this.fIsAlignmentSet = false;
        this.fWidth = 80;
        this.fLocalDate = true;
        this.fDisplaySeconds = false;
        this.fResizable = true;
        this.fGroupable = true;
        this.fResizeTimeStamp = null;
        this.fSortMode = "";
        this.fAggregate = tp.AggregateType.None;
        this.fReadOnly = false;
        this.fVisible = true;
    }
    /**
     * Creates child controls.
     * @protected
     * @returns {void}
     */
    CreateControls() {
        var Self = this;
        var CreateDiv = function (CssClasses) {
            var Result = Self.Handle.ownerDocument.createElement("div");
            tp.GridColumn.SetInfo(Result, Self);
            Result.className = CssClasses;
            return Result;
        };
        this.fTextContainer = CreateDiv(tp.Classes.Text);
        this.Handle.appendChild(this.fTextContainer);
        this.fTextContainer.draggable = true;
        this.fSorter = CreateDiv(tp.Classes.Sorter);
        this.Handle.appendChild(this.fSorter);
        this.fResizer = CreateDiv(tp.Classes.Resizer);
        this.Handle.appendChild(this.fResizer);
        this.fGroupCell = CreateDiv(tp.Classes.GroupCell);
        this.fFilterCell = CreateDiv(tp.Classes.FilterCell);
        this.fFilterCellTextBox = this.Handle.ownerDocument.createElement("input");
        this.fFilterCellTextBox.type = "text";
        this.fFilterCellTextBox.spellcheck = false;
        tp.GridColumn.SetInfo(this.fFilterCellTextBox, this);
        this.fFilterCell.appendChild(this.fFilterCellTextBox);
        this.fFilterCellTextBox.className = tp.Classes.FilterTextBox;
        this.fFilterGroupCell = CreateDiv(tp.Classes.GroupCell);
        this.fSummaryCell = CreateDiv(tp.Classes.SummaryCell);
        this.fSummaryGroupCell = CreateDiv(tp.Classes.GroupCell);
        tp.On(this.fTextContainer, tp.Events.DragStart, this.FuncBind(this.DragStart));
        tp.On(this.fTextContainer, tp.Events.DragEnd, this.FuncBind(this.DragEnd));
        tp.On(this.fTextContainer, tp.Events.DragEnter, this.FuncBind(this.DragEnter));
        tp.On(this.fTextContainer, tp.Events.DragLeave, this.FuncBind(this.DragLeave));
        tp.On(this.fTextContainer, tp.Events.DragOver, this.FuncBind(this.DragOver));
        tp.On(this.fTextContainer, tp.Events.DragDrop, this.FuncBind(this.DragDrop));
        tp.On(this.fFilterCellTextBox, tp.Events.InputChanged, this.FuncBind(this.FilterTextBox_TextChanged));
        tp.On(this.fSummaryCell, tp.Events.ContextMenu, this.FuncBind(this.FooterCell_ContextMenu));
    }
    /**
     * Notifies the grid when a property changes.
     * @protected
     * @param {string} PropName The property name.
     * @returns {void}
     */
    Changed(PropName) {
        if (this.Grid)
            this.Grid.ColumnChanged(this, PropName);
    }
    /**
     * Sets the column width.
     * @protected
     * @param {number|string} Value The width.
     * @returns {void}
     */
    SetWidth(Value) {
        this.fWidth = Value;
        if (tp.IsHTMLElement(this.Handle)) {
            this.Handle.style.width = tp.IsNumber(Value) ? tp.px(Value) : Value;
            if (tp.IsHTMLElement(this.fFilterCell))
                this.fFilterCell.style.width = this.Handle.style.width;
            if (tp.IsHTMLElement(this.fSummaryCell))
                this.fSummaryCell.style.width = this.Handle.style.width;
        }
    }
    /**
     * Sets the width of grouped complement cells.
     * @param {number|string} Value The width.
     * @returns {void}
     */
    SetGroupCellWidth(Value) {
        if (this.fGroupCell) {
            this.fGroupCell.style.width = tp.IsNumber(Value) ? tp.px(Value) : Value;
            this.fFilterGroupCell.style.width = this.fGroupCell.style.width;
            this.fSummaryGroupCell.style.width = this.fGroupCell.style.width;
        }
    }
    /**
     * Creates and returns the inplace editor.
     * @protected
     * @returns {tp.GridInplaceEditor} Returns the editor.
     */
    CreateEditor() {
        if (this.DataType === tp.DataType.Boolean || this.DataColumn && this.DataColumn.ColumnType === tp.DataColumnType.Boolean)
            return new tp.GridInplaceEditorCheckBox(this);
        return new tp.GridInplaceEditorTextBox(this);
    }
    /**
     * Returns lookup display value.
     * @protected
     * @param {*} Value The lookup value.
     * @returns {*} Returns the display value.
     */
    GetLookUpDisplayValue(Value) {
        var Table = this.LookUpTable;
        var Row;
        if (!tp.IsEmpty(Table)) {
            Row = Table.FindRow(this.ListValueField, Value);
            if (!tp.IsEmpty(Row))
                return Row.Get(this.ListDisplayField);
        }
        return null;
    }
    /**
     * Handles drag start.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragStart(e) {
        this.Grid.DraggedColumn = this;
        if (e.dataTransfer)
            e.dataTransfer.setData("text/plain", "just for the Firefox");
        tp.AddClass(this.fTextContainer, tp.Classes.DragSource);
    }
    /**
     * Handles drag end.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragEnd(e) {
        this.Grid.DraggedColumn = null;
        tp.RemoveClass(this.fTextContainer, tp.Classes.DragSource);
    }
    /**
     * Handles drag enter.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragEnter(e) {
        var Column;
        if (this.Grid.AllowUserToOrderColumns) {
            Column = this.Grid.DraggedColumn;
            if (Column && Column !== this)
                tp.AddClass(this.Handle, tp.Classes.DropTarget);
        }
    }
    /**
     * Handles drag leave.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragLeave(e) {
        var Column;
        if (this.Grid.AllowUserToOrderColumns) {
            Column = this.Grid.DraggedColumn;
            if (Column && Column !== this)
                tp.RemoveClass(this.Handle, tp.Classes.DropTarget);
        }
    }
    /**
     * Handles drag over.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragOver(e) {
        var Column;
        if (this.Grid.AllowUserToOrderColumns) {
            Column = this.Grid.DraggedColumn;
            if (Column && Column !== this) {
                if (e.preventDefault)
                    e.preventDefault();
                if (e.dataTransfer)
                    e.dataTransfer.dropEffect = "move";
            }
        }
    }
    /**
     * Handles drop.
     * @protected
     * @param {DragEvent} e The drag event.
     * @returns {void}
     */
    DragDrop(e) {
        var Column;
        if (this.Grid.AllowUserToOrderColumns) {
            Column = this.Grid.DraggedColumn;
            if (Column && Column !== this) {
                tp.RemoveClass(this.Handle, tp.Classes.DropTarget);
                if (e.preventDefault)
                    e.preventDefault();
                if (this.IsGroupColumn)
                    this.Grid.ColumnGrouped(Column, this);
                else
                    this.Grid.ColumnReordered(Column, this);
            }
        }
    }
    /**
     * Handles column resizing.
     * @protected
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    ResizeHandler(e) {
        var Self = this;
        var Doc = this.Grid.Document;
        var Style = Doc.defaultView.getComputedStyle(this.Handle);
        var StartX = e.clientX;
        var StartWidth = parseInt(Style.width, 10);
        var BodyCursor = Doc.body.style.cursor;
        var Resize = function (Event) {
            var Width = StartWidth + Event.clientX - StartX;
            if (Width >= 15 && Width <= 3000)
                Self.Width = Width;
        };
        var ResizeEnd = function (Event) {
            Self.fResizeTimeStamp = tp.Now().getTime();
            Doc.body.style.cursor = BodyCursor;
            Doc.documentElement.removeEventListener("mousemove", Resize, false);
            Doc.documentElement.removeEventListener("mouseup", ResizeEnd, false);
            Self.Grid.fResizing = false;
            Resize(Event);
            Self.Grid.Render();
        };
        this.Grid.fResizing = true;
        Doc.body.style.cursor = tp.Cursors.ResizeCol;
        Doc.documentElement.addEventListener("mousemove", Resize, false);
        Doc.documentElement.addEventListener("mouseup", ResizeEnd, false);
    }
    /**
     * Handles filter text changes.
     * @protected
     * @param {Event} e The event.
     * @returns {void}
     */
    FilterTextBox_TextChanged(e) {
        var Name = this.Name;
        var Text = this.fFilterCellTextBox.value;
        var FilterInfoList = this.Grid.DataSource.FilterInfoList;
        var FilterItem;
        var Flag = false;
        var Info;
        var Value;
        var Result;
        var CancelFilter = function () {
            if (FilterInfoList.Contains(Name)) {
                FilterInfoList.Remove(Name);
                Flag = true;
            }
        };
        var GetStartFilterOp = function (SourceText, DefaultOperator) {
            var OpInfo = {
                Text: SourceText,
                Operator: DefaultOperator || tp.FilterOp.None
            };
            if (!tp.IsBlank(SourceText)) {
                SourceText = tp.TrimStart(SourceText);
                if (tp.StartsWith(SourceText, "<>", false)) {
                    OpInfo.Operator = tp.FilterOp.NotEqual;
                    OpInfo.Text = SourceText.substring(2);
                } else if (tp.StartsWith(SourceText, ">=", false)) {
                    OpInfo.Operator = tp.FilterOp.GreaterOrEqual;
                    OpInfo.Text = SourceText.substring(2);
                } else if (tp.StartsWith(SourceText, "<=", false)) {
                    OpInfo.Operator = tp.FilterOp.LessOrEqual;
                    OpInfo.Text = SourceText.substring(2);
                } else if (tp.StartsWith(SourceText, ">", false)) {
                    OpInfo.Operator = tp.FilterOp.Greater;
                    OpInfo.Text = SourceText.substring(1);
                } else if (tp.StartsWith(SourceText, "=", false)) {
                    OpInfo.Operator = tp.FilterOp.Equal;
                    OpInfo.Text = SourceText.substring(1);
                } else if (tp.StartsWith(SourceText, "<", false)) {
                    OpInfo.Operator = tp.FilterOp.Less;
                    OpInfo.Text = SourceText.substring(1);
                } else if (SourceText.charAt(0) === "?") {
                    OpInfo.Operator = tp.FilterOp.Contains;
                    OpInfo.Text = SourceText.substring(1);
                } else if (SourceText.charAt(0) === "^") {
                    OpInfo.Operator = tp.FilterOp.StartsWith;
                    OpInfo.Text = SourceText.substring(1);
                } else if (SourceText.charAt(0) === "%") {
                    OpInfo.Operator = tp.FilterOp.EndsWith;
                    OpInfo.Text = SourceText.substring(1);
                }
            }
            return OpInfo;
        };
        if (tp.IsBlank(Text)) {
            CancelFilter();
            this.Grid.DoFilter();
            return;
        }
        switch (this.DataType) {
            case tp.DataType.String:
                Info = GetStartFilterOp(Text, tp.FilterOp.Contains);
                FilterItem = FilterInfoList.FindOrAdd(this.Name, Info.Operator, Info.Text);
                if (this.IsLookUp) {
                    FilterItem.LookUpTable = this.LookUpTable;
                    FilterItem.ListValueField = this.ListValueField;
                    FilterItem.ListDisplayField = this.ListDisplayField;
                }
                Flag = true;
                break;
            case tp.DataType.Integer:
                if (this.IsLookUp) {
                    Info = GetStartFilterOp(Text, tp.FilterOp.Contains);
                    FilterItem = FilterInfoList.FindOrAdd(this.Name, Info.Operator, Info.Text);
                    FilterItem.DataType = tp.DataType.String;
                    FilterItem.LookUpTable = this.LookUpTable;
                    FilterItem.ListValueField = this.ListValueField;
                    FilterItem.ListDisplayField = this.ListDisplayField;
                    Flag = true;
                } else {
                    Info = GetStartFilterOp(Text, tp.FilterOp.GreaterOrEqual);
                    Result = tp.TryStrToInt(Info.Text);
                    if (Result.Result) {
                        FilterInfoList.FindOrAdd(this.Name, Info.Operator, Result.Value);
                        Flag = true;
                    }
                }
                break;
            case tp.DataType.Double:
            case tp.DataType.Decimal:
            case tp.DataType.Decimal_:
                Info = GetStartFilterOp(Text, tp.FilterOp.GreaterOrEqual);
                Result = tp.TryStrToFloat(Info.Text);
                if (Result.Result) {
                    FilterInfoList.FindOrAdd(this.Name, Info.Operator, Result.Value);
                    Flag = true;
                }
                break;
            case tp.DataType.Boolean:
                Value = tp.IsSameText(Text, "true") || tp.IsSameText(Text, "1");
                if (Value) {
                    FilterInfoList.FindOrAdd(this.Name, tp.FilterOp.Equal, true);
                    Flag = true;
                } else {
                    Value = tp.IsSameText(Text, "false") || tp.IsSameText(Text, "0");
                    if (Value) {
                        FilterInfoList.FindOrAdd(this.Name, tp.FilterOp.Equal, false);
                        Flag = true;
                    }
                }
                break;
            case tp.DataType.Date:
            case tp.DataType.DateTime:
                Info = GetStartFilterOp(Text, tp.FilterOp.GreaterOrEqual);
                if (Info.Operator !== tp.FilterOp.None && Info.Text.length > 4) {
                    Result = tp.TryParseDateTime(Info.Text);
                    if (Result.Result) {
                        FilterInfoList.FindOrAdd(this.Name, Info.Operator, Result.Value);
                        Flag = true;
                    }
                }
                break;
        }
        if (Flag)
            this.Grid.DoFilter();
        else
            CancelFilter();
    }
    /**
     * Handles footer context menu.
     * @protected
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    FooterCell_ContextMenu(e) {
        this.Grid.AnyContextMenu(e);
    }

    // ● public
    /**
     * Binds this grid column to a data column.
     * @returns {void}
     */
    Bind() {
        this.fDataColumn = null;
        if (this.Grid.IsDataBound)
            this.fDataColumn = this.Grid.DataSource.Table.FindColumn(this.Name);
    }
    /**
     * Applies create parameters.
     * @param {object|null|undefined} Params The parameters.
     * @returns {void}
     */
    ApplyColumnParams(Params) {
        Params = Params || {};
        Object.keys(Params).forEach(Prop => {
            if (!tp.IsFunction(Params[Prop]))
                this[Prop] = Params[Prop];
        });
    }
    /**
     * Renders footer summary text.
     * @param {string} Text The summary text.
     * @returns {void}
     */
    RenderFooterSummary(Text) {
        this.fSummaryCell.textContent = Text;
        this.fSummaryCell.style.justifyContent = tp.Alignment.ToFlex(this.Alignment);
    }
    /**
     * Appends this column to group column panels.
     * @param {number} ColumnHeight The column height.
     * @param {HTMLElement} Groups The groups element.
     * @param {HTMLElement} Columns The columns element.
     * @param {HTMLElement} Filter The filter element.
     * @param {HTMLElement} Summary The summary element.
     * @returns {void}
     */
    AppendToGroupColumns(ColumnHeight, Groups, Columns, Filter, Summary) {
        this.Handle.style.height = tp.px(ColumnHeight);
        tp.AddClass(this.Handle, tp.Classes.Grouped);
        Groups.appendChild(this.Handle);
        Columns.appendChild(this.fGroupCell);
        Filter.appendChild(this.fFilterGroupCell);
        Summary.appendChild(this.fSummaryGroupCell);
    }
    /**
     * Appends this column to value column panels.
     * @param {HTMLElement} Columns The columns element.
     * @param {HTMLElement} Filter The filter element.
     * @param {HTMLElement} Summary The summary element.
     * @returns {void}
     */
    AppendToValueColumns(Columns, Filter, Summary) {
        this.Handle.style.height = "";
        tp.RemoveClass(this.Handle, tp.Classes.Grouped);
        if (this.Visible === true) {
            Columns.appendChild(this.Handle);
            Filter.appendChild(this.fFilterCell);
            Summary.appendChild(this.fSummaryCell);
            this.fFilterCellTextBox.disabled = !tp.DataType.IsSortableType(this.DataType);
        }
    }
    /**
     * Removes this column from the DOM.
     * @returns {void}
     */
    RemoveFromDom() {
        tp.Remove(this.fSummaryGroupCell);
        tp.Remove(this.fSummaryCell);
        tp.Remove(this.fFilterGroupCell);
        tp.Remove(this.fFilterCell);
        tp.Remove(this.fGroupCell);
        tp.Remove(this.Handle);
    }
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns the column name.
     */
    toString() {
        return this.Name;
    }
    /**
     * Handles DOM events.
     * @param {Event} e The event.
     * @returns {void}
     */
    handleEvent(e) {
        var Element = e.target;
        var EventName = tp.Events.ToTripous(e.type);
        var Column;
        var TimeStamp;
        switch (EventName) {
            case tp.Events.MouseDown:
                if (tp.IsHTMLElement(Element)) {
                    Column = tp.GridColumn.GetInfo(Element);
                    if (Column === this && tp.HasClass(Element, tp.Classes.Resizer) && this.Resizable) {
                        this.Grid.HideEditor(false);
                        this.ResizeHandler(e);
                        tp.CancelEvent(e);
                    }
                }
                break;
            case tp.Events.Click:
                tp.CancelEvent(e);
                if (this.fResizeTimeStamp) {
                    TimeStamp = tp.Now().getTime();
                    if (TimeStamp - this.fResizeTimeStamp < 1000) {
                        this.Grid.fResizing = false;
                        this.fResizeTimeStamp = null;
                        e.returnValue = false;
                        return;
                    }
                }
                if (tp.DataType.IsSortableType(this.DataType))
                    this.Sort();
                break;
        }
    }
    /**
     * Sorts this column.
     * @param {string|null|undefined} Mode The sort mode.
     * @returns {void}
     */
    Sort(Mode) {
        var SortMode = "";
        if (this.Visible && tp.DataType.IsSortableType(this.DataType)) {
            if (!tp.IsEmpty(Mode)) {
                if (tp.IsSameText("ASC", Mode))
                    SortMode = "asc";
                else if (tp.IsSameText("DESC", Mode))
                    SortMode = "desc";
            } else {
                SortMode = this.fSortMode;
                if (SortMode === "")
                    SortMode = "asc";
                else if (SortMode === "asc")
                    SortMode = "desc";
                else if (SortMode === "desc")
                    SortMode = "";
            }
            if (SortMode !== this.fSortMode) {
                this.fSortMode = SortMode;
                this.fSorter.innerHTML = tp.IsBlank(SortMode) ? "" : (SortMode === "asc" ? "&utrif;" : "&dtrif;");
                this.Grid.DoSort();
            }
        }
    }
    /**
     * Formats a value as text.
     * @param {*} Value The value to format.
     * @returns {string} Returns the formatted text.
     */
    Format(Value) {
        var DisplayValue;
        if (this.IsLookUp && !tp.IsEmpty(this.LookUpDisplayDataColumn)) {
            DisplayValue = this.GetLookUpDisplayValue(Value);
            return this.LookUpDisplayDataColumn.Format(DisplayValue, true);
        }
        if (this.DataColumn)
            return this.DataColumn.Format(Value, true);
        return tp.Db.Format(Value, this.DataType, this.DataColumn ? this.DataColumn.ColumnType : tp.DataColumnType.None, true, this.Decimals, this.LocalDate, this.DisplaySeconds);
    }
    /**
     * Parses text.
     * @param {string} Text The text to parse.
     * @returns {*} Returns the parsed value.
     */
    Parse(Text) {
        if (this.DataColumn)
            return this.DataColumn.Parse(Text);
        return tp.Db.Parse(Text, this.DataType);
    }
    /**
     * Renders this column value to a cell.
     * @param {HTMLElement} Cell The cell.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    Render(Cell, Row) {
        Cell.textContent = this.Format(this.GetValue(Row));
    }
    /**
     * Gets this column value from a row.
     * @param {tp.DataRow} Row The row.
     * @returns {*} Returns the value.
     */
    GetValue(Row) {
        return this.Grid.DataSource.GetValue(Row, this.DataIndex);
    }
    /**
     * Sets this column value to a row.
     * @param {tp.DataRow} Row The row.
     * @param {*} Value The value.
     * @returns {void}
     */
    SetValue(Row, Value) {
        this.Grid.DataSource.SetValue(Row, this.DataIndex, Value);
    }

    // ● properties
    /**
     * Returns the combo-box of the lookup inplace editor.
     * @protected
     * @returns {tp.ComboBox} Returns the combo-box.
     */
    get ComboBox() {
        if (!(this.fEditor instanceof tp.GridInplaceEditorComboBox)) {
            this.fEditorComboBox = new tp.GridInplaceEditorComboBox(this);
            this.fComboBox = this.fEditorComboBox.ComboBox;
            this.fEditor = this.fEditorComboBox;
        }
        return this.fComboBox;
    }
    /**
     * Gets the column handle.
     * @returns {HTMLElement} Returns the handle.
     */
    get Handle() {
        return this.fHandle;
    }
    /**
     * Gets the owner grid.
     * @returns {tp.Grid} Returns the owner grid.
     */
    get Grid() {
        return this.fGrid;
    }
    /**
     * Gets the bound data column.
     * @returns {tp.DataColumn|null} Returns the data column.
     */
    get DataColumn() {
        return this.fDataColumn;
    }
    /**
     * Returns true if this column is data-bound.
     * @returns {boolean} Returns true when data-bound.
     */
    get IsDataBound() {
        return !tp.IsEmpty(this.Grid) && this.Grid.IsDataBound && !tp.IsEmpty(this.DataColumn);
    }
    /**
     * Gets or sets the column name.
     * @returns {string} Returns the column name.
     */
    get Name() {
        return this.DataColumn instanceof tp.DataColumn ? this.DataColumn.Name : this.fName;
    }
    /**
     * Gets or sets the column name.
     * @param {*} Value The column name.
     * @returns {void}
     */
    set Name(Value) {
        Value = tp.IsNil(Value) ? "" : String(Value);
        if (Value !== this.fName) {
            this.fName = Value;
            this.Changed("Name");
        }
    }
    /**
     * Gets the data type.
     * @returns {number} Returns a tp.DataType value.
     */
    get DataType() {
        return this.DataColumn instanceof tp.DataColumn ? this.DataColumn.DataType : tp.DataType.None;
    }
    /**
     * Gets or sets decimal digits.
     * @returns {number} Returns decimals.
     */
    get Decimals() {
        return this.fDecimals;
    }
    /**
     * Gets or sets decimal digits.
     * @param {number} Value The decimals.
     * @returns {void}
     */
    set Decimals(Value) {
        if (Value !== this.fDecimals) {
            this.fDecimals = Value;
            this.Changed("Decimals");
        }
    }
    /**
     * Gets or sets alignment.
     * @returns {number} Returns a tp.Alignment value.
     */
    get Alignment() {
        if (this.fIsAlignmentSet !== true) {
            if (this.IsLookUp && this.LookUpDisplayDataColumn)
                return tp.DataType.DefaultAlignment(this.LookUpDisplayDataColumn.DataType);
            if (this.DataColumn instanceof tp.DataColumn) {
                if (this.DataColumn.ColumnType === tp.DataColumnType.Boolean || this.DataColumn.DataType === tp.DataType.Boolean)
                    return tp.Alignment.Center;
                return tp.DataType.DefaultAlignment(this.DataColumn.DataType);
            }
        }
        return this.fAlignment;
    }
    /**
     * Gets or sets alignment.
     * @param {number} Value A tp.Alignment value.
     * @returns {void}
     */
    set Alignment(Value) {
        if (Value !== this.fAlignment) {
            this.fAlignment = Value;
            this.fIsAlignmentSet = true;
            this.Changed("Alignment");
        }
    }
    /**
     * Gets or sets title text.
     * @returns {string} Returns title text.
     */
    get Text() {
        return tp.IsHTMLElement(this.fTextContainer) ? this.fTextContainer.textContent : "";
    }
    /**
     * Gets or sets title text.
     * @param {*} Value The title text.
     * @returns {void}
     */
    set Text(Value) {
        if (tp.IsHTMLElement(this.fTextContainer))
            this.fTextContainer.textContent = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets tooltip.
     * @returns {string} Returns tooltip.
     */
    get ToolTip() {
        return tp.IsHTMLElement(this.fTextContainer) ? this.fTextContainer.title : "";
    }
    /**
     * Gets or sets tooltip.
     * @param {*} Value The tooltip.
     * @returns {void}
     */
    set ToolTip(Value) {
        if (tp.IsHTMLElement(this.fTextContainer))
            this.fTextContainer.title = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets width.
     * @returns {number|string} Returns width.
     */
    get Width() {
        return this.fWidth;
    }
    /**
     * Gets or sets width.
     * @param {number|string} Value The width.
     * @returns {void}
     */
    set Width(Value) {
        this.SetWidth(Value);
        this.Changed("Width");
    }
    /**
     * Gets or sets whether dates are displayed as local dates.
     * @returns {boolean} Returns true when local date formatting is used.
     */
    get LocalDate() {
        return this.fLocalDate === true;
    }
    /**
     * Gets or sets whether dates are displayed as local dates.
     * @param {boolean} Value True to use local date formatting.
     * @returns {void}
     */
    set LocalDate(Value) {
        Value = Value === true;
        if (Value !== this.LocalDate) {
            this.fLocalDate = Value;
            this.Changed("LocalDate");
        }
    }
    /**
     * Gets or sets whether seconds are displayed.
     * @returns {boolean} Returns true when seconds are displayed.
     */
    get DisplaySeconds() {
        return this.fDisplaySeconds === true;
    }
    /**
     * Gets or sets whether seconds are displayed.
     * @param {boolean} Value True to display seconds.
     * @returns {void}
     */
    set DisplaySeconds(Value) {
        Value = Value === true;
        if (Value !== this.DisplaySeconds) {
            this.fDisplaySeconds = Value;
            this.Changed("DisplaySeconds");
        }
    }
    /**
     * Gets or sets whether this column is visible.
     * @returns {boolean} Returns true when visible.
     */
    get Visible() {
        if (tp.IsEmpty(this.fVisible))
            return this.DataColumn instanceof tp.DataColumn ? this.DataColumn.IsVisible : true;
        return this.fVisible;
    }
    /**
     * Gets or sets whether this column is visible.
     * @param {boolean} Value True when visible.
     * @returns {void}
     */
    set Visible(Value) {
        if (Value !== this.fVisible) {
            this.fVisible = Value;
            this.Changed("Visible");
        }
    }
    /**
     * Gets or sets whether this column is read-only.
     * @returns {boolean} Returns true when read-only.
     */
    get ReadOnly() {
        if (this.DataColumn instanceof tp.DataColumn && this.DataColumn.IsReadOnly === true)
            return true;
        return this.fReadOnly;
    }
    /**
     * Gets or sets whether this column is read-only.
     * @param {boolean} Value True when read-only.
     * @returns {void}
     */
    set ReadOnly(Value) {
        if (Value !== this.fReadOnly) {
            this.fReadOnly = Value;
            this.Changed("ReadOnly");
        }
    }
    /**
     * Gets or sets whether this column is resizable.
     * @returns {boolean} Returns true when resizable.
     */
    get Resizable() {
        return this.fResizable;
    }
    /**
     * Gets or sets whether this column is resizable.
     * @param {boolean} Value True when resizable.
     * @returns {void}
     */
    set Resizable(Value) {
        if (Value !== this.fResizable) {
            this.fResizable = Value;
            this.fResizer.style.cursor = this.fResizable ? tp.Cursors.ResizeCol : tp.Cursors.Default;
            this.Changed("Resizable");
        }
    }
    /**
     * Gets or sets whether this column is groupable.
     * @returns {boolean} Returns true when groupable.
     */
    get Groupable() {
        return this.fGroupable;
    }
    /**
     * Gets or sets whether this column is groupable.
     * @param {boolean} Value True when groupable.
     * @returns {void}
     */
    set Groupable(Value) {
        if (Value !== this.fGroupable) {
            this.fGroupable = Value;
            this.Changed("Groupable");
        }
    }
    /**
     * Gets data column index.
     * @returns {number} Returns data column index.
     */
    get DataIndex() {
        if (this.Grid.IsDataBound && this.DataColumn instanceof tp.DataColumn)
            return this.Grid.DataSource.Table.IndexOfColumn(this.DataColumn);
        return -1;
    }
    /**
     * Gets group column index.
     * @returns {number} Returns group column index.
     */
    get GroupIndex() {
        return this.Grid ? this.Grid.IndexOfGroupColumn(this) : -1;
    }
    /**
     * Gets value column index.
     * @returns {number} Returns value column index.
     */
    get ValueIndex() {
        return this.Grid ? this.Grid.IndexOfValueColumn(this) : -1;
    }
    /**
     * Gets aggregate column index.
     * @returns {number} Returns aggregate column index.
     */
    get AggregateIndex() {
        return this.Grid ? this.Grid.IndexOfAggregateColumn(this) : -1;
    }
    /**
     * Returns true if this is a group column.
     * @returns {boolean} Returns true when group column.
     */
    get IsGroupColumn() {
        return this.GroupIndex !== -1;
    }
    /**
     * Returns true if this is a value column.
     * @returns {boolean} Returns true when value column.
     */
    get IsValueColumn() {
        return this.ValueIndex !== -1;
    }
    /**
     * Returns true if this is an aggregate column.
     * @returns {boolean} Returns true when aggregate column.
     */
    get IsAggregateColumn() {
        return this.AggregateIndex !== -1;
    }
    /**
     * Gets or sets the inplace editor.
     * @returns {tp.GridInplaceEditor} Returns the editor.
     */
    get Editor() {
        if (tp.IsEmpty(this.fEditor))
            this.fEditor = this.CreateEditor();
        return this.fEditor;
    }
    /**
     * Gets or sets the inplace editor.
     * @param {tp.GridInplaceEditor} Value The editor.
     * @returns {void}
     */
    set Editor(Value) {
        if (Value !== this.fEditor) {
            this.fEditorComboBox = null;
            this.fComboBox = null;
            this.fLookUpTable = null;
            this.fLookUpDisplayDataColumn = null;
            this.fEditor = Value;
            if (this.fEditor instanceof tp.GridInplaceEditorComboBox) {
                this.fEditorComboBox = this.fEditor;
                this.fComboBox = this.fEditorComboBox.ComboBox;
            }
        }
    }
    /**
     * Gets sort mode.
     * @returns {string} Returns sort mode.
     */
    get SortMode() {
        return this.fSortMode;
    }
    /**
     * Returns true if this is a lookup column.
     * @returns {boolean} Returns true when lookup.
     */
    get IsLookUp() {
        return this.fEditor instanceof tp.GridInplaceEditorComboBox;
    }
    /**
     * Gets or sets lookup list source.
     * @returns {tp.DataSource|tp.DataTable|null} Returns list source.
     */
    get ListSource() {
        return this.ComboBox.ListSource;
    }
    /**
     * Gets or sets lookup list source.
     * @param {tp.DataSource|tp.DataTable|null} Value The list source.
     * @returns {void}
     */
    set ListSource(Value) {
        this.ComboBox.ListSource = Value;
    }
    /**
     * Gets or sets list source name.
     * @returns {string} Returns list source name.
     */
    get ListSourceName() {
        return this.ComboBox.ListSourceName;
    }
    /**
     * Gets or sets list source name.
     * @param {string} Value The list source name.
     * @returns {void}
     */
    set ListSourceName(Value) {
        this.ComboBox.ListSourceName = Value;
    }
    /**
     * Gets or sets list value field.
     * @returns {string} Returns list value field.
     */
    get ListValueField() {
        return this.ComboBox.ListValueField;
    }
    /**
     * Gets or sets list value field.
     * @param {string} Value The field name.
     * @returns {void}
     */
    set ListValueField(Value) {
        this.ComboBox.ListValueField = Value;
    }
    /**
     * Gets or sets list display field.
     * @returns {string} Returns list display field.
     */
    get ListDisplayField() {
        return this.ComboBox.ListDisplayField;
    }
    /**
     * Gets or sets list display field.
     * @param {string} Value The field name.
     * @returns {void}
     */
    set ListDisplayField(Value) {
        this.ComboBox.ListDisplayField = Value;
    }
    /**
     * Gets lookup data table.
     * @returns {tp.DataTable|null} Returns lookup table.
     */
    get LookUpTable() {
        if (this.IsLookUp && tp.IsEmpty(this.fLookUpTable) && !tp.IsEmpty(this.ComboBox.ListSource))
            this.fLookUpTable = this.ComboBox.ListSource.Table;
        return this.fLookUpTable;
    }
    /**
     * Gets lookup display data column.
     * @returns {tp.DataColumn|null} Returns lookup display column.
     */
    get LookUpDisplayDataColumn() {
        if (this.IsLookUp && tp.IsEmpty(this.fLookUpDisplayDataColumn) && !tp.IsEmpty(this.LookUpTable))
            this.fLookUpDisplayDataColumn = this.LookUpTable.FindColumn(this.ListDisplayField);
        return this.fLookUpDisplayDataColumn;
    }

    // ● static public
    /**
     * Returns the grid column associated with an element.
     * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
     * @returns {tp.GridColumn|null} Returns the grid column or null.
     */
    static GetInfo(ElementOrSelector) {
        var Element = tp.Select(ElementOrSelector);
        if (!tp.IsEmpty(Element) && Element.__Column instanceof tp.GridColumn)
            return Element.__Column;
        return null;
    }
    /**
     * Associates a grid column with an element.
     * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
     * @param {tp.GridColumn} Value The grid column.
     * @returns {void}
     */
    static SetInfo(ElementOrSelector, Value) {
        var Element = tp.Select(ElementOrSelector);
        if (!tp.IsEmpty(Element))
            Element.__Column = Value;
    }
    /**
     * Returns true if an element is associated with a grid column.
     * @param {HTMLElement|string|null|undefined} ElementOrSelector The element or selector.
     * @returns {boolean} Returns true when associated.
     */
    static HasInfo(ElementOrSelector) {
        return tp.GridColumn.GetInfo(ElementOrSelector) !== null;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.GridColumn.prototype.tpClass = "tp.GridColumn";
