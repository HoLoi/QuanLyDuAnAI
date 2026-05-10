using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.ChatDuAn
{
    public class ChatDuAnGuiTinNhanViewModel
    {
        [Required]
        public int MaPhongChat { get; set; }

        public int MaDuAn { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung tin nhắn.")]
        [MaxLength(2000, ErrorMessage = "Tin nhắn tối đa 2000 ký tự.")]
        public string NoiDungTinNhan { get; set; } = string.Empty;
    }
}
