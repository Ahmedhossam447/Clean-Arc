# CleanArc - Animal Adoption Platform

A modern, scalable animal adoption platform built with **Clean Architecture**, following Domain-Driven Design principles and best practices.

## 🏗️ Architecture

This project follows **Clean Architecture** principles with clear separation of concerns:

```
CleanArc.Core          → Domain entities, interfaces, primitives (no dependencies)
CleanArc.Presentation  → Application layer (commands, queries, handlers, validators)
CleanArc.Infrastructure → External services (database, S3, email, identity)
Clean Arc              → API layer (controllers, middleware, SignalR hubs)
```

### Key Principles
- **Dependency Inversion**: Core layer has no dependencies on Infrastructure
- **CQRS Pattern**: Commands and Queries separated using MediatR
- **Result Pattern**: Functional error handling with `Result<T>` type
- **Domain-Driven Design**: Rich domain models with business logic
- **Unit of Work Pattern**: Transaction management for atomic operations
- **Repository Pattern**: Generic and specialized repositories for data access

## 🚀 Tech Stack

- **.NET 8** - Web API Framework
- **Entity Framework Core** - ORM with SQL Server
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **JWT Authentication** - Identity & Access Management
- **SignalR** - Real-time chat functionality
- **Hangfire** - Background job processing (Redis storage)
- **MassTransit + RabbitMQ** - Message queue for domain events
- **AWS S3** - Image storage with compression
- **Redis** - Distributed caching
- **Swagger/OpenAPI** - API documentation

## ✨ Features

### Core Features
- ✅ **Animal Management** - Create, read, update, delete animals with photos
- ✅ **Medical Records** - One-to-one relationship with animals
- ✅ **Vaccination Tracking** - One-to-many relationship with medical records
- ✅ **Adoption Requests** - Request system for animal adoption with automatic rejection of other pending requests
- ✅ **User Authentication** - JWT-based auth with refresh tokens
- ✅ **Real-time Chat** - SignalR-based messaging system
- ✅ **Real-time Notifications** - SignalR notifications for single or multiple users
- ✅ **Photo Management** - AWS S3 integration with automatic compression
- ✅ **Background Jobs** - Hangfire for async photo deletion and adoption processing
- ✅ **Caching** - Redis distributed cache with invalidation
- ✅ **Domain Events** - MassTransit for event-driven architecture
- ✅ **Transaction Management** - Unit of Work pattern for atomic database operations

### Security Features
- ✅ **Authorization Checks** - Users can only modify their own resources
- ✅ **JWT Authentication** - Secure token-based authentication
- ✅ **Input Validation** - FluentValidation on all commands/queries
- ✅ **Global Exception Handling** - Centralized error handling
- ✅ **Foreign Key Constraints** - Database-level data integrity

## 📁 Project Structure

```
CleanArc/
├── Clean Arc/                    # API Layer
│   ├── Controllers/              # REST API endpoints
│   ├── Middleware/               # GlobalExceptionHandler
│   └── Contracts/                # Request/Response DTOs
│
├── CleanArc.Core/                # Domain Layer
│   ├── Entities/                 # Domain entities (Animal, Request, etc.)
│   ├── Interfaces/               # Repository & service interfaces
│   ├── Primitives/               # Result, Error types
│   └── Events/                   # Domain events
│
├── CleanArc.Presentation/        # Application Layer
│   ├── Commands/                 # Write operations
│   ├── Queries/                  # Read operations
│   ├── Handlers/                 # Command/Query handlers
│   ├── Validations/              # FluentValidation rules
│   ├── Consumers/                # MassTransit event consumers
│   └── Pipeline Behaviour/       # MediatR pipeline behaviors
│
└── CleanArc.Infrastructure/      # Infrastructure Layer
    ├── Persistence/              # EF Core DbContext, repositories, UnitOfWork
    ├── Services/                 # External service implementations (S3, Email, SignalR)
    ├── Hubs/                     # SignalR hubs (ChatHub, NameUserIdProvider)
    ├── Identity/                 # ASP.NET Identity
    └── Migrations/                # Database migrations
```

