/* =========================================================
   FINANCE MANAGEMENT SYSTEM
   ========================================================= */


/* =========================================================
   1. USERS
   ========================================================= */

CREATE TABLE Users
(
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(150) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,

    Role NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Users_Role DEFAULT 'User',

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_Users_Role
        CHECK (Role IN ('User', 'Admin'))
);


CREATE TABLE AdminActivities
(
    AdminActivityId INT IDENTITY(1,1) PRIMARY KEY,
    AdminUserId INT NOT NULL,
    Action NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(50) NOT NULL,
    EntityId INT NULL,
    Description NVARCHAR(500) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT FK_AdminActivities_Users
        FOREIGN KEY (AdminUserId)
        REFERENCES Users(UserId)
);



/* =========================================================
   2. ACCOUNTS
   ========================================================= */

CREATE TABLE Accounts
(
    AccountId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    AccountName NVARCHAR(100) NOT NULL,
    AccountType NVARCHAR(30) NOT NULL,

    Balance DECIMAL(18,2) NOT NULL
        CONSTRAINT DF_Accounts_Balance DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Accounts_CreatedAt DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Accounts_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId),

    CONSTRAINT CK_Accounts_Balance
        CHECK (Balance >= 0),

    CONSTRAINT CK_Accounts_Type
        CHECK (AccountType IN ('Savings', 'Checking', 'Cash')),

    CONSTRAINT UQ_Accounts_User_Name
        UNIQUE (UserId, AccountName)
);


/* =========================================================
   3. CATEGORIES
   ========================================================= */

CREATE TABLE Categories
(
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    Name NVARCHAR(100) NOT NULL,
    Type NVARCHAR(20) NOT NULL,

    CONSTRAINT FK_Categories_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId),

    CONSTRAINT CK_Categories_Type
        CHECK (Type IN ('Income', 'Expense')),

    CONSTRAINT UQ_Categories_User_Name_Type
        UNIQUE (UserId, Name, Type)
);


/* =========================================================
   4. TRANSFERS
   ========================================================= */

CREATE TABLE Transfers
(
    TransferId INT IDENTITY(1,1) PRIMARY KEY,

    FromAccountId INT NOT NULL,
    ToAccountId INT NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    TransferDate DATETIME2 NOT NULL
        CONSTRAINT DF_Transfers_TransferDate
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Transfers_FromAccount
        FOREIGN KEY (FromAccountId)
        REFERENCES Accounts(AccountId),

    CONSTRAINT FK_Transfers_ToAccount
        FOREIGN KEY (ToAccountId)
        REFERENCES Accounts(AccountId),

    CONSTRAINT CK_Transfers_Amount
        CHECK (Amount > 0),

    CONSTRAINT CK_Transfers_DifferentAccounts
        CHECK (FromAccountId <> ToAccountId)
);


/* =========================================================
   5. LOAN PRODUCTS
   ========================================================= */

CREATE TABLE LoanProducts
(
    LoanProductId INT IDENTITY(1,1) PRIMARY KEY,

    Name NVARCHAR(100) NOT NULL,

    Description NVARCHAR(500) NULL,

    InterestRate DECIMAL(5,2) NOT NULL,

    InterestType NVARCHAR(20) NOT NULL,

    MaxAmount DECIMAL(18,2) NOT NULL,

    MaxTenureMonths INT NOT NULL,

    IsActive BIT NOT NULL
        CONSTRAINT DF_LoanProducts_IsActive DEFAULT 1,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_LoanProducts_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT CK_LoanProducts_Rate
        CHECK (InterestRate >= 0),

    CONSTRAINT CK_LoanProducts_MaxAmount
        CHECK (MaxAmount > 0),

    CONSTRAINT CK_LoanProducts_MaxTenure
        CHECK (MaxTenureMonths > 0),

    CONSTRAINT CK_LoanProducts_InterestType
        CHECK (InterestType IN ('Simple', 'Compound'))
);


/* =========================================================
   6. LOANS
   ========================================================= */

CREATE TABLE Loans
(
    LoanId INT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NOT NULL,
AccountId INT NOT NULL,
LoanProductId INT NOT NULL,

PrincipalAmount DECIMAL(18,2) NOT NULL,

    InterestRate DECIMAL(5,2) NOT NULL,

    InterestType NVARCHAR(20) NOT NULL,

    TenureMonths INT NOT NULL,

    TotalInterest DECIMAL(18,2) NOT NULL,

    TotalPayable DECIMAL(18,2) NOT NULL,

    OutstandingAmount DECIMAL(18,2) NOT NULL,

    Status NVARCHAR(20) NOT NULL
        CONSTRAINT DF_Loans_Status DEFAULT 'Pending',

    StartDate DATE NULL,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Loans_CreatedAt DEFAULT SYSUTCDATETIME(),

    ApprovedAt DATETIME2 NULL,

    CONSTRAINT FK_Loans_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId),

    CONSTRAINT FK_Loans_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId),

        CONSTRAINT FK_Loans_LoanProducts
    FOREIGN KEY (LoanProductId)
    REFERENCES LoanProducts(LoanProductId),

    CONSTRAINT CK_Loans_Principal
        CHECK (PrincipalAmount > 0),

    CONSTRAINT CK_Loans_Rate
        CHECK (InterestRate >= 0),

    CONSTRAINT CK_Loans_Tenure
        CHECK (TenureMonths > 0),

    CONSTRAINT CK_Loans_TotalInterest
        CHECK (TotalInterest >= 0),

    CONSTRAINT CK_Loans_TotalPayable
        CHECK (TotalPayable >= PrincipalAmount),

    CONSTRAINT CK_Loans_Outstanding
        CHECK (OutstandingAmount >= 0),

    CONSTRAINT CK_Loans_InterestType
        CHECK (InterestType IN ('Simple', 'Compound')),

    CONSTRAINT CK_Loans_Status
        CHECK
        (
            Status IN
            (
                'Pending',
                'Approved',
                'Rejected',
                'Active',
                'Paid'
            )
        )
);


