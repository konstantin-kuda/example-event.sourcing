using Cocona;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder();
builder.Logging.AddDebug();

var app = builder.Build();
app.AddCommand((ILogger<Program> logger) =>
{
    logger.LogInformation("Hello World!");
});

app.Run();