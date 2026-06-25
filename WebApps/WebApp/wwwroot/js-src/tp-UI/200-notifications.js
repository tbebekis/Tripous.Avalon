// ● notification type
/**
 * Notification type values.
 * @enum {number}
 */
tp.NotificationType = {
    Information: 1,
    Warning: 2,
    Error: 4,
    Success: 5
};
Object.freeze(tp.NotificationType);

// ● notification setup
/**
 * Global settings for notification boxes.
 * @type {object}
 */
tp.NotificationBoxSetup = {
    Information: {
        Title: "Information",
        BackColor: "#ffffd7",
        BorderColor: "#ffeb3b"
    },
    Warning: {
        Title: "Warning",
        BackColor: "#e7ffe7",
        BorderColor: "#4caf50"
    },
    Error: {
        Title: "Error",
        BackColor: "#ffe7e7",
        BorderColor: "#f44336"
    },
    Success: {
        Title: "Success",
        BackColor: "#e7ffff",
        BorderColor: "#2196f3"
    },
    ToTop: false,
    DurationSecs: 10,
    MaxVisible: 5,
    Height: 100,
    Width: 350
};

// ● notification boxes
/**
 * Tracks and places notification boxes.
 * @type {object}
 */
tp.NotificationBoxes = {
    Boxes: [],
    Queue: [],
    SelectedBox: null,
    fKeyDownHandler: null,
    /**
     * Adds a notification box.
     * @param {tp.NotificationBox} Box The notification box.
     * @returns {void}
     */
    Add: function (Box) {
        if (!Box || Box.IsDisposed)
            return;
        if (tp.NotificationBoxes.Boxes.indexOf(Box) === -1 && tp.NotificationBoxes.Queue.indexOf(Box) === -1) {
            tp.NotificationBoxes.HookDocumentKeyDown();
            if (tp.NotificationBoxes.Boxes.length < tp.NotificationBoxes.VisibleLimit)
                tp.NotificationBoxes.Show(Box);
            else
                tp.NotificationBoxes.Enqueue(Box);
        }
    },
    /**
     * Removes a notification box.
     * @param {tp.NotificationBox} Box The notification box.
     * @param {boolean} ActivateNext True to activate the next queued box.
     * @returns {void}
     */
    Remove: function (Box, ActivateNext) {
        tp.ListRemove(tp.NotificationBoxes.Boxes, Box);
        tp.ListRemove(tp.NotificationBoxes.Queue, Box);
        if (tp.NotificationBoxes.SelectedBox === Box)
            tp.NotificationBoxes.ClearSelectedBox(Box);
        tp.NotificationBoxes.Reflow();
        if (ActivateNext === true)
            tp.NotificationBoxes.ShowNext();
        if (tp.NotificationBoxes.Boxes.length === 0 && tp.NotificationBoxes.Queue.length === 0)
            tp.NotificationBoxes.UnhookDocumentKeyDown();
    },
    /**
     * Displays a notification box.
     * @param {tp.NotificationBox} Box The notification box.
     * @returns {void}
     */
    Show: function (Box) {
        if (!Box || Box.IsDisposed)
            return;
        Box.Handle.style.display = "";
        tp.NotificationBoxes.Place(Box.Handle);
        tp.NotificationBoxes.Boxes.push(Box);
        Box.BringToFront();
        Box.StartTimer();
        Box.ShowVisible();
    },
    /**
     * Adds a notification box to the queue.
     * @param {tp.NotificationBox} Box The notification box.
     * @returns {void}
     */
    Enqueue: function (Box) {
        if (!Box || Box.IsDisposed)
            return;
        Box.Handle.style.display = "none";
        tp.NotificationBoxes.Queue.push(Box);
    },
    /**
     * Displays the next queued notification box, if any.
     * @returns {void}
     */
    ShowNext: function () {
        var Box;
        while (tp.NotificationBoxes.Boxes.length < tp.NotificationBoxes.VisibleLimit && tp.NotificationBoxes.Queue.length > 0) {
            Box = tp.NotificationBoxes.Queue.shift();
            if (Box && !Box.IsDisposed)
                tp.NotificationBoxes.Show(Box);
        }
    },
    /**
     * Selects a notification box.
     * @param {tp.NotificationBox} Box The notification box.
     * @returns {void}
     */
    SelectBox: function (Box) {
        if (tp.NotificationBoxes.SelectedBox && tp.NotificationBoxes.SelectedBox !== Box)
            tp.NotificationBoxes.ClearSelectedBox(tp.NotificationBoxes.SelectedBox);
        tp.NotificationBoxes.HookDocumentKeyDown();
        tp.NotificationBoxes.SelectedBox = Box;
        if (Box && Box.HasHandle)
            Box.Handle.classList.add(tp.Classes.Selected);
    },
    /**
     * Clears the selected notification box.
     * @param {tp.NotificationBox|null|undefined} Box The notification box.
     * @returns {void}
     */
    ClearSelectedBox: function (Box) {
        if (Box && Box.HasHandle)
            Box.Handle.classList.remove(tp.Classes.Selected);
        if (tp.NotificationBoxes.SelectedBox === Box)
            tp.NotificationBoxes.SelectedBox = null;
    },
    /**
     * Places a notification box element.
     * @param {HTMLElement} Element The notification box element.
     * @returns {void}
     */
    Place: function (Element) {
        var List = tp.NotificationBoxes.Boxes;
        var RefBox = List.length > 0 ? List[List.length - 1] : null;
        var Rect;
        var Top;
        if (!tp.IsHTMLElement(Element))
            return;
        if (tp.NotificationBoxSetup.ToTop === true) {
            Top = 10;
            if (RefBox) {
                Rect = RefBox.Handle.getBoundingClientRect();
                Top = Rect.top + Rect.height + 10;
            }
        } else {
            Top = tp.Viewport.Height - 10;
            if (RefBox)
                Top = RefBox.Handle.getBoundingClientRect().top - 10;
            Rect = Element.getBoundingClientRect();
            Top -= Rect.height;
        }
        Element.style.top = tp.px(Math.max(10, Top));
    },
    /**
     * Reflows non-moved notification boxes.
     * @returns {void}
     */
    Reflow: function () {
        var Boxes = tp.NotificationBoxes.Boxes.slice();
        var Index;
        tp.NotificationBoxes.Boxes = [];
        for (Index = 0; Index < Boxes.length; Index++) {
            if (Boxes[Index] && !Boxes[Index].IsDisposed && Boxes[Index].HasHandle) {
                tp.NotificationBoxes.Place(Boxes[Index].Handle);
                tp.NotificationBoxes.Boxes.push(Boxes[Index]);
            }
        }
    },
    /**
     * Returns the highest z-index used by notification boxes.
     * @returns {number} Returns the maximum z-index.
     */
    get MaxZIndex() {
        var Result = tp.MaxZIndexOf(document.body);
        var Index;
        var Value;
        for (Index = 0; Index < tp.NotificationBoxes.Boxes.length; Index++) {
            Value = tp.ZIndex(tp.NotificationBoxes.Boxes[Index].Handle);
            Result = Math.max(Result, tp.ExtractNumber(Value));
        }
        return Result;
    },
    /**
     * Returns the visible notification limit.
     * @returns {number} Returns the visible notification limit.
     */
    get VisibleLimit() {
        return Math.max(1, tp.ToInt(tp.NotificationBoxSetup.MaxVisible));
    },
    /**
     * Hooks document key-down.
     * @returns {void}
     */
    HookDocumentKeyDown: function () {
        if (!tp.NotificationBoxes.fKeyDownHandler) {
            tp.NotificationBoxes.fKeyDownHandler = tp.NotificationBoxes.HandleDocumentKeyDown.bind(tp.NotificationBoxes);
            document.addEventListener("keydown", tp.NotificationBoxes.fKeyDownHandler);
        }
    },
    /**
     * Unhooks document key-down.
     * @returns {void}
     */
    UnhookDocumentKeyDown: function () {
        if (tp.NotificationBoxes.fKeyDownHandler) {
            document.removeEventListener("keydown", tp.NotificationBoxes.fKeyDownHandler);
            tp.NotificationBoxes.fKeyDownHandler = null;
        }
    },
    /**
     * Handles document key-down events.
     * @param {KeyboardEvent} e The event object.
     * @returns {void}
     */
    HandleDocumentKeyDown: function (e) {
        var Box = tp.NotificationBoxes.SelectedBox;
        if (tp.IsKey(e, tp.Keys.Escape) && Box && !Box.IsDisposed) {
            Box.Dispose();
            e.preventDefault();
        }
    }
};

