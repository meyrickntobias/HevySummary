namespace HevySummary.Models;

public class MuscleGroupSets(string muscleGroup, decimal primarySets, decimal secondarySets)
{
    public string MuscleGroup { get; set; } = muscleGroup;

    public string MuscleGroupRegion { get; set; } = MapMuscleGroupToRegion(muscleGroup);
    
    public decimal CalculatedSets => PrimarySets + (SecondarySets * 0.5m);
    public decimal PrimarySets { get; set; } = primarySets;
    public decimal SecondarySets { get; set; } = secondarySets;

    public static string MapMuscleGroupToRegion(string muscleGroup)
    {
        muscleGroup = muscleGroup.Replace("_", " ");
        return muscleGroup.ToLower() switch
        {
            "quadriceps" or "hamstrings" or "calves" or "glutes" => "legs",
            "upper back" or "lower back" or "lats" => "back",
            "biceps" or "triceps" => "arms",
            _ => muscleGroup
        };
    }
}