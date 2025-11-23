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

The Repository Pattern abstracts data access logic, providing a clean separation between business logic and data persistence. This section explains the pattern from C# language fundamentals.

#### **Language Fundamentals**

##### **1. Interfaces (Contracts)**
An **interface** is a contract that defines what methods a class must implement, but not how. It enables polymorphism and dependency injection.

```csharp
// Interface defines the contract - WHAT operations are available
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);           // Retrieve single entity
    Task<IEnumerable<T>> GetAllAsync();      // Retrieve all entities
    Task AddAsync(T entity);                  // Add new entity
    Task UpdateAsync(T entity);               // Update existing entity
    Task DeleteAsync(int id);                 // Delete entity
}
```

**Key Concepts:**
- `interface` keyword defines a contract (no implementation)
- `<T>` is a **generic type parameter** (explained below)
- `where T : class` is a **generic constraint** (T must be a reference type)
- Methods are abstract by default (no body)
- Classes implementing this interface MUST provide implementations for all methods

##### **2. Generics (`<T>` - Type Parameters)**
**Generics** allow you to write type-safe, reusable code that works with any type without sacrificing compile-time type checking.

```csharp
// Without Generics (BAD - requires separate repository for each entity)
public interface IPatientRepository {
    Task<Patient> GetByIdAsync(int id);
}
public interface IDoctorRepository {
    Task<Doctor> GetByIdAsync(int id);
}
// ... repeat for every entity!

// With Generics (GOOD - one interface for all entities)
public interface IRepository<T> where T : class {
    Task<T?> GetByIdAsync(int id);  // T is a placeholder for any type
}
```

**How Generics Work:**
- `<T>` is a **type parameter** (like a variable, but for types)
- When you use the interface, you specify the actual type:
  - `IRepository<Patient>` → T becomes `Patient`
  - `IRepository<Doctor>` → T becomes `Doctor`
- The compiler generates type-specific code at compile time (type safety!)
- `where T : class` ensures T is a reference type (not a value type like `int`)

##### **3. Interface Inheritance (Extending Contracts)**
Interfaces can inherit from other interfaces, adding more specific requirements.

```csharp
// Base interface (generic operations)
public interface IRepository<T> where T : class {
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    // ... other CRUD operations
}

// Derived interface (adds Patient-specific operations)
public interface IPatientRepository : IRepository<Patient> {
    // Inherits all methods from IRepository<Patient>
    // PLUS adds Patient-specific methods:
    Task<Patient?> GetByPhoneAsync(string phone);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
}
```

**What Happens Here:**
1. `IPatientRepository` **inherits** from `IRepository<Patient>`
2. The generic `<T>` is replaced with concrete type `Patient`
3. Any class implementing `IPatientRepository` must implement:
   - All methods from `IRepository<Patient>` (GetByIdAsync, GetAllAsync, etc.)
   - All methods from `IPatientRepository` (GetByPhoneAsync, SearchByNameAsync)

##### **4. Generic Repository Implementation (Base Class)**
A **generic repository** implements the `IRepository<T>` interface for any entity type.

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();  // Get the DbSet for type T
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
```

**Key Points:**
- `Repository<T>` implements `IRepository<T>` (provides the HOW)
- `protected` members allow derived classes to access them
- `_context.Set<T>()` gets the appropriate `DbSet<T>` for any entity type
- This ONE class works for Patient, Doctor, Hospital, Appointment, etc.

##### **5. Specific Repository Implementation (Derived Class)**
A **specific repository** inherits from the generic repository and adds entity-specific logic.

```csharp
public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context)
    {
        // Calls the base Repository<Patient> constructor
    }

    // Inherited from Repository<Patient>:
    // - GetByIdAsync, GetAllAsync, AddAsync, UpdateAsync, DeleteAsync

    // Patient-specific methods:
    public async Task<Patient?> GetByPhoneAsync(string phone)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.PhoneNumber == phone);
    }

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        return await _dbSet
            .Where(p => p.Name.Contains(name))
            .ToListAsync();
    }
}
```

**Inheritance Chain:**
```
IRepository<Patient>           (interface - contract)
        ↑
        | implements
        |
Repository<Patient>            (base class - generic implementation)
        ↑
        | inherits
        |
PatientRepository              (derived class - specific implementation)
        ↑
        | implements
        |
