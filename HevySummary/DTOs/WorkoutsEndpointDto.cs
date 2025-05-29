using Newtonsoft.Json;

namespace HevySummary.DTOs;

public class WorkoutsEndpointDto
{
    public int Page { get; set; }
    
    [JsonProperty("page_count")]
    public int PageCount { get; set; }
    
    public List<WorkoutDto> Workouts { get; set; }
}