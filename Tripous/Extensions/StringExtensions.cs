namespace Tripous
{
    using System.Text;
    using System.Globalization;
    using System.Text.RegularExpressions;

    static public class StringExtensions
    {
        /// <summary>
        /// Case insensitive string equality.
        /// <para>Returns true if 1. both are null, 2. both are empty string or 3. they are the same string </para>
        /// </summary>
        static public bool IsSameText(this string A, string B)
        {
            //return (!string.IsNullOrWhiteSpace(A) && !string.IsNullOrWhiteSpace(B))&& (string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0);

            // Compare() returns true if 1. both are null, 2. both are empty string or 3. they are the same string
            return string.Compare(A, B, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
        /// <summary>
        /// Returns true if Value is contained in the Instance.
        /// Performs a case-insensitive check using the invariant culture.
        /// </summary>
        static public bool ContainsText(this string Instance, string Value)
        {
            if ((Instance != null) && !string.IsNullOrWhiteSpace(Value))
            {
                return Instance.IndexOf(Value, StringComparison.InvariantCultureIgnoreCase) != -1;
            }

            return false;
        }
        /// <summary>
        /// Returns true if Instance starts with Value.
        /// Performs a case-insensitive check using the invariant culture.
        /// </summary>
        static public bool StartsWithText(this string Instance, string Value) => !string.IsNullOrWhiteSpace(Instance) && Instance.StartsWith(Value, StringComparison.InvariantCultureIgnoreCase);
        /// <summary>
        /// Returns true if Instance ends with Value.
        /// Performs a case-insensitive check using the invariant culture.
        /// </summary>
        static public bool EndsWithText(this string Instance, string Value) => !string.IsNullOrWhiteSpace(Instance) && Instance.EndsWith(Value, StringComparison.InvariantCultureIgnoreCase);
 
        static public string ToPlural(this string Word)
        {
            if (string.IsNullOrWhiteSpace(Word) || Word.Length < 2)
                return Word;

            string lower = Word.ToLower();

            // ● Rule -y: Category -> Categories (consonant + y)
            if (Word.EndsWith("y", StringComparison.OrdinalIgnoreCase) && !IsVowel(Word[Word.Length - 2]))
                return Word.Substring(0, Word.Length - 1) + "ies";

            // ● Rule -s, -x, -ch, -sh: Box -> Boxes, Dish -> Dishes
            if (Word.EndsWith("s", StringComparison.OrdinalIgnoreCase) || 
                Word.EndsWith("x", StringComparison.OrdinalIgnoreCase) || 
                Word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) || 
                Word.EndsWith("sh", StringComparison.OrdinalIgnoreCase))
                return Word + "es";

            // ● Rule for professional/technical endings: -er, -or, -ar, -ir
            // ● just add the -s (User -> Users, Actor -> Actors)
            if (Word.EndsWith("er", StringComparison.OrdinalIgnoreCase) || 
                Word.EndsWith("or", StringComparison.OrdinalIgnoreCase) || 
                Word.EndsWith("ar", StringComparison.OrdinalIgnoreCase) || 
                Word.EndsWith("ir", StringComparison.OrdinalIgnoreCase))
                return Word + "s";

            // ● no luck
            return Word;
        }

        // ● private methods
        static public bool IsVowel(char C)
        {
            char lower = char.ToLower(C);
            return "aeiou".Contains(lower);
        }
        
        /// <summary>
        /// Splits the specified Text into lines, taking the Environment.NewLine as separator.
        /// </summary>
        static public string[] ToLines(this string Text)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return new string[0];

            Regex rx = new Regex(Environment.NewLine);
            return rx.Split(Text);

        }
        
        /// <summary>
        /// Quotes S, that is returns S surrounded by ' (single quotes)
        /// </summary>
        static public string Quote(this string S)
        {
            StringBuilder SB = new StringBuilder(S);
            SB.Replace('\'', ' ');
            SB.Insert(0, "'");
            SB.Append('\'');

            return SB.ToString();
        }
        /// <summary>
        /// Quotes S, that is returns S surrounded by ' (single quotes)
        /// </summary>
        static public string QS(this string S)
        {
            return Quote(S);
        }
        
        /// <summary>
        /// Quotes a path only if contains spaces.
        /// </summary>
        static public string QuotePath(this string Path)
        {
            if (string.IsNullOrWhiteSpace(Path))
                return "";
    
            // ● Αν έχει κενά και δεν είναι ήδη σε quotes, τα προσθέτουμε
            //if ((Path.Contains(' ') || Path.Contains('.')) && !Path.StartsWith("\""))
            //    return "\"" + Path + "\"";
            
            if (!Path.StartsWith("\""))
                return "\"" + Path + "\"";

            return Path;
        }
        
        static public int ToIntOrDefault(this string S, int DefaultValue = 0) => int.TryParse(S, out int Result) ? Result : DefaultValue;
 
        /// <summary>
        /// Returns true if a specified string is made up of numeric digits.
        /// </summary>
        static public bool IsNumeric(this string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
                return false;

            foreach (char C in Value)
                if (!char.IsDigit(C))
                    return false;

            return true;
        }
        /// <summary>
        /// Converts accented characters of the specified Text into non-accented characters
        /// <para>From: http://stackoverflow.com/questions/359827/ignoring-accented-letters-in-string-comparison</para>
        /// </summary>
        static public string RemoveDiacritics(this string Text)
        {
            if (Text != null)
            {
                return string.Concat(
                    Text.Normalize(NormalizationForm.FormD)
                    .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
                  ).Normalize(NormalizationForm.FormC);
            }

            return Text;
        }
        /// <summary>
        /// Splits a camel case string by adding spaces between words.
        /// <para>It handles acronyms too, i.e. ABCamelDECase</para>
        /// </summary>
        static public string SplitCamelCase(this string Text)
        {
            return string.IsNullOrWhiteSpace(Text) ? string.Empty : Regex.Replace(Text, "(?<=[a-z])([A-Z])", " $1", RegexOptions.Compiled).Trim();
        }
        /// <summary>
        /// Splits a string into words at each uppercase letter. 
        /// Existing spaces are preserved.
        /// </summary>
        static public string SplitToWords(this string Text)
        {
            if (string.IsNullOrWhiteSpace(Text))
                return Text;

            StringBuilder Result = new StringBuilder();
            Result.Append(Text[0]);

            for (int I = 1; I < Text.Length; I++)
            {
                // ● If it's uppercase and the previous character wasn't a space, add a space
                if (char.IsUpper(Text[I]) && Text[I - 1] != ' ')
                {
                    Result.Append(' ');
                }

                Result.Append(Text[I]);
            }

            return Result.ToString();
        }
    }
}
