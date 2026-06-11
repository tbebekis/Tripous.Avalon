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
/// <para>That is, if there is a document module named SalesOrders there must be a document handler with the same name.</para>
/// </summary>
public class DocumentDataModule: AppDataModule
{
    // ● protected fields
    protected int fIsTransforming;
    
    // ● protected
    protected override CodeProviderDef GetCodeProviderDef() => IsPosting? FinalCodeProviderDef: DraftCodeProviderDef;
    /// <summary>
    /// Creates the document handler registered for the current module.
    /// </summary>
    protected virtual DocumentHandler CreateDocumentHandler()
    {
        DocumentHandlerDef HandlerDef = DataRegistry.DocumentHandlers.Find(ModuleDef.Name);
        if (HandlerDef == null)
            throw new TripousDataException($"No document handler is registered for module '{ModuleDef.Name}'.");

        DocumentHandler Result = TypeStore.CreateInstance<DocumentHandler>(HandlerDef.ClassName);
        if (Result == null)
            throw new TripousDataException($"Cannot create document handler '{HandlerDef.ClassName}'.");

        Result.HandlerDef = HandlerDef;
        return Result;
    }
    /// <summary>
    /// Creates the context passed to the document handler.
    /// </summary>
    protected virtual DocumentContext CreateDocumentContext()
    {
        if (CurrentRow == null)
            throw new TripousBusinessException("No document is selected.");

        return new DocumentContext()
        {
            DataModule = this,
            Row = CurrentRow,
            DocumentTypeId = CurrentRow.AsString("DocumentTypeId"),
            DocumentId = CurrentRow.AsString("Id"),
            IsPosting = IsPosting,
            IsCancellation = DocumentType.IsCancellation,
        };
    }
    /// <summary>
    /// Creates a snapshot of values changed by the posting operation.
    /// </summary>
    protected virtual Dictionary<string, object> CreatePostingSnapshot(DataRow Row)
    {
        Dictionary<string, object> Result = new();
        string[] FieldNames = ["Code", "TradeStatusId", "PostingDate", "PostedAt", "PostedBy", "IsLocked"];

        foreach (string FieldName in FieldNames)
        {
            if (Row.Table.Columns.Contains(FieldName))
                Result[FieldName] = Row[FieldName];
        }

        return Result;
    }
    /// <summary>
    /// Restores values changed by a failed posting operation.
    /// </summary>
    protected virtual void RestorePostingSnapshot(DataRow Row, Dictionary<string, object> Snapshot)
    {
        if (Row == null || Row.RowState == DataRowState.Deleted || Row.RowState == DataRowState.Detached)
            return;

        foreach (KeyValuePair<string, object> Entry in Snapshot)
            Row.SetValue(Entry.Key, Entry.Value);
    }
    /// <summary>
    /// Assigns the final document code while posting.
    /// </summary>
    protected override void AssignCodeValue(DbTransaction Transaction)
    {
        if (!IsPosting)
        {
            base.AssignCodeValue(Transaction);
            return;
        }

        if (FinalCodeProviderDef == null || tblItem == null || !tblItem.ContainsColumn("Code"))
            return;

        foreach (DataRow Row in tblItem.Rows)
        {
            if (Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
                Row.SetValue("Code", GetNextCodeLocked(Transaction));
        }
    }
    /// <summary>
    /// Returns the final code provider def, i.e. the one where its pattern does not start with "DRAFT-"
    /// </summary>
    protected virtual CodeProviderDef GetFinalCodeProviderDefFromDocumentType()
    {
        string SqlText = $"""
                          select
                            NumberSeries.Code
                          from
                            DocumentType
                              inner join {DbConfig.SysNumberSeriesTableName} NumberSeries on NumberSeries.Id = DocumentType.NumberSeriesId
                          where
                            DocumentType.ModuleName = :ModuleName
                          """;

        DataRow Row = Store.SelectResults(SqlText, new Dictionary<string, object>()
        {
            ["ModuleName"] = ModuleDef.Name,
        });

        if (Row == null)
            return null;

        string ProviderName = Row.AsString("Code");

        if (string.IsNullOrWhiteSpace(ProviderName))
            return null;

        return DataRegistry.CodeProviders.Get(ProviderName);
    }
    /// <summary>
    /// Assigns the <see cref="CodeProviderDef"/>, <see cref="DraftCodeProviderDef"/> and <see cref="FinalCodeProviderDef"/> property.
    /// </summary>
    protected override void AssignCodeProviderDef()
    {
        DocumentType = new DocumentType(ModuleDef.Name);
        
        base.AssignCodeProviderDef();

        if (CodeProviderDef == null)
            throw new TripousDataException($"{nameof(CodeProviderDef)} is null.");

        DraftCodeProviderDef = null;
        FinalCodeProviderDef = GetFinalCodeProviderDefFromDocumentType();

        if (CodeProviderDef.Name.StartsWith("DRAFT-", StringComparison.OrdinalIgnoreCase))
        {
            DraftCodeProviderDef = CodeProviderDef;

            if (FinalCodeProviderDef == null)
            {
                string ProviderName = CodeProviderDef.Name.Substring("DRAFT-".Length);
                FinalCodeProviderDef = DataRegistry.CodeProviders.Get(ProviderName);
            }
        }
        else
        {
            FinalCodeProviderDef ??= CodeProviderDef;
            DraftCodeProviderDef = DataRegistry.CodeProviders.Get($"DRAFT-{CodeProviderDef.Name}");
        }

        if (DraftCodeProviderDef == null)
            throw new TripousDataException($"{nameof(DraftCodeProviderDef)} is null.");
        if (FinalCodeProviderDef == null)
            throw new TripousDataException($"{nameof(FinalCodeProviderDef)} is null.");
    }
    /// <summary>
    /// Sets default values to the Row. It is called when a commit operation starts.
    /// </summary>
    protected override void SetDefaultValues(DataTable Table, DataRow Row, TableDef TableDef)
    {
        base.SetDefaultValues(Table, Row, TableDef);

        if (Row.RowState == DataRowState.Deleted)
            return;

        if (Table == tblItem && IsInserting)
        {
            Row.SetValue("TradeTypeId", DocumentType.TradeTypeId);
        }
    }

    protected override void NewRowAdded(MemTable Table, DataTableNewRowEventArgs ea)
    {
        base.NewRowAdded(Table, ea);

        if (Table == tblItem || IsTransforming || !Table.ContainsColumn("DisplayOrder"))
            return;

        int DisplayOrder = Table.Rows
            .Cast<DataRow>()
            .Where(Row => Row != ea.Row && Row.RowState != DataRowState.Deleted && Row.RowState != DataRowState.Detached)
            .Select(Row => Row.AsInteger("DisplayOrder"))
            .DefaultIfEmpty(0)
            .Max();

        ea.Row.SetValue("DisplayOrder", DisplayOrder + 10);
    }

    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public DocumentDataModule()
    {
    }

    
    // ● public
    public override void CheckCanCommit(bool Reselect)
    {
        base.CheckCanCommit(Reselect);

        if (!IsPosting && CurrentRow != null && CurrentRow.AsBoolean("IsLocked"))
            throw new TripousBusinessException("A locked document cannot be saved.");
    }
    public override void Initialize(ModuleDef ModuleDef)
    {
        base.Initialize(ModuleDef);

        Handler = CreateDocumentHandler();

        foreach (MemTable Table in ItemTables)
        {
            if (Table.ContainsColumn("DisplayOrder"))
            {
                Table.DataView.Sort = "DisplayOrder ASC, Id ASC";
            }
        }
    }
    /// <summary>
    /// Validates, posts and commits the current document.
    /// </summary>
    public virtual object Post(bool Reselect = true)
    {
        DocumentContext Context = CreateDocumentContext();
        Dictionary<string, object> Snapshot = CreatePostingSnapshot(Context.Row);

        IsPosting = true;
        Context.IsPosting = true;
        try
        {
            Handler.Validate(Context);
            Handler.Post(Context);
            return Commit(Reselect);
        }
        catch
        {
            RestorePostingSnapshot(Context.Row, Snapshot);
            throw;
        }
        finally
        {
            Context.IsPosting = false;
            IsPosting = false;
        }
    }

    // ● properties
    /// <summary>
    /// The draft code provider, the one where its pattern starts with "DRAFT-"
    /// </summary>
    public CodeProviderDef DraftCodeProviderDef { get; protected set; }
    /// <summary>
    /// The POST code provider.
    /// </summary>
    public CodeProviderDef FinalCodeProviderDef { get; protected set; }
    /// <summary>
    /// True while posting a document.
    /// </summary>
    public bool IsPosting { get; protected set; }
    /// <summary>
    /// The handler registered for this document module.
    /// </summary>
    public DocumentHandler Handler { get; protected set; }
    public DocumentType DocumentType { get; protected set; }
    /// <summary>
    /// True while transforming from one type of a document to an other.
    /// </summary>
    public bool IsTransforming
    {
        get => fIsTransforming > 0;
        protected set
        {
            if (value)
                fIsTransforming++;
            else
                fIsTransforming--;

            if (fIsTransforming < 0)
                fIsTransforming = 0;
        }
    }
}
