using System.Text.Json;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Implementation of the settings service
/// </summary>
public class SettingsService : ISettingsService
{
    private DesktopSettings? _settings;
    private readonly string _settingsFilePath;
    private readonly object _settingsLock = new();

    /// <summary>
    /// Event raised when settings are changed
    /// </summary>
    public event EventHandler<DesktopSettings>? SettingsChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class
    /// </summary>
    public SettingsService()
    {
        // Get the application data directory
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "TidyTop");
        
        // Create the directory if it doesn't exist
        if (!Directory.Exists(appFolder))
        {
            Directory.CreateDirectory(appFolder);
        }

        _settingsFilePath = Path.Combine(appFolder, "settings.json");
    }

    /// <inheritdoc/>
    public async Task<DesktopSettings> GetSettingsAsync()
    {
        lock (_settingsLock)
        {
            if (_settings != null)
                return _settings;
        }

        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = await File.ReadAllTextAsync(_settingsFilePath);
                var settings = JsonSerializer.Deserialize<DesktopSettings>(json);
                
                if (settings != null)
                {
                    lock (_settingsLock)
                    {
                        _settings = settings;
                    }
                    return settings;
                }
            }
        }
        catch (Exception ex)
        {
            // Log the error in a real application
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }

        // Return default settings if none could be loaded
        var defaultSettings = GetDefaultSettings();
        
        lock (_settingsLock)
        {
            _settings = defaultSettings;
        }
        
        return defaultSettings;
    }

    /// <inheritdoc/>
    public async Task<bool> SaveSettingsAsync(DesktopSettings settings)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));

        try
        {
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_settingsFilePath, json);

            lock (_settingsLock)
            {
                _settings = settings;
            }

            // Raise the SettingsChanged event
            OnSettingsChanged(settings);
            
            return true;
        }
        catch (Exception ex)
        {
            // Log the error in a real application
            Console.WriteLine($"Error saving settings: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ResetSettingsAsync()
    {
        var defaultSettings = GetDefaultSettings();
        return await SaveSettingsAsync(defaultSettings);
    }

    /// <inheritdoc/>
    public async Task<bool> LoadSettingsFromFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var json = await File.ReadAllTextAsync(filePath);
            var settings = JsonSerializer.Deserialize<DesktopSettings>(json);
            
            if (settings != null)
            {
                return await SaveSettingsAsync(settings);
            }
            
            return false;
        }
        catch (Exception ex)
        {
            // Log the error in a real application
            Console.WriteLine($"Error loading settings from file: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> SaveSettingsToFileAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            var settings = await GetSettingsAsync();
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
            return true;
        }
        catch (Exception ex)
        {
            // Log the error in a real application
            Console.WriteLine($"Error saving settings to file: {ex.Message}");
            return false;
        }
    }

    /// <inheritdoc/>
    public async Task<bool> ImportLegacySettingsAsync(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));

        try
        {
            // TODO: Implement actual legacy VB.NET settings import logic
            // This would involve:
            // 1. Reading the legacy settings file
            // 2. Parsing the VB.NET settings format
            // 3. Converting to the new DesktopSettings format
            // 4. Saving the converted settings

            await Task.CompletedTask;
            return false;
        }
        catch (Exception ex)
        {
            // Log the error in a real application
            Console.WriteLine($"Error importing legacy settings: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Gets the default settings
    /// </summary>
    /// <returns>The default settings</returns>
    private static DesktopSettings GetDefaultSettings()
    {
        return new DesktopSettings
        {
            Theme = ThemeMode.System,
            AutoArrangeIcons = false,
            ShowDesktopIcons = true,
            SnapToGrid = true,
            GridSize = 16,
            IconSpacing = 8,
            FenceOpacity = 0.8,
            FenceBorderColor = "#808080",
            FenceBackgroundColor = "#F0F0F0",
            QuickHideEnabled = true,
            QuickHideHotkey = "Ctrl+Space",
            AutoOrganizationEnabled = false,
            AnimationEnabled = true,
            AnimationSpeed = 300
        };
    }

    /// <summary>
    /// Raises the SettingsChanged event
    /// </summary>
    /// <param name="settings">The settings that changed</param>
    private void OnSettingsChanged(DesktopSettings settings)
    {
        SettingsChanged?.Invoke(this, settings);
    }
}