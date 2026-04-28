namespace Tripous.Data;

static public class SqlParamScanner
{
    // ● private
    static bool IsParamStartChar(char Ch)
    {
        return char.IsLetter(Ch) || Ch == '_';
    }
    static bool IsParamChar(char Ch)
    {
        return char.IsLetterOrDigit(Ch) || Ch == '_';
    }
    static bool TryReadParamName(string SqlText, int StartIndex, out string Name, out int EndIndex)
    {
        Name = string.Empty;
        EndIndex = StartIndex;
        if (StartIndex >= SqlText.Length || !IsParamStartChar(SqlText[StartIndex]))
            return false;
        int Index = StartIndex + 1;
        while (Index < SqlText.Length && IsParamChar(SqlText[Index]))
            Index++;
        Name = SqlText.Substring(StartIndex, Index - StartIndex);
        EndIndex = Index;
        return true;
    }

    // ● static public
    static public List<SqlParamRef> Scan(string SqlText)
    {
        List<SqlParamRef> Result = new List<SqlParamRef>();
        int Index = 0;
        while (Index < SqlText.Length)
        {
            char Ch = SqlText[Index];
            if (Ch == '\'')
            {
                Index++;
                while (Index < SqlText.Length)
                {
                    if (SqlText[Index] == '\'' && Index + 1 < SqlText.Length && SqlText[Index + 1] == '\'')
                    {
                        Index += 2;
                        continue;
                    }
                    if (SqlText[Index] == '\'')
                    {
                        Index++;
                        break;
                    }
                    Index++;
                }
                continue;
            }
            if (Ch == '-' && Index + 1 < SqlText.Length && SqlText[Index + 1] == '-')
            {
                Index += 2;
                while (Index < SqlText.Length && SqlText[Index] != '\n')
                    Index++;
                continue;
            }
            if (Ch == '/' && Index + 1 < SqlText.Length && SqlText[Index + 1] == '*')
            {
                Index += 2;
                while (Index + 1 < SqlText.Length)
                {
                    if (SqlText[Index] == '*' && SqlText[Index + 1] == '/')
                    {
                        Index += 2;
                        break;
                    }
                    Index++;
                }
                continue;
            }
            if (Ch == ':')
            {
                if (Index + 1 < SqlText.Length && SqlText[Index + 1] == ':')
                {
                    Index += 2;
                    continue;
                }
                if (TryReadParamName(SqlText, Index + 1, out string Name, out int EndIndex))
                {
                    Result.Add(new SqlParamRef(Name, Index));
                    Index = EndIndex;
                    continue;
                }
            }
            Index++;
        }
        return Result;
    }
}