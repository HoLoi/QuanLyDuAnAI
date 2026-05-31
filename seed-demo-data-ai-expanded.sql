USE [QuanLyDuAnAI]
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Now DATETIME2(7) = SYSDATETIME();
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);
    DECLARE @Prefix NVARCHAR(120) = N'SEED_AI_REASON_EXPANDED';

    IF OBJECT_ID(N'dbo.DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.DANH_MUC_CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.CT_CONG_VIEC', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NGAN_SACH', N'U') IS NULL
       OR OBJECT_ID(N'dbo.CHI_PHI', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NHAN_VIEN_DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NHAT_KY_PHU_TRACH_DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.YEU_CAU_DOI_QUAN_LY', N'U') IS NULL
       OR OBJECT_ID(N'dbo.AI_NGUYEN_NHAN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.AI_DATASET', N'U') IS NULL
       OR OBJECT_ID(N'dbo.DM_NGUYEN_NHAN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.NGUOI_DUNG', N'U') IS NULL
       OR OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NULL
       OR OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NULL
       OR OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NULL
       OR OBJECT_ID(N'dbo.LOAI_DU_AN', N'U') IS NULL
       OR OBJECT_ID(N'dbo.MUC_DO_UU_TIEN', N'U') IS NULL
    BEGIN
        THROW 52100, N'Thiếu bảng bắt buộc để chạy seed-demo-data-ai-expanded.sql.', 1;
    END;

    IF COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatCongViecChoDuyet') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoDeXuatCongViecChoDuyet INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatCongViecBiTuChoi') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoDeXuatCongViecBiTuChoi INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'ThoiGianDuyetCongViecTrungBinh') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD ThoiGianDuyetCongViecTrungBinh FLOAT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatNganSachChoDuyet') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoDeXuatNganSachChoDuyet INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatNganSachBiTuChoi') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoDeXuatNganSachBiTuChoi INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'ThoiGianDuyetNganSachTrungBinh') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD ThoiGianDuyetNganSachTrungBinh FLOAT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoBaoCaoTienDoChoDuyet') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoBaoCaoTienDoChoDuyet INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoBaoCaoTienDoBiTuChoi') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoBaoCaoTienDoBiTuChoi INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoBaoCaoTienDoYeuCauBoSung') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoBaoCaoTienDoYeuCauBoSung INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'TyLeBaoCaoTienDoBiTuChoi') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD TyLeBaoCaoTienDoBiTuChoi FLOAT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoLanCapNhatTienDo') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoLanCapNhatTienDo INT NULL;
    IF COL_LENGTH('dbo.AI_DATASET', 'SoNgayChamCapNhatTienDo') IS NULL
        ALTER TABLE dbo.AI_DATASET ADD SoNgayChamCapNhatTienDo INT NULL;

    IF COL_LENGTH('dbo.AI_DATASET', 'LaDuAnTre') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'MaDMNguyenNhan') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoNhanVienDuAn') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'TongSoCongViec') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoCongViecTre') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'TyLeCongViecTre') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'ChiPhiDuKien') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'ChiPhiThucTe') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'ChenhLechChiPhi') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoLanThayDoiNhanSu') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoLanThayDoiQuanLy') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoNgayTreTienDo') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatCongViecChoDuyet') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatCongViecBiTuChoi') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'ThoiGianDuyetCongViecTrungBinh') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatNganSachChoDuyet') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoDeXuatNganSachBiTuChoi') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'ThoiGianDuyetNganSachTrungBinh') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoBaoCaoTienDoChoDuyet') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoBaoCaoTienDoBiTuChoi') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoBaoCaoTienDoYeuCauBoSung') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'TyLeBaoCaoTienDoBiTuChoi') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoLanCapNhatTienDo') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'SoNgayChamCapNhatTienDo') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'NgayTongHop') IS NULL
       OR COL_LENGTH('dbo.AI_DATASET', 'GhiChuDataset') IS NULL
    BEGIN
        THROW 52101, N'Thiếu cột bắt buộc trong AI_DATASET.', 1;
    END;

    IF COL_LENGTH('dbo.AI_NGUYEN_NHAN', 'GhiChuXacNhan') IS NULL
    BEGIN
        THROW 52102, N'Thiếu cột GhiChuXacNhan trong AI_NGUYEN_NHAN.', 1;
    END;

    DECLARE @ReasonConfig TABLE
    (
        ReasonOrder INT NOT NULL PRIMARY KEY,
        TenNguyenNhan NVARCHAR(255) NOT NULL,
        SoDuAnTre INT NOT NULL,
        NhanSuMin INT NOT NULL,
        NhanSuMax INT NOT NULL,
        TaskMin INT NOT NULL,
        TaskMax INT NOT NULL,
        TreTaskMin INT NOT NULL,
        TreTaskMax INT NOT NULL,
        TreNgayMin INT NOT NULL,
        TreNgayMax INT NOT NULL,
        ThayDoiNhanSuMin INT NOT NULL,
        ThayDoiNhanSuMax INT NOT NULL,
        ThayDoiQuanLyMin INT NOT NULL,
        ThayDoiQuanLyMax INT NOT NULL,
        LechChiPhiPctMin INT NOT NULL,
        LechChiPhiPctMax INT NOT NULL
    );

    INSERT INTO @ReasonConfig
    (
        ReasonOrder, TenNguyenNhan, SoDuAnTre,
        NhanSuMin, NhanSuMax, TaskMin, TaskMax,
        TreTaskMin, TreTaskMax, TreNgayMin, TreNgayMax,
        ThayDoiNhanSuMin, ThayDoiNhanSuMax,
        ThayDoiQuanLyMin, ThayDoiQuanLyMax,
        LechChiPhiPctMin, LechChiPhiPctMax
    )
    VALUES
        (1, N'Thiếu nhân sự', 24, 3, 7, 24, 40, 7, 15, 6, 18, 4, 9, 0, 2, -2, 14),
        (2, N'Thay đổi yêu cầu liên tục', 24, 6, 12, 35, 60, 8, 18, 7, 17, 2, 6, 1, 4, 6, 20),
        (3, N'Quy trình xử lý chậm', 24, 6, 12, 25, 48, 6, 14, 9, 24, 1, 4, 3, 8, -2, 12),
        (4, N'Vượt ngân sách', 24, 6, 13, 24, 48, 5, 13, 6, 20, 1, 4, 0, 3, 15, 45),
        (5, N'Rủi ro kỹ thuật', 24, 6, 13, 30, 55, 10, 22, 12, 35, 1, 5, 0, 2, 3, 22),
        (6, N'Phối hợp công việc chưa tốt', 24, 7, 14, 40, 70, 14, 30, 9, 26, 1, 5, 0, 3, 0, 16),
        (7, N'Thông tin đầu vào chưa đầy đủ', 24, 6, 11, 28, 46, 7, 14, 8, 20, 1, 4, 0, 1, -2, 10),
        (8, N'Ước lượng thời gian chưa chính xác', 24, 5, 11, 28, 50, 10, 24, 14, 34, 0, 2, 0, 1, -1, 12),
        (9, N'Tiến độ cập nhật không đầy đủ', 24, 6, 12, 26, 46, 6, 14, 5, 18, 1, 4, 1, 3, -3, 10),
        (10, N'Khác', 24, 6, 12, 24, 45, 6, 16, 7, 22, 1, 4, 0, 2, -1, 18);

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
        TenNguyenNhan NVARCHAR(255) NOT NULL
    );

    INSERT INTO @ReasonMap (ReasonOrder, MaDMNguyenNhan, TenNguyenNhan)
    SELECT rc.ReasonOrder, dm.MaDMNguyenNhan, dm.TenNguyenNhan
    FROM @ReasonConfig rc
    JOIN dbo.DM_NGUYEN_NHAN dm
      ON dm.TenNguyenNhan COLLATE Latin1_General_CI_AI = rc.TenNguyenNhan COLLATE Latin1_General_CI_AI;

    IF (SELECT COUNT(*) FROM @ReasonMap) <> 10
    BEGIN
        THROW 52103, N'Không map đủ 10 nguyên nhân trong DM_NGUYEN_NHAN.', 1;
    END;

    DECLARE @ReasonPool TABLE
    (
        ReasonOrder INT NOT NULL PRIMARY KEY,
        MaDMNguyenNhan INT NOT NULL,
        TenNguyenNhan NVARCHAR(255) NOT NULL,
        SoDuAnTre INT NOT NULL,
        DelayStartSeq INT NOT NULL,
        DelayEndSeq INT NOT NULL
    );

    ;WITH reason_window AS
    (
        SELECT
            rc.ReasonOrder,
            rm.MaDMNguyenNhan,
            rm.TenNguyenNhan,
            rc.SoDuAnTre,
            SUM(rc.SoDuAnTre) OVER (ORDER BY rc.ReasonOrder ROWS UNBOUNDED PRECEDING) - rc.SoDuAnTre + 1 AS DelayStartSeq,
            SUM(rc.SoDuAnTre) OVER (ORDER BY rc.ReasonOrder ROWS UNBOUNDED PRECEDING) AS DelayEndSeq
        FROM @ReasonConfig rc
        JOIN @ReasonMap rm ON rm.ReasonOrder = rc.ReasonOrder
    )
    INSERT INTO @ReasonPool (ReasonOrder, MaDMNguyenNhan, TenNguyenNhan, SoDuAnTre, DelayStartSeq, DelayEndSeq)
    SELECT ReasonOrder, MaDMNguyenNhan, TenNguyenNhan, SoDuAnTre, DelayStartSeq, DelayEndSeq
    FROM reason_window;

    DECLARE @DelayProjectTarget INT = (SELECT SUM(SoDuAnTre) FROM @ReasonPool);
    DECLARE @OnTimeProjectTarget INT = 60;
    DECLARE @TotalProjectTarget INT = @DelayProjectTarget + @OnTimeProjectTarget;

    IF @DelayProjectTarget < 240
    BEGIN
        THROW 52117, N'Tổng số dòng trễ seed phải >= 240 để đủ dữ liệu train.', 1;
    END;

    DECLARE @ManagerPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaNguoiDung INT NOT NULL UNIQUE
    );

    INSERT INTO @ManagerPool (MaNguoiDung)
    SELECT DISTINCT au.MaNguoiDung
    FROM dbo.AspNetUsers au
    JOIN dbo.AspNetUserRoles aur ON aur.Asp_Id = au.Id
    JOIN dbo.AspNetRoles ar ON ar.Id = aur.Id
    WHERE ar.Name COLLATE Latin1_General_CI_AI = N'Manager'
      AND au.MaNguoiDung IS NOT NULL;

    IF NOT EXISTS (SELECT 1 FROM @ManagerPool)
    BEGIN
        INSERT INTO @ManagerPool (MaNguoiDung)
        SELECT nd.MaNguoiDung
        FROM dbo.NGUOI_DUNG nd
        JOIN dbo.CHUC_DANH cd ON cd.MaChucDanh = nd.MaChucDanh
        WHERE ISNULL(nd.IsDeleted, 0) = 0
          AND
          (
              cd.TenChucDanh COLLATE Latin1_General_CI_AI LIKE N'%quan ly%'
              OR cd.TenChucDanh COLLATE Latin1_General_CI_AI LIKE N'%manager%'
          )
          AND NOT EXISTS (SELECT 1 FROM @ManagerPool mp WHERE mp.MaNguoiDung = nd.MaNguoiDung);
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
        THROW 52104, N'Không có người dùng hợp lệ để làm quản lý seed.', 1;
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
    IF @UserCount = 0
    BEGIN
        THROW 52105, N'Không có người dùng nền để seed.', 1;
    END;

    DECLARE @LoaiPool TABLE (RowNo INT IDENTITY(1,1) PRIMARY KEY, MaLoaiDuAn INT NOT NULL UNIQUE);
    INSERT INTO @LoaiPool (MaLoaiDuAn)
    SELECT MaLoaiDuAn FROM dbo.LOAI_DU_AN ORDER BY MaLoaiDuAn;
    DECLARE @LoaiCount INT = (SELECT COUNT(*) FROM @LoaiPool);
    IF @LoaiCount = 0
    BEGIN
        THROW 52106, N'Không có LOAI_DU_AN để seed.', 1;
    END;

    DECLARE @MucDoPool TABLE (RowNo INT IDENTITY(1,1) PRIMARY KEY, MaMucDo INT NOT NULL UNIQUE);
    INSERT INTO @MucDoPool (MaMucDo)
    SELECT MaMucDo FROM dbo.MUC_DO_UU_TIEN ORDER BY MaMucDo;
    DECLARE @MucDoCount INT = (SELECT COUNT(*) FROM @MucDoPool);
    IF @MucDoCount = 0
    BEGIN
        THROW 52107, N'Không có MUC_DO_UU_TIEN để seed.', 1;
    END;

    DECLARE @SeedProjects TABLE (MaDuAn INT NOT NULL PRIMARY KEY);
    INSERT INTO @SeedProjects (MaDuAn)
    SELECT da.MaDuAn
    FROM dbo.DU_AN da
    WHERE da.GhiChuDuAn LIKE @Prefix + N'%';

    DELETE akq
    FROM dbo.AI_KET_QUA akq
    WHERE akq.MaData IN
    (
        SELECT d.MaData
        FROM dbo.AI_DATASET d
        WHERE d.GhiChuDataset LIKE @Prefix + N'%'
    );

    DELETE FROM dbo.AI_NGUYEN_NHAN
    WHERE GhiChuXacNhan LIKE @Prefix + N'%';

    DELETE FROM dbo.AI_DATASET
    WHERE GhiChuDataset LIKE @Prefix + N'%';

    IF EXISTS (SELECT 1 FROM @SeedProjects)
    BEGIN
        DELETE cp
        FROM dbo.CHI_PHI cp
        JOIN dbo.NGAN_SACH ns ON ns.MaNganSach = cp.MaNganSach
        JOIN @SeedProjects sp ON sp.MaDuAn = ns.MaDuAn;

        DELETE ns
        FROM dbo.NGAN_SACH ns
        JOIN @SeedProjects sp ON sp.MaDuAn = ns.MaDuAn;

        DELETE ctcv
        FROM dbo.CT_CONG_VIEC ctcv
        JOIN dbo.CONG_VIEC cv ON cv.MaCongViec = ctcv.MaCongViec
        JOIN dbo.DANH_MUC_CONG_VIEC dmcv ON dmcv.MaDanhMucCV = cv.MaDanhMucCV
        JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn;

        DELETE cv
        FROM dbo.CONG_VIEC cv
        JOIN dbo.DANH_MUC_CONG_VIEC dmcv ON dmcv.MaDanhMucCV = cv.MaDanhMucCV
        JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn;

        DELETE yc
        FROM dbo.YEU_CAU_DOI_QUAN_LY yc
        JOIN @SeedProjects sp ON sp.MaDuAn = yc.MaDuAn;

        DELETE nk
        FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk
        JOIN @SeedProjects sp ON sp.MaDuAn = nk.MaDuAn;

        DELETE nvda
        FROM dbo.NHAN_VIEN_DU_AN nvda
        JOIN @SeedProjects sp ON sp.MaDuAn = nvda.MaDuAn;

        DELETE dmcv
        FROM dbo.DANH_MUC_CONG_VIEC dmcv
        JOIN @SeedProjects sp ON sp.MaDuAn = dmcv.MaDuAn;

        DELETE da
        FROM dbo.DU_AN da
        JOIN @SeedProjects sp ON sp.MaDuAn = da.MaDuAn;
    END;

    DECLARE @ProjectPlan TABLE
    (
        Seq INT NOT NULL PRIMARY KEY,
        IsDelay BIT NOT NULL,
        ReasonOrder INT NULL,
        TenNguyenNhan NVARCHAR(255) NULL,
        MaDMNguyenNhan INT NULL,
        SoNhanVienTarget INT NOT NULL,
        TongCongViecTarget INT NOT NULL,
        SoCongViecTreTarget INT NOT NULL,
        SoNgayTreTarget INT NOT NULL,
        SoLanThayDoiNhanSuTarget INT NOT NULL,
        SoLanThayDoiQuanLyTarget INT NOT NULL,
        LechChiPhiPctTarget INT NOT NULL,
        NganSachTarget DECIMAL(18,2) NOT NULL,
        ChiPhiThucTeTarget DECIMAL(18,2) NOT NULL,
        MaManager INT NULL,
        MaLoaiDuAn INT NULL,
        MaMucDo INT NULL,
        MaDuAn INT NULL,
        GhiChuDuAn NVARCHAR(255) NOT NULL
    );

    ;WITH n AS
    (
        SELECT TOP (@TotalProjectTarget) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects a
        CROSS JOIN sys.all_objects b
    ),
    base AS
    (
        SELECT
            n.rn AS Seq,
            CASE WHEN n.rn <= @DelayProjectTarget THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS IsDelay,
            CASE WHEN n.rn <= @DelayProjectTarget THEN rp.ReasonOrder ELSE NULL END AS ReasonOrder,
            ABS(CAST(CHECKSUM(CONCAT(n.rn, N'|A')) AS BIGINT)) AS SeedA,
            ABS(CAST(CHECKSUM(CONCAT(n.rn, N'|B')) AS BIGINT)) AS SeedB,
            ABS(CAST(CHECKSUM(CONCAT(n.rn, N'|C')) AS BIGINT)) AS SeedC,
            ABS(CAST(CHECKSUM(CONCAT(n.rn, N'|D')) AS BIGINT)) AS SeedD,
            ABS(CAST(CHECKSUM(CONCAT(n.rn, N'|E')) AS BIGINT)) AS SeedE,
            ABS(CAST(CHECKSUM(CONCAT(n.rn, N'|F')) AS BIGINT)) AS SeedF,
            ABS(CAST(CHECKSUM(CONCAT(n.rn, N'|G')) AS BIGINT)) AS SeedG
        FROM n
        OUTER APPLY
        (
            SELECT TOP (1) rp.ReasonOrder
            FROM @ReasonPool rp
            WHERE n.rn BETWEEN rp.DelayStartSeq AND rp.DelayEndSeq
            ORDER BY rp.ReasonOrder
        ) rp
    )
    INSERT INTO @ProjectPlan
    (
        Seq, IsDelay, ReasonOrder, TenNguyenNhan, MaDMNguyenNhan,
        SoNhanVienTarget, TongCongViecTarget, SoCongViecTreTarget, SoNgayTreTarget,
        SoLanThayDoiNhanSuTarget, SoLanThayDoiQuanLyTarget, LechChiPhiPctTarget,
        NganSachTarget, ChiPhiThucTeTarget, GhiChuDuAn
    )
    SELECT
        b.Seq,
        b.IsDelay,
        b.ReasonOrder,
        rm.TenNguyenNhan,
        rm.MaDMNguyenNhan,
        CASE
            WHEN b.IsDelay = 1 THEN rp.NhanSuMin + (b.SeedA % (rp.NhanSuMax - rp.NhanSuMin + 1))
            ELSE 7 + (b.SeedA % 9)
        END AS SoNhanVienTarget,
        CASE
            WHEN b.IsDelay = 1 THEN rp.TaskMin + (b.SeedB % (rp.TaskMax - rp.TaskMin + 1))
            ELSE 20 + (b.SeedB % 19)
        END AS TongCongViecTarget,
        CASE
            WHEN b.IsDelay = 1 THEN rp.TreTaskMin + (b.SeedC % (rp.TreTaskMax - rp.TreTaskMin + 1))
            ELSE 0
        END AS SoCongViecTreTarget,
        CASE
            WHEN b.IsDelay = 1 THEN rp.TreNgayMin + (b.SeedD % (rp.TreNgayMax - rp.TreNgayMin + 1))
            ELSE 0
        END AS SoNgayTreTarget,
        CASE
            WHEN b.IsDelay = 1 THEN rp.ThayDoiNhanSuMin + (b.SeedE % (rp.ThayDoiNhanSuMax - rp.ThayDoiNhanSuMin + 1))
            ELSE (b.SeedE % 4)
        END AS SoLanThayDoiNhanSuTarget,
        CASE
            WHEN b.IsDelay = 1 THEN rp.ThayDoiQuanLyMin + (b.SeedF % (rp.ThayDoiQuanLyMax - rp.ThayDoiQuanLyMin + 1))
            ELSE (b.SeedF % 3)
        END AS SoLanThayDoiQuanLyTarget,
        CASE
            WHEN b.IsDelay = 1 THEN rp.LechChiPhiPctMin + (b.SeedG % (rp.LechChiPhiPctMax - rp.LechChiPhiPctMin + 1))
            ELSE -6 + (b.SeedG % 15)
        END AS LechChiPhiPctTarget,
        CAST(
            120000000
            + (b.Seq * 620000)
            + ((b.SeedA % 55) * 390000)
            + (ISNULL(b.ReasonOrder, 0) * 1800000)
            AS DECIMAL(18,2)
        ) AS NganSachTarget,
        CAST(0 AS DECIMAL(18,2)) AS ChiPhiThucTeTarget,
        @Prefix + N'_PROJECT_' + RIGHT(REPLICATE(N'0', 3) + CAST(b.Seq AS NVARCHAR(10)), 3) AS GhiChuDuAn
    FROM base b
    LEFT JOIN @ReasonConfig rp ON rp.ReasonOrder = b.ReasonOrder
    LEFT JOIN @ReasonMap rm ON rm.ReasonOrder = b.ReasonOrder;

    UPDATE pp
    SET pp.TongCongViecTarget = CASE
            WHEN pp.SoCongViecTreTarget >= pp.TongCongViecTarget THEN pp.SoCongViecTreTarget + 3
            ELSE pp.TongCongViecTarget
        END,
        pp.SoNhanVienTarget = CASE
            WHEN pp.SoNhanVienTarget > @UserCount THEN @UserCount
            WHEN pp.SoNhanVienTarget < 1 THEN 1
            ELSE pp.SoNhanVienTarget
        END,
        pp.SoLanThayDoiQuanLyTarget = CASE
            WHEN @ManagerCount < 2 THEN 0
            ELSE pp.SoLanThayDoiQuanLyTarget
        END
    FROM @ProjectPlan pp;

    UPDATE pp
    SET pp.ChiPhiThucTeTarget = CAST(
            pp.NganSachTarget * (1.0 + (pp.LechChiPhiPctTarget / 100.0))
            AS DECIMAL(18,2)
        )
    FROM @ProjectPlan pp;

    UPDATE pp
    SET pp.MaManager = mp.MaNguoiDung
    FROM @ProjectPlan pp
    JOIN @ManagerPool mp ON mp.RowNo = ((pp.Seq - 1) % @ManagerCount) + 1;

    UPDATE pp
    SET pp.MaLoaiDuAn = lp.MaLoaiDuAn
    FROM @ProjectPlan pp
    JOIN @LoaiPool lp ON lp.RowNo = ((pp.Seq - 1) % @LoaiCount) + 1;

    UPDATE pp
    SET pp.MaMucDo = md.MaMucDo
    FROM @ProjectPlan pp
    JOIN @MucDoPool md ON md.RowNo = ((pp.Seq - 1) % @MucDoCount) + 1;

    DECLARE @InsertedProjects TABLE (Seq INT NOT NULL PRIMARY KEY, MaDuAn INT NOT NULL);

    INSERT INTO dbo.DU_AN
    (
        MaNguoiDung, MaLoaiDuAn, TenDuAn, MoTaDuAn, NgayTaoDuAn, NgayBatDauDuAn, NgayKetThucDuAn,
        NgayHoanThanhThucTeDuAn, PhanTramHoanThanh, TrangThaiDuAn, GhiChuDuAn, IsDeleted, DeletedAt, DeletedBy
    )
    SELECT
        pp.MaManager,
        pp.MaLoaiDuAn,
        N'Dự án AI phân tích nguyên nhân ' + RIGHT(REPLICATE(N'0', 3) + CAST(pp.Seq AS NVARCHAR(10)), 3),
        N'Dữ liệu seed tự sinh nghiệp vụ để train AI phân tích nguyên nhân trễ.',
        DATEADD(DAY, -(260 + pp.Seq), @Now),
        DATEADD(DAY, -(200 + (pp.Seq % 45)), @Now),
        CASE
            WHEN pp.IsDelay = 1 THEN DATEADD(DAY, -(3 + (pp.Seq % 80)), CAST(@Today AS DATETIME2(7)))
            ELSE DATEADD(DAY, 15 + (pp.Seq % 60), CAST(@Today AS DATETIME2(7)))
        END,
        CASE
            WHEN pp.IsDelay = 1 THEN NULL
            ELSE DATEADD(DAY, 14 + (pp.Seq % 60), CAST(@Today AS DATETIME2(7)))
        END,
        CASE WHEN pp.IsDelay = 1 THEN 58 + (pp.Seq % 37) ELSE 100 END,
        CASE WHEN pp.IsDelay = 1 THEN N'DangThucHien' ELSE N'HoanThanh' END,
        pp.GhiChuDuAn,
        0, NULL, NULL
    FROM @ProjectPlan pp
    ORDER BY pp.Seq;

    INSERT INTO @InsertedProjects (Seq, MaDuAn)
    SELECT pp.Seq, da.MaDuAn
    FROM @ProjectPlan pp
    JOIN dbo.DU_AN da ON da.GhiChuDuAn = pp.GhiChuDuAn;

    UPDATE pp
    SET pp.MaDuAn = ip.MaDuAn
    FROM @ProjectPlan pp
    JOIN @InsertedProjects ip ON ip.Seq = pp.Seq;

    INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
    SELECT pp.MaDuAn, pp.MaManager, DATEADD(DAY, -(190 + pp.Seq % 40), @Now), N'Leader'
    FROM @ProjectPlan pp;

    ;WITH candidate AS
    (
        SELECT
            pp.Seq,
            pp.MaDuAn,
            pp.SoNhanVienTarget,
            up.MaNguoiDung,
            ROW_NUMBER() OVER
            (
                PARTITION BY pp.Seq
                ORDER BY ABS(CAST(CHECKSUM(CONCAT(pp.Seq, N'|MEM|', up.MaNguoiDung)) AS BIGINT)), up.MaNguoiDung
            ) AS rn
        FROM @ProjectPlan pp
        CROSS JOIN @UserPool up
        WHERE up.MaNguoiDung <> pp.MaManager
    )
    INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
    SELECT
        c.MaDuAn,
        c.MaNguoiDung,
        DATEADD(DAY, -(175 + (c.rn % 30)), @Now),
        N'Member'
    FROM candidate c
    WHERE c.rn <= CASE WHEN c.SoNhanVienTarget > 1 THEN c.SoNhanVienTarget - 1 ELSE 0 END;

    DECLARE @DanhMucMap TABLE
    (
        MaDuAn INT NOT NULL,
        CatNo INT NOT NULL,
        MaDanhMucCV INT NOT NULL,
        PRIMARY KEY (MaDuAn, CatNo)
    );

    ;WITH cat AS
    (
        SELECT 1 AS CatNo, N'Danh mục Khởi tạo' AS TenDM, N'SEED_AI_REASON_EXPANDED_DMCV_KHOI_TAO' AS PrefixTen
        UNION ALL SELECT 2, N'Danh mục Triển khai', N'SEED_AI_REASON_EXPANDED_DMCV_TRIEN_KHAI'
        UNION ALL SELECT 3, N'Danh mục Nghiệm thu', N'SEED_AI_REASON_EXPANDED_DMCV_NGHIEM_THU'
    )
    INSERT INTO dbo.DANH_MUC_CONG_VIEC
    (
        MaDuAn, TenDanhMucCV, MoTaDanhMucCV, NgayTaoDMCV, IsDeleted, DeletedAt, DeletedBy
    )
    SELECT
        pp.MaDuAn,
        cat.PrefixTen + N'_' + RIGHT(REPLICATE(N'0', 3) + CAST(pp.Seq AS NVARCHAR(10)), 3),
        cat.TenDM + N' cho seed AI reason expanded',
        DATEADD(DAY, -(200 + (pp.Seq % 35)), @Now),
        0, NULL, NULL
    FROM @ProjectPlan pp
    CROSS JOIN cat;

    ;WITH cat AS
    (
        SELECT 1 AS CatNo, N'SEED_AI_REASON_EXPANDED_DMCV_KHOI_TAO' AS PrefixTen
        UNION ALL SELECT 2, N'SEED_AI_REASON_EXPANDED_DMCV_TRIEN_KHAI'
        UNION ALL SELECT 3, N'SEED_AI_REASON_EXPANDED_DMCV_NGHIEM_THU'
    )
    INSERT INTO @DanhMucMap (MaDuAn, CatNo, MaDanhMucCV)
    SELECT
        pp.MaDuAn,
        cat.CatNo,
        dmcv.MaDanhMucCV
    FROM @ProjectPlan pp
    JOIN cat ON 1 = 1
    JOIN dbo.DANH_MUC_CONG_VIEC dmcv
      ON dmcv.MaDuAn = pp.MaDuAn
     AND dmcv.TenDanhMucCV = cat.PrefixTen + N'_' + RIGHT(REPLICATE(N'0', 3) + CAST(pp.Seq AS NVARCHAR(10)), 3);

    DECLARE @Numbers TABLE (N INT NOT NULL PRIMARY KEY);
    ;WITH n AS
    (
        SELECT TOP (90) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn
        FROM sys.all_objects
    )
    INSERT INTO @Numbers (N)
    SELECT rn FROM n;

    DECLARE @TaskMap TABLE
    (
        MaDuAn INT NOT NULL,
        TaskNo INT NOT NULL,
        MaCongViec INT NOT NULL,
        PRIMARY KEY (MaDuAn, TaskNo)
    );

    ;WITH project_dates AS
    (
        SELECT pp.Seq, pp.MaDuAn, pp.MaMucDo, pp.IsDelay, pp.TongCongViecTarget, pp.SoCongViecTreTarget, pp.SoNgayTreTarget, da.NgayBatDauDuAn
        FROM @ProjectPlan pp
        JOIN dbo.DU_AN da ON da.MaDuAn = pp.MaDuAn
    ),
    task_seed AS
    (
        SELECT
            pd.Seq,
            pd.MaDuAn,
            pd.MaMucDo,
            num.N AS TaskNo,
            dm.MaDanhMucCV,
            CASE WHEN pd.IsDelay = 1 AND num.N <= pd.SoCongViecTreTarget THEN 1 ELSE 0 END AS IsLate,
            pd.SoNgayTreTarget,
            pd.NgayBatDauDuAn
        FROM project_dates pd
        JOIN @Numbers num ON num.N <= pd.TongCongViecTarget
        JOIN @DanhMucMap dm
          ON dm.MaDuAn = pd.MaDuAn
         AND dm.CatNo = ((num.N - 1) % 3) + 1
    )
    INSERT INTO dbo.CONG_VIEC
    (
        MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, MoTaCongViec,
        NgayBatDauCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe, NgayTaoCongViec,
        TrangThaiCongViec, IsDeleted, DeletedAt, DeletedBy
    )
    SELECT
        NULL,
        ts.MaDanhMucCV,
        ts.MaMucDo,
        N'SEED_AI_REASON_EXPANDED_TASK_' + RIGHT(REPLICATE(N'0', 3) + CAST(ts.Seq AS NVARCHAR(10)), 3) + N'_' + RIGHT(REPLICATE(N'0', 2) + CAST(ts.TaskNo AS NVARCHAR(10)), 2),
        N'Công việc nghiệp vụ mô phỏng để tổng hợp AI_DATASET theo dữ liệu thật.',
        DATEADD(DAY, (ts.TaskNo % 7), ts.NgayBatDauDuAn),
        CASE
            WHEN ts.IsLate = 1 AND ts.TaskNo = 1 THEN DATEADD(DAY, -ts.SoNgayTreTarget, CAST(@Today AS DATETIME2(7)))
            WHEN ts.IsLate = 1 THEN DATEADD(DAY, -(1 + ((ts.TaskNo + ts.Seq) % 24)), CAST(@Today AS DATETIME2(7)))
            ELSE DATEADD(DAY, -(12 + ((ts.TaskNo + ts.Seq) % 28)), CAST(@Today AS DATETIME2(7)))
        END,
        CASE
            WHEN ts.IsLate = 1 AND ts.TaskNo = 1 THEN NULL
            WHEN ts.IsLate = 1 AND ((ts.TaskNo + ts.Seq) % 4 = 0) THEN NULL
            WHEN ts.IsLate = 1 THEN DATEADD
            (
                DAY,
                1 + ((ts.TaskNo + ts.Seq) % 6),
                DATEADD(DAY, -(1 + ((ts.TaskNo + ts.Seq) % 24)), CAST(@Today AS DATETIME2(7)))
            )
            ELSE DATEADD
            (
                DAY,
                -((ts.TaskNo + ts.Seq) % 2),
                DATEADD(DAY, -(12 + ((ts.TaskNo + ts.Seq) % 28)), CAST(@Today AS DATETIME2(7)))
            )
        END,
        DATEADD(DAY, -(210 + ts.Seq % 40), @Now),
        CASE
            WHEN ts.IsLate = 1 AND ts.TaskNo = 1 THEN N'DangThucHien'
            WHEN ts.IsLate = 1 AND ((ts.TaskNo + ts.Seq) % 4 = 0) THEN N'DangThucHien'
            ELSE N'HoanThanh'
        END,
        0, NULL, NULL
    FROM task_seed ts;

    INSERT INTO @TaskMap (MaDuAn, TaskNo, MaCongViec)
    SELECT
        pp.MaDuAn,
        num.N AS TaskNo,
        cv.MaCongViec
    FROM @ProjectPlan pp
    JOIN @Numbers num ON num.N <= pp.TongCongViecTarget
    JOIN dbo.CONG_VIEC cv
      ON cv.TenCongViec = N'SEED_AI_REASON_EXPANDED_TASK_' + RIGHT(REPLICATE(N'0', 3) + CAST(pp.Seq AS NVARCHAR(10)), 3) + N'_' + RIGHT(REPLICATE(N'0', 2) + CAST(num.N AS NVARCHAR(10)), 2)
    JOIN dbo.DANH_MUC_CONG_VIEC dmcv ON dmcv.MaDanhMucCV = cv.MaDanhMucCV AND dmcv.MaDuAn = pp.MaDuAn;

    INSERT INTO dbo.CT_CONG_VIEC
    (
        MaCongViec, TenCTCV, NoiDungChiTietCV, NgayTaoCTCV, NgayBatDauCTCV, NgayKetThucCTCV, TrangThaiCTCV, IsDeleted, DeletedAt, DeletedBy
    )
    SELECT
        tm.MaCongViec,
        N'SEED_AI_REASON_EXPANDED_CTCV_' + RIGHT(REPLICATE(N'0', 3) + CAST(pp.Seq AS NVARCHAR(10)), 3) + N'_' + RIGHT(REPLICATE(N'0', 2) + CAST(tm.TaskNo AS NVARCHAR(10)), 2),
        N'Chi tiết công việc mô phỏng cho dữ liệu AI reason expanded.',
        DATEADD(DAY, -(190 + (tm.TaskNo % 30)), @Now),
        cv.NgayBatDauCongViec,
        CASE WHEN cv.TrangThaiCongViec = N'HoanThanh' THEN cv.NgayKetThucCVThucTe ELSE NULL END,
        CASE WHEN cv.TrangThaiCongViec = N'HoanThanh' THEN N'HoanThanh' ELSE N'DangThucHien' END,
        0, NULL, NULL
    FROM @TaskMap tm
    JOIN dbo.CONG_VIEC cv ON cv.MaCongViec = tm.MaCongViec
    JOIN @ProjectPlan pp ON pp.MaDuAn = tm.MaDuAn;

    INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
    SELECT
        pp.MaManager,
        pp.MaDuAn,
        CASE ((num.N - 1) % 5)
            WHEN 0 THEN N'Thêm nhân sự dự án'
            WHEN 1 THEN N'Xóa nhân sự dự án'
            WHEN 2 THEN N'Điều chuyển nhân sự'
            WHEN 3 THEN N'Cập nhật vai trò phụ trách'
            ELSE N'Gán thành viên mới'
        END,
        DATEADD(DAY, -(120 + ((pp.Seq + num.N) % 60)), @Now)
    FROM @ProjectPlan pp
    JOIN @Numbers num ON num.N <= pp.SoLanThayDoiNhanSuTarget;

    IF @ManagerCount > 1
    BEGIN
        ;WITH manager_change_seed AS
        (
            SELECT
                pp.Seq,
                pp.MaDuAn,
                pp.MaManager AS MaQuanLyHienTai,
                num.N AS ChangeNo,
                ((pp.Seq + num.N - 1) % @ManagerCount) + 1 AS TargetRowNo,
                pp.SoLanThayDoiQuanLyTarget
            FROM @ProjectPlan pp
            JOIN @Numbers num ON num.N <= pp.SoLanThayDoiQuanLyTarget
        )
        INSERT INTO dbo.YEU_CAU_DOI_QUAN_LY
        (
            MaQuanLyDeXuat, MaDuAn, MaNguoiDungDuyet, MaQuanLyHienTai,
            TrangThaiYeuCauDoiQuanLy, NgayTaoYeuCauDoiQuanLy, NgayDuyetYeuCauDoiQuanLy,
            IsDeleted, DeletedAt, DeletedBy
        )
        SELECT
            CASE
                WHEN mp.MaNguoiDung = mcs.MaQuanLyHienTai THEN mp2.MaNguoiDung
                ELSE mp.MaNguoiDung
            END AS MaQuanLyDeXuat,
            mcs.MaDuAn,
            mcs.MaQuanLyHienTai,
            mcs.MaQuanLyHienTai,
            N'DaDuyet',
            DATEADD(DAY, -(95 + ((mcs.Seq + mcs.ChangeNo) % 45)), @Now),
            DATEADD(DAY, -(93 + ((mcs.Seq + mcs.ChangeNo) % 45)), @Now),
            0, NULL, NULL
        FROM manager_change_seed mcs
        JOIN @ManagerPool mp ON mp.RowNo = mcs.TargetRowNo
        JOIN @ManagerPool mp2 ON mp2.RowNo = CASE WHEN mcs.TargetRowNo = 1 THEN 2 ELSE 1 END;
    END;

    DECLARE @BudgetMap TABLE (MaDuAn INT NOT NULL PRIMARY KEY, MaNganSach INT NOT NULL);
    INSERT INTO dbo.NGAN_SACH
    (
        MaNguoiDungDuyet, MaNguoiDungDeXuat, MaDuAn, NganSach, Version, IsActive, MoTaNganSach,
        NgayCapNhatNganSach, NgayDuyetNganSach, TrangThaiNganSach, IsDeleted, DeletedAt, DeletedBy
    )
    OUTPUT inserted.MaDuAn, inserted.MaNganSach INTO @BudgetMap (MaDuAn, MaNganSach)
    SELECT
        pp.MaManager,
        pp.MaManager,
        pp.MaDuAn,
        pp.NganSachTarget,
        1,
        1,
        N'Ngân sách seed ' + @Prefix,
        DATEADD(DAY, -(85 + pp.Seq % 35), @Now),
        DATEADD(DAY, -(84 + pp.Seq % 35), @Now),
        N'DaDuyet',
        0, NULL, NULL
    FROM @ProjectPlan pp;

    ;WITH task_pick AS
    (
        SELECT
            tm.MaDuAn,
            tm.MaCongViec,
            ROW_NUMBER() OVER (PARTITION BY tm.MaDuAn ORDER BY tm.TaskNo) AS rn
        FROM @TaskMap tm
    ),
    cost_parts AS
    (
        SELECT
            pp.MaDuAn,
            CAST(ROUND(pp.ChiPhiThucTeTarget * 0.42, 2) AS DECIMAL(18,2)) AS Part1,
            CAST(ROUND(pp.ChiPhiThucTeTarget * 0.33, 2) AS DECIMAL(18,2)) AS Part2,
            CAST(pp.ChiPhiThucTeTarget AS DECIMAL(18,2)) AS TotalCost
        FROM @ProjectPlan pp
    ),
    cost_assembled AS
    (
        SELECT
            cp.MaDuAn,
            cp.Part1,
            cp.Part2,
            CAST(cp.TotalCost - cp.Part1 - cp.Part2 AS DECIMAL(18,2)) AS Part3
        FROM cost_parts cp
    )
    INSERT INTO dbo.CHI_PHI
    (
        MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi, NgayChi, TrangThaiChiPhi, IsDeleted, DeletedAt, DeletedBy
    )
    SELECT
        tp.MaCongViec,
        bm.MaNganSach,
        N'Chi phí seed ' + @Prefix + N' đợt ' + CAST(tp.rn AS NVARCHAR(5)),
        CASE tp.rn WHEN 1 THEN ca.Part1 WHEN 2 THEN ca.Part2 ELSE ca.Part3 END,
        DATEADD(DAY, -(70 + tp.rn + (pp.Seq % 20)), @Now),
        N'DaDuyet',
        0, NULL, NULL
    FROM task_pick tp
    JOIN @BudgetMap bm ON bm.MaDuAn = tp.MaDuAn
    JOIN cost_assembled ca ON ca.MaDuAn = tp.MaDuAn
    JOIN @ProjectPlan pp ON pp.MaDuAn = tp.MaDuAn
    WHERE tp.rn <= 3;

    INSERT INTO dbo.AI_NGUYEN_NHAN
    (
        MaDuAn, MaDMNguyenNhan, DoTinCay, NgayXacNhan, MaNguoiDungXacNhan, GhiChuXacNhan, IsDeleted, DeletedAt, DeletedBy
    )
    SELECT
        pp.MaDuAn,
        pp.MaDMNguyenNhan,
        CAST(0.62 + ((ABS(CAST(CHECKSUM(CONCAT(pp.Seq, N'|CONF')) AS BIGINT)) % 28) / 100.0) AS FLOAT),
        DATEADD(DAY, -(2 + (pp.Seq % 45)), DATEADD(MINUTE, (pp.Seq % 300), @Now)),
        pp.MaManager,
        @Prefix + N'_CONFIRM_' + RIGHT(REPLICATE(N'0', 3) + CAST(pp.Seq AS NVARCHAR(10)), 3),
        0,
        NULL,
        NULL
    FROM @ProjectPlan pp
    WHERE pp.IsDelay = 1;

    ;WITH seed_project AS
    (
        SELECT pp.Seq, pp.MaDuAn
        FROM @ProjectPlan pp
    ),
    so_nhan_vien AS
    (
        SELECT nv.MaDuAn, COUNT(DISTINCT nv.MaNguoiDung) AS SoNhanVienDuAn
        FROM dbo.NHAN_VIEN_DU_AN nv
        JOIN seed_project sp ON sp.MaDuAn = nv.MaDuAn
        GROUP BY nv.MaDuAn
    ),
    cong_viec_raw AS
    (
        SELECT
            dm.MaDuAn,
            cv.TrangThaiCongViec,
            cv.NgayKetThucCVDuKien,
            cv.NgayKetThucCVThucTe
        FROM dbo.CONG_VIEC cv
        JOIN dbo.DANH_MUC_CONG_VIEC dm ON dm.MaDanhMucCV = cv.MaDanhMucCV
        JOIN seed_project sp ON sp.MaDuAn = dm.MaDuAn
        WHERE ISNULL(cv.IsDeleted, 0) = 0
          AND ISNULL(dm.IsDeleted, 0) = 0
    ),
    cong_viec_agg AS
    (
        SELECT
            r.MaDuAn,
            COUNT(*) AS TongSoCongViec,
            SUM
            (
                CASE
                    WHEN r.NgayKetThucCVDuKien IS NULL THEN 0
                    WHEN r.TrangThaiCongViec IN (N'HoanThanh', N'Hoàn thành', N'Done', N'Completed')
                        THEN CASE WHEN r.NgayKetThucCVThucTe IS NOT NULL AND CAST(r.NgayKetThucCVThucTe AS DATE) > CAST(r.NgayKetThucCVDuKien AS DATE) THEN 1 ELSE 0 END
                    ELSE CASE WHEN @Today > CAST(r.NgayKetThucCVDuKien AS DATE) THEN 1 ELSE 0 END
                END
            ) AS SoCongViecTre,
            MAX
            (
                CASE
                    WHEN r.NgayKetThucCVDuKien IS NULL THEN 0
                    WHEN r.TrangThaiCongViec IN (N'HoanThanh', N'Hoàn thành', N'Done', N'Completed')
                        THEN CASE
                            WHEN r.NgayKetThucCVThucTe IS NULL THEN 0
                            WHEN DATEDIFF(DAY, CAST(r.NgayKetThucCVDuKien AS DATE), CAST(r.NgayKetThucCVThucTe AS DATE)) > 0
                                THEN DATEDIFF(DAY, CAST(r.NgayKetThucCVDuKien AS DATE), CAST(r.NgayKetThucCVThucTe AS DATE))
                            ELSE 0
                        END
                    ELSE CASE
                        WHEN DATEDIFF(DAY, CAST(r.NgayKetThucCVDuKien AS DATE), @Today) > 0
                            THEN DATEDIFF(DAY, CAST(r.NgayKetThucCVDuKien AS DATE), @Today)
                        ELSE 0
                    END
                END
            ) AS SoNgayTreTienDo
        FROM cong_viec_raw r
        GROUP BY r.MaDuAn
    ),
    chi_phi_du_kien AS
    (
        SELECT
            ns.MaDuAn,
            SUM(ns.NganSach) AS ChiPhiDuKien
        FROM dbo.NGAN_SACH ns
        JOIN seed_project sp ON sp.MaDuAn = ns.MaDuAn
        WHERE ISNULL(ns.IsDeleted, 0) = 0
          AND ns.IsActive = 1
          AND ns.TrangThaiNganSach IN (N'DaDuyet', N'Đã duyệt')
        GROUP BY ns.MaDuAn
    ),
    chi_phi_thuc_te AS
    (
        SELECT
            ns.MaDuAn,
            SUM(cp.SoTienDaChi) AS ChiPhiThucTe
        FROM dbo.CHI_PHI cp
        JOIN dbo.NGAN_SACH ns ON ns.MaNganSach = cp.MaNganSach
        JOIN seed_project sp ON sp.MaDuAn = ns.MaDuAn
        WHERE ISNULL(cp.IsDeleted, 0) = 0
          AND ISNULL(ns.IsDeleted, 0) = 0
        GROUP BY ns.MaDuAn
    ),
    thay_doi_nhan_su AS
    (
        SELECT
            nk.MaDuAn,
            COUNT(*) AS SoLanThayDoiNhanSu
        FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk
        JOIN seed_project sp ON sp.MaDuAn = nk.MaDuAn
        WHERE
            (
                LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%themnhansu%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%themnhanvien%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%themthanhvien%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%xoanhansu%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%xoanhanvien%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%xoathanhvien%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%dieuchuyennhansu%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%capnhatvaitrophutrach%'
                OR LOWER(REPLACE(ISNULL(nk.NkHanhDongPTDA, N''), N' ', N'')) COLLATE Latin1_General_CI_AI LIKE N'%ganthanhvien%'
            )
        GROUP BY nk.MaDuAn
    ),
    thay_doi_quan_ly AS
    (
        SELECT
            yc.MaDuAn,
            COUNT(*) AS SoLanThayDoiQuanLy
        FROM dbo.YEU_CAU_DOI_QUAN_LY yc
        JOIN seed_project sp ON sp.MaDuAn = yc.MaDuAn
        WHERE ISNULL(yc.IsDeleted, 0) = 0
          AND yc.NgayDuyetYeuCauDoiQuanLy IS NOT NULL
          AND yc.MaQuanLyHienTai <> yc.MaQuanLyDeXuat
          AND yc.TrangThaiYeuCauDoiQuanLy COLLATE Latin1_General_CI_AI IN (N'DaDuyet', N'Đã duyệt')
        GROUP BY yc.MaDuAn
    ),
    de_xuat_cong_viec_agg AS
    (
        SELECT
            dxcv.MaDuAn,
            SUM(CASE WHEN dxcv.TrangThaiCongViecDeXuat COLLATE Latin1_General_CI_AI IN (N'ChoDuyet', N'Chờ duyệt', N'Cho duyet') THEN 1 ELSE 0 END) AS SoDeXuatCongViecChoDuyet,
            SUM(CASE WHEN dxcv.TrangThaiCongViecDeXuat COLLATE Latin1_General_CI_AI IN (N'TuChoi', N'Từ chối', N'Tu choi') THEN 1 ELSE 0 END) AS SoDeXuatCongViecBiTuChoi,
            AVG(
                CASE
                    WHEN dxcv.NgayDeXuatCongViec IS NOT NULL AND dxcv.NgayDuyetDeXuatCongViec IS NOT NULL
                        THEN CAST(CASE WHEN DATEDIFF(MINUTE, dxcv.NgayDeXuatCongViec, dxcv.NgayDuyetDeXuatCongViec) > 0
                                    THEN DATEDIFF(MINUTE, dxcv.NgayDeXuatCongViec, dxcv.NgayDuyetDeXuatCongViec) / 1440.0
                                    ELSE 0 END AS FLOAT)
                    ELSE NULL
                END
            ) AS ThoiGianDuyetCongViecTrungBinh
        FROM dbo.DE_XUAT_CONG_VIEC dxcv
        JOIN seed_project sp ON sp.MaDuAn = dxcv.MaDuAn
        WHERE ISNULL(dxcv.IsDeleted, 0) = 0
        GROUP BY dxcv.MaDuAn
    ),
    de_xuat_ngan_sach_agg AS
    (
        SELECT
            dxns.MaDuAn,
            SUM(CASE WHEN dxns.TrangThaiDeXuat COLLATE Latin1_General_CI_AI IN (N'ChoDuyet', N'Chờ duyệt', N'Cho duyet') THEN 1 ELSE 0 END) AS SoDeXuatNganSachChoDuyet,
            SUM(CASE WHEN dxns.TrangThaiDeXuat COLLATE Latin1_General_CI_AI IN (N'TuChoi', N'Từ chối', N'Tu choi') THEN 1 ELSE 0 END) AS SoDeXuatNganSachBiTuChoi,
            AVG(
                CASE
                    WHEN dxns.NgayDeXuat IS NOT NULL AND dxns.NgayDuyet IS NOT NULL
                        THEN CAST(CASE WHEN DATEDIFF(MINUTE, dxns.NgayDeXuat, dxns.NgayDuyet) > 0
                                    THEN DATEDIFF(MINUTE, dxns.NgayDeXuat, dxns.NgayDuyet) / 1440.0
                                    ELSE 0 END AS FLOAT)
                    ELSE NULL
                END
            ) AS ThoiGianDuyetNganSachTrungBinh
        FROM dbo.DE_XUAT_NGAN_SACH dxns
        JOIN seed_project sp ON sp.MaDuAn = dxns.MaDuAn
        WHERE ISNULL(dxns.IsDeleted, 0) = 0
        GROUP BY dxns.MaDuAn
    ),
    tien_do_bao_cao_agg AS
    (
        SELECT
            dm.MaDuAn,
            SUM(CASE WHEN td.TrangThaiTienDo COLLATE Latin1_General_CI_AI IN (N'ChoDuyet', N'Chờ duyệt', N'Cho duyet') THEN 1 ELSE 0 END) AS SoBaoCaoTienDoChoDuyet,
            SUM(CASE WHEN td.TrangThaiTienDo COLLATE Latin1_General_CI_AI IN (N'TuChoi', N'Từ chối', N'Tu choi') THEN 1 ELSE 0 END) AS SoBaoCaoTienDoBiTuChoi,
            SUM(CASE WHEN td.TrangThaiTienDo COLLATE Latin1_General_CI_AI IN (N'YeuCauBoSung', N'Yêu cầu bổ sung', N'Yeu cau bo sung') THEN 1 ELSE 0 END) AS SoBaoCaoTienDoYeuCauBoSung,
            COUNT(td.MaTienDo) AS SoLanCapNhatTienDo,
            MAX(CAST(td.ThoiGianCapNhat AS DATE)) AS LanCapNhatTienDoGanNhat
        FROM dbo.DANH_MUC_CONG_VIEC dm
        JOIN dbo.CONG_VIEC cv ON cv.MaDanhMucCV = dm.MaDanhMucCV AND ISNULL(cv.IsDeleted, 0) = 0
        JOIN dbo.CT_CONG_VIEC ct ON ct.MaCongViec = cv.MaCongViec AND ISNULL(ct.IsDeleted, 0) = 0
        LEFT JOIN dbo.TIEN_DO_CONG_VIEC td ON td.MaChiTietCV = ct.MaChiTietCV
        JOIN seed_project sp ON sp.MaDuAn = dm.MaDuAn
        WHERE ISNULL(dm.IsDeleted, 0) = 0
        GROUP BY dm.MaDuAn
    ),
    ly_do_xac_nhan AS
    (
        SELECT
            ann.MaDuAn,
            ann.MaDMNguyenNhan,
            ROW_NUMBER() OVER (PARTITION BY ann.MaDuAn ORDER BY ann.NgayXacNhan DESC, ann.MaAINguyenNhan DESC) AS rn
        FROM dbo.AI_NGUYEN_NHAN ann
        JOIN seed_project sp ON sp.MaDuAn = ann.MaDuAn
        WHERE ISNULL(ann.IsDeleted, 0) = 0
    ),
    dataset_agg AS
    (
        SELECT
            sp.Seq,
            da.MaDuAn,
            ISNULL(sn.SoNhanVienDuAn, 0) AS SoNhanVienDuAn,
            ISNULL(cv.TongSoCongViec, 0) AS TongSoCongViec,
            ISNULL(cv.SoCongViecTre, 0) AS SoCongViecTre,
            CASE WHEN ISNULL(cv.TongSoCongViec, 0) > 0 THEN ROUND(ISNULL(cv.SoCongViecTre, 0) * 100.0 / cv.TongSoCongViec, 2) ELSE 0 END AS TyLeCongViecTre,
            ISNULL(cpd.ChiPhiDuKien, 0) AS ChiPhiDuKien,
            ISNULL(cpt.ChiPhiThucTe, 0) AS ChiPhiThucTe,
            ISNULL(cpt.ChiPhiThucTe, 0) - ISNULL(cpd.ChiPhiDuKien, 0) AS ChenhLechChiPhi,
            ISNULL(tdns.SoLanThayDoiNhanSu, 0) AS SoLanThayDoiNhanSu,
            ISNULL(tdql.SoLanThayDoiQuanLy, 0) AS SoLanThayDoiQuanLy,
            ISNULL(cv.SoNgayTreTienDo, 0) AS SoNgayTreTienDo,
            ISNULL(dxcv.SoDeXuatCongViecChoDuyet, 0) AS SoDeXuatCongViecChoDuyet,
            ISNULL(dxcv.SoDeXuatCongViecBiTuChoi, 0) AS SoDeXuatCongViecBiTuChoi,
            ISNULL(ROUND(dxcv.ThoiGianDuyetCongViecTrungBinh, 2), 0) AS ThoiGianDuyetCongViecTrungBinh,
            ISNULL(dxns.SoDeXuatNganSachChoDuyet, 0) AS SoDeXuatNganSachChoDuyet,
            ISNULL(dxns.SoDeXuatNganSachBiTuChoi, 0) AS SoDeXuatNganSachBiTuChoi,
            ISNULL(ROUND(dxns.ThoiGianDuyetNganSachTrungBinh, 2), 0) AS ThoiGianDuyetNganSachTrungBinh,
            ISNULL(td.SoBaoCaoTienDoChoDuyet, 0) AS SoBaoCaoTienDoChoDuyet,
            ISNULL(td.SoBaoCaoTienDoBiTuChoi, 0) AS SoBaoCaoTienDoBiTuChoi,
            ISNULL(td.SoBaoCaoTienDoYeuCauBoSung, 0) AS SoBaoCaoTienDoYeuCauBoSung,
            CASE
                WHEN ISNULL(td.SoLanCapNhatTienDo, 0) > 0
                    THEN ROUND(ISNULL(td.SoBaoCaoTienDoBiTuChoi, 0) * 100.0 / td.SoLanCapNhatTienDo, 2)
                ELSE 0
            END AS TyLeBaoCaoTienDoBiTuChoi,
            ISNULL(td.SoLanCapNhatTienDo, 0) AS SoLanCapNhatTienDo,
            CASE
                WHEN td.LanCapNhatTienDoGanNhat IS NOT NULL
                    THEN CASE
                            WHEN DATEDIFF(DAY, td.LanCapNhatTienDoGanNhat, ISNULL(CAST(da.NgayKetThucDuAn AS DATE), @Today)) > 0
                                THEN DATEDIFF(DAY, td.LanCapNhatTienDoGanNhat, ISNULL(CAST(da.NgayKetThucDuAn AS DATE), @Today))
                            ELSE 0
                         END
                ELSE 0
            END AS SoNgayChamCapNhatTienDo,
            CASE
                WHEN
                    (
                        da.NgayKetThucDuAn IS NOT NULL
                        AND CAST(da.NgayKetThucDuAn AS DATE) < @Today
                        AND NOT (da.TrangThaiDuAn IN (N'HoanThanh', N'Hoàn thành', N'Done', N'Completed') OR ISNULL(da.PhanTramHoanThanh, 0) >= 100)
                    )
                    OR ISNULL(cv.SoCongViecTre, 0) > 0
                    OR (CASE WHEN ISNULL(cv.TongSoCongViec, 0) > 0 THEN ROUND(ISNULL(cv.SoCongViecTre, 0) * 100.0 / cv.TongSoCongViec, 2) ELSE 0 END) >= 30
                    OR ISNULL(cv.SoNgayTreTienDo, 0) > 0
                THEN CAST(1 AS BIT)
                ELSE CAST(0 AS BIT)
            END AS LaDuAnTre,
            lx.MaDMNguyenNhan
        FROM seed_project sp
        JOIN dbo.DU_AN da ON da.MaDuAn = sp.MaDuAn
        LEFT JOIN so_nhan_vien sn ON sn.MaDuAn = da.MaDuAn
        LEFT JOIN cong_viec_agg cv ON cv.MaDuAn = da.MaDuAn
        LEFT JOIN chi_phi_du_kien cpd ON cpd.MaDuAn = da.MaDuAn
        LEFT JOIN chi_phi_thuc_te cpt ON cpt.MaDuAn = da.MaDuAn
        LEFT JOIN thay_doi_nhan_su tdns ON tdns.MaDuAn = da.MaDuAn
        LEFT JOIN thay_doi_quan_ly tdql ON tdql.MaDuAn = da.MaDuAn
        LEFT JOIN de_xuat_cong_viec_agg dxcv ON dxcv.MaDuAn = da.MaDuAn
        LEFT JOIN de_xuat_ngan_sach_agg dxns ON dxns.MaDuAn = da.MaDuAn
        LEFT JOIN tien_do_bao_cao_agg td ON td.MaDuAn = da.MaDuAn
        LEFT JOIN ly_do_xac_nhan lx ON lx.MaDuAn = da.MaDuAn AND lx.rn = 1
    )
    INSERT INTO dbo.AI_DATASET
    (
        MaDuAn, SoNhanVienDuAn, TongSoCongViec, SoCongViecTre, TyLeCongViecTre,
        ChiPhiDuKien, ChiPhiThucTe, ChenhLechChiPhi, SoLanThayDoiNhanSu, SoLanThayDoiQuanLy,
        SoNgayTreTienDo, SoDeXuatCongViecChoDuyet, SoDeXuatCongViecBiTuChoi, ThoiGianDuyetCongViecTrungBinh,
        SoDeXuatNganSachChoDuyet, SoDeXuatNganSachBiTuChoi, ThoiGianDuyetNganSachTrungBinh,
        SoBaoCaoTienDoChoDuyet, SoBaoCaoTienDoBiTuChoi, SoBaoCaoTienDoYeuCauBoSung, TyLeBaoCaoTienDoBiTuChoi,
        SoLanCapNhatTienDo, SoNgayChamCapNhatTienDo, LaDuAnTre, MaDMNguyenNhan, NgayTongHop, GhiChuDataset
    )
    SELECT
        da.MaDuAn,
        da.SoNhanVienDuAn,
        da.TongSoCongViec,
        da.SoCongViecTre,
        da.TyLeCongViecTre,
        CAST(da.ChiPhiDuKien AS DECIMAL(18,2)),
        CAST(da.ChiPhiThucTe AS DECIMAL(18,2)),
        CAST(da.ChenhLechChiPhi AS DECIMAL(18,2)),
        da.SoLanThayDoiNhanSu,
        da.SoLanThayDoiQuanLy,
        da.SoNgayTreTienDo,
        da.SoDeXuatCongViecChoDuyet,
        da.SoDeXuatCongViecBiTuChoi,
        da.ThoiGianDuyetCongViecTrungBinh,
        da.SoDeXuatNganSachChoDuyet,
        da.SoDeXuatNganSachBiTuChoi,
        da.ThoiGianDuyetNganSachTrungBinh,
        da.SoBaoCaoTienDoChoDuyet,
        da.SoBaoCaoTienDoBiTuChoi,
        da.SoBaoCaoTienDoYeuCauBoSung,
        da.TyLeBaoCaoTienDoBiTuChoi,
        da.SoLanCapNhatTienDo,
        da.SoNgayChamCapNhatTienDo,
        da.LaDuAnTre,
        CASE WHEN da.LaDuAnTre = 1 THEN da.MaDMNguyenNhan ELSE NULL END,
        DATEADD(MINUTE, da.Seq % 180, DATEADD(DAY, -(1 + da.Seq % 25), @Now)),
        @Prefix + N'_DATASET_' + RIGHT(REPLICATE(N'0', 3) + CAST(da.Seq AS NVARCHAR(10)), 3)
    FROM dataset_agg da;

    DECLARE @TotalSeedProjects INT = (
        SELECT COUNT(*)
        FROM dbo.DU_AN da
        WHERE da.GhiChuDuAn LIKE @Prefix + N'%'
    );
    IF @TotalSeedProjects <> @TotalProjectTarget
    BEGIN
        THROW 52108, N'Tổng số dự án demo AI không đúng target.', 1;
    END;

    DECLARE @DelayProjects INT = (
        SELECT COUNT(*)
        FROM @ProjectPlan
        WHERE IsDelay = 1
    );
    IF @DelayProjects <> @DelayProjectTarget
    BEGIN
        THROW 52109, N'Số dự án trễ trong plan không đúng target.', 1;
    END;

    DECLARE @OnTimeProjects INT = (
        SELECT COUNT(*)
        FROM @ProjectPlan
        WHERE IsDelay = 0
    );
    IF @OnTimeProjects <> @OnTimeProjectTarget
    BEGIN
        THROW 52110, N'Số dự án không trễ trong plan không đúng target.', 1;
    END;

    DECLARE @SeedReasonRows INT = (
        SELECT COUNT(*)
        FROM dbo.AI_NGUYEN_NHAN ann
        WHERE ann.GhiChuXacNhan LIKE @Prefix + N'%'
    );
    IF @SeedReasonRows <> @DelayProjectTarget
    BEGIN
        THROW 52111, N'Số dòng AI_NGUYEN_NHAN seed không đúng target.', 1;
    END;

    DECLARE @SeedDatasetRows INT = (
        SELECT COUNT(*)
        FROM dbo.AI_DATASET ds
        WHERE ds.GhiChuDataset LIKE @Prefix + N'%'
    );
    IF @SeedDatasetRows <> @TotalProjectTarget
    BEGIN
        THROW 52112, N'Số dòng AI_DATASET seed không đúng target.', 1;
    END;

    DECLARE @ValidTrainRows INT = (
        SELECT COUNT(*)
        FROM dbo.AI_DATASET ds
        WHERE ds.GhiChuDataset LIKE @Prefix + N'%'
          AND ds.LaDuAnTre = 1
          AND ds.MaDMNguyenNhan IS NOT NULL
          AND ds.SoNhanVienDuAn IS NOT NULL
          AND ds.TongSoCongViec IS NOT NULL
          AND ds.SoCongViecTre IS NOT NULL
          AND ds.TyLeCongViecTre IS NOT NULL
          AND ds.ChiPhiDuKien IS NOT NULL
          AND ds.ChiPhiThucTe IS NOT NULL
          AND ds.ChenhLechChiPhi IS NOT NULL
          AND ds.SoLanThayDoiNhanSu IS NOT NULL
          AND ds.SoLanThayDoiQuanLy IS NOT NULL
          AND ds.SoNgayTreTienDo IS NOT NULL
          AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
          AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
          AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
          AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
          AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
          AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
          AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
          AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
          AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
          AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
          AND ds.SoLanCapNhatTienDo IS NOT NULL
          AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
    );
    IF @ValidTrainRows < 240
    BEGIN
        THROW 52113, N'Số dòng train hợp lệ nhỏ hơn 240.', 1;
    END;

    DECLARE @MinReasonRows INT = (
        SELECT ISNULL(MIN(rc.SoDong), 0)
        FROM
        (
            SELECT ds.MaDMNguyenNhan, COUNT(*) AS SoDong
            FROM dbo.AI_DATASET ds
            WHERE ds.GhiChuDataset LIKE @Prefix + N'%'
              AND ds.LaDuAnTre = 1
              AND ds.MaDMNguyenNhan IS NOT NULL
            GROUP BY ds.MaDMNguyenNhan
        ) rc
    );
    IF @MinReasonRows < 20
    BEGIN
        THROW 52114, N'Có nguyên nhân có ít hơn 20 dòng train.', 1;
    END;

    IF EXISTS
    (
        SELECT 1
        FROM
        (
            SELECT
                MaDMNguyenNhan,
                COUNT(*) AS SoDong
            FROM dbo.AI_DATASET
            WHERE LaDuAnTre = 1
              AND MaDMNguyenNhan IS NOT NULL
            GROUP BY MaDMNguyenNhan
        ) x
        WHERE x.SoDong < 20
    )
    BEGIN
        THROW 50001, N'Có nguyên nhân chưa đủ 20 dòng train.', 1;
    END;

    DECLARE @OnTimeHasReason INT = (
        SELECT COUNT(*)
        FROM dbo.AI_DATASET ds
        WHERE ds.GhiChuDataset LIKE @Prefix + N'%'
          AND ds.LaDuAnTre = 0
          AND ds.MaDMNguyenNhan IS NOT NULL
    );
    IF @OnTimeHasReason > 0
    BEGIN
        THROW 52115, N'Có dự án không trễ nhưng vẫn có MaDMNguyenNhan.', 1;
    END;

    DECLARE @OverBudgetInvalid INT = (
        SELECT COUNT(*)
        FROM dbo.AI_DATASET ds
        JOIN dbo.DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
        WHERE ds.GhiChuDataset LIKE @Prefix + N'%'
          AND ds.LaDuAnTre = 1
          AND dm.TenNguyenNhan COLLATE Latin1_General_CI_AI = N'Vượt ngân sách'
          AND ds.ChenhLechChiPhi <= 0
    );
    IF @OverBudgetInvalid > 0
    BEGIN
        THROW 52116, N'Có mẫu "Vượt ngân sách" nhưng ChenhLechChiPhi <= 0.', 1;
    END;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH;
