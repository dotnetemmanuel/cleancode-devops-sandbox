# Demo-manus: Integrationstester med `TodoDemo.Tests`

## **Syfte med demon**

Visa hur man skriver och kör integrationstester i .NET för ett riktigt API och databas – utan mocking. Vi testar hela kedjan: API-endpoints, databas och dataflöde.

---

## **Steg 1: Projektstruktur och förutsättningar**

**Visa appen i webbläsaren**

**Visa i Rider:**
- Lösningen innehåller:
  - `TodoDemo` (huvudprojektet, API + databas)
  - `TodoDemo.Tests` (testprojektet, xUnit)

      => SKAPA

      => Lägg till följande nuget packages:
      - `Microsoft.AspNetCore.Mvc.Testing`
      - `Microsoft.Data.SqlClient`
      - `Microsoft.EntityFrameworkCore.Sqlite`

      => lägga till 'Följande kod till test csproj

      ```xml
      <!-- Needs to be added for tests to run. Files need to be copied from web project to test project -->
          <PropertyGroup>
              <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
          </PropertyGroup>

          <Target Name="EnsureWebAppBuiltAndCopyDeps" AfterTargets="Build">
              <MSBuild
                      Projects="..\TodoDemo\TodoDemo.csproj"
                      Targets="Build"
                      Properties="Configuration=$(Configuration);TargetFramework=net9.0" />

              <ItemGroup>
                  <WebDepsFiles Include="..\TodoDemo\bin\$(Configuration)\net9.0\*.deps.json" Condition="Exists('..\TodoDemo\bin\$(Configuration)\net9.0')" />
                  <WebTestHostFiles Include="..\TodoDemo\bin\$(Configuration)\net9.0\testhost*.deps.json" Condition="Exists('..\TodoDemo\bin\$(Configuration)\net9.0')" />
              </ItemGroup>

              <Copy
                      SourceFiles="@(WebDepsFiles);@(WebTestHostFiles)"
                      DestinationFolder="$(OutputPath)"
                      SkipUnchangedFiles="true" />
          </Target>

          <Target Name="CopyTestHostFromNuGet" AfterTargets="Build">
              <ItemGroup>
                  <NuGetTestHostFiles Include="$(NuGetPackageRoot)microsoft.testplatform.testhost\**\testhost*.deps.json" />
                  <NuGetTestHostFiles Include="$(UserProfile)\.nuget\packages\microsoft.testplatform.testhost\**\testhost*.deps.json" />
              </ItemGroup>

              <Copy
                      SourceFiles="@(NuGetTestHostFiles)"
                      DestinationFolder="$(OutputPath)"
                      SkipUnchangedFiles="true"
                      ContinueOnError="true" />
          </Target>
      ```

      => Lägg till följande till webproject .csproj if not already TodoApiHealthCheckerTests
      ```xml
      <PreserveCompilationContext>true</PreserveCompilationContext> <!-- Needs to be added for tests to run -->
      ```

- Testprojektet refererar till huvudprojektet och har nödvändiga NuGet-paket:
  - `Microsoft.AspNetCore.Mvc.Testing`
  - `Microsoft.EntityFrameworkCore.SqlServer` eller `Sqlite`
  - `xunit`, `xunit.runner.visualstudio`

**Förklara:**
- Vi använder en riktig databas (t.ex. SQL Server Express eller SQLite) för att testa på riktigt.
- Testerna startar applikationen in-memory via `WebApplicationFactory` – vi behöver inte köra API:t separat.

---

## **Steg 2: Testkonfiguration**

**Visa:**
- `xunit.runner.json` med `shadowCopy: false` för att undvika problem med saknade .deps-filer. => SKAPA

**Exempel:**

```json
{
  "shadowCopy": false
}
```

---

## **Steg 3: CustomWebApplicationFactory**

**Förklara:**
- Vi skapar en egen `CustomWebApplicationFactory` som ser till att testdatabasen används och att databasen nollställs inför varje testrunda.

