# Contracts (DTOs)

This folder contains Data Transfer Objects (DTOs) and contracts used for:
- **Request Models**: Input DTOs for Commands and Queries
- **Response Models**: Output DTOs returned from handlers
- **Common DTOs**: Shared models used across multiple operations

## Recommended Structure

```
Contracts/
├── Requests/          # Request DTOs for Commands
│   ├── Animal/
│   │   ├── CreateAnimalRequest.cs
│   │   ├── UpdateAnimalRequest.cs
│   │   └── DeleteAnimalRequest.cs
│   └── Request/
│       ├── CreateRequestRequest.cs
│       └── UpdateRequestStatusRequest.cs
│
├── Responses/         # Response DTOs
│   ├── Animal/
│   │   ├── AnimalResponse.cs
│   │   └── AnimalListResponse.cs
│   └── Request/
│       └── RequestResponse.cs
│
└── Common/           # Shared DTOs
    ├── PaginationRequest.cs
    └── PaginationResponse.cs
```

## Best Practices

1. **Keep DTOs separate from Domain Entities**: Never expose domain entities directly in API responses
2. **Use AutoMapper or Manual Mapping**: Map between entities and DTOs in handlers
3. **Validation**: Use FluentValidation on request DTOs
4. **Naming Convention**: 
   - Requests: `{Entity}{Action}Request` (e.g., `CreateAnimalRequest`)
   - Responses: `{Entity}Response` (e.g., `AnimalResponse`)

