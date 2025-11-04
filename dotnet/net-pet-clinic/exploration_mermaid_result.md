```mermaid
flowchart TB
    subgraph "Presentation Layer"
        WEB[Web UI/MVC Views]
        API[REST API Controllers]
        SWAGGER[Swagger UI]
    end
    
    subgraph "Application Layer"
        MVC[MVC Controllers]
        APICTRL[API Controllers]
        VAL[Validators]
    end
    
    subgraph "Business Layer"
        REPO[Repository Pattern]
        MODEL[Domain Models]
    end
    
    subgraph "Data Layer"
        EF[Entity Framework]
        SQLITE[(SQLite DB)]
    end
    
    subgraph "Infrastructure"
        LOG[Serilog Logging]
        DI[Dependency Injection]
        DOCKER[Docker Container]
    end
    
    WEB --> MVC
    API --> APICTRL
    SWAGGER --> APICTRL
    
    MVC --> REPO
    APICTRL --> REPO
    MVC --> VAL
    
    REPO --> MODEL
    REPO --> EF
    EF --> SQLITE
    
    MVC --> LOG
    APICTRL --> LOG
    DI --> REPO
    DI --> LOG
    
    DOCKER -.-> WEB
    DOCKER -.-> API
    ```