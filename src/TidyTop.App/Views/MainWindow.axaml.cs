using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TidyTop.App.Views
{
public partial class MainWindow : Window
{
        private List<ApplicationInfo> _desktopApps = new();
        private readonly Dictionary<string, BoxInfo> _boxes = new();

    public MainWindow()
    {
        InitializeComponent();
            InitializeBoxes();
            _ = LoadDesktopApplicationsAsync();
        }

        private void InitializeBoxes()
        {
            _boxes["office"] = new BoxInfo
            {
                Name = "Office Tools",
                Icon = "📊",
                Keywords = new[] { "word", "excel", "powerpoint", "office", "pdf", "acrobat" },
                Extensions = new[] { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf" }
            };

            _boxes["games"] = new BoxInfo
            {
                Name = "Games",
                Icon = "🎮",
                Keywords = new[] { "game", "steam", "epic", "minecraft" },
                Extensions = new[] { ".exe" }
            };

            _boxes["social"] = new BoxInfo
            {
                Name = "Social & Communication",
                Icon = "💬",
                Keywords = new[] { "discord", "telegram", "whatsapp", "chrome", "firefox" },
                Extensions = new[] { ".url" }
            };

            _boxes["files"] = new BoxInfo
            {
                Name = "Files & Documents",
                Icon = "📁",
                Keywords = new[] { "explorer", "notepad", "winrar" },
                Extensions = new[] { ".txt", ".zip", ".rar" }
            };
        }

        private async Task LoadDesktopApplicationsAsync()
        {
            try
            {
                LoadingOverlay.IsVisible = true;
                
                await Task.Run(() =>
                {
                    _desktopApps.Clear();
                    ScanFolder(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                });

                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    OrganizeApplications();
                    UpdateStatusText();
                    LoadingOverlay.IsVisible = false;
                });
            }
            catch (Exception ex)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    LoadingOverlay.IsVisible = false;
                    StatusText.Text = $"❌ Error: {ex.Message}";
                });
            }
        }

        private void ScanFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath)) return;

            try
            {
                var files = Directory.GetFiles(folderPath)
                    .Where(f => IsExecutableOrShortcut(f))
                    .Take(50);

                foreach (var file in files)
                {
                    try
                    {
                        var app = new ApplicationInfo
                        {
                            Name = Path.GetFileNameWithoutExtension(file),
                            FullPath = file,
                            Extension = Path.GetExtension(file).ToLowerInvariant()
                        };
                        _desktopApps.Add(app);
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static bool IsExecutableOrShortcut(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            return ext == ".exe" || ext == ".lnk" || ext == ".url";
        }

        private void OrganizeApplications()
        {
            foreach (var app in _desktopApps)
            {
                var category = CategorizeApplication(app);
                if (!string.IsNullOrEmpty(category) && _boxes.ContainsKey(category))
                {
                    _boxes[category].Applications.Add(app);
                }
            }
        }

        private string CategorizeApplication(ApplicationInfo app)
        {
            var appName = app.Name.ToLowerInvariant();

            foreach (var boxPair in _boxes)
            {
                var box = boxPair.Value;
                if (box.Keywords.Any(keyword => appName.Contains(keyword)))
                {
                    return boxPair.Key;
                }
                if (box.Extensions.Contains(app.Extension))
                {
                    return boxPair.Key;
                }
            }
            return string.Empty;
        }

        private void UpdateStatusText()
        {
            var totalApps = _desktopApps.Count;
            var organizedApps = _boxes.Values.Sum(b => b.Applications.Count);
            StatusText.Text = $"📦 {organizedApps}/{totalApps} applications organized";
        }

        private async void RefreshButton_Click(object? sender, RoutedEventArgs e)
        {
            foreach (var box in _boxes.Values)
                box.Applications.Clear();
            await LoadDesktopApplicationsAsync();
        }

        private void AddBoxButton_Click(object? sender, RoutedEventArgs e)
        {
            StatusText.Text = "💡 Custom box creation coming soon!";
        }

        private void SettingsButton_Click(object? sender, RoutedEventArgs e)
        {
            StatusText.Text = "⚙️ Settings window coming soon!";
        }
    }

    public class ApplicationInfo
    {
        public string Name { get; set; } = string.Empty;
        public string FullPath { get; set; } = string.Empty;
        public string Extension { get; set; } = string.Empty;
    }

    public class BoxInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string[] Keywords { get; set; } = Array.Empty<string>();
        public string[] Extensions { get; set; } = Array.Empty<string>();
        public List<ApplicationInfo> Applications { get; set; } = new();
    }
}