// ● notification box
/**
 * Displays a non-modal notification message.
 *
 * Events:
 * - Disposing
 * - Disposed
 * - ParentChanged
 * - EnabledChanged
 * - VisibleChanged
 * - ElementSizeChanged
 * - SizeModeChanged
 */
tp.NotificationBox = class extends tp.Component {
    // ● constructor
    /**
     * Creates a notification box.
     * @param {string} Message The notification message.
     * @param {number|null|undefined} Type The tp.NotificationType value.
     */
    constructor(Message, Type) {
        var Element = tp.NotificationBox.CreateElement(Message, Type);
        super({ ElementOrSelector: Element });
        this.tpClass = "tp.NotificationBox";
        this.Type = tp.NotificationBox.NormalizeType(Type);
        this.fClickHandler = this.FuncBind(this.HandleClick);
        this.fKeyDownHandler = this.FuncBind(this.HandleKeyDown);
        this.fMouseDownHandler = this.FuncBind(this.HandleMouseDown);
        this.divCaption = tp.Select(this.Handle, "." + tp.Classes.Caption);
        this.divClose = tp.Select(this.Handle, "." + tp.Classes.Close);
        this.divContent = tp.Select(this.Handle, "." + tp.Classes.Content);
        this.edtMessage = tp.Select(this.Handle, "textarea");
        this.Setup();
    }

    // ● protected
    /**
     * Creates the notification box element.
     * @param {string} Message The notification message.
     * @param {number|null|undefined} Type The tp.NotificationType value.
     * @returns {HTMLElement} Returns the notification box element.
     */
    static CreateElement(Message, Type) {
        var Element = document.createElement("div");
        var Setup = tp.NotificationBox.GetSetup(Type);
        var TypeName = tp.NotificationBox.GetTypeName(Type);
        var Caption = document.createElement("div");
        var Title = document.createElement("div");
        var Spacer = document.createElement("div");
        var Close = document.createElement("button");
        var Content = document.createElement("div");
        var TextArea = document.createElement("textarea");
        Element.id = tp.SafeId("tp-NotificationBox");
        Element.className = "tp-NotificationBox tp-" + TypeName;
        Element.style.backgroundColor = Setup.BackColor;
        Element.style.borderColor = Setup.BorderColor;
        Element.style.borderLeftColor = Setup.BorderColor;
        Element.style.height = tp.px(tp.NotificationBoxSetup.Height);
        if (tp.Viewport.IsXSmall) {
            Element.style.left = "2px";
            Element.style.right = "2px";
        } else {
            Element.style.width = tp.px(tp.NotificationBoxSetup.Width);
        }
        Caption.className = tp.Classes.Caption;
        Title.textContent = Setup.Title;
        Spacer.className = tp.Classes.FlexFill;
        Close.type = "button";
        Close.className = tp.Classes.Close;
        Close.textContent = "x";
        Content.className = tp.Classes.Content;
        TextArea.value = tp.IsNil(Message) ? "" : String(Message);
        TextArea.spellcheck = false;
        TextArea.setAttribute("autocorrect", "off");
        Caption.appendChild(Title);
        Caption.appendChild(Spacer);
        Caption.appendChild(Close);
        Content.appendChild(TextArea);
        Element.appendChild(Caption);
        Element.appendChild(Content);
        document.body.appendChild(Element);
        return Element;
    }
    /**
     * Returns a normalized notification type.
     * @param {number|null|undefined} Type The notification type.
     * @returns {number} Returns a tp.NotificationType value.
     */
    static NormalizeType(Type) {
        return tp.IsNumber(Type) && !tp.IsBlank(tp.EnumNameOf(tp.NotificationType, Type))
            ? Type
            : tp.NotificationType.Information;
    }
    /**
     * Returns the setup object for a notification type.
     * @param {number|null|undefined} Type The notification type.
     * @returns {object} Returns the setup object.
     */
    static GetSetup(Type) {
        var Name = tp.NotificationBox.GetTypeName(Type);
        return tp.NotificationBoxSetup[Name] || tp.NotificationBoxSetup.Information;
    }
    /**
     * Returns the enum name for a notification type.
     * @param {number|null|undefined} Type The notification type.
     * @returns {string} Returns the enum name.
     */
    static GetTypeName(Type) {
        var Name = tp.EnumNameOf(tp.NotificationType, tp.NotificationBox.NormalizeType(Type));
        return tp.IsBlank(Name) ? "Information" : Name;
    }
    /**
     * Completes notification box setup.
     * @returns {void}
     */
    Setup() {
        this.Handle.addEventListener("click", this.fClickHandler);
        this.Handle.addEventListener("keydown", this.fKeyDownHandler);
        this.Handle.addEventListener("mousedown", this.fMouseDownHandler);
        this.Dragger = new tp.Dragger(tp.DraggerMode.Both, this.Handle, this.divCaption);
        this.Dragger.MinWidth = 180;
        this.Dragger.MinHeight = 70;
        this.Dragger.On(tp.Events.DragStart, this.HandleDragStart, this);
        this.Handle.tabIndex = 0;
        this.divCaption.tabIndex = 0;
        this.BringToFront();
        tp.NotificationBoxes.Add(this);
    }
    /**
     * Shows this notification box.
     * @returns {void}
     */
    ShowVisible() {
        var Self = this;
        requestAnimationFrame(function () {
            if (!Self.IsDisposed)
                Self.Handle.classList.add(tp.Classes.Visible);
        });
    }
    /**
     * Starts the close timer.
     * @returns {void}
     */
    StartTimer() {
        if (this.fTimerId)
            clearTimeout(this.fTimerId);
        this.fTimerId = setTimeout(function (Self) {
            if (Self.Clicked !== true)
                Self.Dispose();
        }, tp.NotificationBoxSetup.DurationSecs * 1000, this);
    }
    /**
     * Handles click events.
     * @param {MouseEvent} e The event object.
     * @returns {void}
     */
    HandleClick(e) {
        tp.NotificationBoxes.SelectBox(this);
        this.Clicked = true;
        this.Handle.classList.add(tp.Classes.Pinned);
        if (this.fTimerId) {
            clearTimeout(this.fTimerId);
            this.fTimerId = null;
        }
        if (this.divClose.contains(e.target))
            this.Dispose();
    }
    /**
     * Handles key-down events.
     * @param {KeyboardEvent} e The event object.
     * @returns {void}
     */
    HandleKeyDown(e) {
        if (tp.IsKey(e, tp.Keys.Escape)) {
            this.Dispose();
            e.preventDefault();
        }
    }
    /**
     * Handles mouse-down events.
     * @param {MouseEvent} e The event object.
     * @returns {void}
     */
    HandleMouseDown(e) {
        tp.NotificationBoxes.SelectBox(this);
        this.BringToFront();
    }
    /**
     * Handles drag-start notifications.
     * @returns {void}
     */
    HandleDragStart() {
        var Rect = this.Handle.getBoundingClientRect();
        this.Clicked = true;
        tp.NotificationBoxes.Remove(this, true);
        tp.NotificationBoxes.SelectBox(this);
        this.Handle.style.left = tp.px(Rect.left);
        this.Handle.style.top = tp.px(Rect.top);
        this.Handle.style.right = "auto";
        this.Handle.style.position = "fixed";
        this.Handle.classList.add(tp.Classes.Pinned);
        if (this.fTimerId) {
            clearTimeout(this.fTimerId);
            this.fTimerId = null;
        }
    }

    // ● public
    /**
     * Brings this notification box to front.
     * @returns {void}
     */
    BringToFront() {
        this.Handle.style.zIndex = String(tp.NotificationBoxes.MaxZIndex + 1);
    }
    /**
     * Disposes this instance.
     * @returns {void}
     */
    Dispose() {
        if (this.IsDisposed)
            return;
        if (this.fTimerId) {
            clearTimeout(this.fTimerId);
            this.fTimerId = null;
        }
        if (this.Dragger) {
            this.Dragger.Dispose();
            this.Dragger = null;
        }
        if (this.HasHandle) {
            this.Handle.removeEventListener("click", this.fClickHandler);
            this.Handle.removeEventListener("keydown", this.fKeyDownHandler);
            this.Handle.removeEventListener("mousedown", this.fMouseDownHandler);
            tp.NotificationBoxes.Remove(this, true);
        }
        this.divCaption = null;
        this.divClose = null;
        this.divContent = null;
        this.edtMessage = null;
        this.fClickHandler = null;
        this.fKeyDownHandler = null;
        this.fMouseDownHandler = null;
        super.Dispose();
    }

    // ● properties
    /**
     * Returns the notification type name.
     * @returns {string} Returns the notification type name.
     */
    get TypeText() {
        return tp.NotificationBox.GetTypeName(this.Type);
    }
};

