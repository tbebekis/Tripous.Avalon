// ● form serialization
/**
 * Reads the value of an input, select, textarea, or button element and adds a property to a model object.
 * The property name is taken from the element name or id, in that order.
 * File and image input elements are ignored.
 * A multiple select element generates an array value.
 * @param {HTMLElement} Element The element to read.
 * @param {object} Model The target model object.
 * @returns {void}
 */
tp.ElementToProperty = function (Element, Model) {
    var Name;
    var NodeName;
    var Type;
    var List;
    var Index;
    var Values;
    if (!tp.IsHTMLElement(Element) || !tp.IsObject(Model))
        return;
    Name = Element.name || Element.id || "";
    if (tp.IsBlank(Name))
        return;
    NodeName = Element.nodeName.toLowerCase();
    Type = Element.type ? Element.type.toLowerCase() : "";
    switch (NodeName) {
        case "input":
            switch (Type) {
                case "hidden":
                case "text":
                case "password":
                case "color":
                case "date":
                case "datetime-local":
                case "email":
                case "month":
                case "number":
                case "range":
                case "search":
                case "tel":
                case "time":
                case "url":
                case "week":
                    Model[Name] = Element.value;
                    break;
                case "checkbox":
                    Model[Name] = Element.checked === true;
                    break;
                case "radio":
                    if (Element.checked === true)
                        Model[Name] = Element.value;
                    break;
                case "button":
                case "submit":
                case "reset":
                    Model[Name] = Element.value;
                    break;
            }
            break;
        case "button":
            if (Type === "button" || Type === "submit" || Type === "reset")
                Model[Name] = Element.value;
            break;
        case "select":
            if (Type === "select-one") {
                Model[Name] = Element.value;
            } else if (Type === "select-multiple") {
                Values = [];
                List = Element.options;
                for (Index = 0; Index < List.length; Index++) {
                    if (List[Index].selected === true)
                        Values.push(List[Index].value);
                }
                Model[Name] = Values;
            }
            break;
        case "textarea":
            Model[Name] = Element.value;
            break;
    }
};
/**
 * Serializes a form or container to a plain object.
 * Child input, select, textarea, and button elements become model properties named by name or id.
 * File and image input elements are ignored.
 * A multiple select element generates an array value.
 * @param {Element|string} ElementOrSelector The form or container element.
 * @param {object|null|undefined} Model The optional target model.
 * @returns {object} Returns the model where properties were added.
 */
tp.ContainerToModel = function (ElementOrSelector, Model) {
    var Parent = tp.Select(ElementOrSelector);
    var Elements;
    var Index;
    var Element;
    Model = tp.IsObject(Model) ? Model : {};
    if (tp.IsHTMLElement(Parent)) {
        Elements = Parent.nodeName.toLowerCase() === "form" ? Parent.elements : tp.SelectAll(Parent, "input, select, textarea, button");
        for (Index = 0; Index < Elements.length; Index++) {
            Element = Elements[Index];
            if (!tp.IsBlank(Element.name || Element.id))
                tp.ElementToProperty(Element, Model);
        }
    }
    return Model;
};
/**
 * Serializes a form or container to a plain object, including input[type=file] elements.
 * File values are read through tp.ReadFiles() and added as tp.HttpFile arrays.
 * @param {boolean} ShowSpinner True to show the global spinner while processing files.
 * @param {Element|string} ElementOrSelector The form or container element.
 * @returns {Promise<object>} Returns a promise resolved with the model.
 */
