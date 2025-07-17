namespace HevySummary.Helpers;

public static class CacheConstants
{
    public const string Workouts = "Workouts";
    /// <summary>
    /// ExerciseTemplates is the exercise templates used by the user (with the API key)
    /// </summary>
    public const string ExerciseTemplates = "ExerciseTemplates";
    
    /// <summary>
    /// AllExerciseTemplates tries to mirror exactly the templates that Hevy has
    /// </summary>
    public const string AllExerciseTemplates = "AllExerciseTemplates";
    public const string LastTimeHevyWorkoutsWasQueriedUTC = "LastTimeHevyWorkoutsWasQueriedUTC";
}