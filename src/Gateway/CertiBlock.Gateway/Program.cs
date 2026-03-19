using CertiBlock.Gateway;
using CertiBlock.Shared.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddGatewayExtensions(builder.Configuration);

// Error Handling
builder.Services.AddErrorHandling();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseErrorHandling();

app.MapReverseProxy();

app.Run();
