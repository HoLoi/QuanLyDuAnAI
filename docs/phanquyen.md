# Phân tích chức năng phân quyền

## 1. Phạm vi source đã đọc

Các nhóm source đã rà soát:

| Nhóm | File đã đọc/rà soát |
|---|---|
| Định nghĩa quyền | `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs` |
| Seed màn hình, quyền, role mặc định | `QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs` |
| Service phân quyền | `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanQuyenService.cs`, `PermissionHelper.cs`, `AccountService.cs` |
| Interface/ViewModel phân quyền | `Services/Interfaces/IPhanQuyenService.cs`, `Services/Interfaces/IPermissionHelper.cs`, `ViewModels/PhanQuyen/*` |
| Controller phân quyền/tài khoản | `Controllers/PhanQuyenController.cs`, `Controllers/AccountController.cs` |
| Controller nghiệp vụ có check quyền | `AiController.cs`, `AiDatasetController.cs`, `DashboardController.cs`, `DuAnController.cs`, `CongViecController.cs`, `ChiTietCongViecController.cs`, `TienDoCongViecController.cs`, `DeXuatCongViecController.cs`, `DuyetDeXuatCongViecController.cs`, `DeXuatNganSachController.cs`, `DuyetDeXuatNganSachController.cs`, `NganSachController.cs`, `DanhGiaDuAnController.cs`, `DanhGiaNhanVienController.cs`, `ChatDuAnController.cs`, `NhanSuController.cs`, `ChucDanhController.cs`, `TeamController.cs`, `ThanhVienTeamController.cs`, `TeamDuAnController.cs`, `NhanVienDuAnController.cs`, `PhanCongCongViecController.cs`, `PhanCongChiTietCongViecController.cs`, `YeuCauDoiQuanLyController.cs`, `DuyetYeuCauDoiQuanLyController.cs`, `TaiKhoanCaNhanController.cs` |
| Service có ràng buộc phạm vi dữ liệu | `DuAnService.cs`, `CongViecService.cs`, `ChiTietCongViecService.cs`, `TienDoCongViecService.cs`, `ChatDuAnService.cs`, `DanhGiaDuAnService.cs`, `DanhGiaNhanVienService.cs`, `DeXuatCongViecService.cs`, `DeXuatNganSachService.cs`, `DuyetYeuCauDoiQuanLyService.cs`, `TeamDuAnService.cs`, `ThanhVienTeamService.cs`, `NhanSuService.cs`, `AiService.cs` |
| Giao diện/menu | `Views/PhanQuyen/Index.cshtml`, `Views/Shared/_Layout.cshtml`, các view nghiệp vụ có dùng `Model.Permissions` hoặc `PermissionHelper.HasPermissionAsync` |
| CSS | `wwwroot/css/site.css`, `wwwroot/css/layout/sidebar.css`, `wwwroot/css/PhanQuyen/index.css`, `wwwroot/css/Dashboard/index.css`, và danh sách CSS module trong `wwwroot/css/*` |

Ghi chú phạm vi: các thư mục build như `bin`, `obj`, thư viện tĩnh trong `wwwroot/lib` và file upload không được phân tích chi tiết vì không phải source nghiệp vụ phân quyền.

## 2. Hiện trạng chức năng phân quyền

Hệ thống đang dùng mô hình quyền theo claim:

- `Permissions.cs` định nghĩa tên quyền dạng key kỹ thuật, ví dụ `DuAn.Xem`, `ChiTietCongViec.Them`, `AI.Train`.
- `KhoiTaoTaiKhoanMacDinh.cs` seed danh mục màn hình (`DanhMucManHinh`) và danh mục quyền (`DanhMucQuyen`) từ cấu hình hard-code. Tên màn hình đang là key kỹ thuật như `AIDashboard`, `AIDataset`, `CongViec`, `PhanQuyen`.
- Seed role mặc định gồm `Admin`, `Manager`, `Employee`. `Leader` không phải role trong `AspNetRoles`; hệ thống suy ra leader bằng `NhanVienTeam.IsLeader` hoặc `NhanVienDuAn.VaiTroTrongDuAn`.
- `AccountService.AuthenticateAsync` đăng nhập bằng cookie, nạp role vào `ClaimTypes.Role`, nạp role alias, thêm claim `MaNguoiDung`, thêm claim `IsLeader`, rồi nạp quyền từ `Aspnetroleclaims` và `Aspnetuserclaims`.
- `PhanQuyenService.GetGrantedPermissionNamesAsync` lấy quyền từ claim type chứa `permission`, `claim` hoặc `quyen`.
- `PermissionHelper.HasPermissionAsync` kiểm tra người dùng có ít nhất một quyền trong danh sách truyền vào.
- `PhanQuyenService.GetPageViewModelAsync` lấy danh sách role, quyền đã gán cho role, join `DanhMucQuyen` với `DanhMucManHinh`, rồi group theo màn hình để hiển thị.
- `PhanQuyenService.SaveRolePermissionsAsync` nhận danh sách `MaDanhMucQuyen`, lọc quyền hợp lệ, xóa/thêm/cập nhật `Aspnetroleclaims` trong transaction. Riêng role `Admin` bắt buộc giữ `PhanQuyen.Xem` và `PhanQuyen.Luu`.
- `_Layout.cshtml` đọc quyền hiện tại qua `PhanQuyenService.GetGrantedPermissionNamesAsync(User)` để quyết định menu. Menu đã chia nhóm: Hệ thống, Dự án, Công việc, Tài chính, Đánh giá, Trao đổi, AI.
- Controller hầu hết có `[Authorize]` và tự gọi `IPermissionHelper.HasPermissionAsync` cho từng action.
- Một số service nghiệp vụ có thêm ràng buộc phạm vi dữ liệu, ví dụ dự án theo manager, công việc theo phân công/leader, tiến độ theo người được giao, chat theo thành viên dự án/phòng chat.

## 3. Vấn đề giao diện hiện tại

Các điểm chưa đạt hoặc chưa đồng bộ tốt:

- Màn hình phân quyền hiển thị trực tiếp `TenManHinh` và `TenDanhMucQuyen` từ database, nên người dùng thấy key kỹ thuật như `AIDashboard`, `AIPredict`, `ChiTietCongViec`, `CongViec.Xem`.
- Mô tả quyền đang seed dạng `Quyen {tenQuyen}`, chưa giúp người dùng hiểu ý nghĩa nghiệp vụ.
- Bảng phân quyền hiện chỉ có hai cột `Màn hình` và `Danh sách quyền`. Khi số quyền nhiều, từng dòng dễ dài và khó quét.
- Quyền chưa được gom theo nhóm nghiệp vụ ở màn phân quyền. `_Layout.cshtml` đã có nhóm menu, nhưng `Views/PhanQuyen/Index.cshtml` chỉ group theo màn hình.
- Nút `Chọn tất cả`, `Bỏ chọn tất cả`, `Lưu quyền` có style riêng (`btn-action`) và gần với Dashboard, nhưng chưa có icon, chưa có trạng thái theo từng nhóm, chưa có cảnh báo tác động.
- Checkbox đang dùng `form-check` dạng box/pill nhẹ, nhưng label vẫn là key kỹ thuật nên khó hiểu.
- Chưa có tìm kiếm/lọc quyền theo tên màn hình, nhóm chức năng hoặc quyền.
- Chưa có accordion/card theo nhóm nghiệp vụ. Với ma trận quyền lớn, bảng hiện tại sẽ dài và khó thao tác.
- Chưa có cảnh báo khi bỏ quyền cha như `*.Xem` nhưng vẫn giữ quyền thao tác.
- Chưa có thông tin "quyền thao tác cần quyền xem" trên giao diện.
- Responsive hiện chỉ chỉnh padding/font ở mobile; bảng nhiều checkbox vẫn có nguy cơ dài ngang/dọc.

## 4. Vấn đề ràng buộc quyền hiện tại

### Quyền cha - con

Hiện chưa thấy rule tổng quát nào ở backend hoặc client để enforce quan hệ:

- Không có `PermissionDependencyService`, `PermissionRuleProvider` hoặc cấu hình dependency tập trung.
- `SaveRolePermissionsAsync` chỉ lọc quyền hợp lệ theo ID, chưa tự thêm quyền cha khi có quyền con.
- `SaveRolePermissionsAsync` chưa chặn trường hợp có `ChiTietCongViec.Them` nhưng thiếu `ChiTietCongViec.Xem`.
- View chưa disable quyền con khi quyền `Xem` chưa được chọn.
- JS chỉ chọn tất cả/bỏ chọn tất cả toàn bộ, chưa tự tick quyền `Xem` khi chọn quyền con.
- Các controller vẫn check từng quyền riêng. Nếu role được lưu sai, ví dụ có `TienDo.CapNhat` mà không có `TienDo.Xem`, action cập nhật vẫn có thể qua controller nếu endpoint chỉ check `TienDo.CapNhat`.

