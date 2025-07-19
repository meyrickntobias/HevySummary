using HevySummary.DTOs;
using HevySummary.DTOs.Routines;
using HevySummary.Helpers;

namespace HevySummary.Models;

public class Routine
{
    public string Title { get; set; }
    public string? Notes { get; set; }
    public List<Exercise> Exercises { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Routine(RoutineDto routineDto, ISet<ExerciseTemplateDto> exerciseTemplates)
    {
        Title = routineDto.Title;
        Notes = routineDto.Notes;
        CreatedAt = routineDto.CreatedAt;
        UpdatedAt = routineDto.UpdatedAt;
        
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
                primaryMuscleGroup.CapitalizeFirstLetterOfEachWord(), 
                secondaryMuscleGroups.Select(mg => mg.CapitalizeFirstLetterOfEachWord()).ToList()
            ));
        }

        Exercises = exercises.ToList();
    }
}