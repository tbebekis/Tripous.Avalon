// ● async
/**
 * Executes a function inside a Promise.
 * @param {Function} Func The function to execute.
 * @param {*} Info Optional information passed to the function.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {Promise<*>} Returns a Promise.
 */
tp.Async = async function (Func, Info = null, Context = null) {
    return new Promise(function (Resolve, Reject) {
        try {
            Resolve(tp.Call(Func, Context, Info));
        } catch (e) {
            Reject(e);
        }
    });
};
/**
 * Executes Promise-returning calls sequentially.
 * @param {boolean} ShowSpinner True to show the global spinner, when available.
 * @param {Array} List The values to process.
 * @param {Function} Func The function to call for each value.
 * @param {Function|null|undefined} BreakFunc Optional function that can stop the chain.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {Promise<*>} Returns a Promise.
 */
tp.Async.Chain = async function (ShowSpinner, List, Func, BreakFunc = null, Context = null) {
    var Spinner = function (Flag) {
        if (ShowSpinner === true && tp.IsFunction(tp.ShowSpinner))
            tp.ShowSpinner(Flag);
    };
    var ReduceFunc;
    var Result;

    List = tp.IsArray(List) ? List : [];
    Spinner(true);

    ReduceFunc = function (PromiseValue, Current) {
        return PromiseValue.then(function (Value) {
            if (tp.IsFunction(BreakFunc) && tp.Call(BreakFunc, Context, Value) === true)
                return PromiseValue;
            return tp.Call(Func, Context, Current);
        });
    };

    Result = List.reduce(ReduceFunc, Promise.resolve(null));

    if (ShowSpinner === true) {
        Result.finally(function () {
            Spinner(false);
        });
    }

    return Result;
};
/**
 * Executes Promise-returning calls simultaneously.
 * @param {boolean} ShowSpinner True to show the global spinner, when available.
 * @param {Array} List The values to process.
 * @param {Function} Func The function to call for each value.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {Promise<Array>} Returns a Promise.
 */
tp.Async.All = async function (ShowSpinner, List, Func, Context = null) {
    var Spinner = function (Flag) {
        if (ShowSpinner === true && tp.IsFunction(tp.ShowSpinner))
            tp.ShowSpinner(Flag);
    };
    var Result;

    List = tp.IsArray(List) ? List : [];
    Spinner(true);

    Result = Promise.all(List.map(function (Item) {
        return tp.Call(Func, Context, Item);
    }));

    if (ShowSpinner === true) {
        Result.finally(function () {
            Spinner(false);
        });
    }

    return Result;
};
/**
 * Executes a function inside a Promise.
 * @param {Function} Func The function to execute.
 * @param {*} Info Optional information passed to the function.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {Promise<*>} Returns a Promise.
 */
tp.Async.Execute = tp.Async;
/**
 * Executes a function inside a Promise.
 * @param {Function} Func The function to execute.
 * @param {*} Info Optional information passed to the function.
 * @param {object|null|undefined} Context The optional callback context.
 * @returns {Promise<*>} Returns a Promise.
 */
tp.Async.ExecuteAsync = tp.Async;
