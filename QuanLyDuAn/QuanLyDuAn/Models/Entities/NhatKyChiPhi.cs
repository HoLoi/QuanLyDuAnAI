namespace QuanLyDuAn.Models.Entities;

public partial class NhatKyChiPhi
{
    public int MaNhatKyCP { get; set; }
    public int MaCongViec { get; set; }
    public int MaChiPhi { get; set; }
    public decimal? NkSoTienDaChi { get; set; }
    public DateTime? NkNgayChi { get; set; }
    public string? NkTrangThaiChiPhi { get; set; }
    public string? HanhDongNKCP { get; set; }
    public DateTime? ThoiGianNKCP { get; set; }
}