IPatientRepository             (interface - extended contract)
```

**What PatientRepository Gets:**
1. **From `Repository<Patient>`** (via inheritance):
   - All CRUD operations (GetByIdAsync, AddAsync, etc.)
   - Access to `_context` and `_dbSet` (protected members)
2. **Adds its own**:
   - Patient-specific queries (GetByPhoneAsync, SearchByNameAsync)

#### **Complete Example: All Three Layers**

```csharp
// LAYER 1: Generic Interface (Contract for all entities)
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}

// LAYER 2: Generic Implementation (Works for any entity)
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();
    // ... other implementations
}

// LAYER 3A: Specific Interface (Contract for Patient)
public interface IPatientRepository : IRepository<Patient>
{
    Task<Patient?> GetByPhoneAsync(string phone);
    Task<IEnumerable<Patient>> SearchByNameAsync(string name);
}

// LAYER 3B: Specific Implementation (Patient-specific logic)
public class PatientRepository : Repository<Patient>, IPatientRepository
{
    public PatientRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Patient?> GetByPhoneAsync(string phone)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.PhoneNumber == phone);
    }

    public async Task<IEnumerable<Patient>> SearchByNameAsync(string name)
    {
        return await _dbSet.Where(p => p.Name.Contains(name)).ToListAsync();
    }
}
```

#### **Why This Pattern?**

| Benefit | Explanation |
|:--------|:------------|
| **DRY (Don't Repeat Yourself)** | CRUD operations written once in `Repository<T>`, reused for all entities |
| **Testability** | Services depend on `IPatientRepository` (interface), not concrete class → easy to mock |
| **Flexibility** | Can swap implementations (e.g., switch from EF Core to Dapper) without changing services |
| **Type Safety** | Generics ensure compile-time type checking (no runtime type errors) |
| **Separation of Concerns** | Data access logic isolated from business logic |

#### **Dependency Injection Registration**

```csharp
// Program.cs
builder.Services.AddScoped<IPatientRepository, PatientRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
```

**How DI Works:**
1. Service requests `IPatientRepository` in constructor
2. DI container creates `PatientRepository` instance
3. DI container injects `ApplicationDbContext` into `PatientRepository`
4. Service receives the implementation via the interface

#### **Usage in Services**

```csharp
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepository;

    // Constructor injection - receives interface, not concrete class
    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public async Task<Patient?> FindPatientByPhoneAsync(string phone)
    {
        // Uses Patient-specific method
        return await _patientRepository.GetByPhoneAsync(phone);
    }

    public async Task RegisterPatientAsync(Patient patient)
    {
        // Uses generic CRUD method (inherited from Repository<Patient>)
        await _patientRepository.AddAsync(patient);
    }
}
```

*   **DbContext**: `ApplicationDbContext` manages all entities and is injected into repositories



### 3.3 Business Logic Layer (Services)

The Service Layer contains business logic, orchestrates operations across multiple repositories, enforces business rules, and provides a clean API for the Presentation Layer. This section explains services from C# language fundamentals.

#### **Language Fundamentals**

##### **1. Service Interfaces (Contracts)**
Service interfaces define the **business operations** available to the application, abstracting the implementation details.

```csharp
// Service interface defines business operations (not just CRUD)
public interface IAppointmentService
{
    // Business operations with meaningful names
    Task<bool> BookAppointmentAsync(AppointmentDto dto);
    Task<bool> CancelAppointmentAsync(int appointmentId, string reason);
    Task<bool> RescheduleAppointmentAsync(int appointmentId, DateTime newDateTime);
    Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync(int patientId);
    Task<IEnumerable<TimeSlot>> GetAvailableTimeSlotsAsync(int doctorId, int hospitalId, DateTime date);
    Task<bool> ValidateAppointmentConflictAsync(int doctorId, DateTime appointmentTime);
}
```

**Key Differences from Repository Interfaces:**

| Aspect | Repository Interface | Service Interface |
|:-------|:---------------------|:------------------|
| **Purpose** | Data access (CRUD) | Business logic operations |
| **Methods** | `GetByIdAsync`, `AddAsync`, `UpdateAsync` | `BookAppointmentAsync`, `CancelAppointmentAsync` |
| **Focus** | Single entity | Multiple entities, orchestration |
| **Naming** | Technical (CRUD verbs) | Business domain language |
| **Dependencies** | DbContext only | Multiple repositories, external services |

##### **2. Dependency Injection (DI) - The Foundation**

**Dependency Injection** is a design pattern where a class receives its dependencies from external sources rather than creating them itself.

**Without DI (BAD - Tight Coupling):**
```csharp
public class AppointmentService
{
    private readonly AppointmentRepository _appointmentRepo;
    private readonly PatientRepository _patientRepo;
    
