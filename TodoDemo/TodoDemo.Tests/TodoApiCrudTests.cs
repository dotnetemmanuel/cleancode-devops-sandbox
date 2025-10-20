using System.Net;
using System.Net.Http.Json;
using TodoDemo.Models;

namespace TodoDemo.Tests;

public class TodoApiCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TodoApiCrudTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CanCreateTodo()
    {
        //Arrange
        var newTodo = new TodoItem
        {
            Title = "Integration Test Todo",
            Description = "Created by integration test",
            IsDone = false
        };

        //Act
        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);

        //Assert
        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.Equal(newTodo.Title, created.Title);
        Assert.Equal(newTodo.Description, created.Description);
        Assert.False(created.IsDone);
        Assert.True(created.Id > 0);
    }

    [Fact]
    public async Task CanReadTodo()
    {
        //Arrange
        var newTodo = new TodoItem
        {
            Title = "Read Test",
            Description = "Read test desc",
            IsDone = false
        };

        var createResp = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoItem>();

        //Act
        var getResp = await _client.GetAsync($"/api/todos/{created.Id}");

        //Assert
        getResp.EnsureSuccessStatusCode();
        var fetched = await getResp.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal(created.Title, fetched.Title);
        Assert.Equal(created.Description, fetched.Description);
        Assert.False(fetched.IsDone);
    }

    [Fact]
    public async Task CanUpdateTodo()
    {
        //Arrange
        var newTodo = new TodoItem
        {
            Title = "Update Test",
            Description = "Before update",
            IsDone = false
        };
        var createResp = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoItem>();

        //Act
        created.Title = "Updated Title";
        created.Description = "After update";
        created.IsDone = true;

        var updateResp = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", created);

        //Assert
        updateResp.EnsureSuccessStatusCode();

        var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
        var updated = await getResp.Content.ReadFromJsonAsync<TodoItem>();
        Assert.Equal("Updated Title", updated.Title);
        Assert.Equal("After update", updated.Description);
        Assert.True(updated.IsDone);
    }

    [Fact]
    public async Task CanDeleteTodo()
    {
        //Arrange
        var newTodo = new TodoItem
        {
            Title = "Delete Test",
            Description = "To be deleted",
            IsDone = false
        };
        
        var createdResp = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var created = await createdResp.Content.ReadFromJsonAsync<TodoItem>();

        //Act
        var deleteResp = await _client.DeleteAsync($"/api/todos/{created.Id}");

        //Assert
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

        var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }
    
    [Fact]
    public async Task CanListTodos()
    {
        // Arrange: create a couple of todos
        await _client.PostAsJsonAsync("/api/todos", new TodoItem { Title = "List 1", Description = "A", IsDone = false });
        await _client.PostAsJsonAsync("/api/todos", new TodoItem { Title = "List 2", Description = "B", IsDone = false });

        // Act
        var response = await _client.GetAsync("/api/todos");

        // Assert
        response.EnsureSuccessStatusCode();
        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        Assert.NotNull(todos);
        Assert.True(todos.Count >= 2); // Should have at least the two we just added
    }
}