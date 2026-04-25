using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VUP.Core.Engine;
using VUP.Core.Entities;
using VUP.Core.Models; // Đảm bảo import Models từ Core để dùng WordNode

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 1. Đăng ký Database
builder.Services.AddDbContext<VupDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Đăng ký VupProcessor (Bộ não)
builder.Services.AddSingleton<VupProcessor>();

// 3. Đăng ký StanfordParserService (Đôi mắt) - THIẾU DÒNG NÀY SẼ BỊ LỖI RUNTIME
var modelsPath = Path.Combine(Directory.GetCurrentDirectory(), "models");
builder.Services.AddSingleton(new StanfordParserService(modelsPath));

var app = builder.Build();

// Endpoint 1: Bóc tách từ cây cú pháp dựng sẵn
app.MapPost("/api/extract", ([FromBody] WordNode treeRoot, VupProcessor processor) =>
{
    var result = processor.Process(treeRoot);
    return result != null ? Results.Ok(result) : Results.BadRequest("Failed to process the input tree.");
});

// Endpoint 2: Phân tích trực tiếp từ văn bản thô
app.MapPost("/api/analyze", ([FromBody] AnalyzeRequest req,
    StanfordParserService parser,
    VupProcessor processor) =>
{
    // 1. Phân tích văn bản thô thành cây cú pháp
    var treeRoot = parser.ParseToTree(req.Text);

    if (treeRoot == null)
        return Results.BadRequest("Không thể phân tích cú pháp câu này.");

    // 2. Chạy qua 15 Rule C# của chúng ta
    var result = processor.Process(treeRoot);

    // 3. Trả kết quả
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

// DTO định nghĩa input cho /api/analyze
public record AnalyzeRequest(string Text);