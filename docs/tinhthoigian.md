# Rà soát logic ngày giờ toàn hệ thống

## 1. Phạm vi và phương pháp rà soát

Phạm vi rà soát là source hiện tại của `QuanLyDuAn/QuanLyDuAn` và `QuanLyDuAnAIService`, gồm Controller, Service, Entity, ViewModel, Razor View, JavaScript, helper export và FastAPI. Lượt này chỉ phân tích tĩnh, không chạy migration, không gọi database, không gọi tổng hợp `AI_DATASET`, không train model và không sửa source.

Các nhóm từ khóa đã quét: `.Date`, `DateTime.Today`, `DateTime.Now`, `DateOnly`, `DateDiffDay`, `TotalDays`, `TotalHours`, `TimeSpan`, `type="date"`, `datetime-local`, `yyyy-MM-dd`, `dd/MM/yyyy`, `new Date`, `toISOString`, `setHours`, `datetime.utcnow`, `Ngay*`, `ThoiGian*`, `Tre`, `QuaHan`, `DungHan`, `Duyet`, `PhanCong`.

Kết quả quét rộng: 227 file có dấu vết ngày giờ hoặc trường ngày giờ; 52 dòng dùng `.Date`/`DateTime.Today`/`DateOnly` trong 12 file chính; 25 input `type="date"` trong Razor. Các kết luận bên dưới dựa trên đọc ngữ cảnh phương thức, không chỉ dựa vào từ khóa.

## 2. Quy ước thời gian hiện tại của hệ thống

Hệ thống đang lưu đa số mốc nghiệp vụ bằng `DateTime?` và SQL `datetime2(7)`, ví dụ `NgayKetThucDuAn`, `NgayHoanThanhThucTeDuAn`, `NgayKetThucCVDuKien`, `NgayKetThucCVThucTe`, `ThoiGianCapNhat`, `ThoiGianDuyet`, `NgayDeXuat`, `NgayDuyet`, `NgayGiaoViec`, `NgayThamGiaDuAn`. Phần MVC chủ yếu dùng `DateTime.Now` theo giờ local của server.

FastAPI chủ yếu dùng `datetime.utcnow()` cho metadata model, health check, log và thời điểm train. Chưa thấy FastAPI tự tính trễ hạn nghiệp vụ từ các mốc dự án/công việc; contract nhận `NgayTongHop: datetime | None`.

Quy ước chưa thống nhất: một số module hiểu deadline theo timestamp đầy đủ, một số module cắt về ngày lịch bằng `.Date`, và một số màn hình dùng `type="date"` làm mốc nhập bị đặt về `00:00:00`.

## 3. Các khái niệm thời gian nghiệp vụ

### 3.1. Dự án trễ

Nhóm A. Nên dùng `NgayHoanThanhThucTeDuAn > NgayKetThucDuAn` bằng full `DateTime`. Dashboard, AI dataset và một phần `DuAnService` đã đi theo hướng này; `DanhGiaDuAnService` vẫn có đoạn tính theo `.Date`.

### 3.2. Công việc trễ

Nhóm A. Nên dùng `NgayKetThucCVThucTe > NgayKetThucCVDuKien` hoặc `now > NgayKetThucCVDuKien` cho công việc chưa hoàn thành. `DashboardService`, `DuAnService`, `AiDatasetService` dùng full `DateTime`; `CongViecService` và `DanhGiaDuAnService` vẫn dùng `.Date`.

### 3.3. Chi tiết công việc quá hạn

Nhóm A/C. Hiện chi tiết chỉ có `NgayKetThucCTCV` là ngày hoàn thành thực tế, chưa thấy trường deadline riêng cho chi tiết. Nếu nghiệp vụ coi chi tiết có hạn riêng thì cần xác nhận cột nào là hạn, nếu không không nên suy diễn `NgayKetThucCTCV` là deadline.

### 3.4. Báo cáo tiến độ

Nhóm A. `TienDoCongViecService` dùng `ThoiGianCapNhat`, `ThoiGianDuyet`, order theo timestamp và có khóa phụ `MaTienDo`. Bộ lọc ngày lịch dùng `[tuNgay, denNgay + 1)` là hợp lý cho lọc lịch.

### 3.5. Thời gian duyệt

Nhóm A. Đề xuất công việc, đề xuất ngân sách, tiến độ và đánh giá đều ghi thời điểm duyệt bằng `DateTime.Now`. Khi tính thời gian trung bình đang dùng `TotalDays`, giữ được phần giờ nhưng đơn vị hiển thị là ngày thập phân.

### 3.6. Phân công và nhân sự

Nhóm A/C. `NgayGiaoViec`, `NgayGiaoCTCV`, `NgayThamGiaDuAn`, `NgayTeamThamGiaDA` được ghi bằng `DateTime.Now`. Chưa thấy kiểm tra thứ tự timestamp "tham gia trước phân công"; nếu nghiệp vụ yêu cầu thì cần bổ sung ở bước sửa source.

### 3.7. Thay đổi quản lý

Nhóm A. Yêu cầu đổi quản lý ghi `NgayTaoYeuCauDoiQuanLy`, `NgayDuyetYeuCauDoiQuanLy` và nhật ký bằng timestamp. Query "quản lý hiện tại" đã order theo `QLDATuNgay ?? NkThoiGianQLDA ?? DateTime.MinValue`, nhưng cần khóa phụ nếu nhiều dòng cùng timestamp.

## 4. Các module đã sử dụng đầy đủ ngày giờ

| Module | File/phương thức | Nhận xét |
|---|---|---|
| Dashboard | `DashboardService` | Dự án trễ, công việc trễ, top công việc trễ dùng full `DateTime` và `Ceiling(TotalDays)`. |
| Dự án | `DuAnService.GanTinhTrangThoiHanAsync`, `TinhSoNgayTre` | Nhiều logic deadline chi tiết đã dùng full timestamp. |
| AI dataset | `AiDatasetService.BuildProjectSnapshotsAsync` | Công việc trễ, dự án trễ, chậm cập nhật tiến độ dùng full timestamp. |
| AI phân tích | `AiService` | Kiểm tra quá hạn dự án, xác nhận nguyên nhân, chọn dataset mới nhất dùng timestamp và có khóa phụ ở vài nơi. |
| Tiến độ | `TienDoCongViecService` | Gửi/duyệt báo cáo dùng `ThoiGianCapNhat`, `ThoiGianDuyet`; chọn báo cáo mới nhất có `ThenByDescending(MaTienDo)`. |
| Chat/Nhật ký/File | `ChatDuAnService`, `FileDuAnService`, `ExportFileService` | Chủ yếu ghi và hiển thị timestamp, không ảnh hưởng deadline. |

## 5. Các module còn làm mất phần giờ

