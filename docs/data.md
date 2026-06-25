# Báo cáo hiệu chỉnh dữ liệu nghiệp vụ sau lần sửa tiếp theo

## 1. Trạng thái trước lần sửa tiếp theo

- Đã kiểm tra lại 19 dự án cũ: 83, 113, 233, 273, 25, 36, 66, 89, 119, 27, 95, 125, 135, 155, 77, 107, 177, 207, 237.
- File gốc không hậu tố `_fix` chỉ được đọc để đối chiếu, không sửa.
- Không chạy SQL Server, không nhập dữ liệu, không tổng hợp `AI_DATASET`, không train model.
- Các bảng AI được khóa không chỉnh sửa: `AI_DATASET`, `AI_NGUYEN_NHAN`, `AI_KET_QUA`, `AI_MODEL`, `DM_NGUYEN_NHAN`.

## 2. Kiểm tra lại 19 dự án đã sửa

| MaDuAn | Nhóm mục tiêu | Bảng đã sửa | Bằng chứng hiện tại | Đạt yêu cầu | Cần sửa thêm | Giữ hay thay |
| --- | --- | --- | --- | --- | --- | --- |
| 83 | Quy trình xử lý chậm | Không sửa trong lần này | đề xuất từ chối 7; CV trễ 12/12 | Đạt | Không | Giữ |
| 113 | Quy trình xử lý chậm | Không sửa trong lần này | đề xuất từ chối 9; CV trễ 6/6 | Đạt | Không | Giữ |
| 233 | Quy trình xử lý chậm | CONG_VIEC/TIEN_DO_CONG_VIEC | duyệt TB 0.33/3.25 ngày; yêu cầu bổ sung 1; CV trễ 9/9 | Đạt | Đã sửa bổ sung | Giữ |
| 273 | Quy trình xử lý chậm | Không sửa trong lần này | đề xuất từ chối 8; CV trễ 7/7 | Đạt | Không | Giữ |
| 25 | Rủi ro kỹ thuật | Không sửa trong lần này | đề xuất từ chối 2; báo cáo từ chối 24; yêu cầu bổ sung 8; CV trễ 6/6 | Đạt | Không | Giữ |
| 36 | Rủi ro kỹ thuật | Không sửa trong lần này | báo cáo từ chối 15; CV trễ 10/10 | Đạt | Không | Giữ |
| 66 | Rủi ro kỹ thuật | CONG_VIEC/TIEN_DO_CONG_VIEC | đề xuất từ chối 1; báo cáo từ chối 1; yêu cầu bổ sung 1; CV trễ 4/4 | Đạt | Đã sửa bổ sung | Giữ |
| 89 | Rủi ro kỹ thuật | Không sửa trong lần này | đề xuất từ chối 10; báo cáo từ chối 2; yêu cầu bổ sung 1; CV trễ 9/9 | Đạt | Không | Giữ |
| 119 | Rủi ro kỹ thuật | Không sửa trong lần này | đề xuất từ chối 8; báo cáo từ chối 2; yêu cầu bổ sung 1; CV trễ 12/12 | Đạt | Không | Giữ |
| 27 | Thông tin đầu vào chưa đầy đủ | Không sửa trong lần này | đề xuất từ chối 2; yêu cầu bổ sung 30; CV trễ 5/5 | Đạt | Không | Giữ |
| 95 | Thông tin đầu vào chưa đầy đủ | Không sửa trong lần này | đề xuất từ chối 4; yêu cầu bổ sung 3; CV trễ 6/6 | Đạt | Không | Giữ |
| 125 | Thông tin đầu vào chưa đầy đủ | Không sửa trong lần này | đề xuất từ chối 6; yêu cầu bổ sung 3; CV trễ 9/9 | Đạt | Không | Giữ |
| 135 | Thông tin đầu vào chưa đầy đủ | Không sửa trong lần này | báo cáo từ chối 18; yêu cầu bổ sung 6; CV trễ 9/9 | Không | Loại khỏi nhóm mục tiêu | Thay |
| 155 | Thông tin đầu vào chưa đầy đủ | CONG_VIEC/TIEN_DO_CONG_VIEC | yêu cầu bổ sung 3; CV trễ 8/8 | Đạt | Đã sửa bổ sung | Giữ |
| 77 | Ước lượng thời gian chưa chính xác | Không sửa trong lần này | đề xuất từ chối 8; CV trễ 6/6 | Không | Loại khỏi nhóm mục tiêu | Thay |
| 107 | Ước lượng thời gian chưa chính xác | Không sửa trong lần này | đề xuất từ chối 6; CV trễ 9/9 | Không | Loại khỏi nhóm mục tiêu | Thay |
| 177 | Ước lượng thời gian chưa chính xác | Không sửa trong lần này | CV trễ 9/9 | Đạt | Không | Giữ |
| 207 | Ước lượng thời gian chưa chính xác | Không sửa trong lần này | CV trễ 4/4 | Đạt | Không | Giữ |
| 237 | Ước lượng thời gian chưa chính xác | Không sửa trong lần này | CV trễ 6/6 | Đạt | Không | Giữ |

