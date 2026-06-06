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
    // ● protected
    protected override CodeProviderDef GetCodeProviderDef() => IsPosting? FinalCodeProviderDef: DraftCodeProviderDef;
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

        if (Table == tblItem)
        {
            if (IsInserting)
            {
                Row["TradeTypeId"] = DocumentType.TradeTypeId;
            }
        }
    }
    
    // ● construction
    /// <summary>
    /// Constructor
    /// </summary>
    public DocumentDataModule()
    {
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
    public DocumentType DocumentType { get; protected set; }
}