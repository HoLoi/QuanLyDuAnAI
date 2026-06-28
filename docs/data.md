# PHÂN TÍCH TÁI CẤU TRÚC DỮ LIỆU AI CHO 520 DỰ ÁN

Ngày phân tích tĩnh: 2026-06-28. Báo cáo này không chạy SQL Server, không import dữ liệu, không train model và không sửa bất kỳ file nào ngoài tài liệu này.

## 1. Phạm vi và file đã đọc

- Đã đọc tuần tự toàn bộ `520duanhoanthanhtrecodatasetvanguyennhan_fixver2.sql`: 42.384.815 byte, 105.513 câu `INSERT`, UTF-8 không BOM. Bộ tách literal có theo dõi dấu nháy, `N'...'`, dấu phẩy trong chuỗi và `CAST(...)`; không tách bằng dấu phẩy đơn giản.
- Source of truth: `AiDatasetService.cs`, `AiDataset.cs`, `AiNguyenNhan.cs`, `QuanLyDuAnDbContext.cs`, `TrangThai.cs`.
- FastAPI: `constants.py`, `schemas.py`, `feature_builder.py`, `validation_service.py`.
- Tài liệu đối chiếu: `docs/ai-he-thong-phan-tich.md`, `docs/ai-train-dataset-review.md`, `docs/mvc-ai-integration-rules.md`.
- Khi tài liệu nói “số ngày” chung chung, báo cáo dùng source mới hơn: timestamp đầy đủ và `Math.Ceiling(TotalDays)`. `AI_KET_QUA` không được dùng làm nhãn.

## 2. Kết luận tổng quan

- File có đúng **520 dự án**, mã `1..10` và `21..530`; cả 520 đều `HoanThanh`, không xóa mềm, ngày hoàn thành thực tế sau hạn và đạt rule trễ hiện hành.
- Có 520 dòng `AI_DATASET` và 520 dòng `AI_NGUYEN_NHAN`; mỗi dự án đúng một dòng ở mỗi bảng, không có dòng nguyên nhân xóa mềm, không có nhãn `Khác`.
- Phân bố xác nhận hiện hành cực lệch: mã 1 = 172, mã 2 = 310, mã 3 = 1, mã 6 = 37; năm lớp 4, 5, 7, 8, 9 không có nhãn xác nhận.
- `AI_DATASET.MaDMNguyenNhan` lệch xác nhận tại dự án **24** (`AI_DATASET=4`, xác nhận hiện hành=2).
- Tính lại theo source phát hiện **993 ô feature sai trên cả 520 dự án**. Lỗi lớn nhất: `SoLanThayDoiNhanSu` 517 dự án và `SoNgayChamCapNhatTienDo` 434 dự án.
- Dataset hiện có ba feature hằng số: `TyLeCongViecTre=100`, `SoDeXuatCongViecChoDuyet=0`, `SoDeXuatNganSachChoDuyet=0`, `SoBaoCaoTienDoChoDuyet=0`, `SoNgayChamCapNhatTienDo=0`; thực tế feature cuối phải khác 0 ở 434 dự án.
- Phân bổ thiết kế đề xuất là 58 mẫu cho mã 1,2,3,5,6,7,8 và 57 mẫu cho mã 4,9. Hai lớp 57 là `Vượt ngân sách` và `Tiến độ cập nhật không đầy đủ` vì dữ liệu hiện tại có ít bằng chứng trực tiếp nhất.
- Đây **chưa phải căn cứ đủ an toàn để viết một prompt sửa SQL tự động hoàn toàn**: chỉ một dự án đang vượt ngân sách; nhiều dự án được xếp vào lớp 4,5,7,9 theo quota có tín hiệu yếu và cần thiết kế nghiệp vụ thủ công. Danh sách ở mục 13 là danh sách ứng viên có quota, không phải quyền đổi nhãn mù quáng.

## 3. Cấu trúc dữ liệu và quy tắc tổng hợp 22 feature

Quy ước chung: giá trị không có nguồn được đưa về 0; chỉ `DU_AN`, `DANH_MUC_CONG_VIEC`, `CONG_VIEC`, `CT_CONG_VIEC`, `NGAN_SACH`, `CHI_PHI`, `DE_XUAT_*`, `YEU_CAU_DOI_QUAN_LY` có lọc xóa mềm theo source. `NHAN_VIEN_DU_AN`, `NHAT_KY_PHU_TRACH_DU_AN`, `TIEN_DO_CONG_VIEC` không có điều kiện `IsDeleted` trong source. Trạng thái so qua các biến thể chuẩn của `TrangThai`.

