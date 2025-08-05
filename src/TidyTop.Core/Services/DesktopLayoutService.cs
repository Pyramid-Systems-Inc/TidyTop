using System.Collections.Concurrent;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Implementation of the desktop layout service
/// </summary>
public class DesktopLayoutService : IDesktopLayoutService
{
    private readonly ConcurrentDictionary<Guid, DesktopLayout> _layouts = new();
    private readonly IFenceService _fenceService;
    private readonly IDesktopIconService _desktopIconService;
    private Guid? _activeLayoutId;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopLayoutService"/> class
    /// </summary>
    /// <param name="fenceService">The fence service</param>
    /// <param name="desktopIconService">The desktop icon service</param>
    public DesktopLayoutService(IFenceService fenceService, IDesktopIconService desktopIconService)
    {
        _fenceService = fenceService ?? throw new ArgumentNullException(nameof(fenceService));
        _desktopIconService = desktopIconService ?? throw new ArgumentNullException(nameof(desktopIconService));
    }

    /// <inheritdoc/>
    public Task<IEnumerable<DesktopLayout>> GetLayoutsAsync()
    {
        return Task.FromResult(_layouts.Values.ToList().AsEnumerable());
    }

    /// <inheritdoc/>
    public Task<DesktopLayout?> GetLayoutAsync(Guid id)
    {
        _layouts.TryGetValue(id, out var layout);
        return Task.FromResult(layout);
    }

    /// <inheritdoc/>
    public async Task<DesktopLayout?> GetCurrentLayoutAsync()
    {
        if (_activeLayoutId.HasValue && _layouts.TryGetValue(_activeLayoutId.Value, out var layout))
        {
            return layout;
        }

        // If no active layout is set, create a default one
        var defaultLayout = new DesktopLayout
        {
            Id = Guid.NewGuid(),
            Name = "Default Layout",
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        await AddLayoutAsync(defaultLayout);
        await SetActiveLayoutAsync(defaultLayout.Id);
        return defaultLayout;
    }

    /// <inheritdoc/>
    public Task<Guid> AddLayoutAsync(DesktopLayout layout)
    {
        if (layout == null)
            throw new ArgumentNullException(nameof(layout));

        layout.Id = layout.Id == Guid.Empty ? Guid.NewGuid() : layout.Id;
        layout.CreatedDate = layout.CreatedDate == default ? DateTime.Now : layout.CreatedDate;
        layout.ModifiedDate = DateTime.Now;

        var result = _layouts.TryAdd(layout.Id, layout);
        return Task.FromResult(result ? layout.Id : Guid.Empty);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateLayoutAsync(DesktopLayout layout)
    {
        if (layout == null)
            throw new ArgumentNullException(nameof(layout));

        layout.ModifiedDate = DateTime.Now;
        var result = _layouts.TryUpdate(layout.Id, layout, _layouts[layout.Id]);
        return result;
    }

    /// <inheritdoc/>
    public Task<bool> RemoveLayoutAsync(Guid id)
    {
        var result = _layouts.TryRemove(id, out _);
        
        // If this was the active layout, clear it
        if (result && _activeLayoutId == id)
        {
            _activeLayoutId = null;
        }

        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task<bool> SetActiveLayoutAsync(Guid id)
    {
        if (!_layouts.ContainsKey(id))
            return false;

        _activeLayoutId = id;
        
        // TODO: Implement actual layout restoration logic
        // This would involve:
        // 1. Getting the layout from our dictionary
        // 2. Restoring all fences in the layout
        // 3. Positioning icons according to the layout

        await Task.CompletedTask;
        return true;
    }

    /// <inheritdoc/>
    public async Task<Guid> SaveCurrentLayoutAsync(string name)
    {
        var layout = new DesktopLayout
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };

        // TODO: Implement actual layout saving logic
        // This would involve:
        // 1. Getting all current fences and their positions
        // 2. Getting all icons and their positions
        // 3. Storing this information in the layout object

        await AddLayoutAsync(layout);
        return layout.Id;
    }

    /// <inheritdoc/>
    public async Task<bool> RestoreLayoutAsync(Guid id)
    {
        var layout = await GetLayoutAsync(id);
        if (layout == null)
            return false;

        return await SetActiveLayoutAsync(id);
    }

    /// <inheritdoc/>
    public async Task<Guid> CopyLayoutAsync(Guid id, string newName)
    {
        var originalLayout = await GetLayoutAsync(id);
        if (originalLayout == null)
            return Guid.Empty;

        var copiedLayout = originalLayout.Clone();
        copiedLayout.Id = Guid.NewGuid();
        copiedLayout.Name = newName;
        copiedLayout.CreatedDate = DateTime.Now;
        copiedLayout.ModifiedDate = DateTime.Now;

        return await AddLayoutAsync(copiedLayout);
    }
}