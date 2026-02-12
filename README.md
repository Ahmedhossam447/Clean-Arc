# CleanArc - Animal Adoption & Shelter Marketplace

A modern, scalable animal adoption platform built with **Clean Architecture**, following Domain-Driven Design principles and best practices. Users can list animals for adoption and send adoption requests, while shelters sell pet products with integrated online payments and shipment tracking.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
CleanArc.Core          → Domain entities, interfaces, primitives (no dependencies)
CleanArc.Presentation  → Application layer (commands, queries, handlers, validators)
CleanArc.Infrastructure → External services (database, S3, email, identity)
Clean Arc              → API layer (controllers, middleware, SignalR hubs)
CleanArc.Testing       → Unit tests (xUnit, NSubstitute, FluentAssertions)
```

### Key Principles
- **Dependency Inversion**: Core layer has no dependencies on Infrastructure
- **CQRS Pattern**: Commands and Queries separated using MediatR
- **Result Pattern**: Functional error handling with `Result<T>` type
- **Domain-Driven Design**: Rich domain models with business logic
- **Unit of Work Pattern**: Transaction management for atomic operations
- **Repository Pattern**: Generic and specialized repositories for data access with eager loading support

## 🚀 Tech Stack

- **.NET 8** - Web API Framework
- **Entity Framework Core** - ORM with SQL Server
- **ASP.NET Core Identity** - User management, roles, and authentication
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **JWT Authentication** - Identity & Access Management (with role claims)
- **SignalR** - Real-time chat functionality
- **Hangfire** - Background job processing (Redis storage)
- **MassTransit + RabbitMQ** - Message queue for domain events
- **AWS S3** - Image storage with compression
- **Redis** - Distributed caching
- **Paymob** - Payment gateway integration
- **Swagger/OpenAPI** - API documentation

## ✨ Features

### Core Features
- ✅ **Animal Management** - Create, read, update, delete animals with photos
- ✅ **Medical Records** - One-to-one relationship with animals
- ✅ **Vaccination Tracking** - One-to-many relationship with medical records
- ✅ **Adoption Requests** - User-to-user request system with automatic rejection of other pending requests
- ✅ **Product Catalog** - Shelters can add/edit/delete products with photos
- ✅ **Order System** - Flexible cart with add/remove items before checkout
- ✅ **Payment Integration** - Paymob payment gateway with webhook processing
- ✅ **Shipment Tracking** - Per-item status tracking (Pending → Processing → Shipped → Delivered)
- ✅ **Shelter Sales Dashboard** - Shelters can view paid orders containing their products
- ✅ **User Authentication** - JWT-based auth with refresh tokens and role assignment
- ✅ **Real-time Chat** - SignalR-based messaging system (used for shelter-to-user adoption communication)
- ✅ **Real-time Notifications** - SignalR notifications for single or multiple users
- ✅ **Photo Management** - AWS S3 integration with automatic compression
- ✅ **Background Jobs** - Hangfire for async photo deletion and adoption processing
- ✅ **Caching** - Redis distributed cache with invalidation
- ✅ **Domain Events** - MassTransit for event-driven architecture
- ✅ **Transaction Management** - Unit of Work pattern for atomic database operations

### Role-Based Access Control
Three roles are seeded on application startup via `RoleSeederWorker` (IHostedService):

| Role | Capabilities |
|------|-------------|
| **User** | Create animals, send/accept/reject adoption requests, adopt animals, create orders, manage cart items, checkout, chat |
| **Shelter** | Create animals, manage products (CRUD), view sales dashboard, update order item shipment status, chat |
| **Admin** | Update adoption requests |

### Security Features
- ✅ **Role-Based Authorization** - Endpoints restricted by role (`User`, `Shelter`, `Admin`)
- ✅ **Ownership Checks** - Users can only modify their own resources (handler-level validation)
- ✅ **JWT with Role Claims** - Role embedded in token for authorization
- ✅ **HMAC Webhook Validation** - Timing-safe Paymob webhook signature verification
- ✅ **Input Validation** - FluentValidation on all commands/queries
- ✅ **Global Exception Handling** - Centralized error handling middleware
- ✅ **Foreign Key Constraints** - Database-level data integrity
- ✅ **Optimistic Concurrency** - RowVersion on Product entity

### Concurrency & Data Integrity
- ✅ **Atomic SQL** - Stock decrement via raw SQL to prevent race conditions
- ✅ **Lock Ordering** - Order items sorted by ProductId to prevent deadlocks
- ✅ **Transaction Wrapping** - Multi-product stock decrements are all-or-nothing
- ✅ **RowVersion** - Prevents stale EF Core overwrites on Product updates
- ✅ **Duplicate Cart Merging** - Same ProductId entries merged before processing

## 📁 Project Structure

```
CleanArc/
├── Clean Arc/                    # API Layer
│   ├── Controllers/              # REST API endpoints
│   ├── Middleware/               # GlobalExceptionHandler
│   ├── Extensions/               # ResultExtensions (error → HTTP status mapping)
│   └── Contracts/                # Request DTOs
│
├── CleanArc.Core/                # Domain Layer
│   ├── Entities/                 # Domain entities (Animal, Product, Order, OrderItem, etc.)
│   ├── Interfaces/               # Repository & service interfaces
│   ├── Primitives/               # Result, Error types
│   └── Events/                   # Domain events
│
├── CleanArc.Presentation/        # Application Layer
│   ├── Commands/                 # Write operations (Order, Product, Animal, Auth, etc.)
│   ├── Queries/                  # Read operations
│   ├── Handlers/                 # Command/Query handlers
│   ├── Contracts/                # Response DTOs
│   ├── Validations/              # FluentValidation rules
│   ├── Common/Security/          # PaymobSecurity (HMAC validation)
│   ├── Consumers/                # MassTransit event consumers
│   └── Pipeline Behaviour/       # MediatR pipeline behaviors
│
├── CleanArc.Infrastructure/      # Infrastructure Layer
│   ├── Persistence/              # EF Core DbContext, repositories, UnitOfWork, seed
│   ├── Services/                 # External service implementations (S3, Email, Paymob, SignalR)
│   ├── Hubs/                     # SignalR hubs (ChatHub, NameUserIdProvider)
│   ├── Identity/                 # ASP.NET Identity (ApplicationUser)
│   └── Migrations/               # Database migrations
│
└── CleanArc.Testing/             # Test Layer
    └── Unit/                     # Unit tests per feature (Animal, Product, Order, Payment)
