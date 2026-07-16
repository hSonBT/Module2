--Tạo CSDL (DATABASE)
CREATE DATABASE Shop;
GO

USE shop;

GO
--Tạo bảng 
--Danh mục ()
CREATE TABLE Category(
    CategoryId INT NOT NULL PRIMARY KEY IDENTITY(1, 1), --Tự động tăng
    CategoryName NVARCHAR(128) NOT NULL
);
GO
--TRUNCATE TABLE Category;
--Them du lieu vao bang
INSERT INTO Category (CategoryName) VALUES
    (N'Laptop (máy tính xách tay)'),
    (N'Mouse (Chuột máy tính)'),
    (N'Keyboard (bàn phím máy tính)');
GO
--SELECT * FROM Category;
CREATE TABLE Product(
    ProductId INT NOT NULL PRIMARY KEY IDENTITY(1, 1),
    CategoryId INT NOT NULL REFERENCES Category(CategoryId),
    ProdutName NVARCHAR(128) NOT NULL,
    Description NVARCHAR(1024) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    --Content NTEXT NOT NULL
    ImageUrl CHAR(32) NOT NULL,
    Price DECIMAL(10, 2) NOT NULL,
    Quantity SMALLINT NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    Updated DATETIME NOT NULL DEFAULT GETDATE()
);
GO
INSERT INTO Product (CategoryId, ProdutName, [Description], Content, ImageUrl, Price, Quantity) VALUES
    (1, N'Máy tính tính DELL', N'Mô tả sản phẩm cho máy tính DELL', N'Nội dụng DELL', 'dell.png', 1115.72, 5),
    (1, N'Máy tính tính Asus', N'Mô tả sản phẩm cho máy tính Asus', N'Nội dụng Asus', 'asus.png', 1115.72, 5);
GO

--privacy
--SBD (số báo danh)
---SBD: 9317 => 9318, 9319

--2 giá trị (bit => 0 => false, 1 => true)
--3 giá trị (tinyint)
--0 -> Female, 1-> Male

--DROP TABLE Member;
GO

CREATE TABLE Member(
    MemberId CHAR(32) NOT NULL PRIMARY KEY,
    GivenName NVARCHAR(32) NOT NULL,
    Surname NVARCHAR(32),
    Gender BIT NOT NULL,
    Email VARCHAR(64) NOT NULL UNIQUE,
    Phone VARCHAR(10) NOT NULL UNIQUE,
    Password BINARY(64) NOT NULL,
    RegisterDate DATETIME NOT NULL DEFAULT GETDATE(),
    LoginDate DATETIME NOT NULL DEFAULT GETDATE(),
);
GO
--SELECT REPLACE(NEWID(), '-', '');
--SELECT HASHBYTES('SHA2_512', '123');

INSERT INTO Member (MemberId, GivenName, Surname, Gender, Email, Phone, [Password]) VALUES
    (REPLACE(NEWID(), '-', ''), N'Tèo', 'Tony', 1, 'tonyteo@ceb.net.vn', '0133444555', HASHBYTES('SHA2_512', '123')),
    (REPLACE(NEWID(), '-', ''), N'Tí', NULL, 1, 'ti@ceb.net.vn', '0973444555', HASHBYTES('SHA2_512', '456')),
    (REPLACE(NEWID(), '-', ''), N'Sửu', N'Nguyễn Thị', 0, 'ntsuu@ceb.net.vn', '0973444123', HASHBYTES('SHA2_512', '321'));
GO
--SELECT * FROM Member;

--Tạo thủ tục thêm dữ liệu vào bảng Member
CREATE PROC AddMember(
    @MemberId CHAR(32),
    @GivenName NVARCHAR(32),
    @Surname NVARCHAR(32) = NULL,
    @Gender BIT,
    @Email VARCHAR(64),
    @Phone VARCHAR(10),
    @Password BINARY(64)
)
AS
    IF NOT EXISTS (SELECT * FROM Member WHERE MemberId = @MemberId OR Email = @Email OR Phone = @Phone)
        INSERT INTO Member (MemberId, GivenName, Surname, Gender, Email, Phone, [Password]) VALUES
            (@MemberId, @GivenName, @Surname, @Gender, @Email, @Phone, @Password);
GO


DECLARE @Id CHAR(32) = REPLACE(NEWID(), '-', '');
DECLARE @Pwd BINARY(64) = HASHBYTES('SHA2_512', 'abc');
EXEC AddMember @MemberId = @Id, @GivenName = N'Sáng', @Surname = N'Trần Văn', @Gender = 1, 
    @Email = 'tvsang@ceb.net.vn', @Phone = '1371284624', @Password = @Pwd;
GO

--SELECT * FROM Member;

CREATE TABLE Role(
    RoleId BIGINT NOT NULL PRIMARY KEY,
    RoleName NVARCHAR(32) NOT NULL
);
GO
INSERT INTO Role (RoleId, RoleName) VALUES
    (123947239482749823, 'Member'),
    (23234202902922927, 'Supplier'),
    (23489798189124871, 'Admin');
GO

CREATE TABLE MemberInRole(
    MemberId CHAR(32) NOT NULL REFERENCES Member(MemberId),
    RoleId BIGINT NOT NULL REFERENCES Role(RoleId),
    IsDeleted BIT NOT NULL DEFAULT 0,
    PRIMARY KEY (MemberId, RoleId)
);
GO




CREATE TABLE Student(
    StudentId VARCHAR(10) NOT NULL PRIMARY KEY, 
    IdentityCard VARCHAR(16) NOT NULL UNIQUE,
    --IdentityCard VARCHAR(16) NOT NULL PRIMARY KEY,
)

