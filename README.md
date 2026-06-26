# Todo List API

A small educational project based on [roadmap.sh/projects/todo-list-api](https://roadmap.sh/projects/todo-list-api).

## Tech Stack

- .NET 10 & ASP.NET Core
- Entity Framework Core 10
- xUnit
- Microsoft.AspNetCore.Mvc.Testing

## API Endpoints

All requests use `application/json`.

| Method | Route | Description |
|---|---|---|
| POST | `/register` |  |
| POST | `/login` |  |
| GET | `/todos` |  |
| GET | `/todos/{id}` |  |
| POST | `/todos` |  |
| PUT | `/todos/{id}` |  |
| DELETE | `/todos/{id}` |  |

## Paginated Response Example

```json
{
  "data": [],
  "page": 1,
  "limit": 10,
  "total": 0
}
```

## Validation Rules

- Name: Required
- Email: Required
- Password: Required
- Title: Required
- Description: Optional
- Page: Positive integer
- Limit: Positive integer

## Getting Started

Run the API:

```bash
dotnet run --project TodoListApi
```

Run tests:

```bash
dotnet test
```

## Manual Testing

Use `TodoListApi/TodoListApi.http` in your IDE to execute requests against the running API.