```

## 🔧 Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Redis Server
- RabbitMQ Server
- AWS Account (for S3)
- Paymob Account (for payments)

### Configuration

1. **Update `appsettings.json`**:

```json
{
  "ConnectionStrings": {
    "AnimalConnection": "Server=YOUR_SERVER;Database=CleanArc;Trusted_Connection=True;...",
    "RedisConnection": "localhost:6379"
  },
  "Jwt": {
    "Key": "YOUR_SECRET_KEY",
    "Issuer": "YourApp",
    "Audience": "Users",
    "ExpiryMinutes": 60
  },
  "AWS": {
    "AccessKey": "YOUR_AWS_ACCESS_KEY",
    "SecretKey": "YOUR_AWS_SECRET_KEY",
    "Region": "your-region",
    "BucketName": "your-bucket-name",
    "ImageCompression": {
      "MaxWidth": 1920,
      "MaxHeight": 1920,
      "Quality": 85
    }
  },
  "Paymob": {
    "ApiKey": "YOUR_PAYMOB_API_KEY",
    "IntegrationId": "YOUR_INTEGRATION_ID",
    "IframeId": "YOUR_IFRAME_ID",
    "HmacSecret": "YOUR_HMAC_SECRET"
  },
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SenderEmail": "your-email@gmail.com",
    "SenderPassword": "your-app-password"
  }
}
```

2. **Run Database Migrations**:
```bash
dotnet ef database update --project CleanArc.Infrastructure --startup-project "Clean Arc"
```

3. **Start Services**:
   - Start Redis: `redis-server`
   - Start RabbitMQ: `rabbitmq-server`

4. **Run the Application**:
```bash
dotnet run --project "Clean Arc"
```

5. **Access**:
   - API: `https://localhost:5001` or `http://localhost:5000`
   - Swagger: `https://localhost:5001/swagger`
   - Hangfire Dashboard: `https://localhost:5001/jobs`

