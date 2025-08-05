using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System;
using TidyTop.App.Services;

namespace TidyTop.App;

public partial class App : Application
{
    /// <summary>
    /// Gets or sets the application host service
    /// </summary>
    public ApplicationHostService? AppHost { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Create the main window with dependency injection support
            var mainWindow = new MainWindow();
            
            // Set the data context with services if needed
            if (AppHost != null)
            {
                // TODO: Set up the main window's data context with required services
                // For example:
                // mainWindow.DataContext = new MainViewModel(
                //     AppHost.GetService<ISettingsService>(),
                //     AppHost.GetService<IDesktopIconService>(),
                //     AppHost.GetService<IFenceService>(),
                //     AppHost.GetService<IDesktopLayoutService>());
            }
            
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    /// <summary>
    /// Gets a service of the specified type from the dependency injection container
    /// </summary>
    /// <typeparam name="T">The type of service to get</typeparam>
    /// <returns>A service of the specified type</returns>
    public T GetService<T>() where T : notnull
    {
        if (AppHost == null)
            throw new InvalidOperationException("Application host service is not initialized");

        return AppHost.GetService<T>();
    }

    /// <summary>
    /// Gets a service of the specified type from the dependency injection container
    /// </summary>
    /// <param name="serviceType">The type of service to get</param>
    /// <returns>A service object of the specified type</returns>
    public object GetService(Type serviceType)
    {
        if (AppHost == null)
            throw new InvalidOperationException("Application host service is not initialized");

        return AppHost.GetService(serviceType);
    }
}