# TidyTop

A modern cross-platform desktop organization application, built with .NET 8 and Avalonia UI.

## Overview

TidyTop is a desktop organization application that helps you organize your desktop icons and shortcuts into customizable fenced areas. It allows you to create, resize, and organize containers (fences) on your desktop to group related icons together, reducing clutter and improving productivity.

## Features

- **Desktop Fences**: Create customizable fenced areas on your desktop
- **Icon Organization**: Drag and drop icons between fences
- **Auto-Organization**: Automatically sort icons within fences by name, type, or date
- **Layout Management**: Save and restore desktop layouts
- **Quick Hide**: Show/hide fences to reveal a clean desktop
- **Customization**: Customize fence appearance, colors, and transparency
- **Cross-platform**: Works on Windows, macOS, and Linux
- **Rules Engine**: Create rules to automatically place new icons in specific fences

## Technology Stack

- **Framework**: .NET 8
- **UI Framework**: Avalonia UI
- **Architecture**: MVVM (Model-View-ViewModel)
- **Language**: C#
- **Testing**: xUnit

## Project Structure

```
TidyTop/
├── src/
│   ├── TidyTop.App/          # Main application project
│   ├── TidyTop.Core/         # Core business logic
│   └── TidyTop.Data/         # Data access layer
├── tests/
│   ├── TidyTop.App.Tests/    # UI and integration tests
│   └── TidyTop.Core.Tests/   # Unit tests for core logic
└── docs/                     # Documentation
```

## Building and Running

### Prerequisites

- .NET 8 SDK
- Visual Studio Code or Visual Studio 2022

### Building the Solution

```bash
dotnet build
```

### Running the Application

```bash
dotnet run --project src/TidyTop.App/TidyTop.App.csproj
```

### Running Tests

```bash
dotnet test
```

## Development

This project follows a modern development approach with:

- Clean Architecture principles
- Dependency Injection
- Unit Testing
- Continuous Integration

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Submit a pull request

## License

This project is licensed under the MIT License.
