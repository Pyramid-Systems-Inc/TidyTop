using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Service for managing desktop icons
/// </summary>
public interface IDesktopIconService
{
    /// <summary>
    /// Gets all desktop icons
    /// </summary>
    /// <returns>A collection of desktop icons</returns>
    Task<IEnumerable<DesktopIcon>> GetDesktopIconsAsync();

    /// <summary>
    /// Gets a desktop icon by its path
    /// </summary>
    /// <param name="path">The path to the icon</param>
    /// <returns>The desktop icon if found; otherwise, null</returns>
    Task<DesktopIcon?> GetDesktopIconAsync(string path);

    /// <summary>
    /// Adds a new desktop icon
    /// </summary>
    /// <param name="icon">The icon to add</param>
    /// <returns>True if the icon was added successfully; otherwise, false</returns>
    Task<bool> AddDesktopIconAsync(DesktopIcon icon);

    /// <summary>
    /// Updates an existing desktop icon
    /// </summary>
    /// <param name="icon">The icon to update</param>
    /// <returns>True if the icon was updated successfully; otherwise, false</returns>
    Task<bool> UpdateDesktopIconAsync(DesktopIcon icon);

    /// <summary>
    /// Removes a desktop icon
    /// </summary>
    /// <param name="path">The path to the icon to remove</param>
    /// <returns>True if the icon was removed successfully; otherwise, false</returns>
    Task<bool> RemoveDesktopIconAsync(string path);

    /// <summary>
    /// Refreshes the desktop icons cache
    /// </summary>
    /// <returns>A task representing the refresh operation</returns>
    Task RefreshDesktopIconsAsync();

    /// <summary>
    /// Gets the icon for a file or shortcut
    /// </summary>
    /// <param name="path">The path to the file or shortcut</param>
    /// <returns>The icon as a byte array</returns>
    Task<byte[]?> GetIconAsync(string path);
}