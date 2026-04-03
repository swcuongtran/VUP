using Microsoft.AspNetCore.Mvc;
using VUP.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<VupProcessor>();

var app = builder.Build();

app.MapPost("/api/extract", ([FromBody]WordNode treeRoot, VupProcessor processor) =>
{
    var result = processor.Process(treeRoot);
    return result != null ? Results.Ok(result) : Results.BadRequest("Failed to process the input tree.");
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
