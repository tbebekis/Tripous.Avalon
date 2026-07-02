// ● data module action
/**
 * Describes a client-side data module action.
 */
tp.DataModuleAction = class {


    // ● constructor
    /**
     * Creates a data module action.
     * @param {tp.DataModule} DataModule The data module executing the action.
     * @param {string} Name The action name.
     * @param {object|null|undefined} Params The request parameters.
     */
    constructor(DataModule, Name, Params) {
        this.DataModule = DataModule || null;
        this.Name = Name || "";
        this.Params = Params || {};
        this.Packet = null;
        this.Result = null;
    }

    // ● fields
    /**
     * The data module executing the action.
     * @type {tp.DataModule|null}
     */
    DataModule = null;
    /**
     * The action name.
     * @type {string}
     */
    Name = "";
    /**
     * The request parameters.
     * @type {object|null}
     */
    Params = null;
    /**
     * The returned response packet.
     * @type {object|null}
     */
    Packet = null;
    /**
     * The action result object.
     * @type {*}
     */
    Result = null;
};

// ● data module
/**
 * Client-side proxy for a server-side Tripous.Data.DataModule.
 * Events:
 * - Action
 */
tp.DataModule = class extends tp.Object {
    // ● constructor
    /**
     * Creates a data module proxy.
     * @param {string|object|null|undefined} NameOrSource The module name or a JsonDataModule source object.
     */
    constructor(NameOrSource) {
        super();
        this.Name = "";
        this.Title = "";
        this.TitleKey = "";
        this.Group = "";
        this.ClassName = "";
        this.ConnectionName = "";
        this.Description = "";
        this.IsSingleSelect = false;
        this.UseFilters = false;
        this.SecurityLevel = 0;
        this.GuidOids = true;
        this.CascadeDeletes = true;
        this.ItemCaptionField = "";
        this.MainTableName = "";
        this.ListTableName = "";
        this.ItemTableName = "";
        this.State = tp.DataMode.None;
        this.QueryNames = [];
        this.StockNames = [];
        this.DataSet = new tp.DataSet();
        this.LastSelectName = "";
        this.LastWhereText = "";
        if (tp.IsObject(NameOrSource))
            this.Assign(NameOrSource);
        else if (!tp.IsBlank(NameOrSource))
            this.Name = String(NameOrSource);
    }

    // ● protected
    /**
     * Creates data module request parameters.
     * @param {object|null|undefined} ExtraParams Optional extra parameters.
     * @returns {object} Returns request parameters.
     */
    CreateParams(ExtraParams) {
        var Result = {
            ModuleName: this.Name
        };
        if (tp.IsObject(ExtraParams))
            tp.Assign(Result, ExtraParams);
        return Result;
    }
    /**
     * Executes a standard data module Ajax operation.
     * @param {string} OperationName The operation name.
     * @param {object|null|undefined} Params The request parameters.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async ExecuteAction(OperationName, Params) {
        var Action = new tp.DataModuleAction(this, OperationName, Params);
        Action.Packet = await tp.AjaxRequest.Execute(OperationName, Params);
        this.OnAction(Action);
        return Action;
    }
    /**
     * Assigns a returned DataModule packet to this instance.
     * @param {object|null|undefined} Packet The response packet.
     * @returns {object|null} Returns the assigned JsonDataModule object or null.
     */
    AssignDataModulePacket(Packet) {
        var Source = Packet && Packet.DataModule ? Packet.DataModule : Packet;
        if (tp.IsObject(Source))
            this.Assign(Source);
        return tp.IsObject(Source) ? Source : null;
    }

    // ● properties
    /**
     * Gets the list table.
     * @returns {tp.DataTable|null} Returns the list table or null.
     */
    get tblList() {
        return this.FindTable(this.ListTableName);
    }
    /**
     * Gets the item table.
     * @returns {tp.DataTable|null} Returns the item table or null.
     */
    get tblItem() {
        return this.FindTable(this.ItemTableName || this.MainTableName);
    }
    /**
     * Gets the first item row.
     * @returns {tp.DataRow|null} Returns the first item row or null.
     */
    get Row() {
        var Table = this.tblItem;
        return Table && Table.RowCount > 0 ? Table.Rows[0] : null;
    }
    /**
     * Gets the current item id.
     * @returns {*} Returns the current item id.
     */
    get Id() {
        var Row = this.Row;
        var Table = this.tblItem;
        if (Row && Table)
            return Row.Get(Table.KeyField, null);
        return null;
    }
    /**
     * Gets true when the module has been initialized.
     * @returns {boolean} Returns true when initialized.
     */
    get Initialized() {
        return this.DataSet instanceof tp.DataSet && this.DataSet.TableCount > 0;
    }

    // ● public
    /**
     * Returns a plain object used by JSON.stringify().
     * @returns {object} Returns a plain object.
     */
    toJSON() {
        return {
            Name: this.Name,
            Title: this.Title,
            TitleKey: this.TitleKey,
            Group: this.Group,
            ClassName: this.ClassName,
            ConnectionName: this.ConnectionName,
            Description: this.Description,
            IsSingleSelect: this.IsSingleSelect,
            UseFilters: this.UseFilters,
            SecurityLevel: this.SecurityLevel,
            GuidOids: this.GuidOids,
            CascadeDeletes: this.CascadeDeletes,
            ItemCaptionField: this.ItemCaptionField,
            MainTableName: this.MainTableName,
            ListTableName: this.ListTableName,
            ItemTableName: this.ItemTableName,
            State: this.State,
            QueryNames: this.QueryNames.slice(),
            StockNames: this.StockNames.slice(),
            DataSet: this.DataSet.toJSON()
        };
    }
    /**
     * Assigns values from a JsonDataModule source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.Name = Source.Name || this.Name;
        this.Title = Source.Title || "";
        this.TitleKey = Source.TitleKey || "";
        this.Group = Source.Group || "";
        this.ClassName = Source.ClassName || "";
        this.ConnectionName = Source.ConnectionName || "";
        this.Description = Source.Description || "";
        this.IsSingleSelect = Source.IsSingleSelect === true;
        this.UseFilters = Source.UseFilters === true;
        this.SecurityLevel = tp.IsNumber(Source.SecurityLevel) ? Source.SecurityLevel : 0;
        this.GuidOids = Source.GuidOids !== false;
        this.CascadeDeletes = Source.CascadeDeletes !== false;
        this.ItemCaptionField = Source.ItemCaptionField || "";
        this.MainTableName = Source.MainTableName || "";
        this.ListTableName = Source.ListTableName || "";
        this.ItemTableName = Source.ItemTableName || "";
        this.State = tp.IsNumber(Source.State) ? Source.State : tp.DataMode.None;
        this.QueryNames = tp.IsArray(Source.QueryNames) ? Source.QueryNames.slice() : [];
        this.StockNames = tp.IsArray(Source.StockNames) ? Source.StockNames.slice() : [];
        if (tp.IsObject(Source.DataSet))
            this.DataSet.Assign(Source.DataSet);
    }
    /**
     * Finds a table by name.
     * @param {string} TableName The table name.
     * @returns {tp.DataTable|null} Returns the table or null.
     */
    FindTable(TableName) {
        return this.DataSet instanceof tp.DataSet ? this.DataSet.FindTable(TableName) : null;
    }
    /**
     * Initializes the module.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async Initialize() {
        var Action = await this.ExecuteAction("DataModule.Initialize", this.CreateParams());
        Action.Result = this.AssignDataModulePacket(Action.Packet);
        return Action;
    }
    /**
     * Starts an insert operation.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async Insert() {
        var Action = await this.ExecuteAction("DataModule.Insert", this.CreateParams());
        Action.Result = this.AssignDataModulePacket(Action.Packet);
        return Action;
    }
    /**
     * Starts an edit operation.
     * @param {*} Id The item id.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async Edit(Id) {
        var Action = await this.ExecuteAction("DataModule.Edit", this.CreateParams({ Id: Id }));
        Action.Result = this.AssignDataModulePacket(Action.Packet);
        return Action;
    }
    /**
     * Deletes an item.
     * @param {*} Id The item id.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async Delete(Id) {
        var Action = await this.ExecuteAction("DataModule.Delete", this.CreateParams({ Id: Id }));
        if (Action.Packet && Action.Packet.Success === true && this.tblList) {
            var Row = this.tblList.FindRow(this.tblList.KeyField, Id);
            if (Row)
                this.tblList.RemoveRow(Row);
        }
        Action.Result = Action.Packet ? Action.Packet.Success === true : false;
        return Action;
    }
    /**
     * Commits the current item.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async Commit() {
        var Action = await this.ExecuteAction("DataModule.Commit", this.CreateParams({ DataModule: this.toJSON() }));
        Action.Result = this.AssignDataModulePacket(Action.Packet);
        return Action;
    }
    /**
     * Selects the list table.
     * @param {string|null|undefined} SelectName The registered select name.
     * @param {string|null|undefined} WhereText The generated filter WHERE fragment.
     * @returns {Promise<tp.DataModuleAction>} Returns the action.
     */
    async SelectList(SelectName, WhereText) {
        var Params = this.CreateParams({
            SelectName: SelectName || "",
            WhereText: WhereText || ""
        });
        var Action = await this.ExecuteAction("DataModule.SelectList", Params);
        var Table;
        if (Action.Packet && tp.IsObject(Action.Packet.Table)) {
            this.ListTableName = Action.Packet.Table.Name || this.ListTableName;
            Table = this.FindTable(this.ListTableName);
            if (Table)
                Table.Assign(Action.Packet.Table);
            else
                this.DataSet.AddTable(Action.Packet.Table);
            this.LastSelectName = Action.Packet.SelectName || SelectName || "";
            this.LastWhereText = WhereText || "";
            Action.Result = this.tblList;
        }
        return Action;
    }

    // ● events
    /**
     * Event trigger called after an action is executed.
     * @param {tp.DataModuleAction} Action The executed action.
     * @returns {void}
     */
    OnAction(Action) {
        this.Trigger("Action", { Action: Action, DataModule: this });
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.DataModule.prototype.tpClass = "tp.DataModule";
