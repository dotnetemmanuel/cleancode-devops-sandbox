namespace TodoDemo.Tests;

public class TodoApiHealthCheckerTests : IClassFixture<CustomWebApplicationFactory>
{ 
    private readonly HttpClient _client;

    public TodoApiHealthCheckerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthCheck_ReturnsSuccessAndJson()
    {
        //Arrange
        var requestUri = "/api/todos";

        //Act
        var response = await _client.GetAsync(requestUri);

        //Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.True(response.Content.Headers.ContentLength > 0);
    }
}