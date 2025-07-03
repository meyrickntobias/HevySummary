namespace HevySummary.DTOs;

public class ExerciseTemplateDto
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }
    public required string PrimaryMuscleGroup  { get; set; }
    public List<string> SecondaryMuscleGroups { get; set; } = [];
    public bool? IsCustom { get; set; }
}