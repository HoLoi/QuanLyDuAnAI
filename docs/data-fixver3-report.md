# BÁO CÁO HIỆU CHỈNH DỮ LIỆU FIXVER3
## 1. File đầu vào và file đầu ra
- Đầu vào: `520duanhoanthanhtrecodatasetvanguyennhan_fixver2.sql` và `docs/data.md`.
- Đầu ra: `520duanhoanthanhtrecodatasetvanguyennhan_fixver3.sql`.
- SHA-256 khối `AI_DATASET` trước/sau: `a49034ce0f0a6e7739720363290582c1e7a5883da7d1de2a58702c55a823cc95` (giống tuyệt đối).
## 2. Tổng số dự án đã xử lý

520 dự án; không thêm/xóa dòng INSERT, không đổi PK, `MaDuAn` hay cấu trúc script.
## 3. Phân bố nguyên nhân trước và sau

| Mã | Trước | Sau |
|---:|---:|---:|
| 1 | 172 | 58 |
| 2 | 310 | 58 |
| 3 | 1 | 58 |
| 4 | 0 | 57 |
| 5 | 0 | 58 |
| 6 | 37 | 58 |
| 7 | 0 | 58 |
| 8 | 0 | 58 |
| 9 | 0 | 57 |

## 4. Danh sách dự án theo từng nguyên nhân cuối cùng

- **1 – Thiếu nhân sự (58):** 38,42,46,52,55,56,60,62,63,90,91,126,130,131,136,138,142,143,146,147,148,153,155,156,157,160,161,162,163,167,168,170,171,173,176,177,178,182,183,188,190,191,192,195,196,197,198,205,206,212,216,230,233,240,241,246,251,261

- **2 – Thay đổi yêu cầu liên tục (58):** 22,31,43,67,68,69,73,74,76,77,78,79,81,83,87,88,89,92,96,97,99,101,103,104,108,109,112,113,116,117,118,119,123,124,127,128,272,273,276,281,285,296,297,300,309,317,321,336,345,353,357,360,365,381,393,401,429,465

- **3 – Quy trình xử lý chậm (58):** 23,33,47,71,82,95,98,106,111,115,122,133,213,227,248,275,279,282,283,288,290,293,302,303,308,311,318,323,324,325,327,330,332,333,335,338,339,342,348,351,363,366,369,372,378,380,384,387,395,402,408,414,416,423,437,444,507,528

- **4 – Vượt ngân sách (57):** 21,24,28,29,34,39,48,54,59,64,75,84,94,134,139,149,154,159,164,169,174,184,189,194,199,204,209,219,224,229,234,239,244,254,259,264,269,274,289,294,299,304,314,319,329,334,344,349,354,359,364,374,379,394,404,409,419

- **5 – Rủi ro kỹ thuật (58):** 1,2,3,4,5,6,7,8,9,10,25,32,35,36,40,132,135,140,434,446,451,458,463,470,472,474,476,478,479,482,484,486,488,490,491,493,494,496,498,499,500,503,505,506,510,511,512,514,515,517,518,520,522,523,524,526,527,530

- **6 – Phối hợp công việc chưa tốt (58):** 26,30,37,44,50,51,58,65,72,80,86,100,107,110,114,125,141,144,150,151,158,165,172,179,180,185,186,193,200,207,210,214,218,220,221,225,228,235,242,249,255,256,262,263,270,277,284,291,295,298,305,312,326,340,361,368,375,382

- **7 – Thông tin đầu vào chưa đầy đủ (58):** 27,137,315,341,377,389,392,396,399,405,407,411,413,417,420,421,425,426,428,431,432,435,438,440,441,443,447,449,450,452,453,455,456,457,459,461,462,464,467,468,471,473,477,480,483,485,489,492,495,497,501,504,509,513,516,519,521,525

- **8 – Ước lượng thời gian chưa chính xác (58):** 41,45,49,53,57,61,70,85,93,105,121,129,201,203,211,222,226,231,236,237,243,245,247,252,253,257,258,260,266,267,268,287,306,310,316,320,331,337,346,350,352,356,358,362,367,371,373,383,386,388,390,398,400,415,422,430,436,442

