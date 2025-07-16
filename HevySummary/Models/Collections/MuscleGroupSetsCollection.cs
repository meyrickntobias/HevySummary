namespace HevySummary.Models.Collections;

public class MuscleGroupSetsCollection
{
    private Dictionary<string, MuscleGroupSets> MuscleGroups { get; set; } = new();

    public void AddPrimarySets(string muscle, int primarySets)
    {
        if (MuscleGroups.TryGetValue(muscle, out var primaryMuscle))
        {
            primaryMuscle.PrimarySets += primarySets;
        }
        else
        {
            MuscleGroups.Add(muscle, new MuscleGroupSets(muscle, primarySets, 0));
        }
    }

    public void AddSecondarySets(string muscle, int secondarySets)
    {
        if (MuscleGroups.TryGetValue(muscle, out var secondaryMuscle))
        {
            secondaryMuscle.SecondarySets += secondarySets;
        }
        else
        {
            MuscleGroups.Add(muscle, new MuscleGroupSets(muscle, 0, secondarySets));
        }
    }

    public List<MuscleGroupSets> ToList()
    {
        return MuscleGroups.Select(mg => mg.Value).ToList();
    }
}