### Quyền theo role

Role mặc định đang được seed nhưng chưa có rule validate khi người quản trị sửa quyền:

- `Admin` chỉ bị bắt buộc giữ `PhanQuyen.Xem` và `PhanQuyen.Luu`; không có rule giữ quyền quản trị hệ thống khác.
- `Manager` có nhiều quyền nghiệp vụ, nhưng khi lưu phân quyền không có validate để tránh cấp quyền hệ thống quá rộng hoặc thiếu quyền xem bắt buộc.
- `Employee` đang được seed `TienDo.Duyet`, trong khi nghiệp vụ mong muốn Employee chủ yếu xem/cập nhật/gửi tiến độ. Duyệt tiến độ nên thuộc Manager/Leader theo scope.
- `Leader` không phải role riêng. Các quyền dành cho Leader không nên được cấu hình bằng role độc lập, mà nên là Employee có quyền role nền và được mở thao tác theo scope dữ liệu.
- Admin hiện vẫn được seed quyền AI thao tác và một số quyền xem nghiệp vụ. Một số service đã chặn Admin thao tác nghiệp vụ, nhưng rule này chưa thống nhất toàn hệ thống.

### Quyền + phạm vi dữ liệu

Hệ thống có nhiều ràng buộc scope tốt nhưng chưa đồng đều:

- `DuAnService.GetAllAsync` lọc dự án theo manager hoặc employee tham gia.
- `DuAnService.CheckManagerPermissionAsync` yêu cầu dự án thuộc manager hiện tại cho các thao tác workflow dự án.
- `CongViecService.GetAccessibleProjectIdsAsync` giới hạn dự án theo Admin/Manager/Employee; Employee chỉ thấy công việc của dự án tham gia và sau đó lọc leader/phân công.
- `CongViecService.KiemTraQuyenXuLyTrangThaiCongViecAsync` cho manager dự án, leader dự án hoặc leader team liên quan xác nhận/mở lại công việc.
- `TienDoCongViecService` giới hạn chi tiết công việc theo assignment, manager dự án, leader dự án/team; Admin bị chặn thao tác cập nhật/duyệt tiến độ.
- `ChatDuAnService` yêu cầu quyền claim `Chat.Xem`/`Chat.Gui`, đồng thời chặn Admin và yêu cầu người dùng thuộc phạm vi dự án/phòng chat.
- `DanhGiaDuAnService` và `DanhGiaNhanVienService` có nhiều check claim + role + scope, ví dụ Manager chỉ đánh giá dự án mình quản lý, Admin duyệt.
- `DuyetYeuCauDoiQuanLyService` dùng thêm `CoTheXuLy` và kiểm tra quyền duyệt theo role claim/service.

Điểm cần cải thiện là chuẩn hóa cách viết scope check. Hiện mỗi service tự cài đặt hàm lấy `currentUserId`, role flags và logic scope riêng, dễ lệch rule.

### Backend validation khi lưu quyền

`SaveRolePermissionsAsync` đã có:

- Validate đã chọn role.
- Validate role tồn tại.
- Lọc quyền hợp lệ theo `DanhMucQuyen`.
- Transaction khi cập nhật role claims.
- Bắt buộc Admin giữ `PhanQuyen.Xem` và `PhanQuyen.Luu`.

Nhưng còn thiếu:

- Enforce quyền cha-con.
- Validate quyền không hợp lệ theo role.
- Validate quyền thao tác không có quyền xem.
- Chuẩn hóa quyền AI có quyền xem/nhóm cha.
- Rule giữ quyền bắt buộc của Admin rộng hơn nhóm phân quyền.
- Cơ chế trả lỗi thân thiện để UI hiển thị nguyên nhân quyền bị từ chối.

## 5. Ma trận quyền cha-con đề xuất

| Màn hình | Quyền cha | Quyền con phụ thuộc | Rule bắt buộc |
|---|---|---|---|
| Dashboard/Thống kê | `ThongKe.Xem` | `ThongKe.XuatFile` | Muốn xuất file phải có quyền xem thống kê/dashboard. |
| Nhân sự | `NhanSu.Xem` | `NhanSu.Them`, `NhanSu.Sua`, `NhanSu.Xoa`, `NhanSu.Khoa`, `NhanSu.MoKhoa` | Bỏ `NhanSu.Xem` thì bỏ toàn bộ thao tác nhân sự. |
| Chức danh | `ChucDanh.Xem` | `ChucDanh.Them`, `ChucDanh.Sua`, `ChucDanh.Xoa` | Quyền thêm/sửa/xóa cần quyền xem. |
| Nhóm | `Nhom.Xem` | `Nhom.Them`, `Nhom.Sua`, `Nhom.Xoa` | Quyền thao tác nhóm cần quyền xem nhóm. |
| Thành viên nhóm | `ThanhVienNhom.Xem` | `ThanhVienNhom.Them`, `ThanhVienNhom.Xoa` | Quyền thêm/xóa thành viên cần quyền xem thành viên nhóm. |
| Phân quyền | `PhanQuyen.Xem` | `PhanQuyen.Luu` | Quyền lưu cấu hình bắt buộc cần quyền xem phân quyền. |
| Dự án | `DuAn.Xem` | `DuAn.Them`, `DuAn.Sua`, `DuAn.Xoa` | Quyền thao tác dự án cần quyền xem dự án và đúng scope manager. |
| Yêu cầu đổi quản lý | `YeuCauDoiQuanLy.Xem` | `YeuCauDoiQuanLy.Them` | Tạo yêu cầu cần quyền xem và người dùng phải là Manager của dự án. |
| Duyệt yêu cầu đổi quản lý | `DuyetYeuCauDoiQuanLy.Xem` | `DuyetYeuCauDoiQuanLy.Duyet` | Duyệt cần quyền xem danh sách duyệt và thỏa `CoTheXuLy`. |
| Team dự án | `TeamDuAn.Xem` | `TeamDuAn.Them`, `TeamDuAn.Xoa` | Thêm/xóa team dự án cần xem team dự án và đúng scope dự án. |
| Thành viên dự án | `ThanhVienDuAn.Xem` | `ThanhVienDuAn.Them`, `ThanhVienDuAn.Xoa` | Thêm/xóa thành viên cần quyền xem thành viên dự án và đúng scope. |
| Danh mục công việc | `DanhMucCongViec.Xem` | `DanhMucCongViec.Them`, `DanhMucCongViec.Sua`, `DanhMucCongViec.Xoa` | Quyền thao tác danh mục cần quyền xem. |
| Công việc | `CongViec.Xem` | Các thao tác workflow liên quan công việc | Nếu không xem công việc thì không nên cấp thao tác tiến độ/duyệt công việc liên quan. |
| Chi tiết công việc | `ChiTietCongViec.Xem` | `ChiTietCongViec.Them`, `ChiTietCongViec.Sua`, `ChiTietCongViec.Xoa` | Quyền thêm/sửa/xóa chi tiết cần quyền xem chi tiết. |
| Phân công công việc | `PhanCongCongViec.Xem` | `PhanCongCongViec.ThucHien` | Thực hiện phân công cần quyền xem phân công và đúng scope manager/leader. |
| Phân công chi tiết công việc | `PhanCongChiTietCongViec.Xem` | `PhanCongChiTietCongViec.ThucHien` | Thực hiện phân công chi tiết cần quyền xem và đúng scope. |
| Đề xuất công việc | `DeXuatCongViec.Xem` | `DeXuatCongViec.Them` | Tạo đề xuất cần quyền xem danh sách đề xuất. |
| Duyệt đề xuất công việc | `DuyetDeXuatCongViec.Xem` | `DuyetDeXuatCongViec.Duyet` | Duyệt cần quyền xem danh sách duyệt và đúng scope. |
| Tiến độ | `TienDo.Xem` | `TienDo.CapNhat`, `TienDo.Duyet` | Cập nhật/duyệt tiến độ cần quyền xem tiến độ và đúng assignment/scope. |
| Đề xuất ngân sách | `DeXuatNganSach.Xem` | `DeXuatNganSach.Them` | Tạo đề xuất cần quyền xem đề xuất ngân sách. |
| Duyệt ngân sách | `DuyetNganSach.Xem` | `DuyetNganSach.Duyet` | Duyệt cần quyền xem danh sách duyệt và đúng scope. |
| Ngân sách | `NganSach.Xem` | Các thao tác xem chi phí/ngân sách liên quan | Màn hình ngân sách hiện chỉ có quyền xem; nếu thêm thao tác cần phụ thuộc `NganSach.Xem`. |
| Chi phí | `ChiPhi.Xem` | `ChiPhi.Them`, `ChiPhi.Sua` | Thêm/sửa chi phí cần quyền xem chi phí và đúng ngân sách/dự án. |
| Đánh giá dự án | `DanhGiaDuAn.Xem` | `DanhGiaDuAn.DanhGia`, `DanhGiaDuAn.Sua`, `DanhGiaDuAn.Duyet` | Đánh giá/sửa/duyệt cần quyền xem và đúng role/scope. |
| Đánh giá nhân viên | `DanhGiaNhanVien.Xem` | `DanhGiaNhanVien.DanhGia`, `DanhGiaNhanVien.Sua`, `DanhGiaNhanVien.Duyet` | Đánh giá/sửa/duyệt cần quyền xem và đúng role/scope. |
| Chat | `Chat.Xem` | `Chat.Gui` | Gửi tin nhắn cần quyền xem chat và là thành viên dự án/phòng chat; Admin không tham gia. |
| AI Dashboard | `AI.Dashboard` | Quyền xem/tổng quan AI | Nên coi là quyền xem tổng quan AI. |
| AI Dataset | `AI.Dashboard` hoặc quyền xem AI chung | `AI.Dataset` | Quản lý dữ liệu AI nên cần quyền xem AI/tổng quan AI hoặc một quyền cha `AI.Xem`. |
| AI Train | `AI.Dashboard` hoặc quyền xem AI chung | `AI.Train` | Huấn luyện AI nên cần quyền xem AI và chỉ dành Admin/nhóm được ủy quyền. |
| AI Predict | `AI.Dashboard` hoặc quyền xem AI chung | `AI.PhanTichNguyenNhan`, `AI.XacNhan` | Phân tích/xác nhận nguyên nhân trễ cần quyền xem AI và đúng scope dự án/công việc nếu có. |
| Nhật ký | `NhatKy.Xem` | Chưa có quyền con | Nếu sau này thêm xóa/xuất nhật ký thì phụ thuộc quyền xem. |

