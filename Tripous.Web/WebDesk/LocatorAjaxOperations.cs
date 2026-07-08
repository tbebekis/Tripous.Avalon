/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.WebDesk;

/// <summary>
/// Base class for standard WebDesk locator Ajax operations.
/// </summary>
public abstract class LocatorAjaxOperation: AjaxOperation
{
    // ● protected
    /// <summary>
    /// Returns the requested locator definition.
    /// </summary>
    protected LocatorDef GetLocatorDef(AjaxRequest Request)
    {
        LocatorContext Context = GetLocatorContext(Request);
        LocatorDef Result = DataRegistry.GetLocator(Context.LocatorName);
        Result.CheckDescriptor();
        return Result;
    }
    /// <summary>
    /// Returns true when a module access check is allowed.
    /// </summary>
    protected bool CanAccess(ModuleDef ModuleDef)
    {
        AppUser User = Sys.Context != null ? Sys.Context.CurrentUser : null;
        return ModuleDef != null && ModuleDef.CanAccess(User);
    }
    /// <summary>
    /// Returns a boolean request parameter.
    /// </summary>
    protected bool GetBooleanParam(AjaxRequest Request, string Name)
    {
        object Value = Request.GetParam(Name);
        if (Value == null)
            return false;
        if (Value is JsonElement Element)
        {
            if (Element.ValueKind == JsonValueKind.True)
                return true;
            if (Element.ValueKind == JsonValueKind.False)
                return false;
            if (Element.ValueKind == JsonValueKind.Number)
                return Element.GetInt32() != 0;
            if (Element.ValueKind == JsonValueKind.String)
                return Sys.IsSameText(Element.GetString(), "true");
            return false;
        }
        return Convert.ToBoolean(Value, CultureInfo.InvariantCulture);
    }
    /// <summary>
    /// Returns a normalized request parameter value.
    /// </summary>
    protected object GetValueParam(AjaxRequest Request, string Name)
    {
        object Value = Request.GetParam(Name);
        if (Value is not JsonElement Element)
            return Value;

