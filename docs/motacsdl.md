# 4.3 PHÂN TÍCH VÀ THIẾT KẾ CƠ SỞ DỮ LIỆU

## 4.3.1 Phân tích yêu cầu dữ liệu

Cơ sở dữ liệu của hệ thống quản lý dự án được xây dựng để lưu trữ thông tin về người dùng, phân quyền, dự án, công việc, tiến độ, ngân sách, chi phí, đánh giá, trao đổi và dữ liệu phục vụ phân tích nguyên nhân trễ bằng AI. Các thực thể được tổ chức theo từng nhóm chức năng để dữ liệu dễ quản lý, dễ liên kết và hỗ trợ đầy đủ các nghiệp vụ chính trong hệ thống.

### Nhóm người dùng và phân quyền

#### 4.3.1.1 Thực thể Người dùng (NGUOI_DUNG)

Thực thể dữ liệu "NGUOI_DUNG" được dùng để lưu thông tin hồ sơ của nhân sự sử dụng hệ thống. Bảng "NGUOI_DUNG" có khóa chính là "MaNguoiDung", dùng để định danh duy nhất từng người dùng trong các nghiệp vụ quản lý dự án.

Một số thuộc tính quan trọng của bảng gồm "HoTenNguoiDung", "DiaChiNguoiDung", "SdtNguoiDung", "NgaySinh" và "AnhDaiDien". Bảng còn có cột "Id" để liên kết với tài khoản đăng nhập trong bảng "AspNetUsers", đồng thời có "MaChucDanh" để xác định chức danh của người dùng thông qua bảng "CHUC_DANH".

"NGUOI_DUNG" là thực thể trung tâm của nhóm dữ liệu nhân sự. Nhiều bảng khác như "DU_AN", "NHAN_VIEN_DU_AN", "PHAN_CONG_CONG_VIEC", "TIEN_DO_CONG_VIEC", "DANH_GIA_NHAN_VIEN" và các bảng duyệt đều liên kết đến bảng này để xác định người quản lý, người thực hiện, người đề xuất hoặc người duyệt. Bảng có hỗ trợ đánh dấu xóa mềm thông qua các cột "IsDeleted", "DeletedAt" và "DeletedBy".

#### 4.3.1.2 Thực thể Chức danh (CHUC_DANH)

Thực thể dữ liệu "CHUC_DANH" được dùng để lưu danh mục chức danh của nhân sự trong hệ thống. Bảng "CHUC_DANH" có khóa chính là "MaChucDanh".

Các thuộc tính chính của bảng gồm "TenChucDanh" và "MoTaChucDanh". Dữ liệu trong bảng này giúp phân loại người dùng theo vị trí công việc hoặc vai trò tổ chức, từ đó hỗ trợ việc quản lý hồ sơ nhân sự và hiển thị thông tin người dùng rõ ràng hơn.

#### 4.3.1.3 Thực thể Tài khoản (AspNetUsers)

Thực thể dữ liệu "AspNetUsers" được dùng để lưu thông tin tài khoản đăng nhập theo cơ chế Identity của ASP.NET. Bảng "AspNetUsers" có khóa chính là "Id", dùng để định danh duy nhất từng tài khoản trong hệ thống xác thực.

Các thuộc tính quan trọng của bảng gồm "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "PasswordHash", "PhoneNumber", "LockoutEnd", "LockoutEnabled" và "AccessFailedCount". Bảng có cột "MaNguoiDung" liên kết với bảng "NGUOI_DUNG", giúp nối dữ liệu tài khoản đăng nhập với hồ sơ nhân sự trong hệ thống.

Vai trò của bảng "AspNetUsers" là phục vụ đăng nhập, bảo mật tài khoản và quản lý trạng thái truy cập. Nhờ liên kết với "NGUOI_DUNG", hệ thống có thể vừa xử lý xác thực theo Identity, vừa sử dụng thông tin nhân sự trong các nghiệp vụ quản lý dự án.

#### 4.3.1.4 Thực thể Vai trò (AspNetRoles)

Thực thể dữ liệu "AspNetRoles" được dùng để lưu các vai trò hệ thống theo ASP.NET Identity. Bảng "AspNetRoles" có khóa chính là "Id".

Các thuộc tính chính gồm "Name", "NormalizedName" và "ConcurrencyStamp". Bảng này là cơ sở để phân nhóm tài khoản theo vai trò như quản trị, quản lý hoặc nhân viên, sau đó kết hợp với các bảng phân quyền để xác định phạm vi chức năng được sử dụng.

#### 4.3.1.5 Thực thể Tài khoản - Vai trò (AspNetUserRoles)

Thực thể dữ liệu "AspNetUserRoles" được dùng để lưu quan hệ giữa tài khoản và vai trò. Bảng có khóa chính ghép gồm "Asp_Id" và "Id", trong đó "Asp_Id" liên kết với "AspNetUsers", còn "Id" liên kết với "AspNetRoles".

Vai trò của bảng này là xác định một tài khoản đang thuộc vai trò nào trong hệ thống. Đây là bảng trung gian quan trọng để hệ thống áp dụng quyền truy cập theo vai trò cho từng người dùng.

#### 4.3.1.6 Thực thể Claim người dùng (AspNetUserClaims)

Thực thể dữ liệu "AspNetUserClaims" được dùng để lưu các claim gắn trực tiếp với tài khoản người dùng. Bảng có khóa chính là "Id" và có cột "Asp_Id" liên kết với bảng "AspNetUsers".

Các thuộc tính "ClaimType" và "ClaimValue" cho biết loại claim và giá trị claim của người dùng. Bảng này hỗ trợ mở rộng thông tin phân quyền hoặc xác thực riêng cho từng tài khoản khi cần.

#### 4.3.1.7 Thực thể Claim vai trò (AspNetRoleClaims)

Thực thể dữ liệu "AspNetRoleClaims" được dùng để lưu các quyền được gắn cho vai trò. Bảng có khóa chính là "Id", có "Asp_Id" liên kết với "AspNetRoles" và "MaDanhMucQuyen" liên kết với "DANH_MUC_QUYEN".