Đề xuất quan trọng: nên thêm quyền cha `AI.Xem` để tránh dùng `AI.Dashboard` làm quyền cha ngầm cho toàn bộ nhóm AI.

## 5.1 Dependency Permission Matrix

Bảng này là dữ liệu kỹ thuật đề xuất cho `PermissionDependencyProvider`. Khi triển khai, mỗi dòng có `ChildPermission` khác `(không có)` phải được enforce ở cả frontend và backend.

| ParentPermission | ChildPermission | Rule |
|---|---|---|
| `ThongKe.Xem` | `ThongKe.XuatFile` | Bắt buộc có quyền cha. |
| `NhanSu.Xem` | `NhanSu.Them` | Bắt buộc có quyền cha. |
| `NhanSu.Xem` | `NhanSu.Sua` | Bắt buộc có quyền cha. |
| `NhanSu.Xem` | `NhanSu.Xoa` | Bắt buộc có quyền cha. |
| `NhanSu.Xem` | `NhanSu.Khoa` | Bắt buộc có quyền cha. |
| `NhanSu.Xem` | `NhanSu.MoKhoa` | Bắt buộc có quyền cha. |
| `PhanQuyen.Xem` | `PhanQuyen.Luu` | Bắt buộc có quyền cha. |
| `ChucDanh.Xem` | `ChucDanh.Them` | Bắt buộc có quyền cha. |
| `ChucDanh.Xem` | `ChucDanh.Sua` | Bắt buộc có quyền cha. |
| `ChucDanh.Xem` | `ChucDanh.Xoa` | Bắt buộc có quyền cha. |
| `Nhom.Xem` | `Nhom.Them` | Bắt buộc có quyền cha. |
| `Nhom.Xem` | `Nhom.Sua` | Bắt buộc có quyền cha. |
| `Nhom.Xem` | `Nhom.Xoa` | Bắt buộc có quyền cha. |
| `ThanhVienNhom.Xem` | `ThanhVienNhom.Them` | Bắt buộc có quyền cha. |
| `ThanhVienNhom.Xem` | `ThanhVienNhom.Xoa` | Bắt buộc có quyền cha. |
| `DuyetYeuCauDoiQuanLy.Xem` | `DuyetYeuCauDoiQuanLy.Duyet` | Bắt buộc có quyền cha. |
| `DuAn.Xem` | `DuAn.Them` | Bắt buộc có quyền cha. |
| `DuAn.Xem` | `DuAn.Sua` | Bắt buộc có quyền cha. |
| `DuAn.Xem` | `DuAn.Xoa` | Bắt buộc có quyền cha. |
| `YeuCauDoiQuanLy.Xem` | `YeuCauDoiQuanLy.Them` | Bắt buộc có quyền cha. |
| `TeamDuAn.Xem` | `TeamDuAn.Them` | Bắt buộc có quyền cha. |
| `TeamDuAn.Xem` | `TeamDuAn.Xoa` | Bắt buộc có quyền cha. |
| `ThanhVienDuAn.Xem` | `ThanhVienDuAn.Them` | Bắt buộc có quyền cha. |
| `ThanhVienDuAn.Xem` | `ThanhVienDuAn.Xoa` | Bắt buộc có quyền cha. |
| `CongViec.Xem` | `(không có)` | Module hiện chỉ có quyền xem; nếu thêm thao tác công việc sau này thì phải phụ thuộc `CongViec.Xem`. |
| `DanhMucCongViec.Xem` | `DanhMucCongViec.Them` | Bắt buộc có quyền cha. |
| `DanhMucCongViec.Xem` | `DanhMucCongViec.Sua` | Bắt buộc có quyền cha. |
| `DanhMucCongViec.Xem` | `DanhMucCongViec.Xoa` | Bắt buộc có quyền cha. |
| `DeXuatCongViec.Xem` | `DeXuatCongViec.Them` | Bắt buộc có quyền cha. |
| `DuyetDeXuatCongViec.Xem` | `DuyetDeXuatCongViec.Duyet` | Bắt buộc có quyền cha. |
| `ChiTietCongViec.Xem` | `ChiTietCongViec.Them` | Bắt buộc có quyền cha. |
| `ChiTietCongViec.Xem` | `ChiTietCongViec.Sua` | Bắt buộc có quyền cha. |
| `ChiTietCongViec.Xem` | `ChiTietCongViec.Xoa` | Bắt buộc có quyền cha. |
| `PhanCongCongViec.Xem` | `PhanCongCongViec.ThucHien` | Bắt buộc có quyền cha. |
| `PhanCongChiTietCongViec.Xem` | `PhanCongChiTietCongViec.ThucHien` | Bắt buộc có quyền cha. |
| `TienDo.Xem` | `TienDo.CapNhat` | Bắt buộc có quyền cha. |
| `TienDo.Xem` | `TienDo.Duyet` | Bắt buộc có quyền cha. |
| `DeXuatNganSach.Xem` | `DeXuatNganSach.Them` | Bắt buộc có quyền cha. |
| `DuyetNganSach.Xem` | `DuyetNganSach.Duyet` | Bắt buộc có quyền cha. |
| `NganSach.Xem` | `(không có)` | Module hiện chỉ có quyền xem; nếu thêm thao tác ngân sách sau này thì phải phụ thuộc `NganSach.Xem`. |
| `ChiPhi.Xem` | `ChiPhi.Them` | Bắt buộc có quyền cha. |
| `ChiPhi.Xem` | `ChiPhi.Sua` | Bắt buộc có quyền cha. |
| `AI.Xem` | `AI.Dashboard` | Bắt buộc có quyền cha AI chuẩn hóa. |
| `AI.Xem` | `AI.Dataset` | Bắt buộc có quyền cha AI chuẩn hóa. |
| `AI.Xem` | `AI.Train` | Bắt buộc có quyền cha AI chuẩn hóa. |
| `AI.Xem` | `AI.PhanTichNguyenNhan` | Bắt buộc có quyền cha AI chuẩn hóa. |
| `AI.Xem` | `AI.XacNhan` | Bắt buộc có quyền cha AI chuẩn hóa. |
| `DanhGiaDuAn.Xem` | `DanhGiaDuAn.DanhGia` | Bắt buộc có quyền cha. |
| `DanhGiaDuAn.Xem` | `DanhGiaDuAn.Sua` | Bắt buộc có quyền cha. |
| `DanhGiaDuAn.Xem` | `DanhGiaDuAn.Duyet` | Bắt buộc có quyền cha. |
| `DanhGiaNhanVien.Xem` | `DanhGiaNhanVien.DanhGia` | Bắt buộc có quyền cha. |
| `DanhGiaNhanVien.Xem` | `DanhGiaNhanVien.Sua` | Bắt buộc có quyền cha. |
| `DanhGiaNhanVien.Xem` | `DanhGiaNhanVien.Duyet` | Bắt buộc có quyền cha. |
| `Chat.Xem` | `Chat.Gui` | Bắt buộc có quyền cha. |
| `NhatKy.Xem` | `(không có)` | Module hiện chỉ có quyền xem; nếu thêm thao tác nhật ký sau này thì phải phụ thuộc `NhatKy.Xem`. |

