using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using TidyTop.Core.Models;

namespace TidyTop.Core.Services;

/// <summary>
/// Implementation of the desktop icon service
/// </summary>
public class DesktopIconService : IDesktopIconService
{
    private readonly ConcurrentDictionary<string, DesktopIcon> _icons = new();
    private readonly ISettingsService _settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesktopIconService"/> class
    /// </summary>
    /// <param name="settingsService">The settings service</param>
    public DesktopIconService(ISettingsService settingsService)
    {
        _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<DesktopIcon>> GetDesktopIconsAsync()
    {
        await RefreshDesktopIconsAsync();
        return _icons.Values.ToList();
    }

    /// <inheritdoc/>
    public async Task<DesktopIcon?> GetDesktopIconAsync(string path)
    {
        await RefreshDesktopIconsAsync();
        _icons.TryGetValue(path, out var icon);
        return icon;
    }

    /// <inheritdoc/>
    public Task<bool> AddDesktopIconAsync(DesktopIcon icon)
    {
        if (icon == null)
            throw new ArgumentNullException(nameof(icon));

        var result = _icons.TryAdd(icon.FullPath, icon);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<bool> UpdateDesktopIconAsync(DesktopIcon icon)
    {
        if (icon == null)
            throw new ArgumentNullException(nameof(icon));

        var result = _icons.TryUpdate(icon.FullPath, icon, _icons[icon.FullPath]);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public Task<bool> RemoveDesktopIconAsync(string path)
    {
        var result = _icons.TryRemove(path, out _);
        return Task.FromResult(result);
    }

    /// <inheritdoc/>
    public async Task RefreshDesktopIconsAsync()
    {
        _icons.Clear();

        await Task.Run(() =>
        {
            try
            {
                // Get desktop folder path
                var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var commonDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
                
                // Scan both user and common desktop folders
                ScanDesktopFolder(desktopPath);
                if (Directory.Exists(commonDesktopPath) && commonDesktopPath != desktopPath)
                {
                    ScanDesktopFolder(commonDesktopPath);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw to prevent application crash
                Console.WriteLine($"Error refreshing desktop icons: {ex.Message}");
            }
        });
    }

    private void ScanDesktopFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
            return;

        try
        {
            var files = Directory.GetFiles(folderPath);
            var directories = Directory.GetDirectories(folderPath);

            // Process files
            foreach (var file in files)
            {
                try
                {
                    var icon = CreateDesktopIcon(file, false);
                    if (icon != null)
                    {
                        _icons.TryAdd(icon.FullPath, icon);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing file {file}: {ex.Message}");
                }
            }

            // Process directories
            foreach (var directory in directories)
            {
                try
                {
                    var icon = CreateDesktopIcon(directory, true);
                    if (icon != null)
                    {
                        _icons.TryAdd(icon.FullPath, icon);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing directory {directory}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error scanning desktop folder {folderPath}: {ex.Message}");
        }
    }

    private DesktopIcon? CreateDesktopIcon(string path, bool isDirectory)
    {
        try
        {
            var fileInfo = isDirectory ? null : new FileInfo(path);
            var dirInfo = isDirectory ? new DirectoryInfo(path) : null;
            
            var name = isDirectory ? dirInfo!.Name : Path.GetFileNameWithoutExtension(path);
            var extension = isDirectory ? "" : Path.GetExtension(path);
            
            var icon = new DesktopIcon
            {
                Name = name,
                Extension = extension,
                FullPath = path,
                IsDirectory = isDirectory,
                IsShortcut = !isDirectory && (extension.Equals(".lnk", StringComparison.OrdinalIgnoreCase) ||
                                              extension.Equals(".url", StringComparison.OrdinalIgnoreCase)),
                FileSize = isDirectory ? 0 : fileInfo?.Length ?? 0,
                CreatedDate = isDirectory ? dirInfo!.CreationTime : fileInfo!.CreationTime,
                ModifiedDate = isDirectory ? dirInfo!.LastWriteTime : fileInfo!.LastWriteTime,
                Position = new System.Drawing.Point(0, 0), // Will be set by Windows desktop positioning
                IsVisible = true
            };

            // Extract icon asynchronously
            _ = Task.Run(async () =>
            {
                try
                {
                    icon.Icon = await GetIconAsync(path);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error extracting icon for {path}: {ex.Message}");
                }
            });

            return icon;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating desktop icon for {path}: {ex.Message}");
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task<byte[]?> GetIconAsync(string path)
    {
        return await Task.Run(() =>
        {
            try
            {
                if (string.IsNullOrEmpty(path) || !File.Exists(path) && !Directory.Exists(path))
                    return null;

                // Icon extraction is only supported on Windows
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    return null;
                }

                Icon? icon = null;
                
                try
                {
                    if (Directory.Exists(path))
                    {
                        // Extract folder icon
                        icon = ExtractFolderIcon();
                    }
                    else
                    {
                        // Extract file icon
                        icon = Icon.ExtractAssociatedIcon(path);
                    }

                    if (icon != null)
                    {
                        using (icon)
                        using (var bitmap = icon.ToBitmap())
                        using (var stream = new MemoryStream())
                        {
                            bitmap.Save(stream, ImageFormat.Png);
                            return stream.ToArray();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error extracting icon from {path}: {ex.Message}");
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetIconAsync for {path}: {ex.Message}");
                return null;
            }
        });
    }

    private static Icon? ExtractFolderIcon()
    {
        try
        {
            // Use Windows API to get standard folder icon
            var shfi = new SHFILEINFO();
            var result = SHGetFileInfo("folder", FILE_ATTRIBUTE_DIRECTORY, ref shfi, 
                (uint)Marshal.SizeOf(shfi), SHGFI_ICON | SHGFI_USEFILEATTRIBUTES);
            
            if (result != IntPtr.Zero && shfi.hIcon != IntPtr.Zero)
            {
                var icon = Icon.FromHandle(shfi.hIcon);
                return (Icon)icon.Clone(); // Clone to avoid handle issues
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting folder icon: {ex.Message}");
        }
        
        return null;
    }

    // Windows API structures and constants for icon extraction
    [StructLayout(LayoutKind.Sequential)]
    private struct SHFILEINFO
    {
        public IntPtr hIcon;
        public IntPtr iIcon;
        public uint dwAttributes;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string szDisplayName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
        public string szTypeName;
    }

    [DllImport("shell32.dll")]
    private static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, 
        ref SHFILEINFO psfi, uint cbSizeFileInfo, uint uFlags);

    private const uint SHGFI_ICON = 0x100;
    private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
    private const uint FILE_ATTRIBUTE_DIRECTORY = 0x10;
}