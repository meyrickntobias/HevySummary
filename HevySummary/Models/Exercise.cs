using HevySummary.DTOs;

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

    public Exercise(string title, List<ExerciseSetDto> exerciseSetDtoList, string primaryMuscleGroup, List<string> secondaryMuscleGroups) 
        : this(
            title, 
            exerciseSetDtoList.Select(set => new ExerciseSet(title, set.Reps, set.Type)).ToList(), 
            primaryMuscleGroup, 
            secondaryMuscleGroups)
    {
    }
}