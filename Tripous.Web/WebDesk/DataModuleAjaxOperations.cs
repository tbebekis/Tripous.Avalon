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