## 3. Dự án giữ nguyên sau kiểm tra

- Giữ trong nhóm mục tiêu sau kiểm tra: 83, 113, 233, 273, 25, 36, 66, 89, 119, 27, 95, 125, 155, 177, 207, 237.
- Các dự án không cần sửa thêm vẫn được giữ vì có ít nhất một bằng chứng trực tiếp và một bằng chứng hỗ trợ.

## 4. Dự án phải sửa bổ sung

- Sửa bổ sung: 233, 66, 155.
- `233` được bổ sung yêu cầu bổ sung hồ sơ nghiệm thu để có thêm tín hiệu quy trình.
- `66` được làm rõ nội dung lỗi kỹ thuật và kiểm thử lại.
- `155` được bổ sung chuỗi thiếu dữ liệu đầu vào và gửi lại hồ sơ.

## 5. Dự án bị loại khỏi nhóm nguyên nhân mục tiêu

- Loại khỏi nhóm mục tiêu mới: 135, 77, 107.
- `135` có tín hiệu từ chối kỹ thuật mạnh hơn thiếu đầu vào.
- `77` và `107` có nhiều đề xuất bị từ chối nên không còn sạch cho nhóm ước lượng.

## 6. Dự án thay thế

- Thay thế cho nhóm ước lượng: `42`, `48`, `54`, `60`, `90`.
- Thay thế/bổ sung cho nhóm thiếu đầu vào: `21`, `28`, `68`, `108`.
- Bổ sung cho nhóm rủi ro kỹ thuật: `69`, `109`, `129`.
- Bổ sung cho nhóm quy trình chậm: `22`, `23`, `30`, `513`.

## 7. Dự án bổ sung để tăng số mẫu

