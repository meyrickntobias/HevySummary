using System.Collections.Immutable;
using HevySummary.DTOs;

namespace HevySummary.Services;

public interface IMuscleGroupService
{
    public Task<List<ExerciseTemplateDto>> GetExerciseTemplates(IImmutableSet<string> exerciseIds, bool disableCache = false);

    public Task<List<WorkoutDto>> GetWorkoutsSince(DateOnly earliestWorkoutDate, bool disableCache = false);
}