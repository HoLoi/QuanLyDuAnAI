USE [QuanLyDuAnAI]
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Now DATETIME2(7) = SYSDATETIME();
    DECLARE @Today DATE = CAST(GETDATE() AS DATE);

    PRINT N'Đang kiểm tra dữ liệu nền.';

    IF OBJECT_ID(N'dbo.NGUOI_DUNG', N'U') IS NULL OR
       OBJECT_ID(N'dbo.AspNetUsers', N'U') IS NULL OR
       OBJECT_ID(N'dbo.AspNetRoles', N'U') IS NULL OR
       OBJECT_ID(N'dbo.AspNetUserRoles', N'U') IS NULL OR
       OBJECT_ID(N'dbo.TEAM', N'U') IS NULL OR
       OBJECT_ID(N'dbo.NHAN_VIEN_TEAM', N'U') IS NULL OR
       OBJECT_ID(N'dbo.CHUC_DANH', N'U') IS NULL OR
       OBJECT_ID(N'dbo.LOAI_DU_AN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.MUC_DO_UU_TIEN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.DU_AN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.NHAN_VIEN_DU_AN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.TEAM_DU_AN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.DANH_MUC_CONG_VIEC', N'U') IS NULL OR
       OBJECT_ID(N'dbo.CONG_VIEC', N'U') IS NULL OR
       OBJECT_ID(N'dbo.CT_CONG_VIEC', N'U') IS NULL OR
       OBJECT_ID(N'dbo.PHAN_CONG_CONG_VIEC', N'U') IS NULL OR
       OBJECT_ID(N'dbo.PHAN_CONG_CT_CONG_VIEC', N'U') IS NULL OR
       OBJECT_ID(N'dbo.TIEN_DO_CONG_VIEC', N'U') IS NULL OR
       OBJECT_ID(N'dbo.NGAN_SACH', N'U') IS NULL OR
       OBJECT_ID(N'dbo.CHI_PHI', N'U') IS NULL OR
       OBJECT_ID(N'dbo.NHAT_KY_DU_AN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.NHAT_KY_PHU_TRACH_DU_AN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.YEU_CAU_DOI_QUAN_LY', N'U') IS NULL OR
       OBJECT_ID(N'dbo.AI_DATASET', N'U') IS NULL OR
       OBJECT_ID(N'dbo.AI_MODEL', N'U') IS NULL OR
       OBJECT_ID(N'dbo.AI_NGUYEN_NHAN', N'U') IS NULL OR
       OBJECT_ID(N'dbo.DM_NGUYEN_NHAN', N'U') IS NULL
    BEGIN
        RAISERROR(N'Thiếu bảng dữ liệu bắt buộc để seed.', 16, 1);
    END;

    IF COL_LENGTH('dbo.AI_MODEL', 'LoaiModel') IS NULL
    BEGIN
        ALTER TABLE dbo.AI_MODEL ADD LoaiModel NVARCHAR(50) NULL;
    END;

    IF COL_LENGTH('dbo.AI_DATASET', 'TongGioLam') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.AI_DATASET DROP COLUMN TongGioLam;
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

    DECLARE @ManagerPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaNguoiDung INT NOT NULL UNIQUE
    );

    INSERT INTO @ManagerPool (MaNguoiDung)
    SELECT DISTINCT nd.MaNguoiDung
    FROM dbo.NGUOI_DUNG nd
    INNER JOIN dbo.AspNetUsers au ON au.MaNguoiDung = nd.MaNguoiDung
    INNER JOIN dbo.AspNetUserRoles aur ON aur.Asp_Id = au.Id
    INNER JOIN dbo.AspNetRoles ar ON ar.Id = aur.Id
    WHERE ar.Name COLLATE Latin1_General_CI_AI = N'Manager';

    IF NOT EXISTS (SELECT 1 FROM @ManagerPool)
    BEGIN
        INSERT INTO @ManagerPool (MaNguoiDung)
        SELECT DISTINCT nd.MaNguoiDung
        FROM dbo.NGUOI_DUNG nd
        INNER JOIN dbo.CHUC_DANH cd ON cd.MaChucDanh = nd.MaChucDanh
        WHERE cd.TenChucDanh COLLATE Latin1_General_CI_AI LIKE N'%quan ly%'
           OR cd.TenChucDanh COLLATE Latin1_General_CI_AI LIKE N'%manager%';
    END;

    IF NOT EXISTS (SELECT 1 FROM @ManagerPool)
    BEGIN
        INSERT INTO @ManagerPool (MaNguoiDung)
        SELECT DISTINCT nd.MaNguoiDung
        FROM dbo.NGUOI_DUNG nd
        WHERE ISNULL(nd.IsDeleted, 0) = 0
          AND NOT EXISTS (SELECT 1 FROM @ManagerPool mp WHERE mp.MaNguoiDung = nd.MaNguoiDung)
        ORDER BY nd.MaNguoiDung;
    END;

    DECLARE @ManagerCount INT = (SELECT COUNT(*) FROM @ManagerPool);
    IF @ManagerCount = 0
        RAISERROR(N'Không tìm thấy người dùng nền hợp lệ để gán vai trò quản lý dự án seed.', 16, 1);

    DECLARE @TeamPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaTeam INT NOT NULL UNIQUE
    );

    INSERT INTO @TeamPool (MaTeam)
    SELECT t.MaTeam
    FROM dbo.TEAM t
    WHERE ISNULL(t.IsDeleted, 0) = 0
      AND EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_TEAM nvt WHERE nvt.MaTeam = t.MaTeam)
    ORDER BY t.MaTeam;

    DECLARE @TeamCount INT = (SELECT COUNT(*) FROM @TeamPool);
    IF @TeamCount = 0
        RAISERROR(N'Không có team hợp lệ để seed.', 16, 1);

    DECLARE @LoaiPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaLoaiDuAn INT NOT NULL UNIQUE
    );

    INSERT INTO @LoaiPool (MaLoaiDuAn)
    SELECT MaLoaiDuAn
    FROM dbo.LOAI_DU_AN
    ORDER BY MaLoaiDuAn;

    DECLARE @LoaiCount INT = (SELECT COUNT(*) FROM @LoaiPool);
    IF @LoaiCount = 0
        RAISERROR(N'Không có loại dự án hợp lệ.', 16, 1);

    DECLARE @MucDoPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaMucDo INT NOT NULL UNIQUE
    );

    INSERT INTO @MucDoPool (MaMucDo)
    SELECT MaMucDo
    FROM dbo.MUC_DO_UU_TIEN
    ORDER BY MaMucDo;

    DECLARE @MucDoCount INT = (SELECT COUNT(*) FROM @MucDoPool);
    IF @MucDoCount = 0
        RAISERROR(N'Không có mức độ ưu tiên hợp lệ.', 16, 1);

    DECLARE @DanhMucPool TABLE (RowNo INT IDENTITY(1,1) PRIMARY KEY, Ten NVARCHAR(255) NOT NULL);
    INSERT INTO @DanhMucPool (Ten)
    VALUES
        (N'Phân tích yêu cầu'),
        (N'Thiết kế hệ thống'),
        (N'Phát triển chức năng'),
        (N'Kiểm thử và nghiệm thu'),
        (N'Triển khai vận hành'),
        (N'Tài liệu và bàn giao');

    DECLARE @CongViecPool TABLE (RowNo INT IDENTITY(1,1) PRIMARY KEY, Ten NVARCHAR(255) NOT NULL);
    INSERT INTO @CongViecPool (Ten)
    VALUES
        (N'Khảo sát yêu cầu người dùng'),
        (N'Thiết kế cơ sở dữ liệu'),
        (N'Xây dựng giao diện quản trị'),
        (N'Xây dựng chức năng nghiệp vụ'),
        (N'Tích hợp phân quyền'),
        (N'Kiểm thử chức năng chính'),
        (N'Sửa lỗi sau kiểm thử'),
        (N'Chuẩn bị tài liệu bàn giao'),
        (N'Triển khai môi trường thử nghiệm'),
        (N'Đào tạo người dùng nội bộ');

    DECLARE @ChiTietPool TABLE (RowNo INT IDENTITY(1,1) PRIMARY KEY, Ten NVARCHAR(255) NOT NULL);
    INSERT INTO @ChiTietPool (Ten)
    VALUES
        (N'Thu thập yêu cầu từ phòng ban'),
        (N'Vẽ sơ đồ luồng xử lý'),
        (N'Tạo màn hình danh sách'),
        (N'Tạo form thêm mới'),
        (N'Tạo form chỉnh sửa'),
        (N'Kiểm tra validation dữ liệu'),
        (N'Kiểm thử phân quyền người dùng'),
        (N'Ghi nhận lỗi phát sinh'),
        (N'Hoàn thiện tài liệu hướng dẫn'),
        (N'Kiểm tra lần cuối trước bàn giao');

    DECLARE @DanhMucCount INT = (SELECT COUNT(*) FROM @DanhMucPool);
    DECLARE @CongViecCount INT = (SELECT COUNT(*) FROM @CongViecPool);
    DECLARE @ChiTietCount INT = (SELECT COUNT(*) FROM @ChiTietPool);

    DECLARE @ProjectPlan TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        TenDuAn NVARCHAR(255) NOT NULL,
        GhiChuDuAn NVARCHAR(255) NOT NULL UNIQUE,
        NhomDuLieu NVARCHAR(20) NOT NULL, -- Tre, DungHan, Predict
        VuotNganSachMau BIT NOT NULL
    );

    INSERT INTO @ProjectPlan (TenDuAn, GhiChuDuAn, NhomDuLieu, VuotNganSachMau)
    VALUES
        (N'Hệ thống quản lý kho nội bộ', N'SEED_AI_TRAIN_KHO_NOI_BO', N'Tre', 1),
        (N'Website bán hàng thiết bị điện tử', N'SEED_AI_TRAIN_BAN_HANG_DIEN_TU', N'Tre', 0),
        (N'Ứng dụng đặt lịch bảo trì', N'SEED_AI_TRAIN_DAT_LICH_BAO_TRI', N'Tre', 1),
        (N'Cổng thông tin nhân sự', N'SEED_AI_TRAIN_CONG_THONG_TIN_NHAN_SU', N'Tre', 0),
        (N'Hệ thống quản lý tài sản', N'SEED_AI_TRAIN_QUAN_LY_TAI_SAN', N'Tre', 1),
        (N'Ứng dụng chăm sóc khách hàng', N'SEED_AI_TRAIN_CHAM_SOC_KH', N'Tre', 0),
        (N'Website giới thiệu doanh nghiệp', N'SEED_AI_TRAIN_GIOI_THIEU_DN', N'Tre', 1),
        (N'Hệ thống quản lý đào tạo', N'SEED_AI_TRAIN_QUAN_LY_DAO_TAO', N'Tre', 0),
        (N'Phần mềm quản lý hợp đồng', N'SEED_AI_TRAIN_QUAN_LY_HOP_DONG', N'Tre', 1),
        (N'Ứng dụng theo dõi tiến độ thi công', N'SEED_AI_TRAIN_THEO_DOI_THI_CONG', N'Tre', 0),
        (N'Hệ thống báo cáo doanh thu', N'SEED_AI_TRAIN_BAO_CAO_DOANH_THU', N'Tre', 1),
        (N'Cổng đăng ký dịch vụ trực tuyến', N'SEED_AI_TRAIN_DANG_KY_DICH_VU', N'Tre', 0),
        (N'Ứng dụng quản lý bảo hành', N'SEED_AI_TRAIN_QUAN_LY_BAO_HANH', N'Tre', 1),
        (N'Hệ thống quản lý văn bản', N'SEED_AI_TRAIN_QUAN_LY_VAN_BAN', N'Tre', 0),
        (N'Nền tảng hỗ trợ khách hàng', N'SEED_AI_TRAIN_NEN_TANG_HO_TRO', N'Tre', 1),
        (N'Hệ thống điều phối bảo trì nội bộ', N'SEED_AI_TRAIN_DIEU_PHOI_BAO_TRI', N'Tre', 0),
        (N'Ứng dụng quản lý lịch sản xuất', N'SEED_AI_TRAIN_LICH_SAN_XUAT', N'Tre', 1),
        (N'Cổng tích hợp đối tác dịch vụ', N'SEED_AI_TRAIN_TICH_HOP_DOI_TAC', N'Tre', 0),
        (N'Phần mềm quản lý biên bản nghiệm thu', N'SEED_AI_TRAIN_BIEN_BAN_NGHIEM_THU', N'Tre', 1),
        (N'Hệ thống giám sát bảo mật nội bộ', N'SEED_AI_TRAIN_GIAM_SAT_BAO_MAT', N'Tre', 0),
        (N'Ứng dụng theo dõi xử lý sự cố', N'SEED_AI_TRAIN_XU_LY_SU_CO', N'Tre', 1),
        (N'Nền tảng báo cáo vận hành tập trung', N'SEED_AI_TRAIN_BAO_CAO_VAN_HANH', N'Tre', 0),
        (N'Cổng theo dõi chất lượng dự án', N'SEED_AI_TRAIN_CHAT_LUONG_DU_AN', N'Tre', 1),
        (N'Phần mềm quản lý lịch kiểm thử', N'SEED_AI_TRAIN_LICH_KIEM_THU', N'Tre', 0),
        (N'Hệ thống quản lý thay đổi cấu hình', N'SEED_AI_TRAIN_THAY_DOI_CAU_HINH', N'Tre', 1),
        (N'Ứng dụng giám sát tiến độ triển khai', N'SEED_AI_TRAIN_GIAM_SAT_TRIEN_KHAI', N'Tre', 0),
        (N'Cổng điều hành công việc liên phòng ban', N'SEED_AI_TRAIN_DIEU_HANH_LIEN_PHONG', N'Tre', 1),
        (N'Phần mềm quản lý phản hồi khách hàng', N'SEED_AI_TRAIN_PHAN_HOI_KHACH_HANG', N'Tre', 0),
        (N'Hệ thống kiểm soát rủi ro dự án', N'SEED_AI_TRAIN_RUI_RO_DU_AN', N'Tre', 1),
        (N'Ứng dụng lập kế hoạch nguồn lực', N'SEED_AI_TRAIN_KE_HOACH_NGUON_LUC', N'Tre', 0),
        (N'Website tuyển dụng nội bộ', N'SEED_AI_TRAIN_TUYEN_DUNG_NOI_BO', N'DungHan', 0),
        (N'Hệ thống kiểm soát chi phí', N'SEED_AI_TRAIN_KIEM_SOAT_CHI_PHI', N'DungHan', 0),
        (N'Phần mềm quản lý lịch họp', N'SEED_AI_TRAIN_QUAN_LY_LICH_HOP', N'DungHan', 0),
        (N'Ứng dụng quản lý giao việc', N'SEED_AI_TRAIN_QUAN_LY_GIAO_VIEC', N'DungHan', 0),
        (N'Hệ thống quản lý nhà cung cấp', N'SEED_AI_TRAIN_QUAN_LY_NHA_CUNG_CAP', N'DungHan', 0),
        (N'Cổng thông tin dự án', N'SEED_AI_TRAIN_CONG_THONG_TIN_DU_AN', N'DungHan', 0),
        (N'Hệ thống theo dõi KPI', N'SEED_AI_TRAIN_THEO_DOI_KPI', N'DungHan', 0),
        (N'Phần mềm quản lý hồ sơ', N'SEED_AI_TRAIN_QUAN_LY_HO_SO', N'DungHan', 0),
        (N'Website thương mại dịch vụ', N'SEED_AI_TRAIN_TM_DICH_VU', N'DungHan', 0),
        (N'Ứng dụng quản lý yêu cầu hỗ trợ', N'SEED_AI_TRAIN_YEU_CAU_HO_TRO', N'DungHan', 0),
        (N'Hệ thống phân tích dữ liệu bán hàng', N'SEED_AI_TRAIN_PHAN_TICH_BAN_HANG', N'DungHan', 0),
        (N'Cổng quản lý khách hàng', N'SEED_AI_TRAIN_QUAN_LY_KHACH_HANG', N'DungHan', 0),
        (N'Hệ thống quản lý thanh toán', N'SEED_AI_TRAIN_QUAN_LY_THANH_TOAN', N'DungHan', 0),
        (N'Ứng dụng quản lý đội nhóm', N'SEED_AI_TRAIN_QUAN_LY_DOI_NHOM', N'DungHan', 0),
        (N'Phần mềm kiểm thử sản phẩm', N'SEED_AI_TRAIN_KIEM_THU_SAN_PHAM', N'DungHan', 0),
        (N'Ứng dụng quản lý vận hành hiện trường', N'SEED_AI_PREDICT_HIEN_TRUONG', N'Predict', 0),
        (N'Cổng phối hợp xử lý yêu cầu nội bộ', N'SEED_AI_PREDICT_PHOI_HOP_NOI_BO', N'Predict', 0),
        (N'Nền tảng giám sát chất lượng dịch vụ', N'SEED_AI_PREDICT_CHAT_LUONG_DICH_VU', N'Predict', 0);

    DECLARE
        @RowNo INT = 1,
        @TotalRows INT = (SELECT COUNT(*) FROM @ProjectPlan),
        @TenDuAn NVARCHAR(255),
        @GhiChuDuAn NVARCHAR(255),
        @NhomDuLieu NVARCHAR(20),
        @VuotNganSachMau BIT,
        @MaManager INT,
        @MaTeam INT,
        @MaLoaiDuAn INT,
        @MaDuAn INT,
        @MaThanhVienA INT,
        @MaThanhVienB INT,
        @MaDanhMuc1 INT,
        @MaDanhMuc2 INT,
        @MaCongViec1 INT,
        @MaCongViec2 INT,
        @MaChiTiet1 INT,
        @MaChiTiet2 INT,
        @MaNganSach INT,
        @TrangThaiDuAn NVARCHAR(50),
        @PhanTramHoanThanh INT,
        @NgayBatDauDuAn DATETIME2(7),
        @NgayKetThucDuAn DATETIME2(7),
        @NgayHoanThanhThucTeDuAn DATETIME2(7),
        @NgayBatDauCv1 DATETIME2(7),
        @NgayKetThucCv1DuKien DATETIME2(7),
        @NgayKetThucCv1ThucTe DATETIME2(7),
        @NgayBatDauCv2 DATETIME2(7),
        @NgayKetThucCv2DuKien DATETIME2(7),
        @NgayKetThucCv2ThucTe DATETIME2(7),
        @TrangThaiCv2 NVARCHAR(50),
        @TrangThaiCt2 NVARCHAR(50),
        @NgayKetThucCt1 DATETIME2(7),
        @NgayKetThucCt2 DATETIME2(7),
        @PhanTramTienDo2 INT,
        @GhiChuTienDo2 NVARCHAR(255),
        @NganSach DECIMAL(18,2),
        @ChiPhi1 DECIMAL(18,2),
        @ChiPhi2 DECIMAL(18,2),
        @TenDanhMuc1 NVARCHAR(255),
        @TenDanhMuc2 NVARCHAR(255),
        @TenCongViec1 NVARCHAR(255),
        @TenCongViec2 NVARCHAR(255),
        @TenChiTiet1 NVARCHAR(255),
        @TenChiTiet2 NVARCHAR(255),
        @SoLanThayDoiNhanSuMau INT,
        @SoLanDoiQuanLyMau INT,
        @LoopCounter INT,
        @MaQuanLyDeXuat INT,
        @NgayYeuCau DATETIME2(7);

    WHILE @RowNo <= @TotalRows
    BEGIN
        SELECT
            @TenDuAn = TenDuAn,
            @GhiChuDuAn = GhiChuDuAn,
            @NhomDuLieu = NhomDuLieu,
            @VuotNganSachMau = VuotNganSachMau
        FROM @ProjectPlan
        WHERE RowNo = @RowNo;

        SELECT @MaManager = MaNguoiDung FROM @ManagerPool WHERE RowNo = ((@RowNo - 1) % @ManagerCount) + 1;
        SELECT @MaTeam = MaTeam FROM @TeamPool WHERE RowNo = ((@RowNo - 1) % @TeamCount) + 1;
        SELECT @MaLoaiDuAn = MaLoaiDuAn FROM @LoaiPool WHERE RowNo = ((@RowNo - 1) % @LoaiCount) + 1;

        SET @TrangThaiDuAn = CASE WHEN @NhomDuLieu = N'Predict' THEN N'DangThucHien' ELSE N'HoanThanh' END;
        SET @PhanTramHoanThanh = CASE WHEN @NhomDuLieu = N'Predict' THEN 45 + (@RowNo % 35) ELSE 100 END;
        SET @NgayBatDauDuAn = DATEADD(DAY, -(150 + @RowNo), CAST(@Today AS DATETIME2(7)));
        SET @NgayKetThucDuAn = CASE
            WHEN @NhomDuLieu = N'Predict' THEN DATEADD(DAY, 20 + (@RowNo % 18), CAST(@Today AS DATETIME2(7)))
            ELSE DATEADD(DAY, -(30 + (@RowNo % 20)), CAST(@Today AS DATETIME2(7)))
        END;

        SET @NgayBatDauCv1 = DATEADD(DAY, 5, @NgayBatDauDuAn);
        SET @NgayKetThucCv1DuKien = DATEADD(DAY, -15, @NgayKetThucDuAn);
        SET @NgayKetThucCv1ThucTe = DATEADD(DAY, -2, @NgayKetThucCv1DuKien);
        SET @NgayKetThucCt1 = DATEADD(DAY, -1, @NgayKetThucCv1ThucTe);

        SET @NgayBatDauCv2 = DATEADD(DAY, 18, @NgayBatDauDuAn);
        SET @NgayKetThucCv2DuKien = CASE
            WHEN @NhomDuLieu = N'Predict' THEN DATEADD(DAY, -2, CAST(@Today AS DATETIME2(7)))
            ELSE DATEADD(DAY, -5, @NgayKetThucDuAn)
        END;

        IF @NhomDuLieu = N'Tre'
        BEGIN
            SET @TrangThaiCv2 = N'HoanThanh';
            SET @NgayKetThucCv2ThucTe = DATEADD(DAY, 3 + (@RowNo % 7), @NgayKetThucCv2DuKien);
            SET @TrangThaiCt2 = N'HoanThanh';
            SET @NgayKetThucCt2 = DATEADD(DAY, 1, @NgayKetThucCv2ThucTe);
            SET @PhanTramTienDo2 = 100;
            SET @GhiChuTienDo2 = N'Đã hoàn thành muộn so với kế hoạch.';
        END
        ELSE IF @NhomDuLieu = N'DungHan'
        BEGIN
            SET @TrangThaiCv2 = N'HoanThanh';
            SET @NgayKetThucCv2ThucTe = DATEADD(DAY, -1 - (@RowNo % 3), @NgayKetThucCv2DuKien);
            SET @TrangThaiCt2 = N'HoanThanh';
            SET @NgayKetThucCt2 = DATEADD(DAY, -1, @NgayKetThucCv2ThucTe);
            SET @PhanTramTienDo2 = 100;
            SET @GhiChuTienDo2 = N'Đã hoàn thành đúng hạn.';
        END
        ELSE
        BEGIN
            SET @TrangThaiCv2 = N'DangThucHien';
            SET @NgayKetThucCv2ThucTe = NULL;
            SET @TrangThaiCt2 = N'DangThucHien';
            SET @NgayKetThucCt2 = NULL;
            SET @PhanTramTienDo2 = 70;
            SET @GhiChuTienDo2 = N'Đang thực hiện theo kế hoạch.';
        END;

        SET @NgayHoanThanhThucTeDuAn = CASE
            WHEN @NhomDuLieu = N'Predict' THEN NULL
            WHEN @NgayKetThucCv1ThucTe >= @NgayKetThucCv2ThucTe THEN @NgayKetThucCv1ThucTe
            ELSE @NgayKetThucCv2ThucTe
        END;

        SET @NganSach = CAST((170000000 + (@RowNo * 2800000)) AS DECIMAL(18,2));
        IF @NhomDuLieu = N'Predict'
        BEGIN
            SET @ChiPhi1 = ROUND(@NganSach * 0.35, 2);
            SET @ChiPhi2 = ROUND(@NganSach * 0.20, 2);
        END
        ELSE IF @NhomDuLieu = N'Tre' AND @VuotNganSachMau = 1
        BEGIN
            SET @ChiPhi1 = ROUND(@NganSach * 0.55, 2);
            SET @ChiPhi2 = ROUND(@NganSach * 0.56, 2);
        END
        ELSE IF @NhomDuLieu = N'Tre'
        BEGIN
            SET @ChiPhi1 = ROUND(@NganSach * 0.50, 2);
            SET @ChiPhi2 = ROUND(@NganSach * 0.42, 2);
        END
        ELSE
        BEGIN
            SET @ChiPhi1 = ROUND(@NganSach * 0.46, 2);
            SET @ChiPhi2 = ROUND(@NganSach * 0.39, 2);
        END;

        SET @SoLanThayDoiNhanSuMau = CASE
            WHEN @NhomDuLieu = N'Tre' THEN 3 + (@RowNo % 4)
            WHEN @NhomDuLieu = N'DungHan' THEN (@RowNo % 3)
            ELSE 1 + (@RowNo % 2)
        END;

        SET @SoLanDoiQuanLyMau = CASE
            WHEN @NhomDuLieu = N'Tre' AND (@RowNo % 5) IN (0, 1) THEN 2
            WHEN @NhomDuLieu = N'Tre' THEN 1
            WHEN @NhomDuLieu = N'DungHan' AND (@RowNo % 6 = 0) THEN 1
            WHEN @NhomDuLieu = N'Predict' AND (@RowNo % 2 = 0) THEN 1
            ELSE 0
        END;
        IF @ManagerCount < 2
            SET @SoLanDoiQuanLyMau = 0;

        SELECT @TenDanhMuc1 = Ten FROM @DanhMucPool WHERE RowNo = ((@RowNo - 1) % @DanhMucCount) + 1;
        SELECT @TenDanhMuc2 = Ten FROM @DanhMucPool WHERE RowNo = ((@RowNo + 2) % @DanhMucCount) + 1;
        IF @TenDanhMuc2 = @TenDanhMuc1
            SELECT @TenDanhMuc2 = Ten FROM @DanhMucPool WHERE RowNo = ((@RowNo + 3) % @DanhMucCount) + 1;

        SELECT @TenCongViec1 = Ten FROM @CongViecPool WHERE RowNo = ((@RowNo - 1) % @CongViecCount) + 1;
        SELECT @TenCongViec2 = Ten FROM @CongViecPool WHERE RowNo = ((@RowNo + 4) % @CongViecCount) + 1;
        IF @TenCongViec2 = @TenCongViec1
            SELECT @TenCongViec2 = Ten FROM @CongViecPool WHERE RowNo = ((@RowNo + 5) % @CongViecCount) + 1;

        SELECT @TenChiTiet1 = Ten FROM @ChiTietPool WHERE RowNo = ((@RowNo - 1) % @ChiTietCount) + 1;
        SELECT @TenChiTiet2 = Ten FROM @ChiTietPool WHERE RowNo = ((@RowNo + 5) % @ChiTietCount) + 1;
        IF @TenChiTiet2 = @TenChiTiet1
            SELECT @TenChiTiet2 = Ten FROM @ChiTietPool WHERE RowNo = ((@RowNo + 6) % @ChiTietCount) + 1;

        PRINT N'Đang thêm dữ liệu dự án: ' + @TenDuAn;

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
                @MaManager, @MaLoaiDuAn, @TenDuAn, N'Dự án seed phục vụ tổng hợp AI_DATASET từ dữ liệu nghiệp vụ.',
                DATEADD(DAY, -@RowNo, @Now), @NgayBatDauDuAn, @NgayKetThucDuAn, @NgayHoanThanhThucTeDuAn,
                @PhanTramHoanThanh, @TrangThaiDuAn, @GhiChuDuAn,
                0, NULL, NULL
            );

            SET @MaDuAn = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaDuAn = MaDuAn
            FROM dbo.DU_AN
            WHERE GhiChuDuAn = @GhiChuDuAn
            ORDER BY MaDuAn DESC;

            UPDATE dbo.DU_AN
            SET TenDuAn = @TenDuAn,
                MoTaDuAn = N'Dự án seed phục vụ tổng hợp AI_DATASET từ dữ liệu nghiệp vụ.',
                MaNguoiDung = @MaManager,
                MaLoaiDuAn = @MaLoaiDuAn,
                NgayBatDauDuAn = @NgayBatDauDuAn,
                NgayKetThucDuAn = @NgayKetThucDuAn,
                NgayHoanThanhThucTeDuAn = @NgayHoanThanhThucTeDuAn,
                PhanTramHoanThanh = @PhanTramHoanThanh,
                TrangThaiDuAn = @TrangThaiDuAn,
                IsDeleted = 0,
                DeletedAt = NULL,
                DeletedBy = NULL
            WHERE MaDuAn = @MaDuAn;
        END;

        SELECT TOP (1) @MaThanhVienA = nvt.MaNguoiDung
        FROM dbo.NHAN_VIEN_TEAM nvt
        WHERE nvt.MaTeam = @MaTeam
          AND nvt.MaNguoiDung <> @MaManager
        ORDER BY ISNULL(nvt.IsLeader, 0) DESC, nvt.MaNguoiDung;

        SELECT TOP (1) @MaThanhVienB = nvt.MaNguoiDung
        FROM dbo.NHAN_VIEN_TEAM nvt
        WHERE nvt.MaTeam = @MaTeam
          AND nvt.MaNguoiDung NOT IN (@MaManager, ISNULL(@MaThanhVienA, -1))
        ORDER BY ISNULL(nvt.IsLeader, 0) DESC, nvt.MaNguoiDung;

        IF @MaThanhVienA IS NULL SET @MaThanhVienA = @MaManager;
        IF @MaThanhVienB IS NULL SET @MaThanhVienB = @MaThanhVienA;

        IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @MaManager)
            INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn) VALUES (@MaDuAn, @MaManager, @Now, N'Leader');
        IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @MaThanhVienA)
            INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn) VALUES (@MaDuAn, @MaThanhVienA, @Now, N'Member');
        IF @MaThanhVienB <> @MaThanhVienA AND NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @MaThanhVienB)
            INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn) VALUES (@MaDuAn, @MaThanhVienB, @Now, N'Member');
        IF NOT EXISTS (SELECT 1 FROM dbo.TEAM_DU_AN WHERE MaTeam = @MaTeam AND MaDuAn = @MaDuAn)
            INSERT INTO dbo.TEAM_DU_AN (MaTeam, MaDuAn, NgayTeamThamGiaDA) VALUES (@MaTeam, @MaDuAn, @Now);

        SET @LoopCounter = 1;
        WHILE @LoopCounter <= @SoLanThayDoiNhanSuMau
        BEGIN
            IF NOT EXISTS (
                SELECT 1
                FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk
                WHERE nk.MaDuAn = @MaDuAn
                  AND nk.NkHanhDongPTDA = CONCAT(
                        CASE (@LoopCounter % 5)
                            WHEN 1 THEN N'Thêm nhân sự dự án'
                            WHEN 2 THEN N'Xóa nhân sự dự án'
                            WHEN 3 THEN N'Thay đổi phụ trách dự án'
                            WHEN 4 THEN N'Điều chuyển nhân sự dự án'
                            ELSE N'Cập nhật vai trò phụ trách dự án'
                        END,
                        N' - ',
                        @GhiChuDuAn,
                        N' #',
                        @LoopCounter
                  )
            )
            BEGIN
                INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
                VALUES
                (
                    CASE WHEN @LoopCounter % 2 = 0 THEN @MaThanhVienA ELSE @MaThanhVienB END,
                    @MaDuAn,
                    CONCAT(
                        CASE (@LoopCounter % 5)
                            WHEN 1 THEN N'Thêm nhân sự dự án'
                            WHEN 2 THEN N'Xóa nhân sự dự án'
                            WHEN 3 THEN N'Thay đổi phụ trách dự án'
                            WHEN 4 THEN N'Điều chuyển nhân sự dự án'
                            ELSE N'Cập nhật vai trò phụ trách dự án'
                        END,
                        N' - ',
                        @GhiChuDuAn,
                        N' #',
                        @LoopCounter
                    ),
                    DATEADD(DAY, -(@RowNo + @LoopCounter), @Now)
                );
            END;

            SET @LoopCounter += 1;
        END;

        SET @LoopCounter = 1;
        WHILE @LoopCounter <= @SoLanDoiQuanLyMau
        BEGIN
            SELECT @MaQuanLyDeXuat = MaNguoiDung
            FROM @ManagerPool
            WHERE RowNo = ((@RowNo + @LoopCounter) % @ManagerCount) + 1;

            IF @MaQuanLyDeXuat = @MaManager
            BEGIN
                SELECT @MaQuanLyDeXuat = MaNguoiDung
                FROM @ManagerPool
                WHERE RowNo = ((@RowNo + @LoopCounter + 1) % @ManagerCount) + 1;
            END;

            SET @NgayYeuCau = DATEADD(DAY, -(@RowNo + @LoopCounter + 2), @Now);
            IF @MaQuanLyDeXuat <> @MaManager
               AND NOT EXISTS (
                SELECT 1
                FROM dbo.YEU_CAU_DOI_QUAN_LY yc
                WHERE yc.MaDuAn = @MaDuAn
                  AND yc.MaQuanLyHienTai = @MaManager
                  AND yc.MaQuanLyDeXuat = @MaQuanLyDeXuat
                  AND yc.TrangThaiYeuCauDoiQuanLy COLLATE Latin1_General_CI_AI IN (N'DaDuyet', N'Đã duyệt')
                  AND yc.NgayTaoYeuCauDoiQuanLy = @NgayYeuCau
                  AND yc.NgayDuyetYeuCauDoiQuanLy IS NOT NULL
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
                    N'DaDuyet', @NgayYeuCau, DATEADD(HOUR, 6, @NgayYeuCau),
                    0, NULL, NULL
                );
            END;

            SET @LoopCounter += 1;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.DANH_MUC_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc1)
        BEGIN
            INSERT INTO dbo.DANH_MUC_CONG_VIEC (MaDuAn, TenDanhMucCV, MoTaDanhMucCV, NgayTaoDMCV, IsDeleted, DeletedAt, DeletedBy)
            VALUES (@MaDuAn, @TenDanhMuc1, N'Danh mục công việc chính của dự án.', @Now, 0, NULL, NULL);
            SET @MaDanhMuc1 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaDanhMuc1 = MaDanhMucCV FROM dbo.DANH_MUC_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc1 ORDER BY MaDanhMucCV DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.DANH_MUC_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc2)
        BEGIN
            INSERT INTO dbo.DANH_MUC_CONG_VIEC (MaDuAn, TenDanhMucCV, MoTaDanhMucCV, NgayTaoDMCV, IsDeleted, DeletedAt, DeletedBy)
            VALUES (@MaDuAn, @TenDanhMuc2, N'Danh mục công việc hỗ trợ của dự án.', @Now, 0, NULL, NULL);
            SET @MaDanhMuc2 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaDanhMuc2 = MaDanhMucCV FROM dbo.DANH_MUC_CONG_VIEC WHERE MaDuAn = @MaDuAn AND TenDanhMucCV = @TenDanhMuc2 ORDER BY MaDanhMucCV DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CONG_VIEC WHERE MaDanhMucCV = @MaDanhMuc1 AND TenCongViec = @TenCongViec1)
        BEGIN
            INSERT INTO dbo.CONG_VIEC
            (
                MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, MoTaCongViec,
                NgayBatDauCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe,
                NgayTaoCongViec, TrangThaiCongViec, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                NULL, @MaDanhMuc1, (SELECT MaMucDo FROM @MucDoPool WHERE RowNo = ((@RowNo - 1) % @MucDoCount) + 1), @TenCongViec1, N'Công việc trọng tâm của dự án.',
                @NgayBatDauCv1, @NgayKetThucCv1DuKien, @NgayKetThucCv1ThucTe,
                @Now, N'HoanThanh', 0, NULL, NULL
            );
            SET @MaCongViec1 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaCongViec1 = MaCongViec FROM dbo.CONG_VIEC WHERE MaDanhMucCV = @MaDanhMuc1 AND TenCongViec = @TenCongViec1 ORDER BY MaCongViec DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CONG_VIEC WHERE MaDanhMucCV = @MaDanhMuc2 AND TenCongViec = @TenCongViec2)
        BEGIN
            INSERT INTO dbo.CONG_VIEC
            (
                MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, MoTaCongViec,
                NgayBatDauCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe,
                NgayTaoCongViec, TrangThaiCongViec, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                NULL, @MaDanhMuc2, (SELECT MaMucDo FROM @MucDoPool WHERE RowNo = ((@RowNo + 1) % @MucDoCount) + 1), @TenCongViec2, N'Công việc thể hiện trạng thái tiến độ cho mô hình AI.',
                @NgayBatDauCv2, @NgayKetThucCv2DuKien, @NgayKetThucCv2ThucTe,
                @Now, @TrangThaiCv2, 0, NULL, NULL
            );
            SET @MaCongViec2 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaCongViec2 = MaCongViec FROM dbo.CONG_VIEC WHERE MaDanhMucCV = @MaDanhMuc2 AND TenCongViec = @TenCongViec2 ORDER BY MaCongViec DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.PHAN_CONG_CONG_VIEC WHERE MaNguoiDung = @MaThanhVienA AND MaCongViec = @MaCongViec1)
            INSERT INTO dbo.PHAN_CONG_CONG_VIEC (MaNguoiDung, MaCongViec, NgayGiaoViec) VALUES (@MaThanhVienA, @MaCongViec1, @Now);
        IF NOT EXISTS (SELECT 1 FROM dbo.PHAN_CONG_CONG_VIEC WHERE MaNguoiDung = @MaThanhVienB AND MaCongViec = @MaCongViec2)
            INSERT INTO dbo.PHAN_CONG_CONG_VIEC (MaNguoiDung, MaCongViec, NgayGiaoViec) VALUES (@MaThanhVienB, @MaCongViec2, @Now);

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
                @MaCongViec1, @TenChiTiet1, N'Chi tiết công việc chính của dự án.', @Now,
                DATEADD(DAY, 2, @NgayBatDauCv1), @NgayKetThucCt1, N'HoanThanh',
                0, NULL, NULL
            );
            SET @MaChiTiet1 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaChiTiet1 = MaChiTietCV FROM dbo.CT_CONG_VIEC WHERE MaCongViec = @MaCongViec1 AND TenCTCV = @TenChiTiet1 ORDER BY MaChiTietCV DESC;
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
                @MaCongViec2, @TenChiTiet2, N'Chi tiết phục vụ theo dõi tiến độ dự án.', @Now,
                DATEADD(DAY, 2, @NgayBatDauCv2), @NgayKetThucCt2, @TrangThaiCt2,
                0, NULL, NULL
            );
            SET @MaChiTiet2 = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaChiTiet2 = MaChiTietCV FROM dbo.CT_CONG_VIEC WHERE MaCongViec = @MaCongViec2 AND TenCTCV = @TenChiTiet2 ORDER BY MaChiTietCV DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.PHAN_CONG_CT_CONG_VIEC WHERE MaNguoiDung = @MaThanhVienA AND MaChiTietCV = @MaChiTiet1)
            INSERT INTO dbo.PHAN_CONG_CT_CONG_VIEC (MaNguoiDung, MaChiTietCV, NgayGiaoCTCV, VaiTroTrongCTCV) VALUES (@MaThanhVienA, @MaChiTiet1, @Now, N'Thực hiện');
        IF NOT EXISTS (SELECT 1 FROM dbo.PHAN_CONG_CT_CONG_VIEC WHERE MaNguoiDung = @MaThanhVienB AND MaChiTietCV = @MaChiTiet2)
            INSERT INTO dbo.PHAN_CONG_CT_CONG_VIEC (MaNguoiDung, MaChiTietCV, NgayGiaoCTCV, VaiTroTrongCTCV) VALUES (@MaThanhVienB, @MaChiTiet2, @Now, N'Thực hiện');

        IF NOT EXISTS (SELECT 1 FROM dbo.TIEN_DO_CONG_VIEC WHERE MaChiTietCV = @MaChiTiet1)
            INSERT INTO dbo.TIEN_DO_CONG_VIEC (MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet, GhiChuDuyet, PhanTram, GhiChuTienDo, ThoiGianCapNhat, TrangThaiCTCVDeXuat, TrangThaiTienDo)
            VALUES (@MaChiTiet1, @MaThanhVienA, @MaManager, DATEADD(HOUR, 1, @Now), N'Báo cáo tiến độ đã duyệt', 100, N'Đã hoàn thành công việc chính', @Now, N'HoanThanh', N'DaDuyet');

        IF NOT EXISTS (SELECT 1 FROM dbo.TIEN_DO_CONG_VIEC WHERE MaChiTietCV = @MaChiTiet2)
            INSERT INTO dbo.TIEN_DO_CONG_VIEC (MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet, GhiChuDuyet, PhanTram, GhiChuTienDo, ThoiGianCapNhat, TrangThaiCTCVDeXuat, TrangThaiTienDo)
            VALUES (@MaChiTiet2, @MaThanhVienB, @MaManager, DATEADD(HOUR, 2, @Now), N'Báo cáo tiến độ đã duyệt', @PhanTramTienDo2, @GhiChuTienDo2, @Now, @TrangThaiCt2, N'DaDuyet');

        IF NOT EXISTS (SELECT 1 FROM dbo.NGAN_SACH WHERE MaDuAn = @MaDuAn AND IsActive = 1 AND ISNULL(IsDeleted, 0) = 0)
        BEGIN
            INSERT INTO dbo.NGAN_SACH
            (
                MaNguoiDungDuyet, MaNguoiDungDeXuat, MaDuAn, NganSach,
                Version, IsActive, MoTaNganSach, NgayCapNhatNganSach, NgayDuyetNganSach,
                TrangThaiNganSach, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaManager, @MaThanhVienA, @MaDuAn, @NganSach,
                1, 1, N'Ngân sách seed phục vụ tổng hợp dữ liệu AI',
                @Now, @Now, N'DaDuyet', 0, NULL, NULL
            );
            SET @MaNganSach = CAST(SCOPE_IDENTITY() AS INT);
        END
        ELSE
        BEGIN
            SELECT TOP (1) @MaNganSach = MaNganSach
            FROM dbo.NGAN_SACH
            WHERE MaDuAn = @MaDuAn AND IsActive = 1 AND ISNULL(IsDeleted, 0) = 0
            ORDER BY NgayCapNhatNganSach DESC, MaNganSach DESC;
        END;

        IF NOT EXISTS (SELECT 1 FROM dbo.CHI_PHI WHERE MaNganSach = @MaNganSach AND MaCongViec = @MaCongViec1 AND NoiDungChiPhi = N'Chi phí nhân sự đợt 1')
            INSERT INTO dbo.CHI_PHI (MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi, NgayChi, TrangThaiChiPhi, IsDeleted, DeletedAt, DeletedBy)
            VALUES (@MaCongViec1, @MaNganSach, N'Chi phí nhân sự đợt 1', @ChiPhi1, DATEADD(DAY, -2, @Now), N'DaDuyet', 0, NULL, NULL);

        IF NOT EXISTS (SELECT 1 FROM dbo.CHI_PHI WHERE MaNganSach = @MaNganSach AND MaCongViec = @MaCongViec2 AND NoiDungChiPhi = N'Chi phí triển khai đợt 2')
            INSERT INTO dbo.CHI_PHI (MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi, NgayChi, TrangThaiChiPhi, IsDeleted, DeletedAt, DeletedBy)
            VALUES (@MaCongViec2, @MaNganSach, N'Chi phí triển khai đợt 2', @ChiPhi2, DATEADD(DAY, -1, @Now), N'DaDuyet', 0, NULL, NULL);

        SET @RowNo += 1;
    END;

    DECLARE @SeededProjects TABLE
    (
        MaDuAn INT PRIMARY KEY,
        NhomDuLieu NVARCHAR(20) NOT NULL
    );

    INSERT INTO @SeededProjects (MaDuAn, NhomDuLieu)
    SELECT da.MaDuAn, pp.NhomDuLieu
    FROM dbo.DU_AN da
    INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn;

    DECLARE @SeedDatasetTag NVARCHAR(200) = N'SEED_AI_DATASET_REASON_ONLY_DVL11';
    DECLARE @SeedReasonTag NVARCHAR(200) = N'SEED_AI_NGUYEN_NHAN_CONFIRMED_DVL11';
    DECLARE @DefaultManagerForConfirm INT = (SELECT TOP (1) mp.MaNguoiDung FROM @ManagerPool mp ORDER BY mp.RowNo);

    DECLARE @RequiredReasons TABLE (TenNguyenNhan NVARCHAR(255) PRIMARY KEY);
    INSERT INTO @RequiredReasons (TenNguyenNhan)
    VALUES
        (N'Thiếu nhân sự'),
        (N'Thay đổi yêu cầu liên tục'),
        (N'Quy trình xử lý chậm'),
        (N'Vượt ngân sách'),
        (N'Rủi ro kỹ thuật'),
        (N'Phối hợp công việc chưa tốt'),
        (N'Thông tin đầu vào chưa đầy đủ'),
        (N'Ước lượng thời gian chưa chính xác'),
        (N'Tiến độ cập nhật không đầy đủ'),
        (N'Khác');

    INSERT INTO dbo.DM_NGUYEN_NHAN (TenNguyenNhan)
    SELECT rr.TenNguyenNhan
    FROM @RequiredReasons rr
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.DM_NGUYEN_NHAN dm
        WHERE dm.TenNguyenNhan COLLATE Latin1_General_CI_AI = rr.TenNguyenNhan COLLATE Latin1_General_CI_AI
    );

    DECLARE @ReasonPool TABLE
    (
        RowNo INT IDENTITY(1,1) PRIMARY KEY,
        MaDMNguyenNhan INT NOT NULL UNIQUE
    );

    INSERT INTO @ReasonPool (MaDMNguyenNhan)
    SELECT dm.MaDMNguyenNhan
    FROM dbo.DM_NGUYEN_NHAN dm
    WHERE EXISTS
    (
        SELECT 1
        FROM @RequiredReasons rr
        WHERE dm.TenNguyenNhan COLLATE Latin1_General_CI_AI = rr.TenNguyenNhan COLLATE Latin1_General_CI_AI
    )
    ORDER BY dm.MaDMNguyenNhan;

    DECLARE @ReasonPoolCount INT = (SELECT COUNT(*) FROM @ReasonPool);
    IF @ReasonPoolCount = 0
        RAISERROR(N'Danh mục nguyên nhân trễ chưa sẵn sàng để seed.', 16, 1);

    DELETE ann
    FROM dbo.AI_NGUYEN_NHAN ann
    INNER JOIN @SeededProjects sp ON sp.MaDuAn = ann.MaDuAn;

    DELETE ds
    FROM dbo.AI_DATASET ds
    INNER JOIN @SeededProjects sp ON sp.MaDuAn = ds.MaDuAn;

    DECLARE @TreProjectReason TABLE
    (
        MaDuAn INT PRIMARY KEY,
        MaDMNguyenNhan INT NOT NULL
    );

    ;WITH TreProjects AS
    (
        SELECT sp.MaDuAn, ROW_NUMBER() OVER (ORDER BY sp.MaDuAn) AS RowNo
        FROM @SeededProjects sp
        WHERE sp.NhomDuLieu = N'Tre'
    )
    INSERT INTO @TreProjectReason (MaDuAn, MaDMNguyenNhan)
    SELECT tp.MaDuAn, rp.MaDMNguyenNhan
    FROM TreProjects tp
    INNER JOIN @ReasonPool rp ON rp.RowNo = ((tp.RowNo - 1) % @ReasonPoolCount) + 1;

    INSERT INTO dbo.AI_NGUYEN_NHAN
    (
        MaDuAn,
        MaDMNguyenNhan,
        DoTinCay,
        NgayXacNhan,
        MaNguoiDungXacNhan,
        GhiChuXacNhan,
        IsDeleted,
        DeletedAt,
        DeletedBy
    )
    SELECT
        tpr.MaDuAn,
        tpr.MaDMNguyenNhan,
        CAST(0.85 + ((ABS(CHECKSUM(tpr.MaDuAn)) % 10) * 0.01) AS FLOAT) AS DoTinCay,
        DATEADD(MINUTE, ABS(CHECKSUM(tpr.MaDuAn)) % 120, @Now) AS NgayXacNhan,
        COALESCE(da.MaNguoiDung, @DefaultManagerForConfirm) AS MaNguoiDungXacNhan,
        CONCAT(@SeedReasonTag, N' - Manager xác nhận nguyên nhân trễ dự án seed.') AS GhiChuXacNhan,
        0,
        NULL,
        NULL
    FROM @TreProjectReason tpr
    INNER JOIN dbo.DU_AN da ON da.MaDuAn = tpr.MaDuAn;

    ;WITH NhanVienAgg AS
    (
        SELECT nvda.MaDuAn, COUNT(DISTINCT nvda.MaNguoiDung) AS SoNhanVienDuAn
        FROM dbo.NHAN_VIEN_DU_AN nvda
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = nvda.MaDuAn
        GROUP BY nvda.MaDuAn
    ),
    CongViecAgg AS
    (
        SELECT
            dmcv.MaDuAn,
            COUNT(cv.MaCongViec) AS TongSoCongViec,
            SUM(
                CASE
                    WHEN cv.NgayKetThucCVDuKien IS NULL THEN 0
                    WHEN cv.TrangThaiCongViec COLLATE Latin1_General_CI_AI IN (N'HoanThanh', N'DaHoanThanh')
                         AND cv.NgayKetThucCVThucTe IS NOT NULL
                         AND CAST(cv.NgayKetThucCVThucTe AS DATE) > CAST(cv.NgayKetThucCVDuKien AS DATE) THEN 1
                    WHEN cv.TrangThaiCongViec COLLATE Latin1_General_CI_AI NOT IN (N'HoanThanh', N'DaHoanThanh')
                         AND @Today > CAST(cv.NgayKetThucCVDuKien AS DATE) THEN 1
                    ELSE 0
                END
            ) AS SoCongViecTre,
            MAX(
                CASE
                    WHEN cv.NgayKetThucCVDuKien IS NULL THEN 0
                    WHEN cv.TrangThaiCongViec COLLATE Latin1_General_CI_AI IN (N'HoanThanh', N'DaHoanThanh')
                         AND cv.NgayKetThucCVThucTe IS NOT NULL
                         THEN CASE
                                WHEN DATEDIFF(DAY, CAST(cv.NgayKetThucCVDuKien AS DATE), CAST(cv.NgayKetThucCVThucTe AS DATE)) > 0
                                    THEN DATEDIFF(DAY, CAST(cv.NgayKetThucCVDuKien AS DATE), CAST(cv.NgayKetThucCVThucTe AS DATE))
                                ELSE 0
                              END
                    ELSE CASE
                            WHEN DATEDIFF(DAY, CAST(cv.NgayKetThucCVDuKien AS DATE), @Today) > 0
                                THEN DATEDIFF(DAY, CAST(cv.NgayKetThucCVDuKien AS DATE), @Today)
                            ELSE 0
                         END
                END
            ) AS SoNgayTreTienDo
        FROM dbo.DANH_MUC_CONG_VIEC dmcv
        LEFT JOIN dbo.CONG_VIEC cv
            ON cv.MaDanhMucCV = dmcv.MaDanhMucCV
           AND ISNULL(cv.IsDeleted, 0) = 0
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = dmcv.MaDuAn
        WHERE ISNULL(dmcv.IsDeleted, 0) = 0
        GROUP BY dmcv.MaDuAn
    ),
    NganSachAgg AS
    (
        SELECT
            ns.MaDuAn,
            SUM(ISNULL(ns.NganSach, 0)) AS ChiPhiDuKien
        FROM dbo.NGAN_SACH ns
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = ns.MaDuAn
        WHERE ISNULL(ns.IsDeleted, 0) = 0
          AND ISNULL(ns.IsActive, 0) = 1
          AND ns.TrangThaiNganSach COLLATE Latin1_General_CI_AI IN (N'DaDuyet', N'Đã duyệt', N'Da duyet')
        GROUP BY ns.MaDuAn
    ),
    ChiPhiAgg AS
    (
        SELECT
            ns.MaDuAn,
            SUM(ISNULL(cp.SoTienDaChi, 0)) AS ChiPhiThucTe
        FROM dbo.CHI_PHI cp
        INNER JOIN dbo.NGAN_SACH ns ON ns.MaNganSach = cp.MaNganSach
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = ns.MaDuAn
        WHERE ISNULL(cp.IsDeleted, 0) = 0
          AND ISNULL(ns.IsDeleted, 0) = 0
        GROUP BY ns.MaDuAn
    ),
    NhanSuChangeAgg AS
    (
        SELECT nk.MaDuAn, COUNT(*) AS SoLanThayDoiNhanSu
        FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = nk.MaDuAn
        WHERE
            nk.NkHanhDongPTDA IS NOT NULL
            AND
            (
                nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%thêm nhân sự%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%thêm nhân viên%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%thêm thành viên%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%xóa nhân sự%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%xóa nhân viên%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%xóa thành viên%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%thay đổi phụ trách%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%đổi phụ trách%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%điều chuyển nhân sự%'
                OR nk.NkHanhDongPTDA COLLATE Latin1_General_CI_AI LIKE N'%cập nhật vai trò phụ trách%'
            )
        GROUP BY nk.MaDuAn
    ),
    QuanLyChangeAgg AS
    (
        SELECT yc.MaDuAn, COUNT(*) AS SoLanThayDoiQuanLy
        FROM dbo.YEU_CAU_DOI_QUAN_LY yc
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = yc.MaDuAn
        WHERE ISNULL(yc.IsDeleted, 0) = 0
          AND yc.NgayDuyetYeuCauDoiQuanLy IS NOT NULL
          AND yc.MaQuanLyHienTai <> yc.MaQuanLyDeXuat
          AND yc.TrangThaiYeuCauDoiQuanLy COLLATE Latin1_General_CI_AI IN (N'DaDuyet', N'Đã duyệt', N'Da duyet')
        GROUP BY yc.MaDuAn
    ),
    DeXuatCongViecAgg AS
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
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = dxcv.MaDuAn
        WHERE ISNULL(dxcv.IsDeleted, 0) = 0
        GROUP BY dxcv.MaDuAn
    ),
    DeXuatNganSachAgg AS
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
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = dxns.MaDuAn
        WHERE ISNULL(dxns.IsDeleted, 0) = 0
        GROUP BY dxns.MaDuAn
    ),
    TienDoBaoCaoAgg AS
    (
        SELECT
            dmcv.MaDuAn,
            SUM(CASE WHEN td.TrangThaiTienDo COLLATE Latin1_General_CI_AI IN (N'ChoDuyet', N'Chờ duyệt', N'Cho duyet') THEN 1 ELSE 0 END) AS SoBaoCaoTienDoChoDuyet,
            SUM(CASE WHEN td.TrangThaiTienDo COLLATE Latin1_General_CI_AI IN (N'TuChoi', N'Từ chối', N'Tu choi') THEN 1 ELSE 0 END) AS SoBaoCaoTienDoBiTuChoi,
            SUM(CASE WHEN td.TrangThaiTienDo COLLATE Latin1_General_CI_AI IN (N'YeuCauBoSung', N'Yêu cầu bổ sung', N'Yeu cau bo sung') THEN 1 ELSE 0 END) AS SoBaoCaoTienDoYeuCauBoSung,
            COUNT(td.MaTienDo) AS SoLanCapNhatTienDo,
            MAX(CAST(td.ThoiGianCapNhat AS DATE)) AS LanCapNhatTienDoGanNhat
        FROM dbo.DANH_MUC_CONG_VIEC dmcv
        INNER JOIN dbo.CONG_VIEC cv
            ON cv.MaDanhMucCV = dmcv.MaDanhMucCV
           AND ISNULL(cv.IsDeleted, 0) = 0
        INNER JOIN dbo.CT_CONG_VIEC ct
            ON ct.MaCongViec = cv.MaCongViec
           AND ISNULL(ct.IsDeleted, 0) = 0
        LEFT JOIN dbo.TIEN_DO_CONG_VIEC td
            ON td.MaChiTietCV = ct.MaChiTietCV
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = dmcv.MaDuAn
        WHERE ISNULL(dmcv.IsDeleted, 0) = 0
        GROUP BY dmcv.MaDuAn
    ),
    TreReasonAgg AS
    (
        SELECT ann.MaDuAn, MAX(ann.MaDMNguyenNhan) AS MaDMNguyenNhan
        FROM dbo.AI_NGUYEN_NHAN ann
        INNER JOIN @SeededProjects sp ON sp.MaDuAn = ann.MaDuAn
        WHERE ISNULL(ann.IsDeleted, 0) = 0
        GROUP BY ann.MaDuAn
    )
    INSERT INTO dbo.AI_DATASET
    (
        MaDuAn,
        SoNhanVienDuAn,
        TongSoCongViec,
        SoCongViecTre,
        TyLeCongViecTre,
        ChiPhiDuKien,
        ChiPhiThucTe,
        ChenhLechChiPhi,
        SoLanThayDoiNhanSu,
        SoLanThayDoiQuanLy,
        SoNgayTreTienDo,
        SoDeXuatCongViecChoDuyet,
        SoDeXuatCongViecBiTuChoi,
        ThoiGianDuyetCongViecTrungBinh,
        SoDeXuatNganSachChoDuyet,
        SoDeXuatNganSachBiTuChoi,
        ThoiGianDuyetNganSachTrungBinh,
        SoBaoCaoTienDoChoDuyet,
        SoBaoCaoTienDoBiTuChoi,
        SoBaoCaoTienDoYeuCauBoSung,
        TyLeBaoCaoTienDoBiTuChoi,
        SoLanCapNhatTienDo,
        SoNgayChamCapNhatTienDo,
        LaDuAnTre,
        MaDMNguyenNhan,
        NgayTongHop,
        GhiChuDataset
    )
    SELECT
        sp.MaDuAn,
        ISNULL(nv.SoNhanVienDuAn, 0) AS SoNhanVienDuAn,
        ISNULL(cv.TongSoCongViec, 0) AS TongSoCongViec,
        ISNULL(cv.SoCongViecTre, 0) AS SoCongViecTre,
        CASE
            WHEN ISNULL(cv.TongSoCongViec, 0) > 0
                THEN ROUND(ISNULL(cv.SoCongViecTre, 0) * 100.0 / cv.TongSoCongViec, 2)
            ELSE 0
        END AS TyLeCongViecTre,
        ISNULL(ns.ChiPhiDuKien, 0) AS ChiPhiDuKien,
        ISNULL(cp.ChiPhiThucTe, 0) AS ChiPhiThucTe,
        ISNULL(cp.ChiPhiThucTe, 0) - ISNULL(ns.ChiPhiDuKien, 0) AS ChenhLechChiPhi,
        ISNULL(nk.SoLanThayDoiNhanSu, 0) AS SoLanThayDoiNhanSu,
        ISNULL(yc.SoLanThayDoiQuanLy, 0) AS SoLanThayDoiQuanLy,
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
            WHEN sp.NhomDuLieu = N'Tre' THEN CAST(1 AS bit)
            WHEN sp.NhomDuLieu = N'DungHan' THEN CAST(0 AS bit)
            ELSE NULL
        END AS LaDuAnTre,
        CASE
            WHEN sp.NhomDuLieu = N'Tre' THEN tr.MaDMNguyenNhan
            ELSE NULL
        END AS MaDMNguyenNhan,
        @Now AS NgayTongHop,
        CONCAT(@SeedDatasetTag, N' - Chỉ phân tích nguyên nhân trễ.') AS GhiChuDataset
    FROM @SeededProjects sp
    LEFT JOIN NhanVienAgg nv ON nv.MaDuAn = sp.MaDuAn
    LEFT JOIN CongViecAgg cv ON cv.MaDuAn = sp.MaDuAn
    LEFT JOIN NganSachAgg ns ON ns.MaDuAn = sp.MaDuAn
    LEFT JOIN ChiPhiAgg cp ON cp.MaDuAn = sp.MaDuAn
    LEFT JOIN NhanSuChangeAgg nk ON nk.MaDuAn = sp.MaDuAn
    LEFT JOIN QuanLyChangeAgg yc ON yc.MaDuAn = sp.MaDuAn
    LEFT JOIN DeXuatCongViecAgg dxcv ON dxcv.MaDuAn = sp.MaDuAn
    LEFT JOIN DeXuatNganSachAgg dxns ON dxns.MaDuAn = sp.MaDuAn
    LEFT JOIN TienDoBaoCaoAgg td ON td.MaDuAn = sp.MaDuAn
    LEFT JOIN dbo.DU_AN da
        ON da.MaDuAn = sp.MaDuAn
       AND ISNULL(da.IsDeleted, 0) = 0
    LEFT JOIN TreReasonAgg tr ON tr.MaDuAn = sp.MaDuAn;

    UPDATE dbo.AI_MODEL
    SET IsActive = 0,
        IsDeleted = 1,
        DeletedAt = @Now,
        DeletedBy = NULL
    WHERE TenModel = N'delay_demo_v2.joblib';

    IF NOT EXISTS (SELECT 1 FROM dbo.AI_MODEL WHERE TenModel = N'reason_demo_v2.joblib')
    BEGIN
        INSERT INTO dbo.AI_MODEL
        (
            TenModel,
            SoLuongDuLieu,
            DoChinhXac,
            NgayTao,
            MoTaModel,
            LoaiModel,
            IsActive,
            IsDeleted,
            DeletedAt,
            DeletedBy
        )
        VALUES
        (
            N'reason_demo_v2.joblib',
            NULL,
            NULL,
            @Now,
            N'Metadata model demo Nguyên nhân. Chỉ seed metadata nên IsActive=0.',
            N'NguyenNhan',
            0,
            0,
            NULL,
            NULL
        );
    END
    ELSE
    BEGIN
        UPDATE dbo.AI_MODEL
        SET LoaiModel = N'NguyenNhan',
            IsActive = 0,
            IsDeleted = 0,
            DeletedAt = NULL,
            DeletedBy = NULL,
            MoTaModel = N'Metadata model demo Nguyên nhân. Chỉ seed metadata nên IsActive=0.'
        WHERE TenModel = N'reason_demo_v2.joblib';
    END;

    PRINT N'Hoàn tất seed-demo-data-simple.sql.';
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'ROLLBACK do lỗi: ' + ERROR_MESSAGE();
    THROW;
