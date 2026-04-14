# 🎓 ARAK School Management System - Backend

> **Enterprise-grade REST API for Educational Management**
> A comprehensive backend built with ASP.NET Core 9, featuring Role-Based Access Control (RBAC), JWT authentication, and full CRUD operations for school management.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2019+-CC2927?style=flat-square&logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server)
[![Entity Framework](https://img.shields.io/badge/EF_Core-9.0-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/en-us/ef/core/)
[![Status](https://img.shields.io/badge/Status-Production_Ready-green?style=flat-square)]()

---

## 📑 Table of Contents

- [🚀 Quick Start](#-quick-start)
- [✨ Features](#-features)
- [🛠️ Technology Stack](#%EF%B8%8F-technology-stack)
- [📐 Architecture](#-architecture)
- [🔐 Authentication & Authorization](#-authentication--authorization)
- [📦 Installation](#-installation)
- [🗄️ Database Setup](#%EF%B8%8F-database-setup)
- [📡 API Endpoints](#-api-endpoints)
- [🧪 Testing](#-testing)
- [📁 Project Structure](#-project-structure)
- [🔧 Configuration](#-configuration)
- [🤝 Contributing](#-contributing)
- [📝 License](#-license)
- [📞 Support](#-support)

---

## 🚀 Quick Start

### Prerequisites
- **.NET 9 SDK** ([Download](https://dotnet.microsoft.com/download/dotnet/9.0))
- **SQL Server** 2019+ (Express, Developer, or Standard edition)
- **Git**
- **EF Core CLI Tool** - Install globally: `dotnet tool install --global dotnet-ef`

### 1-Minute Setup

```bash
# 1. Clone the repository
git clone https://github.com/AhmedsaadyAS/arak-backend.git
cd arak-backend

# 2. Configure database connection
# Edit Arak.PLL/appsettings.json and update the connection string

# 3. Apply database migrations (creates database & seed data)
dotnet ef database update --project Arak.DAL --startup-project Arak.PLL

# 4. Run the application
dotnet run --project Arak.PLL
```

**The API will be available at:**
- **API**: http://localhost:5000/api
- **Swagger UI**: http://localhost:5000/swagger

### 🔑 Default Login Credentials

| Role | Email | Password | Access Level |
|------|-------|----------|--------------|
| **Super Admin** | `admin@arak.com` | `Admin@123` | Full system access |
| **Academic Admin** | `academic@arak.com` | `Academic@123` | Grades, schedules, tasks |
| **Teacher** | `teacher1@arak.com` | `Teacher@123` | Own classes only |
| **Parent** | `parent1@arak.com` | `Parent@123` | View children's data |

> ⚠️ **Change these passwords in production!** Use environment variables: `ARAK_DEFAULT_PASSWORD`

---

## ✨ Features

### 🔐 Security & Access Control
- **ASP.NET Core Identity** for user management and password hashing
- **JWT Bearer Authentication** with 24-hour token expiry
- **Role-Based Access Control (RBAC)** with 7 distinct roles
- **CORS Policy** configured for frontend origins

### 📊 Comprehensive Management Modules
- **Student Management**: CRUD with pagination, search, and filtering
- **Teacher Management**: Profiles with subject assignments and class tracking
- **Parent Management**: Link to multiple students
- **Class Management**: Grade levels, stages, and student capacity tracking
- **Subject Management**: Course catalog with academic resources
- **Schedule/Timetable**: Weekly timetable with conflict detection
- **Evaluation System**: Grade tracking with Egyptian grading system support
- **Task/Homework Tracker**: Assignment management with status tracking
- **Event Calendar**: School-wide events, holidays, and exams
- **Fees Management**: Financial records and payment tracking
- **Attendance System**: Student attendance records (stub - in development)
- **User Management**: Admin account creation and role assignment

### 🛡️ Data Integrity & Security
- **Cascade Delete Prevention**: All foreign keys use `DeleteBehavior.Restrict`
- **Dependency Validation**: Cannot delete records with active references
- **Atomic Operations**: Schedule creation auto-syncs teacher assignments
- **Grade Locking**: Prevents modifications to locked class grades
- **DTO Pattern**: Clean separation between database entities and API contracts

---

## 🛠️ Technology Stack

| Technology | Version | Purpose |
|------------|---------|---------|
| **ASP.NET Core** | 9.0 | Web API framework |
| **Entity Framework Core** | 9.0.14 | Object-Relational Mapping |
| **SQL Server** | 2019+ | Relational database |
| **ASP.NET Core Identity** | Built-in | User management & password hashing |
| **JWT Bearer** | Custom | Token-based authentication |
| **Swagger/Swashbuckle** | Latest | API documentation |
| **Repository Pattern** | Custom | Data access abstraction |
| **Dependency Injection** | Built-in | Service container |

---

## 📐 Architecture

### Clean Architecture (3-Tier)

```
┌─────────────────────────────────────────┐
│     Arak.PLL (Presentation Layer)       │
│  Controllers │ JWT Auth │ Middleware    │
│  Port: 5000  │  Swagger  │  CORS       │
├─────────────────────────────────────────┤
│     Arak.BLL (Business Logic Layer)     │
│  Services │ DTOs │ Interfaces           │
├─────────────────────────────────────────┤
│     Arak.DAL (Data Access Layer)        │
│  DbContext │ Entities │ Repositories    │
│  Migrations │ DbInitializer             │
├─────────────────────────────────────────┤
│        SQL Server Database (ArakDB)     │
│  Tables │ Relations │ Indexes           │
└─────────────────────────────────────────┘
```

### Dependency Flow
```
Arak.PLL → Arak.BLL → Arak.DAL → SQL Server
```

### Design Patterns
- **Repository Pattern**: Generic repository for CRUD operations + specialized repositories
- **Dependency Injection**: All services and repositories registered in DI container
- **DTO Pattern**: Data Transfer Objects for clean API contracts
- **Unit of Work**: EF Core DbContext manages transactions

---

## 🔐 Authentication & Authorization

### JWT Configuration

| Setting | Value | Description |
|---------|-------|-------------|
| **Algorithm** | HMAC-SHA256 | Symmetric key signing |
| **Token Expiry** | 24 hours | Session duration |
| **Issuer** | `ArakAPI` | Token issuer claim |
| **Audience** | `ArakDashboard` | Token audience claim |

### Role Hierarchy

| Role | Description | Key Permissions |
|------|-------------|-----------------|
| **Super Admin** | Full system access | All endpoints, all operations |
| **Admin** | School administrator | Students, teachers, classes, fees |
| **Academic Admin** | Academic coordinator | Grades, schedules, tasks, events |
| **Fees Admin** | Financial officer | Fees module, financial reports |
| **Users Admin** | User management | Students, teachers, parents CRUD |
| **Teacher** | Teaching staff | Own classes, grade upload, tasks |
| **Parent** | Parent/Guardian | View children's grades, schedule |

### Authorization Enforcement

Controllers use `[Authorize]` attributes with role-based policies:
```csharp
[Authorize(Roles = "Super Admin, Admin, Academic Admin")]
public class StudentsController : ControllerBase
```

> ⚠️ **Important**: The `role` claim in JWT tokens must be a **string name** (e.g., `"Super Admin"`), not an ID. The frontend relies on exact role string matching.

---

## 📦 Installation

### Step 1: Clone Repository

```bash
git clone https://github.com/AhmedsaadyAS/arak-backend.git
cd arak-backend
```

### Step 2: Configure Database Connection

Edit `Arak.PLL/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ArakDB;Integrated Security=True;TrustServerCertificate=True;"
  }
}
```

**Connection String Options:**
- **Local SQL Server**: `Server=.;Database=ArakDB;Integrated Security=True;TrustServerCertificate=True;`
- **Named Instance**: `Server=.\SQLEXPRESS;Database=ArakDB;...`
- **Remote Server**: `Server=192.168.1.100;Database=ArakDB;User Id=sa;Password=yourpassword;...`

### Step 3: Configure JWT Secret (Production)

For production, update the JWT key in `appsettings.json` or use environment variable:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKey_Minimum32Characters_RandomString!",
    "Issuer": "ArakAPI",
    "Audience": "ArakDashboard",
    "ExpirationHours": 24
  }
}
```

> 🔒 **Security**: The key must be at least 32 characters. Use a cryptographically secure random string generator.

### Step 4: Apply Database Migrations

```bash
# From project root directory
dotnet ef database update --project Arak.DAL --startup-project Arak.PLL
```

This will:
1. Create the `ArakDB` database
2. Run all 23 migrations
3. Execute `DbInitializer` to seed:
   - 7 roles (Super Admin, Admin, etc.)
   - Default admin users
   - Sample classes, subjects, students
   - Gender lookup table

### Step 5: Run the Application

```bash
dotnet run --project Arak.PLL
```

The backend will start on **http://localhost:5000**

### Default Admin Users

After running migrations, these accounts are automatically created:

| Role | Email | Password |
|------|-------|----------|
| **Super Admin** | `admin@arak.com` | `Admin@123` |
| **Academic Admin** | `academic@arak.com` | `Academic@123` |
| **Teacher** | `teacher1@arak.com` | `Teacher@123` |
| **Parent** | `parent1@arak.com` | `Parent@123` |

> ⚠️ **Change these in production!** Use environment variables: `ARAK_DEFAULT_PASSWORD`, `ARAK_ADMIN_PASSWORD`.

### Step 6: Verify Installation

- **API Health**: http://localhost:5000/api/metrics
- **Swagger Documentation**: http://localhost:5000/swagger
- **Test Login**: Use Postman or curl to test `POST /api/auth/login`

---

## 🗄️ Database Setup

### Entity Relationship Overview

```
ApplicationUser (Identity)
    ├── Teacher (1:1) ──→ Subject
    │       └── TeacherClass (M:N) ──→ Class
    ├── Parent (1:1) ──→ Student (1:N)
    └── Admin (1:1)

Class ──→ Student (1:N)
    ├── TimeTable (Schedule)
    ├── Evaluation (Grades)
    └── Assignment (Tasks)

Subject ──→ TimeTable
    └── Semester ──→ Teacher (M:N)
```

### Key Database Rules

1. **DeleteBehavior.Restrict** on ALL foreign keys - prevents accidental data loss
2. **Cannot delete** Teacher with active Schedule records
3. **Cannot delete** Student with Evaluation/Attendance records
4. **Cannot delete** Class with enrolled Students
5. **gradesLocked** (bool) on Class → prevents grade mutations when true
6. **MaxStudents** (int, default 30) on Class → student capacity limit
7. **All IDs are `int` primitive** - never GUID or string

### Entity Models (21 total)

| Entity | Description | Key Fields |
|--------|-------------|------------|
| **ApplicationUser** | ASP.NET Identity user | Id, Name, Email, PhoneNumber, Address |
| **Student** | Student records | Id, StudentCode, Name, Age, Grade, Status |
| **Teacher** | Teacher profiles | TeacherId, SubjectId, Experience, Department |
| **Parent** | Parent/Guardian info | ParentId, linked to ApplicationUser |
| **Class** | Academic classes | Id, Name, Grade, Stage, GradesLocked, MaxStudents |
| **Subject** | Course subjects | Id, Name, Code, BookUrl |
| **Semester** | Academic terms | Id, Name, AcademicYear, IsActive |
| **TimeTable** | Schedule lessons | Id, DayOfWeek, StartTime, EndTime, Room |
| **Attendance** | Attendance records | Id, Status, Date, ArrivalTime |
| **Evaluation** | Student grades | Id, Marks, MaxMarks, AssessmentType |
| **Assignment** | Homework/tasks | Id, Title, DeadLine, State |
| **ArakEvent** | School events | Id, Title, Type, Date |
| **Fee** | Financial records | Id, Amount, Status, DueDate |
| **Admin** | Admin user link | AdminId, ApplicationUser |
| **TeacherClass** | M:N junction | TeacherId, ClassId |
| **TeacherSemester** | M:N junction | TeacherId, SemesterId |
| **StudentSubject** | M:N junction | StudentId, SubjectId |
| **StudentAttendance** | M:N junction | StudentId, AttendanceId |
| **Grade** | Grade levels | Id, Name, Description |
| **GradeLevel** | Grade level config | Id |
| **Gender** | Lookup table | Id, Name (Male/Female) |

### Running Migrations

```bash
# Create a new migration after model changes
cd Arak.DAL
dotnet ef migrations add YourMigrationName

# Apply migrations to database
cd ..
dotnet ef database update --project Arak.DAL --startup-project Arak.PLL

# Remove last migration (if not yet applied to database)
dotnet ef migrations remove

# Generate SQL script for production deployment
dotnet ef migrations script --output migrations.sql
```

---

## 📡 API Endpoints

### Base URL
- **Development**: `http://localhost:5000/api`
- **Production**: `https://api.arak.school/api`

### Authentication

| Method | Endpoint | Description | Auth Required |
|--------|----------|-------------|:-------------:|
| `POST` | `/auth/login` | Authenticate user, return JWT | ❌ Public |
| `POST` | `/auth/logout` | Logout (client-side token clear) | ✅ |
| `GET` | `/auth/me` | Get current user from JWT | ✅ |

### Core Resources

| Module | Endpoints | Auth Required |
|--------|-----------|:-------------:|
| **Students** | `GET/POST /students` · `GET/PUT/DELETE /students/{id}` | ✅ Admin+ |
| **Teachers** | `GET/POST /teachers` · `GET/PUT/PATCH/DELETE /teachers/{id}` | ✅ Admin+ |
| **Parents** | `GET/POST /parents` · `GET/PATCH/DELETE /parents/{id}` | ✅ |
| **Users** | `GET/POST /users` · `GET/PATCH/DELETE /users/{id}` | ✅ Super/Admin |
| **Classes** | `GET/POST /classes` · `GET/PATCH/DELETE /classes/{id}` | ✅ Admin+ |
| **Subjects** | `GET/POST /subjects` · `GET/PUT/DELETE /subjects/{id}` | ✅ Admin+ |
| **Roles** | `GET /roles` | ✅ Any authenticated |
| **Schedules** | `GET/POST /schedules` · `PUT/DELETE /schedules/{id}` | ✅ Admin+ |
| **Evaluations** | `GET/POST /evaluations` · `DELETE /evaluations/{id}` | ✅ Admin/Teacher |
| **Tasks** | `GET/POST /tasks` · `GET/PATCH/DELETE /tasks/{id}` | ✅ Admin/Teacher |
| **Events** | `GET/POST /events` · `PUT/DELETE /events/{id}` | ✅ Admin+ |
| **Attendance** | `GET /attendance` | ✅ |
| **Fees** | `GET/POST /fees` · `GET/PUT/DELETE /fees/{id}` | ✅ Admin/Fees |
| **Metrics** | `GET /metrics` | ✅ |

### Response Formats

**Successful GET (Paginated):**
```json
{
  "data": [...],
  "items": 144,
  "page": 1,
  "pageSize": 10
}
```

**Successful GET (List):**
```json
[
  { "id": 1, "name": "Grade 4-A", "grade": "Grade 4", "stage": "primary" }
]
```

**Successful POST (201 Created):**
```json
{
  "id": 15,
  "name": "New Student",
  "email": "student@arak.edu.eg"
}
```

**Error Response (ProblemDetails):**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Validation failed",
  "errors": {
    "Email": ["The Email field is required."]
  }
}
```

### HTTP Status Codes

| Code | Meaning | Example |
|------|---------|---------|
| `200 OK` | Successful GET/PUT/PATCH/DELETE | Resource retrieved/updated |
| `201 Created` | Successful POST | New resource created |
| `400 Bad Request` | Validation failed | Missing required fields |
| `401 Unauthorized` | Missing or invalid JWT | Token expired/missing |
| `403 Forbidden` | Insufficient role/permission | User role not allowed |
| `404 Not Found` | Resource doesn't exist | ID not in database |
| `409 Conflict` | Delete blocked by dependencies | Cannot delete teacher with classes |
| `500 Internal Server Error` | Unexpected server error | Database connection failed |

### Query Parameters

**Pagination (Students):**
```
GET /students?_page=1&_limit=10&q=Ahmed&grade=Grade 4-A&status=Active
```

**Filtering:**
```
GET /evaluations?classId=1&subjectId=2&assessmentType=Month1
GET /schedules?classId=3&teacherId=5
GET /tasks?teacherId=2&status=Pending
```

> 📖 **Interactive Documentation**: Visit http://localhost:5000/swagger for full endpoint details and test requests directly from the browser.

---

## 🧪 Testing

### Run Backend Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test --verbosity normal

# Run specific test project
dotnet test Arak.BLL.Tests/Arak.BLL.Tests.csproj
```

### Manual Testing with Swagger

1. Start the application: `dotnet run --project Arak.PLL`
2. Open browser: http://localhost:5000/swagger
3. Click on any endpoint to expand
4. Click "Try it out"
5. Fill in parameters and click "Execute"

### Testing Authentication

```bash
# Login
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@arak.com","password":"Admin@123"}'

# Use token to access protected endpoint
curl http://localhost:5000/api/students \
  -H "Authorization: Bearer YOUR_JWT_TOKEN_HERE"
```

---

## 📁 Project Structure

```
arak-backend/
├── Arak.DAL/                          # Data Access Layer
│   ├── Database/
│   │   ├── AppDbContext.cs            # EF Core DbContext
│   │   └── DbInitializer.cs           # Database seeding logic
│   ├── Entities/                      # EF Core entity models
│   │   ├── ApplicationUser.cs         # Identity user
│   │   ├── Student.cs                 # Student entity
│   │   ├── Teacher.cs                 # Teacher entity
│   │   ├── Class.cs                   # Class entity
│   │   ├── Subject.cs                 # Subject entity
│   │   ├── Evaluation.cs              # Grade entity
│   │   ├── TimeTable.cs               # Schedule entity
│   │   ├── Assignment.cs              # Task entity
│   │   ├── Attendance.cs              # Attendance entity
│   │   ├── ArakEvent.cs               # Event entity
│   │   ├── Fee.cs                     # Fee entity
│   │   └── ...                        # Junction tables & lookups
│   ├── Migrations/                    # EF Core migrations (23 total)
│   └── Repository/
│       ├── Abstraction/               # Repository interfaces
│       │   ├── IGenericRepository.cs
│       │   ├── IStudentRepository.cs
│       │   └── ITimetableRepository.cs
│       └── Implementation/            # Repository implementations
│           ├── GenericRepository.cs
│           ├── StudentRepository.cs
│           └── TimetableRepository.cs
│
├── Arak.BLL/                          # Business Logic Layer
│   ├── DTOs/                          # Data Transfer Objects
│   │   ├── StudentDto.cs
│   │   ├── TeacherDto.cs
│   │   ├── ClassDto.cs
│   │   ├── ParentDto.cs
│   │   ├── UserDto.cs
│   │   └── PagedResult.cs
│   └── Service/
│       ├── Abstraction/               # Service interfaces
│       │   ├── IAuthService.cs
│       │   ├── IStudentService.cs
│       │   ├── ITeacherService.cs
│       │   ├── IClassService.cs
│       │   ├── IEvaluationService.cs
│       │   └── ...
│       └── Implementation/            # Service implementations
│           ├── AuthService.cs
│           ├── StudentService.cs
│           ├── TeacherService.cs
│           └── ...
│
├── Arak.PLL/                          # Presentation Layer
│   ├── Controllers/                   # API controllers (16 total)
│   │   ├── AuthController.cs          # /api/auth
│   │   ├── StudentsController.cs      # /api/students
│   │   ├── TeachersController.cs      # /api/teachers
│   │   ├── ClassesController.cs       # /api/classes
│   │   ├── EvaluationsController.cs   # /api/evaluations
│   │   ├── SchedulesController.cs     # /api/schedules
│   │   ├── TimetableController.cs     # /api/timetable
│   │   ├── UsersController.cs         # /api/users
│   │   ├── RolesController.cs         # /api/roles
│   │   ├── ParentsController.cs       # /api/parents
│   │   ├── SubjectsController.cs      # /api/subjects
│   │   ├── TasksController.cs         # /api/tasks
│   │   ├── EventsController.cs        # /api/events
│   │   ├── FeesController.cs          # /api/fees
│   │   ├── AttendanceController.cs    # /api/attendance
│   │   └── MetricsController.cs       # /api/metrics
│   ├── Properties/
│   │   └── launchSettings.json        # Development configuration
│   ├── Program.cs                     # Application entry point
│   ├── appsettings.json               # Configuration (database, JWT, CORS)
│   └── appsettings.Example.json       # Template configuration
│
├── Arak.sln                           # Visual Studio solution file
├── run-migration.ps1                  # PowerShell migration script
├── .gitignore                         # Git ignore rules
├── README.md                          # This file
└── ANALYSIS_REPORT.md                 # Architecture analysis report
```

---

## 🔧 Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=ArakDB;Integrated Security=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "Arak_Development_Only_Key_Change_In_Production_2026",
    "Issuer": "ArakAPI",
    "Audience": "ArakDashboard",
    "ExpirationHours": 24
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5173",
      "http://localhost:5174",
      "https://your-production-domain.com"
    ]
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

## ⚙️ Environment Variables

| Variable | Purpose | Default |
|----------|---------|---------|
| `ARAK_DEFAULT_PASSWORD` | Default password for seeded accounts | `{Role}@123` |
| `ARAK_ADMIN_PASSWORD` | Password for admin@arak.com | `Admin@123` |
| `ARAK_JWT_KEY` | JWT signing key | From appsettings.json |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
| `ConnectionStrings__DefaultConnection` | Database connection string | From appsettings.json |

### launchSettings.json

The application is configured to run on:
- **HTTP**: http://localhost:5000
- **Environment**: Development
- **SSL**: Not enabled in development (add HTTPS configuration for production)

---

## 🔗 Related Repositories

- **Frontend Dashboard**: [arak-admin-dashboard](https://github.com/AhmedsaadyAS/arak-admin-dashboard)

## 🤝 Contributing

### Git Workflow

```bash
# 1. Fork the repository
# 2. Clone your fork
git clone https://github.com/YOUR_USERNAME/arak-backend.git
cd arak-backend

# 3. Create a feature branch
git checkout -b feature/your-feature-name

# 4. Make changes and test
dotnet build
dotnet test

# 5. Commit using Conventional Commits
git add .
git commit -m "feat: add batch grade upload endpoint"

# 6. Push and create Pull Request
git push origin feature/your-feature-name
```

### Commit Message Convention

We use [Conventional Commits](https://www.conventionalcommits.org/):

- `feat:` - New feature
- `fix:` - Bug fix
- `docs:` - Documentation changes
- `style:` - Code style changes (formatting, no logic change)
- `refactor:` - Code refactoring
- `test:` - Test changes
- `chore:` - Build/config changes

### Development Guidelines

1. **Follow Clean Architecture**: Keep layers separated (PLL → BLL → DAL)
2. **Use DTOs**: Never expose entities directly in API responses
3. **Async/Await**: All database operations must be asynchronous
4. **Dependency Injection**: Register all services in Program.cs
5. **Error Handling**: Return proper ProblemDetails for errors
6. **Authorization**: Enforce `[Authorize]` on all controllers
7. **Testing**: Write unit tests for services and integration tests for controllers

### Before Submitting PR

- [ ] Code builds successfully (`dotnet build`)
- [ ] All tests pass (`dotnet test`)
- [ ] No compiler warnings
- [ ] Follows Conventional Commits format
- [ ] Updated documentation if API contracts changed
- [ ] Tested with Swagger UI
- [ ] Added/updated unit tests for new features

---

## 🐛 Known Issues

| Issue | Status | Description |
|-------|--------|-------------|
| Auth service mocked fallback | 🔴 In Progress | Some auth flows use mock data instead of real Identity |
| Grade upload N+1 requests | 🟠 Planned | Control sheet sends 1 request per grade (needs batch endpoint) |
| No JWT refresh token | 🟡 Planned | Hard 24h logout (possible data loss with unsaved forms) |
| Chat feature | 🟡 Not Started | 0% implemented (needs SignalR) |
| Attendance controller | 🟠 Stub | Returns empty list (service not implemented) |

---

## 📝 License

This project is **proprietary and confidential**. Unauthorized copying, distribution, or modification is prohibited without explicit permission from the project owner.

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/AhmedsaadyAS/arak-backend/issues)
- **Frontend Repository**: [arak-admin-dashboard](https://github.com/AhmedsaadyAS/arak-admin-dashboard)
- **API Documentation**: http://localhost:5000/swagger (when running)
- **Email**: [ahmed.saady@example.com](mailto:ahmed.saady@example.com)

---

## 🙏 Acknowledgments

- **Microsoft .NET Team** - Excellent backend framework
- **EF Core Team** - Powerful ORM
- **ASP.NET Core Identity** - Robust authentication

---

<div align="center">

**Built with ❤️ by Ahmed Saady**
© 2026

[⭐ Star this repo](https://github.com/AhmedsaadyAS/arak-backend) | [🐛 Report Bug](https://github.com/AhmedsaadyAS/arak-backend/issues) | [💡 Request Feature](https://github.com/AhmedsaadyAS/arak-backend/pulls)

</div>
