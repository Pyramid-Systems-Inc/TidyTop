using System;
using System.Collections.Generic;
using System.Drawing;

namespace TidyTop.Core.Models
{
    /// <summary>
    /// Represents a fence container for organizing desktop icons
    /// </summary>
    public class Fence
    {
        /// <summary>
        /// Gets or sets the unique identifier for the fence
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the title of the fence
        /// </summary>
        public string Title { get; set; } = "New Fence";

        /// <summary>
        /// Gets or sets the position of the fence on the desktop
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the size of the fence
        /// </summary>
        public Size Size { get; set; } = new Size(200, 150);

        /// <summary>
        /// Gets or sets the background color of the fence
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.FromArgb(200, 240, 240, 240);

        /// <summary>
        /// Gets or sets the border color of the fence
        /// </summary>
        public Color BorderColor { get; set; } = Color.FromArgb(200, 180, 180, 180);

        /// <summary>
        /// Gets or sets the title color of the fence
        /// </summary>
        public Color TitleColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the opacity of the fence (0.0 to 1.0)
        /// </summary>
        public double Opacity { get; set; } = 0.8;

        /// <summary>
        /// Gets or sets a value indicating whether the fence is visible
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the fence is locked (cannot be moved/resized)
        /// </summary>
        public bool IsLocked { get; set; } = false;

        /// <summary>
        /// Gets or sets the icons contained within this fence
        /// </summary>
        public List<DesktopIcon> Icons { get; set; } = new List<DesktopIcon>();

        /// <summary>
        /// Gets or sets the sorting rule for icons within the fence
        /// </summary>
        public IconSortRule SortRule { get; set; } = IconSortRule.None;

        /// <summary>
        /// Gets or sets the layout type for icons within the fence
        /// </summary>
        public IconLayoutType LayoutType { get; set; } = IconLayoutType.Grid;

        /// <summary>
        /// Gets or sets the spacing between icons
        /// </summary>
        public int IconSpacing { get; set; } = 5;

        /// <summary>
        /// Gets or sets the date when the fence was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the date when the fence was last modified
        /// </summary>
        public DateTime ModifiedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether to show the fence title
        /// </summary>
        public bool ShowTitle { get; set; } = true;

        /// <summary>
        /// Gets or sets the border radius of the fence
        /// </summary>
        public int CornerRadius { get; set; } = 4;

        /// <summary>
        /// Gets or sets the border width of the fence
        /// </summary>
        public int BorderWidth { get; set; } = 1;
    }

    /// <summary>
    /// Defines the sorting rules for icons within a fence
    /// </summary>
    public enum IconSortRule
    {
        None,
        NameAscending,
        NameDescending,
        DateAscending,
        DateDescending,
        SizeAscending,
        SizeDescending,
        TypeAscending,
        TypeDescending
    }

    /// <summary>
    /// Defines the layout types for icons within a fence
    /// </summary>
    public enum IconLayoutType
    {
        Grid,
        Horizontal,
        Vertical,
        Freeform
    }
}