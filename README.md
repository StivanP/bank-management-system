# Bank Management System

ASP.NET Core Web API for managing branches, bank accounts, and users (customers, employees, managers) with role-based access control and permission mappings.

---

## 🛠️ Tech Stack

- **C# / .NET 10**
- **ASP.NET Core Web API**
- **Entity Framework Core** + SQL Server
- **JWT Authentication**
- **FluentValidation**
- **BCrypt** (password hashing)

---

## 📌 Overview

BankManagementSystem is a backend REST API built around a banking domain model. It provides CRUD operations for core entities and dedicated endpoints for many-to-many relationships such as customer↔account access, employee↔branch assignments, and employee↔account permissions.

Key architectural decisions:
- Dependency Injection throughout — no manual `new DbContext()` or `new Service()`
- Generic `BaseCrudController<TEntity, ...>` and `BaseService<T>` eliminate boilerplate across all resources
- Passwords hashed with BCrypt — never stored as plain text
- Configuration-driven — connection string and JWT secret live in `appsettings.json`, not in source code

---

## ✨ Features

**Authentication & Authorization**
- JWT token-based authentication via `POST /api/auth/login`
- Role-based authorization: `Manager`, `Employee`, `Customer`
- Registration endpoint for new customers
- Role promotion/demotion by Manager (`POST /api/auth/set-role`)

**CRUD Resources**
- `Branches` — Manager only
- `Accounts` — Employee/Manager; Employee access gated by per-account permissions
- `Customers` — Customers see only their own profile; management by Employee/Manager
- `Employees` — Employees see only their own profile; management by Manager
- `Managers` — Manager only

**Junction Resources (composite keys)**
- `CustomerAccounts` — customer access to account with role: `PRIMARY` / `JOINT`
- `EmployeeBranches` — employee assignment to branch with position and start date
- `ManagerBranches` — manager assignment to branch
- `EmployeeAccountPermissions` — per-account permissions for employees: `READ` / `WRITE`

**Search Endpoints**
Every resource exposes `POST /api/<resource>/get` with:
- `Filter` — resource-specific fields (IBAN, date ranges, balance range, etc.)
- `Pager` — `Page`, `PageSize`
- `OrderBy` + `SortAsc`

---

## 🔐 Access Rules

| Role | Access |
|------|--------|
| Manager | Full access to all resources, role management, permission management |
| Employee | Manage accounts (READ permission required for GET, WRITE for PUT/DELETE); own profile only |
| Customer | Own profile only; own CustomerAccounts only |

Default seeded Manager account:
- Email: `stivanp3@gmail.com`
- Password: `admin123`

---

## ▶️ How to Run

**Prerequisites:** .NET 10 SDK, SQL Server (local or Express)

1. Clone the repository
2. Set your connection string in `API/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "Default": "Server=YOUR_SERVER;Database=BankDb;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "BankAPI",
    "Audience": "BankClients",
    "ExpiresMinutes": 60
  }
}
```
3. Apply migrations:
```bash
dotnet tool install --global dotnet-ef
dotnet ef database update --project Common --startup-project API
```
4. Run the API:
```bash
dotnet run --project API
```
5. Open Swagger at `https://localhost:{port}/swagger`, authenticate via `POST /api/auth/login`, and use the returned `Bearer <token>` for protected endpoints.

---

## 📁 Project Structure
```
BankManagementSystem/
├── API/
│   ├── Controllers/
│   └── DTOs/
│       ├── RequestDTOs/
│       └── ResponseDTOs/
│   ├── Services/
├── Common/
│   ├── Entities/
│   ├── Persistence/
│   ├── Services/
│   └── Migrations/
└── README.md
```
---

## 👤 Author

**Stivan Pashaliev** — [linkedin.com/in/stivan-pashaliev](https://linkedin.com/in/stivan-pashaliev) · [github.com/stivanpashaliev](https://github.com/stivanpashaliev)
