using HevySummary.DTOs;

namespace HevySummary.Models;

public class WorkoutUpdatedEvent : WorkoutEvent
{
    public readonly WorkoutDto Workout;
    
    public WorkoutUpdatedEvent(WorkoutDto workout)
    {
        Type = "updated";
        Workout = workout;
    }
}