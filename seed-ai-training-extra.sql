
USE [QuanLyDuAnAI]
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Now DATETIME2(7) = SYSDATETIME();
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @Prefix NVARCHAR(100) = N'SEED_AI_TRAIN_EXTRA';
    DECLARE @ReasonNotePrefix NVARCHAR(140) = N'SEED_AI_NGUYEN_NHAN_TRAIN_EXTRA';

    IF OBJECT_ID(N'dbo.DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NHAN_VIEN_DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.DANH_MUC_CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.CT_CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.TIEN_DO_CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NGAN_SACH', N'U') IS NULL
       OR OBJECT_ID(N'dbo.CHI_PHI', N'U') IS NULL
       OR OBJECT_ID(N'dbo.DE_XUAT_CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.DE_XUAT_NGAN_SACH', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NHAT_KY_PHU_TRACH_DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.YEU_CAU_DOI_QUAN_LY', N'U') IS NULL
       OR OBJECT_ID(N'dbo.AI_NGUYEN_NHAN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.DM_NGUYEN_NHAN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NGUOI_DUNG', N'U') IS NULL
       OR OBJECT_ID(N'dbo.LOAI_DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.MUC_DO_UU_TIEN', N'U') IS NULL
    BEGIN
        THROW 53100, N'Thiếu bảng bắt buộc để chạy seed-ai-training-extra.sql.', 1;
    END;

    DECLARE @ReasonConfig TABLE
    (
        ReasonOrder INT NOT NULL PRIMARY KEY,
        TenNguyenNhan NVARCHAR(255) NOT NULL,
        SoDuAnTre INT NOT NULL,
        HeSoTreNgay INT NOT NULL,
        HeSoTreCongViec INT NOT NULL,
        HeSoLechChiPhi INT NOT NULL,
        HeSoDoiNhanSu INT NOT NULL,
        HeSoDoiQuanLy INT NOT NULL,
        HeSoChamCapNhat INT NOT NULL
    );

    INSERT INTO @ReasonConfig
    (
        ReasonOrder,
        TenNguyenNhan,
        SoDuAnTre,
        HeSoTreNgay,
        HeSoTreCongViec,
        HeSoLechChiPhi,
        HeSoDoiNhanSu,
        HeSoDoiQuanLy,
        HeSoChamCapNhat
    )
    VALUES
        (1, N'Thiếu nhân sự', 20, 12, 2, 5, 5, 1, 2),
        (2, N'Thay đổi yêu cầu liên tục', 20, 11, 2, 10, 4, 2, 3),
        (3, N'Quy trình xử lý chậm', 20, 14, 2, 6, 2, 4, 5),
        (4, N'Vượt ngân sách', 20, 9, 1, 35, 2, 1, 3),
        (5, N'Rủi ro kỹ thuật', 20, 18, 3, 14, 2, 1, 4),
        (6, N'Phối hợp công việc chưa tốt', 20, 13, 3, 9, 3, 2, 6),
        (7, N'Thông tin đầu vào chưa đầy đủ', 20, 12, 2, 4, 2, 1, 4),
        (8, N'Ước lượng thời gian chưa chính xác', 20, 17, 3, 8, 2, 1, 5),
        (9, N'Tiến độ cập nhật không đầy đủ', 20, 10, 1, 6, 2, 1, 14),
        (10, N'Khác', 10, 11, 2, 7, 2, 1, 4);

    INSERT INTO dbo.DM_NGUYEN_NHAN (TenNguyenNhan)
    SELECT rc.TenNguyenNhan
    FROM @ReasonConfig rc
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.DM_NGUYEN_NHAN dm
        WHERE dm.TenNguyenNhan COLLATE Latin1_General_CI_AI = rc.TenNguyenNhan COLLATE Latin1_General_CI_AI
    );

    DECLARE @ReasonMap TABLE
    (
        ReasonOrder INT NOT NULL PRIMARY KEY,
        MaDMNguyenNhan INT NOT NULL,
        TenNguyenNhan NVARCHAR(255) NOT NULL,
        SoDuAnTre INT NOT NULL,
        HeSoTreNgay INT NOT NULL,
        HeSoTreCongViec INT NOT NULL,
        HeSoLechChiPhi INT NOT NULL,
        HeSoDoiNhanSu INT NOT NULL,
        HeSoDoiQuanLy INT NOT NULL,
        HeSoChamCapNhat INT NOT NULL
    );

    INSERT INTO @ReasonMap
    (
        ReasonOrder, MaDMNguyenNhan, TenNguyenNhan, SoDuAnTre,
        HeSoTreNgay, HeSoTreCongViec, HeSoLechChiPhi,
        HeSoDoiNhanSu, HeSoDoiQuanLy, HeSoChamCapNhat
    )
    SELECT
        rc.ReasonOrder,
        dm.MaDMNguyenNhan,
        dm.TenNguyenNhan,
        rc.SoDuAnTre,
        rc.HeSoTreNgay,
        rc.HeSoTreCongViec,
        rc.HeSoLechChiPhi,
        rc.HeSoDoiNhanSu,
        rc.HeSoDoiQuanLy,
        rc.HeSoChamCapNhat
    FROM @ReasonConfig rc
    INNER JOIN dbo.DM_NGUYEN_NHAN dm
        ON dm.TenNguyenNhan COLLATE Latin1_General_CI_AI = rc.TenNguyenNhan COLLATE Latin1_General_CI_AI;

    IF (SELECT COUNT(*) FROM @ReasonMap) <> 10
    BEGIN
        THROW 53101, N'Không map đủ 10 nguyên nhân trong DM_NGUYEN_NHAN.', 1;
    END;

    DECLARE @ManagerPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaNguoiDung INT NOT NULL UNIQUE
    );

    IF OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NOT NULL
       AND OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NOT NULL
    BEGIN
        INSERT INTO @ManagerPool (MaNguoiDung)
        SELECT DISTINCT au.MaNguoiDung
        FROM dbo.AspNetUsers au
        INNER JOIN dbo.AspNetUserRoles aur ON aur.Asp_Id = au.Id
        INNER JOIN dbo.AspNetRoles ar ON ar.Id = aur.Id
        WHERE ar.Name COLLATE Latin1_General_CI_AI = N'Manager'
          AND au.MaNguoiDung IS NOT NULL;
    END;

    IF NOT EXISTS (SELECT 1 FROM @ManagerPool)
    BEGIN
        INSERT INTO @ManagerPool (MaNguoiDung)
        SELECT nd.MaNguoiDung
        FROM dbo.NGUOI_DUNG nd
        WHERE ISNULL(nd.IsDeleted, 0) = 0
          AND NOT EXISTS (SELECT 1 FROM @ManagerPool mp WHERE mp.MaNguoiDung = nd.MaNguoiDung)
        ORDER BY nd.MaNguoiDung;
    END;

    DECLARE @ManagerCount INT = (SELECT COUNT(*) FROM @ManagerPool);
    IF @ManagerCount = 0
    BEGIN
        THROW 53102, N'Không có người dùng hợp lệ để làm quản lý seed.', 1;
    END;

    DECLARE @UserPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaNguoiDung INT NOT NULL UNIQUE
    );

    INSERT INTO @UserPool (MaNguoiDung)
    SELECT nd.MaNguoiDung
    FROM dbo.NGUOI_DUNG nd
    WHERE ISNULL(nd.IsDeleted, 0) = 0
    ORDER BY nd.MaNguoiDung;

    DECLARE @UserCount INT = (SELECT COUNT(*) FROM @UserPool);
    IF @UserCount < 3
    BEGIN
        THROW 53103, N'Cần tối thiểu 3 người dùng để seed dữ liệu train.', 1;
    END;

    DECLARE @LoaiPool TABLE (RowNo INT IDENTITY(1,1) PRIMARY KEY, MaLoaiDuAn INT NOT NULL UNIQUE);
    INSERT INTO @LoaiPool (MaLoaiDuAn)
    SELECT MaLoaiDuAn FROM dbo.LOAI_DU_AN ORDER BY MaLoaiDuAn;

    DECLARE @LoaiCount INT = (SELECT COUNT(*) FROM @LoaiPool);
    IF @LoaiCount = 0
    BEGIN
        THROW 53104, N'Không có LOAI_DU_AN để seed.', 1;
    END;

    DECLARE @MucDoPool TABLE (RowNo INT IDENTITY(1,1) PRIMARY KEY, MaMucDo INT NOT NULL UNIQUE);
    INSERT INTO @MucDoPool (MaMucDo)
    SELECT MaMucDo FROM dbo.MUC_DO_UU_TIEN ORDER BY MaMucDo;

    DECLARE @MucDoCount INT = (SELECT COUNT(*) FROM @MucDoPool);
    IF @MucDoCount = 0
    BEGIN
        THROW 53105, N'Không có MUC_DO_UU_TIEN để seed.', 1;
    END;

    DECLARE @ProjectPlan TABLE
    (
        Seq INT IDENTITY(1,1) PRIMARY KEY,
        ReasonOrder INT NOT NULL,
        MaDMNguyenNhan INT NOT NULL,
        TenNguyenNhan NVARCHAR(255) NOT NULL,
        ThuTuTrongNhom INT NOT NULL,
        GhiChuDuAn NVARCHAR(255) NOT NULL UNIQUE,
        TenDuAn NVARCHAR(255) NOT NULL,
        TrangThaiDuAn NVARCHAR(50) NOT NULL,
        HeSoTreNgay INT NOT NULL,
        HeSoTreCongViec INT NOT NULL,
        HeSoLechChiPhi INT NOT NULL,
        HeSoDoiNhanSu INT NOT NULL,
        HeSoDoiQuanLy INT NOT NULL,
        HeSoChamCapNhat INT NOT NULL
    );

    ;WITH tally AS
    (
        SELECT TOP (30) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS n
        FROM sys.all_objects
    )
    INSERT INTO @ProjectPlan
    (
        ReasonOrder,
        MaDMNguyenNhan,
        TenNguyenNhan,
        ThuTuTrongNhom,
        GhiChuDuAn,
        TenDuAn,
        TrangThaiDuAn,
        HeSoTreNgay,
        HeSoTreCongViec,
        HeSoLechChiPhi,
        HeSoDoiNhanSu,
        HeSoDoiQuanLy,
        HeSoChamCapNhat
    )
    SELECT
        rm.ReasonOrder,
        rm.MaDMNguyenNhan,
        rm.TenNguyenNhan,
        t.n,
        CONCAT(@Prefix, N'_R', RIGHT(N'00' + CAST(rm.ReasonOrder AS NVARCHAR(10)), 2), N'_P', RIGHT(N'000' + CAST(t.n AS NVARCHAR(10)), 3)),
        CONCAT(N'Dự án train mở rộng - ', rm.TenNguyenNhan, N' #', RIGHT(N'000' + CAST(t.n AS NVARCHAR(10)), 3)),
        CASE WHEN (t.n % 4 = 0) THEN N'Archived' ELSE N'HoanThanh' END,
        rm.HeSoTreNgay,
        rm.HeSoTreCongViec,
        rm.HeSoLechChiPhi,
        rm.HeSoDoiNhanSu,
        rm.HeSoDoiQuanLy,
        rm.HeSoChamCapNhat
    FROM @ReasonMap rm
    INNER JOIN tally t ON t.n <= rm.SoDuAnTre
    ORDER BY rm.ReasonOrder, t.n;

    DECLARE @TotalProjectPlan INT = (SELECT COUNT(*) FROM @ProjectPlan);
    IF @TotalProjectPlan = 0
    BEGIN
        THROW 53106, N'Không có kế hoạch dự án để seed.', 1;
    END;

    DECLARE
        @Seq INT = 1,
        @ReasonOrder INT,
        @MaDMNguyenNhan INT,
        @TenNguyenNhan NVARCHAR(255),
        @ThuTuTrongNhom INT,
        @GhiChuDuAn NVARCHAR(255),
        @TenDuAn NVARCHAR(255),
        @TrangThaiDuAn NVARCHAR(50),
        @HeSoTreNgay INT,
        @HeSoTreCongViec INT,
        @HeSoLechChiPhi INT,
        @HeSoDoiNhanSu INT,
        @HeSoDoiQuanLy INT,
        @HeSoChamCapNhat INT,
        @MaManager INT,
        @MaThanhVien1 INT,
        @MaThanhVien2 INT,
        @MaLoaiDuAn INT,
        @MaMucDo1 INT,
        @MaMucDo2 INT,
        @MaMucDo3 INT,
        @MaDuAn INT,
        @MaDanhMuc1 INT,
        @MaDanhMuc2 INT,
        @MaCongViec1 INT,
        @MaCongViec2 INT,
        @MaCongViec3 INT,
        @MaChiTiet1 INT,
        @MaChiTiet2 INT,
        @MaChiTiet3 INT,
        @MaNganSach INT,
        @NgayBatDauDuAn DATETIME2(7),
        @NgayKetThucDuAn DATETIME2(7),
        @NgayHoanThanhThucTeDuAn DATETIME2(7),
        @NgayBatDauCv1 DATETIME2(7),
        @NgayBatDauCv2 DATETIME2(7),
        @NgayBatDauCv3 DATETIME2(7),
        @NgayKetThucCv1DuKien DATETIME2(7),
        @NgayKetThucCv2DuKien DATETIME2(7),
        @NgayKetThucCv3DuKien DATETIME2(7),
        @NgayKetThucCv1ThucTe DATETIME2(7),
        @NgayKetThucCv2ThucTe DATETIME2(7),
        @NgayKetThucCv3ThucTe DATETIME2(7),
        @NganSachDuKien DECIMAL(18,2),
        @TongChiPhi DECIMAL(18,2),
        @ChiPhi1 DECIMAL(18,2),
        @ChiPhi2 DECIMAL(18,2),
        @ChiPhi3 DECIMAL(18,2),
        @SoLanNhatKyNhanSu INT,
        @SoLanDoiQuanLy INT,
        @Loop INT,
        @MaQuanLyDeXuat INT,
        @NgayYeuCau DATETIME2(7),
        @NgayBaoCaoGanNhat DATETIME2(7);

    WHILE @Seq <= @TotalProjectPlan
    BEGIN
        SELECT
            @ReasonOrder = ReasonOrder,
            @MaDMNguyenNhan = MaDMNguyenNhan,
            @TenNguyenNhan = TenNguyenNhan,
            @ThuTuTrongNhom = ThuTuTrongNhom,
            @GhiChuDuAn = GhiChuDuAn,
            @TenDuAn = TenDuAn,
            @TrangThaiDuAn = TrangThaiDuAn,
            @HeSoTreNgay = HeSoTreNgay,
            @HeSoTreCongViec = HeSoTreCongViec,
            @HeSoLechChiPhi = HeSoLechChiPhi,
            @HeSoDoiNhanSu = HeSoDoiNhanSu,
            @HeSoDoiQuanLy = HeSoDoiQuanLy,
            @HeSoChamCapNhat = HeSoChamCapNhat
        FROM @ProjectPlan
        WHERE Seq = @Seq;

        SELECT @MaManager = MaNguoiDung FROM @ManagerPool WHERE RowNo = ((@Seq - 1) % @ManagerCount) + 1;
        SELECT @MaLoaiDuAn = MaLoaiDuAn FROM @LoaiPool WHERE RowNo = ((@Seq - 1) % @LoaiCount) + 1;
        SELECT @MaMucDo1 = MaMucDo FROM @MucDoPool WHERE RowNo = ((@Seq - 1) % @MucDoCount) + 1;
        SELECT @MaMucDo2 = MaMucDo FROM @MucDoPool WHERE RowNo = ((@Seq + 1) % @MucDoCount) + 1;
        SELECT @MaMucDo3 = MaMucDo FROM @MucDoPool WHERE RowNo = ((@Seq + 2) % @MucDoCount) + 1;

        SELECT @MaThanhVien1 = MaNguoiDung FROM @UserPool WHERE RowNo = ((@Seq + 3) % @UserCount) + 1;
        SELECT @MaThanhVien2 = MaNguoiDung FROM @UserPool WHERE RowNo = ((@Seq + 7) % @UserCount) + 1;

        IF @MaThanhVien1 = @MaManager
            SELECT @MaThanhVien1 = MaNguoiDung FROM @UserPool WHERE RowNo = ((@Seq + 11) % @UserCount) + 1;

        IF @MaThanhVien2 IN (@MaManager, @MaThanhVien1)
            SELECT @MaThanhVien2 = MaNguoiDung FROM @UserPool WHERE RowNo = ((@Seq + 15) % @UserCount) + 1;

        IF @MaThanhVien2 IN (@MaManager, @MaThanhVien1)
            SET @MaThanhVien2 = @MaThanhVien1;

        SET @NgayBatDauDuAn = DATEADD(DAY, -(230 + @Seq), @Now);
        SET @NgayKetThucDuAn = DATEADD(DAY, -(35 + (@Seq % 55)), @Now);

        SET @NgayBatDauCv1 = DATEADD(DAY, 5, @NgayBatDauDuAn);
        SET @NgayBatDauCv2 = DATEADD(DAY, 18, @NgayBatDauDuAn);
        SET @NgayBatDauCv3 = DATEADD(DAY, 31, @NgayBatDauDuAn);

        SET @NgayKetThucCv1DuKien = DATEADD(DAY, 22, @NgayBatDauCv1);
        SET @NgayKetThucCv2DuKien = DATEADD(DAY, 20, @NgayBatDauCv2);
        SET @NgayKetThucCv3DuKien = DATEADD(DAY, 18, @NgayBatDauCv3);

        SET @NgayKetThucCv1ThucTe = DATEADD(DAY, @HeSoTreNgay + (@ThuTuTrongNhom % 5), @NgayKetThucCv1DuKien);
        SET @NgayKetThucCv2ThucTe = DATEADD(DAY, CASE WHEN @HeSoTreCongViec >= 2 THEN 4 + (@ThuTuTrongNhom % 3) ELSE 0 END, @NgayKetThucCv2DuKien);
        SET @NgayKetThucCv3ThucTe = DATEADD(DAY, CASE WHEN @HeSoTreCongViec >= 3 THEN 8 + (@ThuTuTrongNhom % 4) ELSE 2 END, @NgayKetThucCv3DuKien);

        IF @NgayKetThucCv1ThucTe > @NgayKetThucDuAn
            SET @NgayKetThucCv1ThucTe = DATEADD(DAY, -2, @NgayKetThucDuAn);

        IF @NgayKetThucCv2ThucTe > @NgayKetThucDuAn
            SET @NgayKetThucCv2ThucTe = DATEADD(DAY, -1, @NgayKetThucDuAn);

        IF @NgayKetThucCv3ThucTe > @NgayKetThucDuAn
            SET @NgayKetThucCv3ThucTe = @NgayKetThucDuAn;

        SET @NgayHoanThanhThucTeDuAn = (
            SELECT MAX(v.NgayHoanThanh)
            FROM (VALUES (@NgayKetThucCv1ThucTe), (@NgayKetThucCv2ThucTe), (@NgayKetThucCv3ThucTe)) v(NgayHoanThanh)
        );

        SET @NganSachDuKien = CAST(90000000 + ((@Seq % 7) * 8500000) AS DECIMAL(18,2));
        SET @TongChiPhi = CAST(@NganSachDuKien * (100 + @HeSoLechChiPhi + (@ThuTuTrongNhom % 4)) / 100.0 AS DECIMAL(18,2));

        IF @ReasonOrder = 4
            SET @TongChiPhi = CAST(@NganSachDuKien * (120 + (@ThuTuTrongNhom % 10)) / 100.0 AS DECIMAL(18,2));

        SET @ChiPhi1 = CAST(@TongChiPhi * 0.35 AS DECIMAL(18,2));
        SET @ChiPhi2 = CAST(@TongChiPhi * 0.40 AS DECIMAL(18,2));
        SET @ChiPhi3 = CAST(@TongChiPhi - @ChiPhi1 - @ChiPhi2 AS DECIMAL(18,2));

        SET @SoLanNhatKyNhanSu = @HeSoDoiNhanSu + (@ThuTuTrongNhom % 3);
        SET @SoLanDoiQuanLy = @HeSoDoiQuanLy + CASE WHEN @ManagerCount > 1 THEN (@ThuTuTrongNhom % 2) ELSE 0 END;

        IF NOT EXISTS (SELECT 1 FROM dbo.DU_AN WHERE GhiChuDuAn = @GhiChuDuAn)
        BEGIN
            INSERT INTO dbo.DU_AN
            (
                MaNguoiDung, MaLoaiDuAn, TenDuAn, MoTaDuAn,
                NgayTaoDuAn, NgayBatDauDuAn, NgayKetThucDuAn, NgayHoanThanhThucTeDuAn,
                PhanTramHoanThanh, TrangThaiDuAn, GhiChuDuAn,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaManager, @MaLoaiDuAn, @TenDuAn,
                N'Dữ liệu seed mở rộng phục vụ train AI phân tích nguyên nhân trễ (chỉ dữ liệu nghiệp vụ).',
                DATEADD(DAY, -(@Seq % 10), @Now),
                @NgayBatDauDuAn,
                @NgayKetThucDuAn,
                @NgayHoanThanhThucTeDuAn,
                100,
                @TrangThaiDuAn,
                @GhiChuDuAn,
                0,
                NULL,
                NULL
            );

            SET @MaDuAn = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaDuAn = da.MaDuAn
            FROM dbo.DU_AN da
            WHERE da.GhiChuDuAn = @GhiChuDuAn
            ORDER BY da.MaDuAn DESC;

            UPDATE dbo.DU_AN
            SET MaNguoiDung = @MaManager,
                MaLoaiDuAn = @MaLoaiDuAn,
                TenDuAn = @TenDuAn,
                MoTaDuAn = N'Dữ liệu seed mở rộng phục vụ train AI phân tích nguyên nhân trễ (chỉ dữ liệu nghiệp vụ).',
                NgayBatDauDuAn = @NgayBatDauDuAn,
                NgayKetThucDuAn = @NgayKetThucDuAn,
                NgayHoanThanhThucTeDuAn = @NgayHoanThanhThucTeDuAn,
                PhanTramHoanThanh = 100,
                TrangThaiDuAn = @TrangThaiDuAn,
                IsDeleted = 0,
                DeletedAt = NULL,
                DeletedBy = NULL
            WHERE MaDuAn = @MaDuAn;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @MaManager)
            INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
            VALUES (@MaDuAn, @MaManager, DATEADD(DAY, 1, @NgayBatDauDuAn), N'Leader');

        IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @MaThanhVien1)
            INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
            VALUES (@MaDuAn, @MaThanhVien1, DATEADD(DAY, 2, @NgayBatDauDuAn), N'Member');

        IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @MaThanhVien2)
            INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
            VALUES (@MaDuAn, @MaThanhVien2, DATEADD(DAY, 3, @NgayBatDauDuAn), N'Member');

        DECLARE @TenDanhMuc1 NVARCHAR(255) = CONCAT(N'Hạng mục chính ', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));
        DECLARE @TenDanhMuc2 NVARCHAR(255) = CONCAT(N'Hạng mục phối hợp ', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));

        IF NOT EXISTS (SELECT 1 FROM dbo.DANH_MUC_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc1)
        BEGIN
            INSERT INTO dbo.DANH_MUC_CONG_VIEC
            (
                MaDuAn, TenDanhMucCV, MoTaDanhMucCV, NgayTaoDMCV,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @TenDanhMuc1, N'Danh mục seed train mở rộng.', DATEADD(DAY, 4, @NgayBatDauDuAn),
                0, NULL, NULL
            );
            SET @MaDanhMuc1 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaDanhMuc1 = MaDanhMucCV
            FROM dbo.DANH_MUC_CONG_VIEC
            WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc1
            ORDER BY MaDanhMucCV DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.DANH_MUC_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc2)
        BEGIN
            INSERT INTO dbo.DANH_MUC_CONG_VIEC
            (
                MaDuAn, TenDanhMucCV, MoTaDanhMucCV, NgayTaoDMCV,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @TenDanhMuc2, N'Danh mục phụ trợ seed train mở rộng.', DATEADD(DAY, 5, @NgayBatDauDuAn),
                0, NULL, NULL
            );
            SET @MaDanhMuc2 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaDanhMuc2 = MaDanhMucCV
            FROM dbo.DANH_MUC_CONG_VIEC
            WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc2
            ORDER BY MaDanhMucCV DESC;
        END;

        DECLARE @TenCongViec1 NVARCHAR(255) = CONCAT(N'Công việc chính trễ #', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));
        DECLARE @TenCongViec2 NVARCHAR(255) = CONCAT(N'Công việc điều phối #', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));
        DECLARE @TenCongViec3 NVARCHAR(255) = CONCAT(N'Công việc nghiệm thu #', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));

        IF NOT EXISTS (SELECT 1 FROM dbo.CONG_VIEC WHERE MaDanhMucCV = @MaDanhMuc1 AND TenCongViec = @TenCongViec1)
        BEGIN
            INSERT INTO dbo.CONG_VIEC
            (
                MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, MoTaCongViec,
                NgayBatDauCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe,
                NgayTaoCongViec, TrangThaiCongViec,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                NULL, @MaDanhMuc1, @MaMucDo1, @TenCongViec1, N'Công việc chính có độ trễ cao để tạo tín hiệu train.',
                @NgayBatDauCv1, @NgayKetThucCv1DuKien, @NgayKetThucCv1ThucTe,
                DATEADD(DAY, 6, @NgayBatDauDuAn), N'HoanThanh',
                0, NULL, NULL
            );
            SET @MaCongViec1 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaCongViec1 = MaCongViec
            FROM dbo.CONG_VIEC
            WHERE MaDanhMucCV = @MaDanhMuc1 AND TenCongViec = @TenCongViec1
            ORDER BY MaCongViec DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CONG_VIEC WHERE MaDanhMucCV = @MaDanhMuc2 AND TenCongViec = @TenCongViec2)
        BEGIN
            INSERT INTO dbo.CONG_VIEC
            (
                MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, MoTaCongViec,
                NgayBatDauCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe,
                NgayTaoCongViec, TrangThaiCongViec,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                NULL, @MaDanhMuc2, @MaMucDo2, @TenCongViec2, N'Công việc phối hợp nội bộ.',
                @NgayBatDauCv2, @NgayKetThucCv2DuKien, @NgayKetThucCv2ThucTe,
                DATEADD(DAY, 7, @NgayBatDauDuAn), N'HoanThanh',
                0, NULL, NULL
            );
            SET @MaCongViec2 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaCongViec2 = MaCongViec
            FROM dbo.CONG_VIEC
            WHERE MaDanhMucCV = @MaDanhMuc2 AND TenCongViec = @TenCongViec2
            ORDER BY MaCongViec DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CONG_VIEC WHERE MaDanhMucCV = @MaDanhMuc2 AND TenCongViec = @TenCongViec3)
        BEGIN
            INSERT INTO dbo.CONG_VIEC
            (
                MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, MoTaCongViec,
                NgayBatDauCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe,
                NgayTaoCongViec, TrangThaiCongViec,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                NULL, @MaDanhMuc2, @MaMucDo3, @TenCongViec3, N'Công việc nghiệm thu và đóng dự án.',
                @NgayBatDauCv3, @NgayKetThucCv3DuKien, @NgayKetThucCv3ThucTe,
                DATEADD(DAY, 8, @NgayBatDauDuAn), N'HoanThanh',
                0, NULL, NULL
            );
            SET @MaCongViec3 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaCongViec3 = MaCongViec
            FROM dbo.CONG_VIEC
            WHERE MaDanhMucCV = @MaDanhMuc2 AND TenCongViec = @TenCongViec3
            ORDER BY MaCongViec DESC;
        END;

        DECLARE @TenChiTiet1 NVARCHAR(255) = CONCAT(N'Chi tiết trễ chính #', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));
        DECLARE @TenChiTiet2 NVARCHAR(255) = CONCAT(N'Chi tiết phối hợp #', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));
        DECLARE @TenChiTiet3 NVARCHAR(255) = CONCAT(N'Chi tiết nghiệm thu #', RIGHT(N'000' + CAST(@Seq AS NVARCHAR(10)), 3));

        IF NOT EXISTS (SELECT 1 FROM dbo.CT_CONG_VIEC WHERE MaCongViec = @MaCongViec1 AND TenCTCV = @TenChiTiet1)
        BEGIN
            INSERT INTO dbo.CT_CONG_VIEC
            (
                MaCongViec, TenCTCV, NoiDungChiTietCV, NgayTaoCTCV,
                NgayBatDauCTCV, NgayKetThucCTCV, TrangThaiCTCV,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaCongViec1, @TenChiTiet1, N'Chi tiết trễ chính phục vụ dữ liệu train.', DATEADD(DAY, 9, @NgayBatDauDuAn),
                DATEADD(DAY, 1, @NgayBatDauCv1), @NgayKetThucCv1ThucTe, N'HoanThanh',
                0, NULL, NULL
            );
            SET @MaChiTiet1 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaChiTiet1 = MaChiTietCV
            FROM dbo.CT_CONG_VIEC
            WHERE MaCongViec = @MaCongViec1 AND TenCTCV = @TenChiTiet1
            ORDER BY MaChiTietCV DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CT_CONG_VIEC WHERE MaCongViec = @MaCongViec2 AND TenCTCV = @TenChiTiet2)
        BEGIN
            INSERT INTO dbo.CT_CONG_VIEC
            (
                MaCongViec, TenCTCV, NoiDungChiTietCV, NgayTaoCTCV,
                NgayBatDauCTCV, NgayKetThucCTCV, TrangThaiCTCV,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaCongViec2, @TenChiTiet2, N'Chi tiết phối hợp liên phòng ban.', DATEADD(DAY, 10, @NgayBatDauDuAn),
                DATEADD(DAY, 1, @NgayBatDauCv2), @NgayKetThucCv2ThucTe, N'HoanThanh',
                0, NULL, NULL
            );
            SET @MaChiTiet2 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaChiTiet2 = MaChiTietCV
            FROM dbo.CT_CONG_VIEC
            WHERE MaCongViec = @MaCongViec2 AND TenCTCV = @TenChiTiet2
            ORDER BY MaChiTietCV DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CT_CONG_VIEC WHERE MaCongViec = @MaCongViec3 AND TenCTCV = @TenChiTiet3)
        BEGIN
            INSERT INTO dbo.CT_CONG_VIEC
            (
                MaCongViec, TenCTCV, NoiDungChiTietCV, NgayTaoCTCV,
                NgayBatDauCTCV, NgayKetThucCTCV, TrangThaiCTCV,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaCongViec3, @TenChiTiet3, N'Chi tiết nghiệm thu và báo cáo cuối kỳ.', DATEADD(DAY, 11, @NgayBatDauDuAn),
                DATEADD(DAY, 1, @NgayBatDauCv3), @NgayKetThucCv3ThucTe, N'HoanThanh',
                0, NULL, NULL
            );
            SET @MaChiTiet3 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaChiTiet3 = MaChiTietCV
            FROM dbo.CT_CONG_VIEC
            WHERE MaCongViec = @MaCongViec3 AND TenCTCV = @TenChiTiet3
            ORDER BY MaChiTietCV DESC;
        END;

        SET @NgayBaoCaoGanNhat = DATEADD(DAY, -@HeSoChamCapNhat, @NgayKetThucDuAn);

        IF NOT EXISTS (SELECT 1 FROM dbo.TIEN_DO_CONG_VIEC WHERE MaChiTietCV = @MaChiTiet1 AND TrangThaiTienDo = N'DaDuyet' AND GhiChuTienDo = N'SEED_EXTRA_DUYET_CT1')
            INSERT INTO dbo.TIEN_DO_CONG_VIEC
            (
                MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet, GhiChuDuyet,
                PhanTram, GhiChuTienDo, ThoiGianCapNhat, TrangThaiCTCVDeXuat, TrangThaiTienDo
            )
            VALUES
            (
                @MaChiTiet1, @MaThanhVien1, @MaManager, DATEADD(HOUR, 3, @NgayKetThucCv1ThucTe), N'Đã duyệt báo cáo tiến độ.',
                100, N'SEED_EXTRA_DUYET_CT1', DATEADD(HOUR, -4, @NgayBaoCaoGanNhat), N'HoanThanh', N'DaDuyet'
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.TIEN_DO_CONG_VIEC WHERE MaChiTietCV = @MaChiTiet2 AND TrangThaiTienDo = N'YeuCauBoSung' AND GhiChuTienDo = N'SEED_EXTRA_BOSUNG_CT2')
            INSERT INTO dbo.TIEN_DO_CONG_VIEC
            (
                MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet, GhiChuDuyet,
                PhanTram, GhiChuTienDo, ThoiGianCapNhat, TrangThaiCTCVDeXuat, TrangThaiTienDo
            )
            VALUES
            (
                @MaChiTiet2, @MaThanhVien2, @MaManager, DATEADD(HOUR, 2, @NgayKetThucCv2ThucTe), N'Yêu cầu cập nhật thêm thông tin.',
                85, N'SEED_EXTRA_BOSUNG_CT2', DATEADD(HOUR, -3, @NgayBaoCaoGanNhat), N'DangThucHien', N'YeuCauBoSung'
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.TIEN_DO_CONG_VIEC WHERE MaChiTietCV = @MaChiTiet2 AND TrangThaiTienDo = N'DaDuyet' AND GhiChuTienDo = N'SEED_EXTRA_DUYET_CT2')
            INSERT INTO dbo.TIEN_DO_CONG_VIEC
            (
                MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet, GhiChuDuyet,
                PhanTram, GhiChuTienDo, ThoiGianCapNhat, TrangThaiCTCVDeXuat, TrangThaiTienDo
            )
            VALUES
            (
                @MaChiTiet2, @MaThanhVien2, @MaManager, DATEADD(HOUR, 1, @NgayKetThucCv2ThucTe), N'Đã duyệt sau bổ sung.',
                100, N'SEED_EXTRA_DUYET_CT2', DATEADD(HOUR, -2, @NgayBaoCaoGanNhat), N'HoanThanh', N'DaDuyet'
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.TIEN_DO_CONG_VIEC WHERE MaChiTietCV = @MaChiTiet3 AND TrangThaiTienDo = N'TuChoi' AND GhiChuTienDo = N'SEED_EXTRA_TUCHOI_CT3')
            INSERT INTO dbo.TIEN_DO_CONG_VIEC
            (
                MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet, GhiChuDuyet,
                PhanTram, GhiChuTienDo, ThoiGianCapNhat, TrangThaiCTCVDeXuat, TrangThaiTienDo
            )
            VALUES
            (
                @MaChiTiet3, @MaThanhVien1, @MaManager, DATEADD(HOUR, 2, @NgayKetThucCv3ThucTe), N'Từ chối do chưa đạt tiêu chí.',
                70, N'SEED_EXTRA_TUCHOI_CT3', DATEADD(HOUR, -1, @NgayBaoCaoGanNhat), N'DangThucHien', N'TuChoi'
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.TIEN_DO_CONG_VIEC WHERE MaChiTietCV = @MaChiTiet3 AND TrangThaiTienDo = N'ChoDuyet' AND GhiChuTienDo = N'SEED_EXTRA_CHODUYET_CT3')
            INSERT INTO dbo.TIEN_DO_CONG_VIEC
            (
                MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet, GhiChuDuyet,
                PhanTram, GhiChuTienDo, ThoiGianCapNhat, TrangThaiCTCVDeXuat, TrangThaiTienDo
            )
            VALUES
            (
                @MaChiTiet3, @MaThanhVien1, NULL, NULL, NULL,
                90, N'SEED_EXTRA_CHODUYET_CT3', @NgayBaoCaoGanNhat, N'DangThucHien', N'ChoDuyet'
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.NGAN_SACH WHERE MaDuAn = @MaDuAn AND MoTaNganSach = N'SEED_EXTRA_NGAN_SACH_CHINH' AND ISNULL(IsDeleted, 0) = 0)
        BEGIN
            INSERT INTO dbo.NGAN_SACH
            (
                MaNguoiDungDuyet, MaNguoiDungDeXuat, MaDuAn,
                NganSach, Version, IsActive, MoTaNganSach,
                NgayCapNhatNganSach, NgayDuyetNganSach, TrangThaiNganSach,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaManager, @MaThanhVien1, @MaDuAn,
                @NganSachDuKien, 1, 1, N'SEED_EXTRA_NGAN_SACH_CHINH',
                DATEADD(DAY, -12, @NgayKetThucDuAn), DATEADD(DAY, -11, @NgayKetThucDuAn), N'DaDuyet',
                0, NULL, NULL
            );
            SET @MaNganSach = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaNganSach = MaNganSach
            FROM dbo.NGAN_SACH
            WHERE MaDuAn = @MaDuAn
              AND MoTaNganSach = N'SEED_EXTRA_NGAN_SACH_CHINH'
              AND ISNULL(IsDeleted, 0) = 0
            ORDER BY MaNganSach DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CHI_PHI WHERE MaNganSach = @MaNganSach AND NoiDungChiPhi = N'SEED_EXTRA_CHI_PHI_1')
            INSERT INTO dbo.CHI_PHI
            (
                MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi,
                NgayChi, TrangThaiChiPhi, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaCongViec1, @MaNganSach, N'SEED_EXTRA_CHI_PHI_1', @ChiPhi1,
                DATEADD(DAY, -10, @NgayKetThucDuAn), N'DaDuyet', 0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.CHI_PHI WHERE MaNganSach = @MaNganSach AND NoiDungChiPhi = N'SEED_EXTRA_CHI_PHI_2')
            INSERT INTO dbo.CHI_PHI
            (
                MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi,
                NgayChi, TrangThaiChiPhi, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaCongViec2, @MaNganSach, N'SEED_EXTRA_CHI_PHI_2', @ChiPhi2,
                DATEADD(DAY, -7, @NgayKetThucDuAn), N'DaDuyet', 0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.CHI_PHI WHERE MaNganSach = @MaNganSach AND NoiDungChiPhi = N'SEED_EXTRA_CHI_PHI_3')
            INSERT INTO dbo.CHI_PHI
            (
                MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi,
                NgayChi, TrangThaiChiPhi, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaCongViec3, @MaNganSach, N'SEED_EXTRA_CHI_PHI_3', @ChiPhi3,
                DATEADD(DAY, -3, @NgayKetThucDuAn), N'DaDuyet', 0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.DE_XUAT_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenCongViecDeXuat = N'SEED_EXTRA_DXCV_CHO_DUYET')
            INSERT INTO dbo.DE_XUAT_CONG_VIEC
            (
                MaDuAn, MaDanhMucCV, MaMucDo, MaNguoiDungDeXuat, MaNguoiDungDuyet,
                TenCongViecDeXuat, MoTaCongViecDeXuat, ChiPhiDeXuat,
                NgayBatDauCongViecDeXuat, NgayKetThucCVDeXuatDuKien,
                NgayDeXuatCongViec, NgayDuyetDeXuatCongViec, TrangThaiCongViecDeXuat,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaDanhMuc1, @MaMucDo1, @MaThanhVien1, NULL,
                N'SEED_EXTRA_DXCV_CHO_DUYET', N'Đề xuất công việc đang chờ duyệt.', CAST(@NganSachDuKien * 0.06 AS DECIMAL(18,2)),
                DATEADD(DAY, -20, @NgayKetThucDuAn), DATEADD(DAY, -6, @NgayKetThucDuAn),
                DATEADD(DAY, -22, @NgayKetThucDuAn), NULL, N'ChoDuyet',
                0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.DE_XUAT_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenCongViecDeXuat = N'SEED_EXTRA_DXCV_TU_CHOI')
            INSERT INTO dbo.DE_XUAT_CONG_VIEC
            (
                MaDuAn, MaDanhMucCV, MaMucDo, MaNguoiDungDeXuat, MaNguoiDungDuyet,
                TenCongViecDeXuat, MoTaCongViecDeXuat, ChiPhiDeXuat,
                NgayBatDauCongViecDeXuat, NgayKetThucCVDeXuatDuKien,
                NgayDeXuatCongViec, NgayDuyetDeXuatCongViec, TrangThaiCongViecDeXuat,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaDanhMuc2, @MaMucDo2, @MaThanhVien2, @MaManager,
                N'SEED_EXTRA_DXCV_TU_CHOI', N'Đề xuất công việc bị từ chối.', CAST(@NganSachDuKien * 0.04 AS DECIMAL(18,2)),
                DATEADD(DAY, -24, @NgayKetThucDuAn), DATEADD(DAY, -12, @NgayKetThucDuAn),
                DATEADD(DAY, -25, @NgayKetThucDuAn), DATEADD(DAY, -23, @NgayKetThucDuAn), N'TuChoi',
                0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.DE_XUAT_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenCongViecDeXuat = N'SEED_EXTRA_DXCV_DA_DUYET')
            INSERT INTO dbo.DE_XUAT_CONG_VIEC
            (
                MaDuAn, MaDanhMucCV, MaMucDo, MaNguoiDungDeXuat, MaNguoiDungDuyet,
                TenCongViecDeXuat, MoTaCongViecDeXuat, ChiPhiDeXuat,
                NgayBatDauCongViecDeXuat, NgayKetThucCVDeXuatDuKien,
                NgayDeXuatCongViec, NgayDuyetDeXuatCongViec, TrangThaiCongViecDeXuat,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaDanhMuc1, @MaMucDo3, @MaThanhVien1, @MaManager,
                N'SEED_EXTRA_DXCV_DA_DUYET', N'Đề xuất công việc đã duyệt.', CAST(@NganSachDuKien * 0.05 AS DECIMAL(18,2)),
                DATEADD(DAY, -18, @NgayKetThucDuAn), DATEADD(DAY, -4, @NgayKetThucDuAn),
                DATEADD(DAY, -21, @NgayKetThucDuAn), DATEADD(DAY, -17, @NgayKetThucDuAn), N'DaDuyet',
                0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.DE_XUAT_NGAN_SACH WHERE MaDuAn = @MaDuAn AND LyDoDeXuat = N'SEED_EXTRA_DXNS_CHO_DUYET')
            INSERT INTO dbo.DE_XUAT_NGAN_SACH
            (
                MaDuAn, MaNganSachCu, NganSachCu, NganSachDeXuat, LyDoDeXuat,
                MaNguoiDungDeXuat, MaNguoiDungDuyet,
                NgayDeXuat, NgayDuyet, TrangThaiDeXuat,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaNganSach, @NganSachDuKien, CAST(@NganSachDuKien * 1.10 AS DECIMAL(18,2)), N'SEED_EXTRA_DXNS_CHO_DUYET',
                @MaThanhVien1, NULL,
                DATEADD(DAY, -19, @NgayKetThucDuAn), NULL, N'ChoDuyet',
                0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.DE_XUAT_NGAN_SACH WHERE MaDuAn = @MaDuAn AND LyDoDeXuat = N'SEED_EXTRA_DXNS_TU_CHOI')
            INSERT INTO dbo.DE_XUAT_NGAN_SACH
            (
                MaDuAn, MaNganSachCu, NganSachCu, NganSachDeXuat, LyDoDeXuat,
                MaNguoiDungDeXuat, MaNguoiDungDuyet,
                NgayDeXuat, NgayDuyet, TrangThaiDeXuat,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaNganSach, @NganSachDuKien, CAST(@NganSachDuKien * 1.18 AS DECIMAL(18,2)), N'SEED_EXTRA_DXNS_TU_CHOI',
                @MaThanhVien2, @MaManager,
                DATEADD(DAY, -18, @NgayKetThucDuAn), DATEADD(DAY, -16, @NgayKetThucDuAn), N'TuChoi',
                0, NULL, NULL
            );

        IF NOT EXISTS (SELECT 1 FROM dbo.DE_XUAT_NGAN_SACH WHERE MaDuAn = @MaDuAn AND LyDoDeXuat = N'SEED_EXTRA_DXNS_DA_DUYET')
            INSERT INTO dbo.DE_XUAT_NGAN_SACH
            (
                MaDuAn, MaNganSachCu, NganSachCu, NganSachDeXuat, LyDoDeXuat,
                MaNguoiDungDeXuat, MaNguoiDungDuyet,
                NgayDeXuat, NgayDuyet, TrangThaiDeXuat,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaNganSach, @NganSachDuKien, CAST(@NganSachDuKien * 1.08 AS DECIMAL(18,2)), N'SEED_EXTRA_DXNS_DA_DUYET',
                @MaThanhVien1, @MaManager,
                DATEADD(DAY, -17, @NgayKetThucDuAn), DATEADD(DAY, -13, @NgayKetThucDuAn), N'DaDuyet',
                0, NULL, NULL
            );

        SET @Loop = 1;
        WHILE @Loop <= @SoLanNhatKyNhanSu
        BEGIN
            IF NOT EXISTS
            (
                SELECT 1
                FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk
                WHERE nk.MaDuAn = @MaDuAn
                  AND nk.NkHanhDongPTDA = CONCAT
                  (
                      CASE (@Loop % 5)
                          WHEN 1 THEN N'Thêm nhân sự dự án'
                          WHEN 2 THEN N'Xóa nhân sự dự án'
                          WHEN 3 THEN N'Thay đổi phụ trách dự án'
                          WHEN 4 THEN N'Điều chuyển nhân sự dự án'
                          ELSE N'Cập nhật vai trò phụ trách dự án'
                      END,
                      N' - ',
                      @GhiChuDuAn,
                      N' #',
                      @Loop
                  )
            )
            BEGIN
                INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
                VALUES
                (
                    CASE WHEN @Loop % 2 = 0 THEN @MaThanhVien1 ELSE @MaThanhVien2 END,
                    @MaDuAn,
                    CONCAT
                    (
                        CASE (@Loop % 5)
                            WHEN 1 THEN N'Thêm nhân sự dự án'
                            WHEN 2 THEN N'Xóa nhân sự dự án'
                            WHEN 3 THEN N'Thay đổi phụ trách dự án'
                            WHEN 4 THEN N'Điều chuyển nhân sự dự án'
                            ELSE N'Cập nhật vai trò phụ trách dự án'
                        END,
                        N' - ',
                        @GhiChuDuAn,
                        N' #',
                        @Loop
                    ),
                    DATEADD(DAY, -(@Loop + 12), @NgayKetThucDuAn)
                );
            END;

            SET @Loop += 1;
        END;

        SET @Loop = 1;
        WHILE @Loop <= @SoLanDoiQuanLy
        BEGIN
            SELECT @MaQuanLyDeXuat = MaNguoiDung
            FROM @ManagerPool
            WHERE RowNo = ((@Seq + @Loop) % @ManagerCount) + 1;

            IF @MaQuanLyDeXuat = @MaManager
                SELECT @MaQuanLyDeXuat = MaNguoiDung
                FROM @ManagerPool
                WHERE RowNo = ((@Seq + @Loop + 3) % @ManagerCount) + 1;

            SET @NgayYeuCau = DATEADD(DAY, -(@Loop + 10), @NgayKetThucDuAn);

            IF @MaQuanLyDeXuat <> @MaManager
               AND NOT EXISTS
               (
                   SELECT 1
                   FROM dbo.YEU_CAU_DOI_QUAN_LY yc
                   WHERE yc.MaDuAn = @MaDuAn
                     AND yc.MaQuanLyHienTai = @MaManager
                     AND yc.MaQuanLyDeXuat = @MaQuanLyDeXuat
                     AND yc.NgayTaoYeuCauDoiQuanLy = @NgayYeuCau
                     AND yc.TrangThaiYeuCauDoiQuanLy COLLATE Latin1_General_CI_AI IN (N'DaDuyet', N'Đã duyệt')
                     AND ISNULL(yc.IsDeleted, 0) = 0
               )
            BEGIN
                INSERT INTO dbo.YEU_CAU_DOI_QUAN_LY
                (
                    MaQuanLyDeXuat, MaDuAn, MaNguoiDungDuyet, MaQuanLyHienTai,
                    TrangThaiYeuCauDoiQuanLy, NgayTaoYeuCauDoiQuanLy, NgayDuyetYeuCauDoiQuanLy,
                    IsDeleted, DeletedAt, DeletedBy
                )
                VALUES
                (
                    @MaQuanLyDeXuat, @MaDuAn, @MaManager, @MaManager,
                    N'DaDuyet', @NgayYeuCau, DATEADD(HOUR, 4, @NgayYeuCau),
                    0, NULL, NULL
                );
            END;

            SET @Loop += 1;
        END;

        IF NOT EXISTS
        (
            SELECT 1
            FROM dbo.AI_NGUYEN_NHAN ann
            WHERE ann.MaDuAn = @MaDuAn
              AND ann.MaDMNguyenNhan = @MaDMNguyenNhan
              AND ann.GhiChuXacNhan = CONCAT(@ReasonNotePrefix, N' - ', @GhiChuDuAn)
              AND ISNULL(ann.IsDeleted, 0) = 0
        )
        BEGIN
            INSERT INTO dbo.AI_NGUYEN_NHAN
            (
                MaDuAn, MaDMNguyenNhan, DoTinCay,
                NgayXacNhan, MaNguoiDungXacNhan, GhiChuXacNhan,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn,
                @MaDMNguyenNhan,
                CAST(0.82 + ((@ThuTuTrongNhom % 12) * 0.01) AS FLOAT),
                DATEADD(MINUTE, @ThuTuTrongNhom, DATEADD(DAY, -2, @NgayKetThucDuAn)),
                @MaManager,
                CONCAT(@ReasonNotePrefix, N' - ', @GhiChuDuAn),
                0,
                NULL,
                NULL
            );
        END;

        SET @Seq += 1;
    END;

    DECLARE @SeedProjects TABLE (MaDuAn INT NOT NULL PRIMARY KEY, GhiChuDuAn NVARCHAR(255) NOT NULL);

    INSERT INTO @SeedProjects (MaDuAn, GhiChuDuAn)
    SELECT da.MaDuAn, da.GhiChuDuAn
    FROM dbo.DU_AN da
    WHERE da.GhiChuDuAn LIKE @Prefix + N'_%'
      AND ISNULL(da.IsDeleted, 0) = 0;

    SELECT COUNT(*) AS SoDuAnSeed
    FROM @SeedProjects;

    SELECT N'DU_AN' AS Bang, COUNT(*) AS SoDong
    FROM dbo.DU_AN da
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = da.MaDuAn
    UNION ALL
    SELECT N'NHAN_VIEN_DU_AN', COUNT(*)
    FROM dbo.NHAN_VIEN_DU_AN nvda
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = nvda.MaDuAn
    UNION ALL
    SELECT N'DANH_MUC_CONG_VIEC', COUNT(*)
    FROM dbo.DANH_MUC_CONG_VIEC dmcv
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn
    UNION ALL
    SELECT N'CONG_VIEC', COUNT(*)
    FROM dbo.CONG_VIEC cv
    INNER JOIN dbo.DANH_MUC_CONG_VIEC dmcv ON dmcv.MaDanhMucCV = cv.MaDanhMucCV
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn
    UNION ALL
    SELECT N'CT_CONG_VIEC', COUNT(*)
    FROM dbo.CT_CONG_VIEC ct
    INNER JOIN dbo.CONG_VIEC cv ON cv.MaCongViec = ct.MaCongViec
    INNER JOIN dbo.DANH_MUC_CONG_VIEC dmcv ON dmcv.MaDanhMucCV = cv.MaDanhMucCV
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn
    UNION ALL
    SELECT N'TIEN_DO_CONG_VIEC', COUNT(*)
    FROM dbo.TIEN_DO_CONG_VIEC td
    INNER JOIN dbo.CT_CONG_VIEC ct ON ct.MaChiTietCV = td.MaChiTietCV
    INNER JOIN dbo.CONG_VIEC cv ON cv.MaCongViec = ct.MaCongViec
    INNER JOIN dbo.DANH_MUC_CONG_VIEC dmcv ON dmcv.MaDanhMucCV = cv.MaDanhMucCV
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn
    UNION ALL
    SELECT N'NGAN_SACH', COUNT(*)
    FROM dbo.NGAN_SACH ns
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = ns.MaDuAn
    UNION ALL
    SELECT N'CHI_PHI', COUNT(*)
    FROM dbo.CHI_PHI cp
    INNER JOIN dbo.NGAN_SACH ns ON ns.MaNganSach = cp.MaNganSach
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = ns.MaDuAn
    UNION ALL
    SELECT N'DE_XUAT_CONG_VIEC', COUNT(*)
    FROM dbo.DE_XUAT_CONG_VIEC dxcv
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = dxcv.MaDuAn
    UNION ALL
    SELECT N'DE_XUAT_NGAN_SACH', COUNT(*)
    FROM dbo.DE_XUAT_NGAN_SACH dxns
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = dxns.MaDuAn
    UNION ALL
    SELECT N'NHAT_KY_PHU_TRACH_DU_AN', COUNT(*)
    FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = nk.MaDuAn
    UNION ALL
    SELECT N'YEU_CAU_DOI_QUAN_LY', COUNT(*)
    FROM dbo.YEU_CAU_DOI_QUAN_LY yc
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = yc.MaDuAn
    UNION ALL
    SELECT N'AI_NGUYEN_NHAN', COUNT(*)
    FROM dbo.AI_NGUYEN_NHAN ann
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = ann.MaDuAn
    WHERE ISNULL(ann.IsDeleted, 0) = 0;

    SELECT
        rm.ReasonOrder,
        rm.TenNguyenNhan,
        COUNT(ann.MaAINguyenNhan) AS SoDuAnTreCoNhan
    FROM @ReasonMap rm
    LEFT JOIN dbo.AI_NGUYEN_NHAN ann
        ON ann.MaDMNguyenNhan = rm.MaDMNguyenNhan
       AND ISNULL(ann.IsDeleted, 0) = 0
    LEFT JOIN @SeedProjects sp
        ON sp.MaDuAn = ann.MaDuAn
    WHERE sp.MaDuAn IS NOT NULL
    GROUP BY rm.ReasonOrder, rm.TenNguyenNhan
    ORDER BY rm.ReasonOrder;

    ;WITH CongViecTreTheoDuAn AS
    (
        SELECT
            dmcv.MaDuAn,
            SUM
            (
                CASE
                    WHEN cv.NgayKetThucCVDuKien IS NULL THEN 0
                    WHEN cv.TrangThaiCongViec COLLATE Latin1_General_CI_AI IN (N'HoanThanh', N'Đã hoàn thành', N'DaHoanThanh')
                         AND cv.NgayKetThucCVThucTe IS NOT NULL
                         AND CAST(cv.NgayKetThucCVThucTe AS DATE) > CAST(cv.NgayKetThucCVDuKien AS DATE)
                        THEN 1
                    WHEN cv.TrangThaiCongViec COLLATE Latin1_General_CI_AI NOT IN (N'HoanThanh', N'Đã hoàn thành', N'DaHoanThanh')
                         AND CAST(@Today AS DATE) > CAST(cv.NgayKetThucCVDuKien AS DATE)
                        THEN 1
                    ELSE 0
                END
            ) AS SoCongViecTre
        FROM dbo.DANH_MUC_CONG_VIEC dmcv
        INNER JOIN dbo.CONG_VIEC cv ON cv.MaDanhMucCV = dmcv.MaDanhMucCV
        INNER JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn
        WHERE ISNULL(dmcv.IsDeleted, 0) = 0
          AND ISNULL(cv.IsDeleted, 0) = 0
        GROUP BY dmcv.MaDuAn
    )
    SELECT
        da.MaDuAn,
        da.TenDuAn,
        da.TrangThaiDuAn,
        ISNULL(cvt.SoCongViecTre, 0) AS SoCongViecTre,
        ann.MaDMNguyenNhan,
        dm.TenNguyenNhan
    FROM dbo.DU_AN da
    INNER JOIN @SeedProjects sp ON sp.MaDuAn = da.MaDuAn
    INNER JOIN dbo.AI_NGUYEN_NHAN ann ON ann.MaDuAn = da.MaDuAn AND ISNULL(ann.IsDeleted, 0) = 0
    INNER JOIN dbo.DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ann.MaDMNguyenNhan
    LEFT JOIN CongViecTreTheoDuAn cvt ON cvt.MaDuAn = da.MaDuAn
    WHERE ISNULL(cvt.SoCongViecTre, 0) = 0
    ORDER BY da.MaDuAn;

    PRINT N'Đã seed dữ liệu nghiệp vụ train mở rộng cho AI nguyên nhân trễ dự án.';
    PRINT N'Cần chạy chức năng tổng hợp AI_DATASET trong MVC sau khi seed.';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrorNumber INT = ERROR_NUMBER();
    DECLARE @ErrorLine INT = ERROR_LINE();

    RAISERROR(N'[seed-ai-training-extra.sql] Lỗi %d tại dòng %d: %s', 16, 1, @ErrorNumber, @ErrorLine, @ErrorMessage);
END CATCH;
GO
