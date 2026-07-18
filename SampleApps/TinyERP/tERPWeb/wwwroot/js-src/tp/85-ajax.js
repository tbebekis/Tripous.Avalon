// ● ajax urls
/**
 * Provides known Tripous URL endpoints.
 * @type {object}
 */
tp.Urls = tp.Urls || {};
/**
 * The WebDesk Ajax execute endpoint.
 * @type {string}
 */
tp.Urls.AjaxExecute = "/Ajax/Execute";

// ● ajax args
/**
 * Represents Ajax call arguments.
 * @property {string} Method The HTTP method to use.
 * @property {string} Url The URL to call.
 * @property {*} Data The data to send.
 * @property {boolean} UriEncodeData True to URI-encode non-string data.
 * @property {number} Timeout The timeout in milliseconds.
 * @property {string} ContentType The request content type.
 * @property {object|null} Context The callback context.
 * @property {string} AntiForgeryToken Anti-forgery token value.
 * @property {Function|null} OnSuccess Success callback.
 * @property {Function|null} OnFailure Failure callback.
 * @property {Function|null} OnRequestHeaders Request headers callback.
 * @property {Function|null} ResponseHandlerFunc Response handler callback.
 * @property {XMLHttpRequest|null} XHR The XMLHttpRequest instance.
 * @property {string} ErrorText Error text, if any.
 * @property {boolean} Result True when the Ajax call succeeded at HTTP level.
 * @property {object} ResponseData The response envelope from the server.
 * @property {*} Packet The parsed packet returned by the server.
 * @property {*} Tag User-defined value.
 */
tp.AjaxArgs = class {

    // ● constructor
    /**
     * Creates Ajax call arguments.
     * @param {object|tp.AjaxArgs|null|undefined} SourceArgs Optional source arguments.
     */
    constructor(SourceArgs = null) {
        var Prop;

        SourceArgs = SourceArgs || {};
        for (Prop in SourceArgs) {
            if (Object.prototype.propertyIsEnumerable.call(SourceArgs, Prop))
                this[Prop] = SourceArgs[Prop];
        }
    }

    // ● fields
    /**
     * The HTTP method to use.
     * @type {string}
     */
    Method = "POST";
    /**
     * The URL to call.
     * @type {string}
     */
    Url = "";
    /**
     * The data to send.
     * @type {*}
     */
    Data = null;
    /**
     * True to URI-encode non-string data.
     * @type {boolean}
     */
    UriEncodeData = true;
    /**
     * The timeout in milliseconds.
     * @type {number}
     */
    Timeout = 0;
    /**
     * The request content type.
     * @type {string}
     */
    ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
    /**
     * The callback context.
     * @type {object|null}
     */
    Context = null;
    /**
     * Anti-forgery token value.
     * @type {string}
     */
    AntiForgeryToken = "";
    /**
     * Success callback.
     * @type {Function|null}
     */
    OnSuccess = null;
    /**
     * Failure callback.
     * @type {Function|null}
     */
    OnFailure = null;
    /**
     * Request headers callback.
     * @type {Function|null}
     */
    OnRequestHeaders = tp.AjaxOnRequestHeadersDefaultHandler;
    /**
     * Response handler callback.
     * @type {Function|null}
     */
    ResponseHandlerFunc = tp.AjaxResponseDefaultHandler;
    /**
     * The XMLHttpRequest instance.
     * @type {XMLHttpRequest|null}
     */
    XHR = null;
    /**
     * Error text, if any.
     * @type {string}
     */
    ErrorText = "";
    /**
     * True when the Ajax call succeeded at HTTP level.
     * @type {boolean}
     */
    Result = false;
    /**
     * The response envelope from the server.
     * @type {object}
     */
    ResponseData = {
        IsSuccess: false,
        ErrorText: "",
        Packet: {}
    };
    /**
     * The parsed packet returned by the server.
     * @type {*}
     */
    Packet = null;
    /**
     * User-defined value.
     * @type {*}
     */
    Tag = null;

    // ● properties
    /**
     * Returns true when this is a POST request.
     * @returns {boolean} Returns true when this is a POST request.
     */
    get IsPost() {
        return tp.IsSameText("post", this.Method);
    }
    /**
     * Returns true when this is a GET request.
     * @returns {boolean} Returns true when this is a GET request.
     */
    get IsGet() {
        return !this.IsPost;
    }
    /**
     * Gets the response text.
     * @returns {string} Returns the response text.
     */
    get ResponseText() {
        return this.XHR ? this.XHR.responseText : "";
    }

    // ● public
    /**
     * Returns a string representation of this instance.
     * @returns {string} Returns a string representation of this instance.
     */
    toString() {
        return "Method: \"" + this.Method + "\"\n"
            + "Url: \"" + this.Url + "\"\n"
            + "AjaxResult: \"" + this.Result + "\"\n"
            + "ErrorText: \"" + this.ErrorText + "\"\n"
            + "ResponseText: \"" + this.ResponseText + "\"\n"
            + "ResponseResult: \"" + this.ResponseData.IsSuccess + "\"\n"
            + "ResponseErrorText: \"" + this.ResponseData.ErrorText + "\"";
    }
};