Các thuộc tính "ClaimType" và "ClaimValue" thể hiện thông tin quyền được cấp cho vai trò. Trong hệ thống, bảng này là nơi kết nối vai trò Identity với danh mục quyền nghiệp vụ, giúp việc phân quyền theo vai trò được quản lý rõ ràng và có thể mở rộng.

#### 4.3.1.8 Thực thể Đăng nhập ngoài (AspNetUserLogins)

Thực thể dữ liệu "AspNetUserLogins" được dùng để lưu thông tin đăng nhập từ nhà cung cấp bên ngoài nếu hệ thống sử dụng. Bảng có khóa chính ghép gồm "LoginProvider" và "ProviderKey", đồng thời có cột "Id" liên kết với "AspNetUsers".

Các thuộc tính như "ProviderDisplayName" giúp mô tả nguồn đăng nhập. Đây là bảng phụ của Identity, phục vụ nhu cầu mở rộng hình thức xác thực tài khoản.

#### 4.3.1.9 Thực thể Token người dùng (AspNetUserTokens)

Thực thể dữ liệu "AspNetUserTokens" được dùng để lưu token gắn với tài khoản người dùng. Bảng có khóa chính ghép gồm "Id", "LoginProvider" và "Name"; trong đó "Id" liên kết với bảng "AspNetUsers".

Thuộc tính "Value" lưu giá trị token. Bảng này là bảng phụ của Identity, hỗ trợ các nghiệp vụ xác thực hoặc bảo mật tài khoản như lưu token theo nhà cung cấp đăng nhập.

#### 4.3.1.10 Thực thể Danh mục màn hình (DANH_MUC_MAN_HINH)

Thực thể dữ liệu "DANH_MUC_MAN_HINH" được dùng để lưu danh sách các màn hình hoặc nhóm chức năng trong hệ thống. Bảng có khóa chính là "MaManHinh".

Các thuộc tính quan trọng gồm "TenManHinh" và "MoTaManHinh". Bảng này liên kết với "DANH_MUC_QUYEN" để tổ chức quyền theo từng màn hình, giúp việc hiển thị và quản lý phân quyền trở nên dễ hiểu hơn.

#### 4.3.1.11 Thực thể Danh mục quyền (DANH_MUC_QUYEN)

Thực thể dữ liệu "DANH_MUC_QUYEN" được dùng để lưu danh sách các quyền nghiệp vụ của hệ thống. Bảng có khóa chính là "MaDanhMucQuyen" và có cột "MaManHinh" liên kết với bảng "DANH_MUC_MAN_HINH".

Các thuộc tính chính gồm "TenDanhMucQuyen" và "MoTaDanhMucQuyen". Bảng này đóng vai trò danh mục chuẩn cho các quyền như xem, thêm, sửa, xóa hoặc duyệt ở từng chức năng. Khi kết hợp với "AspNetRoleClaims", hệ thống có thể cấp quyền cho từng vai trò một cách nhất quán.

### Nhóm quản lý dự án

#### 4.3.1.12 Thực thể Dự án (DU_AN)

Thực thể dữ liệu "DU_AN" được dùng để lưu trữ thông tin chính của các dự án trong hệ thống. Bảng "DU_AN" có khóa chính là "MaDuAn", dùng để định danh duy nhất cho từng dự án.

Các thông tin quan trọng của dự án gồm "TenDuAn", "MoTaDuAn", "NgayTaoDuAn", "NgayBatDauDuAn", "NgayKetThucDuAn", "NgayHoanThanhThucTeDuAn", "PhanTramHoanThanh", "TrangThaiDuAn" và "GhiChuDuAn". Bảng có "MaLoaiDuAn" liên kết với "LOAI_DU_AN" để xác định loại dự án, đồng thời có "MaNguoiDung" liên kết với "NGUOI_DUNG" để xác định người quản lý dự án.

Dữ liệu trong bảng "DU_AN" là trung tâm của hệ thống, được liên kết với nhiều nhóm dữ liệu khác như thành viên dự án, team, công việc, ngân sách, chi phí, đánh giá, trao đổi, nhật ký và dữ liệu phục vụ AI. Bảng có hỗ trợ xóa mềm thông qua "IsDeleted", "DeletedAt" và "DeletedBy" để lưu trạng thái xóa mà không làm mất dữ liệu liên quan.

#### 4.3.1.13 Thực thể Loại dự án (LOAI_DU_AN)

Thực thể dữ liệu "LOAI_DU_AN" được dùng để lưu danh mục loại dự án. Bảng có khóa chính là "MaLoaiDuAn".

Các thuộc tính chính gồm "TenLoai" và "MoTaLoaiDuAn". Dữ liệu trong bảng này giúp phân loại dự án theo nhóm nghiệp vụ hoặc tính chất thực hiện, từ đó hỗ trợ việc quản lý và thống kê dự án.

#### 4.3.1.14 Thực thể Nhân viên dự án (NHAN_VIEN_DU_AN)

Thực thể dữ liệu "NHAN_VIEN_DU_AN" được dùng để lưu danh sách người dùng tham gia vào từng dự án. Bảng có khóa chính ghép gồm "MaDuAn" và "MaNguoiDung".

Bảng liên kết "DU_AN" với "NGUOI_DUNG", đồng thời lưu thêm "NgayThamGiaDuAn" và "VaiTroTrongDuAn". Nhờ bảng này, hệ thống xác định được một dự án có những thành viên nào và từng thành viên đảm nhận vai trò gì trong dự án.

#### 4.3.1.15 Thực thể Team (TEAM)

Thực thể dữ liệu "TEAM" được dùng để lưu thông tin các nhóm làm việc trong hệ thống. Bảng có khóa chính là "MaTeam".

Các thuộc tính quan trọng gồm "TenTeam", "MoTaTeam", "NgayLapTeam" và "TrangThaiTeam". Bảng này hỗ trợ tổ chức nhân sự theo nhóm, từ đó có thể phân bổ team vào dự án hoặc quản lý thành viên trong team. Bảng có hỗ trợ xóa mềm bằng "IsDeleted", "DeletedAt" và "DeletedBy".

