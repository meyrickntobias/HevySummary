using HevySummary.DTOs;
using Newtonsoft.Json;

namespace HevySummary.Services;

public class HevyApiService : IHevyApiService
{
    private readonly HttpClient _httpClient;

    public HevyApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ICacheService cacheService)
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
            var request = new HttpRequestMessage(HttpMethod.Get, $"/v1/workouts?page={page}&pageSize=10");
            var response = await _httpClient.SendAsync(request);
            var workoutsResponse =
                JsonConvert.DeserializeObject<WorkoutsEndpointDto>(await response.Content.ReadAsStringAsync()) ?? new WorkoutsEndpointDto();
            
            if (workoutsResponse.Workouts.Count == 0)
            {
                return workouts;
            }
            
            workouts.AddRange(workoutsResponse.Workouts);
            earliestFetchedWorkoutDate = DateOnly.FromDateTime(workoutsResponse.Workouts[^1].StartTime);
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
    public async Task<List<ExerciseTemplateDto>> GetExerciseTemplates(List<string> exerciseIds)
    {
        List<Task<ExerciseTemplateDto?>> tasks = [];
        tasks.AddRange(exerciseIds.Select(exerciseId => new HttpRequestMessage(HttpMethod.Get, $"/v1/exercise_templates/{exerciseId}"))
            .Select(request => _httpClient.SendAsync(request)
                .ContinueWith(responseTask =>
                {
                    var response = responseTask.Result;
                    var content = response.Content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject<ExerciseTemplateDto>(content);
                })));

        var exerciseTemplates = await Task.WhenAll(tasks);

        return exerciseTemplates
            .Where(et => et != null)
            .Select(et => et!)
            .ToList();
    }
}