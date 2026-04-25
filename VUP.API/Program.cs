using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VUP.Core.Engine;
using VUP.Core.Entities;
using VUP.Core.Models; 

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

    return result != null ? Results.Ok(result) : Results.NotFound("Không tìm thấy Phrasal Verb nào trong câu.");
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