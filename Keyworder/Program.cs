using Keyworder;
using Keyworder.Data;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// For reading these values during local development see the link below:
// https://medium.com/datadigest/user-secrets-in-asp-net-core-with-jetbrains-rider-26c381177391
var applicationInsightsInstrumentationKey = builder.Configuration["APPLICATIONINSIGHTS_INSTRUMENTATION_KEY"];
var storageAccountConnectionString = builder.Configuration["STORAGE_ACCOUNT_CONNECTION_STRING"];

using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Information)
    .AddConsole()
    .AddApplicationInsights(applicationInsightsInstrumentationKey));

builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    InstrumentationKey = applicationInsightsInstrumentationKey,
    EnableAdaptiveSampling = false
});
builder.Services.AddKeyworderServices(storageAccountConnectionString, loggerFactory.CreateLogger<KeywordService>());
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();