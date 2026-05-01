namespace QuanLyDuAn.Models.Entities;

public partial class NhatKyNganSach
{
    public int MaNhatKyNS { get; set; }
    public int MaNganSach { get; set; }
    public int MaDuAn { get; set; }
    public decimal? SoTienNKNS { get; set; }
    public decimal? NganSachTruoc { get; set; }
    public decimal? NganSachSau { get; set; }
    public DateTime? NkNgayCapNhatNS { get; set; }
    public string? NkTrangThaiNganSach { get; set; }
    public string? HanhDongNKNS { get; set; }
    public DateTime? ThoiGianNKNS { get; set; }
}
