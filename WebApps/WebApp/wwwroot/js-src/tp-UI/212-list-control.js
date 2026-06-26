// ● list control
/**
 * Base class for list controls such as tp.ListBox and tp.ComboBox.
 *
 * The control supports simple item arrays, object arrays, tp.DataTable, and tp.DataSource list sources.
 *
 * Events:
 * - DataSourceChanging
 * - DataSourceChanged
 * - DataFieldChanged
 * - ClearDataDisplay
 * - BindCompleted
 * - RequiredChanged
 * - ReadOnlyChanged
 * - SelectedIndexChanged
 */
tp.ListControl = class extends tp.Control {
    // ● constructor
    /**
     * Creates a list control.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        super(CreateParams);
    }

    // ● protected
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fDataBindMode = tp.ControlBindMode.List;
        this.fDataValueProperty = "SelectedValue";
        this.fSelectedIndex = -1;
        this.fSelectedValue = null;
        this.fSelectedItem = null;
        this.fItems = new tp.List();
        this.fListSource = null;
        this.fListSourceName = "";
        this.fListValueField = "";
        this.fListDisplayField = "";
        this.fItemHeight = null;
        this.fCanPostDataValue = true;
    }
    /**
     * Notification called after field initialization and before create params are applied.
     * @protected
     * @returns {void}
     */
    OnFieldsInitialized() {
        super.OnFieldsInitialized();
        this.fItems.On("Changing", this.ListChanging, this);
        this.fItems.On("Changed", this.ListChanged, this);
        this.fItems.EventsEnabled = true;
        this.fListSourceListener = this.CreateListSourceListener();
    }
    /**
     * Applies explicit create params to this list control.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.ListValueField))
            this.ListValueField = Params.ListValueField;
        if (!tp.IsNil(Params.ListDisplayField))
            this.ListDisplayField = Params.ListDisplayField;
        if (!tp.IsNil(Params.ListSourceName))
            this.ListSourceName = Params.ListSourceName;
        if (!tp.IsNil(Params.ItemHeight))
            this.ItemHeight = Params.ItemHeight;
        if (!tp.IsNil(Params.List))
            this.fItems.AddRange(Params.List);
        if (!tp.IsNil(Params.ListItems))
            this.fItems.AddRange(Params.ListItems);
        if (!tp.IsNil(Params.Items))
            this.fItems.AddRange(Params.Items);
        if (!tp.IsNil(Params.ListSource))
            this.ListSource = Params.ListSource;
        if (!tp.IsNil(Params.SelectedValue))
            this.SelectedValue = Params.SelectedValue;
        else if (!tp.IsNil(Params.SelectedItem))
            this.SelectedItem = Params.SelectedItem;
        else if (!tp.IsNil(Params.SelectedIndex))
            this.SelectedIndex = Params.SelectedIndex;
        else if (!this.IsDataBound && this.Items.length > 0 && this.SelectedIndex < 0)
            this.SelectedIndex = 0;
        this.SetScrollerList();
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        if (this.fListSource instanceof tp.DataSource && this.fListSourceListener)
            this.fListSource.RemoveDataListener(this.fListSourceListener);
        if (this.fItems) {
            this.fItems.Off("Changing", this.ListChanging);
            this.fItems.Off("Changed", this.ListChanged);
        }
        if (this.fScroller && tp.IsFunction(this.fScroller.Dispose))
            this.fScroller.Dispose();
        this.fListSourceListener = null;
        this.fListSource = null;
        this.fScroller = null;
        this.fContainer = null;
        super.DoDispose();
    }
    /**
     * Creates a listener for ListSource notifications.
     * @protected
     * @returns {tp.DataSourceListener} Returns the listener.
     */
    CreateListSourceListener() {
        var Listener = new tp.DataSourceListener(this);
        var Self = this;
        Listener.DataSourceRowCreated = function (Table, Row) { Self.ListSourceRowCreated(Table, Row); };
        Listener.DataSourceRowAdded = function (Table, Row) { Self.ListSourceRowAdded(Table, Row); };
        Listener.DataSourceRowModified = function (Table, Row, Column, OldValue, NewValue) { Self.ListSourceRowModified(Table, Row, Column, OldValue, NewValue); };
        Listener.DataSourceRowRemoved = function (Table, Row) { Self.ListSourceRowRemoved(Table, Row); };
        Listener.DataSourcePositionChanged = function (Table, Row, Position) { Self.ListSourcePositionChanged(Table, Row, Position); };
        Listener.DataSourceSorted = function () { Self.ListSourceSorted(); };
        Listener.DataSourceFiltered = function () { Self.ListSourceFiltered(); };
        Listener.DataSourceUpdated = function () { Self.ListSourceUpdated(); };
        return Listener;
    }
    /**
     * Sets the visual text of the concrete control.
     * @protected
     * @param {string} Text The text.
     * @returns {void}
     */
    DoSetText(Text) {
    }
    /**
     * Updates the virtual scroller.
     * @protected
     * @returns {void}
     */
    UpdateScroller() {
        if (this.fScroller)
            this.fScroller.Update();
    }
    /**
     * Applies the current item list to the virtual scroller.
     * @protected
     * @returns {void}
     */
    SetScrollerList() {
        if (this.fScroller) {
            this.fScroller.RowHeight = this.ItemHeight;
            this.fScroller.SetRowList(this.Items);
            this.fScroller.Update();
        }
    }
    /**
     * Returns true if the list contains a field or property.
     * @protected
     * @param {string} FieldName The field name.
     * @returns {boolean} Returns true when found.
     */
    ListContainsField(FieldName) {
        var Item;
        if (this.ListSource instanceof tp.DataSource && this.ListSource.Table instanceof tp.DataTable)
            return this.ListSource.Table.ContainsColumn(FieldName);
        if (this.Items.length > 0) {
            Item = this.Items[0];
            return !tp.IsEmpty(Item) && !tp.IsPrimitive(Item) && FieldName in Item;
        }
        return false;
    }
    /**
     * Returns the list value field.
     * @protected
     * @returns {string} Returns the field name.
     */
    GetListValueField() {
        var Result = this.ListValueField;
        if (tp.IsBlank(Result) && this.ListContainsField("Id"))
            Result = "Id";
        return Result;
    }
    /**
     * Returns the list display field.
     * @protected
     * @returns {string} Returns the field name.
     */
    GetListDisplayField() {
        var Result = this.ListDisplayField;
        if (tp.IsBlank(Result) && this.ListContainsField("Name"))
            Result = "Name";
        return Result;
    }
    /**
     * Returns the display text of an item.
     * @protected
     * @param {*} Item The item.
     * @returns {string} Returns the item text.
     */
    GetItemText(Item) {
        var DisplayField;
        if (!tp.IsEmpty(Item)) {
            if (tp.IsPrimitive(Item))
                return Item.toString();
            DisplayField = this.GetListDisplayField();
            if (!tp.IsBlank(DisplayField)) {
                if (this.ListSource instanceof tp.DataSource && Item instanceof tp.DataRow)
                    return this.ListSource.GetValue(Item, DisplayField, "");
                return !tp.IsNil(Item[DisplayField]) ? String(Item[DisplayField]) : "";
            }
            if (tp.IsFunction(Item.ToString))
                return Item.ToString();
        }
        return "";
    }
    /**
     * Returns the value of an item.
     * @protected
     * @param {*} Item The item.
     * @returns {*} Returns the item value.
     */
    GetItemValue(Item) {
        var ValueField;
        if (!tp.IsEmpty(Item)) {
            if (tp.IsPrimitive(Item))
                return Item;
            ValueField = this.GetListValueField();
            if (!tp.IsBlank(ValueField)) {
                if (this.ListSource instanceof tp.DataSource && Item instanceof tp.DataRow)
                    return this.ListSource.GetValue(Item, ValueField, null);
                return Item[ValueField];
            }
        }
        return null;
    }
    /**
     * Returns true if all items are non-primitive objects.
     * @protected
     * @returns {boolean} Returns true when the list contains objects.
     */
    IsObjectItemList() {
        return tp.All(this.Items, function (Item) {
            return !tp.IsEmpty(Item) && !tp.IsPrimitive(Item);
        });
    }
    /**
     * Returns the index of an item by text.
     * @protected
     * @param {string} Text The text.
     * @returns {number} Returns the index or -1.
     */
    IndexOfText(Text) {
        var Index;
        var List;
        var ItemText;
        if (this.IsObjectItemList() && !tp.IsBlank(this.GetListDisplayField())) {
            List = this.Items;
            for (Index = 0; Index < List.length; Index++) {
                ItemText = this.GetItemText(List[Index]);
                if (tp.IsSameText(Text, ItemText))
                    return Index;
            }
            return -1;
        }
        return tp.ListIndexOfText(this.Items, Text);
    }
    /**
     * Called when SelectedIndex changes.
     * @protected
     * @returns {void}
     */
    DoSelectedIndexChanged() {
        var Item = this.Items[this.SelectedIndex];
        this.fSelectedItem = Item;
        this.DoSetText(this.GetItemText(Item));
        if (!tp.IsEmpty(Item))
            this.fSelectedValue = tp.IsPrimitive(Item) ? Item : this.GetItemValue(Item);
        else {
            this.fSelectedValue = null;
            this.DoSetText("");
        }
        this.DoPost();
        this.OnSelectedIndexChanged();
    }
    /**
     * Called when SelectedValue changes.
     * @protected
     * @returns {void}
     */
    DoSelectedValueChanged() {
        var Index;
        var Item;
        var Value;
        var Found = false;
        var IsObjectList;
        if (this.Items.length > 0) {
            IsObjectList = this.IsObjectItemList();
            if (IsObjectList) {
                for (Index = 0; Index < this.Items.length; Index++) {
                    Item = this.Items[Index];
                    Value = this.GetItemValue(Item);
                    if (Value === this.SelectedValue) {
                        this.fSelectedIndex = Index;
                        this.fSelectedItem = Item;
                        this.DoSetText(this.GetItemText(Item));
                        Found = true;
                        break;
                    }
                }
                if (!Found) {
                    this.fSelectedIndex = -1;
                    this.fSelectedItem = null;
                    this.DoSetText("");
                }
            } else {
                this.fSelectedIndex = this.Items.indexOf(this.SelectedValue);
                this.fSelectedItem = this.SelectedValue;
                this.DoSetText(tp.IsPrimitive(this.SelectedValue) ? this.SelectedValue.toString() : "");
            }
        } else {
            this.fSelectedIndex = -1;
            this.fSelectedItem = null;
            this.DoSetText("");
        }
        this.DoPost();
        this.OnSelectedIndexChanged();
    }
    /**
     * Called when SelectedItem changes.
     * @protected
     * @returns {void}
     */
    DoSelectedItemChanged() {
        var Item = this.SelectedItem;
        if (!tp.IsEmpty(Item)) {
            this.fSelectedValue = this.IsObjectItemList() ? this.GetItemValue(Item) : Item;
            this.fSelectedIndex = this.Items.indexOf(Item);
            this.DoSetText(this.GetItemText(Item));
        } else {
            this.fSelectedIndex = -1;
            this.fSelectedValue = null;
            this.DoSetText("");
        }
        this.DoPost();
        this.OnSelectedIndexChanged();
    }
    /**
     * Writes the selection to the bound data source when allowed.
     * @protected
     * @returns {void}
     */
    DoPost() {
        if (this.IsDataBound && this.fCanPostDataValue === true)
            this.WriteDataValue();
    }
    /**
     * Clears selected item, index, and value.
     * @protected
     * @param {boolean} PostFlag True to post the change to the data source.
     * @returns {void}
     */
    DoClearValue(PostFlag) {
        this.fSelectedIndex = -1;
        this.fSelectedValue = null;
        this.fSelectedItem = null;
        if (PostFlag === true)
            this.DoPost();
    }
    /**
     * Adds or removes visual selection indication.
     * @protected
     * @param {boolean} Flag True to set indication.
     * @returns {void}
     */
    SetSelectionIndication(Flag) {
        this.SetScrollerIndexIndication(this.SelectedIndex, Flag);
    }
    /**
     * Adds or removes visual indication for a specified item index.
     * @protected
     * @param {number} Index The item index.
     * @param {boolean} Flag True to set indication.
     * @returns {void}
     */
    SetScrollerIndexIndication(Index, Flag) {
        var Element;
        var List;
        var i;
        if (!this.fScroller)
            return;
        if (Flag === true) {
            Element = this.GetElementByIndex(Index);
            if (Element instanceof HTMLElement)
                tp.AddClass(Element, tp.Classes.Selected);
        } else {
            List = tp.SelectAll(this.fScroller.Container, "." + tp.Classes.Selected);
            for (i = 0; i < List.length; i++)
                tp.RemoveClass(List[i], tp.Classes.Selected);
        }
    }
    /**
     * Scrolls a row index into view when it is outside the virtual scroller viewport.
     * @protected
     * @param {number} Index The item index.
     * @returns {void}
     */
    ScrollIndexIntoView(Index) {
        var RowTop;
        var RowBottom;
        var ViewTop;
        var ViewBottom;
        var ViewHeight;
        if (!this.fScroller || !tp.InRange(this.Items, Index))
            return;
        ViewHeight = this.fScroller.Viewport.getBoundingClientRect().height;
        RowTop = Index * this.ItemHeight;
        RowBottom = RowTop + this.ItemHeight;
        ViewTop = this.fScroller.Viewport.scrollTop;
        ViewBottom = ViewTop + ViewHeight;
        if (RowTop < ViewTop)
            this.fScroller.Viewport.scrollTop = RowTop;
        else if (RowBottom > ViewBottom)
            this.fScroller.Viewport.scrollTop = RowBottom - ViewHeight;
        this.fScroller.Render();
        this.SetScrollerIndexIndication(Index, true);
    }
    /**
     * Scrolls the selected row into view when it is outside the virtual scroller viewport.
     * @protected
     * @returns {void}
     */
    ScrollSelectedIndexIntoView() {
        this.ScrollIndexIntoView(this.SelectedIndex);
    }
    /**
     * Calculates a new scroller index by a delta.
     * @protected
     * @param {number} Index The current index.
     * @param {number} Delta The index delta.
     * @returns {number} Returns the new index.
     */
    GetMovedScrollerIndex(Index, Delta) {
        if (!this.fScroller || this.Items.length === 0)
            return -1;
        Index = Index < 0 ? 0 : Index + Delta;
        return Math.max(0, Math.min(Index, this.Items.length - 1));
    }
    /**
     * Moves the selected item by a delta.
     * @protected
     * @param {number} Delta The selection delta.
     * @returns {void}
     */
    MoveSelectedIndex(Delta) {
        var Index;
        if (!this.fScroller || this.Items.length === 0)
            return;
        Index = this.GetMovedScrollerIndex(this.SelectedIndex, Delta);
        this.SelectedIndex = Index;
        this.ScrollSelectedIndexIntoView();
    }
    /**
     * Handles a keyboard request to accept the current scroller selection.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {boolean} Returns true when handled.
     */
    AcceptScrollerSelection(e) {
        return false;
    }
    /**
     * Handles virtual scroller keyboard navigation.
     * @protected
     * @param {KeyboardEvent} e The keyboard event.
     * @returns {boolean} Returns true when handled.
     */
    HandleScrollerKeyDown(e) {
        if (!(e instanceof KeyboardEvent) || this.Enabled !== true || this.ReadOnly === true)
            return false;
        if (tp.IsKey(e, tp.Keys.Up)) {
            tp.CancelEvent(e);
            this.MoveSelectedIndex(-1);
            return true;
        }
        if (tp.IsKey(e, tp.Keys.Down)) {
            tp.CancelEvent(e);
            this.MoveSelectedIndex(1);
            return true;
        }
        if (tp.IsKey(e, tp.Keys.Enter) || tp.IsKey(e, tp.Keys.Space)) {
            if (this.AcceptScrollerSelection(e) === true) {
                tp.CancelEvent(e);
                return true;
            }
        }
        return false;
    }
    /**
     * Notification from the item list before it changes.
     * @protected
     * @param {tp.ListEventArgs} Args The event arguments.
     * @returns {void}
     */
    ListChanging(Args) {
        if (this.ListSource instanceof tp.DataSource)
            tp.Throw("ListItems modification not allowed.");
    }
    /**
     * Notification from the item list after it changes.
     * @protected
     * @param {tp.ListEventArgs} Args The event arguments.
     * @returns {void}
     */
    ListChanged(Args) {
        switch (Args.Action) {
            case tp.ListChangeType.Insert:
                this.UpdateScroller();
                break;
            case tp.ListChangeType.Remove:
                if (Args.Index === this.SelectedIndex)
                    this.SelectedIndex = 0;
                this.UpdateScroller();
                break;
            case tp.ListChangeType.Clear:
                this.SelectedValue = null;
                this.SetScrollerList();
                break;
            case tp.ListChangeType.Assign:
                if (this.IsDataBound) {
                    this.fCanPostDataValue = false;
                    try {
                        this.DoSelectedValueChanged();
                    } finally {
                        this.fCanPostDataValue = true;
                    }
                } else {
                    this.SelectedValue = null;
                    if (this.Items.length > 0)
                        this.SelectedIndex = 0;
                }
                this.SetScrollerList();
                break;
            case tp.ListChangeType.Update:
            case tp.ListChangeType.AddRange:
                if (this.IsDataBound) {
                    this.fCanPostDataValue = false;
                    try {
                        this.DoSelectedValueChanged();
                    } finally {
                        this.fCanPostDataValue = true;
                    }
                }
                this.SetScrollerList();
                break;
        }
    }
    /**
     * Notification from ListSource when a row is created.
     * @protected
     * @param {tp.DataTable} Table The table.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    ListSourceRowCreated(Table, Row) {
    }
    /**
     * Notification from ListSource when a row is added.
     * @protected
     * @param {tp.DataTable} Table The table.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    ListSourceRowAdded(Table, Row) {
        this.UpdateScroller();
    }
    /**
     * Notification from ListSource when a row is modified.
     * @protected
     * @param {tp.DataTable} Table The table.
     * @param {tp.DataRow} Row The row.
     * @param {tp.DataColumn} Column The column.
     * @param {*} OldValue The old value.
     * @param {*} NewValue The new value.
     * @returns {void}
     */
    ListSourceRowModified(Table, Row, Column, OldValue, NewValue) {
        this.UpdateScroller();
    }
    /**
     * Notification from ListSource when a row is removed.
     * @protected
     * @param {tp.DataTable} Table The table.
     * @param {tp.DataRow} Row The row.
     * @returns {void}
     */
    ListSourceRowRemoved(Table, Row) {
        var Index = this.Items.indexOf(Row);
        this.UpdateScroller();
        if (Index === this.SelectedIndex)
            this.SelectedIndex = 0;
    }
    /**
     * Notification from ListSource when position changes.
     * @protected
     * @param {tp.DataTable} Table The table.
     * @param {tp.DataRow} Row The row.
     * @param {number} Position The position.
     * @returns {void}
     */
    ListSourcePositionChanged(Table, Row, Position) {
    }
    /**
     * Notification from ListSource after sorting.
     * @protected
     * @returns {void}
     */
    ListSourceSorted() {
        this.UpdateScroller();
    }
    /**
     * Notification from ListSource after filtering.
     * @protected
     * @returns {void}
     */
    ListSourceFiltered() {
        if (!tp.InRange(this.Items, this.SelectedIndex))
            this.SelectedIndex = 0;
        this.SetScrollerList();
    }
    /**
     * Notification from ListSource after update.
     * @protected
     * @returns {void}
     */
    ListSourceUpdated() {
        this.SetScrollerList();
    }
    /**
     * Notification from ListSource after binding.
     * @protected
     * @returns {void}
     */
    ListSourceBind() {
        this.SetScrollerList();
        this.DoSelectedValueChanged();
    }
    /**
     * Returns a rendered row element by item index.
     * @protected
     * @param {number} Index The item index.
     * @returns {HTMLElement|null} Returns the element or null.
     */
    GetElementByIndex(Index) {
        var Selector;
        var Element;
        if (this.fScroller && tp.InRange(this.Items, Index)) {
            Selector = tp.Format("div[data-index=\"{0}\"]", Index);
            Element = tp.Select(this.fScroller.Container, Selector);
            if (Element instanceof HTMLElement)
                return Element;
        }
        return null;
    }
    /**
     * Scrolls by a single row and moves focus when possible.
     * @protected
     * @param {boolean} Up True to scroll up.
     * @returns {void}
     */
    RowScroll(Up) {
        var Element;
        var Index;
        var PageHeight;
        var RowRect;
        var ViewRect;
        var Element2;
        if (!this.fScroller || this.Items.length === 0)
            return;
        Element = this.Document.activeElement;
        if (!(Element instanceof HTMLElement) || !tp.ContainsElement(this.fScroller.Container, Element))
            return;
        Index = tp.StrToInt(tp.Data(Element, "index"), -1);
        Index = Up === true ? Index - 1 : Index + 1;
        if (Index < 0)
            return;
        PageHeight = this.fScroller.Viewport.getBoundingClientRect().height;
        RowRect = Element.getBoundingClientRect();
        ViewRect = this.fScroller.Viewport.getBoundingClientRect();
        if (RowRect.top < ViewRect.top || RowRect.bottom > ViewRect.top + PageHeight)
            this.PageScroll(Up);
        Element2 = this.GetElementByIndex(Index);
        if (Element2 instanceof HTMLElement) {
            this.fScroller.Viewport.focus();
            Element.blur();
            Element2.focus();
        }
    }
    /**
     * Scrolls by a page.
     * @protected
     * @param {boolean} Up True to scroll up.
     * @returns {void}
     */
    PageScroll(Up) {
        var PageHeight;
        var ScrollTop;
        if (!this.fScroller || this.Items.length === 0)
            return;
        PageHeight = this.fScroller.Viewport.getBoundingClientRect().height;
        ScrollTop = Up === true ? this.fScroller.Viewport.scrollTop - PageHeight : this.fScroller.Viewport.scrollTop + PageHeight;
        this.fScroller.Viewport.scrollTop = tp.Truncate(ScrollTop);
        this.fScroller.Container.focus();
    }
    /**
     * Scrolls to start or end.
     * @protected
     * @param {boolean} Start True to scroll to start.
     * @returns {void}
     */
    ControlScroll(Start) {
        var PageHeight;
        var ScrollTop;
        if (!this.fScroller || this.Items.length === 0)
            return;
        PageHeight = this.fScroller.Viewport.getBoundingClientRect().height;
        ScrollTop = Start === true ? 1 : this.ItemHeight * this.Items.length - PageHeight;
        this.fScroller.Viewport.scrollTop = tp.Truncate(ScrollTop);
        this.fScroller.Container.focus();
    }
    /**
     * Renders a virtual scroller row.
     * @protected
     * @param {*} Row The row item.
     * @param {number} RowIndex The row index.
     * @returns {HTMLElement} Returns the row element.
     */
    ItemRenderFunc(Row, RowIndex) {
        var Result = this.Document.createElement("div");
        Result.className = tp.Classes.Item;
        Result.tabIndex = -1;
        tp.SetElementInfo(Result, {
            Item: Row,
            Index: RowIndex
        });
        tp.Data(Result, "index", RowIndex);
        Result.innerHTML = this.GetItemText(Row);
        return Result;
    }
    /**
     * Virtual scroller callback before and after rendering.
     * @protected
     * @param {number} Phase The render phase. 1 is before, 2 is after.
     * @returns {void}
     */
    ScrollFunc(Phase) {
        if (Phase === 1)
            this.SetSelectionIndication(false);
        else if (Phase === 2)
            this.SetSelectionIndication(true);
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
        this.fCanPostDataValue = false;
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
            this.fCanPostDataValue = true;
        }
    }

    // ● public
    /**
     * Clears the control.
     * @returns {void}
     */
    Clear() {
        this.fItems.Clear();
        this.DoSetText("");
    }
    /**
     * Appends items to the list.
     * @param {Array|null|undefined} Items The items to append.
     * @returns {void}
     */
    AddRange(Items) {
        this.fItems.AddRange(Items);
    }

    // ● properties
    /**
     * Gets or sets the selected index.
     * @returns {number} Returns the selected index.
     */
    get SelectedIndex() {
        return this.fSelectedIndex;
    }
    /**
     * Gets or sets the selected index.
     * @param {number} Value The selected index.
     * @returns {void}
     */
    set SelectedIndex(Value) {
        Value = tp.StrToInt(Value, -1);
        if (Value !== this.SelectedIndex && tp.InRange(this.Items, Value)) {
            this.fSelectedIndex = Value;
            this.DoSelectedIndexChanged();
        }
    }
    /**
     * Gets or sets the selected value.
     * @returns {*} Returns the selected value.
     */
    get SelectedValue() {
        return this.fSelectedValue;
    }
    /**
     * Gets or sets the selected value.
     * @param {*} Value The selected value.
     * @returns {void}
     */
    set SelectedValue(Value) {
        if (Value !== this.SelectedValue) {
            this.fSelectedValue = Value;
            this.DoSelectedValueChanged();
        }
    }
    /**
     * Gets or sets the selected item.
     * @returns {*} Returns the selected item.
     */
    get SelectedItem() {
        return this.fSelectedItem;
    }
    /**
     * Gets or sets the selected item.
     * @param {*} Value The selected item.
     * @returns {void}
     */
    set SelectedItem(Value) {
        if (Value !== this.SelectedItem && this.Items.indexOf(Value) !== -1) {
            this.fSelectedItem = Value;
            this.DoSelectedItemChanged();
        }
    }
    /**
     * Gets or sets the non data-bound items.
     * @returns {Array} Returns the items.
     */
    get Items() {
        return this.fListSource instanceof tp.DataSource ? this.fListSource.Rows : this.fItems;
    }
    /**
     * Gets or sets the non data-bound items.
     * @param {Array|null|undefined} Value The items.
     * @returns {void}
     */
    set Items(Value) {
        this.Clear();
        if (tp.IsArray(Value))
            this.fItems.AddRange(Value);
        this.SelectedIndex = 0;
    }
    /**
     * Returns the item count.
     * @returns {number} Returns the item count.
     */
    get Count() {
        return this.Items.length;
    }
    /**
     * Gets or sets the list source.
     * @returns {tp.DataSource|null} Returns the list source.
     */
    get ListSource() {
        return this.fListSource;
    }
    /**
     * Gets or sets the list source.
     * @param {tp.DataSource|tp.DataTable|object[]|null|undefined} Value The list source.
     * @returns {void}
     */
    set ListSource(Value) {
        if (Value === this.fListSource)
            return;
        if (this.fListSource instanceof tp.DataSource && this.fListSourceListener)
            this.fListSource.RemoveDataListener(this.fListSourceListener);
        if (tp.IsArray(Value))
            Value = tp.DataTable.CreateFromList(Value);
        if (Value instanceof tp.DataTable)
            Value = new tp.DataSource(Value);
        this.fListSource = Value instanceof tp.DataSource ? Value : null;
        if (this.fListSource instanceof tp.DataSource) {
            this.fListSource.AddDataListener(this.fListSourceListener);
            this.ListSourceBind();
        } else {
            this.Clear();
        }
    }
    /**
     * Gets or sets the list source name used in declarative scenarios.
     * @returns {string} Returns the list source name.
     */
    get ListSourceName() {
        return this.fListSourceName;
    }
    /**
     * Gets or sets the list source name used in declarative scenarios.
     * @param {string} Value The list source name.
     * @returns {void}
     */
    set ListSourceName(Value) {
        this.fListSourceName = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the value field name.
     * @returns {string} Returns the value field name.
     */
    get ListValueField() {
        return !tp.IsBlank(this.fListValueField) ? this.fListValueField : this.ListDisplayField;
    }
    /**
     * Gets or sets the value field name.
     * @param {string} Value The value field name.
     * @returns {void}
     */
    set ListValueField(Value) {
        this.fListValueField = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the display field name.
     * @returns {string} Returns the display field name.
     */
    get ListDisplayField() {
        return this.fListDisplayField;
    }
    /**
     * Gets or sets the display field name.
     * @param {string} Value The display field name.
     * @returns {void}
     */
    set ListDisplayField(Value) {
        this.fListDisplayField = tp.IsNil(Value) ? "" : String(Value);
    }
    /**
     * Gets or sets the item height.
     * @returns {number} Returns the item height.
     */
    get ItemHeight() {
        if (tp.IsEmpty(this.fItemHeight) || this.fItemHeight <= 0)
            this.fItemHeight = tp.GetLineHeight(this.Handle);
        return this.fItemHeight;
    }
    /**
     * Gets or sets the item height.
     * @param {number|string} Value The item height.
     * @returns {void}
     */
    set ItemHeight(Value) {
        this.fItemHeight = tp.StrToInt(Value, 0);
    }

    // ● event triggers
    /**
     * Triggers the SelectedIndexChanged event.
     * @protected
     * @returns {void}
     */
    OnSelectedIndexChanged() {
        this.SetSelectionIndication(false);
        this.SetSelectionIndication(true);
        this.Trigger("SelectedIndexChanged", {});
    }
};
