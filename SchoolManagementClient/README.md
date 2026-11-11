# schoolmanagementweb

Angular front end for **SchoolManagementAPI** (standalone components, signals, lazy routes, functional guards/interceptors, reactive forms). Same versions as your original client (Angular 22).

## Run
```bash
npm install
npm start            # ng serve → http://localhost:4200
```
1. Start the API (https://localhost:7092). Open `https://localhost:7092/swagger` once and accept the dev certificate.
2. Sign in with `admin / Admin@123`, `teacher_ahmed / Teacher@123` or `student_omar / Student@123`.
3. No backend handy? On the sign-in screen open **Connection → Demo data**.

The API URL lives in `src/environments/` (dev: `environment.development.ts`). It can also be changed at runtime from the sign-in screen.

> The API's CORS policy accepts any `localhost` origin, so port 4200 works as is. For production, add your site's origin to CORS and set `apiUrl` in `environment.ts`.

## Structure
```
src/app
  core/      api + auth + settings services, interceptor, guards, models, demo-mode mock API
  layout/    shell (sidebar, top bar, announcements drawer, Ctrl+K palette)
  shared/    icon/avatar/stamp/charts, dialogs + dynamic form, generic CRUD page and its per-entity configs
  pages/     login, admin dashboard, users, announcements, teacher pages, student pages, profile
```
- **CRUD pages** (students, teachers, courses, classes, grades, attendance, marks, teacher marks) are one component, `shared/crud-page.ts`, driven by `shared/crud-configs.ts`. Add a new entity by adding a config and a route.
- **Demo mode** is an `HttpInterceptor` (`core/api.interceptor.ts` + `core/mock-api.ts`) that answers from memory with the same routes and DTO shapes as the API.
- **Auth**: the session (role, teacherId, studentId) is read from the JWT, so it does not depend on the login response shape.

## Tests
`npm test` runs the small unit tests in `src/app/core/util.spec.ts`.

## Needs these API changes (already made in the fixed API project)
- `GET /api/marks/paged?pageNumber&pageSize&search` (marks list is server-paged)
- `TeacherDto.HireDate`