| MaDuAn | Nhóm mục tiêu | Bằng chứng trước sửa | Điều chỉnh thực hiện | Feature dự kiến | Mức an toàn |
| --- | --- | --- | --- | --- | --- |
| 23 | Quy trình xử lý chậm | đề xuất từ chối 5; duyệt TB 1.26/0.9 ngày; yêu cầu bổ sung 7; CV trễ 7/7 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 7/7; DXCV_TC 3; DXNS_TC 2; TD_TC 0; TD_BS 7; TB duyệt 1.26/0.9 ngày | Cao |
| 30 | Quy trình xử lý chậm | đề xuất từ chối 5; báo cáo từ chối 11; CV trễ 11/11 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 11/11; DXCV_TC 4; DXNS_TC 1; TD_TC 11; TD_BS 0; TB duyệt 0.35/0.25 ngày | Cao |
| 22 | Quy trình xử lý chậm | đề xuất từ chối 8; báo cáo từ chối 20; CV trễ 10/10 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 10/10; DXCV_TC 6; DXNS_TC 2; TD_TC 20; TD_BS 0; TB duyệt 0.34/0.25 ngày | Cao |
| 513 | Quy trình xử lý chậm | đề xuất từ chối 8; yêu cầu bổ sung 1; CV trễ 9/9 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 9/9; DXCV_TC 5; DXNS_TC 3; TD_TC 0; TD_BS 1; TB duyệt 0.33/0.3 ngày | Cao |
| 69 | Rủi ro kỹ thuật | đề xuất từ chối 8; báo cáo từ chối 2; yêu cầu bổ sung 1; CV trễ 7/7 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 7/7; DXCV_TC 7; DXNS_TC 1; TD_TC 2; TD_BS 1; TB duyệt 0.32/0.27 ngày | Cao |
| 109 | Rủi ro kỹ thuật | đề xuất từ chối 9; báo cáo từ chối 2; yêu cầu bổ sung 1; CV trễ 11/11 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 11/11; DXCV_TC 7; DXNS_TC 2; TD_TC 2; TD_BS 1; TB duyệt 0.36/0.29 ngày | Cao |
| 129 | Rủi ro kỹ thuật | đề xuất từ chối 8; báo cáo từ chối 2; yêu cầu bổ sung 1; CV trễ 4/4 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 4/4; DXCV_TC 7; DXNS_TC 1; TD_TC 2; TD_BS 1; TB duyệt 0.3/0.21 ngày | Cao |
| 21 | Thông tin đầu vào chưa đầy đủ | đề xuất từ chối 1; yêu cầu bổ sung 2; CV trễ 9/9 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 9/9; DXCV_TC 1; DXNS_TC 0; TD_TC 0; TD_BS 2; TB duyệt 0.33/0.08 ngày | Cao |
| 28 | Thông tin đầu vào chưa đầy đủ | đề xuất từ chối 1; yêu cầu bổ sung 2; CV trễ 7/7 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 7/7; DXCV_TC 1; DXNS_TC 0; TD_TC 0; TD_BS 2; TB duyệt 0.29/0.08 ngày | Cao |
| 68 | Thông tin đầu vào chưa đầy đủ | đề xuất từ chối 7; yêu cầu bổ sung 2; CV trễ 6/6 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 6/6; DXCV_TC 5; DXNS_TC 2; TD_TC 0; TD_BS 2; TB duyệt 0.3/0.31 ngày | Cao |
| 108 | Thông tin đầu vào chưa đầy đủ | đề xuất từ chối 5; yêu cầu bổ sung 2; CV trễ 10/10 | TIEN_DO_CONG_VIEC.GhiChuTienDo; TIEN_DO_CONG_VIEC.GhiChuDuyet | CV trễ 10/10; DXCV_TC 5; DXNS_TC 0; TD_TC 0; TD_BS 2; TB duyệt 0.34/0.08 ngày | Cao |
| 42 | Ước lượng thời gian chưa chính xác | CV trễ 7/7 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 7/7; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.29/0.08 ngày | Cao |
| 48 | Ước lượng thời gian chưa chính xác | CV trễ 4/4 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 4/4; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.23/0.08 ngày | Cao |
| 54 | Ước lượng thời gian chưa chính xác | CV trễ 10/10 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 10/10; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.35/0.08 ngày | Cao |
| 60 | Ước lượng thời gian chưa chính xác | CV trễ 7/7 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 7/7; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.29/0.08 ngày | Cao |
| 90 | Ước lượng thời gian chưa chính xác | CV trễ 10/10 | Không sửa SQL; dùng bằng chứng sẵn có | CV trễ 10/10; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.35/0.08 ngày | Cao |

## 8. Chi tiết thay đổi theo từng bảng

- Tổng số dự án sửa thêm trong lần này: 16.
- Danh sách dự án có literal bị sửa trong lần này: 21, 28, 42, 48, 54, 60, 66, 68, 69, 90, 108, 109, 129, 155, 233, 513.
- Bảng đã sửa: `CONG_VIEC`, `TIEN_DO_CONG_VIEC`.
- Bảng không sửa: `DE_XUAT_CONG_VIEC`, `DE_XUAT_NGAN_SACH`, `CT_CONG_VIEC`, `FILE_TIEN_DO_CONG_VIEC`, `NHAT_KY_DU_AN`, `NHAN_VIEN_DU_AN`, `YEU_CAU_DOI_QUAN_LY`, các bảng AI.
- Literal thời gian: 39.
- Literal trạng thái: 20.
- Literal nội dung: 48.
- Literal đề xuất và duyệt: 26.
- Literal công việc: 37.
- Literal chi tiết: 0.
- Literal báo cáo: 70.
- Literal file: 0.
- Literal nhật ký: 0.
- Literal nhân sự hoặc quản lý: 0.

## 9. Chi tiết literal trước và sau

