using Microsoft.AspNetCore.Mvc;
using System.Data;
using Tripous.Data;

namespace WebApp.Controllers;

/// <summary>
/// Provides pages used for Tripous Web JavaScript and control demos.
/// </summary>
public class DemosController : Controller
{
    // ● private
    static string CreateDataModuleJson()
    {
        DemoDataModule Module = new();
        JsonDataModule Packet = new(Module);
        return Packet.ToJson();
    }

    /// <summary>
    /// Demo data module used by the Tripous Web data module serialization demo.
    /// </summary>
    class DemoDataModule : DataModule
    {
        // ● private
        /// <summary>
        /// Creates a data column from a field definition.
        /// </summary>
        static DataColumn CreateColumn(FieldDef FieldDef)
        {
            DataColumn Result = new(FieldDef.Name);
            Result.ExtendedProperties["Descriptor"] = FieldDef;
            Result.DataType = FieldDef.DataType.GetNetType();
            Result.Caption = FieldDef.Title;
            if (FieldDef.DataType == DataFieldType.String)
                Result.MaxLength = FieldDef.MaxLength;
            return Result;
        }
        /// <summary>
        /// Adds a column to a table.
        /// </summary>
        void AddColumn(MemTable Table, FieldDef FieldDef) => Table.Columns.Add(CreateColumn(FieldDef));

        // ● construction
        /// <summary>
        /// Constructor.
        /// </summary>
        public DemoDataModule()
        {
            ModuleDef = new ModuleDef();
            ModuleDef.Name = "ProductsModule";
            ModuleDef.TitleKey = "Products";
            ModuleDef.ConnectionName = "Demo";
            ModuleDef.GuidOids = true;

            TableDef TableDef = ModuleDef.Table;
            TableDef.Name = "Products";
            TableDef.Alias = "Products";
            TableDef.KeyField = "Id";

            FieldDef Id = TableDef.AddStringId("Id");
            Id.TitleKey = "Id";
            FieldDef Code = TableDef.AddString("Code", 32, Flags: FieldFlags.Required | FieldFlags.Searchable);
            Code.TitleKey = "Code";
            FieldDef Name = TableDef.AddString("Name", 96, Flags: FieldFlags.Required | FieldFlags.Searchable);
            Name.TitleKey = "Product";
            FieldDef Price = TableDef.AddDecimal("Price", Decimals: 2);
            Price.TitleKey = "Price";
            FieldDef IsActive = TableDef.AddBoolean("IsActive", Flags: FieldFlags.Boolean);
            IsActive.TitleKey = "Active";

            ModuleDef.UpdateReferences();

            DataSet = new DataSet("DS_" + ModuleDef.Name);
            tblItem = new MemTable(TableDef.Name);
            tblItem.KeyField = TableDef.KeyField;
            tblItem.AutoGenerateGuidKeys = ModuleDef.GuidOids;
            DataSet.Tables.Add(tblItem);
            ItemTables.Add(tblItem);

            AddColumn(tblItem, Id);
            AddColumn(tblItem, Code);
            AddColumn(tblItem, Name);
            AddColumn(tblItem, Price);
            AddColumn(tblItem, IsActive);

            tblItem.Rows.Add("P-100", "SKU-100", "Desk chair", 129.90m, 1);
            tblItem.Rows.Add("P-200", "SKU-200", "Monitor arm", 79.50m, 1);
            tblItem.Rows.Add("P-300", "SKU-300", "Cable tray", 22.35m, 0);
            tblItem.AcceptChanges();

            State = DataMode.Edit;
        }
    }

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
    /// Displays the tp.DataSet from JsonDataModule demo.
    /// </summary>
    [Route("/demo/tp-data-module-serialization")]
    public IActionResult TpDataModuleSerialization()
    {
        ViewData["DataModuleJson"] = CreateDataModuleJson();
        return View("Data/TpDataModuleSerialization");
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
    /// Displays the tp.Label demo.
    /// </summary>
    [Route("/demo/tp-ui-label")]
    public IActionResult TpUiLabel()
    {
        return View("UI/TpUiLabel");
    }
    /// <summary>
    /// Displays the tp.CheckBox demo.
    /// </summary>
    [Route("/demo/tp-ui-check-box")]
    public IActionResult TpUiCheckBox()
    {
        return View("UI/TpUiCheckBox");
    }
    /// <summary>
    /// Displays the tp.ListBox demo.
    /// </summary>
    [Route("/demo/tp-ui-list-box")]
    public IActionResult TpUiListBox()
    {
        return View("UI/TpUiListBox");
    }
    /// <summary>
    /// Displays the tp.ComboBox demo.
    /// </summary>
    [Route("/demo/tp-ui-combo-box")]
    public IActionResult TpUiComboBox()
    {
        return View("UI/TpUiComboBox");
    }
    /// <summary>
    /// Displays the tp.HtmlComboBox demo.
    /// </summary>
    [Route("/demo/tp-ui-html-combo-box")]
    public IActionResult TpUiHtmlComboBox()
    {
        return View("UI/TpUiHtmlComboBox");
    }
    /// <summary>
    /// Displays the tp.HtmlListBox demo.
    /// </summary>
    [Route("/demo/tp-ui-html-list-box")]
    public IActionResult TpUiHtmlListBox()
    {
        return View("UI/TpUiHtmlListBox");
    }
    /// <summary>
    /// Displays the tp.TextBox demo.
    /// </summary>
    [Route("/demo/tp-ui-text-box")]
    public IActionResult TpUiTextBox()
    {
        return View("UI/TpUiTextBox");
    }
    /// <summary>
    /// Displays the tp.Memo demo.
    /// </summary>
    [Route("/demo/tp-ui-memo")]
    public IActionResult TpUiMemo()
    {
        return View("UI/TpUiMemo");
    }
    /// <summary>
    /// Displays the tp.ImageBox demo.
    /// </summary>
    [Route("/demo/tp-ui-image-box")]
    public IActionResult TpUiImageBox()
    {
        return View("UI/TpUiImageBox");
    }
    /// <summary>
    /// Displays the tp.AutocompleteList demo.
    /// </summary>
    [Route("/demo/tp-ui-autocomplete-list")]
    public IActionResult TpUiAutocompleteList()
    {
        return View("UI/TpUiAutocompleteList");
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
    /// Displays the tp.VirtualScroller demo.
    /// </summary>
    [Route("/demo/tp-ui-virtual-scroller")]
    public IActionResult TpUiVirtualScroller()
    {
        return View("UI/TpUiVirtualScroller");
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
