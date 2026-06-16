namespace Tripous.Tests;

/// <summary>
/// Tests for date-time extension methods.
/// </summary>
public class DateTimeExtensionsTests
{
    // ● public
    /// <summary>
    /// Ensures EndOfDay returns the last tick of the same date.
    /// </summary>
    [Fact]
    public void EndOfDay_ReturnsLastTickOfDate()
    {
        DateTime Source = new DateTime(2026, 6, 16, 10, 30, 15);
        DateTime Result = Source.EndOfDay();
        Assert.Equal(new DateTime(2026, 6, 16, 23, 59, 59).AddTicks(TimeSpan.TicksPerSecond - 1), Result);
    }
    /// <summary>
    /// Ensures GetWeekNumber keeps valid week 53 values.
    /// </summary>
    [Fact]
    public void GetWeekNumber_KeepsWeek53()
    {
        CultureInfo Culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        Culture.DateTimeFormat.CalendarWeekRule = CalendarWeekRule.FirstFourDayWeek;
        Culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
        int Result = new DateTime(2020, 12, 31).GetWeekNumber(Culture);
        Assert.Equal(53, Result);
    }
}
