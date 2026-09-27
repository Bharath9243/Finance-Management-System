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

# 🧮 Loan Management

The application contains a dedicated loan calculation and management workflow.

## Supported Interest Types

### Simple Interest

The system supports simple-interest calculations based on:

```text
Interest = Principal × Rate × Time



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
