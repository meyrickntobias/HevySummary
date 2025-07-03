namespace HevySummary.Models;

public class WorkoutDeletedEvent : WorkoutEvent
{
    public Guid WorkoutId { get; set; }
    
    public DateTime DeletedAt { get; set; }

    public WorkoutDeletedEvent(Guid workoutId, DateTime deletedAt)
    {
        Type = "deleted";
        WorkoutId = workoutId;
        DeletedAt = deletedAt;
    }
}