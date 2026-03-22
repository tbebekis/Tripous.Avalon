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
    }
}
