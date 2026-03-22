namespace Tripous
{
    using System.Globalization;
    
    /// <summary>
    /// Extensions
    /// </summary>
    static public class DateTimeFormatTypeExtensions
    {
        /// <summary>
        /// Returns the format string for the specified FormatType according to CultureInfo
        /// </summary>
        static public string GetFormatString(this DateTimeFormatType FormatType, CultureInfo CultureInfo)
        {
            DateTimeFormatInfo info = CultureInfo.DateTimeFormat;
            string TimeSep = string.IsNullOrWhiteSpace(info.TimeSeparator) || (info.TimeSeparator.Trim() == string.Empty) ? ":" : info.TimeSeparator.Trim();

            switch (FormatType)
            {
                case DateTimeFormatType.Date: return info.ShortDatePattern;
                case DateTimeFormatType.LongDate: return info.LongDatePattern;

                case DateTimeFormatType.Time: return info.ShortTimePattern;
                case DateTimeFormatType.LongTime: return info.LongTimePattern;

                case DateTimeFormatType.DateTime: return info.ShortDatePattern + " " + info.ShortTimePattern;
                case DateTimeFormatType.DateLongTime: return info.ShortDatePattern + " " + info.LongTimePattern;

                case DateTimeFormatType.Time24: return string.Format("HH{0}mm", TimeSep);
                case DateTimeFormatType.LongTime24: return string.Format("HH{0}mm{0}ss", TimeSep);

                case DateTimeFormatType.DateTime24: return info.ShortDatePattern + " " + string.Format("HH{0}mm", TimeSep);
                case DateTimeFormatType.DateLongTime24: return info.ShortDatePattern + " " + string.Format("HH{0}mm{0}ss", TimeSep);

                case DateTimeFormatType.IsoDate: return "yyyy-MM-dd";
                case DateTimeFormatType.IsoDateTime: return "yyyy-MM-dd HH:mm";
                case DateTimeFormatType.IsoDateLongTime: return "yyyy-MM-dd HH:mm:ss";
            }

            return string.Empty;
        }
        /// <summary>
        /// Returns the format string for the specified FormatType according to
        /// Thread.CurrentThread.CurrentCulture
        /// </summary>
        static public string GetFormatString(this DateTimeFormatType FormatType)
        {
            return GetFormatString(FormatType, Thread.CurrentThread.CurrentCulture);
        }
    }

}
