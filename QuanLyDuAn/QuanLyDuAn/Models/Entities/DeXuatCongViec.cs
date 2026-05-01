namespace QuanLyDuAn.Models.Entities;

public partial class DeXuatCongViec
{
    public int MaDeXuatCV { get; set; }
    public int MaDuAn { get; set; }
    public int MaDanhMucCV { get; set; }
    public int MaMucDo { get; set; }
    public int MaNguoiDungDeXuat { get; set; }
    public int? MaNguoiDungDuyet { get; set; }
    public string? TenCongViecDeXuat { get; set; }
    public string? MoTaCongViecDeXuat { get; set; }
    public decimal? ChiPhiDeXuat { get; set; }
    public DateTime? NgayBatDauCongViecDeXuat { get; set; }
    public DateTime? NgayKetThucCVDeXuatDuKien { get; set; }
    public DateTime? NgayDeXuatCongViec { get; set; }
    public DateTime? NgayDuyetDeXuatCongViec { get; set; }
    public string? TrangThaiCongViecDeXuat { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
