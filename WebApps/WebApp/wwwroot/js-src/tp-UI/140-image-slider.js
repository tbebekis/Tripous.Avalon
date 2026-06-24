// ● image size mode
/**
 * Indicates how an image is sized inside its container.
 * @enum {number}
 */
tp.ImageSizeMode = {
    Unknown: 0,
    Crop: 1,
    Scale: 2,
    Stretch: 4
};
Object.freeze(tp.ImageSizeMode);

// ● image slider
/**
 * Displays a list of images as a background-image slider.
 *
 * Events:
 * - SelectedIndexChanged
 *
 * @implements {tp.ISelectedIndex}
 */
tp.ImageSlider = class extends tp.Component {
    // ● constructor
    /**
     * Creates an image slider.
     * @param {tp.CreateParams|object|HTMLElement|string} CreateParams The image slider create parameters, handle, or selector.
     * @param {object|null|undefined} Options Optional settings used when the first argument is a handle or selector.
     */
    constructor(CreateParams, Options) {
        var Params = tp.ImageSlider.CreateParams(CreateParams, Options);
        super(Params);
        this.tpClass = "tp.ImageSlider";
        tp.AddClass(this.Handle, tp.Classes.ImageSlider);
        this.fImages = [];
        this.fSelectedIndex = -1;
        this.fAutoCycleInterval = 6000;
        this.fAutoCycle = true;
        this.PauseOnHover = true;
        this.ChangeOnClick = true;
        this.fDisplayCycleButtons = true;
        this.fMouseEnterHandler = this.FuncBind(this.HandleMouseEnter);
        this.fMouseLeaveHandler = this.FuncBind(this.HandleMouseLeave);
        this.fClickHandler = this.FuncBind(this.HandleClick);
        this.CreateCycleButtons();
        this.ApplyImageSliderParams(this.CreateParams);
        this.Handle.addEventListener("mouseenter", this.fMouseEnterHandler);
        this.Handle.addEventListener("mouseleave", this.fMouseLeaveHandler);
        this.Handle.addEventListener("click", this.fClickHandler);
        this.DoAutoCycle(this.AutoCycle);
    }

    // ● protected
    /**
     * Creates normalized image slider create parameters.
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
     * Applies create parameters specific to tp.ImageSlider.
     * @param {tp.CreateParams|object|null|undefined} Params The create parameters.
     * @returns {void}
     */
    ApplyImageSliderParams(Params) {
        if (!Params)
            return;
        if (!tp.IsNil(Params.Width))
            this.Handle.style.width = tp.IsNumber(Params.Width) ? tp.px(Params.Width) : String(Params.Width);
        if (!tp.IsNil(Params.Height))
            this.Handle.style.height = tp.IsNumber(Params.Height) ? tp.px(Params.Height) : String(Params.Height);
        if (!tp.IsNil(Params.ImageMode))
            this.ImageMode = Params.ImageMode;
        if (!tp.IsNil(Params.PauseOnHover))
            this.PauseOnHover = Params.PauseOnHover === true;
        if (!tp.IsNil(Params.ChangeOnClick))
            this.ChangeOnClick = Params.ChangeOnClick === true;
        if (!tp.IsNil(Params.DisplayCycleButtons))
            this.DisplayCycleButtons = Params.DisplayCycleButtons;
        if (!tp.IsNil(Params.AutoCycleMSecs))
            this.AutoCycleMSecs = Params.AutoCycleMSecs;
        if (!tp.IsNil(Params.AutoCycle))
            this.fAutoCycle = Params.AutoCycle === true;
        if (tp.IsArray(Params.Images))
            this.Images = Params.Images;
        if (!tp.IsNil(Params.SelectedIndex))
            this.SelectedIndex = Params.SelectedIndex;
        else if (this.ImageCount > 0)
            this.SelectedIndex = 0;
    }
    /**
     * Creates the previous and next buttons.
     * @returns {void}
     */
    CreateCycleButtons() {
        this.fPrev = this.Document.createElement("div");
        this.fPrev.className = tp.Classes.Prev;
        this.Handle.appendChild(this.fPrev);
        this.fNext = this.Document.createElement("div");
        this.fNext.className = tp.Classes.Next;
        this.Handle.appendChild(this.fNext);
        this.DisplayCycleButtonsChanged();
    }
    /**
     * Returns a CSS background-image value for an image URL.
     * @param {string} Url The image URL.
     * @returns {string} Returns a CSS background-image value.
     */
    GetBackgroundImage(Url) {
        if (tp.IsBlank(Url))
            return "";
        Url = String(Url);
        return tp.StartsWith(Url, "url(", false) ? Url : "url(\"" + Url.replace(/"/g, "\\\"") + "\")";
    }
    /**
     * Starts or stops the auto-cycling of images.
     * @param {boolean} Flag True starts auto-cycling; false stops it.
     * @returns {void}
     */
    DoAutoCycle(Flag) {
        if (!tp.IsNil(this.fAutoCycleId)) {
            clearInterval(this.fAutoCycleId);
            this.fAutoCycleId = null;
        }
        if (Flag === true && this.ImageCount > 1) {
            this.fAutoCycleId = setInterval(function (Self) {
                Self.SelectNext();
            }, this.AutoCycleMSecs, this);
        }
    }
    /**
     * Updates the previous and next button display.
     * @returns {void}
     */
    DisplayCycleButtonsChanged() {
        if (tp.IsHTMLElement(this.fNext) && tp.IsHTMLElement(this.fPrev)) {
            this.fNext.style.display = this.DisplayCycleButtons ? "" : "none";
            this.fPrev.style.display = this.DisplayCycleButtons ? "" : "none";
        }
    }
    /**
     * Handles mouse-enter events.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleMouseEnter(e) {
        if (this.AutoCycle && this.PauseOnHover)
            this.DoAutoCycle(false);
    }
    /**
     * Handles mouse-leave events.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleMouseLeave(e) {
        if (this.AutoCycle && this.PauseOnHover)
            this.DoAutoCycle(true);
    }
    /**
     * Handles click events.
     * @param {MouseEvent} e The mouse event.
     * @returns {void}
     */
    HandleClick(e) {
        if (this.ImageCount <= 1)
            return;
        if (e.target === this.fPrev)
            this.SelectPrevious();
        else if (e.target === this.fNext || this.ChangeOnClick === true)
            this.SelectNext();
    }
    /**
     * Event trigger called after SelectedIndex changes.
     * @param {number} CurrentIndex The previous selected index.
     * @param {number} NewIndex The new selected index.
     * @returns {tp.EventArgs|null} Returns event arguments or null.
     */
    OnSelectedIndexChanged(CurrentIndex, NewIndex) {
        return this.Trigger("SelectedIndexChanged", { CurrentIndex: CurrentIndex, NewIndex: NewIndex });
    }

    // ● public
    /**
     * Selects the next image.
     * @returns {void}
     */
    SelectNext() {
        var Index;
        if (this.ImageCount > 0) {
            Index = this.SelectedIndex + 1;
            if (Index > this.ImageCount - 1)
                Index = 0;
            this.SelectedIndex = Index;
        }
    }
    /**
     * Selects the previous image.
     * @returns {void}
     */
    SelectPrevious() {
        var Index;
        if (this.ImageCount > 0) {
            Index = this.SelectedIndex - 1;
            if (Index < 0)
                Index = this.ImageCount - 1;
            this.SelectedIndex = Index;
        }
    }
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        this.DoAutoCycle(false);
        if (this.HasHandle) {
            this.Handle.removeEventListener("mouseenter", this.fMouseEnterHandler);
            this.Handle.removeEventListener("mouseleave", this.fMouseLeaveHandler);
            this.Handle.removeEventListener("click", this.fClickHandler);
        }
        this.fNext = null;
        this.fPrev = null;
        this.fMouseEnterHandler = null;
        this.fMouseLeaveHandler = null;
        this.fClickHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Returns the number of images.
     * @returns {number} Returns the image count.
     */
    get ImageCount() {
        return this.Images.length;
    }
    /**
     * Gets or sets the image URL array.
     * @returns {string[]} Returns the image URL array.
     */
    get Images() {
        if (!tp.IsArray(this.fImages))
            this.fImages = [];
        return this.fImages;
    }
    /**
     * Gets or sets the image URL array.
     * @param {string[]} Value The image URL array.
     * @returns {void}
     */
    set Images(Value) {
        if (tp.IsArray(Value)) {
            this.fImages = Value.slice();
            if (this.SelectedIndex >= this.ImageCount)
                this.fSelectedIndex = -1;
            if (this.ImageCount > 0 && this.SelectedIndex < 0)
                this.SelectedIndex = 0;
        }
    }
    /**
     * Gets or sets the selected image index.
     * @returns {number} Returns the selected image index.
     */
    get SelectedIndex() {
        return this.fSelectedIndex;
    }
    /**
     * Gets or sets the selected image index.
     * @param {number} Value The selected image index.
     * @returns {void}
     */
    set SelectedIndex(Value) {
        var CurrentIndex = this.fSelectedIndex;
        var NewIndex = tp.ToInt(Value);
        var ImageUrl;
        if (NewIndex >= 0 && NewIndex < this.ImageCount && NewIndex !== CurrentIndex) {
            this.fSelectedIndex = NewIndex;
            ImageUrl = this.Images[NewIndex];
            this.Handle.style.backgroundImage = this.GetBackgroundImage(ImageUrl);
            this.OnSelectedIndexChanged(CurrentIndex, NewIndex);
        }
    }
    /**
     * Gets or sets the image size mode.
     * @returns {number} Returns a tp.ImageSizeMode value.
     */
    get ImageMode() {
        var Value = tp.StyleProp(this.Handle, "backgroundSize");
        if (Value === "cover")
            return tp.ImageSizeMode.Crop;
        if (Value === "contain")
            return tp.ImageSizeMode.Scale;
        if (Value === "100% 100%")
            return tp.ImageSizeMode.Stretch;
        return tp.ImageSizeMode.Unknown;
    }
    /**
     * Gets or sets the image size mode.
     * @param {number|string} Value The tp.ImageSizeMode value or enum name.
     * @returns {void}
     */
    set ImageMode(Value) {
        if (tp.IsString(Value)) {
            if (tp.IsSameText(Value, "Crop"))
                Value = tp.ImageSizeMode.Crop;
            else if (tp.IsSameText(Value, "Scale"))
                Value = tp.ImageSizeMode.Scale;
            else if (tp.IsSameText(Value, "Stretch"))
                Value = tp.ImageSizeMode.Stretch;
            else
                Value = tp.ImageSizeMode.Unknown;
        }
        if (Value === tp.ImageSizeMode.Crop)
            this.Handle.style.backgroundSize = "cover";
        else if (Value === tp.ImageSizeMode.Scale)
            this.Handle.style.backgroundSize = "contain";
        else if (Value === tp.ImageSizeMode.Stretch)
            this.Handle.style.backgroundSize = "100% 100%";
    }
    /**
     * Gets or sets a value indicating whether images auto-cycle.
     * @returns {boolean} Returns true when auto-cycle is enabled.
     */
    get AutoCycle() {
        return this.fAutoCycle === true;
    }
    /**
     * Gets or sets a value indicating whether images auto-cycle.
     * @param {boolean} Value True to enable auto-cycle.
     * @returns {void}
     */
    set AutoCycle(Value) {
        Value = Value === true;
        if (Value !== this.AutoCycle) {
            this.fAutoCycle = Value;
            this.DoAutoCycle(Value);
        }
    }
    /**
     * Gets or sets the auto-cycle interval in milliseconds.
     * @returns {number} Returns the auto-cycle interval.
     */
    get AutoCycleMSecs() {
        return this.fAutoCycleInterval;
    }
    /**
     * Gets or sets the auto-cycle interval in milliseconds.
     * @param {number} Value The auto-cycle interval.
     * @returns {void}
     */
    set AutoCycleMSecs(Value) {
        Value = Math.max(100, tp.ToInt(Value));
        if (Value !== this.fAutoCycleInterval) {
            this.fAutoCycleInterval = Value;
            if (this.AutoCycle) {
                this.DoAutoCycle(false);
                this.DoAutoCycle(true);
            }
        }
    }
    /**
     * Gets or sets whether previous and next buttons are visible.
     * @returns {boolean} Returns true when cycle buttons are visible.
     */
    get DisplayCycleButtons() {
        return this.fDisplayCycleButtons === true;
    }
    /**
     * Gets or sets whether previous and next buttons are visible.
     * @param {boolean} Value True to display cycle buttons.
     * @returns {void}
     */
    set DisplayCycleButtons(Value) {
        Value = Value === true;
        if (Value !== this.DisplayCycleButtons) {
            this.fDisplayCycleButtons = Value;
            this.DisplayCycleButtonsChanged();
        }
    }
};

