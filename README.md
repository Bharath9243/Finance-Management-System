# 💰 Finance Management System

A full-stack **personal finance and loan management web application** built with **Blazor Web App, .NET 10, Dapper, and Microsoft SQL Server**.

The application provides a centralized platform for users to manage their accounts, income, expenses, transfers, and loans, while administrators can manage loan products, review loan applications, monitor active loans, and track administrative activities.

---

## 🚀 Overview

The **Finance Management System** is designed to simulate a real-world financial management platform with separate experiences for **Users** and **Administrators**.

### Users can:

- Manage multiple financial accounts
- Track income and expenses
- Transfer money between accounts
- Apply for and manage loans
- Make partial or full loan payments
- Monitor outstanding loan balances
- Receive financial notifications
- View financial activity through dashboards

### Administrators can:

- Monitor overall system activity
- Manage users
- Create and manage loan products
- Review loan applications
- Approve or reject loan applications
- Monitor active loans
- View administrative activity history

The application uses **database transactions** for critical financial operations to maintain consistency between account balances, transactions, loans, payments, and notifications.

---

## ✨ Features

### 👤 User Features

#### Authentication & Account Management

- User registration
- Secure login and logout
- Cookie-based authentication
- Role-based authorization
- Password hashing using BCrypt
- Forgot password functionality
- Password reset workflow
- User profile management

#### 💳 Financial Accounts

- Create and manage accounts
- Supported account types:
  - Savings
  - Checking
  - Cash
- View account balances
- Deposit money
- Withdraw money
- Prevent invalid withdrawals

#### 💵 Income & Expenses

- Record income
- Record expenses
- Categorize financial transactions
- View transaction history
- Track financial activity by account
- Separate income and expense categories

#### 🔄 Money Transfers

- Transfer money between personal accounts
- Validate source and destination accounts
- Validate available balance
- Record transfer history
- Create corresponding transaction records

#### 🏦 Loan Management

- Browse available loan products
- Apply for loans
- View loan details
- Track loan status
- View outstanding balance
- Make partial loan payments
- Make full loan payments
- View payment history
- Automatic loan balance updates

#### 🔔 Notifications

Users receive notifications for important events such as:

- Loan applications
- Loan approvals
- Loan rejections
- Loan payments
- Fully paid loans
- System notifications

#### 📊 User Dashboard

The user dashboard provides an overview of:

- Account balances
- Income
- Expenses
- Transactions
- Loans
- Outstanding loan amounts
- Financial activity

---

# 🛡️ Admin Features

Administrators have a dedicated management area.

### 📊 Admin Dashboard

Provides system-level information and financial statistics.

### 👥 User Management

Administrators can:

- View registered users
- View individual user details
- Inspect user accounts
- Review user-related financial information

### 🏦 Loan Product Management

Administrators can:

- Create loan products
- Update loan products
- Activate/deactivate loan products
- Configure:
  - Interest rate
  - Interest type
  - Maximum loan amount
  - Maximum tenure

Supported interest types:

- Simple Interest
- Compound Interest

### 📋 Loan Application Management

Administrators can:

- View pending applications
- Review loan details
- Approve applications
- Reject applications
- Monitor approved and active loans

### 💳 Active Loan Management

Administrators can monitor:

- Active loans
- Principal amount
- Interest
- Total payable amount
- Outstanding balance
- Loan status
- Payment activity

### 📝 Activity History

Important administrator actions are recorded through an activity history system for better traceability.

---

# 🔐 Security

The application implements several security mechanisms.

### Authentication

Authentication is implemented using:

- ASP.NET Core Cookie Authentication
- Claims-based identity
- Role-based authorization

### Password Security

Passwords are never stored as plain text.

The application uses:

**BCrypt.Net-Next**

for password hashing and verification.

### Role-Based Access

The system separates access between:

~~~text
User
Admin
~~~

Administrative pages are protected from regular users.

---

# 🔄 Financial Transaction Safety

Financial operations often involve multiple database changes.

For example, a loan payment may require:

~~~text
Account Balance Update
        ↓
Loan Payment Record
        ↓
Loan Balance Update
        ↓
Transaction Record
        ↓
Notification
~~~

These operations are handled using database transactions where appropriate so that related changes are committed together.

If an operation fails, the transaction can be rolled back instead of leaving the system in an inconsistent state.

Examples include:

- Money transfers
- Loan disbursement
- Loan payments
- Account balance updates
- Related transaction creation

---

# 🏗️ Architecture

The application follows a layered architecture separating UI, business logic, and data access.

~~~text
┌─────────────────────────────────────┐
│        Blazor Web App / UI          │
│        Razor Components             │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│             Services                │
│          Business Logic             │
│                                     │
│  Account │ Loan │ Transaction       │
│  Payment │ Auth │ Notification      │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│           Repositories              │
│             Dapper                  │
│                                     │
│  CRUD + SQL Queries + Transactions  │
└──────────────────┬──────────────────┘
                   │
                   ▼
┌─────────────────────────────────────┐
│           SQL Server                │
│       FinanceManagementDB           │
└─────────────────────────────────────┘
~~~

# 🛠️ Technology Stack

