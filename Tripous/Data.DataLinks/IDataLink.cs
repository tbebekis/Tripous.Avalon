namespace Tripous.Data;

public interface IDataLink
{
    // ● public
    DataLinkResult Execute(DataLinkOp Op);
    Task<DataLinkResult> ExecuteAsync(DataLinkOp Op);

    Task<MemTable> Select(ModuleDef ModuleDef, SelectDef SelectDef);
    Task<MemTable> Select(string SqlText);
}