| # | Feature | Nguồn/điều kiện/công thức | Đơn vị, null, làm tròn, mốc | Bảng phải chỉnh nếu đổi |
|---:|---|---|---|---|
| 1 | SoNhanVienDuAn | `NHAN_VIEN_DU_AN`, đếm distinct `MaNguoiDung` theo dự án | người; thiếu = 0; không làm tròn | `NHAN_VIEN_DU_AN` |
| 2 | TongSoCongViec | `CONG_VIEC` join `DANH_MUC_CONG_VIEC`; cả hai `IsDeleted!=1` | công việc; thiếu = 0 | hai bảng nguồn |
| 3 | SoCongViecTre | CV hoàn thành: thực tế > dự kiến; chưa hoàn thành: thời điểm tổng hợp > dự kiến | công việc; so timestamp đầy đủ | `CONG_VIEC` |
| 4 | TyLeCongViecTre | `SoCongViecTre*100/TongSoCongViec` | %, 0..100; tổng 0 => 0; round 2 | như #2,#3 |
| 5 | ChiPhiDuKien | tổng `NGAN_SACH.NganSach` active, đã duyệt, không xóa | tiền; null = 0; decimal | `NGAN_SACH` |
| 6 | ChiPhiThucTe | tổng `CHI_PHI.SoTienDaChi` không xóa, join ngân sách không xóa | tiền; null = 0 | `CHI_PHI`, `NGAN_SACH` |
| 7 | ChenhLechChiPhi | thực tế - dự kiến | tiền; có thể âm; không round thêm | như #5,#6 |
| 8 | SoLanThayDoiNhanSu | `NHAT_KY_PHU_TRACH_DU_AN`; hành động normalize khớp 14 cụm từ source | lần; thiếu = 0 | `NHAT_KY_PHU_TRACH_DU_AN` |
| 9 | SoLanThayDoiQuanLy | `YEU_CAU_DOI_QUAN_LY` không xóa, đã duyệt, có ngày duyệt, QL cũ != mới | lần | `YEU_CAU_DOI_QUAN_LY` |
| 10 | SoNgayTreTienDo | dự án hoàn thành/lưu trữ: `Ceiling(thực tế-hạn)` nếu dương | ngày; tối thiểu 1 khi dương | `DU_AN` |
| 11 | SoDeXuatCongViecChoDuyet | `DE_XUAT_CONG_VIEC` không xóa, trạng thái Chờ duyệt | đề xuất | `DE_XUAT_CONG_VIEC` |
| 12 | SoDeXuatCongViecBiTuChoi | như #11, trạng thái Từ chối | đề xuất | `DE_XUAT_CONG_VIEC` |
| 13 | ThoiGianDuyetCongViecTrungBinh | TB `max(0, NgayDuyet-NgayDeXuat)` của dòng đủ hai mốc | ngày thực; rỗng=0; round 2 | `DE_XUAT_CONG_VIEC` |
| 14 | SoDeXuatNganSachChoDuyet | `DE_XUAT_NGAN_SACH` không xóa, Chờ duyệt | đề xuất | `DE_XUAT_NGAN_SACH` |
| 15 | SoDeXuatNganSachBiTuChoi | như #14, Từ chối | đề xuất | `DE_XUAT_NGAN_SACH` |
| 16 | ThoiGianDuyetNganSachTrungBinh | TB `max(0, NgayDuyet-NgayDeXuat)` | ngày thực; rỗng=0; round 2 | `DE_XUAT_NGAN_SACH` |
| 17 | SoBaoCaoTienDoChoDuyet | `TIEN_DO_CONG_VIEC` join CT/CV/DMCV còn hiệu lực; Chờ duyệt | báo cáo | `TIEN_DO_CONG_VIEC` |
| 18 | SoBaoCaoTienDoBiTuChoi | như #17, Từ chối | báo cáo | `TIEN_DO_CONG_VIEC` |
| 19 | SoBaoCaoTienDoYeuCauBoSung | như #17, Yêu cầu bổ sung | báo cáo | `TIEN_DO_CONG_VIEC` |
| 20 | TyLeBaoCaoTienDoBiTuChoi | `#18/#21*100` | %, 0..100; #21=0 => 0; round 2 | như #17,#18 |
| 21 | SoLanCapNhatTienDo | tổng mọi báo cáo qua join còn hiệu lực | lần | `TIEN_DO_CONG_VIEC`, CT/CV/DMCV |
| 22 | SoNgayChamCapNhatTienDo | `Ceiling(hạn dự án - MAX(ThoiGianCapNhat))` nếu dương; không báo cáo dùng chính hạn | ngày; thiếu = 0 | `TIEN_DO_CONG_VIEC`, `DU_AN` |

`MaDMNguyenNhan` được lấy từ bản ghi `AI_NGUYEN_NHAN` không xóa mới nhất theo `NgayXacNhan`, rồi `MaAINguyenNhan`; chỉ gán khi `LaDuAnTre=true`.

## 4. Kiểm kê dữ liệu thực tế trong file SQL

