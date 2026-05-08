namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnWorkStatusSummaryViewModel
    {
        public int TongCongViec { get; set; }
        public int CongViecHoanThanh { get; set; }
        public int CongViecDangThucHien { get; set; }
        public int CongViecTreHan { get; set; }
        public int CongViecTamDung { get; set; }
        public int CongViecChuaBatDau { get; set; }
        public decimal? TiLeHoanThanh { get; set; }
        public decimal TiLeHoanThanhCap { get; set; }
    }
}
