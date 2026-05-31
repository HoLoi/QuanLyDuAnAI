namespace QuanLyDuAn.ViewModels.DanhGiaDuAn
{
    public class DanhGiaDuAnRelatedReasonViewModel
    {
        public int? MaDMNguyenNhan { get; set; }
        public string? TenNguyenNhan { get; set; }
        public double? Score { get; set; }
        public string? MucPhuHop { get; set; }
    }

    public class DanhGiaDuAnAiInsightViewModel
    {
        public bool CoDuLieuAi { get; set; }
        public bool? DuAnBiTreTheoAi { get; set; }
        public string TinhTrangTienDo { get; set; } = "Chưa có dữ liệu AI";
        public string NguyenNhanAiDuDoan { get; set; } = "Chưa có gợi ý AI";
        public string DoTinCayAi { get; set; } = "Chưa có";
        public string ThoiGianDuDoanAi { get; set; } = "Chưa có";
        public string NguonNguyenNhanAi { get; set; } = "Chưa xác định";
        public string ModelTreHan { get; set; } = "Không dùng";
        public string ModelNguyenNhan { get; set; } = "Fallback rule";
        public string NguyenNhanManagerXacNhan { get; set; } = "Chưa xác nhận";
        public string DoTinCayManagerXacNhan { get; set; } = "Chưa xác nhận";
        public bool HienThiThongBaoKhongTre { get; set; }
        public string ThongBaoKhongTre { get; set; } = "Dự án không trễ, không cần xác nhận nguyên nhân.";
        public bool KetQuaAiCoTheDaCu { get; set; }
        public string? CanhBaoDuLieuAi { get; set; }
        public string TrangThaiDuLieuAi { get; set; } = "Chưa có kết quả phân tích nguyên nhân trễ cho dự án này.";
        public bool CoThePhanTichAi { get; set; }
        public bool CanPhanTichAi { get; set; }
        public bool TuDongPhanTichAi { get; set; }
        public string? LyDoCanPhanTichAi { get; set; }
        public string NutPhanTichText { get; set; } = "Phân tích AI";
        public List<DanhGiaDuAnRelatedReasonViewModel>? DanhSachNguyenNhanLienQuan { get; set; }
    }
}
