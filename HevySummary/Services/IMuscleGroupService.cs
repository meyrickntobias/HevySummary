using HevySummary.DTOs;

namespace HevySummary.Services;

public interface IMuscleGroupService
{
    public Task<List<ExerciseTemplateDto>> GetExerciseTemplates(List<string> exerciseIds, bool disableCache = false);
}