using CertiBlock.Services.Metrics.Core;
using CertiBlock.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services
    .AddCore(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseInfrastructure();

app.UseObservability();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();