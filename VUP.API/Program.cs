using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VUP.Core.Engine;
using VUP.Core.Entities;
using VUP.Core.Models;
using VUP.Core.Rules;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<VupDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<VupProcessor>();

var modelsPath = Path.Combine(Directory.GetCurrentDirectory(), "models");
builder.Services.AddSingleton(new StanfordParserService(modelsPath));

var matcherTypes = typeof(BaseMatcher).Assembly.GetTypes()
    .Where(t => t.IsSubclassOf(typeof(BaseMatcher)) && !t.IsAbstract);

foreach (var type in matcherTypes)
{
    builder.Services.AddSingleton(typeof(BaseMatcher), type);
}

builder.Services.AddSingleton<VupProcessor>();

var app = builder.Build();

app.MapPost("/api/extract", ([FromBody] WordNode treeRoot, VupProcessor processor) =>
{
    var result = processor.Process(treeRoot);
    return result != null ? Results.Ok(result) : Results.BadRequest("Failed to process the input tree.");
});

app.MapPost("/api/analyze", ([FromBody] AnalyzeRequest req,
    StanfordParserService parser,
    VupProcessor processor) =>
{
    var treeRoot = parser.ParseToTree(req.Text);

    if (treeRoot == null)
        return Results.BadRequest("Không thể phân tích cú pháp câu này.");

    var result = processor.Process(treeRoot);

    return Results.Ok(new
    {
        Extraction = result ?? new ExtractionResult("Unknown", "Unknown", "Unknown", 0, false),
        DebugTree = treeRoot // Xuất toàn bộ cây Stanford ra Swagger
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

public record AnalyzeRequest(string Text);