/* =========================================================
   7. LOAN PAYMENTS
   ========================================================= */

CREATE TABLE LoanPayments
(
    PaymentId INT IDENTITY(1,1) PRIMARY KEY,

    LoanId INT NOT NULL,
    AccountId INT NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    PrincipalPaid DECIMAL(18,2) NOT NULL,
    InterestPaid DECIMAL(18,2) NOT NULL,

    PaymentDate DATETIME2 NOT NULL
        CONSTRAINT DF_LoanPayments_PaymentDate
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_LoanPayments_Loans
        FOREIGN KEY (LoanId)
        REFERENCES Loans(LoanId),

    CONSTRAINT FK_LoanPayments_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId),

    CONSTRAINT CK_LoanPayments_Amount
        CHECK (Amount > 0),

    CONSTRAINT CK_LoanPayments_PrincipalPaid
        CHECK (PrincipalPaid >= 0),

    CONSTRAINT CK_LoanPayments_InterestPaid
        CHECK (InterestPaid >= 0),

    CONSTRAINT CK_LoanPayments_Split
        CHECK (Amount = PrincipalPaid + InterestPaid)
);


/* =========================================================
   8. NOTIFICATIONS
   ========================================================= */

CREATE TABLE Notifications
(
    NotificationId INT IDENTITY(1,1) PRIMARY KEY,

    UserId INT NOT NULL,

    Title NVARCHAR(150) NOT NULL,
    Message NVARCHAR(500) NOT NULL,

    NotificationType NVARCHAR(30) NOT NULL,

    IsRead BIT NOT NULL
        CONSTRAINT DF_Notifications_IsRead DEFAULT 0,

    CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Notifications_CreatedAt
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Notifications_Users
        FOREIGN KEY (UserId)
        REFERENCES Users(UserId),

    CONSTRAINT CK_Notifications_Type
        CHECK
        (
            NotificationType IN
            (
                'LoanApplication',
                'LoanApproval',
                'LoanRejection',
                'LoanPayment',
                'LoanPaid',
                'System'
            )
        )
);


/* =========================================================
   9. TRANSACTIONS
   ========================================================= */

CREATE TABLE Transactions
(
    TransactionId INT IDENTITY(1,1) PRIMARY KEY,

    AccountId INT NOT NULL,

    CategoryId INT NULL,
    TransferId INT NULL,
    LoanId INT NULL,
    LoanPaymentId INT NULL,

    TransactionType NVARCHAR(30) NOT NULL,

    Amount DECIMAL(18,2) NOT NULL,

    Description NVARCHAR(250) NULL,

    TransactionDate DATETIME2 NOT NULL
        CONSTRAINT DF_Transactions_TransactionDate
        DEFAULT SYSUTCDATETIME(),

    CONSTRAINT FK_Transactions_Accounts
        FOREIGN KEY (AccountId)
        REFERENCES Accounts(AccountId),

    CONSTRAINT FK_Transactions_Categories
        FOREIGN KEY (CategoryId)
        REFERENCES Categories(CategoryId),

    CONSTRAINT FK_Transactions_Transfers
        FOREIGN KEY (TransferId)
        REFERENCES Transfers(TransferId),

    CONSTRAINT FK_Transactions_Loans
        FOREIGN KEY (LoanId)
        REFERENCES Loans(LoanId),

    CONSTRAINT FK_Transactions_LoanPayments
        FOREIGN KEY (LoanPaymentId)
        REFERENCES LoanPayments(PaymentId),

    CONSTRAINT CK_Transactions_Amount
        CHECK (Amount > 0),

    CONSTRAINT CK_Transactions_Type
        CHECK
        (
            TransactionType IN
            (
                'Deposit',
                'Withdrawal',
                'Income',
                'Expense',
                'Transfer',
                'LoanDisbursement',
                'LoanPayment'
            )
        )
);


/* =========================================================
   INDEXES
   ========================================================= */

CREATE INDEX IX_Transactions_AccountId_Date
ON Transactions(AccountId, TransactionDate DESC);

CREATE INDEX IX_Loans_UserId_Status
ON Loans(UserId, Status);

CREATE INDEX IX_LoanPayments_LoanId_Date
ON LoanPayments(LoanId, PaymentDate DESC);

CREATE INDEX IX_Notifications_UserId_Read
ON Notifications(UserId, IsRead, CreatedAt DESC);