| Kiểm tra | Kết quả |
|---|---:|
| DU_AN / hoàn thành trễ hợp lệ | 520 / 520 |
| AI_DATASET / dự án distinct | 520 / 520 |
| AI_NGUYEN_NHAN / hiện hành / dự án distinct | 520 / 520 / 520 |
| Thiếu nhãn / nhiều nhãn hiện hành / nhãn Khác | 0 / 0 / 0 |
| Nhãn nhưng thiếu dataset / dataset thiếu nhãn / dataset trùng dự án | 0 / 0 / 0 |
| Dataset lệch xác nhận hiện hành | 1: dự án 24 |
| Dataset ngoài HoanThanh hoặc Archived | 0 |
| PK trùng trong các bảng INSERT | 0 |
| FK mồ côi ở các quan hệ dùng để tính 22 feature | 0 |
| Null trong 22 feature / âm ở feature đếm,tỷ lệ,thời gian | 0 / 0 |
| Tỷ lệ ngoài 0..100 | 0 |
| Ngày dự án đảo / ngày duyệt đề xuất đảo / ngày duyệt báo cáo đảo | 0 / 0 / 0 |

Các lượng nghiệp vụ lớn: `CONG_VIEC=3.738`, `CT_CONG_VIEC=7.869`, `TIEN_DO_CONG_VIEC=19.723`, `DE_XUAT_CONG_VIEC=4.664`, `DE_XUAT_NGAN_SACH=1.053`, `CHI_PHI=3.738`, `NHAN_VIEN_DU_AN=3.442`.

## 5. Phân bố nguyên nhân hiện tại

| Mã | Tên | Tổng bản ghi | Dự án hiện hành | Tỷ lệ | Train | Ghi chú |
|---:|---|---:|---:|---:|---|---|
| 1 | Thiếu nhân sự | 172 | 172 | 33,08% | Có | quá nhiều; feature nhân sự đang sai 517 dự án |
| 2 | Thay đổi yêu cầu liên tục | 310 | 310 | 59,62% | Có | nhãn gom chung rõ rệt |
| 3 | Quy trình xử lý chậm | 1 | 1 | 0,19% | Chưa, dưới 5 | dự án 33 |
| 4 | Vượt ngân sách | 0 | 0 | 0% | Không | dataset dự án 24 có mã 4 nhưng xác nhận là 2 |
| 5 | Rủi ro kỹ thuật | 0 | 0 | 0% | Không | thiếu nhãn xác nhận |
| 6 | Phối hợp công việc chưa tốt | 37 | 37 | 7,12% | Có | đủ 30 |
| 7 | Thông tin đầu vào chưa đầy đủ | 0 | 0 | 0% | Không | thiếu nhãn xác nhận |
| 8 | Ước lượng thời gian chưa chính xác | 0 | 0 | 0% | Không | thiếu nhãn xác nhận |
| 9 | Tiến độ cập nhật không đầy đủ | 0 | 0 | 0% | Không | feature dataset đang sai hàng loạt |
| 10 | Khác | 0 | 0 | 0% | Không | giữ danh mục, không dùng trong 520 nhãn |

Lớp lớn nhất/nhỏ nhất có mẫu: 310/1; max/min=310; chênh 309. Có 3 lớp đạt 5 mẫu, 3 lớp đạt 30 mẫu. Không có lịch sử hay xóa mềm trong `AI_NGUYEN_NHAN`.

## 6. Các lỗi giữa AI_NGUYEN_NHAN và AI_DATASET

- Duy nhất dự án 24: dòng dataset mã 4, bản ghi xác nhận `MaAINguyenNhan=14` mã 2, hiện hành. Khi tổng hợp lại, source sẽ ghi mã 2.
- Không có nhãn `Khác`, nhãn mồ côi, nhiều xác nhận hiện hành hay dataset thiếu label.

## 7. Các lỗi giữa AI_DATASET và dữ liệu nghiệp vụ

| Feature | Số dự án sai | Phân loại |
|---|---:|---|
| SoLanThayDoiNhanSu | 517 | dataset sai; parser source chỉ nhận đúng cụm hành động |
| SoNgayChamCapNhatTienDo | 434 | dataset lưu 0; nghiệp vụ tính lại 1..4 ngày |
| SoLanThayDoiQuanLy | 4 | dự án 2,4,7,10: dataset 1, tính lại 0 |
| SoBaoCaoTienDoYeuCauBoSung | 15 | 21,28,66,68,69,89,95,108,109,119,125,129,155,233,513 |
| ThoiGianDuyetNganSachTrungBinh | 11 | 27,77,83,89,95,107,113,119,125,233,273 |
| SoBaoCaoTienDoBiTuChoi | 6 | 66,69,89,109,119,129 |
| TyLeBaoCaoTienDoBiTuChoi | 6 | 66,69,89,109,119,129 |

