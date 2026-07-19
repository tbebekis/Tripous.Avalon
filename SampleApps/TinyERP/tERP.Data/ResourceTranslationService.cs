/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace tERP.Data;

/// <summary>
/// Represents a language column in the resource translation editor.
/// </summary>
public class ResourceTranslationLanguage
{
    // ● properties
    /// <summary>
    /// The language row id.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// The language code.
    /// </summary>
    public string Code { get; set; }
    /// <summary>
    /// The language name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// The language culture name.
    /// </summary>
    public string CultureName { get; set; }
    /// <summary>
    /// The grid column name.
    /// </summary>
    public string ColumnName { get; set; }
    /// <summary>
    /// True when this is the English source language.
    /// </summary>
    public bool IsEnglish { get; set; }
}

/// <summary>
/// Represents the resource translation editor table and its language metadata.
/// </summary>
public class ResourceTranslationTable
{
    // ● properties
    /// <summary>
    /// The resource translation table.
    /// </summary>
    public DataTable Table { get; set; }
    /// <summary>
    /// The language columns.
    /// </summary>
    public List<ResourceTranslationLanguage> Languages { get; set; } = [];
}

/// <summary>
/// Loads and stores system resource translations.
/// </summary>
static public class ResourceTranslationService
{
    // ● private
    static string CreateColumnName(string Code, int Index)
    {
        string Result = $"Lang_{Code}";
        Result = Regex.Replace(Result, @"\W+", "_");
        if (string.IsNullOrWhiteSpace(Result) || Result == "Lang_")
            Result = $"Lang_{Index + 1}";
        return Result;
    }
    static DataTable CreateTable(List<ResourceTranslationLanguage> Languages)
    {
        DataTable Result = new("ResourceTranslations");

        DataColumn Column = Result.Columns.Add("ResKey", typeof(string));
        Column.Caption = "Resource Key";
        Column.ReadOnly = true;

        for (int i = 0; i < Languages.Count; i++)
        {
            ResourceTranslationLanguage Language = Languages[i];
            Column = Result.Columns.Add(Language.ColumnName, typeof(string));
            Column.Caption = string.IsNullOrWhiteSpace(Language.Name) ? Language.Code : Language.Name;
            Column.ExtendedProperties["LanguageId"] = Language.Id;
            Column.ExtendedProperties["LanguageCode"] = Language.Code;
        }

        return Result;
    }
    static List<ResourceTranslationLanguage> LoadLanguages(SqlStore Store)
    {
        DataTable Table = Store.Select("""
            select
              Id,
              Code,
              Name,
              CultureName
            from SYS_LANG
            where IsActive = 1
            order by
              IsDefault desc,
              Code
            """);

        List<ResourceTranslationLanguage> Result = [];
        for (int i = 0; i < Table.Rows.Count; i++)
        {
            DataRow Row = Table.Rows[i];
            string Code = Row.AsString("Code");
            string CultureName = Row.AsString("CultureName");
            Result.Add(new ResourceTranslationLanguage()
            {
                Id = Row.AsString("Id"),
                Code = Code,
                Name = Row.AsString("Name"),
                CultureName = CultureName,
                ColumnName = CreateColumnName(Code, i),
                IsEnglish = Code.IsSameText("EN") || CultureName.StartsWith("en-", StringComparison.OrdinalIgnoreCase)
            });
        }

        return Result;
    }
    static Dictionary<string, DataRow> LoadRows(DataTable Target)
    {
        Dictionary<string, DataRow> Result = new(StringComparer.OrdinalIgnoreCase);

        foreach (DataRow Row in Target.Rows)
            Result[Row.AsString("ResKey")] = Row;

        return Result;
    }

