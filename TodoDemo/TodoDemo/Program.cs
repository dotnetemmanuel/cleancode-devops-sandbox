using Microsoft.EntityFrameworkCore;
using TodoDemo.Components;
using TodoDemo.Data;
using TodoDemo.Repositories;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default") ??
                       throw new InvalidOperationException("missing connection string 'Default' .");

builder.Services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

builder.Services.AddControllers();

// Register HttpContextAccessor and a scoped HttpClient with a request-based BaseAddress
builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services.AddScoped(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var req = httpContextAccessor.HttpContext?.Request;
    var baseUri = req != null
        ? new Uri($"{req.Scheme}://{req.Host.Value}{req.PathBase}")
        : new Uri("https://localhost:5001");
    return new System.Net.Http.HttpClient { BaseAddress = baseUri };
});

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();
app.MapControllers();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

public partial class Program { }