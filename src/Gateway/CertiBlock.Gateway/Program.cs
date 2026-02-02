using CertiBlock.Gateway;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddGatewayExtensions(builder.Configuration);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapOpenApi();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.MapReverseProxy();

app.Run();