| Module | Vị trí | Vấn đề chính |
|---|---|---|
| Công việc | `CongViecService.ApDungLocTinhTrangThoiHan`, `GanThongTinThoiHan` | Dùng `DateTime.Today` và `.Date` để quyết định quá hạn, hoàn thành trễ, hoàn thành đúng hạn. |
| Đánh giá dự án | `DanhGiaDuAnService.LoadThongTinHoTroDanhGiaAsync`, `GanTrangThaiThoiHanDuAn` | Tính công việc trễ, chi tiết quá hạn, số ngày chậm cập nhật và dự án đúng/trễ bằng `.Date`. |
| Chi tiết công việc | `ChiTietCongViecService.CreateAsync/UpdateAsync` | Ghi `NgayBatDauCTCV = form.Value.Date`, input là `type="date"`; nếu trường này cần giờ thì mất giờ khi lưu. |
| Đề xuất công việc | `DeXuatCongViecService.CreateAsync` và `_Form.cshtml` | Mốc bắt đầu/kết thúc đề xuất nhập bằng `type="date"` và so với dự án bằng `.Date`. |
| Dự án create/edit | `DuAn/_Form.cshtml`, `DuAnCreateUpdateViewModel` | `NgayBatDauDuAn`, `NgayKetThucDuAn` nhập `type="date"` nên deadline mặc định 00:00. |
| Export | `ExportSupport.FormatDate`, các controller export | Một số cột deadline/thực tế xuất chỉ `dd/MM/yyyy`, làm mất giờ khi người đọc đối chiếu trễ trong cùng ngày. |

## 6. Các module chỉ cần sử dụng ngày

| Module | File | Chức năng | Lý do chỉ cần ngày | Có nguy cơ ảnh hưởng deadline không |
|---|---|---|---|---|
| Dashboard | `DashboardService.ChuanHoaKhoangThoiGian` | Lọc lịch theo ngày | Người dùng chọn ngày lịch; code dùng `< denNgay + 1` | Thấp, nếu không dùng làm deadline |
| Tiến độ | `TienDoCongViecService.ChuanHoaKhoangNgay` | Lọc báo cáo theo ngày | Lọc lịch và query dùng khoảng nửa mở | Thấp |
| Đánh giá dự án/nhân viên | `DanhGiaDuAnService`, `DanhGiaNhanVienService` | Lọc ngày đánh giá | Ngày đánh giá trong filter là lịch | Thấp |
| Nhân sự | `TaiKhoanCaNhan`, `NhanSu` | Ngày sinh | Ngày sinh không cần giờ | Không |
| Thống kê tháng | `DashboardService`, `AiService` | Group theo tháng | Báo cáo tổng hợp theo lịch | Không |

## 7. Các trường hợp cần xác nhận nghiệp vụ

1. `NgayKetThucDuAn` và `NgayKetThucCVDuKien` có phải deadline đúng timestamp hay chỉ cuối ngày lịch?
2. Khi form hiện chỉ `type="date"`, người dùng có cần nhập giờ cho ngày bắt đầu/kết thúc dự án và công việc không?
3. Chi tiết công việc có deadline riêng không, hay chỉ có ngày hoàn thành thực tế?
4. `NgayThamGiaDuAn` có hiệu lực từ đầu ngày hay đúng thời điểm được thêm?
5. Phân công trước khi nhân sự tham gia cùng ngày nhưng khác giờ có bị coi là không hợp lệ không?
6. Số ngày trễ hiển thị nên là số ngày làm tròn lên từ giờ trễ, số ngày lịch, hay số ngày đủ 24 giờ?

## 8. Rà soát chức năng dự án

`DuAnService` đã có logic deadline tốt hơn ở các phần danh sách và chi tiết: `ApplyDeadlineFilter(..., DateTime.Now)`, `GanTinhTrangThoiHanAsync` so sánh `cv.NgayKetThucCVThucTe.Value > cv.NgayKetThucCVDuKien.Value`, dự án hoàn thành đúng hạn dùng `NgayHoanThanhThucTeDuAn <= NgayKetThucDuAn`.

Rủi ro còn lại nằm ở input create/edit: `Views/DuAn/_Form.cshtml:40,46` dùng `type="date"`. Nếu hệ thống cần hạn đến đúng 10:00 hay 18:00, mốc nhập sẽ bị lưu ở 00:00. Danh sách/chi tiết dự án cũng hiển thị `NgayKetThucDuAn` dạng `dd/MM/yyyy`, trong khi `NgayHoanThanhThucTeDuAn` ở chi tiết hiển thị `dd/MM/yyyy HH:mm`.

## 9. Rà soát chức năng công việc

`CongViecService` là vùng cần sửa ưu tiên. `GetPageAsync` đặt `homNay = DateTime.Today`; `ApDungLocTinhTrangThoiHan` dùng `x.NgayKetThucCVDuKien.Value.Date < homNay`, `NgayKetThucCVThucTe.Value.Date > NgayKetThucCVDuKien.Value.Date`; `GanThongTinThoiHan` tiếp tục cắt `ngayDuKien` và `ngayThucTe`.

Hậu quả: hạn `20/06/2026 10:00`, hoàn thành `20/06/2026 18:00` bị xem là đúng hạn trong danh sách công việc, filter "Hoàn thành trễ" và export. Màn hình `Views/CongViec/_Table.cshtml` cũng hiển thị bắt đầu/hạn/hoàn tất chỉ `dd/MM/yyyy`.

## 10. Rà soát chi tiết công việc

`ChiTietCongViecService` mặc định `NgayBatDauCTCV = DateTime.Today`, khi create/update gán `form.NgayBatDauCTCV!.Value.Date`. Form và inline edit dùng `type="date"`. Điều này đúng nếu ngày bắt đầu chi tiết chỉ là ngày lịch. Nếu cần kiểm tra "bắt đầu lúc 14:00 nhưng 09:00 vẫn chưa bắt đầu" thì đây là lỗi mất giờ.

Ngày hoàn thành chi tiết được ghi trong `TienDoCongViecService` bằng `DateTime.Now` khi báo cáo được duyệt hoàn thành, giữ giờ đúng.

## 11. Rà soát tiến độ và duyệt tiến độ

`TienDoCongViecService` nhìn chung đúng cho timestamp: gửi báo cáo ghi `ThoiGianCapNhat = DateTime.Now`, duyệt ghi `ThoiGianDuyet = DateTime.Now`, chọn báo cáo mới nhất bằng `OrderByDescending(ThoiGianCapNhat).ThenByDescending(MaTienDo)`. Bộ lọc `tuNgayBaoCao/denNgayBaoCao` dùng ngày lịch và `< den + 1`, hợp lý.

Rủi ro cần xác nhận: "số ngày chậm cập nhật" ở `DanhGiaDuAnService` lại lấy `lanBaoCaoMoiNhat.Value.Date`, khác với AI dataset dùng full timestamp.

## 12. Rà soát đề xuất công việc

`DeXuatCongViecService.CreateAsync` kiểm tra ngày đề xuất không vượt dự án bằng `.Date`, và form đề xuất dùng `type="date"` cho `NgayBatDauCongViecDeXuat`, `NgayKetThucCVDeXuatDuKien`. Nếu đề xuất công việc cần hạn theo giờ, đây là vị trí mất giờ. Duyệt/từ chối đề xuất ghi `NgayDuyetDeXuatCongViec = DateTime.Now`, giữ giờ.

