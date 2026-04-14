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

## Run (Backend + React Frontend)

1. Start backend API + MVC host:
   - `dotnet run`
   - default HTTP URL: `http://localhost:5167`
2. In a second terminal, start React UI:
   - `cd frontend`
   - `npm install`
   - `npm run dev`
3. Open the React app:
   - `http://localhost:5173`

The React dev server proxies `/api/*` requests to `http://localhost:5167`.

## Build

- Backend: `dotnet build`
- Frontend: `cd frontend && npm run build`