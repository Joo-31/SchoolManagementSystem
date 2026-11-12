// @ts-nocheck — demo-data generator; behaviour verified by running it, kept loosely typed on purpose.
// In-memory fake of the School Management API, same routes and DTO shapes. Used for demo mode.
const rng = seed => () => { seed |= 0; seed = seed + 0x6D2B79F5 | 0; let t = Math.imul(seed ^ seed >>> 15, 1 | seed); t = t + Math.imul(t ^ t >>> 7, 61 | t) ^ t; return ((t ^ t >>> 14) >>> 0) / 4294967296; };

const FIRST_M = ['Omar', 'Youssef', 'Ahmed', 'Mostafa', 'Karim', 'Ali', 'Hassan', 'Tarek', 'Adam', 'Ziad', 'Mazen', 'Hamza'];
const FIRST_F = ['Mariam', 'Nour', 'Salma', 'Farida', 'Laila', 'Hana', 'Jana', 'Yasmin', 'Habiba', 'Rania', 'Malak', 'Dina'];
const LAST = ['Hassan', 'Mahmoud', 'Ibrahim', 'Saleh', 'Nasser', 'Fouad', 'Gaber', 'Khalil', 'Samir', 'Adel', 'Ragab', 'Zaki', 'Fawzy', 'Anwar'];
const SPECS = ['Math', 'Science', 'English', 'Arabic', 'History'];
const COURSE_NAMES = { Math: ['Algebra', 'Geometry', 'Statistics'], Science: ['Physics', 'Chemistry', 'Biology'], English: ['English Literature', 'Writing Lab'], Arabic: ['Arabic Language', 'Arabic Grammar'], History: ['World History', 'Civics'] };

const err = (status, message) => Object.assign(new Error(message), { status });
const day = d => { const x = new Date(d); x.setHours(0, 0, 0, 0); return x; };
const iso = d => { const x = new Date(d); x.setMinutes(x.getMinutes() - x.getTimezoneOffset()); return x.toISOString().slice(0, 10); };

