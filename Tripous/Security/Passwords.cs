using System.Security.Cryptography;
using System.Text;

namespace Tripous;

static public class Passwords
{
    static public readonly string SpecialChars = "!@#$%^*()-_=+[]{}|?";  // exclude & and < 
    static readonly string UpperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    static readonly string LowerChars = "abcdefghijklmnopqrstuvwxyz";
    static readonly string NumberChars = "0123456789";
    
    // ● private
    /// <summary>
    /// Returns a random character from a character set.
    /// </summary>
    static char GetRandomChar(string CharSet)
    {
        int Index = RandomNumberGenerator.GetInt32(CharSet.Length);
        return CharSet[Index];
    }
    /// <summary>
    /// Shuffles a character list using a cryptographically secure random generator.
    /// </summary>
    static void Shuffle(List<char> List)
    {
        for (int i = List.Count - 1; i > 0; i--)
        {
            int j = RandomNumberGenerator.GetInt32(i + 1);

            char Temp = List[i];
            List[i] = List[j];
            List[j] = Temp;
        }
        
    }
    
    // ● public
    /// <summary>
    /// Generates a random password and guarantees that each selected character category appears at least once.
    /// </summary>
    static public string GeneratePassword(
        int MinLength = 12,
        bool UseUpperCase = true,
        bool UseLowerCase = true,
        bool UseNumbers = true,
        bool UseSpecialChars = false)
    {
        List<string> EnabledSets = new List<string>();
        StringBuilder AllChars = new StringBuilder();

        if (UseUpperCase)
        {
            EnabledSets.Add(UpperChars);
            AllChars.Append(UpperChars);
        }

        if (UseLowerCase)
        {
            EnabledSets.Add(LowerChars);
            AllChars.Append(LowerChars);
        }

        if (UseNumbers)
        {
            EnabledSets.Add(NumberChars);
            AllChars.Append(NumberChars);
        }

        if (UseSpecialChars)
        {
            EnabledSets.Add(SpecialChars);
            AllChars.Append(SpecialChars);
        }

        if (EnabledSets.Count == 0)
            throw new Exception("At least one character category must be selected.");

        if (MinLength < EnabledSets.Count)
            MinLength = EnabledSets.Count;

        List<char> Result = new List<char>();

        foreach (string CharSet in EnabledSets)
            Result.Add(GetRandomChar(CharSet));

        while (Result.Count < MinLength)
            Result.Add(GetRandomChar(AllChars.ToString()));

        Shuffle(Result);

        return new string(Result.ToArray());
    }

    /// <summary>
    /// Validates a password, according to specified requirements regarding its content and length, and returns the result.
    /// </summary>
    static public bool IsValid(string Password, int MinLength = 8, int MaxLength = 12, bool UseLowerChars = true, bool UseUpperChars = true, bool UseDigitChars = true, bool UseSpecialChars = true)
    {
        if (MinLength < 6)
            MinLength = 6;
        if (MaxLength < 8)
            MaxLength = 8;
        if (MaxLength < MinLength)
        {
            MinLength = 8;
            MaxLength = 12;
        }

        if (string.IsNullOrWhiteSpace(Password))
            return false;
        if (Password.Length < MinLength || Password.Length > MaxLength)
            return false;
        if (UseLowerChars && !Password.Any(char.IsLower))
            return false;
        if (UseUpperChars && !Password.Any(char.IsUpper))
            return false;
        if (UseDigitChars && !Password.Any(char.IsDigit))
            return false;
        if (UseSpecialChars && !Password.Any(SpecialChars.Contains))
            return false;

        return true;
    }
    

}