GO

DECLARE @Prefix NVARCHAR(120) = N'SEED_AI_REASON_EXPANDED';

IF OBJECT_ID('tempdb..#SeedProjects') IS NOT NULL DROP TABLE #SeedProjects;
IF OBJECT_ID('tempdb..#SeedDataset') IS NOT NULL DROP TABLE #SeedDataset;

SELECT da.MaDuAn, da.GhiChuDuAn
INTO #SeedProjects
FROM dbo.DU_AN da
WHERE da.GhiChuDuAn LIKE @Prefix + N'%';

SELECT ds.*
INTO #SeedDataset
FROM dbo.AI_DATASET ds
WHERE ds.GhiChuDataset LIKE @Prefix + N'%';

SELECT
    COUNT(*) AS TongDuAnDemoAI,
    SUM(CASE WHEN ds.LaDuAnTre = 1 THEN 1 ELSE 0 END) AS SoDuAnTre,
    SUM(CASE WHEN ds.LaDuAnTre = 0 THEN 1 ELSE 0 END) AS SoDuAnKhongTre
FROM #SeedProjects sp
LEFT JOIN #SeedDataset ds ON ds.MaDuAn = sp.MaDuAn;

SELECT
    COUNT(*) AS TongDongAiDatasetSeed
FROM #SeedDataset;

