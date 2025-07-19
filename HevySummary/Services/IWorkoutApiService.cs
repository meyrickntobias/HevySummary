using HevySummary.DTOs;
using HevySummary.DTOs.Routines;
using HevySummary.Models;

namespace HevySummary.Services;

public interface IWorkoutApiService
{
    public Task<List<WorkoutDto>> GetWorkoutsSince(DateOnly earliestRequestedWorkoutDate);
    
    public Task<List<ExerciseTemplateDto>> GetAllExerciseTemplates();
    
    public Task<ISet<ExerciseTemplateDto>> GetExerciseTemplates(IEnumerable<string> exerciseIds);

    public Task<List<WorkoutEventDto>> GetWorkoutEventsSince(DateTime since);
    
    public Task<List<RoutineDto>> GetRoutines();
}