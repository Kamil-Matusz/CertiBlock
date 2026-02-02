using CertiBlock.Services.Ethereum.Application;
using CertiBlock.Services.Ethereum.Core;
using CertiBlock.Services.Ethereum.Infrastructure;

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

app.MapControllers();

app.MapHealthChecks("/health");

app.Run();