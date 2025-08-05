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
    private MainWindowViewModel ViewModel => DataContext as MainWindowViewModel;
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

    private void OnWindowPointerPressed(object sender, PointerPressedEventArgs e)
    {
        var point = e.GetCurrentPoint(this);
        
        // Right-click to show/hide control panel
        if (point.Properties.IsRightButtonPressed)
        {
            if (ViewModel != null)
            {
                ViewModel.ToggleControlPanelCommand.Execute(Unit.Default);
            }
        }
    }

    private void OnWindowPointerMoved(object sender, PointerEventArgs e)
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

    private void OnWindowPointerReleased(object sender, PointerReleasedEventArgs e)
    {
        _isDragging = false;
        _draggedFence = null;
        _draggedIcon = null;
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
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

    private void OnFenceHeaderMouseDown(object sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Tag is Fence fence)
        {
            var point = e.GetCurrentPoint(this);
            
            if (point.Properties.IsLeftButtonPressed)
            {
                _isDragging = true;
                _dragStartPoint = point.Position;
                _draggedFence = fence;
                
                // Bring fence to front
                if (ViewModel?.Fences.Contains(fence) == true)
                {
                    ViewModel.Fences.Remove(fence);
                    ViewModel.Fences.Add(fence);
                }
            }
            else if (point.Properties.IsRightButtonPressed)
            {
                // Show context menu for fence
                ShowFenceContextMenu(fence, point.Position);
            }
        }
    }

    private void OnIconPointerPressed(object sender, PointerPressedEventArgs e)
    {
        if (sender is Border border && border.Tag is DesktopIcon icon)
        {
            var point = e.GetCurrentPoint(this);
            
            if (point.Properties.IsLeftButtonPressed)
            {
                if (e.ClickCount == 2)
                {
                    // Double-click to launch icon
                    LaunchIcon(icon);
                }
                else
                {
                    // Start dragging
                    _isDragging = true;
                    _dragStartPoint = point.Position;
                    _draggedIcon = icon;
                }
            }
            else if (point.Properties.IsRightButtonPressed)
            {
                // Show context menu for icon
                ShowIconContextMenu(icon, point.Position);
            }
        }
    }

    private void ShowFenceContextMenu(Fence fence, Point position)
    {
        var contextMenu = new ContextMenu();
        
        // Edit fence
        var editItem = new MenuItem { Header = "Edit Fence" };
        editItem.Click += (s, e) => EditFence(fence);
        contextMenu.Items.Add(editItem);
        
        // Delete fence
        var deleteItem = new MenuItem { Header = "Delete Fence" };
        deleteItem.Click += (s, e) => DeleteFence(fence);
        contextMenu.Items.Add(deleteItem);
        
        // Separator
        contextMenu.Items.Add(new Separator());
        
        // Bring to front
        var frontItem = new MenuItem { Header = "Bring to Front" };
        frontItem.Click += (s, e) => BringFenceToFront(fence);
        contextMenu.Items.Add(frontItem);
        
        // Send to back
        var backItem = new MenuItem { Header = "Send to Back" };
        backItem.Click += (s, e) => SendFenceToBack(fence);
        contextMenu.Items.Add(backItem);
        
        // Open context menu
        contextMenu.Open(this);
    }

    private void ShowIconContextMenu(DesktopIcon icon, Point position)
    {
        var contextMenu = new ContextMenu();
        
        // Open icon
        var openItem = new MenuItem { Header = "Open" };
        openItem.Click += (s, e) => LaunchIcon(icon);
        contextMenu.Items.Add(openItem);
        
        // Properties
        var propertiesItem = new MenuItem { Header = "Properties" };
        propertiesItem.Click += (s, e) => ShowIconProperties(icon);
        contextMenu.Items.Add(propertiesItem);
        
        // Separator
        contextMenu.Items.Add(new Separator());
        
        // Remove from fence
        var removeItem = new MenuItem { Header = "Remove from Fence" };
        removeItem.Click += (s, e) => RemoveIconFromFence(icon);
        contextMenu.Items.Add(removeItem);
        
        // Open context menu
        contextMenu.Open(this);
    }

    private void EditFence(Fence fence)
    {
        // Implementation for fence editing dialog
        // This would open a dialog to edit fence properties
    }

    private void DeleteFence(Fence fence)
    {
        if (ViewModel?.Fences.Contains(fence) == true)
        {
            ViewModel.Fences.Remove(fence);
        }
    }

    private void BringFenceToFront(Fence fence)
    {
        if (ViewModel?.Fences.Contains(fence) == true)
        {
            ViewModel.Fences.Remove(fence);
            ViewModel.Fences.Add(fence);
        }
    }

    private void SendFenceToBack(Fence fence)
    {
        if (ViewModel?.Fences.Contains(fence) == true)
        {
            ViewModel.Fences.Remove(fence);
            ViewModel.Fences.Insert(0, fence);
        }
    }

    private void LaunchIcon(DesktopIcon icon)
    {
        try
        {
            if (!string.IsNullOrEmpty(icon.FullPath))
            {
                var process = new System.Diagnostics.Process
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = icon.FullPath,
                        UseShellExecute = true
                    }
                };
                process.Start();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to launch icon: {ex.Message}");
        }
    }

    private void ShowIconProperties(DesktopIcon icon)
    {
        // Implementation for icon properties dialog
    }

    private void RemoveIconFromFence(DesktopIcon icon)
    {
        // Implementation for removing icon from fence
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