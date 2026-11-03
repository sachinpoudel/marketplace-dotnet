# Marketplace API

A .NET marketplace backend built with a clean architecture split across API, application, domain, and infrastructure layers.

## Current architecture

- API layer: [src/MarketPlace.API](src/MarketPlace.API)
- Application layer: [src/MarketPlace.Application](src/MarketPlace.Application)
- Domain layer: [src/MarketPlace.Domain](src/MarketPlace.Domain)
- Infrastructure layer: [src/MarketPlace.Infrastructure](src/MarketPlace.Infrastructure)

### Key patterns already in use

- ASP.NET Core Web API with minimal endpoint wiring via `Program.cs`
- MediatR for commands and queries
- Repository + UnitOfWork pattern for persistence
- ASP.NET Identity + JWT-based auth
- EF Core with PostgreSQL/Npgsql
- Serilog structured logging
- Clean separation of business logic from infrastructure concerns

## Core flow implemented

The project already contains the main marketplace flow for:

- Authentication and authorization
- Product management
- Category and vendor management
- Cart operations
- Order lifecycle
- Payment handling
- Review flow

## Documentation

- Infrastructure and complexity guide: [docs/infrastructure-and-complexity-guide.md](docs/infrastructure-and-complexity-guide.md)

## Suggested next phase

The next step is not more basic CRUD. The strongest next move is to improve the platform infrastructure and operational readiness:

1. production-grade persistence and backups
2. identity security hardening
3. cache, jobs, and messaging
4. file storage and search
5. observability and release automation

This project is already structurally strong enough to evolve into a real marketplace platform with more scale, reliability, and operational control.
