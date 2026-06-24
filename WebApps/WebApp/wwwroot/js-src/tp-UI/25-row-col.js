// ● row
/**
 * A responsive row.
 * A row listens to its own element resize changes and propagates SizeMode changes to direct child components.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 *
 * @example
 * <div id="Row">
 *     <div class="tp-Col"></div>
 *     <div class="tp-Col"></div>
 * </div>
 * <script>
 *     var Row = new tp.Row("#Row", { Breakpoints: [400, 700, 1000, 1200, 1400] });
 * </script>
 */
tp.Row = class extends tp.Component {
    // ● constructor
    /**
     * Creates a responsive row.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The row create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.Row.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.Row";
        tp.AddClass(this.Handle, tp.Classes.Row);
        this.IsElementResizeListener = true;
    }

    // ● protected
    /**
     * Creates normalized row create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        if (arguments.length > 1) {
            Params = new tp.CreateParams(Options);
            Params.Handle = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        return Params;
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Row.prototype.tpClass = "tp.Row";

// ● column
/**
 * A responsive column.
 * WidthPercents contains one percent width for each size mode:
 * XSmall, Small, Medium, Large, XLarge, and XXLarge.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 *
 * @example
 * <div id="Col"></div>
 * <script>
 *     var Col = new tp.Col("#Col", { WidthPercents: [100, 100, 50, 33.33, 33.33, 25] });
 * </script>
 */
tp.Col = class extends tp.Component {
    // ● constructor
    /**
     * Creates a responsive column.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The column create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.Col.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.Col";
        tp.AddClass(this.Handle, tp.Classes.Col);
        this.ApplyColParams(this.CreateParams);
    }

    // ● protected
    /**
     * Creates normalized column create parameters.
     * @param {tp.CreateParams|object|HTMLElement|string|null|undefined} CreateParams The source create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when CreateParams is a handle or selector.
     * @returns {tp.CreateParams} Returns normalized create parameters.
     */
    static CreateParams(CreateParams, Options) {
        var Params;
        if (arguments.length > 1) {
            Params = new tp.CreateParams(Options);
            Params.Handle = CreateParams;
        } else {
            Params = tp.Component.CreateParams(CreateParams);
        }
        return Params;
    }
    /**
     * Applies create parameters specific to tp.Col.
     * @param {tp.CreateParams|object|null|undefined} Params The create parameters.
     * @returns {void}
     */
    ApplyColParams(Params) {
        if (!Params)
            return;
        if (tp.IsArray(Params.WidthPercents))
            this.WidthPercents = this.NormalizePercents(Params.WidthPercents, this.WidthPercents);
        if (tp.IsArray(Params.ControlWidthPercents))
            this.ControlWidthPercents = this.NormalizePercents(Params.ControlWidthPercents, this.ControlWidthPercents);
    }
    /**
     * Normalizes a percent array to the number of supported size modes.
     * @param {number[]} Source The source percent array.
     * @param {number[]} Default The default percent array.
     * @returns {number[]} Returns a normalized percent array.
     */
    NormalizePercents(Source, Default) {
        var Result = Default.slice();
        var Index;
        if (tp.IsArray(Source)) {
            for (Index = 0; Index < Source.length && Index < Result.length; Index++) {
                if (tp.IsNumber(Source[Index]))
                    Result[Index] = Source[Index];
            }
        }
        return Result;
    }

    // ● public
    /**
     * Notification called by a parent component when its SizeMode changes.
     * @param {string} ParentSizeMode A tp.SizeMode value.
     * @returns {void}
     */
    ParentSizeModeChanged(ParentSizeMode) {
        var Index = tp.SizeModes.indexOf(ParentSizeMode);
        var Percent;
        var List;
        if (Index > 0) {
            Percent = this.WidthPercents[Index - 1];
            if (tp.IsNumber(Percent))
                this.Handle.style.width = Percent + "%";
            List = this.GetComponentList();
            List.forEach(function (Component) {
                if (tp.IsFunction(tp.CtrlRow) && Component instanceof tp.CtrlRow && tp.IsFunction(Component.SetControlPercentWidth)) {
                    Percent = this.ControlWidthPercents[Index - 1];
                    if (tp.IsNumber(Percent))
                        Component.SetControlPercentWidth(Percent + "%");
                }
            }, this);
        }
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.Col.prototype.tpClass = "tp.Col";
/**
 * Percent widths to occupy from parent row according to size mode.
 * Values correspond to XSmall, Small, Medium, Large, XLarge, and XXLarge.
 * @type {number[]}
 */
tp.Col.prototype.WidthPercents = [100, 100, 50, 33.33, 33.33, 25];
/**
 * Percent widths for the control part of a child tp.CtrlRow according to size mode.
 * Values correspond to XSmall, Small, Medium, Large, XLarge, and XXLarge.
 * @type {number[]}
 */
tp.Col.prototype.ControlWidthPercents = [100, 100, 60, 65, 65, 65];
