namespace QuanLyDuAn.Models.Entities;

public partial class TinNhan
{
    public int MaTinNhan { get; set; }
    public int MaPhongChat { get; set; }
    public int MaNguoiDung { get; set; }
    public string? NoiDungTinNhan { get; set; }
    public DateTime? ThoiGianGui { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedBy { get; set; }
}
