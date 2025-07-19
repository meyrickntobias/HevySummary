using System.Collections.Immutable;
using HevySummary.DTOs;

namespace HevySummary.Models.Collections;

public class WorkoutCollection(List<WorkoutDto> workouts)
{
    public List<WorkoutDto> Workouts { get; set; } = workouts;

    public Dictionary<DateRange, List<WorkoutDto>> SplitIntoDateRanges(List<DateRange> dateRanges)
    {
        var workoutGroups = new Dictionary<DateRange, List<WorkoutDto>>();
        foreach (var dateRange in dateRanges)
        {
            var workoutsInDateRange = workouts
                .FindAll(w => DateOnly.FromDateTime(w.StartTime) >= dateRange.StartDate 
                              && DateOnly.FromDateTime(w.StartTime) <= dateRange.EndDate);
            
            workoutGroups.Add(dateRange, workoutsInDateRange);
        }
        return workoutGroups;
    }

    public IImmutableSet<string> GetDistinctExerciseTemplateIds()
    {
        return Workouts.SelectMany(w => w.Exercises)
            .Select(e => e.ExerciseTemplateId)
            .Distinct()
            .ToImmutableHashSet();
    }
}