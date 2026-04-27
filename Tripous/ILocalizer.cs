namespace Tripous;

public interface ILocalizer
{
    string GetText(string Key);
}

static public class Texts
{
    static public ILocalizer Current { get; set; }

    static public string GS(string Key) => L(Key, Key);
    static public string GS(string Key, string Default) => L(Key, Default);
    
    static public string L(string Key) => L(Key, Key);
    static public string L(string Key, string Default)
    {
        if (string.IsNullOrWhiteSpace(Key))
            return Default;

        if (string.IsNullOrWhiteSpace(Default))
            Default = Key;

        return Current != null ? Current.GetText(Key) : Default;
    }
 
}