#### 4.3.1.16 Thực thể Nhân viên team (NHAN_VIEN_TEAM)

Thực thể dữ liệu "NHAN_VIEN_TEAM" được dùng để lưu thành viên thuộc từng team. Bảng có khóa chính ghép gồm "MaNguoiDung" và "MaTeam".

Bảng liên kết "NGUOI_DUNG" với "TEAM", đồng thời lưu "VaiTroTrongTeam", "NgayThamGiaTeam" và "IsLeader". Dữ liệu này giúp hệ thống quản lý cơ cấu nhân sự trong team, bao gồm thành viên thông thường và người đứng đầu team.

#### 4.3.1.17 Thực thể Team dự án (TEAM_DU_AN)

Thực thể dữ liệu "TEAM_DU_AN" được dùng để lưu quan hệ giữa team và dự án. Bảng có khóa chính ghép gồm "MaTeam" và "MaDuAn".

Bảng liên kết "TEAM" với "DU_AN" và lưu thêm "NgayTeamThamGiaDA". Thực thể này cho phép một dự án có thể được nhiều team tham gia và một team cũng có thể tham gia nhiều dự án khác nhau.

#### 4.3.1.18 Thực thể Yêu cầu đổi quản lý (YEU_CAU_DOI_QUAN_LY)

Thực thể dữ liệu "YEU_CAU_DOI_QUAN_LY" được dùng để lưu các yêu cầu thay đổi người quản lý dự án. Bảng có khóa chính là "MaYeuCauDoiQuanLy".

Các thuộc tính quan trọng gồm "MaDuAn", "MaQuanLyHienTai", "MaQuanLyDeXuat", "MaNguoiDungDuyet", "TrangThaiYeuCauDoiQuanLy", "NgayTaoYeuCauDoiQuanLy" và "NgayDuyetYeuCauDoiQuanLy". Bảng liên kết với "DU_AN" để xác định dự án được yêu cầu đổi quản lý và liên kết nhiều lần với "NGUOI_DUNG" để xác định quản lý hiện tại, quản lý đề xuất và người duyệt.

Vai trò của bảng này là lưu lại quá trình đề xuất và xét duyệt thay đổi người phụ trách dự án. Bảng có hỗ trợ xóa mềm, giúp hệ thống giữ được lịch sử xử lý ngay cả khi bản ghi không còn hiển thị trong nghiệp vụ thông thường.

#### 4.3.1.19 Thực thể File dự án (FILE_DU_AN)

Thực thể dữ liệu "FILE_DU_AN" được dùng để lưu thông tin tệp tin đính kèm của dự án. Bảng có khóa chính là "MaFileDA" và có "MaDuAn" liên kết với "DU_AN".

Các thuộc tính chính gồm "TenFileDA", "DuongDanFileDA" và "NgayUploadFileDA". Bảng này giúp hệ thống quản lý tài liệu, minh chứng hoặc tệp liên quan đến từng dự án, đồng thời có hỗ trợ xóa mềm nếu cần ẩn file khỏi giao diện sử dụng.

### Nhóm quản lý công việc và tiến độ

#### 4.3.1.20 Thực thể Danh mục công việc (DANH_MUC_CONG_VIEC)

Thực thể dữ liệu "DANH_MUC_CONG_VIEC" được dùng để lưu các nhóm hoặc danh mục công việc trong từng dự án. Bảng có khóa chính là "MaDanhMucCV" và có "MaDuAn" liên kết với "DU_AN".

Các thuộc tính quan trọng gồm "TenDanhMucCV", "MoTaDanhMucCV" và "NgayTaoDMCV". Bảng này giúp phân nhóm công việc theo từng dự án, tạo nền tảng để quản lý các công việc chi tiết một cách có tổ chức. Bảng có hỗ trợ xóa mềm.

#### 4.3.1.21 Thực thể Công việc (CONG_VIEC)

Thực thể dữ liệu "CONG_VIEC" được dùng để lưu thông tin các công việc chính cần thực hiện trong dự án. Bảng có khóa chính là "MaCongViec".

Các thuộc tính quan trọng gồm "TenCongViec", "MoTaCongViec", "NgayBatDauCongViec", "NgayKetThucCVDuKien", "NgayKetThucCVThucTe", "NgayTaoCongViec" và "TrangThaiCongViec". Bảng liên kết với "DANH_MUC_CONG_VIEC" thông qua "MaDanhMucCV", liên kết với "MUC_DO_UU_TIEN" thông qua "MaMucDo" và có thể liên kết với "DE_XUAT_CONG_VIEC" thông qua "MaDeXuatCV" nếu công việc được tạo từ một đề xuất.

"CONG_VIEC" là bảng quan trọng trong quá trình theo dõi thực hiện dự án. Dữ liệu công việc được sử dụng để phân công nhân sự, chia thành chi tiết công việc, ghi nhận tiến độ, phát sinh chi phí và phục vụ đánh giá nhân viên. Bảng có hỗ trợ xóa mềm để bảo toàn dữ liệu liên quan.

#### 4.3.1.22 Thực thể Chi tiết công việc (CT_CONG_VIEC)

Thực thể dữ liệu "CT_CONG_VIEC" được dùng để lưu các đầu việc chi tiết thuộc một công việc chính. Bảng có khóa chính là "MaChiTietCV" và có "MaCongViec" liên kết với "CONG_VIEC".

Các thuộc tính quan trọng gồm "TenCTCV", "NoiDungChiTietCV", "NgayTaoCTCV", "NgayBatDauCTCV", "NgayKetThucCTCV" và "TrangThaiCTCV". Việc tách chi tiết công việc giúp hệ thống theo dõi tiến độ ở mức nhỏ hơn, phù hợp với hoạt động cập nhật tiến độ và phân công cụ thể cho từng nhân sự.

Bảng "CT_CONG_VIEC" có liên kết với các bảng như "PHAN_CONG_CT_CONG_VIEC", "TIEN_DO_CONG_VIEC", "FILE_CT_CONG_VIEC" và "NHAT_KY_PHAN_CONG_CT_CONG_VIEC". Bảng có hỗ trợ xóa mềm để tránh mất lịch sử tiến độ hoặc phân công liên quan.

