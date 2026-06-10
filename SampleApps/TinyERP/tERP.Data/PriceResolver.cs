/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Resolves sales prices from active PriceList records.
/// </summary>
/// <remarks>
/// Customer-specific rows take precedence over generic rows. Within the
/// selected scope, the highest applicable minimum quantity and the most
/// recent validity date take precedence.
/// </remarks>
[TypeStore]
public class PriceResolver: IPriceResolver
{
    // ● private fields
    readonly SqlStore fStore = Db.DefaultStore;

    // ● private methods
    MemTable LoadPrices(PriceResolveArgs Args, DateTime TradeDate)
    {
        string SqlText = @"
select
    PL.Id,
    PL.PriceListTypeId,
    PL.DiscountCategoryId,
    PL.CustomerId,
    PL.MinQuantity,
    PL.UnitPrice,
    PLT.CurrencyId,
    PLT.IsTaxIncluded
from
    PriceList PL
    inner join PriceListType PLT on PLT.Id = PL.PriceListTypeId
where
    PL.IsActive = 1
    and PLT.IsActive = 1
    and PL.PriceListTypeId = :PriceListTypeId
    and PL.ProductId = :ProductId
    and PL.UnitOfMeasureId = :UnitOfMeasureId
    and PLT.CurrencyId = :CurrencyId
    and PL.MinQuantity <= :Quantity
    and (PL.CustomerId = :PersonId or PL.CustomerId is null)
    and (PL.ValidFrom is null or PL.ValidFrom <= :TradeDate)
    and (PL.ValidTo is null or PL.ValidTo >= :TradeDate)
order by
    case when PL.CustomerId = :PersonId then 0 else 1 end,
    PL.MinQuantity desc,
    case when PL.ValidFrom is null then 1 else 0 end,
    PL.ValidFrom desc,
    PL.Id
";

        return fStore.Select(SqlText, new Dictionary<string, object>()
        {
            ["PriceListTypeId"] = Args.PriceListTypeId,
            ["ProductId"] = Args.ProductId,
            ["UnitOfMeasureId"] = Args.UnitOfMeasureId,
            ["CurrencyId"] = Args.CurrencyId,
            ["Quantity"] = Math.Abs(Args.Quantity),
            ["PersonId"] = Args.PersonId,
            ["TradeDate"] = TradeDate.Date,
        });
    }

    // ● construction
    public PriceResolver()
    {
    }

    // ● public
    /// <summary>
    /// Resolves the applicable product price.
    /// </summary>
    public PriceResult Resolve(PriceResolveArgs Args)
    {
        if (Args == null)
            throw new ArgumentNullException(nameof(Args));

        PriceResult Result = new();
        if (Args.TradeType != TradeType.Sales
            || string.IsNullOrWhiteSpace(Args.PriceListTypeId)
            || string.IsNullOrWhiteSpace(Args.ProductId)
            || string.IsNullOrWhiteSpace(Args.UnitOfMeasureId)
            || string.IsNullOrWhiteSpace(Args.CurrencyId))
            return Result;

        DateTime TradeDate = Args.TradeDate == DateTime.MinValue ? DateTime.Today : Args.TradeDate;
        MemTable Table = LoadPrices(Args, TradeDate);
        if (Table.Rows.Count == 0)
            return Result;

        DataRow Row = Table.Rows[0];
        Result.IsFound = true;
        Result.PriceListId = Row.AsString("Id");
        Result.PriceListTypeId = Row.AsString("PriceListTypeId");
        Result.CurrencyId = Row.AsString("CurrencyId");
        Result.DiscountCategoryId = Row.AsString("DiscountCategoryId");
        Result.CustomerId = Row.AsString("CustomerId");
        Result.MinQuantity = Row.AsDecimal("MinQuantity");
        Result.UnitPrice = Row.AsDecimal("UnitPrice");
        Result.IsTaxIncluded = Row.AsBoolean("IsTaxIncluded");

        return Result;
    }
}
