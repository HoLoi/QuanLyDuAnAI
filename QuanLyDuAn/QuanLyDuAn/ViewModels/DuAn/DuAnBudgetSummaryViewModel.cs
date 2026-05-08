namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnBudgetSummaryViewModel
    {
        public decimal? TongNganSachDaDuyet { get; set; }
        public decimal TongChiPhiDaDung { get; set; }
        public decimal? SoTienConLai { get; set; }
        public decimal? PhanTramSuDung { get; set; }
        public decimal PhanTramSuDungCap { get; set; }
        public string TrangThaiHienThi { get; set; } = string.Empty;
        public string TrangThaiCss { get; set; } = "normal";
        public bool CoNganSachDaDuyet { get; set; }
        public bool CoChiPhi { get; set; }
        public bool VuotNganSach { get; set; }
        public bool SapVuotNganSach { get; set; }
    }
}
