# MyWishList

ASP.NET Core MVC wishlist app (MyRegistry-style MVP) using SQL Server.

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

## Run (Backend + Queue Worker + React Frontend)

1. Start backend API + MVC host:
   - `dotnet run`
   - default HTTP URL: `http://localhost:5167`
2. If using Azure Storage queues, start the queue worker (WebJob host):
   - `dotnet run --project MyWishList.WebJobs`
3. In another terminal, start React UI:
   - `cd frontend`
   - `npm install`
   - `npm run dev`
4. Open the React app:
   - `http://localhost:5173`

The React dev server proxies `/api/*` requests to `http://localhost:5167`.

### Queue processing behavior

- If `StorageQueue:ConnectionString` is configured, queue messages are intended to be processed by `MyWishList.WebJobs`.
- If `StorageQueue:ConnectionString` is empty, the web app falls back to an in-memory queue worker for local development.

## Build

- Backend: `dotnet build`
- WebJob: `dotnet build MyWishList.WebJobs/MyWishList.WebJobs.csproj`
- Frontend: `cd frontend && npm run build`