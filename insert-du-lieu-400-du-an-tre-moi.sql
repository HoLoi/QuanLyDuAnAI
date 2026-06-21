SET NOCOUNT ON;
SET XACT_ABORT ON;

/* =========================================================
   1. KIEM TRA DU LIEU NEN
   ========================================================= */

DECLARE @MaxAllowed DATETIME2(0) = '2026-06-22T23:59:59';
DECLARE @BatchPrefix NVARCHAR(20) = N'DATA10-';

IF EXISTS
(
    SELECT 1
    FROM dbo.DU_AN
    WHERE TenDuAn LIKE N'DATA10-%'
)
BEGIN
    THROW 50001,
        N'Dữ liệu DATA10 đã tồn tại hoặc đã được tạo một phần. Không thực hiện chèn lại.',
        1;
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
    THROW 50002, N'Thiếu dữ liệu nền bắt buộc: người dùng, team, loại dự án hoặc mức độ ưu tiên.', 1;
END;

DECLARE @SoDuAnTruoc INT =
(
    SELECT COUNT(*)
    FROM dbo.DU_AN
    WHERE ISNULL(IsDeleted, 0) = 0
);

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
    @MinDetailPerWork INT,
    @MaxDetailPerWork INT,
    @DetailCount INT,
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
    @TongChiPhiDaPhanBo DECIMAL(18,2),
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
    @MaPhongChat INT,
    @TenHangMuc NVARCHAR(255),
    @TenCongViecNghiepVu NVARCHAR(255),
    @TenChiTietNghiepVu NVARCHAR(255);

