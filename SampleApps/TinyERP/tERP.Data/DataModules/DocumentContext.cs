/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

[TypeStore]
public class DocumentContext
{
    // ● properties
    public DocumentDataModule DataModule { get; set; }
    public DataRow Row { get; set; }
    public string DocumentTypeId { get; set; }
    public string DocumentId { get; set; }
    public bool IsPosting { get; set; }
    public bool IsCancellation { get; set; }
}