END CATCH;

SELECT COUNT(*) AS TongSoDuAnTrongProjectPlan
FROM @ProjectPlan;

SELECT
    NhomDuLieu,
    COUNT(*) AS SoDuAn
FROM @ProjectPlan
GROUP BY NhomDuLieu;

SELECT COUNT(*) AS TongSoDuAnSeedDaTonTai
FROM dbo.DU_AN da
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn;

SELECT COUNT(*) AS TongSoDongAiDatasetSeed
FROM dbo.AI_DATASET ds
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%';

SELECT
    ds.LaDuAnTre,
    COUNT(*) AS SoDong
FROM dbo.AI_DATASET ds
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%'
GROUP BY ds.LaDuAnTre;

SELECT
    ds.MaDMNguyenNhan,
    dm.TenNguyenNhan,
    COUNT(*) AS SoDong
FROM dbo.AI_DATASET ds
LEFT JOIN dbo.DM_NGUYEN_NHAN dm ON dm.MaDMNguyenNhan = ds.MaDMNguyenNhan
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE ds.LaDuAnTre = 1
  AND ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%'
GROUP BY ds.MaDMNguyenNhan, dm.TenNguyenNhan
ORDER BY ds.MaDMNguyenNhan;

SELECT
    COUNT(*) AS SoDongTrainHopLe