**Visa koden:**
[TodoDemo.Tests/CustomWebApplicationFactory.cs](file:///D:/Education/cleancode-devops-sandbox/TodoDemo/TodoDemo.Tests/CustomWebApplicationFactory.cs)

```csharp
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoDemo.Data;

namespace TodoDemo.Tests;

// This class helps us spin up a fake version of our web app — just for testing.
// It lets us swap out real services (like the database) with test-friendly ones.
public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    // TProgram = just a placeholder name for the app’s starting class (usually called Program).
    // Think of it like saying: "Use whatever class starts the app here."

    // : class = a simple rule that says:
    // "TProgram must be a real class, not a number or a tiny data type."

    // This method lets us change how the web part of the app is set up — like replacing services.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 🧹 Step 1: Remove the app’s original database setup.
            // Why? Because Program.cs already told the app to use the "real" database.
            // In tests we don’t want that, so we take it out here.

            // A "descriptor" is just a little note that says:
            // "This service type has been registered in the app’s service list."
            // Here we look for the descriptor that matches DbContextOptions<TodoDbContext>,
            // which is the setup for the real database. If we find it, we remove it.

            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // 🧹 Step 2: Remove any existing SQLite connection.
            // Descriptor here means: "a description of a service that was registered."
            // If the app already had a SQLite connection lying around, we clear it out
            // so we don’t accidentally use the wrong one in tests.
            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SqliteConnection));
            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            // 🧪 Step 3: Add a new SQLite connection that lives only in memory.
            // This means the database is super fast and disappears after the test ends.
            // We keep the connection open so EF Core can reuse it during the whole test run.

            services.AddSingleton<SqliteConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open(); // Keep it open so EF Core doesn’t wipe it between uses.
                return connection;
            });

            // 🧪 Step 4: Register our test version of the database context.
            // Here we tell EF Core: "Use the in‑memory SQLite connection we just created."
            // So whenever the app asks for TodoDbContext in tests, it gets this fake one.

            services.AddDbContext<TodoDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<SqliteConnection>();
                options.UseSqlite(connection);
            });
        });
    }

    // This method runs after the app is fully built.
    // We use it to make sure the test database is ready to go.
    // We override it so we can do some extra setup just for tests.
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder); // Build the app like normal (using Program.cs and all its settings).

        // 🛠️ Extra step for tests:
        // After the app is built, we make sure the test database is ready.
        // Think of it like saying: "Before we run any tests, check that the tables exist."

        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        db.Database.EnsureCreated(); 
        // This command creates the database tables if they don’t already exist.
        // It’s like setting up the playing field before the game starts.


        return host; // Finally, return the fully prepared test host.

    }
}
```

---

## **Steg 4: Health Check-test**

**Förklara:**
- Vi börjar med att testa att API:t startar och svarar på `/api/todos`.

**Visa koden:**
[TodoDemo.Tests/TodoApiHealthCheckerTests.cs](file:///D:/Education/cleancode-devops-sandbox/TodoDemo/TodoDemo.Tests/TodoApiHealthCheckerTests.cs)

```csharp
namespace TodoDemo.Tests;

public class TodoApiHealthCheckerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{ 
    private readonly HttpClient _client;

    public TodoApiHealthCheckerTests(CustomWebApplicationFactory<Program> factory)
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
```

---

## **Steg 5: CRUD-integrationstester**

**Förklara:**
- Vi testar hela CRUD-flödet: skapa, läsa, uppdatera och ta bort todos via API:t.

**Visa och gå igenom koden:**
[TodoDemo.Tests/TodoApiCrudTests.cs](file:///D:/Education/cleancode-devops-sandbox/TodoDemo/TodoDemo.Tests/TodoApiCrudTests.cs)

```csharp
using System.Net;
using System.Net.Http.Json;
using TodoDemo.Models;

namespace TodoDemo.Tests;

public class TodoApiCrudTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoApiCrudTests(CustomWebApplicationFactory<Program> factory)
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
```

---

## **Steg 6: Summering och frågor**

**Sammanfatta:**
- Vi har nu testat hela API:t och databasen på riktigt – utan mocking.
- Integrationstester ger oss trygghet att hela kedjan fungerar, och är enkla att köra både lokalt och i CI/CD.

**Fråga publiken:**
- Vad är skillnaden mellan integrationstest och enhetstest?
- När är det lämpligt att använda riktiga databaser i tester?
