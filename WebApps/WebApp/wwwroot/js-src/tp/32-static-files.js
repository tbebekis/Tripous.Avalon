// ● static files
/**
 * Loads and unloads JavaScript and CSS files dynamically in the document head.
 * Dynamically loaded files are reference counted and removed when the counter reaches zero.
 * @type {object}
 */
tp.StaticFiles = {
    /**
     * Gets the dynamically loaded JavaScript files.
     * @type {object[]}
     */
    JavascriptFiles: [],
    /**
     * Gets the dynamically loaded CSS files.
     * @type {object[]}
     */
    CssFiles: [],

    // ● protected
    /**
     * Normalizes a file URL for lookups.
     * @param {string} Url The file URL.
     * @returns {string} Returns the normalized URL.
     */
    NormalizeUrl: function (Url) {
        return tp.IsString(Url) ? Url.toLowerCase() : "";
    },
    /**
     * Gets the current document head element.
     * @returns {HTMLHeadElement|null} Returns the document head element or null.
     */
    GetHead: function () {
        if (typeof document === "undefined")
            return null;
        return document.head || tp.Select("head");
    },
    /**
     * Finds a loaded file registration by URL.
     * @param {object[]} List The registration list.
     * @param {string} FileUrl The normalized file URL.
     * @returns {object|null} Returns the registration or null.
     */
    FindFile: function (List, FileUrl) {
        var Index;
        for (Index = 0; Index < List.length; Index++) {
            if (List[Index].FileUrl === FileUrl)
                return List[Index];
        }
        return null;
    },

    // ● public
    /**
     * Loads a JavaScript file dynamically.
     * @param {string} Url The file URL.
     * @returns {Promise<void>} Returns a promise resolved when the file is loaded.
     */
    LoadJavascriptFile: async function (Url) {
        var FileUrl = tp.StaticFiles.NormalizeUrl(Url);
        var File = tp.StaticFiles.FindFile(tp.StaticFiles.JavascriptFiles, FileUrl);
        var Head;
        var Element;
        if (tp.IsBlank(FileUrl))
            return Promise.resolve();
        if (tp.IsValid(File)) {
            File.Counter += 1;
            return Promise.resolve();
        }
        Head = tp.StaticFiles.GetHead();
        if (!Head)
            return Promise.reject(new Error("Document head element not found."));
        Element = document.createElement("script");
        Element.src = Url;
        return new Promise(function (Resolve, Reject) {
            Element.onload = function () {
                tp.StaticFiles.JavascriptFiles.push({
                    FileUrl: FileUrl,
                    Counter: 1,
                    Element: Element
                });
                Resolve();
            };
            Element.onerror = function (e) {
                Reject(e);
            };
            Head.appendChild(Element);
        });
    },
    /**
     * Unloads a dynamically loaded JavaScript file.
     * @param {string} Url The file URL.
     * @returns {void}
     */
    UnLoadJavascriptFile: function (Url) {
        var FileUrl = tp.StaticFiles.NormalizeUrl(Url);
        var File = tp.StaticFiles.FindFile(tp.StaticFiles.JavascriptFiles, FileUrl);
        if (tp.IsValid(File)) {
            File.Counter -= 1;
            if (File.Counter <= 0) {
                if (File.Element.parentNode)
                    File.Element.parentNode.removeChild(File.Element);
                tp.ListRemove(tp.StaticFiles.JavascriptFiles, File);
            }
        }
    },
    /**
     * Loads a list of JavaScript files dynamically.
     * @param {string[]} UrlList The file URL list.
     * @returns {Promise<void>} Returns a promise resolved when all files are loaded.
     */
    LoadJavascriptFiles: async function (UrlList) {
        var Index;
        if (tp.IsArray(UrlList)) {
            for (Index = 0; Index < UrlList.length; Index++)
                await tp.StaticFiles.LoadJavascriptFile(UrlList[Index]);
        }
    },
    /**
     * Unloads a list of dynamically loaded JavaScript files.
     * @param {string[]} UrlList The file URL list.
     * @returns {void}
     */
    UnLoadJavascriptFiles: function (UrlList) {
        var Index;
        if (tp.IsArray(UrlList)) {
            for (Index = 0; Index < UrlList.length; Index++)
                tp.StaticFiles.UnLoadJavascriptFile(UrlList[Index]);
        }
    },
    /**
     * Loads a CSS file dynamically.
     * @param {string} Url The file URL.
     * @returns {Promise<void>} Returns a promise resolved when the file is loaded.
     */
    LoadCssFile: async function (Url) {
        var FileUrl = tp.StaticFiles.NormalizeUrl(Url);
        var File = tp.StaticFiles.FindFile(tp.StaticFiles.CssFiles, FileUrl);
        var Head;
        var Element;
        if (tp.IsBlank(FileUrl))
            return Promise.resolve();
        if (tp.IsValid(File)) {
            File.Counter += 1;
            return Promise.resolve();
        }
        Head = tp.StaticFiles.GetHead();
        if (!Head)
            return Promise.reject(new Error("Document head element not found."));
        Element = document.createElement("link");
        Element.href = Url;
        Element.rel = "stylesheet";
        Element.type = "text/css";
        return new Promise(function (Resolve, Reject) {
            Element.onload = function () {
                tp.StaticFiles.CssFiles.push({
                    FileUrl: FileUrl,
                    Counter: 1,
                    Element: Element
                });
                Resolve();
            };
            Element.onerror = function (e) {
                Reject(e);
            };
            Head.appendChild(Element);
        });
    },
    /**
     * Unloads a dynamically loaded CSS file.
     * @param {string} Url The file URL.
     * @returns {void}
     */
    UnLoadCssFile: function (Url) {
        var FileUrl = tp.StaticFiles.NormalizeUrl(Url);
        var File = tp.StaticFiles.FindFile(tp.StaticFiles.CssFiles, FileUrl);
        if (tp.IsValid(File)) {
            File.Counter -= 1;
            if (File.Counter <= 0) {
                if (File.Element.parentNode)
                    File.Element.parentNode.removeChild(File.Element);
                tp.ListRemove(tp.StaticFiles.CssFiles, File);
            }
        }
    },
    /**
     * Loads a list of CSS files dynamically.
     * @param {string[]} UrlList The file URL list.
     * @returns {Promise<void>} Returns a promise resolved when all files are loaded.
     */
    LoadCssFiles: async function (UrlList) {
        var Index;
        if (tp.IsArray(UrlList)) {
            for (Index = 0; Index < UrlList.length; Index++)
                await tp.StaticFiles.LoadCssFile(UrlList[Index]);
        }
    },
    /**
     * Unloads a list of dynamically loaded CSS files.
     * @param {string[]} UrlList The file URL list.
     * @returns {void}
     */
    UnLoadCssFiles: function (UrlList) {
        var Index;
        if (tp.IsArray(UrlList)) {
            for (Index = 0; Index < UrlList.length; Index++)
                tp.StaticFiles.UnLoadCssFile(UrlList[Index]);
        }
    }
};
