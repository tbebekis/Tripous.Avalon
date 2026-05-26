/*
 * Tripous.Avalon
 * Copyright (c) Theo Bebekis
 *
 * Licensed under the Tripous License.
 * See License.txt for details.
 */

namespace Tripous;
 
static public class Texts
{
    static public ILocalizer Current { get; set; }

    static public string GS(string Key) => L(Key, SplitKeys ? Key.SplitToWords() : Key);
    static public string GS(string Key, string Default) => L(Key, Default);

    static public string L(string Key) => L(Key, SplitKeys ? Key.SplitToWords() : Key);
 
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

    static public bool SplitKeys { get; set; } = true;

}