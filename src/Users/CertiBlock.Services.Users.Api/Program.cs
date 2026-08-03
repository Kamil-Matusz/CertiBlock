using CertiBlock.Services.Users.Application;
using CertiBlock.Services.Users.Core;
using CertiBlock.Services.Users.Infrastructure;
using CertiBlock.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services
    .AddCore()
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseHttpsRedirection();

app.UseInfrastructure();

app.UseObservability();

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();