using Microsoft.AspNetCore.Identity;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Data
{
    public class ApplicationUser : IdentityUser
    {
        public NhanVien? NhanVien { get; set; }
    }
}
