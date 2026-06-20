SET NOCOUNT ON;
SET XACT_ABORT ON;

/* =========================================================
   1. KIEM TRA DU LIEU NEN
   ========================================================= */

DECLARE @MaxAllowed DATETIME2(0) = '2026-06-22T23:59:59';
DECLARE @BatchPrefix NVARCHAR(20) = N'DATA8-';

IF EXISTS
(
    SELECT 1
    FROM dbo.DU_AN
    WHERE TenDuAn LIKE N'DATA8-%'
)
BEGIN
    THROW 50001, N'Du lieu DATA8 da ton tai. Khong thuc hien chen lai.', 1;
END;

DECLARE
    @Admin INT,
    @ManagerNam INT,
    @ManagerHuong INT,
    @ManagerBao INT,
    @LeadDat INT,
    @LeadMai INT,
    @LeadKhanh INT,
    @DevKiet INT,
    @DevMinhAnh INT,
    @DevHuy INT,
    @DevVy INT,
    @DevLan INT,
    @DevPhuc INT,
    @LoaiPhanMem INT,
    @LoaiBaoTri INT,
    @LoaiAI INT,
    @TeamBackend INT,
    @TeamFrontend INT,
    @TeamKiemThu INT,
    @MucThap INT,
    @MucTrungBinh INT,
    @MucCao INT,
    @MucKhanCap INT;

SELECT @Admin = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'ADMIN@LOCAL.APP' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @ManagerNam = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'HOPHUOCLOI108@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @ManagerHuong = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'HPLOI1082004@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @ManagerBao = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'HPLOI10082004@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @LeadDat = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'HPLOI1008@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @LeadMai = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'NHANVIENQLDA2@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @LeadKhanh = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'LOI224453@STUDENT.NCTU.EDU.VN' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @DevKiet = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'HPLOI531@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @DevMinhAnh = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'BOOKSHARE04@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @DevHuy = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'NHANVIENQLDA1@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @DevVy = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'NHANVIENQLDA3@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @DevLan = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'LUCGIOI1001@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;
SELECT @DevPhuc = nd.MaNguoiDung FROM dbo.NGUOI_DUNG nd JOIN dbo.AspNetUsers au ON au.Id = nd.Id WHERE au.NormalizedEmail = N'HPLCLONE1@GMAIL.COM' AND ISNULL(nd.IsDeleted, 0) = 0;

SELECT @LoaiPhanMem = MaLoaiDuAn FROM dbo.LOAI_DU_AN WHERE TenLoai = N'Phát triển phần mềm';
SELECT @LoaiBaoTri = MaLoaiDuAn FROM dbo.LOAI_DU_AN WHERE TenLoai = N'Bảo trì, nâng cấp';
SELECT @LoaiAI = MaLoaiDuAn FROM dbo.LOAI_DU_AN WHERE TenLoai = N'Nghiên cứu AI';

SELECT @TeamBackend = MaTeam FROM dbo.TEAM WHERE TenTeam = N'Backend và Dữ liệu' AND ISNULL(IsDeleted, 0) = 0;
SELECT @TeamFrontend = MaTeam FROM dbo.TEAM WHERE TenTeam = N'Frontend và UI/UX' AND ISNULL(IsDeleted, 0) = 0;
SELECT @TeamKiemThu = MaTeam FROM dbo.TEAM WHERE TenTeam = N'Team Phân tích và Kiểm thử' AND ISNULL(IsDeleted, 0) = 0;

SELECT @MucThap = MaMucDo FROM dbo.MUC_DO_UU_TIEN WHERE TenMucDo = N'Thấp';
SELECT @MucTrungBinh = MaMucDo FROM dbo.MUC_DO_UU_TIEN WHERE TenMucDo = N'Trung bình';
SELECT @MucCao = MaMucDo FROM dbo.MUC_DO_UU_TIEN WHERE TenMucDo = N'Cao';
SELECT @MucKhanCap = MaMucDo FROM dbo.MUC_DO_UU_TIEN WHERE TenMucDo = N'Khẩn cấp';

IF @Admin IS NULL OR @ManagerNam IS NULL OR @ManagerHuong IS NULL OR @ManagerBao IS NULL
   OR @LeadDat IS NULL OR @LeadMai IS NULL OR @LeadKhanh IS NULL
   OR @DevKiet IS NULL OR @DevMinhAnh IS NULL OR @DevHuy IS NULL OR @DevVy IS NULL OR @DevLan IS NULL OR @DevPhuc IS NULL
   OR @LoaiPhanMem IS NULL OR @LoaiBaoTri IS NULL OR @LoaiAI IS NULL
   OR @TeamBackend IS NULL OR @TeamFrontend IS NULL OR @TeamKiemThu IS NULL
   OR @MucThap IS NULL OR @MucTrungBinh IS NULL OR @MucCao IS NULL OR @MucKhanCap IS NULL
BEGIN
    THROW 50002, N'Thieu du lieu nen bat buoc: nguoi dung, team, loai du an hoac muc do uu tien.', 1;
END;

DECLARE @DuAnMoi TABLE
(
    BatchNo INT PRIMARY KEY,
    MaDuAn INT NOT NULL,
    TenDuAn NVARCHAR(255) NOT NULL,
    TrangThaiDuAn NVARCHAR(50) NOT NULL,
    NgayBatDau DATETIME2(0) NOT NULL,
    NgayKetThuc DATETIME2(0) NOT NULL,
    NgayHoanThanhThucTe DATETIME2(0) NULL
);

DECLARE @DanhMucMoi TABLE
(
    BatchNo INT NOT NULL,
    ThuTu INT NOT NULL,
    MaDanhMucCV INT NOT NULL,
    PRIMARY KEY (BatchNo, ThuTu)
);

DECLARE @CongViecMoi TABLE
(
    BatchNo INT NOT NULL,
    ThuTu INT NOT NULL,
    MaCongViec INT NOT NULL,
    MaDeXuatCV INT NOT NULL,
    TrangThaiCongViec NVARCHAR(50) NOT NULL,
    NgayBatDau DATETIME2(0) NOT NULL,
    NgayKetThucDuKien DATETIME2(0) NOT NULL,
    NgayKetThucThucTe DATETIME2(0) NULL,
    PRIMARY KEY (BatchNo, ThuTu)
);

DECLARE @ChiTietMoi TABLE
(
    BatchNo INT NOT NULL,
    WorkNo INT NOT NULL,
    DetailNo INT NOT NULL,
    MaChiTietCV INT NOT NULL,
    MaNguoiDungPhuTrach INT NOT NULL,
    TrangThaiCTCV NVARCHAR(50) NOT NULL,
    NgayBatDau DATETIME2(0) NOT NULL,
    NgayKetThuc DATETIME2(0) NULL,
    PRIMARY KEY (BatchNo, WorkNo, DetailNo)
);

DECLARE @NganSachMoi TABLE
(
    BatchNo INT PRIMARY KEY,
    MaNganSach INT NOT NULL
);

DECLARE @Member TABLE
(
    BatchNo INT NOT NULL,
    Code NCHAR(1) NOT NULL,
    MaNguoiDung INT NOT NULL,
    Ten NVARCHAR(255) NULL,
    PRIMARY KEY (BatchNo, Code)
);

DECLARE
    @BatchNo INT = 1,
    @TenDuAn NVARCHAR(255),
    @MoTa NVARCHAR(1000),
    @Scenario NVARCHAR(255),
    @MaLoaiDuAn INT,
    @MaQuanLyCu INT,
    @MaQuanLyChinh INT,
    @NgayTao DATETIME2(0),
    @NgayBatDau DATETIME2(0),
    @NgayKetThuc DATETIME2(0),
    @NgayHoanThanh DATETIME2(0),
    @TrangThai NVARCHAR(50),
    @PhanTram INT,
    @Budget DECIMAL(18,2),
    @CostTotal DECIMAL(18,2),
    @CategoryCount INT,
    @WorkCount INT,
    @DetailPerWork INT,
    @ReportBase INT,
    @CompleteWorkCount INT,
    @StaffChanges INT,
    @ManagerChanges INT,
    @DxcvReject INT,
    @DxcvPending INT,
    @DxnsReject INT,
    @DxnsPending INT,
    @TeamMask NVARCHAR(20),
    @MemberMask NVARCHAR(40),
    @LeaderMask NVARCHAR(20),
    @MissingFileEvery INT,
    @MaDuAn INT,
    @MaNganSach INT,
    @QuanLyDeXuatLan1 INT,
    @i INT,
    @MaDanhMucCV INT,
    @MaMucDo INT,
    @MaDeXuatCV INT,
    @MaCongViec INT,
    @NgayDeXuatCV DATETIME2(0),
    @NgayDuyetCV DATETIME2(0),
    @NgayBatDauCV DATETIME2(0),
    @NgayKetThucCVDuKien DATETIME2(0),
    @NgayKetThucCVThucTe DATETIME2(0),
    @TrangThaiCV NVARCHAR(50),
    @ChiPhi DECIMAL(18,2),
    @MaChiPhi INT,
    @d INT,
    @MaChiTietCV INT,
    @TrangThaiCT NVARCHAR(50),
    @NgayBatDauCT DATETIME2(0),
    @NgayKetThucCT DATETIME2(0),
    @Assignee INT,
    @r INT,
    @ReportCount INT,
    @TrangThaiTienDo NVARCHAR(50),
    @TrangThaiCTDeXuat NVARCHAR(50),
    @ThoiGianCapNhat DATETIME2(0),
    @ThoiGianDuyet DATETIME2(0),
    @MaTienDo INT,
    @PhanTramTienDo INT,
    @MaPhongChat INT;

