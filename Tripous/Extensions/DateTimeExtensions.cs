namespace Tripous
{
    using System.Globalization;
    
    /// <summary>
    /// Extensions
    /// </summary>
    static public class DateTimeExtensions
    {
        /// <summary>
        /// Creates and returns a file name based on DT
        /// <para>The returned string has the format </para>
        /// <para><c>yyyy-MM-dd HH_mm_ss</c></para>
        /// </summary>
        public static string ToFileName(this DateTime DT)
        {
            return ToFileName(DT, false);
        }
        /// <summary>
        /// Creates and returns a file name based on DT.
        /// <para>The returned string has the format </para>
        /// <para><c>yyyy-MM-dd HH_mm_ss__fff</c></para>
        /// </summary>
        public static string ToFileName(this DateTime DT, bool UseMSecs)
        {
            return UseMSecs ? DT.ToString("yyyy-MM-dd HH_mm_ss__fff") : DT.ToString("yyyy-MM-dd HH_mm_ss");             
        }

        /// <summary>
        /// Returns the date of the first day of the week of the specified date 
        /// </summary>
        static public DateTime StartOfWeek(this DateTime DT, CultureInfo CI)
        {
            DayOfWeek Day = CI.DateTimeFormat.FirstDayOfWeek;

            while (DT.DayOfWeek != Day)
                DT = DT.AddDays(-1);

            return DT;
        }
        /// <summary>
        /// Returns the date of the first day of the week of the specified date 
        /// </summary>
        static public DateTime StartOfWeek(this DateTime DT)
        {
            return StartOfWeek(DT, CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// Returns the week number the specified date falls in 
        /// <para>Always between 1 - 52</para>
        /// </summary>
        static public int GetWeekNumber(this DateTime DT, CultureInfo CI)
        {
            int Result = CI.Calendar.GetWeekOfYear(DT, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
            return Result > 52 ? 1 : Result;
        }
        /// <summary>
        /// Returns the week number the specified date falls in 
        /// <para>Always between 1 - 52</para>
        /// </summary>
        static public int GetWeekNumber(this DateTime DT)
        {
            return GetWeekNumber(DT, CultureInfo.CurrentCulture);
        }
        /// <summary>
        /// Returns the start date-time of DT, i.e. yyyy-MM-dd 00:00:00
        /// </summary>
        static public DateTime StartOfDay(this DateTime DT)
        {
            return DT.Date;
        }
        /// <summary>
        /// Returns the end date-time of DT, i.e yyyy-MM-dd 23:59:59
        /// </summary>
        static public DateTime EndOfDay(this DateTime DT)
        {
            DT = DT.Date;
            DT = DT.AddDays(1);
            DT = DT.AddSeconds(-1);
            return DT;
        }
    }



}
