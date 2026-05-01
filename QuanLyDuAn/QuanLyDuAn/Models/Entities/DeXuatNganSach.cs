namespace QuanLyDuAn.Models.Entities;

public partial class DeXuatNganSach
{
    public int MaDeXuatNS { get; set; }
    public int MaDuAn { get; set; }
    public int? MaNganSachCu { get; set; }
    public decimal? NganSachCu { get; set; }
    public decimal? NganSachDeXuat { get; set; }
    public string? LyDoDeXuat { get; set; }
    public int MaNguoiDungDeXuat { get; set; }
    public int? MaNguoiDungDuyet { get; set; }
    public DateTime? NgayDeXuat { get; set; }
    public DateTime? NgayDuyet { get; set; }
    public string? TrangThaiDeXuat { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