// ● prototype
/**
 * Gets the Tripous class name.
 * @type {string}
 */
tp.NotificationBox.prototype.tpClass = "tp.NotificationBox";
/**
 * Notification type.
 * @type {number}
 */
tp.NotificationBox.prototype.Type = tp.NotificationType.Information;
/**
 * Caption element.
 * @type {HTMLElement|null}
 */
tp.NotificationBox.prototype.divCaption = null;
/**
 * Close button element.
 * @type {HTMLElement|null}
 */
tp.NotificationBox.prototype.divClose = null;
/**
 * Content element.
 * @type {HTMLElement|null}
 */
tp.NotificationBox.prototype.divContent = null;
/**
 * Message textarea element.
 * @type {HTMLTextAreaElement|null}
 */
tp.NotificationBox.prototype.edtMessage = null;
/**
 * True when clicked or moved.
 * @type {boolean}
 */
tp.NotificationBox.prototype.Clicked = false;
/**
 * Dragger instance.
 * @type {tp.Dragger|null}
 */
tp.NotificationBox.prototype.Dragger = null;
/**
 * Close timeout id.
 * @type {number|null}
 */
tp.NotificationBox.prototype.fTimerId = null;
/**
 * Click handler.
 * @type {Function|null}
 */
tp.NotificationBox.prototype.fClickHandler = null;
/**
 * Key-down handler.
 * @type {Function|null}
 */
