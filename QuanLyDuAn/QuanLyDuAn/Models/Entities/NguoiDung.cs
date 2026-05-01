namespace QuanLyDuAn.Models.Entities;

public partial class NguoiDung
{
    public int MaNguoiDung { get; set; }
    public int MaChucDanh { get; set; }
    public string Id { get; set; } = null!;
    public string? HoTenNguoiDung { get; set; }
    public string? DiaChiNguoiDung { get; set; }
    public string? SdtNguoiDung { get; set; }
    public DateTime? NgaySinh { get; set; }
    public string? AnhDaiDien { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
