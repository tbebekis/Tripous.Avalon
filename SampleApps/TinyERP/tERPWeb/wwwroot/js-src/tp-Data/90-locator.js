// ● locator result status
/**
 * Status of a locator resolution operation.
 * @enum {string}
 */
tp.LocatorResultStatus = {
    None: "None",
    InvalidRequest: "InvalidRequest",
    InvalidContext: "InvalidContext",
    NoResult: "NoResult",
    SingleResult: "SingleResult",
    MultipleResults: "MultipleResults",
    TooManyResults: "TooManyResults",
    Error: "Error"
};
Object.freeze(tp.LocatorResultStatus);

// ● locator context
/**
 * Context of a locator resolution operation.
 */
tp.LocatorContext = class {
    // ● constructor
    /**
     * Creates a locator context.
     * @param {string|object|null|undefined} LocatorNameOrSource The locator name or source object.
     */
    constructor(LocatorNameOrSource = null) {
        /**
         * The locator name.
         * @type {string}
         */
        this.LocatorName = "";
        /**
         * Runtime execution parameters.
         * @type {object}
         */
        this.Params = {};
        if (tp.IsObject(LocatorNameOrSource))
            this.Assign(LocatorNameOrSource);
        else if (!tp.IsBlank(LocatorNameOrSource))
            this.LocatorName = String(LocatorNameOrSource);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.LocatorName = tp.IsNil(Source.LocatorName) ? this.LocatorName : String(Source.LocatorName);
        this.Params = tp.IsObject(Source.Params) ? tp.MergePropsShallow({}, Source.Params) : {};
    }
    /**
     * Returns a plain object used by JSON.stringify().
     * @returns {object} Returns a plain object.
     */
    toJSON() {
        return {
            LocatorName: this.LocatorName,
            Params: this.Params || {}
        };
    }
};

// ● locator request
/**
 * Request for a locator resolution operation.
 */
tp.LocatorRequest = class {
    // ● constructor
    /**
     * Creates a locator request.
     * @param {string|object|null|undefined} LocatorNameOrSource The locator name or source object.
     */
    constructor(LocatorNameOrSource = null) {
        /**
         * Exact key value to resolve.
         * @type {*}
         */
        this.KeyValue = null;
        /**
         * Search text.
         * @type {string}
         */
        this.SearchTerm = "";
        /**
         * Search field name.
         * @type {string}
         */
        this.SearchField = "";
        /**
         * True when this is a multi-row locator request.
         * @type {boolean}
         */
        this.IsMultiRow = false;
        /**
         * Locator context.
         * @type {tp.LocatorContext}
         */
        this.Context = new tp.LocatorContext();
        if (tp.IsObject(LocatorNameOrSource))
            this.Assign(LocatorNameOrSource);
        else if (!tp.IsBlank(LocatorNameOrSource))
            this.Context.LocatorName = String(LocatorNameOrSource);
    }

    // ● properties
    /**
     * Gets the locator name.
     * @returns {string} Returns the locator name.
     */
    get LocatorName() {
        return this.Context ? this.Context.LocatorName : "";
    }
    /**
     * Sets the locator name.
     * @param {string} Value The locator name.
     * @returns {void}
     */
    set LocatorName(Value) {
        if (!(this.Context instanceof tp.LocatorContext))
            this.Context = new tp.LocatorContext();
        this.Context.LocatorName = tp.IsNil(Value) ? "" : String(Value);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.KeyValue = "KeyValue" in Source ? Source.KeyValue : this.KeyValue;
        this.SearchTerm = tp.IsNil(Source.SearchTerm) ? this.SearchTerm : String(Source.SearchTerm);
        this.SearchField = tp.IsNil(Source.SearchField) ? this.SearchField : String(Source.SearchField);
        this.IsMultiRow = Source.IsMultiRow === true;
        if (tp.IsObject(Source.Context))
            this.Context = new tp.LocatorContext(Source.Context);
        if (!tp.IsBlank(Source.LocatorName))
            this.LocatorName = Source.LocatorName;
    }
    /**
     * Returns a plain object used by JSON.stringify().
     * @returns {object} Returns a plain object.
     */
    toJSON() {
        return {
            KeyValue: this.KeyValue,
            SearchTerm: this.SearchTerm,
            SearchField: this.SearchField,
            IsMultiRow: this.IsMultiRow === true,
            Context: this.Context ? this.Context.toJSON() : new tp.LocatorContext().toJSON()
        };
    }
};

