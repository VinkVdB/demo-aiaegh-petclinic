# .NET PetClinic - VSCode Setup

This directory contains the Visual Studio Code workspace and configuration files for the .NET PetClinic project.

## Quick Start

### Opening the Workspace
1. Open `pet-clinic.code-workspace` in VS Code
2. Install the recommended extensions when prompted
3. The workspace will automatically configure .NET development settings

### Running the Application

#### Debug Mode (F5)
- Press `F5` or use "Run and Debug" panel
- Select ".NET Core Launch (web)" for normal debugging
- Select ".NET Core Launch (Debug Mode)" for verbose logging
- Application will automatically open in browser

#### Development with Hot Reload
- Press `Ctrl+Shift+P` → "Tasks: Run Task" → "watch"
- Or use terminal: `dotnet watch run --project PetClinic/PetClinic.csproj`

#### Production Docker
- Press `Ctrl+Shift+P` → "Tasks: Run Task" → "docker-run"
- Or use terminal: `docker-compose up -d`
- Access at http://localhost:8080

### Available VSCode Tasks

| Task | Description | Shortcut |
|------|-------------|----------|
| **build** | Build the solution | `Ctrl+Shift+P` → "Tasks: Run Build Task" |
| **test** | Run all tests | `Ctrl+Shift+P` → "Tasks: Run Test Task" |
| **clean** | Clean build artifacts | Task menu |
| **restore** | Restore NuGet packages | Task menu |
| **watch** | Run with hot reload | Task menu |
| **test-watch** | Run tests with watch mode | Task menu |
| **docker-build** | Build Docker image | Task menu |
| **docker-run** | Run in Docker (includes build) | Task menu |
| **docker-stop** | Stop Docker containers | Task menu |

### Debug Configurations

| Configuration | Description | Use Case |
|---------------|-------------|----------|
| **.NET Core Launch (web)** | Standard web debugging | General development |
| **.NET Core Launch (Debug Mode)** | Verbose logging enabled | Troubleshooting |
| **.NET Core Attach** | Attach to running process | Advanced debugging |
| **Run All Tests** | Execute test suite | Test validation |

### Project Structure
```
.vscode/
├── extensions.json     # Recommended extensions
├── launch.json        # Debug configurations
├── settings.json      # Workspace settings
└── tasks.json         # Build and run tasks

pet-clinic.code-workspace  # Main workspace file
docker-compose.yml         # Docker configuration
PetClinic.sln             # Solution file
PetClinic/                # Main application
```

### Extensions Included
- **C# Dev Kit** - Primary .NET development
- **C#** - Language support
- **.NET Install Tool** - Runtime management
- **JSON** - Configuration file support
- **Tailwind CSS** - CSS framework support
- **Auto Rename Tag** - HTML editing
- **PowerShell** - Script support

### Development Workflow
1. **Code** → Edit files in VSCode
2. **Build** → `Ctrl+Shift+B` or F5 to run
3. **Test** → `Ctrl+Shift+P` → "Tasks: Run Test Task"
4. **Debug** → Set breakpoints, press F5
5. **Deploy** → Use docker-run task for containerized testing

### Port Configuration
- **Development**: http://localhost:5000 (HTTP) / https://localhost:5001 (HTTPS)
- **Docker**: http://localhost:8080

### Database
- **Development**: SQLite in-memory database with sample data
- **Production**: SQLite file database (`/app/data/petclinic.db` in container)

## Troubleshooting

### Build Issues
- Run "clean" task then "restore" task
- Check that .NET 8.0 SDK is installed
- Verify all extensions are installed and enabled

### Docker Issues
- Ensure Docker is running
- Run `docker-compose down` then `docker-compose up --build`
- Check port 8080 is not in use by other applications

### IntelliSense Issues
- Press `Ctrl+Shift+P` → "Developer: Reload Window"
- Ensure C# Dev Kit extension is enabled
- Check .NET SDK version with `dotnet --version`

---

**Migration Note**: This configuration mirrors the Spring PetClinic VSCode setup, adapted for .NET development patterns and tooling.
