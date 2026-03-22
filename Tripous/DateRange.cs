namespace Tripous
{

    /// <summary>
    /// Indicates how to construct a range between two dates.
    /// </summary>
    public enum DateRange
    {
        /// <summary>
        /// Custom range
        /// </summary>
        Custom,

        /// <summary>
        /// From today To today
        /// </summary>
        Today,
        /// <summary>
        /// From yesterday To today
        /// </summary>
        Yesterday,
        /// <summary>
        /// From today To Tomorrow
        /// </summary>
        Tomorrow,

        /* From Today To ... */

        /// <summary>
        /// From 7 days before To today
        /// </summary>
        LastWeek,
        /// <summary>
        /// From 14 days before To today
        /// </summary>
        LastTwoWeeks,
        /// <summary>
        /// From 30 before To today
        /// </summary>
        LastMonth,
        /// <summary>
        /// From 60 before To today
        /// </summary>
        LastTwoMonths,
        /// <summary>
        /// From 90 before To today
        /// </summary>
        LastThreeMonths,
        /// <summary>
        /// From 180 before To today
        /// </summary>
        LastSemester,
        /// <summary>
        /// From 365 before To today
        /// </summary>
        LastYear,
        /// <summary>
        /// From 730 before To today
        /// </summary>
        LastTwoYears,

        /* From ... To Today */

        /// <summary>
        /// NextWeek
        /// </summary>
        NextWeek,
        /// <summary>
        /// NextTwoWeeks
        /// </summary>
        NextTwoWeeks,
        /// <summary>
        /// NextMonth
        /// </summary>
        NextMonth,
        /// <summary>
        /// NextTwoMonths
        /// </summary>
        NextTwoMonths,
        /// <summary>
        /// NextThreeMonths
        /// </summary>
        NextThreeMonths,
        /// <summary>
        /// NextSemester
        /// </summary>
        NextSemester,
        /// <summary>
        /// NextYear 
        /// </summary>
        NextYear,
        /// <summary>
        /// NextTwoYears
        /// </summary>
        NextTwoYears,
    }


    static public class DateRangeExtensions
    {
        /// <summary>
        /// Constant. The range values than can be used to a WHERE clause filter.
        /// </summary>
        static public readonly DateRange[] WhereRanges = {
            DateRange.Custom,
            DateRange.Today,
            DateRange.Yesterday,
            DateRange.LastWeek,
            DateRange.LastTwoWeeks,
            DateRange.LastMonth,
            DateRange.LastTwoMonths,
            DateRange.LastThreeMonths,
            DateRange.LastSemester,
            DateRange.LastYear,
            DateRange.LastTwoYears,
        };
        /// <summary>
        /// Returns the range values than can be used to a WHERE clause filter.
        /// </summary>
        static public DateRange[] GetWhereRanges() => WhereRanges;

        /// <summary>
        /// Converts a <see cref="DateRange"/> to two DateTime values.
        /// </summary>
        static public bool ToDates(this DateRange Range, ref DateTime FromDate, ref DateTime ToDate)
        {
            DateTime Today = DateTime.Today.Date;
            return ToDates(Range, Today, ref FromDate, ref ToDate);
        }
        /// <summary>
        /// Converts a <see cref="DateRange"/> to two DateTime values.
        /// </summary>
        static public bool ToDates(this DateRange Range, DateTime Today, ref DateTime FromDate, ref DateTime ToDate)
        {
            bool Result = true;

            FromDate = Today;
            ToDate = Today;

            switch (Range)
            {
                case DateRange.Today: break;
                case DateRange.Yesterday: { FromDate = FromDate.AddDays(-1); ToDate = ToDate.AddDays(-1); } break;
                case DateRange.Tomorrow: { FromDate = FromDate.AddDays(1); ToDate = ToDate.AddDays(1); } break;

                case DateRange.LastWeek: FromDate = FromDate.AddDays(-7); break;
                case DateRange.LastTwoWeeks: FromDate = FromDate.AddDays(-14); break;
                case DateRange.LastMonth: FromDate = FromDate.AddDays(-30); break;
                case DateRange.LastTwoMonths: FromDate = FromDate.AddDays(-60); break;
                case DateRange.LastThreeMonths: FromDate = FromDate.AddDays(-90); break;
                case DateRange.LastSemester: FromDate = FromDate.AddDays(-180); break;
                case DateRange.LastYear: FromDate = FromDate.AddDays(-365); break;
                case DateRange.LastTwoYears: FromDate = FromDate.AddDays(-730); break;

                case DateRange.NextWeek: ToDate = ToDate.AddDays(7); break;
                case DateRange.NextTwoWeeks: ToDate = ToDate.AddDays(14); break;
                case DateRange.NextMonth: ToDate = ToDate.AddDays(30); break;
                case DateRange.NextTwoMonths: ToDate = ToDate.AddDays(60); break;
                case DateRange.NextThreeMonths: ToDate = ToDate.AddDays(90); break;
                case DateRange.NextSemester: ToDate = ToDate.AddDays(180); break;
                case DateRange.NextYear: ToDate = ToDate.AddDays(365); break;
                case DateRange.NextTwoYears: ToDate = ToDate.AddDays(730); break;

                default: Result = false; break;
            }

            return Result;
        }
    }
}
