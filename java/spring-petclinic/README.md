# Spring PetClinic Sample Application

A sample Spring Boot application demonstrating the Spring Framework with Maven, Docker, and VS Code development environment.

## 🚀 Quick Start

### Prerequisites

- **Java 17+**
- **Git**
- **Docker** (optional, for databases/containers)
- **VS Code** (recommended with this workspace)

### Get Started

```bash
git clone TODO repo-url of fork
cd spring-petclinic
code spring-petclinic.code-workspace  # Opens VS Code with workspace
```

It's advised to install recommended extensions when prompted.

## 🏃‍♂️ How to Run

### Fastest: VS Code Tasks

`Cmd+Shift+P` → "Tasks: Run Task" → **Spring Boot: Run**

### Maven Commands

```bash
./mvnw spring-boot:run                    # Run application
./mvnw test                               # Run tests
./mvnw package                            # Build JAR
java -jar target/*.jar                    # Run JAR
```

### VS Code Debug

`F5` → Select **Spring Boot App** (or Debug variant)

**Application URL**: http://localhost:8080

## 🧪 Testing

```bash
./mvnw test                               # All tests
```

**Development Test Applications** (with DevTools):

- `PetClinicIntegrationTests` - H2 database
- `MySqlTestApplication` - MySQL via Testcontainers
- `PostgresIntegrationTests` - PostgreSQL via Docker Compose

## 💾 Databases

### Default: H2 (In-Memory)

- **Console**: http://localhost:8080/h2-console
- **JDBC**: `jdbc:h2:mem:<uuid>` (see startup logs)
- **User**: `sa` / **Password**: (empty)

### MySQL/PostgreSQL

```bash
# Start database
docker compose up mysql     # or postgres

# Run with profile
./mvnw spring-boot:run -Dspring-boot.run.profiles=mysql
./mvnw spring-boot:run -Dspring-boot.run.profiles=postgres
```

Or set environment: `cp .env.example .env` and modify.

## 📋 API Documentation

### Swagger UI

- **URL**: http://localhost:8080/swagger-ui/index.html
- **OpenAPI JSON**: http://localhost:8080/v3/api-docs

### REST Endpoints

```bash
# Get all veterinarians
curl http://localhost:8080/vets

# Application health check
curl http://localhost:8080/actuator/health
```

## 🐳 Docker

### Spring Boot Buildpacks (Recommended)

```bash
./mvnw spring-boot:build-image -Dspring-boot.build-image.imageName=spring-petclinic:latest
docker run -p 8080:8080 spring-petclinic:latest
```

### Full Stack with Docker Compose

```bash
docker compose up           # App + database
docker compose down         # Stop all
```

**VS Code Tasks**: Use `Docker Compose: Up` task for integrated experience.

## 🔧 Development

### VS Code Features

- **Launch Configurations** (`F5`):
  - `Spring Boot App` - Run application with development profile
  - `Spring Boot App (Debug)` - Debug mode with DevTools and verbose logging
  - `Run All Tests` - Execute all unit/integration tests
- **Tasks** (`Cmd+Shift+P` → "Tasks: Run Task"):

  - **Build**: `Maven: Clean`, `Maven: Compile`, `Maven: Package`, `Maven: Clean Install`
  - **Test**: `Maven: Test`, `Maven: Run Tests (Surefire Report)`
  - **Run**: `Spring Boot: Run`
  - **Docker**: `Spring Boot: Build Docker Image`, `Docker: Run Container`, `Docker Compose: Up/Down`
  - **Analysis**: `Maven: Dependency Tree`, `Maven: Site (Generate Documentation)`

- **Extensions**: Java Pack, Spring Boot Dev Pack, Maven for Java, Lombok support
- **Problem Panel**: Build errors and warnings shown automatically

### Profile Management

```bash
./mvnw spring-boot:run -Dspring-boot.run.profiles=dev     # Development
```

### CSS Development

```bash
./mvnw package -P css       # Recompile SCSS → CSS
```

## 📁 Project Structure

```
spring-petclinic/
├── pom.xml                           # Maven config
├── mvnw, mvnw.cmd                    # Maven wrapper
├── docker-compose.yml                # Multi-service setup
├── .vscode/                          # VS Code config
├── spring-petclinic.code-workspace   # Workspace file
└── src/
    ├── main/java/                    # Application code
    ├── main/resources/               # Config, static files
    └── test/java/                    # Tests
```

### Key Components

| Component     | Location                        |
| ------------- | ------------------------------- |
| Main Class    | `PetClinicApplication.java`     |
| Configuration | `application*.properties`       |
| Controllers   | `src/main/java/.../web/`        |
| Repositories  | `src/main/java/.../repository/` |
