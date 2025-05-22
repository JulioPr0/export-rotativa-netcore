IF EXISTS (SELECT 1 FROM sys.databases WHERE name = N'RotativaDemoDb')
    DROP DATABASE RotativaDemoDb;
GO

CREATE DATABASE RotativaDemoDb;
GO
USE RotativaDemoDb;
GO

IF OBJECT_ID('dbo.StudentScores','U') IS NULL
BEGIN
    CREATE TABLE dbo.StudentScores
    (
        Id    INT IDENTITY PRIMARY KEY,
        Nama  NVARCHAR(100) NOT NULL,
        Nilai INT NOT NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.StudentScores)
BEGIN
    INSERT INTO dbo.StudentScores (Nama, Nilai) VALUES
    (N'Julio', 95),
    (N'Hoiluj', 90),
    (N'永城', 90);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_GetStudentScores
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH ScoreCTE AS
    (
        SELECT
            Id,
            Nama,
            Nilai,
            CAST(ROW_NUMBER() OVER(ORDER BY Nilai DESC) AS INT) AS Rank
        FROM dbo.StudentScores
    )
    SELECT Id, Rank AS No, Nama, Nilai

    FROM ScoreCTE
    ORDER BY Rank;
END
GO
