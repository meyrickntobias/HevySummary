using System.Collections.Immutable;
using HevySummary.Helpers;
using HevySummary.Models;
using HevySummary.Services;
using Microsoft.AspNetCore.Mvc;

namespace HevySummary.Controllers;

[ApiController]
[Route("routines/")]
public class RoutineController(
    IWorkoutApiService workoutApiService,
    IMuscleGroupService muscleGroupService)
{
    /// <summary>
    /// Retrieves routines from Hevy and adds muscle group information to each
    /// exercise
    /// </summary>
    /// <returns></returns>
    [HttpGet("")]
    public async Task<List<Routine>> GetRoutines()
    {
        var savedRoutines = await workoutApiService.GetRoutines();
        
        var exercisesInRoutines = savedRoutines
            .SelectMany(r => r.Exercises)
            .Select(e => e.ExerciseTemplateId)
            .ToImmutableHashSet();
        
        var exerciseTemplates = await muscleGroupService.GetExerciseTemplates(exercisesInRoutines);

        return savedRoutines.Select(routineDto => new Routine(routineDto, exerciseTemplates)).ToList();
    }
}