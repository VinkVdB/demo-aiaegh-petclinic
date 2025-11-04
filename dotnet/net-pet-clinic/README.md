# .NET PetClinic Sample Application

A sample ASP.NET Core 8.0 MVC application demonstrating modern .NET patterns with EF Core, SQLite, and Docker support.

## 🚀 Quick Start

### Prerequisites

- **.NET 8.0 SDK**
- **Git**
- **Docker** (optional, for containerized deployment)
- **VS Code** (recommended)

## 🏃‍♂️ How to Run

```bash
cd PetClinic
dotnet run                              # Run application
dotnet test                             # Run tests (when available)
dotnet build                            # Build application
```

**Application URL**: https://localhost:5172

## 🧪 Testing

```bash
dotnet test                             # All tests (when implemented)
```

**Development Database**: SQLite (`petclinic-dev.db`) with sample data

## 💾 Database

### Default: SQLite

- **Development**: `petclinic-dev.db`
- **Production**: `petclinic.db`
- **Sample Data**: Automatically seeded on startup

### Database Inspection

```bash
# SQLite command line (if installed)
sqlite3 petclinic-dev.db
.tables
SELECT * FROM owners;
```

## 📋 API Documentation

### Health Check

```bash
# Application health check
curl https://localhost:5172/health
```

### Swagger UI

- **URL**: https://localhost:5172/swagger (development only)

## 🐳 Docker

```bash
docker-compose up --build               # Build and run
# Access: http://localhost:8080
```

## Development Tools

```bash
cd PetClinic
rm *.db                                 # Delete database
dotnet run                              # Recreate and seed
```