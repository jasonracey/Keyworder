using Keyworder;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting Keyworder application");

    var builder = WebApplication.CreateBuilder(args);

    var options = new ApplicationInsightsServiceOptions
    {
        ConnectionString = Environment.GetEnvironmentVariable("APPLICATIONINSIGHTS_CONNECTION_STRING")
    };

    builder.Services.AddApplicationInsightsTelemetry(options);
    builder.Services.AddKeyworderServices();
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