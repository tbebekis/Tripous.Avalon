/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;
 
/// <summary>
/// Provides static methods for getting localized text.
/// </summary>
static public class Texts
{
    /// <summary>
    /// Gets or sets the current localizer.
    /// </summary>
    static public ILocalizer Current { get; set; }

    /// <summary>
    /// Gets the localized text for the specified key.
    /// </summary>
    static public string GS(string Key) => L(Key, SplitKeys ? Key.SplitToWords() : Key);
    /// <summary>
    /// Gets the localized text for the specified key.
    /// </summary>
    static public string GS(string Key, string Default) => L(Key, Default);

    /// <summary>
    /// Gets the localized text for the specified key.
    /// </summary>
    static public string L(string Key) => L(Key, SplitKeys ? Key.SplitToWords() : Key);
    /// <summary>
    /// Gets the localized text for the specified key.
    /// </summary>
    static public string L(string Key, string Default)
    {
        if (string.IsNullOrWhiteSpace(Key))
            return Default;

        if (string.IsNullOrWhiteSpace(Default))
            Default = SplitKeys ? Key.SplitToWords() : Key;

        string Result = Current != null ? Current.GetText(Key) : Default;

        if (!string.IsNullOrWhiteSpace(Result))
            Result = Result.Replace("__", " ");
        
        return Result;
    }

    /// <summary>
    /// True if the keys should be split into words.
    /// </summary>
    static public bool SplitKeys { get; set; } = true;

}