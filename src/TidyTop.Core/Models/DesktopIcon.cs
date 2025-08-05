using System;
using System.Drawing;

namespace TidyTop.Core.Models
{
    /// <summary>
    /// Represents a desktop icon with its properties and position
    /// </summary>
    public class DesktopIcon
    {
        /// <summary>
        /// Gets or sets the name of the icon
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file extension
        /// </summary>
        public string Extension { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the full path to the file
        /// </summary>
        public string FullPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon image data
        /// </summary>
        public byte[]? Icon { get; set; }

        /// <summary>
        /// Gets or sets the position of the icon on the desktop
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a shortcut
        /// </summary>
        public bool IsShortcut { get; set; }

        /// <summary>
        /// Gets or sets the size of the icon
        /// </summary>
        public Size Size { get; set; } = new Size(32, 32);

        /// <summary>
        /// Gets or sets the fence ID this icon belongs to
        /// </summary>
        public string? FenceId { get; set; }

        /// <summary>
        /// Gets or sets the date when the icon was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date when the icon was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the file size in bytes
        /// </summary>
        public long FileSize { get; set; }
    }
}