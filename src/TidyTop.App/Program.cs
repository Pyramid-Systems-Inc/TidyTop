using Avalonia;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using TidyTop.App.Services;

namespace TidyTop.App;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Create the host builder
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add TidyTop services
                services.AddTidyTopServices();
                
                // Add the application host service
                services.AddHostedService<ApplicationHostService>();
            })
            .Build();

        // Start the host
        host.Start();

        // Get the application host service
        var appHost = host.Services.GetRequiredService<IHostedService>() as ApplicationHostService;
        
        // Build and start the Avalonia application
        BuildAvaloniaApp(appHost)
            .StartWithClassicDesktopLifetime(args);

        // Stop the host when the application exits
        host.StopAsync().Wait();
        host.Dispose();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp(ApplicationHostService? appHost = null)
        => AppBuilder.Configure<App>()
            .WithInterFont()
            .LogToTrace()
            .AfterSetup(builder =>
            {
                // Set the application host service
                if (appHost != null)
                {
                    var app = builder.Instance as App;
                    if (app != null)
                    {
                        app.AppHost = appHost;
                    }
                }
            })
            .UsePlatformDetect();
}