## 13. Rà soát đề xuất và duyệt ngân sách

Đề xuất ngân sách và duyệt ngân sách dùng `DateTime.Now` cho `NgayDeXuat`, `NgayDuyet`, `NgayCapNhatNganSach`, `NgayDuyetNganSach`, `ThoiGianNKNS`. Danh sách order theo `NgayDeXuat` và khóa phụ `MaDeXuatNS`. Chưa thấy mất giờ trong nghiệp vụ duyệt. Export ngân sách hiện dùng `FormatDate` cho ngày duyệt/cập nhật nên hiển thị thiếu giờ.

## 14. Rà soát nhân sự, phân công và team

`NhanVienDuAnService`, `TeamDuAnService`, `PhanCongCongViecService`, `PhanCongChiTietCongViecService` ghi timestamp bằng `DateTime.Now`. View phân công công việc và phân công chi tiết hiển thị `dd/MM/yyyy HH:mm`. `ThanhVienTeam/_Table.cshtml` và `DuAn/Details.cshtml` có nơi hiển thị ngày tham gia chỉ `dd/MM/yyyy`.

Chưa thấy ràng buộc kiểm tra phân công phải sau `NgayThamGiaDuAn`; đây là nhóm C nếu nghiệp vụ yêu cầu kiểm soát thứ tự trong cùng ngày.

## 15. Rà soát yêu cầu đổi quản lý

Tạo, duyệt và từ chối yêu cầu đổi quản lý dùng timestamp. View hiển thị ngày tạo/ngày duyệt bằng `dd/MM/yyyy HH:mm`. Query lấy nhật ký quản lý hiện tại order theo thời gian, nhưng nếu hai dòng cùng timestamp thì nên thêm khóa phụ ID để phá hòa.

## 16. Rà soát chức năng đánh giá dự án

| Nội dung | File/phương thức | Công thức hiện tại | Có giữ giờ | Có giống Dashboard/AI | Cần sửa |
|---|---|---|---|---|---|
| Điều kiện danh sách dự án có công việc trễ | `DanhGiaDuAnService.GetPageAsync` | `NgayKetThucCVDuKien < DateTime.Now` | Có | Gần giống | Không cấp bách |
| Công việc đang trễ | `LoadThongTinHoTroDanhGiaAsync` | `NgayKetThucCVDuKien.Value.Date < homNay` | Không | Không | Có |
| Công việc hoàn thành trễ | `LoadThongTinHoTroDanhGiaAsync` | `NgayKetThucCVThucTe.Value.Date > NgayKetThucCVDuKien.Value.Date` | Không | Không | Có |
| Chi tiết quá hạn | `LoadThongTinHoTroDanhGiaAsync` | `NgayKetThucCTCV.Value.Date < homNay` | Không | Chưa rõ vì thiếu deadline chi tiết | Cần xác nhận |
| Báo cáo mới nhất trong ngày | `LoadThongTinHoTroDanhGiaAsync` | Đếm mọi báo cáo trong ngày của lần mới nhất | Theo ngày | Khác AI | Cần xác nhận |
| Chậm cập nhật tiến độ | `LoadThongTinHoTroDanhGiaAsync` | `(moc.Date - lanBaoCao.Date).Days` | Không | Không | Có |
| Trạng thái thời hạn dự án | `GanTrangThaiThoiHanDuAn` | `(NgayKetThucThucTe.Date - NgayKetThucDuAn.Date).Days` | Không | Không | Có |
| Gọi AI | `GanDuLieuAi*` | Dựa AI dataset/kết quả AI | Giữ giờ hiển thị | Phụ thuộc dataset | Kiểm tra sau khi sửa |
| Hiển thị hỗ trợ | `_ProjectStats.cshtml` | Ngày kết thúc/thực tế hiển thị `dd/MM/yyyy` | Không | Không | Có |

Trả lời 12 câu hỏi trọng tâm: đánh giá dự án đang cho đánh giá theo trạng thái/điều kiện service hiện hữu; có xét phần trăm và dữ liệu hỗ trợ; có dùng ngày hoàn thành thực tế/fallback công việc cuối; có dùng `.Date` trong nhiều thống kê; có thể làm dự án hoàn thành trễ vài giờ cùng ngày bị xem đúng hạn; AI dataset hiện đã dùng công thức khác với `DanhGiaDuAn`; phần xác nhận nguyên nhân AI cần đồng bộ lại sau khi sửa công thức nguồn.

## 17. Rà soát dashboard và thống kê

Dashboard là module đã được harden tốt nhất cho deadline: dự án trễ hợp lệ yêu cầu `NgayHoanThanhThucTeDuAn > NgayKetThucDuAn` và có công việc trễ/vượt hạn dự án; công việc trễ dùng full timestamp; số ngày trễ làm tròn lên bằng `Ceiling(TotalDays)`. Bộ lọc ngày dashboard dùng `[tu, den + 1)`, phù hợp cho lọc lịch.

## 18. Rà soát các chức năng AI

`AiDatasetService` hiện dùng full timestamp cho `soCongViecTre`, `soNgayTreTienDo`, `soNgayChamCapNhatTienDo`, `laDuAnTre`. `AiService` kiểm tra quá hạn dự án bằng `now > NgayKetThucDuAn`, chọn dataset/kết quả mới nhất theo timestamp và có khóa phụ ở một số query.

Rủi ro không nằm ở FastAPI tính trễ, mà ở contract/metadata timezone: FastAPI dùng UTC cho model/log, MVC dùng local `DateTime.Now`. `Views/Ai/Models.cshtml` có `CreatedAt.ToLocalTime()`, cần thống nhất quy ước để tránh lệch khi hiển thị lịch sử model.

## 19. Rà soát Razor View và model binding

| View | Trường | Kiểu ViewModel | Kiểu input | Giá trị hiển thị | Giá trị submit | Có mất giờ | Đề xuất |
|---|---|---|---|---|---|---|---|
| `DuAn/_Form.cshtml` | `NgayBatDauDuAn` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Có nếu cần giờ | `datetime-local` hoặc quy ước cuối/ngày |
| `DuAn/_Form.cshtml` | `NgayKetThucDuAn` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Có | Ưu tiên `datetime-local` cho deadline |
| `DeXuatCongViec/_Form.cshtml` | `NgayBatDauCongViecDeXuat` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Có nếu cần giờ | Xác nhận nghiệp vụ |
| `DeXuatCongViec/_Form.cshtml` | `NgayKetThucCVDeXuatDuKien` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Có | Ưu tiên `datetime-local` nếu là hạn |
| `ChiTietCongViec/_Form.cshtml` | `NgayBatDauCTCV` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Có nếu cần giờ | Xác nhận nghiệp vụ |
| `ChiTietCongViec/_Table.cshtml` | inline `NgayBatDauCTCV` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Có | Không dùng nếu cần giờ |
| `CongViec/_Filter.cshtml` | `tuNgay/denNgay` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Không, đây là filter lịch | Giữ `[tu, den+1)` |
| `Dashboard/Index.cshtml` | `tuNgay/denNgay` | `DateTime?` | `date` | `yyyy-MM-dd` | 00:00 | Không, filter lịch | Giữ |
| `TaiKhoanCaNhan/CapNhat.cshtml` | `NgaySinh` | `DateTime?` | `date` | ngày sinh | 00:00 | Không | Giữ |

