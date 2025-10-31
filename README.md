# Combat Tracker - D&D Combat Management Application

[![PR Pipeline](https://github.com/tijmenwritesprograms/CombatTracker/actions/workflows/pr-pipeline.yml/badge.svg)](https://github.com/tijmenwritesprograms/CombatTracker/actions/workflows/pr-pipeline.yml)

A web application designed to manage and streamline combat encounters in tabletop RPGs, primarily Dungeons & Dragons (5e). Built with .NET 8, Blazor WebAssembly, and .NET Aspire orchestration.

## Features

- Party and character management
- Combat encounter tracking
- Initiative order management
- Hit point tracking
- Client-side state persistence with browser localStorage
- AI-assisted statblock parsing (planned)
- Combat narration generation (planned)

## Project Structure

```
CombatTracker/
├── CombatTracker.AppHost/           # .NET Aspire orchestration host
├── CombatTracker.WebAssembly/       # Blazor WebAssembly application
│   ├── Components/                  # Blazor components and pages
│   ├── Models/                      # Data models
│   └── Services/                    # Business logic services
├── CombatTracker.WebAssembly.Tests/ # WebAssembly tests (bUnit + xUnit)
├── CombatTracker.ServiceDefaults/   # Shared service configurations
├── CombatTracker.ApiService/        # API service for AI features (future)
├── CombatTracker.Web/               # Legacy Blazor Server application (for reference)
└── CombatTracker.Web.Tests/         # Legacy Blazor Server tests
```

## Architecture

The application uses **Blazor WebAssembly** for client-side execution:
- All UI rendering happens in the browser
- State is managed client-side with services
- Data persistence uses browser localStorage
- No server-side SignalR connection required
- Improved scalability and offline-capable design

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (version 8.0.415 or later)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling) (for development orchestration)

## Setup

### Install .NET Aspire Workload

If you haven't already installed the Aspire workload:

```bash
dotnet workload install aspire
```

### Restore Dependencies

```bash
dotnet restore
```

## Running the Application

### Using the Aspire App Host (Recommended for local development)

Run the application through the Aspire orchestration:

```bash
cd CombatTracker.AppHost
dotnet run
```

This will:
- Start the Aspire dashboard
- Launch the Blazor WebAssembly application
- Launch the API service
- Configure service discovery and monitoring
- Provide live logs and metrics

The Aspire dashboard will be available at `https://localhost:17xxx` (check console output for exact port).
The Blazor application will be available at `https://localhost:7xxx` (check Aspire dashboard for exact URL).

**Note**: The Aspire App Host requires Docker Desktop or Podman to be running on your local machine. If you encounter DCP (Developer Control Plane) errors, ensure Docker is running or use the direct method below.

### Running the WebAssembly Application Directly

Alternatively, you can run just the Blazor WebAssembly app:

```bash
cd CombatTracker.WebAssembly
dotnet run
```

The application will be available at `https://localhost:7xxx` (check console output).

**WebAssembly Deployment**: The compiled WebAssembly app can be hosted on any static file server (GitHub Pages, Azure Static Web Apps, Netlify, etc.) since all logic runs client-side.

## Development

### Hot Reload

The application supports hot reload for rapid development. When running via `dotnet run`, changes to `.razor` and `.cs` files will automatically reload in the browser.

### Running Tests

Run all tests:

```bash
dotnet test
```

Run only WebAssembly tests:

```bash
dotnet test CombatTracker.WebAssembly.Tests
```

### Project Configuration

- **global.json**: Pins the .NET SDK version to 8.0.415
- **CombatTracker.sln**: Solution file containing all projects
- **ServiceDefaults**: Shared configuration for logging, health checks, and OpenTelemetry

## Technology Stack

- **.NET 8**: Latest LTS version of .NET
- **Blazor WebAssembly**: Client-side web UI running in the browser via WebAssembly
- **.NET Aspire**: Cloud-ready orchestration and observability
- **bUnit**: Unit testing library for Blazor components
- **xUnit**: Testing framework

## Migration Notes

This application was migrated from Blazor Server to Blazor WebAssembly:
- **Benefits**: Better scalability, offline capability, reduced server load, CDN deployment
- **Changes**: All state management moved to client-side, localStorage used for persistence
- **Compatibility**: The legacy Blazor Server project (`CombatTracker.Web`) is kept for reference

## Next Steps

1. ✅ Implement party management features
2. ✅ Create combat encounter setup screens
3. ✅ Build initiative tracker
4. Add AI integration for statblock parsing
5. Implement combat narration generation
6. Add offline support with service workers

## Documentation

See [Functional Specification](spec/functionalspecs.md) for detailed feature requirements.

## License

[Add your license here]

## Contributing

[Add contribution guidelines here]