#### 4.3.1.23 Thực thể Mức độ ưu tiên (MUC_DO_UU_TIEN)

Thực thể dữ liệu "MUC_DO_UU_TIEN" được dùng để lưu danh mục mức độ ưu tiên của công việc. Bảng có khóa chính là "MaMucDo".

Các thuộc tính chính gồm "TenMucDo" và "MoTaMucDo". Bảng này được liên kết với "CONG_VIEC" và "DE_XUAT_CONG_VIEC", giúp hệ thống nhận biết mức độ quan trọng hoặc độ khẩn cấp của công việc.

#### 4.3.1.24 Thực thể Phân công công việc (PHAN_CONG_CONG_VIEC)

Thực thể dữ liệu "PHAN_CONG_CONG_VIEC" được dùng để lưu việc phân công người dùng vào công việc chính. Bảng có khóa chính ghép gồm "MaNguoiDung" và "MaCongViec".

Bảng liên kết "NGUOI_DUNG" với "CONG_VIEC" và lưu thêm "NgayGiaoViec". Nhờ bảng này, hệ thống biết được công việc nào được giao cho nhân sự nào, từ đó phục vụ theo dõi trách nhiệm và đánh giá kết quả thực hiện.

#### 4.3.1.25 Thực thể Phân công chi tiết công việc (PHAN_CONG_CT_CONG_VIEC)

Thực thể dữ liệu "PHAN_CONG_CT_CONG_VIEC" được dùng để lưu việc phân công người dùng vào từng chi tiết công việc. Bảng có khóa chính ghép gồm "MaNguoiDung" và "MaChiTietCV".

Bảng liên kết "NGUOI_DUNG" với "CT_CONG_VIEC", đồng thời lưu "NgayGiaoCTCV" và "VaiTroTrongCTCV". Dữ liệu này giúp quản lý phân công ở mức chi tiết, đặc biệt khi một công việc chính được chia thành nhiều phần nhỏ cho nhiều người cùng tham gia.

#### 4.3.1.26 Thực thể Tiến độ công việc (TIEN_DO_CONG_VIEC)

Thực thể dữ liệu "TIEN_DO_CONG_VIEC" được dùng để lưu các lần cập nhật tiến độ của chi tiết công việc. Bảng có khóa chính là "MaTienDo".

Các thuộc tính quan trọng gồm "MaChiTietCV", "MaNguoiDung", "MaNguoiDungDuyet", "ThoiGianCapNhat", "PhanTram", "GhiChuTienDo", "TrangThaiCTCVDeXuat", "TrangThaiTienDo", "ThoiGianDuyet" và "GhiChuDuyet". Bảng liên kết với "CT_CONG_VIEC" để xác định đầu việc được báo cáo, liên kết với "NGUOI_DUNG" để xác định người báo cáo và người duyệt tiến độ.

"TIEN_DO_CONG_VIEC" là thực thể quan trọng để theo dõi quá trình thực hiện công việc theo thời gian. Dữ liệu trong bảng này còn được sử dụng để đánh giá tình trạng chậm tiến độ, kiểm soát phê duyệt báo cáo và tổng hợp dữ liệu phục vụ phân tích AI.

#### 4.3.1.27 Thực thể File công việc (FILE_CONG_VIEC)

Thực thể dữ liệu "FILE_CONG_VIEC" được dùng để lưu tệp tin đính kèm của công việc chính. Bảng có khóa chính là "MaFileCV" và có "MaCongViec" liên kết với "CONG_VIEC".

Các thuộc tính chính gồm "TenFileCV", "DuongDanFileCV" và "NgayUploadFileCV". Bảng giúp lưu tài liệu hoặc minh chứng liên quan đến công việc và có hỗ trợ xóa mềm.

#### 4.3.1.28 Thực thể File chi tiết công việc (FILE_CT_CONG_VIEC)

Thực thể dữ liệu "FILE_CT_CONG_VIEC" được dùng để lưu tệp tin đính kèm của chi tiết công việc. Bảng có khóa chính là "MaFileCTCV" và có "MaChiTietCV" liên kết với "CT_CONG_VIEC".

Các thuộc tính quan trọng gồm "TenFileCTCV", "DuongDanFileCTCV" và "NgayUploadFileCTCV". Bảng này giúp người dùng lưu lại tài liệu, hình ảnh hoặc minh chứng cho từng đầu việc chi tiết; bảng cũng có hỗ trợ xóa mềm.

#### 4.3.1.29 Thực thể File tiến độ công việc (FILE_TIEN_DO_CONG_VIEC)

Thực thể dữ liệu "FILE_TIEN_DO_CONG_VIEC" được dùng để lưu tệp tin minh chứng cho từng lần cập nhật tiến độ. Bảng có khóa chính là "MaFileTDCV" và có "MaTienDo" liên kết với "TIEN_DO_CONG_VIEC".

Các thuộc tính chính gồm "TenFileTDCV", "DuongDanFileTDCV" và "NgayUploadFileTDCV". Bảng này giúp bổ sung bằng chứng cho báo cáo tiến độ và có hỗ trợ xóa mềm khi cần.

### Nhóm ngân sách và chi phí

#### 4.3.1.30 Thực thể Ngân sách (NGAN_SACH)

Thực thể dữ liệu "NGAN_SACH" được dùng để lưu thông tin ngân sách của dự án. Bảng có khóa chính là "MaNganSach".

Các thuộc tính quan trọng gồm "MaDuAn", "NganSach", "Version", "IsActive", "MoTaNganSach", "NgayCapNhatNganSach", "NgayDuyetNganSach" và "TrangThaiNganSach". Bảng liên kết với "DU_AN" để xác định ngân sách thuộc dự án nào, đồng thời liên kết với "NGUOI_DUNG" thông qua "MaNguoiDungDeXuat" và "MaNguoiDungDuyet" để xác định người đề xuất và người duyệt.