// ● prototype
/**
 * Gets or sets whether auto-cycle pauses while the mouse is over the control.
 * @type {boolean}
 */
tp.ImageSlider.prototype.PauseOnHover = true;
/**
 * Gets or sets whether clicking the image selects the next image.
 * @type {boolean}
 */
tp.ImageSlider.prototype.ChangeOnClick = true;
/**
 * Private field.
 * @type {string[]}
 */
tp.ImageSlider.prototype.fImages = [];
/**
 * Private field.
 * @type {number}
 */
tp.ImageSlider.prototype.fSelectedIndex = -1;
/**
 * Private field.
 * @type {boolean}
 */
tp.ImageSlider.prototype.fAutoCycle = true;
/**
 * Private field.
 * @type {number|null}
 */
tp.ImageSlider.prototype.fAutoCycleId = null;
/**
 * Private field.
 * @type {number}
 */
tp.ImageSlider.prototype.fAutoCycleInterval = 6000;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ImageSlider.prototype.fNext = null;
/**
 * Private field.
 * @type {HTMLElement|null}
 */
tp.ImageSlider.prototype.fPrev = null;
/**
 * Private field.
 * @type {boolean}
 */
tp.ImageSlider.prototype.fDisplayCycleButtons = true;
/**
 * Private field.
 * @type {Function|null}
 */
tp.ImageSlider.prototype.fMouseEnterHandler = null;
/**
 * Private field.
 * @type {Function|null}
 */
tp.ImageSlider.prototype.fMouseLeaveHandler = null;
/**
 * Private field.
 * @type {Function|null}
 */
tp.ImageSlider.prototype.fClickHandler = null;
