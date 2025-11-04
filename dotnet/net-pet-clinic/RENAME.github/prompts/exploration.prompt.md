---
mode: 'agent'
description: 'Generate a concise architecture report (text + Mermaid)'
tools: ['runCommands', 'runTasks', 'extensions', 'todos', 'runTests'] 
---
# 📄 Application Architecture Report for ${workspaceFolderBasename}

## 1 . Purpose  
Summarise the business goal and primary functionalities of the application in 2‑3 sentences.

## 2 . Technology stack  
* Detect main languages, frameworks and build tools by scanning:  
  `package.json`, `pom.xml`, `build.gradle`, `requirements.txt`, `Dockerfile`, etc.  
* List runtime versions (Node, JDK, Python …) and key third‑party libraries.

## 3 . High‑level component map (Mermaid)  
1. Identify major **domains / bounded contexts** (top‑level folders or packages).  
2. Detect public entry points (HTTP controllers, CLI commands, scheduled jobs).  
3. Map edges according to import or API‑call graphs (one‑way arrows).  
4. Draw a clear, *top‑to‑bottom* Mermaid diagram:

```mermaid
flowchart TB
    %% Replace the example nodes below %%
    subgraph Frontend
        FE[Web UI]
    end
    subgraph Backend
        BE[API Gateway] --> SVC[Business Service]
        SVC --> DB[(Database)]
    end
    FE -->|REST| BE
````

* Use `classDiagram` instead if object relationships are clearer.
* Keep node labels < 15 chars where possible for readability.

## 4 . Data flow

Describe critical data paths (request/response, event streams, batch ETL).
Highlight persistence choices (SQL vs NoSQL), caching layers and external integrations.

## 5 . Deployment & runtime concerns

* Container / cloud platform (Kubernetes, ECS, App Service …).
* CI/CD pipeline brief (build, test, deploy stages).
* Scaling strategy and fault‑tolerance mechanisms.

## 6 . Observability & quality gates

List logging, metrics, APM, security scanners and test coverage stats detected in the repo.
