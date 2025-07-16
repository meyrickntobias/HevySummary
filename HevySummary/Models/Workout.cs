namespace HevySummary.Models;

public class Workout
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<Exercise> Exercises { get; set; } = [];
}