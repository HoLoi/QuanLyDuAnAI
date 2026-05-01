namespace QuanLyDuAn.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public int TongDuAn { get; set; }
        public int TongCongViec { get; set; }
        public int TongNhanVien { get; set; }
        public decimal TongNganSach { get; set; }

        public List<string> TenDuAn { get; set; } = [];
        public List<int> PhanTramTienDo { get; set; } = [];
        public List<decimal> ChiPhiTheoDuAn { get; set; } = [];

        public int DuAnDungTienDo { get; set; }
        public int DuAnTreTienDo { get; set; }
        public int CongViecTreHan { get; set; }
        public int NhanSuQuaTai { get; set; }
        public int DuAnVuotNganSach { get; set; }
        public int DuAnThieuDatasetAi { get; set; }

        public List<WorkflowHealthItemViewModel> WorkflowHealthItems { get; set; } = [];
        public List<SuggestionItemViewModel> Suggestions { get; set; } = [];
    }
}