        switch (Element.ValueKind)
        {
            case JsonValueKind.String:
                return Element.GetString();
            case JsonValueKind.Number:
                if (Element.TryGetInt32(out int IntValue))
                    return IntValue;
                if (Element.TryGetInt64(out long LongValue))
                    return LongValue;
                if (Element.TryGetDecimal(out decimal DecimalValue))
                    return DecimalValue;
                return Element.GetDouble();
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
                return false;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                return null;
            default:
                return Element.ToString();
        }
    }
    /// <summary>
    /// Returns a locator context request parameter.
    /// </summary>
    protected LocatorContext GetLocatorContext(AjaxRequest Request)
    {
        object Value = Request.GetParam("Context");
        LocatorContext Result = null;

        if (Value is LocatorContext Context)
            Result = Context;
        else if (Value is JsonElement Element)
            Result = Json.Deserialize<LocatorContext>(Element.GetRawText());
        else if (Value != null)
            Result = Json.Deserialize<LocatorContext>(Json.Serialize(Value));

        Result ??= new();

        string LocatorName = GetStringParam(Request, "LocatorName");
        if (!string.IsNullOrWhiteSpace(LocatorName))
            Result.LocatorName = LocatorName;

        if (string.IsNullOrWhiteSpace(Result.LocatorName))
            Sys.Throw("No LocatorName specified.");

        return Result;
    }
    /// <summary>
    /// Creates a locator request from Ajax request parameters.
    /// </summary>
    protected LocatorRequest CreateLocatorRequest(AjaxRequest Request)
    {
        return new LocatorRequest()
        {
            KeyValue = GetValueParam(Request, "KeyValue"),
            SearchTerm = GetStringParam(Request, "SearchTerm"),
            SearchField = GetStringParam(Request, "SearchField"),
            IsMultiRow = GetBooleanParam(Request, "IsMultiRow"),
            Context = GetLocatorContext(Request),
        };
    }
    /// <summary>
    /// Returns a target table definition for optional mapping.
    /// </summary>
    protected TableDef FindTargetTable(AjaxRequest Request)
    {
        string ModuleName = GetStringParam(Request, "ModuleName");
        string TableName = GetStringParam(Request, "TableName");

        if (string.IsNullOrWhiteSpace(TableName) && string.IsNullOrWhiteSpace(ModuleName))
            return null;

        if (!string.IsNullOrWhiteSpace(ModuleName))
        {
            ModuleDef ModuleDef = DataRegistry.Modules.Find(ModuleName);
            if (ModuleDef == null)
                Sys.Throw($"DataModule not found: {ModuleName}");
            if (!CanAccess(ModuleDef))
                Sys.Throw($"Access denied to DataModule: {ModuleName}");

            if (string.IsNullOrWhiteSpace(TableName))
                return ModuleDef.Table;

            return ModuleDef.GetTables().FirstOrDefault(item => item.Name.IsSameText(TableName));
        }

        foreach (ModuleDef ModuleDef in DataRegistry.Modules)
        {
            TableDef TableDef = ModuleDef.GetTables().FirstOrDefault(item => item.Name.IsSameText(TableName));
            if (TableDef != null)
                return TableDef;
        }

        return null;
    }
    /// <summary>
    /// Finds the target reference field.
    /// </summary>
    protected FieldDef FindReferenceField(TableDef TargetTable, string ReferenceFieldName)
    {
        if (TargetTable == null || string.IsNullOrWhiteSpace(ReferenceFieldName))
            return null;

        return TargetTable.Fields.FirstOrDefault(item =>
            item.Name.IsSameText(ReferenceFieldName) || item.Alias.IsSameText(ReferenceFieldName));
    }
    /// <summary>
    /// Creates a JSON locator mapping plan when target context is supplied.
    /// </summary>
    protected JsonLocatorMapPlan CreateMapPlan(AjaxRequest Request, LocatorDef LocatorDef)
    {
        TableDef TargetTable = FindTargetTable(Request);
        string ReferenceFieldName = GetStringParam(Request, "ReferenceField");

        if (TargetTable == null || string.IsNullOrWhiteSpace(ReferenceFieldName))
            return null;

        FieldDef ReferenceField = FindReferenceField(TargetTable, ReferenceFieldName);
        if (ReferenceField == null)
            Sys.Throw($"Reference field not found: {ReferenceFieldName}");

        LocatorMapPlan Plan = new LocatorMapper().CreatePlan(LocatorDef, TargetTable, ReferenceField);
        return new JsonLocatorMapPlan(Plan);
    }
}

/// <summary>
/// Executes a locator request.
/// </summary>
[AjaxOperation("Locator.Execute")]
public class LocatorExecute: LocatorAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        LocatorRequest LocatorRequest = CreateLocatorRequest(Request);
        LocatorDef LocatorDef = DataRegistry.GetLocator(LocatorRequest.Context.LocatorName);
        LocatorResult LocatorResult = Locators.Execute(LocatorRequest);

        AjaxResponse Result = new(Request.OperationName);
        Result["Status"] = LocatorResult.Status.ToString();
        Result["Message"] = LocatorResult.Message;
        Result["Count"] = LocatorResult.Count;
        Result["WebForm"] = LocatorDef.WebForm;
        Result["Table"] = LocatorResult.Table != null ? new JsonDataTable(LocatorResult.Table) : null;
        Result["MapPlan"] = CreateMapPlan(Request, LocatorDef);
        return Result;
    }
}

/// <summary>
/// Returns locator metadata.
/// </summary>
[AjaxOperation("Locator.GetInfo")]
public class LocatorGetInfo: LocatorAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        LocatorDef LocatorDef = GetLocatorDef(Request);

        AjaxResponse Result = new(Request.OperationName);
        Result["Locator"] = new JsonLocatorDef(LocatorDef);
        Result["MapPlan"] = CreateMapPlan(Request, LocatorDef);
        return Result;
    }
}
