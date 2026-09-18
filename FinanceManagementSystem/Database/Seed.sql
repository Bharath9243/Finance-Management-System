USE FinanceManagementDB;
GO

BEGIN TRANSACTION;

BEGIN TRY

    ------------------------------------------------------------
    -- 1. Create Admin User
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Users
        WHERE Email = 'admin@finance.com'
    )
    BEGIN
        INSERT INTO Users
        (
            Name,
            Email,
            PasswordHash,
            Role
        )
        VALUES
        (
            'Finance Admin',
            'admin@finance.com',
            '$2a$11$hSR7k2A/7ezboEDQnJaQYOMxQXVk2kjZrCf7Ni8cAHrOR3Ud5I5F2',
            'Admin'
        );
    END
    ELSE
    BEGIN
        -- Make sure the seeded admin account has Admin role.
        UPDATE Users
        SET Role = 'Admin'
        WHERE Email = 'admin@finance.com';
    END;


    ------------------------------------------------------------
    -- 2. Create Demo User
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Users
        WHERE Email = 'demo@finance.com'
    )
    BEGIN
        INSERT INTO Users
        (
            Name,
            Email,
            PasswordHash,
            Role
        )
        VALUES
        (
            'Demo User',
            'demo@finance.com',
            '$2a$11$bCnLjpV8xbOCIOnfkw8i4eyzQDdnFZ0XE8f4GeWCtncJedqPFLydO',
            'User'
        );
    END;


    ------------------------------------------------------------
    -- 3. Get Demo User ID
    ------------------------------------------------------------

    DECLARE @DemoUserId INT;

    SELECT @DemoUserId = UserId
    FROM Users
    WHERE Email = 'demo@finance.com';


    ------------------------------------------------------------
    -- 4. Create Demo Categories
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Categories
        WHERE UserId = @DemoUserId
          AND Name = 'Salary'
          AND Type = 'Income'
    )
    BEGIN
        INSERT INTO Categories
        (
            UserId,
            Name,
            Type
        )
        VALUES
        (
            @DemoUserId,
            'Salary',
            'Income'
        );
    END;


    IF NOT EXISTS
    (
        SELECT 1
        FROM Categories
        WHERE UserId = @DemoUserId
          AND Name = 'Food'
          AND Type = 'Expense'
    )
    BEGIN
        INSERT INTO Categories
        (
            UserId,
            Name,
            Type
        )
        VALUES
        (
            @DemoUserId,
            'Food',
            'Expense'
        );
    END;


    IF NOT EXISTS
    (
        SELECT 1
        FROM Categories
        WHERE UserId = @DemoUserId
          AND Name = 'Transport'
          AND Type = 'Expense'
    )
    BEGIN
        INSERT INTO Categories
        (
            UserId,
            Name,
            Type
        )
        VALUES
        (
            @DemoUserId,
            'Transport',
            'Expense'
        );
    END;


    ------------------------------------------------------------
    -- 5. Create Demo Accounts
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Accounts
        WHERE UserId = @DemoUserId
          AND AccountName = 'Demo Savings'
    )
    BEGIN
        INSERT INTO Accounts
        (
            UserId,
            AccountName,
            AccountType,
            Balance
        )
        VALUES
        (
            @DemoUserId,
            'Demo Savings',
            'Savings',
            50000.00
        );
    END;


    IF NOT EXISTS
    (
        SELECT 1
        FROM Accounts
        WHERE UserId = @DemoUserId
          AND AccountName = 'Demo Cash'
    )
    BEGIN
        INSERT INTO Accounts
        (
            UserId,
            AccountName,
            AccountType,
            Balance
        )
        VALUES
        (
            @DemoUserId,
            'Demo Cash',
            'Cash',
            10000.00
        );
    END;


    ------------------------------------------------------------
    -- 6. Demo User Transactions
    ------------------------------------------------------------

    DECLARE @DemoSavingsAccountId INT;
    DECLARE @SalaryCategoryId INT;
    DECLARE @FoodCategoryId INT;
    DECLARE @TransportCategoryId INT;

    SELECT @DemoSavingsAccountId = AccountId
    FROM Accounts
    WHERE UserId = @DemoUserId
      AND AccountName = 'Demo Savings';

    SELECT @SalaryCategoryId = CategoryId
    FROM Categories
    WHERE UserId = @DemoUserId
      AND Name = 'Salary'
      AND Type = 'Income';

    SELECT @FoodCategoryId = CategoryId
    FROM Categories
    WHERE UserId = @DemoUserId
      AND Name = 'Food'
      AND Type = 'Expense';

    SELECT @TransportCategoryId = CategoryId
    FROM Categories
    WHERE UserId = @DemoUserId
      AND Name = 'Transport'
      AND Type = 'Expense';


    ------------------------------------------------------------
    -- Salary
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Transactions
        WHERE AccountId = @DemoSavingsAccountId
          AND TransactionType = 'Income'
          AND Amount = 60000.00
          AND Description = 'Monthly salary'
    )
    BEGIN
        INSERT INTO Transactions
        (
            AccountId,
            CategoryId,
            TransactionType,
            Amount,
            Description
        )
        VALUES
        (
            @DemoSavingsAccountId,
            @SalaryCategoryId,
            'Income',
            60000.00,
            'Monthly salary'
        );
    END;


    ------------------------------------------------------------
    -- Food Expense
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Transactions
        WHERE AccountId = @DemoSavingsAccountId
          AND TransactionType = 'Expense'
          AND Amount = 5000.00
          AND Description = 'Food expenses'
    )
    BEGIN
        INSERT INTO Transactions
        (
            AccountId,
            CategoryId,
            TransactionType,
            Amount,
            Description
        )
        VALUES
        (
            @DemoSavingsAccountId,
            @FoodCategoryId,
            'Expense',
            5000.00,
            'Food expenses'
        );
    END;


    ------------------------------------------------------------
    -- Transport Expense
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM Transactions
        WHERE AccountId = @DemoSavingsAccountId
          AND TransactionType = 'Expense'
          AND Amount = 2000.00
          AND Description = 'Transport expenses'
    )
    BEGIN
        INSERT INTO Transactions
        (
            AccountId,
            CategoryId,
            TransactionType,
            Amount,
            Description
        )
        VALUES
        (
            @DemoSavingsAccountId,
            @TransportCategoryId,
            'Expense',
            2000.00,
            'Transport expenses'
        );
    END;


    ------------------------------------------------------------
    -- Set Demo Savings balance
    ------------------------------------------------------------

    UPDATE Accounts
    SET Balance = 103000.00
    WHERE AccountId = @DemoSavingsAccountId;


    ------------------------------------------------------------
    -- 7. Seed Loan Products
    ------------------------------------------------------------

    IF NOT EXISTS
    (
        SELECT 1
        FROM LoanProducts
        WHERE Name = 'Personal Loan'
    )
    BEGIN
        INSERT INTO LoanProducts
        (
            Name,
            Description,
            InterestRate,
            InterestType,
            MaxAmount,
            MaxTenureMonths,
            IsActive,
            CreatedAt
        )
        VALUES
        (
            'Personal Loan',
            'General purpose personal loan.',
            10.00,
            'Simple',
            500000.00,
            60,
            1,
            GETDATE()
        );
    END;


    IF NOT EXISTS
    (
        SELECT 1
        FROM LoanProducts
        WHERE Name = 'Emergency Loan'
    )
    BEGIN
        INSERT INTO LoanProducts
        (
            Name,
            Description,
            InterestRate,
            InterestType,
            MaxAmount,
            MaxTenureMonths,
            IsActive,
            CreatedAt
        )
        VALUES
        (
            'Emergency Loan',
            'Loan for emergency financial needs.',
            8.00,
            'Simple',
            200000.00,
            24,
            1,
            GETDATE()
        );
    END;


    ------------------------------------------------------------
    -- Get Personal Loan Product ID
    ------------------------------------------------------------

    DECLARE @PersonalLoanProductId INT;

    SELECT @PersonalLoanProductId = LoanProductId
    FROM LoanProducts
    WHERE Name = 'Personal Loan'
      AND IsActive = 1;


    ------------------------------------------------------------
    -- 8. Demo Active Loan
    ------------------------------------------------------------

    DECLARE @DemoLoanId INT;

    -- Make sure we don't create the demo loan twice.
    SELECT @DemoLoanId = LoanId
    FROM Loans
    WHERE UserId = @DemoUserId
      AND PrincipalAmount = 40000.00
      AND InterestRate = 10.00
      AND InterestType = 'Simple'
      AND TenureMonths = 12;


    IF @DemoLoanId IS NULL
    BEGIN

        --------------------------------------------------------
        -- Create the loan
        --------------------------------------------------------

        INSERT INTO Loans
        (
            UserId,
            AccountId,
            LoanProductId,
            PrincipalAmount,
            InterestRate,
            InterestType,
            TenureMonths,
            TotalInterest,
            TotalPayable,
            OutstandingAmount,
            Status,
            StartDate,
            ApprovedAt
        )
        VALUES
        (
            @DemoUserId,
            @DemoSavingsAccountId,
            @PersonalLoanProductId,
            40000.00,
            10.00,
            'Simple',
            12,
            4000.00,
            44000.00,
            44000.00,
            'Active',
            CAST(GETDATE() AS DATE),
            SYSUTCDATETIME()
        );

        SET @DemoLoanId = SCOPE_IDENTITY();


        --------------------------------------------------------
        -- Credit the loan principal to Demo Savings
        --------------------------------------------------------

        UPDATE Accounts
        SET Balance = Balance + 40000.00
        WHERE AccountId = @DemoSavingsAccountId;


        --------------------------------------------------------
        -- Create loan disbursement transaction
        --------------------------------------------------------

        INSERT INTO Transactions
        (
            AccountId,
            LoanId,
            TransactionType,
            Amount,
            Description
        )
        VALUES
        (
            @DemoSavingsAccountId,
            @DemoLoanId,
            'LoanDisbursement',
            40000.00,
            'Loan disbursement'
        );


        --------------------------------------------------------
        -- Create notification
        --------------------------------------------------------

        INSERT INTO Notifications
        (
            UserId,
            Title,
            Message,
            NotificationType,
            IsRead
        )
        VALUES
        (
            @DemoUserId,
            'Loan Approved',
            'Your loan #'
                + CAST(@DemoLoanId AS NVARCHAR(20))
                + ' has been approved and ₹40,000.00 has been credited to your account.',
            'LoanApproval',
            0
        );

    END;

    ------------------------------------------------------------
    -- 9. Commit
    ------------------------------------------------------------

    COMMIT TRANSACTION;

    PRINT 'Seed completed successfully.';

END TRY

BEGIN CATCH

    ROLLBACK TRANSACTION;

    THROW;

END CATCH;
GO