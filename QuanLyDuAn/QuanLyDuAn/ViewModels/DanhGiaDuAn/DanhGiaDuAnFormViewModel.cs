using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnFormViewModel
    {
        public int? MaDanhGiaDuAn { get; set; }

        [Required]
        public int MaDuAn { get; set; }

        public string TenDuAn { get; set; } = string.Empty;
        public string TenNguoiQuanLy { get; set; } = string.Empty;
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int PhanTramHoanThanh { get; set; }
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public int TongCongViec { get; set; }
        public int CongViecHoanThanh { get; set; }
        public int CongViecTreHan { get; set; }
        public int TongChiTietCongViec { get; set; }
        public int ChiTietHoanThanh { get; set; }
        public int ChiTietTreHan { get; set; }
        public int SoBaoCaoTienDo { get; set; }
        public int SoBaoCaoMoiNhat { get; set; }
        public decimal TongNganSach { get; set; }
        public decimal TongChiPhi { get; set; }
        public double TyLeSuDungNganSach { get; set; }
        public bool? CoDuLieuAi { get; set; }
        public bool? DuAnBiTreTheoAi { get; set; }
        public string? TenNguyenNhanAiDuDoan { get; set; }
        public double? DoTinCayAi { get; set; }
        public DateTime? ThoiGianDuDoanAi { get; set; }
        public string? TenNguyenNhanManagerXacNhan { get; set; }
        public double? DoTinCayManagerXacNhan { get; set; }
        public string? TrangThaiDuLieuAi { get; set; }

        public List<DanhGiaDuAnTieuChiViewModel> TieuChi { get; set; } = new();

        [StringLength(500)]
        public string? NhanXetTongDuAn { get; set; }

        public double DiemTongKet { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public string TrangThaiDanhGia { get; set; } = string.Empty;

        public bool CoTheLuu { get; set; }
        public bool CoTheGuiDuyet { get; set; }
        public bool CoTheDuyet { get; set; }
        public bool CoTheTuChoi { get; set; }

        public DanhGiaDuAnThongKeViewModel ThongKe { get; set; } = new();
    }
}