> **Note**: Default roles (`User`, `Shelter`, `Admin`) are automatically seeded on startup.

## 📡 API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register new user (with role: `User` or `Shelter`)
- `POST /api/auth/login` - Login and get JWT token (includes role claim)
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Logout and revoke token
- `POST /api/auth/confirm-email` - Confirm email address
- `POST /api/auth/forgot-password` - Request password reset
- `POST /api/auth/reset-password` - Reset password

### Animals (`/api/animal`)
- `POST /api/animal` - Create animal (with photo upload) **[Authorize]**
- `GET /api/animal/{id}` - Get animal by ID
- `GET /api/animal` - Get all animals (paginated)
- `GET /api/animal/Available/{userId}` - Get available animals for adoption
- `GET /api/animal/Search` - Search animals by filters
- `GET /api/animal/Owner/{ownerId}` - Get animals by owner **[Authorize]**
- `PUT /api/animal/{id}` - Update animal (with photo) **[Authorize]**
- `DELETE /api/animal/{id}` - Delete animal **[Authorize]**
- `POST /api/animal/{animalid}/Adopt` - Adopt an animal **[User]**

### Products (`/api/product`)
- `POST /api/product` - Create product (with photo) **[Shelter]**
- `GET /api/product/{id}` - Get product by ID
- `GET /api/product` - Get all products (paginated)
- `PUT /api/product/{id}` - Update product (with photo) **[Shelter]**
- `DELETE /api/product/{id}` - Delete product **[Shelter]**

### Orders (`/api/order`)
- `POST /api/order` - Create order from cart items **[User]**
- `POST /api/order/{orderId}/items` - Add item to pending order **[User]**
- `DELETE /api/order/{orderId}/items/{itemId}` - Remove item from pending order **[User]**
- `POST /api/order/{orderId}/checkout` - Checkout and get payment URL **[User]**
- `GET /api/order/my-sales` - Get sales for shelter (paginated, paid orders only) **[Shelter]**
- `PATCH /api/order/{orderId}/items/{itemId}/status` - Update item shipment status **[Shelter]**

### Payments (`/api/payment`)
- `POST /api/payment/webhook` - Paymob webhook (HMAC validated, raw body parsing)

### Adoption Requests (`/api/request`) — User-only feature
- `POST /api/request/{animalId}` - Create adoption request **[User]**
- `GET /api/request/{id}` - Get request by ID **[User]**
- `GET /api/request/my` - Get my sent requests **[User]**
- `GET /api/request/received` - Get received requests for my animals **[User]**
- `POST /api/request/{id}/accept` - Accept request **[User]**
- `POST /api/request/{id}/reject` - Reject request **[User]**
- `DELETE /api/request/{id}` - Delete/cancel request **[User]**
- `PUT /api/request/{id}` - Admin update request **[Admin]**

### Medical Records (`/api/medicalrecord`)
- `GET /api/medicalrecord/animal/{animalId}` - Get medical record by animal ID
- `PUT /api/medicalrecord/animal/{animalId}` - Update medical record **[Authorize]**

### Vaccinations (`/api/vaccinations`)
- `POST /api/vaccinations` - Add vaccination **[Authorize]**
- `PUT /api/vaccinations/{id}` - Update vaccination **[Authorize]**
- `DELETE /api/vaccinations/{id}` - Delete vaccination **[Authorize]**

### Users (`/api/user`)
- `GET /api/user/profile` - Get current user profile **[Authorize]**
- `GET /api/user/{userId}` - Get public user profile
- `PUT /api/user/profile` - Update profile **[Authorize]**

### Chat (`/api/chat`)
- `GET /api/chat/history/{otherUserId}` - Get chat history **[Authorize]**
- `GET /api/chat/unread` - Get unread messages **[Authorize]**
- `PUT /api/chat/read/{senderId}` - Mark messages as read **[Authorize]**

## 🔐 Security

### Role-Based Authorization
- **User**: Adoption requests, ordering products, adopting animals
- **Shelter**: Product management, sales dashboard, shipment status updates
- **Admin**: Administrative request updates
- **Any authenticated**: Animal CRUD (ownership validated in handlers), medical records, vaccinations, chat
- Roles are seeded automatically at startup via `RoleSeederWorker` (`IHostedService`)
- Role is assigned during registration and embedded in the JWT token

