// ● drop-down handler
/**
 * Handles a drop-down operation by toggling the visibility and positioning of a list element.
 * The list element may use absolute or fixed positioning.
 * @param {string|Element} ButtonOrSelector The button element or selector.
 * @param {string|Element} ListOrSelector The drop-down list element or selector.
 * @param {string|null|undefined} CssClass The CSS class to toggle.
 * @returns {object|null} Returns a small handler object with Dispose(), or null.
 */
tp.DropDownHandler = function (ButtonOrSelector, ListOrSelector, CssClass) {
    var Button = tp.Select(ButtonOrSelector);
    var List = tp.Select(ListOrSelector);
    var VisibleClass = tp.IsBlank(CssClass) ? tp.Classes.Visible : CssClass;
    var PositionList;
    var ButtonClicked;
    var WindowClicked;
    var WindowScrolled;
    if (!tp.IsHTMLElement(Button) || !tp.IsHTMLElement(List))
        return null;
    PositionList = function () {
        var Position = tp.GetComputedStyle(List).position;
        var ButtonRect = Button.getBoundingClientRect();
        var Parent;
        var ParentRect;
        if (tp.IsSameText(Position, "fixed")) {
            List.style.left = tp.px(Math.round(ButtonRect.left));
            List.style.top = tp.px(Math.round(ButtonRect.top + ButtonRect.height));
        } else {
            Parent = List.offsetParent || List.parentElement;
            ParentRect = Parent && tp.IsFunction(Parent.getBoundingClientRect) ? Parent.getBoundingClientRect() : { left: 0, top: 0 };
            List.style.left = tp.px(Math.round(ButtonRect.left - ParentRect.left));
            List.style.top = tp.px(Math.round(ButtonRect.top - ParentRect.top + ButtonRect.height));
        }
    };
    ButtonClicked = function (ev) {
        if (List.classList.toggle(VisibleClass))
            PositionList();
        ev.stopPropagation();
    };
    WindowClicked = function (ev) {
        if (List.classList.contains(VisibleClass)
            && !tp.ContainsEventTarget(Button, ev.target)
            && !tp.ContainsEventTarget(List, ev.target)) {
            List.classList.remove(VisibleClass);
        }
    };
    WindowScrolled = function () {
        List.classList.remove(VisibleClass);
    };
    Button.addEventListener("click", ButtonClicked);
    window.addEventListener("click", WindowClicked);
    window.addEventListener("scroll", WindowScrolled, true);
    return {
        /**
         * Disposes the drop-down handler.
         * @returns {void}
         */
        Dispose: function () {
            Button.removeEventListener("click", ButtonClicked);
            window.removeEventListener("click", WindowClicked);
            window.removeEventListener("scroll", WindowScrolled, true);
        }
    };
};