// ● locator mapping
/**
 * A locator mapping item.
 */
tp.LocatorMapItem = class {
    // ● constructor
    /**
     * Creates a locator mapping item.
     * @param {object|null|undefined} Source The source object.
     */
    constructor(Source = null) {
        /**
         * Locator result field name.
         * @type {string}
         */
        this.SourceField = "";
        /**
         * Target row field name.
         * @type {string}
         */
        this.TargetField = "";
        this.Assign(Source);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.SourceField = tp.IsNil(Source.SourceField) ? this.SourceField : String(Source.SourceField);
        this.TargetField = tp.IsNil(Source.TargetField) ? this.TargetField : String(Source.TargetField);
    }
};

/**
 * A locator mapping plan.
 */
tp.LocatorMapPlan = class {
    // ● constructor
    /**
     * Creates a locator mapping plan.
     * @param {object|null|undefined} Source The source object.
     */
    constructor(Source = null) {
        /**
         * Locator name.
         * @type {string}
         */
        this.LocatorName = "";
        /**
         * Reference field name.
         * @type {string}
         */
        this.ReferenceField = "";
        /**
         * Mapping items.
         * @type {tp.LocatorMapItem[]}
         */
        this.Items = [];
        this.Assign(Source);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        var Index;
        if (!tp.IsObject(Source))
            return;
        this.LocatorName = tp.IsNil(Source.LocatorName) ? this.LocatorName : String(Source.LocatorName);
        this.ReferenceField = tp.IsNil(Source.ReferenceField) ? this.ReferenceField : String(Source.ReferenceField);
        this.Items = [];
        if (tp.IsArray(Source.Items)) {
            for (Index = 0; Index < Source.Items.length; Index++)
                this.Items.push(new tp.LocatorMapItem(Source.Items[Index]));
        }
    }
    /**
     * Applies the mapping plan to a target row.
     * @param {tp.DataRow|object} SourceRow The selected locator result row or plain object.
     * @param {tp.DataRow|object} TargetRow The target row or plain object.
     * @returns {void}
     */
    Apply(SourceRow, TargetRow) {
        var Index;
        var Item;
        var Value;
        if (tp.IsEmpty(SourceRow) || tp.IsEmpty(TargetRow))
            return;
        for (Index = 0; Index < this.Items.length; Index++) {
            Item = this.Items[Index];
            Value = SourceRow instanceof tp.DataRow ? SourceRow.Get(Item.SourceField, null) : SourceRow[Item.SourceField];
            if (TargetRow instanceof tp.DataRow)
                TargetRow.Set(Item.TargetField, Value);
            else
                TargetRow[Item.TargetField] = Value;
        }
    }
};

// ● locator definition
/**
 * A locator field definition.
 */
tp.LocatorFieldDef = class {
    // ● constructor
    /**
     * Creates a locator field definition.
     * @param {object|null|undefined} Source The source object.
     */
    constructor(Source = null) {
        /**
         * Field name.
         * @type {string}
         */
        this.Name = "";
        /**
         * Field data type.
         * @type {number}
         */
        this.DataType = tp.DataFieldType.String;
        this.Assign(Source);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.Name = tp.IsNil(Source.Name) ? this.Name : String(Source.Name);
        this.DataType = tp.IsNumber(Source.DataType) ? Source.DataType : this.DataType;
    }
};

/**
 * A locator definition.
 */
