using Newtonsoft.Json;

namespace HevySummary.DTOs;

public class ExerciseTemplateDto
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Type { get; set; }
    
    [JsonProperty("primary_muscle_group")]
    public string PrimaryMuscleGroup  { get; set; }
    [JsonProperty("secondary_muscle_groups")]
    public List<string> SecondaryMuscleGroups  { get; set; }
    [JsonProperty("is_custom")]
    public string IsCustom { get; set; }
}