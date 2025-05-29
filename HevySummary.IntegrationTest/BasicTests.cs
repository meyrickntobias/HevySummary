using Microsoft.AspNetCore.Mvc.Testing;

namespace HevySummary.IntegrationTest;

public class BasicTests
{
    [Fact]
    public async Task ShouldReturnMuscleGroupsPerSet()
    {
        var appFactory = new WebApplicationFactory<Program>();
        var client = appFactory.CreateClient();

        var res = await client.GetAsync($"/dashboard");
        var resText = await res.Content.ReadAsStringAsync();
    }
}