tp.LocatorDef = class {
    // ● constructor
    /**
     * Creates a locator definition.
     * @param {object|null|undefined} Source The source object.
     */
    constructor(Source = null) {
        /**
         * Locator name.
         * @type {string}
         */
        this.Name = "";
        /**
         * Key field name.
         * @type {string}
         */
        this.KeyField = "Id";
        /**
         * Desktop form name.
         * @type {string}
         */
        this.Form = "";
        /**
         * Web form name.
         * @type {string}
         */
        this.WebForm = "";
        /**
         * Minimum search text length.
         * @type {number}
         */
        this.MinimumSearchLength = 0;
        /**
         * Maximum result row count.
         * @type {number}
         */
        this.MaximumResultCount = 0;
        /**
         * Locator fields.
         * @type {tp.LocatorFieldDef[]}
         */
        this.Fields = [];
        /**
         * Fields used by single-row locator UI.
         * @type {string[]}
         */
        this.SingleRowSearchFields = [];
        /**
         * Fields used by multi-row locator UI.
         * @type {string[]}
         */
        this.MultiRowSearchFields = [];
        /**
         * Fields returned by locator execution.
         * @type {string[]}
         */
        this.ResultFields = [];
        /**
         * Fields displayed by locator list UIs.
         * @type {string[]}
         */
        this.ListVisibleFields = [];
        this.Assign(Source);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        var Index;
        if (!tp.IsObject(Source))
            return;
        this.Name = tp.IsNil(Source.Name) ? this.Name : String(Source.Name);
        this.KeyField = tp.IsNil(Source.KeyField) ? this.KeyField : String(Source.KeyField);
        this.Form = tp.IsNil(Source.Form) ? this.Form : String(Source.Form);
        this.WebForm = tp.IsNil(Source.WebForm) ? this.WebForm : String(Source.WebForm);
        this.MinimumSearchLength = tp.IsNumber(Source.MinimumSearchLength) ? Source.MinimumSearchLength : this.MinimumSearchLength;
        this.MaximumResultCount = tp.IsNumber(Source.MaximumResultCount) ? Source.MaximumResultCount : this.MaximumResultCount;
        this.Fields = [];
        if (tp.IsArray(Source.Fields)) {
            for (Index = 0; Index < Source.Fields.length; Index++)
                this.Fields.push(new tp.LocatorFieldDef(Source.Fields[Index]));
        }
        this.SingleRowSearchFields = tp.IsArray(Source.SingleRowSearchFields) ? Source.SingleRowSearchFields.slice() : [];
        this.MultiRowSearchFields = tp.IsArray(Source.MultiRowSearchFields) ? Source.MultiRowSearchFields.slice() : [];
        this.ResultFields = tp.IsArray(Source.ResultFields) ? Source.ResultFields.slice() : [];
        this.ListVisibleFields = tp.IsArray(Source.ListVisibleFields) ? Source.ListVisibleFields.slice() : [];
    }
    /**
     * Returns search field names for a locator UI mode.
     * @param {boolean} IsMultiRow True for multi-row locator UI.
     * @returns {string[]} Returns search field names.
     */
    GetSearchFields(IsMultiRow) {
        return IsMultiRow === true ? this.MultiRowSearchFields.slice() : this.SingleRowSearchFields.slice();
    }
    /**
     * Returns display input field names.
     * @param {boolean} IsMultiRow True for multi-row locator UI.
     * @returns {string[]} Returns display field names.
     */
    GetInputFields(IsMultiRow) {
        var Result = this.GetSearchFields(IsMultiRow);
        var Index;
        var FieldName;
        if (Result.length === 0)
            Result = this.ResultFields.slice();
        if (Result.length === 0) {
            for (Index = 0; Index < this.Fields.length; Index++) {
                FieldName = this.Fields[Index].Name;
                if (!tp.IsBlank(FieldName))
                    Result.push(FieldName);
            }
        }
        return Result;
    }
    /**
     * Returns fields displayed by locator result UI.
     * @returns {string[]} Returns display field names.
     */
    GetListVisibleFields() {
        return this.ListVisibleFields.length > 0 ? this.ListVisibleFields.slice() : this.ResultFields.slice();
    }
};

