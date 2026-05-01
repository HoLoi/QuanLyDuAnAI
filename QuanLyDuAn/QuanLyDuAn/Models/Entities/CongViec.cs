namespace QuanLyDuAn.Models.Entities;

public partial class CongViec
{
    public int MaCongViec { get; set; }
    public int? MaDeXuatCV { get; set; }
    public int MaDanhMucCV { get; set; }
    public int MaMucDo { get; set; }
    public string? TenCongViec { get; set; }
    public string? MoTaCongViec { get; set; }
    public DateTime? NgayBatDauCongViec { get; set; }
    public DateTime? NgayKetThucCVDuKien { get; set; }
    public DateTime? NgayKetThucCVThucTe { get; set; }
    public DateTime? NgayTaoCongViec { get; set; }
    public string? TrangThaiCongViec { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
