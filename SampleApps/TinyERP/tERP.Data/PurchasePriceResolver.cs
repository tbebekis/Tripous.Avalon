/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Resolves purchase prices from active ProductSupplier records.
/// </summary>
[TypeStore]
public class PurchasePriceResolver: IPriceResolver
{
    // ● private fields
    readonly SqlStore fStore = Db.DefaultStore;

    // ● private methods
    MemTable LoadPrices(PriceResolveArgs Args)
    {
        string SqlText = """
                         select
                             ProductSupplier.Id,
                             ProductSupplier.SupplierId,
                             ProductSupplier.LastCost
                         from ProductSupplier
                         where ProductSupplier.IsActive = 1
                           and ProductSupplier.ProductId = :ProductId
                           and ProductSupplier.LastCost is not null
                           and (ProductSupplier.SupplierId = :PersonId or ProductSupplier.IsDefault = 1)
                         order by
                             case when ProductSupplier.SupplierId = :PersonId then 0 else 1 end,
                             ProductSupplier.IsDefault desc,
                             ProductSupplier.Id
                         """;

        return fStore.Select(SqlText, new Dictionary<string, object>()
        {
            ["ProductId"] = Args.ProductId,
            ["PersonId"] = Args.PersonId,
        });
    }

    // ● construction
    /// <summary>
    /// Creates a new instance.
    /// </summary>
    public PurchasePriceResolver()
    {
    }

    // ● public
    /// <summary>
    /// Resolves the applicable product purchase price.
    /// </summary>
    public PriceResult Resolve(PriceResolveArgs Args)
    {
        if (Args == null)
            throw new ArgumentNullException(nameof(Args));

        PriceResult Result = new();
        if (Args.TradeType != TradeType.Purchases
            || string.IsNullOrWhiteSpace(Args.ProductId))
            return Result;

        MemTable Table = LoadPrices(Args);
        if (Table.Rows.Count == 0)
            return Result;

        DataRow Row = Table.Rows[0];
        Result.IsFound = true;
        Result.PriceListId = Row.AsString("Id");
        Result.CustomerId = Row.AsString("SupplierId");
        Result.UnitPrice = Row.AsDecimal("LastCost");
        Result.IsTaxIncluded = false;
        return Result;
    }
}