// ● ajax response
/**
 * Handles an Ajax response.
 * @param {tp.AjaxArgs} Args The Ajax call arguments.
 * @returns {void}
 */
tp.AjaxResponseDefaultHandler = function (Args) {
    var Data;
    var JsonResult;
    var ErrorText = function (Text) {
        return tp.IsString(Text) && !tp.IsBlank(Text) ? Text : "Unknown error";
    };

    if (Args.Result !== true)
        tp.Throw("Ajax network error: " + ErrorText(Args.ErrorText));

    Data = JSON.parse(Args.ResponseText);
    Args.ResponseData = Data;

    if (!tp.IsEmpty(Data) && Data.IsSuccess !== true)
        tp.Throw("Ajax operation error: " + ErrorText(Data.ErrorText));

    if (tp.IsValid(Args.ResponseData)) {
        if (tp.IsString(Args.ResponseData.Packet) && !tp.IsBlank(Args.ResponseData.Packet)) {
            JsonResult = tp.TryParseJson(Args.ResponseData.Packet);
            if (JsonResult.Result === true) {
                Args.ResponseData.Packet = JsonResult.Value;
                Args.Packet = JsonResult.Value;
            }
        } else if (tp.IsValid(Args.ResponseData.Packet)) {
            Args.Packet = Args.ResponseData.Packet;
        }
    }
};
/**
 * Called before Ajax request headers are sent.
 * @param {tp.AjaxArgs} Args The Ajax call arguments.
 * @returns {void}
 */
tp.AjaxOnRequestHeadersDefaultHandler = function (Args) {
};

// ● ajax
/**
 * Executes Ajax requests.
 * @param {tp.AjaxArgs} Args The Ajax call arguments.
 * @returns {void}
 */
tp.Ajax = function (Args) {
    var Context = Args.Context;
    var Data = Args.Data;
    var Url = encodeURI(Args.Url.toLowerCase());
    var Async = true;
    var XHR;
    var Succeeded = function (Request) {
        return !tp.IsEmpty(Request) && (Request.status === 0 || Request.status >= 200 && Request.status < 300 || Request.status === 304 || Request.status === 1223);
    };
    var OnError = function (e) {
        var List = ["Ajax call failed. Url: " + Url];

        Args.ErrorText = "";

        if (tp.IsEmpty(e))
            List.push("Status Text: " + XHR.statusText);
        else if (e instanceof ProgressEvent)
            List.push("Ajax call failed because of a failure on the network level");
        else
            List.push("Error Text: " + tp.ExceptionText(e));

        Args.ErrorText = List.join("\n");

        if (tp.IsFunction(Args.OnFailure))
            tp.Call(Args.OnFailure, Context, Args);
        else
            tp.Throw(Args.ErrorText);
    };

    if (!tp.IsEmpty(Data) && !tp.IsString(Data) && Args.UriEncodeData === true)
        Data = tp.EncodeArgs(Data);

    if (Args.IsGet && !tp.IsEmpty(Data))
        Url += "?" + Data;

    XHR = new XMLHttpRequest();
    Args.XHR = XHR;

    XHR.onload = function (e) {
        if (XHR.readyState === XMLHttpRequest.DONE) {
            if (Succeeded(XHR)) {
                try {
                    Args.Result = true;
                    tp.Call(Args.ResponseHandlerFunc, null, Args);
                    tp.Call(Args.OnSuccess, Context, Args);
                } catch (Error) {
                    OnError(Error);
                }
            } else {
                OnError(e);
            }
        }
    };
    XHR.onerror = function (e) {
        OnError(e);
    };

    try {
        XHR.open(Args.Method, Url, Async);

        if (Async)
            XHR.timeout = Args.Timeout;

        XHR.setRequestHeader("Content-Type", Args.ContentType);
        XHR.setRequestHeader("Accept", "*/*");

        if (!tp.IsBlank(Args.AntiForgeryToken))
            XHR.setRequestHeader("__RequestVerificationToken", Args.AntiForgeryToken);

        tp.Call(Args.OnRequestHeaders, Context, Args);

        Data = Args.IsPost ? Data : null;
        XHR.send(Data);
    } catch (e) {
        OnError(e);
    }
};
/**
 * Executes an Ajax request inside a Promise.
 * @param {tp.AjaxArgs} Args The Ajax call arguments.
 * @returns {Promise<tp.AjaxArgs>} Returns a Promise.
 */
