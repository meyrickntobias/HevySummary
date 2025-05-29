using HevySummary.DTOs;

namespace HevySummary.Services;

public interface IHevyApiService
{
    public Task<List<WorkoutDto>> GetWorkoutsSince(DateOnly earliestRequestedWorkoutDate);

    public Task<List<ExerciseTemplateDto>> GetExerciseTemplates(List<string> exerciseIds);
}