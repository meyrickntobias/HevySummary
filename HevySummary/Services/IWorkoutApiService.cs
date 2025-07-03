using HevySummary.DTOs;
using HevySummary.Models;

namespace HevySummary.Services;

public interface IWorkoutApiService
{
    public Task<List<WorkoutDto>> GetWorkoutsSince(DateOnly earliestRequestedWorkoutDate);

    public Task<List<ExerciseTemplateDto>> GetExerciseTemplates(IEnumerable<string> exerciseIds);

    public Task<List<WorkoutEvent>> GetWorkoutEventsSince(DateTime since);
}