SELECT
    COUNT(*) AS SoDongTrainHopLe
FROM #SeedDataset ds
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NOT NULL
  AND ds.SoNhanVienDuAn IS NOT NULL
  AND ds.TongSoCongViec IS NOT NULL
  AND ds.SoCongViecTre IS NOT NULL
  AND ds.TyLeCongViecTre IS NOT NULL
  AND ds.ChiPhiDuKien IS NOT NULL
  AND ds.ChiPhiThucTe IS NOT NULL
  AND ds.ChenhLechChiPhi IS NOT NULL
  AND ds.SoLanThayDoiNhanSu IS NOT NULL
  AND ds.SoLanThayDoiQuanLy IS NOT NULL
  AND ds.SoNgayTreTienDo IS NOT NULL
  AND ds.SoDeXuatCongViecChoDuyet IS NOT NULL
  AND ds.SoDeXuatCongViecBiTuChoi IS NOT NULL
  AND ds.ThoiGianDuyetCongViecTrungBinh IS NOT NULL
  AND ds.SoDeXuatNganSachChoDuyet IS NOT NULL
  AND ds.SoDeXuatNganSachBiTuChoi IS NOT NULL
  AND ds.ThoiGianDuyetNganSachTrungBinh IS NOT NULL
  AND ds.SoBaoCaoTienDoChoDuyet IS NOT NULL
  AND ds.SoBaoCaoTienDoBiTuChoi IS NOT NULL
  AND ds.SoBaoCaoTienDoYeuCauBoSung IS NOT NULL
  AND ds.TyLeBaoCaoTienDoBiTuChoi IS NOT NULL
  AND ds.SoLanCapNhatTienDo IS NOT NULL
  AND ds.SoNgayChamCapNhatTienDo IS NOT NULL;

