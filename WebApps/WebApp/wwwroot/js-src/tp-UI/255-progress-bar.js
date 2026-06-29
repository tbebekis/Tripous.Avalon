// ● progress bar
/**
 * A native HTML progress control.
 */
tp.ProgressBar = class extends tp.Component {
    // ● constructor
    /**
     * Creates a progress bar.
     * @param {tp.CreateParams|object|null|undefined} CreateParams The create params.
     */
    constructor(CreateParams) {
        var Params = tp.Component.CreateParams(CreateParams);
        if (tp.IsNil(Params.ElementOrSelector))
            Params.ElementOrSelector = "progress";
        super(Params);
    }

    // ● protected
    /**
     * Initializes the 'pseudo-static' and 'read-only' class metadata fields such as the ElementType, ElementSubtype and DataValueProperty
     * @returns {void}
     */
    InitClass() {
        super.InitClass();
        this.fElementType = "progress";
    }
    /**
     * Initializes fields and properties before applying create params.
     * @returns {void}
     */
    InitializeFields() {
        super.InitializeFields();
        this.fInfiniteLoopTimer = 0;
        this.fInfiniteLoopInterval = 150;
        if (this.Handle instanceof HTMLProgressElement) {
            this.Handle.max = tp.IsNil(this.Handle.max) || this.Handle.max <= 0 ? 100 : this.Handle.max;
            this.Handle.value = tp.IsNil(this.Handle.value) ? 0 : this.Handle.value;
        }
    }
    /**
     * Applies explicit create params to this progress bar.
     * @param {tp.CreateParams|object|null|undefined} Params The create params to apply.
     * @returns {void}
     */
    ApplyCreateParams(Params) {
        super.ApplyCreateParams(Params);
        if (!Params)
            return;
        if (!tp.IsNil(Params.Max))
            this.Max = Params.Max;
        if (!tp.IsNil(Params.Value))
            this.Value = Params.Value;
    }
    /**
     * Notification called after handle creation.
     * @returns {void}
     */
    OnHandleCreated() {
        super.OnHandleCreated();
        tp.AddClass(this.Handle, tp.Classes.ProgressBar);
    }
    /**
     * Releases resources held by this instance.
     * @protected
     * @returns {void}
     */
    DoDispose() {
        this.StopInfiniteLoop();
        super.DoDispose();
    }
    /**
     * Advances the progress value by one step, wrapping to zero at Max.
     * @protected
     * @returns {void}
     */
    StepLoop() {
        if (this.Value < this.Max)
            this.Value += 1;
        else
            this.Value = 0;
    }

    // ● properties
    /**
     * Gets or sets the maximum value.
     * @returns {number} Returns the maximum value.
     */
    get Max() {
        return this.Handle instanceof HTMLProgressElement ? this.Handle.max : 0;
    }
    /**
     * Gets or sets the maximum value.
     * @param {number} Value The maximum value.
     * @returns {void}
     */
    set Max(Value) {
        if (this.Handle instanceof HTMLProgressElement)
            this.Handle.max = tp.IsNil(Value) ? 100 : Number(Value);
    }
    /**
     * Gets or sets the current value.
     * @returns {number} Returns the current value.
     */
    get Value() {
        return this.Handle instanceof HTMLProgressElement ? this.Handle.value : 0;
    }
    /**
     * Gets or sets the current value.
     * @param {number} Value The current value.
     * @returns {void}
     */
    set Value(Value) {
        if (this.Handle instanceof HTMLProgressElement)
            this.Handle.value = tp.IsNil(Value) ? 0 : Number(Value);
    }
    /**
     * Returns true when the infinite loop is running.
     * @returns {boolean} Returns true when the infinite loop is running.
     */
    get IsInfiniteLoopRunning() {
        return this.fInfiniteLoopTimer !== 0;
    }

    // ● public
    /**
     * Starts an infinite progress loop.
     * @param {number|null|undefined} Interval The interval in milliseconds.
     * @returns {void}
     */
    StartInfiniteLoop(Interval) {
        var Instance = this;
        this.StopInfiniteLoop();
        if (tp.IsNumber(Interval) && Interval > 0)
            this.fInfiniteLoopInterval = Interval;
        this.fInfiniteLoopTimer = setInterval(function () {
            Instance.StepLoop();
        }, this.fInfiniteLoopInterval);
    }
    /**
     * Stops the infinite progress loop.
     * @returns {void}
     */
    StopInfiniteLoop() {
        if (this.fInfiniteLoopTimer !== 0) {
            clearInterval(this.fInfiniteLoopTimer);
            this.fInfiniteLoopTimer = 0;
        }
    }
};

tp.Ui.RegisterType(["ProgressBar", "tp-ProgressBar"], tp.ProgressBar);
