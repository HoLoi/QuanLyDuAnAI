namespace QuanLyDuAn.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? LocNhanh { get; set; }

        public int TongDuAn { get; set; }
        public int TongCongViec { get; set; }
        public int TongNhanVien { get; set; }
        public decimal TongNganSach { get; set; }
        public decimal TongChiPhi { get; set; }
        public int TongDeXuat { get; set; }
        public int DuAnHoanThanhTrongKy { get; set; }
        public decimal NganSachConLai { get; set; }
        public decimal TyLeSuDungNganSach { get; set; }

        public List<string> TenDuAn { get; set; } = [];
        public List<int> PhanTramTienDo { get; set; } = [];
        public List<decimal> ChiPhiTheoDuAn { get; set; } = [];

        public int DuAnKhoiTao { get; set; }
        public int DuAnDangThucHien { get; set; }
        public int DuAnTamDung { get; set; }
        public int DuAnChoXacNhanHoanThanh { get; set; }
        public int DuAnHoanThanh { get; set; }
        public int DuAnHoanThanhDungHan { get; set; }
        public int DuAnHoanThanhTreHan { get; set; }
        public int DuAnLuuTru { get; set; }
        public int DuAnDungTienDo { get; set; }
        public int DuAnTreTienDo { get; set; }
        public int CongViecTreHan { get; set; }
        public int NhanSuQuaTai { get; set; }
        public int DuAnVuotNganSach { get; set; }
        public int DuAnThieuDatasetAi { get; set; }
        public int DeXuatCongViecChoDuyet { get; set; }
        public int DeXuatNganSachChoDuyet { get; set; }
        public int YeuCauDoiQuanLyChoDuyet { get; set; }

        public List<WorkflowHealthItemViewModel> WorkflowHealthItems { get; set; } = [];
        public List<SuggestionItemViewModel> Suggestions { get; set; } = [];
    }
}
