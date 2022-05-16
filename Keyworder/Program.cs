using Keyworder;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Log.Information("Starting Keyworder application");

try
{
    var builder = WebApplication.CreateBuilder(args);

    // For reading these values during local development see the link below:
    // https://medium.com/datadigest/user-secrets-in-asp-net-core-with-jetbrains-rider-26c381177391
    var applicationInsightsConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
    var storageAccountConnectionString = builder.Configuration["STORAGE_ACCOUNT_CONNECTION_STRING"];

    builder.Services.AddApplicationInsightsTelemetry(new ApplicationInsightsServiceOptions
    {
        ConnectionString = applicationInsightsConnectionString
    });
    builder.Services.AddKeyworderServices(storageAccountConnectionString);
    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    
    builder.Host
        .UseSerilog((_, services, loggerConfiguration) => loggerConfiguration
            .WriteTo.ApplicationInsights(
                services.GetRequiredService<TelemetryConfiguration>(),
                TelemetryConverter.Traces));

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Keyworder application start failed");
}
finally
{
    Log.CloseAndFlush();
}