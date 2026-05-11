namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnThongKeViewModel
    {
        public string TenDuAn { get; set; } = string.Empty;
        public string TenNguoiQuanLy { get; set; } = string.Empty;
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
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
        public string? TenNguyenNhanAiDuDoan { get; set; }
        public double? DoTinCayAi { get; set; }
        public DateTime? ThoiGianDuDoanAi { get; set; }
        public string? TenNguyenNhanManagerXacNhan { get; set; }
        public double? DoTinCayManagerXacNhan { get; set; }
        public string? TrangThaiDuLieuAi { get; set; }

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
