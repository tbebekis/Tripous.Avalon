/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

[TypeStore]
public abstract class DocumentHandler
{
    // ● construction
    public DocumentHandler()
    {
    }

    // ● public
    public virtual void Validate(DocumentContext Context)
    {
        if (Context == null)
            throw new TripousDataException($"{nameof(Context)} is null.");
        if (Context.DataModule == null)
            throw new TripousDataException($"{nameof(Context.DataModule)} is null.");
        if (Context.Row == null)
            throw new TripousDataException($"{nameof(Context.Row)} is null.");
        if (string.IsNullOrWhiteSpace(Context.DocumentTypeId))
            throw new TripousDataException($"{nameof(Context.DocumentTypeId)} is empty.");
        if (string.IsNullOrWhiteSpace(Context.DocumentId))
            throw new TripousDataException($"{nameof(Context.DocumentId)} is empty.");
    }
    public virtual void Post(DocumentContext Context)
    {
    }
    public virtual void Cancel(DocumentContext Context)
    {
    }

    // ● properties
    public DocumentHandlerDef HandlerDef { get; set; }
}