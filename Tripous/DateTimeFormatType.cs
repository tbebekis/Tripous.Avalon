namespace Tripous
{
    /// <summary>
    /// Indicates how a DateTime should be formatted as a string
    /// </summary>
    public enum DateTimeFormatType
    {
        /// <summary>
        /// Short date. 
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 31/1/2012</para>
        /// </summary>
        Date,
        /// <summary>
        /// Long Date.
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: Monday 1 January 2012</para>
        /// </summary>
        LongDate,

        /// <summary>
        /// Short time.
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 11:30 pm</para>
        /// </summary>
        Time,
        /// <summary>
        /// Long time.
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 11:30:30 pm</para>
        /// </summary>
        LongTime,

        /// <summary>
        /// Short date-time.
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 31/1/2012 11:30 pm</para>
        /// </summary>
        DateTime,
        /// <summary>
        /// Short date, long time
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 31/1/2012 11:30:30 pm</para>
        /// </summary>
        DateLongTime,

        /// <summary>
        /// Short 24h time
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 13:30</para>
        /// </summary>
        Time24,
        /// <summary>
        /// Long 24h time
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 13:30:30</para>
        /// </summary>
        LongTime24,

        /// <summary>
        /// Short date, short 24h time
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 31/1/2012 13:30</para>
        /// </summary>
        DateTime24,
        /// <summary>
        /// Short date, long 24h time
        /// <para>NOTE: Thread.CurrentThread.CurrentCulture depended.</para>
        /// <para>Could be as: 31/1/2012 13:30:30</para>
        /// </summary>
        DateLongTime24,

        /// <summary>
        /// ISO date: yyyy-MM-dd
        /// </summary>
        IsoDate,
        /// <summary>
        /// ISO date-time: yyyy-MM-dd HH:mm
        /// </summary>
        IsoDateTime,
        /// <summary>
        /// ISO date-time: yyyy-MM-dd HH:mm:ss
        /// </summary>
        IsoDateLongTime,

        /// <summary>
        /// A user supplied format
        /// </summary>
        Custom,
    }
}
