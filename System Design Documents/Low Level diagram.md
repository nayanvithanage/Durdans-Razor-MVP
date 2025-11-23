# Low Level Design - Durdans Razor MVP (Current Scope)

This document provides the detailed class design, database schema, and method signatures for the Durdans Hospital Clinic Management System (Razor Pages).

## 1. Database Schema (SQL Server)

```mermaid
erDiagram
    Patients {
        int Id PK
        nvarchar(100) Name
        date DateOfBirth
        nvarchar(15) ContactNumber
        datetime CreatedAt
    }

    Doctors {
        int Id PK
        nvarchar(100) Name
        nvarchar(50) Specialization
        decimal ConsultationFee
        nvarchar(MAX) AvailabilityJson "Stores days/times"
    }

    Hospitals {
        int Id PK
        nvarchar(100) Name
        nvarchar(200) Address
    }

    DoctorHospitals {
        int DoctorId FK
        int HospitalId FK
    }

    Appointments {
        int Id PK
        int PatientId FK
        int DoctorId FK
        int HospitalId FK
        datetime AppointmentDate
        nvarchar(20) Status "Booked, Cancelled, Completed"
        datetime CreatedAt
    }

    Patients ||--o{ Appointments : "makes"
    Doctors ||--o{ Appointments : "has"
    Hospitals ||--o{ Appointments : "hosts"
    Doctors }|--|{ Hospitals : "works_at"
```

## 2. Class Design (Backend)

### 2.1 Domain Models

```csharp
public class Patient {
    public int Id { get; set; }
    [Required] public string Name { get; set; }
    public DateTime DateOfBirth { get; set; }
    [Phone] public string ContactNumber { get; set; }
}

public class Doctor {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Specialization { get; set; }
    public decimal ConsultationFee { get; set; }
    public List<Hospital> Hospitals { get; set; } // Many-to-Many
}

public class Hospital {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public List<Doctor> Doctors { get; set; }
}

public class Appointment {
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public int HospitalId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Status { get; set; }
}
```

### 2.2 Data Access Layer (Repositories)

```mermaid
classDiagram
    class IRepository~T~ {
        +GetAll() IEnumerable~T~
        +GetById(int id) T
        +Add(T entity) void
        +Update(T entity) void
        +Delete(int id) void
    }

    class PatientRepository {
        +GetByPhone(string phone) Patient
    }

    class DoctorRepository {
        +GetBySpecialization(string spec) IEnumerable~Doctor~
        +GetAvailableSlots(int doctorId, DateTime date) List~DateTime~
    }

    class AppointmentRepository {
        +GetByDoctorAndDate(int doctorId, DateTime date) IEnumerable~Appointment~
    }

    IRepository <|-- PatientRepository
    IRepository <|-- DoctorRepository
    IRepository <|-- AppointmentRepository
```

### 2.3 Business Logic Layer (Services)

*   **AppointmentService**
    *   `BookAppointment(AppointmentDto dto)`: Validates availability, checks for double booking, saves to DB.
    *   `GetAvailableSlots(int doctorId, int hospitalId, DateTime date)`: Returns list of free time slots.

## 3. Razor Pages Design (UI Layer)

### 3.1 Patient Pages

*   **Register (`/Patients/Register`)**
    *   `OnGet()`: Returns empty form.
    *   `OnPost()`:
        1.  Validates `ModelState`.
        2.  Calls `PatientRepository.Add()`.
        3.  Redirects to Index.

### 3.2 Appointment Pages

*   **Book (`/Appointments/Book`)**
    *   `OnGet()`: Loads Dropdowns (Specializations, Hospitals).
    *   `OnGetDoctors(string specialization)`: AJAX handler to fetch doctors.
    *   `OnPost()`:
        1.  Validates selection.
        2.  Calls `AppointmentService.BookAppointment()`.
        3.  Returns Success/Failure message.

### 3.3 Hospital Pages

*   **Create (`/Hospitals/Create`)**
    *   `OnPost()`: Adds new hospital.

## 4. API Endpoints (Optional for AJAX)

If using AJAX for dynamic dropdowns:
*   `GET /api/doctors?specialization={spec}`
*   `GET /api/slots?doctor={id}&date={date}`