## 20. Rà soát JavaScript phía client

Không tìm thấy `new Date(...)`, `toISOString()`, `toLocaleDateString()`, `setHours(0,0,0,0)` trong JS ứng dụng ngoài thư viện. `wwwroot/js/ai/charts.js`, `site.js`, `layout/sidebar.js`, `approval/index.js`, `phanquyen/index.js` không thấy logic chuyển đổi ngày giờ nghiệp vụ. Rủi ro lệch timezone phía client hiện thấp.

## 21. Rà soát FastAPI và contract ngày giờ

FastAPI có các mốc `datetime.utcnow()` trong `model_service.py`, `decision_tree_model.py`, `log_service.py`, `health_router.py`, `admin_ai_service.py`. Đây là metadata hệ thống, không trực tiếp tính deadline. Schema `NgayTongHop: datetime | None` cho phép nhận timestamp. Cần ghi rõ contract MVC local time hay UTC khi gửi/nhận metadata model.

## 22. Các công thức đang không thống nhất

| Khái niệm | Module 1 | Công thức 1 | Module 2 | Công thức 2 | Module 3 | Công thức 3 | Công thức nên thống nhất |
|---|---|---|---|---|---|---|---|
| Dự án hoàn thành trễ | Dashboard | `ThucTe > Han` full `DateTime` | Đánh giá dự án | `ThucTe.Date - Han.Date` | AI dataset | `TinhSoNgayTre(full)` | `ThucTe > Han`, ngày trễ `Ceiling(TotalDays)` |
| Dự án đang quá hạn | AI/Dashboard | `now > Han` | Đánh giá dự án | `DateTime.Now.Date > Han.Date` | View | chỉ hiển thị ngày | `now > Han` |
| Công việc trễ | Dashboard/AI | full `DateTime` | Công việc | `.Date` | Đánh giá dự án | `.Date` | full `DateTime` |
| Công việc đang quá hạn | Dashboard | `Han < now` | Công việc | `Han.Date < Today` | Đánh giá dự án | `Han.Date < Today` | `Han < now` |
| Chi tiết quá hạn | Đánh giá dự án | `NgayKetThucCTCV.Date < Today` | Tiến độ | hoàn thành bằng `DateTime.Now` | AI | không rõ deadline chi tiết | cần xác nhận trường deadline |
| Số ngày trễ | Dashboard/AI/DuAn | `Ceiling(TotalDays)` | Công việc | `(Date.Date - Date.Date).Days` | Đánh giá dự án | `.Days` theo ngày | `Ceiling(TotalDays)` cho deadline |
| Duyệt chậm | AI/Đánh giá | `TotalDays` | View | hiển thị ngày thập phân | - | - | giữ `TotalDays`, cân nhắc hiển thị giờ |
| Báo cáo chậm cập nhật | AI dataset | full timestamp | Đánh giá dự án | `.Date` | Tiến độ | order timestamp | thống nhất full timestamp |
| Sắp đến hạn | DuAn detail | `TotalDays/TotalHours` | Công việc | `.Date >= Today` | Dashboard | full timestamp | full timestamp |
| Hoàn thành đúng hạn | DuAn | `ThucTe <= Han` | Công việc | `ThucTe.Date <= Han.Date` | Đánh giá dự án | `.Date` | `ThucTe <= Han` |

## 23. Danh sách vị trí cần chỉnh sửa

### 23.1. Mức nghiêm trọng

| Mức độ | Module | File | Lớp/phương thức | Dòng hoặc đoạn code | Logic hiện tại | Vấn đề | Logic đề xuất | Ảnh hưởng |
|---|---|---|---|---|---|---|---|---|
| Nghiêm trọng | Công việc | `Services/Implementations/CongViecService.cs` | `ApDungLocTinhTrangThoiHan` | `NgayKetThucCVThucTe.Value.Date > NgayKetThucCVDuKien.Value.Date` | So sánh ngày | Trễ vài giờ cùng ngày thành đúng hạn | `NgayKetThucCVThucTe > NgayKetThucCVDuKien` | Sai filter và badge công việc trễ |
| Nghiêm trọng | Công việc | `Services/Implementations/CongViecService.cs` | `GanThongTinThoiHan` | `ngayThucTe <= ngayDuKien` | Cắt giờ | Hiển thị đúng hạn sai | full `DateTime`, `Ceiling(TotalDays)` | Sai danh sách/export |
| Nghiêm trọng | Đánh giá dự án | `Services/Implementations/DanhGiaDuAnService.cs` | `GanTrangThaiThoiHanDuAn` | `(ThucTe.Date - Han.Date).Days` | Cắt giờ dự án | Dự án trễ vài giờ cùng ngày thành đúng hạn | `ThucTe > Han`, số ngày `Ceiling(TotalDays)` | Sai điều kiện phân tích/hiển thị đánh giá |
| Nghiêm trọng | Dự án form | `Views/DuAn/_Form.cshtml` | Create/Edit | `input type="date"` cho `NgayKetThucDuAn` | Submit 00:00 | Làm mất deadline giờ/phút | `datetime-local` hoặc quy ước nghiệp vụ rõ | Dữ liệu deadline sai từ đầu vào |

### 23.2. Mức cao

