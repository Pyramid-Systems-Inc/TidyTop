using System;
using System.Collections.Generic;
using System.Drawing;

namespace TidyTop.Core.Models
{
    /// <summary>
    /// Represents global desktop settings for the application
    /// </summary>
    public class DesktopSettings
    {
        /// <summary>
        /// Gets or sets the default fence background color
        /// </summary>
        public Color DefaultFenceBackgroundColor { get; set; } = Color.FromArgb(200, 240, 240, 240);

        /// <summary>
        /// Gets or sets the default fence border color
        /// </summary>
        public Color DefaultFenceBorderColor { get; set; } = Color.FromArgb(200, 180, 180, 180);

        /// <summary>
        /// Gets or sets the default fence title color
        /// </summary>
        public Color DefaultFenceTitleColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the default fence opacity
        /// </summary>
        public double DefaultFenceOpacity { get; set; } = 0.8;

        /// <summary>
        /// Gets or sets the default icon size
        /// </summary>
        public Size DefaultIconSize { get; set; } = new Size(32, 32);

        /// <summary>
        /// Gets or sets the default icon spacing
        /// </summary>
        public int DefaultIconSpacing { get; set; } = 5;

        /// <summary>
        /// Gets or sets the default fence corner radius
        /// </summary>
        public int DefaultFenceCornerRadius { get; set; } = 4;

        /// <summary>
        /// Gets or sets the default fence border width
        /// </summary>
        public int DefaultFenceBorderWidth { get; set; } = 1;

        /// <summary>
        /// Gets or sets a value indicating whether to show fence titles by default
        /// </summary>
        public bool ShowFenceTitles { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable quick hide functionality
        /// </summary>
        public bool EnableQuickHide { get; set; } = true;

        /// <summary>
        /// Gets or sets the quick hide hotkey
        /// </summary>
        public string QuickHideHotkey { get; set; } = "Ctrl+Space";

        /// <summary>
        /// Gets or sets a value indicating whether to enable auto-organization
        /// </summary>
        public bool EnableAutoOrganize { get; set; } = true;

        /// <summary>
        /// Gets or sets the auto-organization interval in minutes
        /// </summary>
        public int AutoOrganizeInterval { get; set; } = 30;

        /// <summary>
        /// Gets or sets a value indicating whether to start with Windows
        /// </summary>
        public bool StartWithWindows { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether to show notifications
        /// </summary>
        public bool ShowNotifications { get; set; } = true;

        /// <summary>
        /// Gets or sets the language for the application
        /// </summary>
        public string Language { get; set; } = "en-US";

        /// <summary>
        /// Gets or sets the theme for the application
        /// </summary>
        public ApplicationTheme Theme { get; set; } = ApplicationTheme.System;

        /// <summary>
        /// Gets or sets the list of auto-organization rules
        /// </summary>
        public List<AutoOrganizeRule> AutoOrganizeRules { get; set; } = new List<AutoOrganizeRule>();

        /// <summary>
        /// Gets or sets the date when settings were last modified
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets a value indicating whether to enable desktop portal functionality
        /// </summary>
        public bool EnableDesktopPortals { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to enable fence animations
        /// </summary>
        public bool EnableAnimations { get; set; } = true;

        /// <summary>
        /// Gets or sets the animation speed in milliseconds
        /// </summary>
        public int AnimationSpeed { get; set; } = 300;

        /// <summary>
        /// Gets or sets a value indicating whether to enable fence snapping to grid
        /// </summary>
        public bool EnableGridSnapping { get; set; } = true;

        /// <summary>
        /// Gets or sets the grid size for snapping
        /// </summary>
        public int GridSize { get; set; } = 10;

        /// <summary>
        /// Creates a deep copy of these settings
        /// </summary>
        /// <returns>A new DesktopSettings object with the same values</returns>
        public DesktopSettings Clone()
        {
            return new DesktopSettings
            {
                DefaultFenceBackgroundColor = DefaultFenceBackgroundColor,
                DefaultFenceBorderColor = DefaultFenceBorderColor,
                DefaultFenceTitleColor = DefaultFenceTitleColor,
                DefaultFenceOpacity = DefaultFenceOpacity,
                DefaultIconSize = DefaultIconSize,
                DefaultIconSpacing = DefaultIconSpacing,
                DefaultFenceCornerRadius = DefaultFenceCornerRadius,
                DefaultFenceBorderWidth = DefaultFenceBorderWidth,
                ShowFenceTitles = ShowFenceTitles,
                EnableQuickHide = EnableQuickHide,
                QuickHideHotkey = QuickHideHotkey,
                EnableAutoOrganize = EnableAutoOrganize,
                AutoOrganizeInterval = AutoOrganizeInterval,
                StartWithWindows = StartWithWindows,
                ShowNotifications = ShowNotifications,
                Language = Language,
                Theme = Theme,
                AutoOrganizeRules = AutoOrganizeRules.ConvertAll(r => r.Clone()),
                LastModified = DateTime.Now,
                EnableDesktopPortals = EnableDesktopPortals,
                EnableAnimations = EnableAnimations,
                AnimationSpeed = AnimationSpeed,
                EnableGridSnapping = EnableGridSnapping,
                GridSize = GridSize
            };
        }
    }

    /// <summary>
    /// Defines the application theme options
    /// </summary>
    public enum ApplicationTheme
    {
        System,
        Light,
        Dark
    }

    /// <summary>
    /// Represents an auto-organization rule for placing icons in specific fences
    /// </summary>
    public class AutoOrganizeRule
    {
        /// <summary>
        /// Gets or sets the unique identifier for the rule
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Gets or sets the name of the rule
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rule condition (e.g., file extension, name pattern)
        /// </summary>
        public string Condition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the rule type (e.g., Extension, Name, Date)
        /// </summary>
        public RuleType RuleType { get; set; } = RuleType.Extension;

        /// <summary>
        /// Gets or sets the target fence ID for this rule
        /// </summary>
        public string TargetFenceId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether this rule is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the priority of the rule (lower numbers = higher priority)
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Gets or sets the date when this rule was created
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Creates a deep copy of this rule
        /// </summary>
        /// <returns>A new AutoOrganizeRule object with the same values</returns>
        public AutoOrganizeRule Clone()
        {
            return new AutoOrganizeRule
            {
                Id = Guid.NewGuid().ToString(),
                Name = Name,
                Condition = Condition,
                RuleType = RuleType,
                TargetFenceId = TargetFenceId,
                IsEnabled = IsEnabled,
                Priority = Priority,
                CreatedDate = CreatedDate
            };
        }
    }

    /// <summary>
    /// Defines the types of auto-organization rules
    /// </summary>
    public enum RuleType
    {
        Extension,
        Name,
        DateCreated,
        DateModified,
        Size,
        Path
    }
}