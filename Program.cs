using System.Text.Json;
using System.Text.Json.Serialization;
using Anipi.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// Register services
builder.Services.AddSingleton<IAnimeService, AnimeService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // Allow all origins
            .AllowAnyMethod()     // Allow all methods: GET, POST, etc.
            .AllowAnyHeader();    // Allow all headers
    });
});

// Add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Anipi - Professional Anime API",
        Version = "v1",
        Description = "A comprehensive API for anime information",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@anipi.example.com"
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseCors("AllowAll");

// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Anipi API v1");
    c.RoutePrefix = string.Empty; // Set Swagger UI at the root
});

app.UseHttpsRedirection();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Legacy endpoints for backward compatibility
app.MapGet("/v1/anime", async (IAnimeService animeService) =>
{
    var database = await animeService.GetFullDatabaseAsync();
    return Results.Ok(database);
})
.WithName("GetAnimeListLegacy");

app.MapGet("/v1", () =>
{
    var version = new
    {
        version = "v1",
        href = "/api/v1/anime",
        message = "This endpoint is deprecated. Please use /api/v1 instead."
    };
    return Results.Ok(version);
})
.WithName("GetVersionApiLegacy");

app.Run();