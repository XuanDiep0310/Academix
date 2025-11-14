# Academix - Online Classroom Platform

Academix is an Online Classroom Platform designed to support class management, learning materials, multiple-choice quizzes, and tracking academic results.

## 🚀 Technology Stack

The system is built on a client-server model using modern technologies:

* **Backend:** .NET (Provides APIs secured with JWT)
* **Frontend:** React.js
* **Database:** SQL Server
* **Storage:** File storage on a free cloud platform (e.g., Firebase Storage, Cloudinary, or Google Drive API)

## ✨ Key Features

* **Authentication & Authorization:**
    * Robust authentication system using Access Tokens (JWT) and Refresh Tokens.
    * Supports: Login, Logout, Change Password, Forgot/Reset Password.
    * Clear Role-Based Access Control (RBAC): Admin, Teacher, Student.
* **Class Management:** Allows Admins to create classes, Teachers to manage them, and Students to join.
* **Material Management:** Teachers can upload materials (PDF, links, videos...), and Students can view/download them.
* **Multiple-Choice Quizzes:**
    * Teachers manage a question bank.
    * Teachers create quizzes from the question bank and assign them to classes.
    * Students take quizzes with a countdown timer.
* **Result Management:**
    * Students can view their scores, correct/incorrect counts, and detailed answers.
    * Teachers can view submissions, scores, and export results to an Excel file.

## 🧑‍💻 Roles and Functions

The system has 3 primary roles:

### 1. Admin
The system administrator.

* **Account Management:** View, add, edit, delete, or lock Teacher and Student accounts.
* **Class Management:** Create new classes (name, description, class code) and view all classes in the system.
* **Member Management:** Add Teachers and Students to classes.

### 2. Teacher
The instructor, responsible for course content.

* **Class Management:** View the list of classes they teach and the list of students in each class.
* **Learning Material Management:** Add, edit, or delete materials (PDF, link, image, video).
* **Question Management:** Build and manage a bank of multiple-choice questions.
* **Quiz Creation:** Create quizzes by selecting questions, setting a title, and scheduling release time for a class.
* **View Results:** View submissions, grades, submission status, and export to Excel.

### 3. Student
The learner, participating in the courses.

* **Classes:** View the list of classes they are enrolled in.
* **Materials:** View and download course materials.
* **Take Quizzes:** Complete multiple-choice quizzes with a countdown timer and submit.
* **View Results:** View scores, correct/incorrect counts, and detailed answers after completion.

## 📋 Non-functional Requirements

* **Performance:** API response < 1 second; system supports $\ge100$ concurrent users.
* **Security:** Passwords hashed using bcrypt; authentication via JWT (Access & Refresh Token).
* **UI/UX:** Friendly, modern, responsive design; easy to use on desktop and mobile.
* **Maintainability:** Code is well-organized, easy to upgrade and deploy.
