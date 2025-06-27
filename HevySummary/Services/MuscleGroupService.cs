using HevySummary.DTOs;

namespace HevySummary.Services;

public class MuscleGroupService : IMuscleGroupService
{
    private readonly ICacheService _cacheService;
    private readonly IHevyApiService _hevyApiService;

    public MuscleGroupService(ICacheService cacheService, IHevyApiService hevyApiService)
    {
        _cacheService = cacheService;
        _hevyApiService = hevyApiService;
    }

    public async Task<List<ExerciseTemplateDto>> GetExerciseTemplates(List<string> exerciseIds, bool disableCache = false)
    {
        if (disableCache)
        {
            return await _hevyApiService.GetExerciseTemplates(exerciseIds);
        }
        
        List<ExerciseTemplateDto> exerciseTemplates = [];

        var cachedExerciseTemplates =
            await _cacheService.GetCacheValueAsync<List<ExerciseTemplateDto>>("ExerciseTemplates") ?? [];
        var cachedIds = cachedExerciseTemplates.Select(t => t.Id).ToList();
        
        // Maybe use set instead?
        if (cachedIds.SequenceEqual(exerciseIds))
        {
            return cachedExerciseTemplates;
        }
        
        exerciseTemplates.AddRange(cachedExerciseTemplates);
        var nonCachedTemplates = await _hevyApiService.GetExerciseTemplates(exerciseIds);
        exerciseTemplates.AddRange(nonCachedTemplates);

        if (nonCachedTemplates.Count != 0)
        {
            await UpdateExerciseTemplatesCache(cachedExerciseTemplates, nonCachedTemplates);
        }
        
        return exerciseTemplates;
    }

    private async Task UpdateExerciseTemplatesCache(
        List<ExerciseTemplateDto> exerciseTemplatesAlreadyCached, 
        List<ExerciseTemplateDto> exerciseTemplatesNotCached)
    {
        var allExerciseTemplates = new List<ExerciseTemplateDto>();
        allExerciseTemplates.AddRange(exerciseTemplatesNotCached);
        allExerciseTemplates.AddRange(exerciseTemplatesAlreadyCached);
        await _cacheService.SetCacheValueAsync("ExerciseTemplates", allExerciseTemplates);
    }
}