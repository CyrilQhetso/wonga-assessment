A full-stack authentication application built with **React**, **C# .NET 8**, **PostgreSQL**, and **Docker**.
---

## Overview

This project demonstrates a secure, production-aware authentication system. Users can register, log in, and access a protected profile page. The entire application runs inside Docker containers with a single command.

---

## Features

- ✅ User registration and login
- ✅ Password hashing with **BCrypt** (work factor 12)
- ✅ **JWT** authentication & authorisation
- ✅ **Rate limiting** on auth endpoints — 5 requests per minute (brute-force protection)
- ✅ **HTTP Security Headers** — X-Frame-Options, Content-Security-Policy, HSTS, X-Content-Type-Options, and more
- ✅ Protected frontend routes — `/me` is inaccessible without a valid session
- ✅ Auto database migration on startup
- ✅ Unit tests & integration tests
- ✅ Swagger UI (available in development mode at `http://localhost:5000/swagger`)

---

## Architecture

Three Docker containers are managed by Docker Compose:

| Container | Port | Description |
|---|---|---|
| `wonga_frontend` | `3000` | React SPA served via Nginx |
| `wonga_api` | `5000` | C# .NET 8 Web API |
| `wonga_db` | `5432` | PostgreSQL 16 with persistent volume |

---

## Prerequisites

Make sure you have the following installed before running the app:

- [Docker Desktop](https://docs.docker.com/get-started/get-docker/) (includes Docker Compose)
- That's it — Docker handles everything else

---

## How to Run

### Option 1 — Build Script (Recommended)

```bash
# Clone the repository
git clone https://github.com/CyrilQhetso/wonga-assessment.git
cd wonga-assessment

# Make the build script executable and run it
chmod +x build.sh
./build.sh
```

The build script will:
1. Run all backend unit tests
2. Build all Docker images
3. Start all containers

### Option 2 — Docker Compose Directly

```bash
# Clone the repository
git clone https://github.com/YOUR_USERNAME/wonga-assessment.git
cd wonga-assessment

# Start all containers
docker compose up --build
```

### Accessing the Application

Once running, open your browser and go to:

| Service | URL |
|---|---|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5000 |
| Swagger UI | http://localhost:5000/swagger |

### Stopping the Application

```bash
docker compose down
```

To also remove the database volume (wipes all data):

```bash
docker compose down -v
```

---

## API Endpoints

| Method | Endpoint | Auth Required | Description |
|---|---|---|---|
| `POST` | `/api/auth/register` |  No | Register a new user |
| `POST` | `/api/auth/login` |  No | Log in and receive a JWT token |
| `GET` | `/api/auth/me` |  Yes | Get the authenticated user's details |

### Register — Request Body

```json
{
  "firstName": "Cyril",
  "lastName": "Qhetso",
  "email": "cyril@qhetso.com",
  "password": "Password123!"
}
```

### Login — Request Body

```json
{
  "email": "cyril@qhetso.com",
  "password": "Password123!"
}
```

### Login / Register — Response

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "firstName": "Cyril",
    "lastName": "Qhetso",
    "email": "john@example.com",
    "createdAt": "2025-01-01T10:00:00Z"
  }
}
```

### Get User Details — Request Header

```
Authorization: Bearer <your_token_here>
```

---

## Security

This application implements the following security measures:

**Password Security**
- Passwords are hashed using BCrypt with a work factor of 12 — never stored in plain text.

**Authentication**
- JWT tokens are signed with HMAC-SHA256 and expire after 60 minutes.

**Brute-Force Protection**
- Auth endpoints are rate limited to 5 requests per minute per IP. Exceeding this returns HTTP `429 Too Many Requests`.

**HTTP Security Headers**
The following headers are applied to every response via a custom middleware:

| Header | Value | Purpose |
|---|---|---|
| `X-Frame-Options` | `DENY` | Prevents clickjacking |
| `X-Content-Type-Options` | `nosniff` | Prevents MIME sniffing |
| `X-XSS-Protection` | `1; mode=block` | XSS protection for older browsers |
| `Content-Security-Policy` | `default-src 'self'; connect-src 'self' http://localhost:5000` | Restricts resource loading |
| `Strict-Transport-Security` | `max-age=63072000` | Enforces HTTPS |
| `Referrer-Policy` | `strict-origin-when-cross-origin` | Controls referrer info |

**Vague Error Messages**
- Login returns a generic `"Invalid credentials."` message regardless of whether the email exists — this prevents email enumeration attacks.

---

## Running Tests

### Unit Tests Only

```bash
cd backend
dotnet test AuthService.Tests --verbosity normal
```

### All Tests via Docker

```bash
docker compose run --rm backend dotnet test
```

---

## Project Structure

```
wonga-assessment/
├── build.sh                        # Build & run script
├── docker-compose.yml              # Multi-container setup
├── README.md
│
├── backend/
│   ├── Dockerfile
│   ├── Wonga.sln
│   ├── AuthService/
│   │   ├── Controllers/
│   │   │   └── AuthController.cs   # Register, Login, GetMe endpoints
│   │   ├── Data/
│   │   │   └── AppDbContext.cs     # EF Core database context
│   │   ├── Middleware/
│   │   │   └── SecurityHeadersMiddleware.cs
|   |   ├── Migrations/             # Database migrations
│   │   ├── Models/
│   │   │   ├── User.cs
│   │   │   └── DTOs.cs
│   │   ├── Services/
│   │   │   └── AuthService.cs      # Business logic
│   │   └── Program.cs              # App configuration & middleware pipeline
│   └── AuthService.Tests/
│       ├── AuthServiceTests.cs     # Unit tests
│       └── IntegrationTests.cs     # Integration tests
│
└── frontend/
    ├── Dockerfile
    ├── nginx.conf
    └── src/
        ├── api/
        │   └── auth.ts             # Axios API calls
        ├── context/
        │   └── AuthContext.tsx     # Global auth state
        ├── components/
        │   └── ProtectedRoute.tsx  # Route guard
        └── pages/
            ├── Register.tsx
            ├── Login.tsx
            └── UserDetails.tsx
```

---

## Environment Variables

These are configured inside `docker-compose.yml` and do not require a `.env` file to run locally.

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `Jwt__Secret` | Secret key used to sign JWT tokens (min 32 characters) |
| `Jwt__Issuer` | JWT issuer name |
| `Jwt__Audience` | JWT audience name |
| `Jwt__ExpiryMinutes` | Token expiry duration in minutes |

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | React 18 + TypeScript |
| Backend | C# .NET 10.0 Web API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core |
| Auth | JWT Bearer + BCrypt.Net |
| Containerisation | Docker + Docker Compose |
| Web Server | Nginx (frontend) |
| Testing | xUnit + Moq + FluentAssertions |
