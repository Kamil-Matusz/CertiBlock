using CertiBlock.Gateway;
using CertiBlock.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen();

builder.Services.AddGatewayExtensions(builder.Configuration);

// Error Handling
builder.Services.AddErrorHandling();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseErrorHandling();

app.MapReverseProxy();

app.Run();
