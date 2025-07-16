using HevySummary.Helpers;
using HevySummary.Models;
using HevySummary.Models.Collections;
using HevySummary.Services;
using Microsoft.AspNetCore.Mvc;

namespace HevySummary.Controllers;

[ApiController]
[Route("")]
public class MuscleGroupController(
    IMuscleGroupService muscleGroupService,
    TimeProvider timeProvider)
{
    private readonly DateHelper _dateHelper = new(timeProvider);
    
    /// <summary>
    /// Gets a summary of the sets performed for each muscle group over the last N weeks.
    /// </summary>
    /// <param name="weeks">The number of weeks to return. The default value is 4.</param>
    /// <returns>Each date range alongside all the muscle groups, ordered by calculated sets (primary
    /// sets plus secondary sets multiplied by 0.5). Also shows primary and secondary sets. Warmup sets
    /// are ignored. </returns>
    [HttpGet("/muscle-groups")]
    public async Task<List<SetVolumeSummary>> MuscleGroupSets(int weeks = 4)
    {
        var dateRanges = _dateHelper.GetWeeksUpToCurrentWeek(weeks);
        var earliestWorkoutDate = dateRanges[^1].StartDate;
        var workouts = await muscleGroupService.GetWorkoutsSince(earliestWorkoutDate);
        
        var exerciseTemplateIds = workouts.GetDistinctExerciseTemplateIds();
        var exerciseTemplates = await muscleGroupService.GetExerciseTemplates(exerciseTemplateIds);
        
        var workoutsInWeeks = workouts.SplitIntoDateRanges(dateRanges);
        var exercisesInWeeks = new DateRangeIndexedExerciseCollection(workoutsInWeeks, exerciseTemplates);
        
        var weeklySetVolume = exercisesInWeeks.CalculateSetVolumes(workoutsInWeeks);
        
        return weeklySetVolume;
    }
}