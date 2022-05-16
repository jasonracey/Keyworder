using Keyworder;
using Keyworder.Data;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

// For reading these values during local development see the link below:
// https://medium.com/datadigest/user-secrets-in-asp-net-core-with-jetbrains-rider-26c381177391
var applicationInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
var applicationInsightsInstrumentationKey = builder.Configuration["APPLICATIONINSIGHTS_INSTRUMENTATION_KEY"];
var storageAccountConnectionString = builder.Configuration["STORAGE_ACCOUNT_CONNECTION_STRING"];

builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
{
    ConnectionString = applicationInsightsConnectionString
});
using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
    .SetMinimumLevel(LogLevel.Trace)
    .AddConsole()
    .AddApplicationInsights(applicationInsightsInstrumentationKey));
builder.Services.AddKeyworderServices(storageAccountConnectionString, loggerFactory.CreateLogger<KeywordService>());
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.Run();