FROM dbo.AI_DATASET ds
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
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
  AND ds.SoNgayChamCapNhatTienDo IS NOT NULL
  AND ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%';

SELECT COUNT(*) AS SoDuAnTreThieuNguyenNhan
FROM dbo.AI_DATASET ds
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE ds.LaDuAnTre = 1
  AND ds.MaDMNguyenNhan IS NULL
  AND ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%';

SELECT COUNT(*) AS SoDuAnDungHanSaiRule
FROM dbo.AI_DATASET ds
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE pp.NhomDuLieu = N'DungHan'
  AND (
        ISNULL(ds.SoCongViecTre, 0) > 0
        OR ISNULL(ds.SoNgayTreTienDo, 0) > 0
        OR ds.LaDuAnTre = 1
        OR ds.MaDMNguyenNhan IS NOT NULL
      )
  AND ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%';

SELECT COUNT(*) AS SoDuAnPredictGanNguyenNhan
FROM dbo.AI_DATASET ds
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE pp.NhomDuLieu = N'Predict'
  AND ds.MaDMNguyenNhan IS NOT NULL
  AND ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%';

SELECT COUNT(*) AS SoDongAiNguyenNhanDaXacNhanSeed
FROM dbo.AI_NGUYEN_NHAN ann
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ann.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE ISNULL(ann.IsDeleted, 0) = 0
  AND ann.NgayXacNhan IS NOT NULL
  AND ann.MaNguoiDungXacNhan IS NOT NULL
  AND ann.GhiChuXacNhan LIKE N'SEED_AI_NGUYEN_NHAN_CONFIRMED_DVL11%'
  AND ann.MaDMNguyenNhan IS NOT NULL;

