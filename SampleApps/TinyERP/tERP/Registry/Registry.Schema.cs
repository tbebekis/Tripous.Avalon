/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP;

static internal partial class Registry
{
    // ● schemas
    static internal void RegisterSchemas()
    {
        List<SchemaVersionDef> SchemaVersionList = [];
        SchemaVersionList.AddRange([new SchemaVersion1()]);
        
        foreach (SchemaVersionDef Version in SchemaVersionList)
            Version.Register();
    }
}