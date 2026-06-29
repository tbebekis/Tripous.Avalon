// ● calendar box event args
/**
 * Denotes the type of date change that happened in a calendar because of a mouse click.
 * @type {object}
 */
tp.CalenderBoxClickChangeType = {
    Date: 1,
    Month: 2,
    Year: 4
};
Object.freeze(tp.CalenderBoxClickChangeType);
/**
 * Alias for the old misspelled tp.CalenderBoxClickChangeType object.
 * @type {object}
 */
tp.CalendarBoxClickChangeType = tp.CalenderBoxClickChangeType;
/**
 * Event arguments used by tp.CalendarBox ClickChange.
 */
tp.CalendarBoxClickChangeEventArgs = class extends tp.EventArgs {
    // ● constructor
    /**
     * Creates the event arguments.
     * @param {number} ChangeType One of the tp.CalendarBoxClickChangeType constants.
     */
    constructor(ChangeType) {
        super("ClickChange");
        this.ChangeType = ChangeType;
    }
};

// ● html date box
/**
 * A native HTML date input control.
 *
 * Example markup:
 * <pre>
 *     <input type="date" data-setup="{ Date: '2000-12-25' }" />
 * </pre>
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 * - ValueChanged
 */
tp.HtmlDateBox = class extends tp.InputControl {
    // ● private
    /**
     * Formats a Date value as native input date text.
     * @param {Date|string|null|undefined} Value The source value.
     * @returns {string} Returns yyyy-MM-dd text or empty string.
     */
    static ToNativeDateText(Value) {
        var DateValue = tp.ParseDateText(Value);
        return tp.IsValidDate(DateValue) ? tp.FormatDateTime(DateValue, tp.DateFormatISO) : "";
    }
    /**
     * Parses native input date text.
     * @param {string|null|undefined} Value The native date text.
     * @returns {Date|null} Returns a date or null.
     */
    static FromNativeDateText(Value) {
        return tp.IsBlank(Value) ? null : tp.ParseDateText(String(Value), "ISO");
    }

    // ● constructor
    /**
     * Creates a HTML date box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fElementSubType = "date";
        this.fDataValueProperty = "Date";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
    }
    /**
     * Applies explicit create params to this date box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Min))
            this.Min = Params.Min;
        if (!tp.IsNil(Params.Max))
            this.Max = Params.Max;
        if (!tp.IsNil(Params.Step))
            this.Step = Params.Step;
        if (!tp.IsNil(Params.Date))
            this.Date = Params.Date;
        if (!tp.IsNil(Params.Value))
            this.Value = Params.Value;
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = String(Params.Placeholder);
        if (!tp.IsNil(Params.TextAlign))
            this.TextAlign = Params.TextAlign;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        tp.AddClass(this.Handle, tp.Classes.HtmlDateBox);
        super.OnHandleCreated();
    }
    /**
     * Converts a data-source value to a date-box date.
     * @param {*} Value The data-source value.
     * @returns {Date|null} Returns the date value.
     */
    DataValueToDataProperty(Value) {
        return tp.ParseDateText(Value);
    }
    /**
     * Converts a date-box date to a data-source value.
     * @param {*} Value The date-box date.
     * @returns {Date|null} Returns the data-source value.
     */
    DataPropertyToDataValue(Value) {
        return tp.ParseDateText(Value);
    }

    // ● properties
    /**
     * Gets or sets the selected date.
     * @returns {Date|null} Returns the selected date.
     */
    get Date() {
        return this.Handle instanceof HTMLInputElement ? tp.HtmlDateBox.FromNativeDateText(this.Handle.value) : null;
    }
    /**
     * Gets or sets the selected date.
     * @param {Date|string|null|undefined} Value The date value.
     * @returns {void}
     */
    set Date(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.value = tp.HtmlDateBox.ToNativeDateText(Value);
    }
    /**
     * Gets or sets the native input value.
     * @returns {string} Returns native yyyy-MM-dd text.
     */
    get Value() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.value || "" : "";
    }
    /**
     * Gets or sets the native input value.
     * @param {string|Date|null|undefined} Value The native value or date.
     * @returns {void}
     */
    set Value(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.value = tp.HtmlDateBox.ToNativeDateText(Value);
    }
    /**
     * Gets or sets the minimum allowed date.
     * @returns {Date|null} Returns the minimum date.
     */
    get Min() {
        return this.Handle instanceof HTMLInputElement ? tp.HtmlDateBox.FromNativeDateText(this.Handle.min) : null;
    }
    /**
     * Gets or sets the minimum allowed date.
     * @param {Date|string|null|undefined} Value The minimum date.
     * @returns {void}
     */
    set Min(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.min = tp.HtmlDateBox.ToNativeDateText(Value);
    }
    /**
     * Gets or sets the maximum allowed date.
     * @returns {Date|null} Returns the maximum date.
     */
    get Max() {
        return this.Handle instanceof HTMLInputElement ? tp.HtmlDateBox.FromNativeDateText(this.Handle.max) : null;
    }
    /**
     * Gets or sets the maximum allowed date.
     * @param {Date|string|null|undefined} Value The maximum date.
     * @returns {void}
     */
    set Max(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.max = tp.HtmlDateBox.ToNativeDateText(Value);
    }
    /**
     * Gets or sets the native step value.
     * @returns {string} Returns the step value.
     */
    get Step() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.step || "1" : "1";
    }
    /**
     * Gets or sets the native step value.
     * @param {string|number|null|undefined} Value The step value.
     * @returns {void}
     */
    set Step(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.step = tp.IsNil(Value) ? "1" : String(Value);
    }
    /**
     * Gets or sets the placeholder text.
     * @returns {string} Returns the placeholder text.
     */
    get Placeholder() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.placeholder || "" : "";
    }
    /**
     * Gets or sets the placeholder text.
     * @param {string|null|undefined} Value The placeholder text.
     * @returns {void}
     */
    set Placeholder(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.placeholder = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the input text alignment.
     * @returns {string} Returns the CSS text-align value.
     */
    get TextAlign() {
        return this.Handle instanceof HTMLInputElement ? this.Handle.style.textAlign || "" : "";
    }
    /**
     * Gets or sets the input text alignment.
     * @param {string|null|undefined} Value The CSS text-align value.
     * @returns {void}
     */
    set TextAlign(Value) {
        if (this.Handle instanceof HTMLInputElement)
            this.Handle.style.textAlign = tp.IsNil(Value) ? "" : String(Value);
    }
};

