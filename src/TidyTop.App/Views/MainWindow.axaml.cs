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
    }

    private void InitializeDesktopOverlay()
    {
        // Set window to cover entire desktop
        if (Screens.Primary is { } screen)
        {
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Position = new PixelPoint(screen.WorkingArea.X, screen.WorkingArea.Y);
        }
        
        // Make window transparent and click-through
        Background = Avalonia.Media.Brushes.Transparent;
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
        
        // Keep window on top
        Topmost = true;
        
        // Hide from taskbar
        ShowInTaskbar = false;
        
        // Disable resizing
        CanResize = false;
        
        // Remove window chrome
        ExtendClientAreaToDecorationsHint = true;
        ExtendClientAreaChromeHints = Avalonia.Platform.ExtendClientAreaChromeHints.NoChrome;
        ExtendClientAreaTitleBarHeightHint = -1;
    }

    private void OnWindowPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        
        // Right-click to show/hide control panel
        if (point.Properties.IsRightButtonPressed)
        {
            ViewModel?.ToggleControlPanelCommand.Execute(Unit.Default);
        }
    }

    private void OnWindowPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging) return;
        
        var point = e.GetCurrentPoint(this);
        var currentPosition = point.Position;
        
        if (_draggedFence != null)
        {
            // Update fence position
            var deltaX = currentPosition.X - _dragStartPoint.X;
            var deltaY = currentPosition.Y - _dragStartPoint.Y;
            
            _draggedFence.Position = new System.Drawing.Point(
                _draggedFence.Position.X + (int)deltaX,
                _draggedFence.Position.Y + (int)deltaY);
            
            _dragStartPoint = currentPosition;
        }
        else if (_draggedIcon != null)
        {
            // Update icon position
            var deltaX = currentPosition.X - _dragStartPoint.X;
            var deltaY = currentPosition.Y - _dragStartPoint.Y;
            
            _draggedIcon.Position = new System.Drawing.Point(
                _draggedIcon.Position.X + (int)deltaX,
                _draggedIcon.Position.Y + (int)deltaY);
            
            _dragStartPoint = currentPosition;
        }
    }

    private void OnWindowPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _draggedFence = null;
        _draggedIcon = null;
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Global keyboard shortcuts
        switch (e.Key)
        {
            case Key.F12:
                // Toggle visibility
                ViewModel?.ToggleVisibilityCommand.Execute(Unit.Default);
                break;
                
            case Key.F11:
                // Refresh desktop
                ViewModel?.RefreshDesktopCommand.Execute(Unit.Default);
                break;
                
            case Key.F10:
                // Create new fence
                ViewModel?.CreateFenceCommand.Execute(Unit.Default);
                break;
                
            case Key.F9:
                // Save layout
                ViewModel?.SaveLayoutCommand.Execute(Unit.Default);
                break;
                
            case Key.Escape:
                // Hide control panel
                if (ViewModel != null && ViewModel.ShowControlPanel)
                {
                    ViewModel.ToggleControlPanelCommand.Execute(Unit.Default);
                }
                break;
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        
        // Ensure window is properly positioned and sized
        if (Screens.Primary is { } screen)
        {
            Width = screen.WorkingArea.Width;
            Height = screen.WorkingArea.Height;
            Position = new PixelPoint(screen.WorkingArea.X, screen.WorkingArea.Y);
        }
    }
}