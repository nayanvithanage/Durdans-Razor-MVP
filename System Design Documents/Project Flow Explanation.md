# Project Flow Explanation - Durdans Razor MVP

This document explains the end-to-end flow of data and control for the key features of the Durdans Hospital Clinic Management System (Razor Pages implementation).

## 1. Patient Registration Flow

**Goal**: A new patient registers themselves in the system.

### Step-by-Step Flow
1.  **UI Layer (`Pages/Patients/Register.cshtml`)**:
    *   User navigates to `/Patients/Register`.
    *   `OnGet()` method renders the empty registration form.
    *   User fills in Name, Date of Birth, and Contact Number.
    *   User clicks "Register". Form data is POSTed to the server.

2.  **Page Model Layer (`Register.cshtml.cs`)**:
    *   `OnPost()` method is triggered.
    *   **Validation**: Checks `ModelState.IsValid` (e.g., Name is required, Phone format).
    *   **Action**: Calls `_patientRepository.Add(patient)`.

3.  **Data Access Layer (`PatientRepository.cs`)**:
    *   `Add(Patient patient)` method receives the domain model.
    *   **EF Core**: Adds the entity to the `Patients` DbSet and calls `SaveChanges()`.
    *   **Database**: SQL Server executes `INSERT INTO Patients ...`.

4.  **Result**:
    *   User is redirected to the Patient Index page (`/Patients/Index`) with a success message.

---

## 2. Doctor Registration Flow

**Goal**: An Admin registers a new doctor and assigns them to hospitals.

### Step-by-Step Flow
1.  **UI Layer (`Pages/Doctors/Create.cshtml`)**:
    *   Admin navigates to `/Doctors/Create`.
    *   `OnGet()` loads the list of available Hospitals from `HospitalRepository` to populate a checkbox list.
    *   Admin enters Name, Specialization, Fee, and selects associated Hospitals.
    *   Admin clicks "Save".

2.  **Page Model Layer (`Create.cshtml.cs`)**:
    *   `OnPost()` method is triggered.
    *   **Mapping**: Maps form data to a `Doctor` entity, including the list of selected `Hospital` IDs.
    *   **Action**: Calls `_doctorRepository.Add(doctor)`.

3.  **Data Access Layer (`DoctorRepository.cs`)**:
    *   `Add(Doctor doctor)` method receives the entity.
    *   **EF Core**:
        *   Inserts the Doctor record.
        *   Inserts records into the junction table `DoctorHospitals` for the selected hospitals.
        *   Executes inside a transaction to ensure consistency.

4.  **Result**:
    *   Admin is redirected to the Doctor List.

---

## 3. Appointment Booking Flow

**Goal**: A patient books an appointment with a specific doctor at a specific hospital.

### Step-by-Step Flow
1.  **UI Layer (`Pages/Appointments/Book.cshtml`)**:
    *   **Initial Load**: `OnGet()` loads the "Specialization" dropdown.
    *   **User Interaction**:
        1.  User selects a Specialization.
        2.  **AJAX Call**: JavaScript fetches `/api/doctors?specialization=...`.
        3.  **Dynamic Update**: Doctor dropdown is populated.
        4.  User selects a Doctor.
        5.  **AJAX Call**: JavaScript fetches `/api/slots?doctor=...`.
        6.  **Dynamic Update**: Available time slots are shown.
        7.  User selects a Date, Hospital, and Time Slot.
        8.  User clicks "Confirm Booking".

2.  **Page Model Layer (`Book.cshtml.cs`)**:
    *   `OnPost()` receives `DoctorId`, `PatientId`, `HospitalId`, and `Date`.
    *   **Action**: Calls `_appointmentService.BookAppointment(dto)`.

3.  **Business Logic Layer (`AppointmentService.cs`)**:
    *   **Validation**: Checks if the slot is still free (concurrency check).
    *   **Business Rule**: Ensures the doctor is actually available at that hospital on that day.
    *   **Transaction**: Starts a database transaction.
    *   **Save**: Calls `_appointmentRepository.Add(appointment)`.
    *   **Commit**: Commits the transaction if all checks pass.

4.  **Data Access Layer (`AppointmentRepository.cs`)**:
    *   Executes `INSERT INTO Appointments ...`.

5.  **Result**:
    *   User sees a "Booking Confirmed" page with the Appointment ID.