Ghi chú kỹ thuật: `Permissions.AI.DuDoan` hiện là alias trỏ về `AI.PhanTichNguyenNhan`, không phải một quyền độc lập cần seed/lưu thêm.

## Quy tắc quyền AI chuẩn hóa

Kết luận chính thức cho thiết kế triển khai sau:

- Tạo quyền cha mới: `AI.Xem`.
- Các quyền con của `AI.Xem` gồm:
  - `AI.Dashboard`
  - `AI.Dataset`
  - `AI.Train`
  - `AI.PhanTichNguyenNhan`
  - `AI.XacNhan`

Rule bắt buộc:

- Không có `AI.Xem` thì không được giữ bất kỳ quyền AI con nào.
- Chọn một quyền AI con thì tự chọn `AI.Xem`.
- Bỏ `AI.Xem` thì tự bỏ toàn bộ quyền AI con.
- Backend khi lưu quyền phải normalize hoặc reject mọi danh sách quyền có quyền AI con nhưng thiếu `AI.Xem`.
- Menu AI trong `_Layout.cshtml` nên dùng `AI.Xem` làm điều kiện nhóm tổng, sau đó từng link vẫn kiểm quyền con tương ứng.

Không nên dùng `AI.Dashboard` làm quyền cha vì `Dashboard` là một màn hình cụ thể, không phải quyền nền của toàn bộ phân hệ AI. Nếu dùng `AI.Dashboard` làm quyền cha, một role cần thao tác `AI.Dataset` hoặc `AI.Train` nhưng không cần xem dashboard vẫn bị phụ thuộc sai vào một màn hình UI. `AI.Xem` tách rõ quyền truy cập phân hệ AI khỏi quyền xem từng màn hình/chức năng, giúp rule dễ hiểu, dễ enforce và không làm lệch ý nghĩa `AI.Dashboard`.

## Permission Inventory

Danh sách dưới đây được đọc trực tiếp từ `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`. Chỉ liệt kê các key quyền đang tồn tại trong source hiện tại.

### Nhóm Hệ thống

* `NhanSu.Xem`
* `NhanSu.Them`
* `NhanSu.Sua`
* `NhanSu.Xoa`
* `NhanSu.Khoa`
* `NhanSu.MoKhoa`
* `PhanQuyen.Xem`
* `PhanQuyen.Luu`
* `ChucDanh.Xem`
* `ChucDanh.Them`
* `ChucDanh.Sua`
* `ChucDanh.Xoa`
* `Nhom.Xem`
* `Nhom.Them`
* `Nhom.Sua`
* `Nhom.Xoa`
* `ThanhVienNhom.Xem`
* `ThanhVienNhom.Them`
* `ThanhVienNhom.Xoa`

### Nhóm Dự án

* `DuAn.Xem`
* `DuAn.Them`
* `DuAn.Sua`
* `DuAn.Xoa`
* `YeuCauDoiQuanLy.Xem`
* `YeuCauDoiQuanLy.Them`
* `DuyetYeuCauDoiQuanLy.Xem`
* `DuyetYeuCauDoiQuanLy.Duyet`
* `TeamDuAn.Xem`
* `TeamDuAn.Them`
* `TeamDuAn.Xoa`
* `ThanhVienDuAn.Xem`
* `ThanhVienDuAn.Them`
* `ThanhVienDuAn.Xoa`

### Nhóm Công việc

* `CongViec.Xem`
* `DanhMucCongViec.Xem`
* `DanhMucCongViec.Them`
* `DanhMucCongViec.Sua`
* `DanhMucCongViec.Xoa`
* `DeXuatCongViec.Xem`
* `DeXuatCongViec.Them`
* `DuyetDeXuatCongViec.Xem`
* `DuyetDeXuatCongViec.Duyet`
* `ChiTietCongViec.Xem`
* `ChiTietCongViec.Them`
* `ChiTietCongViec.Sua`
* `ChiTietCongViec.Xoa`
* `PhanCongCongViec.Xem`
* `PhanCongCongViec.ThucHien`
* `PhanCongChiTietCongViec.Xem`
* `PhanCongChiTietCongViec.ThucHien`
* `TienDo.Xem`
* `TienDo.CapNhat`
* `TienDo.Duyet`

### Nhóm Tài chính

* `DeXuatNganSach.Xem`
* `DeXuatNganSach.Them`
* `DuyetNganSach.Xem`
* `DuyetNganSach.Duyet`
* `NganSach.Xem`
* `ChiPhi.Xem`
* `ChiPhi.Them`
* `ChiPhi.Sua`

### Nhóm AI

* `AI.Dataset`
* `AI.Train`
* `AI.PhanTichNguyenNhan`
* `AI.Dashboard`
* `AI.XacNhan`

Ghi chú: `AI.DuDoan` trong `Permissions.cs` là alias trỏ về `AI.PhanTichNguyenNhan`, không phải một key quyền riêng.

### Nhóm Đánh giá

* `DanhGiaDuAn.Xem`
* `DanhGiaDuAn.DanhGia`
* `DanhGiaDuAn.Sua`
* `DanhGiaDuAn.Duyet`
* `DanhGiaNhanVien.Xem`
* `DanhGiaNhanVien.DanhGia`
* `DanhGiaNhanVien.Sua`
* `DanhGiaNhanVien.Duyet`

### Nhóm Chat

* `Chat.Xem`
* `Chat.Gui`

### Nhóm Báo cáo

* `ThongKe.Xem`
* `ThongKe.XuatFile`
* `NhatKy.Xem`

Tổng số quyền hiện có: 79

Tổng số màn hình hiện có: 28

Danh sách quyền này được xem là nguồn dữ liệu chính thức để triển khai dependency và validation.

## 6. Ma trận quyền theo vai trò đề xuất

| Nhóm chức năng | Admin | Manager | Employee | Leader |
|---|---|---|---|---|
| Hệ thống/nhân sự/chức danh/nhóm | Có toàn quyền quản trị cần thiết | Không mặc định, trừ khi được ủy quyền | Không | Không |
| Phân quyền | Bắt buộc `PhanQuyen.Xem`, `PhanQuyen.Luu` | Không mặc định | Không | Không |
| Nhật ký/thống kê hệ thống | Có xem/xuất | Có xem/xuất báo cáo nghiệp vụ thuộc phạm vi | Có xem báo cáo cá nhân nếu cần | Có xem báo cáo team/dự án thuộc phạm vi |
| Dự án | Xem/quản trị hệ thống; hạn chế thao tác workflow nghiệp vụ hằng ngày | Xem/thêm/sửa/xóa dự án mình quản lý theo trạng thái | Xem dự án tham gia | Xem dự án liên quan, thao tác trong scope leader |
| Team/thành viên dự án | Có thể quản trị dữ liệu nền nếu cần | Quản lý team/thành viên của dự án mình phụ trách | Không thao tác mặc định | Có thể xem, đề xuất hoặc thao tác trong phạm vi team nếu nghiệp vụ cho phép |
| Công việc/danh mục công việc | Nên hạn chế thao tác nghiệp vụ; có thể xem để audit | Quản lý danh mục, công việc, phân công trong dự án phụ trách | Xem công việc được giao/tham gia | Xem và hỗ trợ điều phối công việc trong team/dự án liên quan |
| Chi tiết công việc/phân công | Không thao tác hằng ngày | Tạo/sửa/xóa/phân công trong dự án phụ trách | Xem/cập nhật phần được giao | Phân công/cập nhật trong team/dự án liên quan nếu được cấp quyền role nền |
| Tiến độ | Không cập nhật/duyệt tiến độ nghiệp vụ | Xem/duyệt tiến độ dự án phụ trách | Xem/cập nhật/gửi tiến độ phần được giao; không duyệt mặc định | Duyệt/cập nhật trong phạm vi team/dự án liên quan nếu được cấp quyền role nền |
| Đề xuất công việc/ngân sách | Không mặc định thao tác | Duyệt đề xuất trong dự án phụ trách | Tạo đề xuất liên quan công việc/dự án tham gia | Có thể tạo/duyệt theo phạm vi nếu nghiệp vụ giao leader duyệt |
| Ngân sách/chi phí | Xem audit hoặc quản trị dữ liệu nền | Xem/quản lý ngân sách, chi phí dự án phụ trách | Không mặc định hoặc chỉ xem phần liên quan | Xem trong phạm vi nếu cần |
| Đánh giá | Admin duyệt/kiểm soát cấu hình | Đánh giá dự án/nhân viên thuộc phạm vi quản lý | Xem đánh giá cá nhân/liên quan | Hỗ trợ đánh giá team nếu nghiệp vụ yêu cầu |
| AI Dataset/Train | Có Dataset/Train/Dashboard | Không train mặc định; có Dashboard/Predict/Xác nhận nếu phục vụ phân tích trễ | Không mặc định | Có thể xem phân tích liên quan nếu cần |
| AI phân tích nguyên nhân trễ | Có thể xem/tổng hợp hệ thống | Phân tích và xác nhận nguyên nhân trễ trong dự án phụ trách | Xem kết quả liên quan phần việc nếu cần | Xác nhận/hỗ trợ nguyên nhân trong phạm vi team/dự án |
| Chat | Không tham gia chat nghiệp vụ dự án | Chat trong dự án mình quản lý | Chat trong dự án tham gia | Chat trong team/dự án liên quan |