tp.Ui.RegisterType(["HtmlDateBox", "tp-HtmlDateBox"], tp.HtmlDateBox);

// ● calendar box
/**
 * A calendar control.
 *
 * Example markup:
 * <pre>
 *     <table data-setup="{ Date: '2000-11-18' }"></table>
 * </pre>
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 * - DateChanged
 * - ClickChange
 */
tp.CalendarBox = class extends tp.Control {
    // ● private
    /**
     * Creates calendar-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateCalendarBoxParams(CreateParams) {
        var Args = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "table";
        return Args;
    }
    /**
     * Converts a value to a local date.
     * @param {Date|string|null|undefined} Value The value.
     * @returns {Date} Returns a valid date.
     */
    static ToDate(Value) {
        var Match;
        var Result;
        if (tp.IsValidDate(Value))
            return tp.ClearTime(tp.DateClone(Value));
        if (tp.IsString(Value)) {
            Match = /^(\d{4})-(\d{1,2})-(\d{1,2})/.exec(Value);
            if (Match) {
                Result = new Date(tp.ToInt(Match[1]), tp.ToInt(Match[2]) - 1, tp.ToInt(Match[3]));
                if (tp.IsValidDate(Result))
                    return tp.ClearTime(Result);
            }
            Result = new Date(Value);
            if (tp.IsValidDate(Result))
                return tp.ClearTime(Result);
        }
        return tp.Today();
    }

    // ● constructor
    /**
     * Creates a calendar box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.CalendarBox.CreateCalendarBoxParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Date";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDate = tp.Today();
        this.fYear = this.fDate.getFullYear();
        this.fMonth = this.fDate.getMonth();
        this.fWeekRows = [];
        this.fDayCells = [];
        this.fClickHandler = this.FuncBind(this.HandleClick);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateCalendarTable();
        this.Update();
    }
    /**
     * Applies explicit create params to this calendar box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Date))
            this.Date = Params.Date;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.CalendarBox);
        this.elTable = this.Handle;
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.Handle && this.fClickHandler)
            this.Handle.removeEventListener("click", this.fClickHandler, false);
        this.fClickHandler = null;
        this.elTable = null;
        this.fDateRow = null;
        this.fDaysRow = null;
        this.fWeekRows = null;
        this.fDayCells = null;
        this.fPrevYearCell = null;
        this.fPrevMonthCell = null;
        this.fCurDateCell = null;
        this.fNextMonthCell = null;
        this.fNextYearCell = null;
        super.DoDispose();
    }
    /**
     * Creates the calendar table rows and cells.
     * @protected
     * @returns {void}
     */
    CreateCalendarTable() {
        var HtmlText;
        var i;
        var j;
        this.Handle.setAttribute("border", "0");
        HtmlText =
            "<tbody>" +
            "<tr class=\"" + tp.Classes.CalendarBoxHeaderRow + "\"><th></th><th></th><th colspan=\"3\"></th><th></th><th></th></tr>" +
            "<tr class=\"" + tp.Classes.CalendarBoxDaysRow + "\"><th></th><th></th><th></th><th></th><th></th><th></th><th></th></tr>" +
            "<tr class=\"" + tp.Classes.CalendarBoxWeekRow + "\"><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
            "<tr class=\"" + tp.Classes.CalendarBoxWeekRow + "\"><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
            "<tr class=\"" + tp.Classes.CalendarBoxWeekRow + "\"><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
            "<tr class=\"" + tp.Classes.CalendarBoxWeekRow + "\"><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
            "<tr class=\"" + tp.Classes.CalendarBoxWeekRow + "\"><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
            "<tr class=\"" + tp.Classes.CalendarBoxWeekRow + "\"><td></td><td></td><td></td><td></td><td></td><td></td><td></td></tr>" +
            "</tbody>";
        this.Html = HtmlText;
        this.fDateRow = this.elTable.rows[0];
        this.fDaysRow = this.elTable.rows[1];
        this.fWeekRows.length = 0;
        this.fDayCells.length = 0;
        for (i = 2; i < 8; i++)
            this.fWeekRows.push(this.elTable.rows[i]);
        this.fPrevYearCell = this.fDateRow.cells[0];
        this.fPrevMonthCell = this.fDateRow.cells[1];
        this.fCurDateCell = this.fDateRow.cells[2];
        this.fNextMonthCell = this.fDateRow.cells[3];
        this.fNextYearCell = this.fDateRow.cells[4];
        this.fPrevYearCell.textContent = "<";
        this.fNextYearCell.textContent = ">";
        this.fPrevMonthCell.textContent = "<";
        this.fNextMonthCell.textContent = ">";
        tp.AddClass(this.fPrevYearCell, tp.Classes.CalendarBoxYearCell);
        tp.AddClass(this.fNextYearCell, tp.Classes.CalendarBoxYearCell);
        tp.AddClass(this.fPrevMonthCell, tp.Classes.CalendarBoxMonthCell);
        tp.AddClass(this.fNextMonthCell, tp.Classes.CalendarBoxMonthCell);
        this.RenderDayNames();
        for (i = 0; i < this.fWeekRows.length; i++) {
            for (j = 0; j < this.fWeekRows[i].cells.length; j++)
                this.fDayCells.push(this.fWeekRows[i].cells[j]);
        }
        this.Handle.addEventListener("click", this.fClickHandler, false);
    }
    /**
     * Renders localized day names, Monday first and Sunday last.
     * @protected
     * @returns {void}
     */
    RenderDayNames() {
        var Names = tp.Cultures.Current && tp.Cultures.Current.AbbreviatedDayNames.length > 0 ? tp.Cultures.Current.AbbreviatedDayNames : tp.DayNames;
        var i;
        this.fDaysRow.cells[6].textContent = Names[0] || "Sun";
        for (i = 1; i < 7; i++)
            this.fDaysRow.cells[i - 1].textContent = Names[i] || "";
    }
    /**
     * Handles calendar clicks.
     * @protected
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleClick(e) {
        var ChangeType = null;
        var Info;
        var Value;
        if (this.ReadOnly === true || this.Enabled !== true)
            return;
        if (this.fCurDateCell === e.target) {
            Value = tp.Today();
            this.fMonth = Value.getMonth();
            this.fYear = Value.getFullYear();
            this.Update();
            ChangeType = tp.CalendarBoxClickChangeType.Date;
        } else if (this.fPrevYearCell === e.target) {
            this.fYear--;
            this.Update();
            ChangeType = tp.CalendarBoxClickChangeType.Year;
        } else if (this.fNextYearCell === e.target) {
            this.fYear++;
            this.Update();
            ChangeType = tp.CalendarBoxClickChangeType.Year;
        } else if (this.fPrevMonthCell === e.target) {
            this.fMonth--;
            ChangeType = tp.CalendarBoxClickChangeType.Month;
            if (this.fMonth < 0) {
                this.fMonth = 11;
                this.fYear--;
                ChangeType = tp.CalendarBoxClickChangeType.Year;
            }
            this.Update();
        } else if (this.fNextMonthCell === e.target) {
            this.fMonth++;
            ChangeType = tp.CalendarBoxClickChangeType.Month;
            if (this.fMonth > 11) {
                this.fMonth = 0;
                this.fYear++;
                ChangeType = tp.CalendarBoxClickChangeType.Year;
            }
            this.Update();
        } else if (e.target instanceof HTMLTableCellElement && this.fDayCells.indexOf(e.target) !== -1 && tp.HasElementInfo(e.target)) {
            Info = tp.GetElementInfo(e.target);
            this.Date = Info.Date;
            ChangeType = tp.CalendarBoxClickChangeType.Date;
        }
        if (ChangeType !== null)
            this.OnClickChange(ChangeType);
    }
    /**
     * Binds the control to its data source.
     * @protected
     * @returns {void}
     */
    Bind() {
        super.Bind();
        this.ReadDataValue();
    }
    /**
     * Reads the bound data value.
     * @protected
     * @returns {void}
     */
    ReadDataValue() {
        var Value;
        if (this.ReadingDataValue === true || this.WritingDataValue === true)
            return;
        try {
            if (this.IsDataBound && this.DataSource.Position >= 0) {
                this.ReadingDataValue = true;
                try {
                    Value = this.DataSource.Get(this.DataField);
                    this[this.DataValueProperty] = Value;
                } finally {
                    this.ReadingDataValue = false;
                }
            }
        } finally {
        }
    }
    /**
     * Writes the date to the bound data source when allowed.
     * @protected
     * @returns {void}
     */
    DoPost() {
        if (this.IsDataBound)
            this.WriteDataValue();
    }
    /**
     * Returns the zero-based cell index for a date day.
     * @protected
     * @param {Date} Value The date.
     * @returns {number} Returns the cell index.
     */
    GetDayCellIndex(Value) {
        var Day = Value.getDay();
        return Day === 0 ? 6 : Day - 1;
    }

    // ● public
    /**
     * Updates the rendered calendar.
     * @returns {void}
     */
    Update() {
        var Today = tp.Today();
        var Month = this.fMonth;
        var Year = this.fYear;
        var FirstDate = new Date(Year, Month, 1);
        var LastDate = new Date(Year, Month, tp.DaysInMonth(Year, Month));
        var FirstDay = this.GetDayCellIndex(FirstDate);
        var CurrentDate = tp.AddDays(tp.DateClone(FirstDate), -FirstDay);
        var CellDate;
        var MonthNames = tp.Cultures.Current && tp.Cultures.Current.AbbreviatedMonthNames.length > 0 ? tp.Cultures.Current.AbbreviatedMonthNames : tp.MonthNames;
        var i;
        this.fCurDateCell.textContent = (MonthNames[Month] || "") + " " + Year;
        for (i = 0; i < this.fDayCells.length; i++) {
            CellDate = tp.ClearTime(tp.DateClone(CurrentDate));
            this.fDayCells[i].textContent = CellDate.getDate();
            this.fDayCells[i].className = tp.Classes.CalendarBoxDateCell;
            tp.SetElementInfo(this.fDayCells[i], { Date: CellDate });
            if (!tp.DateBetween(CellDate, FirstDate, LastDate))
                tp.AddClass(this.fDayCells[i], tp.Classes.Inactive);
            if (tp.DateCompare(CellDate, Today) === 0)
                tp.AddClass(this.fDayCells[i], tp.Classes.Marked);
            else if (tp.DateCompare(CellDate, this.Date) === 0)
                tp.AddClass(this.fDayCells[i], tp.Classes.Selected);
            tp.AddDays(CurrentDate, 1);
        }
    }

    // ● properties
    /**
     * Gets or sets the selected date.
     * @returns {Date} Returns the selected date.
     */
    get Date() {
        return tp.DateClone(this.fDate);
    }
    /**
     * Gets or sets the selected date.
     * @param {Date|string|null|undefined} Value The date value.
     * @returns {void}
     */
    set Date(Value) {
        var DateValue = tp.CalendarBox.ToDate(Value);
        if (tp.DateCompare(this.fDate, DateValue) !== 0) {
            this.fDate = DateValue;
            this.fMonth = this.fDate.getMonth();
            this.fYear = this.fDate.getFullYear();
            this.Update();
            this.DoPost();
            this.OnDateChanged();
        }
    }

    // ● event triggers
    /**
     * Triggers the DateChanged event.
     * @protected
     * @returns {void}
     */
    OnDateChanged() {
        this.Trigger("DateChanged", {});
    }
    /**
     * Triggers the ClickChange event.
     * @protected
     * @param {number} ChangeType One of the tp.CalendarBoxClickChangeType constants.
     * @returns {void}
     */
    OnClickChange(ChangeType) {
        this.Trigger("ClickChange", new tp.CalendarBoxClickChangeEventArgs(ChangeType));
    }
};

