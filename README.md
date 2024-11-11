# RamandTechProject
در اولین قدم کوئری ها و استور پروسیجر های زیر را لطفا اجرا نمایید

Create TABLE Users (
    Id INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100)
);

CREATE PROCEDURE spAuthenticate
    @Email NVARCHAR(255),
    @Password NVARCHAR(255)
AS
BEGIN
    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email and PasswordHash = @Password)
    BEGIN
        SELECT Id, Username, FullName, Email
        FROM Users
        WHERE Email = @Email;
    END
    ELSE
    BEGIN
        RAISERROR('Username or Password is wrong.', 15, 1);
        RETURN;
		END
END;


CREATE PROCEDURE spGetAllUsers
AS
BEGIN
    SELECT Id, Username, FullName, Email
    FROM Users;
END;


CREATE PROCEDURE spGetUserByEmail
    @Email NVARCHAR(100)
AS
BEGIN
    IF @Email IS NULL OR @Email = ''
    BEGIN
        RAISERROR('Email cannot be empty.', 15, 1);
        RETURN;
    END
    SELECT Id, Username, PasswordHash, Email,FullName
    FROM Users
    WHERE Email = @Email;
END
GO


CREATE PROCEDURE spGetUserById
    @Id int
AS
BEGIN
    IF @Id IS NULL
    BEGIN
        RAISERROR('Id cannot be empty.', 15, 1);
        RETURN;
    END
    SELECT Id, Username, Email,FullName
    FROM Users
    WHERE Id = @Id;
END
GO

در قدم دوم در فایل appsetting مقادیر RabbitMq و ConnectionStirng را تنظیم نموده
همینطور در پروژه RabbitConsume در فایل RabbitMqService مقادیر را برای کانکشن ربیت خود کانفیگ کنید

حال میتوانید پروژه ApiService را اجرا نمایید 

برای کانسیوم کردن پیام های درون صف پروژه RabbitConsumer رو یک اینستنس جاگانه بسازید 
