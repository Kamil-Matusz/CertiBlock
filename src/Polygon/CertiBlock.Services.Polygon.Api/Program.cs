using CertiBlock.Services.Polygon.Application;
using CertiBlock.Services.Polygon.Core;
using CertiBlock.Services.Polygon.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

builder.Services
    .AddCore(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseInfrastructure();

app.MapControllers();

app.Run();