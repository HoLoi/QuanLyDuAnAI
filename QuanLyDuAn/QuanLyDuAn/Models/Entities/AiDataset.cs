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
    public double? TongGioLam { get; set; }
    public int? SoLanThayDoiNhanSu { get; set; }
    public int? SoLanThayDoiQuanLy { get; set; }
    public int? SoNgayTreTienDo { get; set; }
    public bool? IsTre { get; set; }
}
