using System.Diagnostics;

namespace HevySummary.Models;

[DebuggerDisplay("{StartDate} - {EndDate}")]
public class DateRange
{
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }
    
    public DateRange(DateOnly startDate, DateOnly endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }
        
        var dr = (DateRange)obj;
        return dr.StartDate == StartDate && dr.EndDate == EndDate;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartDate, EndDate);
    }
}