// ● web item page builder
/**
 * Builds the first generated WebDesk item page surface from a data module item table.
 *
 * This builder handles scalar top-table fields and generated detail grid branches.
 * Custom editors, one-to-one detail layouts and save workflow are added by later WebDesk steps.
 */
tp.WebItemPageBuilder = class {
    // ● constructor
    /**
     * Creates a WebDesk item page builder.
     * @param {tp.WebDataForm} Form The owner data form.
     */
    constructor(Form) {
        /**
         * The owner data form.
         * @type {tp.WebDataForm|null}
         */
        this.Form = Form instanceof tp.WebDataForm ? Form : null;
        /**
         * The item table data source.
         * @type {tp.DataSource|null}
         */
        this.DataSource = null;
        /**
         * Created detail data sources.
         * @type {tp.DataSource[]}
         */
        this.DetailSources = [];
        /**
         * Created data sources keyed by table name.
         * @type {object}
         */
        this.SourceByTable = {};
        /**
         * Created lookup list data sources keyed by lookup source name.
         * @type {object}
         */
        this.ListSourceByName = {};
        /**
         * Created detail grids.
         * @type {tp.Grid[]}
         */
        this.DetailGrids = [];
        /**
         * The root tab control.
         * @type {tp.TabControl|null}
         */
        this.RootTabControl = null;
        /**
         * The detail tab control.
         * @type {tp.TabControl|null}
         */
        this.DetailTabControl = null;
        /**
         * Created field controls.
         * @type {tp.Control[]}
         */
        this.Controls = [];
        /**
         * The visual column count used for field groups.
         * @type {number}
         */
        this.ColumnCount = tp.WebItemPageBuilder.GetDefaultColumnCount();
        /**
         * Maximum controls placed in a visual column before continuing to the next column.
         * @type {number}
         */
        this.MaxControlsPerColumn = 8;
        /**
         * The visual field column width in pixels.
         * @type {number}
         */
        this.ColumnWidth = tp.WebItemPageBuilder.GetDefaultColumnWidth(this.ColumnCount);
    }

    // ● static public
    /**
     * Normalizes a visual column count.
     * @param {number} ColumnCount The requested column count.
     * @returns {number} Returns a value between 1 and 3.
     */
    static NormalizeColumnCount(ColumnCount) {
        ColumnCount = tp.ToInt(ColumnCount);
        if (ColumnCount < 1)
            return 1;
        if (ColumnCount > 3)
            return 3;
        return ColumnCount;
    }
    /**
     * Returns the approximate available width for WebDesk item field columns.
     * @returns {number} Returns the available width in pixels.
     */
    static GetAvailableWidth() {
        var Width = window.innerWidth || document.documentElement.clientWidth || 1024;
        var SidebarWidth = 350;
        return Math.max(320, Width - SidebarWidth);
    }
    /**
     * Returns the default visual column count, following the desktop rule.
     * @returns {number} Returns the default column count.
     */
    static GetDefaultColumnCount() {
        return tp.WebItemPageBuilder.GetAvailableWidth() > 1100 ? 3 : 2;
    }
    /**
     * Returns the default visual column width, following the desktop rule.
     * @param {number} ColumnCount The visual column count.
     * @returns {number} Returns the column width in pixels.
     */
    static GetDefaultColumnWidth(ColumnCount) {
        ColumnCount = tp.WebItemPageBuilder.NormalizeColumnCount(ColumnCount);
        return Math.floor(tp.WebItemPageBuilder.GetAvailableWidth() / ColumnCount);
    }

    // ● protected
    /**
     * Clears previously generated content.
     * @returns {void}
     */
    Clear() {
        this.Controls = [];
        this.DetailSources = [];
        this.SourceByTable = {};
        this.ListSourceByName = {};
        this.DetailGrids = [];
        this.RootTabControl = null;
        this.DetailTabControl = null;
        this.DataSource = null;
        if (this.IsServerRenderedItemPage() !== true && this.Form && this.Form.ItemPage instanceof HTMLElement)
            tp.RemoveChildren(this.Form.ItemPage);
    }
    /**
     * Returns the server-rendered item page root.
     * @returns {HTMLElement|null} Returns the item page root or null.
     */
    GetServerItemPageRoot() {
        if (!this.Form || !(this.Form.ItemPage instanceof HTMLElement))
            return null;
        return this.Form.ItemPage.querySelector("[data-wdf-server-item-page='true']");
    }
    /**
     * Returns true when the item page is server-rendered.
     * @returns {boolean} Returns true for a server-rendered item page.
     */
    IsServerRenderedItemPage() {
        return this.GetServerItemPageRoot() instanceof HTMLElement;
    }
    /**
     * Returns true when a column should be rendered as a scalar item field.
     * @param {tp.DataColumn} Column The data column.
     * @param {tp.DataTable} Table The item table.
     * @returns {boolean} Returns true when the column should be rendered.
     */
    CanRenderColumn(Column, Table) {
        if (!(Column instanceof tp.DataColumn) || !(Table instanceof tp.DataTable))
            return false;
        if (!Column.IsBindable || Column.IsNoInsertOrUpdate || Column.IsExtraField)
            return false;
        if (tp.IsSameText(Column.Name, Table.KeyField))
            return false;
        if (!tp.IsBlank(Table.MasterTableName) && tp.IsSameText(Column.Name, Table.DetailField))
            return false;
        if (Column.IsForeignKeyField && !Column.IsLocator)
            return false;
        if (Column.IsBlob || Column.IsImage)
            return false;
        return true;
    }
    /**
     * Returns the display group name for a column.
     * @param {tp.DataColumn} Column The data column.
     * @returns {string} Returns the group name.
     */
    GetColumnGroup(Column) {
        return Column instanceof tp.DataColumn && !tp.IsBlank(Column.Group) ? Column.Group : "General";
    }
    /**
     * Splits group columns into visual columns.
     * @param {tp.DataColumn[]} Columns The group columns.
     * @returns {tp.DataColumn[][]} Returns visual columns.
     */
    SplitColumns(Columns) {
        var VisualColumnCount = tp.WebItemPageBuilder.NormalizeColumnCount(this.ColumnCount);
        var MaxControlsPerColumn = tp.ToInt(this.MaxControlsPerColumn);
        var Result = [];
        var Index;
        var ColumnIndex;
        if (MaxControlsPerColumn < 1)
            MaxControlsPerColumn = 8;
        for (Index = 0; Index < VisualColumnCount; Index++)
            Result.push([]);
        for (Index = 0; Index < Columns.length; Index++) {
            ColumnIndex = Math.floor(Index / MaxControlsPerColumn);
            if (ColumnIndex >= VisualColumnCount)
                ColumnIndex = VisualColumnCount - 1;
            Result[ColumnIndex].push(Columns[Index]);
        }
        return Result;
    }
    /**
     * Returns a control type name for a column.
     * @param {tp.DataColumn} Column The data column.
     * @returns {string} Returns a registered control type name.
     */
    GetControlTypeName(Column) {
        if (Column.IsLocator)
            return "LocatorBox";
        if (Column.IsLookup)
            return "ComboBox";
        if (Column.IsBoolean)
            return "CheckBox";
        if (Column.IsMemo || Column.IsLargeMemo)
            return "Memo";
        if (Column.ColumnType === tp.DataColumnType.Date)
            return "HtmlDateBox";
        if (Column.IsNumeric)
            return "NumberBox";
        return "TextBox";
    }
    /**
     * Creates create params for a field control.
     * @param {tp.DataColumn} Column The data column.
     * @returns {object} Returns control create params.
     */
    CreateControlParams(Column) {
        var Table = Column.Table;
        var Result = {
            TypeName: this.GetControlTypeName(Column),
            DataField: Column.Name,
            DataSource: this.DataSource,
            Required: Column.IsRequired,
            ReadOnly: this.IsReadOnlyColumn(Column),
            TableName: Table ? Table.Name : ""
        };
        if (Column.MaxLength > 0)
            Result.MaxLength = Column.MaxLength;
        if (Column.Decimals >= 0)
            Result.Decimals = Column.Decimals;
        if (Column.IsLocator) {
            Result.LocatorName = Column.Locator;
            Result.ModuleName = this.Form ? this.Form.ModuleName : "";
            Result.ReferenceField = Column.Name;
            Result.TargetRow = this.Form && this.Form.Module ? this.Form.Module.Row : null;
        }
        if (Column.IsLookup) {
            Result.ListOnly = true;
            Result.ListSourceName = Column.LookupSource;
            Result.ListValueField = "Id";
            Result.ListDisplayField = "Name";
        }
        return Result;
    }
    /**
     * Returns true when a column should be read-only.
     * @param {tp.DataColumn} Column The data column.
     * @returns {boolean} Returns true when read-only.
     */
    IsReadOnlyColumn(Column) {
        if (!this.Form || this.Form.IsReadOnly === true)
            return true;
        if (Column.IsReadOnly || Column.IsReadOnlyUI)
            return true;
        if (this.Form.FormState === tp.WebDataFormState.Edit && Column.IsReadOnlyEdit)
            return true;
        return false;
    }
    /**
     * Creates the accordion that hosts top field groups.
     * @param {HTMLElement} Parent The parent element.
     * @returns {tp.Accordion} Returns the accordion.
     */
    CreateAccordion(Parent) {
        var Element = Parent.ownerDocument.createElement("div");
        var Result = new tp.Accordion({
            ElementOrSelector: Element
        });
        Result.AllowMultiExpand = true;
        Parent.appendChild(Element);
        return Result;
    }
    /**
     * Creates a field group inside an accordion and returns its content element.
     * @param {tp.Accordion} Accordion The accordion.
     * @param {string} Title The group title.
     * @param {boolean} Expanded True to expand the group.
     * @returns {HTMLElement|null} Returns the content element.
     */
    CreateAccordionGroup(Accordion, Title, Expanded) {
        var Index = Accordion.Count;
        var Item = Accordion.AddItem(Title);
        var TitleElement;
        if (Item instanceof HTMLElement) {
            TitleElement = Accordion.TitleElementOf(Item);
            if (TitleElement instanceof HTMLElement)
                TitleElement.style.fontSize = "0.9em";
            Accordion.Expand(Expanded === true, Index);
            return Accordion.ContentElementOf(Item);
        }
        return null;
    }
    /**
     * Creates a root element that contains visual field columns.
     * @param {HTMLElement} Parent The parent element.
     * @returns {HTMLElement} Returns the created root element.
     */
    CreateColumnRoot(Parent) {
        var Result = Parent.ownerDocument.createElement("div");
        var ColumnWidth = Math.max(280, tp.ToInt(this.ColumnWidth));
        var MinColumnWidth = Math.min(360, ColumnWidth);
        Result.style.display = "grid";
        Result.style.boxSizing = "border-box";
        Result.style.width = "100%";
        Result.style.gridTemplateColumns = "repeat(auto-fit, minmax(min(100%, " + MinColumnWidth + "px), 1fr))";
        Result.style.columnGap = "16px";
        Result.style.rowGap = "6px";
        Result.style.alignItems = "start";
        Result.style.justifyContent = "stretch";
        Result.style.padding = "8px 6px 6px 6px";
        Parent.appendChild(Result);
        return Result;
    }
    /**
     * Creates a visual field column.
     * @param {HTMLElement} Parent The parent element.
     * @returns {HTMLElement} Returns the created column element.
     */
    CreateVisualColumn(Parent) {
        var Result = Parent.ownerDocument.createElement("div");
        Result.style.display = "grid";
        Result.style.gridAutoRows = "max-content";
        Result.style.rowGap = "6px";
        Result.style.minWidth = "0";
        Parent.appendChild(Result);
        return Result;
    }
    /**
     * Creates a field row.
     * @param {HTMLElement} Parent The parent element.
     * @param {tp.DataColumn} Column The data column.
     * @returns {tp.Component|null} Returns the created row.
     */
    CreateFieldRow(Parent, Column) {
        var Params = {
            Parent: Parent,
            Text: Column.DisplayTitle,
            Control: this.CreateControlParams(Column)
        };
        var Row = Column.IsBoolean ? new tp.CheckBoxRow(Params) : new tp.CtrlRow(Params);
        if (Row.Control instanceof tp.Control) {
            Row.Control.ToolTip = Column.DisplayToolTip;
            this.Controls.push(Row.Control);
        }
        return Row;
    }
    /**
     * Applies desktop-like row layout to generated field rows.
     * @returns {void}
     */
    ApplyFieldRowLayout() {
        this.Controls.forEach(function (Control) {
            var Row = tp.Ui.GetCtrlRow(Control.Handle);
            if (Row instanceof HTMLElement) {
                if (Control instanceof tp.CheckBox) {
                    Control.Handle.style.gridTemplateColumns = "auto auto minmax(0, 1fr)";
                } else {
                    Row.style.gridTemplateColumns = "28fr 70fr 2fr";
                }
            }
        });
    }
    /**
     * Builds scalar field groups for a table.
     * @param {HTMLElement} Parent The parent element.
     * @param {tp.DataTable} Table The data table.
     * @param {tp.DataSource} DataSource The data source.
     * @returns {void}
     */
    BuildFieldGroups(Parent, Table, DataSource) {
        var Groups = {};
        var GroupNames = [];
        var Index;
        var Column;
        var GroupName;
        var Accordion;
        var GroupContent;
        var VisualColumns;
        var ColumnRoot;
        var ColumnElement;
        var ColumnParent;
        var VisualColumnIndex;
        if (!(Parent instanceof HTMLElement) || !(Table instanceof tp.DataTable) || !(DataSource instanceof tp.DataSource))
            return;
        this.DataSource = DataSource;
        for (Index = 0; Index < Table.Columns.length; Index++) {
            Column = Table.Columns[Index];
            if (!this.CanRenderColumn(Column, Table))
                continue;
            GroupName = this.GetColumnGroup(Column);
            if (!Groups[GroupName]) {
                Groups[GroupName] = [];
                GroupNames.push(GroupName);
            }
            Groups[GroupName].push(Column);
        }
        Accordion = this.CreateAccordion(Parent);
        for (Index = 0; Index < GroupNames.length; Index++) {
            GroupName = GroupNames[Index];
            GroupContent = this.CreateAccordionGroup(Accordion, GroupName, tp.IsSameText(GroupName, "General"));
            if (!(GroupContent instanceof HTMLElement))
                continue;
            ColumnRoot = this.CreateColumnRoot(GroupContent);
            VisualColumns = this.SplitColumns(Groups[GroupName]);
            for (VisualColumnIndex = 0; VisualColumnIndex < VisualColumns.length; VisualColumnIndex++) {
                ColumnElement = this.CreateVisualColumn(ColumnRoot);
                ColumnParent = ColumnElement;
                VisualColumns[VisualColumnIndex].forEach((Item) => this.CreateFieldRow(ColumnParent, Item));
            }
        }
        this.ApplyFieldRowLayout();
    }
    /**
     * Returns the first-level detail tables of a parent table.
     * @param {tp.DataTable} ParentTable The parent table.
     * @returns {tp.DataTable[]} Returns the detail tables.
     */
    GetChildDetailTables(ParentTable) {
        var DataSet;
        var Result = [];
        var Index;
        var Table;
        var Name;
        if (!(ParentTable instanceof tp.DataTable) || !(ParentTable.DataSet instanceof tp.DataSet))
            return Result;
        DataSet = ParentTable.DataSet;
        if (tp.IsArray(ParentTable.Details) && ParentTable.Details.length > 0) {
            for (Index = 0; Index < ParentTable.Details.length; Index++) {
                Name = ParentTable.Details[Index];
                Table = DataSet.FindTable(Name);
                if (Table instanceof tp.DataTable && tp.IsSameText(Table.MasterTableName, ParentTable.Name))
                    Result.push(Table);
            }
        } else {
            for (Index = 0; Index < DataSet.Tables.length; Index++) {
                Table = DataSet.Tables[Index];
                if (Table instanceof tp.DataTable && tp.IsSameText(Table.MasterTableName, ParentTable.Name))
                    Result.push(Table);
            }
        }
        return Result;
    }
    /**
     * Returns a title for a detail table tab.
     * @param {tp.DataTable} Table The detail table.
     * @returns {string} Returns the tab title.
     */
    GetTableTitle(Table) {
        var Source;
        if (!(Table instanceof tp.DataTable))
            return "";
        Source = Table.DisplayTitle || Table.Title || Table.Caption || Table.Name;
        return tp.IsBlank(Source) ? Table.Name : tp.SplitOnUpperCase(String(Source));
    }
    /**
     * Creates the root tab control for the generated item page.
     * @param {HTMLElement} Parent The parent element.
     * @returns {tp.TabControl} Returns the tab control.
     */
    CreateRootTabControl(Parent) {
        var Element = Parent.ownerDocument.createElement("div");
        var Result = new tp.TabControl({
            ElementOrSelector: Element
        });
        Element.style.boxSizing = "border-box";
        Element.style.width = "100%";
        Element.style.height = "100%";
        Element.style.minHeight = "0";
        Parent.appendChild(Element);
        return Result;
    }
    /**
     * Creates a tab control.
     * @param {HTMLElement} Parent The parent element.
     * @returns {tp.TabControl} Returns the tab control.
     */
    CreateTabControl(Parent) {
        var Element = Parent.ownerDocument.createElement("div");
        var Result = new tp.TabControl({
            ElementOrSelector: Element
        });
        Element.style.boxSizing = "border-box";
        Element.style.width = "100%";
        Element.style.height = "min(64vh, 720px)";
        Element.style.minHeight = "360px";
        Element.style.marginTop = "8px";
        Parent.appendChild(Element);
        return Result;
    }
    /**
     * Creates a detail grid page.
     * @param {HTMLElement} Parent The parent element.
     * @param {tp.DataTable} DetailTable The detail table.
     * @param {tp.DataSource} MasterSource The master data source.
     * @returns {tp.Grid|null} Returns the detail grid.
     */
    CreateDetailGrid(Parent, DetailTable, MasterSource) {
        var Element;
        var Source;
        var Grid;
        if (!(Parent instanceof HTMLElement) || !(DetailTable instanceof tp.DataTable) || !(MasterSource instanceof tp.DataSource))
            return null;
        Element = Parent.ownerDocument.createElement("div");
        Element.style.height = "100%";
        Element.style.minHeight = "260px";
        Element.style.overflow = "hidden";
        Parent.appendChild(Element);
        Source = new tp.DataSource(DetailTable);
        Source.MasterKeyField = DetailTable.MasterField;
        Source.DetailKeyField = DetailTable.DetailField;
        Source.MasterSource = MasterSource;
        Grid = new tp.Grid({
            ElementOrSelector: Element,
            DataSource: Source,
            AutoGenerateColumns: false,
            ToolBarVisible: false,
            GroupsVisible: false,
            FilterVisible: false,
            FooterVisible: false
        });
        this.CreateDetailGridColumns(Grid);
        this.DetailSources.push(Source);
        this.DetailGrids.push(Grid);
        return Grid;
    }
    /**
     * Returns or creates a data source for a table name.
     * @param {string} TableName The table name.
     * @returns {tp.DataSource|null} Returns the data source or null.
     */
    GetServerDataSource(TableName) {
        var Table;
        var MasterSource;
        var Source;
        if (!(this.Form && this.Form.Module instanceof tp.DataModule) || tp.IsBlank(TableName))
            return null;
        if (this.SourceByTable[TableName] instanceof tp.DataSource)
            return this.SourceByTable[TableName];
        Table = this.Form.Module.FindTable(TableName);
        if (!(Table instanceof tp.DataTable))
            return null;
        if (Table === this.Form.Module.tblItem) {
            Source = this.DataSource;
        } else {
            MasterSource = this.GetServerDataSource(Table.MasterTableName);
            if (!(MasterSource instanceof tp.DataSource))
                return null;
            Source = new tp.DataSource(Table);
            Source.MasterKeyField = Table.MasterField;
            Source.DetailKeyField = Table.DetailField;
            Source.MasterSource = MasterSource;
            this.DetailSources.push(Source);
        }
        this.SourceByTable[TableName] = Source;
        return Source;
    }
    /**
     * Returns or creates a lookup list data source by name.
     * @param {string} SourceName The lookup source name.
     * @returns {tp.DataSource|null} Returns the lookup source or null.
     */
    GetServerListSource(SourceName) {
        var Table;
        var Source;
        if (!(this.Form && this.Form.Module instanceof tp.DataModule) || tp.IsBlank(SourceName))
            return null;
        if (this.ListSourceByName[SourceName] instanceof tp.DataSource)
            return this.ListSourceByName[SourceName];
        Table = this.Form.Module.FindTable(SourceName);
        if (!(Table instanceof tp.DataTable))
            return null;
        Source = new tp.DataSource(Table);
        this.ListSourceByName[SourceName] = Source;
        return Source;
    }
    /**
     * Applies lookup list binding to a combo box.
     * @param {tp.ComboBox} ComboBox The combo box.
     * @returns {void}
     */
    ApplyComboBoxListSource(ComboBox) {
        var ListSource;
        if (!(ComboBox instanceof tp.ComboBox) || tp.IsBlank(ComboBox.ListSourceName))
            return;
        ListSource = this.GetServerListSource(ComboBox.ListSourceName);
        if (ListSource instanceof tp.DataSource)
            ComboBox.ListSource = ListSource;
    }
    /**
     * Creates a detail grid column for a data column.
     * @param {tp.Grid} Grid The detail grid.
     * @param {tp.DataColumn} Column The data column.
     * @returns {tp.GridColumn|null} Returns the created grid column or null.
     */
    AddDetailGridColumn(Grid, Column) {
        var ListSource;
        if (!(Grid instanceof tp.Grid) || !(Column instanceof tp.DataColumn))
            return null;
        if (Column.IsLookup) {
            ListSource = this.GetServerListSource(Column.LookupSource);
            if (ListSource instanceof tp.DataSource)
                return Grid.AddLookUpColumn(Column.Name, Column.DisplayTitle, "Id", "Name", ListSource);
        }
        return Grid.AddColumn(Column.Name, Column.DisplayTitle);
    }
    /**
     * Creates columns for a detail grid.
     * @param {tp.Grid} Grid The detail grid.
     * @returns {void}
     */
    CreateDetailGridColumns(Grid) {
        var Table;
        var Index;
        var Column;
        if (!(Grid instanceof tp.Grid) || !(Grid.DataSource instanceof tp.DataSource) || !(Grid.DataSource.Table instanceof tp.DataTable))
            return;
        Table = Grid.DataSource.Table;
        Grid.ClearColumns();
        for (Index = 0; Index < Table.Columns.length; Index++) {
            Column = Table.Columns[Index];
            if (this.CanRenderColumn(Column, Table))
                this.AddDetailGridColumn(Grid, Column);
        }
    }
    /**
     * Initializes a server-rendered component only once.
     * @param {HTMLElement} Element The component element.
     * @param {Function} ComponentClass The component class.
     * @returns {tp.Component|null} Returns the component.
     */
    EnsureServerComponent(Element, ComponentClass) {
        var Component;
        if (!(Element instanceof HTMLElement) || !tp.IsFunction(ComponentClass))
            return null;
        Component = tp.GetComponent(Element);
        if (!(Component instanceof ComponentClass))
            Component = new ComponentClass({ ElementOrSelector: Element });
        return Component;
    }
    /**
     * Applies the standard field row layout to a single control.
     * @param {tp.Control} Control The field control.
     * @returns {void}
     */
    ApplyControlRowLayout(Control) {
        var Row;
        if (!(Control instanceof tp.Control))
            return;
        Row = tp.Ui.GetCtrlRow(Control.Handle);
        if (Row instanceof HTMLElement) {
            if (Control instanceof tp.CheckBox)
                Control.Handle.style.gridTemplateColumns = "auto auto minmax(0, 1fr)";
            else
                Row.style.gridTemplateColumns = "28fr 70fr 2fr";
        }
    }
    /**
     * Initializes a server-rendered field row.
     * @param {HTMLElement} Element The row element.
     * @returns {void}
     */
    InitializeServerFieldRow(Element) {
        var Row;
        var Control;
        var TableName;
        var Source;
        if (!(Element instanceof HTMLElement))
            return;
        Row = tp.HasClass(Element, tp.Classes.CheckBoxRow)
            ? this.EnsureServerComponent(Element, tp.CheckBoxRow)
            : this.EnsureServerComponent(Element, tp.CtrlRow);
        Control = Row && Row.Control instanceof tp.Control ? Row.Control : null;
        if (!(Control instanceof tp.Control))
            return;
        TableName = Element.getAttribute("data-wdf-table") || "";
        Source = this.GetServerDataSource(TableName);
        if (Source instanceof tp.DataSource)
            Control.DataSource = Source;
        Control.ReadOnly = this.IsReadOnlyColumn(Control.DataColumn);
        if (Control instanceof tp.LocatorBox) {
            Control.ModuleName = this.Form ? this.Form.ModuleName : "";
            Control.TargetRow = this.Form && this.Form.Module ? this.Form.Module.Row : null;
        }
        this.ApplyComboBoxListSource(Control);
        this.Controls.push(Control);
        this.ApplyControlRowLayout(Control);
    }
    /**
     * Initializes a server-rendered detail grid.
     * @param {HTMLElement} Element The grid element.
     * @returns {void}
     */
    InitializeServerDetailGrid(Element) {
        var TableName;
        var Source;
        var Grid;
        if (!(Element instanceof HTMLElement))
            return;
        TableName = Element.getAttribute("data-wdf-table") || "";
        Source = this.GetServerDataSource(TableName);
        if (!(Source instanceof tp.DataSource))
            return;
        Grid = tp.GetComponent(Element);
        if (!(Grid instanceof tp.Grid)) {
            Grid = new tp.Grid({
                ElementOrSelector: Element,
                DataSource: Source,
                AutoGenerateColumns: false,
                ToolBarVisible: false,
                GroupsVisible: false,
                FilterVisible: false,
                FooterVisible: false
            });
        } else {
            Grid.AutoGenerateColumns = false;
            Grid.DataSource = Source;
        }
        this.CreateDetailGridColumns(Grid);
        this.DetailGrids.push(Grid);
    }
    /**
     * Initializes server-rendered item page markup and binds it to data sources.
     * @returns {void}
     */
    InitializeServerRenderedItemPage() {
        var Root = this.GetServerItemPageRoot();
        var List;
        var Index;
        var Accordion;
        var ItemList;
        if (!(Root instanceof HTMLElement))
            return;
        List = Root.querySelectorAll("[data-wdf-role='root-tabs'], [data-wdf-role='detail-tabs']");
        for (Index = 0; Index < List.length; Index++) {
            this.EnsureServerComponent(List[Index], tp.TabControl);
            if (tp.HasClass(List[Index], "tp-WebItemPage-RootTabs"))
                this.RootTabControl = tp.GetComponent(List[Index]);
            else if (!this.DetailTabControl)
                this.DetailTabControl = tp.GetComponent(List[Index]);
        }
        List = Root.querySelectorAll("[data-wdf-role='field-groups']");
        for (Index = 0; Index < List.length; Index++) {
            Accordion = this.EnsureServerComponent(List[Index], tp.Accordion);
            if (Accordion instanceof tp.Accordion) {
                Accordion.AllowMultiExpand = true;
                ItemList = Accordion.GetElementList();
                ItemList.forEach(function (Item, ItemIndex) {
                    Accordion.Expand(Item.getAttribute("data-wdf-expanded") === "true", ItemIndex);
                });
            }
        }
        List = Root.querySelectorAll("[data-wdf-role='detail-splitter']");
        for (Index = 0; Index < List.length; Index++) {
            if (!(tp.GetComponent(List[Index]) instanceof tp.Splitter)) {
                var Splitter = new tp.Splitter(List[Index]);
                Splitter.IsHorizontal = true;
                Splitter.Panel1MinSize = 180;
                Splitter.Panel2MinSize = 160;
            }
        }
        List = Root.querySelectorAll("[data-wdf-role='field-row']");
        for (Index = 0; Index < List.length; Index++)
            this.InitializeServerFieldRow(List[Index]);
        List = Root.querySelectorAll("[data-wdf-role='detail-grid']");
        for (Index = 0; Index < List.length; Index++)
            this.InitializeServerDetailGrid(List[Index]);
        this.RefreshDetailGridLayout();
    }
    /**
     * Creates a branch container for a detail and its child details.
     * @param {HTMLElement} Parent The parent element.
     * @param {boolean} ApplyMinimumHeight True to apply the larger branch minimum height.
     * @returns {HTMLElement} Returns the branch container.
     */
    CreateDetailBranchContainer(Parent, ApplyMinimumHeight) {
        var Result = Parent.ownerDocument.createElement("div");
        Result.style.boxSizing = "border-box";
        Result.style.display = "flex";
        Result.style.flexDirection = "column";
        Result.style.width = "100%";
        Result.style.height = "100%";
        Result.style.minHeight = ApplyMinimumHeight === true ? "560px" : "0";
        Result.style.minWidth = "0";
        Result.style.overflow = "hidden";
        Parent.appendChild(Result);
        return Result;
    }
    /**
     * Creates a flex panel used by a detail branch.
     * @param {HTMLElement} Parent The parent element.
     * @param {number} MinHeight The minimum panel height.
     * @returns {HTMLElement} Returns the created panel.
     */
    CreateDetailBranchPanel(Parent, MinHeight) {
        var Result = Parent.ownerDocument.createElement("div");
        Result.style.boxSizing = "border-box";
        Result.style.flex = "1 1 0";
        Result.style.minHeight = tp.px(MinHeight);
        Result.style.minWidth = "0";
        Result.style.overflow = "hidden";
        Parent.appendChild(Result);
        return Result;
    }
    /**
     * Creates a horizontal splitter for a detail branch.
     * @param {HTMLElement} Parent The parent element.
     * @returns {tp.Splitter} Returns the splitter.
     */
    CreateDetailSplitter(Parent) {
        var Element = Parent.ownerDocument.createElement("div");
        var Result;
        Element.style.flex = "0 0 5px";
        Element.style.backgroundColor = "#cccccc";
        Parent.appendChild(Element);
        Result = new tp.Splitter(Element);
        Result.IsHorizontal = true;
        Result.Panel1MinSize = 180;
        Result.Panel2MinSize = 160;
        return Result;
    }
    /**
     * Creates a panel for a single child detail branch.
     * @param {HTMLElement} Parent The parent element.
     * @param {tp.DataTable} DetailTable The child detail table.
     * @param {tp.DataSource} MasterSource The master data source.
     * @returns {void}
     */
    CreateSingleChildDetail(Parent, DetailTable, MasterSource) {
        var Header;
        var Content;
        if (!(Parent instanceof HTMLElement) || !(DetailTable instanceof tp.DataTable) || !(MasterSource instanceof tp.DataSource))
            return;
        Parent.style.display = "flex";
        Parent.style.flexDirection = "column";
        Header = Parent.ownerDocument.createElement("div");
        Header.textContent = this.GetTableTitle(DetailTable);
        Header.style.flex = "0 0 auto";
        Header.style.fontWeight = "600";
        Header.style.fontSize = "0.9em";
        Header.style.margin = "0 0 4px 0";
        Parent.appendChild(Header);
        Content = Parent.ownerDocument.createElement("div");
        Content.style.flex = "1 1 0";
        Content.style.minHeight = "0";
        Content.style.overflow = "hidden";
        Parent.appendChild(Content);
        this.BuildDetailBranch(Content, DetailTable, MasterSource, false);
    }
    /**
     * Creates child detail tabs.
     * @param {HTMLElement} Parent The parent element.
     * @param {tp.DataTable[]} Details The child detail tables.
     * @param {tp.DataSource} MasterSource The master data source.
     * @returns {void}
     */
    CreateChildDetailTabs(Parent, Details, MasterSource) {
        var TabControl;
        var DetailPage;
        var Index;
        if (!(Parent instanceof HTMLElement) || !tp.IsArray(Details) || !(MasterSource instanceof tp.DataSource))
            return;
        TabControl = this.CreateTabControl(Parent);
        TabControl.Handle.style.height = "100%";
        TabControl.Handle.style.minHeight = "0";
        TabControl.Handle.style.marginTop = "0";
        TabControl.On("SelectedIndexChanged", this.RefreshDetailGridLayout, this);
        for (Index = 0; Index < Details.length; Index++) {
            DetailPage = TabControl.AddPage(this.GetTableTitle(Details[Index]));
            if (DetailPage instanceof tp.TabPage)
                this.BuildDetailBranch(DetailPage.Handle, Details[Index], MasterSource, false);
        }
        TabControl.SelectedIndex = 0;
    }
    /**
     * Builds a detail branch recursively.
     * @param {HTMLElement} Parent The parent element.
     * @param {tp.DataTable} DetailTable The detail table.
     * @param {tp.DataSource} MasterSource The master data source.
     * @param {boolean} ApplyMinimumHeight True to apply the larger branch minimum height.
     * @returns {void}
     */
    BuildDetailBranch(Parent, DetailTable, MasterSource, ApplyMinimumHeight) {
        var Children = this.GetChildDetailTables(DetailTable);
        var Branch;
        var ParentPanel;
        var ChildPanel;
        var Grid;
        var DetailSource;
        if (!(Parent instanceof HTMLElement) || !(DetailTable instanceof tp.DataTable) || !(MasterSource instanceof tp.DataSource))
            return;
        if (Children.length === 0) {
            this.CreateDetailGrid(Parent, DetailTable, MasterSource);
            return;
        }
        Branch = this.CreateDetailBranchContainer(Parent, ApplyMinimumHeight !== false);
        ParentPanel = this.CreateDetailBranchPanel(Branch, 260);
        Grid = this.CreateDetailGrid(ParentPanel, DetailTable, MasterSource);
        DetailSource = Grid instanceof tp.Grid ? Grid.DataSource : null;
        this.CreateDetailSplitter(Branch);
        ChildPanel = this.CreateDetailBranchPanel(Branch, 280);
        if (DetailSource instanceof tp.DataSource) {
            if (Children.length === 1)
                this.CreateSingleChildDetail(ChildPanel, Children[0], DetailSource);
            else
                this.CreateChildDetailTabs(ChildPanel, Children, DetailSource);
        }
    }
    /**
     * Builds detail grid tabs under a parent table.
     * @param {HTMLElement} Parent The parent element.
     * @param {tp.DataTable} ParentTable The parent table.
     * @param {tp.DataSource} ParentSource The parent data source.
     * @returns {void}
     */
    BuildDetailTabs(Parent, ParentTable, ParentSource) {
        var Details = this.GetChildDetailTables(ParentTable);
        var TabControl;
        var DetailPage;
        var Index;
        if (!(Parent instanceof HTMLElement) || !(ParentTable instanceof tp.DataTable) || !(ParentSource instanceof tp.DataSource) || Details.length === 0)
            return;
        TabControl = this.CreateTabControl(Parent);
        this.DetailTabControl = TabControl;
        TabControl.On("SelectedIndexChanged", this.RefreshDetailGridLayout, this);
        for (Index = 0; Index < Details.length; Index++) {
            DetailPage = TabControl.AddPage(this.GetTableTitle(Details[Index]));
            if (DetailPage instanceof tp.TabPage)
                this.BuildDetailBranch(DetailPage.Handle, Details[Index], ParentSource, true);
        }
        TabControl.SelectedIndex = 0;
        this.RefreshDetailGridLayout();
    }
    /**
     * Refreshes detail grid layout after tab changes.
     * @returns {void}
     */
    RefreshDetailGridLayout() {
        setTimeout(() => {
            this.DetailGrids.forEach(function (Grid) {
                if (Grid instanceof tp.Grid && !Grid.IsDisposed)
                    Grid.RefreshLayout();
            });
        }, 0);
    }
    /**
     * Builds the top table page content.
     * @param {tp.DataTable} TopTable The top item table.
     * @param {HTMLElement} Parent The parent element.
     * @returns {void}
     */
    BuildTopTablePageLayout(TopTable, Parent) {
        this.BuildFieldGroups(Parent, TopTable, this.DataSource);
        this.BuildDetailTabs(Parent, TopTable, this.DataSource);
    }
    /**
     * Builds a single-page layout for the top table.
     * @param {tp.DataTable} TopTable The top item table.
     * @returns {void}
     */
    BuildSinglePageLayout(TopTable) {
        this.BuildTopTablePageLayout(TopTable, this.Form.ItemPage);
    }
    /**
     * Builds a tabbed layout for a top table with details.
     * @param {tp.DataTable} TopTable The top item table.
     * @returns {void}
     */
    BuildTabbedTopLayout(TopTable) {
        var TopPage;
        this.RootTabControl = this.CreateRootTabControl(this.Form.ItemPage);
        this.RootTabControl.On("SelectedIndexChanged", this.RefreshDetailGridLayout, this);
        TopPage = this.RootTabControl.AddPage(this.GetTableTitle(TopTable));
        if (TopPage instanceof tp.TabPage)
            this.BuildTopTablePageLayout(TopTable, TopPage.Handle);
        this.RootTabControl.SelectedIndex = 0;
    }

    // ● public
    /**
     * Builds the generated item page.
     * @returns {void}
     */
    Build() {
        var Table;
        var Details;
        this.Clear();
        if (!this.Form || !(this.Form.ItemPage instanceof HTMLElement) || !(this.Form.Module instanceof tp.DataModule))
            return;
        Table = this.Form.Module.tblItem;
        if (!(Table instanceof tp.DataTable))
            return;
        this.DataSource = new tp.DataSource(Table);
        this.SourceByTable[Table.Name] = this.DataSource;
        if (this.IsServerRenderedItemPage() === true) {
            this.InitializeServerRenderedItemPage();
            return;
        }
        Details = this.GetChildDetailTables(Table);
        if (Details.length > 0)
            this.BuildTabbedTopLayout(Table);
        else
            this.BuildSinglePageLayout(Table);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.WebItemPageBuilder.prototype.tpClass = "tp.WebItemPageBuilder";
/**
 * The owner data form.
 * @type {tp.WebDataForm|null}
 */
tp.WebItemPageBuilder.prototype.Form = null;
/**
 * The item table data source.
 * @type {tp.DataSource|null}
 */
tp.WebItemPageBuilder.prototype.DataSource = null;
/**
 * Created detail data sources.
 * @type {tp.DataSource[]|null}
 */
tp.WebItemPageBuilder.prototype.DetailSources = null;
/**
 * Created data sources keyed by table name.
 * @type {object|null}
 */
tp.WebItemPageBuilder.prototype.SourceByTable = null;
/**
 * Created lookup list data sources keyed by lookup source name.
 * @type {object|null}
 */
tp.WebItemPageBuilder.prototype.ListSourceByName = null;
/**
 * Created detail grids.
 * @type {tp.Grid[]|null}
 */
tp.WebItemPageBuilder.prototype.DetailGrids = null;
/**
 * The root tab control.
 * @type {tp.TabControl|null}
 */
tp.WebItemPageBuilder.prototype.RootTabControl = null;
/**
 * The detail tab control.
 * @type {tp.TabControl|null}
 */
tp.WebItemPageBuilder.prototype.DetailTabControl = null;
/**
 * Created field controls.
 * @type {tp.Control[]|null}
 */
tp.WebItemPageBuilder.prototype.Controls = null;
/**
 * The visual column count used for field groups.
 * @type {number}
 */
tp.WebItemPageBuilder.prototype.ColumnCount = 2;
/**
 * Maximum controls placed in a visual column before continuing to the next column.
 * @type {number}
 */
tp.WebItemPageBuilder.prototype.MaxControlsPerColumn = 8;
/**
 * The visual field column width in pixels.
 * @type {number}
 */
tp.WebItemPageBuilder.prototype.ColumnWidth = 420;
