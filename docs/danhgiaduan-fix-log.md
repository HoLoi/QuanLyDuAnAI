# Log sửa phần Dữ liệu hỗ trợ Đánh giá dự án

## Ngày sửa

10/06/2026

## File đã sửa

- `QuanLyDuAn/QuanLyDuAn/ViewModels/DanhGiaDuAn/DanhGiaDuAnThongKeViewModel.cs`
- `QuanLyDuAn/QuanLyDuAn/Services/Implementations/DanhGiaDuAnService.cs`
- `QuanLyDuAn/QuanLyDuAn/Views/DanhGiaDuAn/_ProjectStats.cshtml`
- `QuanLyDuAn/QuanLyDuAn/wwwroot/css/DanhGiaDuAn/index.css`
- `docs/danhgiaduan-fix-log.md`

Không sửa controller, route, permission, workflow, entity, migration, database, các action `Luu`, `GuiDuyet`, `Duyet`, `TuChoi`.

## Property ViewModel đã thêm

Trong `DanhGiaDuAnThongKeViewModel`:

- Nhóm công việc: `CongViecChuaHoanThanh`, `CongViecDangTreHan`, `CongViecHoanThanhTreHan`, `TyLeCongViecTreHan`.
- Nhóm chi tiết công việc: `ChiTietChuaHoanThanh`, `ChiTietBiCanTro`, `TyLeChiTietHoanThanh`.
- Nhóm báo cáo tiến độ: `SoBaoCaoTienDoChoDuyet`, `SoBaoCaoTienDoDaDuyet`, `SoBaoCaoTienDoBiTuChoi`, `SoBaoCaoTienDoYeuCauBoSung`, `TyLeBaoCaoTienDoBiTuChoi`, `LanCapNhatTienDoGanNhat`, `SoNgayChamCapNhatTienDo`.
- Nhóm ngân sách/chi phí: `NganSachConLai`, `SoTienVuotNganSach`, `TyLeVuotNganSach`, `TrangThaiNganSach`.
- Nhóm đề xuất: `SoDeXuatCongViecChoDuyet`, `SoDeXuatCongViecBiTuChoi`, `ThoiGianDuyetCongViecTrungBinh`, `SoDeXuatNganSachChoDuyet`, `SoDeXuatNganSachBiTuChoi`, `ThoiGianDuyetNganSachTrungBinh`.
- Nhóm nhân sự: `SoNhanVienThamGia`, `SoLanThayDoiNhanSu`, `SoLanThayDoiQuanLy`.

Giữ nguyên các property AI đã có như `TenNguyenNhanAiDuDoan`, `NguonNguyenNhanAi`, `TenModelTreHanAi`, `TenModelNguyenNhanAi`, `DoTinCayAi`, `MucPhuHopAi`, `ThoiGianDuDoanAi`, `TenNguyenNhanManagerXacNhan`, `KetQuaAiCoTheDaCu`, `CanhBaoDuLieuAi`.

## Hàm service đã sửa

- `LoadThongTinHoTroDanhGiaAsync(...)`: bổ sung tính dữ liệu hỗ trợ trực tiếp từ bảng nghiệp vụ hiện tại.
- Thêm helper `TinhTyLe(...)` để tính tỷ lệ phần trăm thống nhất.
- Thêm helper `LaHanhDongThayDoiNhanSu(...)`, bám theo logic nhận diện hành động thay đổi nhân sự đang dùng trong `AiDatasetService`.

## Chỉ số dữ liệu hỗ trợ đã bổ sung

