namespace HevySummary.DTOs;

public class WorkoutUpdatedEventDto : WorkoutEventDto
{
    public readonly WorkoutDto Workout;
    
    public WorkoutUpdatedEventDto(WorkoutDto workout)
    {
        Type = "updated";
        Workout = workout;
    }
}