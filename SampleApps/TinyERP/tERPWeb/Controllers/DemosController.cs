using Microsoft.AspNetCore.Mvc;
using System.Data;
using Tripous.Data;

namespace tERPWeb.Controllers;

/// <summary>
/// Provides server-side Tripous Web demos.
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
    /// Displays the server-side demos index page.
    /// </summary>
    [Route("/demos")]
    public IActionResult Index()
    {
        return View();
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
}
