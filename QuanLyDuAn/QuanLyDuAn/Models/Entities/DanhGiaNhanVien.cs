namespace QuanLyDuAn.Models.Entities;

public partial class DanhGiaNhanVien
{
    public int MaDanhGiaNhanVien { get; set; }
    public int MaNguoiDung { get; set; }
    public int MaDuAn { get; set; }
    public int MaNguoiDungDanhGia { get; set; }
    public int? DiemTongDanhGiaNV { get; set; }
    public DateTime? NgayDanhGiaNV { get; set; }
    public string? XepLoai { get; set; }
    public string? NhanXetTongQuanNV { get; set; }
    public string? TrangThaiDanhGiaNV { get; set; }
    public int? MaNguoiDungDuyet { get; set; }
    public DateTime? NgayDuyetDanhGiaNV { get; set; }
    public string? LyDoTuChoiDanhGiaNV { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}