Điểm cần sửa trong seed hiện tại: `Employee` không nên có `TienDo.Duyet` mặc định. Nếu Leader cần duyệt tiến độ, nên kiểm qua scope `IsLeader`/vai trò dự án và quyền nền riêng, không cấp đại trà cho toàn bộ Employee.

## Role Constraint Matrix

Bảng này xác định các quyền tuyệt đối không nên cấp cho từng role mặc định trong nghiệp vụ hiện tại. Các quyền bị chặn ở đây vẫn có thể tồn tại trong hệ thống, nhưng `SaveRolePermissionsAsync` nên từ chối khi role tương ứng cố lưu các quyền đó.

| Role | Không được cấp | Lý do |
|---|---|---|
| `Admin` | `Chat.Gui`, `TienDo.CapNhat`, `TienDo.Duyet`, `DeXuatCongViec.Them`, `DeXuatNganSach.Them`, `YeuCauDoiQuanLy.Them` | Admin quản trị hệ thống, không tham gia tác nghiệp dự án hằng ngày; source hiện đã có chặn Admin ở chat, tiến độ và tạo yêu cầu đổi quản lý. |
| `Admin` | `PhanCongCongViec.ThucHien`, `PhanCongChiTietCongViec.ThucHien`, `ChiTietCongViec.Them`, `ChiTietCongViec.Sua`, `ChiTietCongViec.Xoa` | Đây là thao tác điều phối/thực hiện công việc theo scope dự án/team; nên thuộc Manager/Leader/Employee được phân công, không thuộc Admin hệ thống. |
| `Manager` | `PhanQuyen.Luu`, `NhanSu.Them`, `NhanSu.Sua`, `NhanSu.Xoa`, `NhanSu.Khoa`, `NhanSu.MoKhoa`, `ChucDanh.Them`, `ChucDanh.Sua`, `ChucDanh.Xoa` | Manager quản lý dự án, không quản trị tài khoản, phân quyền và danh mục nhân sự hệ thống nếu không có ủy quyền đặc biệt. |
| `Manager` | `AI.Dataset`, `AI.Train` | Dataset/train AI là chức năng quản trị AI, nên để Admin hoặc nhóm vận hành AI; Manager dùng `AI.Xem`, `AI.Dashboard`, `AI.PhanTichNguyenNhan`, `AI.XacNhan` nếu nghiệp vụ cần. |
| `Employee` | `PhanQuyen.Xem`, `PhanQuyen.Luu`, `NhanSu.Them`, `NhanSu.Sua`, `NhanSu.Xoa`, `NhanSu.Khoa`, `NhanSu.MoKhoa`, `ChucDanh.Them`, `ChucDanh.Sua`, `ChucDanh.Xoa`, `Nhom.Them`, `Nhom.Sua`, `Nhom.Xoa`, `ThanhVienNhom.Them`, `ThanhVienNhom.Xoa` | Employee không quản trị hệ thống, nhân sự, role hoặc nhóm hệ thống. |
| `Employee` | `DuAn.Them`, `DuAn.Sua`, `DuAn.Xoa`, `TeamDuAn.Them`, `TeamDuAn.Xoa`, `ThanhVienDuAn.Them`, `ThanhVienDuAn.Xoa`, `DanhMucCongViec.Them`, `DanhMucCongViec.Sua`, `DanhMucCongViec.Xoa` | Employee chỉ xem/thực hiện phần được giao; quản lý cấu trúc dự án thuộc Manager. |
| `Employee` | `DuyetNganSach.Duyet`, `DuyetDeXuatCongViec.Duyet`, `DuyetYeuCauDoiQuanLy.Duyet`, `DanhGiaDuAn.Duyet`, `DanhGiaNhanVien.Duyet`, `TienDo.Duyet` | Employee không duyệt nghiệp vụ đại trà. Nếu là Leader thì mở theo scope leader, không cấp mặc định cho toàn role Employee. |
| `Employee` | `ChiPhi.Them`, `ChiPhi.Sua`, `NganSach.Xem` nếu không thuộc phạm vi dự án | Chi phí/ngân sách là nghiệp vụ tài chính dự án; chỉ mở nếu có yêu cầu rõ và phải check scope dữ liệu. |
| `Employee` | `AI.Dataset`, `AI.Train`, `AI.XacNhan` | Employee không quản trị AI hoặc xác nhận nguyên nhân trễ cấp quản lý; nếu cần chỉ xem kết quả liên quan phần việc. |
| `Leader` | Không cấu hình như role độc lập trong `AspNetRoles` | Leader là Employee có `IsLeader = true` hoặc vai trò leader trong team/dự án; quyền Leader phải là rule scope, không phải role claim riêng. |

## 7. Đề xuất cải tiến backend

Chỉ phân tích, chưa sửa code.

### Service/helper cần thêm

- Thêm `PermissionRuleProvider` hoặc `PermissionDependencyService`.
- Cấu hình rule gồm:
  - `ParentPermission`
  - `ChildPermissions`
  - `RequiredForRoles`
  - `DeniedForRoles`
  - `IsSystemRequired`
  - `DisplayGroup`
  - `DisplayName`
  - `Description`
- Thêm service scope dùng chung, ví dụ `IUserScopeService`, để gom:
  - lấy `MaNguoiDung`
  - lấy role flags
  - xác định Admin/Manager/Employee/Leader
  - kiểm tra dự án được quản lý
  - kiểm tra dự án tham gia
  - kiểm tra leader team/dự án
  - kiểm tra assignment công việc/chi tiết công việc

### Method cần sửa

- `PhanQuyenService.GetPageViewModelAsync`: trả thêm nhóm nghiệp vụ, display name, mô tả, quyền cha, danh sách quyền con.
- `PhanQuyenService.SaveRolePermissionsAsync`: normalize danh sách quyền trước khi lưu:
  - nếu có quyền con thì tự thêm quyền cha hoặc trả lỗi.
  - nếu bỏ quyền cha thì bỏ quyền con.
  - validate role-specific deny/allow.
  - giữ quyền bắt buộc của Admin.
- `KhoiTaoTaiKhoanMacDinh.DamBaoDanhMucManHinhVaChucNangAsync`: seed thêm tên hiển thị/mô tả tiếng Việt thay vì chỉ key kỹ thuật.
- `PermissionHelper.HasPermissionAsync`: có thể bổ sung mode yêu cầu tất cả quyền (`RequireAll`) nếu cần.
- Các service nghiệp vụ: gom logic role/scope đang lặp vào helper dùng chung.

### Rule cần enforce khi lưu quyền

- Không lưu quyền con nếu thiếu quyền cha.
- Với role `Admin`: giữ nhóm quản trị bắt buộc, tối thiểu `PhanQuyen.Xem`, `PhanQuyen.Luu`, `NhanSu.Xem`, quyền mở khóa/khóa nếu hệ thống yêu cầu.
- Với role `Employee`: không cấp quyền duyệt/duyệt ngân sách/duyệt đánh giá đại trà nếu không có scope rule tương ứng.
- Với role `Manager`: quyền thao tác dự án chỉ có ý nghĩa khi scope là dự án mình phụ trách.
- Với AI: nếu chưa có `AI.Xem`, dùng rule tạm là `AI.Dashboard` làm quyền cha cho `AI.Dataset`, `AI.Train`, `AI.PhanTichNguyenNhan`, `AI.XacNhan`.

## Backend Validation Rules

Các rule dưới đây cần enforce trong `PhanQuyenService.SaveRolePermissionsAsync()` ở bước triển khai sau.

### Rule lưu quyền cha-con

- Nhận danh sách `SelectedPermissionIds`, map sang tên quyền từ `DanhMucQuyen`.
- Chuẩn hóa quyền theo `PermissionDependencyProvider`.
- Nếu cấu hình chọn chiến lược tự sửa:
  - Khi có quyền con, tự thêm quyền cha tương ứng.
  - Khi thiếu `AI.Xem` nhưng có quyền AI con, tự thêm `AI.Xem`.
- Nếu cấu hình chọn chiến lược strict:
  - Từ chối lưu khi có quyền con nhưng thiếu quyền cha.
  - Trả lỗi rõ: `Quyền {ChildPermission} bắt buộc cần {ParentPermission}.`
- Dù chọn chiến lược nào, dữ liệu cuối cùng lưu vào `Aspnetroleclaims` không được còn quyền con thiếu quyền cha.

