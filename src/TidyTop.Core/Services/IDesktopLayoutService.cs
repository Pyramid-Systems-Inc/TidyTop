using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Service for managing desktop layouts
/// </summary>
public interface IDesktopLayoutService
{
    /// <summary>
    /// Gets all desktop layouts
    /// </summary>
    /// <returns>A collection of desktop layouts</returns>
    Task<IEnumerable<DesktopLayout>> GetLayoutsAsync();

    /// <summary>
    /// Gets a desktop layout by its ID
    /// </summary>
    /// <param name="id">The ID of the layout</param>
    /// <returns>The desktop layout if found; otherwise, null</returns>
    Task<DesktopLayout?> GetLayoutAsync(Guid id);

    /// <summary>
    /// Gets the current active desktop layout
    /// </summary>
    /// <returns>The current active desktop layout</returns>
    Task<DesktopLayout?> GetCurrentLayoutAsync();

    /// <summary>
    /// Adds a new desktop layout
    /// </summary>
    /// <param name="layout">The layout to add</param>
    /// <returns>The ID of the newly created layout</returns>
    Task<Guid> AddLayoutAsync(DesktopLayout layout);

    /// <summary>
    /// Updates an existing desktop layout
    /// </summary>
    /// <param name="layout">The layout to update</param>
    /// <returns>True if the layout was updated successfully; otherwise, false</returns>
    Task<bool> UpdateLayoutAsync(DesktopLayout layout);

    /// <summary>
    /// Removes a desktop layout
    /// </summary>
    /// <param name="id">The ID of the layout to remove</param>
    /// <returns>True if the layout was removed successfully; otherwise, false</returns>
    Task<bool> RemoveLayoutAsync(Guid id);

    /// <summary>
    /// Sets a layout as the active layout
    /// </summary>
    /// <param name="id">The ID of the layout to set as active</param>
    /// <returns>True if the layout was set as active successfully; otherwise, false</returns>
    Task<bool> SetActiveLayoutAsync(Guid id);

    /// <summary>
    /// Saves the current desktop state as a new layout
    /// </summary>
    /// <param name="name">The name for the new layout</param>
    /// <returns>The ID of the newly created layout</returns>
    Task<Guid> SaveCurrentLayoutAsync(string name);

    /// <summary>
    /// Restores a layout to the desktop
    /// </summary>
    /// <param name="id">The ID of the layout to restore</param>
    /// <returns>True if the layout was restored successfully; otherwise, false</returns>
    Task<bool> RestoreLayoutAsync(Guid id);

    /// <summary>
    /// Creates a copy of an existing layout
    /// </summary>
    /// <param name="id">The ID of the layout to copy</param>
    /// <param name="newName">The name for the copied layout</param>
    /// <returns>The ID of the newly created layout copy</returns>
    Task<Guid> CopyLayoutAsync(Guid id, string newName);
}