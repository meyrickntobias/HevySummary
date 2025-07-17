using HevySummary.DTOs;
using HevySummary.Helpers;
using HevySummary.Services;
using Microsoft.AspNetCore.Mvc;

namespace HevySummary.Controllers;

[ApiController]
[Route("exercise-templates/")]
public class ExerciseTemplateController(
    IMuscleGroupService muscleGroupService)
{
    /// <summary>
    /// Retrieves exercises from Hevy and searches by keyword
    /// </summary>
    /// <returns></returns>
    [HttpGet("search")]
    public async Task<List<ExerciseTemplateDto>> SearchExerciseTemplates(string keyword)
    {
        var allExerciseTemplates = await muscleGroupService.GetAllExerciseTemplates();
        
        var matches = SearchExercises.Search(keyword, allExerciseTemplates);
        
        matches.ForEach(et =>
        {
            et.PrimaryMuscleGroup = et.PrimaryMuscleGroup.CapitalizeFirstLetterOfEachWord();
            et.SecondaryMuscleGroups = et.SecondaryMuscleGroups
                .Select(smg => smg.CapitalizeFirstLetterOfEachWord())
                .ToList();
        });

        return matches;
    }
}