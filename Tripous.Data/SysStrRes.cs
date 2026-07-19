/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous.Data;

/// <summary>
/// Represents a system language row.
/// </summary>
public class SysLangInfo
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
    /// True when this is the default language.
    /// </summary>
    public bool IsDefault { get; set; }
    /// <summary>
    /// True when this language is active.
    /// </summary>
    public bool IsActive { get; set; }
}

/// <summary>
/// Provides cached access to the system string resource table.
/// </summary>
static public class SysStrRes
{
    // ● private fields
    static readonly object fLock = new();
    static bool fIsDirty = true;
    static MemTable fTable;
    static List<SysLangInfo> fLanguages = [];
    static string fEnglishLanguageId;
    static string fDefaultLanguageId;

    // ● private methods
    static List<SysLangInfo> LoadLanguages(SqlStore Store)
    {
        MemTable Table = Store.Select("""
            select
              Id,
              Code,
              Name,
              CultureName,
              IsDefault,
              IsActive
            from SYS_LANG
            order by
              IsDefault desc,
              Code
            """);

        return Table.Rows.Cast<DataRow>().Select(Row => new SysLangInfo()
        {
            Id = Row.AsString("Id"),
            Code = Row.AsString("Code"),
            Name = Row.AsString("Name"),
            CultureName = Row.AsString("CultureName"),
            IsDefault = Row.AsBoolean("IsDefault"),
            IsActive = Row.AsBoolean("IsActive")
        }).ToList();
    }
    static bool TryFind(string LangId, string Key, out string Value)
    {
        Value = "";
        if (fTable == null || string.IsNullOrWhiteSpace(LangId) || string.IsNullOrWhiteSpace(Key))
            return false;

        DataRow Row = fTable.Rows
            .Cast<DataRow>()
            .FirstOrDefault(item => item.AsString("LanguageId").IsSameText(LangId)
                                    && item.AsString("ResKey").IsSameText(Key));

        if (Row == null)
            return false;

        Value = Row.AsString("ResValue");
        return true;
    }
    static string Find(string LangId, string Key) => TryFind(LangId, Key, out string Value) ? Value : "";
    static bool IsEnglish(SysLangInfo Language)
    {
        return Language != null
               && (Language.Code.IsSameText("EN")
                   || Language.CultureName.StartsWith("en-", StringComparison.OrdinalIgnoreCase)
                   || Language.CultureName.IsSameText("en"));
    }
    static string GetMissingKeyValue(string Key, string Default)
    {
        if (!string.IsNullOrWhiteSpace(Default))
            return Default;
        return Texts.SplitKeys ? Key.SplitToWords() : Key;
    }
    static void AddTableRow(string Id, string LanguageId, string Key, string Value)
    {
        if (fTable == null)
            return;

        DataRow Row = fTable.NewRow();
        Row["Id"] = Id;
        Row["LanguageId"] = LanguageId;
        Row["ResKey"] = Key;
        Row["ResValue"] = Value;
        fTable.Rows.Add(Row);
        Row.AcceptChanges();
    }
    static void SendChangedMessage(string LanguageId, string Key, bool WasInsert)
    {
        Broadcaster.Send("SysStrRes.Changed", null, new Dictionary<string, object>()
        {
            ["LanguageId"] = LanguageId ?? "",
            ["ResKey"] = Key ?? "",
            ["WasInsert"] = WasInsert
        });
    }
    static string InsertMissingEnglishKey(SqlStore Store, string Key, string Default)
    {
        if (!AutoInsertMissingKeys || string.IsNullOrWhiteSpace(fEnglishLanguageId) || TryFind(fEnglishLanguageId, Key, out _))
            return "";

        string Id = Sys.GenId();
        string Value = GetMissingKeyValue(Key, Default);
        Store.ExecSql("""
            insert into SYS_STR_RES
              (Id, LanguageId, ResKey, ResValue)
            values
              (:Id, :LanguageId, :ResKey, :ResValue)
            """, new Dictionary<string, object>()
            {
                ["Id"] = Id,
                ["LanguageId"] = fEnglishLanguageId,
                ["ResKey"] = Key,
                ["ResValue"] = Value
            });
        AddTableRow(Id, fEnglishLanguageId, Key, Value);
        SendChangedMessage(fEnglishLanguageId, Key, true);
        return Value;
    }

