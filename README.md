# 📚 Academix – Next-Gen E-Learning Platform

Academix is a modern e-learning platform designed to enhance online education through interactive features, real-time communication, and advanced analytics. Built with **.NET 9**, **Next.js**, and **SQL Server**, it provides a scalable solution for students, teachers, organizations, and admins.

---

## 🚀 Key Features

### 👨‍🎓 Student
- Submit assignments, take exams, view scores, track history.
- Access learning materials (documents & videos).
- Engage with **comments, likes/dislikes**, and a built-in **chatbot**.
- **Notifications**: new exams, upcoming deadlines, new materials, published grades.
- **Progress tracking**: percentage of completed content/tests.
- **Leaderboard** for competitive motivation.
- **Anti-cheating**: track focus, copy-paste warnings, optional face detection.
- **Q&A forum** for public class discussions.
- **Gamification**: earn points for completing activities.

### 👩‍🏫 Teacher
- Manage students, create classes, upload/update materials.
- Create exams using a **Question Bank** (MCQ, essay, matching, fill-in).
- **Grading**: auto-grading for MCQ, manual or AI-assisted grading for essays.
- **Analytics**: performance by class, question-level insights.
- Advanced class management: groups, group assignments, online attendance.
- **Document versioning**: notify students when new versions are available.
- **Cheating alerts**: highlight suspicious student behavior.

### 🏢 Organization
- Manage teachers & students, open classes, handle **subscriptions**.
- License tracking & annual billing.
- Financial management: payment history, e-invoices.
- High-level reports: classes, users, engagement, performance.
- **White-label support**: custom branding, domain, color theme.
- Privacy & security policy management.

### 🛠️ Admin
- Global management of users, organizations, and features.
- Revenue dashboard: by month, plan, country/region.
- Subscription management: renewal, expiration, suspension.
- Support & complaint system (ticket-based).
- System configuration & feature toggling.
- **Audit logs** for monitoring activities.

### 🌍 Common Features
- Profile management, password change, **2FA**.
- **Real-time notifications** (SignalR / WebSocket).
- **Advanced search**: across documents, classes, videos, exams.
- **Dark mode** & **multi-language** support.

---

## 🏗️ Tech Stack

- **Frontend**: Next.js (React), TailwindCSS
- **Backend**: .NET 9, ASP.NET Core, EF Core
- **Database**: SQL Server
- **Real-time**: SignalR / WebSocket
- **Authentication**: OAuth2, JWT, 2FA
- **Deployment**: Docker, Docker Compose

---

## 📂 Project Structure

```bash
Academix/
├── backend/                # .NET 9 Web API
│   ├── Academix.API/       # Controllers, middleware
│   ├── Academix.Core/      # Domain models, business logic
│   ├── Academix.Infrastructure/ # EF Core, repositories, migrations
│   ├── Academix.Tests/     # Unit & integration tests
│   └── Dockerfile
│
├── frontend/               # Next.js app
│   ├── components/         # Reusable UI components
│   ├── pages/              # Next.js pages (routing)
│   ├── services/           # API calls & auth handlers
│   ├── styles/             # Global styles, Tailwind config
│   └── Dockerfile
│
├── database/               # SQL Server schema & seed data
│   └── init.sql
│
├── docker-compose.yml      # Multi-container setup
├── .gitignore
└── README.md
```
