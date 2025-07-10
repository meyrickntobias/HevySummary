using System.Collections.Immutable;
using System.Text;
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
        var hevyRoutines = await workoutApiService.GetRoutines();
        
        var exercisesInRoutines = hevyRoutines
            .SelectMany(r => r.Exercises)
            .Select(e => e.ExerciseTemplateId)
            .ToImmutableHashSet();
        
        var exerciseTemplates = await muscleGroupService.GetExerciseTemplates(exercisesInRoutines);

        var routines = new List<Routine>();
        foreach (var routineDto in hevyRoutines)
        {
            var exercises = new List<Exercise>();
            foreach (var exerciseDto in routineDto.Exercises)
            {
                var exerciseTemplate =
                    exerciseTemplates.FirstOrDefault(template => exerciseDto.ExerciseTemplateId == template.Id);
                
                if (exerciseTemplate == null)
                {
                    throw new InvalidDataException("No exercise template found for exercise within routine");
                }
                var primaryMuscleGroup = exerciseTemplate.PrimaryMuscleGroup;
                var secondaryMuscleGroups = exerciseTemplate.SecondaryMuscleGroups;
                
                exercises.Add(new Exercise(
                    exerciseDto.Title, 
                    exerciseDto.Sets.Where(s => s.Type != "warmup").ToList(), 
                    CapitalizeFirstLetterOfEachWord(primaryMuscleGroup), 
                    secondaryMuscleGroups.Select(CapitalizeFirstLetterOfEachWord).ToList()
                ));
            }
            
            var routine = new Routine(routineDto.Title, routineDto.Notes, exercises, routineDto.CreatedAt, routineDto.UpdatedAt);
            routines.Add(routine);
        }
        
        return routines;
    }

    private string CapitalizeFirstLetterOfEachWord(string input)
    {
        var splitWords = input.Split(" ");
        var sb = new StringBuilder();
        foreach (var word in splitWords)
        {
            sb.Append(char.ToUpperInvariant(word[0]));
            if (word.Length == 1)
            {
                continue;
            }
            sb.Append(word[1..]);
        }
        return sb.ToString();
    }
}