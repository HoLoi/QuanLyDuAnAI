using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDuAn.Migrations
{
    public partial class DanhGiaWorkflowSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF COL_LENGTH('DANH_GIA_DU_AN', 'TrangThaiDanhGia') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_DU_AN] ADD [TrangThaiDanhGia] NVARCHAR(50) NULL;
                IF COL_LENGTH('DANH_GIA_DU_AN', 'MaNguoiDungDuyet') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_DU_AN] ADD [MaNguoiDungDuyet] INT NULL;
                IF COL_LENGTH('DANH_GIA_DU_AN', 'NgayDuyet') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_DU_AN] ADD [NgayDuyet] DATETIME2 NULL;
                IF COL_LENGTH('DANH_GIA_DU_AN', 'LyDoTuChoi') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_DU_AN] ADD [LyDoTuChoi] NVARCHAR(500) NULL;

                IF COL_LENGTH('CT_DANH_GIA_DU_AN', 'MaTieuChi') IS NULL
                    ALTER TABLE [dbo].[CT_DANH_GIA_DU_AN] ADD [MaTieuChi] INT NULL;

                IF COL_LENGTH('DANH_GIA_NHAN_VIEN', 'NhanXetTongQuan') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN] ADD [NhanXetTongQuan] NVARCHAR(500) NULL;
                IF COL_LENGTH('DANH_GIA_NHAN_VIEN', 'TrangThaiDanhGia') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN] ADD [TrangThaiDanhGia] NVARCHAR(50) NULL;
                IF COL_LENGTH('DANH_GIA_NHAN_VIEN', 'MaNguoiDungDuyet') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN] ADD [MaNguoiDungDuyet] INT NULL;
                IF COL_LENGTH('DANH_GIA_NHAN_VIEN', 'NgayDuyet') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN] ADD [NgayDuyet] DATETIME2 NULL;
                IF COL_LENGTH('DANH_GIA_NHAN_VIEN', 'LyDoTuChoi') IS NULL
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN] ADD [LyDoTuChoi] NVARCHAR(500) NULL;

                IF COL_LENGTH('CT_DANH_GIA_NHAN_VIEN', 'MaTieuChi') IS NULL
                    ALTER TABLE [dbo].[CT_DANH_GIA_NHAN_VIEN] ADD [MaTieuChi] INT NULL;

                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'[dbo].[CT_DANH_GIA_NHAN_VIEN]')
                      AND name = N'MaCongViec'
                      AND is_nullable = 0
                )
                BEGIN
                    ALTER TABLE [dbo].[CT_DANH_GIA_NHAN_VIEN] ALTER COLUMN [MaCongViec] INT NULL;
                END

                IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'[dbo].[DANH_GIA_NHAN_VIEN]')
                      AND name = N'MaTieuChi'
                      AND is_nullable = 0
                )
                BEGIN
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN] ALTER COLUMN [MaTieuChi] INT NULL;
                END

                UPDATE dg
                SET dg.TrangThaiDanhGia = COALESCE(NULLIF(LTRIM(RTRIM(dg.TrangThaiDanhGia)), N''), N'DaDuyet')
                FROM [dbo].[DANH_GIA_DU_AN] dg;

                UPDATE dgnv
                SET dgnv.TrangThaiDanhGia = COALESCE(NULLIF(LTRIM(RTRIM(dgnv.TrangThaiDanhGia)), N''), N'DaDuyet')
                FROM [dbo].[DANH_GIA_NHAN_VIEN] dgnv;

                UPDATE ct
                SET ct.MaTieuChi = dg.MaTieuChi
                FROM [dbo].[CT_DANH_GIA_NHAN_VIEN] ct
                INNER JOIN [dbo].[DANH_GIA_NHAN_VIEN] dg ON dg.MaDanhGiaNhanVien = ct.MaDanhGiaNhanVien
                WHERE ct.MaTieuChi IS NULL;
                """);

            migrationBuilder.Sql(
                """
                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DGDA_DUYET_NGUOI_DUNG'
                )
                BEGIN
                    ALTER TABLE [dbo].[DANH_GIA_DU_AN]  WITH CHECK
                    ADD CONSTRAINT [FK_DGDA_DUYET_NGUOI_DUNG]
                    FOREIGN KEY([MaNguoiDungDuyet]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]);
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DGNV_DUYET_NGUOI_DUNG'
                )
                BEGIN
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN]  WITH CHECK
                    ADD CONSTRAINT [FK_DGNV_DUYET_NGUOI_DUNG]
                    FOREIGN KEY([MaNguoiDungDuyet]) REFERENCES [dbo].[NGUOI_DUNG] ([MaNguoiDung]);
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CT_DGDA_TIEU_CHI'
                )
                BEGIN
                    ALTER TABLE [dbo].[CT_DANH_GIA_DU_AN]  WITH CHECK
                    ADD CONSTRAINT [FK_CT_DGDA_TIEU_CHI]
                    FOREIGN KEY([MaTieuChi]) REFERENCES [dbo].[TIEU_CHI_DANH_GIA] ([MaTieuChi]);
                END

                IF NOT EXISTS (
                    SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CT_DGNV_TIEU_CHI'
                )
                BEGIN
                    ALTER TABLE [dbo].[CT_DANH_GIA_NHAN_VIEN]  WITH CHECK
                    ADD CONSTRAINT [FK_CT_DGNV_TIEU_CHI]
                    FOREIGN KEY([MaTieuChi]) REFERENCES [dbo].[TIEU_CHI_DANH_GIA] ([MaTieuChi]);
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CT_DGNV_TIEU_CHI')
                    ALTER TABLE [dbo].[CT_DANH_GIA_NHAN_VIEN] DROP CONSTRAINT [FK_CT_DGNV_TIEU_CHI];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_CT_DGDA_TIEU_CHI')
                    ALTER TABLE [dbo].[CT_DANH_GIA_DU_AN] DROP CONSTRAINT [FK_CT_DGDA_TIEU_CHI];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DGNV_DUYET_NGUOI_DUNG')
                    ALTER TABLE [dbo].[DANH_GIA_NHAN_VIEN] DROP CONSTRAINT [FK_DGNV_DUYET_NGUOI_DUNG];
                IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DGDA_DUYET_NGUOI_DUNG')
                    ALTER TABLE [dbo].[DANH_GIA_DU_AN] DROP CONSTRAINT [FK_DGDA_DUYET_NGUOI_DUNG];
                """);
        }
    }
}
