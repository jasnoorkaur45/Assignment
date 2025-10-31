using BasicTaskManager;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicyName = "FrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyName, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});

var app = builder.Build();

app.UseCors(CorsPolicyName);

var tasks = new List<TaskItem>
{
    new TaskItem { Id = Guid.NewGuid(), Description = "Buy groceries", IsCompleted = false },
    new TaskItem { Id = Guid.NewGuid(), Description = "Walk the dog", IsCompleted = true }
};

app.MapGet("/api/tasks", () => Results.Ok(tasks));

app.MapPost("/api/tasks", (TaskItem newTask) =>
{
    if (string.IsNullOrWhiteSpace(newTask.Description))
    {
        return Results.BadRequest(new { message = "Description is required" });
    }

    var task = new TaskItem
    {
        Id = newTask.Id == Guid.Empty ? Guid.NewGuid() : newTask.Id,
        Description = newTask.Description.Trim(),
        IsCompleted = newTask.IsCompleted
    };
    tasks.Add(task);
    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.MapPut("/api/tasks/{id:guid}", (Guid id, TaskItem update) =>
{
    var existing = tasks.FirstOrDefault(t => t.Id == id);
    if (existing is null) return Results.NotFound();

    if (!string.IsNullOrWhiteSpace(update.Description))
    {
        existing.Description = update.Description.Trim();
    }

    existing.IsCompleted = update.IsCompleted;

    return Results.Ok(existing);
});

app.MapDelete("/api/tasks/{id:guid}", (Guid id) =>
{
    var removed = tasks.RemoveAll(t => t.Id == id);
    return removed > 0 ? Results.NoContent() : Results.NotFound();
});

var port = Environment.GetEnvironmentVariable("PORT") ?? "5005";
app.Urls.Add($"http://localhost:{port}");

app.Run();
