# School Settings Report

## Summary

Added an Admin-only, lazy-loaded school settings page with reactive editing, file upload preview, JSON import/export, reset confirmation, dirty-state navigation warning, and toasts. A startup-loaded signal service stores full school settings through the new API contract, mock API, or browser storage fallback; branding updates the shell, login page, document title, document direction, accent variable, and favicon.

## Files created or modified

| File | Reason |
|---|---|
| `src/app/app.config.ts` | Load school settings once during application initialization. |
| `src/app/app.routes.ts` | Add the Admin-only lazy Settings route and leave guard. |
| `src/app/core/models.ts` | Define school settings, term, and grade band API contracts. |
| `src/app/core/mock-api.ts` | Implement demo GET/PUT settings persistence with safe localStorage access. |
| `src/app/core/nav.ts` | Add Settings to Admin navigation and Ctrl+K palette. |
| `src/app/layout/shell.ts` | Apply school name/logo in sidebar and top bar. |
| `src/app/pages/login.ts` | Apply school name/logo to login branding. |
| `src/styles.css` | Add responsive settings layout, preview, sticky bar, and reduced-motion styles. |
| `src/app/core/school-settings.service.ts` | Add defaults, settings signal, API/local persistence, and document branding updates. |
| `src/app/core/school-settings.validation.ts` | Add grade band, term overlap, logo file, and import validation helpers. |
| `src/app/core/school-settings.validation.spec.ts` | Test the requested validation helpers. |
| `src/app/pages/settings.ts` | Implement the settings UI, reactive form, connection check, import/export, and reset. |
| `src/app/core/settings.guard.ts` | Confirm navigation away from dirty settings without eagerly loading the page. |
| `SETTINGS_REPORT.md` | Provide the requested implementation and verification report. |

## Settings fields

Defaults below match the built-in settings object; empty strings mean no value is configured.

| Section | Field | Type | Default | Validation |
|---|---|---|---|---|
| School & Branding | School name | string | `Chalkline` | Required, 2–80 characters |
| School & Branding | Short name | string | `Chalkline` | Maximum 20 characters |
| School & Branding | Logo | nullable data URL string | `null` | PNG/JPG/SVG/WEBP only, maximum 512 KB |
| School & Branding | Tagline, address, phone, email, website | string | empty | Email and URL use typed inputs; no further business validation currently enforced |
| Academic | Academic year | string | Current year / next year | Required input; no format rule currently enforced |
| Academic | Terms | array of `{name,startDate,endDate}` | Term 1 and Term 2 with blank dates | Dated terms may not overlap; start must precede end |
| Academic | Week starts on | number (0/1/6) | `0` (Sunday) | Sunday, Monday, or Saturday option |
| Grading | Pass mark | number | `50` | 0–100 |
| Grading | Grade bands | array of `{label,minPercent}` | A:90, B:80, C:70, D:50, F:0 | Labels required, minima 0–100, unique minima, must include 0; descending thresholds define contiguous ranges |
| Attendance | Late threshold | number | `10` minutes | Minimum 0 |
| Attendance | Count excused absences | boolean | `false` | Checkbox |
| Localization | Language | `English` or `Arabic` | `English` | Select; changes document language and direction |
| Localization | Date format | string | `d MMM y` | Free text; not yet consumed by all existing date displays |
| Localization | Timezone | string | `Africa/Cairo` | Free text; IANA timezone validation not implemented |
| Appearance | Default theme | `light`, `dark`, or `system` | `system` | Select; system leaves the existing theme behavior intact |
| Appearance | Accent color | CSS color string | `#3D4DF5` | Native color input; contrast ratio check is not implemented |
| Connection | API mode | `demo` or `live` | `live` | Select; synchronized with existing `SettingsService` |
| Connection | API URL | string | Existing environment API URL | URL input; existing API service trims trailing slash |

## Branding locations

School name and logo now appear in the sidebar and top bar; the school name/logo also appear in login branding. The school name drives `document.title`; the logo data URL replaces the favicon when present. If no logo is configured, the current Chalkline mark and existing favicon remain in use.

## Suggested groups and additional decisions

- Implemented: school info, academic terms/year/week start, grading thresholds/pass mark, attendance rules, language and RTL/LTR switching, date format/timezone fields, theme/accent fields, connection mode/API URL with a test button, JSON export/import, and confirmed reset.
- Partially implemented: appearance accent color is editable but has no contrast check; date format/timezone values are editable but are not yet applied throughout existing date formatting; grade bands are configurable but existing grade labels/colors in `util.ts` and `BandVarPipe` still use their original hard-coded thresholds.
- Skipped: none of the requested groups were omitted entirely. A dedicated server health endpoint is not specified, so “Test connection” requests `GET /api/settings/school`; a missing endpoint reports the API error/local fallback state.
- Extra: tagline/contact fields, favicon replacement, short-name branding, and a persistent local-only status notice were included as requested details around the core groups.

## API contract and backend outline

- `GET /api/settings/school` returns the full `SchoolSettings` JSON object.
- `PUT /api/settings/school` accepts that full object as its JSON body and returns the saved full object.
- The logo is a nullable data URL string. Demo mode persists to `localStorage` using key `cl.schoolSettings`; live mode falls back to local storage when the endpoint returns 404 and shows a notice on the Settings page.