"NGAN_SACH" là bảng quan trọng trong việc kiểm soát nguồn lực tài chính của dự án. Các cột "Version" và "IsActive" hỗ trợ theo dõi phiên bản ngân sách đang có hiệu lực, trong khi trạng thái ngân sách cho biết quá trình đề xuất hoặc phê duyệt. Bảng có hỗ trợ xóa mềm.

#### 4.3.1.31 Thực thể Chi phí (CHI_PHI)

Thực thể dữ liệu "CHI_PHI" được dùng để lưu các khoản chi phát sinh trong quá trình thực hiện công việc. Bảng có khóa chính là "MaChiPhi".

Các thuộc tính quan trọng gồm "MaCongViec", "MaNganSach", "NoiDungChiPhi", "SoTienDaChi", "NgayChi" và "TrangThaiChiPhi". Bảng liên kết với "CONG_VIEC" để xác định khoản chi thuộc công việc nào, đồng thời liên kết với "NGAN_SACH" để kiểm soát khoản chi trong phạm vi ngân sách tương ứng.

Vai trò của bảng "CHI_PHI" là hỗ trợ theo dõi chi phí thực tế, đối chiếu với ngân sách và cung cấp dữ liệu cho thống kê tài chính của dự án. Bảng có hỗ trợ xóa mềm, giúp giữ lại dữ liệu lịch sử nếu cần kiểm tra sau này.

#### 4.3.1.32 Thực thể Đề xuất ngân sách (DE_XUAT_NGAN_SACH)

Thực thể dữ liệu "DE_XUAT_NGAN_SACH" được dùng để lưu các đề xuất thay đổi hoặc bổ sung ngân sách cho dự án. Bảng có khóa chính là "MaDeXuatNS".

Các thuộc tính quan trọng gồm "MaDuAn", "MaNganSachCu", "NganSachCu", "NganSachDeXuat", "LyDoDeXuat", "MaNguoiDungDeXuat", "MaNguoiDungDuyet", "NgayDeXuat", "NgayDuyet" và "TrangThaiDeXuat". Bảng liên kết với "DU_AN", "NGAN_SACH" và "NGUOI_DUNG" để lưu đầy đủ thông tin dự án, ngân sách cũ, người đề xuất và người duyệt. Bảng có hỗ trợ xóa mềm.

#### 4.3.1.33 Thực thể Nhật ký ngân sách (NHAT_KY_NGAN_SACH)

Thực thể dữ liệu "NHAT_KY_NGAN_SACH" được dùng để lưu lịch sử thay đổi ngân sách. Bảng có khóa chính là "MaNhatKyNS" và liên kết với "NGAN_SACH" qua "MaNganSach", liên kết với "DU_AN" qua "MaDuAn".

Các thuộc tính như "SoTienNKNS", "NganSachTruoc", "NganSachSau", "NkNgayCapNhatNS", "NkTrangThaiNganSach", "HanhDongNKNS" và "ThoiGianNKNS" giúp ghi lại nội dung thay đổi ngân sách theo thời gian.

#### 4.3.1.34 Thực thể Nhật ký chi phí (NHAT_KY_CHI_PHI)

Thực thể dữ liệu "NHAT_KY_CHI_PHI" được dùng để lưu lịch sử thay đổi của các khoản chi phí. Bảng có khóa chính là "MaNhatKyCP" và liên kết với "CONG_VIEC" qua "MaCongViec", liên kết với "CHI_PHI" qua "MaChiPhi".

Các thuộc tính chính gồm "NkSoTienDaChi", "NkNgayChi", "NkTrangThaiChiPhi", "HanhDongNKCP" và "ThoiGianNKCP". Bảng này giúp hệ thống theo dõi các thao tác phát sinh liên quan đến chi phí, phục vụ kiểm tra và đối chiếu dữ liệu.

### Nhóm đề xuất và duyệt

#### 4.3.1.35 Thực thể Đề xuất công việc (DE_XUAT_CONG_VIEC)

Thực thể dữ liệu "DE_XUAT_CONG_VIEC" được dùng để lưu các đề xuất tạo mới hoặc bổ sung công việc trong dự án. Bảng có khóa chính là "MaDeXuatCV".

Các thuộc tính quan trọng gồm "MaDuAn", "MaDanhMucCV", "MaMucDo", "MaNguoiDungDeXuat", "MaNguoiDungDuyet", "TenCongViecDeXuat", "MoTaCongViecDeXuat", "ChiPhiDeXuat", "NgayBatDauCongViecDeXuat", "NgayKetThucCVDeXuatDuKien", "NgayDeXuatCongViec", "NgayDuyetDeXuatCongViec" và "TrangThaiCongViecDeXuat". Bảng liên kết với "DU_AN", "DANH_MUC_CONG_VIEC", "MUC_DO_UU_TIEN" và "NGUOI_DUNG".

Vai trò của bảng này là ghi nhận nhu cầu phát sinh công việc trước khi công việc chính thức được tạo trong "CONG_VIEC". Nhờ có trạng thái đề xuất và người duyệt, hệ thống có thể kiểm soát luồng xét duyệt trước khi công việc được đưa vào kế hoạch thực hiện. Bảng có hỗ trợ xóa mềm.

#### 4.3.1.36 Thực thể Yêu cầu đổi quản lý (YEU_CAU_DOI_QUAN_LY)

Thực thể "YEU_CAU_DOI_QUAN_LY" đã được mô tả trong nhóm quản lý dự án vì nội dung yêu cầu đổi quản lý gắn trực tiếp với dự án và người phụ trách dự án. Trong nhóm đề xuất và duyệt, có thể hiểu bảng này cũng là một bảng nghiệp vụ duyệt, dùng để lưu trạng thái yêu cầu, người duyệt và thời điểm duyệt khi thay đổi người quản lý dự án.

### Nhóm đánh giá

#### 4.3.1.37 Thực thể Tiêu chí đánh giá (TIEU_CHI_DANH_GIA)

Thực thể dữ liệu "TIEU_CHI_DANH_GIA" được dùng để lưu danh mục tiêu chí phục vụ đánh giá dự án và đánh giá nhân viên. Bảng có khóa chính là "MaTieuChi".

