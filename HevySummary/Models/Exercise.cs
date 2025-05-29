namespace HevySummary.Models;

public class Exercise(
    string title,
    List<ExerciseSet> sets,
    string primaryMuscleGroup,
    List<string> secondaryMuscleGroups)
{
    public string Title { get; set; } = title;
    public List<ExerciseSet> Sets { get; set; } = sets;
    public string PrimaryMuscleGroup  { get; set; } = primaryMuscleGroup;
    public List<string> SecondaryMuscleGroups { get; set; } = secondaryMuscleGroups;
}