BEGIN TRY
    BEGIN TRANSACTION;

    /* =========================================================
       2. CAU HINH 10 DU AN DATA10
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
        MinDetailPerWork INT,
        MaxDetailPerWork INT,
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

    DECLARE @Seed TABLE
    (
        BatchNo INT PRIMARY KEY,
        TenNghiepVu NVARCHAR(220) NOT NULL,
        Scenario NVARCHAR(255) NOT NULL
    );

    INSERT INTO @Seed (BatchNo, TenNghiepVu, Scenario)
    VALUES
        (1, N'hệ thống quản lý kho linh kiện điện tử - giai đoạn chuẩn hóa quy trình', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (2, N'nền tảng điều phối giao nhận khu vực - giai đoạn chuẩn hóa quy trình', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (3, N'cổng quản lý hồ sơ khám ngoại trú - giai đoạn chuẩn hóa quy trình', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (4, N'kho dữ liệu phân tích bán hàng - giai đoạn chuẩn hóa quy trình', N'Phối hợp nhiều team chưa đồng bộ'),
        (5, N'hệ thống đối soát tài chính nội bộ - giai đoạn chuẩn hóa quy trình', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (6, N'cổng thương mại điện tử ngành hàng tiêu dùng - giai đoạn chuẩn hóa quy trình', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (7, N'ứng dụng theo dõi đơn hàng đa kênh - giai đoạn chuẩn hóa quy trình', N'Ước lượng thời gian thấp hơn thực tế'),
        (8, N'bảng điều phối vận tải tuyến ngắn - giai đoạn chuẩn hóa quy trình', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (9, N'nền tảng đào tạo nội bộ cho nhân viên - giai đoạn chuẩn hóa quy trình', N'Hạ tầng thử nghiệm không ổn định'),
        (10, N'cổng tuyển dụng và phỏng vấn trực tuyến - giai đoạn chuẩn hóa quy trình', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (11, N'hệ thống quản lý tài sản cố định - giai đoạn chuẩn hóa quy trình', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (12, N'kho hồ sơ hợp đồng và pháp lý số - giai đoạn chuẩn hóa quy trình', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (13, N'bảng kế hoạch sản xuất theo lệnh - giai đoạn chuẩn hóa quy trình', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (14, N'hệ thống kiểm soát chất lượng đầu vào - giai đoạn chuẩn hóa quy trình', N'Phối hợp nhiều team chưa đồng bộ'),
        (15, N'ứng dụng bảo trì thiết bị nhà xưởng - giai đoạn chuẩn hóa quy trình', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (16, N'trung tâm chăm sóc khách hàng đa kênh - giai đoạn chuẩn hóa quy trình', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (17, N'cổng quản lý dịch vụ sau bán - giai đoạn chuẩn hóa quy trình', N'Ước lượng thời gian thấp hơn thực tế'),
        (18, N'hệ thống đánh giá nhà cung cấp - giai đoạn chuẩn hóa quy trình', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (19, N'quy trình phê duyệt mua hàng tự động - giai đoạn chuẩn hóa quy trình', N'Hạ tầng thử nghiệm không ổn định'),
        (20, N'bảng quản lý ngân sách phòng ban - giai đoạn chuẩn hóa quy trình', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (21, N'cổng đặt lịch hẹn kỹ thuật viên - giai đoạn chuẩn hóa quy trình', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (22, N'hệ thống quản lý vận đơn logistics - giai đoạn chuẩn hóa quy trình', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (23, N'nền tảng quản lý lớp học hybrid - giai đoạn chuẩn hóa quy trình', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (24, N'hồ sơ năng lực nhân sự số - giai đoạn chuẩn hóa quy trình', N'Phối hợp nhiều team chưa đồng bộ'),
        (25, N'cổng thông tin điều hành chi nhánh - giai đoạn chuẩn hóa quy trình', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (26, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - giai đoạn chuẩn hóa quy trình', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (27, N'trung tâm xử lý sự cố công nghệ thông tin - giai đoạn chuẩn hóa quy trình', N'Ước lượng thời gian thấp hơn thực tế'),
        (28, N'kho báo cáo vận hành đa phòng ban - giai đoạn chuẩn hóa quy trình', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (29, N'mô hình AI phân loại phản hồi dịch vụ - giai đoạn chuẩn hóa quy trình', N'Hạ tầng thử nghiệm không ổn định'),
        (30, N'hệ thống dự báo nhu cầu tồn kho - giai đoạn chuẩn hóa quy trình', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (31, N'quy trình tự động hóa hoàn ứng - giai đoạn chuẩn hóa quy trình', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (32, N'dự án nâng cấp hệ thống kế toán cũ - giai đoạn chuẩn hóa quy trình', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (33, N'luồng tích hợp dữ liệu ERP về kho - giai đoạn chuẩn hóa quy trình', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (34, N'nền tảng chuyển đổi dữ liệu khách hàng - giai đoạn chuẩn hóa quy trình', N'Phối hợp nhiều team chưa đồng bộ'),
        (35, N'cổng quản lý khuyến mãi thương mại - giai đoạn chuẩn hóa quy trình', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (36, N'ứng dụng kiểm kê kho lạnh - giai đoạn chuẩn hóa quy trình', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (37, N'hệ thống theo dõi xe giao hàng - giai đoạn chuẩn hóa quy trình', N'Ước lượng thời gian thấp hơn thực tế'),
        (38, N'quản lý lịch trực bác sĩ phòng khám - giai đoạn chuẩn hóa quy trình', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (39, N'cổng học liệu số hóa cho đào tạo - giai đoạn chuẩn hóa quy trình', N'Hạ tầng thử nghiệm không ổn định'),
        (40, N'hệ thống đo lường trải nghiệm khách hàng - giai đoạn chuẩn hóa quy trình', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (41, N'hệ thống quản lý kho linh kiện điện tử - phiên bản mở rộng liên phòng ban', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (42, N'nền tảng điều phối giao nhận khu vực - phiên bản mở rộng liên phòng ban', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (43, N'cổng quản lý hồ sơ khám ngoại trú - phiên bản mở rộng liên phòng ban', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (44, N'kho dữ liệu phân tích bán hàng - phiên bản mở rộng liên phòng ban', N'Phối hợp nhiều team chưa đồng bộ'),
        (45, N'hệ thống đối soát tài chính nội bộ - phiên bản mở rộng liên phòng ban', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (46, N'cổng thương mại điện tử ngành hàng tiêu dùng - phiên bản mở rộng liên phòng ban', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (47, N'ứng dụng theo dõi đơn hàng đa kênh - phiên bản mở rộng liên phòng ban', N'Ước lượng thời gian thấp hơn thực tế'),
        (48, N'bảng điều phối vận tải tuyến ngắn - phiên bản mở rộng liên phòng ban', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (49, N'nền tảng đào tạo nội bộ cho nhân viên - phiên bản mở rộng liên phòng ban', N'Hạ tầng thử nghiệm không ổn định'),
        (50, N'cổng tuyển dụng và phỏng vấn trực tuyến - phiên bản mở rộng liên phòng ban', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (51, N'hệ thống quản lý tài sản cố định - phiên bản mở rộng liên phòng ban', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (52, N'kho hồ sơ hợp đồng và pháp lý số - phiên bản mở rộng liên phòng ban', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (53, N'bảng kế hoạch sản xuất theo lệnh - phiên bản mở rộng liên phòng ban', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (54, N'hệ thống kiểm soát chất lượng đầu vào - phiên bản mở rộng liên phòng ban', N'Phối hợp nhiều team chưa đồng bộ'),
        (55, N'ứng dụng bảo trì thiết bị nhà xưởng - phiên bản mở rộng liên phòng ban', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (56, N'trung tâm chăm sóc khách hàng đa kênh - phiên bản mở rộng liên phòng ban', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (57, N'cổng quản lý dịch vụ sau bán - phiên bản mở rộng liên phòng ban', N'Ước lượng thời gian thấp hơn thực tế'),
        (58, N'hệ thống đánh giá nhà cung cấp - phiên bản mở rộng liên phòng ban', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (59, N'quy trình phê duyệt mua hàng tự động - phiên bản mở rộng liên phòng ban', N'Hạ tầng thử nghiệm không ổn định'),
        (60, N'bảng quản lý ngân sách phòng ban - phiên bản mở rộng liên phòng ban', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (61, N'cổng đặt lịch hẹn kỹ thuật viên - phiên bản mở rộng liên phòng ban', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (62, N'hệ thống quản lý vận đơn logistics - phiên bản mở rộng liên phòng ban', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (63, N'nền tảng quản lý lớp học hybrid - phiên bản mở rộng liên phòng ban', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (64, N'hồ sơ năng lực nhân sự số - phiên bản mở rộng liên phòng ban', N'Phối hợp nhiều team chưa đồng bộ'),
        (65, N'cổng thông tin điều hành chi nhánh - phiên bản mở rộng liên phòng ban', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (66, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - phiên bản mở rộng liên phòng ban', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (67, N'trung tâm xử lý sự cố công nghệ thông tin - phiên bản mở rộng liên phòng ban', N'Ước lượng thời gian thấp hơn thực tế'),
        (68, N'kho báo cáo vận hành đa phòng ban - phiên bản mở rộng liên phòng ban', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (69, N'mô hình AI phân loại phản hồi dịch vụ - phiên bản mở rộng liên phòng ban', N'Hạ tầng thử nghiệm không ổn định'),
        (70, N'hệ thống dự báo nhu cầu tồn kho - phiên bản mở rộng liên phòng ban', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (71, N'quy trình tự động hóa hoàn ứng - phiên bản mở rộng liên phòng ban', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (72, N'dự án nâng cấp hệ thống kế toán cũ - phiên bản mở rộng liên phòng ban', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (73, N'luồng tích hợp dữ liệu ERP về kho - phiên bản mở rộng liên phòng ban', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (74, N'nền tảng chuyển đổi dữ liệu khách hàng - phiên bản mở rộng liên phòng ban', N'Phối hợp nhiều team chưa đồng bộ'),
        (75, N'cổng quản lý khuyến mãi thương mại - phiên bản mở rộng liên phòng ban', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (76, N'ứng dụng kiểm kê kho lạnh - phiên bản mở rộng liên phòng ban', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (77, N'hệ thống theo dõi xe giao hàng - phiên bản mở rộng liên phòng ban', N'Ước lượng thời gian thấp hơn thực tế'),
        (78, N'quản lý lịch trực bác sĩ phòng khám - phiên bản mở rộng liên phòng ban', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (79, N'cổng học liệu số hóa cho đào tạo - phiên bản mở rộng liên phòng ban', N'Hạ tầng thử nghiệm không ổn định'),
        (80, N'hệ thống đo lường trải nghiệm khách hàng - phiên bản mở rộng liên phòng ban', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (81, N'hệ thống quản lý kho linh kiện điện tử - cấu phần tích hợp dữ liệu', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (82, N'nền tảng điều phối giao nhận khu vực - cấu phần tích hợp dữ liệu', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (83, N'cổng quản lý hồ sơ khám ngoại trú - cấu phần tích hợp dữ liệu', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (84, N'kho dữ liệu phân tích bán hàng - cấu phần tích hợp dữ liệu', N'Phối hợp nhiều team chưa đồng bộ'),
        (85, N'hệ thống đối soát tài chính nội bộ - cấu phần tích hợp dữ liệu', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (86, N'cổng thương mại điện tử ngành hàng tiêu dùng - cấu phần tích hợp dữ liệu', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (87, N'ứng dụng theo dõi đơn hàng đa kênh - cấu phần tích hợp dữ liệu', N'Ước lượng thời gian thấp hơn thực tế'),
        (88, N'bảng điều phối vận tải tuyến ngắn - cấu phần tích hợp dữ liệu', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (89, N'nền tảng đào tạo nội bộ cho nhân viên - cấu phần tích hợp dữ liệu', N'Hạ tầng thử nghiệm không ổn định'),
        (90, N'cổng tuyển dụng và phỏng vấn trực tuyến - cấu phần tích hợp dữ liệu', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (91, N'hệ thống quản lý tài sản cố định - cấu phần tích hợp dữ liệu', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (92, N'kho hồ sơ hợp đồng và pháp lý số - cấu phần tích hợp dữ liệu', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (93, N'bảng kế hoạch sản xuất theo lệnh - cấu phần tích hợp dữ liệu', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (94, N'hệ thống kiểm soát chất lượng đầu vào - cấu phần tích hợp dữ liệu', N'Phối hợp nhiều team chưa đồng bộ'),
        (95, N'ứng dụng bảo trì thiết bị nhà xưởng - cấu phần tích hợp dữ liệu', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (96, N'trung tâm chăm sóc khách hàng đa kênh - cấu phần tích hợp dữ liệu', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (97, N'cổng quản lý dịch vụ sau bán - cấu phần tích hợp dữ liệu', N'Ước lượng thời gian thấp hơn thực tế'),
        (98, N'hệ thống đánh giá nhà cung cấp - cấu phần tích hợp dữ liệu', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (99, N'quy trình phê duyệt mua hàng tự động - cấu phần tích hợp dữ liệu', N'Hạ tầng thử nghiệm không ổn định'),
        (100, N'bảng quản lý ngân sách phòng ban - cấu phần tích hợp dữ liệu', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (101, N'cổng đặt lịch hẹn kỹ thuật viên - cấu phần tích hợp dữ liệu', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (102, N'hệ thống quản lý vận đơn logistics - cấu phần tích hợp dữ liệu', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (103, N'nền tảng quản lý lớp học hybrid - cấu phần tích hợp dữ liệu', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (104, N'hồ sơ năng lực nhân sự số - cấu phần tích hợp dữ liệu', N'Phối hợp nhiều team chưa đồng bộ'),
        (105, N'cổng thông tin điều hành chi nhánh - cấu phần tích hợp dữ liệu', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (106, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - cấu phần tích hợp dữ liệu', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (107, N'trung tâm xử lý sự cố công nghệ thông tin - cấu phần tích hợp dữ liệu', N'Ước lượng thời gian thấp hơn thực tế'),
        (108, N'kho báo cáo vận hành đa phòng ban - cấu phần tích hợp dữ liệu', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (109, N'mô hình AI phân loại phản hồi dịch vụ - cấu phần tích hợp dữ liệu', N'Hạ tầng thử nghiệm không ổn định'),
        (110, N'hệ thống dự báo nhu cầu tồn kho - cấu phần tích hợp dữ liệu', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (111, N'quy trình tự động hóa hoàn ứng - cấu phần tích hợp dữ liệu', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (112, N'dự án nâng cấp hệ thống kế toán cũ - cấu phần tích hợp dữ liệu', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (113, N'luồng tích hợp dữ liệu ERP về kho - cấu phần tích hợp dữ liệu', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (114, N'nền tảng chuyển đổi dữ liệu khách hàng - cấu phần tích hợp dữ liệu', N'Phối hợp nhiều team chưa đồng bộ'),
        (115, N'cổng quản lý khuyến mãi thương mại - cấu phần tích hợp dữ liệu', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (116, N'ứng dụng kiểm kê kho lạnh - cấu phần tích hợp dữ liệu', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (117, N'hệ thống theo dõi xe giao hàng - cấu phần tích hợp dữ liệu', N'Ước lượng thời gian thấp hơn thực tế'),
        (118, N'quản lý lịch trực bác sĩ phòng khám - cấu phần tích hợp dữ liệu', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (119, N'cổng học liệu số hóa cho đào tạo - cấu phần tích hợp dữ liệu', N'Hạ tầng thử nghiệm không ổn định'),
        (120, N'hệ thống đo lường trải nghiệm khách hàng - cấu phần tích hợp dữ liệu', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (121, N'hệ thống quản lý kho linh kiện điện tử - luồng vận hành thử nghiệm', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (122, N'nền tảng điều phối giao nhận khu vực - luồng vận hành thử nghiệm', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (123, N'cổng quản lý hồ sơ khám ngoại trú - luồng vận hành thử nghiệm', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (124, N'kho dữ liệu phân tích bán hàng - luồng vận hành thử nghiệm', N'Phối hợp nhiều team chưa đồng bộ'),
        (125, N'hệ thống đối soát tài chính nội bộ - luồng vận hành thử nghiệm', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (126, N'cổng thương mại điện tử ngành hàng tiêu dùng - luồng vận hành thử nghiệm', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (127, N'ứng dụng theo dõi đơn hàng đa kênh - luồng vận hành thử nghiệm', N'Ước lượng thời gian thấp hơn thực tế'),
        (128, N'bảng điều phối vận tải tuyến ngắn - luồng vận hành thử nghiệm', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (129, N'nền tảng đào tạo nội bộ cho nhân viên - luồng vận hành thử nghiệm', N'Hạ tầng thử nghiệm không ổn định'),
        (130, N'cổng tuyển dụng và phỏng vấn trực tuyến - luồng vận hành thử nghiệm', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (131, N'hệ thống quản lý tài sản cố định - luồng vận hành thử nghiệm', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (132, N'kho hồ sơ hợp đồng và pháp lý số - luồng vận hành thử nghiệm', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (133, N'bảng kế hoạch sản xuất theo lệnh - luồng vận hành thử nghiệm', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (134, N'hệ thống kiểm soát chất lượng đầu vào - luồng vận hành thử nghiệm', N'Phối hợp nhiều team chưa đồng bộ'),
        (135, N'ứng dụng bảo trì thiết bị nhà xưởng - luồng vận hành thử nghiệm', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (136, N'trung tâm chăm sóc khách hàng đa kênh - luồng vận hành thử nghiệm', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (137, N'cổng quản lý dịch vụ sau bán - luồng vận hành thử nghiệm', N'Ước lượng thời gian thấp hơn thực tế'),
        (138, N'hệ thống đánh giá nhà cung cấp - luồng vận hành thử nghiệm', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (139, N'quy trình phê duyệt mua hàng tự động - luồng vận hành thử nghiệm', N'Hạ tầng thử nghiệm không ổn định'),
        (140, N'bảng quản lý ngân sách phòng ban - luồng vận hành thử nghiệm', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (141, N'cổng đặt lịch hẹn kỹ thuật viên - luồng vận hành thử nghiệm', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (142, N'hệ thống quản lý vận đơn logistics - luồng vận hành thử nghiệm', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (143, N'nền tảng quản lý lớp học hybrid - luồng vận hành thử nghiệm', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (144, N'hồ sơ năng lực nhân sự số - luồng vận hành thử nghiệm', N'Phối hợp nhiều team chưa đồng bộ'),
        (145, N'cổng thông tin điều hành chi nhánh - luồng vận hành thử nghiệm', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (146, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - luồng vận hành thử nghiệm', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (147, N'trung tâm xử lý sự cố công nghệ thông tin - luồng vận hành thử nghiệm', N'Ước lượng thời gian thấp hơn thực tế'),
        (148, N'kho báo cáo vận hành đa phòng ban - luồng vận hành thử nghiệm', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (149, N'mô hình AI phân loại phản hồi dịch vụ - luồng vận hành thử nghiệm', N'Hạ tầng thử nghiệm không ổn định'),
        (150, N'hệ thống dự báo nhu cầu tồn kho - luồng vận hành thử nghiệm', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (151, N'quy trình tự động hóa hoàn ứng - luồng vận hành thử nghiệm', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (152, N'dự án nâng cấp hệ thống kế toán cũ - luồng vận hành thử nghiệm', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (153, N'luồng tích hợp dữ liệu ERP về kho - luồng vận hành thử nghiệm', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (154, N'nền tảng chuyển đổi dữ liệu khách hàng - luồng vận hành thử nghiệm', N'Phối hợp nhiều team chưa đồng bộ'),
        (155, N'cổng quản lý khuyến mãi thương mại - luồng vận hành thử nghiệm', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (156, N'ứng dụng kiểm kê kho lạnh - luồng vận hành thử nghiệm', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (157, N'hệ thống theo dõi xe giao hàng - luồng vận hành thử nghiệm', N'Ước lượng thời gian thấp hơn thực tế'),
        (158, N'quản lý lịch trực bác sĩ phòng khám - luồng vận hành thử nghiệm', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (159, N'cổng học liệu số hóa cho đào tạo - luồng vận hành thử nghiệm', N'Hạ tầng thử nghiệm không ổn định'),
        (160, N'hệ thống đo lường trải nghiệm khách hàng - luồng vận hành thử nghiệm', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (161, N'hệ thống quản lý kho linh kiện điện tử - gói bổ sung kiểm soát rủi ro', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (162, N'nền tảng điều phối giao nhận khu vực - gói bổ sung kiểm soát rủi ro', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (163, N'cổng quản lý hồ sơ khám ngoại trú - gói bổ sung kiểm soát rủi ro', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (164, N'kho dữ liệu phân tích bán hàng - gói bổ sung kiểm soát rủi ro', N'Phối hợp nhiều team chưa đồng bộ'),
        (165, N'hệ thống đối soát tài chính nội bộ - gói bổ sung kiểm soát rủi ro', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (166, N'cổng thương mại điện tử ngành hàng tiêu dùng - gói bổ sung kiểm soát rủi ro', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (167, N'ứng dụng theo dõi đơn hàng đa kênh - gói bổ sung kiểm soát rủi ro', N'Ước lượng thời gian thấp hơn thực tế'),
        (168, N'bảng điều phối vận tải tuyến ngắn - gói bổ sung kiểm soát rủi ro', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (169, N'nền tảng đào tạo nội bộ cho nhân viên - gói bổ sung kiểm soát rủi ro', N'Hạ tầng thử nghiệm không ổn định'),
        (170, N'cổng tuyển dụng và phỏng vấn trực tuyến - gói bổ sung kiểm soát rủi ro', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (171, N'hệ thống quản lý tài sản cố định - gói bổ sung kiểm soát rủi ro', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (172, N'kho hồ sơ hợp đồng và pháp lý số - gói bổ sung kiểm soát rủi ro', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (173, N'bảng kế hoạch sản xuất theo lệnh - gói bổ sung kiểm soát rủi ro', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (174, N'hệ thống kiểm soát chất lượng đầu vào - gói bổ sung kiểm soát rủi ro', N'Phối hợp nhiều team chưa đồng bộ'),
        (175, N'ứng dụng bảo trì thiết bị nhà xưởng - gói bổ sung kiểm soát rủi ro', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (176, N'trung tâm chăm sóc khách hàng đa kênh - gói bổ sung kiểm soát rủi ro', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (177, N'cổng quản lý dịch vụ sau bán - gói bổ sung kiểm soát rủi ro', N'Ước lượng thời gian thấp hơn thực tế'),
        (178, N'hệ thống đánh giá nhà cung cấp - gói bổ sung kiểm soát rủi ro', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (179, N'quy trình phê duyệt mua hàng tự động - gói bổ sung kiểm soát rủi ro', N'Hạ tầng thử nghiệm không ổn định'),
        (180, N'bảng quản lý ngân sách phòng ban - gói bổ sung kiểm soát rủi ro', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (181, N'cổng đặt lịch hẹn kỹ thuật viên - gói bổ sung kiểm soát rủi ro', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (182, N'hệ thống quản lý vận đơn logistics - gói bổ sung kiểm soát rủi ro', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (183, N'nền tảng quản lý lớp học hybrid - gói bổ sung kiểm soát rủi ro', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (184, N'hồ sơ năng lực nhân sự số - gói bổ sung kiểm soát rủi ro', N'Phối hợp nhiều team chưa đồng bộ'),
        (185, N'cổng thông tin điều hành chi nhánh - gói bổ sung kiểm soát rủi ro', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (186, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - gói bổ sung kiểm soát rủi ro', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (187, N'trung tâm xử lý sự cố công nghệ thông tin - gói bổ sung kiểm soát rủi ro', N'Ước lượng thời gian thấp hơn thực tế'),
        (188, N'kho báo cáo vận hành đa phòng ban - gói bổ sung kiểm soát rủi ro', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (189, N'mô hình AI phân loại phản hồi dịch vụ - gói bổ sung kiểm soát rủi ro', N'Hạ tầng thử nghiệm không ổn định'),
        (190, N'hệ thống dự báo nhu cầu tồn kho - gói bổ sung kiểm soát rủi ro', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (191, N'quy trình tự động hóa hoàn ứng - gói bổ sung kiểm soát rủi ro', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (192, N'dự án nâng cấp hệ thống kế toán cũ - gói bổ sung kiểm soát rủi ro', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (193, N'luồng tích hợp dữ liệu ERP về kho - gói bổ sung kiểm soát rủi ro', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (194, N'nền tảng chuyển đổi dữ liệu khách hàng - gói bổ sung kiểm soát rủi ro', N'Phối hợp nhiều team chưa đồng bộ'),
        (195, N'cổng quản lý khuyến mãi thương mại - gói bổ sung kiểm soát rủi ro', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (196, N'ứng dụng kiểm kê kho lạnh - gói bổ sung kiểm soát rủi ro', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (197, N'hệ thống theo dõi xe giao hàng - gói bổ sung kiểm soát rủi ro', N'Ước lượng thời gian thấp hơn thực tế'),
        (198, N'quản lý lịch trực bác sĩ phòng khám - gói bổ sung kiểm soát rủi ro', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (199, N'cổng học liệu số hóa cho đào tạo - gói bổ sung kiểm soát rủi ro', N'Hạ tầng thử nghiệm không ổn định'),
        (200, N'hệ thống đo lường trải nghiệm khách hàng - gói bổ sung kiểm soát rủi ro', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (201, N'hệ thống quản lý kho linh kiện điện tử - phiên bản bàn giao theo đợt', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (202, N'nền tảng điều phối giao nhận khu vực - phiên bản bàn giao theo đợt', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (203, N'cổng quản lý hồ sơ khám ngoại trú - phiên bản bàn giao theo đợt', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (204, N'kho dữ liệu phân tích bán hàng - phiên bản bàn giao theo đợt', N'Phối hợp nhiều team chưa đồng bộ'),
        (205, N'hệ thống đối soát tài chính nội bộ - phiên bản bàn giao theo đợt', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (206, N'cổng thương mại điện tử ngành hàng tiêu dùng - phiên bản bàn giao theo đợt', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (207, N'ứng dụng theo dõi đơn hàng đa kênh - phiên bản bàn giao theo đợt', N'Ước lượng thời gian thấp hơn thực tế'),
        (208, N'bảng điều phối vận tải tuyến ngắn - phiên bản bàn giao theo đợt', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (209, N'nền tảng đào tạo nội bộ cho nhân viên - phiên bản bàn giao theo đợt', N'Hạ tầng thử nghiệm không ổn định'),
        (210, N'cổng tuyển dụng và phỏng vấn trực tuyến - phiên bản bàn giao theo đợt', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (211, N'hệ thống quản lý tài sản cố định - phiên bản bàn giao theo đợt', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (212, N'kho hồ sơ hợp đồng và pháp lý số - phiên bản bàn giao theo đợt', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (213, N'bảng kế hoạch sản xuất theo lệnh - phiên bản bàn giao theo đợt', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (214, N'hệ thống kiểm soát chất lượng đầu vào - phiên bản bàn giao theo đợt', N'Phối hợp nhiều team chưa đồng bộ'),
        (215, N'ứng dụng bảo trì thiết bị nhà xưởng - phiên bản bàn giao theo đợt', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (216, N'trung tâm chăm sóc khách hàng đa kênh - phiên bản bàn giao theo đợt', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (217, N'cổng quản lý dịch vụ sau bán - phiên bản bàn giao theo đợt', N'Ước lượng thời gian thấp hơn thực tế'),
        (218, N'hệ thống đánh giá nhà cung cấp - phiên bản bàn giao theo đợt', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (219, N'quy trình phê duyệt mua hàng tự động - phiên bản bàn giao theo đợt', N'Hạ tầng thử nghiệm không ổn định'),
        (220, N'bảng quản lý ngân sách phòng ban - phiên bản bàn giao theo đợt', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (221, N'cổng đặt lịch hẹn kỹ thuật viên - phiên bản bàn giao theo đợt', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (222, N'hệ thống quản lý vận đơn logistics - phiên bản bàn giao theo đợt', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (223, N'nền tảng quản lý lớp học hybrid - phiên bản bàn giao theo đợt', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (224, N'hồ sơ năng lực nhân sự số - phiên bản bàn giao theo đợt', N'Phối hợp nhiều team chưa đồng bộ'),
        (225, N'cổng thông tin điều hành chi nhánh - phiên bản bàn giao theo đợt', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (226, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - phiên bản bàn giao theo đợt', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (227, N'trung tâm xử lý sự cố công nghệ thông tin - phiên bản bàn giao theo đợt', N'Ước lượng thời gian thấp hơn thực tế'),
        (228, N'kho báo cáo vận hành đa phòng ban - phiên bản bàn giao theo đợt', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (229, N'mô hình AI phân loại phản hồi dịch vụ - phiên bản bàn giao theo đợt', N'Hạ tầng thử nghiệm không ổn định'),
        (230, N'hệ thống dự báo nhu cầu tồn kho - phiên bản bàn giao theo đợt', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (231, N'quy trình tự động hóa hoàn ứng - phiên bản bàn giao theo đợt', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (232, N'dự án nâng cấp hệ thống kế toán cũ - phiên bản bàn giao theo đợt', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (233, N'luồng tích hợp dữ liệu ERP về kho - phiên bản bàn giao theo đợt', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (234, N'nền tảng chuyển đổi dữ liệu khách hàng - phiên bản bàn giao theo đợt', N'Phối hợp nhiều team chưa đồng bộ'),
        (235, N'cổng quản lý khuyến mãi thương mại - phiên bản bàn giao theo đợt', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (236, N'ứng dụng kiểm kê kho lạnh - phiên bản bàn giao theo đợt', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (237, N'hệ thống theo dõi xe giao hàng - phiên bản bàn giao theo đợt', N'Ước lượng thời gian thấp hơn thực tế'),
        (238, N'quản lý lịch trực bác sĩ phòng khám - phiên bản bàn giao theo đợt', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (239, N'cổng học liệu số hóa cho đào tạo - phiên bản bàn giao theo đợt', N'Hạ tầng thử nghiệm không ổn định'),
        (240, N'hệ thống đo lường trải nghiệm khách hàng - phiên bản bàn giao theo đợt', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (241, N'hệ thống quản lý kho linh kiện điện tử - cấu phần tối ưu hiệu năng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (242, N'nền tảng điều phối giao nhận khu vực - cấu phần tối ưu hiệu năng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (243, N'cổng quản lý hồ sơ khám ngoại trú - cấu phần tối ưu hiệu năng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (244, N'kho dữ liệu phân tích bán hàng - cấu phần tối ưu hiệu năng', N'Phối hợp nhiều team chưa đồng bộ'),
        (245, N'hệ thống đối soát tài chính nội bộ - cấu phần tối ưu hiệu năng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (246, N'cổng thương mại điện tử ngành hàng tiêu dùng - cấu phần tối ưu hiệu năng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (247, N'ứng dụng theo dõi đơn hàng đa kênh - cấu phần tối ưu hiệu năng', N'Ước lượng thời gian thấp hơn thực tế'),
        (248, N'bảng điều phối vận tải tuyến ngắn - cấu phần tối ưu hiệu năng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (249, N'nền tảng đào tạo nội bộ cho nhân viên - cấu phần tối ưu hiệu năng', N'Hạ tầng thử nghiệm không ổn định'),
        (250, N'cổng tuyển dụng và phỏng vấn trực tuyến - cấu phần tối ưu hiệu năng', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (251, N'hệ thống quản lý tài sản cố định - cấu phần tối ưu hiệu năng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (252, N'kho hồ sơ hợp đồng và pháp lý số - cấu phần tối ưu hiệu năng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (253, N'bảng kế hoạch sản xuất theo lệnh - cấu phần tối ưu hiệu năng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (254, N'hệ thống kiểm soát chất lượng đầu vào - cấu phần tối ưu hiệu năng', N'Phối hợp nhiều team chưa đồng bộ'),
        (255, N'ứng dụng bảo trì thiết bị nhà xưởng - cấu phần tối ưu hiệu năng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (256, N'trung tâm chăm sóc khách hàng đa kênh - cấu phần tối ưu hiệu năng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (257, N'cổng quản lý dịch vụ sau bán - cấu phần tối ưu hiệu năng', N'Ước lượng thời gian thấp hơn thực tế'),
        (258, N'hệ thống đánh giá nhà cung cấp - cấu phần tối ưu hiệu năng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (259, N'quy trình phê duyệt mua hàng tự động - cấu phần tối ưu hiệu năng', N'Hạ tầng thử nghiệm không ổn định'),
        (260, N'bảng quản lý ngân sách phòng ban - cấu phần tối ưu hiệu năng', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (261, N'cổng đặt lịch hẹn kỹ thuật viên - cấu phần tối ưu hiệu năng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (262, N'hệ thống quản lý vận đơn logistics - cấu phần tối ưu hiệu năng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (263, N'nền tảng quản lý lớp học hybrid - cấu phần tối ưu hiệu năng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (264, N'hồ sơ năng lực nhân sự số - cấu phần tối ưu hiệu năng', N'Phối hợp nhiều team chưa đồng bộ'),
        (265, N'cổng thông tin điều hành chi nhánh - cấu phần tối ưu hiệu năng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (266, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - cấu phần tối ưu hiệu năng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (267, N'trung tâm xử lý sự cố công nghệ thông tin - cấu phần tối ưu hiệu năng', N'Ước lượng thời gian thấp hơn thực tế'),
        (268, N'kho báo cáo vận hành đa phòng ban - cấu phần tối ưu hiệu năng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (269, N'mô hình AI phân loại phản hồi dịch vụ - cấu phần tối ưu hiệu năng', N'Hạ tầng thử nghiệm không ổn định'),
        (270, N'hệ thống dự báo nhu cầu tồn kho - cấu phần tối ưu hiệu năng', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (271, N'quy trình tự động hóa hoàn ứng - cấu phần tối ưu hiệu năng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (272, N'dự án nâng cấp hệ thống kế toán cũ - cấu phần tối ưu hiệu năng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (273, N'luồng tích hợp dữ liệu ERP về kho - cấu phần tối ưu hiệu năng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (274, N'nền tảng chuyển đổi dữ liệu khách hàng - cấu phần tối ưu hiệu năng', N'Phối hợp nhiều team chưa đồng bộ'),
        (275, N'cổng quản lý khuyến mãi thương mại - cấu phần tối ưu hiệu năng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (276, N'ứng dụng kiểm kê kho lạnh - cấu phần tối ưu hiệu năng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (277, N'hệ thống theo dõi xe giao hàng - cấu phần tối ưu hiệu năng', N'Ước lượng thời gian thấp hơn thực tế'),
        (278, N'quản lý lịch trực bác sĩ phòng khám - cấu phần tối ưu hiệu năng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (279, N'cổng học liệu số hóa cho đào tạo - cấu phần tối ưu hiệu năng', N'Hạ tầng thử nghiệm không ổn định'),
        (280, N'hệ thống đo lường trải nghiệm khách hàng - cấu phần tối ưu hiệu năng', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (281, N'hệ thống quản lý kho linh kiện điện tử - giai đoạn đồng bộ quyền truy cập', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (282, N'nền tảng điều phối giao nhận khu vực - giai đoạn đồng bộ quyền truy cập', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (283, N'cổng quản lý hồ sơ khám ngoại trú - giai đoạn đồng bộ quyền truy cập', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (284, N'kho dữ liệu phân tích bán hàng - giai đoạn đồng bộ quyền truy cập', N'Phối hợp nhiều team chưa đồng bộ'),
        (285, N'hệ thống đối soát tài chính nội bộ - giai đoạn đồng bộ quyền truy cập', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (286, N'cổng thương mại điện tử ngành hàng tiêu dùng - giai đoạn đồng bộ quyền truy cập', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (287, N'ứng dụng theo dõi đơn hàng đa kênh - giai đoạn đồng bộ quyền truy cập', N'Ước lượng thời gian thấp hơn thực tế'),
        (288, N'bảng điều phối vận tải tuyến ngắn - giai đoạn đồng bộ quyền truy cập', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (289, N'nền tảng đào tạo nội bộ cho nhân viên - giai đoạn đồng bộ quyền truy cập', N'Hạ tầng thử nghiệm không ổn định'),
        (290, N'cổng tuyển dụng và phỏng vấn trực tuyến - giai đoạn đồng bộ quyền truy cập', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (291, N'hệ thống quản lý tài sản cố định - giai đoạn đồng bộ quyền truy cập', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (292, N'kho hồ sơ hợp đồng và pháp lý số - giai đoạn đồng bộ quyền truy cập', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (293, N'bảng kế hoạch sản xuất theo lệnh - giai đoạn đồng bộ quyền truy cập', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (294, N'hệ thống kiểm soát chất lượng đầu vào - giai đoạn đồng bộ quyền truy cập', N'Phối hợp nhiều team chưa đồng bộ'),
        (295, N'ứng dụng bảo trì thiết bị nhà xưởng - giai đoạn đồng bộ quyền truy cập', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (296, N'trung tâm chăm sóc khách hàng đa kênh - giai đoạn đồng bộ quyền truy cập', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (297, N'cổng quản lý dịch vụ sau bán - giai đoạn đồng bộ quyền truy cập', N'Ước lượng thời gian thấp hơn thực tế'),
        (298, N'hệ thống đánh giá nhà cung cấp - giai đoạn đồng bộ quyền truy cập', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (299, N'quy trình phê duyệt mua hàng tự động - giai đoạn đồng bộ quyền truy cập', N'Hạ tầng thử nghiệm không ổn định'),
        (300, N'bảng quản lý ngân sách phòng ban - giai đoạn đồng bộ quyền truy cập', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (301, N'cổng đặt lịch hẹn kỹ thuật viên - giai đoạn đồng bộ quyền truy cập', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (302, N'hệ thống quản lý vận đơn logistics - giai đoạn đồng bộ quyền truy cập', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (303, N'nền tảng quản lý lớp học hybrid - giai đoạn đồng bộ quyền truy cập', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (304, N'hồ sơ năng lực nhân sự số - giai đoạn đồng bộ quyền truy cập', N'Phối hợp nhiều team chưa đồng bộ'),
        (305, N'cổng thông tin điều hành chi nhánh - giai đoạn đồng bộ quyền truy cập', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (306, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - giai đoạn đồng bộ quyền truy cập', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (307, N'trung tâm xử lý sự cố công nghệ thông tin - giai đoạn đồng bộ quyền truy cập', N'Ước lượng thời gian thấp hơn thực tế'),
        (308, N'kho báo cáo vận hành đa phòng ban - giai đoạn đồng bộ quyền truy cập', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (309, N'mô hình AI phân loại phản hồi dịch vụ - giai đoạn đồng bộ quyền truy cập', N'Hạ tầng thử nghiệm không ổn định'),
        (310, N'hệ thống dự báo nhu cầu tồn kho - giai đoạn đồng bộ quyền truy cập', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (311, N'quy trình tự động hóa hoàn ứng - giai đoạn đồng bộ quyền truy cập', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (312, N'dự án nâng cấp hệ thống kế toán cũ - giai đoạn đồng bộ quyền truy cập', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (313, N'luồng tích hợp dữ liệu ERP về kho - giai đoạn đồng bộ quyền truy cập', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (314, N'nền tảng chuyển đổi dữ liệu khách hàng - giai đoạn đồng bộ quyền truy cập', N'Phối hợp nhiều team chưa đồng bộ'),
        (315, N'cổng quản lý khuyến mãi thương mại - giai đoạn đồng bộ quyền truy cập', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (316, N'ứng dụng kiểm kê kho lạnh - giai đoạn đồng bộ quyền truy cập', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (317, N'hệ thống theo dõi xe giao hàng - giai đoạn đồng bộ quyền truy cập', N'Ước lượng thời gian thấp hơn thực tế'),
        (318, N'quản lý lịch trực bác sĩ phòng khám - giai đoạn đồng bộ quyền truy cập', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (319, N'cổng học liệu số hóa cho đào tạo - giai đoạn đồng bộ quyền truy cập', N'Hạ tầng thử nghiệm không ổn định'),
        (320, N'hệ thống đo lường trải nghiệm khách hàng - giai đoạn đồng bộ quyền truy cập', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (321, N'hệ thống quản lý kho linh kiện điện tử - luồng báo cáo quản trị nâng cao', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (322, N'nền tảng điều phối giao nhận khu vực - luồng báo cáo quản trị nâng cao', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (323, N'cổng quản lý hồ sơ khám ngoại trú - luồng báo cáo quản trị nâng cao', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (324, N'kho dữ liệu phân tích bán hàng - luồng báo cáo quản trị nâng cao', N'Phối hợp nhiều team chưa đồng bộ'),
        (325, N'hệ thống đối soát tài chính nội bộ - luồng báo cáo quản trị nâng cao', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (326, N'cổng thương mại điện tử ngành hàng tiêu dùng - luồng báo cáo quản trị nâng cao', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (327, N'ứng dụng theo dõi đơn hàng đa kênh - luồng báo cáo quản trị nâng cao', N'Ước lượng thời gian thấp hơn thực tế'),
        (328, N'bảng điều phối vận tải tuyến ngắn - luồng báo cáo quản trị nâng cao', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (329, N'nền tảng đào tạo nội bộ cho nhân viên - luồng báo cáo quản trị nâng cao', N'Hạ tầng thử nghiệm không ổn định'),
        (330, N'cổng tuyển dụng và phỏng vấn trực tuyến - luồng báo cáo quản trị nâng cao', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (331, N'hệ thống quản lý tài sản cố định - luồng báo cáo quản trị nâng cao', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (332, N'kho hồ sơ hợp đồng và pháp lý số - luồng báo cáo quản trị nâng cao', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (333, N'bảng kế hoạch sản xuất theo lệnh - luồng báo cáo quản trị nâng cao', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (334, N'hệ thống kiểm soát chất lượng đầu vào - luồng báo cáo quản trị nâng cao', N'Phối hợp nhiều team chưa đồng bộ'),
        (335, N'ứng dụng bảo trì thiết bị nhà xưởng - luồng báo cáo quản trị nâng cao', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (336, N'trung tâm chăm sóc khách hàng đa kênh - luồng báo cáo quản trị nâng cao', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (337, N'cổng quản lý dịch vụ sau bán - luồng báo cáo quản trị nâng cao', N'Ước lượng thời gian thấp hơn thực tế'),
        (338, N'hệ thống đánh giá nhà cung cấp - luồng báo cáo quản trị nâng cao', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (339, N'quy trình phê duyệt mua hàng tự động - luồng báo cáo quản trị nâng cao', N'Hạ tầng thử nghiệm không ổn định'),
        (340, N'bảng quản lý ngân sách phòng ban - luồng báo cáo quản trị nâng cao', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (341, N'cổng đặt lịch hẹn kỹ thuật viên - luồng báo cáo quản trị nâng cao', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (342, N'hệ thống quản lý vận đơn logistics - luồng báo cáo quản trị nâng cao', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (343, N'nền tảng quản lý lớp học hybrid - luồng báo cáo quản trị nâng cao', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (344, N'hồ sơ năng lực nhân sự số - luồng báo cáo quản trị nâng cao', N'Phối hợp nhiều team chưa đồng bộ'),
        (345, N'cổng thông tin điều hành chi nhánh - luồng báo cáo quản trị nâng cao', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (346, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - luồng báo cáo quản trị nâng cao', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (347, N'trung tâm xử lý sự cố công nghệ thông tin - luồng báo cáo quản trị nâng cao', N'Ước lượng thời gian thấp hơn thực tế'),
        (348, N'kho báo cáo vận hành đa phòng ban - luồng báo cáo quản trị nâng cao', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (349, N'mô hình AI phân loại phản hồi dịch vụ - luồng báo cáo quản trị nâng cao', N'Hạ tầng thử nghiệm không ổn định'),
        (350, N'hệ thống dự báo nhu cầu tồn kho - luồng báo cáo quản trị nâng cao', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (351, N'quy trình tự động hóa hoàn ứng - luồng báo cáo quản trị nâng cao', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (352, N'dự án nâng cấp hệ thống kế toán cũ - luồng báo cáo quản trị nâng cao', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (353, N'luồng tích hợp dữ liệu ERP về kho - luồng báo cáo quản trị nâng cao', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (354, N'nền tảng chuyển đổi dữ liệu khách hàng - luồng báo cáo quản trị nâng cao', N'Phối hợp nhiều team chưa đồng bộ'),
        (355, N'cổng quản lý khuyến mãi thương mại - luồng báo cáo quản trị nâng cao', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (356, N'ứng dụng kiểm kê kho lạnh - luồng báo cáo quản trị nâng cao', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (357, N'hệ thống theo dõi xe giao hàng - luồng báo cáo quản trị nâng cao', N'Ước lượng thời gian thấp hơn thực tế'),
        (358, N'quản lý lịch trực bác sĩ phòng khám - luồng báo cáo quản trị nâng cao', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (359, N'cổng học liệu số hóa cho đào tạo - luồng báo cáo quản trị nâng cao', N'Hạ tầng thử nghiệm không ổn định'),
        (360, N'hệ thống đo lường trải nghiệm khách hàng - luồng báo cáo quản trị nâng cao', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (361, N'hệ thống quản lý kho linh kiện điện tử - phiên bản chấp nhận người dùng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (362, N'nền tảng điều phối giao nhận khu vực - phiên bản chấp nhận người dùng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (363, N'cổng quản lý hồ sơ khám ngoại trú - phiên bản chấp nhận người dùng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (364, N'kho dữ liệu phân tích bán hàng - phiên bản chấp nhận người dùng', N'Phối hợp nhiều team chưa đồng bộ'),
        (365, N'hệ thống đối soát tài chính nội bộ - phiên bản chấp nhận người dùng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (366, N'cổng thương mại điện tử ngành hàng tiêu dùng - phiên bản chấp nhận người dùng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (367, N'ứng dụng theo dõi đơn hàng đa kênh - phiên bản chấp nhận người dùng', N'Ước lượng thời gian thấp hơn thực tế'),
        (368, N'bảng điều phối vận tải tuyến ngắn - phiên bản chấp nhận người dùng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (369, N'nền tảng đào tạo nội bộ cho nhân viên - phiên bản chấp nhận người dùng', N'Hạ tầng thử nghiệm không ổn định'),
        (370, N'cổng tuyển dụng và phỏng vấn trực tuyến - phiên bản chấp nhận người dùng', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (371, N'hệ thống quản lý tài sản cố định - phiên bản chấp nhận người dùng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (372, N'kho hồ sơ hợp đồng và pháp lý số - phiên bản chấp nhận người dùng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (373, N'bảng kế hoạch sản xuất theo lệnh - phiên bản chấp nhận người dùng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (374, N'hệ thống kiểm soát chất lượng đầu vào - phiên bản chấp nhận người dùng', N'Phối hợp nhiều team chưa đồng bộ'),
        (375, N'ứng dụng bảo trì thiết bị nhà xưởng - phiên bản chấp nhận người dùng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (376, N'trung tâm chăm sóc khách hàng đa kênh - phiên bản chấp nhận người dùng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (377, N'cổng quản lý dịch vụ sau bán - phiên bản chấp nhận người dùng', N'Ước lượng thời gian thấp hơn thực tế'),
        (378, N'hệ thống đánh giá nhà cung cấp - phiên bản chấp nhận người dùng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (379, N'quy trình phê duyệt mua hàng tự động - phiên bản chấp nhận người dùng', N'Hạ tầng thử nghiệm không ổn định'),
        (380, N'bảng quản lý ngân sách phòng ban - phiên bản chấp nhận người dùng', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (381, N'cổng đặt lịch hẹn kỹ thuật viên - phiên bản chấp nhận người dùng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (382, N'hệ thống quản lý vận đơn logistics - phiên bản chấp nhận người dùng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (383, N'nền tảng quản lý lớp học hybrid - phiên bản chấp nhận người dùng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (384, N'hồ sơ năng lực nhân sự số - phiên bản chấp nhận người dùng', N'Phối hợp nhiều team chưa đồng bộ'),
        (385, N'cổng thông tin điều hành chi nhánh - phiên bản chấp nhận người dùng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (386, N'hệ thống tiếp nhận yêu cầu hỗ trợ kỹ thuật - phiên bản chấp nhận người dùng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (387, N'trung tâm xử lý sự cố công nghệ thông tin - phiên bản chấp nhận người dùng', N'Ước lượng thời gian thấp hơn thực tế'),
        (388, N'kho báo cáo vận hành đa phòng ban - phiên bản chấp nhận người dùng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (389, N'mô hình AI phân loại phản hồi dịch vụ - phiên bản chấp nhận người dùng', N'Hạ tầng thử nghiệm không ổn định'),
        (390, N'hệ thống dự báo nhu cầu tồn kho - phiên bản chấp nhận người dùng', N'Phạm vi phát sinh cần xử lý bổ sung'),
        (391, N'quy trình tự động hóa hoàn ứng - phiên bản chấp nhận người dùng', N'Thiếu nhân sự kiểm thử trong giai đoạn tích hợp cuối'),
        (392, N'dự án nâng cấp hệ thống kế toán cũ - phiên bản chấp nhận người dùng', N'Thay đổi yêu cầu nghiệp vụ sau khi chạy thử'),
        (393, N'luồng tích hợp dữ liệu ERP về kho - phiên bản chấp nhận người dùng', N'Quy trình duyệt kéo dài làm chậm bàn giao'),
        (394, N'nền tảng chuyển đổi dữ liệu khách hàng - phiên bản chấp nhận người dùng', N'Phối hợp nhiều team chưa đồng bộ'),
        (395, N'cổng quản lý khuyến mãi thương mại - phiên bản chấp nhận người dùng', N'Dữ liệu đầu vào thiếu chuẩn hóa'),
        (396, N'ứng dụng kiểm kê kho lạnh - phiên bản chấp nhận người dùng', N'Tích hợp hệ thống ngoài phát sinh lỗi'),
        (397, N'hệ thống theo dõi xe giao hàng - phiên bản chấp nhận người dùng', N'Ước lượng thời gian thấp hơn thực tế'),
        (398, N'quản lý lịch trực bác sĩ phòng khám - phiên bản chấp nhận người dùng', N'Tài liệu nghiệp vụ cập nhật chậm'),
        (399, N'cổng học liệu số hóa cho đào tạo - phiên bản chấp nhận người dùng', N'Hạ tầng thử nghiệm không ổn định'),
        (400, N'hệ thống đo lường trải nghiệm khách hàng - phiên bản chấp nhận người dùng', N'Phạm vi phát sinh cần xử lý bổ sung');

    INSERT INTO @Cfg
    (
        BatchNo, TenDuAn, MoTa, Scenario, MaLoaiDuAn, MaQuanLyCu, MaQuanLyChinh,
        NgayTao, NgayBatDau, NgayKetThuc, NgayHoanThanh, TrangThai, PhanTram,
        Budget, CostTotal, CategoryCount, WorkCount, MinDetailPerWork, MaxDetailPerWork,
        ReportBase, CompleteWorkCount, StaffChanges, ManagerChanges, DxcvReject,
        DxcvPending, DxnsReject, DxnsPending, TeamMask, MemberMask, LeaderMask, MissingFileEvery
    )
    SELECT
        s.BatchNo,
        CONCAT(N'DATA10-', FORMAT(s.BatchNo, '000'), N' - ', s.TenNghiepVu),
        CONCAT(N'Triển khai ', LOWER(s.TenNghiepVu), N' phục vụ vận hành thực tế, có dữ liệu công việc, tiến độ, chi phí và phối hợp nhân sự.'),
        s.Scenario,
        CASE WHEN s.BatchNo % 10 IN (2, 8) THEN @LoaiBaoTri WHEN s.BatchNo % 10 IN (7) THEN @LoaiAI ELSE @LoaiPhanMem END,
        CASE
            WHEN CASE WHEN s.BatchNo IN (20, 50, 80, 95) THEN 2 WHEN s.BatchNo % 7 = 0 THEN 1 ELSE 0 END = 0 THEN
                CASE WHEN s.BatchNo % 3 = 0 THEN @ManagerNam WHEN s.BatchNo % 3 = 1 THEN @ManagerHuong ELSE @ManagerBao END
            WHEN CASE WHEN s.BatchNo % 3 = 0 THEN @ManagerNam WHEN s.BatchNo % 3 = 1 THEN @ManagerHuong ELSE @ManagerBao END = @ManagerNam THEN @ManagerHuong
            ELSE @ManagerNam
        END,
        CASE WHEN s.BatchNo IN (20, 50, 80, 95) THEN @ManagerBao WHEN s.BatchNo % 3 = 0 THEN @ManagerNam WHEN s.BatchNo % 3 = 1 THEN @ManagerHuong ELSE @ManagerBao END,
        DATEADD(HOUR, 7 + (s.BatchNo % 6), DATEADD(DAY, (s.BatchNo * 3) % 42, CONVERT(DATETIME2(0), '2026-03-15T00:00:00'))),
        DATEADD(HOUR, 8 + (s.BatchNo % 4), DATEADD(DAY, 1 + ((s.BatchNo * 3) % 42), CONVERT(DATETIME2(0), '2026-03-15T00:00:00'))),
        DATEADD(HOUR, 17, DATEADD(DAY, 9 + ((s.BatchNo * 3) % 42) + CASE WHEN s.BatchNo % 20 = 0 THEN 42 WHEN s.BatchNo % 11 = 0 THEN 30 WHEN s.BatchNo % 7 = 0 THEN 22 WHEN s.BatchNo % 5 = 0 THEN 16 ELSE 8 + (s.BatchNo % 10) END, CONVERT(DATETIME2(0), '2026-03-15T00:00:00'))),
        CASE WHEN s.BatchNo <= 140 THEN DATEADD(HOUR, 19 + (s.BatchNo % 4), DATEADD(DAY, 10 + ((s.BatchNo * 3) % 42) + CASE WHEN s.BatchNo % 20 = 0 THEN 42 WHEN s.BatchNo % 11 = 0 THEN 30 WHEN s.BatchNo % 7 = 0 THEN 22 WHEN s.BatchNo % 5 = 0 THEN 16 ELSE 8 + (s.BatchNo % 10) END + (s.BatchNo % 5), CONVERT(DATETIME2(0), '2026-03-15T00:00:00'))) ELSE NULL END,
        CASE WHEN s.BatchNo <= 140 THEN N'HoanThanh' ELSE N'DangThucHien' END,
        CASE WHEN s.BatchNo <= 140 THEN 100 ELSE 45 + (s.BatchNo % 51) END,
        CAST(60000000 + (s.BatchNo % 16) * 7500000 AS DECIMAL(18,2)),
        CAST((60000000 + (s.BatchNo % 16) * 7500000) * CASE WHEN s.BatchNo % 10 IN (4, 9) THEN 0.96 WHEN s.BatchNo % 10 IN (2, 5, 8) THEN 0.82 WHEN s.BatchNo % 10 IN (1, 6) THEN 0.68 ELSE 0.54 END AS DECIMAL(18,2)),
        2 + (s.BatchNo % 5),
        4 + (s.BatchNo % 7),
        1,
        3,
        1 + (s.BatchNo % 3),
        CASE
            WHEN s.BatchNo <= 140 THEN 4 + (s.BatchNo % 7)
            ELSE (4 + (s.BatchNo % 7)) - (1 + (s.BatchNo % 3))
        END,
        CASE WHEN s.BatchNo % 11 = 0 THEN 4 WHEN s.BatchNo % 5 = 0 THEN 3 WHEN s.BatchNo % 3 = 0 THEN 2 WHEN s.BatchNo % 2 = 0 THEN 1 ELSE 0 END,
        CASE WHEN s.BatchNo IN (20, 50, 80, 95) THEN 2 WHEN s.BatchNo % 7 = 0 THEN 1 ELSE 0 END,
        CASE WHEN s.BatchNo <= 140 THEN 0 ELSE s.BatchNo % 4 END,
        CASE WHEN s.BatchNo <= 140 THEN 0 ELSE s.BatchNo % 3 END,
        CASE WHEN s.BatchNo <= 140 THEN 0 ELSE s.BatchNo % 3 END,
        CASE WHEN s.BatchNo <= 140 THEN 0 ELSE s.BatchNo % 2 END,
        CASE WHEN s.BatchNo % 6 = 0 THEN N'B,F,K' WHEN s.BatchNo % 3 = 0 THEN N'B,K' WHEN s.BatchNo % 3 = 1 THEN N'B,F' ELSE N'F,K' END,
        CASE s.BatchNo % 7
            WHEN 0 THEN N'D,M,K,H,V,L,P,A,N'
            WHEN 1 THEN N'D,K,H,V'
            WHEN 2 THEN N'M,K,V,L,P'
            WHEN 3 THEN N'D,M,H,V,L,P'
            WHEN 4 THEN N'D,M,K,H,V,L'
            WHEN 5 THEN N'K,H,V,P,A'
            ELSE N'D,M,K,L,P,A,N'
        END,
        CASE WHEN s.BatchNo % 3 = 0 THEN N'D,K' WHEN s.BatchNo % 3 = 1 THEN N'D,M' ELSE N'M,K' END,
        CASE WHEN s.BatchNo % 5 = 0 THEN 0 ELSE 2 + (s.BatchNo % 4) END
    FROM @Seed s;

    WHILE @BatchNo <= 400
    BEGIN
        SET @TenDuAn = NULL;
        SET @NgayHoanThanh = NULL;
        SET @MaNganSach = NULL;
        SET @MaDeXuatCV = NULL;
        SET @MaCongViec = NULL;
        SET @MaChiPhi = NULL;
        SET @MaChiTietCV = NULL;
        SET @MaTienDo = NULL;
        SET @MaPhongChat = NULL;

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
            @MinDetailPerWork = MinDetailPerWork,
            @MaxDetailPerWork = MaxDetailPerWork,
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

        IF @TenDuAn IS NULL
        BEGIN
            THROW 50004, N'Thiếu cấu hình dự án DATA10 trong bảng @Cfg.', 1;
        END;

        /* =========================================================
           3. DU AN DATA10 - TAO DU AN, TEAM, NHAN SU VA NHAT KY
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
        VALUES (@MaDuAn, @MaQuanLyChinh, N'Gán quản lý phụ trách dự án DATA10', DATEADD(MINUTE, 10, @NgayTao), @NgayTao, NULL);

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
                CONCAT(N'Hạng mục DATA10-', FORMAT(@BatchNo, '000'), N'-', FORMAT(@i, '00')),
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
            N'DaDuyet', N'Duyệt ngân sách ban đầu cho dự án DATA10',
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

        SET @TongChiPhiDaPhanBo = 0;
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
            SET @TrangThaiCV = CASE
                WHEN @i <= @CompleteWorkCount THEN N'HoanThanh'
                ELSE N'DangThucHien'
            END;
            SET @ChiPhi = CASE
                WHEN @i = @WorkCount THEN @CostTotal - @TongChiPhiDaPhanBo
                ELSE ROUND(@CostTotal / @WorkCount, 0)
            END;
            SET @TongChiPhiDaPhanBo = @TongChiPhiDaPhanBo + @ChiPhi;

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
                CONCAT(N'Công việc ', FORMAT(@i, '00'), N' - DATA10-', FORMAT(@BatchNo, '000')),
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

            SET @DetailCount = @MinDetailPerWork + ((@BatchNo + @i) % (@MaxDetailPerWork - @MinDetailPerWork + 1));
            IF @DetailCount < 1 SET @DetailCount = 1;
            IF @DetailCount > 4 SET @DetailCount = 4;

            SET @d = 1;
            WHILE @d <= @DetailCount
            BEGIN
                SET @Assignee = (SELECT TOP 1 MaNguoiDung FROM @Member WHERE BatchNo = @BatchNo ORDER BY ABS(CHECKSUM(CONCAT(@BatchNo, '-', @i, '-', @d, '-', MaNguoiDung))));
                SET @TrangThaiCT = CASE
                    WHEN @TrangThaiCV = N'HoanThanh' THEN N'HoanThanh'
                    WHEN @d < @DetailCount AND (@BatchNo + @i + @d) % 3 = 0 THEN N'HoanThanh'
                    WHEN @d = @DetailCount AND (@BatchNo + @i) % 7 = 0 THEN N'BiCanCan'
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

                IF NOT EXISTS
                (
                    SELECT 1 FROM dbo.PHAN_CONG_CONG_VIEC
                    WHERE MaNguoiDung = @Assignee AND MaCongViec = @MaCongViec
                )
                BEGIN
                    INSERT INTO dbo.PHAN_CONG_CONG_VIEC (MaNguoiDung, MaCongViec, NgayGiaoViec)
                    VALUES (@Assignee, @MaCongViec, DATEADD(MINUTE, -30, @NgayBatDauCV));

                    INSERT INTO dbo.NHAT_KY_PHAN_CONG_CONG_VIEC
                    (MaNguoiDung, MaCongViec, MaNguoiDungGhi, HanhDongPCCV, ThoiGianPCCV)
                    VALUES
                    (@Assignee, @MaCongViec, @MaQuanLyChinh, N'Thêm phân công công việc', DATEADD(MINUTE, -25, @NgayBatDauCV));
                END;

                INSERT INTO dbo.PHAN_CONG_CT_CONG_VIEC (MaNguoiDung, MaChiTietCV, NgayGiaoCTCV, VaiTroTrongCTCV)
                VALUES (@Assignee, @MaChiTietCV, DATEADD(MINUTE, -20, @NgayBatDauCT), N'Thực hiện');

                INSERT INTO dbo.NHAT_KY_PHAN_CONG_CT_CONG_VIEC
                (MaNguoiDung, MaChiTietCV, MaNguoiDungGhi, HanhDongPCCTCV, ThoiGianPCCTCV)
                VALUES
                (@Assignee, @MaChiTietCV, @MaQuanLyChinh, N'Thêm phân công chi tiết công việc', DATEADD(MINUTE, -15, @NgayBatDauCT));

                SET @r = 1;
                SET @ReportCount = @ReportBase + CASE WHEN (@i + @d + @BatchNo) % 3 = 0 THEN 1 ELSE 0 END;
                WHILE @r <= @ReportCount
                BEGIN
                    SET @ThoiGianCapNhat = DATEADD(HOUR, 4 + (@r * 10), DATEADD(DAY, @r - 1, @NgayBatDauCT));
                    IF @ThoiGianCapNhat > @MaxAllowed SET @ThoiGianCapNhat = DATEADD(HOUR, -(@ReportCount - @r + 2), @MaxAllowed);
                    IF @NgayHoanThanh IS NOT NULL AND @ThoiGianCapNhat > DATEADD(HOUR, -2, @NgayHoanThanh)
                        SET @ThoiGianCapNhat = DATEADD(HOUR, -(@ReportCount - @r + 4), @NgayHoanThanh);

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
                    IF @NgayHoanThanh IS NOT NULL AND @ThoiGianDuyet IS NOT NULL AND @ThoiGianDuyet > DATEADD(MINUTE, -20, @NgayHoanThanh)
                        SET @ThoiGianDuyet = DATEADD(MINUTE, -20, @NgayHoanThanh);
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
                            CONCAT(N'minh-chung-data9-', FORMAT(@BatchNo, '000'), N'-', FORMAT(@i, '00'), N'-', FORMAT(@d, '00'), N'-', FORMAT(@r, '00'), N'.pdf'),
                            CONCAT(N'/uploads/data10/', FORMAT(@BatchNo, '000'), N'/bc-', @MaTienDo, N'.pdf'),
                            CASE
                                WHEN @NgayHoanThanh IS NOT NULL AND DATEADD(MINUTE, 10, @ThoiGianCapNhat) > @NgayHoanThanh THEN DATEADD(MINUTE, -5, @NgayHoanThanh)
                                ELSE DATEADD(MINUTE, 10, @ThoiGianCapNhat)
                            END,
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
                CONCAT(N'Đề xuất phát sinh bị từ chối ', @i, N' - DATA10-', FORMAT(@BatchNo, '000')),
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
                CONCAT(N'Đề xuất công việc đang chờ duyệt ', @i, N' - DATA10-', FORMAT(@BatchNo, '000')),
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
        FROM @Member m
        WHERE BatchNo = @BatchNo
          AND NOT EXISTS
          (
              SELECT 1
              FROM dbo.THANH_VIEN_PHONG_CHAT tv
              WHERE tv.MaPhongChat = @MaPhongChat AND tv.MaNguoiDung = m.MaNguoiDung
          );

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

    IF (SELECT COUNT(*) FROM @DuAnMoi) <> 400
        INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
        VALUES
        (
            N'DATA10_COUNT',
            N'Số lượng dự án',
            N'DATA10',
            N'@DuAnMoi',
            N'COUNT',
            CONCAT(N'Số dự án DATA10 vừa tạo = ', (SELECT COUNT(*) FROM @DuAnMoi), N', mong đợi = 100.')
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
        N'PCCV_TIME',
        N'Phân công công việc',
        da.TenDuAn,
        N'PHAN_CONG_CONG_VIEC',
        CONCAT(N'MaNguoiDung=', pc.MaNguoiDung, N'; MaCongViec=', pc.MaCongViec),
        CONCAT(
            N'NgayGiaoViec=', COALESCE(CONVERT(NVARCHAR(30), pc.NgayGiaoViec, 126), N'NULL'),
            N'; NgayBatDauCongViec=', COALESCE(CONVERT(NVARCHAR(30), cv.NgayBatDau, 126), N'NULL'),
            N'. Phân công công việc phải trước hoặc đúng thời điểm bắt đầu công việc.'
        )
    FROM @CongViecMoi cv
    JOIN @DuAnMoi da ON da.BatchNo = cv.BatchNo
    JOIN dbo.PHAN_CONG_CONG_VIEC pc ON pc.MaCongViec = cv.MaCongViec
    WHERE pc.NgayGiaoViec > cv.NgayBatDau;

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
        N'Người được phân công chi tiết không thuộc NHAN_VIEN_DU_AN của đúng dự án DATA10.'
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
        N'DA_OPEN_NO_OPEN_WORK',
        N'Dự án chưa hoàn thành',
        da.TenDuAn,
        N'DU_AN/CONG_VIEC',
        CONCAT(N'MaDuAn=', da.MaDuAn),
        N'Dự án chưa hoàn thành nhưng không còn công việc chưa hoàn thành.'
    FROM @DuAnMoi da
    WHERE da.TrangThaiDuAn <> N'HoanThanh'
      AND NOT EXISTS
      (
          SELECT 1
          FROM @CongViecMoi cv
          WHERE cv.BatchNo = da.BatchNo
            AND cv.TrangThaiCongViec <> N'HoanThanh'
      );

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CV_OPEN_NO_OPEN_DETAIL',
        N'Công việc chưa hoàn thành',
        da.TenDuAn,
        N'CONG_VIEC/CT_CONG_VIEC',
        CONCAT(N'MaCongViec=', cv.MaCongViec),
        N'Công việc chưa hoàn thành nhưng tất cả chi tiết đều đã hoàn thành.'
    FROM @CongViecMoi cv
    JOIN @DuAnMoi da ON da.BatchNo = cv.BatchNo
    WHERE cv.TrangThaiCongViec <> N'HoanThanh'
      AND NOT EXISTS
      (
          SELECT 1
          FROM @ChiTietMoi ct
          WHERE ct.BatchNo = cv.BatchNo
            AND ct.WorkNo = cv.ThuTu
            AND ct.TrangThaiCTCV <> N'HoanThanh'
      );

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CV_OPEN_HAS_END_DATE',
        N'Công việc chưa hoàn thành',
        da.TenDuAn,
        N'CONG_VIEC',
        CONCAT(N'MaCongViec=', cv.MaCongViec),
        N'Công việc chưa hoàn thành nhưng có ngày kết thúc thực tế.'
    FROM @CongViecMoi cv
    JOIN @DuAnMoi da ON da.BatchNo = cv.BatchNo
    WHERE cv.TrangThaiCongViec <> N'HoanThanh'
      AND cv.NgayKetThucThucTe IS NOT NULL;

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CT_OPEN_HAS_END_DATE',
        N'Chi tiết chưa hoàn thành',
        da.TenDuAn,
        N'CT_CONG_VIEC',
        CONCAT(N'MaChiTietCV=', ct.MaChiTietCV),
        N'Chi tiết chưa hoàn thành nhưng có ngày kết thúc thực tế.'
    FROM @ChiTietMoi ct
    JOIN @DuAnMoi da ON da.BatchNo = ct.BatchNo
    WHERE ct.TrangThaiCTCV <> N'HoanThanh'
      AND ct.NgayKetThuc IS NOT NULL;

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

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'CHI_PHI_TOTAL',
        N'Chi phí và ngân sách',
        da.TenDuAn,
        N'CHI_PHI',
        CONCAT(N'MaDuAn=', da.MaDuAn),
        CONCAT(N'TongChiPhi=', CONVERT(NVARCHAR(50), x.TongChiPhi), N'; CostTotal=', CONVERT(NVARCHAR(50), cfg.CostTotal), N'. Tổng chi phí phải khớp cấu hình CostTotal của batch.')
    FROM @DuAnMoi da
    JOIN @Cfg cfg ON cfg.BatchNo = da.BatchNo
    CROSS APPLY
    (
        SELECT SUM(cp.SoTienDaChi) AS TongChiPhi
        FROM dbo.CHI_PHI cp
        JOIN @CongViecMoi cvm ON cvm.MaCongViec = cp.MaCongViec
        WHERE cvm.BatchNo = da.BatchNo
    ) x
    WHERE ABS(ISNULL(x.TongChiPhi, 0) - cfg.CostTotal) > 0.01;

    INSERT INTO @LoiKiemTra (MaLoi, NhomLoi, DuAn, BangDuLieu, KhoaBanGhi, NoiDung)
    SELECT
        N'DA_HOAN_THANH_KHONG_TRE',
        N'Dự án hoàn thành không trễ',
        da.TenDuAn,
        N'DU_AN',
        CONCAT(N'MaDuAn=', da.MaDuAn),
        CONCAT(
            N'NgayKetThucDuAn=',
            CONVERT(NVARCHAR(30), da.NgayKetThuc, 126),
            N'; NgayHoanThanhThucTe=',
            COALESCE(CONVERT(NVARCHAR(30), da.NgayHoanThanhThucTe, 126), N'NULL'),
            N'. Dự án thuộc batch DATA10 phải hoàn thành sau thời hạn kế hoạch.'
        )
    FROM @DuAnMoi da
    WHERE da.TrangThaiDuAn = N'HoanThanh'
      AND
      (
          da.NgayHoanThanhThucTe IS NULL
          OR da.NgayHoanThanhThucTe <= da.NgayKetThuc
      );

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
            N'DATA10',
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

        THROW 50003, N'Phát hiện lỗi logic trong batch DATA10. Xem danh sách lỗi phía trên.', 1;
    END;

    COMMIT TRANSACTION;

    /* =========================================================
       8. TRUY VAN KIEM TRA SAU COMMIT
       ========================================================= */

    SELECT COUNT(*) AS SoDuAnMoi
    FROM dbo.DU_AN
    WHERE TenDuAn LIKE N'DATA10-%';

    SELECT
        @SoDuAnTruoc AS SoDuAnTruoc,
        COUNT(*) AS SoDuAnSau,
        COUNT(*) - @SoDuAnTruoc AS ChenhLech
    FROM dbo.DU_AN
    WHERE ISNULL(IsDeleted, 0) = 0;

    SELECT
        da.MaDuAn,
        da.TenDuAn,
        nd.HoTenNguoiDung AS QuanLy,
        da.TrangThaiDuAn,
        da.NgayBatDauDuAn,
        da.NgayKetThucDuAn,
        da.NgayHoanThanhThucTeDuAn,
        DATEDIFF(DAY, da.NgayKetThucDuAn, COALESCE(da.NgayHoanThanhThucTeDuAn, @MaxAllowed)) AS SoNgayTreDuKien
    FROM dbo.DU_AN da
    JOIN dbo.NGUOI_DUNG nd ON nd.MaNguoiDung = da.MaNguoiDung
    WHERE da.TenDuAn LIKE N'DATA10-%'
    ORDER BY da.TenDuAn;

    ;WITH DuAnData9 AS
    (
        SELECT MaDuAn
        FROM dbo.DU_AN
        WHERE TenDuAn LIKE N'DATA10-%'
    ),
    TeamAgg AS
    (
        SELECT MaDuAn, COUNT(*) AS SoTeam
        FROM dbo.TEAM_DU_AN
        WHERE MaDuAn IN (SELECT MaDuAn FROM DuAnData9)
        GROUP BY MaDuAn
    ),
    MemberAgg AS
    (
        SELECT MaDuAn, COUNT(*) AS SoNhanVien
        FROM dbo.NHAN_VIEN_DU_AN
        WHERE MaDuAn IN (SELECT MaDuAn FROM DuAnData9)
        GROUP BY MaDuAn
    ),
    CategoryAgg AS
    (
        SELECT MaDuAn, COUNT(*) AS SoDanhMuc
        FROM dbo.DANH_MUC_CONG_VIEC
        WHERE MaDuAn IN (SELECT MaDuAn FROM DuAnData9) AND ISNULL(IsDeleted, 0) = 0
        GROUP BY MaDuAn
    ),
    WorkAgg AS
    (
        SELECT dm.MaDuAn, COUNT(*) AS SoCongViec
        FROM dbo.DANH_MUC_CONG_VIEC dm
        JOIN dbo.CONG_VIEC cv ON cv.MaDanhMucCV = dm.MaDanhMucCV
        WHERE dm.MaDuAn IN (SELECT MaDuAn FROM DuAnData9) AND ISNULL(dm.IsDeleted, 0) = 0 AND ISNULL(cv.IsDeleted, 0) = 0
        GROUP BY dm.MaDuAn
    ),
    DetailAgg AS
    (
        SELECT dm.MaDuAn, COUNT(*) AS SoChiTiet
        FROM dbo.DANH_MUC_CONG_VIEC dm
        JOIN dbo.CONG_VIEC cv ON cv.MaDanhMucCV = dm.MaDanhMucCV
        JOIN dbo.CT_CONG_VIEC ct ON ct.MaCongViec = cv.MaCongViec
        WHERE dm.MaDuAn IN (SELECT MaDuAn FROM DuAnData9) AND ISNULL(dm.IsDeleted, 0) = 0 AND ISNULL(cv.IsDeleted, 0) = 0 AND ISNULL(ct.IsDeleted, 0) = 0
        GROUP BY dm.MaDuAn
    ),
    DxcvAgg AS
    (
        SELECT MaDuAn,
               SUM(CASE WHEN TrangThaiCongViecDeXuat = N'DaDuyet' THEN 1 ELSE 0 END) AS DxcvDaDuyet,
               SUM(CASE WHEN TrangThaiCongViecDeXuat = N'TuChoi' THEN 1 ELSE 0 END) AS DxcvTuChoi,
               SUM(CASE WHEN TrangThaiCongViecDeXuat = N'ChoDuyet' THEN 1 ELSE 0 END) AS DxcvChoDuyet
        FROM dbo.DE_XUAT_CONG_VIEC
        WHERE MaDuAn IN (SELECT MaDuAn FROM DuAnData9) AND ISNULL(IsDeleted, 0) = 0
        GROUP BY MaDuAn
    ),
    DxnsAgg AS
    (
        SELECT MaDuAn,
               SUM(CASE WHEN TrangThaiDeXuat = N'DaDuyet' THEN 1 ELSE 0 END) AS DxnsDaDuyet,
               SUM(CASE WHEN TrangThaiDeXuat = N'TuChoi' THEN 1 ELSE 0 END) AS DxnsTuChoi,
               SUM(CASE WHEN TrangThaiDeXuat = N'ChoDuyet' THEN 1 ELSE 0 END) AS DxnsChoDuyet
        FROM dbo.DE_XUAT_NGAN_SACH
        WHERE MaDuAn IN (SELECT MaDuAn FROM DuAnData9) AND ISNULL(IsDeleted, 0) = 0
        GROUP BY MaDuAn
    ),
    ProgressAgg AS
    (
        SELECT dm.MaDuAn,
               SUM(CASE WHEN td.TrangThaiTienDo = N'DaDuyet' THEN 1 ELSE 0 END) AS BaoCaoDaDuyet,
               SUM(CASE WHEN td.TrangThaiTienDo = N'TuChoi' THEN 1 ELSE 0 END) AS BaoCaoTuChoi,
               SUM(CASE WHEN td.TrangThaiTienDo = N'YeuCauBoSung' THEN 1 ELSE 0 END) AS BaoCaoYeuCauBoSung,
               SUM(CASE WHEN td.TrangThaiTienDo = N'ChoDuyet' THEN 1 ELSE 0 END) AS BaoCaoChoDuyet,
               COUNT(f.MaFileTDCV) AS SoFile
        FROM dbo.DANH_MUC_CONG_VIEC dm
        JOIN dbo.CONG_VIEC cv ON cv.MaDanhMucCV = dm.MaDanhMucCV
        JOIN dbo.CT_CONG_VIEC ct ON ct.MaCongViec = cv.MaCongViec
        JOIN dbo.TIEN_DO_CONG_VIEC td ON td.MaChiTietCV = ct.MaChiTietCV
        LEFT JOIN dbo.FILE_TIEN_DO_CONG_VIEC f ON f.MaTienDo = td.MaTienDo AND ISNULL(f.IsDeleted, 0) = 0
        WHERE dm.MaDuAn IN (SELECT MaDuAn FROM DuAnData9)
        GROUP BY dm.MaDuAn
    ),
    StaffAgg AS
    (
        SELECT MaDuAn, COUNT(*) AS SoLanThayDoiNhanSu
        FROM dbo.NHAT_KY_PHU_TRACH_DU_AN
        WHERE MaDuAn IN (SELECT MaDuAn FROM DuAnData9)
          AND (NkHanhDongPTDA LIKE N'Thêm nhân sự%' OR NkHanhDongPTDA LIKE N'Điều chuyển nhân sự%' OR NkHanhDongPTDA LIKE N'Cập nhật vai trò%')
        GROUP BY MaDuAn
    ),
    ManagerAgg AS
    (
        SELECT MaDuAn, COUNT(*) AS SoLanDoiQuanLy
        FROM dbo.YEU_CAU_DOI_QUAN_LY
        WHERE MaDuAn IN (SELECT MaDuAn FROM DuAnData9)
          AND TrangThaiYeuCauDoiQuanLy = N'DaDuyet'
          AND MaQuanLyHienTai <> MaQuanLyDeXuat
        GROUP BY MaDuAn
    ),
    BudgetAgg AS
    (
        SELECT ns.MaDuAn, SUM(ns.[NganSach]) AS NganSach, SUM(ISNULL(cp.SoTienDaChi, 0)) AS TongChiPhi
        FROM dbo.NGAN_SACH ns
        LEFT JOIN dbo.CHI_PHI cp ON cp.MaNganSach = ns.MaNganSach AND ISNULL(cp.IsDeleted, 0) = 0
        WHERE ns.MaDuAn IN (SELECT MaDuAn FROM DuAnData9) AND ISNULL(ns.IsDeleted, 0) = 0
        GROUP BY ns.MaDuAn
    )
    SELECT
        da.MaDuAn,
        da.TenDuAn,
        ISNULL(t.SoTeam, 0) AS SoTeam,
        ISNULL(m.SoNhanVien, 0) AS SoNhanVien,
        ISNULL(ca.SoDanhMuc, 0) AS SoDanhMuc,
        ISNULL(w.SoCongViec, 0) AS SoCongViec,
        ISNULL(d.SoChiTiet, 0) AS SoChiTiet,
        ISNULL(dx.DxcvDaDuyet, 0) AS DxcvDaDuyet,
        ISNULL(dx.DxcvTuChoi, 0) AS DxcvTuChoi,
        ISNULL(dx.DxcvChoDuyet, 0) AS DxcvChoDuyet,
        ISNULL(nsx.DxnsDaDuyet, 0) AS DxnsDaDuyet,
        ISNULL(nsx.DxnsTuChoi, 0) AS DxnsTuChoi,
        ISNULL(nsx.DxnsChoDuyet, 0) AS DxnsChoDuyet,
        ISNULL(p.BaoCaoDaDuyet, 0) AS BaoCaoDaDuyet,
        ISNULL(p.BaoCaoTuChoi, 0) AS BaoCaoTuChoi,
        ISNULL(p.BaoCaoYeuCauBoSung, 0) AS BaoCaoYeuCauBoSung,
        ISNULL(p.BaoCaoChoDuyet, 0) AS BaoCaoChoDuyet,
        ISNULL(p.SoFile, 0) AS SoFile,
        ISNULL(sa.SoLanThayDoiNhanSu, 0) AS SoLanThayDoiNhanSu,
        ISNULL(ma.SoLanDoiQuanLy, 0) AS SoLanDoiQuanLy,
        ISNULL(ba.NganSach, 0) AS NganSach,
        ISNULL(ba.TongChiPhi, 0) AS TongChiPhi
    FROM dbo.DU_AN da
    LEFT JOIN TeamAgg t ON t.MaDuAn = da.MaDuAn
    LEFT JOIN MemberAgg m ON m.MaDuAn = da.MaDuAn
    LEFT JOIN CategoryAgg ca ON ca.MaDuAn = da.MaDuAn
    LEFT JOIN WorkAgg w ON w.MaDuAn = da.MaDuAn
    LEFT JOIN DetailAgg d ON d.MaDuAn = da.MaDuAn
    LEFT JOIN DxcvAgg dx ON dx.MaDuAn = da.MaDuAn
    LEFT JOIN DxnsAgg nsx ON nsx.MaDuAn = da.MaDuAn
    LEFT JOIN ProgressAgg p ON p.MaDuAn = da.MaDuAn
    LEFT JOIN StaffAgg sa ON sa.MaDuAn = da.MaDuAn
    LEFT JOIN ManagerAgg ma ON ma.MaDuAn = da.MaDuAn
    LEFT JOIN BudgetAgg ba ON ba.MaDuAn = da.MaDuAn
    WHERE da.TenDuAn LIKE N'DATA10-%'
    ORDER BY da.TenDuAn;

    SELECT MIN(v.Ngay) AS NgayNhoNhatTrongBatch, MAX(v.Ngay) AS NgayLonNhatTrongBatch, @MaxAllowed AS GioiHanNgay
    FROM
    (
        SELECT d.NgayTaoDuAn AS Ngay FROM dbo.DU_AN d WHERE d.TenDuAn LIKE N'DATA10-%'
        UNION ALL SELECT d.NgayBatDauDuAn FROM dbo.DU_AN d WHERE d.TenDuAn LIKE N'DATA10-%'
        UNION ALL SELECT d.NgayKetThucDuAn FROM dbo.DU_AN d WHERE d.TenDuAn LIKE N'DATA10-%'
        UNION ALL SELECT d.NgayHoanThanhThucTeDuAn FROM dbo.DU_AN d WHERE d.TenDuAn LIKE N'DATA10-%'
        UNION ALL SELECT td.ThoiGianCapNhat FROM dbo.TIEN_DO_CONG_VIEC td JOIN dbo.CT_CONG_VIEC ct ON ct.MaChiTietCV = td.MaChiTietCV JOIN dbo.CONG_VIEC cv ON cv.MaCongViec = ct.MaCongViec JOIN dbo.DANH_MUC_CONG_VIEC dm ON dm.MaDanhMucCV = cv.MaDanhMucCV JOIN dbo.DU_AN d ON d.MaDuAn = dm.MaDuAn WHERE d.TenDuAn LIKE N'DATA10-%'
        UNION ALL SELECT td.ThoiGianDuyet FROM dbo.TIEN_DO_CONG_VIEC td JOIN dbo.CT_CONG_VIEC ct ON ct.MaChiTietCV = td.MaChiTietCV JOIN dbo.CONG_VIEC cv ON cv.MaCongViec = ct.MaCongViec JOIN dbo.DANH_MUC_CONG_VIEC dm ON dm.MaDanhMucCV = cv.MaDanhMucCV JOIN dbo.DU_AN d ON d.MaDuAn = dm.MaDuAn WHERE d.TenDuAn LIKE N'DATA10-%'
        UNION ALL SELECT f.NgayUploadFileTDCV FROM dbo.FILE_TIEN_DO_CONG_VIEC f JOIN dbo.TIEN_DO_CONG_VIEC td ON td.MaTienDo = f.MaTienDo JOIN dbo.CT_CONG_VIEC ct ON ct.MaChiTietCV = td.MaChiTietCV JOIN dbo.CONG_VIEC cv ON cv.MaCongViec = ct.MaCongViec JOIN dbo.DANH_MUC_CONG_VIEC dm ON dm.MaDanhMucCV = cv.MaDanhMucCV JOIN dbo.DU_AN d ON d.MaDuAn = dm.MaDuAn WHERE d.TenDuAn LIKE N'DATA10-%'
    ) v
    WHERE v.Ngay IS NOT NULL;

    SELECT N'Không có lệnh ghi dữ liệu vào nhóm bảng phân tích AI trong script DATA10.' AS XacNhanBangAI;
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
   CANH BAO: NHOM BANG AI KHONG THUOC PHAM VI SCRIPT DATA10:
   AI_DATASET, AI_KET_QUA, AI_NGUYEN_NHAN, AI_MODEL, DM_NGUYEN_NHAN.
   ========================================================= */
