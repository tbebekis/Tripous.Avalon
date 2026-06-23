// ● initialization
/**
 * Initializes the Tripous JavaScript runtime.
 * @returns {void}
 */
(function () {
    /**
     * Executes Tripous JavaScript runtime initialization.
     * @returns {void}
     */
    var Initialize = function () {
        var List;
        var Listener;
        var Index;
        if (tp.IsReady === true)
            return;
        tp.IsReady = true;
        if (tp.IsFunction(tp.AppInitializeBefore))
            tp.AppInitializeBefore();
        List = tp.ReadyListeners.slice();
        for (Index = 0; Index < List.length; Index++) {
            Listener = List[Index];
            if (Listener && tp.IsFunction(Listener.Func))
                Listener.Func.call(Listener.Context || null);
        }
        tp.ReadyListeners.length = 0;
        if (tp.IsFunction(tp.AppInitializeAfter))
            tp.AppInitializeAfter();
        if (tp.IsFunction(tp.Main))
            tp.Main();
    };

    if (document.readyState === "loading")
        document.addEventListener("DOMContentLoaded", Initialize, { once: true });
    else
        Initialize();
})();
