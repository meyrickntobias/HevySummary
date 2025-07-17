using System.Collections.Immutable;
using HevySummary.DTOs;
using HevySummary.Models.Collections;

namespace HevySummary.Services;

public interface IMuscleGroupService
{
    public Task<List<ExerciseTemplateDto>> GetAllExerciseTemplates();
    
    public Task<ISet<ExerciseTemplateDto>> GetExerciseTemplates(IImmutableSet<string> exerciseIds);

    public Task<WorkoutCollection> GetWorkoutsSince(DateOnly earliestWorkoutDate);
}