| Mức độ | Module | File | Lớp/phương thức | Dòng hoặc đoạn code | Logic hiện tại | Vấn đề | Logic đề xuất | Ảnh hưởng |
|---|---|---|---|---|---|---|---|---|
| Cao | Đánh giá dự án | `DanhGiaDuAnService.cs` | `LoadThongTinHoTroDanhGiaAsync` | công việc trễ dùng `.Date` | Khác Dashboard/AI | Sai số công việc trễ | full `DateTime` | Sai dữ liệu hỗ trợ |
| Cao | Đánh giá dự án | `DanhGiaDuAnService.cs` | `LoadThongTinHoTroDanhGiaAsync` | `moc.Date - lanBaoCao.Date` | Cắt giờ báo cáo | Sai chậm cập nhật trong cùng ngày | full `DateTime` | Sai thống kê tiến độ |
| Cao | Đề xuất công việc | `DeXuatCongViecService.cs` | `CreateAsync` | `model.NgayKetThuc...Value.Date > duAn.NgayKetThucDuAn.Value.Date` | Cho phép/khóa theo ngày | Không phân biệt cùng ngày khác giờ | full `DateTime` nếu deadline theo giờ | Sai ràng buộc đề xuất |
| Cao | Đề xuất công việc | `Views/DeXuatCongViec/_Form.cshtml` | Create | `type="date"` deadline đề xuất | Submit 00:00 | Mất giờ hạn công việc đề xuất | `datetime-local` nếu cần giờ | Dữ liệu công việc tạo sau duyệt sai |
| Cao | Chi tiết công việc | `ChiTietCongViecService.cs` | `CreateAsync/UpdateAsync` | `form.Value.Date` | Ghi mất giờ | Sai nếu start time có nghĩa | Không cắt `.Date` nếu cần timestamp | Sai trạng thái bắt đầu |
| Cao | Export | `CongViecController.cs` | `XuatFile` | `ExportSupport.FormatDate` cho hạn/thực tế | Xuất mất giờ | Người đọc thấy đúng hạn sai | `FormatDateTime` cho deadline/thực tế | Sai báo cáo |
| Cao | Export | `DanhGiaDuAnController.cs` | `XuatFile` | `NgayDanhGia` dùng `FormatDate` | Mất giờ đánh giá | Lịch sử duyệt khó đối chiếu | `FormatDateTime` | Sai audit |
| Cao | Export | `NganSachController.cs` | `XuatFile` | ngày duyệt/cập nhật dùng `FormatDate` | Mất giờ | Audit ngân sách thiếu thứ tự | `FormatDateTime` | Sai nhật ký ngoài hệ thống |
| Cao | AI/MVC timezone | `AiService`, FastAPI `model_service.py` | metadata model | MVC local, FastAPI UTC | Chưa có contract rõ | Lệch hiển thị model/history | Ghi quy ước timezone | Sai diễn giải thời điểm |

### 23.3. Mức trung bình

| Mức độ | Module | File | Lớp/phương thức | Dòng hoặc đoạn code | Logic hiện tại | Vấn đề | Logic đề xuất | Ảnh hưởng |
|---|---|---|---|---|---|---|---|---|
| Trung bình | Dự án view | `Views/DuAn/_Table.cshtml`, `Details.cshtml` | Hiển thị hạn | `dd/MM/yyyy` | Thiếu giờ | Người dùng khó thấy trễ cùng ngày | `dd/MM/yyyy HH:mm` cho deadline | Hiển thị |
| Trung bình | Công việc view | `Views/CongViec/_Table.cshtml` | Hiển thị hạn/thực tế | `dd/MM/yyyy` | Thiếu giờ | Badge và text không đủ bằng chứng | `dd/MM/yyyy HH:mm` | Hiển thị |
| Trung bình | Đánh giá dự án view | `_ProjectStats.cshtml` | Hạn/thực tế tham chiếu | `dd/MM/yyyy` | Thiếu giờ | Khác dữ liệu AI/Dashboard | `dd/MM/yyyy HH:mm` | Hiển thị |
| Trung bình | Thành viên team | `Views/ThanhVienTeam/_Table.cshtml` | `NgayThamGiaTeam` | `dd/MM/yyyy` | Thiếu giờ | Khó audit thứ tự | `dd/MM/yyyy HH:mm` nếu cần | Hiển thị |
| Trung bình | Đổi quản lý | `DuyetYeuCauDoiQuanLyService.cs` | order nhật ký | Chỉ order thời gian | Hòa timestamp | Có thể chọn sai dòng cùng giờ | thêm khóa phụ ID | Edge case |
| Trung bình | Model AI | `Views/Ai/Models.cshtml` | `ToLocalTime()` | Chuyển local hiển thị | Chưa rõ nguồn UTC/local | Lệch giờ | chuẩn hóa contract | Hiển thị |

### 23.4. Mức thấp

| Mức độ | Module | File | Lớp/phương thức | Dòng hoặc đoạn code | Logic hiện tại | Vấn đề | Logic đề xuất | Ảnh hưởng |
|---|---|---|---|---|---|---|---|---|
| Thấp | Filter lịch | nhiều `_Filter.cshtml` | `type="date"` | ngày lịch | Đúng mục đích | Không sửa quá mức | giữ `[tu, den+1)` | Thấp |
| Thấp | Ngày sinh | `TaiKhoanCaNhan`, `NhanSu` | `DataType.Date`, `type="date"` | ngày sinh | Đúng mục đích | Không sửa | giữ ngày | Không |
| Thấp | Tên file export | `ExportFileService.cs` | `yyyy-MM-dd` | ngày trong tên file | Không ảnh hưởng nghiệp vụ | Giữ | giữ | Không |

Tổng phân loại vị trí cần xử lý: Nhóm A bắt buộc full `DateTime`: 28; Nhóm B chỉ cần ngày: 20; Nhóm C cần xác nhận nghiệp vụ: 11. Lỗi nghiêm trọng: 4; cao: 9; trung bình: 6; thấp: 3.

## 24. Đề xuất quy tắc thống nhất toàn hệ thống

| Nghiệp vụ | Quy tắc TO-BE |
|---|---|
| Dự án hoàn thành trễ | `TrangThaiDuAn` hoàn thành/lưu trữ và `NgayHoanThanhThucTeDuAn > NgayKetThucDuAn` |
| Dự án đang quá hạn | Chưa hoàn thành và `DateTime.Now > NgayKetThucDuAn` |
| Công việc hoàn thành trễ | `NgayKetThucCVThucTe > NgayKetThucCVDuKien` |
| Công việc đang quá hạn | Chưa hoàn thành/chưa chờ xác nhận và `DateTime.Now > NgayKetThucCVDuKien` |
| Đúng hạn | `thucTe <= han` |
| Số ngày trễ deadline | `Ceiling((thucTe - han).TotalDays)`, tối thiểu 1 khi trễ dương |
| Bộ lọc ngày lịch | `Ngay >= tu.Date AND Ngay < den.Date.AddDays(1)` |
| Mốc mới nhất | `ThoiGian DESC, Id DESC` |
| Workflow | So sánh đúng timestamp, không dùng `.Date` |

Không đề xuất đổi schema, không đổi hàng loạt sang `DateTimeOffset`, không thêm migration.

## 25. Kế hoạch chỉnh sửa source ở bước tiếp theo

1. Chốt nghiệp vụ deadline dự án/công việc là timestamp hay cuối ngày lịch.
2. Sửa `CongViecService` để bỏ `.Date` trong deadline, giữ filter ngày lịch như hiện tại.
3. Sửa `DanhGiaDuAnService` để đồng bộ công thức với Dashboard/AI.
4. Sửa các form deadline quan trọng từ `type="date"` sang `datetime-local` nếu nghiệp vụ xác nhận cần giờ.
5. Sửa export/view hiển thị `dd/MM/yyyy HH:mm` cho các mốc deadline/thực tế/duyệt cần audit.
6. Bổ sung khóa phụ khi chọn "mới nhất/cuối cùng" ở các query còn thiếu.
7. Thêm test cho các trường hợp cùng ngày khác giờ.

## 26. Kết luận mức độ sẵn sàng viết prompt chỉnh sửa