| MaDuAn | Nhóm mục tiêu | Bảng | Khóa chính | Cột | Giá trị trước | Giá trị sau | Lý do |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 233 | Quy trình xử lý chậm | TIEN_DO_CONG_VIEC | 10623 | TrangThaiTienDo/ThoiGianDuyet/GhiChu* | DaDuyet, duyệt nhanh, ghi chú chung | YeuCauBoSung, duyệt sau 30 giờ, yêu cầu bổ sung hồ sơ nghiệm thu | Bổ sung tín hiệu quy trình chậm |
| 513 | Quy trình xử lý chậm | TIEN_DO_CONG_VIEC | 19685 | TrangThaiTienDo/ThoiGianDuyet/GhiChu* | DaDuyet, duyệt nhanh, ghi chú chung | YeuCauBoSung, duyệt sau 30 giờ, thiếu xác nhận biểu mẫu | Bổ sung tín hiệu quy trình chậm |
| 66 | Rủi ro kỹ thuật | TIEN_DO_CONG_VIEC | 3729, 3731 | GhiChuTienDo/GhiChuDuyet | Nội dung kỹ thuật còn mỏng | Lỗi tích hợp API, bổ sung nhật ký sửa lỗi và kiểm thử lại | Làm rõ bằng chứng kỹ thuật |
| 69 | Rủi ro kỹ thuật | TIEN_DO_CONG_VIEC | 3822, 3823, 3826 | TrangThaiTienDo/GhiChu* | DaDuyet, ghi chú chung | TuChoi/YeuCauBoSung do lỗi đồng bộ và timeout tích hợp | Bổ sung dự án kỹ thuật |
| 109 | Rủi ro kỹ thuật | TIEN_DO_CONG_VIEC | 6087, 6088, 6092 | TrangThaiTienDo/GhiChu* | DaDuyet, ghi chú chung | TuChoi/YeuCauBoSung do lỗi phân quyền và dữ liệu biên | Bổ sung dự án kỹ thuật |
| 129 | Rủi ro kỹ thuật | TIEN_DO_CONG_VIEC | 7288, 7289, 7292 | TrangThaiTienDo/GhiChu* | DaDuyet, ghi chú chung | TuChoi/YeuCauBoSung do thiết bị thử nghiệm và cấu hình môi trường | Bổ sung dự án kỹ thuật |
| 155 | Thông tin đầu vào chưa đầy đủ | TIEN_DO_CONG_VIEC | 8104, 8105, 8107 | TrangThaiTienDo/GhiChu* | Chuỗi bổ sung còn ngắn | YeuCauBoSung do thiếu dữ liệu, phụ lục nguồn và gửi lại hồ sơ | Củng cố thiếu đầu vào |
| 21 | Thông tin đầu vào chưa đầy đủ | TIEN_DO_CONG_VIEC | 1137, 1141 | TrangThaiTienDo/GhiChu* | DaDuyet, ghi chú chung | YeuCauBoSung do thiếu quy tắc nhập liệu và file đối chiếu | Bổ sung dự án thiếu đầu vào |
| 28 | Thông tin đầu vào chưa đầy đủ | TIEN_DO_CONG_VIEC | 1556, 1559 | TrangThaiTienDo/GhiChu* | DaDuyet, ghi chú chung | YeuCauBoSung do thiếu danh sách nguồn và biên bản xác nhận đầu vào | Bổ sung dự án thiếu đầu vào |
| 68 | Thông tin đầu vào chưa đầy đủ | TIEN_DO_CONG_VIEC | 3772, 3775 | TrangThaiTienDo/GhiChu* | DaDuyet, ghi chú chung | YeuCauBoSung do thiếu mẫu biểu và tài liệu đối chiếu | Bổ sung dự án thiếu đầu vào |
| 108 | Thông tin đầu vào chưa đầy đủ | TIEN_DO_CONG_VIEC | 6037, 6040 | TrangThaiTienDo/GhiChu* | DaDuyet, ghi chú chung | YeuCauBoSung do thiếu bảng ánh xạ và dữ liệu mẫu đủ trường | Bổ sung dự án thiếu đầu vào |
| 42,48,54,60,90 | Ước lượng thời gian chưa chính xác | CONG_VIEC | 37 công việc | NgayKetThucCVDuKien | Mốc kế hoạch quá sát thực tế ở một số việc | Mốc kế hoạch sớm hơn thực tế 30-72 giờ, đa dạng theo từng việc | Tăng tín hiệu ước lượng sai mà không tạo workflow cạnh tranh |

