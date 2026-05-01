namespace QuanLyDuAn.Models.Entities;

public partial class NganSach
{
    public int MaNganSach { get; set; }
    public int MaNguoiDungDuyet { get; set; }
    public int MaNguoiDungDeXuat { get; set; }
    public int MaDuAn { get; set; }
    public decimal? SoTienNganSach { get; set; }
    public int? Version { get; set; }
    public bool? IsActive { get; set; }
    public string? MoTaNganSach { get; set; }
    public DateTime? NgayCapNhatNganSach { get; set; }
    public DateTime? NgayDuyetNganSach { get; set; }
    public string? TrangThaiNganSach { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
