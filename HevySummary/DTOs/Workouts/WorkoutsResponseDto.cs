namespace HevySummary.DTOs;

public class WorkoutsResponseDto
{
    public int Page { get; set; }
    
    public int PageCount { get; set; }

    public List<WorkoutDto> Workouts { get; set; } = [];
}