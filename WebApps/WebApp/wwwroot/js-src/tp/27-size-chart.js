// ● size mode
/**
 * Container size mode CSS class names.
 * A size mode describes a component/container width, not the browser viewport.
 * @type {object}
 */
tp.SizeMode = {
    None: "",
    XSmall: "tp-XSmall",
    Small: "tp-Small",
    Medium: "tp-Medium",
    Large: "tp-Large",
    XLarge: "tp-XLarge",
    XXLarge: "tp-XXLarge"
};
Object.freeze(tp.SizeMode);
/**
 * Ordered size mode values.
 * @type {string[]}
 */
tp.SizeModes = [
    tp.SizeMode.None,
    tp.SizeMode.XSmall,
    tp.SizeMode.Small,
    tp.SizeMode.Medium,
    tp.SizeMode.Large,
    tp.SizeMode.XLarge,
    tp.SizeMode.XXLarge
];
Object.freeze(tp.SizeModes);
/**
 * Default container width breakpoints.
 * These values match the Tripous viewport breakpoint thresholds.
 * @type {number[]}
 */
tp.DefaultBreakpoints = [
    575.98,
    767.98,
    991.98,
    1199.98,
    1399.98
];
Object.freeze(tp.DefaultBreakpoints);

// ● size chart
/**
 * Detects container size mode changes from a width value.
 * This is used by UI containers and controls whose layout depends on their own width.
 */
tp.SizeChart = class {
    // ● constructor
    /**
     * Creates a size chart.
     * @param {number[]|null|undefined} Source Optional breakpoint values.
     */
    constructor(Source) {
        this.Breakpoints = tp.DefaultBreakpoints.slice();
        this.Mode = tp.SizeMode.None;
        this.LastMode = tp.SizeMode.None;
        this.Assign(Source);
    }

    // ● protected
    /**
     * Returns the size mode for a width.
     * @param {number} Width The width.
     * @returns {string} Returns a tp.SizeMode value.
     * @protected
     */
    GetMode(Width) {
        var Index;
        var Limit;
        if (!tp.IsNumber(Width) || Width <= 0)
            return tp.SizeMode.None;
        for (Index = 0; Index < this.Breakpoints.length; Index++) {
            Limit = this.Breakpoints[Index];
            if (tp.IsNumber(Limit) && Width <= Limit)
                return tp.SizeModes[Index + 1];
        }
        return tp.SizeMode.XXLarge;
    }

    // ● public
    /**
     * Returns true when a width changes the current size mode.
     * @param {number} Width The width.
     * @returns {boolean} Returns true when the size mode changed.
     */
    IsModeChange(Width) {
        var NewMode = this.GetMode(Width);
        if (NewMode !== tp.SizeMode.None && this.Mode !== NewMode) {
            this.LastMode = this.Mode;
            this.Mode = NewMode;
            return true;
        }
        return false;
    }
    /**
     * Assigns custom breakpoints.
     * The source may contain up to five numbers: XSmall, Small, Medium, Large, and XLarge upper bounds.
     * @param {number[]|null|undefined} Source The breakpoint values.
     * @returns {void}
     */
    Assign(Source) {
        var Index;
        if (tp.IsArray(Source) && Source.length > 0 && Source.length <= tp.DefaultBreakpoints.length) {
            for (Index = 0; Index < Source.length; Index++) {
                if (tp.IsNumber(Source[Index]))
                    this.Breakpoints[Index] = Source[Index];
            }
        }
    }
};

// ● prototype
/**
 * The current size mode.
 * @type {string}
 */
tp.SizeChart.prototype.Mode = tp.SizeMode.None;
/**
 * The last size mode before the current mode.
 * @type {string}
 */
tp.SizeChart.prototype.LastMode = tp.SizeMode.None;
/**
 * The breakpoint values.
 * @type {number[]}
 */
tp.SizeChart.prototype.Breakpoints = [];