### Webhook Security
- **HMAC-SHA512** validation for Paymob webhooks
- **Timing-safe comparison** using `CryptographicOperations.FixedTimeEquals` to prevent timing attacks
- **Raw body parsing** — request body read manually via `StreamReader` to bypass ASP.NET model binding
- Invalid signatures return `401 Unauthorized`

### Error Handling
- **GlobalExceptionHandler** middleware catches all exceptions
- Returns appropriate HTTP status codes:
  - `400 Bad Request` - Validation errors
  - `401 Unauthorized` - Invalid signatures
  - `403 Forbidden` - Unauthorized access
  - `404 Not Found` - Resource not found
  - `409 Conflict` - Already processed / concurrency conflict
  - `500 Internal Server Error` - Server errors
  - `503 Service Unavailable` - Configuration errors

## 🗄️ Database Schema

### Key Relationships
- **Animal** ↔ **MedicalRecord**: One-to-One
- **MedicalRecord** ↔ **Vaccination**: One-to-Many
- **Animal** ↔ **Request**: One-to-Many
- **ApplicationUser** ↔ **Notification**: One-to-Many (Cascade Delete)
- **Animal.Userid** → **ApplicationUser.Id**: Foreign Key (Restrict)
- **Request.Userid** → **ApplicationUser.Id**: Foreign Key (Restrict)
- **Request.Useridreq** → **ApplicationUser.Id**: Foreign Key (Restrict)
- **Notification.UserId** → **ApplicationUser.Id**: Foreign Key (Cascade)
- **Product.ShelterId** → **ApplicationUser.Id**: Foreign Key
- **Order** ↔ **PaymentTransaction**: One-to-One (SetNull on delete)
- **Order** ↔ **OrderItem**: One-to-Many (Cascade Delete)
- **Order.BuyerId** → **ApplicationUser.Id**: Foreign Key (Restrict)
- **OrderItem.ProductId** → **Product.Id**: Foreign Key (Restrict)
- **OrderItem.ShelterId** → **ApplicationUser.Id**: Foreign Key (Restrict)

## 🎯 Key Implementations

### Order & Payment Flow
```
User → POST /api/order [{productId:1, qty:2}, ...]
  → Validate stock (soft check)
  → Merge duplicate cart items
  → Save Order + OrderItems to DB (Status: "Pending")
  → Return CreateOrderResponse with OrderId

User → POST /api/order/{id}/items   (add items)
User → DELETE /api/order/{id}/items/{itemId}  (remove items)

User → POST /api/order/{id}/checkout
  → Recalculate subtotal from current items
  → Re-validate stock
  → Create PaymentTransaction (Pending)
  → Call Paymob API → get payment URL
  → Return CheckoutOrderResponse with PaymentUrl

User → Pays on Paymob iframe

Paymob → POST /api/payment/webhook?hmac=xxx
  → Validate HMAC signature (timing-safe)
  → Payment success?
    → Decrement stock (atomic SQL, sorted by ProductId, in transaction)
    → Mark order "PaymentReceived" ✅
  → Payment failed?
    → Mark order "PaymentFailed" ❌ (stock untouched)

Shelter → GET /api/order/my-sales  (view paid orders)
Shelter → PATCH /api/order/{id}/items/{itemId}/status  (update shipment)
  → Status flow: Pending → Processing → Shipped → Delivered
```

### Adoption Flow
```
For User-listed animals:
  User A creates animal → User B sends adoption request
  → User A accepts/rejects → Accepted: animal marked adopted, other requests auto-rejected

For Shelter-listed animals:
  Shelter creates animal → User contacts shelter via real-time chat
  → Adoption handled directly (no request system)
```

### Stock Concurrency Strategy
- **Atomic SQL**: `UPDATE Products SET StockQuantity = StockQuantity - @qty WHERE Id = @id AND StockQuantity >= @qty` — prevents race conditions
- **Lock Ordering**: Items sorted by `ProductId` before processing — prevents deadlocks
- **Transaction Wrapping**: All stock decrements in one transaction — all-or-nothing
- **RowVersion**: `byte[] RowVersion` on Product — prevents stale EF Core overwrites
- **DB-First Saving**: Order saved to DB before calling Paymob — prevents orphaned payment orders