tp.Ajax.Async = async function (Args) {
    var Context = Args.Context || null;
    var OnSuccess = Args.OnSuccess || null;
    var OnFailure = Args.OnFailure || null;
    var GetRejectText = function (InnerArgs) {
        if (InnerArgs instanceof tp.AjaxArgs
            && !tp.IsEmpty(InnerArgs.ResponseData)
            && !tp.IsBlank(InnerArgs.ResponseData.ErrorText))
            return InnerArgs.ResponseData.ErrorText;
        return tp.ExceptionText(InnerArgs);
    };

    return new Promise(function (Resolve, Reject) {
        Args.Context = null;
        Args.OnSuccess = function (InnerArgs) {
            if (tp.IsFunction(OnSuccess)) {
                tp.Call(OnSuccess, Context, InnerArgs);

                if (!tp.IsEmpty(InnerArgs.ResponseData) && InnerArgs.ResponseData.IsSuccess === false && !tp.IsBlank(InnerArgs.ResponseData.ErrorText))
                    Reject(GetRejectText(InnerArgs));
                else
                    Resolve(InnerArgs);
            } else {
                Resolve(InnerArgs);
            }
        };
        Args.OnFailure = function (InnerArgs) {
            if (tp.IsFunction(OnFailure))
                tp.Call(OnFailure, Context, InnerArgs);
            Reject(GetRejectText(InnerArgs));
        };

        try {
            tp.Ajax(Args);
        } catch (e) {
            Reject(tp.ExceptionText(e));
        }
    });
};
/**
 * Executes a GET Ajax request.
 * @param {string} Url The URL to call.
 * @param {object|null|undefined} Data Optional data to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {void}
 */
tp.Ajax.Get = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    tp.Ajax(tp.Ajax.GetArgs(Url, Data, OnSuccess, OnFailure, Context));
};
/**
 * Executes a POST Ajax request.
 * @param {string} Url The URL to call.
 * @param {object|null|undefined} Data Optional data to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {void}
 */
tp.Ajax.Post = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    tp.Ajax(tp.Ajax.PostArgs(Url, Data, OnSuccess, OnFailure, Context));
};
/**
 * Executes a POST Ajax request with a JSON model.
 * @param {string} Url The URL to call.
 * @param {object|string} Model The model to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {void}
 */
tp.Ajax.PostModel = function (Url, Model, OnSuccess = null, OnFailure = null, Context = null) {
    tp.Ajax(tp.Ajax.ModelArgs(Url, Model, OnSuccess, OnFailure, Context));
};
/**
 * Executes a GET Ajax request inside a Promise.
 * @param {string} Url The URL to call.
 * @param {object|null|undefined} Data Optional data to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {Promise<tp.AjaxArgs>} Returns a Promise.
 */
tp.Ajax.GetAsync = async function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    return tp.Ajax.Async(tp.Ajax.GetArgs(Url, Data, OnSuccess, OnFailure, Context));
};
/**
 * Executes a POST Ajax request inside a Promise.
 * @param {string} Url The URL to call.
 * @param {object|null|undefined} Data Optional data to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {Promise<tp.AjaxArgs>} Returns a Promise.
 */
