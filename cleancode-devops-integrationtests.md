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
      - `FluentAssertions`
      - `Microsoft.AspNetCore.Mvc.Testing`
      - `Microsoft.Data.SqlClient`
      - `Microsoft.EntityFrameworkCore.SqlServer`

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
- `appsettings.Test.json` med testdatabasens connection string.
- `xunit.runner.json` med `shadowCopy: false` för att undvika problem med saknade .deps-filer. => SKAPA

**Exempel:**
```json
{
  "ConnectionStrings": {
    "Default": "Data Source=:memory:"
  },
  "UseSqlite": true
}
```

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
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoDemo.Data;

namespace TodoDemo.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
  private SqliteConnection? _connection;

     protected override IHost CreateHost(IHostBuilder builder)
     {
        builder.UseEnvironment("Test");
        builder.ConfigureServices(services =>
        {
           var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
           if (descriptor != null)
              services.Remove(descriptor);

           _connection = new SqliteConnection("Data Source=:memory:");
           _connection.Open();

           services.AddDbContext<TodoDbContext>(options =>
           {
              options.UseSqlite(_connection);
           });

           var sp = services.BuildServiceProvider();
           using var scope = sp.CreateScope();
           var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
           db.Database.EnsureCreated();
        });
        return base.CreateHost(builder);
     }

     protected override void Dispose(bool disposing)
     {
        base.Dispose(disposing);
        _connection?.Close();
        _connection?.Dispose();
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
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

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
        var response = await _client.GetAsync("/api/todos");
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
using System.Threading.Tasks;
using Xunit;
using TodoDemo.Data; // Justera namespace om nödvändigt

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
        var newTodo = new TodoItem
        {
            Title = "Integration Test",
            Description = "Created by integration test",
            IsDone = false
        };

        var response = await _client.PostAsJsonAsync("/api/todos", newTodo);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(created);
        Assert.Equal(newTodo.Title, created.Title);
        Assert.Equal(newTodo.Description, created.Description);
        Assert.False(created.IsDone);
    }

    [Fact]
    public async Task CanReadTodo()
    {
        var newTodo = new TodoItem
        {
            Title = "Read Test",
            Description = "Read test desc",
            IsDone = false
        };
        var createResp = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoItem>();

        var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
        getResp.EnsureSuccessStatusCode();
        var fetched = await getResp.Content.ReadFromJsonAsync<TodoItem>();
        Assert.NotNull(fetched);
        Assert.Equal(created.Id, fetched.Id);
        Assert.Equal(newTodo.Title, fetched.Title);
    }

    [Fact]
    public async Task CanUpdateTodo()
    {
        var newTodo = new TodoItem
        {
            Title = "Update Test",
            Description = "Before update",
            IsDone = false
        };
        var createResp = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoItem>();

        created.Title = "Updated Title";
        created.Description = "After update";
        created.IsDone = true;

        var updateResp = await _client.PutAsJsonAsync($"/api/todos/{created.Id}", created);
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
        var newTodo = new TodoItem
        {
            Title = "Delete Test",
            Description = "To be deleted",
            IsDone = false
        };
        var createResp = await _client.PostAsJsonAsync("/api/todos", newTodo);
        var created = await createResp.Content.ReadFromJsonAsync<TodoItem>();

        var deleteResp = await _client.DeleteAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResp.StatusCode);

        var getResp = await _client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResp.StatusCode);
    }

    [Fact]
    public async Task CanListTodos()
    {
        await _client.PostAsJsonAsync("/api/todos", new TodoItem { Title = "List Test 1", Description = "A", IsDone = false });
        await _client.PostAsJsonAsync("/api/todos", new TodoItem { Title = "List Test 2", Description = "B", IsDone = false });

        var response = await _client.GetAsync("/api/todos");
        response.EnsureSuccessStatusCode();

        var todos = await response.Content.ReadFromJsonAsync<List<TodoItem>>();
        Assert.NotNull(todos);
        Assert.True(todos.Count >= 2);
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