tp.ContainerToModelAsync = async function (ShowSpinner, ElementOrSelector) {
    var Model = {};
    var Parent = tp.Select(ElementOrSelector);
    var Elements;
    var ElementList = [];
    var FileElementList = [];
    var PromiseList = [];
    var Index;
    var Element;
    var Spinner = function (Flag) {
        if (ShowSpinner === true && tp.IsFunction(tp.ShowSpinner))
            tp.ShowSpinner(Flag);
    };
    if (!tp.IsHTMLElement(Parent))
        return Model;
    Elements = Parent.nodeName.toLowerCase() === "form" ? Parent.elements : tp.SelectAll(Parent, "input, select, textarea, button");
    for (Index = 0; Index < Elements.length; Index++) {
        Element = Elements[Index];
        if (!tp.IsBlank(Element.name || Element.id)) {
            if (Element instanceof HTMLInputElement && tp.IsSameText(Element.type, "file"))
                FileElementList.push(Element);
            else
                ElementList.push(Element);
        }
    }
    FileElementList.forEach(function (FileElement) {
        PromiseList.push(tp.ReadFiles(false, FileElement.files).then(function (FileList) {
            Model[FileElement.name || FileElement.id] = FileList;
        }));
    });
    PromiseList.push(new Promise(function (Resolve) {
        for (Index = 0; Index < ElementList.length; Index++)
            tp.ElementToProperty(ElementList[Index], Model);
        Resolve();
    }));
    Spinner(true);
    return Promise.all(PromiseList).then(function () {
        Spinner(false);
        return Model;
    }).catch(function (e) {
        if (tp.IsFunction(tp.ForceHideSpinner))
            tp.ForceHideSpinner();
        throw e;
    });
};

// ● file serialization
/**
 * Converts an ArrayBuffer to a hexadecimal string.
 * @param {ArrayBuffer} Buffer The buffer to convert.
 * @returns {string} Returns the hexadecimal string.
 */
tp.ArrayBufferToHex = function (Buffer) {
    var Values = new Uint8Array(Buffer);
    var Result = new Array(Values.length);
    var Index = Values.length;
    while (Index--)
        Result[Index] = (Values[Index] < 16 ? "0" : "") + Values[Index].toString(16);
    return Result.join("");
};
/**
 * File data prepared for posting to the server.
 */
tp.HttpFile = class {
    /**
     * Initializes a new instance.
     */
    constructor() {
        /** @type {string} The file name. */
        this.FileName = "";
        /** @type {number} The file size in bytes. */
        this.Size = 0;
        /** @type {string} The file MIME type. */
        this.MimeType = "";
        /** @type {string} The file content as Base64 or hexadecimal text. */
        this.Data = "";
    }
};
/**
 * Reads files and returns a list of tp.HttpFile objects.
 * @param {boolean} ShowSpinner True to show the global spinner while processing files.
 * @param {string|HTMLInputElement|FileList|File[]} FileListOrSelector An input[type=file], selector, FileList, or File array.
 * @param {Function|null|undefined} OnDone Optional callback called with the result list.
 * @param {Function|null|undefined} OnError Optional callback called with the error and current file.
 * @param {object|null|undefined} Context Optional callback context.
 * @param {boolean|null|undefined} AsHex True to read file data as hexadecimal text; otherwise, Base64 is used.
 * @returns {Promise<tp.HttpFile[]>} Returns a promise resolved with the file list.
 */
