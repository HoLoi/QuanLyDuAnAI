namespace QuanLyDuAn.Models.Entities;

public partial class DuAn
{
    public int MaDuAn { get; set; }
    public int MaNguoiDung { get; set; }
    public int MaLoaiDuAn { get; set; }
    public string? TenDuAn { get; set; }
    public string? MoTaDuAn { get; set; }
    public DateTime? NgayTaoDuAn { get; set; }
    public DateTime? NgayBatDauDuAn { get; set; }
    public DateTime? NgayKetThucDuAn { get; set; }
    public int? PhanTramHoanThanh { get; set; }
    public string? TrangThaiDuAn { get; set; }
    public string? GhiChuDuAn { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