Đã đủ bằng chứng để viết prompt sửa source cho các nhóm ưu tiên cao, đặc biệt `CongViecService`, `DanhGiaDuAnService`, form dự án/công việc và export hiển thị. Cần xác nhận nghiệp vụ trước khi sửa các trường đang nhập `type="date"` nhưng có thể vốn chỉ được thiết kế là ngày lịch.

## Phụ lục A. Danh sách toàn bộ file đã rà soát

Đã quét toàn bộ source trong `QuanLyDuAn/QuanLyDuAn` và `QuanLyDuAnAIService`, loại trừ `bin`, `obj`, `wwwroot/lib`, `__pycache__`. Các file đọc sâu gồm:

- `Services/Implementations/CongViecService.cs`
- `Services/Implementations/DuAnService.cs`
- `Services/Implementations/DashboardService.cs`
- `Services/Implementations/DanhGiaDuAnService.cs`
- `Services/Implementations/AiDatasetService.cs`
- `Services/Implementations/AiService.cs`
- `Services/Implementations/TienDoCongViecService.cs`
- `Services/Implementations/ChiTietCongViecService.cs`
- `Services/Implementations/DeXuatCongViecService.cs`
- `Services/Implementations/DyetDeXuatCongViecService.cs`
- `Services/Implementations/DeXuatNganSachService.cs`
- `Services/Implementations/DuyetDeXuatNganSachService.cs`
- `Services/Implementations/NhanVienDuAnService.cs`
- `Services/Implementations/TeamDuAnService.cs`
- `Services/Implementations/PhanCongCongViecService.cs`
- `Services/Implementations/PhanCongChiTietCongViecService.cs`
- `Services/Implementations/DuyetYeuCauDoiQuanLyService.cs`
- `Services/Implementations/YeuCauDoiQuanLyService.cs`
- `Helpers/ExportSupport.cs`
- `Views/DuAn/*`, `Views/CongViec/*`, `Views/ChiTietCongViec/*`, `Views/DanhGiaDuAn/*`, `Views/TienDoCongViec/*`, `Views/DeXuatCongViec/*`, `Views/DeXuatNganSach/*`, `Views/Ai/*`
- `QuanLyDuAnAIService/app/**/*.py`

## Phụ lục B. Danh sách tất cả vị trí dùng .Date hoặc DateOnly

Không tìm thấy `DateOnly`. Các vị trí `.Date`/`DateTime.Today` đáng chú ý:

- `CongViecService.cs`: filter và badge deadline công việc.
- `DanhGiaDuAnService.cs`: filter ngày đánh giá, thống kê công việc trễ, chi tiết trễ, chậm cập nhật, trạng thái thời hạn dự án.
- `ChiTietCongViecService.cs`: mặc định/gán ngày bắt đầu chi tiết.
- `DeXuatCongViecService.cs`: so sánh ngày đề xuất với ngày dự án.
- `DuAnService.cs`: kiểm tra ngày bắt đầu trong quá khứ, chuẩn hóa filter, phần deadline đã có nhiều chỗ full timestamp.
- `DashboardService.cs`: filter lịch/tháng; phần deadline chính đã full timestamp.
- `TienDoCongViecService.cs`: chuẩn hóa filter lịch.
- `DeXuatNganSachService.cs`, `DanhGiaNhanVienService.cs`: chuẩn hóa filter lịch.
- `TaiKhoanCaNhanService.cs`, `CapNhatTaiKhoanCaNhanViewModel.cs`: ngày sinh.

## Phụ lục C. Danh sách input làm mất giờ

- `Views/DuAn/_Form.cshtml`: `NgayBatDauDuAn`, `NgayKetThucDuAn`.
- `Views/DeXuatCongViec/_Form.cshtml`: `NgayBatDauCongViecDeXuat`, `NgayKetThucCVDeXuatDuKien`.
- `Views/ChiTietCongViec/_Form.cshtml` và `_Table.cshtml`: `NgayBatDauCTCV`.
- Các filter `Dashboard`, `DuAn`, `CongViec`, `TienDoCongViec`, `DanhGiaDuAn`, `DanhGiaNhanVien`, `DeXuatCongViec`, `DeXuatNganSach` dùng `type="date"` nhưng là filter lịch, không coi là lỗi.
- `NhanSu/_Form.cshtml`, `TaiKhoanCaNhan/CapNhat.cshtml`: `NgaySinh`, chỉ cần ngày.

## Phụ lục D. Danh sách phép tính số ngày

- `DashboardService.TinhSoNgayTre`: `Ceiling(TotalDays)`, đúng cho deadline.
- `DuAnService.TinhSoNgayTre`: `Ceiling(TotalDays)`, đúng cho deadline.
- `AiDatasetService.TinhSoNgayTre`: `Ceiling(TotalDays)`, đúng cho AI dataset.
- `CongViecService.GanThongTinThoiHan`: `(Date - Date).Days`, cần sửa.
- `DanhGiaDuAnService.GanTrangThaiThoiHanDuAn`: `(Date - Date).Days`, cần sửa.
- `DanhGiaDuAnService.LoadThongTinHoTroDanhGiaAsync`: `(moc.Date - lanBaoCao.Date).Days`, cần sửa/xác nhận.
- `AiDatasetService` và `DanhGiaDuAnService`: thời gian duyệt trung bình dùng `TotalDays`, giữ giờ nhưng cần thống nhất đơn vị hiển thị.

## Phụ lục E. Bản đồ công thức theo từng màn hình

| Màn hình | Dự án trễ | Công việc trễ | Báo cáo tiến độ | AI |
|---|---|---|---|---|
| Dashboard | full `DateTime` | full `DateTime` | thống kê theo timestamp | không gọi trực tiếp |
| Danh sách dự án | full ở service chính | full khi enrich | không chính | có panel AI ở chi tiết |
| Chi tiết dự án | full cho recent deadline | full cho recent work | timestamp | hiển thị timestamp |
| Danh sách công việc | `.Date` | `.Date` | không chính | không |
| Đánh giá dự án | `.Date` | `.Date` | một phần `.Date` | lấy AI nhưng công thức hỗ trợ khác |
| AI dataset | full `DateTime` | full `DateTime` | full `DateTime` | nguồn train/predict |
| Tiến độ | không chính | trạng thái detail theo workflow | full timestamp | không |

## Phụ lục F. Các trường hợp kiểm thử cần bổ sung

