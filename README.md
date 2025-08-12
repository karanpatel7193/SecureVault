# SecureVault

SecureVault is a cross-platform password management application built with .NET MAUI. It allows users to securely store, manage, and retrieve passwords across Android, iOS, Windows devices.

## Features

- Secure password storage using SQLite
- User authentication (login/register)
- Password list and detail views
- Responsive UI with custom fonts and icons
- Multi-platform support (.NET 8)

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (with .NET MAUI workload installed)
- Platform-specific requirements (Android/iOS/MacCatalyst/Windows/Tizen)

### Setup

1. Clone the repository:

2. Restore NuGet packages:

3. Build and run for your target platform:
- Android: `dotnet build -f net8.0-android`
- iOS: `dotnet build -f net8.0-ios`
- Windows: `dotnet build -f net8.0-windows10.0.19041.0`
- MacCatalyst: `dotnet build -f net8.0-maccatalyst`
- Tizen: (optional, see [Tizen.NET](https://github.com/Samsung/Tizen.NET))

### Dependencies

- [CommunityToolkit.Maui](https://github.com/CommunityToolkit/Maui)
- [Microsoft.Maui.Controls](https://github.com/dotnet/maui)
- [sqlite-net-pcl](https://github.com/praeclarum/sqlite-net)
- [Microsoft.Extensions.Logging.Debug](https://learn.microsoft.com/en-us/dotnet/core/extensions/logging)

### Database

- Uses SQLite for local password storage.
- No external database configuration required.

### Testing

- Unit and UI tests can be added using xUnit, NUnit, or MSTest.
- To run tests: