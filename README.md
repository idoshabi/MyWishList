# MyWishList

ASP.NET Core MVC wishlist app  using SQL Server.

Link mywishlist-68210.azurewebsites.net
Swagger mywishlist-68210.azurewebsites.net/swagger/index.html
## Features

- User registration and login
- User profile fields in DB:
  - username
  - email
  - first name
  - last name
  - password (hashed)
  - date of birth
- Each user can create multiple wishlists
- Each wishlist can contain multiple items
- Item fields:
  - product name
  - link
  - merchant
  - type
  - wishlist id

## Tech Stack

- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server (LocalDB default connection)
- Cookie authentication

## Run

```bash
dotnet run
```

Default HTTP URL is printed by Kestrel (see `launchSettings.json`). Swagger UI: `/swagger`.

### Queue (add-item requests)

- If `StorageQueue:ConnectionString` is set, the app uses Azure Storage Queues and processes messages in-process via `AddItemQueueWebJob`.
- If it is empty, an in-memory queue is used for local development (same background worker).

## Build

```bash
dotnet build
```