tp.Ui.RegisterType(["CalendarBox", "tp-CalendarBox"], tp.CalendarBox);

// ● date box
/**
 * A text date box with a drop-down tp.CalendarBox selector.
 *
 * Example markup:
 * <pre>
 *     <div data-setup="{ Date: '2000-05-08' }"></div>
 * </pre>
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 * - DateChanged
 *
 * @implements {tp.IDropDownBoxListener}
 */
tp.DateBox = class extends tp.Control {
    // ● private
    /**
     * Creates date-box create params.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     * @returns {tp.CreateParams|object} Returns normalized create params.
     */
    static CreateDateBoxParams(CreateParams) {
        var Args = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Args.ElementOrSelector))
            Args.ElementOrSelector = "div";
        return Args;
    }
    // ● constructor
    /**
     * Creates a date box.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(tp.DateBox.CreateDateBoxParams(CreateParams));
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fDataBindMode = tp.ControlBindMode.Simple;
        this.fDataValueProperty = "Date";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDate = null;
        this.fTextBoxChangeHandler = this.FuncBind(this.HandleTextBoxChange);
        this.fTextBoxKeyDownHandler = this.FuncBind(this.HandleTextBoxKeyDown);
        this.fButtonClickHandler = this.FuncBind(this.HandleButtonClick);
        this.fDocumentClickHandler = this.FuncBind(this.HandleDocumentClick);
        this.fCalendarClickChangeHandler = this.FuncBind(this.HandleCalendarClickChange);
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.CreateInnerControls();
    }
    /**
     * Applies explicit create params to this date box.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Placeholder))
            this.Placeholder = Params.Placeholder;
        if (!tp.IsNil(Params.TextAlign))
            this.TextAlign = Params.TextAlign;
        if (!tp.IsNil(Params.Date))
            this.Date = Params.Date;
        if (!tp.IsNil(Params.Text))
            this.Text = Params.Text;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.DateBox);
    }
    /**
     * Creates inner controls.
     * @protected
     * @returns {void}
     */
    CreateInnerControls() {
        var ControlContainer;
        ControlContainer = this.Document.createElement("div");
        ControlContainer.className = tp.Classes.Strip;
        this.Handle.appendChild(ControlContainer);
        this.fControlContainer = ControlContainer;
        this.fTextBox = this.Document.createElement("input");
        this.fTextBox.type = "text";
        this.fTextBox.spellcheck = false;
        this.fTextBox.className = tp.Classes.Text;
        ControlContainer.appendChild(this.fTextBox);
        this.fButton = this.Document.createElement("div");
        this.fButton.className = tp.Classes.Btn;
        this.fButton.innerHTML = "&#9662;";
        ControlContainer.appendChild(this.fButton);
        this.fDropDownBox = new tp.DropDownBox(null, {
            Associate: ControlContainer,
            Owner: this,
            Width: 292,
            Parent: this.Handle
        });
        this.fDropDownBox.Dragger.Active = false;
        tp.AddClass(this.fDropDownBox.Handle, tp.Classes.DateBoxDropDown);
        this.fCalendar = new tp.CalendarBox();
        this.fDropDownBox.Handle.appendChild(this.fCalendar.Handle);
        this.fTextBox.addEventListener("change", this.fTextBoxChangeHandler, false);
        this.fTextBox.addEventListener("blur", this.fTextBoxChangeHandler, false);
        this.fTextBox.addEventListener("keydown", this.fTextBoxKeyDownHandler, false);
        this.fButton.addEventListener("click", this.fButtonClickHandler, false);
        this.Document.addEventListener("click", this.fDocumentClickHandler, false);
        this.fCalendar.On("ClickChange", this.fCalendarClickChangeHandler, this);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fTextBox) {
            this.fTextBox.removeEventListener("change", this.fTextBoxChangeHandler, false);
            this.fTextBox.removeEventListener("blur", this.fTextBoxChangeHandler, false);
            this.fTextBox.removeEventListener("keydown", this.fTextBoxKeyDownHandler, false);
        }
        if (this.fButton)
            this.fButton.removeEventListener("click", this.fButtonClickHandler, false);
        if (this.Document)
            this.Document.removeEventListener("click", this.fDocumentClickHandler, false);
        if (this.fCalendar)
            this.fCalendar.Off("ClickChange", this.fCalendarClickChangeHandler);
        if (this.fDropDownBox) {
            this.fDropDownBox.Dispose();
            this.fDropDownBox = null;
        }
        this.fTextBoxChangeHandler = null;
        this.fTextBoxKeyDownHandler = null;
        this.fButtonClickHandler = null;
        this.fDocumentClickHandler = null;
        this.fCalendarClickChangeHandler = null;
        this.fControlContainer = null;
        this.fTextBox = null;
        this.fButton = null;
        this.fCalendar = null;
        super.DoDispose();
    }
    /**
     * Binds the control to its data source.
     * @protected
     * @returns {void}
     */
    Bind() {
        super.Bind();
        this.ReadDataValue();
    }
    /**
     * Notification called after read-only changes.
     * @protected
     * @returns {void}
     */
    OnReadOnlyChanged() {
        if (this.fTextBox)
            this.fTextBox.readOnly = this.ReadOnly;
        super.OnReadOnlyChanged();
    }
    /**
     * Handles inner text-box change or focus loss.
     * @protected
     * @param {Event} e The event.
     * @returns {void}
     */
    HandleTextBoxChange(e) {
        var DateValue;
        tp.CancelEvent(e);
        DateValue = tp.ParseDateText(this.Text);
        if (DateValue)
            this.Date = DateValue;
        else
            this.Date = null;
    }
    /**
     * Handles inner text-box keyboard input.
     * @protected
     * @param {KeyboardEvent} e The event.
     * @returns {void}
     */
    HandleTextBoxKeyDown(e) {
        if (tp.IsKey(e, tp.Keys.Enter)) {
            tp.CancelEvent(e);
            this.HandleTextBoxChange(e);
            this.Close();
        } else if (tp.IsKey(e, tp.Keys.Escape)) {
            tp.CancelEvent(e);
            this.Close();
        }
    }
    /**
     * Handles drop-down button click.
     * @protected
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleButtonClick(e) {
        if (this.Enabled === true && this.ReadOnly !== true) {
            tp.CancelEvent(e);
            this.Toggle();
        }
    }
    /**
     * Handles document clicks for closing the drop-down.
     * @protected
     * @param {MouseEvent} e The event.
     * @returns {void}
     */
    HandleDocumentClick(e) {
        if (!this.IsOpen)
            return;
        if (tp.ContainsEventTarget(this.Handle, e.target) || tp.ContainsEventTarget(this.fDropDownBox.Handle, e.target))
            return;
        this.Close();
    }
    /**
     * Handles calendar click changes.
     * @protected
     * @param {tp.CalendarBoxClickChangeEventArgs} Args The event arguments.
     * @returns {void}
     */
    HandleCalendarClickChange(Args) {
        if (tp.Bf.In(tp.CalendarBoxClickChangeType.Date, Args.ChangeType)) {
            this.Date = this.fCalendar.Date;
            this.Close();
            this.fTextBox.focus();
        }
    }
    /**
     * Writes the date to the bound data source when allowed.
     * @protected
     * @returns {void}
     */
    DoPost() {
        if (this.IsDataBound)
            this.WriteDataValue();
    }
    /**
     * Applies visual text from Date.
     * @protected
     * @returns {void}
     */
    UpdateText() {
        this.fTextBox.value = tp.IsValidDate(this.fDate) ? tp.FormatDateTime(this.fDate, tp.GetDateFormat()) : "";
    }

    // ● public
    /**
     * Displays the drop-down box.
     * @returns {void}
     */
    Open() {
        if (this.ReadOnly !== true && this.Enabled === true && this.fDropDownBox)
            this.fDropDownBox.Open();
    }
    /**
     * Hides the drop-down box.
     * @returns {void}
     */
    Close() {
        if (this.fDropDownBox)
            this.fDropDownBox.Close();
    }
    /**
     * Displays or hides the drop-down box.
     * @returns {void}
     */
    Toggle() {
        if (this.ReadOnly !== true && this.Enabled === true) {
            if (this.IsOpen)
                this.Close();
            else
                this.Open();
        }
    }
    /**
     * Called by the drop-down box to inform its owner about a stage change.
     * @param {tp.DropDownBox} Sender The sender.
     * @param {number} Stage One of the tp.DropDownBoxStage constants.
     * @returns {void}
     */
    OnDropDownBoxEvent(Sender, Stage) {
        var Bounds;
        if (Stage === tp.DropDownBoxStage.Opened) {
            if (tp.IsValidDate(this.Date))
                this.fCalendar.Date = this.Date;
            Bounds = this.fCalendar.Handle.getBoundingClientRect();
            this.fDropDownBox.Height = Bounds.height + 18;
            this.fDropDownBox.Width = 292;
        }
    }
    /**
     * Returns true if this control is valid.
     * @returns {boolean} Returns true when valid.
     */
    CheckValidity() {
        return tp.IsValidatableElement(this.fTextBox) ? this.fTextBox.checkValidity() : true;
    }
    /**
     * Sets a custom validation message.
     * @param {string} MessageText The validation message.
     * @returns {void}
     */
    SetValidationMessage(MessageText) {
        if (tp.IsValidatableElement(this.fTextBox))
            this.fTextBox.setCustomValidity(MessageText);
    }

    // ● properties
    /**
     * Gets or sets the text of the control.
     * @returns {string} Returns the text.
     */
    get Text() {
        return this.fTextBox instanceof HTMLInputElement ? this.fTextBox.value : "";
    }
    /**
     * Gets or sets the text of the control.
     * @param {string|null|undefined} Value The text.
     * @returns {void}
     */
    set Text(Value) {
        if (this.fTextBox)
            this.fTextBox.value = tp.IsNil(Value) ? "" : String(Value);
        this.Date = tp.ParseDateText(this.Text);
    }
    /**
     * Gets or sets the selected date.
     * @returns {Date|null} Returns the date.
     */
    get Date() {
        return tp.IsValidDate(this.fDate) ? tp.DateClone(this.fDate) : null;
    }
    /**
     * Gets or sets the selected date.
     * @param {Date|string|null|undefined} Value The date value.
     * @returns {void}
     */
    set Date(Value) {
        var DateValue = tp.ParseDateText(Value);
        if (tp.DateCompare(this.fDate, DateValue) !== 0) {
            this.fDate = DateValue;
            this.UpdateText();
            if (this.fCalendar && DateValue)
                this.fCalendar.Date = DateValue;
            this.DoPost();
            this.OnDateChanged();
        } else {
            this.UpdateText();
        }
    }
    /**
     * Gets or sets the text-box placeholder.
     * @returns {string} Returns the placeholder.
     */
    get Placeholder() {
        return this.fTextBox ? this.fTextBox.placeholder : "";
    }
    /**
     * Gets or sets the text-box placeholder.
     * @param {string|null|undefined} Value The placeholder.
     * @returns {void}
     */
    set Placeholder(Value) {
        if (this.fTextBox)
            this.fTextBox.placeholder = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the inner text-box alignment.
     * @returns {string} Returns the CSS text-align value.
     */
    get TextAlign() {
        return this.fTextBox instanceof HTMLInputElement ? this.fTextBox.style.textAlign || "" : "";
    }
    /**
     * Gets or sets the inner text-box alignment.
     * @param {string|null|undefined} Value The CSS text-align value.
     * @returns {void}
     */
    set TextAlign(Value) {
        if (this.fTextBox instanceof HTMLInputElement)
            this.fTextBox.style.textAlign = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Returns true while the drop-down box is visible.
     * @returns {boolean} Returns true while open.
     */
    get IsOpen() {
        return this.fDropDownBox ? this.fDropDownBox.IsOpen : false;
    }

    // ● event triggers
    /**
     * Notification called after Required changes.
     * @protected
     * @returns {void}
     */
    OnRequiredChanged() {
        this.SetRequiredMark(this.fTextBox);
        super.OnRequiredChanged();
    }
    /**
     * Triggers the DateChanged event.
     * @protected
     * @returns {void}
     */
    OnDateChanged() {
        this.Trigger("DateChanged", {});
    }
};

tp.Ui.RegisterType(["DateBox", "tp-DateBox"], tp.DateBox);