/**
 * Result of a locator metadata request.
 */
tp.LocatorInfo = class {
    // ● constructor
    /**
     * Creates locator metadata.
     * @param {object|null|undefined} Source The source object.
     */
    constructor(Source = null) {
        /**
         * Locator definition.
         * @type {tp.LocatorDef|null}
         */
        this.Locator = null;
        /**
         * Optional mapping plan.
         * @type {tp.LocatorMapPlan|null}
         */
        this.MapPlan = null;
        this.Assign(Source);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.Locator = tp.IsObject(Source.Locator) ? new tp.LocatorDef(Source.Locator) : null;
        this.MapPlan = tp.IsObject(Source.MapPlan) ? new tp.LocatorMapPlan(Source.MapPlan) : null;
    }
};

// ● locator result
/**
 * Result of a locator resolution operation.
 */
tp.LocatorResult = class {
    // ● constructor
    /**
     * Creates a locator result.
     * @param {object|null|undefined} Source The source object.
     */
    constructor(Source = null) {
        /**
         * Result status.
         * @type {string}
         */
        this.Status = tp.LocatorResultStatus.None;
        /**
         * Result message.
         * @type {string}
         */
        this.Message = "";
        /**
         * Result count.
         * @type {number}
         */
        this.Count = 0;
        /**
         * Reference web form name.
         * @type {string}
         */
        this.WebForm = "";
        /**
         * Result table.
         * @type {tp.DataTable|null}
         */
        this.Table = null;
        /**
         * Optional mapping plan.
         * @type {tp.LocatorMapPlan|null}
         */
        this.MapPlan = null;
        this.Assign(Source);
    }

    // ● properties
    /**
     * Gets true when the result has one row.
     * @returns {boolean} Returns true when the result has one row.
     */
    get HasSingleResult() {
        return this.Status === tp.LocatorResultStatus.SingleResult && this.Count === 1;
    }
    /**
     * Gets true when the result has multiple rows.
     * @returns {boolean} Returns true when the result has multiple rows.
     */
    get HasMultipleResults() {
        return this.Status === tp.LocatorResultStatus.MultipleResults && this.Count > 1;
    }
    /**
     * Gets true when the result is too broad.
     * @returns {boolean} Returns true when the result is too broad.
     */
    get HasTooManyResults() {
        return this.Status === tp.LocatorResultStatus.TooManyResults;
    }
    /**
     * Gets true when the result is an error.
     * @returns {boolean} Returns true when the result is an error.
     */
    get HasError() {
        return this.Status === tp.LocatorResultStatus.Error;
    }
    /**
     * Gets the first result row.
     * @returns {tp.DataRow|null} Returns the first result row or null.
     */
    get FirstRow() {
        return this.Table instanceof tp.DataTable && this.Table.RowCount > 0 ? this.Table.Rows[0] : null;
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (!tp.IsObject(Source))
            return;
        this.Status = tp.IsNil(Source.Status) ? this.Status : String(Source.Status);
        this.Message = tp.IsNil(Source.Message) ? "" : String(Source.Message);
        this.Count = tp.IsNumber(Source.Count) ? Source.Count : 0;
        this.WebForm = tp.IsNil(Source.WebForm) ? "" : String(Source.WebForm);
        this.Table = tp.IsObject(Source.Table) ? new tp.DataTable(Source.Table) : null;
        this.MapPlan = tp.IsObject(Source.MapPlan) ? new tp.LocatorMapPlan(Source.MapPlan) : null;
    }
    /**
     * Applies the mapping plan to a target row.
     * @param {tp.DataRow|object|null|undefined} TargetRow The target row or plain object.
     * @param {tp.DataRow|object|null|undefined} SourceRow The selected locator result row or plain object.
     * @returns {void}
     */
    Apply(TargetRow, SourceRow = null) {
        if (this.MapPlan instanceof tp.LocatorMapPlan)
            this.MapPlan.Apply(SourceRow || this.FirstRow, TargetRow);
    }
};

