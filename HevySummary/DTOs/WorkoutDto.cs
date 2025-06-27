using Newtonsoft.Json;

namespace HevySummary.DTOs;

public class WorkoutDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    
    [JsonProperty("start_time")]
    public DateTime StartTime { get; set; }
    [JsonProperty("end_time")]
    public DateTime EndTime { get; set; }

    public List<ExerciseDto> Exercises { get; set; } = [];
}