export function createMock() {
  const R = rng(42), pick = a => a[Math.floor(R() * a.length)], int = (a, b) => a + Math.floor(R() * (b - a + 1));
  let seq = 1000; const nid = () => ++seq;
  const now = new Date();

  const grades = Array.from({ length: 6 }, (_, i) => ({ id: i + 1, name: `Grade ${i + 7}`, level: i + 7 }));
  const teachers = Array.from({ length: 14 }, (_, i) => {
    const f = pick([...FIRST_M, ...FIRST_F]), l = pick(LAST);
    return { id: i + 1, name: `${f} ${l}`, email: `${f}.${l}@school.test`.toLowerCase(), phone: '010' + int(10000000, 99999999), specialization: SPECS[i % 5], hireDate: new Date(now.getFullYear() - int(1, 14), int(0, 11), int(1, 28)).toISOString() };
  });
  const classes = []; grades.forEach(g => ['A', 'B'].forEach((s, i) => classes.push({ id: classes.length + 1, name: `${g.level}${s}`, gradeId: g.id, classTeacherId: teachers[(classes.length) % teachers.length].id })));
  const courses = []; grades.forEach((g, gi) => SPECS.forEach((sp, si) => { const names = COURSE_NAMES[sp]; if ((gi + si) % 2 === 0 || si < 3) classes.filter(c => c.gradeId === g.id).forEach((cls, ci) => courses.push({ id: courses.length + 1, name: names[gi % names.length], code: (sp.slice(0, 3) + (g.level * 10 + si)).toUpperCase(), credits: int(1, 4), gradeId: g.id, classId: cls.id, teacherId: teachers.filter(t => t.specialization === sp)[(gi + ci) % teachers.filter(t => t.specialization === sp).length].id })); }));
  const students = [];
  classes.forEach(c => { for (let i = 0; i < 11; i++) { const male = R() > .5; const g = grades.find(x => x.id === c.gradeId); students.push({ id: students.length + 1, firstName: pick(male ? FIRST_M : FIRST_F), lastName: pick(LAST), birthDate: new Date(now.getFullYear() - (g.level + 5), int(0, 11), int(1, 28)).toISOString(), gender: male ? 'Male' : 'Female', classId: c.id }); } });
  students[0].firstName = 'Omar'; students[0].lastName = 'Hassan'; students[0].gender = 'Male';
  const attendances = [];
  for (let d = 1; d <= 24; d++) { const dt = day(now); dt.setDate(dt.getDate() - d); if ([5, 6].includes(dt.getDay())) continue; students.forEach(s => attendances.push({ id: nid(), studentId: s.id, classId: s.classId, date: dt.toISOString(), isPresent: R() > (0.1 + (s.id % 7 === 0 ? .25 : 0)) })); }
  const marks = [];
  students.forEach(s => { courses.filter(c => c.classId === s.classId).forEach(c => { if (R() > .15) marks.push({ id: nid(), studentId: s.id, courseId: c.id, teacherId: c.teacherId, score: Math.round((35 + R() * 65) * 10) / 10, date: new Date(now - int(1, 40) * 864e5).toISOString(), notes: pick(['Good effort', 'Needs improvement', 'Excellent work', '', 'Participates well', '']) }); }); });
  const homework = courses.slice(0, 18).map((c, i) => ({ id: i + 1, title: ['Chapter review quiz', 'Practice problems set', 'Reading + summary', 'Lab report', 'Revision video'][i % 5], description: 'Complete before the next lesson and bring your questions.', link: 'https://example.com/hw/' + (i + 1), courseId: c.id, teacherId: c.teacherId, createdAt: new Date(now - i * 864e5 * 1.5).toISOString() }));
  const notifications = [
    { id: 1, title: 'Welcome back', message: 'The new term starts Sunday. Check your timetable.', targetRole: 'All', createdAt: new Date(now - 864e5).toISOString() },
    { id: 2, title: 'Mid-term marks due', message: 'Teachers: submit mid-term marks by Thursday.', targetRole: 'Teacher', createdAt: new Date(now - 2 * 864e5).toISOString() },
    { id: 3, title: 'Library hours extended', message: 'The library now closes at 6 pm on weekdays.', targetRole: 'Student', createdAt: new Date(now - 3 * 864e5).toISOString() },
  ];
  const users = [{ id: 1, username: 'admin', role: 'Admin', password: 'Admin@123', createdAt: new Date(now - 90 * 864e5).toISOString() },
  { id: 2, username: 'teacher_ahmed', role: 'Teacher', teacherId: 1, password: 'Teacher@123', createdAt: new Date(now - 80 * 864e5).toISOString() },
  { id: 3, username: 'student_omar', role: 'Student', studentId: 1, password: 'Student@123', createdAt: new Date(now - 70 * 864e5).toISOString() }];
  teachers[0].name = 'Ahmed Hassan'; teachers[0].specialization = 'Math';
  // make the demo teacher own a class and a Math course
  classes[0].classTeacherId = 1; classes[1].classTeacherId = 1; courses.filter(c => c.name === 'Algebra' && c.classId === classes[0].id || c.name === 'Geometry' && c.classId === classes[1].id).forEach(c => c.teacherId = 1);
  marks.forEach(m => { m.teacherId = courses.find(c => c.id === m.courseId).teacherId; });

  // ---- projections ----
  const age = d => { const b = new Date(d); let a = now.getFullYear() - b.getFullYear(); if (new Date(now.getFullYear(), b.getMonth(), b.getDate()) > now) a--; return a; };
  const by = (arr, id) => arr.find(x => x.id === id);
  const dto = {
    student: s => ({ id: s.id, firstName: s.firstName, lastName: s.lastName, fullName: `${s.firstName} ${s.lastName}`, birthDate: s.birthDate, age: age(s.birthDate), gender: s.gender, classId: s.classId, className: by(classes, s.classId)?.name || '' }),
    teacher: t => ({ id: t.id, name: t.name, email: t.email, phone: t.phone, specialization: t.specialization, yearsOfExperience: age(t.hireDate) }),
    grade: g => ({ ...g }),
    cls: c => ({ id: c.id, name: c.name, gradeId: c.gradeId, classTeacherId: c.classTeacherId, gradeName: by(grades, c.gradeId)?.name || '', classTeacherName: by(teachers, c.classTeacherId)?.name || '' }),
    course: c => ({ ...c, gradeName: by(grades, c.gradeId)?.name || '', className: by(classes, c.classId)?.name || '', teacherName: by(teachers, c.teacherId)?.name || '' }),
    att: a => ({ ...a, studentName: dto.student(by(students, a.studentId) || { firstName: '?', lastName: '', birthDate: now }).fullName, className: by(classes, a.classId)?.name || '' }),
    mark: m => ({ ...m, studentName: dto.student(by(students, m.studentId)).fullName, courseName: by(courses, m.courseId)?.name || '', teacherName: by(teachers, m.teacherId)?.name || '' }),
    hw: x => ({ id: x.id, title: x.title, description: x.description, link: x.link, courseName: by(courses, x.courseId)?.name || '', teacherName: by(teachers, x.teacherId)?.name || '', createdAt: x.createdAt }),
    user: u => ({ id: u.id, username: u.username, role: u.role, studentId: u.studentId ?? null, teacherId: u.teacherId ?? null, createdAt: u.createdAt }),
  };
  const paged = (arr, q) => { const n = Math.max(1, +q.get('pageNumber') || 1), s = Math.min(50, +q.get('pageSize') || 10); return { data: arr.slice((n - 1) * s, n * s), pageNumber: n, pageSize: s, totalCount: arr.length, totalPages: Math.max(1, Math.ceil(arr.length / s)) }; };
  const has = (v, k) => String(v).toLowerCase().includes(String(k || '').toLowerCase());
  const countBy = (arr, fn) => arr.reduce((o, x) => { const k = fn(x); o[k] = (o[k] || 0) + 1; return o; }, {});

  const jwt = u => { const p = { 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role': u.role, 'http://schemas.microsoft.com/ws/2008/06/identity/claims/name': u.username, exp: Math.floor(Date.now() / 1000) + 3600 }; if (u.teacherId) p.TeacherId = String(u.teacherId); if (u.studentId) p.StudentId = String(u.studentId); const b = o => btoa(JSON.stringify(o)).replace(/=/g, '').replace(/\+/g, '-').replace(/\//g, '_'); return `${b({ alg: 'none' })}.${b(p)}.demo`; };

  // CRUD helper for simple tables
  function crud(name, arr, project, { onCreate, onUpdate, adminOnly = true, search } = {}) {
    return {
      name, arr, project, onCreate, onUpdate, adminOnly, search,
    };
  }
  const T = {
    students: crud('students', students, dto.student, { search: (s, k) => has(`${s.firstName} ${s.lastName} ${by(classes, s.classId)?.name}`, k), onCreate: b => ({ firstName: b.firstName, lastName: b.lastName, birthDate: b.birthDate, gender: b.gender === 1 || b.gender === 'Female' ? 'Female' : 'Male', classId: +b.classId }), onUpdate: b => ({ firstName: b.firstName, lastName: b.lastName, birthDate: b.birthDate, gender: b.gender === 1 || b.gender === 'Female' ? 'Female' : 'Male', classId: +b.classId }) }),
    teachers: crud('teachers', teachers, dto.teacher, { search: (t, k) => has(`${t.name} ${t.email} ${t.specialization}`, k), onCreate: b => ({ name: b.name, email: b.email, phone: b.phone, specialization: b.specialization, hireDate: b.hireDate }), onUpdate: b => ({ name: b.name, email: b.email, phone: b.phone, specialization: b.specialization, hireDate: b.hireDate }) }),
    courses: crud('courses', courses, dto.course, { search: (c, k) => has(`${c.name} ${c.code} ${by(classes, c.classId)?.name}`, k), onCreate: b => ({ name: b.name, code: b.code, credits: +b.credits, gradeId: +b.gradeId, classId: +b.classId, teacherId: +b.teacherId }), onUpdate: b => ({ name: b.name, code: b.code, credits: +b.credits, gradeId: +b.gradeId, classId: +b.classId, teacherId: +b.teacherId }) }),
    classes: crud('classes', classes, dto.cls, { search: (c, k) => has(c.name, k), onCreate: b => ({ name: b.name, gradeId: +b.gradeId, classTeacherId: +b.classTeacherId }), onUpdate: b => ({ name: b.name, gradeId: +b.gradeId, classTeacherId: +b.classTeacherId }) }),
    grades: crud('grades', grades, dto.grade, { search: (g, k) => has(g.name, k), onCreate: b => ({ name: b.name, level: +b.level }), onUpdate: b => ({ name: b.name, level: +b.level }) }),
  };
  const need = (u, ...roles) => { if (!u) throw err(401, 'Sign in first'); if (roles.length && !roles.includes(u.role)) throw err(403, "You don't have permission to do that."); };

  return async function handle(method, fullPath, body, u) {
    await new Promise(r => setTimeout(r, 90 + Math.random() * 120));
    const url = new URL(fullPath, 'http://x'); const p = url.pathname.replace(/\/+$/, ''); const q = url.searchParams; const seg = p.split('/').filter(Boolean);
    const [res, a, b2, c3] = seg;

    if (p === '/settings/school') {
      const key = 'cl.schoolSettings';
      if (method === 'GET') { try { const saved = localStorage.getItem(key); if (saved) return JSON.parse(saved); } catch { /* ignore unavailable/corrupt storage */ } return null; }
      if (method === 'PUT') { try { localStorage.setItem(key, JSON.stringify(body)); } catch { /* storage can be unavailable or full */ } return body; }
    }

    if (p === '/auth/login') {
      const usr = users.find(x => x.username === body.username && x.password === body.password);
      if (!usr) throw err(401, 'Invalid username or password');
      return { token: jwt(usr), role: usr.role, username: usr.username, teaacherId: usr.teacherId ?? null, studentId: usr.studentId ?? null };
    }
    need(u);

    // ----- generic tables -----
    if (T[res]) {
      const t = T[res];
      if (!a) {
        if (method === 'GET') return t.arr.map(t.project);
        if (method === 'POST') {
          need(u, 'Admin'); const row = { id: nid(), ...t.onCreate(body) };
          if (res === 'students' || res === 'teachers') { if (!body.username || !body.password) throw err(400, 'Username and password are required.'); if (users.some(x => x.username === body.username)) throw err(400, 'Username already exists'); users.push({ id: nid(), username: body.username, password: body.password, role: res === 'students' ? 'Student' : 'Teacher', [res === 'students' ? 'studentId' : 'teacherId']: row.id, createdAt: new Date().toISOString() }); }
          t.arr.push(row); return t.project(row);
        }
      }
      if (a === 'paged') return (() => { const r = paged(t.arr.map(t.project), q); return r; })();
      if (a === 'search') return t.arr.filter(x => t.search(x, q.get('keyword'))).map(t.project);
      if (a === 'count') return { count: t.arr.length };
      if (res === 'students' && a === 'me' && !b2) { need(u, 'Student'); return dto.student(by(students, u.studentId)); }
      if (res === 'students' && a === 'statistics' && b2 === 'count-by-class') return countBy(students, s => s.classId);
      if (res === 'teachers' && a === 'statistics' && b2 === 'count-by-specialization') return countBy(teachers, x => x.specialization);
      if (res === 'grades' && a === 'statistics' && b2 === 'student-count') return countBy(students, s => by(grades, by(classes, s.classId).gradeId).name);
      if (res === 'grades' && a === 'statistics' && b2 === 'class-count') return countBy(classes, c => by(grades, c.gradeId).name);
      if (res === 'teachers' && a === 'me') {
        need(u, 'Teacher');
        if (!b2) return dto.teacher(by(teachers, u.teacherId));
        if (b2 === 'classes' && !c3) return classes.filter(c => c.classTeacherId === u.teacherId || courses.some(course => course.teacherId === u.teacherId && course.classId === c.id)).map(dto.cls);
        if (b2 === 'classes' && seg[4] === 'students') { const cls = by(classes, +c3); if (!cls) throw err(404, 'Class not found'); if (cls.classTeacherId !== u.teacherId && !courses.some(course => course.teacherId === u.teacherId && course.classId === cls.id)) throw err(403, 'You are not assigned to this class'); return students.filter(s => s.classId === +c3).map(dto.student); }
      }
      if (res === 'students' && a === 'me' && b2 === 'attendances') return attendances.filter(x => x.studentId === u.studentId).map(dto.att);
      if (a && /^\d+$/.test(a)) {
        const row = by(t.arr, +a); if (!row) throw err(404, 'Not found');
        if (method === 'GET') return t.project(row);
        if (method === 'PUT') { if (res !== 'students') need(u, 'Admin'); Object.assign(row, t.onUpdate(body)); return t.project(row); }
        if (method === 'DELETE') { need(u, 'Admin'); t.arr.splice(t.arr.indexOf(row), 1); if (res === 'students') { [attendances, marks].forEach(l => { for (let i = l.length - 1; i >= 0; i--) if (l[i].studentId === row.id) l.splice(i, 1); }); } return {}; }
      }
    }
    if (res === 'students' && a === 'me' && b2 === 'attendances') return attendances.filter(x => x.studentId === u.studentId).map(dto.att);

    // ----- attendances -----
    if (res === 'attendances') {
      if (a === 'filter') {
        need(u, 'Admin', 'Teacher'); let rows = attendances.slice().sort((x, y) => new Date(y.date) - new Date(x.date) || x.id - y.id);
        if (q.get('date')) rows = rows.filter(x => iso(x.date) === q.get('date').slice(0, 10));
        if (q.get('classId')) rows = rows.filter(x => x.classId === +q.get('classId'));
        if (q.get('status')) rows = rows.filter(x => x.isPresent === (q.get('status') === 'present'));
        const r = paged(rows.map(dto.att), q); return r;
      }
      if (a === 'take') {
        need(u, 'Teacher'); const c = by(classes, body.classId); if (!c) throw err(404, 'Class not found'); if (c.classTeacherId !== u.teacherId && !courses.some(course => course.teacherId === u.teacherId && course.classId === c.id)) throw err(403, 'You are not assigned to this class');
        const d = iso(body.date);
        body.records.forEach(r => { const ex = attendances.find(x => x.studentId === r.studentId && x.classId === c.id && iso(x.date) === d); if (ex) ex.isPresent = r.isPresent; else attendances.push({ id: nid(), studentId: r.studentId, classId: c.id, date: day(body.date).toISOString(), isPresent: r.isPresent }); });
        return { message: 'Attendance recorded successfully' };
      }
      if (a === 'class') { need(u, 'Admin', 'Teacher'); const cls = by(classes, +b2); if (!cls) throw err(404, 'Class not found'); if (u.role === 'Teacher' && cls.classTeacherId !== u.teacherId && !courses.some(course => course.teacherId === u.teacherId && course.classId === cls.id)) throw err(403, 'You are not assigned to this class'); return attendances.filter(x => x.classId === +b2).map(dto.att); }
      if (a === 'student') return attendances.filter(x => x.studentId === +b2).map(dto.att);
      if (a === 'statistics' && b2 === 'percentage') { const rows = attendances.filter(x => iso(x.date) === c3.slice(0, 10)); return { percentage: rows.length ? Math.round(rows.filter(x => x.isPresent).length / rows.length * 1000) / 10 : 0 }; }
      if (a === 'statistics' && b2 === 'present') return { presentCount: attendances.filter(x => iso(x.date) === c3 && x.isPresent).length };
      if (a === 'statistics' && b2 === 'absent') return { absentCount: attendances.filter(x => iso(x.date) === c3 && !x.isPresent).length };
      if (!a && method === 'GET') { need(u, 'Admin', 'Teacher'); return attendances.map(dto.att); }
      if (!a && method === 'POST') { need(u, 'Admin', 'Teacher'); const row = { id: nid(), studentId: +body.studentId, classId: +body.classId, date: body.date, isPresent: !!body.isPresent }; attendances.push(row); return dto.att(row); }
      if (/^\d+$/.test(a)) {
        const row = by(attendances, +a); if (!row) throw err(404, 'Not found');
        if (method === 'GET') return dto.att(row);
        if (method === 'PUT') { need(u, 'Admin', 'Teacher'); Object.assign(row, { studentId: +body.studentId, classId: +body.classId, date: body.date, isPresent: !!body.isPresent }); return dto.att(row); }
        if (method === 'DELETE') { need(u, 'Admin'); attendances.splice(attendances.indexOf(row), 1); return {}; }
      }
    }

    // ----- marks -----
    if (res === 'marks') {
      const mine = () => marks.filter(m => m.teacherId === u.teacherId);
      if (a === 'paged') {
        need(u, 'Admin', 'Teacher'); let rows = u.role === 'Teacher' ? mine() : marks.slice();
        const k = (q.get('search') || '').toLowerCase();
        if (k) rows = rows.filter(m => `${dto.student(by(students, m.studentId)).fullName} ${by(courses, m.courseId)?.name}`.toLowerCase().includes(k));
        rows.sort((x, y) => new Date(y.date) - new Date(x.date));
        return paged(rows.map(dto.mark), q);
      }
      if (a === 'me') { need(u, 'Student'); return marks.filter(m => m.studentId === u.studentId).map(dto.mark); }
      if (a === 'my-marks') { need(u, 'Teacher'); return mine().map(dto.mark); }
      if (a === 'my-courses') { need(u, 'Teacher'); return courses.filter(c => c.teacherId === u.teacherId && c.classId).map(dto.course); }
      if (a === 'my-students') { need(u, 'Teacher'); const cl = [...new Set(courses.filter(c => c.teacherId === u.teacherId && c.classId).map(c => c.classId))]; return students.filter(s => cl.includes(s.classId)).map(dto.student); }
      if (!a && method === 'GET') { need(u, 'Admin', 'Teacher'); return marks.map(dto.mark); }
      if (!a && method === 'POST') {
        need(u, 'Admin', 'Teacher'); const tid = u.role === 'Teacher' ? u.teacherId : +body.teacherId; const c = by(courses, +body.courseId);
        if (!c) throw err(404, 'Course not found'); if (c.teacherId !== tid) throw err(403, 'You are not the teacher of this course'); if (by(students, +body.studentId)?.classId !== c.classId) throw err(400, "Student does not belong to this course's class");
        if (marks.some(m => m.studentId === +body.studentId && m.courseId === c.id)) throw err(400, 'Student already has a mark for this course. Please update it instead.');
        const row = { id: nid(), studentId: +body.studentId, courseId: c.id, teacherId: tid, score: +body.score, date: body.date, notes: body.notes || '' }; marks.push(row); return dto.mark(row);
      }
      if (/^\d+$/.test(a)) {
        const row = by(marks, +a); if (!row) throw err(404, 'Not found'); need(u, 'Admin', 'Teacher');
        if (u.role === 'Teacher' && row.teacherId !== u.teacherId) throw err(403, 'You can only change your own marks');
        if (method === 'PUT') { Object.assign(row, { score: +body.score, date: body.date, notes: body.notes }); return dto.mark(row); }
        if (method === 'DELETE') { marks.splice(marks.indexOf(row), 1); return { message: 'Mark deleted successfully' }; }
        return dto.mark(row);
      }
    }

    // ----- homework -----
    if (res === 'homework') {
      if (a === 'my-homework') { need(u, 'Teacher'); return homework.filter(x => x.teacherId === u.teacherId).map(dto.hw).reverse(); }
      if (a === 'student') { need(u, 'Student'); const s = by(students, u.studentId), g = by(classes, s.classId).gradeId; return homework.filter(x => by(courses, x.courseId).gradeId === g).map(dto.hw).reverse(); }
      if (!a && method === 'POST') { need(u, 'Teacher'); if (!body.title || !body.link) throw err(400, 'Title and Link are required.'); homework.push({ id: nid(), title: body.title, description: body.description, link: body.link, courseId: +body.courseId, teacherId: u.teacherId, createdAt: new Date().toISOString() }); return { message: 'Homework added successfully!' }; }
      if (method === 'DELETE') { need(u, 'Admin', 'Teacher'); const i = homework.findIndex(x => x.id === +a); if (i >= 0) homework.splice(i, 1); return { message: 'Deleted successfully' }; }
    }

    // ----- notifications -----
    if (res === 'notifications') {
      if (!a && method === 'GET') return notifications.filter(n => u.role === 'Admin' || n.targetRole === 'All' || n.targetRole === u.role).slice().reverse();
      if (!a && method === 'POST') { need(u, 'Admin'); if (!body.title || !body.message) throw err(400, 'Title and Message are required.'); notifications.push({ id: nid(), title: body.title, message: body.message, targetRole: body.targetRole || 'All', createdAt: new Date().toISOString() }); return { message: 'Notification sent successfully!' }; }
      if (method === 'DELETE') { need(u, 'Admin'); const i = notifications.findIndex(n => n.id === +a); if (i >= 0) notifications.splice(i, 1); return {}; }
    }

    // ----- users -----
    if (res === 'users') {
      need(u, 'Admin');
      if (!a) return users.map(dto.user);
      if (b2 === 'reset-password') { const x = by(users, +a); if (!x) throw err(404, 'User not found'); if (x.role === 'Admin') throw err(400, 'Cannot reset password for Admin accounts. Please contact system support.'); if (!body.newPassword || body.newPassword.length < 6) throw err(400, 'Password must be at least 6 characters.'); x.password = body.newPassword; return { message: 'Password reset successfully!' }; }
    }
    throw err(404, `Demo API: no route for ${method} ${p}`);
  };
}
