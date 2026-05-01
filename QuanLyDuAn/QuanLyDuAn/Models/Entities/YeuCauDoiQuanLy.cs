namespace QuanLyDuAn.Models.Entities;

public partial class YeuCauDoiQuanLy
{
    public int MaYeuCauDoiQuanLy { get; set; }
    public int MaQuanLyDeXuat { get; set; }
    public int MaDuAn { get; set; }
    public int? MaNguoiDungDuyet { get; set; }
    public int MaQuanLyHienTai { get; set; }
    public string? TrangThaiYeuCauDoiQuanLy { get; set; }
    public DateTime? NgayTaoYeuCauDoiQuanLy { get; set; }
    public DateTime? NgayDuyetYeuCauDoiQuanLy { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
