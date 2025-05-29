namespace HevySummary.Models;

public class SetVolumeSummary(DateOnly startDate, DateOnly endDate, List<MuscleGroupSets> muscleGroups)
{
    public DateOnly StartDate { get; set; } = startDate;
    public DateOnly EndDate { get; set; } = endDate;
    public List<MuscleGroupSets> MuscleGroups { get; set; } = muscleGroups;
}