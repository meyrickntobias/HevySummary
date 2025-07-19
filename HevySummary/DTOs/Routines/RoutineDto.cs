namespace HevySummary.DTOs.Routines;

public class RoutineDto(
    string title, 
    string? notes, 
    List<ExerciseDto> exercises, 
    DateTime createdAt,
    DateTime updatedAt)
{
    public string Title { get; set; } = title;
    public string? Notes { get; set; } = notes;
    public List<ExerciseDto> Exercises { get; set; } = exercises;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime UpdatedAt { get; set; } = updatedAt;
}