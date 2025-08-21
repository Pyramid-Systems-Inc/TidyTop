using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform;
using Avalonia.Threading;
using System;
using System.Linq;
using System.Reactive;
using TidyTop.App.ViewModels;
using TidyTop.Core.Models;

namespace TidyTop.App.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;
    private bool _isDragging;
    private Point _dragStartPoint;
    private Fence? _draggedFence;
    private DesktopIcon? _draggedIcon;

    public MainWindow()
    {
        InitializeComponent();
        
        // Set up window properties for desktop overlay
        InitializeDesktopOverlay();
        
        // Add event handlers
        this.PointerPressed += OnWindowPointerPressed;
        this.PointerMoved += OnWindowPointerMoved;
        this.PointerReleased += OnWindowPointerReleased;
        this.KeyDown += OnKeyDown;
        
        // Set up data context changed handler
        this.DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (DataContext is MainWindowViewModel viewModel)
        {
            // Listen for property changes
            viewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(MainWindowViewModel.ShowControlPanel))
                {
                    UpdateClickThroughMode();
                }
            };
        }
    }

    private void InitializeDesktopOverlay()
    {
        // Set window to cover entire screen (not just working area)
        if (Screens.Primary is { } screen)
        {
            Width = screen.Bounds.Width;
            Height = screen.Bounds.Height;
            Position = new PixelPoint(screen.Bounds.X, screen.Bounds.Y);
        }
        
        // Make window transparent
        Background = Avalonia.Media.Brushes.Transparent;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
        
        // Keep window on top but allow other windows to be focused
        Topmost = true;
        
        // Hide from taskbar
        ShowInTaskbar = false;
        
        // Disable resizing and moving
        CanResize = false;
        
        // Remove window chrome completely
        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
        ExtendClientAreaTitleBarHeightHint = -1;
        
        // Set window state
        WindowState = WindowState.Normal;
        
        // Set initial click-through mode
        UpdateClickThroughMode();
    }
    
    private void UpdateClickThroughMode()
    {
        // Make the window click-through when not interacting with fences
        // This allows users to interact with their desktop normally
        if (ViewModel != null && !ViewModel.ShowControlPanel && !_isDragging)
        {
            // Make window slightly transparent when not interacting
            this.Opacity = Math.Max(0.3, ViewModel.Opacity * 0.5);
        }
        else
        {
            // Make window fully visible when control panel is shown or dragging
            this.Opacity = ViewModel?.Opacity ?? 0.7;
        }
    }

    private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        
        // Right-click to show/hide control panel
        if (point.Properties.IsRightButtonPressed)
        {
            ViewModel?.ToggleControlPanelCommand.Execute(Unit.Default);
            UpdateClickThroughMode();
            return;
        }
        
        // Check if we're clicking on a fence or icon
        // Note: GetVisualsAt is not available in Avalonia, so we'll use a different approach
        // We'll rely on the individual event handlers for fences and icons
        bool isClickingOnFenceOrIcon = false;
        
        // If not clicking on a fence or icon, make window click-through
        if (!isClickingOnFenceOrIcon && !ViewModel?.ShowControlPanel == true)
        {
            // Make window click-through by setting opacity to very low
            this.Opacity = 0.01;
        }
    }
    
    private void StartFenceDrag(Border fenceBorder, Point position)
    {
        if (fenceBorder.DataContext is Fence fence)
        {
            _isDragging = true;
            _draggedFence = fence;
            _dragStartPoint = position;
            
            // Make window interactive while dragging
            if (ViewModel != null)
            {
                this.Opacity = ViewModel.Opacity;
            }
        }
    }
    
    private void StartIconDrag(Border iconBorder, Point position)
    {
        if (iconBorder.DataContext is DesktopIcon icon)
        {
            _isDragging = true;
            _draggedIcon = icon;
            _dragStartPoint = position;
            
            // Make window interactive while dragging
            if (ViewModel != null)
            {
                this.Opacity = ViewModel.Opacity;
            }
            else
            {
                this.Opacity = 0.7;
            }
        }
    }

    private void OnWindowPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging) return;
        
        var point = e.GetCurrentPoint(this);
        var currentPosition = point.Position;
        
        if (_draggedFence != null)
        {
            // Update fence position directly to current mouse position
            _draggedFence.Position = new System.Drawing.Point(
                Math.Max(0, (int)(currentPosition.X - 100)), // Offset for better dragging feel
                Math.Max(0, (int)(currentPosition.Y - 20))); // Adjust for title bar
                
            // Trigger UI update through property change notification
            if (ViewModel != null)
            {
                // Force refresh of the fence collection to update UI
                var fences = ViewModel.Fences.ToList();
                ViewModel.Fences.Clear();
                foreach (var fence in fences)
                {
                    ViewModel.Fences.Add(fence);
                }
            }
        }
        else if (_draggedIcon != null)
        {
            // Update icon position
            _draggedIcon.Position = new System.Drawing.Point(
                Math.Max(0, (int)(currentPosition.X - 32)), // Center on cursor
                Math.Max(0, (int)(currentPosition.Y - 32)));
                
            // Trigger UI update through property change notification
            if (ViewModel != null)
            {
                // Force refresh of the desktop icons collection to update UI
                var icons = ViewModel.DesktopIcons.ToList();
                ViewModel.DesktopIcons.Clear();
                foreach (var icon in icons)
                {
                    ViewModel.DesktopIcons.Add(icon);
                }
            }
        }
    }

    private void OnWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _draggedFence = null;
        _draggedIcon = null;
        
        // Restore click-through mode after dragging
        UpdateClickThroughMode();
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Global keyboard shortcuts
        var modifiers = e.KeyModifiers;
        
        // Handle different key combinations
        if (modifiers.HasFlag(KeyModifiers.Control))
        {
            switch (e.Key)
            {
                case Key.H:
                    // Ctrl+H: Toggle visibility (like Hide/Show)
                    ViewModel?.ToggleVisibilityCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.R:
                    // Ctrl+R: Refresh desktop
                    ViewModel?.RefreshDesktopCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.N:
                    // Ctrl+N: Create new fence
                    ViewModel?.CreateFenceCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.S:
                    // Ctrl+S: Save layout
                    ViewModel?.SaveLayoutCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.Space:
                    // Ctrl+Space: Toggle control panel
                    ViewModel?.ToggleControlPanelCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
            }
        }
        else
        {
            switch (e.Key)
            {
                case Key.F12:
                    // F12: Toggle visibility
                    ViewModel?.ToggleVisibilityCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.F11:
                    // F11: Refresh desktop
                    ViewModel?.RefreshDesktopCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.F10:
                    // F10: Create new fence
                    ViewModel?.CreateFenceCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.F9:
                    // F9: Save layout
                    ViewModel?.SaveLayoutCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
                    
                case Key.Escape:
                    // Escape: Hide control panel or exit dragging mode
                    if (_isDragging)
                    {
                        // Cancel dragging
                        _isDragging = false;
                        _draggedFence = null;
                        _draggedIcon = null;
                        UpdateClickThroughMode();
                        e.Handled = true;
                    }
                    else if (ViewModel != null && ViewModel.ShowControlPanel)
                    {
                        ViewModel.ToggleControlPanelCommand.Execute(Unit.Default);
                        e.Handled = true;
                    }
                    break;
                    
                case Key.Tab:
                    // Tab: Toggle control panel
                    ViewModel?.ToggleControlPanelCommand.Execute(Unit.Default);
                    e.Handled = true;
                    break;
            }
        }
    }
    
    private void Fence_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is Fence fence)
        {
            var point = e.GetCurrentPoint(this);
            
            // Make window interactive for fence interaction
            if (ViewModel != null)
            {
                this.Opacity = ViewModel.Opacity;
            }
            
            // Left click to start dragging
            if (point.Properties.IsLeftButtonPressed)
            {
                StartFenceDrag(border, point.Position);
            }
            // Right click to show context menu
            else if (point.Properties.IsRightButtonPressed)
            {
                // Context menu will be shown automatically by the Flyout
                e.Handled = true;
            }
        }
    }
    
    private void Icon_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.DataContext is DesktopIcon icon)
        {
            var point = e.GetCurrentPoint(this);
            
            // Make window interactive for icon interaction
            if (ViewModel != null)
            {
                this.Opacity = ViewModel.Opacity;
            }
            
            // Left click to start dragging
            if (point.Properties.IsLeftButtonPressed)
            {
                StartIconDrag(border, point.Position);
            }
            // Right click to show context menu
            else if (point.Properties.IsRightButtonPressed)
            {
                // TODO: Implement icon context menu
                e.Handled = true;
            }
            // Double click to launch the icon
            else if (e.ClickCount == 2)
            {
                LaunchIcon(icon);
                e.Handled = true;
            }
        }
    }
    
    private void LaunchIcon(DesktopIcon icon)
    {
        try
        {
            if (!string.IsNullOrEmpty(icon.FullPath))
            {
                var process = new System.Diagnostics.Process();
                process.StartInfo.FileName = icon.FullPath;
                process.StartInfo.UseShellExecute = true;
                process.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to launch icon: {ex.Message}");
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        
        // Ensure window covers the entire screen
        if (Screens.Primary is { } screen)
        {
            Width = screen.Bounds.Width;
            Height = screen.Bounds.Height;
            Position = new PixelPoint(screen.Bounds.X, screen.Bounds.Y);
            
            // Force window to stay on top
            Topmost = false;
            Topmost = true;
        }
        
        // Start loading desktop icons
        Dispatcher.UIThread.InvokeAsync(() =>
        {
            if (ViewModel != null)
            {
                ViewModel.RefreshDesktopCommand.Execute().Subscribe();
            }
        });
    }
}