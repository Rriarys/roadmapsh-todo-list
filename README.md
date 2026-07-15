# Todo List API

An educational REST API for managing user-owned todo items. The project was created as part of the [roadmap.sh Todo List API project](https://roadmap.sh/projects/todo-list-api).

## Features

- User registration and login
- JWT authentication
- Management of user-owned todo items
- Filtering todos by title
- Sorting todos by title
- Pagination
- Request validation
- RFC 7807 `ProblemDetails` error responses
- Integration tests with the EF Core In-Memory database

## Technologies

- .NET 10
- ASP.NET Core
- Entity Framework Core 10
- SQLite
- JWT Bearer Authentication
- xUnit
- `Microsoft.AspNetCore.Mvc.Testing`

## Setup and Run

Install the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```bash
git clone https://github.com/Rriarys/roadmapsh-todo-list.git
cd roadmapsh-todo-list
dotnet restore
dotnet run --project ToDoList.Api
```

The application uses the following SQLite database by default:

```text
ToDoList.Api/Data/todolistdb.db
```

Database migrations are applied automatically when the application starts, except in the `Testing` environment.

Run the tests with:

```bash
dotnet test
```

## Configuration

The main configuration is located in `ToDoList.Api/appsettings.json`:

- `ConnectionStrings:SQLiteConnection` — SQLite connection string
- `JwtSettings:SecretKey` — JWT signing key
- `JwtSettings:Issuer` — JWT issuer
- `JwtSettings:Audience` — JWT audience
- `JwtSettings:ExpirationMinutes` — token lifetime in minutes

The JWT secret key should not be stored in a public repository. For local development, settings can be overridden using `appsettings.Development.json`, environment variables, or .NET User Secrets.

## Authentication

1. Register a user with `POST /api/auth/register`
2. Read the JWT token from the `token` property
3. Send the token in the `Authorization` header:

```http
Authorization: Bearer <token>
```

All `/api/todos` endpoints require authentication.

## API Endpoints

| Method | Endpoint | Description | Authentication |
|---|---|---|---|
| POST | `/api/auth/register` | Register a new user | No |
| POST | `/api/auth/login` | Authenticate an existing user | No |
| GET | `/api/todos` | Get a paginated todo list | Yes |
| GET | `/api/todos/{id}` | Get a todo item by ID | Yes |
| POST | `/api/todos` | Create a todo item | Yes |
| PUT | `/api/todos/{id}` | Update a todo item | Yes |
| DELETE | `/api/todos/{id}` | Delete a todo item | Yes |

## API Usage Examples

### Register

```http
POST /api/auth/register
Content-Type: application/json

{
  "name": "Test User",
  "email": "user@example.com",
  "password": "Password123"
}
```

Response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

### Login

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "Password123"
}
```

Response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

### Create a Todo

```http
POST /api/todos
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Complete documentation",
  "description": "Update the project README"
}
```

Response:

```json
{
  "id": "7c3f2c3f-5f7e-4a3b-8b22-8e8f2f9a3f4e",
  "title": "Complete documentation",
  "description": "Update the project README"
}
```

### Get Todos

```http
GET /api/todos?page=1&pageSize=10
Authorization: Bearer <token>
```

Filtering and sorting example:

```http
GET /api/todos?title=Task&sortBy=title&sortDescending=false&page=1&pageSize=10
Authorization: Bearer <token>
```

Response:

```json
{
  "items": [
    {
      "id": "7c3f2c3f-5f7e-4a3b-8b22-8e8f2f9a3f4e",
      "title": "Complete documentation",
      "description": "Update the project README"
    }
  ],
  "totalCount": 1,
  "page": 1,
  "pageSize": 10
}
```

### Update a Todo

```http
PUT /api/todos/7c3f2c3f-5f7e-4a3b-8b22-8e8f2f9a3f4e
Authorization: Bearer <token>
Content-Type: application/json

{
  "title": "Documentation completed",
  "description": "The README has been updated"
}
```

### Delete a Todo

```http
DELETE /api/todos/7c3f2c3f-5f7e-4a3b-8b22-8e8f2f9a3f4e
Authorization: Bearer <token>
```

A successful deletion returns `204 No Content`.

## Error Responses

API errors use the `ProblemDetails` format. Common status codes include:

- `400 Bad Request` — invalid request data
- `401 Unauthorized` — missing or invalid JWT token
- `404 Not Found` — todo item was not found or belongs to another user
- `409 Conflict` — a user with the specified email already exists
- `429 Too Many Requests` — authentication request limit exceeded