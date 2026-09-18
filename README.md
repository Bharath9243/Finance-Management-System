# Finance Management System

A personal finance and loan management web application built with **Blazor Web App**, **.NET 10**, **Dapper**, and **SQL Server**.

The system allows users to manage accounts, income, expenses, transfers, and loans, while administrators can review loan applications, manage loan products, and monitor important system activity.

## Features

### User
- User registration and login
- Role-based authentication and authorization
- Manage savings, checking, and cash accounts
- Deposit and withdraw funds
- Transfer money between own accounts
- Record and view income and expenses
- Manage transaction categories
- Apply for loans from active loan products
- View loan details and payment history
- Make partial or full loan payments
- Track outstanding loan amount
- View in-app notifications
- Personal finance dashboard

### Admin
- Admin dashboard with system-level statistics
- View registered users
- View user account details
- Create and update loan products
- Activate/deactivate loan products
- Review pending loan applications
- Approve or reject loan applications
- Disburse approved loans into user accounts
- View active loans
- View admin activity history

## Loan Management

The system supports:

- Simple interest
- Compound interest calculated monthly
- Configurable interest rates
- Maximum loan amount and tenure based on loan products
- Partial and full payments
- Interest-first payment allocation
- Automatic outstanding balance tracking
- Loan status flow:
  - Pending
  - Active
  - Paid
  - Rejected

Loan approval/disbursement and loan payments use database transactions so related financial changes are committed together or rolled back on failure.

## Transaction Safety

Financial operations are designed to maintain consistent account balances.

Examples include:

- Deposit → account balance update + transaction record
- Withdrawal → balance validation + balance update + transaction record
- Transfer → source debit + destination credit + transfer record + transaction records
- Loan disbursement → loan update + account credit + transaction + notification
- Loan payment → account debit + payment allocation + loan update + transaction + notification

Database transactions are used for multi-step financial operations to prevent partial updates when an operation fails.

## Architecture

The application follows a layered architecture:

```text
Blazor Web App / Razor Components
            │
            ▼
        Services
   (Business Logic)
            │
            ▼
       Repositories
      (Dapper / SQL)
            │
            ▼
        SQL Server
