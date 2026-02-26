# CleanArc - Animal Adoption & Shelter Marketplace

An animal adoption platform where users list animals for adoption, send adoption requests, and shelters sell pet products with integrated payments and shipment tracking.

I built an [MVC version](https://github.com/Ahmedhossam447/Pet-Adoption) of this platform first, then wanted an API-focused backend — Clean Architecture, CQRS, and best practices for maintainable, testable code.

---


## Architecture

```
CleanArc.Core          → Domain entities, interfaces, primitives, event publisher interface (no dependencies)
CleanArc.Application   → Commands, queries, handlers, validators (depends on Core only)
CleanArc.Infrastructure → Database, S3, Paymob webhook security, RabbitMQ, MassTransit consumers (depends on Core only)
CleanArc.API           → Controllers, middleware, DI composition (depends on Application + Infrastructure)
CleanArc.Testing       → Unit + architecture tests
```

```mermaid
graph TD
    Client([Client])

    Client -->|REST| API
    Client <-->|WebSocket| API

    subgraph API[CleanArc.API]
        Controllers
        ExceptionHandler
    end

    Controllers -->|MediatR| Application

    subgraph Application[CleanArc.Application]
        Validation[FluentValidation]
        Handlers[Command / Query Handlers]
    end

    Handlers -.->|depends on| Core

    subgraph Core[CleanArc.Core]
        Entities
        Interfaces
        ResultPattern["Result‹T› Pattern"]
        EventPub[IEventPublisher]
    end

    Interfaces -.->|implemented by| Infrastructure

    subgraph Infrastructure[CleanArc.Infrastructure]
        Repositories[UnitOfWork + Repositories]
        Services[Auth · Image · Payment]
        SignalR[SignalR Hubs]
        MassTransit[MassTransit Consumers]
        Hangfire[Hangfire Jobs]
    end

    Repositories --> DB[(SQL Server)]
    Services --> S3[AWS S3]
    Services --> Paymob
    Services --> Google[Google OAuth]
    Handlers -->|Cache-Aside| Redis[(Redis)]
    Hangfire -->|Storage| Redis
    EventPub --> MassTransit
    MassTransit --> RabbitMQ[(RabbitMQ)]
```

### Patterns & Practices

- **Clean Architecture** – Core has no infrastructure dependencies; business logic stays isolated
- **CQRS** – Commands and queries separated via MediatR
- **Result pattern** – `Result<T>` for expected failures instead of exceptions
- **Unit of Work** – Shared DbContext, transaction management for atomic operations
- **Repository** – Generic `Repository<T>` with eager loading, specialized `AnimalRepository` for domain-specific queries
- **Domain events** – `IEventPublisher` abstraction in Core, MassTransit implementation in Infrastructure, RabbitMQ consumers for adoption emails and audit logging
- **FluentValidation** – Request validation in the pipeline
- **Role-based auth** – JWT with User, Shelter, Admin roles

---

## Tech Stack

- .NET 10, Entity Framework Core, SQL Server
- ASP.NET Core Identity, JWT
- MediatR, FluentValidation
- SignalR (chat, notifications)
- Hangfire (background jobs), Redis (cache, Hangfire storage)
- MassTransit + RabbitMQ (domain events)
- AWS S3, Paymob
- Docker + Docker Compose

---

## Features

### Core Features

- **Animal Management** – Create, read, update, delete animals with photos
- **Medical Records** – One-to-one with animals
- **Vaccination Tracking** – One-to-many with medical records
- **Adoption Requests** – User-to-user requests; accepting one auto-rejects other pending requests
- **Product Catalog** – Shelters add/edit/delete products with photos and stock
- **Order System** – Cart, add/remove items, checkout; order saved as Pending before payment
- **Payment Integration** – Paymob webhook; stock decremented only on confirmed payment
- **Shipment Tracking** – Per-item status (Pending → Processing → Shipped → Delivered)
- **Shelter Sales Dashboard** – Shelters view paid orders containing their products
- **User Authentication** – JWT with refresh tokens, role assignment (User/Shelter)
- **Social Login** – OAuth 2.0 Authentication with automatic account linking for existing users
- **Real-time Chat** – SignalR for user-to-user and user-to-shelter messaging
- **Real-time Notifications** – SignalR notifications for single or multiple users
- **Photo Management** – AWS S3 with compression; Hangfire for async deletion
- **Background Jobs** – Hangfire for photo deletion and adoption request processing
- **Caching** – Redis distributed cache with invalidation on writes
- **Domain Events** – MassTransit + RabbitMQ (adoption emails, audit logging)
- **Transaction Management** – Unit of Work for atomic operations

### Role-Based Access Control

| Role | Capabilities |
|------|--------------|
| **User** | Create animals, send/accept/reject adoption requests, adopt, create orders, manage cart, checkout, chat |
| **Shelter** | Create animals, manage products (CRUD), view sales dashboard, update shipment status, chat |
| **Admin** | Update adoption requests |

### Security

- Role-based authorization on endpoints
- Ownership checks in handlers (users modify only their own resources)
- JWT with role claims
- HMAC-SHA512 webhook validation (timing-safe)
- FluentValidation on commands/queries
- Global exception handling middleware
- RowVersion on Product for optimistic concurrency

### Concurrency & Data Integrity

- **Atomic SQL** – Stock decrement via raw SQL to prevent race conditions
- **Lock ordering** – Items sorted by ProductId before processing to prevent deadlocks
- **Transaction wrapping** – Multi-product stock decrements are all-or-nothing
- **Duplicate cart merging** – Same ProductId entries merged before processing

---

## Flows

### Order & Payment Flow

```
User → POST /api/order [{productId, qty}, ...]
  → Validate stock (soft check)
  → Merge duplicate product IDs
  → Save Order + OrderItems to DB (Status: "Pending")
  → Return CreateOrderResponse with OrderId

User → POST /api/order/{id}/items   (add items)
User → DELETE /api/order/{id}/items/{itemId}  (remove items)

User → POST /api/order/{id}/checkout
  → Re-validate stock, recalculate subtotal
  → Create PaymentTransaction (Pending)
  → Call Paymob API → get payment URL
  → Return CheckoutOrderResponse with PaymentUrl

User → Pays on Paymob iframe

Paymob → POST /api/payment/webhook?hmac=xxx
  → Validate HMAC (timing-safe)
  → Success? → Atomic SQL stock decrement (sorted by ProductId, in transaction)
              → Order status "PaymentReceived"
  → Failed?  → Order status "PaymentFailed" (stock untouched)

Shelter → GET /api/order/my-sales  (view paid orders)
Shelter → PATCH /api/order/{id}/items/{itemId}/status  (update shipment)
  → Status: Pending → Processing → Shipped → Delivered
```

### Adoption Flow

```
User-listed animals:
  User A creates animal → User B sends adoption request
  → User A accepts/rejects → Accepted: animal marked adopted, other requests auto-rejected

Shelter-listed animals:
  Shelter creates animal → User contacts shelter via chat
  → Adoption handled directly (no request system)

Domain event:
  On adoption → AnimalAdoptedEvent published
  → Consumers: send email, write audit log (MassTransit + RabbitMQ)
```

### OAuth 2.0 Authentication Flow

```
Client → OAuth 2.0 Provider (e.g., Google)
  → User logs in & grants consent
  → Provider issues OpenID Connect JWT (id_token)

Client → POST /api/auth/google-login { "tokenId": "..." }
  → Validate Provider JWT signature & audience
  → IF User exists (by email):
      → Ensure EmailConfirmed = true
      → Add UserLogin mapping (links Provider ID to existing account)
  → IF User is new:
      → Generate unique username (email prefix + random) and secure password
      → Register User in Db with Role "User" and EmailConfirmed = true
      → Add UserLogin mapping
  → Generate system JWT Access Token & Refresh Token
  → Return Tokens to Client
```

### Stock Concurrency Strategy

- **Atomic SQL** – `UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE Id = @id AND StockQuantity >= @qty`
- **Lock ordering** – Order items sorted by ProductId before decrement
- **Transaction wrapping** – All decrements in one transaction (all-or-nothing)
- **RowVersion** – On Product entity for optimistic concurrency
- **DB-first** – Order saved before Paymob call to avoid orphaned payment orders

### Photo Management

- **Upload** – Images compressed and resized before S3 upload
- **Update** – Old photo deleted via Hangfire, new photo uploaded
- **Delete** – Photo deletion queued via Hangfire background job
- **Compression** – Configurable max dimensions and quality

### Background Jobs (Hangfire)

- Photo deletion queued asynchronously
- Adoption request processing (reject other pending requests)
- Dashboard at `/jobs`

### Caching (Redis) – Cache-Aside

- **Cache-Aside** (lazy loading): read from cache first; on miss, load from DB and populate cache
- Writes go to DB; handlers invalidate cache (`RemoveAsync`) on create/update/delete
- `ICacheableQuery` pipeline behavior for cacheable queries

### Unit of Work & Repository

- **Unit of Work** – `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`, `ExecuteSqlRawAsync`
- **Repository** – Generic `Repository<T>()` and specialized `AnimalRepository`
- **Eager loading** – `GetAsync` with `Include` expressions
- All repositories share the same DbContext for transaction consistency

### SignalR & Notifications

- **ChatHub** – Real-time messaging
- **NotificationService** – `SendNotificationToUserAsync`, `SendNotificationAsync` (broadcast)
- **IUserIdProvider** – User ID from JWT claims

---

## Project Structure

```
CleanArc/
├── CleanArc.API/             # API (controllers, middleware, Dockerfile)
├── CleanArc.Core/            # Domain (entities, interfaces, primitives)
├── CleanArc.Application/     # Commands, queries, handlers, validators
├── CleanArc.Infrastructure/  # DbContext, repositories, UnitOfWork, services, hubs
├── CleanArc.Testing/         # Unit tests
├── docker-compose.yml        # SQL Server, Redis, RabbitMQ, API
└── docker-compose.override.yml
```

---

## Setup

### Docker (recommended)

```bash
docker-compose up --build
```

This starts the API, SQL Server, Redis, and RabbitMQ. The API is available at `http://localhost:8080`.

### Local

**Prerequisites:** .NET 10 SDK, SQL Server, Redis, RabbitMQ, AWS (S3), Paymob

1. Configure `appsettings.json` (connection strings, JWT, AWS, Paymob, email).
2. `dotnet ef database update --project CleanArc.Infrastructure --startup-project "CleanArc.API"`
3. Start Redis and RabbitMQ.
4. `dotnet run --project "CleanArc.API"`

Swagger at `/swagger`, Hangfire at `/jobs`. Roles seeded on startup.

---

## API Overview

- **Auth** – Register, login, google-login, refresh, logout, confirm email, forgot/reset password
- **Animals** – CRUD, search, available for adoption, adopt
- **Products** – CRUD (Shelter)
- **Orders** – Create, add/remove items, checkout, sales (Shelter), shipment status
- **Payments** – Webhook (Paymob)
- **Requests** – Create, accept, reject, list (User)
- **Chat** – History, unread, mark read
- **Users** – Profile, update

---

## Testing

94 unit tests in `CleanArc.Testing` covering all command handlers across the application:

| Domain | Handlers Tested |
|--------|----------------|
| **Auth** | Login, GoogleLogin, Register |
| **Orders** | CreateOrder, AddOrderItem, RemoveOrderItem, UpdateOrderItemStatus, CheckoutOrder |
| **Animals** | AdoptAnimal, CreateAnimal, DeleteAnimal, UpdateAnimal |
| **Products** | CreateProduct, DeleteProduct |
| **Payments** | ProcessPaymobWebhook |
| **Requests** | CreateRequest, AcceptRequest, RejectRequest, DeleteRequest, UpdateRequest |

Architecture tests via NetArchTest. NSubstitute for mocking, FluentAssertions for readable assertions.

```bash
dotnet test "Clean Arc.sln"
```