tp.Ajax.PostAsync = async function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    return tp.Ajax.Async(tp.Ajax.PostArgs(Url, Data, OnSuccess, OnFailure, Context));
};
/**
 * Executes a POST Ajax request with a JSON model inside a Promise.
 * @param {string} Url The URL to call.
 * @param {object|string} Model The model to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {Promise<tp.AjaxArgs>} Returns a Promise.
 */
tp.Ajax.PostModelAsync = async function (Url, Model, OnSuccess = null, OnFailure = null, Context = null) {
    return tp.Ajax.Async(tp.Ajax.ModelArgs(Url, Model, OnSuccess, OnFailure, Context));
};
/**
 * Executes a POST Ajax request with a container model inside a Promise.
 * @param {Element|string} ElementOrSelector The container element or selector.
 * @param {string|null|undefined} Url The URL to call. When empty and the element is a form, the form action is used.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {Promise<tp.AjaxArgs>} Returns a Promise.
 */
tp.Ajax.PostContainerAsync = async function (ElementOrSelector, Url, OnSuccess = null, OnFailure = null, Context = null) {
    var Element = tp(ElementOrSelector);

    if (tp.IsBlank(Url) && Element instanceof HTMLFormElement)
        Url = Element.action;

    return tp.ContainerToModelAsync(true, Element)
        .then(function (Model) {
            return tp.Ajax.PostModelAsync(Url, Model, OnSuccess, OnFailure, Context);
        });
};
/**
 * Executes a list of Ajax calls simultaneously.
 * @param {boolean} ShowSpinner True to show the global spinner while processing.
 * @param {tp.AjaxArgs[]} ArgsList The Ajax argument list.
 * @returns {Promise<Array>} Returns a Promise.
 */
tp.Ajax.AllAsync = async function (ShowSpinner, ArgsList) {
    return tp.Async.All(ShowSpinner, ArgsList, tp.Ajax.Async);
};
/**
 * Executes a list of Ajax calls sequentially.
 * @param {boolean} ShowSpinner True to show the global spinner while processing.
 * @param {tp.AjaxArgs[]} ArgsList The Ajax argument list.
 * @param {Function|null|undefined} BreakFunc Optional function that can stop the chain.
 * @returns {Promise<*>} Returns a Promise.
 */
tp.Ajax.ChainAsync = async function (ShowSpinner, ArgsList, BreakFunc = null) {
    return tp.Async.Chain(ShowSpinner, ArgsList, tp.Ajax.Async, BreakFunc);
};
/**
 * Creates Ajax arguments for a POST request.
 * @param {string} Url The URL to call.
 * @param {object|null|undefined} Data Optional data to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {tp.AjaxArgs} Returns Ajax arguments.
 */
tp.Ajax.PostArgs = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    return new tp.AjaxArgs({
        Url: Url,
        Method: "POST",
        Data: Data,
        Context: Context,
        OnSuccess: OnSuccess,
        OnFailure: OnFailure
    });
};
/**
 * Creates Ajax arguments for a GET request.
 * @param {string} Url The URL to call.
 * @param {object|null|undefined} Data Optional data to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {tp.AjaxArgs} Returns Ajax arguments.
 */
tp.Ajax.GetArgs = function (Url, Data = null, OnSuccess = null, OnFailure = null, Context = null) {
    return new tp.AjaxArgs({
        Url: Url,
        Method: "GET",
        Data: Data,
        Context: Context,
        OnSuccess: OnSuccess,
        OnFailure: OnFailure
    });
};
/**
 * Creates Ajax arguments for posting a JSON model.
 * @param {string} Url The URL to call.
 * @param {object|string} Model The model to send.
 * @param {Function|null|undefined} OnSuccess Optional success callback.
 * @param {Function|null|undefined} OnFailure Optional failure callback.
 * @param {object|null|undefined} Context Optional callback context.
 * @returns {tp.AjaxArgs} Returns Ajax arguments.
 */