- **9 – Tiến độ cập nhật không đầy đủ (57):** 66,102,120,145,152,166,175,181,187,202,208,215,217,223,232,238,250,265,271,278,280,286,292,301,307,313,322,328,343,347,355,370,376,385,391,397,403,406,410,412,418,424,427,433,439,445,448,454,460,466,469,475,481,487,502,508,529

## 5. Tổng số dự án giữ nguyên nguyên nhân

150 dự án.

## 6. Tổng số dự án đổi nguyên nhân

370 dự án.

## 7. Bảng nghiệp vụ đã chỉnh

| Bảng | Số bản ghi chỉnh | Cột đã chỉnh | Lý do |
|---|---:|---|---|
| AI_NGUYEN_NHAN | 520 | GhiChuXacNhan, MaDMNguyenNhan | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |
| CHI_PHI | 409 | NoiDungChiPhi, SoTienDaChi | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |
| DE_XUAT_CONG_VIEC | 361 | MoTaCongViecDeXuat, NgayDuyetDeXuatCongViec | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |
| DE_XUAT_NGAN_SACH | 58 | NgayDuyet | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |
| DU_AN | 58 | NgayHoanThanhThucTeDuAn | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |
| NHAT_KY_CHI_PHI | 409 | HanhDongNKCP, NkSoTienDaChi | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |
| NHAT_KY_PHU_TRACH_DU_AN | 308 | NkHanhDongPTDA | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |
| TIEN_DO_CONG_VIEC | 1657 | GhiChuDuyet, GhiChuTienDo, ThoiGianCapNhat, ThoiGianDuyet, TrangThaiTienDo | Tạo/đồng bộ tín hiệu nghiệp vụ và nhãn mục tiêu |

## 8. Chi tiết theo từng dự án

