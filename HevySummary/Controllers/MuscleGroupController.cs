using HevySummary.DTOs;
using HevySummary.Helpers;
using HevySummary.Models;
using HevySummary.Services;
using Microsoft.AspNetCore.Mvc;

namespace HevySummary.Controllers;

[ApiController]
[Route("")]
public class MuscleGroupController(IHevyApiService hevyApiService)
{
    /// <summary>
    /// Gets a summary of the sets performed for each muscle group over the last N weeks.
    /// </summary>
    /// <param name="weeks">The number of weeks to return. The default value is 4.</param>
    /// <returns>Each date range alongside all the muscle groups, ordered by calculated sets (primary
    /// sets plus secondary sets multiplied by 0.5. Also shows primary and secondary sets. Warmup sets
    /// are ignored. </returns>
    [HttpGet("/muscle-groups")]
    public async Task<List<SetVolumeSummary>> MuscleGroupSets(int weeks = 4)
    {
        var dateRanges = new DateHelper(TimeProvider.System).GetWeeksUpToCurrentWeek(weeks);
        var earliestWorkoutDate = dateRanges[^1].StartDate;
        var workouts = await hevyApiService.GetWorkoutsSince(earliestWorkoutDate);
        
        var exerciseTemplateIds = workouts
            .SelectMany(w => w.Exercises)
            .Distinct(new ExerciseDtoEqualityComparer())
            .Select(e => e.ExerciseTemplateId)
            .ToList();
        
        var exerciseTemplates = await hevyApiService.GetExerciseTemplates(exerciseTemplateIds);
        var exerciseTemplatesDict = exerciseTemplates
            .ToDictionary(et => et.Id, et => et);
        
        var workoutsInWeeks = GroupWorkoutsIntoDateRanges(workouts, dateRanges);
        var exercisesGroupedInWeek = GroupExercisesIntoDateRanges(workoutsInWeeks, exerciseTemplatesDict);
        
        var weeklySetVolume = new List<SetVolumeSummary>();
        
        foreach (var week in exercisesGroupedInWeek)
        {
            // All the exercises in the week get
            var muscleSetsInWeek = SetsPerMuscle(week.Exercises)
                .Select(kvp => new MuscleGroupSets(kvp.Key, kvp.Value.Item1, kvp.Value.Item2))
                .OrderByDescending(m => m.CalculatedSets)
                .ToList();
            
            var muscleGroupWeek = new SetVolumeSummary(
                week.StartDate,
                week.EndDate,
                muscleSetsInWeek);
            
            weeklySetVolume.Add(muscleGroupWeek);
        }
        
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

    // TODO: Refactor away from tuples as they cause confusion
    private Dictionary<string, (decimal, decimal)> SetsPerMuscle(List<Exercise> exercises)
    {
        var setsPerMuscle = new Dictionary<string, (decimal, decimal)>();
        
        foreach (var exercise in exercises)
        {
            var primaryMuscle = exercise.PrimaryMuscleGroup;
            var secondaryMuscles = exercise.SecondaryMuscleGroups;
            
            var sets = exercise.Sets.Count(s => s.Type != "warmup");
            
            if (!setsPerMuscle.TryAdd(primaryMuscle, (sets, 0)))
            {
                setsPerMuscle[primaryMuscle] = (setsPerMuscle[primaryMuscle].Item1 + sets, setsPerMuscle[primaryMuscle].Item2);
            }
                
            secondaryMuscles.ForEach(secondaryMuscle =>
            {
                if (!setsPerMuscle.TryAdd(secondaryMuscle, (0, sets)))
                {
                    setsPerMuscle[secondaryMuscle] = (setsPerMuscle[secondaryMuscle].Item1, setsPerMuscle[secondaryMuscle].Item2 + sets);
                }
            });
        }
        
        return setsPerMuscle;
    }
}