Tổng 993 sai khác, phủ đủ 520 dự án. 15 feature còn lại khớp phép tính source tại ngưỡng 0,01. Vì vậy không có dự án nào chỉ được đổi nhãn mà không tái tổng hợp dataset; **520 dự án cần chỉnh/tái tổng hợp `AI_DATASET`**. Dữ liệu nghiệp vụ không “sai” chỉ vì dataset cũ; riêng các lớp mục tiêu thiếu tín hiệu sẽ cần chỉnh nghiệp vụ có kiểm soát ở bước sau.

## 8. Thống kê chất lượng 22 feature

`N=520`, null=0 cho mọi feature. Ngoại lệ dùng IQR 1,5.

| Feature | Min | Q1 | Median | Mean | Q3 | Max | SD | Zero | Outlier | Giá trị phổ biến |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|
| SoNhanVienDuAn | 4 | 6 | 7 | 6,62 | 7 | 9 | 1,30 | 0 | 92 | 7(166),6(159),5(77) |
| TongSoCongViec | 4 | 5 | 7 | 7,19 | 9 | 12 | 2,16 | 0 | 0 | 6(79),5(71),8(70) |
| SoCongViecTre | 4 | 5 | 7 | 7,19 | 9 | 12 | 2,16 | 0 | 0 | giống tổng CV |
| TyLeCongViecTre | 100 | 100 | 100 | 100 | 100 | 100 | 0 | 0 | 0 | 100(520), hằng số |
| ChiPhiDuKien | 58m | 82,5m | 112,5m | 115,04m | 142,5m | 172,5m | 34,62m | 0 | 0 | 105m/75m/90m (33) |
| ChiPhiThucTe | 30m | 60,75m | 79,95m | 84,62m | 107,1m | 165,6m | 31,71m | 0 | 0 | 61,5m(14) |
| ChenhLechChiPhi | -79,35m | -44,85m | -27m | -30,42m | -13,5m | 27m | 21,18m | 0 | 0 | -21,6m/-31,05m (20) |
| SoLanThayDoiNhanSu | 4 | 6 | 7 | 7,59 | 9 | 13 | 1,98 | 0 | 0 | 7(103),6(94),8(85) |
| SoLanThayDoiQuanLy | 0 | 0 | 0 | 0,18 | 0 | 2 | 0,43 | 433 | 87 | 0(433) |
| SoNgayTreTienDo | 1 | 1 | 1 | 1,07 | 1 | 2 | 0,25 | 0 | 36 | 1(484) |
| DXCV chờ / DXNS chờ / báo cáo chờ | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 520 | 0 | hằng số |
| SoDeXuatCongViecBiTuChoi | 0 | 0 | 1 | 1,78 | 3 | 7 | 1,75 | 183 | 0 | 0(183),1(84),3(80) |
| TGDuyetCongViecTB | 0 | 0,27 | 0,30 | 0,30 | 0,33 | 1,67 | 0,11 | 9 | 13 | 0,31(63) |
| SoDeXuatNganSachBiTuChoi | 0 | 0 | 1 | 1,02 | 2 | 3 | 1,03 | 216 | 0 | 0(216) |
| TGDuyetNganSachTB | 0 | 0,08 | 0,24 | 0,20 | 0,29 | 2 | 0,15 | 10 | 3 | 0,08(208) |
| SoBaoCaoTienDoBiTuChoi | 0 | 0 | 0 | 0,43 | 0 | 24 | 2,43 | 499 | 21 | 0(499) |
| SoBaoCaoTienDoYeuCauBoSung | 0 | 0 | 0 | 0,45 | 0 | 66 | 3,70 | 503 | 17 | 0(503) |
| TyLeBaoCaoTienDoBiTuChoi | 0 | 0 | 0 | 0,74 | 0 | 30,3 | 4,07 | 499 | 21 | 0(499) |
| SoLanCapNhatTienDo | 8 | 23 | 33 | 37,93 | 49 | 156 | 22,63 | 0 | 16 | 23(62) |
| SoNgayChamCapNhatTienDo | 0 | 0 | 0 | 0 | 0 | 0 | 0 | 520 | 0 | hằng số sai |

Phụ thuộc toán học: tỷ lệ CV và chênh lệch chi phí khớp 520/520; tỷ lệ báo cáo từ chối cũ sai ở 6 dự án nêu trên. Chỉ một cặp vector 22 feature trùng hoàn toàn: dự án 142 và 478, cùng nhãn 1; không có vector trùng khác nhãn. Dữ liệu có mẫu lặp rất mạnh dù ít vector trùng tuyệt đối.

Theo nhãn hiện tại, trung bình nổi bật: mã 1 có 7,84 CV/6,26 người; mã 2 có 2,89 đề xuất CV bị từ chối; mã 3 (chỉ một mẫu) có duyệt 1,49/2 ngày và 34 yêu cầu bổ sung; mã 6 có 1,16 lần đổi quản lý. Các thống kê “theo lớp” của lớp một mẫu không có ý nghĩa suy rộng.

## 9. Hồ sơ tín hiệu của từng nguyên nhân