| MaDuAn | Nguyên nhân cũ | Nguyên nhân mới | Bảng đã chỉnh | Tín hiệu nghiệp vụ chính | Thời gian | Workflow |
|---:|---|---|---|---|---|---|
| 1 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 2 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 3 | 6 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 4 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 5 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 6 | 6 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 7 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 8 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 9 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 10 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 21 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 22 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 23 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 24 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 25 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 26 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 27 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 28 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 29 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 30 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 31 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 32 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 33 | 3 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 34 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 35 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 36 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 37 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 38 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 39 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 40 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 41 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 42 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 43 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 44 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 45 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 46 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 47 | 1 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 48 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 49 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 50 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 51 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 52 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 53 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 54 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 55 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 56 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 57 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 58 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 59 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 60 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 61 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 62 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 63 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 64 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 65 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 66 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 67 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 68 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 69 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 70 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 71 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 72 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 73 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 74 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 75 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 76 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 77 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 78 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 79 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 80 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 81 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 82 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 83 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 84 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 85 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 86 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 87 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 88 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 89 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 90 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 91 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 92 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 93 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 94 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 95 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 96 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 97 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 98 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 99 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 100 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 101 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 102 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 103 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 104 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 105 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 106 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 107 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 108 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 109 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 110 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 111 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 112 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 113 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 114 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 115 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 116 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 117 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 118 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 119 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 120 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 121 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 122 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 123 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 124 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 125 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 126 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 127 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 128 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 129 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 130 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 131 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 132 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 133 | 1 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 134 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 135 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 136 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 137 | 6 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 138 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 139 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 140 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 141 | 1 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 142 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 143 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 144 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 145 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 146 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 147 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 148 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 149 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 150 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 151 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 152 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 153 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 154 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 155 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 156 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 157 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 158 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 159 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 160 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 161 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 162 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 163 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 164 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 165 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 166 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 167 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 168 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 169 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 170 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 171 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 172 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 173 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 174 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 175 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 176 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 177 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 178 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 179 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 180 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 181 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 182 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 183 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 184 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 185 | 1 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 186 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 187 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 188 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 189 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 190 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 191 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 192 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 193 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 194 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 195 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 196 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 197 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 198 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 199 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 200 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 201 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 202 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 203 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 204 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 205 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 206 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 207 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 208 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 209 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 210 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 211 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 212 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 213 | 1 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 214 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 215 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 216 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 217 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 218 | 1 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 219 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 220 | 1 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 221 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 222 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 223 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 224 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 225 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 226 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 227 | 1 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 228 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 229 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 230 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 231 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 232 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 233 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 234 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 235 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 236 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 237 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 238 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 239 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 240 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 241 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 242 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 243 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 244 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 245 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 246 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 247 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 248 | 1 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 249 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 250 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 251 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 252 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 253 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 254 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 255 | 1 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 256 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 257 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 258 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 259 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 260 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 261 | 1 | 1 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Tỷ lệ người/khối lượng và nhật ký điều chuyển | Đạt | Đạt |
| 262 | 1 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 263 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 264 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 265 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 266 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 267 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 268 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 269 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 270 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 271 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 272 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 273 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 274 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 275 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 276 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 277 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 278 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 279 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 280 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 281 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 282 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 283 | 1 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 284 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 285 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 286 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 287 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 288 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 289 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 290 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 291 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 292 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 293 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 294 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 295 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 296 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 297 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 298 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 299 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 300 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 301 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 302 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 303 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 304 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 305 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 306 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 307 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 308 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 309 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 310 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 311 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 312 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 313 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 314 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 315 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 316 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 317 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 318 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 319 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 320 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 321 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 322 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 323 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 324 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 325 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 326 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 327 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 328 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 329 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 330 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 331 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 332 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 333 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 334 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 335 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 336 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 337 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 338 | 1 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 339 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 340 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 341 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 342 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 343 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 344 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 345 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 346 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 347 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 348 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 349 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 350 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 351 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 352 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 353 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 354 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 355 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 356 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 357 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 358 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 359 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 360 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 361 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 362 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 363 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 364 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 365 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 366 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 367 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 368 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 369 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 370 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 371 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 372 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 373 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 374 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 375 | 2 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 376 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 377 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 378 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 379 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 380 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 381 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 382 | 6 | 6 | AI_NGUYEN_NHAN, NHAT_KY_PHU_TRACH_DU_AN, TIEN_DO_CONG_VIEC | Đổi phụ trách và chờ bàn giao | Đạt | Đạt |
| 383 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 384 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 385 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 386 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 387 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 388 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 389 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 390 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 391 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 392 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 393 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 394 | 1 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 395 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 396 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 397 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 398 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 399 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 400 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 401 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 402 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 403 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 404 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 405 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 406 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 407 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 408 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 409 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 410 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 411 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 412 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 413 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 414 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 415 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 416 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 417 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 418 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 419 | 2 | 4 | AI_NGUYEN_NHAN, CHI_PHI, NHAT_KY_CHI_PHI | Tổng chi thực tế vượt ngân sách active đã duyệt | Đạt | Đạt |
| 420 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 421 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 422 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 423 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 424 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 425 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 426 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 427 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 428 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 429 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 430 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 431 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 432 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 433 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 434 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 435 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 436 | 2 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 437 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 438 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 439 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 440 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 441 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 442 | 1 | 8 | AI_NGUYEN_NHAN, DU_AN, TIEN_DO_CONG_VIEC | Khối lượng vượt ước lượng, workflow sạch | Đạt | Đạt |
| 443 | 1 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 444 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 445 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 446 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 447 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 448 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 449 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 450 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 451 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 452 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 453 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 454 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 455 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 456 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 457 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 458 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 459 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 460 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 461 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 462 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 463 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 464 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 465 | 2 | 2 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, TIEN_DO_CONG_VIEC | Chuỗi đề xuất điều chỉnh phạm vi | Đạt | Đạt |
| 466 | 6 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 467 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 468 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 469 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 470 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 471 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 472 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 473 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 474 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 475 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 476 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 477 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 478 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 479 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 480 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 481 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 482 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 483 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 484 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 485 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 486 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 487 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 488 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 489 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 490 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 491 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 492 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 493 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 494 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 495 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 496 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 497 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 498 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 499 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 500 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 501 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 502 | 1 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 503 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 504 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 505 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 506 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 507 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 508 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 509 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 510 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 511 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 512 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 513 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 514 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 515 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 516 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 517 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 518 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 519 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 520 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 521 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 522 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 523 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 524 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 525 | 2 | 7 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Thiếu đầu vào, bổ sung rồi gửi lại | Đạt | Đạt |
| 526 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 527 | 1 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |
| 528 | 2 | 3 | AI_NGUYEN_NHAN, DE_XUAT_CONG_VIEC, DE_XUAT_NGAN_SACH, TIEN_DO_CONG_VIEC | Thời gian duyệt dài và vòng bổ sung | Đạt | Đạt |
| 529 | 2 | 9 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Khoảng cập nhật cuối cách xa hạn dự án | Đạt | Đạt |
| 530 | 2 | 5 | AI_NGUYEN_NHAN, TIEN_DO_CONG_VIEC | Lỗi kỹ thuật, xử lý lại rồi được duyệt | Đạt | Đạt |

