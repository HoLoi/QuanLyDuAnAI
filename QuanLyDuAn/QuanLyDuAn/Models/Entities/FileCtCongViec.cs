namespace QuanLyDuAn.Models.Entities
{
    public class FileCtCongViec
    {
        public int MaFileCTCV { get; set; }
        public int MaChiTietCV { get; set; }
        public string? TenFileCTCV { get; set; }
        public string? DuongDanFileCTCV { get; set; }
        public DateTime? NgayUploadFileCTCV { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int? DeletedBy { get; set; }
    }
}
