using HevySummary.DTOs;
using Newtonsoft.Json;

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
        // Lookup exercise templates in the cache
        List<ExerciseTemplateDto> exerciseTemplates = [];
        List<ExerciseTemplateDto> deserializedCachedTemplates = [];

        if (disableCache)
        {
            return await _hevyApiService.GetExerciseTemplates(exerciseIds);
        }
        
        var cachedExerciseTemplates = await _cacheService.GetCacheValue("ExerciseTemplates");

        if (!string.IsNullOrEmpty(cachedExerciseTemplates))
        {
            deserializedCachedTemplates = JsonConvert.DeserializeObject<List<ExerciseTemplateDto>>(cachedExerciseTemplates) ?? [];
            var cachedIds = deserializedCachedTemplates.Select(t => t.Id).ToList();
            // Maybe use set instead?
            if (cachedIds.SequenceEqual(exerciseIds))
            {
                return deserializedCachedTemplates;
            }
            
            exerciseTemplates.AddRange(deserializedCachedTemplates);
        }
        
        var nonCachedTemplates = await _hevyApiService.GetExerciseTemplates(exerciseIds);
        exerciseTemplates.AddRange(nonCachedTemplates);

        if (nonCachedTemplates.Any())
        {
            await UpdateExerciseTemplatesCache(deserializedCachedTemplates, nonCachedTemplates);
        }
        
        return exerciseTemplates;
    }

    private async Task UpdateExerciseTemplatesCache(List<ExerciseTemplateDto> alreadyCached, List<ExerciseTemplateDto> exerciseTemplatesToAdd)
    {
        var allTemplates = new List<ExerciseTemplateDto>();
        allTemplates.AddRange(exerciseTemplatesToAdd);
        allTemplates.AddRange(alreadyCached);
        await _cacheService.SetCacheValue("ExerciseTemplates", JsonConvert.SerializeObject(allTemplates));
    }
}