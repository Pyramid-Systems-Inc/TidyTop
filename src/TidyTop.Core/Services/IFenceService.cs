using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Service for managing fence containers
/// </summary>
public interface IFenceService
{
    /// <summary>
    /// Gets all fences
    /// </summary>
    /// <returns>A collection of fences</returns>
    Task<IEnumerable<Fence>> GetFencesAsync();

    /// <summary>
    /// Gets a fence by its ID
    /// </summary>
    /// <param name="id">The ID of the fence</param>
    /// <returns>The fence if found; otherwise, null</returns>
    Task<Fence?> GetFenceAsync(Guid id);

    /// <summary>
    /// Adds a new fence
    /// </summary>
    /// <param name="fence">The fence to add</param>
    /// <returns>The ID of the newly created fence</returns>
    Task<Guid> AddFenceAsync(Fence fence);

    /// <summary>
    /// Updates an existing fence
    /// </summary>
    /// <param name="fence">The fence to update</param>
    /// <returns>True if the fence was updated successfully; otherwise, false</returns>
    Task<bool> UpdateFenceAsync(Fence fence);

    /// <summary>
    /// Removes a fence
    /// </summary>
    /// <param name="id">The ID of the fence to remove</param>
    /// <returns>True if the fence was removed successfully; otherwise, false</returns>
    Task<bool> RemoveFenceAsync(Guid id);

    /// <summary>
    /// Adds an icon to a fence
    /// </summary>
    /// <param name="fenceId">The ID of the fence</param>
    /// <param name="iconPath">The path to the icon</param>
    /// <returns>True if the icon was added successfully; otherwise, false</returns>
    Task<bool> AddIconToFenceAsync(Guid fenceId, string iconPath);

    /// <summary>
    /// Removes an icon from a fence
    /// </summary>
    /// <param name="fenceId">The ID of the fence</param>
    /// <param name="iconPath">The path to the icon</param>
    /// <returns>True if the icon was removed successfully; otherwise, false</returns>
    Task<bool> RemoveIconFromFenceAsync(Guid fenceId, string iconPath);

    /// <summary>
    /// Gets all icons in a fence
    /// </summary>
    /// <param name="fenceId">The ID of the fence</param>
    /// <returns>A collection of desktop icons in the fence</returns>
    Task<IEnumerable<DesktopIcon>> GetIconsInFenceAsync(Guid fenceId);

    /// <summary>
    /// Moves a fence to a new position
    /// </summary>
    /// <param name="fenceId">The ID of the fence</param>
    /// <param name="x">The new X coordinate</param>
    /// <param name="y">The new Y coordinate</param>
    /// <returns>True if the fence was moved successfully; otherwise, false</returns>
    Task<bool> MoveFenceAsync(Guid fenceId, int x, int y);

    /// <summary>
    /// Resizes a fence
    /// </summary>
    /// <param name="fenceId">The ID of the fence</param>
    /// <param name="width">The new width</param>
    /// <param name="height">The new height</param>
    /// <returns>True if the fence was resized successfully; otherwise, false</returns>
    Task<bool> ResizeFenceAsync(Guid fenceId, int width, int height);
}