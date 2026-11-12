<div align="center">

# 🏫 School Management System

**A comprehensive, modern School Management System built with .NET 8 and Angular 22, featuring full Arabic/English localization, role-based dashboards, and advanced data management.**

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
- **BCrypt** password hashing.
- **Secure API endpoints** (no anonymous access to sensitive data).

### 👥 Entities Management (CRUD + Pagination + Search)
- Students, Teachers, Courses, Classes, Grades, Attendances, Marks, Homeworks, and Notifications.
- **Server-side Pagination** and **Search** for all lists.

### 📊 Advanced Dashboards
- **Admin Dashboard:** KPI cards, Attendance chart, Students per grade, Teachers by subject, Announcements.
- **Teacher Dashboard:** My Classes, Take Attendance, Add Marks, Add Homework.
- **Student Dashboard:** My Marks, My Attendance, Homework.

### 🌍 Full Localization (Arabic/English)
- **Language toggle** (AR/EN) in the sidebar and login page.
- **Full RTL/LTR support**: When Arabic is selected, the layout mirrors, sidebar moves to the right, and all text is translated.
- **Persistence**: Language preference is saved in `localStorage`.

### 📂 File Upload for Homework
- Teachers can upload **PDF files** as homework.
- Files are stored in `wwwroot/uploads/homework` and accessible to students.

### 🛠️ Data Management (Admin Only)
- **Reset All Data**: Deletes all school data (keeps Admin accounts).
- **Seed Random Data**: Generates random data (Teachers, Students, Courses, etc.) for testing.

### 📢 Notifications & Announcements
- Admin can send announcements to specific roles (All, Teachers, Students).
- Notification bell with unread count.

---

## 🛠️ Tech Stack

### Backend
- **Framework:** .NET 8 Web API (C#)
- **ORM:** Entity Framework Core 8
- **Database:** SQL Server
- **Auth:** JWT (JSON Web Tokens)
- **Mapping:** AutoMapper
- **Logging:** Serilog
- **Security:** BCrypt.Net
- **Testing:** xUnit, Moq, FluentAssertions (70+ Tests)

### Frontend
- **Framework:** Angular 22
- **Language:** TypeScript
- **State/RxJS:** RxJS Observables
- **Styling:** Custom CSS (Responsive, RTL Support, Dark Mode)
- **i18n:** Custom Language Service (en/ar JSON files)

### Architecture
- Clean Architecture (N-Tier)
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
├── SchoolManagementAPI.Tests/     → Unit Tests (xUnit)
└── SchoolManagementClient/        → Frontend (Angular)
    └── src/app/
        ├── core/                  → Services, Guards, Interceptors
        ├── pages/                 → Components
        ├── shared/                → Reusable Components
        └── layout/                → Shell & Sidebar


🚀 Getting Started
Prerequisites
.NET 8 SDK

Node.js 20+ & Angular CLI

SQL Server (LocalDB or Full)

1. Backend Setup

cd SchoolManagementAPI
dotnet ef database update
dotnet run

API: https://localhost:7092/swagger

2. Frontend Setup

cd SchoolManagementClient
npm install
npm start

Frontend: http://localhost:4200

🔑 Default Users (Seeded)


Role	Username	Password
Admin	admin	Admin@123

Note: Users are created by the Admin (via Data Management or Seed). There is no public registration.

📸 Screenshots


<table> <tr> <td><img src="screenshots/login.png" alt="Login" width="400"/></td> <td><img src="screenshots/add-mark.png" alt="Add Mark" width="400"/></td> </tr> <tr> <td><img src="screenshots/dashboard-en.png" alt="Dashboard (English)" width="400"/></td> <td><img src="screenshots/dashboard-ar.png" alt="Dashboard (Arabic)" width="400"/></td> </tr> </table>


🧪 Testing

cd SchoolManagementAPI.Tests
dotnet test
👤 Author
Youssef Adel

GitHub: @Joo-31

📝 License
This project is for educational and portfolio purposes.

⭐ Star this repo if you find it useful! ⭐







