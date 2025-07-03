namespace HevySummary.DTOs;

public class WorkoutDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public List<ExerciseDto> Exercises { get; set; } = [];
}