/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Saves a single system resource translation.
/// </summary>
[AjaxOperation("App.ResourceTranslations.Save")]
public class ResourceTranslationsSave: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            Sys.Throw("Access denied.");

        string LanguageId = GetStringParam(Request, "LanguageId");
        string ResKey = GetStringParam(Request, "ResKey");
        string ResValue = GetStringParam(Request, "ResValue");
        ResourceTranslationService.Save(Db.DefaultStore, LanguageId, ResKey, ResValue);

        AjaxResponse Result = new(Request.OperationName);
        Result["Saved"] = true;
        return Result;
    }
}
