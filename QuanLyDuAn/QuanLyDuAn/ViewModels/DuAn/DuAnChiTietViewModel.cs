namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnChiTietViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
        public string? MoTaDuAn { get; set; }
        public int MaLoaiDuAn { get; set; }
        public string TenLoaiDuAn { get; set; } = string.Empty;
        public DateTime? NgayTaoDuAn { get; set; }
        public DateTime? NgayBatDauDuAn { get; set; }
        public DateTime? NgayKetThucDuAn { get; set; }
        public DateTime? NgayHoanThanhThucTeDuAn { get; set; }
        public string TrangThaiDuAn { get; set; } = string.Empty;
        public int PhanTramHoanThanh { get; set; }
        public string? GhiChuDuAn { get; set; }
        public int MaNguoiDung { get; set; }
        public string TenNguoiQuanLy { get; set; } = string.Empty;

        public int SoLuongTeam { get; set; }
        public int SoLuongThanhVien { get; set; }
        public int SoLuongCongViec { get; set; }
        public int SoLuongChiTietCongViec { get; set; }

        public int? MaCongViecDauTien { get; set; }

        public List<DuAnFileItemViewModel> DanhSachFile { get; set; } = new();
        public bool CoTheQuanLyFile { get; set; }
        public bool CoTheYeuCauDoiQuanLy { get; set; }
        public string? LyDoKhongTheYeuCauDoiQuanLy { get; set; }

        public ProjectStatusCheckViewModel? StatusCheck { get; set; }
        public bool HasApprovedBudget { get; set; }
        public int? SoNgayConLai { get; set; }
        public bool IsSapDenHan { get; set; }
        public bool IsQuaHan { get; set; }

        public DuAnBudgetSummaryViewModel? NganSachTongHop { get; set; }
        public DuAnWorkStatusSummaryViewModel TienDoCongViec { get; set; } = new();
        public DuAnDeadlinePreviewViewModel? DeadlineGanNhat { get; set; }
        public List<DuAnRecentWorkItemViewModel> CongViecGanDay { get; set; } = new();
        public List<DuAnRecentFileViewModel> TepGanDay { get; set; } = new();
        public List<DuAnMemberPreviewViewModel> ThanhVienNoiBat { get; set; } = new();
        public List<DuAnActivityPreviewViewModel> HoatDongGanDay { get; set; } = new();

        public string? TuKhoa { get; set; }
        public int? LocMaLoaiDuAn { get; set; }
        public string? LocTrangThaiDuAn { get; set; }
        public DateTime? TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public string? LocTheoNgay { get; set; }

        public HashSet<string> Permissions { get; set; } = new();
    }
}