- Công việc chưa hoàn thành.
- Công việc đang trễ hạn.
- Công việc hoàn thành trễ hạn.
- Tỷ lệ công việc trễ hạn.
- Chi tiết chưa hoàn thành.
- Chi tiết bị cản trở.
- Tỷ lệ chi tiết hoàn thành.
- Báo cáo tiến độ chờ duyệt/đã duyệt/bị từ chối/yêu cầu bổ sung.
- Tỷ lệ báo cáo tiến độ bị từ chối.
- Lần cập nhật tiến độ gần nhất.
- Số ngày chậm cập nhật tiến độ.
- Ngân sách còn lại.
- Số tiền vượt ngân sách.
- Tỷ lệ vượt ngân sách.
- Trạng thái ngân sách: `Trong ngân sách`, `Vượt ngân sách`, `Chưa có ngân sách`.
- Đề xuất công việc chờ duyệt/bị từ chối.
- Thời gian duyệt đề xuất công việc trung bình.
- Đề xuất ngân sách chờ duyệt/bị từ chối.
- Thời gian duyệt đề xuất ngân sách trung bình.
- Số nhân viên tham gia.
- Số lần thay đổi nhân sự.
- Số lần thay đổi quản lý.

## View/partial đã sửa

- `_ProjectStats.cshtml` được nhóm lại thành các cụm: Thời hạn dự án, Công việc, Chi tiết công việc, Báo cáo tiến độ, Ngân sách / chi phí, Đề xuất, Nhân sự, AI.
- Đổi nhãn rõ nghĩa hơn: `Công việc đang trễ hạn`, `Công việc hoàn thành trễ hạn`, `Tỷ lệ công việc hoàn thành`, `Ngày hoàn thành tham chiếu`, `Ngân sách còn lại`, `Số tiền vượt ngân sách`.
- Thêm cảnh báo nhẹ bằng class scoped trong `wwwroot/css/DanhGiaDuAn/index.css`, không sửa CSS toàn hệ thống.

## Những gì giữ nguyên

- Không đổi controller route.
- Không đổi permission.
- Không đổi workflow đánh giá dự án.
- Không đổi trạng thái `Nhap`, `ChoDuyet`, `DaDuyet`, `TuChoi`.
- Không đổi cách tính điểm đánh giá.
- Không lưu dữ liệu hỗ trợ vào `DANH_GIA_DU_AN`.
- Không thay đổi `AI_NGUYEN_NHAN`.
- Không tự động gọi AI khi mở form.
- Không dùng `AI_KET_QUA` làm ground truth cho dữ liệu hỗ trợ; chỉ hiển thị các thông tin AI đã có trong ViewModel.

## Kết quả build/test

- Đã chạy: `dotnet build QuanLyDuAn/QuanLyDuAn.sln --no-restore`.
- Kết quả sau lần kiểm tra cuối: build thành công, `0 Warning(s)`, `0 Error(s)`.

Chưa chạy kiểm thử runtime từng kịch bản dữ liệu trên trình duyệt vì cần phiên đăng nhập và dữ liệu DB cụ thể cho các trường hợp đúng hạn, trễ hạn, vượt ngân sách, báo cáo bị từ chối/yêu cầu bổ sung, chưa có ngân sách, không có công việc, form khóa ở `ChoDuyet`/`DaDuyet`.

## Lưu ý còn tồn tại

- `CongViecTreHan` cũ vẫn được giữ để tương thích phần code hiện có; dữ liệu hỗ trợ mới dùng nhãn rõ hơn là `CongViecDangTreHan` và tách thêm `CongViecHoanThanhTreHan`.
- `Ngày hoàn thành tham chiếu` vẫn dựa trên rule service hiện tại: ưu tiên ngày hoàn thành thực tế của dự án, nếu chưa có thì fallback từ ngày kết thúc thực tế công việc cuối cùng.
- Chưa sửa bất nhất độ dài `NhanXetTongDuAn` giữa ViewModel/service và mapping DbContext vì không thuộc phạm vi dữ liệu hỗ trợ.

## Ghi chú encoding/font tiếng Việt

- Các file đã sửa được ghi và đọc bằng UTF-8.
- Đã kiểm tra các mẫu mojibake phổ biến theo yêu cầu trên các file đã chạm; không phát hiện lỗi.
