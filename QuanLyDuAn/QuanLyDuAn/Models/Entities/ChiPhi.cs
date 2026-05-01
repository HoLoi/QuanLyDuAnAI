namespace QuanLyDuAn.Models.Entities;

public partial class ChiPhi
{
    public int MaChiPhi { get; set; }
    public int MaCongViec { get; set; }
    public int MaNganSach { get; set; }
    public string? NoiDungChiPhi { get; set; }
    public decimal? SoTienDaChi { get; set; }
    public DateTime? NgayChi { get; set; }
    public string? TrangThaiChiPhi { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
