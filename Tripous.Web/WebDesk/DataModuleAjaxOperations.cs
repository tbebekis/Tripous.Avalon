/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Base class for standard WebDesk data module Ajax operations.
/// </summary>
public abstract class DataModuleAjaxOperation: AjaxOperation
{
    // ● protected
    /// <summary>
    /// Returns the requested module name.
    /// </summary>
    protected string GetModuleName(AjaxRequest Request)
    {
        string Result = GetStringParam(Request, "ModuleName");
        if (string.IsNullOrWhiteSpace(Result))
            Result = GetStringParam(Request, "DataModuleName");
        if (string.IsNullOrWhiteSpace(Result))
            Result = GetStringParam(Request, "Module");
        if (string.IsNullOrWhiteSpace(Result))
            Sys.Throw("No DataModule ModuleName specified.");
        return Result;
    }
    /// <summary>
    /// Returns the registered module definition after applying access checks.
    /// </summary>
    protected ModuleDef GetModuleDef(AjaxRequest Request)
    {
        string ModuleName = GetModuleName(Request);
        ModuleDef Result = DataRegistry.Modules.Find(ModuleName);
        if (Result == null)
            Sys.Throw($"DataModule not found: {ModuleName}");

        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        if (!Result.CanAccess(User))
            Sys.Throw($"Access denied to DataModule: {ModuleName}");

        return Result;
    }
    /// <summary>
    /// Creates and returns the requested module.
    /// </summary>
    protected DataModule CreateModule(AjaxRequest Request)
    {
        ModuleDef Def = GetModuleDef(Request);
        return Def.Create();
    }
    /// <summary>
    /// Returns a JSON data module packet request parameter.
    /// </summary>
    protected JsonDataModule GetDataModulePacket(AjaxRequest Request)
    {
        object Value = Request.GetParam("DataModule");
        if (Value == null)
            Value = Request.GetParam("Packet");
        if (Value == null)
            Sys.Throw("No DataModule packet specified.");
        if (Value is JsonDataModule Packet)
            return Packet;
        if (Value is JsonElement Element)
            return Json.Deserialize<JsonDataModule>(Element.GetRawText());
        return Json.Deserialize<JsonDataModule>(Json.Serialize(Value));
    }
    /// <summary>
    /// Creates a response containing a full data module packet.
    /// </summary>
    protected AjaxResponse CreateDataModuleResponse(AjaxRequest Request, JsonDataModule Packet)
    {
        AjaxResponse Result = new(Request.OperationName);
        Result["DataModule"] = Packet;
        return Result;
    }
}

/// <summary>
/// Initializes a registered data module.
/// </summary>
[AjaxOperation("DataModule.Initialize")]
public class DataModuleInitialize: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        return CreateDataModuleResponse(Request, Module.JsonInitialize());
    }
}

/// <summary>
/// Returns the item list of a registered lookup source.
/// </summary>
[AjaxOperation("Lookup.GetList")]
public class LookupGetList: AjaxOperation
{
    // ● private
    /// <summary>
    /// Returns the transport type for lookup item values.
    /// </summary>
    Type GetValueType(List<LookupItem> Items)
    {
        foreach (LookupItem Item in Items)
        {
            if (Item.Value == null)
                continue;

            Type Result = Item.Value.GetType();
            return Result.IsEnum ? typeof(int) : Result;
        }

        return typeof(string);
    }
    /// <summary>
    /// Creates the transport table for lookup items.
    /// </summary>
    DataTable CreateLookupTable(string Name, List<LookupItem> Items)
    {
        DataTable Result = new(Name);
        Result.Columns.Add("Id", GetValueType(Items));
        Result.Columns.Add("Name", typeof(string));

        foreach (LookupItem Item in Items)
        {
            DataRow Row = Result.NewRow();
            Row["Id"] = Item.Value ?? DBNull.Value;
            Row["Name"] = Item.DisplayText ?? string.Empty;
            Result.Rows.Add(Row);
        }

        Result.AcceptChanges();
        return Result;
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        string LookupName = GetStringParam(Request, "LookupName");
        if (string.IsNullOrWhiteSpace(LookupName))
            LookupName = GetStringParam(Request, "Name");
        if (string.IsNullOrWhiteSpace(LookupName))
            Sys.Throw("No lookup name specified.");

        LookupDef Def = DataRegistry.Lookups.Find(LookupName);
        if (Def == null)
            Sys.Throw($"Lookup not found: {LookupName}");

        LookupSource Source = Def.Create();
        DataTable Table = CreateLookupTable(LookupName, Source.GetList());
        AjaxResponse Result = new(Request.OperationName);
        Result["Table"] = new JsonDataTable(Table);
        return Result;
    }
}