1. Hạn 10:00, hoàn thành 18:00 cùng ngày -> trễ.
2. Hạn 18:00, hoàn thành 10:00 cùng ngày -> đúng hạn.
3. Hạn bằng thực tế -> đúng hạn.
4. Trễ 1 phút -> trễ 1 ngày quy đổi nếu dùng `Ceiling(TotalDays)`.
5. Trễ 23 giờ -> trễ 1 ngày quy đổi.
6. Trễ 25 giờ -> trễ 2 ngày quy đổi.
7. Báo cáo gửi 10:00, duyệt 10:01 -> thời gian chờ > 0.
8. Báo cáo gửi và duyệt cùng timestamp -> không âm.
9. Đề xuất tạo 14:00, duyệt 13:00 -> không hợp lệ hoặc cần cảnh báo dữ liệu.
10. Người tham gia 15:00 nhưng phân công 09:00 cùng ngày -> cần xác nhận hợp lệ.
11. Công việc bắt đầu 14:00 nhưng lúc 09:00 vẫn chưa bắt đầu.
12. Bộ lọc đến ngày 20/06 phải lấy cả bản ghi 20/06 lúc 23:59.
13. Edit mốc 15:30 không làm thành 00:00.
14. Hai báo cáo cùng ngày phải chọn bản có timestamp mới hơn.
15. Hai bản cùng timestamp phải dùng ID làm khóa phụ.
16. Đánh giá dự án hoàn thành trễ vài giờ phải được phân loại trễ.
17. Dashboard, chi tiết, đánh giá và AI phải cho cùng kết quả.

## Kết quả chỉnh sửa source

Đã chỉnh source để các logic deadline và hoàn thành chính dùng full `DateTime` thay vì cắt về ngày lịch. Các thay đổi không sửa database, không sửa schema, không tạo migration, không chạy migration, không sửa dữ liệu SQL, không tổng hợp lại `AI_DATASET`, không train model và không đổi contract AI 22 feature.

## Các file đã chỉnh sửa

| File | Class/Module | Method/Vị trí | Logic trước | Logic sau | Ảnh hưởng database | Thao tác thủ công |
|---|---|---|---|---|---|---|
| `Services/Implementations/CongViecService.cs` | `CongViecService` | `GetPageAsync`, `ApDungLocTinhTrangThoiHan`, `GanThongTinThoiHan` | Dùng `DateTime.Today`, `.Date` và `(Date - Date).Days` cho deadline công việc | Dùng một `now = DateTime.Now`, so sánh `NgayKetThucCVThucTe > NgayKetThucCVDuKien`, `now > NgayKetThucCVDuKien`, tính trễ bằng `Ceiling(TotalDays)` | Không | Không |
| `Services/Implementations/DanhGiaDuAnService.cs` | `DanhGiaDuAnService` | `GetPageAsync`, `LoadThongTinHoTroDanhGiaAsync`, `GanTrangThaiThoiHanDuAn` | Một số thống kê công việc/dự án/chậm cập nhật còn dùng ngày lịch hoặc chỉ đếm đang quá hạn | Dùng full timestamp cho dự án trễ, công việc trễ, công việc chờ xác nhận, báo cáo mới nhất theo `ThoiGianCapNhat DESC, MaTienDo DESC`, số ngày trễ/chậm cập nhật bằng `Ceiling(TotalDays)` | Không | Không |
| `Services/Implementations/DeXuatCongViecService.cs` | `DeXuatCongViecService` | validation tạo đề xuất | So sánh mốc đề xuất với dự án theo `.Date`, cho phép cùng ngày dù sai giờ | So sánh `NgayBatDauCongViecDeXuat < NgayKetThucCVDeXuatDuKien` và nằm trong khoảng dự án bằng full timestamp | Không | Không |
| `Services/Implementations/PhanCongCongViecService.cs` | `PhanCongCongViecService` | `AddAsync`, `KiemTraNhanVienHopLeAsync` | Chỉ kiểm tra nhân viên thuộc dự án | Kiểm tra thêm `NgayThamGiaDuAn <= NgayGiaoViec` khi `NgayThamGiaDuAn` có giá trị; vẫn tương thích dữ liệu cũ null | Không | Không |
| `Services/Implementations/PhanCongChiTietCongViecService.cs` | `PhanCongChiTietCongViecService` | `AddAsync`, `KiemTraNhanVienHopLeAsync` | Chỉ kiểm tra thuộc dự án và đã phân công công việc cha | Kiểm tra thêm `NgayThamGiaDuAn <= NgayGiaoCTCV` khi `NgayThamGiaDuAn` có giá trị | Không | Không |
| `ViewModels/DuAn/DuAnCreateUpdateViewModel.cs` | `DuAnCreateUpdateViewModel` | `Validate` | Cho phép ngày kết thúc bằng ngày bắt đầu | Bắt buộc `NgayBatDauDuAn < NgayKetThucDuAn` bằng full timestamp | Không | Không |
| `Views/DuAn/_Form.cshtml` | form dự án | `NgayBatDauDuAn`, `NgayKetThucDuAn` | `type="date"`, edit có nguy cơ mất giờ | `type="datetime-local"`, value `yyyy-MM-ddTHH:mm`; hidden archive cũng giữ giờ/phút | Không | Không |
| `Views/DeXuatCongViec/_Form.cshtml` | form đề xuất công việc | `NgayBatDauCongViecDeXuat`, `NgayKetThucCVDeXuatDuKien` | `type="date"` | `type="datetime-local"`, value `yyyy-MM-ddTHH:mm` | Không | Không |
| `Views/DuAn/_Table.cshtml`, `Views/DuAn/Details.cshtml`, `Views/DuAn/ChiTiet.cshtml` | hiển thị dự án | deadline, hoàn thành, deadline công việc gần nhất, tham gia dự án | Một số nơi chỉ `dd/MM/yyyy` | Các mốc deadline/audit liên quan hiển thị `dd/MM/yyyy HH:mm` | Không | Không |
| `Views/CongViec/_Table.cshtml` | danh sách công việc | bắt đầu, hạn, hoàn tất | Chỉ `dd/MM/yyyy` | Hiển thị `dd/MM/yyyy HH:mm` | Không | Không |
| `Views/DeXuatCongViec/_Table.cshtml` | danh sách đề xuất | mốc bắt đầu/kết thúc đề xuất | Chỉ `dd/MM/yyyy` | Hiển thị `dd/MM/yyyy HH:mm` | Không | Không |
| `Views/DanhGiaDuAn/_ProjectStats.cshtml` | thống kê đánh giá dự án | mốc dự án và mốc hoàn thành tham chiếu | Chỉ `dd/MM/yyyy` | Hiển thị `dd/MM/yyyy HH:mm` | Không | Không |
| `Controllers/CongViecController.cs` | export công việc | cột bắt đầu, hạn, hoàn thành thực tế | `FormatDate` | `FormatDateTime` | Không | Không |
| `Controllers/DuAnController.cs` | export dự án | cột bắt đầu, kết thúc, hoàn thành thực tế | `FormatDate` | `FormatDateTime` | Không | Không |
| `Controllers/DanhGiaDuAnController.cs` | export đánh giá dự án | cột ngày đánh giá | `FormatDate` | `FormatDateTime` | Không | Không |
| `Controllers/NganSachController.cs` | export ngân sách | ngày duyệt, ngày cập nhật | `FormatDate` | `FormatDateTime` | Không | Không |

## Các logic .Date đã loại bỏ

