using CertiBlock.Services.Polygon.Application;
using CertiBlock.Services.Polygon.Core;
using CertiBlock.Services.Polygon.Infrastructure;
using CertiBlock.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services
    .AddCore(builder.Configuration)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseInfrastructure();

app.UseObservability();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();