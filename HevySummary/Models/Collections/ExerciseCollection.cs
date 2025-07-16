using HevySummary.DTOs;

namespace HevySummary.Models.Collections;

public class ExerciseCollection
{
    public List<Exercise> Exercises { get; set; }

    public ExerciseCollection(IEnumerable<Exercise> exercises)
    {
        Exercises = exercises.ToList();
    }

    public ExerciseCollection(IEnumerable<ExerciseDto> exercises, ISet<ExerciseTemplateDto> templates)
    {
        Exercises = exercises.Select(e =>
            {
                var template = templates.FirstOrDefault(t => t.Id == e.ExerciseTemplateId);
                if (template == null)
                {
                    throw new InvalidDataException($"Could not find exercise template for {e.Title}, template ID: {e.ExerciseTemplateId}.");
                }
                return new Exercise(
                    e.Title,
                    e.Sets,
                    template.PrimaryMuscleGroup,
                    template.SecondaryMuscleGroups
                );
            })
            .ToList();
    }

    public List<MuscleGroupSets> CalculateSetsPerMuscleGroup()
    {
        var setsPerMuscleGroup = new MuscleGroupSetsCollection();
        foreach (var exercise in Exercises)
        {
            var sets = exercise.Sets.Count(s => s.Type != "warmup");
            
            setsPerMuscleGroup.AddPrimarySets(exercise.PrimaryMuscleGroup, sets);
            
            foreach (var secondaryMuscleExercise in exercise.SecondaryMuscleGroups)
            {
                setsPerMuscleGroup.AddSecondarySets(secondaryMuscleExercise, sets);
            }
        }

        return setsPerMuscleGroup.ToList();
    }
}