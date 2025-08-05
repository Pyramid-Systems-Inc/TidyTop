using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using Avalonia.Threading;
using ReactiveUI;
using TidyTop.Core.Models;
using TidyTop.Core.Services;

namespace TidyTop.App.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IDesktopIconService _desktopIconService;
    private readonly IFenceService _fenceService;
    private readonly IDesktopLayoutService _desktopLayoutService;
    private readonly ISettingsService _settingsService;
    
    private bool _isVisible;
    private double _opacity;
    private ObservableCollection<DesktopIcon> _desktopIcons = new();
    private ObservableCollection<Fence> _fences = new();
    private DesktopLayout? _currentLayout;
    private DesktopSettings? _settings;
    private bool _showControlPanel;

    public MainWindowViewModel(
        IDesktopIconService desktopIconService,
        IFenceService fenceService,
        IDesktopLayoutService desktopLayoutService,
        ISettingsService settingsService)
    {
        _desktopIconService = desktopIconService;
        _fenceService = fenceService;
        _desktopLayoutService = desktopLayoutService;
        _settingsService = settingsService;

        // Initialize commands
        ToggleVisibilityCommand = ReactiveCommand.Create(ToggleVisibility);
        RefreshDesktopCommand = ReactiveCommand.CreateFromTask(RefreshDesktopAsync);
        CreateFenceCommand = ReactiveCommand.Create(CreateNewFence);
        SaveLayoutCommand = ReactiveCommand.CreateFromTask(SaveCurrentLayoutAsync);
        ToggleControlPanelCommand = ReactiveCommand.Create(ToggleControlPanel);
        
        // Fence commands
        DeleteFenceCommand = ReactiveCommand.Create<Fence>(DeleteFence);
        EditFenceCommand = ReactiveCommand.Create<Fence>(EditFence);
        ChangeFenceColorCommand = ReactiveCommand.Create<Fence>(ChangeFenceColor);
        AddIconsToFenceCommand = ReactiveCommand.Create<Fence>(AddIconsToFence);
        AutoArrangeFenceCommand = ReactiveCommand.Create<Fence>(AutoArrangeFence);

        // Initialize collections
        DesktopIcons = new ObservableCollection<DesktopIcon>();
        Fences = new ObservableCollection<Fence>();

        // Set initial state
        IsVisible = true;
        Opacity = 0.7; // Semi-transparent by default
        ShowControlPanel = false; // Control panel hidden by default

        // Load settings and layout
        InitializeAsync();
        
        CreateNewFence(); // Create a default fence on startup
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public double Opacity
    {
        get => _opacity;
        set => this.RaiseAndSetIfChanged(ref _opacity, value);
    }

    public ObservableCollection<DesktopIcon> DesktopIcons
    {
        get => _desktopIcons;
        set => this.RaiseAndSetIfChanged(ref _desktopIcons, value);
    }

    public ObservableCollection<Fence> Fences
    {
        get => _fences;
        set => this.RaiseAndSetIfChanged(ref _fences, value);
    }

    public DesktopLayout? CurrentLayout
    {
        get => _currentLayout;
        set => this.RaiseAndSetIfChanged(ref _currentLayout, value);
    }

    public DesktopSettings? Settings
    {
        get => _settings;
        set => this.RaiseAndSetIfChanged(ref _settings, value);
    }

    public bool ShowControlPanel
    {
        get => _showControlPanel;
        set
        {
            this.RaiseAndSetIfChanged(ref _showControlPanel, value);
            // Make the window interactive when control panel is shown
            if (value)
            {
                // This will be handled by the view's UpdateClickThroughMode method
            }
        }
    }

    public ReactiveCommand<Unit, Unit> ToggleVisibilityCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshDesktopCommand { get; }
    public ReactiveCommand<Unit, Unit> CreateFenceCommand { get; }
    public ReactiveCommand<Unit, Unit> SaveLayoutCommand { get; }
    public ReactiveCommand<Unit, Unit> ToggleControlPanelCommand { get; }
    
    // Fence Commands
    public ReactiveCommand<Fence, Unit> DeleteFenceCommand { get; }
    public ReactiveCommand<Fence, Unit> EditFenceCommand { get; }
    public ReactiveCommand<Fence, Unit> ChangeFenceColorCommand { get; }
    public ReactiveCommand<Fence, Unit> AddIconsToFenceCommand { get; }
    public ReactiveCommand<Fence, Unit> AutoArrangeFenceCommand { get; }

    private async void InitializeAsync()
    {
        try
        {
            // Load settings
            Settings = await _settingsService.GetSettingsAsync() ?? new DesktopSettings();
            
            // Apply settings
            Opacity = Settings.DefaultFenceOpacity;
            
            // Load current layout
            CurrentLayout = await _desktopLayoutService.GetCurrentLayoutAsync();
            
            if (CurrentLayout != null)
            {
                // Load fences
                var fences = await _fenceService.GetFencesAsync();
                Fences.Clear();
                foreach (var fence in fences)
                {
                    Fences.Add(fence);
                }
                
                // Load desktop icons
                var icons = await _desktopIconService.GetDesktopIconsAsync();
                DesktopIcons.Clear();
                foreach (var icon in icons)
                {
                    DesktopIcons.Add(icon);
                }
            }
        }
        catch (Exception ex)
        {
            // Log error or handle initialization failure
            Console.WriteLine($"Failed to initialize application: {ex.Message}");
        }
    }

    private void ToggleVisibility()
    {
        IsVisible = !IsVisible;
    }

    private void ToggleControlPanel()
    {
        ShowControlPanel = !ShowControlPanel;
    }

    private async Task RefreshDesktopAsync()
    {
        try
        {
            var icons = await _desktopIconService.GetDesktopIconsAsync();
            DesktopIcons.Clear();
            foreach (var icon in icons)
            {
                DesktopIcons.Add(icon);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to refresh desktop: {ex.Message}");
        }
    }

    private void CreateNewFence()
    {
        var newFence = new Fence
        {
            Title = $"New Fence {Fences.Count + 1}",
            Position = new System.Drawing.Point(100, 100),
            Size = new System.Drawing.Size(200, 150),
            IsVisible = true,
            CreatedDate = DateTime.Now,
            ModifiedDate = DateTime.Now
        };
        
        Fences.Add(newFence);
    }

    private async Task SaveCurrentLayoutAsync()
    {
        try
        {
            if (CurrentLayout == null)
            {
                CurrentLayout = new DesktopLayout
                {
                    Name = "Default Layout",
                    CreatedDate = DateTime.Now,
                    ModifiedDate = DateTime.Now
                };
            }
            
            // Update layout with current fences and icons
            CurrentLayout.Fences = Fences.ToList();
            CurrentLayout.UnfencedIcons = DesktopIcons.ToList();
            CurrentLayout.ModifiedDate = DateTime.Now;
            
            await _desktopLayoutService.UpdateLayoutAsync(CurrentLayout);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to save layout: {ex.Message}");
        }
    }

    private System.Drawing.Size GetScreenSize()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (desktop.MainWindow?.Screens.Primary is { } screen)
            {
                return new System.Drawing.Size(screen.WorkingArea.Width, screen.WorkingArea.Height);
            }
        }
        
        return new System.Drawing.Size(1920, 1080); // Default fallback
    }
    
    private void DeleteFence(Fence? fence)
    {
        if (fence != null)
        {
            Fences.Remove(fence);
        }
    }

    private void EditFence(Fence? fence)
    {
        // TODO: Implement a dialog or view for editing fence properties
        Console.WriteLine($"Editing fence: {fence?.Title}");
    }

    private void ChangeFenceColor(Fence? fence)
    {
        // TODO: Implement a color picker dialog
        Console.WriteLine($"Changing color for fence: {fence?.Title}");
    }

    private void AddIconsToFence(Fence? fence)
    {
        // TODO: Implement a file picker to add icons to the fence
        Console.WriteLine($"Adding icons to fence: {fence?.Title}");
    }

    private void AutoArrangeFence(Fence? fence)
    {
        // TODO: Implement logic to automatically arrange icons within the fence
        Console.WriteLine($"Auto-arranging icons in fence: {fence?.Title}");
    }
}