| Technology                      | Purpose                       |
| ------------------------------- | ----------------------------- |
| **.NET 10**                     | Application framework         |
| **Blazor Web App**              | User interface                |
| **Interactive Server**          | Server-side interactive UI    |
| **C#**                          | Application development       |
| **Dapper**                      | Data access / micro ORM       |
| **Microsoft SQL Server**        | Relational database           |
| **Microsoft.Data.SqlClient**    | SQL Server connectivity       |
| **BCrypt.Net-Next**             | Password hashing              |
| **ASP.NET Core Authentication** | Authentication                |
| **Cookie Authentication**       | Session authentication        |
| **Dependency Injection**        | Service/repository management |

---

# 🗄️ Database Design

The application uses **Microsoft SQL Server** with relational constraints and indexes.

### Main Tables

~~~text
Users
  │
  ├── Accounts
  │      │
  │      ├── Transactions
  │      └── Transfers
  │
  ├── Categories
  │
  ├── Loans
  │      │
  │      └── LoanPayments
  │
  ├── Notifications
  │
  └── PasswordResetTokens

LoanProducts
      │
      └── Loans

AdminActivities
~~~

# ⚙️ Getting Started

## Prerequisites

Install the following:

- **.NET 10 SDK**
- **SQL Server** or **SQL Server LocalDB**
- **Visual Studio 2022/2026** with ASP.NET and web development support
- Git

Verify .NET installation:

~~~bash
dotnet --version
~~~

---

# 📥 Installation

### 1. Clone the repository

~~~bash
git clone https://github.com/Bharath9243/Finance-Management-System.git
~~~

### 2. Navigate to the project

~~~bash
cd Finance-Management-System
~~~

Then navigate to the application directory:

~~~bash
cd FinanceManagementSystem
~~~

### 3. Restore dependencies

~~~bash
dotnet restore
~~~

---

# 🗄️ Database Setup

The project contains:

~~~text
Database/
├── Schema.sql
└── Seed.sql
~~~

---

# 🔁 Application Workflow

## User Financial Workflow

~~~text
Register / Login
       ↓
User Dashboard
       ↓
Create Account
       ↓
Add Income / Expense
       ↓
Manage Transactions
       ↓
Transfer Between Accounts
       ↓
Apply for Loan
       ↓
Loan Approval
       ↓
Loan Disbursement
       ↓
Make Loan Payments
       ↓
Loan Fully Paid
~~~

## Admin Loan Workflow

~~~text
Create Loan Product
        ↓
User Applies for Loan
        ↓
Pending Loan
        ↓
Admin Reviews Application
        ↓
   ┌────┴────┐
   ↓         ↓
Approve    Reject
   ↓
Loan Disbursement
   ↓
Active Loan
   ↓
User Makes Payments
   ↓
Outstanding = 0
   ↓
Paid
~~~

---

# 📊 Key Design Decisions

### Dapper Instead of a Full ORM

Dapper is used for data access to maintain direct control over SQL queries while keeping database operations lightweight.

### Repository Pattern

Repositories isolate SQL/database operations from business logic.

For example:

~~~text
LoanRepository
LoanPaymentRepository
AccountRepository
TransactionRepository
~~~

### Service Layer

Services contain business rules and coordinate repository operations.

For example:

~~~text
LoanService
LoanPaymentService
AccountService
TransactionService
~~~

This prevents business logic from being tightly coupled to Razor components.

### Dependency Injection

Repositories and services are registered through ASP.NET Core's built-in dependency injection container.

---

# 🧠 Important Technical Concepts Demonstrated

This project demonstrates practical implementation of:

- Blazor Interactive Server
- ASP.NET Core
- C#
- Dependency Injection
- Repository Pattern
- Service Layer Architecture
- Dapper
- SQL Server
- Relational database design
- Foreign key relationships
- Database constraints
- Database transactions
- Authentication
- Authorization
- Claims
- Role-based access control
- Password hashing
- CRUD operations
- Financial transaction processing
- Loan calculations
- Payment allocation
- Notification systems
- Error handling
- Validation

---

# 🧪 Development & Testing

Before deploying the application, test the following scenarios:

### Authentication

- Register user
- Login
- Logout
- Invalid login
- Password reset
- Role-based access

### Accounts

- Create account
- Deposit
- Withdraw
- Insufficient balance
- Transfer between accounts

### Transactions

- Income
- Expense
- Categories
- Transaction history

### Loans

- Apply for loan
- Approve loan
- Reject loan
- Loan disbursement
- Partial payment
- Full payment
- Outstanding balance
- Loan completion

### Admin

- User management
- Loan product management
- Pending loan review
- Active loan management
- Activity history

---

# 📌 Project Highlights

The project focuses on building a realistic financial application rather than only implementing basic CRUD functionality.

Key areas include:

**Financial consistency**

> Account balances, transactions, transfers, loan payments, and loan balances are kept synchronized during financial operations.

**Layered architecture**

> UI, business logic, and database access are separated using Razor Components, Services, and Repositories.

**Security**

> Authentication, authorization, role-based access, and BCrypt password hashing are implemented.

**Loan lifecycle**

> The complete workflow from loan product creation to application, approval, disbursement, payment, and completion is supported.

**Database integrity**

> Foreign keys, check constraints, unique constraints, defaults, and indexes are used to enforce data integrity.

---

# 📄 License

This project is intended for educational and portfolio purposes.
