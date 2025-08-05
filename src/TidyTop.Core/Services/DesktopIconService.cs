using System.Collections.Concurrent;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Implementation of the desktop icon service
/// </summary>
public class DesktopIconService : IDesktopIconService
{
    private readonly ConcurrentDictionary<string, DesktopIcon> _icons = new();
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopIconService"/> class
    /// </summary>
    /// <param name="settingsService">The settings service</param>
    public DesktopIconService(ISettingsService settingsService)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DesktopIcon>> GetDesktopIconsAsync()
    {
        await RefreshDesktopIconsAsync();
        return _icons.Values.ToList();
    }

    /// <inheritdoc/>
    public async Task<DesktopIcon?> GetDesktopIconAsync(string path)
    {
        await RefreshDesktopIconsAsync();
        _icons.TryGetValue(path, out var icon);
        return icon;
    }

    /// <inheritdoc/>
    public Task<bool> AddDesktopIconAsync(DesktopIcon icon)
    {
        if (icon == null)
            throw new ArgumentNullException(nameof(icon));

        var result = _icons.TryAdd(icon.FullPath, icon);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<bool> UpdateDesktopIconAsync(DesktopIcon icon)
    {
        if (icon == null)
            throw new ArgumentNullException(nameof(icon));

        var result = _icons.TryUpdate(icon.FullPath, icon, _icons[icon.FullPath]);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<bool> RemoveDesktopIconAsync(string path)
    {
        var result = _icons.TryRemove(path, out _);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task RefreshDesktopIconsAsync()
    {
        // In a real implementation, this would scan the desktop for icons
        // For now, we'll just clear and reload from our cache
        _icons.Clear();

        // TODO: Implement actual desktop scanning logic
        // This would involve:
        // 1. Getting the desktop path
        // 2. Enumerating files and shortcuts
        // 3. Creating DesktopIcon objects for each
        // 4. Adding them to our dictionary

        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task<byte[]?> GetIconAsync(string path)
    {
        // In a real implementation, this would extract the icon from the file
        // For now, we'll return null
        // TODO: Implement actual icon extraction logic
        
        await Task.CompletedTask;
        return null;
    }
}