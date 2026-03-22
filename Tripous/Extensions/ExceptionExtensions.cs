namespace Tripous
{
    using System.Text;
    
    static public class ExceptionExtensions
    {

        /// <summary>
        /// Adds E.Data dictionary information to SB
        /// </summary>
        static public void AddDataDictionaryTo(Exception E, StringBuilder SB)
        {
            if ((E != null) && (E.Data.Count > 0))
            {
                SB.AppendLine();

                foreach (object Key in E.Data.Keys)
                    SB.AppendLine(string.Format("{0}: {1}", Key, E.Data[Key]));

                SB.AppendLine();
            }

        }

        /// <summary>
        /// Returns a string containing all exception information,
        /// including the Data dictionary and the inner exceptions
        /// </summary>
        static public string GetFullText(this Exception Ex)
        {
            StringBuilder SB = new StringBuilder();

            Action<Exception> Proc = null;
            Proc = delegate (Exception E)
            {
                if (E != null)
                {
                    SB.AppendLine(E.ToString());
                    AddDataDictionaryTo(E, SB);

                    if (E.InnerException != null)
                    {
                        SB.AppendLine(" ----------------------------------------------------------------");
                        SB.AppendLine(" ");

                        Proc(E.InnerException);
                    }
                }
            };

            //SB.AppendLine(" ================================================================");
            SB.AppendLine(" ");

            Proc(Ex);

            return SB.ToString();

        }

        static public List<string> GetErrors(this Exception Ex)
        {
            List<string> Result = new();

            Exception E = Ex; 

            while (E != null)
            {
                Result.Add($"{E.GetType().Name}: {E.Message}");
                E = E.InnerException;
            }

            return Result;
        }
        static public string GetErrorText(this Exception Ex)
        {
            List<string> ErrorList = GetErrors(Ex);
            return string.Join(Environment.NewLine, ErrorList);
        }
    }
}