## 10. Kiểm tra workflow

| MaDuAn | MaCTCV | Báo cáo đầu | Báo cáo trung gian | Báo cáo cuối | Chuỗi trạng thái | Hợp lệ |
| --- | --- | --- | --- | --- | --- | --- |
| 233 | 3895 | 10622:DaDuyet | 10623:YeuCauBoSung | 10624:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 513 | 7814 | 19684:DaDuyet | 19685:YeuCauBoSung | 19686:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 66 | 1217 | 3729:TuChoi | 3729:TuChoi | 3730:DaDuyet/HoanThanh/100 | TuChoi -> DaDuyet | Có |
| 66 | 1218 | 3731:YeuCauBoSung | 3731:YeuCauBoSung | 3732:DaDuyet/HoanThanh/100 | YeuCauBoSung -> DaDuyet | Có |
| 69 | 1252 | 3821:DaDuyet | 3822:TuChoi | 3824:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> YeuCauBoSung -> DaDuyet | Có |
| 69 | 1252 | 3821:DaDuyet | 3823:YeuCauBoSung | 3824:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> YeuCauBoSung -> DaDuyet | Có |
| 69 | 1253 | 3825:DaDuyet | 3826:TuChoi | 3829:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> DaDuyet -> DaDuyet -> DaDuyet | Có |
| 109 | 2033 | 6086:DaDuyet | 6087:TuChoi | 6090:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> YeuCauBoSung -> DaDuyet -> DaDuyet | Có |
| 109 | 2033 | 6086:DaDuyet | 6088:YeuCauBoSung | 6090:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> YeuCauBoSung -> DaDuyet -> DaDuyet | Có |
| 109 | 2034 | 6091:DaDuyet | 6092:TuChoi | 6094:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> DaDuyet -> DaDuyet | Có |
| 129 | 2450 | 7287:DaDuyet | 7288:TuChoi | 7290:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> YeuCauBoSung -> DaDuyet | Có |
| 129 | 2450 | 7287:DaDuyet | 7289:YeuCauBoSung | 7290:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> YeuCauBoSung -> DaDuyet | Có |
| 129 | 2451 | 7291:DaDuyet | 7292:TuChoi | 7295:DaDuyet/HoanThanh/100 | DaDuyet -> TuChoi -> DaDuyet -> DaDuyet -> DaDuyet | Có |
| 155 | 2800 | 8104:YeuCauBoSung | 8104:YeuCauBoSung | 8106:DaDuyet/HoanThanh/100 | YeuCauBoSung -> YeuCauBoSung -> DaDuyet | Có |
| 155 | 2800 | 8104:YeuCauBoSung | 8105:YeuCauBoSung | 8106:DaDuyet/HoanThanh/100 | YeuCauBoSung -> YeuCauBoSung -> DaDuyet | Có |
| 155 | 2801 | 8107:YeuCauBoSung | 8107:YeuCauBoSung | 8108:DaDuyet/HoanThanh/100 | YeuCauBoSung -> DaDuyet | Có |
| 21 | 320 | 1136:DaDuyet | 1137:YeuCauBoSung | 1138:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 21 | 322 | 1141:YeuCauBoSung | 1141:YeuCauBoSung | 1142:DaDuyet/HoanThanh/100 | YeuCauBoSung -> DaDuyet | Có |
| 28 | 458 | 1555:DaDuyet | 1556:YeuCauBoSung | 1557:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 28 | 459 | 1558:DaDuyet | 1559:YeuCauBoSung | 1560:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 68 | 1237 | 3771:DaDuyet | 3772:YeuCauBoSung | 3773:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 68 | 1238 | 3774:DaDuyet | 3775:YeuCauBoSung | 3776:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 108 | 2018 | 6036:DaDuyet | 6037:YeuCauBoSung | 6038:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet | Có |
| 108 | 2019 | 6039:DaDuyet | 6040:YeuCauBoSung | 6042:DaDuyet/HoanThanh/100 | DaDuyet -> YeuCauBoSung -> DaDuyet -> DaDuyet | Có |

## 11. Kiểm tra chuỗi thời gian