| Mã | Tín hiệu trực tiếp và hỗ trợ | Cạnh tranh/mâu thuẫn | Bảng nghiệp vụ |
|---:|---|---|---|
| 1 | ít người so khối lượng, phân công dồn, log giảm/điều chuyển nhân sự | nhiều đổi người nhưng nhân lực vẫn cao là phối hợp | NVDA, phân công, nhật ký phụ trách |
| 2 | đề xuất CV, thay nội dung/phạm vi, phát sinh CV; ghi chú yêu cầu đổi | từ chối do thiếu hồ sơ/kỹ thuật phải sang 5/7 | DE_XUAT_CONG_VIEC, CONG_VIEC |
| 3 | thời gian duyệt dài, chờ duyệt, nhiều vòng bổ sung | từ chối kỹ thuật không tự động là quy trình | hai bảng đề xuất, TIEN_DO |
| 4 | chi thực tế > ngân sách active đã duyệt; nhật ký/đề xuất ngân sách | chỉ có 1 dự án dương hiện tại; âm là mâu thuẫn mạnh | NGAN_SACH, CHI_PHI, nhật ký/đề xuất NS |
| 5 | ghi chú lỗi tích hợp, kiểm thử, môi trường; từ chối/bổ sung kỹ thuật | 22 feature không có cột kỹ thuật trực tiếp | TIEN_DO, FILE_TIEN_DO, CT/CV |
| 6 | đổi quản lý, bàn giao/phụ thuộc nhóm, thay đổi nhân sự có ngữ cảnh | số đổi nhân sự một mình cạnh tranh với thiếu người | YEU_CAU_DOI_QUAN_LY, TEAM_DU_AN, phân công, nhật ký |
| 7 | yêu cầu bổ sung vì thiếu hồ sơ/dữ liệu nguồn | feature #19 không phân biệt thiếu hồ sơ với kỹ thuật | TIEN_DO, DE_XUAT_*, file |
| 8 | trễ kế hoạch nhưng không có tín hiệu mạnh 1–7/9 | contract chỉ có ngày trễ và tỷ lệ trễ, rất yếu vì 100% CV trễ | DU_AN, CONG_VIEC, CT_CONG_VIEC |
| 9 | cập nhật cuối sớm hơn hạn, ít cập nhật so khối lượng, còn chờ | #22 hiện chỉ 1..4 ngày sau tính lại, tín hiệu hẹp | TIEN_DO_CONG_VIEC |

## 10. Dự án có nhãn phù hợp và có thể giữ nguyên

Theo phép xếp hạng tín hiệu có quota (không ngẫu nhiên, không dùng `MaDuAn % n`), **150 dự án** giữ nhãn hiện tại. Chúng là các dòng trong mục 13 có mã mục tiêu trùng mã hiện tại. “Giữ nhãn” không đồng nghĩa giữ dataset: cả 150 vẫn phải tái tổng hợp vì lỗi feature.

## 11. Dự án có nhãn không phù hợp

**370 dự án** đổi lớp trong phương án quota. Trong đó các dự án đưa vào mã 4,5,7,8,9 mà tín hiệu trực tiếp bằng 0 phải kiểm tra thủ công và chỉnh nghiệp vụ trước; không được chỉ sửa nhãn. Mức chắc chắn toàn cục thấp do 4 feature hằng số, 517 lỗi nhân sự và 434 lỗi cập nhật.

## 12. Phương án phân bổ cân bằng mục tiêu

| Mã | Nguyên nhân | Hiện tại | Mục tiêu | Lý do 57/58 |
|---:|---|---:|---:|---|
| 1 | Thiếu nhân sự | 172 | 58 | nhiều ứng viên tương đối |
| 2 | Thay đổi yêu cầu liên tục | 310 | 58 | tín hiệu đề xuất mạnh nhất |
| 3 | Quy trình xử lý chậm | 1 | 58 | có thời gian duyệt/vòng bổ sung |
| 4 | Vượt ngân sách | 0 | 57 | khan hiếm nhất: chỉ dự án 24 dương |
| 5 | Rủi ro kỹ thuật | 0 | 58 | cần nội dung nghiệp vụ, feature gián tiếp |
| 6 | Phối hợp công việc chưa tốt | 37 | 58 | có đổi quản lý/nhân sự |
| 7 | Thông tin đầu vào chưa đầy đủ | 0 | 58 | có bổ sung nhưng cần đọc ngữ nghĩa |
| 8 | Ước lượng thời gian chưa chính xác | 0 | 58 | lớp loại trừ, cần đa dạng độ trễ |
| 9 | Tiến độ cập nhật không đầy đủ | 0 | 57 | #22 hiện hẹp và dataset cũ sai |

Tổng 520, max-min=1, không có mã 10.

## 13. Danh sách phân bổ mục tiêu cho toàn bộ dự án

