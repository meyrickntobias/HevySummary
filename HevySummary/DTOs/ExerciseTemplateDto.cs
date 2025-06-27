using Newtonsoft.Json;

namespace HevySummary.DTOs;

public class ExerciseTemplateDto
{
    public required string Id { get; set; }
    public required string Title { get; set; }
    public required string Type { get; set; }
    
    [JsonProperty("primary_muscle_group")]
    public required string PrimaryMuscleGroup  { get; set; }

    [JsonProperty("secondary_muscle_groups")]
    public List<string> SecondaryMuscleGroups { get; set; } = [];
    [JsonProperty("is_custom")]
    public required string IsCustom { get; set; }
}