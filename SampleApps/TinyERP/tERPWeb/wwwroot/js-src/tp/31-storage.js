// ● browser storage
/**
 * Base class for browser storage helpers.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/Web_Storage_API|MDN Web Storage API}
 */
tp.BrowserStorage = class {
    // ● protected
    /**
     * Gets the browser storage object, if available.
     * @returns {Storage|null} Returns the browser storage object, if available; otherwise, null.
     * @protected
     */
    static get Storage() {
        return null;
    }

    // ● properties
    /**
     * Returns true when the storage object is available.
     * @returns {boolean} Returns true when the storage object is available.
     */
    static get Available() {
        return this.Storage !== null;
    }

    // ● public
    /**
     * Clears all entries in the storage object for this origin.
     * @returns {void}
     */
    static Clear() {
        var Storage = this.Storage;
        if (Storage)
            Storage.clear();
    }
    /**
     * Removes an entry from the storage object.
     * @param {string} Key The entry key.
     * @returns {void}
     */
    static Remove(Key) {
        var Storage = this.Storage;
        if (Storage)
            Storage.removeItem(Key);
    }
    /**
     * Gets a string value from the storage object.
     * @param {string} Key The entry key.
     * @param {string|null|undefined} Default The default value to return when the key is not found.
     * @returns {string|null|undefined} Returns the stored value or the default value.
     */
    static Get(Key, Default) {
        var Storage = this.Storage;
        var Result = Storage ? Storage.getItem(Key) : null;
        return tp.IsBlank(Result) ? Default : Result;
    }
    /**
     * Sets a string value in the storage object.
     * @param {string} Key The entry key.
     * @param {*} Value The value to store.
     * @returns {void}
     */
    static Set(Key, Value) {
        var Storage = this.Storage;
        if (Storage)
            Storage.setItem(Key, tp.IsNil(Value) ? "" : String(Value));
    }
    /**
     * Gets an object value from the storage object.
     * @param {string} Key The entry key.
     * @param {object|null|undefined} Default The default value to return when the key is not found or the stored JSON is invalid.
     * @returns {object|null|undefined} Returns the stored object or the default value.
     */
    static GetObject(Key, Default) {
        var Text = this.Get(Key, null);
        if (!tp.IsString(Text))
            return Default;
        try {
            return JSON.parse(Text);
        } catch (e) {
            return Default;
        }
    }
    /**
     * Sets an object value in the storage object.
     * @param {string} Key The entry key.
     * @param {object|null|undefined} Value The value to store.
     * @returns {void}
     */
    static SetObject(Key, Value) {
        if (!tp.IsNil(Value))
            this.Set(Key, JSON.stringify(Value));
    }
};

// ● local storage
/**
 * Provides access to browser local storage.
 * Data stored in localStorage has no expiration time.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/Window/localStorage|MDN localStorage}
 */
tp.Local = class extends tp.BrowserStorage {
    // ● protected
    /**
     * Gets the browser localStorage object, if available.
     * @returns {Storage|null} Returns the browser localStorage object, if available; otherwise, null.
     * @protected
     */
    static get Storage() {
        try {
            if (typeof window === "undefined" || !window.localStorage)
                return null;
            return window.localStorage;
        } catch (e) {
            return null;
        }
    }
};

// ● session storage
/**
 * Provides access to browser session storage.
 * Data stored in sessionStorage is kept for the duration of the page session.
 * @see {@link https://developer.mozilla.org/en-US/docs/Web/API/Window/sessionStorage|MDN sessionStorage}
 */
tp.Session = class extends tp.BrowserStorage {
    // ● protected
    /**
     * Gets the browser sessionStorage object, if available.
     * @returns {Storage|null} Returns the browser sessionStorage object, if available; otherwise, null.
     * @protected
     */
    static get Storage() {
        try {
            if (typeof window === "undefined" || !window.sessionStorage)
                return null;
            return window.sessionStorage;
        } catch (e) {
            return null;
        }
    }
};
