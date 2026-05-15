using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using MongoDB.EntityFrameworkCore.Infrastructure;
using KeepCalm.Data;
using KeepCalm.Services;

var builder = WebApplication.CreateBuilder(args);

// Microsoft.Extensions.Logging (Console + Debug)
builder.Logging
    .AddConsole()
    .AddDebug()
    .AddFilter("Microsoft", LogLevel.Warning)
    .AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);

// MongoDB
var mongoUri = builder.Configuration.GetValue<string>("MongoDb:Uri") ?? "mongodb://localhost:27017";
var dbName = builder.Configuration.GetValue<string>("MongoDb:DatabaseName") ?? "keepcalm";

builder.Services.AddDbContext<MongoDbContext>(options =>
    options.UseMongoDB(mongoUri, dbName));

// Services
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IFocusSessionService, FocusSessionService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "KeepCalm API",
        Version = "v1",
        Description = "API para gerenciamento de tarefas — app para pessoas ansiosas"
    });
});

var app = builder.Build();

// Map controllers
app.MapControllers();

// Swagger (dev only)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "KeepCalm API v1");
        c.RoutePrefix = "swagger";
    });
}

// CORS
app.UseCors("AllowFrontend");

// Global exception handling
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Unhandled exception occurred");
        await HandleExceptionAsync(context, ex);
    }
});

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

static async Task HandleExceptionAsync(HttpContext context, Exception exception)
{
    context.Response.ContentType = "application/json";

    var statusCode = exception switch
    {
        KeyNotFoundException => StatusCodes.Status404NotFound,
        ArgumentException => StatusCodes.Status400BadRequest,
        _ => StatusCodes.Status500InternalServerError
    };

    context.Response.StatusCode = statusCode;

    var body = System.Text.Json.JsonSerializer.Serialize(new
    {
        error = statusCode == StatusCodes.Status500InternalServerError
            ? "An unexpected error occurred"
            : exception.Message,
        timestamp = DateTime.UtcNow
    });

    await context.Response.WriteAsync(body);
}
