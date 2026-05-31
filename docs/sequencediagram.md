```plantuml
@startuml
title sd Dang nhap

actor ":NguoiDung" as NguoiDung
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NguoiDung -> HeThong: dangNhap()
activate HeThong

HeThong -> DB: timTaiKhoanTheoTenDangNhap()
activate DB
DB --> HeThong: thongTinTaiKhoan
deactivate DB

alt tai khoan khong ton tai hoac sai mat khau
    HeThong --> NguoiDung: thongBaoDangNhapThatBai()
else tai khoan bi khoa
    HeThong --> NguoiDung: thongBaoTaiKhoanBiKhoa()
else hop le
    HeThong -> DB: layVaiTroVaQuyen()
    activate DB
    DB --> HeThong: danhSachVaiTroVaDanhSachQuyen
    deactivate DB
    HeThong --> NguoiDung: dangNhapThanhCongVaMoDashboard()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Quan ly du an

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: quanLyDuAn()
activate HeThong
HeThong -> DB: layDanhSachDuAnTheoQuyen()
activate DB
DB --> HeThong: danhSachDuAn
deactivate DB

loop voi moi du an
    HeThong -> DB: tinhTrangThaiVaThongKeDuAn()
    DB --> HeThong: trangThaiVaThongKe
end

alt taoDuAn()
    HeThong -> DB: kiemTraLoaiDuAnVaNgayBatDau()
    DB --> HeThong: hopLe
    HeThong -> DB: luuDuAnMoi()
    HeThong -> DB: taoPhongChatDuAn()
    HeThong -> DB: ghiNhatKyTaoDuAn()
    HeThong --> QuanLyDuAn: thongBaoTaoDuAnThanhCong()
else capNhatDuAn()
    HeThong -> DB: kiemTraQuanLyHienTaiVaTrangThaiDuAn()
    DB --> HeThong: duocCapNhat
    alt chuyenTrangThaiHopLe
        HeThong -> DB: capNhatDuAn()
        HeThong --> QuanLyDuAn: thongBaoCapNhatThanhCong()
    else chuyenTrangThaiKhongHopLe
        HeThong --> QuanLyDuAn: thongBaoKhongChoPhepChuyenTrangThai()
    end
else xoaDuAn()
    HeThong -> DB: kiemTraDuAnKhoiTaoVaKhongPhatSinhDuLieu()
    DB --> HeThong: duocXoa
    HeThong -> DB: danhDauXoaDuAn()
    HeThong --> QuanLyDuAn: thongBaoXoaDuAnThanhCong()
else batDauDuAn()
    HeThong -> DB: kiemTraThanhVienDanhMucCongViec()
    DB --> HeThong: duDieuKienBatDau
    HeThong -> DB: capNhatTrangThaiDangThucHien()
    HeThong --> QuanLyDuAn: thongBaoBatDauDuAnThanhCong()
else yeuCauHoanThanh()
    HeThong -> DB: kiemTraDuDieuKienHoanThanhDuAn()
    DB --> HeThong: duDieuKienGuiYeuCau
    HeThong -> DB: capNhatTrangThaiChoXacNhanHoanThanh()
    HeThong --> QuanLyDuAn: thongBaoGuiYeuCauThanhCong()
else xacNhanHoanThanh()
    HeThong -> DB: kiemTraTrangThaiChoXacNhanVaDieuKienHoanThanh()
    DB --> HeThong: duDieuKienXacNhan
    HeThong -> DB: capNhatTrangThaiHoanThanh()
    HeThong --> QuanLyDuAn: thongBaoHoanThanhDuAn()
else tamDungDuAn()
    HeThong -> DB: kiemTraLyDoTamDung()
    DB --> HeThong: hopLe
    HeThong -> DB: capNhatTrangThaiTamDungVaGhiChu()
    HeThong --> QuanLyDuAn: thongBaoTamDungDuAn()
else moLaiDuAn()
    HeThong -> DB: kiemTraDuAnDangHoanThanhVaLyDoMoLai()
    DB --> HeThong: hopLe
    HeThong -> DB: capNhatTrangThaiDangThucHienVaGhiNhatKy()
    HeThong --> QuanLyDuAn: thongBaoMoLaiDuAn()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Quan ly cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: quanLyCongViec()
activate HeThong
HeThong -> DB: layDanhSachCongViecTheoScope()
activate DB
DB --> HeThong: danhSachCongViec
deactivate DB

alt xacNhanHoanThanh()
    HeThong -> DB: kiemTraQuyenXuLyVaTrangThaiChoXacNhan()
    DB --> HeThong: duocXacNhan
    HeThong -> DB: capNhatTrangThaiCongViecHoanThanh()
    ref over HeThong
    Dong bo trang thai du an theo cong viec
    end ref
    HeThong --> QuanLyDuAn: thongBaoXacNhanHoanThanh()
else moLaiCongViec()
    HeThong -> DB: kiemTraCongViecDaHoanThanhVaDuAnChuaDong()
    DB --> HeThong: duocMoLai
    HeThong -> DB: capNhatTrangThaiDangThucHienVaGhiNhatKy()
    ref over HeThong
    Dong bo trang thai du an theo cong viec
    end ref
    HeThong --> QuanLyDuAn: thongBaoMoLaiCongViec()
else chiXemDanhSach
    HeThong --> QuanLyDuAn: hienThiDanhSachCongViec()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Quan ly chi tiet cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: quanLyChiTietCongViec(maCongViec)
activate HeThong
HeThong -> DB: layChiTietCongViecTheoCongViec()
activate DB
DB --> HeThong: danhSachChiTiet
deactivate DB

alt themChiTietCongViec()
    HeThong -> DB: kiemTraQuyenVaTrangThaiCongViecCha()
    DB --> HeThong: duocThem
    HeThong -> DB: kiemTraDuLieuDauVaoVaMocNgay()
    DB --> HeThong: hopLe
    HeThong -> DB: luuChiTietCongViecMoi()
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> QuanLyDuAn: thongBaoThemThanhCong()
else capNhatChiTietCongViec()
    HeThong -> DB: kiemTraQuyenVaTrangThaiCongViecCha()
    DB --> HeThong: duocCapNhat
    HeThong -> DB: capNhatChiTietCongViec()
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> QuanLyDuAn: thongBaoCapNhatThanhCong()
else xoaChiTietCongViec()
    HeThong -> DB: kiemTraQuyenVaTrangThaiCongViecCha()
    DB --> HeThong: duocXoa
    HeThong -> DB: danhDauXoaChiTietCongViec()
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> QuanLyDuAn: thongBaoXoaThanhCong()
else chiXemDanhSach
    HeThong --> QuanLyDuAn: hienThiDanhSachChiTietCongViec()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Phan cong cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: phanCongCongViec(maCongViec)
activate HeThong
HeThong -> DB: layThongTinCongViecVaThanhVienDuAn()
activate DB
DB --> HeThong: duLieuPhanCong
deactivate DB

alt themPhanCongCongViec()
    HeThong -> DB: kiemTraQuyenPhanCongTheoDuAnTeam()
    DB --> HeThong: coQuyen
    HeThong -> DB: kiemTraTrangThaiCongViecVaNhanVienHopLe()
    DB --> HeThong: hopLe
    HeThong -> DB: luuPhanCongCongViec()
    HeThong -> DB: ghiNhatKyPhanCongCongViec()
    HeThong --> QuanLyDuAn: thongBaoThemPhanCongThanhCong()
else xoaPhanCongCongViec()
    HeThong -> DB: kiemTraQuyenPhanCongTheoDuAnTeam()
    DB --> HeThong: coQuyen
    HeThong -> DB: xoaPhanCongCongViec()
    HeThong -> DB: ghiNhatKyXoaPhanCongCongViec()
    HeThong --> QuanLyDuAn: thongBaoXoaPhanCongThanhCong()
else chiXemDanhSachPhanCong
    HeThong --> QuanLyDuAn: hienThiDanhSachPhanCong()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Bao cao tien do cong viec

actor ":NhanVien" as NhanVien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> HeThong: guiBaoCaoTienDo(maChiTietCV)
activate HeThong
HeThong -> DB: kiemTraNguoiDuocPhanCongVaTrangThaiHienTai()
activate DB
DB --> HeThong: duocGuiBaoCao
deactivate DB

HeThong -> DB: kiemTraKhongCoBaoCaoChoDuyetTruocDo()
DB --> HeThong: hopLe
HeThong -> DB: kiemTraKhongLuiTrangThaiVaNoiDungBaoCao()
DB --> HeThong: hopLe

alt coFileMinhChung
    HeThong -> DB: luuBaoCaoTienDoChoDuyet()
    HeThong -> DB: luuFileMinhChungTienDo()
    HeThong --> NhanVien: thongBaoGuiBaoCaoThanhCong()
else khongCoFileMinhChung
    alt trangThaiCanFileBatBuoc
        HeThong --> NhanVien: thongBaoThieuFileMinhChung()
    else trangThaiKhongBatBuocFile
        HeThong -> DB: luuBaoCaoTienDoChoDuyet()
        HeThong --> NhanVien: thongBaoGuiBaoCaoThanhCong()
    end
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet bao cao tien do

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: duyetBaoCaoTienDo(maTienDo)
activate HeThong
HeThong -> DB: kiemTraQuyenDuyetTheoScopeDuAn()
activate DB
DB --> HeThong: coQuyen
deactivate DB

HeThong -> DB: kiemTraBaoCaoDangChoDuyetVaKhongTuDuyet()
DB --> HeThong: hopLe

alt duyetBaoCaoTienDo()
    HeThong -> DB: capNhatTrangThaiBaoCaoDaDuyet()
    HeThong -> DB: capNhatTrangThaiChiTietCongViecTheoDeXuat()
    ref over HeThong
    Dong bo chuoi trang thai tu cong viec
    end ref
    HeThong --> QuanLyDuAn: thongBaoDuyetThanhCong()
else yeuCauBoSungBaoCaoTienDo()
    HeThong -> DB: capNhatTrangThaiBaoCaoYeuCauBoSung()
    HeThong --> QuanLyDuAn: thongBaoDaGuiYeuCauBoSung()
else tuChoiBaoCaoTienDo()
    HeThong -> DB: capNhatTrangThaiBaoCaoTuChoi()
    HeThong --> QuanLyDuAn: thongBaoDaTuChoiBaoCao()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd De xuat cong viec

actor ":NhanVien" as NhanVien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> HeThong: deXuatCongViec(maDuAn)
activate HeThong
HeThong -> DB: kiemTraQuyenDeXuatTheoVaiTroDuAnTeam()
activate DB
DB --> HeThong: coQuyen
deactivate DB

alt taoDeXuatCongViec()
    HeThong -> DB: kiemTraTrangThaiDuAnVaNganSachHienHanh()
    DB --> HeThong: duocDeXuat
    HeThong -> DB: kiemTraChiPhiDeXuatVaTrungLapChoDuyet()
    DB --> HeThong: hopLe
    HeThong -> DB: luuDeXuatCongViecTrangThaiChoDuyet()
    HeThong -> DB: ghiNhatKyQuanLyDuAn()
    HeThong --> NhanVien: thongBaoTaoDeXuatThanhCong()
else huyDeXuatCongViec()
    HeThong -> DB: kiemTraNguoiTaoVaTrangThaiChoDuyet()
    DB --> HeThong: duocHuy
    HeThong -> DB: capNhatTrangThaiDeXuatDaHuy()
    HeThong --> NhanVien: thongBaoHuyDeXuatThanhCong()
else chiXemDanhSachDeXuat
    HeThong --> NhanVien: hienThiDanhSachDeXuatCongViec()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet de xuat cong viec

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: duyetDeXuatCongViec(maDeXuatCV)
activate HeThong
HeThong -> DB: kiemTraNguoiDuyetLaQuanLyDuAn()
activate DB
DB --> HeThong: coQuyen
deactivate DB

HeThong -> DB: kiemTraDeXuatDangChoDuyet()
DB --> HeThong: hopLe

alt pheDuyetDeXuat()
    HeThong -> DB: kiemTraDanhMucMucDoNganSachHienHanh()
    DB --> HeThong: hopLe
    HeThong -> DB: taoCongViecTuDeXuat()
    HeThong -> DB: taoChiPhiTuDeXuat()
    HeThong -> DB: capNhatTrangThaiDeXuatDaDuyet()
    alt duAnDangChoXacNhanHoanThanh
        HeThong -> DB: chuyenDuAnVeDangThucHien()
    end
    HeThong -> DB: ghiNhatKyDuyetDeXuat()
    HeThong --> QuanLyDuAn: thongBaoDuyetDeXuatThanhCong()
else tuChoiDeXuat()
    HeThong -> DB: capNhatTrangThaiDeXuatTuChoi()
    HeThong -> DB: ghiNhatKyTuChoiDeXuat()
    HeThong --> QuanLyDuAn: thongBaoTuChoiDeXuatThanhCong()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd De xuat ngan sach

actor ":NhanVien" as NhanVien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> HeThong: deXuatNganSach(maDuAn)
activate HeThong
HeThong -> DB: kiemTraQuyenDeXuatTheoVaiTroDuAnTeam()
activate DB
DB --> HeThong: coQuyen
deactivate DB

alt taoDeXuatNganSach()
    HeThong -> DB: kiemTraTrangThaiDuAnVaKhongCoDeXuatChoDuyet()
    DB --> HeThong: duocDeXuat
    HeThong -> DB: kiemTraNganSachDeXuatKhongNhoHonChiPhiDaDung()
    DB --> HeThong: hopLe
    HeThong -> DB: luuDeXuatNganSachTrangThaiChoDuyet()
    HeThong -> DB: ghiNhatKyNganSachVaNhatKyDuAn()
    HeThong --> NhanVien: thongBaoTaoDeXuatNganSachThanhCong()
else huyDeXuatNganSach()
    HeThong -> DB: kiemTraNguoiTaoVaTrangThaiChoDuyet()
    DB --> HeThong: duocHuy
    HeThong -> DB: capNhatTrangThaiDeXuatNganSachDaHuy()
    HeThong -> DB: ghiNhatKyHuyDeXuatNganSach()
    HeThong --> NhanVien: thongBaoHuyDeXuatNganSachThanhCong()
else chiXemDanhSachDeXuatNganSach
    HeThong --> NhanVien: hienThiDanhSachDeXuatNganSach()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet ngan sach

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: duyetDeXuatNganSach(maDeXuatNS)
activate HeThong
HeThong -> DB: kiemTraNguoiDuyetLaQuanLyDuAn()
activate DB
DB --> HeThong: coQuyen
deactivate DB

HeThong -> DB: kiemTraDeXuatNganSachDangChoDuyet()
DB --> HeThong: hopLe

alt pheDuyetDeXuatNganSach()
    HeThong -> DB: kiemTraNganSachDeXuatKhongNhoHonChiPhiDaDung()
    DB --> HeThong: hopLe
    HeThong -> DB: ngungKichHoatNganSachCu()
    HeThong -> DB: taoNganSachMoiVersionTangDan()
    HeThong -> DB: capNhatTrangThaiDeXuatDaDuyet()
    HeThong -> DB: ghiNhatKyNganSachVaNhatKyDuAn()
    HeThong --> QuanLyDuAn: thongBaoDuyetNganSachThanhCong()
else tuChoiDeXuatNganSach()
    HeThong -> DB: capNhatTrangThaiDeXuatTuChoi()
    HeThong -> DB: ghiNhatKyTuChoiNganSach()
    HeThong --> QuanLyDuAn: thongBaoTuChoiNganSachThanhCong()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Danh gia du an

actor ":QuanLyDuAn" as QuanLyDuAn
actor ":QuanTriVien" as QuanTriVien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: danhGiaDuAn(maDuAn)
activate HeThong
HeThong -> DB: layDuAnTieuChiThongKeVaDuLieuAI()
activate DB
DB --> HeThong: duLieuFormDanhGia
deactivate DB

ref over HeThong
sd AI du doan va phan tich nguyen nhan tre
end ref

ref over HeThong
sd Xac nhan nguyen nhan tre
end ref

alt luuDanhGiaDuAn()
    HeThong -> DB: kiemTraQuanLyDuAnVaTrangThaiHoSo()
    DB --> HeThong: duocLuu
    HeThong -> DB: luuDanhGiaDuAnVaChiTietTieuChi()
    HeThong --> QuanLyDuAn: thongBaoLuuDanhGiaThanhCong()
else guiDuyetDanhGiaDuAn()
    HeThong -> DB: kiemTraTrangThaiDuAnVaDuLieuTieuChi()
    DB --> HeThong: duocGuiDuyet
    HeThong -> DB: capNhatTrangThaiDanhGiaChoDuyet()
    HeThong --> QuanLyDuAn: thongBaoGuiDuyetThanhCong()
end

QuanTriVien -> HeThong: duyetHoacTuChoiDanhGiaDuAn()
alt hoSoDangChoDuyet
    HeThong -> DB: capNhatTrangThaiDaDuyetHoacTuChoi()
    HeThong --> QuanTriVien: thongBaoXuLyDanhGiaThanhCong()
else hoSoKhongHopLe
    HeThong --> QuanTriVien: thongBaoKhongTheXuLy()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Danh gia nhan vien

actor ":NhanVien" as NhanVien
actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> HeThong: danhGiaNhanVien(maDuAn, maNhanVien)
activate HeThong
HeThong -> DB: layTieuChiVaThongKeNhanVienTheoScope()
activate DB
DB --> HeThong: duLieuFormDanhGia
deactivate DB

alt luuDanhGiaNhanVien()
    HeThong -> DB: kiemTraKhongTuDanhGiaVaQuyenTheoScope()
    DB --> HeThong: duocLuu
    HeThong -> DB: luuDanhGiaNhanVienVaChiTiet()
    HeThong --> NhanVien: thongBaoLuuDanhGiaThanhCong()
else guiDuyetDanhGiaNhanVien()
    HeThong -> DB: kiemTraTrangThaiNhapHoacTuChoi()
    DB --> HeThong: duocGuiDuyet
    HeThong -> DB: capNhatTrangThaiDanhGiaChoDuyet()
    HeThong --> NhanVien: thongBaoGuiDuyetThanhCong()
end

QuanLyDuAn -> HeThong: duyetHoacTuChoiDanhGiaNhanVien()
alt hoSoDangChoDuyetVaDungPhamViDuAn
    HeThong -> DB: capNhatTrangThaiDaDuyetHoacTuChoi()
    HeThong --> QuanLyDuAn: thongBaoXuLyDanhGiaThanhCong()
else hoSoKhongHopLe
    HeThong --> QuanLyDuAn: thongBaoKhongTheXuLy()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd AI du doan va phan tich nguyen nhan tre

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: phanTichAiDuAn(maDuAn)
activate HeThong
HeThong -> DB: kiemTraScopeDuAnVaDieuKienPhanTichAI()
activate DB
DB --> HeThong: duocPhanTich
deactivate DB

alt duAnHoanThanhDungHan
    HeThong --> QuanLyDuAn: thongBaoKhongCanPhanTichNguyenNhanTre()
else canPhanTich
    HeThong -> DB: layAI_DATASETMoiNhatTheoDuAn()
    DB --> HeThong: duLieuDataset
    alt chuaCoDataset
        HeThong -> DB: tongHopAI_DATASETChoDuAn()
        HeThong -> DB: napLaiDuLieuDataset()
    end

    HeThong -> HeThong: goiModelDuDoanTreHanVaNguyenNhan()
    alt duDoanThanhCong
        HeThong -> DB: mapDanhMucNguyenNhanVaLuuAI_KET_QUA()
        alt mapNguyenNhanKhongKhop
            HeThong -> DB: dungDanhMucMacDinhDeLuuKetQua()
        end
        HeThong --> QuanLyDuAn: traKetQuaPhanTichAI()
    else duDoanThatBai
        HeThong --> QuanLyDuAn: thongBaoKhongKetNoiDuocDichVuAI()
    end
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Xac nhan nguyen nhan tre

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: xacNhanNguyenNhan(maDuAn, maDmNguyenNhan, doTinCay)
activate HeThong
HeThong -> DB: kiemTraQuyenVaScopeDuAn()
activate DB
DB --> HeThong: coQuyen
deactivate DB

HeThong -> DB: kiemTraDanhMucNguyenNhanTonTai()
DB --> HeThong: hopLe
HeThong -> DB: kiemTraDuAnDangTreTheoAI()
DB --> HeThong: canXacNhan

alt canXacNhan
    HeThong -> DB: themHoacCapNhatAI_NGUYEN_NHAN()
    HeThong --> QuanLyDuAn: thongBaoXacNhanThanhCong()
else khongCanXacNhan
    HeThong --> QuanLyDuAn: thongBaoDuAnKhongTre()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Yeu cau doi quan ly du an

actor ":QuanLyDuAn" as QuanLyDuAn
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanLyDuAn -> HeThong: taoYeuCauDoiQuanLy(maDuAn, maQuanLyDeXuat)
activate HeThong
HeThong -> DB: kiemTraNguoiTaoLaQuanLyHienTai()
activate DB
DB --> HeThong: hopLe
deactivate DB

HeThong -> DB: kiemTraTrangThaiDuAnVaYeuCauChoDuyetTonTai()
DB --> HeThong: duocTao
HeThong -> DB: kiemTraUngVienQuanLyCoRoleHoacChucDanhPhuHop()
DB --> HeThong: hopLe
HeThong -> DB: luuYEU_CAU_DOI_QUAN_LYTrangThaiChoDuyet()
HeThong -> DB: ghiNhatKyTaoYeuCau()
HeThong --> QuanLyDuAn: thongBaoTaoYeuCauThanhCong()

alt huyYeuCauDoiQuanLy()
    QuanLyDuAn -> HeThong: huyYeuCauDoiQuanLy(maYeuCau)
    HeThong -> DB: kiemTraNguoiTaoVaTrangThaiChoDuyet()
    DB --> HeThong: duocHuy
    HeThong -> DB: capNhatTrangThaiYeuCauDaHuy()
    HeThong -> DB: ghiNhatKyHuyYeuCau()
    HeThong --> QuanLyDuAn: thongBaoHuyYeuCauThanhCong()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Duyet yeu cau doi quan ly

actor ":QuanTriVien" as QuanTriVien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanTriVien -> HeThong: duyetYeuCauDoiQuanLy(maYeuCau)
activate HeThong
HeThong -> DB: kiemTraQuyenDuyetYeuCau()
activate DB
DB --> HeThong: coQuyen
deactivate DB

HeThong -> DB: layYeuCauDangChoDuyet()
DB --> HeThong: thongTinYeuCau

alt pheDuyetYeuCau
    HeThong -> DB: kiemTraQuanLyHienTaiChuaThayDoi()
    DB --> HeThong: hopLe
    HeThong -> DB: kiemTraQuanLyDeXuatHopLe()
    DB --> HeThong: hopLe
    HeThong -> DB: capNhatYeuCauDaDuyet()
    HeThong -> DB: capNhatQuanLyMoiChoDU_AN()
    HeThong -> DB: dongNhatKyQuanLyCuVaMoNhatKyQuanLyMoi()
    HeThong --> QuanTriVien: thongBaoDuyetYeuCauThanhCong()
else tuChoiYeuCau
    HeThong -> DB: capNhatYeuCauTuChoi()
    HeThong -> DB: ghiNhatKyTuChoiYeuCau()
    HeThong --> QuanTriVien: thongBaoTuChoiYeuCauThanhCong()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Chat du an

actor ":NhanVien" as NhanVien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

NhanVien -> HeThong: moChatDuAn(maDuAn)
activate HeThong
HeThong -> DB: kiemTraQuyenChatVaScopeDuAn()
activate DB
DB --> HeThong: duocTruyCap
deactivate DB

HeThong -> DB: damBaoPhongChatDuAn()
HeThong -> DB: dongBoThanhVienPhongChat()

loop voi moi phong chat trong scope
    HeThong -> DB: layThongTinPhongChatVaTinNhanMoiNhat()
    DB --> HeThong: thongTinPhongChat
end

alt guiTinNhan()
    NhanVien -> HeThong: guiTinNhan(maPhongChat, noiDung)
    HeThong -> DB: kiemTraThanhVienPhongVaTrangThaiDuAn()
    DB --> HeThong: duocGui
    HeThong -> DB: luuTIN_NHAN()
    HeThong --> NhanVien: capNhatKhungChat()
else chiXemTinNhan()
    HeThong -> DB: layDanhSachTIN_NHANTheoPhong()
    DB --> HeThong: danhSachTinNhan
    HeThong --> NhanVien: hienThiTinNhan()
end

deactivate HeThong
@enduml
```

```plantuml
@startuml
title sd Phan quyen

actor ":QuanTriVien" as QuanTriVien
participant ":HeThongQuanLyDuAn" as HeThong
database ":Database" as DB

QuanTriVien -> HeThong: quanLyPhanQuyen(roleId)
activate HeThong
HeThong -> DB: layDanhSachVaiTroVaDanhMucQuyen()
activate DB
DB --> HeThong: duLieuPhanQuyen
deactivate DB
HeThong --> QuanTriVien: hienThiMaTranPhanQuyen()

QuanTriVien -> HeThong: luuPhanQuyen(roleId, selectedPermissions)
HeThong -> DB: kiemTraVaiTroTonTaiVaDanhMucQuyenHopLe()
DB --> HeThong: hopLe

alt roleAdminThieuQuyenBatBuoc
    HeThong --> QuanTriVien: thongBaoAdminBatBuocCoQuyenPhanQuyen()
else hopLe
    HeThong -> DB: capNhatAspNetRoleClaimsTheoDanhSachMoi()
    HeThong --> QuanTriVien: thongBaoCapNhatPhanQuyenThanhCong()
end

deactivate HeThong
@enduml
```