- `CongViecService.GetPageAsync` bỏ `DateTime.Today` cho lọc tình trạng thời hạn, dùng `now`.
- `CongViecService.ApDungLocTinhTrangThoiHan` bỏ `.Value.Date` ở các filter quá hạn, hoàn thành trễ, hoàn thành đúng hạn, còn hạn.
- `CongViecService.GanThongTinThoiHan` bỏ `.Date` khi so sánh hạn/thực tế/hiện tại.
- `CongViecService.GanThongTinThoiHan` bỏ `(thucTe.Date - han.Date).Days`, thay bằng `TinhSoNgayTre(DateTime han, DateTime thucTe)`.
- `DanhGiaDuAnService.GetPageAsync` bỏ gọi `DateTime.Now` trực tiếp trong expression thống kê, dùng một biến `now` cho request.
- `DanhGiaDuAnService.LoadThongTinHoTroDanhGiaAsync` bỏ `.Date` khi tính công việc trễ/đúng hạn, báo cáo mới nhất và chậm cập nhật.
- `DanhGiaDuAnService.GanTrangThaiThoiHanDuAn` bỏ `.Date` khi xác định dự án hoàn thành trễ, đang quá hạn và số ngày trễ.
- `DeXuatCongViecService` bỏ `.Date` trong validation mốc đề xuất nằm trong khoảng dự án.

Tổng số nhóm logic `.Date`/`DateTime.Today` đã loại bỏ khỏi deadline nghiệp vụ: 8.

## Các vị trí vẫn giữ .Date có chủ đích

- `CongViecService.ChuanHoaKhoangNgay`: giữ `tuNgay?.Date`, `denNgay?.Date` cho filter ngày lịch `[từ ngày, đến ngày + 1)`.
- `DanhGiaDuAnService.ChuanHoaKhoangNgay`: giữ `.Date` cho filter ngày đánh giá.
- `DeXuatCongViecService.ChuanHoaKhoangNgay`: giữ `.Date` cho filter ngày đề xuất.
- `DuAnCreateUpdateViewModel.Validate`: giữ `NgayBatDauDuAn.Value.Date < DateTime.Today` cho ràng buộc tạo mới không chọn ngày bắt đầu trước hôm nay theo ngày lịch.
- Các filter ngày, ngày sinh, group/thống kê theo ngày/tháng/năm và ngày trong tên file export không đổi.

Tổng số vị trí `.Date`/`DateTime.Today` còn lại có chủ đích trong các file đã chạm: 7.

## Các form đã chuyển sang datetime-local

- `Views/DuAn/_Form.cshtml`: `NgayBatDauDuAn`, `NgayKetThucDuAn`.
- `Views/DeXuatCongViec/_Form.cshtml`: `NgayBatDauCongViecDeXuat`, `NgayKetThucCVDeXuatDuKien`.

Không chuyển form chi tiết công việc vì `NgayBatDauCTCV` đang là ngày bắt đầu lịch của chi tiết, không có deadline chi tiết riêng trong schema.

## Các định dạng hiển thị đã cập nhật

- Dự án: ngày bắt đầu, ngày kết thúc, ngày hoàn thành thực tế, deadline gần nhất.
- Công việc: ngày bắt đầu, hạn kết thúc, ngày hoàn thành thực tế.
- Đề xuất công việc: ngày bắt đầu/kết thúc dự kiến trong danh sách.
- Đánh giá dự án: ngày bắt đầu/kết thúc dự án, ngày hoàn thành tham chiếu, lần cập nhật gần nhất và thời gian AI tiếp tục hiển thị `dd/MM/yyyy HH:mm`.
- Phân công/audit liên quan trong chi tiết dự án: ngày tham gia dự án hiển thị thêm giờ để phục vụ kiểm tra thứ tự.

## Các export đã cập nhật

- `CongViecController.Export`: deadline công việc, ngày bắt đầu, ngày hoàn thành thực tế dùng `FormatDateTime`.
- `DuAnController.Export`: ngày bắt đầu, ngày kết thúc, ngày hoàn thành thực tế dùng `FormatDateTime`.
- `DanhGiaDuAnController.Export`: ngày đánh giá dùng `FormatDateTime`.
- `NganSachController.Export`: ngày duyệt và ngày cập nhật dùng `FormatDateTime`.
- Metadata filter `Từ ngày`/`Đến ngày` vẫn dùng `FormatDate` vì là filter ngày lịch.

## Kết quả kiểm tra tính nhất quán

- Dashboard không bị sửa; kiểm tra source cho thấy các công thức full `DateTime` hiện có không bị đưa về `.Date`.
- `CongViecService` và `DanhGiaDuAnService` dùng cùng quy tắc: thực tế `>` hạn là trễ, thực tế `<=` hạn là đúng hạn.
- Công việc chưa hoàn thành dùng `now > NgayKetThucCVDuKien`; trạng thái hoàn thành/chờ xác nhận được tách khỏi nhóm đang quá hạn.
- Số ngày trễ dùng `Math.Max(1, (int)Math.Ceiling((thucTe - han).TotalDays))` khi độ trễ dương.
- Báo cáo tiến độ mới nhất trong đánh giá dự án chọn theo `ThoiGianCapNhat DESC`, sau đó `MaTienDo DESC`.
- `AI_DATASET`, `AI_NGUYEN_NHAN`, `AI_KET_QUA`, model FastAPI và contract 22 feature không thay đổi.

## Kết quả build và test

- Đã chạy `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore`: thành công, 0 lỗi.
- Build còn 2 warning CS1998 sẵn có ở `FileTienDoCongViecService.cs`, không thuộc thay đổi này.
- Không tìm thấy test project trong solution (`rg --files -g "*.csproj" -g "*.sln" -g "*Tests*"` chỉ thấy solution và project MVC), nên không bổ sung kiến trúc test mới và không chạy test tự động.
- Đã kiểm tra UTF-8 các file đã chạm: không có ký tự replacement và không thấy các marker mojibake cấm theo yêu cầu.

## Các nội dung chưa chỉnh sửa và lý do

- Không sửa database, schema, migration, `DbContext` theo hướng đổi schema, dữ liệu SQL hoặc script dữ liệu.
- Không chạy migration, không chạy `database update`, không kết nối hoặc ghi dữ liệu SQL.
- Không sửa/tổng hợp/train AI vì Dashboard và AI hiện đã dùng full `DateTime`; thay đổi chỉ đồng bộ service đánh giá và hiển thị.
- Không đổi timezone ở `Views/Ai/Models.cshtml` vì chưa có bằng chứng chắc chắn contract nguồn là UTC hay local; tránh chuyển local hai lần.
- Không chuyển `ChiTietCongViec` sang deadline theo giờ vì schema hiện chỉ có ngày bắt đầu/ngày hoàn thành chi tiết, không có deadline chi tiết riêng; lần này chỉ tránh tính chi tiết trễ theo suy diễn deadline.
- Không đổi permission, scope Manager hoặc workflow duyệt/đổi quản lý; các query đổi quản lý đã có thứ tự thời gian DESC và khóa phụ DESC.
