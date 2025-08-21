using System;
using System.Collections.Generic;

namespace TidyTop.Core.Models
{
    /// <summary>
    /// Represents a category for automatic application organization
    /// </summary>
    public class ApplicationCategory
    {
        /// <summary>
        /// Gets or sets the unique identifier for the category
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display name of the category
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the icon identifier for the category
        /// </summary>
        public string Icon { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the color theme for the category
        /// </summary>
        public string ColorTheme { get; set; } = "#4A90E2";

        /// <summary>
        /// Gets or sets the file extensions associated with this category
        /// </summary>
        public List<string> FileExtensions { get; set; } = new();

        /// <summary>
        /// Gets or sets the application names/patterns associated with this category
        /// </summary>
        public List<string> ApplicationPatterns { get; set; } = new();

        /// <summary>
        /// Gets or sets the keywords for content-based categorization
        /// </summary>
        public List<string> Keywords { get; set; } = new();

        /// <summary>
        /// Gets or sets the priority for categorization conflicts
        /// </summary>
        public int Priority { get; set; } = 0;

        /// <summary>
        /// Gets or sets whether this category is system-defined or user-created
        /// </summary>
        public bool IsSystemCategory { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this category is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the creation date
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets predefined system categories
        /// </summary>
        public static List<ApplicationCategory> GetSystemCategories()
        {
            return new List<ApplicationCategory>
            {
                new ApplicationCategory
                {
                    Id = "office-tools",
                    Name = "Office Tools",
                    Icon = "📊",
                    ColorTheme = "#2E7D32",
                    FileExtensions = { ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".pdf", ".odt", ".ods", ".odp" },
                    ApplicationPatterns = { "word", "excel", "powerpoint", "acrobat", "reader", "office", "libreoffice", "openoffice" },
                    Keywords = { "document", "spreadsheet", "presentation", "pdf", "office" },
                    Priority = 10,
                    IsSystemCategory = true
                },
                new ApplicationCategory
                {
                    Id = "games",
                    Name = "Games",
                    Icon = "🎮",
                    ColorTheme = "#7B1FA2",
                    FileExtensions = { ".exe" },
                    ApplicationPatterns = { "steam", "epic", "game", "games", "blizzard", "origin", "uplay", "gog" },
                    Keywords = { "game", "gaming", "play", "entertainment", "steam", "epic" },
                    Priority = 8,
                    IsSystemCategory = true
                },
                new ApplicationCategory
                {
                    Id = "social-communication",
                    Name = "Social & Communication",
                    Icon = "💬",
                    ColorTheme = "#1976D2",
                    ApplicationPatterns = { "discord", "telegram", "whatsapp", "skype", "zoom", "teams", "slack", "outlook", "thunderbird", "chrome", "firefox", "edge" },
                    Keywords = { "chat", "messenger", "email", "browser", "communication", "social", "meeting" },
                    Priority = 9,
                    IsSystemCategory = true
                },
                new ApplicationCategory
                {
                    Id = "files-documents",
                    Name = "Files & Documents",
                    Icon = "📁",
                    ColorTheme = "#F57C00",
                    FileExtensions = { ".txt", ".rtf", ".md", ".zip", ".rar", ".7z" },
                    ApplicationPatterns = { "explorer", "notepad", "winrar", "7zip", "totalcommander", "filezilla" },
                    Keywords = { "file", "folder", "archive", "text", "document", "manager" },
                    Priority = 5,
                    IsSystemCategory = true
                },
                new ApplicationCategory
                {
                    Id = "development-tools",
                    Name = "Development Tools",
                    Icon = "🛠️",
                    ColorTheme = "#388E3C",
                    FileExtensions = { ".cs", ".js", ".ts", ".py", ".java", ".cpp", ".h" },
                    ApplicationPatterns = { "visual studio", "code", "intellij", "eclipse", "atom", "sublime", "notepad++", "git", "github" },
                    Keywords = { "code", "development", "programming", "ide", "editor", "git", "debug" },
                    Priority = 7,
                    IsSystemCategory = true
                },
                new ApplicationCategory
                {
                    Id = "creative-tools",
                    Name = "Creative Tools",
                    Icon = "🎨",
                    ColorTheme = "#E91E63",
                    FileExtensions = { ".psd", ".ai", ".png", ".jpg", ".jpeg", ".gif", ".mp4", ".mov", ".avi" },
                    ApplicationPatterns = { "photoshop", "illustrator", "premiere", "aftereffects", "blender", "gimp", "inkscape", "audacity" },
                    Keywords = { "photo", "image", "video", "audio", "design", "creative", "edit", "art" },
                    Priority = 6,
                    IsSystemCategory = true
                },
                new ApplicationCategory
                {
                    Id = "system-tools",
                    Name = "System Tools",
                    Icon = "⚙️",
                    ColorTheme = "#607D8B",
                    ApplicationPatterns = { "control", "settings", "regedit", "cmd", "powershell", "task manager", "device manager", "disk" },
                    Keywords = { "system", "control", "settings", "admin", "utility", "tool", "configuration" },
                    Priority = 4,
                    IsSystemCategory = true
                },
                new ApplicationCategory
                {
                    Id = "web-applications",
                    Name = "Web Applications",
                    Icon = "🌐",
                    ColorTheme = "#00ACC1",
                    FileExtensions = { ".url", ".html", ".htm" },
                    ApplicationPatterns = { "web", "online", "cloud" },
                    Keywords = { "web", "online", "cloud", "internet", "browser", "url" },
                    Priority = 3,
                    IsSystemCategory = true
                }
            };
        }
    }
}
