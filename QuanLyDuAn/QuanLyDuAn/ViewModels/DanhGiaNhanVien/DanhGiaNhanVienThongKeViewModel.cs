namespace QuanLyDuAn.ViewModels.DanhGiaNhanVien
{
    public class DanhGiaNhanVienThongKeViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public int MaNhanVien { get; set; }
        public string TenNhanVien { get; set; } = string.Empty;
        public string VaiTroTrongDuAn { get; set; } = string.Empty;

        public int TongChiTietDuocGiao { get; set; }
        public int ChiTietHoanThanh { get; set; }
        public int ChiTietDangLam { get; set; }
        public int ChiTietTreHan { get; set; }
        public double TyLeHoanThanh { get; set; }

        public int SoLanCapNhatTienDo { get; set; }
        public int SoBaoCaoDaDuyet { get; set; }
        public int SoBaoCaoTuChoiHoacBoSung { get; set; }
        public int SoFileMinhChung { get; set; }
        public DateTime? LanCapNhatGanNhat { get; set; }
        public double DiemTrungBinhTienDo { get; set; }

        public int SoBaoCaoTienDoDaGui
        {
            get => SoLanCapNhatTienDo;
            set => SoLanCapNhatTienDo = value;
        }

        public DateTime? LanCapNhatTienDoGanNhat
        {
            get => LanCapNhatGanNhat;
            set => LanCapNhatGanNhat = value;
        }
    }
}
