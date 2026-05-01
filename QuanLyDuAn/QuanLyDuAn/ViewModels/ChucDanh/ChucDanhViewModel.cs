using System.ComponentModel.DataAnnotations;

namespace QuanLyDuAn.ViewModels.ChucDanh
{
    public class ChucDanhViewModel
    {
        public int MaChucDanh { get; set; }
        public string TenChucDanh { get; set; } = string.Empty;
        public string? MoTaChucDanh { get; set; }
    }
}