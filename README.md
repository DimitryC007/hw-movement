# Movement Group HW - Clean Architecture .NET 8

A RESTful API built with **Clean Architecture** principles using **.NET 8**, PostgreSQL, Redis, and Docker.

---

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8) _(only needed for local development without Docker)_

---

## Setup & Run

### 1. Clone the repository

```bash
git clone https://github.com/DimitryC007/hw-movement
cd hw-movement
```

### 2. Create the environment file

The `.env` file will be provided via email. Place it in the root project directory (next to `docker-compose.yml`).

Edit `.env` if you need to change any ports or credentials. Key variables:

| Variable | Description |
|---|---|
| `API_PORT` | Host port for the Web API |
| `POSTGRES_PORT` | Host port for PostgreSQL |
| `REDIS_PORT` | Host port for Redis |
| `POSTGRES_DB` | PostgreSQL database name |
| `POSTGRES_USER` | PostgreSQL username |
| `POSTGRES_PASSWORD` | PostgreSQL password |
| `ConnectionStrings__Default` | Full PostgreSQL connection string |
| `ConnectionStrings__Redis` | Redis connection string |
| `MemoryCache__Capacity` | LRU cache capacity (min: 3, max: 100) |

### 3. Start the application

```bash
docker compose up --build
```

This starts three containers:
- **api** — the .NET 8 Web API
- **db** — PostgreSQL 16
- **redis** — Redis 7

### 4. Access the API

| Resource | URL |
|---|---|
| Swagger UI | `http://localhost:5080/swagger` |
| API base | `http://localhost:5080/api/v1` |

---

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/api/v1/data` | Create a new user |
| `GET` | `/api/v1/data/{id}` | Get a user by ID |

---

## Design Patterns Used

### Clean Architecture
The solution is split into four layers with strict dependency rules — outer layers depend on inner layers, never the reverse:

```
Domain  ←  Application  ←  Infrastructure  ←  WebApi / WebHost
```

- **Domain** — core entities (`User`), no dependencies
- **Application** — business logic (`UserService`), interfaces (`IUserRepository`, `IUserService`)
- **Infrastructure** — implementations (PostgreSQL, Redis, custom cache)
- **WebApi / WebHost** — controllers, middleware, DI composition root

---

### Repository Pattern
`IUserRepository` abstracts data access from the application layer. The concrete implementation (`UserRepository`) uses Entity Framework Core against PostgreSQL.

---

### Decorator Pattern — Cached Repository
`CachedUserRepository` wraps `UserRepository` and adds a **two-tier cache-aside strategy** transparently, without changing the application layer.

```
UserService
    └── CachedUserRepository  (decorator)
            ├── RedisCacheService (distributed cache)  ← checked first
            ├── MemoryCacheService (in-memory LRU)     ← checked second
            └── UserRepository (PostgreSQL)            ← fallback
```

---

### Cache-Aside Pattern (Two-Tier)
On every read:
1. Check **Redis** (distributed) — return immediately if hit
2. Check **in-memory LRU cache** — backfill Redis if hit, then return
3. Query **PostgreSQL** — backfill both caches on success

---

### LRU Cache
`MemoryCacheService<TKey, TValue>` implements a **Least Recently Used (LRU)** eviction policy using a `Dictionary` for O(1) lookups combined with a `DoublyLinkedList` to track access order. Capacity is configured via `MemoryCache__Capacity` in `.env`.

---

### Global Exception Handler
`GlobalExceptionHandler` implements `IExceptionHandler` and maps domain exceptions to HTTP responses centrally:

| Exception | HTTP Status |
|---|---|
| `ValidationException` | `400 Bad Request` |
| `NotFoundException<T>` | `404 Not Found` |
| Unhandled | `500 Internal Server Error` |

---

### API Versioning
Routes are versioned via URL segments (`/api/v{version}/...`), making it safe to introduce breaking changes in future versions without affecting existing consumers.

---

## Project Structure

```
├── Domain/                         # Entities
├── Application/                    # Business logic, interfaces, validators
│   └── Common/                     # Shared interfaces (IDistributedCacheService)
├── Infrastructure/                 # EF Core, Redis, LRU cache, migrations
│   ├── Caching/
│   │   ├── Memory/                 # LRU in-memory cache (MemoryCacheService)
│   │   └── Distributed/            # Distributed cache (RedisCacheService)
│   └── Persistence/
│       └── Users/                  # UserRepository + CachedUserRepository
├── WebApi/                         # Controllers, exception handler, request/response models
├── WebHost/                        # Program.cs, Startup, DI composition root
├── docker-compose.yml
└── .env.example