### Photo Management
- **Upload**: Images compressed and resized before S3 upload
- **Update**: Old photo deleted via background job, new photo uploaded
- **Delete**: Photo deletion queued via Hangfire background job
- **Compression**: Configurable max dimensions and quality

### Background Jobs
- **Hangfire** with Redis storage
- Photo deletion jobs queued asynchronously
- Adoption request processing (rejecting other pending requests)
- Dashboard available at `/jobs`

### Caching Strategy
- Redis distributed cache
- Cache invalidation after write operations
- No cancellation tokens on cache operations (data consistency)

### Domain Events
- **MassTransit + RabbitMQ** for event publishing
- Separate queues for different consumers
- Retry policies configured

### Unit of Work Pattern
- **Transaction Management**: Atomic operations for complex workflows
- **Repository Access**: Centralized access to specialized repositories (`IAnimalRepository`, `IRequestRepository`)
- **Generic Repositories**: Support for any entity via `Repository<T>()`
- **Eager Loading**: `GetAsync` supports `Include` expressions for related data
- **Raw SQL Support**: `ExecuteSqlRawAsync` for atomic operations that bypass EF Core
- **Transaction Support**: `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`

### Repository Pattern
- **Generic Repository**: Base `Repository<TEntity>` with common CRUD operations and `Include` support
- **Specialized Repositories**: 
  - `AnimalRepository` - `GetAvailableAnimalsForAdoption()`
  - `RequestRepository` - `GetRequestWithAnimalAsync()`, `GetPendingRequestsForAnimalAsync()`, `RemoveRange()`
- **Unit of Work Integration**: All repositories share the same `DbContext` for transaction consistency

### SignalR & Notifications
- **Real-time Chat**: `ChatHub` for user-to-user and user-to-shelter messaging
- **Notification Service**: 
  - `SendNotificationToUserAsync()` - Single user notifications
  - `SendNotificationAsync()` - Multiple users (broadcast)
- **User Identification**: Custom `IUserIdProvider` extracts user ID from JWT claims
- **Infrastructure Layer**: SignalR hubs and services located in Infrastructure for proper dependency flow

## 🧪 Testing

Unit tests located in `CleanArc.Testing` project:

| Test Suite | Handlers Tested |
|---|---|
| **AnimalTests** | `AdoptAnimalCommandHandler`, `DeleteAnimalCommandHandler` |
| **ProductTests** | `CreateProductCommandHandler`, `DeleteProductCommandHandler` |
| **OrderTests** | `CreateOrderCommandHandler` |
| **PaymentTests** | `ProcessPaymobWebhookCommandHandler` |

- **NSubstitute** for mocking dependencies
- **FluentAssertions** for readable assertions
- **MockExtension** helpers for `IUnitOfWork` and `Repository<T>` setup

## 📝 Development Guidelines

### Adding New Features
1. Define domain entity in `CleanArc.Core/Entities`
2. Create command/query in `CleanArc.Presentation/Commands` or `Queries`
3. Implement handler in `CleanArc.Presentation/Handlers`
4. Add FluentValidation rules
5. Create controller endpoint with appropriate `[Authorize(Roles = "...")]`
6. Add EF Core configuration if needed
7. Write unit tests in `CleanArc.Testing`

### Error Handling
- Use `Result<T>` pattern for operations that can fail
- Domain errors defined in entity `Errors` static class
- Throw exceptions only for exceptional cases (caught by GlobalExceptionHandler)
- **Structured Logging**: ILogger for non-critical operation failures (cache, SignalR, events)
- **Transaction Rollback**: Automatic rollback on exceptions within transactions
- **Graceful Degradation**: Non-critical operations (cache, notifications) fail gracefully without breaking core functionality

## 📄 License

This project is part of a learning exercise demonstrating Clean Architecture principles.

## 👤 Author

Built with Clean Architecture principles and best practices.

---

**Note**: Remember to update connection strings and secrets before deploying to production!
