using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoDemo.Data;

namespace TodoDemo.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
   //SQL
   // protected override IHost CreateHost(IHostBuilder builder)
   // {
   //    builder.UseEnvironment("Test");
   //    builder.ConfigureServices(services =>
   //    {
   //       // Remove the app's DbContext registration
   //       var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TodoDbContext>));
   //       if (descriptor != null)
   //          services.Remove(descriptor);
   //
   //       services.AddDbContext<TodoDbContext>(options =>
   //       {
   //          options.UseSqlServer(
   //             "Server=.\\SQLExpress;Database=TodoDemoTest;Trusted_Connection=True;TrustServerCertificate=True");
   //       });
   //
   //       var sp = services.BuildServiceProvider();
   //       using var scope = sp.CreateScope();
   //       var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
   //       db.Database.EnsureDeleted();
   //       db.Database.EnsureCreated();
   //    });
   //    return base.CreateHost(builder);
   // }
   
   //SQlite
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