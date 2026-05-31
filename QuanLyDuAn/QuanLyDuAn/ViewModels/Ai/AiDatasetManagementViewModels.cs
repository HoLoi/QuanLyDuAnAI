namespace QuanLyDuAn.ViewModels.Ai
{
    public class AiDatasetTongHopResultViewModel
    {
        public int TongSoDuAnXuLy { get; set; }
        public int SoDuAnTaoMoi { get; set; }
        public int SoDuAnCapNhat { get; set; }
        public int SoDuAnBoQua { get; set; }
        public DateTime ThoiGianTongHop { get; set; } = DateTime.Now;
        public List<string> ThongBao { get; set; } = [];
    }

    public class AiDatasetQualitySummaryViewModel
    {
        public int TongSoDong { get; set; }
        public int SoDongDuLabel { get; set; }
        public int SoDongThieuLabel { get; set; }
        public int SoDongHopLeTrain { get; set; }
        public int SoMauDuAnTre { get; set; }
        public int SoMauKhongTre { get; set; }
        public int SoDongCoNguyenNhan { get; set; }
        public int MinTrainRows { get; set; }
        public bool DuDieuKienTrain { get; set; }
        public Dictionary<string, int> PhanBoTheoNguyenNhan { get; set; } = [];
        public List<string> LyDoKhongDat { get; set; } = [];
        public List<string> GhiChuChatLuongDuLieu { get; set; } = [];
    }

    public class AiDatasetProjectOptionViewModel
    {
        public int MaDuAn { get; set; }
        public string TenDuAn { get; set; } = string.Empty;
    }

    public class AiReasonTrainingQualitySummaryViewModel
    {
        public int TongSoDongJoin { get; set; }
        public int SoDongHopLeTrain { get; set; }
        public int SoLoaiNguyenNhan { get; set; }
        public int MinTrainRows { get; set; }
        public int MinLoaiNguyenNhan { get; set; }
        public int MinDongMoiLoai { get; set; }
        public bool DuDieuKienTrain { get; set; }
        public Dictionary<string, int> PhanBoTheoNguyenNhan { get; set; } = [];
        public double ImbalanceRatio { get; set; }
        public List<string> LyDoKhongDat { get; set; } = [];
        public List<string> GhiChuChatLuongDuLieu { get; set; } = [];
    }
}
