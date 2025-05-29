namespace HevySummary.Models;

public class ExerciseSet(string exerciseName, int reps, string type)
{
    public string ExerciseName { get; set; } = exerciseName;
    public int Reps { get; set; } = reps;
    public string Type { get; set; } = type;
}