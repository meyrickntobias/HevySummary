using System.Text.Json;
using System.Text.Json.Serialization;
using HevySummary.DTOs;
using HevySummary.Models;

namespace HevySummary.Helpers;

public class WorkoutEventsJsonConverter : JsonConverter<List<WorkoutEvent?>>
{
    public override List<WorkoutEvent?> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException();
        }

        List<WorkoutEvent?> workoutEvents = [];
        
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                return workoutEvents;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }
            
            // Start of object
            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            // Type property
            reader.Read();
            var type = reader.GetString();

            switch (type)
            {
                case "updated":
                {
                    reader.Read();
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.StartObject)
                    {
                        throw new JsonException();
                    }

                    var workout = JsonSerializer.Deserialize<WorkoutDto>(ref reader, options);
                    if (workout is null)
                    {
                        throw new JsonException();
                    }
                
                    workoutEvents.Add(new WorkoutUpdatedEvent(workout)
                    {
                        Type = "updated"
                    });
                    break;
                }
                case "deleted":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }
                    reader.Read();
                    var id = reader.GetGuid();
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }
                    reader.Read();
                    var deletedAtDate = reader.GetDateTime();

                    workoutEvents.Add(new WorkoutDeletedEvent(id, deletedAtDate)
                    {
                        Type = "deleted"
                    });
                    break;
                }
                default:
                    throw new JsonException();
            }
            
            if (!reader.Read() || reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }
        }
        
        return workoutEvents;
    }

    public override void Write(Utf8JsonWriter writer, List<WorkoutEvent?> value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}