- Mọi dự án vẫn thỏa `NgayBatDauDuAn < NgayKetThucDuAn < NgayHoanThanhThucTeDuAn`.
- Mỗi dự án còn ít nhất một công việc trễ và có công việc kết thúc sau hạn dự án.
- `MAX(NgayKetThucCVThucTe) <= NgayHoanThanhThucTeDuAn` cho toàn bộ 520 dự án.
- Báo cáo đã sửa giữ `ThoiGianCapNhat < ThoiGianDuyet <= NgayKetThucCTCV`.
- Công việc đã sửa trong nhóm ước lượng giữ `NgayBatDauCongViec < NgayKetThucCVDuKien < NgayKetThucCVThucTe`.

## 12. Feature dự kiến sau tổng hợp

| MaDuAn | Nhóm mục tiêu | Feature dự kiến | Tín hiệu cạnh tranh | Nguyên nhân nổi bật hơn |
| --- | --- | --- | --- | --- |
| 83 | Quy trình xử lý chậm | CV trễ 12/12; DXCV_TC 4; DXNS_TC 3; TD_TC 0; TD_BS 0; TB duyệt 0.38/0.99 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 113 | Quy trình xử lý chậm | CV trễ 6/6; DXCV_TC 6; DXNS_TC 3; TD_TC 0; TD_BS 0; TB duyệt 0.3/0.92 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 233 | Quy trình xử lý chậm | CV trễ 9/9; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 1; TB duyệt 0.33/3.25 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 273 | Quy trình xử lý chậm | CV trễ 7/7; DXCV_TC 5; DXNS_TC 3; TD_TC 0; TD_BS 0; TB duyệt 0.31/0.78 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 23 | Quy trình xử lý chậm | CV trễ 7/7; DXCV_TC 3; DXNS_TC 2; TD_TC 0; TD_BS 7; TB duyệt 1.26/0.9 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 30 | Quy trình xử lý chậm | CV trễ 11/11; DXCV_TC 4; DXNS_TC 1; TD_TC 11; TD_BS 0; TB duyệt 0.35/0.25 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 22 | Quy trình xử lý chậm | CV trễ 10/10; DXCV_TC 6; DXNS_TC 2; TD_TC 20; TD_BS 0; TB duyệt 0.34/0.25 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 513 | Quy trình xử lý chậm | CV trễ 9/9; DXCV_TC 5; DXNS_TC 3; TD_TC 0; TD_BS 1; TB duyệt 0.33/0.3 ngày | CV trễ là nền chung | Quy trình xử lý chậm |
| 25 | Rủi ro kỹ thuật | CV trễ 6/6; DXCV_TC 2; DXNS_TC 0; TD_TC 24; TD_BS 8; TB duyệt 0.27/0.08 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 36 | Rủi ro kỹ thuật | CV trễ 10/10; DXCV_TC 0; DXNS_TC 0; TD_TC 15; TD_BS 0; TB duyệt 0.35/0.08 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 66 | Rủi ro kỹ thuật | CV trễ 4/4; DXCV_TC 1; DXNS_TC 0; TD_TC 1; TD_BS 1; TB duyệt 0.24/0.08 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 89 | Rủi ro kỹ thuật | CV trễ 9/9; DXCV_TC 7; DXNS_TC 3; TD_TC 2; TD_BS 1; TB duyệt 0.34/0.66 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 119 | Rủi ro kỹ thuật | CV trễ 12/12; DXCV_TC 5; DXNS_TC 3; TD_TC 2; TD_BS 1; TB duyệt 0.37/0.56 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 69 | Rủi ro kỹ thuật | CV trễ 7/7; DXCV_TC 7; DXNS_TC 1; TD_TC 2; TD_BS 1; TB duyệt 0.32/0.27 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 109 | Rủi ro kỹ thuật | CV trễ 11/11; DXCV_TC 7; DXNS_TC 2; TD_TC 2; TD_BS 1; TB duyệt 0.36/0.29 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 129 | Rủi ro kỹ thuật | CV trễ 4/4; DXCV_TC 7; DXNS_TC 1; TD_TC 2; TD_BS 1; TB duyệt 0.3/0.21 ngày | CV trễ là nền chung | Rủi ro kỹ thuật |
| 27 | Thông tin đầu vào chưa đầy đủ | CV trễ 5/5; DXCV_TC 1; DXNS_TC 1; TD_TC 0; TD_BS 30; TB duyệt 0.26/0.56 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 95 | Thông tin đầu vào chưa đầy đủ | CV trễ 6/6; DXCV_TC 1; DXNS_TC 3; TD_TC 0; TD_BS 3; TB duyệt 0.27/0.54 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 125 | Thông tin đầu vào chưa đầy đủ | CV trễ 9/9; DXCV_TC 3; DXNS_TC 3; TD_TC 0; TD_BS 3; TB duyệt 0.31/0.79 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 155 | Thông tin đầu vào chưa đầy đủ | CV trễ 8/8; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 3; TB duyệt 0.31/0.08 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 21 | Thông tin đầu vào chưa đầy đủ | CV trễ 9/9; DXCV_TC 1; DXNS_TC 0; TD_TC 0; TD_BS 2; TB duyệt 0.33/0.08 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 28 | Thông tin đầu vào chưa đầy đủ | CV trễ 7/7; DXCV_TC 1; DXNS_TC 0; TD_TC 0; TD_BS 2; TB duyệt 0.29/0.08 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 68 | Thông tin đầu vào chưa đầy đủ | CV trễ 6/6; DXCV_TC 5; DXNS_TC 2; TD_TC 0; TD_BS 2; TB duyệt 0.3/0.31 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 108 | Thông tin đầu vào chưa đầy đủ | CV trễ 10/10; DXCV_TC 5; DXNS_TC 0; TD_TC 0; TD_BS 2; TB duyệt 0.34/0.08 ngày | CV trễ là nền chung | Thông tin đầu vào chưa đầy đủ |
| 177 | Ước lượng thời gian chưa chính xác | CV trễ 9/9; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.33/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |
| 207 | Ước lượng thời gian chưa chính xác | CV trễ 4/4; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.23/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |
| 237 | Ước lượng thời gian chưa chính xác | CV trễ 6/6; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.27/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |
| 42 | Ước lượng thời gian chưa chính xác | CV trễ 7/7; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.29/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |
| 48 | Ước lượng thời gian chưa chính xác | CV trễ 4/4; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.23/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |
| 54 | Ước lượng thời gian chưa chính xác | CV trễ 10/10; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.35/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |
| 60 | Ước lượng thời gian chưa chính xác | CV trễ 7/7; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.29/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |
| 90 | Ước lượng thời gian chưa chính xác | CV trễ 10/10; DXCV_TC 0; DXNS_TC 0; TD_TC 0; TD_BS 0; TB duyệt 0.35/0.08 ngày | Không nổi bật | Ước lượng thời gian chưa chính xác |

