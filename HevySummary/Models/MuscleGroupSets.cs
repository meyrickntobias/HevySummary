namespace HevySummary.Models;

public class MuscleGroupSets(string muscleGroup, decimal primarySets, decimal secondarySets)
{
    public string MuscleGroup { get; set; } = muscleGroup;
    public decimal CalculatedSets => PrimarySets + (SecondarySets * 0.5m);
    public decimal PrimarySets { get; set; } = primarySets;
    public decimal SecondarySets { get; set; } = secondarySets;
}