using CertiBlock.Gateway;
using CertiBlock.Shared.CORS;
using CertiBlock.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();

builder.Services.AddGatewayExtensions(builder.Configuration);

// Error Handling
builder.Services.AddErrorHandling();

var app = builder.Build();

app.UseCorsPolicy();

app.UseHttpsRedirection();

app.UseErrorHandling();

app.UseRequestTimeouts();
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health");
app.MapReverseProxy();

app.Run();