BEGIN TRY
    BEGIN TRANSACTION;

    /* =========================================================
       2. CAU HINH 10 DU AN DATA8
       ========================================================= */

    DECLARE @Cfg TABLE
    (
        BatchNo INT PRIMARY KEY,
        TenDuAn NVARCHAR(255),
        MoTa NVARCHAR(1000),
        Scenario NVARCHAR(255),
        MaLoaiDuAn INT,
        MaQuanLyCu INT,
        MaQuanLyChinh INT,
        NgayTao DATETIME2(0),
        NgayBatDau DATETIME2(0),
        NgayKetThuc DATETIME2(0),
        NgayHoanThanh DATETIME2(0) NULL,
        TrangThai NVARCHAR(50),
        PhanTram INT,
        Budget DECIMAL(18,2),
        CostTotal DECIMAL(18,2),
        CategoryCount INT,
        WorkCount INT,
        DetailPerWork INT,
        ReportBase INT,
        CompleteWorkCount INT,
        StaffChanges INT,
        ManagerChanges INT,
        DxcvReject INT,
        DxcvPending INT,
        DxnsReject INT,
        DxnsPending INT,
        TeamMask NVARCHAR(20),
        MemberMask NVARCHAR(40),
        LeaderMask NVARCHAR(20),
        MissingFileEvery INT
    );

    /* DATA8-01: thieu nhan su, 1 team, 3 nhan vien, 9 cong viec, 18 chi tiet, 2 thay doi nhan su, khong doi quan ly. */
    INSERT INTO @Cfg VALUES
    (1, N'DATA8-01 - Hệ thống điều phối bán hàng đa kênh', N'Triển khai nền tảng điều phối đơn hàng và tồn kho cho chuỗi bán lẻ.', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối.', @LoaiPhanMem, @ManagerNam, @ManagerNam, '2026-06-04T08:00:00', '2026-06-05T08:00:00', '2026-06-12T17:00:00', '2026-06-18T16:30:00', N'HoanThanh', 100, 90000000, 78000000, 4, 9, 2, 2, 9, 2, 0, 1, 0, 0, 0, N'B', N'D,K,P', N'D', 4);

    /* DATA8-02: thay doi yeu cau, 2 team, 6 nhan vien, 10 cong viec, 20 chi tiet, nhieu de xuat bi tu choi va cho duyet. */
    INSERT INTO @Cfg VALUES
    (2, N'DATA8-02 - Sàn thương mại điện tử nông sản địa phương', N'Xây dựng sàn đặt hàng, thanh toán và quản lý gian hàng nông sản.', N'Thay đổi yêu cầu giao diện gian hàng và luồng khuyến mãi liên tục.', @LoaiPhanMem, @ManagerNam, @ManagerNam, '2026-06-03T09:00:00', '2026-06-04T08:00:00', '2026-06-13T17:00:00', NULL, N'DangThucHien', 74, 120000000, 86000000, 5, 10, 2, 3, 6, 3, 0, 4, 2, 1, 1, N'B,F', N'D,M,K,H,V,L', N'D,M', 3);

    /* DATA8-03: quy trinh duyet cham, 1 team, 5 nhan vien, 7 cong viec, 21 chi tiet, de xuat va bao cao xu ly lau. */
    INSERT INTO @Cfg VALUES
    (3, N'DATA8-03 - Cổng xét duyệt học vụ trực tuyến', N'Tự động hóa quy trình tiếp nhận, thẩm định và phản hồi hồ sơ học vụ.', N'Quy trình duyệt đề xuất và báo cáo kéo dài qua nhiều ngày.', @LoaiBaoTri, @ManagerHuong, @ManagerHuong, '2026-06-05T08:30:00', '2026-06-06T08:00:00', '2026-06-15T17:00:00', NULL, N'DangThucHien', 68, 72000000, 64000000, 3, 7, 3, 2, 4, 1, 0, 2, 1, 1, 1, N'K', N'M,K,V,L,P', N'K', 0);

    /* DATA8-04: vuot ngan sach, 2 team, 7 nhan vien, 8 cong viec, 16 chi tiet, hoan thanh tre va chi phi vuot ngan sach. */
    INSERT INTO @Cfg VALUES
    (4, N'DATA8-04 - Nền tảng quản lý lịch khám chuyên khoa', N'Triển khai lịch khám, phòng khám, bác sĩ và nhắc lịch tự động.', N'Tích hợp ngoài phạm vi làm chi phí thực tế vượt ngân sách.', @LoaiPhanMem, @ManagerHuong, @ManagerHuong, '2026-06-02T08:00:00', '2026-06-03T08:00:00', '2026-06-14T17:00:00', '2026-06-22T15:20:00', N'HoanThanh', 100, 105000000, 132000000, 4, 8, 2, 2, 8, 0, 0, 1, 0, 2, 0, N'B,F', N'D,M,K,H,V,L,P', N'D,M', 5);

    /* DATA8-05: rui ro ky thuat, 2 team, 5 nhan vien, 6 cong viec, 24 chi tiet, nhieu chi tiet bi can. */
    INSERT INTO @Cfg VALUES
    (5, N'DATA8-05 - Hệ thống giám sát thiết bị IoT nhà máy', N'Giám sát dữ liệu cảm biến, cảnh báo sự cố và lịch bảo trì thiết bị.', N'Rủi ro kỹ thuật khi kết nối thiết bị thử nghiệm và đồng bộ dữ liệu.', @LoaiPhanMem, @ManagerBao, @ManagerBao, '2026-06-07T08:00:00', '2026-06-08T08:00:00', '2026-06-16T17:00:00', NULL, N'DangThucHien', 62, 98000000, 76000000, 3, 6, 4, 3, 3, 2, 0, 1, 1, 0, 0, N'B,K', N'D,K,H,V,P', N'D,K', 2);

    /* DATA8-06: phoi hop nhieu team, 3 team, 8 nhan vien, 12 cong viec, 24 chi tiet, 1 doi quan ly. */
    INSERT INTO @Cfg VALUES
    (6, N'DATA8-06 - Cổng logistics giao nhận nội bộ', N'Theo dõi điều phối xe, kho trung chuyển, giao nhận và đối soát vận đơn.', N'Phối hợp giữa backend, giao diện và kiểm thử chưa đồng bộ.', @LoaiPhanMem, @ManagerNam, @ManagerBao, '2026-06-01T08:00:00', '2026-06-02T08:00:00', '2026-06-11T17:00:00', '2026-06-20T18:00:00', N'HoanThanh', 100, 150000000, 142000000, 6, 12, 2, 3, 12, 2, 1, 2, 1, 0, 0, N'B,F,K', N'D,M,K,H,V,L,P,A', N'D,M,K', 3);

    /* DATA8-07: thieu thong tin dau vao, 1 team, 4 nhan vien, 5 cong viec, 15 chi tiet, nhieu bao cao yeu cau bo sung. */
    INSERT INTO @Cfg VALUES
    (7, N'DATA8-07 - Kho dữ liệu khách hàng hợp nhất', N'Hợp nhất dữ liệu khách hàng từ CRM, bán hàng và chăm sóc sau bán.', N'Thiếu tài liệu mapping dữ liệu và quy tắc làm sạch đầu vào.', @LoaiBaoTri, @ManagerBao, @ManagerBao, '2026-06-06T09:00:00', '2026-06-07T08:00:00', '2026-06-17T17:00:00', NULL, N'DangThucHien', 59, 83000000, 69000000, 2, 5, 3, 4, 3, 1, 0, 0, 1, 0, 1, N'K', N'K,H,L,P', N'K', 2);

    /* DATA8-08: uoc luong thoi gian chua chinh xac, 2 team, 6 nhan vien, 7 cong viec, 7 chi tiet, hoan thanh tre. */
    INSERT INTO @Cfg VALUES
    (8, N'DATA8-08 - Tự động hóa quy trình bảo trì văn phòng', N'Tự động hóa tiếp nhận yêu cầu sửa chữa, phân công và nghiệm thu nội bộ.', N'Ước lượng ban đầu thấp hơn thời gian xử lý thực tế.', @LoaiBaoTri, @ManagerHuong, @ManagerHuong, '2026-05-31T15:00:00', '2026-06-01T08:00:00', '2026-06-10T17:00:00', '2026-06-21T10:15:00', N'HoanThanh', 100, 62000000, 59000000, 3, 7, 1, 3, 7, 0, 0, 1, 0, 0, 0, N'B,F', N'D,M,H,V,L,P', N'D,M', 0);

    /* DATA8-09: cap nhat tien do khong day du, 2 team, 5 nhan vien, 6 cong viec, 18 chi tiet, dang cho xac nhan hoan thanh. */
    INSERT INTO @Cfg VALUES
    (9, N'DATA8-09 - Dashboard phân tích vận hành dịch vụ', N'Xây dựng dashboard tổng hợp SLA, phản hồi khách hàng và cảnh báo vận hành.', N'Tiến độ cập nhật không đều, nhiều chi tiết cập nhật muộn.', @LoaiPhanMem, @ManagerBao, @ManagerBao, '2026-06-08T08:00:00', '2026-06-09T08:00:00', '2026-06-18T17:00:00', NULL, N'ChoXacNhanHoanThanh', 96, 78000000, 72000000, 3, 6, 3, 1, 6, 1, 0, 0, 1, 0, 0, N'F,K', N'M,K,V,L,P', N'M,K', 2);

    /* DATA8-10: ket hop nhieu yeu to, 3 team, 9 nhan vien, 11 cong viec, 33 chi tiet, 2 doi quan ly. */
    INSERT INTO @Cfg VALUES
    (10, N'DATA8-10 - Hệ thống quản lý hợp đồng dịch vụ khách hàng', N'Quản lý vòng đời hợp đồng, phụ lục, nhắc hạn và chăm sóc khách hàng doanh nghiệp.', N'Kết hợp đổi quản lý, phát sinh yêu cầu và xử lý tiến độ chậm.', @LoaiPhanMem, @ManagerNam, @ManagerBao, '2026-06-01T10:00:00', '2026-06-02T08:00:00', '2026-06-12T17:00:00', NULL, N'DangThucHien', 71, 135000000, 128000000, 5, 11, 3, 2, 7, 4, 2, 3, 1, 1, 0, N'B,F,K', N'D,M,K,H,V,L,P,A,N', N'D,M,K', 4);

    WHILE @BatchNo <= 10
    BEGIN
        SELECT
            @TenDuAn = TenDuAn,
            @MoTa = MoTa,
            @Scenario = Scenario,
            @MaLoaiDuAn = MaLoaiDuAn,
            @MaQuanLyCu = MaQuanLyCu,
            @MaQuanLyChinh = MaQuanLyChinh,
            @NgayTao = NgayTao,
            @NgayBatDau = NgayBatDau,
            @NgayKetThuc = NgayKetThuc,
            @NgayHoanThanh = NgayHoanThanh,
            @TrangThai = TrangThai,
            @PhanTram = PhanTram,
            @Budget = Budget,
            @CostTotal = CostTotal,
            @CategoryCount = CategoryCount,
            @WorkCount = WorkCount,
            @DetailPerWork = DetailPerWork,
            @ReportBase = ReportBase,
            @CompleteWorkCount = CompleteWorkCount,
            @StaffChanges = StaffChanges,
            @ManagerChanges = ManagerChanges,
            @DxcvReject = DxcvReject,
            @DxcvPending = DxcvPending,
            @DxnsReject = DxnsReject,
            @DxnsPending = DxnsPending,
            @TeamMask = TeamMask,
            @MemberMask = MemberMask,
            @LeaderMask = LeaderMask,
            @MissingFileEvery = MissingFileEvery
        FROM @Cfg
        WHERE BatchNo = @BatchNo;

        /* =========================================================
           3. DU AN DATA8 - TAO DU AN, TEAM, NHAN SU VA NHAT KY
           ========================================================= */

        INSERT INTO dbo.DU_AN
        (
            MaNguoiDung, MaLoaiDuAn, TenDuAn, MoTaDuAn, NgayTaoDuAn, NgayBatDauDuAn,
            NgayKetThucDuAn, NgayHoanThanhThucTeDuAn, PhanTramHoanThanh,
            TrangThaiDuAn, GhiChuDuAn, IsDeleted, DeletedAt, DeletedBy
        )
        VALUES
        (
            @MaQuanLyChinh, @MaLoaiDuAn, @TenDuAn, @MoTa, @NgayTao, @NgayBatDau,
            @NgayKetThuc, @NgayHoanThanh, @PhanTram,
            @TrangThai, @Scenario, 0, NULL, NULL
        );

        SET @MaDuAn = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO @DuAnMoi (BatchNo, MaDuAn, TenDuAn, TrangThaiDuAn, NgayBatDau, NgayKetThuc, NgayHoanThanhThucTe)
        VALUES (@BatchNo, @MaDuAn, @TenDuAn, @TrangThai, @NgayBatDau, @NgayKetThuc, @NgayHoanThanh);

        IF CHARINDEX(N'B', @TeamMask) > 0
        BEGIN
            INSERT INTO dbo.TEAM_DU_AN (MaTeam, MaDuAn, NgayTeamThamGiaDA)
            VALUES (@TeamBackend, @MaDuAn, DATEADD(MINUTE, 20, @NgayTao));
            INSERT INTO dbo.NHAT_KY_DU_AN (MaTeam, MaDuAn, TeamCuPhuTrach, TeamMoiPhuTrach, HanhDongNKDA, ThoiGianNKDA)
            VALUES (@TeamBackend, @MaDuAn, NULL, @TeamBackend, N'Gán team Backend và Dữ liệu phụ trách dự án', DATEADD(MINUTE, 25, @NgayTao));
        END;

        IF CHARINDEX(N'F', @TeamMask) > 0
        BEGIN
            INSERT INTO dbo.TEAM_DU_AN (MaTeam, MaDuAn, NgayTeamThamGiaDA)
            VALUES (@TeamFrontend, @MaDuAn, DATEADD(MINUTE, 30, @NgayTao));
            INSERT INTO dbo.NHAT_KY_DU_AN (MaTeam, MaDuAn, TeamCuPhuTrach, TeamMoiPhuTrach, HanhDongNKDA, ThoiGianNKDA)
            VALUES (@TeamFrontend, @MaDuAn, NULL, @TeamFrontend, N'Gán team Frontend và UI/UX phụ trách dự án', DATEADD(MINUTE, 35, @NgayTao));
        END;

        IF CHARINDEX(N'K', @TeamMask) > 0
        BEGIN
            INSERT INTO dbo.TEAM_DU_AN (MaTeam, MaDuAn, NgayTeamThamGiaDA)
            VALUES (@TeamKiemThu, @MaDuAn, DATEADD(MINUTE, 40, @NgayTao));
            INSERT INTO dbo.NHAT_KY_DU_AN (MaTeam, MaDuAn, TeamCuPhuTrach, TeamMoiPhuTrach, HanhDongNKDA, ThoiGianNKDA)
            VALUES (@TeamKiemThu, @MaDuAn, NULL, @TeamKiemThu, N'Gán team Phân tích và Kiểm thử phụ trách dự án', DATEADD(MINUTE, 45, @NgayTao));
        END;

        INSERT INTO @Member
        SELECT @BatchNo, v.Code, v.MaNguoiDung, nd.HoTenNguoiDung
        FROM (VALUES
            (N'D', @LeadDat), (N'M', @LeadMai), (N'K', @LeadKhanh),
            (N'H', @DevKiet), (N'V', @DevVy), (N'L', @DevLan),
            (N'P', @DevPhuc), (N'A', @DevMinhAnh), (N'N', @DevHuy)
        ) AS v(Code, MaNguoiDung)
        JOIN dbo.NGUOI_DUNG nd ON nd.MaNguoiDung = v.MaNguoiDung
        WHERE CHARINDEX(v.Code, @MemberMask) > 0;

        INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
        SELECT
            @MaDuAn,
            MaNguoiDung,
            CASE WHEN CHARINDEX(Code, @LeaderMask) > 0 THEN DATEADD(MINUTE, 55, @NgayTao) ELSE DATEADD(HOUR, 1, @NgayTao) END,
            CASE WHEN CHARINDEX(Code, @LeaderMask) > 0 THEN N'Leader' ELSE N'Member' END
        FROM @Member
        WHERE BatchNo = @BatchNo;

        INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
        SELECT MaNguoiDung, @MaDuAn, N'Gán nhân sự khởi tạo vào dự án', DATEADD(HOUR, 1, @NgayTao)
        FROM @Member
        WHERE BatchNo = @BatchNo;

        INSERT INTO dbo.NHAT_KY_QUAN_LY_DU_AN (MaDuAn, MaNguoiDung, NkHanhDongQLDA, NkThoiGianQLDA, QLDATuNgay, QLDADenNgay)
        VALUES (@MaDuAn, @MaQuanLyChinh, N'Gán quản lý phụ trách dự án DATA8', DATEADD(MINUTE, 10, @NgayTao), @NgayTao, NULL);

        IF @StaffChanges >= 1
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @DevLan)
            BEGIN
                INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
                VALUES (@MaDuAn, @DevLan, DATEADD(HOUR, -1, DATEADD(DAY, 3, @NgayBatDau)), N'Member');
            END;

            INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
            VALUES (@DevLan, @MaDuAn, N'Thêm nhân sự bổ sung xử lý hạng mục trễ', DATEADD(DAY, 3, @NgayBatDau));
        END;

        IF @StaffChanges >= 2
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @DevHuy)
            BEGIN
                INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
                VALUES (@MaDuAn, @DevHuy, DATEADD(HOUR, -1, DATEADD(DAY, 4, @NgayBatDau)), N'Member');
            END;

            INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
            VALUES (@DevHuy, @MaDuAn, N'Thêm nhân sự hỗ trợ do khối lượng công việc tăng', DATEADD(DAY, 4, @NgayBatDau));
        END;

        IF @StaffChanges >= 3
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @DevVy)
            BEGIN
                INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
                VALUES (@MaDuAn, @DevVy, DATEADD(HOUR, -1, DATEADD(DAY, 5, @NgayBatDau)), N'Member');
            END;

            INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
            VALUES (@DevVy, @MaDuAn, N'Điều chuyển nhân sự hỗ trợ kiểm thử tích hợp', DATEADD(DAY, 5, @NgayBatDau));
        END;

        IF @StaffChanges >= 4
        BEGIN
            IF NOT EXISTS (SELECT 1 FROM dbo.NHAN_VIEN_DU_AN WHERE MaDuAn = @MaDuAn AND MaNguoiDung = @DevPhuc)
            BEGIN
                INSERT INTO dbo.NHAN_VIEN_DU_AN (MaDuAn, MaNguoiDung, NgayThamGiaDuAn, VaiTroTrongDuAn)
                VALUES (@MaDuAn, @DevPhuc, DATEADD(HOUR, -1, DATEADD(DAY, 6, @NgayBatDau)), N'Member');
            END;

            INSERT INTO dbo.NHAT_KY_PHU_TRACH_DU_AN (MaNguoiDung, MaDuAn, NkHanhDongPTDA, NkThoiGianPTDA)
            VALUES (@DevPhuc, @MaDuAn, N'Cập nhật vai trò phụ trách do thay đổi phạm vi công việc', DATEADD(DAY, 6, @NgayBatDau));
        END;

        IF @ManagerChanges >= 1
        BEGIN
            SET @QuanLyDeXuatLan1 = CASE WHEN @ManagerChanges >= 2 THEN @ManagerHuong ELSE @MaQuanLyChinh END;

            INSERT INTO dbo.YEU_CAU_DOI_QUAN_LY
            (
                MaQuanLyDeXuat, MaDuAn, MaNguoiDungDuyet, MaQuanLyHienTai,
                TrangThaiYeuCauDoiQuanLy, NgayTaoYeuCauDoiQuanLy, NgayDuyetYeuCauDoiQuanLy,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @QuanLyDeXuatLan1, @MaDuAn, @Admin, @MaQuanLyCu,
                N'DaDuyet', DATEADD(DAY, 2, @NgayBatDau), DATEADD(HOUR, 8, DATEADD(DAY, 2, @NgayBatDau)),
                0, NULL, NULL
            );

            INSERT INTO dbo.NHAT_KY_QUAN_LY_DU_AN (MaDuAn, MaNguoiDung, NkHanhDongQLDA, NkThoiGianQLDA, QLDATuNgay, QLDADenNgay)
            VALUES (@MaDuAn, @QuanLyDeXuatLan1, N'Đổi quản lý dự án đã được duyệt', DATEADD(HOUR, 8, DATEADD(DAY, 2, @NgayBatDau)), DATEADD(HOUR, 8, DATEADD(DAY, 2, @NgayBatDau)), CASE WHEN @ManagerChanges >= 2 THEN DATEADD(HOUR, 10, DATEADD(DAY, 5, @NgayBatDau)) ELSE NULL END);
        END;

        IF @ManagerChanges >= 2
        BEGIN
            INSERT INTO dbo.YEU_CAU_DOI_QUAN_LY
            (
                MaQuanLyDeXuat, MaDuAn, MaNguoiDungDuyet, MaQuanLyHienTai,
                TrangThaiYeuCauDoiQuanLy, NgayTaoYeuCauDoiQuanLy, NgayDuyetYeuCauDoiQuanLy,
                IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaQuanLyChinh, @MaDuAn, @Admin, @ManagerHuong,
                N'DaDuyet', DATEADD(DAY, 5, @NgayBatDau), DATEADD(HOUR, 10, DATEADD(DAY, 5, @NgayBatDau)),
                0, NULL, NULL
            );

            INSERT INTO dbo.NHAT_KY_QUAN_LY_DU_AN (MaDuAn, MaNguoiDung, NkHanhDongQLDA, NkThoiGianQLDA, QLDATuNgay, QLDADenNgay)
            VALUES (@MaDuAn, @MaQuanLyChinh, N'Ổn định quản lý mới sau lần điều phối thứ hai', DATEADD(HOUR, 10, DATEADD(DAY, 5, @NgayBatDau)), DATEADD(HOUR, 10, DATEADD(DAY, 5, @NgayBatDau)), NULL);
        END;

        /* =========================================================
           4. DANH MUC CONG VIEC, NGAN SACH VA DE XUAT NGAN SACH
           ========================================================= */

        SET @i = 1;
        WHILE @i <= @CategoryCount
        BEGIN
            INSERT INTO dbo.DANH_MUC_CONG_VIEC (MaDuAn, TenDanhMucCV, MoTaDanhMucCV, NgayTaoDMCV, IsDeleted, DeletedAt, DeletedBy)
            VALUES
            (
                @MaDuAn,
                CONCAT(N'Hạng mục DATA8-', FORMAT(@BatchNo, '00'), N'-', FORMAT(@i, '00')),
                CONCAT(N'Hạng mục phục vụ kịch bản: ', @Scenario),
                DATEADD(HOUR, 2 + @i, @NgayTao),
                0, NULL, NULL
            );

            INSERT INTO @DanhMucMoi (BatchNo, ThuTu, MaDanhMucCV)
            VALUES (@BatchNo, @i, CONVERT(INT, SCOPE_IDENTITY()));

            SET @i += 1;
        END;

        INSERT INTO dbo.DE_XUAT_NGAN_SACH
        (
            MaDuAn, MaNganSachCu, NganSachCu, NganSachDeXuat, LyDoDeXuat,
            MaNguoiDungDeXuat, MaNguoiDungDuyet, NgayDeXuat, NgayDuyet,
            TrangThaiDeXuat, IsDeleted, DeletedAt, DeletedBy
        )
        VALUES
        (
            @MaDuAn, NULL, NULL, @Budget,
            CONCAT(N'Đề xuất ngân sách triển khai dự án ', @TenDuAn),
            (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY MaNguoiDung),
            @MaQuanLyChinh,
            DATEADD(HOUR, 4, @NgayTao),
            DATEADD(HOUR, CASE WHEN @BatchNo = 3 THEN 52 ELSE 6 END, @NgayTao),
            N'DaDuyet', 0, NULL, NULL
        );

        INSERT INTO dbo.NGAN_SACH
        (
            MaNguoiDungDuyet, MaNguoiDungDeXuat, MaDuAn, [NganSach], Version, IsActive,
            MoTaNganSach, NgayCapNhatNganSach, NgayDuyetNganSach, TrangThaiNganSach,
            IsDeleted, DeletedAt, DeletedBy
        )
        VALUES
        (
            @MaQuanLyChinh,
            (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY MaNguoiDung),
            @MaDuAn, @Budget, 1, 1,
            CONCAT(N'Ngân sách active cho ', @TenDuAn),
            DATEADD(HOUR, CASE WHEN @BatchNo = 3 THEN 52 ELSE 6 END, @NgayTao),
            DATEADD(HOUR, CASE WHEN @BatchNo = 3 THEN 52 ELSE 6 END, @NgayTao),
            N'DaDuyet', 0, NULL, NULL
        );

        SET @MaNganSach = CONVERT(INT, SCOPE_IDENTITY());
        INSERT INTO @NganSachMoi (BatchNo, MaNganSach) VALUES (@BatchNo, @MaNganSach);

        INSERT INTO dbo.NHAT_KY_NGAN_SACH
        (
            MaNganSach, MaDuAn, SoTienNKNS, NganSachTruoc, NganSachSau,
            NkNgayCapNhatNS, NkTrangThaiNganSach, HanhDongNKNS, ThoiGianNKNS
        )
        VALUES
        (
            @MaNganSach, @MaDuAn, @Budget, 0, @Budget,
            DATEADD(HOUR, CASE WHEN @BatchNo = 3 THEN 52 ELSE 6 END, @NgayTao),
            N'DaDuyet', N'Duyệt ngân sách ban đầu cho dự án DATA8',
            DATEADD(HOUR, CASE WHEN @BatchNo = 3 THEN 52 ELSE 6 END, @NgayTao)
        );

        SET @i = 1;
        WHILE @i <= @DxnsReject
        BEGIN
            INSERT INTO dbo.DE_XUAT_NGAN_SACH
            (
                MaDuAn, MaNganSachCu, NganSachCu, NganSachDeXuat, LyDoDeXuat,
                MaNguoiDungDeXuat, MaNguoiDungDuyet, NgayDeXuat, NgayDuyet,
                TrangThaiDeXuat, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaNganSach, @Budget, @Budget + (@i * 15000000),
                N'Đề xuất tăng ngân sách nhưng chưa chứng minh đủ phạm vi phát sinh',
                (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY MaNguoiDung DESC),
                @MaQuanLyChinh,
                DATEADD(DAY, 3 + @i, @NgayBatDau),
                DATEADD(HOUR, 10, DATEADD(DAY, 3 + @i, @NgayBatDau)),
                N'TuChoi', 0, NULL, NULL
            );
            SET @i += 1;
        END;

        SET @i = 1;
        WHILE @i <= @DxnsPending
        BEGIN
            INSERT INTO dbo.DE_XUAT_NGAN_SACH
            (
                MaDuAn, MaNganSachCu, NganSachCu, NganSachDeXuat, LyDoDeXuat,
                MaNguoiDungDeXuat, MaNguoiDungDuyet, NgayDeXuat, NgayDuyet,
                TrangThaiDeXuat, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaNganSach, @Budget, @Budget + 12000000,
                N'Đề xuất bổ sung ngân sách đang chờ quản lý xem xét',
                (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY MaNguoiDung),
                NULL, DATEADD(HOUR, -12, @MaxAllowed), NULL,
                N'ChoDuyet', 0, NULL, NULL
            );
            SET @i += 1;
        END;

        /* =========================================================
           5. DE XUAT CONG VIEC, CONG VIEC, CHI PHI VA CHI TIET
           ========================================================= */

        SET @i = 1;
        WHILE @i <= @WorkCount
        BEGIN
            SELECT @MaDanhMucCV = MaDanhMucCV
            FROM @DanhMucMoi
            WHERE BatchNo = @BatchNo AND ThuTu = ((@i - 1) % @CategoryCount) + 1;

            SET @MaMucDo = CASE
                WHEN @i % 5 = 0 THEN @MucKhanCap
                WHEN @i % 3 = 0 THEN @MucCao
                WHEN @i % 2 = 0 THEN @MucTrungBinh
                ELSE @MucThap
            END;
            SET @NgayDeXuatCV = DATEADD(HOUR, 8 + @i, @NgayBatDau);
            SET @NgayDuyetCV = DATEADD(HOUR, CASE WHEN @BatchNo = 3 THEN 36 + @i ELSE 3 + @i END, @NgayDeXuatCV);
            SET @NgayBatDauCV = DATEADD(HOUR, 1, @NgayDuyetCV);
            SET @NgayKetThucCVDuKien = DATEADD(DAY, 2 + (@i % 4), @NgayBatDauCV);
            SET @NgayKetThucCVThucTe = CASE
                WHEN @i <= @CompleteWorkCount THEN DATEADD(DAY, CASE WHEN @i % 2 = 0 THEN 2 ELSE 1 END, @NgayKetThucCVDuKien)
                ELSE NULL
            END;
            IF @NgayKetThucCVThucTe IS NOT NULL AND @NgayHoanThanh IS NOT NULL AND @NgayKetThucCVThucTe > @NgayHoanThanh
                SET @NgayKetThucCVThucTe = DATEADD(HOUR, -2, @NgayHoanThanh);
            SET @TrangThaiCV = CASE WHEN @i <= @CompleteWorkCount THEN N'HoanThanh' ELSE N'DangThucHien' END;
            SET @ChiPhi = ROUND(@CostTotal / @WorkCount, 0);

            INSERT INTO dbo.DE_XUAT_CONG_VIEC
            (
                MaDuAn, MaDanhMucCV, MaMucDo, MaNguoiDungDeXuat, MaNguoiDungDuyet,
                TenCongViecDeXuat, MoTaCongViecDeXuat, ChiPhiDeXuat,
                NgayBatDauCongViecDeXuat, NgayKetThucCVDeXuatDuKien,
                NgayDeXuatCongViec, NgayDuyetDeXuatCongViec,
                TrangThaiCongViecDeXuat, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaDanhMucCV, @MaMucDo,
                (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY ABS(CHECKSUM(CONCAT(@BatchNo, '-', @i, '-dx-', MaNguoiDung)))),
                @MaQuanLyChinh,
                CONCAT(N'Công việc ', FORMAT(@i, '00'), N' - ', @Scenario),
                CONCAT(N'Thực hiện phần việc số ', @i, N' của dự án ', @TenDuAn),
                @ChiPhi, @NgayBatDauCV, @NgayKetThucCVDuKien,
                @NgayDeXuatCV, @NgayDuyetCV, N'DaDuyet',
                0, NULL, NULL
            );
            SET @MaDeXuatCV = CONVERT(INT, SCOPE_IDENTITY());

            INSERT INTO dbo.CONG_VIEC
            (
                MaDeXuatCV, MaDanhMucCV, MaMucDo, TenCongViec, MoTaCongViec,
                NgayBatDauCongViec, NgayKetThucCVDuKien, NgayKetThucCVThucTe,
                NgayTaoCongViec, TrangThaiCongViec, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDeXuatCV, @MaDanhMucCV, @MaMucDo,
                CONCAT(N'Công việc ', FORMAT(@i, '00'), N' - DATA8-', FORMAT(@BatchNo, '00')),
                CONCAT(N'Công việc sinh từ đề xuất đã duyệt cho kịch bản: ', @Scenario),
                @NgayBatDauCV, @NgayKetThucCVDuKien, @NgayKetThucCVThucTe,
                @NgayDuyetCV, @TrangThaiCV, 0, NULL, NULL
            );
            SET @MaCongViec = CONVERT(INT, SCOPE_IDENTITY());

            INSERT INTO @CongViecMoi
            (
                BatchNo, ThuTu, MaCongViec, MaDeXuatCV, TrangThaiCongViec,
                NgayBatDau, NgayKetThucDuKien, NgayKetThucThucTe
            )
            VALUES
            (
                @BatchNo, @i, @MaCongViec, @MaDeXuatCV, @TrangThaiCV,
                @NgayBatDauCV, @NgayKetThucCVDuKien, @NgayKetThucCVThucTe
            );

            INSERT INTO dbo.CHI_PHI
            (
                MaCongViec, MaNganSach, NoiDungChiPhi, SoTienDaChi, NgayChi,
                TrangThaiChiPhi, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaCongViec, @MaNganSach,
                CONCAT(N'Chi phí thực hiện công việc ', FORMAT(@i, '00'), N' của ', @TenDuAn),
                @ChiPhi, DATEADD(HOUR, 2, @NgayBatDauCV),
                N'DaDuyet', 0, NULL, NULL
            );

            SET @MaChiPhi = CONVERT(INT, SCOPE_IDENTITY());
            INSERT INTO dbo.NHAT_KY_CHI_PHI
            (
                MaCongViec, MaChiPhi, NkSoTienDaChi, NkNgayChi,
                NkTrangThaiChiPhi, HanhDongNKCP, ThoiGianNKCP
            )
            VALUES
            (
                @MaCongViec, @MaChiPhi, @ChiPhi, DATEADD(HOUR, 2, @NgayBatDauCV),
                N'DaDuyet', N'Ghi nhận chi phí theo công việc đã duyệt',
                DATEADD(HOUR, 2, @NgayBatDauCV)
            );

            SET @d = 1;
            WHILE @d <= @DetailPerWork
            BEGIN
                SET @Assignee = (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY ABS(CHECKSUM(CONCAT(@BatchNo, '-', @i, '-', @d, '-', MaNguoiDung))));
                SET @TrangThaiCT = CASE
                    WHEN @TrangThaiCV = N'HoanThanh' THEN N'HoanThanh'
                    WHEN @d = @DetailPerWork AND @BatchNo IN (5, 10) THEN N'BiCanCan'
                    ELSE N'DangThucHien'
                END;
                SET @NgayBatDauCT = DATEADD(HOUR, @d, @NgayBatDauCV);
                SET @NgayKetThucCT = CASE WHEN @TrangThaiCT = N'HoanThanh' THEN ISNULL(@NgayKetThucCVThucTe, DATEADD(HOUR, -1, @NgayKetThucCVDuKien)) ELSE NULL END;

                INSERT INTO dbo.CT_CONG_VIEC
                (
                    MaCongViec, TenCTCV, NoiDungChiTietCV, NgayTaoCTCV,
                    NgayBatDauCTCV, NgayKetThucCTCV, TrangThaiCTCV,
                    IsDeleted, DeletedAt, DeletedBy
                )
                VALUES
                (
                    @MaCongViec,
                    CONCAT(N'Chi tiết ', FORMAT(@d, '00'), N' của công việc ', FORMAT(@i, '00')),
                    CONCAT(N'Xử lý chi tiết nghiệp vụ theo kịch bản: ', @Scenario),
                    @NgayBatDauCT, @NgayBatDauCT, @NgayKetThucCT,
                    @TrangThaiCT, 0, NULL, NULL
                );
                SET @MaChiTietCV = CONVERT(INT, SCOPE_IDENTITY());

                INSERT INTO @ChiTietMoi
                (
                    BatchNo, WorkNo, DetailNo, MaChiTietCV, MaNguoiDungPhuTrach,
                    TrangThaiCTCV, NgayBatDau, NgayKetThuc
                )
                VALUES
                (
                    @BatchNo, @i, @d, @MaChiTietCV, @Assignee,
                    @TrangThaiCT, @NgayBatDauCT, @NgayKetThucCT
                );

                INSERT INTO dbo.PHAN_CONG_CONG_VIEC (MaNguoiDung, MaCongViec, NgayGiaoViec)
                SELECT TOP 1 @Assignee, @MaCongViec, DATEADD(MINUTE, 10, @NgayBatDauCV)
                WHERE NOT EXISTS
                (
                    SELECT 1 FROM dbo.PHAN_CONG_CONG_VIEC
                    WHERE MaNguoiDung = @Assignee AND MaCongViec = @MaCongViec
                );

                INSERT INTO dbo.NHAT_KY_PHAN_CONG_CONG_VIEC
                (MaNguoiDung, MaCongViec, MaNguoiDungGhi, HanhDongPCCV, ThoiGianPCCV)
                VALUES
                (@Assignee, @MaCongViec, @MaQuanLyChinh, N'Thêm phân công công việc', DATEADD(MINUTE, 15, @NgayBatDauCV));

                INSERT INTO dbo.PHAN_CONG_CT_CONG_VIEC (MaNguoiDung, MaChiTietCV, NgayGiaoCTCV, VaiTroTrongCTCV)
                VALUES (@Assignee, @MaChiTietCV, DATEADD(MINUTE, 20, @NgayBatDauCV), N'Thực hiện');

                INSERT INTO dbo.NHAT_KY_PHAN_CONG_CT_CONG_VIEC
                (MaNguoiDung, MaChiTietCV, MaNguoiDungGhi, HanhDongPCCTCV, ThoiGianPCCTCV)
                VALUES
                (@Assignee, @MaChiTietCV, @MaQuanLyChinh, N'Thêm phân công chi tiết công việc', DATEADD(MINUTE, 25, @NgayBatDauCV));

                SET @r = 1;
                SET @ReportCount = @ReportBase + CASE WHEN (@i + @d + @BatchNo) % 3 = 0 THEN 1 ELSE 0 END;
                WHILE @r <= @ReportCount
                BEGIN
                    SET @ThoiGianCapNhat = DATEADD(HOUR, 4 + (@r * 10), DATEADD(DAY, @r - 1, @NgayBatDauCT));
                    IF @ThoiGianCapNhat > @MaxAllowed SET @ThoiGianCapNhat = DATEADD(HOUR, -(@ReportCount - @r + 2), @MaxAllowed);

                    SET @TrangThaiTienDo = CASE
                        WHEN @TrangThaiCT <> N'HoanThanh' AND @r = @ReportCount AND @BatchNo IN (2, 3, 7) THEN N'ChoDuyet'
                        WHEN @TrangThaiCT IN (N'HoanThanh', N'BiCanCan') AND @r = @ReportCount THEN N'DaDuyet'
                        WHEN @r = 2 AND @BatchNo IN (2, 5, 6, 10) THEN N'TuChoi'
                        WHEN @r IN (2, 3) AND @BatchNo IN (3, 5, 7) THEN N'YeuCauBoSung'
                        ELSE N'DaDuyet'
                    END;
                    SET @TrangThaiCTDeXuat = CASE
                        WHEN @r = @ReportCount THEN @TrangThaiCT
                        WHEN @r = 1 THEN N'DangThucHien'
                        ELSE CASE WHEN @TrangThaiCT = N'BiCanCan' THEN N'BiCanCan' ELSE N'DangThucHien' END
                    END;
                    SET @ThoiGianDuyet = CASE WHEN @TrangThaiTienDo = N'ChoDuyet' THEN NULL ELSE DATEADD(HOUR, CASE WHEN @BatchNo = 3 THEN 30 ELSE 2 END, @ThoiGianCapNhat) END;
                    IF @ThoiGianDuyet > @MaxAllowed SET @ThoiGianDuyet = DATEADD(MINUTE, -30, @MaxAllowed);
                    SET @PhanTramTienDo = CASE WHEN @TrangThaiCTDeXuat = N'HoanThanh' THEN 100 ELSE CASE WHEN 20 + (@r * 20) > 95 THEN 95 ELSE 20 + (@r * 20) END END;

                    INSERT INTO dbo.TIEN_DO_CONG_VIEC
                    (
                        MaChiTietCV, MaNguoiDung, MaNguoiDungDuyet, ThoiGianDuyet,
                        GhiChuDuyet, PhanTram, GhiChuTienDo, ThoiGianCapNhat,
                        TrangThaiCTCVDeXuat, TrangThaiTienDo
                    )
                    VALUES
                    (
                        @MaChiTietCV, @Assignee,
                        CASE WHEN @TrangThaiTienDo = N'ChoDuyet' THEN NULL ELSE @MaQuanLyChinh END,
                        @ThoiGianDuyet,
                        CASE
                            WHEN @TrangThaiTienDo = N'DaDuyet' THEN N'Đã duyệt báo cáo tiến độ'
                            WHEN @TrangThaiTienDo = N'TuChoi' THEN N'Từ chối do nội dung chưa khớp minh chứng'
                            WHEN @TrangThaiTienDo = N'YeuCauBoSung' THEN N'Yêu cầu bổ sung thêm tài liệu và kết quả kiểm thử'
                            ELSE NULL
                        END,
                        @PhanTramTienDo,
                        CONCAT(N'Cập nhật tiến độ lần ', @r, N' cho chi tiết công việc trong dự án ', @TenDuAn),
                        @ThoiGianCapNhat, @TrangThaiCTDeXuat, @TrangThaiTienDo
                    );
                    SET @MaTienDo = CONVERT(INT, SCOPE_IDENTITY());

                    IF @MissingFileEvery = 0 OR (@MaTienDo % @MissingFileEvery <> 0)
                    BEGIN
                        INSERT INTO dbo.FILE_TIEN_DO_CONG_VIEC
                        (MaTienDo, TenFileTDCV, DuongDanFileTDCV, NgayUploadFileTDCV, IsDeleted, DeletedAt, DeletedBy)
                        VALUES
                        (
                            @MaTienDo,
                            CONCAT(N'minh-chung-data8-', FORMAT(@BatchNo, '00'), N'-', FORMAT(@i, '00'), N'-', FORMAT(@d, '00'), N'-', FORMAT(@r, '00'), N'.pdf'),
                            CONCAT(N'/uploads/data8/', FORMAT(@BatchNo, '00'), N'/bc-', @MaTienDo, N'.pdf'),
                            DATEADD(MINUTE, 10, @ThoiGianCapNhat),
                            0, NULL, NULL
                        );
                    END;

                    SET @r += 1;
                END;

                SET @d += 1;
            END;

            SET @i += 1;
        END;

        SET @i = 1;
        WHILE @i <= @DxcvReject
        BEGIN
            SELECT @MaDanhMucCV = MaDanhMucCV
            FROM @DanhMucMoi
            WHERE BatchNo = @BatchNo AND ThuTu = ((@i - 1) % @CategoryCount) + 1;

            INSERT INTO dbo.DE_XUAT_CONG_VIEC
            (
                MaDuAn, MaDanhMucCV, MaMucDo, MaNguoiDungDeXuat, MaNguoiDungDuyet,
                TenCongViecDeXuat, MoTaCongViecDeXuat, ChiPhiDeXuat,
                NgayBatDauCongViecDeXuat, NgayKetThucCVDeXuatDuKien,
                NgayDeXuatCongViec, NgayDuyetDeXuatCongViec,
                TrangThaiCongViecDeXuat, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaDanhMucCV, @MucTrungBinh,
                (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY MaNguoiDung DESC),
                @MaQuanLyChinh,
                CONCAT(N'Đề xuất phát sinh bị từ chối ', @i, N' - DATA8-', FORMAT(@BatchNo, '00')),
                N'Đề xuất chưa đủ căn cứ phạm vi hoặc thời gian nên không tạo công việc thật',
                5000000 + (@i * 2000000),
                DATEADD(DAY, 2 + @i, @NgayBatDau),
                DATEADD(DAY, 4 + @i, @NgayBatDau),
                DATEADD(HOUR, 9, DATEADD(DAY, 2 + @i, @NgayBatDau)),
                DATEADD(HOUR, 16, DATEADD(DAY, 2 + @i, @NgayBatDau)),
                N'TuChoi', 0, NULL, NULL
            );
            SET @i += 1;
        END;

        SET @i = 1;
        WHILE @i <= @DxcvPending
        BEGIN
            SELECT @MaDanhMucCV = MaDanhMucCV
            FROM @DanhMucMoi
            WHERE BatchNo = @BatchNo AND ThuTu = ((@i - 1) % @CategoryCount) + 1;

            INSERT INTO dbo.DE_XUAT_CONG_VIEC
            (
                MaDuAn, MaDanhMucCV, MaMucDo, MaNguoiDungDeXuat, MaNguoiDungDuyet,
                TenCongViecDeXuat, MoTaCongViecDeXuat, ChiPhiDeXuat,
                NgayBatDauCongViecDeXuat, NgayKetThucCVDeXuatDuKien,
                NgayDeXuatCongViec, NgayDuyetDeXuatCongViec,
                TrangThaiCongViecDeXuat, IsDeleted, DeletedAt, DeletedBy
            )
            VALUES
            (
                @MaDuAn, @MaDanhMucCV, @MucCao,
                (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY MaNguoiDung),
                NULL,
                CONCAT(N'Đề xuất công việc đang chờ duyệt ', @i, N' - DATA8-', FORMAT(@BatchNo, '00')),
                N'Đề xuất bổ sung đang chờ quản lý xem xét, chưa tạo công việc thật',
                6500000 + (@i * 1500000),
                DATEADD(HOUR, -8, @MaxAllowed),
                @MaxAllowed,
                DATEADD(HOUR, -10, @MaxAllowed),
                NULL,
                N'ChoDuyet', 0, NULL, NULL
            );
            SET @i += 1;
        END;

        /* =========================================================
           6. PHONG CHAT VA THANH VIEN PHONG CHAT
           ========================================================= */

        INSERT INTO dbo.PHONG_CHAT (MaDuAn, TenPhong, IsDeleted, DeletedAt, DeletedBy)
        VALUES (@MaDuAn, CONCAT(N'Phòng trao đổi ', @TenDuAn), 0, NULL, NULL);

        SET @MaPhongChat = CONVERT(INT, SCOPE_IDENTITY());

        INSERT INTO dbo.THANH_VIEN_PHONG_CHAT (MaPhongChat, MaNguoiDung, VaiTroTrongPhongChat)
        VALUES (@MaPhongChat, @MaQuanLyChinh, N'QuanLy');

        INSERT INTO dbo.THANH_VIEN_PHONG_CHAT (MaPhongChat, MaNguoiDung, VaiTroTrongPhongChat)
        SELECT
            @MaPhongChat,
            MaNguoiDung,
            CASE WHEN CHARINDEX(Code, @LeaderMask) > 0 THEN N'Leader' ELSE N'ThanhVien' END
        FROM @Member
        WHERE BatchNo = @BatchNo;

        SET @BatchNo += 1;
    END;

    /* =========================================================
       7. KIEM TRA TRUOC COMMIT
       ========================================================= */

    DECLARE @LoiKiemTra TABLE
    (
        STT INT IDENTITY(1,1),
        MaLoi NVARCHAR(50),
        NhomLoi NVARCHAR(150),
        DuAn NVARCHAR(300),
        BangDuLieu NVARCHAR(150),
        KhoaBanGhi NVARCHAR(300),
        NoiDung NVARCHAR(1000)
    );

    IF (SELECT COUNT(*) FROM @DuAnMoi) <> 10
        INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
        VALUES
        (
            N'DATA8_COUNT',
            N'Số lượng dự án',
            N'DATA8',
            N'@DuAnMoi',
            N'COUNT',
            CONCAT(N'Số dự án DATA8 vừa tạo = ', (SELECT COUNT(*) FROM @DuAnMoi), N', mong đợi = 10.')
        );

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'DXNS_TIME',
        N'Đề xuất ngân sách',
        da.TenDuAn,
        N'DE_XUAT_NGAN_SACH',
        CONCAT(N'MaDeXuatNS=', dx.MaDeXuatNS),
        CONCAT(
            N'TrangThai=', ISNULL(dx.TrangThaiDeXuat, N'NULL'),
            N'; NgayDeXuat=', COALESCE(CONVERT(NVARCHAR(30), dx.NgayDeXuat, 126), N'NULL'),
            N'; NgayDuyet=', COALESCE(CONVERT(NVARCHAR(30), dx.NgayDuyet, 126), N'NULL'),
            N'; MaNguoiDungDuyet=', COALESCE(CONVERT(NVARCHAR(30), dx.MaNguoiDungDuyet), N'NULL'),
            N'. Quy tắc: ChoDuyet không có ngày/người duyệt; DaDuyet/TuChoi phải có ngày duyệt sau ngày đề xuất.'
        )
    FROM dbo.DE_XUAT_NGAN_SACH dx
    JOIN @DuAnMoi da ON da.MaDuAn = dx.MaDuAn
    WHERE (dx.TrangThaiDeXuat = N'ChoDuyet' AND (dx.NgayDuyet IS NOT NULL OR dx.MaNguoiDungDuyet IS NOT NULL))
       OR (dx.TrangThaiDeXuat IN (N'DaDuyet', N'TuChoi') AND (dx.NgayDuyet IS NULL OR dx.NgayDuyet <= dx.NgayDeXuat));

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'DXCV_TIME',
        N'Đề xuất công việc',
        da.TenDuAn,
        N'DE_XUAT_CONG_VIEC',
        CONCAT(N'MaDeXuatCV=', dx.MaDeXuatCV),
        CONCAT(
            N'TrangThai=', ISNULL(dx.TrangThaiCongViecDeXuat, N'NULL'),
            N'; NgayDeXuat=', COALESCE(CONVERT(NVARCHAR(30), dx.NgayDeXuatCongViec, 126), N'NULL'),
            N'; NgayDuyet=', COALESCE(CONVERT(NVARCHAR(30), dx.NgayDuyetDeXuatCongViec, 126), N'NULL'),
            N'; MaNguoiDungDuyet=', COALESCE(CONVERT(NVARCHAR(30), dx.MaNguoiDungDuyet), N'NULL'),
            N'. Quy tắc: ChoDuyet không có ngày/người duyệt; DaDuyet/TuChoi phải có ngày duyệt sau ngày đề xuất.'
        )
    FROM dbo.DE_XUAT_CONG_VIEC dx
    JOIN @DuAnMoi da ON da.MaDuAn = dx.MaDuAn
    WHERE (dx.TrangThaiCongViecDeXuat = N'ChoDuyet' AND (dx.NgayDuyetDeXuatCongViec IS NOT NULL OR dx.MaNguoiDungDuyet IS NOT NULL))
       OR (dx.TrangThaiCongViecDeXuat IN (N'DaDuyet', N'TuChoi') AND (dx.NgayDuyetDeXuatCongViec IS NULL OR dx.NgayDuyetDeXuatCongViec <= dx.NgayDeXuatCongViec));

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CV_BEFORE_APPROVAL',
        N'Công việc từ đề xuất',
        da.TenDuAn,
        N'CONG_VIEC',
        CONCAT(N'MaCongViec=', cv.MaCongViec, N'; MaDeXuatCV=', cv.MaDeXuatCV),
        CONCAT(
            N'NgayTaoCongViec=', COALESCE(CONVERT(NVARCHAR(30), cv.NgayTaoCongViec, 126), N'NULL'),
            N'; NgayDuyetDeXuatCongViec=', COALESCE(CONVERT(NVARCHAR(30), dx.NgayDuyetDeXuatCongViec, 126), N'NULL'),
            N'. Công việc không được tạo trước khi đề xuất được duyệt.'
        )
    FROM dbo.CONG_VIEC cv
    JOIN @CongViecMoi m ON m.MaCongViec = cv.MaCongViec
    JOIN @DuAnMoi da ON da.BatchNo = m.BatchNo
    JOIN dbo.DE_XUAT_CONG_VIEC dx ON dx.MaDeXuatCV = cv.MaDeXuatCV
    WHERE cv.NgayTaoCongViec < dx.NgayDuyetDeXuatCongViec;

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'TD_TIME',
        N'Báo cáo tiến độ',
        da.TenDuAn,
        N'TIEN_DO_CONG_VIEC',
        CONCAT(N'MaTienDo=', td.MaTienDo, N'; MaChiTietCV=', td.MaChiTietCV),
        CONCAT(
            N'TrangThai=', ISNULL(td.TrangThaiTienDo, N'NULL'),
            N'; ThoiGianCapNhat=', COALESCE(CONVERT(NVARCHAR(30), td.ThoiGianCapNhat, 126), N'NULL'),
            N'; ThoiGianDuyet=', COALESCE(CONVERT(NVARCHAR(30), td.ThoiGianDuyet, 126), N'NULL'),
            N'; MaNguoiDungDuyet=', COALESCE(CONVERT(NVARCHAR(30), td.MaNguoiDungDuyet), N'NULL'),
            N'. Quy tắc: ChoDuyet không có duyệt; các trạng thái đã xử lý phải duyệt sau cập nhật.'
        )
    FROM dbo.TIEN_DO_CONG_VIEC td
    JOIN @ChiTietMoi ct ON ct.MaChiTietCV = td.MaChiTietCV
    JOIN @DuAnMoi da ON da.BatchNo = ct.BatchNo
    WHERE (td.TrangThaiTienDo = N'ChoDuyet' AND (td.ThoiGianDuyet IS NOT NULL OR td.MaNguoiDungDuyet IS NOT NULL))
       OR (td.TrangThaiTienDo IN (N'DaDuyet', N'TuChoi', N'YeuCauBoSung') AND (td.ThoiGianDuyet IS NULL OR td.ThoiGianDuyet <= td.ThoiGianCapNhat));

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CT_STATUS_LATEST_APPROVED',
        N'Đồng bộ trạng thái chi tiết',
        da.TenDuAn,
        N'CT_CONG_VIEC/TIEN_DO_CONG_VIEC',
        CONCAT(N'MaChiTietCV=', ct.MaChiTietCV),
        CONCAT(
            N'TrangThaiCTCV hiện tại=', ISNULL(ct.TrangThaiCTCV, N'NULL'),
            N'; trạng thái từ báo cáo DaDuyet mới nhất=', ISNULL(tdDuyetCuoi.TrangThaiCTCVDeXuat, N'NULL'),
            N'; MaTienDoDaDuyetCuoi=', COALESCE(CONVERT(NVARCHAR(30), tdDuyetCuoi.MaTienDo), N'NULL'),
            N'. Chỉ báo cáo DaDuyet mới được xem là nguồn cập nhật trạng thái thật.'
        )
    FROM @ChiTietMoi ct
    JOIN @DuAnMoi da ON da.BatchNo = ct.BatchNo
    OUTER APPLY
    (
        SELECT TOP 1 td.MaTienDo, td.TrangThaiCTCVDeXuat
        FROM dbo.TIEN_DO_CONG_VIEC td
        WHERE td.MaChiTietCV = ct.MaChiTietCV
          AND td.TrangThaiTienDo = N'DaDuyet'
        ORDER BY td.ThoiGianDuyet DESC, td.MaTienDo DESC
    ) tdDuyetCuoi
    WHERE ISNULL(tdDuyetCuoi.TrangThaiCTCVDeXuat, N'DangThucHien') <> ct.TrangThaiCTCV;

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'PCCT_TIME',
        N'Phân công chi tiết',
        da.TenDuAn,
        N'PHAN_CONG_CT_CONG_VIEC',
        CONCAT(N'MaNguoiDung=', pc.MaNguoiDung, N'; MaChiTietCV=', pc.MaChiTietCV),
        CONCAT(
            N'NgayGiaoCTCV=', COALESCE(CONVERT(NVARCHAR(30), pc.NgayGiaoCTCV, 126), N'NULL'),
            N'; NgayBatDauCTCV=', COALESCE(CONVERT(NVARCHAR(30), ct.NgayBatDau, 126), N'NULL'),
            N'. Phân công phải trước hoặc đúng thời điểm bắt đầu chi tiết.'
        )
    FROM @ChiTietMoi ct
    JOIN @DuAnMoi da ON da.BatchNo = ct.BatchNo
    JOIN dbo.PHAN_CONG_CT_CONG_VIEC pc ON pc.MaChiTietCV = ct.MaChiTietCV AND pc.MaNguoiDung = ct.MaNguoiDungPhuTrach
    WHERE pc.NgayGiaoCTCV > ct.NgayBatDau;

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'PCCT_MEMBER_SCOPE',
        N'Phân công chi tiết',
        da.TenDuAn,
        N'PHAN_CONG_CT_CONG_VIEC/NHAN_VIEN_DU_AN',
        CONCAT(N'MaNguoiDung=', pc.MaNguoiDung, N'; MaChiTietCV=', pc.MaChiTietCV),
        N'Người được phân công chi tiết không thuộc NHAN_VIEN_DU_AN của đúng dự án DATA8.'
    FROM dbo.PHAN_CONG_CT_CONG_VIEC pc
    JOIN @ChiTietMoi ct ON ct.MaChiTietCV = pc.MaChiTietCV
    JOIN @DuAnMoi da ON da.BatchNo = ct.BatchNo
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.NHAN_VIEN_DU_AN nv
        WHERE nv.MaDuAn = da.MaDuAn AND nv.MaNguoiDung = pc.MaNguoiDung
    );

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CV_DONE_WITH_OPEN_CT',
        N'Workflow công việc',
        da.TenDuAn,
        N'CONG_VIEC/CT_CONG_VIEC',
        CONCAT(N'MaCongViec=', cv.MaCongViec),
        N'Công việc HoanThanh còn ít nhất một chi tiết chưa HoanThanh.'
    FROM dbo.CONG_VIEC cv
    JOIN @CongViecMoi m ON m.MaCongViec = cv.MaCongViec
    JOIN @DuAnMoi da ON da.BatchNo = m.BatchNo
    WHERE cv.TrangThaiCongViec = N'HoanThanh'
      AND EXISTS
      (
          SELECT 1 FROM dbo.CT_CONG_VIEC ct
          WHERE ct.MaCongViec = cv.MaCongViec AND ct.TrangThaiCTCV <> N'HoanThanh'
      );

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'DA_DONE_WITH_OPEN_CV',
        N'Workflow dự án',
        da.TenDuAn,
        N'DU_AN/CONG_VIEC',
        CONCAT(N'MaDuAn=', da.MaDuAn),
        N'Dự án HoanThanh còn ít nhất một công việc chưa HoanThanh.'
    FROM @DuAnMoi da
    WHERE da.TrangThaiDuAn = N'HoanThanh'
      AND EXISTS
      (
          SELECT 1
          FROM @CongViecMoi cv
          WHERE cv.BatchNo = da.BatchNo AND cv.TrangThaiCongViec <> N'HoanThanh'
      );

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CHI_PHI_BUDGET_SCOPE',
        N'Chi phí và ngân sách',
        da.TenDuAn,
        N'CHI_PHI/NGAN_SACH/CONG_VIEC',
        CONCAT(N'MaChiPhi=', cp.MaChiPhi, N'; MaCongViec=', cp.MaCongViec, N'; MaNganSach=', cp.MaNganSach),
        CONCAT(N'Batch công việc=', cvm.BatchNo, N'; batch ngân sách=', nsm.BatchNo, N'. Chi phí phải dùng ngân sách cùng dự án.')
    FROM dbo.CHI_PHI cp
    JOIN @CongViecMoi cvm ON cvm.MaCongViec = cp.MaCongViec
    JOIN @NganSachMoi nsm ON nsm.MaNganSach = cp.MaNganSach
    JOIN @DuAnMoi da ON da.BatchNo = cvm.BatchNo
    WHERE nsm.BatchNo <> cvm.BatchNo;

    DECLARE @MaxInsertedDate DATETIME2(0);

    SELECT @MaxInsertedDate = MAX(v.Ngay)
    FROM
    (
        SELECT d.NgayTaoDuAn AS Ngay FROM dbo.DU_AN d JOIN @DuAnMoi da ON da.MaDuAn = d.MaDuAn
        UNION ALL SELECT d.NgayBatDauDuAn FROM dbo.DU_AN d JOIN @DuAnMoi da ON da.MaDuAn = d.MaDuAn
        UNION ALL SELECT d.NgayKetThucDuAn FROM dbo.DU_AN d JOIN @DuAnMoi da ON da.MaDuAn = d.MaDuAn
        UNION ALL SELECT d.NgayHoanThanhThucTeDuAn FROM dbo.DU_AN d JOIN @DuAnMoi da ON da.MaDuAn = d.MaDuAn
        UNION ALL SELECT NgayTaoDMCV FROM dbo.DANH_MUC_CONG_VIEC dm JOIN @DuAnMoi da ON da.MaDuAn = dm.MaDuAn
        UNION ALL SELECT NgayDeXuat FROM dbo.DE_XUAT_NGAN_SACH dx JOIN @DuAnMoi da ON da.MaDuAn = dx.MaDuAn
        UNION ALL SELECT NgayDuyet FROM dbo.DE_XUAT_NGAN_SACH dx JOIN @DuAnMoi da ON da.MaDuAn = dx.MaDuAn
        UNION ALL SELECT NgayCapNhatNganSach FROM dbo.NGAN_SACH ns JOIN @DuAnMoi da ON da.MaDuAn = ns.MaDuAn
        UNION ALL SELECT NgayDuyetNganSach FROM dbo.NGAN_SACH ns JOIN @DuAnMoi da ON da.MaDuAn = ns.MaDuAn
        UNION ALL SELECT NgayDeXuatCongViec FROM dbo.DE_XUAT_CONG_VIEC dx JOIN @DuAnMoi da ON da.MaDuAn = dx.MaDuAn
        UNION ALL SELECT NgayDuyetDeXuatCongViec FROM dbo.DE_XUAT_CONG_VIEC dx JOIN @DuAnMoi da ON da.MaDuAn = dx.MaDuAn
        UNION ALL SELECT NgayBatDauCongViec FROM dbo.CONG_VIEC cv JOIN @CongViecMoi m ON m.MaCongViec = cv.MaCongViec
        UNION ALL SELECT NgayKetThucCVDuKien FROM dbo.CONG_VIEC cv JOIN @CongViecMoi m ON m.MaCongViec = cv.MaCongViec
        UNION ALL SELECT NgayKetThucCVThucTe FROM dbo.CONG_VIEC cv JOIN @CongViecMoi m ON m.MaCongViec = cv.MaCongViec
        UNION ALL SELECT NgayTaoCongViec FROM dbo.CONG_VIEC cv JOIN @CongViecMoi m ON m.MaCongViec = cv.MaCongViec
        UNION ALL SELECT NgayChi FROM dbo.CHI_PHI cp JOIN @CongViecMoi m ON m.MaCongViec = cp.MaCongViec
        UNION ALL SELECT NgayTaoCTCV FROM dbo.CT_CONG_VIEC ct JOIN @ChiTietMoi m ON m.MaChiTietCV = ct.MaChiTietCV
        UNION ALL SELECT NgayBatDauCTCV FROM dbo.CT_CONG_VIEC ct JOIN @ChiTietMoi m ON m.MaChiTietCV = ct.MaChiTietCV
        UNION ALL SELECT NgayKetThucCTCV FROM dbo.CT_CONG_VIEC ct JOIN @ChiTietMoi m ON m.MaChiTietCV = ct.MaChiTietCV
        UNION ALL SELECT ThoiGianCapNhat FROM dbo.TIEN_DO_CONG_VIEC td JOIN @ChiTietMoi m ON m.MaChiTietCV = td.MaChiTietCV
        UNION ALL SELECT ThoiGianDuyet FROM dbo.TIEN_DO_CONG_VIEC td JOIN @ChiTietMoi m ON m.MaChiTietCV = td.MaChiTietCV
        UNION ALL SELECT NgayUploadFileTDCV FROM dbo.FILE_TIEN_DO_CONG_VIEC f JOIN dbo.TIEN_DO_CONG_VIEC td ON td.MaTienDo = f.MaTienDo JOIN @ChiTietMoi m ON m.MaChiTietCV = td.MaChiTietCV
        UNION ALL SELECT NgayTaoYeuCauDoiQuanLy FROM dbo.YEU_CAU_DOI_QUAN_LY yc JOIN @DuAnMoi da ON da.MaDuAn = yc.MaDuAn
        UNION ALL SELECT NgayDuyetYeuCauDoiQuanLy FROM dbo.YEU_CAU_DOI_QUAN_LY yc JOIN @DuAnMoi da ON da.MaDuAn = yc.MaDuAn
        UNION ALL SELECT NkThoiGianQLDA FROM dbo.NHAT_KY_QUAN_LY_DU_AN nk JOIN @DuAnMoi da ON da.MaDuAn = nk.MaDuAn
        UNION ALL SELECT NkThoiGianPTDA FROM dbo.NHAT_KY_PHU_TRACH_DU_AN nk JOIN @DuAnMoi da ON da.MaDuAn = nk.MaDuAn
    ) v
    WHERE v.Ngay IS NOT NULL;

    IF @MaxInsertedDate > @MaxAllowed
        INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
        VALUES
        (
            N'MAX_DATE',
            N'Giới hạn thời gian',
            N'DATA8',
            N'Nhiều bảng',
            N'MaxInsertedDate',
            CONCAT(N'Có ngày vượt giới hạn 2026-06-22T23:59:59: ', CONVERT(NVARCHAR(30), @MaxInsertedDate, 126))
        );

    IF EXISTS (SELECT 1 FROM @LoiKiemTra)
    BEGIN
        SELECT
            STT,
            MaLoi,
            NhomLoi,
            DuAn,
            BangDuLieu,
            KhoaBanGhi,
            NoiDung
        FROM @LoiKiemTra
        ORDER BY STT;

        THROW 50003, N'Phát hiện lỗi logic trong batch DATA8. Xem danh sách lỗi phía trên.', 1;
    END;

    COMMIT TRANSACTION;

    /* =========================================================
       8. TRUY VAN KIEM TRA SAU COMMIT
       ========================================================= */

    SELECT COUNT(*) AS SoDuAnMoi
    FROM dbo.DU_AN
    WHERE TenDuAn LIKE N'DATA8-%';

    SELECT @MaxInsertedDate AS NgayLonNhatTrongBatch, @MaxAllowed AS GioiHanNgay;

    SELECT da.MaDuAn, da.TenDuAn, da.TrangThaiDuAn, da.NgayBatDauDuAn, da.NgayKetThucDuAn, da.NgayHoanThanhThucTeDuAn
    FROM dbo.DU_AN da
    WHERE da.TenDuAn LIKE N'DATA8-%'
    ORDER BY da.TenDuAn;

    SELECT dx.MaDuAn, dx.TrangThaiCongViecDeXuat, COUNT(*) AS SoLuong
    FROM dbo.DE_XUAT_CONG_VIEC dx
    JOIN dbo.DU_AN da ON da.MaDuAn = dx.MaDuAn
    WHERE da.TenDuAn LIKE N'DATA8-%'
    GROUP BY dx.MaDuAn, dx.TrangThaiCongViecDeXuat
    ORDER BY dx.MaDuAn, dx.TrangThaiCongViecDeXuat;

    SELECT dx.MaDuAn, dx.TrangThaiDeXuat, COUNT(*) AS SoLuong
    FROM dbo.DE_XUAT_NGAN_SACH dx
    JOIN dbo.DU_AN da ON da.MaDuAn = dx.MaDuAn
    WHERE da.TenDuAn LIKE N'DATA8-%'
    GROUP BY dx.MaDuAn, dx.TrangThaiDeXuat
    ORDER BY dx.MaDuAn, dx.TrangThaiDeXuat;

    SELECT da.MaDuAn, td.TrangThaiTienDo, COUNT(*) AS SoLuong
    FROM dbo.DU_AN da
    JOIN dbo.DANH_MUC_CONG_VIEC dm ON dm.MaDuAn = da.MaDuAn
    JOIN dbo.CONG_VIEC cv ON cv.MaDanhMucCV = dm.MaDanhMucCV
    JOIN dbo.CT_CONG_VIEC ct ON ct.MaCongViec = cv.MaCongViec
    JOIN dbo.TIEN_DO_CONG_VIEC td ON td.MaChiTietCV = ct.MaChiTietCV
    WHERE da.TenDuAn LIKE N'DATA8-%'
    GROUP BY da.MaDuAn, td.TrangThaiTienDo
    ORDER BY da.MaDuAn, td.TrangThaiTienDo;

    SELECT N'Không có lệnh ghi dữ liệu vào nhóm bảng phân tích AI trong script DATA8.' AS XacNhanBangAI;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    SELECT
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_LINE() AS ErrorLine,
        ERROR_MESSAGE() AS ErrorMessage;

    THROW;
END CATCH;

/* =========================================================
   CANH BAO: NHOM BANG AI KHONG THUOC PHAM VI SCRIPT DATA8:
   AI_DATASET, AI_KET_QUA, AI_NGUYEN_NHAN, AI_MODEL, DM_NGUYEN_NHAN.
   ========================================================= */
