using HevySummary.DTOs;

namespace HevySummary.Models.Collections;

public class DateRangeIndexedExerciseCollection
{
    public Dictionary<DateRange, ExerciseCollection> Exercises { get; set; }

    public DateRangeIndexedExerciseCollection(
        Dictionary<DateRange, List<WorkoutDto>> workoutPeriods,
        ISet<ExerciseTemplateDto> exerciseTemplates)
    {
        Exercises = workoutPeriods.Select(
                workoutsInRange => new KeyValuePair<DateRange, ExerciseCollection>(
                    workoutsInRange.Key,
                    new ExerciseCollection(workoutsInRange.Value.SelectMany(w => w.Exercises), exerciseTemplates)))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public List<SetVolumeSummary> CalculateSetVolumes(Dictionary<DateRange, List<WorkoutDto>> workoutPeriods)
    {
        var weeklySetVolume = new List<SetVolumeSummary>();
        
        foreach (var (week, exercises) in Exercises)
        {
            var muscleSetsInWeek = exercises.CalculateSetsPerMuscleGroup()
                .OrderByDescending(m => m.CalculatedSets)
                .ToList();

            var dateRange = new DateRange(week.StartDate, week.EndDate);
            
            var muscleGroupWeek = new SetVolumeSummary(
                week.StartDate,
                week.EndDate,
                workoutPeriods[dateRange].Count,
                muscleSetsInWeek);
            
            weeklySetVolume.Add(muscleGroupWeek);
        }
        
        return weeklySetVolume;
    }
}