SELECT
    dm.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(*) AS SoDong
FROM dbo.AI_DATASET ds
JOIN dbo.DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NOT NULL
GROUP BY dm.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY SoDong ASC;

IF EXISTS
(
    SELECT 1
    FROM
    (
        SELECT
            MaDMNguyenNhan,
            COUNT(*) AS SoDong
        FROM dbo.AI_DATASET
        WHERE LaDuAnTre = 1
          AND MaDMNguyenNhan IS NOT NULL
        GROUP BY MaDMNguyenNhan
    ) x
    WHERE x.SoDong < 20
)
BEGIN
    THROW 50001, N'Có nguyên nhân chưa đủ 20 dòng train.', 1;
END;

SELECT
    COUNT(*) AS SoDongAiNguyenNhanGroundTruth
FROM dbo.AI_NGUYEN_NHAN ann
WHERE ann.GhiChuXacNhan LIKE @Prefix + N'%'
  AND ISNULL(ann.IsDeleted, 0) = 0;

SELECT
    CASE WHEN COL_LENGTH('dbo.AI_DATASET', 'IsTre') IS NULL THEN N'Không có cột IsTre' ELSE N'Còn cột IsTre' END AS KiemTraIsTre,
    CASE WHEN COL_LENGTH('dbo.AI_DATASET', 'TreHan') IS NULL THEN N'Không có cột TreHan' ELSE N'Còn cột TreHan' END AS KiemTraTreHan,
    CASE WHEN COL_LENGTH('dbo.AI_DATASET', 'TongGioLam') IS NULL THEN N'Không có cột TongGioLam' ELSE N'Còn cột TongGioLam' END AS KiemTraTongGioLam;