Các thuộc tính quan trọng gồm "TenTieuChi", "DiemTieuChi", "MoTa", "LoaiTieuChi" và "TrangThaiTieuChi". Bảng này giúp hệ thống chuẩn hóa nội dung đánh giá, tránh việc mỗi lần đánh giá phải nhập lại tiêu chí thủ công.

#### 4.3.1.38 Thực thể Đánh giá dự án (DANH_GIA_DU_AN)

Thực thể dữ liệu "DANH_GIA_DU_AN" được dùng để lưu thông tin đánh giá tổng thể của dự án. Bảng có khóa chính là "MaDanhGiaDuAn".

Các thuộc tính quan trọng gồm "MaDuAn", "MaNguoiDung", "DiemTongDanhGiaDA", "NhanXetTongDuAn", "NgayDanhGiaDA", "TrangThaiDanhGiaDA", "MaNguoiDungDuyet", "NgayDuyetDanhGiaDA" và "LyDoTuChoiDanhGiaDA". Bảng liên kết với "DU_AN" để xác định dự án được đánh giá và liên kết với "NGUOI_DUNG" để xác định người đánh giá, người duyệt hoặc người liên quan đến thao tác xóa mềm.

Bảng "DANH_GIA_DU_AN" giúp hệ thống lưu kết quả đánh giá ở mức tổng quát, làm cơ sở cho việc xem xét hiệu quả thực hiện dự án. Bảng có hỗ trợ xóa mềm để bảo toàn lịch sử đánh giá khi cần.

#### 4.3.1.39 Thực thể Chi tiết đánh giá dự án (CT_DANH_GIA_DU_AN)

Thực thể dữ liệu "CT_DANH_GIA_DU_AN" được dùng để lưu điểm và nhận xét theo từng tiêu chí trong một lần đánh giá dự án. Bảng có khóa chính là "MaChiTietDGDA".

Bảng liên kết với "DANH_GIA_DU_AN" qua "MaDanhGiaDuAn" và liên kết với "TIEU_CHI_DANH_GIA" qua "MaTieuChi". Các thuộc tính như "NhanXetDuAn" và "DiemDanhGiaDA" giúp lưu nội dung đánh giá chi tiết theo từng tiêu chí. Bảng có hỗ trợ xóa mềm.

#### 4.3.1.40 Thực thể Đánh giá nhân viên (DANH_GIA_NHAN_VIEN)

Thực thể dữ liệu "DANH_GIA_NHAN_VIEN" được dùng để lưu thông tin đánh giá nhân viên trong phạm vi dự án. Bảng có khóa chính là "MaDanhGiaNhanVien".

Các thuộc tính quan trọng gồm "MaNguoiDung", "MaDuAn", "MaNguoiDungDanhGia", "DiemTongDanhGiaNV", "NgayDanhGiaNV", "XepLoai", "NhanXetTongQuanNV", "TrangThaiDanhGiaNV", "MaNguoiDungDuyet", "NgayDuyetDanhGiaNV" và "LyDoTuChoiDanhGiaNV". Bảng liên kết với "NGUOI_DUNG" để xác định nhân viên được đánh giá, người đánh giá và người duyệt; đồng thời liên kết với "DU_AN" để xác định bối cảnh dự án.

Vai trò của bảng này là lưu kết quả đánh giá hiệu quả làm việc của nhân viên, phục vụ theo dõi chất lượng tham gia dự án và hỗ trợ các báo cáo đánh giá nhân sự. Bảng có hỗ trợ xóa mềm.

#### 4.3.1.41 Thực thể Chi tiết đánh giá nhân viên (CT_DANH_GIA_NHAN_VIEN)

Thực thể dữ liệu "CT_DANH_GIA_NHAN_VIEN" được dùng để lưu chi tiết điểm đánh giá nhân viên theo từng tiêu chí. Bảng có khóa chính là "MaChiTietDGNV".

Bảng liên kết với "DANH_GIA_NHAN_VIEN" qua "MaDanhGiaNhanVien", liên kết với "TIEU_CHI_DANH_GIA" qua "MaTieuChi" và có thể liên kết với "CONG_VIEC" qua "MaCongViec". Các thuộc tính như "NoiDungDanhGiaNhanVien" và "DiemDanhGiaNV" giúp thể hiện nội dung đánh giá cụ thể hơn cho từng tiêu chí hoặc công việc liên quan. Bảng có hỗ trợ xóa mềm.

### Nhóm AI phân tích nguyên nhân trễ

#### 4.3.1.42 Thực thể Dữ liệu AI (AI_DATASET)

Thực thể dữ liệu "AI_DATASET" được dùng để lưu dữ liệu đặc trưng được tổng hợp từ dự án, công việc, tiến độ, nhân sự và chi phí để phục vụ huấn luyện hoặc phân tích AI. Bảng có khóa chính là "MaData" và có "MaDuAn" liên kết với "DU_AN".

Các thuộc tính quan trọng gồm "SoNhanVienDuAn", "TongSoCongViec", "SoCongViecTre", "TyLeCongViecTre", "ChiPhiDuKien", "ChiPhiThucTe", "ChenhLechChiPhi", "SoLanThayDoiNhanSu", "SoLanThayDoiQuanLy", "SoNgayTreTienDo", "SoDeXuatCongViecChoDuyet", "SoDeXuatCongViecBiTuChoi", "ThoiGianDuyetCongViecTrungBinh", "SoDeXuatNganSachChoDuyet", "SoBaoCaoTienDoBiTuChoi", "TyLeBaoCaoTienDoBiTuChoi", "SoLanCapNhatTienDo", "SoNgayChamCapNhatTienDo", "LaDuAnTre" và "NgayTongHop". Bảng còn có "MaDMNguyenNhan" liên kết với "DM_NGUYEN_NHAN" để ghi nhận nhóm nguyên nhân nếu có.

Vai trò của bảng "AI_DATASET" là cung cấp dữ liệu đầu vào có cấu trúc cho chức năng AI. Dữ liệu trong bảng này phản ánh tình trạng dự án ở nhiều khía cạnh, nhưng chỉ phục vụ phân tích nguyên nhân trễ dự án; AI không tự quyết định nghiệp vụ cuối cùng.

