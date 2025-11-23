# User Stories - Durdans Hospital Clinic Management System (MVP)

## Epics
1.  **Patient Management**
2.  **Doctor Management**
3.  **Hospital Management**
4.  **Appointment Scheduling**

---

## 1. Patient Management
> **As a Receptionist/Admin**, I want to manage patient records so that we can track who is visiting the hospital.

*   **US-1.1 Register Patient**
    *   "As a **Receptionist**, I want to **register a new patient** with their name, DOB, and contact number, so that they can book appointments."
    *   *Acceptance Criteria:* System saves patient details; Contact number must be valid.

*   **US-1.2 View Patients**
    *   "As a **Receptionist**, I want to **view a list of all registered patients**, so that I can quickly find their IDs."

## 2. Doctor Management
> **As an Admin**, I want to manage the doctor registry so that patients know who is available for consultation.

*   **US-2.1 Register Doctor**
    *   "As an **Admin**, I want to **add a new doctor** with their specialization and consultation fee, time slots, so that they appear in the booking system."
    *   *Acceptance Criteria:* Name, Specialization, and Fee, Hospital, Time Slots are required fields.

*   **US-2.2 View Doctor List**
    *   "As a **Patient/Admin**, I want to **see a list of all doctors**, so that I can choose the right specialist."

*   **US-2.3 Update Doctor Details**
    *   "As an **Admin**, I want to **update a doctor's details**, so that I can correct any errors or changes."

*   **US-2.4 Delete Doctor**
    *   "As an **Admin**, I want to **delete a doctor's details**, so that I can remove any errors or changes."

*   **US-2.5 Availability Settings**
    *   "As a **Admin/Doctor**, I want to **set the availability of a doctor**, so that I can control when the doctor is available for consultations."

## 3. Hospital Management
> **As an Admin**, I want to manage the hospital registry so that patients know where to visit for their appointments.

*   **US-3.1 Register Hospital**
    *   "As an **Admin**, I want to **add a new hospital** with its name and address, so that it appears in the booking system."
    *   *Acceptance Criteria:* Name and Address are required fields.

*   **US-3.2 View Hospital List**
    *   "As a **Patient/Admin**, I want to **see a list of all hospitals**, so that I can choose the right location."

*   **US-3.3 Update Hospital Details**
    *   "As an **Admin**, I want to **update a hospital's details**, so that I can correct any errors or changes."

*   **US-3.4 Delete Hospital**
    *   "As an **Admin**, I want to **delete a hospital's details**, so that I can remove any errors or changes."

## 4. Appointment Scheduling
> **As a Patient**, I want to book appointments easily so that I can receive medical care without long waits.

*   **US-4.1 Book Appointment**
    *   "As a **Patient**, I want to **book an appointment** with a specific doctor, so that I can reserve a time slot."
    *   *Acceptance Criteria:* Must select a valid Doctor and Patient; Date must be in the future.

*   **US-4.2 View Appointments**
    *   "As a **Doctor/Admin**, I want to **view all upcoming appointments**, so that I can prepare for the consultations."
    *   *Acceptance Criteria:* List shows Doctor Name, Patient Name, and Date/Time.

*   **US-4.3 Cancel/Reschedule Appointment**
    *   "As a **Patient**, I want to **cancel or reschedule an appointment**, so that I can manage my schedule."
    *   *Acceptance Criteria:* Must select a valid Appointment ID; Date must be in the future.

