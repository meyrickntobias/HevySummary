using System.Collections.Immutable;
using HevySummary.DTOs;

namespace HevySummary.Services;

public interface IMuscleGroupService
{
    public Task<List<ExerciseTemplateDto>> GetExerciseTemplates(IImmutableSet<string> exerciseIds);

    public Task<List<WorkoutDto>> GetWorkoutsSince(DateOnly earliestWorkoutDate);
}