#### 4.3.1.43 Thực thể Mô hình AI (AI_MODEL)

Thực thể dữ liệu "AI_MODEL" được dùng để lưu thông tin các mô hình AI được tạo hoặc sử dụng trong hệ thống. Bảng có khóa chính là "MaModel".

Các thuộc tính quan trọng gồm "TenModel", "SoLuongDuLieu", "DoChinhXac", "TrainSize", "TestSize", "NgayTao", "MoTaModel", "LoaiModel", "IsActive" và "IsDeleted". Bảng này giúp hệ thống quản lý phiên bản mô hình, trạng thái hoạt động và thông tin mô tả liên quan đến quá trình huấn luyện hoặc sử dụng mô hình.

#### 4.3.1.44 Thực thể Kết quả AI (AI_KET_QUA)

Thực thể dữ liệu "AI_KET_QUA" được dùng để lưu kết quả gợi ý của AI khi phân tích nguyên nhân trễ dự án. Bảng có khóa chính là "MaAiKetQua".

Các thuộc tính quan trọng gồm "MaDuAn", "MaData", "MaModel", "MaDMNguyenNhan", "DoTinCayKetQua", "ThoiGianDuDoanKetQua", "ReasonSource", "CanhBaoNguyenNhan" và "NoiDungPhanTich". Bảng liên kết với "DU_AN", "AI_DATASET", "AI_MODEL" và "DM_NGUYEN_NHAN" để xác định dự án, bộ dữ liệu đầu vào, mô hình sử dụng và nhóm nguyên nhân được gợi ý.

Kết quả trong "AI_KET_QUA" là thông tin tham khảo cho người quản lý khi xem xét nguyên nhân trễ. Hệ thống không xem kết quả AI là quyết định nghiệp vụ cuối cùng; người quản lý vẫn là người xác nhận nguyên nhân thực tế.

#### 4.3.1.45 Thực thể Nguyên nhân AI được xác nhận (AI_NGUYEN_NHAN)

Thực thể dữ liệu "AI_NGUYEN_NHAN" được dùng để lưu nguyên nhân thực tế do người quản lý xác nhận sau khi xem xét dữ liệu và gợi ý phân tích. Bảng có khóa chính là "MaAINguyenNhan".

Các thuộc tính quan trọng gồm "MaDuAn", "MaDMNguyenNhan", "DoTinCay", "NgayXacNhan", "MaNguoiDungXacNhan" và "GhiChuXacNhan". Bảng liên kết với "DU_AN" để xác định dự án và liên kết với "DM_NGUYEN_NHAN" để xác định nhóm nguyên nhân. Cột "MaNguoiDungXacNhan" lưu người xác nhận nguyên nhân.

Vai trò của bảng này là tách rõ dữ liệu AI gợi ý với nguyên nhân đã được con người xác nhận. Nhờ đó, hệ thống giữ đúng định hướng AI chỉ hỗ trợ phân tích, còn kết luận cuối cùng vẫn thuộc về người quản lý. Bảng có hỗ trợ xóa mềm.

#### 4.3.1.46 Thực thể Danh mục nguyên nhân (DM_NGUYEN_NHAN)

Thực thể dữ liệu "DM_NGUYEN_NHAN" được dùng để lưu danh mục các nguyên nhân trễ dự án. Bảng có khóa chính là "MaDMNguyenNhan".

Thuộc tính chính của bảng là "TenNguyenNhan". Bảng này được liên kết với "AI_DATASET", "AI_KET_QUA" và "AI_NGUYEN_NHAN", giúp hệ thống chuẩn hóa cách phân loại nguyên nhân trong quá trình huấn luyện, dự đoán và xác nhận.

### Nhóm trao đổi trong dự án

#### 4.3.1.47 Thực thể Phòng chat (PHONG_CHAT)

Thực thể dữ liệu "PHONG_CHAT" được dùng để lưu phòng trao đổi gắn với dự án. Bảng có khóa chính là "MaPhongChat" và có "MaDuAn" liên kết với "DU_AN".

Các thuộc tính quan trọng gồm "TenPhong", "IsDeleted", "DeletedAt" và "DeletedBy". Bảng này giúp mỗi dự án có không gian trao đổi riêng, phục vụ giao tiếp giữa các thành viên trong quá trình thực hiện.

#### 4.3.1.48 Thực thể Thành viên phòng chat (THANH_VIEN_PHONG_CHAT)

Thực thể dữ liệu "THANH_VIEN_PHONG_CHAT" được dùng để lưu danh sách người dùng tham gia từng phòng chat. Bảng có khóa chính ghép gồm "MaPhongChat" và "MaNguoiDung".

Bảng liên kết "PHONG_CHAT" với "NGUOI_DUNG" và lưu thêm "VaiTroTrongPhongChat". Dữ liệu này giúp hệ thống xác định ai được tham gia phòng trao đổi và vai trò của người đó trong phòng chat.

#### 4.3.1.49 Thực thể Tin nhắn (TIN_NHAN)

Thực thể dữ liệu "TIN_NHAN" được dùng để lưu nội dung tin nhắn trong phòng chat. Bảng có khóa chính là "MaTinNhan".

Các thuộc tính quan trọng gồm "MaPhongChat", "MaNguoiDung", "NoiDungTinNhan" và "ThoiGianGui". Bảng liên kết với "PHONG_CHAT" để xác định phòng chứa tin nhắn và liên kết với "NGUOI_DUNG" để xác định người gửi. Bảng có hỗ trợ xóa mềm.

### Nhóm nhật ký hệ thống

#### 4.3.1.50 Thực thể Nhật ký dự án (NHAT_KY_DU_AN)

Thực thể dữ liệu "NHAT_KY_DU_AN" được dùng để lưu lịch sử thay đổi liên quan đến team phụ trách dự án. Bảng có khóa chính là "MaNhatKyTeamDA".