Example response/request body:

```json
{
  "schoolName": "Northstar School",
  "shortName": "Northstar",
  "logo": null,
  "tagline": "Learning together",
  "address": "12 Main Street",
  "phone": "+20 2 1234 5678",
  "email": "office@northstar.example",
  "website": "https://northstar.example",
  "academicYear": "2026/2027",
  "terms": [{ "name": "Term 1", "startDate": "2026-09-01", "endDate": "2027-01-15" }],
  "weekStartsOn": 0,
  "passMark": 50,
  "gradeBands": [{ "label": "A", "minPercent": 90 }, { "label": "F", "minPercent": 0 }],
  "lateThresholdMinutes": 10,
  "countExcusedAbsences": false,
  "language": "English",
  "dateFormat": "d MMM y",
  "timezone": "Africa/Cairo",
  "defaultTheme": "system",
  "accentColor": "#3D4DF5",
  "apiMode": "live",
  "apiUrl": "https://api.example"
}
```

Suggested C# backend outline (not added to the .NET backend):

```csharp
public sealed record SchoolTermDto(string Name, DateOnly? StartDate, DateOnly? EndDate);
public sealed record GradeBandDto(string Label, decimal MinPercent);
public sealed record SchoolSettingsDto(
    string SchoolName, string ShortName, string? Logo, string Tagline, string Address,
    string Phone, string Email, string Website, string AcademicYear,
    IReadOnlyList<SchoolTermDto> Terms, int WeekStartsOn, decimal PassMark,
    IReadOnlyList<GradeBandDto> GradeBands, int LateThresholdMinutes,
    bool CountExcusedAbsences, string Language, string DateFormat, string Timezone,
    string DefaultTheme, string AccentColor, string ApiMode, string ApiUrl);

[ApiController]
[Route("api/settings/school")]
[Authorize(Roles = "Admin")]
public sealed class SchoolSettingsController(ISchoolSettingsStore store) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<SchoolSettingsDto>> Get(CancellationToken ct)
        => Ok(await store.GetAsync(ct));

    [HttpPut]
    public async Task<ActionResult<SchoolSettingsDto>> Put(
        [FromBody] SchoolSettingsDto settings, CancellationToken ct)
    {
        // Validate name lengths, ranges, non-overlapping terms, and contiguous grade bands.
        return Ok(await store.SaveAsync(settings, ct));
    }
}
```

The backend should persist one settings document per school/tenant, authorize both actions to Admin, validate the same constraints server-side, and support the chosen logo data URL storage limits.

## Verification

- `npm run build`: **passed**, Angular production build completed. Final summary: `Application bundle generation complete. [3.320 seconds] - 2026-11-10T10:43:47.255Z`; output: `D:\schoolmanagementweb\dist\schoolmanagementweb`.
- `npm test`: **passed**. Exact summary: `Test Files  2 passed (2)` and `Tests  10 passed (10)`; duration `1.84s`.
- An initial `npm test -- --run` attempt was rejected by Angular CLI as an unknown argument; the project's supported `npm test` command was run successfully afterward.
- No browser/manual interaction with login, dashboards, role redirects, live 404 behavior, file upload, or generic CRUD pages was performed. Their compilation was covered by the Angular production build.

## Limitations and assumptions

- The existing interceptor turns HTTP errors into `ApiError`; the 404 fallback checks that wrapper. The mock API's generic handler still uses existing untyped legacy code, which was not refactored.
- Grade-band settings currently validate and persist but do not drive existing hard-coded grade-band labels/colors because those utilities do not receive the school settings signal.
- Accent contrast is not checked. Theme `system` does not add a separate system-theme mode; it preserves the app's existing theme selection behavior.
- Term overlap is checked only when both dates are populated. The form requires no dates; partially dated terms are not rejected.
- The import validator checks important core fields and the grading/term shape, but does not fully validate every imported field's exact type or sanitize SVG contents.
- Local storage access is guarded for settings persistence. If browser storage is unavailable, settings can remain in memory for the current runtime but cannot persist across reloads.
- The test button uses the settings GET endpoint as the connectivity probe. A backend returning 404 will indicate the endpoint is not implemented rather than distinguishing network health from endpoint support.

## Manual verification checklist

1. Sign in as Admin and open Settings from the sidebar and Ctrl+K palette; verify Teacher and Student are redirected and do not see the nav item.
2. Change school name/short name, save, and check sidebar, top bar, login page after sign-out, document title, and fallback branding after clearing the logo.
3. Upload valid PNG/JPG/SVG/WEBP files; verify preview, replace/remove, and clear errors for invalid MIME type and files above 512 KB.
4. Add overlapping terms and invalid grade bands; verify the inline errors disable Save changes. Save valid values and confirm the “Changes saved” toast.
5. Navigate away with edits and verify the discard confirmation; use Discard and verify values revert.
6. Export settings JSON, import the export, then try malformed JSON and verify the error toast; test Reset to defaults and cancel/confirm behavior.
7. In demo mode, reload and verify saved settings persist. In live mode against a backend without the endpoint, verify the local-only notice and browser storage fallback; with the endpoint implemented, verify GET/PUT and the connection test.
8. Switch language and theme, use a narrow viewport and keyboard navigation, and confirm the form remains operable and focus remains visible.
