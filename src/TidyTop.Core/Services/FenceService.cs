using System.Collections.Concurrent;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Implementation of the fence service
/// </summary>
public class FenceService : IFenceService
{
    private readonly ConcurrentDictionary<Guid, Fence> _fences = new();
    private readonly ConcurrentDictionary<Guid, ConcurrentDictionary<string, DesktopIcon>> _fenceIcons = new();
    private readonly IDesktopIconService _desktopIconService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FenceService"/> class
    /// </summary>
    /// <param name="desktopIconService">The desktop icon service</param>
    public FenceService(IDesktopIconService desktopIconService)
    {
        _desktopIconService = desktopIconService ?? throw new ArgumentNullException(nameof(desktopIconService));
    }

    /// <inheritdoc/>
    public Task<IEnumerable<Fence>> GetFencesAsync()
    {
        return Task.FromResult(_fences.Values.ToList().AsEnumerable());
    }

    /// <inheritdoc/>
    public Task<Fence?> GetFenceAsync(Guid id)
    {
        _fences.TryGetValue(id, out var fence);
        return Task.FromResult(fence);
    }

    /// <inheritdoc/>
    public Task<Guid> AddFenceAsync(Fence fence)
    {
        if (fence == null)
            throw new ArgumentNullException(nameof(fence));

        Guid fenceId = string.IsNullOrEmpty(fence.Id) ? Guid.NewGuid() : Guid.Parse(fence.Id);
        fence.Id = fenceId.ToString();
        var result = _fences.TryAdd(fenceId, fence);
        
        if (result)
        {
            _fenceIcons.TryAdd(fenceId, new ConcurrentDictionary<string, DesktopIcon>());
        }

        return Task.FromResult(fenceId);
    }

    /// <inheritdoc/>
    public Task<bool> UpdateFenceAsync(Fence fence)
    {
        if (fence == null)
            throw new ArgumentNullException(nameof(fence));

        var fenceId = Guid.Parse(fence.Id);
        var result = _fences.TryUpdate(fenceId, fence, _fences[fenceId]);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<bool> RemoveFenceAsync(Guid id)
    {
        var fenceRemoved = _fences.TryRemove(id, out _);
        var iconsRemoved = _fenceIcons.TryRemove(id, out _);
        return Task.FromResult(fenceRemoved && iconsRemoved);
    }

    /// <inheritdoc/>
    public async Task<bool> AddIconToFenceAsync(Guid fenceId, string iconPath)
    {
        if (!_fences.ContainsKey(fenceId))
            return false;

        var icon = await _desktopIconService.GetDesktopIconAsync(iconPath);
        if (icon == null)
            return false;

        if (!_fenceIcons.ContainsKey(fenceId))
            _fenceIcons.TryAdd(fenceId, new ConcurrentDictionary<string, DesktopIcon>());

        return _fenceIcons[fenceId].TryAdd(iconPath, icon);
    }

    /// <inheritdoc/>
    public Task<bool> RemoveIconFromFenceAsync(Guid fenceId, string iconPath)
    {
        if (!_fenceIcons.ContainsKey(fenceId))
            return Task.FromResult(false);

        var result = _fenceIcons[fenceId].TryRemove(iconPath, out _);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<DesktopIcon>> GetIconsInFenceAsync(Guid fenceId)
    {
        if (!_fenceIcons.ContainsKey(fenceId))
            return Task.FromResult(Enumerable.Empty<DesktopIcon>());

        return Task.FromResult(_fenceIcons[fenceId].Values.ToList().AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task<bool> MoveFenceAsync(Guid fenceId, int x, int y)
    {
        var fence = await GetFenceAsync(fenceId);
        if (fence == null)
            return false;

        fence.Position = new System.Drawing.Point(x, y);
        return await UpdateFenceAsync(fence);
    }

    /// <inheritdoc/>
    public async Task<bool> ResizeFenceAsync(Guid fenceId, int width, int height)
    {
        var fence = await GetFenceAsync(fenceId);
        if (fence == null)
            return false;

        fence.Size = new System.Drawing.Size(width, height);
        return await UpdateFenceAsync(fence);
    }
}