## 9. Kiểm tra AI_NGUYEN_NHAN

- 520 dòng, 520 dự án distinct, mỗi dự án một dòng.
- Không mã 10; phân bố đúng 58/58/58/57/58/58/58/58/57.

## 10. Kiểm tra AI_DATASET

- Không literal nào bị chỉnh.
- SHA-256 nội dung toàn bộ các dòng INSERT trước và sau giống nhau: `a49034ce0f0a6e7739720363290582c1e7a5883da7d1de2a58702c55a823cc95`.

## 11. Kiểm tra khóa chính và khóa ngoại

- Số INSERT từng bảng giữ nguyên; không đổi PK/FK/`MaDuAn`.
- Chi phí chỉ được đổi số tiền/nội dung trên bản ghi hiện có; quan hệ ngân sách–chi phí–nhật ký giữ nguyên.

## 12. Kiểm tra chuỗi thời gian

- 520/520 đạt `Ngày bắt đầu < hạn < hoàn thành thực tế`.
- Không đề xuất hoặc báo cáo nào bị duyệt trước khi tạo/cập nhật; công việc kết thúc không sau dự án.

## 13. Kiểm tra workflow

- Báo cáo cuối mỗi chi tiết kết thúc `DaDuyet`; vòng từ chối/bổ sung có báo cáo sau.
- Dự án và công việc vẫn hoàn thành trễ; không thay đổi quan hệ đề xuất, nhân sự hay team.

## 14. Kiểm tra encoding

- UTF-8 không BOM, không U+FEFF, không marker mojibake đã cấm.
- Các chuỗi mới là literal Unicode `N'...'`; dấu nháy đơn được escape.

## 15. Các dự án có tín hiệu còn yếu

Không có dự án thiếu chuỗi bằng chứng tối thiểu theo kiểm tra tĩnh.

Đã tự tính lại đủ 22 feature cho 520 dự án theo `AiDatasetService`: mọi dự án đạt cổng tín hiệu tối thiểu của lớp mục tiêu; không có vector 22 feature trùng hoàn toàn nhưng mang nhãn khác nhau. Các feature được dùng làm kiểm tra trực tiếp gồm thay đổi nhân sự (mã 1), đề xuất công việc bị từ chối (mã 2), thời gian/vòng duyệt (mã 3), chênh lệch chi phí dương (mã 4), vòng xử lý lại (mã 5), đổi phụ trách/quản lý (mã 6), yêu cầu bổ sung (mã 7), số ngày trễ (mã 8), và số ngày chậm cập nhật (mã 9).

## 16. Kết luận

File fixver3 giữ nguyên cấu trúc export và toàn bộ `AI_DATASET`; nhãn đạt quota mục tiêu. Dữ liệu đã qua kiểm tra tĩnh về phân bố, quan hệ, thời gian, workflow và encoding; chưa chạy SQL Server theo phạm vi nhiệm vụ.
