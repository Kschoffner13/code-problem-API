# Code Problem Fetcher API

Minimal ASP.NET Core API for storing and retrieving coding problems (for example, LeetCode-style problems) using Entity Framework Core and SQL Server.

## Tech Stack

- .NET 9 Minimal APIs
- Entity Framework Core 9
- SQL Server provider (`Microsoft.EntityFrameworkCore.SqlServer`)

## Project Structure

```text
Code-Problem-Fetcher.sln
Code-Problem-Fetcher/
	Program.cs                    # API endpoints and startup
	problemService.cs             # Service abstraction + EF-backed implementation
	Data/
		problem.cs                  # Problem model
		ProblemDbContext.cs         # EF Core DbContext and value conversions
	MyRequests.http               # Ready-to-run HTTP requests
	appsettings.json              # Connection string + app config
```

## Prerequisites

- .NET SDK 9.0+
- A reachable SQL Server / Azure SQL database

## Configuration

The API uses the `DefaultConnection` connection string from:

- `Code-Problem-Fetcher/appsettings.json`

Example format:

```json
"ConnectionStrings": {
	"DefaultConnection": "Server=<server>;Database=<db>;User ID=<user>;Password=<password>;Encrypt=True;TrustServerCertificate=False;"
}
```

Security note: do not commit real credentials to source control. Prefer local secrets (User Secrets, environment variables, or a local untracked config file).

## Run the API

From repository root:

```bash
dotnet restore
dotnet run --project Code-Problem-Fetcher
```

Default local URLs (from launch settings):

- `http://localhost:5186`
- `https://localhost:7074`

On startup, the app calls `EnsureCreated()` to create the database schema if it does not exist.

## API Endpoints

Base URL (local): `http://localhost:5186`

- `GET /test`
  - Health-style check endpoint
  - Returns: `"Hello World!"`

- `GET /Problems`
  - Returns all stored problems

- `GET /Problems/{id}`
  - Returns the problem with the provided ID

- `POST /Problems`
  - Creates a new problem
  - Returns `201 Created` with location `/Problems/{id}` when valid
  - Returns `400 ValidationProblem` when required fields are missing

- `DELETE /Problems/{id}`
  - Deletes problem by ID
  - Returns `204 No Content`

## Problem Payload Schema

```json
{
  "id": "1",
  "title": "Two Sum",
  "description": "Given an array of integers nums and an integer target...",
  "difficulty": "Easy",
  "category": "Array",
  "tags": ["Array", "Hash Table"],
  "link": "https://leetcode.com/problems/two-sum/",
  "input": {
    "nums": [2, 7, 11, 15],
    "target": 9
  },
  "expectedOutput": [0, 1],
  "constraints": {
    "arrayLength": "2 <= nums.length <= 10^4",
    "uniqueSolution": "Only one valid answer exists"
  }
}
```

Required fields validated in `POST /Problems`:

- `id`
- `title`
- `description`
- `difficulty`
- `category`
- `tags` (at least one)
- `input`
- `expectedOutput`
- `constraints` (at least one)

## Try Requests Quickly

Use the included request file:

- `Code-Problem-Fetcher/MyRequests.http`

In VS Code:

1. Open `MyRequests.http`.
2. Start the API.
3. Click "Send Request" above any HTTP request.

## Data Storage Notes

- `Problem` is stored in a single table.
- `Tags` is persisted as a comma-separated string.
- `Input`, `ExpectedOutput`, and `Constraints` are persisted as JSON via EF Core value conversions.

## Current Limitations

- No `PUT`/`PATCH` endpoint is currently exposed, even though service update logic exists.
- No authentication or authorization.
- No paging/filtering yet on `GET /Problems`.

## Troubleshooting

- If startup fails with DB connection errors:
  - Verify `DefaultConnection` points to a reachable SQL Server.
  - Confirm credentials and firewall/network access.

- If HTTPS certificate issues occur in local testing:

```bash
dotnet dev-certs https --trust
```
