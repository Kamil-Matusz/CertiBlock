using CertiBlock.Gateway;
using CertiBlock.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddGatewayExtensions(builder.Configuration);

// Error Handling
builder.Services.AddErrorHandling();

var app = builder.Build();

app.UseCors();

app.UseHttpsRedirection();

app.UseErrorHandling();

app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();
