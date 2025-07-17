using System.Collections.Immutable;
using System.Text.Json;
using HevySummary.DTOs;
using HevySummary.Models;

namespace HevySummary.Services;

public class HevyApiService : IWorkoutApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _serializerOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower };

    public HevyApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClient = httpClientFactory.CreateClient();
        _httpClient.BaseAddress = new Uri("https://api.hevyapp.com");
        var apiKey = configuration.GetValue<string>("HevyApiKey");
        if (apiKey == null)
        {
            throw new Exception("Hevy Api Key not set");
        }
        _httpClient.DefaultRequestHeaders.Add("api-key", apiKey);
    }
    
    /// <summary>
    /// Fetches all workouts since the specified date.
    /// Will keep requesting workouts until the specified date is reached or no more workouts are available.
    /// </summary>
    /// <param name="earliestRequestedWorkoutDate"></param>
    /// <returns></returns>
    public async Task<List<WorkoutDto>> GetWorkoutsSince(DateOnly earliestRequestedWorkoutDate)
    {
        var earliestFetchedWorkoutDate = DateOnly.MaxValue;
        var page = 1;
        var workouts = new List<WorkoutDto>();
        
        while (earliestFetchedWorkoutDate > earliestRequestedWorkoutDate)
        {
            var workoutsResponse = await _httpClient.GetFromJsonAsync<WorkoutsEndpointDto>(
                $"/v1/workouts?page={page}&pageSize=10", _serializerOptions) ?? new WorkoutsEndpointDto();
            
            if (workoutsResponse.Workouts.Count == 0)
            {
                return workouts;
            }
            
            workouts.AddRange(workoutsResponse?.Workouts ?? []);
            earliestFetchedWorkoutDate = DateOnly.FromDateTime(workoutsResponse!.Workouts[^1].StartTime);
            page++;
        }
        
        return workouts
            .Where(w => DateOnly.FromDateTime(w.StartTime) >= earliestRequestedWorkoutDate)
            .ToList();
    }
    
    /// <summary>
    /// Fetches exercise templates for the specified exercise IDs.
    /// Runs requests in parallel to speed things up.
    /// </summary>
    /// <param name="exerciseIds">The exercise templates to fetch</param>
    /// <returns>The list of exercise templates</returns>
    public async Task<ISet<ExerciseTemplateDto>> GetExerciseTemplates(IEnumerable<string> exerciseIds)
    {
        var exerciseTemplates = await Task.WhenAll(exerciseIds.Select(exerciseId => 
            _httpClient.GetFromJsonAsync<ExerciseTemplateDto>($"/v1/exercise_templates/{exerciseId}", _serializerOptions)));

        return exerciseTemplates
            .Where(et => et != null)
            .Select(et => et!)
            .ToHashSet();
    }
    
    /// <summary>
    /// Fetches all exercise templates.
    /// Runs requests in parallel to speed things up.
    /// </summary>
    /// <returns>The list of all exercise templates</returns>
    public async Task<List<ExerciseTemplateDto>> GetAllExerciseTemplates()
    {
        var firstBatch = await _httpClient.GetFromJsonAsync<ExerciseTemplateResponseDto>("/v1/exercise_templates?page=1&pageSize=100", _serializerOptions);

        if (firstBatch == null)
        {
            return [];
        }

        List<Task<ExerciseTemplateResponseDto?>> tasks = [];
        for (var i = 2; i <= firstBatch.PageCount; i++)
        {
            var task = _httpClient.GetFromJsonAsync<ExerciseTemplateResponseDto>(
                $"/v1/exercise_templates?page={i}&pageSize=100", _serializerOptions);
            tasks.Add(task);
        }
        
        var exerciseTemplateResponses = await Task.WhenAll(tasks);

        var mergedResponse = exerciseTemplateResponses.Append(firstBatch);

        var allExerciseTemplates = mergedResponse
            .SelectMany(response => response?.ExerciseTemplates ?? []);

        return allExerciseTemplates.OrderBy(et => et.Title).ToList();
    }

    public async Task<List<WorkoutEvent>> GetWorkoutEventsSince(DateTime since)
    {
        var response = await _httpClient.GetFromJsonAsync<WorkoutEventResponse>(
            $"/v1/workouts/events?page=1&pageSize=10&since={since:yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'}", _serializerOptions);

        return response?.Events ?? [];
    }

    public async Task<List<RoutineDto>> GetRoutines()
    {
        // Assume 1 page of workouts
        var response = await _httpClient.GetFromJsonAsync<RoutineResponse>($"/v1/routines?page=1&pageSize=10", _serializerOptions);

        return response?.Routines ?? [];
    }
}

