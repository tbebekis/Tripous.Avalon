using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers;

/// <summary>
/// Provides pages used for Tripous Web JavaScript and control demos.
/// </summary>
public class DemosController : Controller
{
    // ● public
    /// <summary>
    /// Displays the demos index page.
    /// </summary>
    [Route("/demos")]
    public IActionResult Index()
    {
        return View();
    }
    /// <summary>
    /// Displays the core tp demo.
    /// </summary>
    [Route("/demo/tp")]
    public IActionResult Tp()
    {
        return View("Core/Tp");
    }
    /// <summary>
    /// Displays the tp.Ready demo.
    /// </summary>
    [Route("/demo/tp-ready")]
    public IActionResult TpReady()
    {
        return View("Core/TpReady");
    }
    /// <summary>
    /// Displays the tp helper functions demo.
    /// </summary>
    [Route("/demo/tp-helpers")]
    public IActionResult TpHelpers()
    {
        return View("Core/TpHelpers");
    }
    /// <summary>
    /// Displays the tp DOM helper functions demo.
    /// </summary>
    [Route("/demo/tp-dom")]
    public IActionResult TpDom()
    {
        return View("Core/TpDom");
    }
    /// <summary>
    /// Displays the tp.Object class demo.
    /// </summary>
    [Route("/demo/tp-object")]
    public IActionResult TpObject()
    {
        return View("Core/TpObject");
    }
    /// <summary>
    /// Displays the basic tp.DataTable demo.
    /// </summary>
    [Route("/demo/tp-data-table-basic")]
    public IActionResult TpDataTableBasic()
    {
        return View("Data/TpDataTableBasic");
    }
    /// <summary>
    /// Displays the tp.DataTable serialization demo.
    /// </summary>
    [Route("/demo/tp-data-table-serialization")]
    public IActionResult TpDataTableSerialization()
    {
        return View("Data/TpDataTableSerialization");
    }
    /// <summary>
    /// Displays the basic tp.DataSet demo.
    /// </summary>
    [Route("/demo/tp-data-set-basic")]
    public IActionResult TpDataSetBasic()
    {
        return View("Data/TpDataSetBasic");
    }
    /// <summary>
    /// Displays the screen overlay and spinner demo.
    /// </summary>
    [Route("/demo/tp-ui-overlay-spinner")]
    public IActionResult TpUiOverlaySpinner()
    {
        return View("UI/TpUiOverlaySpinner");
    }
    /// <summary>
    /// Displays the tp.ItemBar demo.
    /// </summary>
    [Route("/demo/tp-ui-item-bar")]
    public IActionResult TpUiItemBar()
    {
        return View("UI/TpUiItemBar");
    }
    /// <summary>
    /// Displays the tp.Button demo.
    /// </summary>
    [Route("/demo/tp-ui-button")]
    public IActionResult TpUiButton()
    {
        return View("UI/TpUiButton");
    }
    /// <summary>
    /// Displays the tp.ToolBar and tp.ButtonEx demo.
    /// </summary>
    [Route("/demo/tp-ui-toolbar")]
    public IActionResult TpUiToolBar()
    {
        return View("UI/TpUiToolBar");
    }
    /// <summary>
    /// Displays the tp.Menu and tp.ContextMenu demo.
    /// </summary>
    [Route("/demo/tp-ui-menu")]
    public IActionResult TpUiMenu()
    {
        return View("UI/TpUiMenu");
    }
    /// <summary>
    /// Displays the tp.SiteMenu demo.
    /// </summary>
    [Route("/demo/tp-ui-site-menu")]
    public IActionResult TpUiSiteMenu()
    {
        return View("UI/TpUiSiteMenu");
    }
    /// <summary>
    /// Displays the tp.DropDownBox demo.
    /// </summary>
    [Route("/demo/tp-ui-dropdown-box")]
    public IActionResult TpUiDropDownBox()
    {
        return View("UI/TpUiDropDownBox");
    }
    /// <summary>
    /// Displays the tp.ResizeDetector demo.
    /// </summary>
    [Route("/demo/tp-ui-resize-detector")]
    public IActionResult TpUiResizeDetector()
    {
        return View("UI/TpUiResizeDetector");
    }
    /// <summary>
    /// Displays the element size mode demo.
    /// </summary>
    [Route("/demo/tp-ui-element-size-mode")]
    public IActionResult TpUiElementSizeMode()
    {
        return View("UI/TpUiElementSizeMode");
    }
    /// <summary>
    /// Displays the tp.Splitter demo.
    /// </summary>
    [Route("/demo/tp-ui-splitter")]
    public IActionResult TpUiSplitter()
    {
        return View("UI/TpUiSplitter");
    }
    /// <summary>
    /// Displays the tp.GroupBox demo.
    /// </summary>
    [Route("/demo/tp-ui-group-box")]
    public IActionResult TpUiGroupBox()
    {
        return View("UI/TpUiGroupBox");
    }
    /// <summary>
    /// Displays the tp.Accordion demo.
    /// </summary>
    [Route("/demo/tp-ui-accordion")]
    public IActionResult TpUiAccordion()
    {
        return View("UI/TpUiAccordion");
    }
    /// <summary>
    /// Displays the tp.TabControl demo.
    /// </summary>
    [Route("/demo/tp-ui-tab-control")]
    public IActionResult TpUiTabControl()
    {
        return View("UI/TpUiTabControl");
    }
    /// <summary>
    /// Displays the tp.PanelList demo.
    /// </summary>
    [Route("/demo/tp-ui-panel-list")]
    public IActionResult TpUiPanelList()
    {
        return View("UI/TpUiPanelList");
    }
    /// <summary>
    /// Displays the tp.ImageSlider demo.
    /// </summary>
    [Route("/demo/tp-ui-image-slider")]
    public IActionResult TpUiImageSlider()
    {
        return View("UI/TpUiImageSlider");
    }
    /// <summary>
    /// Displays the tp.IFrame demo.
    /// </summary>
    [Route("/demo/tp-ui-iframe")]
    public IActionResult TpUiIFrame()
    {
        return View("UI/TpUiIFrame");
    }
    /// <summary>
    /// Displays the notifications demo.
    /// </summary>
    [Route("/demo/tp-ui-notifications")]
    public IActionResult TpUiNotifications()
    {
        return View("UI/TpUiNotifications");
    }
    /// <summary>
    /// Displays the notification dialogs demo.
    /// </summary>
    [Route("/demo/tp-ui-notification-dialogs")]
    public IActionResult TpUiNotificationDialogs()
    {
        return View("UI/TpUiNotificationDialogs");
    }
    /// <summary>
    /// Displays the frame box demo.
    /// </summary>
    [Route("/demo/tp-ui-frame-box")]
    public IActionResult TpUiFrameBox()
    {
        return View("UI/TpUiFrameBox");
    }
    /// <summary>
    /// Displays the content window demo.
    /// </summary>
    [Route("/demo/tp-ui-content-window")]
    public IActionResult TpUiContentWindow()
    {
        return View("UI/TpUiContentWindow");
    }
}
