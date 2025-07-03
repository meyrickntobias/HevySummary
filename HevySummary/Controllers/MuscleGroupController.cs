using System.Collections.Immutable;
using System.Diagnostics;
using HevySummary.DTOs;
using HevySummary.Helpers;
using HevySummary.Models;
using HevySummary.Services;
using Microsoft.AspNetCore.Mvc;

namespace HevySummary.Controllers;

[ApiController]
[Route("")]
public class MuscleGroupController(
    IMuscleGroupService muscleGroupService, 
    ILogger<MuscleGroupController> logger,
    TimeProvider timeProvider)
{
    private readonly DateHelper _dateHelper = new(timeProvider);
    
    /// <summary>
    /// Gets a summary of the sets performed for each muscle group over the last N weeks.
    /// </summary>
    /// <param name="weeks">The number of weeks to return. The default value is 4.</param>
    /// <param name="disableCache">When set to true, values aren't cached.</param>
    /// <returns>Each date range alongside all the muscle groups, ordered by calculated sets (primary
    /// sets plus secondary sets multiplied by 0.5). Also shows primary and secondary sets. Warmup sets
    /// are ignored. </returns>
    [HttpGet("/muscle-groups")]
    public async Task<List<SetVolumeSummary>> MuscleGroupSets(int weeks = 4, bool disableCache = false)
    {
        var stopwatch = Stopwatch.StartNew();
        var dateRanges = _dateHelper.GetWeeksUpToCurrentWeek(weeks);
        var earliestWorkoutDate = dateRanges[^1].StartDate;
        var workouts = await muscleGroupService.GetWorkoutsSince(earliestWorkoutDate, disableCache);
        
        var exerciseTemplateIds = workouts
            .SelectMany(w => w.Exercises)
            .Distinct(new ExerciseDtoEqualityComparer())
            .Select(e => e.ExerciseTemplateId)
            .ToImmutableHashSet();
        
        var exerciseTemplates = await muscleGroupService.GetExerciseTemplates(exerciseTemplateIds, disableCache);
        var exerciseTemplatesDict = exerciseTemplates
            .ToDictionary(et => et.Id, et => et);
        
        var workoutsInWeeks = GroupWorkoutsIntoDateRanges(workouts, dateRanges);
        var exercisesGroupedInWeek = GroupExercisesIntoDateRanges(workoutsInWeeks, exerciseTemplatesDict);
        
        var weeklySetVolume = new List<SetVolumeSummary>();
        
        foreach (var week in exercisesGroupedInWeek)
        {
            // All the exercises in the week get
            var muscleSetsInWeek = CalculateSetsPerMuscle(week.Exercises)
                .Select(kvp => kvp.Value)
                .OrderByDescending(m => m.CalculatedSets)
                .ToList();

            var dateRange = new DateRange(week.StartDate, week.EndDate);
            
            var muscleGroupWeek = new SetVolumeSummary(
                week.StartDate,
                week.EndDate,
                workoutsInWeeks[dateRange].Count,
                muscleSetsInWeek);
            
            weeklySetVolume.Add(muscleGroupWeek);
        }
        
        stopwatch.Stop();
        var formattedTime = stopwatch.Elapsed.ToString(@"ss\.fff");
        logger.LogInformation($"Duration of request: {formattedTime} secs");
        return weeklySetVolume;
    }

    private Dictionary<DateRange, List<WorkoutDto>> GroupWorkoutsIntoDateRanges(List<WorkoutDto> workouts,
        List<DateRange> dateRanges)
    {
        var weeklyWorkoutGroupings = new Dictionary<DateRange, List<WorkoutDto>>();
        foreach (var dateRange in dateRanges)
        {
            var workoutsInDateRange = workouts
                .FindAll(w => DateOnly.FromDateTime(w.StartTime) >= dateRange.StartDate 
                              && DateOnly.FromDateTime(w.StartTime) <= dateRange.EndDate);
            
            weeklyWorkoutGroupings.Add(dateRange, workoutsInDateRange);
        }
        return weeklyWorkoutGroupings;
    }

    private List<ExercisesInDateRange> GroupExercisesIntoDateRanges(
        Dictionary<DateRange, List<WorkoutDto>> dateRangeWorkoutGroupings,
        Dictionary<string, ExerciseTemplateDto> exerciseTemplates)
    {
        var weeklyExerciseGroups = new List<ExercisesInDateRange>();
        // TODO: Only do lookup on template ID once per exercise
        
        foreach (var (dateRange, workouts) in dateRangeWorkoutGroupings)
        {
            var exercisesInWeek = workouts
                .SelectMany(w => w.Exercises);

            var exercises = new List<Exercise>();

            foreach (var exerciseDto in exercisesInWeek)
            {
                if (!exerciseTemplates.TryGetValue(exerciseDto.ExerciseTemplateId, out var template))
                {
                    throw new InvalidDataException($"Could not find template for given exercise. Invalid template id: {exerciseDto.ExerciseTemplateId}");
                }

                var exerciseSets = exerciseDto.Sets.Select(set => new ExerciseSet(exerciseDto.Title, set.Reps, set.Type));
                var exercise = new Exercise(
                    exerciseDto.Title, 
                    exerciseSets.ToList(), 
                    template.PrimaryMuscleGroup,
                    template.SecondaryMuscleGroups);
                
                exercises.Add(exercise);
            }
            
            var exerciseWeekSummary = new ExercisesInDateRange
            {
                StartDate = dateRange.StartDate,
                EndDate = dateRange.EndDate,
                Exercises = exercises
            };
            
            weeklyExerciseGroups.Add(exerciseWeekSummary);
        }
        return weeklyExerciseGroups.ToList();
    }

    private Dictionary<string, MuscleGroupSets> CalculateSetsPerMuscle(List<Exercise> exercises)
    {
        var setsPerMuscle = new System.Collections.Generic.Dictionary<string, MuscleGroupSets>();
        
        foreach (var exercise in exercises)
        {
            var primaryMuscle = exercise.PrimaryMuscleGroup;
            var secondaryMuscles = exercise.SecondaryMuscleGroups;
            
            var sets = exercise.Sets.Count(s => s.Type != "warmup");
            
            var primarySets = new MuscleGroupSets(primaryMuscle, sets, 0);
            
            if (!setsPerMuscle.TryAdd(primaryMuscle, primarySets))
            {
                setsPerMuscle[primaryMuscle].PrimarySets += sets;
            }
                
            secondaryMuscles.ForEach(secondaryMuscle =>
            {
                var secondarySets = new MuscleGroupSets(secondaryMuscle, 0, sets);
                
                if (!setsPerMuscle.TryAdd(secondaryMuscle, secondarySets))
                {
                    setsPerMuscle[secondaryMuscle].SecondarySets += sets;
                }
            });
        }
        
        return setsPerMuscle;
    }
}