## 🔧 Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (or SQL Server Express)
- Redis Server
- RabbitMQ Server
- AWS Account (for S3)

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

## 📡 API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login and get JWT token
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
- `POST /api/animal/{animalid}/Adopt` - Adopt an animal **[Authorize]**

### Medical Records (`/api/medicalrecord`)
- `GET /api/medicalrecord/animal/{animalId}` - Get medical record by animal ID
- `PUT /api/medicalrecord/animal/{animalId}` - Update medical record **[Authorize]**

### Vaccinations (`/api/vaccinations`)
- `POST /api/vaccinations` - Add vaccination **[Authorize]**
- `PUT /api/vaccinations/{id}` - Update vaccination **[Authorize]**
- `DELETE /api/vaccinations/{id}` - Delete vaccination **[Authorize]**

### Requests (`/api/request`)
- `POST /api/request/{animalId}` - Create adoption request **[Authorize]**
- `GET /api/request/{id}` - Get request by ID **[Authorize]**
- `GET /api/request/my` - Get my requests **[Authorize]**
- `PUT /api/request/{id}/accept` - Accept request **[Authorize]**
- `PUT /api/request/{id}/reject` - Reject request **[Authorize]**

### Users (`/api/user`)
- `GET /api/user/profile` - Get current user profile **[Authorize]**
- `GET /api/user/{userId}` - Get public user profile
- `PUT /api/user/profile` - Update profile **[Authorize]**

### Chat (`/api/chat`)
- `GET /api/chat/history/{otherUserId}` - Get chat history **[Authorize]**
- `GET /api/chat/unread` - Get unread messages **[Authorize]**
- `PUT /api/chat/read/{senderId}` - Mark messages as read **[Authorize]**

## 🔐 Security

### Authorization
- Users can only **update/delete their own animals**
- JWT tokens extracted from `ClaimTypes.NameIdentifier`
- Authorization checks performed at handler level

### Error Handling
- **GlobalExceptionHandler** middleware catches all exceptions
- Returns appropriate HTTP status codes:
  - `400 Bad Request` - Validation errors
  - `403 Forbidden` - Unauthorized access
  - `404 Not Found` - Resource not found
  - `500 Internal Server Error` - Server errors

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

## 🎯 Key Implementations

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
- **Transaction Support**: `BeginTransactionAsync`, `CommitTransactionAsync`, `RollbackTransactionAsync`

### Repository Pattern
- **Generic Repository**: Base `Repository<TEntity>` with common CRUD operations
- **Specialized Repositories**: 
  - `AnimalRepository` - `GetAvailableAnimalsForAdoption()`
  - `RequestRepository` - `GetRequestWithAnimalAsync()`, `GetPendingRequestsForAnimalAsync()`, `RemoveRange()`
- **Unit of Work Integration**: All repositories share the same `DbContext` for transaction consistency

### SignalR & Notifications
- **Real-time Chat**: `ChatHub` for user-to-user messaging
- **Notification Service**: 
  - `SendNotificationToUserAsync()` - Single user notifications
  - `SendNotificationAsync()` - Multiple users (broadcast)
- **User Identification**: Custom `IUserIdProvider` extracts user ID from JWT claims
- **Infrastructure Layer**: SignalR hubs and services located in Infrastructure for proper dependency flow

## 🧪 Testing

Unit tests located in `CleanArc.Testing` project:
- Handler tests using NSubstitute
- FluentAssertions for assertions

## 📝 Development Guidelines

### Adding New Features
1. Define domain entity in `CleanArc.Core/Entities`
2. Create command/query in `CleanArc.Presentation/Commands` or `Queries`
3. Implement handler in `CleanArc.Presentation/Handlers`
4. Add FluentValidation rules
5. Create controller endpoint
6. Add EF Core configuration if needed

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
