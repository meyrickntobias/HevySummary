using System.Text.Json.Serialization;
using HevySummary.Helpers;
using HevySummary.Models;

namespace HevySummary.DTOs;

public class WorkoutEventResponse
{
    public int Page { get; set; }
    public int PageCount { get; set; }

    [JsonConverter(typeof(WorkoutEventsJsonConverter))]
    public List<WorkoutEventDto> Events { get; set; } = [];
}