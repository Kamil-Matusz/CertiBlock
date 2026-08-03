using CertiBlock.Services.Certificates.Core;
using CertiBlock.Shared.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddCore(builder.Configuration);

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