Bảng liên kết với "TEAM" qua "MaTeam" và liên kết với "DU_AN" qua "MaDuAn". Các thuộc tính như "TeamCuPhuTrach", "TeamMoiPhuTrach", "HanhDongNKDA" và "ThoiGianNKDA" giúp ghi lại nội dung thay đổi theo thời gian.

#### 4.3.1.51 Thực thể Nhật ký phân công công việc (NHAT_KY_PHAN_CONG_CONG_VIEC)

Thực thể dữ liệu "NHAT_KY_PHAN_CONG_CONG_VIEC" được dùng để lưu lịch sử phân công công việc chính. Bảng có khóa chính là "MaNhatKyPCCV".

Bảng liên kết với "NGUOI_DUNG" qua "MaNguoiDung" để xác định người được phân công, liên kết với "CONG_VIEC" qua "MaCongViec" và liên kết với "NGUOI_DUNG" qua "MaNguoiDungGhi" để xác định người ghi nhận thao tác. Các thuộc tính "HanhDongPCCV" và "ThoiGianPCCV" giúp lưu nội dung và thời điểm thay đổi.

#### 4.3.1.52 Thực thể Nhật ký phân công chi tiết công việc (NHAT_KY_PHAN_CONG_CT_CONG_VIEC)

Thực thể dữ liệu "NHAT_KY_PHAN_CONG_CT_CONG_VIEC" được dùng để lưu lịch sử phân công ở mức chi tiết công việc. Bảng có khóa chính là "MaNhatKyPCCTCV".

Bảng liên kết với "CT_CONG_VIEC" qua "MaChiTietCV" và liên kết với "NGUOI_DUNG" qua "MaNguoiDung" và "MaNguoiDungGhi". Các thuộc tính "HanhDongPCCTCV" và "ThoiGianPCCTCV" giúp hệ thống theo dõi ai được phân công, ai ghi nhận và thao tác diễn ra khi nào.

#### 4.3.1.53 Thực thể Nhật ký phụ trách dự án (NHAT_KY_PHU_TRACH_DU_AN)

Thực thể dữ liệu "NHAT_KY_PHU_TRACH_DU_AN" được dùng để lưu lịch sử phụ trách dự án của người dùng. Bảng có khóa chính là "MaNhatKyPTDA".

Bảng liên kết với "NGUOI_DUNG" qua "MaNguoiDung" và liên kết với "DU_AN" qua "MaDuAn". Các thuộc tính "NkHanhDongPTDA" và "NkThoiGianPTDA" giúp lưu lại thao tác và thời điểm liên quan đến việc phụ trách dự án.

#### 4.3.1.54 Thực thể Nhật ký quản lý dự án (NHAT_KY_QUAN_LY_DU_AN)

Thực thể dữ liệu "NHAT_KY_QUAN_LY_DU_AN" được dùng để lưu lịch sử quản lý dự án. Bảng có khóa chính là "MaNhatKyQLDA".

Bảng liên kết với "DU_AN" qua "MaDuAn" và liên kết với "NGUOI_DUNG" qua "MaNguoiDung". Các thuộc tính "NkHanhDongQLDA", "NkThoiGianQLDA", "QLDATuNgay" và "QLDADenNgay" giúp ghi nhận quá trình người dùng đảm nhiệm vai trò quản lý dự án theo từng khoảng thời gian.

#### 4.3.1.55 Thực thể Nhật ký ngân sách (NHAT_KY_NGAN_SACH)

Thực thể "NHAT_KY_NGAN_SACH" thuộc nhóm nhật ký hệ thống vì bảng lưu vết các thay đổi ngân sách theo thời gian. Bảng đã được mô tả trong nhóm ngân sách và chi phí; trong vai trò nhật ký, bảng giúp kiểm tra lịch sử điều chỉnh ngân sách, trạng thái ngân sách và số tiền trước sau mỗi lần thay đổi.

#### 4.3.1.56 Thực thể Nhật ký chi phí (NHAT_KY_CHI_PHI)

Thực thể "NHAT_KY_CHI_PHI" thuộc nhóm nhật ký hệ thống vì bảng lưu vết các thay đổi liên quan đến chi phí. Bảng đã được mô tả trong nhóm ngân sách và chi phí; trong vai trò nhật ký, bảng hỗ trợ kiểm tra quá trình cập nhật khoản chi và trạng thái chi phí theo từng công việc.

### Kết luận

Cơ sở dữ liệu giữ vai trò nền tảng trong toàn bộ hệ thống quản lý dự án, giúp lưu trữ có tổ chức các thông tin từ người dùng, dự án, công việc, tiến độ đến ngân sách và đánh giá. Các bảng được liên kết bằng khóa chính và khóa ngoại để bảo đảm dữ liệu giữa các chức năng có sự thống nhất, hạn chế trùng lặp và hỗ trợ truy xuất thông tin chính xác.

Nhóm dữ liệu dự án và công việc là phần trung tâm, vì hầu hết các nghiệp vụ như phân công, cập nhật tiến độ, chi phí, đánh giá và trao đổi đều xoay quanh dự án. Nhóm người dùng và phân quyền giúp hệ thống kiểm soát quyền truy cập, xác định đúng vai trò của từng tài khoản khi thực hiện nghiệp vụ.

Các bảng ngân sách, chi phí, đề xuất và duyệt giúp ghi nhận quá trình xử lý tài chính và thay đổi nghiệp vụ một cách rõ ràng. Các bảng nhật ký hỗ trợ lưu vết thao tác, phục vụ kiểm tra lại lịch sử thay đổi khi cần.

Đối với nhóm AI, cơ sở dữ liệu cung cấp dữ liệu đặc trưng, kết quả gợi ý và nguyên nhân được xác nhận để hỗ trợ phân tích nguyên nhân trễ dự án. Tuy nhiên, kết quả AI chỉ đóng vai trò tham khảo; quyết định nghiệp vụ cuối cùng vẫn do người quản lý xác nhận.

Nhờ cấu trúc dữ liệu này, hệ thống có thể quản lý dự án theo nhiều khía cạnh, đồng thời tạo nền tảng cho thống kê, đánh giá và mở rộng chức năng trong tương lai.
