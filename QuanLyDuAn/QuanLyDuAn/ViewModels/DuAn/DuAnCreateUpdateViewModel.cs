using System.ComponentModel.DataAnnotations;
using QuanLyDuAn.Constants;

namespace QuanLyDuAn.ViewModels.DuAn
{
    public class DuAnCreateUpdateViewModel : IValidatableObject
    {
        public int? MaDuAn { get; set; }
        public string? TenNguoiQuanLy { get; set; }

        [Required(ErrorMessage = "Tên dự án không được để trống")]
        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string TenDuAn { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Tối đa 255 ký tự")]
        public string? MoTaDuAn { get; set; }

        public int? MaNguoiDung { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại dự án")]
        public int? MaLoaiDuAn { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        public DateTime? NgayBatDauDuAn { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        public DateTime? NgayKetThucDuAn { get; set; }

        [MaxLength(50, ErrorMessage = "Tối đa 50 ký tự")]
        public string TrangThaiDuAn { get; set; } = TrangThai.KhoiTao;

        [MaxLength(500, ErrorMessage = "Tối đa 500 ký tự")]
        public string? GhiChuDuAn { get; set; }

        // Status check properties for UI
        public ProjectStatusCheckViewModel? StatusCheck { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate end date >= start date
            if (NgayBatDauDuAn.HasValue && NgayKetThucDuAn.HasValue && NgayKetThucDuAn < NgayBatDauDuAn)
            {
                yield return new ValidationResult(
                    "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.",
                    new[] { nameof(NgayKetThucDuAn) });
            }

            // Validate start date >= today (only on create)
            if (!MaDuAn.HasValue && NgayBatDauDuAn.HasValue)
            {
                if (NgayBatDauDuAn.Value.Date < DateTime.Today)
                {
                    yield return new ValidationResult(
                        "Ngày bắt đầu không được nhỏ hơn ngày hôm nay.",
                        new[] { nameof(NgayBatDauDuAn) });
                }
            }

            // Validate GhiChuDuAn required when status is TamDung
            if (!string.IsNullOrWhiteSpace(TrangThaiDuAn) && 
                TrangThai.EqualsValue(TrangThaiDuAn, TrangThai.TamDung) &&
                string.IsNullOrWhiteSpace(GhiChuDuAn))
            {
                yield return new ValidationResult(
                    "Ghi chú lý do tạm dừng không được để trống.",
                    new[] { nameof(GhiChuDuAn) });
            }
        }
    }
}