tp.NotificationBox.prototype.fKeyDownHandler = null;
/**
 * Mouse-down handler.
 * @type {Function|null}
 */
tp.NotificationBox.prototype.fMouseDownHandler = null;

// ● helpers
/**
 * Displays a notification message.
 * @param {string} Message The notification message.
 * @param {number|null|undefined} Type The tp.NotificationType value.
 * @returns {tp.NotificationBox} Returns the notification box.
 */
tp.NotifyFunc = function (Message, Type) {
    return new tp.NotificationBox(Message, Type);
};
/**
 * Displays a notification message.
 * @param {string} Message The notification message.
 * @param {number|null|undefined} Type The tp.NotificationType value.
 * @returns {tp.NotificationBox|null} Returns the notification box, if any.
 */
tp.Notify = function (Message, Type) {
    if (tp.IsFunction(tp.NotifyFunc))
        return tp.NotifyFunc(Message, Type);
    return null;
};
/**
 * Displays an information notification.
 * @param {string} Message The notification message.
 * @returns {tp.NotificationBox|null} Returns the notification box, if any.
 */
tp.InfoNote = function (Message) {
    return tp.Notify(Message, tp.NotificationType.Information);
};
/**
 * Displays a warning notification.
 * @param {string} Message The notification message.
 * @returns {tp.NotificationBox|null} Returns the notification box, if any.
 */
tp.WarningNote = function (Message) {
    return tp.Notify(Message, tp.NotificationType.Warning);
};
/**
 * Displays an error notification.
 * @param {string} Message The notification message.
 * @returns {tp.NotificationBox|null} Returns the notification box, if any.
 */
tp.ErrorNote = function (Message) {
    return tp.Notify(Message, tp.NotificationType.Error);
};
/**
 * Displays a success notification.
 * @param {string} Message The notification message.
 * @returns {tp.NotificationBox|null} Returns the notification box, if any.
 */
tp.SuccessNote = function (Message) {
    return tp.Notify(Message, tp.NotificationType.Success);
};
