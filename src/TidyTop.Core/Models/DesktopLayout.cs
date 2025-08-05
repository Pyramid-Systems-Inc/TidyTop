using System;
using System.Collections.Generic;

namespace TidyTop.Core.Models
{
    /// <summary>
    /// Represents a complete desktop layout with all fences and icon positions
    /// </summary>
    public class DesktopLayout
    {
        /// <summary>
        /// Gets or sets the unique identifier for the layout
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the layout
        /// </summary>
        public string Name { get; set; } = "Default Layout";

        /// <summary>
        /// Gets or sets the description of the layout
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of fences in this layout
        /// </summary>
        public List<Fence> Fences { get; set; } = new List<Fence>();

        /// <summary>
        /// Gets or sets the list of desktop icons not in any fence
        /// </summary>
        public List<DesktopIcon> UnfencedIcons { get; set; } = new List<DesktopIcon>();

        /// <summary>
        /// Gets or sets the desktop resolution when this layout was saved
        /// </summary>
        public Size DesktopResolution { get; set; }

        /// <summary>
        /// Gets or sets the date when this layout was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date when this layout was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether this is the default layout
        /// </summary>
        public bool IsDefault { get; set; } = false;

        /// <summary>
        /// Gets or sets the application version that created this layout
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// Gets or sets the global settings for the desktop
        /// </summary>
        public DesktopSettings Settings { get; set; } = new DesktopSettings();

        /// <summary>
        /// Creates a deep copy of this layout
        /// </summary>
        /// <returns>A new DesktopLayout object with the same values</returns>
        public DesktopLayout Clone()
        {
            return new DesktopLayout
            {
                Id = Guid.NewGuid().ToString(),
                Name = $"{Name} (Copy)",
                Description = Description,
                Fences = Fences.ConvertAll(f => CloneFence(f)),
                UnfencedIcons = UnfencedIcons.ConvertAll(i => CloneIcon(i)),
                DesktopResolution = DesktopResolution,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now,
                IsDefault = false,
                Version = Version,
                Settings = Settings.Clone()
            };
        }

        /// <summary>
        /// Creates a deep copy of a fence
        /// </summary>
        /// <param name="fence">The fence to clone</param>
        /// <returns>A new Fence object with the same values</returns>
        private Fence CloneFence(Fence fence)
        {
            return new Fence
            {
                Id = Guid.NewGuid().ToString(),
                Title = fence.Title,
                Position = fence.Position,
                Size = fence.Size,
                BackgroundColor = fence.BackgroundColor,
                BorderColor = fence.BorderColor,
                TitleColor = fence.TitleColor,
                Opacity = fence.Opacity,
                IsVisible = fence.IsVisible,
                IsLocked = fence.IsLocked,
                Icons = fence.Icons.ConvertAll(i => CloneIcon(i)),
                SortRule = fence.SortRule,
                LayoutType = fence.LayoutType,
                IconSpacing = fence.IconSpacing,
                CreatedDate = fence.CreatedDate,
                ModifiedDate = fence.ModifiedDate,
                ShowTitle = fence.ShowTitle,
                CornerRadius = fence.CornerRadius,
                BorderWidth = fence.BorderWidth
            };
        }

        /// <summary>
        /// Creates a deep copy of a desktop icon
        /// </summary>
        /// <param name="icon">The icon to clone</param>
        /// <returns>A new DesktopIcon object with the same values</returns>
        private DesktopIcon CloneIcon(DesktopIcon icon)
        {
            return new DesktopIcon
            {
                Name = icon.Name,
                Extension = icon.Extension,
                FullPath = icon.FullPath,
                Icon = icon.Icon,
                Position = icon.Position,
                IsShortcut = icon.IsShortcut,
                Size = icon.Size,
                FenceId = icon.FenceId,
                CreatedDate = icon.CreatedDate,
                ModifiedDate = icon.ModifiedDate,
                FileSize = icon.FileSize
            };
        }
    }
}