    // ● public
    /// <summary>
    /// Loads the resource translation table.
    /// </summary>
    /// <param name="Store">The SQL store.</param>
    /// <returns>The resource translation table.</returns>
    static public ResourceTranslationTable Load(SqlStore Store)
    {
        if (Store == null)
            throw new TripousArgumentNullException(nameof(Store));

        List<ResourceTranslationLanguage> Languages = LoadLanguages(Store);
        DataTable ResultTable = CreateTable(Languages);
        Dictionary<string, DataRow> Rows = LoadRows(ResultTable);
        Dictionary<string, ResourceTranslationLanguage> LanguageMap = Languages.ToDictionary(item => item.Id, StringComparer.OrdinalIgnoreCase);

        DataTable Source = SysStrRes.GetTable(Store);

        foreach (DataRow SourceRow in Source.Rows)
        {
            string LanguageId = SourceRow.AsString("LanguageId");
            string ResKey = SourceRow.AsString("ResKey");
            if (string.IsNullOrWhiteSpace(ResKey) || !LanguageMap.TryGetValue(LanguageId, out ResourceTranslationLanguage Language))
                continue;

            if (!Rows.TryGetValue(ResKey, out DataRow TargetRow))
            {
                TargetRow = ResultTable.NewRow();
                TargetRow["ResKey"] = ResKey;
                ResultTable.Rows.Add(TargetRow);
                Rows[ResKey] = TargetRow;
            }

            TargetRow[Language.ColumnName] = SourceRow.AsString("ResValue");
        }

        ResultTable.AcceptChanges();
        ResultTable.Columns["ResKey"].ReadOnly = true;
        foreach (ResourceTranslationLanguage Language in Languages.Where(item => item.IsEnglish))
            ResultTable.Columns[Language.ColumnName].ReadOnly = true;

        return new ResourceTranslationTable() { Table = ResultTable, Languages = Languages };
    }
    /// <summary>
    /// Saves a single resource translation.
    /// </summary>
    /// <param name="Store">The SQL store.</param>
    /// <param name="LanguageId">The language id.</param>
    /// <param name="ResKey">The resource key.</param>
    /// <param name="ResValue">The resource value.</param>
    static public void Save(SqlStore Store, string LanguageId, string ResKey, string ResValue)
    {
        if (Store == null)
            throw new TripousArgumentNullException(nameof(Store));
        if (string.IsNullOrWhiteSpace(LanguageId))
            throw new TripousArgumentNullException(nameof(LanguageId));
        if (string.IsNullOrWhiteSpace(ResKey))
            throw new TripousArgumentNullException(nameof(ResKey));

        Dictionary<string, object> Params = new()
        {
            ["LanguageId"] = LanguageId,
            ["ResKey"] = ResKey,
            ["ResValue"] = ResValue ?? ""
        };

        using SqlTransactionContext Context = Store.BeginTransactionContext();
        Context.BeginTransaction();

        DataRow Row = Store.SelectResults(Context.Transaction, """
            select Id
            from SYS_STR_RES
            where LanguageId = :LanguageId
              and ResKey = :ResKey
            """, Params);

        if (Row == null)
        {
            if (string.IsNullOrWhiteSpace(ResValue))
            {
                Context.Commit();
                return;
            }

            Params["Id"] = Sys.GenId();
            Store.ExecSql(Context.Transaction, """
                insert into SYS_STR_RES
                  (Id, LanguageId, ResKey, ResValue)
                values
                  (:Id, :LanguageId, :ResKey, :ResValue)
                """, Params);
        }
        else
        {
            Params["Id"] = Row.AsString("Id");
            Store.ExecSql(Context.Transaction, """
                update SYS_STR_RES
                set ResValue = :ResValue
                where Id = :Id
                """, Params);
        }

        Context.Commit();
        SysStrRes.MarkDirty();
    }
    /// <summary>
    /// Deletes all translations for a resource key.
    /// </summary>
    /// <param name="Store">The SQL store.</param>
    /// <param name="ResKey">The resource key.</param>
    static public void DeleteResourceKey(SqlStore Store, string ResKey)
    {
        if (Store == null)
            throw new TripousArgumentNullException(nameof(Store));
        if (string.IsNullOrWhiteSpace(ResKey))
            throw new TripousArgumentNullException(nameof(ResKey));

        Store.ExecSql("""
            delete from SYS_STR_RES
            where ResKey = :ResKey
            """, new Dictionary<string, object>() { ["ResKey"] = ResKey });
        SysStrRes.MarkDirty();
    }
}
