namespace HevySummary.Models;

public class SetVolumeSummary(DateOnly startDate, DateOnly endDate, int workouts, List<MuscleGroupSets> muscleGroups)
{
    public DateOnly StartDate { get; set; } = startDate;
    public DateOnly EndDate { get; set; } = endDate;
    public int Workouts { get; set; } = workouts;
    public List<MuscleGroupSets> MuscleGroups { get; set; } = muscleGroups;
}