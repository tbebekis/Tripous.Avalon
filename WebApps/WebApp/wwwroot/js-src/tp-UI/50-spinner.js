// ● spinner
/**
 * Default spinner visual implementation.
 */
tp.DefaultSpinner = class {
    // ● constructor
    /**
     * Creates a default spinner implementation.
     */
    constructor() {
        this.Overlay = null;
        this.Container = null;
        this.Spinner = null;
    }

    // ● public
    /**
     * Shows the spinner.
     * @returns {void}
     */
    Show() {
        if (!this.Overlay)
            this.Overlay = new tp.ScreenOverlay();
        this.Overlay.Show();
        if (!this.Container) {
            this.Container = this.Overlay.Document.createElement("div");
            this.Container.className = tp.Classes.SpinnerContainer;
            this.Overlay.Handle.appendChild(this.Container);
        }
        if (!this.Spinner) {
            this.Spinner = this.Overlay.Document.createElement("div");
            this.Spinner.className = tp.Classes.Spinner;
            this.Container.appendChild(this.Spinner);
        }
    }
    /**
     * Hides and destroys the spinner.
     * @returns {void}
     */
    Hide() {
        this.Dispose();
    }
    /**
     * Disposes the spinner.
     * @returns {void}
     */
    Dispose() {
        if (this.Overlay)
            this.Overlay.Dispose();
        this.Overlay = null;
        this.Container = null;
        this.Spinner = null;
    }
};

// ● prototype
/**
 * Overlay instance.
 * @type {tp.ScreenOverlay}
 */
tp.DefaultSpinner.prototype.Overlay = null;
/**
 * Spinner container element.
 * @type {HTMLElement}
 */
tp.DefaultSpinner.prototype.Container = null;
/**
 * Spinner element.
 * @type {HTMLElement}
 */
tp.DefaultSpinner.prototype.Spinner = null;

// ● spinner service
/**
 * Global spinner service.
 * @type {object}
 */
tp.Spinner = {
    fCounter: 0,
    fImplementation: null,
    fInstance: null,
    /**
     * Shows or hides the spinner.
     * Multiple show calls are reference-counted.
     * @param {boolean} Flag True to show; false to hide.
     * @returns {void}
     */
    Show: function (Flag) {
        if (Flag === true)
            tp.Spinner.DoShow();
        else
            tp.Spinner.DoHide();
    },
    /**
     * Forces the spinner to hide and resets the reference counter.
     * @returns {void}
     */
    ForceHide: function () {
        if (tp.Spinner.fInstance && tp.IsFunction(tp.Spinner.fInstance.Hide))
            tp.Spinner.fInstance.Hide();
        tp.Spinner.fCounter = 0;
        tp.Spinner.fInstance = null;
    },
    /**
     * Sets the spinner implementation.
     * The implementation must provide Show(), Hide(), and Dispose().
     * @param {object|null|undefined} Implementation The spinner implementation.
     * @returns {void}
     */
    SetSpinnerImplementation: function (Implementation) {
        if (tp.IsNil(Implementation)) {
            tp.Spinner.fImplementation = null;
            return;
        }
        if (!tp.Spinner.IsValidImplementation(Implementation))
            tp.Throw("Spinner implementation must provide Show(), Hide(), and Dispose().");
        tp.Spinner.fImplementation = Implementation;
    },
    /**
     * Returns true while the spinner is visible.
     * @returns {boolean} Returns true while the spinner is visible.
     */
    get IsShowing() {
        return tp.Spinner.fCounter > 0 && !tp.IsNil(tp.Spinner.fInstance);
    },
    /**
     * Gets the spinner show counter.
     * @returns {number} Returns the show counter.
     */
    get ShowingCounter() {
        return tp.Spinner.fCounter;
    },
    /**
     * Shows the active spinner implementation.
     * @protected
     * @returns {void}
     */
    DoShow: function () {
        tp.Spinner.fCounter++;
        if (tp.Spinner.fCounter === 1) {
            tp.Spinner.fInstance = tp.Spinner.GetImplementation();
            tp.Spinner.fInstance.Show();
        }
    },
    /**
     * Hides the active spinner implementation.
     * @protected
     * @returns {void}
     */
    DoHide: function () {
        if (tp.Spinner.fCounter > 0)
            tp.Spinner.fCounter--;
        if (tp.Spinner.fCounter === 0 && tp.Spinner.fInstance) {
            tp.Spinner.fInstance.Hide();
            tp.Spinner.fInstance = null;
        }
    },
    /**
     * Returns the spinner implementation to use.
     * @returns {object} Returns the spinner implementation.
     */
    GetImplementation: function () {
        if (tp.Spinner.IsValidImplementation(tp.Spinner.fImplementation))
            return tp.Spinner.fImplementation;
        return new tp.DefaultSpinner();
    },
    /**
     * Returns true when an implementation provides the spinner contract.
     * @param {object|null|undefined} Implementation The implementation to check.
     * @returns {boolean} Returns true when the implementation is valid.
     */
    IsValidImplementation: function (Implementation) {
        return !tp.IsNil(Implementation)
            && tp.IsFunction(Implementation.Show)
            && tp.IsFunction(Implementation.Hide)
            && tp.IsFunction(Implementation.Dispose);
    }
};

// ● helpers
/**
 * Shows or hides the global spinner.
 * @param {boolean} Flag True to show; false to hide.
 * @returns {void}
 */
tp.ShowSpinner = function (Flag) {
    tp.Spinner.Show(Flag);
};
/**
 * Forces the global spinner to hide.
 * @returns {void}
 */
tp.ForceHideSpinner = function () {
    tp.Spinner.ForceHide();
};
