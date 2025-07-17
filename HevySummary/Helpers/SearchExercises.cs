using HevySummary.DTOs;

namespace HevySummary.Helpers;

public static class SearchExercises
{
    public static List<ExerciseTemplateDto> Search(
        string searchTerm, 
        IEnumerable<ExerciseTemplateDto> exerciseTemplates,
        int limit = 20)
    {
        var strippedSearchTerm = StripSpecialChars(searchTerm.ToLower());
        var splitSearchTerm = strippedSearchTerm.Split(" ");
    
        var matchedExercises = new Dictionary<ExerciseTemplateDto, decimal>();
    
        foreach (var exercise in exerciseTemplates)
        {
            var exerciseLower = exercise.Title.ToLower();
            var strippedExercise = StripSpecialChars(exerciseLower);
            var splitExercise = strippedExercise.Split(" ");

            var wordMatches = 0;
            foreach (var searchTermWord in splitSearchTerm)
            {
                if (splitExercise.Contains(searchTermWord))
                {
                    wordMatches++;
                }
            }

            if (wordMatches > 0)
            {
                matchedExercises.Add(exercise, (wordMatches / (decimal)splitExercise.Length));
            }
        }

        return matchedExercises
            .OrderByDescending(e => e.Value)
            .Take(limit)
            .Select(e => e.Key)
            .ToList();
    }
    
    private static string StripSpecialChars(string input)
    {
        var charsToRemove = new[] { "-", ",", "(", ")" };
        foreach (var character in charsToRemove)
        {
            input = input.Replace(character, string.Empty);
        }
        return input;
    }
}