    public AppointmentService()
    {
        // Creating dependencies inside the class (TIGHT COUPLING!)
        var context = new ApplicationDbContext();
        _appointmentRepo = new AppointmentRepository(context);
        _patientRepo = new PatientRepository(context);
    }
}
```

**Problems:**
- Hard to test (can't mock dependencies)
- Violates Single Responsibility Principle (class manages its own dependencies)
- Difficult to swap implementations
- Creates new DbContext instances (not shared)

**With DI (GOOD - Loose Coupling):**
```csharp
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IDoctorRepository _doctorRepo;
    
    // Constructor Injection - dependencies provided from outside
    public AppointmentService(
        IAppointmentRepository appointmentRepo,
        IPatientRepository patientRepo,
        IDoctorRepository doctorRepo)
    {
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
        _doctorRepo = doctorRepo;
    }
}
```

**Benefits:**
- ✅ Testable (can inject mock repositories)
- ✅ Flexible (can swap implementations)
- ✅ Single Responsibility (class focuses on business logic)
- ✅ Shared DbContext across repositories (managed by DI container)

##### **3. Constructor Injection Pattern**

**Constructor Injection** is the recommended DI pattern where dependencies are provided through the constructor.

```csharp
public class AppointmentService : IAppointmentService
{
    // 1. Declare private readonly fields for dependencies
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IDoctorRepository _doctorRepo;
    private readonly IHospitalRepository _hospitalRepo;
    
    // 2. Constructor receives dependencies (injected by DI container)
    public AppointmentService(
        IAppointmentRepository appointmentRepo,
        IPatientRepository patientRepo,
        IDoctorRepository doctorRepo,
        IHospitalRepository hospitalRepo)
    {
        // 3. Assign to fields for use throughout the class
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
        _doctorRepo = doctorRepo;
        _hospitalRepo = hospitalRepo;
    }
    
