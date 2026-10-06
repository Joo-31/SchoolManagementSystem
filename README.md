# 🏫 School Management System

A comprehensive **School Management System** built with **.NET 8 Web API** and **Angular 22**, 
following **Clean Architecture** and **SOLID Principles**.

---

## 🚀 Features

### 🔐 Authentication & Authorization
- **JWT Authentication** with roles (Admin, Teacher, Student)
- **Role-Based Access Control** — each role sees different pages
- **Password Hashing** with BCrypt

### 👥 Entities Management
- **Students** — CRUD + Pagination
- **Teachers** — CRUD + Pagination
- **Courses** — CRUD + Pagination
- **Classes** — CRUD + Pagination
- **Grades** — CRUD + Pagination
- **Attendances** — CRUD + Pagination
- **Marks** — CRUD with teacher-specific permissions

### 🎯 Role-Based Dashboards
- **Admin Dashboard** — Manage everything + Statistics
- **Teacher Dashboard** — My Classes, My Students, Take Attendance, My Marks
- **Student Dashboard** — My Profile, My Attendance, My Marks

### 📊 Statistics Dashboard
- Total Students, Teachers, Courses, Classes, Grades

### 🔒 Business Rules
- Teachers can only add marks for their own courses
- One mark per student per course
- Only admins can delete entities

### ✨ Additional Features
- **Toast Notifications** — Beautiful success/error messages
- **401 Interceptor** — Auto logout on token expiry
- **Pagination** — All list pages

---

## 🛠️ Tech Stack

### Backend
- **.NET 8** (C#)
- **ASP.NET Core Web API**
- **Entity Framework Core 8** (SQL Server)
- **JWT Authentication**
- **AutoMapper** (DTOs)
- **Serilog** (Logging)
- **BCrypt.Net** (Password Hashing)
- **Swagger** (API Documentation)

### Frontend
- **Angular 22**
- **TypeScript**
- **RxJS**
- **Custom Toast Notifications**
- **Role-Based UI**

### Architecture
- **Repository Pattern**
- **Unit of Work Pattern**
- **Dependency Injection**
- **SOLID Principles**
- **Clean Architecture**

### Testing & DevOps
- **xUnit** + **Moq** + **FluentAssertions**
- **Docker** + **Docker Compose**
- **GitHub Actions** (CI/CD)

---

## 🏗️ Project Structure

SchoolManagementSystemSolution/
├── SchoolManagementAPI/ → Web API (.NET 8)
│ ├── Controllers/
│ ├── Services/
│ ├── Repositories/
│ ├── Models/
│ ├── DTOs/
│ ├── Database/
│ ├── Middlewares/
│ └── Migrations/
├── SchoolManagementAPI.Tests/ → Unit Tests (xUnit)
└── SchoolManagementClient/ → Frontend (Angular 22)
└── src/app/
├── pages/
├── services/
├── models/
├── interceptors/
└── components/


---

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









