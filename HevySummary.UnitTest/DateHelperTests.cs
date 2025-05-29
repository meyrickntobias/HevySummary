using FluentAssertions;
using HevySummary;
using HevySummary.Helpers;
using HevySummary.Models;
using Microsoft.Extensions.Time.Testing;

namespace HevySummary.UnitTest;

public class DateHelperTests
{
    [Theory]
    [InlineData("2025-05-26")]
    [InlineData("2025-05-27")]
    [InlineData("2025-05-28")]
    [InlineData("2025-05-29")]
    [InlineData("2025-05-30")]
    [InlineData("2025-05-31")]
    [InlineData("2025-06-01")]
    public void GetWeeksUpToCurrentWeek_GetsWeeksStartingFromMondayOfCurrentWeek(string today)
    {
        // 2025-05-29 is a Thursday which means the current week start is 2025-05-26 (Monday)
        var todayDate = DateTime.Parse(today);
        var timeProvider = new FakeTimeProvider(todayDate);
        var dateHelper = new DateHelper(timeProvider);

        var dateRanges = dateHelper.GetWeeksUpToCurrentWeek(4);

        dateRanges.Should().BeEquivalentTo(new List<DateRange>
        {
            // Most recent week, mon 26th may - sun 1st june
            new(new DateOnly(2025, 5, 26), new DateOnly(2025, 6, 1)),
            // mon 19th may - sun 25th may
            new(new DateOnly(2025, 5, 19), new DateOnly(2025, 5, 25)),
            // mon 12th may - sun 18th may
            new(new DateOnly(2025, 5, 12), new DateOnly(2025, 5, 18)),
            // mon 5th may - sun 11th may
            new(new DateOnly(2025, 5, 5), new DateOnly(2025, 5, 11)),
        });
    }
}