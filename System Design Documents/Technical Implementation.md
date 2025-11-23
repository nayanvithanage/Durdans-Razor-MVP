# Technical Implementation - Durdans Razor MVP

## 1. Architecture Overview

The system follows **Clean Architecture** principles adapted for ASP.NET Core Razor Pages, ensuring separation of concerns, testability, and maintainability.

### Layers
1.  **Presentation Layer (UI)**: Razor Pages (`.cshtml` + `.cshtml.cs` PageModels)
2.  **Business Logic Layer (BLL)**: Service classes with interfaces
3.  **Data Access Layer (DAL)**: Repository pattern with EF Core
4.  **Database**: Microsoft SQL Server

---

## 2. Technology Stack

| Component | Technology | Reason |
| :--- | :--- | :--- |
| **Framework** | .NET 8.0 (ASP.NET Core) | Modern, cross-platform, high-performance framework |
| **Frontend** | Razor Pages + Bootstrap 5 | Page-focused model, responsive UI |
| **Language** | C# 12 | Latest language features, nullable reference types |
| **Database** | Microsoft SQL Server | Enterprise-grade relational database |
| **ORM** | Entity Framework Core 8 | Code-first migrations, LINQ queries |
| **Dependency Injection** | Built-in .NET Core DI | Inversion of Control, testability |
| **Validation** | Data Annotations + FluentValidation | Client and server-side validation |

---

## 3. Implementation Strategy

### 3.1 Database Design (EF Core Code-First)

*   **Entities**: `Patient`, `Doctor`, `Hospital`, `Appointment`
*   **Relationships**:
    *   Doctor ↔ Hospital: Many-to-Many (via `DoctorHospitals` junction table)
    *   Patient → Appointment: One-to-Many
    *   Doctor → Appointment: One-to-Many
    *   Hospital → Appointment: One-to-Many
*   **Migrations**: Use `dotnet ef migrations add` for schema changes

### 3.2 Data Access Layer (Repository Pattern)

*   **Generic Repository**: `IRepository<T>` with CRUD operations
*   **Specific Repositories**: Extend generic for entity-specific queries
    ```csharp
    public interface IPatientRepository : IRepository<Patient> {
        Task<Patient> GetByPhoneAsync(string phone);
    }
    ```
*   **DbContext**: `ApplicationDbContext` manages all entities

### 3.3 Business Logic Layer (Services)

*   **Service Interfaces**: Define contracts (e.g., `IAppointmentService`)
*   **Service Implementation**: Business rules, validation, orchestration
    ```csharp
    public class AppointmentService : IAppointmentService {
        public async Task<bool> BookAppointmentAsync(AppointmentDto dto) {
            // 1. Validate availability
            // 2. Check for conflicts
            // 3. Start transaction
            // 4. Save appointment
            // 5. Commit
        }
    }
    ```
*   **Dependency Injection**: Register in `Program.cs`
    ```csharp
    builder.Services.AddScoped<IAppointmentService, AppointmentService>();
    ```

### 3.4 Presentation Layer (Razor Pages)

*   **PageModels**: Handle HTTP requests, inject services
*   **Binding**: Use `[BindProperty]` for form data
*   **Validation**: Automatic via `ModelState.IsValid`
*   **Tag Helpers**: `asp-for`, `asp-validation-for` for forms

---

## 4. Security Implementation

*   **Authentication**: ASP.NET Core Identity (future scope)
*   **Authorization**: Role-based policies
*   **Input Validation**: Data Annotations + server-side checks
*   **SQL Injection Prevention**: EF Core parameterized queries
*   **HTTPS**: Enforce SSL in production

---

## 5. Feature Implementation Details

### 5.1 Patient Management
*   **Register**: Form with validation → `PatientService.RegisterAsync()` → `PatientRepository.AddAsync()`
*   **Search**: LINQ query on `DbSet<Patient>` filtered by name/phone
*   **Edit**: Load entity → Bind to form → Update via repository

### 5.2 Doctor Management
*   **Create**: Multi-select hospitals → Map to `Doctor.Hospitals` collection → EF Core handles junction table
*   **Availability**: Store as JSON in `AvailabilityJson` column or separate `DoctorAvailability` table

### 5.3 Hospital Management
*   **CRUD**: Standard repository operations
*   **Dropdown**: Load all hospitals in `OnGet()` for doctor registration

### 5.4 Appointment Booking
*   **Dynamic Dropdowns**: Use AJAX handlers (`OnGetDoctors`, `OnGetSlots`)
*   **Concurrency**: Use `[ConcurrencyCheck]` attribute or row versioning
*   **Transaction**: Wrap booking in `using var transaction = await _context.Database.BeginTransactionAsync()`

---

## 6. Deployment

*   **Server**: IIS or Kestrel (cross-platform)
*   **Configuration**: `appsettings.json` for connection strings
*   **Migrations**: Run `dotnet ef database update` on deployment
*   **Environment**: Use `appsettings.Development.json` and `appsettings.Production.json`