    // ● public
    /// <summary>
    /// Loads the system string resource table.
    /// </summary>
    /// <param name="Store">The SQL store.</param>
    static public void Load(SqlStore Store = null)
    {
        Store ??= Db.DefaultStore;

        lock (fLock)
        {
            fLanguages = LoadLanguages(Store);
            fEnglishLanguageId = fLanguages.FirstOrDefault(IsEnglish)?.Id ?? "";
            fDefaultLanguageId = fLanguages.FirstOrDefault(item => item.IsDefault)?.Id ?? fEnglishLanguageId;
            fTable = Store.Select("""
                select
                  Id,
                  LanguageId,
                  ResKey,
                  ResValue
                from SYS_STR_RES
                order by
                  ResKey,
                  LanguageId
                """);
            fIsDirty = false;
        }
    }
    /// <summary>
    /// Returns the cached system string resource table.
    /// </summary>
    /// <param name="Store">The SQL store.</param>
    /// <returns>The cached system string resource table.</returns>
    static public MemTable GetTable(SqlStore Store = null)
    {
        if (fTable == null || fIsDirty)
            Load(Store);
        return fTable;
    }
    /// <summary>
    /// Marks the cached system string resource table as dirty.
    /// </summary>
    static public void MarkDirty()
    {
        fIsDirty = true;
    }
    /// <summary>
    /// Installs a system string resource localizer as the current text localizer.
    /// </summary>
    static public void RegisterLocalizer()
    {
        Texts.Current = new SysStrResLocalizer();
    }
    /// <summary>
    /// Returns the cached languages.
    /// </summary>
    /// <param name="Store">The SQL store.</param>
    /// <returns>The cached languages.</returns>
    static public List<SysLangInfo> GetLanguages(SqlStore Store = null)
    {
        if (fTable == null || fIsDirty)
            Load(Store);
        return fLanguages.ToList();
    }
    /// <summary>
    /// Returns the language id for a culture code.
    /// </summary>
    /// <param name="CultureCode">The culture code.</param>
    /// <param name="Store">The SQL store.</param>
    /// <returns>The language id.</returns>
    static public string GetLanguageId(string CultureCode, SqlStore Store = null)
    {
        if (fTable == null || fIsDirty)
            Load(Store);

        SysLangInfo Language = null;
        if (!string.IsNullOrWhiteSpace(CultureCode))
        {
            Language = fLanguages.FirstOrDefault(item => item.IsActive && item.CultureName.IsSameText(CultureCode));
            if (Language == null && CultureCode.Contains('-'))
            {
                string Code = CultureCode.Split('-')[0];
                Language = fLanguages.FirstOrDefault(item => item.IsActive && item.Code.IsSameText(Code));
            }
        }

        return Language?.Id ?? fDefaultLanguageId ?? fEnglishLanguageId ?? "";
    }
    /// <summary>
    /// Returns the current user language id.
    /// </summary>
    /// <param name="Store">The SQL store.</param>
    /// <returns>The current user language id.</returns>
    static public string GetCurrentLanguageId(SqlStore Store = null)
    {
        string CultureCode = Sys.Context?.CurrentUser?.CultureCode;
        if (string.IsNullOrWhiteSpace(CultureCode))
            CultureCode = CultureInfo.CurrentCulture.Name;
        return GetLanguageId(CultureCode, Store);
    }
    /// <summary>
    /// Returns the localized text for a language and key.
    /// </summary>
    /// <param name="LangId">The language id.</param>
    /// <param name="Key">The resource key.</param>
    /// <param name="Default">The default text.</param>
    /// <returns>The localized text.</returns>
    static public string L(string LangId, string Key, string Default = null)
    {
        if (string.IsNullOrWhiteSpace(Key))
            return Default;

        lock (fLock)
        {
            SqlStore Store = Db.DefaultStore;
            GetTable(Store);

            string Result = Find(LangId, Key);
            if (string.IsNullOrWhiteSpace(Result) && !LangId.IsSameText(fEnglishLanguageId))
                Result = Find(fEnglishLanguageId, Key);
            if (string.IsNullOrWhiteSpace(Result))
                Result = InsertMissingEnglishKey(Store, Key, Default);

            if (string.IsNullOrWhiteSpace(Result))
                Result = Default ?? Key;

            return Result;
        }
    }
    /// <summary>
    /// Returns the localized text for the current user language and key.
    /// </summary>
    /// <param name="Key">The resource key.</param>
    /// <param name="Default">The default text.</param>
    /// <returns>The localized text.</returns>
    static public string L(string Key, string Default = null)
    {
        return L(GetCurrentLanguageId(), Key, Default);
    }
    /// <summary>
    /// Returns a key-text dictionary for a language.
    /// </summary>
    /// <param name="LangId">The language id.</param>
    /// <param name="IncludeEnglishFallback">True to include English fallback values.</param>
    /// <returns>The key-text dictionary.</returns>
    static public Dictionary<string, string> GetDictionary(string LangId, bool IncludeEnglishFallback = true)
    {
        lock (fLock)
        {
            GetTable();

            Dictionary<string, string> Result = new(StringComparer.OrdinalIgnoreCase);
            if (IncludeEnglishFallback && !string.IsNullOrWhiteSpace(fEnglishLanguageId))
            {
                foreach (DataRow Row in fTable.Rows.Cast<DataRow>().Where(item => item.AsString("LanguageId").IsSameText(fEnglishLanguageId)))
                    Result[Row.AsString("ResKey")] = Row.AsString("ResValue");
            }

            if (!string.IsNullOrWhiteSpace(LangId))
            {
                foreach (DataRow Row in fTable.Rows.Cast<DataRow>().Where(item => item.AsString("LanguageId").IsSameText(LangId)))
                    Result[Row.AsString("ResKey")] = Row.AsString("ResValue");
            }

            return Result;
        }
    }
    /// <summary>
    /// Returns a key-text dictionary for the current user language.
    /// </summary>
    /// <returns>The key-text dictionary.</returns>
    static public Dictionary<string, string> GetCurrentDictionary()
    {
        return GetDictionary(GetCurrentLanguageId());
    }

    // ● properties
    /// <summary>
    /// Gets or sets a value indicating that the cached system string resource table must be reloaded.
    /// </summary>
    static public bool IsDirty
    {
        get => fIsDirty;
        set => fIsDirty = value;
    }
    /// <summary>
    /// Gets or sets a value indicating whether missing resource keys should be inserted into the English language.
    /// </summary>
    static public bool AutoInsertMissingKeys { get; set; } = true;
    /// <summary>
    /// Returns the cached system string resource table.
    /// </summary>
    static public MemTable Table => GetTable();
}

/// <summary>
/// Localizes text using the system string resource cache.
/// </summary>
public class SysStrResLocalizer: ILocalizer
{
    // ● public
    /// <summary>
    /// Gets the localized text for the specified key.
    /// </summary>
    public string GetText(string Key)
    {
        return SysStrRes.L(Key);
    }
    /// <summary>
    /// Gets the localized text for the specified key.
    /// </summary>
    public string GetText(string Key, string Default)
    {
        return SysStrRes.L(Key, Default);
    }
    /// <summary>
    /// Gets the localized text for the specified language and key.
    /// </summary>
    public string GetText(string LangId, string Key, string Default)
    {
        return SysStrRes.L(LangId, Key, Default);
    }
}