### Rule bắt buộc của Admin

Admin bắt buộc giữ tối thiểu:

- `PhanQuyen.Xem`
- `PhanQuyen.Luu`
- `NhanSu.Xem`
- `NhanSu.Khoa`
- `NhanSu.MoKhoa`
- `ChucDanh.Xem`
- `Nhom.Xem`
- `ThongKe.Xem`
- `NhatKy.Xem`

Nếu triển khai `AI.Xem`, Admin nên giữ:

- `AI.Xem`
- `AI.Dashboard`
- `AI.Dataset`
- `AI.Train`

Không nên bắt buộc Admin giữ các quyền tác nghiệp dự án như `Chat.Gui`, `TienDo.CapNhat`, `TienDo.Duyet`.

### Rule chặn quyền theo role

- Không cho `Employee` giữ quyền phân quyền: `PhanQuyen.Xem`, `PhanQuyen.Luu`.
- Không cho `Employee` giữ quyền duyệt ngân sách: `DuyetNganSach.Duyet`.
- Không cho `Employee` giữ quyền duyệt đề xuất công việc: `DuyetDeXuatCongViec.Duyet`.
- Không cho `Employee` giữ quyền duyệt yêu cầu đổi quản lý: `DuyetYeuCauDoiQuanLy.Duyet`.
- Không cho `Employee` giữ quyền duyệt đánh giá: `DanhGiaDuAn.Duyet`, `DanhGiaNhanVien.Duyet`.
- Không cho `Employee` giữ `TienDo.Duyet` mặc định. Nếu cần Leader duyệt, dùng scope leader trong service.
- Không cho `Employee` giữ quyền quản trị nhân sự, chức danh, nhóm và thành viên nhóm.
- Không cho `Employee` giữ quyền quản trị AI: `AI.Dataset`, `AI.Train`.
- Không cho `Manager` giữ `PhanQuyen.Luu` nếu nghiệp vụ không yêu cầu phân quyền ủy quyền.
- Không cho `Manager` giữ quyền quản trị nhân sự hệ thống như `NhanSu.Khoa`, `NhanSu.MoKhoa`, `NhanSu.Xoa`.
- Không cho `Manager` giữ `AI.Train`, `AI.Dataset` nếu Manager chỉ dùng AI để phân tích nguyên nhân trễ.
- Không cho `Admin` giữ các quyền tác nghiệp bị chặn theo nghiệp vụ: `Chat.Gui`, `TienDo.CapNhat`, `TienDo.Duyet`, `DeXuatCongViec.Them`, `DeXuatNganSach.Them`, `YeuCauDoiQuanLy.Them`.

### Rule validate quyền hợp lệ

- Chỉ lưu quyền có tồn tại trong `DanhMucQuyen`.
- Nếu thêm `AI.Xem` vào `Permissions.cs`, phải seed `DanhMucQuyen` trước khi role có thể lưu quyền này.
- Không lưu trùng quyền.
- `ClaimType` khi lưu phải là `Permissions.ClaimTypesCustom.Permission`.
- `ClaimValue` phải là tên quyền chuẩn, không dùng nhãn hiển thị tiếng Việt.
- Không xóa sạch quyền bắt buộc của role nếu request từ UI gửi thiếu do lỗi client.

### Rule transaction và lỗi

- Toàn bộ xóa/thêm/cập nhật role claim phải nằm trong transaction như hiện tại.
- Validate dependency và role constraint trước khi xóa claim cũ.
- Khi validate thất bại, không được ghi một phần.
- Lỗi trả về controller phải đủ cụ thể để view hiển thị cảnh báo trước/sau lưu.

### Rule cần enforce khi check quyền thao tác

- Controller vẫn check quyền claim như hiện tại.
- Service phải tiếp tục check scope dữ liệu, không dựa vào claim đơn lẻ.
- Admin không nên thao tác các nghiệp vụ đã xác định là nghiệp vụ hằng ngày: chat dự án, cập nhật/duyệt tiến độ, tạo yêu cầu đổi quản lý.
- Manager chỉ thao tác dự án mình phụ trách.
- Employee chỉ thao tác phần việc/chi tiết được giao.
- Leader chỉ mở rộng thao tác trong team/dự án liên quan, không phải toàn hệ thống.

## 8. Đề xuất cải tiến frontend

Chỉ phân tích, chưa sửa code.

## UI Mockup đề xuất

Mô phỏng giao diện bằng text cho màn hình phân quyền sau khi cải tiến:

```text
[THANH CÔNG CỤ]
Vai trò: [Admin v]
Tìm kiếm: [Nhập tên màn hình, quyền, nhóm chức năng...]
[Chọn tất cả] [Bỏ chọn tất cả] [Lưu quyền]

[HỆ THỐNG]
  [Nhân sự]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Sửa
    - [ ] Xóa
    - [ ] Khóa
    - [ ] Mở khóa

  [Chức danh]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Sửa
    - [ ] Xóa

  [Phân quyền]
    - [ ] Xem
    - [ ] Lưu cấu hình

  [Nhóm]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Sửa
    - [ ] Xóa

  [Thành viên nhóm]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Xóa

[DỰ ÁN]
  [Dự án]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Sửa
    - [ ] Xóa

  [Yêu cầu đổi quản lý]
    - [ ] Xem
    - [ ] Thêm yêu cầu

  [Duyệt yêu cầu đổi quản lý]
    - [ ] Xem
    - [ ] Duyệt

  [Team dự án]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Xóa

  [Thành viên dự án]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Xóa

[CÔNG VIỆC]
  [Danh mục công việc]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Sửa
    - [ ] Xóa

  [Công việc]
    - [ ] Xem

  [Chi tiết công việc]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Sửa
    - [ ] Xóa

  [Phân công công việc]
    - [ ] Xem
    - [ ] Thực hiện

  [Phân công chi tiết công việc]
    - [ ] Xem
    - [ ] Thực hiện

  [Đề xuất công việc]
    - [ ] Xem
    - [ ] Thêm

  [Duyệt đề xuất công việc]
    - [ ] Xem
    - [ ] Duyệt

  [Tiến độ]
    - [ ] Xem
    - [ ] Cập nhật
    - [ ] Duyệt

[TÀI CHÍNH]
  [Đề xuất ngân sách]
    - [ ] Xem
    - [ ] Thêm

  [Duyệt ngân sách]
    - [ ] Xem
    - [ ] Duyệt

  [Ngân sách]
    - [ ] Xem

  [Chi phí]
    - [ ] Xem
    - [ ] Thêm
    - [ ] Sửa

[ĐÁNH GIÁ]
  [Đánh giá dự án]
    - [ ] Xem
    - [ ] Đánh giá
    - [ ] Sửa
    - [ ] Duyệt

  [Đánh giá nhân viên]
    - [ ] Xem
    - [ ] Đánh giá
    - [ ] Sửa
    - [ ] Duyệt

[CHAT]
  [Chat dự án]
    - [ ] Xem
    - [ ] Gửi

[AI]
  [AI]
    - [ ] Xem AI

  [Tổng quan AI]
    - [ ] Xem tổng quan AI

  [Dữ liệu AI]
    - [ ] Quản lý dữ liệu AI

  [Huấn luyện AI]
    - [ ] Huấn luyện AI

  [Phân tích nguyên nhân trễ]
    - [ ] Phân tích
    - [ ] Xác nhận

[BÁO CÁO]
  [Thống kê]
    - [ ] Xem
    - [ ] Xuất file

  [Nhật ký]
    - [ ] Xem
```

Ghi chú UI: mỗi nhóm nên có nút `Chọn nhóm` và `Bỏ chọn nhóm`. Mỗi quyền con nên hiển thị tooltip nêu quyền cha bắt buộc, ví dụ `Cần quyền Xem`.

## UI Architecture Decision

### Cấu trúc giao diện

Màn hình phân quyền sử dụng kiến trúc:

Card Group + Accordion

Không dùng:

* Table dài toàn trang
* Tree View nhiều cấp
* Tab quá nhiều tầng

### Bố cục

Header Card

Toolbar:

* Chọn vai trò
* Tìm kiếm
* Chọn tất cả
* Bỏ chọn tất cả
* Lưu quyền

Accordion nhóm chức năng:

* Hệ thống
* Dự án
* Công việc
* Tài chính
* Đánh giá
* Chat
* AI
* Báo cáo

Trong mỗi Accordion:

Card màn hình

Trong mỗi Card:

Danh sách quyền dạng pill checkbox.

### Màu sắc

Tuân thủ Dashboard hiện tại.

Không tạo theme mới.

Không thay đổi sidebar.

Không thay đổi layout tổng thể.

### Responsive

Desktop:

* Accordion đầy đủ.

Tablet:

* Accordion thu gọn.

Mobile:

* Card xếp dọc.

Không dùng table ngang.

### Quy tắc hiển thị quyền

