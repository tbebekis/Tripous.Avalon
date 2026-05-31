namespace tERP.Data;

public class DocumentContext
{
    public DocumentDataModule DataModule { get; set; }

    public DataRow Row { get; set; }

    public string DocumentTypeId { get; set; }

    public string DocumentId { get; set; }

    public bool IsPosting { get; set; }
    public bool IsCancellation { get; set; }
}

[TypeStore]
public abstract class DocumentHandler
{
    public virtual void Validate(DocumentContext Context)
    {
    }
    public virtual void Post(DocumentContext Context)
    {
    }
    public virtual void Cancel(DocumentContext Context)
    {
    }
}

public class DocumentDataModule: DataModule
{
    public DocumentDataModule()
    {
    }
}

public class DocumentTypeDataModule: DocumentDataModule
{
    public DocumentTypeDataModule()
    {
    }
}

public class TradeDataModule: DocumentDataModule
{
    public TradeDataModule()
    {
    }
}

public class StockTradeDataModule: DocumentDataModule
{
    public StockTradeDataModule()
    {
    }
}

public class StockMovementDataModule: DocumentDataModule
{
    public StockMovementDataModule()
    {
    }
}

public class StockBalanceDataModule: DocumentDataModule
{
    public StockBalanceDataModule()
    {
    }
}

public class StockCountDataModule: DocumentDataModule
{
    public StockCountDataModule()
    {
    }
}

public class StockReservationDataModule: DocumentDataModule
{
    public StockReservationDataModule()
    {
    }
}

public class FinanceMovementDataModule: DocumentDataModule
{
    public FinanceMovementDataModule()
    {
    }
}

public class AccountDataModule: DocumentDataModule
{
    public AccountDataModule()
    {
    }
}

public class JournalEntryDataModule: DocumentDataModule
{
    public JournalEntryDataModule()
    {
    }
}

public class AssetDataModule: DocumentDataModule
{
    public AssetDataModule()
    {
    }
}

public class FinanceBalanceDataModule: DocumentDataModule
{
    public FinanceBalanceDataModule()
    {
    }
}