/// <summary>
/// Returns the FactBox HTML for a registered data module item page.
/// </summary>
[AjaxOperation("DataModule.GetFactBoxes")]
public class DataModuleGetFactBoxes: DataModuleAjaxOperation
{
    // ● private
    /// <summary>
    /// Returns the requested web form definition, if any.
    /// </summary>
    WebFormDef GetWebFormDef(AjaxRequest Request)
    {
        string FormName = GetStringParam(Request, "WebFormName");
        if (string.IsNullOrWhiteSpace(FormName))
            FormName = GetStringParam(Request, "FormName");
        if (string.IsNullOrWhiteSpace(FormName))
            FormName = GetStringParam(Request, "Form");
        return string.IsNullOrWhiteSpace(FormName) ? null : WebDeskRegistry.FindForm(FormName);
    }
    /// <summary>
    /// Returns true when the FactBox pane is enabled by configuration.
    /// </summary>
    bool GetFactBoxPaneEnabled()
    {
        try
        {
            string Text = Config.GetValue(Config.SShowDataFormFactBoxPane);
            return string.IsNullOrWhiteSpace(Text) || Convert.ToBoolean(Text, CultureInfo.InvariantCulture);
        }
        catch
        {
            return true;
        }
    }
    /// <summary>
    /// Returns the effective JavaScript form class name.
    /// </summary>
    string GetFormJsClassName(WebFormDef Form)
    {
        return Form != null && !string.IsNullOrWhiteSpace(Form.JsFormClassType) ? Form.JsFormClassType : "tp.WebDataForm";
    }
    /// <summary>
    /// Returns the effective JavaScript item page class name.
    /// </summary>
    string GetItemPageJsClassName(WebFormDef Form)
    {
        return Form != null && !string.IsNullOrWhiteSpace(Form.ItemViewName) ? "tp.WebItemPageBuilder" : string.Empty;
    }
    /// <summary>
    /// Returns the effective Razor view path.
    /// </summary>
    string GetViewPath(string ViewName)
    {
        if (string.IsNullOrWhiteSpace(ViewName))
            return string.Empty;
        if (ViewName.StartsWith("/", StringComparison.Ordinal))
            return ViewName;

        string ViewFileName = ViewName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase) ? ViewName : $"{ViewName}.cshtml";
        return $"/Views/WebForms/{ViewFileName}";
    }
    /// <summary>
    /// Returns the visible FactBox definitions for the module and web form.
    /// </summary>
    /// <param name="ModuleDef">The module definition.</param>
    /// <param name="Form">The web form definition.</param>
    /// <returns>The visible FactBox definitions.</returns>
    List<ItemFactBoxDef> GetVisibleFactBoxes(ModuleDef ModuleDef, WebFormDef Form)
    {
        List<ItemFactBoxDef> Result = [];

        void AddRange(DefList<ItemFactBoxDef> List)
        {
            if (List == null)
                return;

            foreach (ItemFactBoxDef Def in List)
            {
                if (Def.IsVisible && !Result.Any(Item => Sys.IsSameText(Item.Name, Def.Name)))
                    Result.Add(Def);
            }
        }

        if (ModuleDef != null)
            AddRange(ModuleDef.FactBoxes);
        if (Form != null)
            AddRange(Form.FactBoxes);

        return Result;
    }

    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        WebFormDef Form = GetWebFormDef(Request);
        ModuleDef ModuleDef = GetModuleDef(Request);
        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        if (Form != null && !Form.CanAccess(User))
            Sys.Throw($"Access denied to WebForm: {Form.Name}");

        AjaxResponse Result = new(Request.OperationName);
        if (!GetFactBoxPaneEnabled())
        {
            Result["Html"] = string.Empty;
            Result["FactBoxCount"] = 0;
            Result["ShowPane"] = false;
            return Result;
        }

        object KeyValue = Request.GetParam("KeyValue");
        List<ItemFactBoxDef> CustomFactBoxes = GetVisibleFactBoxes(ModuleDef, Form);
        DataModule Module = ModuleDef.Create();
        if (CustomFactBoxes.Count > 0 && !Sys.IsNull(KeyValue))
            Module.Edit(KeyValue);

        ItemFactBoxContext FactBoxContext = new()
        {
            FormName = Form != null ? Form.Name : string.Empty,
            FormClassName = GetViewPath(Form?.ViewName),
            FormJsClassName = GetFormJsClassName(Form),
            ItemPageClassName = GetViewPath(Form?.ItemViewName),
            ItemPageJsClassName = GetItemPageJsClassName(Form),
            Module = Module,
            Row = Module.tblItem?.CurrentRow,
            KeyValue = KeyValue,
            RowState = GetStringParam(Request, "RowState")
        };

        string Html = new ItemFactBoxHtmlRenderer(Context.ViewToStringConverter).Render(FactBoxContext, CustomFactBoxes);

        Result["Html"] = Html;
        Result["FactBoxCount"] = 1 + CustomFactBoxes.Count;
        Result["ShowPane"] = false;
        return Result;
    }
}

/// <summary>
/// Starts an insert operation on a registered data module.
/// </summary>
[AjaxOperation("DataModule.Insert")]
public class DataModuleInsert: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        return CreateDataModuleResponse(Request, Module.JsonInsert());
    }
}

/// <summary>
/// Starts an edit operation on a registered data module.
/// </summary>
[AjaxOperation("DataModule.Edit")]
public class DataModuleEdit: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        string Id = GetStringParam(Request, "Id");
        return CreateDataModuleResponse(Request, Module.JsonEdit(Id));
    }
}

/// <summary>
/// Deletes an item through a registered data module.
/// </summary>
[AjaxOperation("DataModule.Delete")]
public class DataModuleDelete: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        string Id = GetStringParam(Request, "Id");
        Module.JsonDelete(Id);

        AjaxResponse Result = new(Request.OperationName);
        Result["Success"] = true;
        return Result;
    }
}

/// <summary>
/// Commits a data module item packet.
/// </summary>
[AjaxOperation("DataModule.Commit")]
public class DataModuleCommit: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        JsonDataModule Packet = GetDataModulePacket(Request);
        return CreateDataModuleResponse(Request, Module.JsonCommit(Packet));
    }
}

/// <summary>
/// Selects a data module list table.
/// </summary>
[AjaxOperation("DataModule.SelectList")]
public class DataModuleSelectList: DataModuleAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        DataModule Module = CreateModule(Request);
        string SelectName = GetStringParam(Request, "SelectName");
        JsonSelectFilters Filters = JsonSelectFilters.From(Request.GetParam("Filters"));
        JsonDataTable Table = Module.JsonSelectList(SelectName, Filters);

        AjaxResponse Result = new(Request.OperationName);
        Result["Table"] = Table;
        Result["SelectName"] = SelectName;
        return Result;
    }
}