Quyền cha:

* Hiển thị đầu tiên.
* Màu nổi bật hơn.

Quyền con:

* Hiển thị phía dưới.
* Disable khi thiếu quyền cha.

### Quy tắc hiển thị role constraint

Nếu role không được phép giữ quyền:

* Checkbox disabled.
* Có tooltip giải thích.

Ví dụ:

"Role Employee không được cấp quyền duyệt ngân sách."

### Quy tắc hiển thị dependency

Tooltip:

"Cần quyền Xem."

Hiển thị icon cảnh báo nếu dependency chưa hợp lệ.

## Frontend Behavior Rules

Các rule Javascript dưới đây đủ chi tiết để triển khai sau trong script riêng của màn phân quyền.

### Rule 1: Chọn quyền con

- Mỗi checkbox cần có metadata:
  - `data-permission="DuAn.Them"`
  - `data-parent-permission="DuAn.Xem"` nếu là quyền con
  - `data-group="DuAn"` hoặc nhóm nghiệp vụ
- Khi người dùng tick quyền con:
  - Tìm checkbox quyền cha theo `data-permission`.
  - Nếu quyền cha tồn tại và chưa được tick, tự tick quyền cha.
  - Bỏ trạng thái cảnh báo dependency của quyền con.

### Rule 2: Bỏ quyền cha

- Khi người dùng bỏ tick quyền cha:
  - Tìm tất cả checkbox có `data-parent-permission` bằng quyền cha.
  - Tự bỏ tick toàn bộ quyền con.
  - Có thể hiện confirm nếu quyền con đang được chọn, nội dung: `Bỏ quyền Xem sẽ bỏ toàn bộ quyền thao tác phụ thuộc.`
- Nếu người dùng hủy confirm thì giữ nguyên quyền cha và quyền con.

### Rule 3: Disable quyền con khi thiếu quyền cha

- Sau mỗi lần thay đổi checkbox, chạy hàm `syncDependencyState()`.
- Với mỗi quyền con:
  - Nếu quyền cha chưa chọn, disable quyền con và bỏ tick quyền con.
  - Nếu quyền cha đã chọn, enable quyền con.
- Ngoại lệ: khi người dùng tick quyền con trực tiếp, Rule 1 chạy trước để tự tick quyền cha, sau đó quyền con vẫn enabled.

### Rule 4: Chọn tất cả theo nhóm

- Nút chọn nhóm cần có `data-select-group="HeThong"` hoặc group key tương ứng.
- Khi bấm:
  - Tick toàn bộ quyền cha trong nhóm trước.
  - Tick toàn bộ quyền con trong nhóm sau.
  - Gọi `syncDependencyState()`.
- Với role đang bị role constraint, quyền bị cấm phải disabled và không được tick.

### Rule 5: Bỏ chọn tất cả theo nhóm

- Nút bỏ chọn nhóm cần có `data-clear-group="HeThong"`.
- Khi bấm:
  - Bỏ tick toàn bộ checkbox trong nhóm.
  - Gọi `syncDependencyState()`.
- Nếu nhóm có quyền bắt buộc của role hiện tại, ví dụ Admin bắt buộc giữ `PhanQuyen.Xem`, `PhanQuyen.Luu`, các quyền này không được bỏ tick.

### Rule 6: Tìm kiếm/lọc quyền

- Ô tìm kiếm lọc theo:
  - tên màn hình tiếng Việt
  - tên quyền hiển thị
  - key kỹ thuật
  - nhóm chức năng
- Mỗi block quyền cần có metadata text như `data-search-text="du an dự án DuAn DuAn.Xem xem"`.
- Khi nhập từ khóa:
  - Ẩn block không khớp.
  - Giữ nhóm cha hiển thị nếu còn ít nhất một block con khớp.
  - Hiển thị trạng thái rỗng nếu không có kết quả.

### Rule 7: Hiển thị cảnh báo dependency trước khi lưu

- Trước submit form, chạy validate client:
  - Tìm mọi quyền con đang checked nhưng quyền cha chưa checked.
  - Tìm mọi quyền bị role constraint nhưng vẫn checked.
  - Tìm Admin thiếu quyền bắt buộc.
- Nếu có lỗi:
  - Chặn submit.
  - Hiển thị danh sách lỗi ở đầu form.
  - Scroll tới cảnh báo.
- Nếu không có lỗi:
  - Cho submit bình thường.
- Client validation chỉ hỗ trợ trải nghiệm; backend vẫn phải validate lại đầy đủ trong `SaveRolePermissionsAsync`.

### View cần sửa

- `Views/PhanQuyen/Index.cshtml` nên chuyển từ bảng hai cột sang layout nhóm:
  - Bộ chọn vai trò ở đầu trang.
  - Thanh tìm kiếm/lọc nhóm quyền.
  - Accordion hoặc card theo nhóm nghiệp vụ: Hệ thống, Dự án, Công việc, Tài chính, Đánh giá, AI, Chat/Báo cáo.
  - Mỗi màn hình là một block nhỏ, quyền hiển thị bằng nhãn tiếng Việt.
  - Quyền cha `Xem` đặt nổi bật ở đầu nhóm màn hình.
- Có thể giữ table bên trong từng accordion nếu cần so sánh nhanh, nhưng không nên để toàn bộ quyền thành một bảng dài.

### JS cần thêm

- Khi chọn quyền con, tự tick quyền cha `*.Xem`.
- Khi bỏ quyền `*.Xem`, tự bỏ quyền con cùng màn hình hoặc hiện confirm trước khi bỏ.
- Disable quyền con nếu quyền cha chưa chọn, trừ khi người dùng vừa chọn quyền con thì tự tick cha.
- `Chọn tất cả`/`Bỏ chọn tất cả` theo từng nhóm nghiệp vụ và toàn bộ.
- Tìm kiếm/lọc theo tên màn hình tiếng Việt, tên key kỹ thuật và tên quyền.
- Hiển thị cảnh báo trước khi lưu nếu còn dependency vi phạm.

### CSS cần thêm/sửa

- Giữ phong cách Dashboard/sidebar: nền sáng, border `#e4e9f3`, shadow nhẹ, header gradient nhẹ.
- Giảm lạm dụng card lồng nhau: page container, header, toolbar và nhóm quyền nên rõ ràng nhưng không quá nhiều lớp card.
- Button nên dùng cùng nhịp với `.dashboard-action-btn` hoặc chuẩn hóa `.btn-action` dùng chung.
- Checkbox quyền nên dùng pill/badge nhẹ, nhưng có trạng thái disabled rõ ràng và label ngắn.
- Thêm layout responsive:
  - Desktop: 2 cột nhóm hoặc accordion full-width.
  - Tablet/mobile: mỗi màn hình một block, checkbox xuống dòng tự nhiên.
  - Tránh bảng quá rộng trên mobile.

### Cách hiển thị tiếng Việt

Đề xuất mapping màn hình:

| Key hiện tại | Tên hiển thị |
|---|---|
| `Dashboard` | Tổng quan hệ thống |
| `AIDashboard` | Tổng quan AI |
| `AIDataset` | Dữ liệu AI |
| `AITrain` | Huấn luyện AI |
| `AIPredict` | Phân tích nguyên nhân trễ |
| `AIXacNhan` | Xác nhận nguyên nhân trễ |
| `NhanSu` | Nhân sự |
| `ChucDanh` | Chức danh |
| `PhanQuyen` | Phân quyền |
| `Nhom` | Nhóm |
| `ThanhVienNhom` | Thành viên nhóm |
| `DuAn` | Dự án |
| `YeuCauDoiQuanLy` | Yêu cầu đổi quản lý |
| `DuyetYeuCauDoiQuanLy` | Duyệt yêu cầu đổi quản lý |
| `TeamDuAn` | Team dự án |
| `ThanhVienDuAn` | Thành viên dự án |
| `DanhMucCongViec` | Danh mục công việc |
| `CongViec` | Công việc |
| `ChiTietCongViec` | Chi tiết công việc |
| `PhanCongCongViec` | Phân công công việc |
| `PhanCongChiTietCongViec` | Phân công chi tiết công việc |
| `DeXuatCongViec` | Đề xuất công việc |
| `DuyetDeXuatCongViec` | Duyệt đề xuất công việc |
| `TienDo` | Tiến độ |
| `DeXuatNganSach` | Đề xuất ngân sách |
| `DuyetNganSach` | Duyệt ngân sách |
| `NganSach` | Ngân sách |
| `ChiPhi` | Chi phí |
| `DanhGiaDuAn` | Đánh giá dự án |
| `DanhGiaNhanVien` | Đánh giá nhân viên |
| `Chat` | Chat dự án |
| `NhatKy` | Nhật ký |

Đề xuất mapping hậu tố quyền:

