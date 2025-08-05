using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Service for managing application settings
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets the current desktop settings
    /// </summary>
    /// <returns>The current desktop settings</returns>
    Task<DesktopSettings> GetSettingsAsync();

    /// <summary>
    /// Saves the desktop settings
    /// </summary>
    /// <param name="settings">The settings to save</param>
    /// <returns>True if the settings were saved successfully; otherwise, false</returns>
    Task<bool> SaveSettingsAsync(DesktopSettings settings);

    /// <summary>
    /// Resets settings to default values
    /// </summary>
    /// <returns>True if the settings were reset successfully; otherwise, false</returns>
    Task<bool> ResetSettingsAsync();

    /// <summary>
    /// Loads settings from a file
    /// </summary>
    /// <param name="filePath">The path to the settings file</param>
    /// <returns>True if the settings were loaded successfully; otherwise, false</returns>
    Task<bool> LoadSettingsFromFileAsync(string filePath);

    /// <summary>
    /// Saves settings to a file
    /// </summary>
    /// <param name="filePath">The path to save the settings file</param>
    /// <returns>True if the settings were saved successfully; otherwise, false</returns>
    Task<bool> SaveSettingsToFileAsync(string filePath);

    /// <summary>
    /// Imports settings from the legacy VB.NET format
    /// </summary>
    /// <param name="filePath">The path to the legacy settings file</param>
    /// <returns>True if the settings were imported successfully; otherwise, false</returns>
    Task<bool> ImportLegacySettingsAsync(string filePath);

    /// <summary>
    /// Event raised when settings are changed
    /// </summary>
    event EventHandler<DesktopSettings>? SettingsChanged;
}