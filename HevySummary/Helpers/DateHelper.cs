using HevySummary.Models;

namespace HevySummary.Helpers;

public class DateHelper(TimeProvider timeProvider)
{
    /// <summary>
    /// Gets a list of date ranges representing the last specified number of weeks.
    /// Will start from the current week and work backwards.
    /// For example, if the current day is Sunday, it will begin from the Monday of the current week.
    /// </summary>
    /// <param name="weeks">Number of weeks to get</param>
    /// <returns>A list of date ranges (start date, end date) ordered from most recent to oldest</returns>
    public List<DateRange> GetWeeksUpToCurrentWeek(int weeks)
    {
        var startOfWeek = GetStartOfWeek();
        var dateRanges = new List<DateRange>();
        
        for (var i = 0; i < weeks; i++)
        {
            var startDate = startOfWeek.AddDays(-(i * 7));
            var endDate = startDate.AddDays(6);
            dateRanges.Add(new DateRange(startDate, endDate));
        }
        
        return dateRanges;
    }
    
    private DateOnly GetStartOfWeek()
    {
        const int mondayDayOfWeek = 1;
        var now = timeProvider.GetUtcNow().DateTime;
        
        // Convert Sunday to 7 for easier calculations
        var currentDayOfWeek = (int)now.DayOfWeek == 0 
            ? 7 
            : (int)now.DayOfWeek;
        
        var daysFromMon = currentDayOfWeek - mondayDayOfWeek;
        return DateOnly.FromDateTime(now.AddDays(-daysFromMon));
    }
}