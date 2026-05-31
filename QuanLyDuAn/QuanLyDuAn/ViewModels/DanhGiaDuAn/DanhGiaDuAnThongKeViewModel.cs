namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnThongKeViewModel
    {
        public string TenDuAn { get; set; } = string.Empty;
        public string TenNguoiQuanLy { get; set; } = string.Empty;
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public DateTime? NgayKetThucThucTeDuAn { get; set; }
        public int? SoNgayConLai { get; set; }
        public int? SoNgayQuaHan { get; set; }
        public string TrangThaiThoiHanDuAn { get; set; } = string.Empty;
        public bool ChuaCoMocKetThucDuKien { get; set; }
        public int PhanTramHoanThanh { get; set; }

        public int TongCongViec { get; set; }
        public int CongViecHoanThanh { get; set; }
        public int CongViecTreHan { get; set; }
        public int TongChiTietCongViec { get; set; }
        public int ChiTietHoanThanh { get; set; }
        public int ChiTietTreHan { get; set; }
        public double TyLeHoanThanh { get; set; }
        public int SoBaoCaoTienDo { get; set; }
        public int SoBaoCaoMoiNhat { get; set; }
        public decimal TongNganSach { get; set; }
        public decimal TongChiPhi { get; set; }
        public double TyLeSuDungNganSach { get; set; }
        public int SoFileDuAn { get; set; }
        public bool? CoDuLieuAi { get; set; }
        public bool? DuAnBiTreTheoAi { get; set; }
        public int? MaDmNguyenNhanAiDuDoan { get; set; }
        public string? TenNguyenNhanAiDuDoan { get; set; }
        public string? NguonNguyenNhanAi { get; set; }
        public string? TenModelTreHanAi { get; set; }
        public string? TenModelNguyenNhanAi { get; set; }
        public double? DoTinCayAi { get; set; }
        public string? MucPhuHopAi { get; set; }
        public List<DanhGiaDuAnRelatedReasonViewModel>? DanhSachNguyenNhanLienQuan { get; set; }
        public DateTime? ThoiGianDuDoanAi { get; set; }
        public int? MaDmNguyenNhanManagerXacNhan { get; set; }
        public string? TenNguyenNhanManagerXacNhan { get; set; }
        public double? DoTinCayManagerXacNhan { get; set; }
        public DateTime? ThoiGianManagerXacNhan { get; set; }
        public string? TrangThaiDuLieuAi { get; set; }
        public bool KetQuaAiCoTheDaCu { get; set; }
        public string? CanhBaoDuLieuAi { get; set; }
        public bool CoThePhanTichAi { get; set; }
        public bool CanPhanTichAi { get; set; }
        public bool TuDongPhanTichAi { get; set; }
        public string? LyDoCanPhanTichAi { get; set; }

        public DateTime? NgayBatDau
        {
            get => NgayBatDauDuAn;
            set => NgayBatDauDuAn = value;
        }

        public DateTime? NgayKetThuc
        {
            get => NgayKetThucDuAn;
            set => NgayKetThucDuAn = value;
        }

        public int SoCongViec
        {
            get => TongCongViec;
            set => TongCongViec = value;
        }

        public int SoChiTietCongViec
        {
            get => TongChiTietCongViec;
            set => TongChiTietCongViec = value;
        }

        public int SoCongViecHoanThanh
        {
            get => CongViecHoanThanh;
            set => CongViecHoanThanh = value;
        }

        public int SoCongViecTreHan
        {
            get => CongViecTreHan;
            set => CongViecTreHan = value;
        }

        public decimal TongNganSachDaDuyet
        {
            get => TongNganSach;
            set => TongNganSach = value;
        }

        public decimal TongChiPhiDaDung
        {
            get => TongChiPhi;
            set => TongChiPhi = value;
        }
    }
}