tp.ReadFiles = function (ShowSpinner, FileListOrSelector, OnDone, OnError, Context, AsHex) {
    var Spinner = function (Flag) {
        if (ShowSpinner === true && tp.IsFunction(tp.ShowSpinner))
            tp.ShowSpinner(Flag);
    };
    var RejectWith = function (Reject, Error, File) {
        Spinner(false);
        if (tp.IsFunction(OnError))
            tp.Call(OnError, Context, Error, File);
        Reject(Error);
    };
    var GetFileList = function () {
        var Element;
        if (tp.IsString(FileListOrSelector) || FileListOrSelector instanceof HTMLInputElement) {
            Element = tp.Select(FileListOrSelector);
            return Element instanceof HTMLInputElement ? Element.files : null;
        }
        if (tp.IsArrayLike(FileListOrSelector))
            return FileListOrSelector;
        return null;
    };
    var CreateHttpFile = function (File, Data) {
        var Result = new tp.HttpFile();
        Result.FileName = File.name;
        Result.Size = File.size;
        Result.MimeType = File.type;
        Result.Data = Data;
        return Result;
    };
    var ReadAsBase64 = function (ResultList, File, ReadNext, Reject) {
        var Reader = new FileReader();
        Reader.onload = function () {
            var Data = Reader.result || "";
            var Parts = String(Data).split("base64,");
            if (Parts.length === 2)
                Data = Parts[1];
            ResultList.push(CreateHttpFile(File, Data));
            ReadNext();
        };
        Reader.onerror = function (e) {
            RejectWith(Reject, e, File);
        };
        Reader.onabort = Reader.onerror;
        Reader.readAsDataURL(File);
    };
    var ReadAsHex = function (ResultList, File, ReadNext, Reject) {
        var Reader = new FileReader();
        Reader.onload = function () {
            ResultList.push(CreateHttpFile(File, tp.ArrayBufferToHex(Reader.result)));
            ReadNext();
        };
        Reader.onerror = function (e) {
            RejectWith(Reject, e, File);
        };
        Reader.onabort = Reader.onerror;
        Reader.readAsArrayBuffer(File);
    };
    return new Promise(function (Resolve, Reject) {
        var FileList;
        var ResultList = [];
        var Index = 0;
        var ReadNext = function () {
            var File;
            if (!FileList || Index >= FileList.length) {
                Spinner(false);
                if (tp.IsFunction(OnDone))
                    tp.Call(OnDone, Context, ResultList);
                Resolve(ResultList);
                return;
            }
            File = FileList[Index++];
            if (AsHex === true)
                ReadAsHex(ResultList, File, ReadNext, Reject);
            else
                ReadAsBase64(ResultList, File, ReadNext, Reject);
        };
        try {
            FileList = GetFileList();
            Spinner(true);
            ReadNext();
        } catch (e) {
            RejectWith(Reject, e, null);
        }
    });
};

// ● post
/**
 * Creates a temporary HTML form and submits a model to a URL using POST.
 * Array values become indexed field names such as Name[0], Name[1].
 * Non-simple values are serialized with JSON.stringify().
 * @param {string} Url The submit URL.
 * @param {object} Model The model to post.
 * @returns {void}
 */
tp.PostModelAsForm = function (Url, Model) {
    var Form;
    var Data = {};
    var Name;
    var Value;
    var Index;
    var Input;
    var NormalizeValue = function (Item) {
        return tp.IsSimple(Item) ? Item : JSON.stringify(Item);
    };
    var AddInput = function (InputName, InputValue) {
        Input = document.createElement("input");
        Input.setAttribute("type", "hidden");
        Input.setAttribute("name", InputName);
        Input.setAttribute("value", NormalizeValue(InputValue));
        Form.appendChild(Input);
    };
    if (tp.IsBlank(Url) || !tp.IsObject(Model))
        return;
    for (Name in Model) {
        if (Object.prototype.propertyIsEnumerable.call(Model, Name)) {
            Value = Model[Name];
            if (!tp.IsEmpty(Value) && !tp.IsFunction(Value)) {
                if (Value instanceof Date)
                    Value = Value.toISOString();
                Data[Name] = Value;
            }
        }
    }
    Form = document.createElement("form");
    Form.action = Url;
    Form.method = "post";
    for (Name in Data) {
        if (Object.prototype.propertyIsEnumerable.call(Data, Name)) {
            Value = Data[Name];
            if (tp.IsArray(Value)) {
                for (Index = 0; Index < Value.length; Index++)
                    AddInput(Name + "[" + Index + "]", Value[Index]);
            } else {
                AddInput(Name, Value);
            }
        }
    }
    document.body.appendChild(Form);
    Form.submit();
    setTimeout(function () {
        tp.Remove(Form);
    }, 1000 * 3);
};