Mỗi mã dự án dưới đây xuất hiện đúng một lần. Đây là biểu diễn đầy đủ, gọn hơn 520 dòng; nhãn hiện tại tra theo mục 5. Tín hiệu/feature/bảng cần chỉnh tra theo hồ sơ mã ở mục 9 và ma trận mục 17.

| Mã mục tiêu | Danh sách MaDuAn |
|---:|---|
| 1 | 38,42,46,52,55,56,60,62,63,90,91,126,130,131,136,138,142,143,146,147,148,153,155,156,157,160,161,162,163,167,168,170,171,173,176,177,178,182,183,188,190,191,192,195,196,197,198,205,206,212,216,230,233,240,241,246,251,261 |
| 2 | 22,31,43,67,68,69,73,74,76,77,78,79,81,83,87,88,89,92,96,97,99,101,103,104,108,109,112,113,116,117,118,119,123,124,127,128,272,273,276,281,285,296,297,300,309,317,321,336,345,353,357,360,365,381,393,401,429,465 |
| 3 | 23,33,47,71,82,95,98,106,111,115,122,133,213,227,248,275,279,282,283,288,290,293,302,303,308,311,318,323,324,325,327,330,332,333,335,338,339,342,348,351,363,366,369,372,378,380,384,387,395,402,408,414,416,423,437,444,507,528 |
| 4 | 21,24,28,29,34,39,48,54,59,64,75,84,94,134,139,149,154,159,164,169,174,184,189,194,199,204,209,219,224,229,234,239,244,254,259,264,269,274,289,294,299,304,314,319,329,334,344,349,354,359,364,374,379,394,404,409,419 |
| 5 | 1,2,3,4,5,6,7,8,9,10,25,32,35,36,40,132,135,140,434,446,451,458,463,470,472,474,476,478,479,482,484,486,488,490,491,493,494,496,498,499,500,503,505,506,510,511,512,514,515,517,518,520,522,523,524,526,527,530 |
| 6 | 26,30,37,44,50,51,58,65,72,80,86,100,107,110,114,125,141,144,150,151,158,165,172,179,180,185,186,193,200,207,210,214,218,220,221,225,228,235,242,249,255,256,262,263,270,277,284,291,295,298,305,312,326,340,361,368,375,382 |
| 7 | 27,137,315,341,377,389,392,396,399,405,407,411,413,417,420,421,425,426,428,431,432,435,438,440,441,443,447,449,450,452,453,455,456,457,459,461,462,464,467,468,471,473,477,480,483,485,489,492,495,497,501,504,509,513,516,519,521,525 |
| 8 | 41,45,49,53,57,61,70,85,93,105,121,129,201,203,211,222,226,231,236,237,243,245,247,252,253,257,258,260,266,267,268,287,306,310,316,320,331,337,346,350,352,356,358,362,367,371,373,383,386,388,390,398,400,415,422,430,436,442 |
| 9 | 66,102,120,145,152,166,175,181,187,202,208,215,217,223,232,238,250,265,271,278,280,286,292,301,307,313,322,328,343,347,355,370,376,385,391,397,403,406,410,412,418,424,427,433,439,445,448,454,460,466,469,475,481,487,502,508,529 |

## 14. Kế hoạch thay đổi AI_NGUYEN_NHAN

- Với 150 dự án giữ nhãn: giữ bản ghi xác nhận, không tạo lịch sử giả.
- Với 370 dự án đổi: không sửa đè chứng cứ lịch sử. Xóa mềm bản ghi hiện hành cũ (`IsDeleted`, `DeletedAt`, `DeletedBy`) và thêm một xác nhận mới sau khi dữ liệu nghiệp vụ đã được duyệt; `NgayXacNhan` phải sau thay đổi, người xác nhận hợp lệ, ghi chú nêu bằng chứng.
- Mỗi dự án sau cùng đúng một bản ghi không xóa; bản mới nhất được chọn theo `NgayXacNhan`, rồi PK. Không xóa mã 10 khỏi danh mục.

## 15. Kế hoạch thay đổi AI_DATASET

- Không chỉnh tay feature độc lập với nghiệp vụ. Sau khi sửa nghiệp vụ, chạy lại đúng hàm tổng hợp để thay 22 feature, `MaDMNguyenNhan`, `NgayTongHop`.
- 520 dòng đều cần tái tổng hợp. Ưu tiên sửa lỗi source-data hiện hữu: #8, #22, #9, #19, #16, #18, #20.
- Mục tiêu không phải một ngưỡng cứng: mỗi lớp phải có khoảng dao động, tín hiệu hỗ trợ và một phần giao thoa; không nhân bản vector.

## 16. Kế hoạch thay đổi dữ liệu nghiệp vụ

