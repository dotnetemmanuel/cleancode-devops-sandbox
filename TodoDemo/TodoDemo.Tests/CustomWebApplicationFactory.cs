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
    // This method lets us change how the web part of the app is set up — like replacing services.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 🧹 Step 1: Remove the app’s original database setup.
            // Why? Because we don’t want to use the real database in tests.
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // 🧹 Step 2: Remove any existing SQLite connection, just in case something was already registered.
            var dbConnectionDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SqliteConnection));
            if (dbConnectionDescriptor != null)
                services.Remove(dbConnectionDescriptor);

            // 🧪 Step 3: Add a new SQLite connection that lives in memory (RAM only).
            // This means the database is super fast and disappears after the test ends.
            services.AddSingleton<SqliteConnection>(container =>
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open(); // Keep it open so EF Core doesn’t wipe it between uses.
                return connection;
            });

            // 🧪 Step 4: Register our test version of the database context.
            // It uses the in-memory SQLite connection we just created.
            services.AddDbContext<TodoDbContext>((container, options) =>
            {
                var connection = container.GetRequiredService<SqliteConnection>();
                options.UseSqlite(connection);
            });
        });

        // 🏷️ Optional: Set the environment to "Test" so the app knows it’s in test mode.
        // builder.UseEnvironment("Test");
    }

    // This method runs after the app is fully built.
    // We use it to make sure the test database is ready to go.
    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder); // Build the app like normal.

        // 🛠️ After the app is built, we create the database schema.
        // This is like saying: "Make sure the tables exist before we run any tests."
        using var scope = host.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
        db.Database.EnsureCreated(); // Create tables if they don’t exist.

        return host;
    }
}