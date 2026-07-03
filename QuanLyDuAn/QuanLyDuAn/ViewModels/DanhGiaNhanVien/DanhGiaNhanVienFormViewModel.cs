using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.DanhGiaNhanVien
{
    public class DanhGiaNhanVienFormViewModel
    {
        public int? MaDanhGiaNhanVien { get; set; }

        [Required]
        public int MaDuAn { get; set; }

        public string TenDuAn { get; set; } = string.Empty;

        [Required]
        public int MaNhanVien { get; set; }

        public string TenNhanVien { get; set; } = string.Empty;
        public string VaiTroTrongDuAn { get; set; } = string.Empty;
        public int TongChiTietDuocGiao { get; set; }
        public int ChiTietHoanThanh { get; set; }
        public int ChiTietDangLam { get; set; }
        public int ChiTietTreHan { get; set; }
        public double TyLeHoanThanh { get; set; }
        public int SoLanCapNhatTienDo { get; set; }
        public DateTime? LanCapNhatGanNhat { get; set; }
        public int SoFileMinhChung { get; set; }
        public double DiemTrungBinhTienDo { get; set; }

        [MinLength(1, ErrorMessage = "Vui lòng nhập ít nhất một tiêu chí đánh giá.")]
        public List<DanhGiaNhanVienTieuChiViewModel> TieuChi { get; set; } = new();

        [StringLength(500)]
        public string? NhanXetTongQuan { get; set; }

        public double DiemTongKet { get; set; }
        public string XepLoai { get; set; } = string.Empty;
        public string TrangThaiDanhGia { get; set; } = string.Empty;

        public bool CoTheLuu { get; set; }
        public bool CoTheGuiDuyet { get; set; }
        public bool CoTheDuyet { get; set; }
        public bool CoTheTuChoi { get; set; }

        public DanhGiaNhanVienThongKeViewModel ThongKe { get; set; } = new();
    }
}