| Lớp | Bảng chỉnh nhiều | Dạng chỉnh hợp lệ | Bảng không cần đụng nếu không liên quan |
|---:|---|---|---|
| 1 | NVDA, phân công, NK phụ trách | quan hệ người/khối lượng và sự kiện thật | ngân sách |
| 2 | DE_XUAT_CONG_VIEC, CONG_VIEC | chuỗi thay đổi phạm vi có ngày hợp lý | chi phí nếu không phát sinh |
| 3 | DE_XUAT_*, TIEN_DO | tăng thời gian/vòng xử lý, giữ tạo trước duyệt | NVDA |
| 4 | NGAN_SACH, CHI_PHI, nhật ký | chi hợp lệ làm chênh dương, không sửa số vô cớ | nhân sự |
| 5 | TIEN_DO, FILE_TIEN_DO, CT/CV | lỗi kỹ thuật có từ chối/bổ sung rồi khắc phục | ngân sách nếu không liên quan |
| 6 | TEAM_DU_AN, phân công, YC đổi QL, nhật ký | phụ thuộc/bàn giao/đổi phụ trách | chi phí |
| 7 | TIEN_DO, DE_XUAT, file | thiếu đầu vào, bổ sung rồi gửi lại | ngân sách |
| 8 | DU_AN, CONG_VIEC, CT | kế hoạch ước lượng lệch nhưng workflow sạch | AI tables trước nghiệp vụ |
| 9 | TIEN_DO | khoảng cập nhật, số lần và chờ duyệt hợp lý | chi phí, nhân sự |

## 17. Ma trận đối chiếu ba lớp dữ liệu

Áp dụng từng dự án theo mã mục tiêu ở mục 13:

| Mã mục tiêu | AI_NGUYEN_NHAN | AI_DATASET | Nghiệp vụ/feature tác động | Mức sửa và rủi ro |
|---:|---|---|---|---|
| 1 | giữ hoặc lịch sử+thêm mã 1 | tổng hợp lại | NVDA/NK/phân công; #1,#8 | ít-vừa; FK nhân sự |
| 2 | mã 2 | tổng hợp lại | DXCV/CV; #2,#11-13 | vừa; workflow đề xuất |
| 3 | mã 3 | tổng hợp lại | DXCV,DXNS,TIEN_DO; #11-20 | vừa; đảo mốc duyệt |
| 4 | mã 4 | tổng hợp lại | NGAN_SACH/CHI_PHI; #5-7,#14-16 | nhiều ở 56/57 ứng viên; tài chính |
| 5 | mã 5 | tổng hợp lại | TIEN_DO/CT/CV/file; #18-21 gián tiếp | nhiều; feature không trực tiếp |
| 6 | mã 6 | tổng hợp lại | đổi QL/NK/team/phân công; #8,#9 | vừa; quyền/phụ trách |
| 7 | mã 7 | tổng hợp lại | TIEN_DO/đề xuất/file; #19 gián tiếp | nhiều; cần phân biệt kỹ thuật |
| 8 | mã 8 | tổng hợp lại | DU_AN/CV/CT; #3,#4,#10 | vừa; không mất điều kiện trễ |
| 9 | mã 9 | tổng hợp lại | TIEN_DO; #17-22 | vừa; chuỗi báo cáo |

Không dự án nào thuộc mức “chỉ sửa AI_NGUYEN_NHAN” ở trạng thái file hiện tại vì cả 520 có ít nhất một feature sai. Dự án 24 là trường hợp duy nhất lỗi label giữa hai bảng AI, nhưng vẫn có lỗi feature nên không xếp vào mức “chỉ nhãn”.

## 18. Kiểm tra workflow và chuỗi thời gian

- 520/520 giữ `NgayBatDauDuAn < NgayKetThucDuAn < NgayHoanThanhThucTeDuAn`; không có đề xuất hay báo cáo duyệt trước lúc tạo/gửi.
- Khi chỉnh phải giữ: DMCV/CV/CT theo dự án; báo cáo gửi trước duyệt; Từ chối/Yêu cầu bổ sung có báo cáo sau; báo cáo cuối duyệt/100%; CV hoàn thành không vượt ngày hoàn thành dự án; chi phí thuộc ngân sách đúng dự án.
- Rủi ro cao nhất: `DU_AN.NgayKetThucDuAn/NgayHoanThanhThucTeDuAn`, ba mốc `CONG_VIEC`, ba mốc `CT_CONG_VIEC`, `TIEN_DO_CONG_VIEC.ThoiGianCapNhat/ThoiGianDuyet/TrangThai*`, ngày đề xuất/duyệt, `NGAN_SACH.IsActive/TrangThai`, FK chi phí.

## 19. Rủi ro dữ liệu quá nhân tạo, trùng lặp hoặc rò rỉ nhãn

