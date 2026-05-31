namespace QuanLyDuAn.Models.Entities;

public partial class AiDataset
{
    public int MaData { get; set; }
    public int MaDuAn { get; set; }
    public int? SoNhanVienDuAn { get; set; }
    public int? TongSoCongViec { get; set; }
    public int? SoCongViecTre { get; set; }
    public double? TyLeCongViecTre { get; set; }
    public decimal? ChiPhiDuKien { get; set; }
    public decimal? ChiPhiThucTe { get; set; }
    public decimal? ChenhLechChiPhi { get; set; }
    public int? SoLanThayDoiNhanSu { get; set; }
    public int? SoLanThayDoiQuanLy { get; set; }
    public int? SoNgayTreTienDo { get; set; }
    public int? SoDeXuatCongViecChoDuyet { get; set; }
    public int? SoDeXuatCongViecBiTuChoi { get; set; }
    public double? ThoiGianDuyetCongViecTrungBinh { get; set; }
    public int? SoDeXuatNganSachChoDuyet { get; set; }
    public int? SoDeXuatNganSachBiTuChoi { get; set; }
    public double? ThoiGianDuyetNganSachTrungBinh { get; set; }
    public int? SoBaoCaoTienDoChoDuyet { get; set; }
    public int? SoBaoCaoTienDoBiTuChoi { get; set; }
    public int? SoBaoCaoTienDoYeuCauBoSung { get; set; }
    public double? TyLeBaoCaoTienDoBiTuChoi { get; set; }
    public int? SoLanCapNhatTienDo { get; set; }
    public int? SoNgayChamCapNhatTienDo { get; set; }
    public bool? LaDuAnTre { get; set; }
    public int? MaDMNguyenNhan { get; set; }
    public DateTime? NgayTongHop { get; set; }
    public string? GhiChuDataset { get; set; }
}