    // 4. Use dependencies in methods
    public async Task<bool> BookAppointmentAsync(AppointmentDto dto)
    {
        // Use injected repositories
        var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
        var doctor = await _doctorRepo.GetByIdAsync(dto.DoctorId);
        // ... business logic
    }
}
```

**Key Concepts:**
- `readonly` ensures dependencies can't be changed after construction
- Constructor parameters are **interfaces**, not concrete classes
- DI container automatically resolves and injects dependencies
- All dependencies are available throughout the class lifetime

##### **4. Service Implementation - Orchestration & Business Logic**

Services **orchestrate** operations across multiple repositories and enforce **business rules**.

```csharp
public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepo;
    private readonly IPatientRepository _patientRepo;
    private readonly IDoctorRepository _doctorRepo;
    private readonly IHospitalRepository _hospitalRepo;
    private readonly ApplicationDbContext _context;

    public AppointmentService(
        IAppointmentRepository appointmentRepo,
        IPatientRepository patientRepo,
        IDoctorRepository doctorRepo,
        IHospitalRepository hospitalRepo,
        ApplicationDbContext context)
    {
        _appointmentRepo = appointmentRepo;
        _patientRepo = patientRepo;
        _doctorRepo = doctorRepo;
        _hospitalRepo = hospitalRepo;
        _context = context;
    }

    public async Task<bool> BookAppointmentAsync(AppointmentDto dto)
    {
        // STEP 1: Validation - Verify entities exist
        var patient = await _patientRepo.GetByIdAsync(dto.PatientId);
        if (patient == null)
            throw new ArgumentException("Patient not found");

        var doctor = await _doctorRepo.GetByIdAsync(dto.DoctorId);
        if (doctor == null)
            throw new ArgumentException("Doctor not found");

        var hospital = await _hospitalRepo.GetByIdAsync(dto.HospitalId);
        if (hospital == null)
            throw new ArgumentException("Hospital not found");

        // STEP 2: Business Rule - Check if doctor works at this hospital
        if (!doctor.Hospitals.Any(h => h.HospitalId == dto.HospitalId))
            throw new InvalidOperationException("Doctor does not work at this hospital");

        // STEP 3: Business Rule - Check for appointment conflicts
        var hasConflict = await ValidateAppointmentConflictAsync(
            dto.DoctorId, 
            dto.AppointmentDateTime);
        if (hasConflict)
            throw new InvalidOperationException("Time slot already booked");

        // STEP 4: Business Rule - Check doctor availability
        var isAvailable = await CheckDoctorAvailabilityAsync(
            dto.DoctorId, 
            dto.AppointmentDateTime);
        if (!isAvailable)
            throw new InvalidOperationException("Doctor not available at this time");

        // STEP 5: Transaction - Ensure atomicity
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Create appointment entity
            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                HospitalId = dto.HospitalId,
                AppointmentDateTime = dto.AppointmentDateTime,
                Status = AppointmentStatus.Scheduled,
                CreatedAt = DateTime.UtcNow
            };

            // Save to database
            await _appointmentRepo.AddAsync(appointment);

            // Commit transaction
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            // Rollback on error
            await transaction.RollbackAsync();
            throw;
        }
    }

    // Helper method - Business logic encapsulation
    private async Task<bool> ValidateAppointmentConflictAsync(
        int doctorId, 
        DateTime appointmentTime)
    {
        var existingAppointments = await _appointmentRepo
            .GetByDoctorAndDateAsync(doctorId, appointmentTime.Date);

        // Check for appointments within 30 minutes
        return existingAppointments.Any(a => 
            Math.Abs((a.AppointmentDateTime - appointmentTime).TotalMinutes) < 30);
    }

    private async Task<bool> CheckDoctorAvailabilityAsync(
        int doctorId, 
        DateTime appointmentTime)
    {
        var doctor = await _doctorRepo.GetByIdAsync(doctorId);
        // Parse availability JSON and check if time slot is available
        // Implementation depends on how availability is stored
        return true; // Simplified
    }
}
```

**Service Responsibilities:**
1. **Validation**: Ensure data integrity and entity existence
2. **Business Rules**: Enforce domain-specific constraints
3. **Orchestration**: Coordinate operations across multiple repositories
4. **Transaction Management**: Ensure data consistency
5. **Error Handling**: Provide meaningful error messages

##### **5. DTOs (Data Transfer Objects)**

**DTOs** are simple objects used to transfer data between layers, decoupling the presentation layer from domain entities.

```csharp
// DTO - Used for data transfer (no business logic)
public class AppointmentDto
{
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int HospitalId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string? Notes { get; set; }
}

// Domain Entity - Used in business logic (may have methods, validation)
public class Appointment
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int HospitalId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Navigation properties
    public Patient Patient { get; set; }
    public Doctor Doctor { get; set; }
    public Hospital Hospital { get; set; }
}
```

**Why Use DTOs?**

| Benefit | Explanation |
|:--------|:------------|
| **Decoupling** | Presentation layer doesn't depend on entity structure |
| **Security** | Expose only necessary properties (hide sensitive data) |
| **Versioning** | Can change entity structure without breaking API |
| **Validation** | Can have different validation rules than entities |
| **Performance** | Transfer only required data (no navigation properties) |

##### **6. Service Lifetime in DI Container**

Services are registered with different lifetimes in `Program.cs`:

```csharp
// Program.cs - Service Registration

// SCOPED: One instance per HTTP request (RECOMMENDED for services)
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IHospitalService, HospitalService>();

// SCOPED: DbContext is scoped (one per request)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// TRANSIENT: New instance every time (use for lightweight, stateless services)
builder.Services.AddTransient<IEmailService, EmailService>();

// SINGLETON: One instance for application lifetime (use for caching, configuration)
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
```

**Lifetime Comparison:**

| Lifetime | Scope | Use Case |
|:---------|:------|:---------|
| **Scoped** | Per HTTP request | Services, Repositories, DbContext |
| **Transient** | Per injection | Lightweight, stateless services |
| **Singleton** | Application lifetime | Configuration, caching, logging |

**Why Scoped for Services?**
- DbContext is scoped (one per request)
- All repositories in a request share the same DbContext
- Enables transaction management across multiple repositories
- Disposed automatically at end of request

#### **Complete Example: Service Layer Architecture**

```csharp
// ===== LAYER 1: Service Interface (Contract) =====
public interface IPatientService
{
    Task<PatientDto?> GetPatientByIdAsync(int id);
    Task<PatientDto?> GetPatientByPhoneAsync(string phone);
    Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm);
    Task<int> RegisterPatientAsync(PatientDto dto);
    Task<bool> UpdatePatientAsync(int id, PatientDto dto);
    Task<bool> DeletePatientAsync(int id);
}