tp.Ajax.ModelArgs = function (Url, Model, OnSuccess = null, OnFailure = null, Context = null) {
    var Args = new tp.AjaxArgs({
        Url: Url,
        Method: "POST",
        ContentType: "application/json; charset=utf-8",
        Context: Context,
        OnSuccess: OnSuccess,
        OnFailure: OnFailure
    });

    if (!tp.IsEmpty(Model)) {
        if (tp.IsObject(Model) && "__RequestVerificationToken" in Model) {
            Args.AntiForgeryToken = Model.__RequestVerificationToken;
            delete Model.__RequestVerificationToken;
        }

        Args.Data = tp.IsString(Model) ? Model : JSON.stringify(Model);
    }

    return Args;
};
/**
 * Merges extra data into Ajax arguments.
 * @param {tp.AjaxArgs} Args The Ajax arguments.
 * @param {object|null|undefined} ExtraData Extra data to merge.
 * @returns {tp.AjaxArgs} Returns Ajax arguments.
 */
tp.Ajax.AddExtraData = function (Args, ExtraData) {
    var Prop;
    Args.Data = Args.Data || {};

    if (!tp.IsEmpty(ExtraData)) {
        for (Prop in ExtraData) {
            if (Object.prototype.propertyIsEnumerable.call(ExtraData, Prop))
                Args.Data[Prop] = ExtraData[Prop];
        }
    }

    return Args;
};

// ● ajax request
/**
 * Represents a WebDesk Ajax request.
 * @property {string} Id Optional request id.
 * @property {string} OperationName Required operation name.
 * @property {object} Params Optional request parameters.
 * @property {string} Type Request type. Usually Ui or Proc.
 * @property {boolean} IsSingleInstance True when the requested UI may have a single client instance.
 * @property {string} CommandId Optional command id that caused this request.
 * @property {string} CommandName Optional command name that caused this request.
 */
tp.AjaxRequest = class {

    // ● constructor
    /**
     * Creates a WebDesk Ajax request.
     * @param {string} OperationName The operation name.
     * @param {object|null|undefined} Params Optional parameters.
     */
    constructor(OperationName, Params = null) {
        tp.AjaxRequest.Counter++;

        this.Id = tp.AjaxRequest.Counter.toString();
        this.OperationName = OperationName || "";
        this.Params = Params || {};
    }

    // ● static
    /**
     * Ajax request id counter.
     * @type {number}
     */
    static Counter = 0;
    /**
     * Executes an Ajax request and returns the response packet.
     * @param {tp.AjaxRequest|object|string} RequestOrOperationName The request, request-like object, or operation name.
     * @param {object|null|undefined} Params Optional parameters when the first argument is an operation name.
     * @returns {Promise<object|null>} Returns the packet.
     */
    static async Execute(RequestOrOperationName, Params = null) {
        var Request = null;
        var Args;

        if (tp.IsString(RequestOrOperationName)) {
            Request = new tp.AjaxRequest(RequestOrOperationName, Params);
        } else if (tp.IsObject(RequestOrOperationName) && !tp.IsBlankString(RequestOrOperationName.OperationName)) {
            Request = RequestOrOperationName;
        } else {
            tp.Throw("Cannot execute Ajax request. Invalid parameters.");
        }

        Args = await tp.Ajax.PostModelAsync(tp.Urls.AjaxExecute, Request);
        return Args.Packet;
    }
    /**
     * Executes an Ajax request and returns the response packet.
     * @param {tp.AjaxRequest|object|string} RequestOrOperationName The request, request-like object, or operation name.
     * @param {object|null|undefined} Params Optional parameters when the first argument is an operation name.
     * @returns {Promise<object|null>} Returns the packet.
     */
    static async ExecuteAsync(RequestOrOperationName, Params = null) {
        return tp.AjaxRequest.Execute(RequestOrOperationName, Params);
    }    
    
    // ● public
    /**
     * Optional request id.
     * @type {string}
     */
    Id = "";
    /**
     * Required operation name.
     * @type {string}
     */
    OperationName = "";
    /**
     * Optional request parameters.
     * @type {object}
     */
    Params = {};
    /**
     * Request type. Usually Ui or Proc.
     * @type {string}
     */
    Type = "Ui";
    /**
     * True when the requested UI may have a single client instance.
     * @type {boolean}
     */
    IsSingleInstance = false;
    /**
     * Optional command id that caused this request.
     * @type {string}
     */
    CommandId = "";
    /**
     * Optional command name that caused this request.
     * @type {string}
     */
    CommandName = "";
};