SELECT
    COUNT(*) AS SoDongVuotNganSachSaiDieuKien
FROM #SeedDataset ds
JOIN dbo.DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
WHERE ds.LaDuAnTre = 1
  AND dm.TenNguyenNhan COLLATE Latin1_General_CI_AI = N'Vượt ngân sách'
  AND ds.ChenhLechChiPhi <= 0;

SELECT
    COUNT(*) AS SoDongKhongTreCoNguyenNhan
FROM #SeedDataset ds
WHERE ds.LaDuAnTre = 0
  AND ds.MaDMNguyenNhan IS NOT NULL;

SELECT
    N'DU_AN' AS Bang,
    COUNT(*) AS SoDongSeed
FROM dbo.DU_AN da
WHERE da.GhiChuDuAn LIKE @Prefix + N'%'
UNION ALL
SELECT N'DANH_MUC_CONG_VIEC', COUNT(*)
FROM dbo.DANH_MUC_CONG_VIEC dm
WHERE dm.TenDanhMucCV LIKE @Prefix + N'%'
UNION ALL
SELECT N'CONG_VIEC', COUNT(*)
FROM dbo.CONG_VIEC cv
WHERE cv.TenCongViec LIKE @Prefix + N'%'
UNION ALL
SELECT N'CT_CONG_VIEC', COUNT(*)
FROM dbo.CT_CONG_VIEC ctcv
WHERE ctcv.TenCTCV LIKE @Prefix + N'%'
UNION ALL
SELECT N'NGAN_SACH', COUNT(*)
FROM dbo.NGAN_SACH ns
JOIN #SeedProjects sp ON sp.MaDuAn = ns.MaDuAn
WHERE ns.MoTaNganSach LIKE N'%SEED_AI_REASON_EXPANDED%'
UNION ALL
SELECT N'CHI_PHI', COUNT(*)
FROM dbo.CHI_PHI cp
WHERE cp.NoiDungChiPhi LIKE N'Chi phí seed SEED_AI_REASON_EXPANDED%'
UNION ALL
SELECT N'NHAN_VIEN_DU_AN', COUNT(*)
FROM dbo.NHAN_VIEN_DU_AN nv
JOIN #SeedProjects sp ON sp.MaDuAn = nv.MaDuAn
UNION ALL
SELECT N'NHAT_KY_PHU_TRACH_DU_AN', COUNT(*)
FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk
JOIN #SeedProjects sp ON sp.MaDuAn = nk.MaDuAn
UNION ALL
SELECT N'YEU_CAU_DOI_QUAN_LY', COUNT(*)
FROM dbo.YEU_CAU_DOI_QUAN_LY yc
JOIN #SeedProjects sp ON sp.MaDuAn = yc.MaDuAn
UNION ALL
SELECT N'AI_NGUYEN_NHAN', COUNT(*)
FROM dbo.AI_NGUYEN_NHAN ann
WHERE ann.GhiChuXacNhan LIKE @Prefix + N'%'
UNION ALL
SELECT N'AI_DATASET', COUNT(*)
FROM #SeedDataset;



