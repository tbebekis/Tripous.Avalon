// ● command
/**
 * Represents a named application command.
 */
tp.Command = class extends tp.Def {
    // ● constructor
    /**
     * Creates a command.
     * @param {object|string|null|undefined} Source Optional source command or command name.
     */
    constructor(Source = null) {
        super();
        this.Commands = new tp.DefList(tp.Command);

        if (tp.IsString(Source))
            this.Name = Source;
        else if (tp.IsObject(Source))
            this.Assign(Source);
    }

    // ● public
    /**
     * Assigns values from a source object.
     * @param {object|null|undefined} Source The source object.
     * @returns {void}
     */
    Assign(Source) {
        if (tp.IsNil(Source))
            return;

        super.Assign(Source);

        this.ImageFileName = Source.ImageFileName || "";
        this.Form = Source.Form || "";
        this.Type = Source.Type || "";
        this.IsToggle = Source.IsToggle === true;
        this.IsSingleInstance = Source.IsSingleInstance === true;

        this.Commands.Clear();
        if (tp.IsArray(Source.Commands))
            this.Commands.AddRange(Source.Commands);
        else if (tp.IsArray(Source.Children))
            this.Commands.AddRange(Source.Children);
    }
    /**
     * Adds a child command.
     * @param {object|string|tp.Command} Source The source command.
     * @returns {tp.Command} Returns the added command.
     */
    Add(Source) {
        return this.Commands.Add(Source);
    }
    /**
     * Adds a range of child commands.
     * @param {Array<object|string|tp.Command>|null|undefined} Commands The source commands.
     * @returns {void}
     */
    AddRange(Commands) {
        this.Commands.AddRange(Commands);
    }
    /**
     * Returns true when this command opens a UI form.
     * @returns {boolean} Returns true when this command opens a UI form.
     */
    IsUiCommand() {
        return !tp.IsBlankString(this.Form);
    }
    /**
     * Creates an Ajax request from this command.
     * @returns {tp.AjaxRequest} Returns the Ajax request.
     */
    ToAjaxRequest() {
        var Params = {};
        var Result;

        if (tp.IsObject(this.Params))
            tp.MergePropsShallow(Params, this.Params);

        if (!tp.IsBlankString(this.Form))
            Params.Form = this.Form;

        Result = new tp.AjaxRequest(this.Name, Params);
        Result.Type = !tp.IsBlankString(this.Type) ? this.Type : "Ui";
        Result.IsSingleInstance = this.IsSingleInstance === true;
        Result.CommandId = this.Name;
        Result.CommandName = this.Name;
        return Result;
    }
    /**
     * Returns a JSON-friendly object.
     * @returns {object} Returns a JSON-friendly object.
     */
    toJSON() {
        return {
            Name: this.Name,
            TitleKey: this.TitleKey,
            Title: this.Title,
            ImageFileName: this.ImageFileName,
            Form: this.Form,
            Type: this.Type,
            IsToggle: this.IsToggle,
            IsSingleInstance: this.IsSingleInstance,
            Params: this.Params,
            Commands: this.Commands.Items
        };
    }

    // ● public
    /**
     * The image file name used when this command is displayed.
     * @type {string}
     */
    ImageFileName = "";
    /**
     * The registered form opened by this command.
     * @type {string}
     */
    Form = "";
    /**
     * The command type.
     * @type {string}
     */
    Type = "";
    /**
     * True when this command toggles a boolean value.
     * @type {boolean}
     */
    IsToggle = false;
    /**
     * True when this command should open a single UI instance.
     * @type {boolean}
     */
    IsSingleInstance = false;
    /**
     * Child commands.
     * @type {tp.DefList}
     */
    Commands = null;

    // ● properties
    /**
     * Gets true when this command contains child commands.
     * @returns {boolean} Returns true when this command contains child commands.
     */
    get HasChildren() {
        return this.Commands !== null && this.Commands.Count > 0;
    }
    /**
     * Gets child commands.
     * @returns {tp.DefList} Returns child commands.
     */
    get Children() {
        return this.Commands;
    }
};

/**
 * Creates and returns an Ajax request based on a command.
 * @param {tp.Command} Command The command.
 * @returns {tp.AjaxRequest} Returns the Ajax request.
 */
tp.AjaxRequest.CreateFromCommand = function (Command) {
    if (!(Command instanceof tp.Command))
        Command = new tp.Command(Command);
    return Command.ToAjaxRequest();
};
