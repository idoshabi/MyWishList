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

## Run

1. Restore and build:
   - `dotnet restore`
   - `dotnet build`
2. Start app:
   - `dotnet run`
3. Open:
   - `http://localhost:5000` or `https://localhost:5001` (per launch settings)

The app currently uses `Database.EnsureCreated()` at startup to create tables automatically.