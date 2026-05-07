namespace Tripous;

static public class ExceptionExtensions
{
    /// <summary>
    /// Returns a string containing all exception information,
    /// including the Data dictionary and the inner exceptions
    /// </summary>
    static public string GetErrorTextFull(this Exception Ex)
    {
        // ------------------------------------------------------------
        void AddDataDictionaryTo(Exception E, StringBuilder SB)
        {
            if (E != null && E.Data != null && E.Data.Count > 0)
            {
                SB.AppendLine();

                foreach (object Key in E.Data.Keys)
                    SB.AppendLine($"{Key}: {E.Data[Key]}"); 

                SB.AppendLine();
            }
        }
        // ------------------------------------------------------------
        
        StringBuilder SB = new StringBuilder();

        Exception E = Ex; 

        while (E != null)
        {
            SB.AppendLine(E.Message);
            
            if (Sys.DebugMode)
            {
                SB.AppendLine(E.ToString());
                AddDataDictionaryTo(E, SB);
            }
            
            if (E.InnerException != null)
            {
                SB.AppendLine(" ----------------------------------------------------------------");
                SB.AppendLine(" ");
            }

            E = E.InnerException;
        }

        SB.AppendLine(" ");

        string Result = SB.ToString();
        return Result;
    }
    /// <summary>
    /// Returns a string containing all exception information,
    /// including the inner exceptions
    /// </summary>
    static public string GetErrorText(this Exception Ex)
    {
        // --------------------------------------------------------
        List<string> GetErrors(Exception Exception)
        {
            List<string> Result = new();

            Exception E = Exception; 

            while (E != null)
            {
                Result.Add($"{E.GetType().Name}: {E.Message}");
                E = E.InnerException;
            }

            return Result;
        }
        // --------------------------------------------------------
        
        List<string> ErrorList = GetErrors(Ex);
        return string.Join(Environment.NewLine, ErrorList);
    }
}

