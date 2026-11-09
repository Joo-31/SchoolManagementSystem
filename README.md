<div align="center">

# 🏫 School Management System

**A comprehensive, enterprise-grade School Management System built with .NET 8 Web API and Angular 22.**

[![.NET 8](https://img.shields.io/badge/.NET-8.0-blueviolet?style=for-the-badge&logo=.net)](https://dotnet.microsoft.com/)
[![Angular](https://img.shields.io/badge/Angular-22-red?style=for-the-badge&logo=angular)](https://angular.io/)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2022-blue?style=for-the-badge&logo=microsoft-sql-server)](https://www.microsoft.com/en-us/sql-server/)
[![JWT](https://img.shields.io/badge/JWT-Authentication-orange?style=for-the-badge&logo=json-web-tokens)](https://jwt.io/)

</div>

---

## 📖 About The Project
A full-stack School Management System designed to streamline administrative tasks, enhance communication, and provide a seamless experience for Admins, Teachers, and Students. Built following **Clean Architecture** and **SOLID Principles**, with a focus on scalability, security, and modern UI/UX.

---

## 🚀 Key Features

### 🔐 Authentication & Authorization
- **JWT Authentication** with Role-Based Access Control (Admin, Teacher, Student).
- **Password Hashing** using BCrypt.
- **Auto-Logout** on token expiry via Angular Interceptors.

### 👥 Entities Management (CRUD + Pagination + Search)
- Students, Teachers, Courses, Classes, Grades, Attendances, and Marks.
- **Smart Search** across all lists (by name, course, grade, etc.).
- **Server-side Pagination** for high performance with massive datasets.

### 📊 Dashboards & Analytics
- **Admin Dashboard:** Full control + Statistics (Total Students, Teachers, etc.).
- **Teacher Dashboard:** My Classes, My Students, Take Attendance, Assign Homework.
- **Student Dashboard:** My Profile, My Attendance (with Stats & Date Filter), My Marks (with GPA & Stats).

### 🔔 Advanced Systems (New!)
- **Notifications System:** Admin can broadcast announcements to specific roles (All, Teachers, or Students). Users get a bell icon with unread counts.
- **Homework System:** Teachers can upload assignments (via URL/Link) for their courses. Students can view them in a dedicated page.
- **Dark Mode:** A beautiful, persistent dark theme across the entire application.

### 🔒 Business Rules Enforced
- Teachers can only add/edit marks for their own assigned courses.
- Only one mark per student per course is allowed.
- Only Admins can delete entities (Teachers, Students, etc.).
- Teachers can only create homework for their assigned courses.

---

## 🛠️ Tech Stack

### Backend
- **Framework:** .NET 8 Web API (C#)
- **ORM:** Entity Framework Core 8 (Code-First, Migrations)
- **Database:** SQL Server
- **Auth:** JWT (JSON Web Tokens)
- **Mapping:** AutoMapper
- **Logging:** Serilog
- **Security:** BCrypt.Net
- **Documentation:** Swagger (OpenAPI)
- **Testing:** xUnit, Moq, FluentAssertions (Unit Tests for Services)

### Frontend
- **Framework:** Angular 22 (Standalone Components)
- **Language:** TypeScript
- **State/RxJS:** RxJS Observables & Interceptors
- **Styling:** Custom CSS (Responsive, Dark Mode support)
- **Features:** Custom Toast Notifications, Role-Based Routing

### Architecture & Patterns
- Clean Architecture (N-Tier layered conceptually)
- Repository Pattern & Unit of Work
- Dependency Injection
- SOLID Principles

---

## 🏗️ Project Structure

```text
SchoolManagementSystemSolution/
├── SchoolManagementAPI/           → Web API (.NET 8)
│   ├── Controllers/
│   ├── Services/
│   ├── Repositories/
│   ├── Models/
│   ├── DTOs/
│   ├── Database/
│   ├── Middlewares/
│   └── Migrations/
├── SchoolManagementAPI.Tests/     → Unit Tests (xUnit + Moq)
└── SchoolManagementClient/        → Frontend (Angular)
    └── src/app/
        ├── pages/
        ├── services/
        ├── models/
        ├── interceptors/
        └── components/

## 🚀 Getting Started

### Prerequisites
- **.NET 8 SDK**
- **Node.js 20+**
- **SQL Server** (or LocalDB)
- **Docker** (optional)

### Backend Setup

```bash
cd SchoolManagementAPI

# Update database
dotnet ef database update

# Run
dotnet run
API: https://localhost:7092/swagger

Frontend Setup

cd SchoolManagementClient

# Install dependencies
npm install

# Run
ng serve

Frontend: http://localhost:4200

Run with Docker

docker-compose up -d

🔑 Default Users
Role	  Username	          Password
Admin	  admin	             Admin@123
Teacher   teacher_ahmed	     Teacher@123
Student	  student_omar	     Student@123

📸 Screenshots
Coming soon...

📌 API Endpoints


Authentication

POST /api/auth/register — Register new user

POST /api/auth/login — Login and get JWT token

Students

GET /api/students — Get all students

GET /api/students/paged — Paginated students

GET /api/students/me — Get current student profile

GET /api/students/me/attendances — Get current student's attendance

POST/PUT/DELETE — CRUD operations

Teachers

GET /api/teachers/me — Get current teacher profile

GET /api/teachers/me/classes — Get current teacher's classes

GET /api/teachers/me/classes/{id}/students — Get students in a class

POST/PUT/DELETE — CRUD operations

Marks

GET /api/marks — All marks (Admin/Teacher)

GET /api/marks/me — Current student's marks

GET /api/marks/my-marks — Current teacher's marks

GET /api/marks/my-courses — Teacher's own courses

GET /api/marks/my-students — Students in teacher's classes

POST /api/marks — Add mark (with business rules)

PUT/DELETE — Update/Delete (with permissions)

... (other entities: similar pattern)

🧪 Testing

cd SchoolManagementAPI.Tests
dotnet test


👤 Author
Youssef Adel

GitHub: @Joo-31

📝 License
This project is for educational purposes.

.

🌟 Star this repo if you find it useful!