| Hậu tố/key | Tên hiển thị |
|---|---|
| `.Xem` | Xem |
| `.Them` | Thêm |
| `.Sua` | Sửa |
| `.Xoa` | Xóa |
| `.Duyet` | Duyệt |
| `.Luu` | Lưu cấu hình |
| `.CapNhat` | Cập nhật |
| `.Gui` | Gửi |
| `.XacNhan` | Xác nhận |
| `.Khoa` | Khóa |
| `.MoKhoa` | Mở khóa |
| `.ThucHien` | Thực hiện |
| `.DanhGia` | Đánh giá |
| `.XuatFile` | Xuất file |
| `AI.Dataset` | Quản lý dữ liệu AI |
| `AI.Train` | Huấn luyện AI |
| `AI.PhanTichNguyenNhan` | Phân tích nguyên nhân trễ |
| `AI.Dashboard` | Xem tổng quan AI |
| `AI.XacNhan` | Xác nhận nguyên nhân trễ |

## 9. Danh sách file cần chỉnh sửa ở bước triển khai sau

| File | Mục đích sửa |
|---|---|
| `Constants/Permissions.cs` | Cân nhắc thêm `AI.Xem`; chuẩn hóa comment/nhóm quyền nếu cần. |
| `Data/KhoiTaoTaiKhoanMacDinh.cs` | Seed tên hiển thị/mô tả tiếng Việt; sửa quyền mặc định Employee bỏ `TienDo.Duyet`; bổ sung rule quyền theo role. |
| `Services/Implementations/PhanQuyenService.cs` | Enforce cha-con, role rule, quyền bắt buộc, trả dữ liệu display/group/dependency cho view. |
| `Services/Interfaces/IPhanQuyenService.cs` | Bổ sung contract nếu page model cần rule/display metadata. |
| `Services/Implementations/PermissionHelper.cs` | Cân nhắc thêm helper `HasAllPermissionsAsync` hoặc API rõ hơn cho nhiều quyền. |
| `Services/Implementations/AccountService.cs` | Giữ cách nạp claim, nhưng nên chuẩn hóa alias role và claim `IsLeader` nếu dùng rộng. |
| `Services/Implementations/*Service.cs` nghiệp vụ | Gom logic current user/role/scope vào service dùng chung; kiểm tra lại Admin bị chặn ở các nghiệp vụ cần chặn. |
| `ViewModels/PhanQuyen/*` | Thêm display name, group name, parent permission, child permission, warning/description. |
| `Views/PhanQuyen/Index.cshtml` | Chuyển UI sang nhóm/accordion/card; thêm tìm kiếm, checkbox dependency, nút theo nhóm, nhãn tiếng Việt. |
| `wwwroot/css/PhanQuyen/index.css` | Chuẩn hóa layout nhóm quyền, checkbox pill, toolbar, responsive, đồng bộ Dashboard/sidebar. |
| `Views/Shared/_Layout.cshtml` | Nếu thêm quyền mới như `AI.Xem`, cập nhật rule hiển thị menu AI. |
| `wwwroot/js/site.js` hoặc JS riêng cho PhanQuyen | Tách JS dependency/search/select group khỏi Razor inline script nếu triển khai lớn. |

## 10. Kết luận

Hệ thống đã có nền tảng phân quyền claim-based khá rõ: quyền được seed, lưu ở role claims, nạp vào cookie khi đăng nhập, controller check bằng `PermissionHelper`, menu ẩn/hiện theo quyền. Một số service đã làm đúng nguyên tắc "có claim vẫn phải đúng scope dữ liệu", đặc biệt ở dự án, công việc, tiến độ và chat.

Vấn đề chính không nằm ở việc thiếu cơ chế claim, mà nằm ở thiếu rule tập trung. Hiện chưa có ràng buộc quyền cha-con, chưa validate quyền theo role khi lưu, UI còn hiển thị key kỹ thuật và chưa hỗ trợ dependency giữa checkbox. Hướng triển khai phù hợp nhất là thêm một lớp rule/provider tập trung cho permission metadata và dependency, cập nhật `SaveRolePermissionsAsync` để enforce backend, sau đó cải tiến `Views/PhanQuyen/Index.cshtml` và CSS/JS để người dùng cấu hình quyền theo nhóm nghiệp vụ bằng tiếng Việt, dễ hiểu và khó lưu sai.

## Source Of Truth

Khi triển khai phân quyền, thứ tự ưu tiên nguồn dữ liệu chính thức là:

1. `Permissions.cs`
2. `PermissionDependencyProvider`
3. Role Constraint Matrix
4. Backend Validation Rules
5. Frontend Behavior Rules

Nếu có mâu thuẫn giữa các nguồn trên, `Permissions.cs` là nguồn dữ liệu gốc.

Không được tự suy diễn ngoài các nguồn trên.

## Thứ tự triển khai đề xuất

Bước 1: Tạo `PermissionDependencyProvider`

- File cần thêm: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PermissionDependencyProvider.cs`.
- File cần thêm: `QuanLyDuAn/QuanLyDuAn/Services/Interfaces/IPermissionDependencyProvider.cs`.
- Nội dung: trả về dependency matrix, display group, display name, role constraints và quyền bắt buộc theo role.

Bước 2: Sửa `Permissions.cs` và seed quyền AI

- File cần sửa: `QuanLyDuAn/QuanLyDuAn/Constants/Permissions.cs`.
- File cần sửa: `QuanLyDuAn/QuanLyDuAn/Data/KhoiTaoTaiKhoanMacDinh.cs`.
- Nội dung: thêm `AI.Xem`, seed vào `DanhMucQuyen`, cập nhật seed quyền AI mặc định theo role.

Bước 3: Sửa `PhanQuyenService`

- File cần sửa: `QuanLyDuAn/QuanLyDuAn/Services/Implementations/PhanQuyenService.cs`.
- Nội dung: inject provider, normalize dependency, validate role constraint, giữ quyền bắt buộc, trả lỗi rõ ràng, giữ transaction hiện có.

Bước 4: Sửa ViewModel

- File cần sửa: `QuanLyDuAn/QuanLyDuAn/ViewModels/PhanQuyen/PhanQuyenPageViewModel.cs`.
- File cần sửa: `QuanLyDuAn/QuanLyDuAn/ViewModels/PhanQuyen/PhanQuyenNhomManHinhViewModel.cs`.
- File cần sửa: `QuanLyDuAn/QuanLyDuAn/ViewModels/PhanQuyen/PhanQuyenItemViewModel.cs`.
- Nội dung: thêm `DisplayName`, `GroupName`, `ParentPermission`, `IsParent`, `IsDeniedByRole`, `IsRequiredByRole`, `DependencyMessage`.

Bước 5: Sửa View

- File cần sửa: `QuanLyDuAn/QuanLyDuAn/Views/PhanQuyen/Index.cshtml`.
- Nội dung: chuyển từ bảng hai cột sang nhóm/accordion, thêm toolbar tìm kiếm, nút chọn/bỏ theo nhóm, metadata `data-*` cho checkbox.

Bước 6: Thêm Javascript

- File nên thêm: `QuanLyDuAn/QuanLyDuAn/wwwroot/js/phanquyen/index.js`.
- File cần sửa: `QuanLyDuAn/QuanLyDuAn/Views/PhanQuyen/Index.cshtml` để reference script.
- Nội dung: implement 7 frontend behavior rules, client validation trước submit, đồng bộ disabled/checked theo dependency.

Bước 7: Sửa CSS

- File cần sửa: `QuanLyDuAn/QuanLyDuAn/wwwroot/css/PhanQuyen/index.css`.
- Nội dung: style group/accordion, toolbar, permission pill, trạng thái disabled, cảnh báo dependency, responsive theo Dashboard/sidebar hiện tại.

Bước 8: Cập nhật menu AI nếu có `AI.Xem`

- File cần sửa: `QuanLyDuAn/QuanLyDuAn/Views/Shared/_Layout.cshtml`.
- Nội dung: dùng `AI.Xem` làm điều kiện hiển thị nhóm AI; từng menu con vẫn dùng quyền con tương ứng.

Bước 9: Đăng ký service

- File cần sửa: `QuanLyDuAn/QuanLyDuAn/Services/CauHinhDichVu.cs` hoặc nơi đăng ký DI hiện tại.
- Nội dung: đăng ký `IPermissionDependencyProvider`.

Bước 10: Kiểm thử

- File cần kiểm thử thủ công: `Views/PhanQuyen/Index.cshtml`.
- Lệnh kiểm thử đề xuất: `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore /p:UseAppHost=false`.
- Kịch bản kiểm thử:
  - Chọn quyền con tự tick quyền cha.
  - Bỏ quyền cha tự bỏ quyền con.
  - Employee không lưu được quyền duyệt/phân quyền.
  - Admin không mất quyền bắt buộc.
  - `AI.Xem` điều khiển toàn bộ quyền AI con.
  - Menu và action nghiệp vụ vẫn giữ workflow hiện tại.