## 13. Số dự án dự kiến theo từng nguyên nhân

| Nguyên nhân | Số dự án có bằng chứng trước | Giữ | Loại | Bổ sung | Dự kiến sau | Đạt tối thiểu 5 |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| Quy trình xử lý chậm | 4 | 4 | 0 | 4 | 8 | Có |
| Rủi ro kỹ thuật | 5 | 5 | 0 | 3 | 8 | Có |
| Thông tin đầu vào chưa đầy đủ | 5 | 4 | 1 | 4 | 8 | Có |
| Ước lượng thời gian chưa chính xác | 5 | 3 | 2 | 5 | 8 | Có |

## 14. Kiểm tra lỗi font và encoding

- `520duanhoanthanhtrecodatasetvanguyennhan_fix.sql`: UTF-8 without BOM, không có `EF BB BF`, không có `U+FEFF`.
- `docs/data.md`: UTF-8, không có `U+FEFF`.
- Không phát hiện các marker lỗi font theo danh sách cấm của yêu cầu trong file SQL hoặc tài liệu.

## 15. Kết luận mức sẵn sàng tổng hợp AI_DATASET

- File SQL đã sẵn sàng để nhập lại và chạy chức năng tổng hợp `AI_DATASET` ở bước riêng.
- Chưa sửa `AI_DATASET`, `AI_NGUYEN_NHAN` hoặc bất kỳ bảng AI nào.
- Chưa thêm hoặc xóa bản ghi; không đổi ID; không đổi khóa ngoại; không đổi schema.
- Sau khi nhập lại, cần tự chạy tổng hợp `AI_DATASET`, kiểm tra 22 feature, đánh giá lại nhãn `AI_NGUYEN_NHAN`, rồi mới train model.
