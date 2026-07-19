/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Loads system resource translations for the web translation editor.
/// </summary>
[AjaxOperation("App.ResourceTranslations.Load")]
public class ResourceTranslationsLoad: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        if (!CanCurrentUserAccess(UserLevel.Admin))
            Sys.Throw("Access denied.");

        ResourceTranslationTable TranslationTable = ResourceTranslationService.Load(Db.DefaultStore);
        AjaxResponse Result = new(Request.OperationName);
        Result["Table"] = new JsonDataTable(TranslationTable.Table);
        Result["Languages"] = TranslationTable.Languages.Select(Language => new
        {
            Language.Id,
            Language.Code,
            Language.Name,
            Language.CultureName,
            Language.ColumnName,
            Language.IsEnglish
        }).ToArray();
        return Result;
    }
}
