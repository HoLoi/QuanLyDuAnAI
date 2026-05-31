USE [QuanLyDuAnAI];
GO

IF COL_LENGTH(N'dbo.DU_AN', N'NgayHoanThanhThucTeDuAn') IS NULL
BEGIN
    ALTER TABLE dbo.DU_AN
    ADD NgayHoanThanhThucTeDuAn DATETIME2(7) NULL;
END;