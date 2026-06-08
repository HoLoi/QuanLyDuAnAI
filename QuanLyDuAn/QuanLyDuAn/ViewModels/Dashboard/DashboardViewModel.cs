namespace QuanLyDuAn.ViewModels.Dashboard
{
    public class DashboardViewModel
    {
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? LocNhanh { get; set; }
        public int? LocMaDuAn { get; set; }
        public int? LocMaQuanLy { get; set; }
        public int? LocMaTeam { get; set; }
        public string? LocTrangThai { get; set; }
        public int? LocMaLoaiDuAn { get; set; }
        public string? LocTheoNgay { get; set; }

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
        public int DeXuatCongViecChoDuyet { get; set; }
        public int DeXuatNganSachChoDuyet { get; set; }
        public int YeuCauDoiQuanLyChoDuyet { get; set; }

        public List<DashboardFilterOptionViewModel> DuAnOptions { get; set; } = [];
        public List<DashboardFilterOptionViewModel> QuanLyOptions { get; set; } = [];
        public List<DashboardFilterOptionViewModel> TeamOptions { get; set; } = [];
        public List<DashboardFilterOptionViewModel> LoaiDuAnOptions { get; set; } = [];
        public List<DashboardStatusChartItemViewModel> TrangThaiDuAnChart { get; set; } = [];
        public List<DashboardStatusChartItemViewModel> TrangThaiCongViecChart { get; set; } = [];
        public List<DashboardTimelineItemViewModel> ThongKeTheoThang { get; set; } = [];
        public List<DashboardDelayedProjectItemViewModel> TopDuAnTre { get; set; } = [];
        public List<DashboardBudgetOverrunItemViewModel> TopDuAnVuotNganSach { get; set; } = [];
        public List<DashboardOverloadedEmployeeItemViewModel> TopNhanSuQuaTai { get; set; } = [];
        public List<DashboardDelayedTaskItemViewModel> TopCongViecTre { get; set; } = [];

        public List<WorkflowHealthItemViewModel> WorkflowHealthItems { get; set; } = [];
        public List<SuggestionItemViewModel> Suggestions { get; set; } = [];
    }

    public class DashboardFilterOptionViewModel
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class DashboardStatusChartItemViewModel
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class DashboardTimelineItemViewModel
    {
        public string Label { get; set; } = string.Empty;
        public int DuAnTaoMoi { get; set; }
        public int DuAnHoanThanh { get; set; }
        public int CongViecHoanThanh { get; set; }
        public int CongViecTre { get; set; }
        public decimal ChiPhi { get; set; }
    }

    public class DashboardDelayedProjectItemViewModel
    {
        public string TenDuAn { get; set; } = string.Empty;
        public string TenQuanLy { get; set; } = string.Empty;
        public DateTime? NgayKetThuc { get; set; }
        public int SoNgayTre { get; set; }
        public int PhanTramHoanThanh { get; set; }
    }

    public class DashboardBudgetOverrunItemViewModel
    {
        public string TenDuAn { get; set; } = string.Empty;
        public decimal NganSach { get; set; }
        public decimal ChiPhi { get; set; }
        public decimal ChenhLech { get; set; }
    }

    public class DashboardOverloadedEmployeeItemViewModel
    {
        public string TenNhanVien { get; set; } = string.Empty;
        public int SoCongViec { get; set; }
        public int SoChiTietCongViec { get; set; }
        public int CongViecTre { get; set; }
    }

    public class DashboardDelayedTaskItemViewModel
    {
        public string TenCongViec { get; set; } = string.Empty;
        public string TenDuAn { get; set; } = string.Empty;
        public string NguoiPhuTrach { get; set; } = string.Empty;
        public int SoNgayTre { get; set; }
    }

}