// ===== LAYER 2: Service Implementation (Business Logic) =====
public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepo;
    private readonly IAppointmentRepository _appointmentRepo;

    // Constructor Injection
    public PatientService(
        IPatientRepository patientRepo,
        IAppointmentRepository appointmentRepo)
    {
        _patientRepo = patientRepo;
        _appointmentRepo = appointmentRepo;
    }

    public async Task<PatientDto?> GetPatientByIdAsync(int id)
    {
        var patient = await _patientRepo.GetByIdAsync(id);
        return patient != null ? MapToDto(patient) : null;
    }

    public async Task<PatientDto?> GetPatientByPhoneAsync(string phone)
    {
        var patient = await _patientRepo.GetByPhoneAsync(phone);
        return patient != null ? MapToDto(patient) : null;
    }

    public async Task<int> RegisterPatientAsync(PatientDto dto)
    {
        // Business Rule: Check for duplicate phone number
        var existing = await _patientRepo.GetByPhoneAsync(dto.PhoneNumber);
        if (existing != null)
            throw new InvalidOperationException("Patient with this phone number already exists");

        // Business Rule: Validate age
        if (dto.DateOfBirth > DateTime.Now.AddYears(-1))
            throw new ArgumentException("Invalid date of birth");

        // Map DTO to Entity
        var patient = new Patient
        {
            Name = dto.Name,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow
        };

        // Save via repository
        await _patientRepo.AddAsync(patient);
        return patient.PatientId;
    }

    public async Task<bool> DeletePatientAsync(int id)
    {
        // Business Rule: Cannot delete patient with upcoming appointments
        var upcomingAppointments = await _appointmentRepo
            .GetUpcomingByPatientIdAsync(id);
        
        if (upcomingAppointments.Any())
            throw new InvalidOperationException(
                "Cannot delete patient with upcoming appointments");

        await _patientRepo.DeleteAsync(id);
        return true;
    }

    // Helper method - Entity to DTO mapping
    private PatientDto MapToDto(Patient patient)
    {
        return new PatientDto
        {
            PatientId = patient.PatientId,
            Name = patient.Name,
            PhoneNumber = patient.PhoneNumber,
            Email = patient.Email,
            DateOfBirth = patient.DateOfBirth,
            Address = patient.Address
        };
    }
}

// ===== LAYER 3: DTO (Data Transfer Object) =====
public class PatientDto
{
    public int PatientId { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }
    
    [Required]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
}

// ===== LAYER 4: Registration in Program.cs =====
// Program.cs
builder.Services.AddScoped<IPatientService, PatientService>();
```

#### **Service Layer Best Practices**

| Practice | Explanation |
|:---------|:------------|
| **Interface-Based** | Always define service interfaces for testability and flexibility |
| **Single Responsibility** | Each service handles one domain area (Patient, Appointment, etc.) |
| **Business Logic Only** | No data access code (use repositories), no UI logic (use PageModels) |
| **Transaction Management** | Use transactions for operations spanning multiple repositories |
| **Validation** | Validate business rules in services, not just data annotations |
| **Error Handling** | Throw meaningful exceptions with business context |
| **DTOs** | Use DTOs to decouple layers and control data exposure |
| **Async/Await** | Use async methods for all I/O operations (database, external APIs) |

#### **Dependency Flow**

```
Presentation Layer (Razor PageModel)
        ↓ depends on
Service Interface (IPatientService)
        ↓ implemented by
Service Implementation (PatientService)
        ↓ depends on
Repository Interface (IPatientRepository)
        ↓ implemented by
Repository Implementation (PatientRepository)
        ↓ depends on
DbContext (ApplicationDbContext)
        ↓ accesses
Database (SQL Server)
```

**Key Principle**: Each layer depends on **interfaces** (abstractions), not concrete implementations. This enables:
- **Testability**: Mock interfaces in unit tests
- **Flexibility**: Swap implementations without changing dependent code
- **Maintainability**: Changes in one layer don't cascade to others

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
