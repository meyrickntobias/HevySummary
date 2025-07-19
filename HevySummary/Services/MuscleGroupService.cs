using System.Collections.Immutable;
using HevySummary.DTOs;
using HevySummary.Models;
using HevySummary.Models.Collections;
using static HevySummary.Helpers.CacheConstants;

namespace HevySummary.Services;

public class MuscleGroupService : IMuscleGroupService
{
    private readonly ICacheService _cacheService;
    private readonly IWorkoutApiService _workoutApiService;

    public MuscleGroupService(ICacheService cacheService, IWorkoutApiService workoutApiService)
    {
        _cacheService = cacheService;
        _workoutApiService = workoutApiService;
    }

    public async Task<List<ExerciseTemplateDto>> GetAllExerciseTemplates()
    {
        List<ExerciseTemplateDto> exerciseTemplates;

        var cachedExerciseTemplates =
            await _cacheService.GetCacheValueAsync<List<ExerciseTemplateDto>>(AllExerciseTemplates) ?? [];

        if (cachedExerciseTemplates.Count == 0)
        {
            exerciseTemplates = await _workoutApiService.GetAllExerciseTemplates();
            await _cacheService.SetCacheValueAsync(AllExerciseTemplates, exerciseTemplates, TimeSpan.FromDays(1));
        }
        else
        {
            exerciseTemplates = cachedExerciseTemplates;
        }
        
        return exerciseTemplates;
    }

    public async Task<ISet<ExerciseTemplateDto>> GetExerciseTemplates(IImmutableSet<string> exerciseIds)
    {
        HashSet<ExerciseTemplateDto> exerciseTemplates = [];

        var cachedExerciseTemplates =
            await _cacheService.GetCacheValueAsync<ImmutableHashSet<ExerciseTemplateDto>>(ExerciseTemplates) ?? [];
        
        var cachedIds = cachedExerciseTemplates
            .Select(t => t.Id)
            .ToImmutableHashSet();
        
        var nonCachedIds = exerciseIds.Except(cachedIds);
        if (nonCachedIds.Count == 0)
        {
            return cachedExerciseTemplates;
        }
        
        foreach (var cachedExerciseTemplate in cachedExerciseTemplates)
        {
            exerciseTemplates.Add(cachedExerciseTemplate);
        }
        
        var nonCachedTemplates = await _workoutApiService.GetExerciseTemplates(nonCachedIds);
        
        foreach (var nonCachedTemplate in nonCachedTemplates)
        {
            exerciseTemplates.Add(nonCachedTemplate);
        }

        if (nonCachedTemplates.Count != 0)
        {
            await UpdateExerciseTemplatesCache(cachedExerciseTemplates, nonCachedTemplates);
        }
        
        return exerciseTemplates;
    }

    public async Task<WorkoutCollection> GetWorkoutsSince(DateOnly earliestWorkoutDate)
    {
        var cachedWorkouts = await _cacheService.GetAllFromHashSetAsync<WorkoutDto>(Workouts);
        if (cachedWorkouts.Count == 0)
        {
            return new WorkoutCollection(await RequestWorkoutsAndPushToCache(earliestWorkoutDate));
        }
        
        await SyncWorkoutsCache();
        var updatedCachedWorkouts = await _cacheService.GetAllFromHashSetAsync<WorkoutDto?>(Workouts);
        
        return new WorkoutCollection(updatedCachedWorkouts
            .Where(w => w != null)
            .Select(w => w!)
            .Where(w => DateOnly.FromDateTime(w.StartTime) >= earliestWorkoutDate)
            .ToList());
    }

    private async Task<List<WorkoutDto>> RequestWorkoutsAndPushToCache(DateOnly earliestWorkoutDate)
    {
        var workouts = await _workoutApiService.GetWorkoutsSince(earliestWorkoutDate);
        await _cacheService.SetCacheValueAsync(LastTimeHevyWorkoutsWasQueriedUTC, DateTime.UtcNow);
        await _cacheService.AddToHashSetAsync(Workouts, workouts, WorkoutHashSelector);

        return workouts;
            
        string WorkoutHashSelector(WorkoutDto workout) => workout.Id.ToString();
    }

    private async Task SyncWorkoutsCache()
    {
        var lastQueried = await _cacheService.GetCacheValueAsync<DateTime>("LastTimeHevyWorkoutsWasQueriedUTC");
        var workoutEvents = await _workoutApiService.GetWorkoutEventsSince(lastQueried);
        await _cacheService.SetCacheValueAsync("LastTimeHevyWorkoutsWasQueriedUTC", DateTime.UtcNow);

        if (workoutEvents.Count == 0)
        {
            return;
        }

        // TODO: can this be done all at once, rather than once per event?
        foreach (var workoutEvent in workoutEvents)
        {
            switch (workoutEvent)
            {
                case WorkoutUpdatedEventDto updatedEvent:
                    await _cacheService.AddToHashSetAsync("Workouts", updatedEvent.Workout.Id.ToString(), updatedEvent.Workout);
                    break;
                case WorkoutDeletedEventDto deletedEvent:
                    await _cacheService.RemoveFromHashSetAsync("Workouts", deletedEvent.WorkoutId.ToString());
                    break;
            }
        }
    }

    private async Task UpdateExerciseTemplatesCache(
        IEnumerable<ExerciseTemplateDto> exerciseTemplatesAlreadyCached, 
        IEnumerable<ExerciseTemplateDto> exerciseTemplatesNotCached)
    {
        var allExerciseTemplates = new List<ExerciseTemplateDto>();
        allExerciseTemplates.AddRange(exerciseTemplatesNotCached);
        allExerciseTemplates.AddRange(exerciseTemplatesAlreadyCached);
        await _cacheService.SetCacheValueAsync(ExerciseTemplates, allExerciseTemplates);
    }
}