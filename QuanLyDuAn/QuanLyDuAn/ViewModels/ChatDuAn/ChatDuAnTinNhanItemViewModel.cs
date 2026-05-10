namespace QuanLyDuAn.ViewModels.ChatDuAn
{
    public class ChatDuAnTinNhanItemViewModel
    {
        public int MaTinNhan { get; set; }
        public int MaPhongChat { get; set; }
        public int MaNguoiDung { get; set; }
        public string HoTenNguoiDung { get; set; } = string.Empty;
        public string? AnhDaiDien { get; set; }
        public string NoiDungTinNhan { get; set; } = string.Empty;
        public DateTime? ThoiGianGui { get; set; }
        public bool LaTinNhanCuaToi { get; set; }
    }
}
