namespace HevySummary.Models;

public class ExercisesInDateRange
{
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<Exercise> Exercises { get; set; } = [];
}