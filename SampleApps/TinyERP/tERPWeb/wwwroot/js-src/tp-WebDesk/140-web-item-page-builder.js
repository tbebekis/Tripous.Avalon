// ● web item page builder
/**
 * Builds the first generated WebDesk item page surface from a data module item table.
 *
 * This builder handles scalar top-table fields and the first generated detail grid tabs.
 * Custom editors, nested detail layouts and save workflow are added by later WebDesk steps.
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
         * Created detail grids.
         * @type {tp.Grid[]}
         */
        this.DetailGrids = [];
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
        this.DetailGrids = [];
        this.DataSource = null;
        if (this.Form && this.Form.ItemPage instanceof HTMLElement)
            tp.RemoveChildren(this.Form.ItemPage);
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
        if (Item instanceof HTMLElement) {
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
        var ColumnCount = tp.WebItemPageBuilder.NormalizeColumnCount(this.ColumnCount);
        Result.style.display = "grid";
        Result.style.gridTemplateColumns = "repeat(" + ColumnCount + ", minmax(0, " + this.ColumnWidth + "px))";
        Result.style.columnGap = "16px";
        Result.style.rowGap = "6px";
        Result.style.alignItems = "start";
        Result.style.justifyContent = "start";
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
                    Row.style.display = "grid";
                    Row.style.gridTemplateColumns = "28fr 70fr 2fr";
                    Control.Handle.style.gridColumn = "2";
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
     * Creates a tab control.
     * @param {HTMLElement} Parent The parent element.
     * @returns {tp.TabControl} Returns the tab control.
     */
    CreateTabControl(Parent) {
        var Element = Parent.ownerDocument.createElement("div");
        var Result = new tp.TabControl({
            ElementOrSelector: Element
        });
        Element.style.minHeight = "300px";
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
        Parent.appendChild(Element);
        Source = new tp.DataSource(DetailTable);
        Source.MasterKeyField = DetailTable.MasterField;
        Source.DetailKeyField = DetailTable.DetailField;
        Source.MasterSource = MasterSource;
        Grid = new tp.Grid({
            ElementOrSelector: Element,
            DataSource: Source,
            AutoGenerateColumns: true,
            ToolBarVisible: false,
            GroupsVisible: false,
            FilterVisible: false,
            FooterVisible: false
        });
        this.DetailSources.push(Source);
        this.DetailGrids.push(Grid);
        return Grid;
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
        for (Index = 0; Index < Details.length; Index++) {
            DetailPage = TabControl.AddPage(Details[Index].Name);
            if (DetailPage instanceof tp.TabPage)
                this.CreateDetailGrid(DetailPage.Handle, Details[Index], ParentSource);
        }
        TabControl.SelectedIndex = 0;
    }
    /**
     * Builds a single-page layout for the top table and its first-level details.
     * @param {tp.DataTable} TopTable The top item table.
     * @returns {void}
     */
    BuildSinglePageLayout(TopTable) {
        this.BuildFieldGroups(this.Form.ItemPage, TopTable, this.DataSource);
        this.BuildDetailTabs(this.Form.ItemPage, TopTable, this.DataSource);
    }

    // ● public
    /**
     * Builds the generated item page.
     * @returns {void}
     */
    Build() {
        var Table;
        this.Clear();
        if (!this.Form || !(this.Form.ItemPage instanceof HTMLElement) || !(this.Form.Module instanceof tp.DataModule))
            return;
        Table = this.Form.Module.tblItem;
        if (!(Table instanceof tp.DataTable))
            return;
        this.DataSource = new tp.DataSource(Table);
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
 * Created detail grids.
 * @type {tp.Grid[]|null}
 */
tp.WebItemPageBuilder.prototype.DetailGrids = null;
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
