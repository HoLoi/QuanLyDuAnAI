```plantuml
@startuml
title sd Dang nhap

actor ":NguoiDung" as NguoiDung
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NguoiDung -> GiaoDien:
Nhap thong tin dang nhap
(tenDangNhap, matKhau)

GiaoDien -> HeThong:
Gui yeu cau dang nhap (dangNhap)

activate HeThong
HeThong -> DB:
Kiem tra tai khoan theo ten dang nhap

activate DB
DB --> HeThong:
Thong tin tai khoan
deactivate DB

alt tai khoan khong ton tai hoac sai mat khau
    HeThong --> GiaoDien:
    Tra thong bao dang nhap that bai
    GiaoDien --> NguoiDung:
    Hien thi loi dang nhap
else tai khoan bi khoa
    HeThong --> GiaoDien:
    Tra thong bao tai khoan bi khoa
    GiaoDien --> NguoiDung:
    Hien thi thong bao tai khoan bi khoa
else hop le
    HeThong -> DB:
    Lay vai tro va danh sach quyen
    activate DB
    DB --> HeThong:
    Danh sach vai tro va quyen
    deactivate DB
    HeThong --> GiaoDien:
    Tra ket qua dang nhap thanh cong
    GiaoDien --> NguoiDung:
    Hien thi trang tong quan
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Quan ly du an

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Mo man hinh quan ly du an

GiaoDien -> HeThong:
Gui yeu cau tai danh sach du an (xemDanhSachDuAn)

activate HeThong
HeThong -> DB:
Lay danh sach du an theo quyen

activate DB
DB --> HeThong:
Danh sach du an
deactivate DB

loop voi moi du an
    HeThong -> DB:
    Tinh trang thai va thong ke du an
    DB --> HeThong:
    Trang thai va thong ke
end

HeThong --> GiaoDien:
Tra danh sach du an
GiaoDien --> QuanLyDuAn:
Hien thi danh sach du an

alt tao du an moi
    QuanLyDuAn -> GiaoDien:
    Nhap thong tin du an moi
    (tenDuAn, maLoaiDuAn, ngayBatDau, ngayKetThuc, moTa)
    GiaoDien -> HeThong:
    Gui yeu cau tao du an (taoDuAn)
    HeThong -> DB:
    Kiem tra loai du an va ngay bat dau
    DB --> HeThong:
    Ket qua kiem tra hop le
    HeThong -> DB:
    Luu du an moi
    HeThong -> DB:
    Tao phong chat du an
    HeThong -> DB:
    Ghi nhat ky tao du an
    DB --> HeThong:
    Ket qua luu du an
    HeThong --> GiaoDien:
    Tra ket qua tao du an thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao tao du an thanh cong
else cap nhat du an
    QuanLyDuAn -> GiaoDien:
    Nhap thong tin cap nhat du an
    (maDuAn, tenDuAn, trangThaiMoi, moTa, lyDo)
    GiaoDien -> HeThong:
    Gui yeu cau cap nhat du an (capNhatDuAn)
    HeThong -> DB:
    Kiem tra quan ly hien tai va trang thai du an
    DB --> HeThong:
    Ket qua cho phep cap nhat
    alt chuyen trang thai hop le
        HeThong -> DB:
        Cap nhat du an
        DB --> HeThong:
        Ket qua cap nhat
        HeThong --> GiaoDien:
        Tra ket qua cap nhat thanh cong
        GiaoDien --> QuanLyDuAn:
        Hien thi thong bao cap nhat thanh cong
    else chuyen trang thai khong hop le
        HeThong --> GiaoDien:
        Tra thong bao khong cho phep chuyen trang thai
        GiaoDien --> QuanLyDuAn:
        Hien thi loi chuyen trang thai
    end
else xoa du an
    QuanLyDuAn -> GiaoDien:
    Chon xoa du an
    (maDuAn)
    GiaoDien -> HeThong:
    Gui yeu cau xoa du an (xoaDuAn)
    HeThong -> DB:
    Kiem tra du an khoi tao va khong phat sinh du lieu
    DB --> HeThong:
    Ket qua cho phep xoa
    HeThong -> DB:
    Danh dau xoa du an
    DB --> HeThong:
    Ket qua xoa du an
    HeThong --> GiaoDien:
    Tra ket qua xoa du an
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao xoa du an thanh cong
else bat dau du an
    QuanLyDuAn -> GiaoDien:
    Chon bat dau du an
    (maDuAn)
    GiaoDien -> HeThong:
    Gui yeu cau bat dau du an (batDauDuAn)
    HeThong -> DB:
    Kiem tra thanh vien va danh muc cong viec
    DB --> HeThong:
    Ket qua du dieu kien bat dau
    HeThong -> DB:
    Cap nhat trang thai dang thuc hien
    DB --> HeThong:
    Ket qua cap nhat trang thai
    HeThong --> GiaoDien:
    Tra ket qua bat dau du an
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao bat dau du an thanh cong
else yeu cau hoan thanh
    QuanLyDuAn -> GiaoDien:
    Chon gui yeu cau hoan thanh
    (maDuAn)
    GiaoDien -> HeThong:
    Gui yeu cau hoan thanh du an (yeuCauHoanThanh)
    HeThong -> DB:
    Kiem tra dieu kien hoan thanh du an
    DB --> HeThong:
    Ket qua du dieu kien gui yeu cau
    HeThong -> DB:
    Cap nhat trang thai cho xac nhan hoan thanh
    DB --> HeThong:
    Ket qua cap nhat trang thai
    HeThong --> GiaoDien:
    Tra ket qua gui yeu cau hoan thanh
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao gui yeu cau thanh cong
else xac nhan hoan thanh
    QuanLyDuAn -> GiaoDien:
    Chon xac nhan hoan thanh du an
    (maDuAn)
    GiaoDien -> HeThong:
    Gui yeu cau xac nhan hoan thanh (xacNhanHoanThanh)
    HeThong -> DB:
    Kiem tra trang thai cho xac nhan va dieu kien hoan thanh
    DB --> HeThong:
    Ket qua du dieu kien xac nhan
    HeThong -> DB:
    Cap nhat trang thai hoan thanh
    DB --> HeThong:
    Ket qua cap nhat trang thai
    HeThong --> GiaoDien:
    Tra ket qua xac nhan hoan thanh
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao hoan thanh du an
else tam dung du an
    QuanLyDuAn -> GiaoDien:
    Nhap ly do tam dung
    (maDuAn, lyDoTamDung)
    GiaoDien -> HeThong:
    Gui yeu cau tam dung du an (tamDungDuAn)
    HeThong -> DB:
    Kiem tra ly do tam dung
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Cap nhat trang thai tam dung va ghi chu
    DB --> HeThong:
    Ket qua cap nhat trang thai
    HeThong --> GiaoDien:
    Tra ket qua tam dung du an
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao tam dung du an
else mo lai du an
    QuanLyDuAn -> GiaoDien:
    Nhap ly do mo lai
    (maDuAn, lyDoMoLai)
    GiaoDien -> HeThong:
    Gui yeu cau mo lai du an (moLaiDuAn)
    HeThong -> DB:
    Kiem tra du an dang hoan thanh va ly do mo lai
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Cap nhat trang thai dang thuc hien va ghi nhat ky
    DB --> HeThong:
    Ket qua cap nhat trang thai
    HeThong --> GiaoDien:
    Tra ket qua mo lai du an
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao mo lai du an
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Quan ly cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Mo man hinh quan ly cong viec

GiaoDien -> HeThong:
Gui yeu cau tai danh sach cong viec (xemDanhSachCongViec)

activate HeThong
HeThong -> DB:
Lay danh sach cong viec theo pham vi

activate DB
DB --> HeThong:
Danh sach cong viec
deactivate DB

alt xac nhan hoan thanh cong viec
    QuanLyDuAn -> GiaoDien:
    Chon xac nhan hoan thanh cong viec
    (maCongViec)
    GiaoDien -> HeThong:
    Gui yeu cau xac nhan hoan thanh (xacNhanHoanThanh)
    HeThong -> DB:
    Kiem tra quyen xu ly va trang thai cho xac nhan
    DB --> HeThong:
    Ket qua duoc xac nhan
    HeThong -> DB:
    Cap nhat trang thai cong viec hoan thanh
    DB --> HeThong:
    Ket qua cap nhat trang thai
    ref over HeThong
    Dong bo trang thai du an theo cong viec
    end ref
    HeThong --> GiaoDien:
    Tra ket qua xac nhan hoan thanh
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao xac nhan hoan thanh
else mo lai cong viec
    QuanLyDuAn -> GiaoDien:
    Chon mo lai cong viec
    (maCongViec, lyDoMoLai)
    GiaoDien -> HeThong:
    Gui yeu cau mo lai cong viec (moLaiCongViec)
    HeThong -> DB:
    Kiem tra cong viec da hoan thanh va du an chua dong
    DB --> HeThong:
    Ket qua duoc mo lai
    HeThong -> DB:
    Cap nhat trang thai dang thuc hien va ghi nhat ky
    DB --> HeThong:
    Ket qua cap nhat trang thai
    ref over HeThong
    Dong bo trang thai du an theo cong viec
    end ref
    HeThong --> GiaoDien:
    Tra ket qua mo lai cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao mo lai cong viec
else chi xem danh sach
    HeThong --> GiaoDien:
    Tra danh sach cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi danh sach cong viec
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Quan ly chi tiet cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Mo man hinh chi tiet cong viec
(maCongViec)

GiaoDien -> HeThong:
Gui yeu cau tai danh sach chi tiet cong viec (xemChiTietCongViec)

activate HeThong
HeThong -> DB:
Lay chi tiet cong viec theo cong viec

activate DB
DB --> HeThong:
Danh sach chi tiet cong viec
deactivate DB

alt them chi tiet cong viec
    QuanLyDuAn -> GiaoDien:
    Nhap thong tin chi tiet cong viec
    (maCongViec, tenChiTiet, ngayBatDau, ngayKetThuc, moTa)
    GiaoDien -> HeThong:
    Gui yeu cau them chi tiet cong viec (themChiTietCongViec)
    HeThong -> DB:
    Kiem tra quyen va trang thai cong viec cha
    DB --> HeThong:
    Ket qua duoc them
    HeThong -> DB:
    Kiem tra du lieu dau vao va moc ngay
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Luu chi tiet cong viec moi
    DB --> HeThong:
    Ket qua luu chi tiet cong viec
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> GiaoDien:
    Tra ket qua them chi tiet cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao them thanh cong
else cap nhat chi tiet cong viec
    QuanLyDuAn -> GiaoDien:
    Nhap thong tin cap nhat chi tiet cong viec
    (maChiTietCV, tenChiTiet, ngayBatDau, ngayKetThuc, moTa)
    GiaoDien -> HeThong:
    Gui yeu cau cap nhat chi tiet cong viec (capNhatChiTietCongViec)
    HeThong -> DB:
    Kiem tra quyen va trang thai cong viec cha
    DB --> HeThong:
    Ket qua duoc cap nhat
    HeThong -> DB:
    Cap nhat chi tiet cong viec
    DB --> HeThong:
    Ket qua cap nhat chi tiet cong viec
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> GiaoDien:
    Tra ket qua cap nhat chi tiet cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao cap nhat thanh cong
else xoa chi tiet cong viec
    QuanLyDuAn -> GiaoDien:
    Chon xoa chi tiet cong viec
    (maChiTietCV)
    GiaoDien -> HeThong:
    Gui yeu cau xoa chi tiet cong viec (xoaChiTietCongViec)
    HeThong -> DB:
    Kiem tra quyen va trang thai cong viec cha
    DB --> HeThong:
    Ket qua duoc xoa
    HeThong -> DB:
    Danh dau xoa chi tiet cong viec
    DB --> HeThong:
    Ket qua xoa chi tiet cong viec
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> GiaoDien:
    Tra ket qua xoa chi tiet cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao xoa thanh cong
else chi xem danh sach
    HeThong --> GiaoDien:
    Tra danh sach chi tiet cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi danh sach chi tiet cong viec
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Phan cong cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Mo man hinh phan cong cong viec
(maCongViec)

GiaoDien -> HeThong:
Gui yeu cau tai du lieu phan cong (xemPhanCongCongViec)

activate HeThong
HeThong -> DB:
Lay thong tin cong viec va thanh vien du an

activate DB
DB --> HeThong:
Du lieu phan cong cong viec
deactivate DB

alt them phan cong cong viec
    QuanLyDuAn -> GiaoDien:
    Chon nhan vien de phan cong
    (maCongViec, maNhanVien, vaiTroTrongCongViec)
    GiaoDien -> HeThong:
    Gui yeu cau them phan cong cong viec (themPhanCongCongViec)
    HeThong -> DB:
    Kiem tra quyen phan cong theo du an team
    DB --> HeThong:
    Ket qua co quyen
    HeThong -> DB:
    Kiem tra trang thai cong viec va nhan vien hop le
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Luu phan cong cong viec
    HeThong -> DB:
    Ghi nhat ky phan cong cong viec
    DB --> HeThong:
    Ket qua them phan cong
    HeThong --> GiaoDien:
    Tra ket qua them phan cong cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao them phan cong thanh cong
else xoa phan cong cong viec
    QuanLyDuAn -> GiaoDien:
    Chon xoa phan cong cong viec
    (maPhanCong)
    GiaoDien -> HeThong:
    Gui yeu cau xoa phan cong cong viec (xoaPhanCongCongViec)
    HeThong -> DB:
    Kiem tra quyen phan cong theo du an team
    DB --> HeThong:
    Ket qua co quyen
    HeThong -> DB:
    Xoa phan cong cong viec
    HeThong -> DB:
    Ghi nhat ky xoa phan cong cong viec
    DB --> HeThong:
    Ket qua xoa phan cong
    HeThong --> GiaoDien:
    Tra ket qua xoa phan cong cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao xoa phan cong thanh cong
else chi xem danh sach phan cong
    HeThong --> GiaoDien:
    Tra danh sach phan cong cong viec
    GiaoDien --> QuanLyDuAn:
    Hien thi danh sach phan cong
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Bao cao tien do cong viec

actor ":NhanVien" as NhanVien
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> GiaoDien:
Nhap bao cao tien do cong viec
(maChiTietCV, trangThaiDeXuat, phanTram, noiDungBaoCao, danhSachFile)

GiaoDien -> HeThong:
Gui yeu cau bao cao tien do (guiBaoCaoTienDo)

activate HeThong
HeThong -> DB:
Kiem tra nguoi duoc phan cong va trang thai hien tai

activate DB
DB --> HeThong:
Ket qua duoc gui bao cao
deactivate DB

HeThong -> DB:
Kiem tra khong co bao cao cho duyet truoc do
DB --> HeThong:
Ket qua hop le

HeThong -> DB:
Kiem tra khong lui trang thai va noi dung bao cao
DB --> HeThong:
Ket qua hop le

alt co file minh chung
    HeThong -> DB:
    Luu bao cao tien do cho duyet
    HeThong -> DB:
    Luu file minh chung tien do
    DB --> HeThong:
    Ket qua luu bao cao va file
    HeThong --> GiaoDien:
    Tra ket qua gui bao cao thanh cong
    GiaoDien --> NhanVien:
    Hien thi thong bao gui bao cao thanh cong
else khong co file minh chung
    alt trang thai can file bat buoc
        HeThong --> GiaoDien:
        Tra thong bao thieu file minh chung
        GiaoDien --> NhanVien:
        Hien thi loi thieu file minh chung
    else trang thai khong bat buoc file
        HeThong -> DB:
        Luu bao cao tien do cho duyet
        DB --> HeThong:
        Ket qua luu bao cao
        HeThong --> GiaoDien:
        Tra ket qua gui bao cao thanh cong
        GiaoDien --> NhanVien:
        Hien thi thong bao gui bao cao thanh cong
    end
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet bao cao tien do

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Chon xu ly bao cao tien do
(maTienDo, hanhDong, ghiChu)

GiaoDien -> HeThong:
Gui yeu cau xu ly bao cao tien do (duyetBaoCaoTienDo)

activate HeThong
HeThong -> DB:
Kiem tra quyen duyet theo scope du an

activate DB
DB --> HeThong:
Ket qua co quyen
deactivate DB

HeThong -> DB:
Kiem tra bao cao dang cho duyet va khong tu duyet
DB --> HeThong:
Ket qua hop le

alt duyet bao cao tien do
    HeThong -> DB:
    Cap nhat trang thai bao cao da duyet
    HeThong -> DB:
    Cap nhat trang thai chi tiet cong viec theo de xuat
    DB --> HeThong:
    Ket qua cap nhat bao cao va chi tiet cong viec
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> GiaoDien:
    Tra ket qua duyet bao cao thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao duyet thanh cong
else yeu cau bo sung bao cao tien do
    HeThong -> DB:
    Cap nhat trang thai bao cao yeu cau bo sung
    DB --> HeThong:
    Ket qua cap nhat yeu cau bo sung
    HeThong --> GiaoDien:
    Tra ket qua yeu cau bo sung
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao da gui yeu cau bo sung
else tu choi bao cao tien do
    HeThong -> DB:
    Cap nhat trang thai bao cao tu choi
    DB --> HeThong:
    Ket qua cap nhat tu choi
    HeThong --> GiaoDien:
    Tra ket qua tu choi bao cao
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao da tu choi bao cao
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd De xuat cong viec

actor ":NhanVien" as NhanVien
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> GiaoDien:
Mo man hinh de xuat cong viec
(maDuAn)

GiaoDien -> HeThong:
Gui yeu cau tai de xuat cong viec (xemDeXuatCongViec)

activate HeThong
HeThong -> DB:
Kiem tra quyen de xuat theo vai tro du an team

activate DB
DB --> HeThong:
Ket qua co quyen
deactivate DB

alt tao de xuat cong viec
    NhanVien -> GiaoDien:
    Nhap thong tin de xuat cong viec
    (maDuAn, tieuDeCongViec, moTa, ngayBatDau, ngayKetThuc, chiPhiDuKien)
    GiaoDien -> HeThong:
    Gui yeu cau tao de xuat cong viec (taoDeXuatCongViec)
    HeThong -> DB:
    Kiem tra trang thai du an va ngan sach hien hanh
    DB --> HeThong:
    Ket qua duoc de xuat
    HeThong -> DB:
    Kiem tra chi phi de xuat va trung lap cho duyet
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Luu de xuat cong viec trang thai cho duyet
    HeThong -> DB:
    Ghi nhat ky quan ly du an
    DB --> HeThong:
    Ket qua luu de xuat
    HeThong --> GiaoDien:
    Tra ket qua tao de xuat thanh cong
    GiaoDien --> NhanVien:
    Hien thi thong bao tao de xuat thanh cong
else huy de xuat cong viec
    NhanVien -> GiaoDien:
    Chon huy de xuat cong viec
    (maDeXuatCV, lyDoHuy)
    GiaoDien -> HeThong:
    Gui yeu cau huy de xuat cong viec (huyDeXuatCongViec)
    HeThong -> DB:
    Kiem tra nguoi tao va trang thai cho duyet
    DB --> HeThong:
    Ket qua duoc huy
    HeThong -> DB:
    Cap nhat trang thai de xuat da huy
    DB --> HeThong:
    Ket qua cap nhat huy
    HeThong --> GiaoDien:
    Tra ket qua huy de xuat thanh cong
    GiaoDien --> NhanVien:
    Hien thi thong bao huy de xuat thanh cong
else chi xem danh sach de xuat
    HeThong -> DB:
    Lay danh sach de xuat cong viec
    DB --> HeThong:
    Danh sach de xuat cong viec
    HeThong --> GiaoDien:
    Tra danh sach de xuat cong viec
    GiaoDien --> NhanVien:
    Hien thi danh sach de xuat cong viec
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet de xuat cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Chon xu ly de xuat cong viec
(maDeXuatCV, hanhDong, ghiChu)

GiaoDien -> HeThong:
Gui yeu cau duyet de xuat cong viec (duyetDeXuatCongViec)

activate HeThong
HeThong -> DB:
Kiem tra nguoi duyet la quan ly du an

activate DB
DB --> HeThong:
Ket qua co quyen
deactivate DB

HeThong -> DB:
Kiem tra de xuat dang cho duyet
DB --> HeThong:
Ket qua hop le

alt phe duyet de xuat
    HeThong -> DB:
    Kiem tra danh muc muc do ngan sach hien hanh
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Tao cong viec tu de xuat
    HeThong -> DB:
    Tao chi phi tu de xuat
    HeThong -> DB:
    Cap nhat trang thai de xuat da duyet
    alt du an dang cho xac nhan hoan thanh
        HeThong -> DB:
        Chuyen du an ve dang thuc hien
        DB --> HeThong:
        Ket qua chuyen trang thai du an
    end
    HeThong -> DB:
    Ghi nhat ky duyet de xuat
    DB --> HeThong:
    Ket qua phe duyet de xuat
    HeThong --> GiaoDien:
    Tra ket qua duyet de xuat thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao duyet de xuat thanh cong
else tu choi de xuat
    HeThong -> DB:
    Cap nhat trang thai de xuat tu choi
    HeThong -> DB:
    Ghi nhat ky tu choi de xuat
    DB --> HeThong:
    Ket qua tu choi de xuat
    HeThong --> GiaoDien:
    Tra ket qua tu choi de xuat
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao tu choi de xuat thanh cong
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd De xuat ngan sach

actor ":NhanVien" as NhanVien
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> GiaoDien:
Mo man hinh de xuat ngan sach
(maDuAn)

GiaoDien -> HeThong:
Gui yeu cau tai de xuat ngan sach (xemDeXuatNganSach)

activate HeThong
HeThong -> DB:
Kiem tra quyen de xuat theo vai tro du an team

activate DB
DB --> HeThong:
Ket qua co quyen
deactivate DB

alt tao de xuat ngan sach
    NhanVien -> GiaoDien:
    Nhap thong tin de xuat ngan sach
    (maDuAn, nganSachDeXuat, lyDoDeXuat, ghiChu)
    GiaoDien -> HeThong:
    Gui yeu cau tao de xuat ngan sach (taoDeXuatNganSach)
    HeThong -> DB:
    Kiem tra trang thai du an va khong co de xuat cho duyet
    DB --> HeThong:
    Ket qua duoc de xuat
    HeThong -> DB:
    Kiem tra ngan sach de xuat khong nho hon chi phi da dung
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Luu de xuat ngan sach trang thai cho duyet
    HeThong -> DB:
    Ghi nhat ky ngan sach va nhat ky du an
    DB --> HeThong:
    Ket qua luu de xuat ngan sach
    HeThong --> GiaoDien:
    Tra ket qua tao de xuat ngan sach thanh cong
    GiaoDien --> NhanVien:
    Hien thi thong bao tao de xuat ngan sach thanh cong
else huy de xuat ngan sach
    NhanVien -> GiaoDien:
    Chon huy de xuat ngan sach
    (maDeXuatNS, lyDoHuy)
    GiaoDien -> HeThong:
    Gui yeu cau huy de xuat ngan sach (huyDeXuatNganSach)
    HeThong -> DB:
    Kiem tra nguoi tao va trang thai cho duyet
    DB --> HeThong:
    Ket qua duoc huy
    HeThong -> DB:
    Cap nhat trang thai de xuat ngan sach da huy
    HeThong -> DB:
    Ghi nhat ky huy de xuat ngan sach
    DB --> HeThong:
    Ket qua huy de xuat ngan sach
    HeThong --> GiaoDien:
    Tra ket qua huy de xuat ngan sach thanh cong
    GiaoDien --> NhanVien:
    Hien thi thong bao huy de xuat ngan sach thanh cong
else chi xem danh sach de xuat ngan sach
    HeThong -> DB:
    Lay danh sach de xuat ngan sach
    DB --> HeThong:
    Danh sach de xuat ngan sach
    HeThong --> GiaoDien:
    Tra danh sach de xuat ngan sach
    GiaoDien --> NhanVien:
    Hien thi danh sach de xuat ngan sach
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet ngan sach

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Chon xu ly de xuat ngan sach
(maDeXuatNS, hanhDong, ghiChu)

GiaoDien -> HeThong:
Gui yeu cau duyet de xuat ngan sach (duyetDeXuatNganSach)

activate HeThong
HeThong -> DB:
Kiem tra nguoi duyet la quan ly du an

activate DB
DB --> HeThong:
Ket qua co quyen
deactivate DB

HeThong -> DB:
Kiem tra de xuat ngan sach dang cho duyet
DB --> HeThong:
Ket qua hop le

alt phe duyet de xuat ngan sach
    HeThong -> DB:
    Kiem tra ngan sach de xuat khong nho hon chi phi da dung
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Ngung kich hoat ngan sach cu
    HeThong -> DB:
    Tao ngan sach moi version tang dan
    HeThong -> DB:
    Cap nhat trang thai de xuat da duyet
    HeThong -> DB:
    Ghi nhat ky ngan sach va nhat ky du an
    DB --> HeThong:
    Ket qua duyet ngan sach
    HeThong --> GiaoDien:
    Tra ket qua duyet ngan sach thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao duyet ngan sach thanh cong
else tu choi de xuat ngan sach
    HeThong -> DB:
    Cap nhat trang thai de xuat tu choi
    HeThong -> DB:
    Ghi nhat ky tu choi ngan sach
    DB --> HeThong:
    Ket qua tu choi ngan sach
    HeThong --> GiaoDien:
    Tra ket qua tu choi ngan sach
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao tu choi ngan sach thanh cong
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Danh gia du an

actor ":QuanLyDuAn" as QuanLyDuAn
actor ":QuanTriVien" as QuanTriVien
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Mo man hinh danh gia du an
(maDuAn)

GiaoDien -> HeThong:
Gui yeu cau tai form danh gia du an (xemDanhGiaDuAn)

activate HeThong
HeThong -> DB:
Lay du an, tieu chi, thong ke va du lieu AI tham khao

activate DB
DB --> HeThong:
Du lieu form danh gia du an
deactivate DB

ref over HeThong
sd AI du doan va phan tich nguyen nhan tre
end ref

ref over HeThong
sd Xac nhan nguyen nhan tre
end ref

HeThong --> GiaoDien:
Tra du lieu form danh gia du an
GiaoDien --> QuanLyDuAn:
Hien thi man hinh danh gia du an

alt luu danh gia du an
    QuanLyDuAn -> GiaoDien:
    Nhap thong tin danh gia du an
    (maDuAn, danhSachTieuChi, nhanXetTongQuan)
    GiaoDien -> HeThong:
    Gui yeu cau luu danh gia du an (luuDanhGiaDuAn)
    HeThong -> DB:
    Kiem tra quan ly du an va trang thai ho so
    DB --> HeThong:
    Ket qua duoc luu
    HeThong -> DB:
    Luu danh gia du an va chi tiet tieu chi
    DB --> HeThong:
    Ket qua luu danh gia du an
    HeThong --> GiaoDien:
    Tra ket qua luu danh gia thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao luu danh gia thanh cong
else gui duyet danh gia du an
    QuanLyDuAn -> GiaoDien:
    Chon gui duyet danh gia du an
    (maDanhGiaDuAn)
    GiaoDien -> HeThong:
    Gui yeu cau gui duyet danh gia du an (guiDuyetDanhGiaDuAn)
    HeThong -> DB:
    Kiem tra trang thai du an va du lieu tieu chi
    DB --> HeThong:
    Ket qua duoc gui duyet
    HeThong -> DB:
    Cap nhat trang thai danh gia cho duyet
    DB --> HeThong:
    Ket qua cap nhat cho duyet
    HeThong --> GiaoDien:
    Tra ket qua gui duyet thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao gui duyet thanh cong
end

QuanTriVien -> GiaoDien:
Chon duyet hoac tu choi danh gia du an
(maDanhGiaDuAn, hanhDong, lyDo)

GiaoDien -> HeThong:
Gui yeu cau xu ly danh gia du an (duyetHoacTuChoiDanhGiaDuAn)

alt ho so dang cho duyet
    HeThong -> DB:
    Cap nhat trang thai da duyet hoac tu choi
    DB --> HeThong:
    Ket qua xu ly danh gia du an
    HeThong --> GiaoDien:
    Tra ket qua xu ly danh gia thanh cong
    GiaoDien --> QuanTriVien:
    Hien thi thong bao xu ly danh gia thanh cong
else ho so khong hop le
    HeThong --> GiaoDien:
    Tra thong bao khong the xu ly
    GiaoDien --> QuanTriVien:
    Hien thi thong bao khong the xu ly
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Danh gia nhan vien

actor ":NhanVien" as NhanVien
actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> GiaoDien:
Mo man hinh danh gia nhan vien
(maDuAn, maNhanVien)

GiaoDien -> HeThong:
Gui yeu cau tai form danh gia nhan vien (xemDanhGiaNhanVien)

activate HeThong
HeThong -> DB:
Lay tieu chi va thong ke nhan vien theo pham vi

activate DB
DB --> HeThong:
Du lieu form danh gia nhan vien
deactivate DB

alt luu danh gia nhan vien
    NhanVien -> GiaoDien:
    Nhap thong tin danh gia nhan vien
    (maDuAn, maNhanVien, danhSachTieuChi, nhanXetTongQuan)
    GiaoDien -> HeThong:
    Gui yeu cau luu danh gia nhan vien (luuDanhGiaNhanVien)
    HeThong -> DB:
    Kiem tra khong tu danh gia va quyen theo pham vi
    DB --> HeThong:
    Ket qua duoc luu
    HeThong -> DB:
    Luu danh gia nhan vien va chi tiet
    DB --> HeThong:
    Ket qua luu danh gia nhan vien
    HeThong --> GiaoDien:
    Tra ket qua luu danh gia thanh cong
    GiaoDien --> NhanVien:
    Hien thi thong bao luu danh gia thanh cong
else gui duyet danh gia nhan vien
    NhanVien -> GiaoDien:
    Chon gui duyet danh gia nhan vien
    (maDanhGiaNhanVien)
    GiaoDien -> HeThong:
    Gui yeu cau gui duyet danh gia nhan vien (guiDuyetDanhGiaNhanVien)
    HeThong -> DB:
    Kiem tra trang thai nhap hoac tu choi
    DB --> HeThong:
    Ket qua duoc gui duyet
    HeThong -> DB:
    Cap nhat trang thai danh gia cho duyet
    DB --> HeThong:
    Ket qua cap nhat cho duyet
    HeThong --> GiaoDien:
    Tra ket qua gui duyet thanh cong
    GiaoDien --> NhanVien:
    Hien thi thong bao gui duyet thanh cong
end

QuanLyDuAn -> GiaoDien:
Chon duyet hoac tu choi danh gia nhan vien
(maDanhGiaNhanVien, hanhDong, lyDo)

GiaoDien -> HeThong:
Gui yeu cau xu ly danh gia nhan vien (duyetHoacTuChoiDanhGiaNhanVien)

alt ho so dang cho duyet va dung pham vi du an
    HeThong -> DB:
    Cap nhat trang thai da duyet hoac tu choi
    DB --> HeThong:
    Ket qua xu ly danh gia nhan vien
    HeThong --> GiaoDien:
    Tra ket qua xu ly danh gia thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao xu ly danh gia thanh cong
else ho so khong hop le
    HeThong --> GiaoDien:
    Tra thong bao khong the xu ly
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao khong the xu ly
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd AI du doan va phan tich nguyen nhan tre

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Chon phan tich AI cho du an
(maDuAn)

GiaoDien -> HeThong:
Gui yeu cau phan tich AI du an (phanTichAiDuAn)

activate HeThong
HeThong -> DB:
Kiem tra scope du an va dieu kien phan tich AI

activate DB
DB --> HeThong:
Ket qua duoc phan tich
deactivate DB

alt du an hoan thanh dung han
    HeThong --> GiaoDien:
    Tra thong bao khong can phan tich nguyen nhan tre
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao khong can phan tich
else can phan tich
    HeThong -> DB:
    Lay AI_DATASET moi nhat theo du an
    DB --> HeThong:
    Du lieu dataset
    alt chua co dataset
        HeThong -> DB:
        Tong hop AI_DATASET cho du an
        HeThong -> DB:
        Nap lai du lieu dataset
        DB --> HeThong:
        Du lieu dataset sau tong hop
    end

    HeThong -> HeThong:
    Goi model du doan tre han va nguyen nhan
    alt du doan thanh cong
        HeThong -> DB:
        Map danh muc nguyen nhan va luu AI_KET_QUA
        alt map nguyen nhan khong khop
            HeThong -> DB:
            Dung danh muc mac dinh de luu ket qua
            DB --> HeThong:
            Ket qua luu theo danh muc mac dinh
        end
        DB --> HeThong:
        Ket qua luu du doan AI
        HeThong --> GiaoDien:
        Tra ket qua phan tich AI
        GiaoDien --> QuanLyDuAn:
        Hien thi ket qua du doan va nguyen nhan tre
    else du doan that bai
        HeThong --> GiaoDien:
        Tra thong bao khong ket noi duoc dich vu AI
        GiaoDien --> QuanLyDuAn:
        Hien thi thong bao loi AI
    end
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Xac nhan nguyen nhan tre

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Nhap thong tin xac nhan nguyen nhan tre
(maDuAn, maDmNguyenNhan, doTinCay)

GiaoDien -> HeThong:
Gui yeu cau xac nhan nguyen nhan tre (xacNhanNguyenNhan)

activate HeThong
HeThong -> DB:
Kiem tra quyen va scope du an

activate DB
DB --> HeThong:
Ket qua co quyen
deactivate DB

HeThong -> DB:
Kiem tra danh muc nguyen nhan ton tai
DB --> HeThong:
Ket qua hop le

HeThong -> DB:
Kiem tra du an dang tre theo AI
DB --> HeThong:
Ket qua can xac nhan

alt can xac nhan
    HeThong -> DB:
    Them hoac cap nhat nguyen nhan tre da xac nhan
    DB --> HeThong:
    Ket qua luu xac nhan nguyen nhan
    HeThong --> GiaoDien:
    Tra ket qua xac nhan thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao xac nhan thanh cong
else khong can xac nhan
    HeThong --> GiaoDien:
    Tra thong bao du an khong tre
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao khong can xac nhan
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Yeu cau doi quan ly du an

actor ":QuanLyDuAn" as QuanLyDuAn
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> GiaoDien:
Nhap thong tin yeu cau doi quan ly
(maDuAn, maQuanLyDeXuat, lyDo)

GiaoDien -> HeThong:
Gui yeu cau doi quan ly du an (taoYeuCauDoiQuanLy)

activate HeThong
HeThong -> DB:
Kiem tra nguoi tao la quan ly hien tai

activate DB
DB --> HeThong:
Ket qua hop le
deactivate DB

HeThong -> DB:
Kiem tra trang thai du an va yeu cau cho duyet ton tai
DB --> HeThong:
Ket qua duoc tao

HeThong -> DB:
Kiem tra ung vien quan ly co vai tro hoac chuc danh phu hop
DB --> HeThong:
Ket qua hop le

HeThong -> DB:
Luu yeu cau doi quan ly trang thai cho duyet
HeThong -> DB:
Ghi nhat ky tao yeu cau
DB --> HeThong:
Ket qua tao yeu cau doi quan ly

HeThong --> GiaoDien:
Tra ket qua tao yeu cau thanh cong
GiaoDien --> QuanLyDuAn:
Hien thi thong bao tao yeu cau thanh cong

alt huy yeu cau doi quan ly
    QuanLyDuAn -> GiaoDien:
    Chon huy yeu cau doi quan ly
    (maYeuCau, lyDoHuy)
    GiaoDien -> HeThong:
    Gui yeu cau huy doi quan ly (huyYeuCauDoiQuanLy)
    HeThong -> DB:
    Kiem tra nguoi tao va trang thai cho duyet
    DB --> HeThong:
    Ket qua duoc huy
    HeThong -> DB:
    Cap nhat trang thai yeu cau da huy
    HeThong -> DB:
    Ghi nhat ky huy yeu cau
    DB --> HeThong:
    Ket qua huy yeu cau
    HeThong --> GiaoDien:
    Tra ket qua huy yeu cau thanh cong
    GiaoDien --> QuanLyDuAn:
    Hien thi thong bao huy yeu cau thanh cong
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet yeu cau doi quan ly

actor ":QuanTriVien" as QuanTriVien
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanTriVien -> GiaoDien:
Chon duyet yeu cau doi quan ly
(maYeuCau, hanhDong, lyDo)

GiaoDien -> HeThong:
Gui yeu cau xu ly doi quan ly (duyetYeuCauDoiQuanLy)

activate HeThong
HeThong -> DB:
Kiem tra quyen duyet yeu cau

activate DB
DB --> HeThong:
Ket qua co quyen
deactivate DB

HeThong -> DB:
Lay yeu cau dang cho duyet
DB --> HeThong:
Thong tin yeu cau doi quan ly

alt phe duyet yeu cau
    HeThong -> DB:
    Kiem tra quan ly hien tai chua thay doi
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Kiem tra quan ly de xuat hop le
    DB --> HeThong:
    Ket qua hop le
    HeThong -> DB:
    Cap nhat yeu cau da duyet
    HeThong -> DB:
    Cap nhat quan ly moi cho du an
    HeThong -> DB:
    Dong nhat ky quan ly cu va mo nhat ky quan ly moi
    DB --> HeThong:
    Ket qua duyet yeu cau doi quan ly
    HeThong --> GiaoDien:
    Tra ket qua duyet yeu cau thanh cong
    GiaoDien --> QuanTriVien:
    Hien thi thong bao duyet yeu cau thanh cong
else tu choi yeu cau
    HeThong -> DB:
    Cap nhat yeu cau tu choi
    HeThong -> DB:
    Ghi nhat ky tu choi yeu cau
    DB --> HeThong:
    Ket qua tu choi yeu cau doi quan ly
    HeThong --> GiaoDien:
    Tra ket qua tu choi yeu cau
    GiaoDien --> QuanTriVien:
    Hien thi thong bao tu choi yeu cau thanh cong
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Chat du an

actor ":NhanVien" as NhanVien
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> GiaoDien:
Mo man hinh chat du an
(maDuAn)

GiaoDien -> HeThong:
Gui yeu cau tai phong chat du an (moChatDuAn)

activate HeThong
HeThong -> DB:
Kiem tra quyen chat va scope du an

activate DB
DB --> HeThong:
Ket qua duoc truy cap
deactivate DB

HeThong -> DB:
Dam bao phong chat du an
HeThong -> DB:
Dong bo thanh vien phong chat

loop voi moi phong chat trong scope
    HeThong -> DB:
    Lay thong tin phong chat va tin nhan moi nhat
    DB --> HeThong:
    Thong tin phong chat
end

HeThong --> GiaoDien:
Tra du lieu phong chat du an
GiaoDien --> NhanVien:
Hien thi man hinh chat du an

alt gui tin nhan
    NhanVien -> GiaoDien:
    Nhap noi dung tin nhan
    (maPhongChat, noiDung)
    GiaoDien -> HeThong:
    Gui yeu cau gui tin nhan (guiTinNhan)
    HeThong -> DB:
    Kiem tra thanh vien phong va trang thai du an
    DB --> HeThong:
    Ket qua duoc gui
    HeThong -> DB:
    Luu tin nhan
    DB --> HeThong:
    Ket qua luu tin nhan
    HeThong --> GiaoDien:
    Tra khung chat da cap nhat
    GiaoDien --> NhanVien:
    Hien thi tin nhan moi
else chi xem tin nhan
    GiaoDien -> HeThong:
    Gui yeu cau xem tin nhan (xemTinNhan)
    HeThong -> DB:
    Lay danh sach tin nhan theo phong
    DB --> HeThong:
    Danh sach tin nhan
    HeThong --> GiaoDien:
    Tra danh sach tin nhan
    GiaoDien --> NhanVien:
    Hien thi danh sach tin nhan
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Phan quyen

actor ":QuanTriVien" as QuanTriVien
boundary ":GiaoDien" as GiaoDien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanTriVien -> GiaoDien:
Mo man hinh phan quyen
(roleId)

GiaoDien -> HeThong:
Gui yeu cau tai du lieu phan quyen (xemPhanQuyen)

activate HeThong
HeThong -> DB:
Lay danh sach vai tro va danh muc quyen

activate DB
DB --> HeThong:
Du lieu phan quyen
deactivate DB

HeThong --> GiaoDien:
Tra ma tran phan quyen
GiaoDien --> QuanTriVien:
Hien thi ma tran phan quyen

QuanTriVien -> GiaoDien:
Chon danh sach quyen can luu
(roleId, selectedPermissions)

GiaoDien -> HeThong:
Gui yeu cau luu phan quyen (luuPhanQuyen)

HeThong -> DB:
Kiem tra vai tro ton tai va danh muc quyen hop le
DB --> HeThong:
Ket qua hop le

alt role admin thieu quyen bat buoc
    HeThong --> GiaoDien:
    Tra thong bao admin bat buoc co quyen phan quyen
    GiaoDien --> QuanTriVien:
    Hien thi thong bao role admin thieu quyen bat buoc
else hop le
    HeThong -> DB:
    Cap nhat role claims theo danh sach moi
    DB --> HeThong:
    Ket qua cap nhat phan quyen
    HeThong --> GiaoDien:
    Tra ket qua cap nhat phan quyen thanh cong
    GiaoDien --> QuanTriVien:
    Hien thi thong bao cap nhat phan quyen thanh cong
end

deactivate HeThong
@enduml
```
