namespace QuanLyDuAn.ViewModels.DanhMucCongViec
{
    public class DanhMucCongViecViewModel
    {
        public int MaDanhMucCV { get; set; }
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string TenDanhMucCV { get; set; } = string.Empty;
        public string? MoTaDanhMucCV { get; set; }
        public DateTime? NgayTaoDMCV { get; set; }
        public int SoLuongCongViec { get; set; }
    }
}
