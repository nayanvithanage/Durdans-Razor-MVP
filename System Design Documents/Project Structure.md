# Project Structure - Durdans Razor MVP

This document outlines the folder structure and organization of the Durdans Hospital Clinic Management System (Razor Pages).

## Root Directory Structure

```
Durdans-Razor-MVP/
├── Data/                          # Database Context and Migrations
│   └── ApplicationDbContext.cs    # EF Core DbContext
├── Models/                        # Domain Models (Entities)
│   ├── Patient.cs
│   ├── Doctor.cs
│   ├── Hospital.cs
│   └── Appointment.cs
├── Pages/                         # Razor Pages (UI Layer)
│   ├── Patients/
│   │   ├── Register.cshtml        # Patient registration form
│   │   ├── Register.cshtml.cs     # Page model for registration
│   │   ├── Index.cshtml           # Patient list/search
│   │   └── Edit.cshtml            # Edit patient details
│   ├── Doctors/
│   │   ├── Create.cshtml          # Doctor registration
│   │   ├── Index.cshtml           # Doctor list
│   │   └── Edit.cshtml            # Edit doctor details
│   ├── Hospitals/
│   │   ├── Create.cshtml          # Hospital registration
│   │   └── Index.cshtml           # Hospital list
│   ├── Appointments/
│   │   ├── Book.cshtml            # Appointment booking
│   │   └── Index.cshtml           # Appointment list
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Master layout
│   │   └── _ValidationScriptsPartial.cshtml
│   ├── Index.cshtml               # Home page
│   └── _ViewImports.cshtml        # Global using statements
├── Repositories/                  # Data Access Layer (DAL)
│   ├── IRepository.cs             # Generic repository interface
│   ├── PatientRepository.cs
│   ├── DoctorRepository.cs
│   ├── HospitalRepository.cs
│   └── AppointmentRepository.cs
├── Services/                      # Business Logic Layer (BLL)
│   ├── IPatientService.cs
│   ├── PatientService.cs
│   ├── IDoctorService.cs
│   ├── DoctorService.cs
│   ├── IAppointmentService.cs
│   └── AppointmentService.cs
├── wwwroot/                       # Static files
│   ├── css/                       # Stylesheets (Bootstrap, custom)
│   ├── js/                        # JavaScript files
│   └── lib/                       # Client-side libraries
├── System Design Documents/       # Design documentation
│   ├── High Level diagram.md
│   ├── Low Level diagram.md
│   ├── Project Flow Explanation.md
│   └── Project Structure.md       # This file
├── appsettings.json               # Configuration (connection strings)
├── appsettings.Development.json   # Development-specific settings
├── Program.cs                     # Application entry point
└── DurdansRazor.csproj            # Project file
```

## Key Directories Explained

### `/Data`
Contains the EF Core `DbContext` class that manages database connections and entity configurations.

### `/Models`
Domain entities representing database tables. These are POCOs (Plain Old CLR Objects) with data annotations for validation.

### `/Pages`
Razor Pages organized by feature. Each `.cshtml` file has a corresponding `.cshtml.cs` code-behind (PageModel).

### `/Repositories`
Implements the Repository pattern for data access. Each repository handles CRUD operations for a specific entity.

### `/Services`
Contains business logic and orchestrates operations between the UI and data layers. Services are injected into PageModels via Dependency Injection.

### `/wwwroot`
Static assets served directly to the browser (CSS, JavaScript, images).

## Naming Conventions

*   **Pages**: PascalCase (e.g., `Register.cshtml`)
*   **Models**: PascalCase singular (e.g., `Patient.cs`)
*   **Repositories**: `{Entity}Repository.cs`
*   **Services**: `{Entity}Service.cs`
*   **Interfaces**: Prefixed with `I` (e.g., `IPatientService.cs`)