SELECT
    dm.TenNguyenNhan,
    COUNT(*) AS SoDong
FROM dbo.AI_NGUYEN_NHAN ann
JOIN dbo.DM_NGUYEN_NHAN dm ON ann.MaDMNguyenNhan = dm.MaDMNguyenNhan
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ann.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE ISNULL(ann.IsDeleted, 0) = 0
GROUP BY dm.TenNguyenNhan;

SELECT COUNT(*) AS SoModelTreHanConHoatDong
FROM dbo.AI_MODEL
WHERE ISNULL(IsDeleted, 0) = 0
  AND LoaiModel COLLATE Latin1_General_CI_AI = N'TreHan';

SELECT COUNT(*) AS SoModelDelayDemoConHoatDong
FROM dbo.AI_MODEL
WHERE TenModel = N'delay_demo_v2.joblib'
  AND ISNULL(IsDeleted, 0) = 0;

SELECT
    MIN(SoLanThayDoiNhanSu) AS MinSoLanThayDoiNhanSu,
    MAX(SoLanThayDoiNhanSu) AS MaxSoLanThayDoiNhanSu,
    MIN(SoLanThayDoiQuanLy) AS MinSoLanThayDoiQuanLy,
    MAX(SoLanThayDoiQuanLy) AS MaxSoLanThayDoiQuanLy
FROM dbo.AI_DATASET ds
INNER JOIN dbo.DU_AN da ON da.MaDuAn = ds.MaDuAn
INNER JOIN @ProjectPlan pp ON pp.GhiChuDuAn = da.GhiChuDuAn
WHERE ds.GhiChuDataset LIKE N'SEED_AI_DATASET_REASON_ONLY_DVL11%';

SELECT
    CASE
        WHEN COL_LENGTH('dbo.AI_DATASET', 'TongGioLam') IS NULL THEN N'Không còn TongGioLam'
        ELSE N'Còn TongGioLam'
    END AS KiemTraTongGioLam;

SELECT
    CASE
        WHEN COL_LENGTH('dbo.AI_DATASET', 'IsTre') IS NULL THEN N'Không còn IsTre'
        ELSE N'Còn IsTre'
    END AS KiemTraIsTre;


