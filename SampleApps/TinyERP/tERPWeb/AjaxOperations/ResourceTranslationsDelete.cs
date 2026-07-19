/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Deletes all system resource translations for a resource key.
/// </summary>
[AjaxOperation("App.ResourceTranslations.Delete")]
public class ResourceTranslationsDelete: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            Sys.Throw("Access denied.");

        string ResKey = GetStringParam(Request, "ResKey");
        ResourceTranslationService.DeleteResourceKey(Db.DefaultStore, ResKey);

        AjaxResponse Result = new(Request.OperationName);
        Result["Deleted"] = true;
        return Result;
    }
}
