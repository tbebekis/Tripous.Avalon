/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Web;

/// <summary>
/// Provides controller extension methods.
/// </summary>
static public class ControllerExtensions
{
    // ● static public
    /// <summary>
    /// Renders a view to a string. The view can be a main view or a partial view.
    /// </summary>
    static public string RenderViewToString(this ControllerBase Instance, string ViewName, object Model, bool IsMainView, IDictionary<string, object> PlusViewData = null)
    {
        IRazorViewEngine ViewEngine = Instance.HttpContext.RequestServices.GetRequiredService<IRazorViewEngine>();
        ActionContext ActionContext = new(Instance.HttpContext, Instance.RouteData, Instance.ControllerContext.ActionDescriptor, Instance.ModelState);

        ViewDataDictionary ViewData;
        ITempDataDictionary TempData;

        if (Instance is Controller Controller)
        {
            ViewData = Controller.ViewData;
            TempData = Controller.TempData;
        }
        else
        {
            IModelMetadataProvider MetadataProvider = Instance.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();
            ITempDataProvider TempDataProvider = Instance.HttpContext.RequestServices.GetRequiredService<ITempDataProvider>();

            ViewData = new ViewDataDictionary(MetadataProvider, new ModelStateDictionary());
            TempData = new TempDataDictionary(ActionContext.HttpContext, TempDataProvider);
        }

        ViewData.Model = Model;

        if (PlusViewData != null)
        {
            foreach (var Entry in PlusViewData)
                ViewData[Entry.Key] = Entry.Value;
        }

        if (string.IsNullOrWhiteSpace(ViewName))
            ViewName = Instance.ControllerContext.ActionDescriptor.ActionName;

        ViewEngineResult ViewResult = ViewEngine.FindView(ActionContext, ViewName, IsMainView);

        if (ViewResult.View == null)
        {
            ViewResult = ViewEngine.GetView(null, ViewName, false);
            if (ViewResult.View == null)
                throw new TripousException($"View not found: {ViewName}");
        }

        using StringWriter Writer = new();
        ViewContext ViewContext = new(ActionContext, ViewResult.View, ViewData, TempData, Writer, new HtmlHelperOptions());
        ViewResult.View.RenderAsync(ViewContext).GetAwaiter().GetResult();
        return Writer.GetStringBuilder().ToString();
    }
    /// <summary>
    /// Renders a partial view to a string.
    /// </summary>
    static public string RenderPartialViewToString(this ControllerBase Instance, string ViewName, object Model, IDictionary<string, object> PlusViewData = null)
    {
        return RenderViewToString(Instance, ViewName, Model, false, PlusViewData);
    }
    /// <summary>
    /// Renders a partial view to a string.
    /// </summary>
    static public string RenderPartialViewToString(this ControllerBase Instance, string ViewName, IDictionary<string, object> PlusViewData = null)
    {
        return RenderViewToString(Instance, ViewName, null, false, PlusViewData);
    }
    /// <summary>
    /// Renders a partial view to a string using the current action name as the view name.
    /// </summary>
    static public string RenderPartialViewToString(this ControllerBase Instance, IDictionary<string, object> PlusViewData = null)
    {
        return RenderViewToString(Instance, null, null, false, PlusViewData);
    }
}
