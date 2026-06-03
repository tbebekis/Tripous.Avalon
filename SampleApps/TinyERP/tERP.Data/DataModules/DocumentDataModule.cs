/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// A data module capable of handling documents (i.e. transactions).
/// <para>A document module depends on the DocumentTypeId of its top-table in learning how to handle its documents.</para>
/// <para>NOTE: There is an one-to-one relationship between document handlers and document modules, based on their names.</para>
/// <para>That is, if there is a document module name SalesOrders there must be a document handler with the same name.</para>
/// </summary>
public class DocumentDataModule: DataModule
{
    public DocumentDataModule()
    {
    }
}