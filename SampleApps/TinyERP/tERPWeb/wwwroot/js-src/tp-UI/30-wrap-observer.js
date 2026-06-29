// ● wrap observer
/**
 * Observes containers and assigns CSS classes to wrapped child elements.
 * @type {object}
 */
tp.WrapObserver = (function () {
    var Initialized = false;
    var Containers = [];
    var WrappedClass = tp.Classes.Wrapped;
    var WrapFirstClass = tp.Classes.WrapFirst;
    var WrapLastClass = tp.Classes.WrapLast;
    /**
     * Processes a registered container.
     * @param {HTMLElement} Container The container element.
     * @returns {boolean} Returns true when the container is wrapped.
     */
    var ProcessContainer = function (Container) {
        var IsWrapped = false;
        var BoundaryClasses = [WrapFirstClass, WrapLastClass];
        var FirstElement = null;
        var PreviousElement = null;
        var PreviousRect = null;
        var Element;
        var Rect;
        var List;
        var Index;
        if (!tp.IsHTMLElement(Container))
            return false;
        List = Container.children;
        for (Index = 0; Index < List.length; Index++) {
            Element = List[Index];
            if (!tp.IsHTMLElement(Element))
                continue;
            if (!FirstElement)
                FirstElement = Element;
            tp.RemoveClass(Element, BoundaryClasses);
            Rect = Element.getBoundingClientRect();
            if (PreviousRect && PreviousRect.top < Rect.top) {
                tp.AddClass(PreviousElement, WrapLastClass);
                tp.AddClass(Element, WrapFirstClass);
                if (Index === List.length - 1)
                    tp.AddClass(Element, WrapLastClass);
                IsWrapped = true;
            }
            PreviousElement = Element;
            PreviousRect = Rect;
        }
        if (IsWrapped) {
            tp.AddClass(Container, WrappedClass);
            if (FirstElement)
                tp.AddClass(FirstElement, WrapFirstClass);
        } else {
            tp.RemoveClass(Container, WrappedClass);
            if (FirstElement)
                tp.RemoveClass(FirstElement, WrapFirstClass);
        }
        return IsWrapped;
    };
    /**
     * Processes all registered containers on window resize.
     * @returns {void}
     */
    var WindowResized = function () {
        var Index;
        for (Index = 0; Index < Containers.length; Index++)
            ProcessContainer(Containers[Index]);
    };
    return {
        /**
         * Initializes this service.
         * @param {string|null|undefined} Wrapped The wrapped container CSS class.
         * @param {string|null|undefined} WrapFirst The first wrapped child CSS class.
         * @param {string|null|undefined} WrapLast The last wrapped child CSS class.
         * @returns {void}
         */
        Initialize: function (Wrapped, WrapFirst, WrapLast) {
            WrappedClass = tp.IsBlank(Wrapped) ? tp.Classes.Wrapped : Wrapped;
            WrapFirstClass = tp.IsBlank(WrapFirst) ? tp.Classes.WrapFirst : WrapFirst;
            WrapLastClass = tp.IsBlank(WrapLast) ? tp.Classes.WrapLast : WrapLast;
            if (!Initialized && typeof window !== "undefined") {
                Initialized = true;
                window.addEventListener("resize", WindowResized, true);
            }
        },
        /**
         * Registers one or more container elements.
         * @param {...(HTMLElement|string)} Elements The container elements or selectors.
         * @returns {void}
         */
        AddContainer: function (...Elements) {
            var Element;
            var Index;
            this.Initialize();
            for (Index = 0; Index < Elements.length; Index++) {
                Element = tp.Select(Elements[Index]);
                if (tp.IsHTMLElement(Element) && !tp.ListContains(Containers, Element)) {
                    Containers.push(Element);
                    ProcessContainer(Element);
                }
            }
        },
        /**
         * Unregisters one or more container elements.
         * @param {...(HTMLElement|string)} Elements The container elements or selectors.
         * @returns {void}
         */
        RemoveContainer: function (...Elements) {
            var Element;
            var Index;
            for (Index = 0; Index < Elements.length; Index++) {
                Element = tp.Select(Elements[Index]);
                if (tp.IsHTMLElement(Element))
                    tp.ListRemove(Containers, Element);
            }
        },
        /**
         * Processes all registered containers.
         * @returns {void}
         */
        Process: function () {
            WindowResized();
        },
        /**
         * Processes a single container.
         * @param {HTMLElement|string} Element The container element or selector.
         * @returns {boolean} Returns true when the container is wrapped.
         */
        ProcessContainer: function (Element) {
            return ProcessContainer(tp.Select(Element));
        },
        /**
         * Gets the registered containers.
         * @returns {HTMLElement[]} Returns a copy of the registered containers.
         */
        GetContainers: function () {
            return Containers.slice();
        }
    };
})();
