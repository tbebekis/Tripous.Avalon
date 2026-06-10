/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Resolves indirect taxes by evaluating active <c>TaxRule</c> records.
/// </summary>
/// <remarks>
/// The resolver is country-neutral. Country, tax-zone, state, county, city,
/// and special-district behavior is defined by tax setup data rather than
/// hard-coded country rules.
/// </remarks>
[TypeStore]
public class TaxResolver: ITaxResolver
{
    // ● private fields
    readonly SqlStore fStore = Db.DefaultStore;

    // ● private methods
    decimal RoundAmount(decimal Value) => Math.Round(Value, 4, MidpointRounding.AwayFromZero);
    bool IsAddressMatch(DataRow Row, PersonAddress Address)
    {
        string CountryId = Row.AsString("CountryId");
        string RegionCode = Row.AsString("RegionCode");
        string PostalCodePattern = Row.AsString("PostalCodePattern");

        if (!string.IsNullOrWhiteSpace(CountryId) && !CountryId.IsSameText(Address.CountryId))
            return false;
        if (!string.IsNullOrWhiteSpace(RegionCode) && !RegionCode.IsSameText(Address.Region))
            return false;
        if (!IsPostalCodeMatch(PostalCodePattern, Address.PostalCode))
            return false;

        return !string.IsNullOrWhiteSpace(CountryId);
    }
    bool IsPostalCodeMatch(string Pattern, string PostalCode)
    {
        if (string.IsNullOrWhiteSpace(Pattern))
            return true;
        if (string.IsNullOrWhiteSpace(PostalCode))
            return false;
        if (Pattern.EndsWith("*", StringComparison.Ordinal))
            return PostalCode.StartsWith(Pattern[..^1], StringComparison.OrdinalIgnoreCase);

        return Pattern.IsSameText(PostalCode);
    }
    int GetJurisdictionSpecificity(DataRow Row)
    {
        int Result = 0;

        if (!string.IsNullOrWhiteSpace(Row.AsString("CountryId")))
            Result += 1;
        if (!string.IsNullOrWhiteSpace(Row.AsString("RegionCode")))
            Result += 10;
        if (!string.IsNullOrWhiteSpace(Row.AsString("PostalCodePattern")))
            Result += 100;

        return Result;
    }
    DataRow FindJurisdiction(MemTable Table, string JurisdictionId)
    {
        if (string.IsNullOrWhiteSpace(JurisdictionId))
            return null;

        return Table.Rows.Cast<DataRow>().FirstOrDefault(Row => Row.AsString("Id").IsSameText(JurisdictionId));
    }
    DataRow ResolveJurisdiction(MemTable Table, string JurisdictionId, PersonAddress Address)
    {
        DataRow ExplicitRow = FindJurisdiction(Table, JurisdictionId);
        if (ExplicitRow != null)
            return ExplicitRow;
        if (Address == null || string.IsNullOrWhiteSpace(Address.CountryId))
            return null;

        return Table.Rows.Cast<DataRow>()
            .Where(Row => IsAddressMatch(Row, Address))
            .OrderByDescending(GetJurisdictionSpecificity)
            .ThenByDescending(Row => Row.AsInteger("JurisdictionTypeId"))
            .FirstOrDefault();
    }
    List<string> GetJurisdictionPath(MemTable Table, DataRow Row)
    {
        List<string> Result = [];
        HashSet<string> Visited = new(StringComparer.OrdinalIgnoreCase);

        while (Row != null)
        {
            string Id = Row.AsString("Id");
            if (string.IsNullOrWhiteSpace(Id) || !Visited.Add(Id))
                break;

            Result.Add(Id);
            Row = FindJurisdiction(Table, Row.AsString("ParentId"));
        }

        return Result;
    }
    bool IsTaxZoneRule(DataRow RuleRow, MemTable JurisdictionTable)
    {
        DataRow JurisdictionRow = FindJurisdiction(JurisdictionTable, RuleRow.AsString("DestinationTaxJurisdictionId"));
        return JurisdictionRow != null && JurisdictionRow.AsInteger("JurisdictionTypeId") == (int)TaxJurisdictionType.TaxZone;
    }
    bool IsRuleMatch(DataRow RuleRow, MemTable JurisdictionTable, DataRow OriginRow, DataRow DestinationRow, List<string> OriginPath, List<string> DestinationPath)
    {
        string RuleOriginId = RuleRow.AsString("OriginTaxJurisdictionId");
        string RuleDestinationId = RuleRow.AsString("DestinationTaxJurisdictionId");

        if (string.IsNullOrWhiteSpace(RuleOriginId)
            && string.IsNullOrWhiteSpace(RuleDestinationId)
            && OriginRow == null
            && DestinationRow == null)
            return false;
        if (!string.IsNullOrWhiteSpace(RuleOriginId) && !OriginPath.Contains(RuleOriginId, StringComparer.OrdinalIgnoreCase))
            return false;
        if (!string.IsNullOrWhiteSpace(RuleDestinationId) && !DestinationPath.Contains(RuleDestinationId, StringComparer.OrdinalIgnoreCase))
            return false;

        if (IsTaxZoneRule(RuleRow, JurisdictionTable))
        {
            if (OriginRow == null || DestinationRow == null)
                return false;
            if (OriginRow.AsString("Id").IsSameText(DestinationRow.AsString("Id")))
                return false;
            if (!OriginPath.Contains(RuleDestinationId, StringComparer.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }
    List<DataRow> SelectRules(IEnumerable<DataRow> Rows)
    {
        List<DataRow> Result = Rows.ToList();

        if (Result.Any(Row => !string.IsNullOrWhiteSpace(Row.AsString("DestinationTaxJurisdictionId"))))
            Result = Result.Where(Row => !string.IsNullOrWhiteSpace(Row.AsString("DestinationTaxJurisdictionId"))).ToList();
        if (Result.Any(Row => !string.IsNullOrWhiteSpace(Row.AsString("OriginTaxJurisdictionId"))))
            Result = Result.Where(Row => !string.IsNullOrWhiteSpace(Row.AsString("OriginTaxJurisdictionId"))).ToList();

        return Result;
    }
    MemTable LoadJurisdictions()
    {
        string SqlText = @"
select
    Id,
    ParentId,
    CountryId,
    JurisdictionTypeId,
    RegionCode,
    PostalCodePattern
from
    TaxJurisdiction
where
    IsActive = 1
";

        return fStore.Select(SqlText);
    }
    MemTable LoadRules(TaxResolveArgs Args, DateTime TradeDate)
    {
        string SqlText = @"
select
    R.Id,
    R.OriginTaxJurisdictionId,
    R.DestinationTaxJurisdictionId,
    R.TaxRateId,
    R.TaxClauseId,
    R.TaxCalculationTypeId,
    R.Priority,
    R.IsExempt,
    R.IsReverseCharge,
    Rate.Percent as TaxRatePercent,
    coalesce(TaxClause.ClauseText, '') as ClauseText
from
    TaxRule R
    inner join TaxRate Rate on Rate.Id = R.TaxRateId
    left join TaxClause TaxClause on TaxClause.Id = R.TaxClauseId
where
    R.IsActive = 1
    and Rate.IsActive = 1
    and R.TaxBusinessGroupId = :TaxBusinessGroupId
    and R.TaxProductGroupId = :TaxProductGroupId
    and (R.TradeTypeId = 0 or R.TradeTypeId = :TradeTypeId)
    and (R.ValidFrom is null or R.ValidFrom <= :TradeDate)
    and (R.ValidTo is null or R.ValidTo >= :TradeDate)
order by
    R.Priority,
    R.Id
";

        return fStore.Select(SqlText, new Dictionary<string, object>()
        {
            ["TaxBusinessGroupId"] = Args.TaxBusinessGroupId,
            ["TaxProductGroupId"] = Args.TaxProductGroupId,
            ["TradeTypeId"] = (int)Args.TradeType,
            ["TradeDate"] = TradeDate.Date,
        });
    }
    TaxComponent CalculateComponent(DataRow RuleRow, string OriginTaxJurisdictionId, string DestinationTaxJurisdictionId, decimal TaxableAmount, decimal PreviousTaxAmount, int SequenceNo)
    {
        TaxCalculationType CalculationType = (TaxCalculationType)RuleRow.AsInteger("TaxCalculationTypeId");
        decimal ComponentTaxableAmount = CalculationType == TaxCalculationType.TaxOnTax
            ? RoundAmount(TaxableAmount + PreviousTaxAmount)
            : TaxableAmount;
        decimal TaxRatePercent = RuleRow.AsDecimal("TaxRatePercent");
        bool IsExempt = RuleRow.AsBoolean("IsExempt");
        bool IsReverseCharge = RuleRow.AsBoolean("IsReverseCharge");
        decimal TaxAmount = IsExempt || IsReverseCharge
            ? 0
            : RoundAmount(ComponentTaxableAmount * TaxRatePercent / 100);
        string TaxJurisdictionId = RuleRow.AsString("DestinationTaxJurisdictionId");

        if (string.IsNullOrWhiteSpace(TaxJurisdictionId))
            TaxJurisdictionId = DestinationTaxJurisdictionId;
        if (string.IsNullOrWhiteSpace(TaxJurisdictionId))
            TaxJurisdictionId = RuleRow.AsString("OriginTaxJurisdictionId");
        if (string.IsNullOrWhiteSpace(TaxJurisdictionId))
            TaxJurisdictionId = OriginTaxJurisdictionId;

        return new TaxComponent
        {
            TaxRuleId = RuleRow.AsString("Id"),
            TaxRateId = RuleRow.AsString("TaxRateId"),
            TaxJurisdictionId = TaxJurisdictionId,
            TaxClauseId = RuleRow.AsString("TaxClauseId"),
            SequenceNo = SequenceNo,
            TaxCalculationType = CalculationType,
            TaxRatePercent = TaxRatePercent,
            TaxableAmount = ComponentTaxableAmount,
            TaxAmount = TaxAmount,
            IsExempt = IsExempt,
            IsReverseCharge = IsReverseCharge,
            TaxClauseText = RuleRow.AsString("ClauseText"),
        };
    }

    // ● construction
    public TaxResolver()
    {
    }

    // ● public
    /// <summary>
    /// Resolves the applicable tax rules and calculates the tax result.
    /// </summary>
    public TaxResult Resolve(TaxResolveArgs Args)
    {
        if (Args == null)
            throw new ArgumentNullException(nameof(Args));

        TaxResult Result = new();
        if (Args.TaxableAmount == 0
            || string.IsNullOrWhiteSpace(Args.TaxBusinessGroupId)
            || string.IsNullOrWhiteSpace(Args.TaxProductGroupId))
            return Result;

        DateTime TradeDate = Args.TradeDate == DateTime.MinValue ? DateTime.Today : Args.TradeDate;

        MemTable JurisdictionTable = LoadJurisdictions();
        DataRow OriginRow = ResolveJurisdiction(JurisdictionTable, Args.OriginTaxJurisdictionId, Args.OriginAddress);
        DataRow DestinationRow = ResolveJurisdiction(JurisdictionTable, Args.DestinationTaxJurisdictionId, Args.DestinationAddress);
        List<string> OriginPath = GetJurisdictionPath(JurisdictionTable, OriginRow);
        List<string> DestinationPath = GetJurisdictionPath(JurisdictionTable, DestinationRow);
        MemTable RuleTable = LoadRules(Args, TradeDate);
        List<DataRow> RuleRows = SelectRules(RuleTable.Rows.Cast<DataRow>()
            .Where(Row => IsRuleMatch(Row, JurisdictionTable, OriginRow, DestinationRow, OriginPath, DestinationPath)));

        Result.OriginTaxJurisdictionId = OriginRow?.AsString("Id") ?? "";
        Result.DestinationTaxJurisdictionId = DestinationRow?.AsString("Id") ?? "";

        decimal PreviousTaxAmount = 0;
        int SequenceNo = 0;
        foreach (DataRow RuleRow in RuleRows)
        {
            TaxComponent Component = CalculateComponent(RuleRow, Result.OriginTaxJurisdictionId, Result.DestinationTaxJurisdictionId, Args.TaxableAmount, PreviousTaxAmount, ++SequenceNo);
            Result.Components.Add(Component);
            PreviousTaxAmount += Component.TaxAmount;
        }

        Result.TaxAmount = RoundAmount(Result.Components.Sum(Component => Component.TaxAmount));
        Result.TaxPercent = Args.TaxableAmount != 0
            ? RoundAmount(Result.TaxAmount * 100 / Args.TaxableAmount)
            : 0;
        Result.IsExempt = Result.Components.Count > 0 && Result.Components.All(Component => Component.IsExempt);
        Result.IsReverseCharge = Result.Components.Any(Component => Component.IsReverseCharge);

        return Result;
    }
}
