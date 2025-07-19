namespace HevySummary.DTOs;

public class WorkoutDeletedEventDto : WorkoutEventDto
{
    public Guid WorkoutId { get; set; }
    
    public DateTime DeletedAt { get; set; }

    public WorkoutDeletedEventDto(Guid workoutId, DateTime deletedAt)
    {
        Type = "deleted";
        WorkoutId = workoutId;
        DeletedAt = deletedAt;
    }
}