- 100% dự án có 100% công việc trễ; ngày trễ chỉ 1 hoặc 2; bốn feature chờ duyệt/cập nhật cũ là hằng số; đây là dấu vết sinh mẫu.
- Chi phí và số công việc lặp theo tập giá trị nhỏ; nhiều cụm thời gian duyệt giống nhau. Vector 142=478 là bản sao tuyệt đối.
- Nhãn hiện tại gom theo vài pattern, đặc biệt mã 2. Nếu biến mã mục tiêu thành một ngưỡng tuyệt đối (ví dụ mọi mã 4 đều chênh >0 còn lớp khác luôn <0), cây sẽ học thuộc.
- Không thêm nhiễu ngẫu nhiên. Đa dạng phải đến từ số CV/CT, quy mô nhân sự, số vòng xử lý, loại lỗi/nội dung, thời lượng, giá trị chi phí và chuỗi sự kiện hợp lệ. Giữ tín hiệu chính nhưng cho phép hỗ trợ/cạnh tranh vừa phải.

## 20. Thứ tự chỉnh sửa đề xuất

1. Chốt thủ công 370 ứng viên đổi nhãn, đặc biệt toàn bộ lớp 4,5,7,9.
2. Sửa nghiệp vụ theo chuỗi giao dịch và kiểm tra FK/thời gian sau từng dự án.
3. Tính lại 22 feature từ nghiệp vụ; không sửa dataset trước.
4. Tạo lịch sử xác nhận mới; bảo đảm một bản ghi hiện hành.
5. Tổng hợp lại `AI_DATASET`, đối chiếu 22 feature và label.
6. Chạy kiểm tra trùng, phân bố, leakage; chỉ sau đó mới cân nhắc train.

## 21. Checklist kiểm tra sau khi chỉnh sửa

- [ ] 520 dự án hợp lệ; mỗi dự án đúng một dataset và một xác nhận hiện hành.
- [ ] Phân bố `58,58,58,57,58,58,58,58,57`; tổng 520; không mã 10.
- [ ] 22 feature tính lại khớp 100%; null/âm/tỷ lệ sai miền = 0.
- [ ] Không feature hằng số ngoài lý do nghiệp vụ; không vector trùng khác nhãn.
- [ ] `MaDMNguyenNhan` dataset khớp xác nhận mới nhất.
- [ ] Mọi FK tồn tại; mọi mốc tạo/gửi trước duyệt; workflow kết thúc hợp lệ.
- [ ] Vẫn đủ điều kiện dự án hoàn thành trễ theo source hiện hành.
- [ ] UTF-8 không BOM, không `U+FEFF`, không mojibake.

## 22. Kết luận đã đủ dữ liệu để viết prompt sửa SQL hay chưa

1. **Có**, đúng 520 dự án hợp lệ.
2. Dùng **9 nguyên nhân**, loại mã 10 khỏi train nhưng không xóa danh mục.
3. Hiện tại: mã 1=172, 2=310, 3=1, 4=0, 5=0, 6=37, 7=0, 8=0, 9=0.
4. Mục tiêu: 1=58, 2=58, 3=58, 4=57, 5=58, 6=58, 7=58, 8=58, 9=57.
5. Có 150 dự án giữ nhãn theo phương án quota.
6. Có 370 dự án đổi nhãn.
7. Chỉ đổi `AI_NGUYEN_NHAN`: **0**, vì mọi dự án đều có feature cần tái tổng hợp.
8. Cần chỉnh/tái tổng hợp `AI_DATASET`: **520**.
9. Chắc chắn cần chỉnh nghiệp vụ để tạo tín hiệu mục tiêu: tối thiểu 56/57 dự án lớp 4; các ứng viên tín hiệu yếu ở lớp 5,7,9 cũng cần duyệt thủ công. Chưa thể nêu một tổng “cần sửa nghiệp vụ” đáng tin hơn mà không biến quota thành dữ liệu bịa.
10. Bảng dự kiến chỉnh nhiều nhất: `TIEN_DO_CONG_VIEC`, `DE_XUAT_CONG_VIEC`, `CONG_VIEC`, `NGAN_SACH`, `CHI_PHI`, `NHAT_KY_PHU_TRACH_DU_AN`.
11. Feature trực tiếp chưa đủ tốt cho `Rủi ro kỹ thuật`, `Thông tin đầu vào chưa đầy đủ` và `Ước lượng thời gian chưa chính xác`; #18/#19 chỉ là proxy, không chứa ngữ nghĩa nguyên nhân.
12. Nguy cơ dữ liệu nhân tạo/Decision Tree học thuộc **cao**.
13. **Chưa đủ căn cứ để viết prompt sửa SQL tự động cuối cùng.**
14. Thiếu chính xác: quyết định thủ công cho các dự án quota có tín hiệu thấp; feature mục tiêu theo từng dự án lớp 4/5/7/9; nội dung sự kiện kỹ thuật/thiếu đầu vào; ngân sách/chi phí hợp lệ cho 56 ứng viên lớp 4; quy tắc nghiệp vụ chi tiết của lịch sử `NHAT_KY_PHU_TRACH_DU_AN` vì literal hiện tại không khớp bộ từ khóa source ở 517 dự án. Chỉ sau khi chốt các dữ liệu đó mới có thể viết prompt sửa SQL mà không bịa nghiệp vụ.
