/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERPWeb.AjaxOps;

/// <summary>
/// Adds missing tERP sample data versions to the database.
/// </summary>
[AjaxOperation("App.AddSampleData")]
public class AddSampleData: AppAjaxOperation
{
    // ● public
    /// <summary>
    /// Executes the operation.
    /// </summary>
    public override AjaxResponse Execute(AjaxRequest Request, AjaxOperationContext Context)
    {
        AjaxResponse Result = new(Request.OperationName);
        SampleData[] NotAddedSampleData = SampleData.GetNotAdded();
        if (NotAddedSampleData.Length == 0)
        {
            Result["Success"] = true;
            Result["Message"] = "Sample data is already added.";
            Result["AddedVersions"] = Array.Empty<int>();
            return Result;
        }

        int[] Versions = NotAddedSampleData.Select(item => item.VersionNumber).ToArray();
        SampleData.AddSampleDataAsync(NotAddedSampleData).GetAwaiter().GetResult();

        Result["Success"] = true;
        Result["Message"] = "Sample data added.";
        Result["AddedVersions"] = Versions;
        return Result;
    }
}