// ● locator
/**
 * Client-side proxy for server-side locator operations.
 */
tp.Locator = class {
    // ● static
    /**
     * Returns locator metadata.
     * @param {string|object} LocatorNameOrParams The locator name or request parameters.
     * @param {object|null|undefined} Params Optional request parameters when the first argument is a locator name.
     * @returns {Promise<tp.LocatorInfo>} Returns locator metadata.
     */
    static async GetInfoAsync(LocatorNameOrParams, Params = null) {
        var Request;
        var Packet;
        if (tp.IsString(LocatorNameOrParams)) {
            Request = tp.IsObject(Params) ? tp.Assign({}, Params) : {};
            Request.LocatorName = LocatorNameOrParams;
        } else if (tp.IsObject(LocatorNameOrParams)) {
            Request = tp.Assign({}, LocatorNameOrParams);
        } else {
            tp.Throw("Cannot get locator metadata. Invalid parameters.");
        }
        Packet = await tp.AjaxRequest.Execute("Locator.GetInfo", Request);
        return new tp.LocatorInfo(Packet);
    }
    /**
     * Executes a locator request.
     * @param {tp.LocatorRequest|object|string} RequestOrLocatorName The request, request-like object, or locator name.
     * @param {object|null|undefined} Params Optional request parameters when the first argument is a locator name.
     * @returns {Promise<tp.LocatorResult>} Returns the locator result.
     */
    static async ExecuteAsync(RequestOrLocatorName, Params = null) {
        var Request;
        var ExtraParams = null;
        var PacketParams;
        var Packet;
        if (RequestOrLocatorName instanceof tp.LocatorRequest) {
            Request = RequestOrLocatorName;
            ExtraParams = Params;
        } else if (tp.IsString(RequestOrLocatorName)) {
            Request = new tp.LocatorRequest(RequestOrLocatorName);
            if (tp.IsObject(Params)) {
                Request.Assign(Params);
                ExtraParams = Params;
            }
        } else if (tp.IsObject(RequestOrLocatorName)) {
            Request = new tp.LocatorRequest(RequestOrLocatorName);
            ExtraParams = RequestOrLocatorName;
        } else {
            tp.Throw("Cannot execute locator request. Invalid parameters.");
        }
        PacketParams = Request.toJSON();
        tp.Locator.CopyExtraParams(PacketParams, ExtraParams);
        Packet = await tp.AjaxRequest.Execute("Locator.Execute", PacketParams);
        return new tp.LocatorResult(Packet);
    }
    /**
     * Copies extra request parameters to a packet.
     * @param {object} PacketParams The packet parameters.
     * @param {object|null|undefined} ExtraParams The extra source parameters.
     * @returns {void}
     */
    static CopyExtraParams(PacketParams, ExtraParams) {
        var Name;
        if (!tp.IsObject(PacketParams) || !tp.IsObject(ExtraParams))
            return;
        for (Name in ExtraParams) {
            if (Object.prototype.propertyIsEnumerable.call(ExtraParams, Name) && !(Name in PacketParams))
                PacketParams[Name] = ExtraParams[Name];
        }
    }
};

// ● prototypes
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorContext.prototype.tpClass = "tp.LocatorContext";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorRequest.prototype.tpClass = "tp.LocatorRequest";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorMapItem.prototype.tpClass = "tp.LocatorMapItem";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorMapPlan.prototype.tpClass = "tp.LocatorMapPlan";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorFieldDef.prototype.tpClass = "tp.LocatorFieldDef";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorDef.prototype.tpClass = "tp.LocatorDef";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorInfo.prototype.tpClass = "tp.LocatorInfo";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.LocatorResult.prototype.tpClass = "tp.LocatorResult";
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Locator.prototype.tpClass = "tp.Locator";
