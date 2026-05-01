namespace QuanLyDuAn.Models.Entities;

public partial class AiKetQua
{
    public int MaAiKetQua { get; set; }
    public int MaDMNguyenNhan { get; set; }
    public int MaModel { get; set; }
    public int MaData { get; set; }
    public int MaDuAn { get; set; }
    public double? DoTinCayKetQua { get; set; }
    public DateTime? ThoiGianDuDoanKetQua { get; set; }
}
