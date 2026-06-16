using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Implementations;

namespace QuanLyDuAn.Data;

public static class KhoiTaoTaiKhoanMacDinh
{
    public static async Task DamBaoDuLieuAsync(QuanLyDuAnDbContext dbContext)
    {
        await DamBaoDanhMucManHinhVaChucNangAsync(dbContext);
        await DamBaoDanhMucTinhBatBuocAsync(dbContext);
        await DamBaoTaiKhoanAdminMacDinhAsync(dbContext);
    }

    private static async Task<Aspnetroles> DamBaoRoleAsync(QuanLyDuAnDbContext dbContext, string tenVaiTro)
    {
        var role = await dbContext.Aspnetroles.FirstOrDefaultAsync(x => x.NormalizedName == tenVaiTro.ToUpper());
        if (role is not null)
        {
            return role;
        }

        role = new Aspnetroles
        {
            Id = Guid.NewGuid().ToString("N"),
            Name = tenVaiTro,
            NormalizedName = tenVaiTro.ToUpper(),
            ConcurrencyStamp = Guid.NewGuid().ToString("N")
        };

        dbContext.Aspnetroles.Add(role);
        await dbContext.SaveChangesAsync();
        return role;
    }

    private static async Task DamBaoTaiKhoanAdminMacDinhAsync(QuanLyDuAnDbContext dbContext)
    {
        var roleAdmin = await DamBaoRoleAsync(dbContext, "Admin");
        var roleManager = await DamBaoRoleAsync(dbContext, "Manager");
        var roleEmployee = await DamBaoRoleAsync(dbContext, "Employee");
        var permissionProvider = new PermissionDependencyProvider();

        var taiKhoanAdmin = await dbContext.Aspnetusers
            .FirstOrDefaultAsync(x => x.NormalizedUserName == "ADMIN");

        if (taiKhoanAdmin is null)
        {
            var passwordHasher = new PasswordHasher<Aspnetusers>();
            await TaoNguoiDungMacDinhAsync(dbContext, passwordHasher, "admin", "Quản trị hệ thống", roleAdmin.Id);
            taiKhoanAdmin = await dbContext.Aspnetusers.FirstAsync(x => x.NormalizedUserName == "ADMIN");
        }

        var daGanRoleAdmin = await dbContext.Aspnetuserroles
            .AnyAsync(x => x.Asp_Id == taiKhoanAdmin.Id && x.Id == roleAdmin.Id);

        if (!daGanRoleAdmin)
        {
            dbContext.Aspnetuserroles.Add(new Aspnetuserroles
            {
                Asp_Id = taiKhoanAdmin.Id,
                Id = roleAdmin.Id
            });
            await dbContext.SaveChangesAsync();
        }

        //await GanQuyenMacDinhChoRoleAdminAsync(dbContext, roleAdmin.Id);
        await DamBaoRoleClaimToiThieuAsync(dbContext, roleAdmin.Id, new[]
        {
            // ===== HỆ THỐNG =====
            Permissions.NhanSu.Xem,
            Permissions.NhanSu.Them,
            Permissions.NhanSu.Sua,
            Permissions.NhanSu.Xoa,
            Permissions.NhanSu.Khoa,
            Permissions.NhanSu.MoKhoa,

            Permissions.ChucDanh.Xem,
            Permissions.ChucDanh.Them,
            Permissions.ChucDanh.Sua,
            Permissions.ChucDanh.Xoa,

            Permissions.PhanQuyen.Xem,
            Permissions.PhanQuyen.Luu,

            Permissions.Nhom.Xem,
            Permissions.Nhom.Them,
            Permissions.Nhom.Sua,
            Permissions.Nhom.Xoa,

            Permissions.ThanhVienNhom.Xem,
            Permissions.ThanhVienNhom.Them,
            Permissions.ThanhVienNhom.Xoa,

            Permissions.DuyetYeuCauDoiQuanLy.Xem,
            Permissions.DuyetYeuCauDoiQuanLy.Duyet,

            // ===== LOG / THEO DÕI =====
            Permissions.NhatKy.Xem,

            // ===== THỐNG KÊ =====
            Permissions.ThongKe.Xem,
            Permissions.ThongKe.XuatFile,

            // ===== CHAT (tùy chọn) =====
            Permissions.Chat.Xem,
            Permissions.DanhGiaDuAn.Xem,
            Permissions.DanhGiaDuAn.Duyet,
            Permissions.DanhGiaNhanVien.Xem,

            // AI 
            Permissions.AI.Xem,
            Permissions.AI.Dataset,
            Permissions.AI.Train,
            Permissions.AI.Dashboard
            
        });
        await DamBaoRoleClaimToiThieuAsync(dbContext, roleManager.Id, new[]
        {
            // Dự án
            Permissions.DuAn.Xem,
            Permissions.DuAn.Them,
            Permissions.DuAn.Sua,
            Permissions.DuAn.Xoa,

            // Team dự án
            Permissions.TeamDuAn.Xem,
            Permissions.TeamDuAn.Them,
            Permissions.TeamDuAn.Xoa,

            // Thành viên dự án
            Permissions.ThanhVienDuAn.Xem,
            Permissions.ThanhVienDuAn.Them,
            Permissions.ThanhVienDuAn.Xoa,

            // Danh mục công việc 
            Permissions.DanhMucCongViec.Xem,
            Permissions.DanhMucCongViec.Them,
            Permissions.DanhMucCongViec.Sua,
            Permissions.DanhMucCongViec.Xoa,

            // Duyệt đề xuất công việc
            Permissions.DuyetDeXuatCongViec.Xem,
            Permissions.DuyetDeXuatCongViec.Duyet,

            // Công việc
            Permissions.CongViec.Xem,

            // Chi tiết công việc
            Permissions.ChiTietCongViec.Xem,

            // Phân công
            Permissions.PhanCongCongViec.Xem,
            Permissions.PhanCongCongViec.ThucHien,

            // Phân công chi tiết công việc
            Permissions.PhanCongChiTietCongViec.Xem,
            Permissions.PhanCongChiTietCongViec.ThucHien,

            // Tiến độ
            Permissions.TienDo.Xem,
            Permissions.TienDo.Duyet,

            // Duyệt đề xuất ngân sách
            Permissions.DuyetNganSach.Xem,
            Permissions.DuyetNganSach.Duyet,

            // Ngân sách
            Permissions.NganSach.Xem,

            // Chi phí
            Permissions.ChiPhi.Xem,
            Permissions.ChiPhi.Them,
            Permissions.ChiPhi.Sua,

            // Chat
            Permissions.Chat.Xem,
            Permissions.Chat.Gui,

            // Thống kê
            Permissions.ThongKe.Xem,
            Permissions.ThongKe.XuatFile,

            // Đánh giá
            Permissions.DanhGiaDuAn.Xem,
            Permissions.DanhGiaDuAn.DanhGia,
            Permissions.DanhGiaDuAn.Sua,
            Permissions.DanhGiaNhanVien.Duyet,
            Permissions.DanhGiaNhanVien.Xem,
            Permissions.DanhGiaNhanVien.DanhGia,
            Permissions.DanhGiaNhanVien.Sua,

            // AI
            Permissions.AI.Xem,
            Permissions.AI.PhanTichNguyenNhan,
            Permissions.AI.Dashboard,
            Permissions.AI.XacNhan,

            // Yêu cầu đổi quản lý
            Permissions.YeuCauDoiQuanLy.Xem,
            Permissions.YeuCauDoiQuanLy.Them
        });

        await DamBaoRoleClaimToiThieuAsync(dbContext, roleEmployee.Id, new[]
        {
            // Dự án 
            Permissions.DuAn.Xem,

            // Công việc
            Permissions.CongViec.Xem,

            // Đề xuất ngân sách
            Permissions.DeXuatNganSach.Xem,
            Permissions.DeXuatNganSach.Them,

            // Đề xuất công việc
            Permissions.DeXuatCongViec.Xem,
            Permissions.DeXuatCongViec.Them,

            // Phân công công việc
            Permissions.PhanCongCongViec.Xem,
            Permissions.PhanCongCongViec.ThucHien,

            // Chi tiết công việc
            Permissions.ChiTietCongViec.Xem,
            Permissions.ChiTietCongViec.Them,
            Permissions.ChiTietCongViec.Sua,
            Permissions.ChiTietCongViec.Xoa,

            // Phân công chi tiết công việc
            Permissions.PhanCongChiTietCongViec.Xem,
            Permissions.PhanCongChiTietCongViec.ThucHien,

            // Tiến độ
            Permissions.TienDo.Xem,
            Permissions.TienDo.CapNhat,

            // Chat
            Permissions.Chat.Xem,
            Permissions.Chat.Gui,

            // Xem thống kê đơn giản
            Permissions.ThongKe.Xem,
            Permissions.ThongKe.XuatFile,

            // Xem đánh giá
            Permissions.DanhGiaNhanVien.Xem,
            Permissions.DanhGiaDuAn.Xem
        });

        await DamBaoRoleKhongCoQuyenAsync(dbContext, roleAdmin.Id, permissionProvider.GetDeniedPermissionsForRole("Admin"));
        await DamBaoRoleKhongCoQuyenAsync(dbContext, roleManager.Id, permissionProvider.GetDeniedPermissionsForRole("Manager"));
        await DamBaoRoleKhongCoQuyenAsync(dbContext, roleEmployee.Id, permissionProvider.GetDeniedPermissionsForRole("Employee"));
    }

    //private static async Task GanQuyenMacDinhChoRoleAdminAsync(QuanLyDuAnDbContext dbContext, string roleId)
    //{
    //    var tatCaQuyen = await dbContext.DanhMucQuyen
    //        .Select(x => new { x.MaDanhMucQuyen, x.TenDanhMucQuyen })
    //        .ToListAsync();

    //    if (tatCaQuyen.Count == 0)
    //    {
    //        return;
    //    }

    //    var daCo = await dbContext.Aspnetroleclaims
    //        .Where(x => x.Asp_Id == roleId)
    //        .Select(x => x.MaDanhMucQuyen)
    //        .ToListAsync();

    //    foreach (var quyen in tatCaQuyen.Where(x => !daCo.Contains(x.MaDanhMucQuyen)))
    //    {
    //        dbContext.Aspnetroleclaims.Add(new Aspnetroleclaims
    //        {
    //            Asp_Id = roleId,
    //            MaDanhMucQuyen = quyen.MaDanhMucQuyen,
    //            ClaimType = "permission",
    //            ClaimValue = quyen.TenDanhMucQuyen
    //        });
    //    }

    //    await dbContext.SaveChangesAsync();
    //}

    private static async Task DamBaoRoleClaimToiThieuAsync(QuanLyDuAnDbContext dbContext, string roleId, IEnumerable<string> danhSachTenQuyen)
    {
        var dsQuyenHopLe = await dbContext.DanhMucQuyen
            .AsNoTracking()
            .Where(x => x.TenDanhMucQuyen != null && danhSachTenQuyen.Contains(x.TenDanhMucQuyen))
            .Select(x => new { x.MaDanhMucQuyen, x.TenDanhMucQuyen })
            .ToListAsync();

        if (dsQuyenHopLe.Count == 0)
        {
            return;
        }

        var dsQuyenDaCo = await dbContext.Aspnetroleclaims
            .AsNoTracking()
            .Where(x => x.Asp_Id == roleId)
            .Select(x => x.MaDanhMucQuyen)
            .ToListAsync();

        foreach (var quyen in dsQuyenHopLe.Where(x => !dsQuyenDaCo.Contains(x.MaDanhMucQuyen)))
        {
            dbContext.Aspnetroleclaims.Add(new Aspnetroleclaims
            {
                Asp_Id = roleId,
                MaDanhMucQuyen = quyen.MaDanhMucQuyen,
                ClaimType = Permissions.ClaimTypesCustom.Permission,
                ClaimValue = quyen.TenDanhMucQuyen
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task DamBaoRoleKhongCoQuyenAsync(QuanLyDuAnDbContext dbContext, string roleId, IEnumerable<string> danhSachTenQuyen)
    {
        var tenQuyenBiChan = danhSachTenQuyen
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        if (tenQuyenBiChan.Count == 0)
        {
            return;
        }

        var maQuyenBiChan = await dbContext.DanhMucQuyen
            .AsNoTracking()
            .Where(x => x.TenDanhMucQuyen != null && tenQuyenBiChan.Contains(x.TenDanhMucQuyen))
            .Select(x => x.MaDanhMucQuyen)
            .ToListAsync();
        if (maQuyenBiChan.Count == 0)
        {
            return;
        }

        var claimsBiChan = await dbContext.Aspnetroleclaims
            .Where(x => x.Asp_Id == roleId && maQuyenBiChan.Contains(x.MaDanhMucQuyen))
            .ToListAsync();
        if (claimsBiChan.Count == 0)
        {
            return;
        }

        dbContext.Aspnetroleclaims.RemoveRange(claimsBiChan);
        await dbContext.SaveChangesAsync();
    }

    private static async Task DamBaoDanhMucTinhBatBuocAsync(QuanLyDuAnDbContext dbContext)
    {
        await DamBaoChucDanhMacDinhAsync(dbContext);
        await DamBaoLoaiDuAnMacDinhAsync(dbContext);
        await DamBaoMucDoUuTienMacDinhAsync(dbContext);
        await DamBaoTieuChiDanhGiaMacDinhAsync(dbContext);
        await DamBaoDmNguyenNhanMacDinhAsync(dbContext);
    }

    private static async Task DamBaoChucDanhMacDinhAsync(QuanLyDuAnDbContext dbContext)
    {
        var mau = new[]
         {
            (ten: "Quản trị viên", moTa: "Quản trị hệ thống và phân quyền"),
            (ten: "Quản lý dự án", moTa: "Điều phối và duyệt nghiệp vụ dự án"),
            (ten: "Developer", moTa: "Lập trình viên tham gia phát triển"),
            (ten: "Tester", moTa: "Kiểm thử và đảm bảo chất lượng"),
            (ten: "Business Analyst", moTa: "Phân tích nghiệp vụ")
        };

        var mappingChuanHoa = new Dictionary<string, (string ten, string moTa)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Quan tri vien"] = ("Quản trị viên", "Quản trị hệ thống và phân quyền"),
            ["Quan ly du an"] = ("Quản lý dự án", "Điều phối và duyệt nghiệp vụ dự án"),
            ["Developer"] = ("Developer", "Lập trình viên tham gia phát triển"),
            ["Tester"] = ("Tester", "Kiểm thử và đảm bảo chất lượng"),
            ["Business Analyst"] = ("Business Analyst", "Phân tích nghiệp vụ")
        };

        var rows = await dbContext.ChucDanh
            .Where(x => x.TenChucDanh != null)
            .ToListAsync();

        foreach (var row in rows)
        {
            var tenGoc = row.TenChucDanh?.Trim();
            if (tenGoc != null && mappingChuanHoa.TryGetValue(tenGoc, out var tenChuan))
            {
                var rowChuan = rows.FirstOrDefault(x =>
                    x.MaChucDanh != row.MaChucDanh
                    && dbContext.Entry(x).State != EntityState.Deleted
                    && string.Equals(x.TenChucDanh?.Trim(), tenChuan.ten, StringComparison.OrdinalIgnoreCase));

                if (rowChuan is null)
                {
                    row.TenChucDanh = tenChuan.ten;
                    row.MoTaChucDanh = tenChuan.moTa;
                    continue;
                }

                rowChuan.MoTaChucDanh = tenChuan.moTa;
                await ChuyenThamChieuChucDanhAsync(dbContext, row.MaChucDanh, rowChuan.MaChucDanh);
                await dbContext.SaveChangesAsync();
                dbContext.ChucDanh.Remove(row);
            }
        }

        var hienCo = rows
            .Where(x => dbContext.Entry(x).State != EntityState.Deleted
                && !string.IsNullOrWhiteSpace(x.TenChucDanh))
            .Select(x => x.TenChucDanh!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var item in mau)
        {
            if (hienCo.Contains(item.ten))
            {
                continue;
            }

            dbContext.ChucDanh.Add(new ChucDanh
            {
                TenChucDanh = item.ten,
                MoTaChucDanh = item.moTa
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task DamBaoLoaiDuAnMacDinhAsync(QuanLyDuAnDbContext dbContext)
    {
        var mau = new[]
        {
            (ten: "Phát triển phần mềm", moTa: "Dự án xây dựng hệ thống phần mềm"),
            (ten: "Bảo trì, nâng cấp", moTa: "Dự án bảo trì và nâng cấp hệ thống"),
            (ten: "Nghiên cứu AI", moTa: "Dự án tập trung nghiên cứu về AI")
        };

        var mappingChuanHoa = new Dictionary<string, (string ten, string moTa)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Phat trien phan mem"] = ("Phát triển phần mềm", "Dự án xây dựng hệ thống phần mềm"),
            ["Bao tri nang cap"] = ("Bảo trì, nâng cấp", "Dự án bảo trì và nâng cấp hệ thống"),
            ["Nghien cuu AI"] = ("Nghiên cứu AI", "Dự án tập trung nghiên cứu về AI")
        };

        var rows = await dbContext.LoaiDuAn
            .Where(x => x.TenLoai != null)
            .ToListAsync();

        foreach (var row in rows)
        {
            var tenGoc = row.TenLoai?.Trim();
            if (tenGoc != null && mappingChuanHoa.TryGetValue(tenGoc, out var tenChuan))
            {
                var rowChuan = rows.FirstOrDefault(x =>
                    x.MaLoaiDuAn != row.MaLoaiDuAn
                    && dbContext.Entry(x).State != EntityState.Deleted
                    && string.Equals(x.TenLoai?.Trim(), tenChuan.ten, StringComparison.OrdinalIgnoreCase));

                if (rowChuan is null)
                {
                    row.TenLoai = tenChuan.ten;
                    row.MoTaLoaiDuAn = tenChuan.moTa;
                    continue;
                }

                rowChuan.MoTaLoaiDuAn = tenChuan.moTa;
                await ChuyenThamChieuLoaiDuAnAsync(dbContext, row.MaLoaiDuAn, rowChuan.MaLoaiDuAn);
                await dbContext.SaveChangesAsync();
                dbContext.LoaiDuAn.Remove(row);
            }
        }

        var hienCo = rows
            .Where(x => dbContext.Entry(x).State != EntityState.Deleted
                && !string.IsNullOrWhiteSpace(x.TenLoai))
            .Select(x => x.TenLoai!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var item in mau)
        {
            if (hienCo.Contains(item.ten))
            {
                continue;
            }

            dbContext.LoaiDuAn.Add(new LoaiDuAn
            {
                TenLoai = item.ten,
                MoTaLoaiDuAn = item.moTa
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task DamBaoMucDoUuTienMacDinhAsync(QuanLyDuAnDbContext dbContext)
    {
        var mau = new[]
        {
            (ten: "Thấp", moTa: "Mức ưu tiên thấp"),
            (ten: "Trung bình", moTa: "Mức ưu tiên trung bình"),
            (ten: "Cao", moTa: "Mức ưu tiên cao"),
            (ten: "Khẩn cấp", moTa: "Xử lý ngay lập tức")
        };

        var mappingChuanHoa = new Dictionary<string, (string ten, string moTa)>(StringComparer.OrdinalIgnoreCase)
        {
            ["Thap"] = ("Thấp", "Mức ưu tiên thấp"),
            ["Trung binh"] = ("Trung bình", "Mức ưu tiên trung bình"),
            ["Cao"] = ("Cao", "Mức ưu tiên cao"),
            ["Khan cap"] = ("Khẩn cấp", "Xử lý ngay lập tức")
        };

        var rows = await dbContext.MucDoUuTien
            .Where(x => x.TenMucDo != null)
            .ToListAsync();

        foreach (var row in rows)
        {
            var tenGoc = row.TenMucDo?.Trim();
            if (tenGoc != null && mappingChuanHoa.TryGetValue(tenGoc, out var tenChuan))
            {
                var rowChuan = rows.FirstOrDefault(x =>
                    x.MaMucDo != row.MaMucDo
                    && dbContext.Entry(x).State != EntityState.Deleted
                    && string.Equals(x.TenMucDo?.Trim(), tenChuan.ten, StringComparison.OrdinalIgnoreCase));

                if (rowChuan is null)
                {
                    row.TenMucDo = tenChuan.ten;
                    row.MoTaMucDo = tenChuan.moTa;
                    continue;
                }

                rowChuan.MoTaMucDo = tenChuan.moTa;
                await ChuyenThamChieuMucDoUuTienAsync(dbContext, row.MaMucDo, rowChuan.MaMucDo);
                await dbContext.SaveChangesAsync();
                dbContext.MucDoUuTien.Remove(row);
            }
        }

        var hienCo = rows
            .Where(x => dbContext.Entry(x).State != EntityState.Deleted
                && !string.IsNullOrWhiteSpace(x.TenMucDo))
            .Select(x => x.TenMucDo!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var item in mau)
        {
            if (hienCo.Contains(item.ten))
            {
                continue;
            }

            dbContext.MucDoUuTien.Add(new MucDoUuTien
            {
                TenMucDo = item.ten,
                MoTaMucDo = item.moTa
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task DamBaoTieuChiDanhGiaMacDinhAsync(QuanLyDuAnDbContext dbContext)
    {
        var mau = new[]
        {
            (ten: "Tiến độ", diem: 10d, moTa: "Đánh giá theo mức độ đáp ứng tiến độ", loai: "NhanVien"),
            (ten: "Chất lượng công việc", diem: 10d, moTa: "Đánh giá chất lượng đầu ra", loai: "NhanVien"),
            (ten: "Trách nhiệm và chủ động", diem: 10d, moTa: "Đánh giá tinh thần trách nhiệm và chủ động", loai: "NhanVien"),
            (ten: "Phối hợp", diem: 10d, moTa: "Đánh giá khả năng phối hợp trong dự án", loai: "NhanVien"),
            (ten: "Báo cáo và minh chứng", diem: 10d, moTa: "Đánh giá chất lượng báo cáo tiến độ", loai: "NhanVien"),
            (ten: "Tiến độ", diem: 10d, moTa: "Đánh giá tiến độ tổng thể dự án", loai: "DuAn"),
            (ten: "Chất lượng", diem: 10d, moTa: "Đánh giá chất lượng tổng thể dự án", loai: "DuAn"),
            (ten: "Chi phí ngân sách", diem: 10d, moTa: "Đánh giá khả năng kiểm soát chi phí", loai: "DuAn"),
            (ten: "Phối hợp", diem: 10d, moTa: "Đánh giá sự phối hợp giữa các bên", loai: "DuAn"),
            (ten: "Hiệu quả tổng thể", diem: 10d, moTa: "Đánh giá hiệu quả đầu ra của dự án", loai: "DuAn"),
            (ten: "Tiến độ", diem: 10d, moTa: "Đánh giá tiến độ tổng thể dự án", loai: "DanhGiaDuAn"),
            (ten: "Chất lượng", diem: 10d, moTa: "Đánh giá chất lượng tổng thể dự án", loai: "DanhGiaDuAn"),
            (ten: "Chi phí ngân sách", diem: 10d, moTa: "Đánh giá khả năng kiểm soát chi phí", loai: "DanhGiaDuAn"),
            (ten: "Phối hợp", diem: 10d, moTa: "Đánh giá sự phối hợp giữa các bên", loai: "DanhGiaDuAn"),
            (ten: "Hiệu quả tổng thể", diem: 10d, moTa: "Đánh giá hiệu quả đầu ra của dự án", loai: "DanhGiaDuAn"),
            (ten: "Tiến độ", diem: 10d, moTa: "Đánh giá theo mức độ đáp ứng tiến độ", loai: "DanhGiaNhanVien"),
            (ten: "Chất lượng công việc", diem: 10d, moTa: "Đánh giá chất lượng đầu ra", loai: "DanhGiaNhanVien"),
            (ten: "Trách nhiệm và chủ động", diem: 10d, moTa: "Đánh giá tinh thần trách nhiệm và chủ động", loai: "DanhGiaNhanVien"),
            (ten: "Phối hợp", diem: 10d, moTa: "Đánh giá khả năng phối hợp trong dự án", loai: "DanhGiaNhanVien"),
            (ten: "Báo cáo và minh chứng", diem: 10d, moTa: "Đánh giá chất lượng báo cáo tiến độ", loai: "DanhGiaNhanVien")
        };

        var mappingChuanHoa = new Dictionary<(string ten, string loai), (string ten, string moTa)>(new TieuChiDanhGiaComparer())
        {
            [("Tien do", "NhanVien")] = ("Tiến độ", "Đánh giá theo mức độ đáp ứng tiến độ"),
            [("Chat luong cong viec", "NhanVien")] = ("Chất lượng công việc", "Đánh giá chất lượng đầu ra"),
            [("Trach nhiem va chu dong", "NhanVien")] = ("Trách nhiệm và chủ động", "Đánh giá tinh thần trách nhiệm và chủ động"),
            [("Phoi hop", "NhanVien")] = ("Phối hợp", "Đánh giá khả năng phối hợp trong dự án"),
            [("Bao cao va minh chung", "NhanVien")] = ("Báo cáo và minh chứng", "Đánh giá chất lượng báo cáo tiến độ"),
            [("Tien do", "DuAn")] = ("Tiến độ", "Đánh giá tiến độ tổng thể dự án"),
            [("Chat luong", "DuAn")] = ("Chất lượng", "Đánh giá chất lượng tổng thể dự án"),
            [("Chi phi ngan sach", "DuAn")] = ("Chi phí ngân sách", "Đánh giá khả năng kiểm soát chi phí"),
            [("Phoi hop", "DuAn")] = ("Phối hợp", "Đánh giá sự phối hợp giữa các bên"),
            [("Hieu qua tong the", "DuAn")] = ("Hiệu quả tổng thể", "Đánh giá hiệu quả đầu ra của dự án"),
            [("Tien do", "DanhGiaDuAn")] = ("Tiến độ", "Đánh giá tiến độ tổng thể dự án"),
            [("Chat luong", "DanhGiaDuAn")] = ("Chất lượng", "Đánh giá chất lượng tổng thể dự án"),
            [("Chi phi ngan sach", "DanhGiaDuAn")] = ("Chi phí ngân sách", "Đánh giá khả năng kiểm soát chi phí"),
            [("Phoi hop", "DanhGiaDuAn")] = ("Phối hợp", "Đánh giá sự phối hợp giữa các bên"),
            [("Hieu qua tong the", "DanhGiaDuAn")] = ("Hiệu quả tổng thể", "Đánh giá hiệu quả đầu ra của dự án"),
            [("Tien do", "DanhGiaNhanVien")] = ("Tiến độ", "Đánh giá theo mức độ đáp ứng tiến độ"),
            [("Chat luong cong viec", "DanhGiaNhanVien")] = ("Chất lượng công việc", "Đánh giá chất lượng đầu ra"),
            [("Trach nhiem va chu dong", "DanhGiaNhanVien")] = ("Trách nhiệm và chủ động", "Đánh giá tinh thần trách nhiệm và chủ động"),
            [("Phoi hop", "DanhGiaNhanVien")] = ("Phối hợp", "Đánh giá khả năng phối hợp trong dự án"),
            [("Bao cao va minh chung", "DanhGiaNhanVien")] = ("Báo cáo và minh chứng", "Đánh giá chất lượng báo cáo tiến độ")
        };

        var rows = await dbContext.TieuChiDanhGia
            .Where(x => x.TenTieuChi != null && x.LoaiTieuChi != null)
            .ToListAsync();

        foreach (var row in rows)
        {
            var key = (row.TenTieuChi!.Trim(), row.LoaiTieuChi!.Trim());
            if (mappingChuanHoa.TryGetValue(key, out var tieuChiChuan))
            {
                var rowChuan = rows.FirstOrDefault(x =>
                    x.MaTieuChi != row.MaTieuChi
                    && dbContext.Entry(x).State != EntityState.Deleted
                    && string.Equals(x.TenTieuChi?.Trim(), tieuChiChuan.ten, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.LoaiTieuChi?.Trim(), row.LoaiTieuChi?.Trim(), StringComparison.OrdinalIgnoreCase));

                if (rowChuan is null)
                {
                    row.TenTieuChi = tieuChiChuan.ten;
                    row.MoTa = tieuChiChuan.moTa;
                    continue;
                }

                rowChuan.MoTa = tieuChiChuan.moTa;
                await ChuyenThamChieuTieuChiDanhGiaAsync(dbContext, row.MaTieuChi, rowChuan.MaTieuChi);
                await dbContext.SaveChangesAsync();
                dbContext.TieuChiDanhGia.Remove(row);
            }
        }

        var hienCo = rows
            .Where(x => dbContext.Entry(x).State != EntityState.Deleted
                && !string.IsNullOrWhiteSpace(x.TenTieuChi)
                && !string.IsNullOrWhiteSpace(x.LoaiTieuChi))
            .Select(x => (ten: x.TenTieuChi!.Trim(), loai: x.LoaiTieuChi!.Trim()))
            .ToHashSet(new TieuChiDanhGiaComparer());

        foreach (var item in mau)
        {
            if (hienCo.Contains((item.ten, item.loai)))
            {
                continue;
            }

            dbContext.TieuChiDanhGia.Add(new TieuChiDanhGia
            {
                TenTieuChi = item.ten,
                DiemTieuChi = item.diem,
                MoTa = item.moTa,
                LoaiTieuChi = item.loai,
                TrangThaiTieuChi = TrangThai.DangSuDung
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task DamBaoDmNguyenNhanMacDinhAsync(QuanLyDuAnDbContext dbContext)
    {
        var mappingChuanHoa = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Thieu nhan su"] = "Thiếu nhân sự",
            ["Thay doi yeu cau lien tuc"] = "Thay đổi yêu cầu liên tục",
            ["Cham phe duyet"] = "Quy trình xử lý chậm",
            ["Vuot ngan sach"] = "Vượt ngân sách",
            ["Rui ro ky thuat"] = "Rủi ro kỹ thuật",
            ["Cong viec phu thuoc bi cham"] = "Phối hợp công việc chưa tốt",
            ["Thieu du lieu hoac tai lieu"] = "Thông tin đầu vào chưa đầy đủ",
            ["Uoc luong thoi gian chua chinh xac"] = "Ước lượng thời gian chưa chính xác",
            ["Tien do cap nhat khong day du"] = "Tiến độ cập nhật không đầy đủ",
            ["Chậm phê duyệt"] = "Quy trình xử lý chậm",
            ["Công việc phụ thuộc bị chậm"] = "Phối hợp công việc chưa tốt",
            ["Thiếu dữ liệu hoặc tài liệu"] = "Thông tin đầu vào chưa đầy đủ",
            ["Khac"] = "Khác"
        };

        var danhMucBatBuoc = new[]
        {
            "Thiếu nhân sự",
            "Thay đổi yêu cầu liên tục",
            "Quy trình xử lý chậm",
            "Vượt ngân sách",
            "Rủi ro kỹ thuật",
            "Phối hợp công việc chưa tốt",
            "Thông tin đầu vào chưa đầy đủ",
            "Ước lượng thời gian chưa chính xác",
            "Tiến độ cập nhật không đầy đủ",
            "Khác"
        };

        var rows = await dbContext.DmNguyenNhan.ToListAsync();
        foreach (var row in rows)
        {
            if (string.IsNullOrWhiteSpace(row.TenNguyenNhan))
            {
                continue;
            }

            var tenGoc = row.TenNguyenNhan.Trim();
            if (mappingChuanHoa.TryGetValue(tenGoc, out var tenChuan))
            {
                var rowChuan = rows.FirstOrDefault(x =>
                    x.MaDMNguyenNhan != row.MaDMNguyenNhan
                    && dbContext.Entry(x).State != EntityState.Deleted
                    && string.Equals(x.TenNguyenNhan?.Trim(), tenChuan, StringComparison.OrdinalIgnoreCase));

                if (rowChuan is null)
                {
                    row.TenNguyenNhan = tenChuan;
                    continue;
                }

                await ChuyenThamChieuNguyenNhanAsync(dbContext, row.MaDMNguyenNhan, rowChuan.MaDMNguyenNhan);
                await dbContext.SaveChangesAsync();
                dbContext.DmNguyenNhan.Remove(row);
            }
        }

        var hienCo = rows
            .Where(x => dbContext.Entry(x).State != EntityState.Deleted
                && !string.IsNullOrWhiteSpace(x.TenNguyenNhan))
            .Select(x => x.TenNguyenNhan!.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var ten in danhMucBatBuoc)
        {
            if (hienCo.Contains(ten))
            {
                continue;
            }

            dbContext.DmNguyenNhan.Add(new DmNguyenNhan
            {
                TenNguyenNhan = ten
            });
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task DamBaoDanhMucManHinhVaChucNangAsync(QuanLyDuAnDbContext dbContext)
    {
        var permissionProvider = new PermissionDependencyProvider();
        var cauHinh = new Dictionary<string, string[]>
        {
            // ===== DASHBOARD =====
            ["Dashboard"] = new[]
            {
                Permissions.ThongKe.Xem,
                Permissions.ThongKe.XuatFile
            },

            // ===== HỆ THỐNG =====
            ["NhanSu"] = new[]
            {
                Permissions.NhanSu.Xem,
                Permissions.NhanSu.Them,
                Permissions.NhanSu.Sua,
                Permissions.NhanSu.Xoa,
                Permissions.NhanSu.Khoa,
                Permissions.NhanSu.MoKhoa
            },

            ["ChucDanh"] = new[]
            {
                Permissions.ChucDanh.Xem,
                Permissions.ChucDanh.Them,
                Permissions.ChucDanh.Sua,
                Permissions.ChucDanh.Xoa
            },

            ["PhanQuyen"] = new[]
            {
                Permissions.PhanQuyen.Xem,
                Permissions.PhanQuyen.Luu
            },

            ["Nhom"] = new[]
            {
                Permissions.Nhom.Xem,
                Permissions.Nhom.Them,
                Permissions.Nhom.Sua,
                Permissions.Nhom.Xoa
            },

            ["ThanhVienNhom"] = new[]
                {
                    Permissions.ThanhVienNhom.Xem,
                    Permissions.ThanhVienNhom.Them,
                    Permissions.ThanhVienNhom.Xoa
                },

            ["DuyetYeuCauDoiQuanLy"] = new[]
            {
                Permissions.DuyetYeuCauDoiQuanLy.Xem,
                Permissions.DuyetYeuCauDoiQuanLy.Duyet
            },

            // ===== DỰ ÁN =====
            ["DuAn"] = new[]
            {
                Permissions.DuAn.Xem,
                Permissions.DuAn.Them,
                Permissions.DuAn.Sua,
                Permissions.DuAn.Xoa
            },

            ["YeuCauDoiQuanLy"] = new[]
            {
                Permissions.YeuCauDoiQuanLy.Xem,
                Permissions.YeuCauDoiQuanLy.Them
            },

            ["TeamDuAn"] = new[]
            {
                Permissions.TeamDuAn.Xem,
                Permissions.TeamDuAn.Them,
                Permissions.TeamDuAn.Xoa
            },

            ["ThanhVienDuAn"] = new[]
            {
                Permissions.ThanhVienDuAn.Xem,
                Permissions.ThanhVienDuAn.Them,
                Permissions.ThanhVienDuAn.Xoa
            },

            // ===== CÔNG VIỆC =====
            ["DanhMucCongViec"] = new[]
            {
                Permissions.DanhMucCongViec.Xem,
                Permissions.DanhMucCongViec.Them,
                Permissions.DanhMucCongViec.Sua,
                Permissions.DanhMucCongViec.Xoa
            },

            ["DeXuatCongViec"] = new[]
            {
                Permissions.DeXuatCongViec.Xem,
                Permissions.DeXuatCongViec.Them
            },

            ["DuyetDeXuatCongViec"] = new[]
            {
                Permissions.DuyetDeXuatCongViec.Xem,
                Permissions.DuyetDeXuatCongViec.Duyet
            },

            ["CongViec"] = new[]
            {
                Permissions.CongViec.Xem
            },

            ["ChiTietCongViec"] = new[]
            {
                Permissions.ChiTietCongViec.Xem,
                Permissions.ChiTietCongViec.Them,
                Permissions.ChiTietCongViec.Sua,
                Permissions.ChiTietCongViec.Xoa
            },

            ["PhanCongCongViec"] = new[]
            {
                Permissions.PhanCongCongViec.Xem,
                Permissions.PhanCongCongViec.ThucHien
            },

            ["PhanCongChiTietCongViec"] = new[]
            {
                Permissions.PhanCongChiTietCongViec.Xem,
                Permissions.PhanCongChiTietCongViec.ThucHien
            },

            ["TienDo"] = new[]
            {
                Permissions.TienDo.Xem,
                Permissions.TienDo.CapNhat,
                Permissions.TienDo.Duyet
            },

            // ===== TÀI CHÍNH =====
            ["DeXuatNganSach"] = new[]
            {
                Permissions.DeXuatNganSach.Xem,
                Permissions.DeXuatNganSach.Them
            },

            ["DuyetNganSach"] = new[]
            {
                Permissions.DuyetNganSach.Xem,
                Permissions.DuyetNganSach.Duyet
            },

            ["NganSach"] = new[]
            {
                Permissions.NganSach.Xem
            },

            ["ChiPhi"] = new[]
            {
                Permissions.ChiPhi.Xem,
                Permissions.ChiPhi.Them,
                Permissions.ChiPhi.Sua
            },

            // ===== AI =====
            ["AI"] = new[]
            {
                Permissions.AI.Xem
            },

            ["AIDataset"] = new[]
            {
                Permissions.AI.Dataset
            },

            ["AITrain"] = new[]
            {
                Permissions.AI.Train
            },

            ["AIPredict"] = new[]
            {
                Permissions.AI.PhanTichNguyenNhan
            },

            ["AIXacNhan"] = new[]
            {
                Permissions.AI.XacNhan
            },

            ["AIDashboard"] = new[]
            {
                Permissions.AI.Dashboard
            },

            // ===== ĐÁNH GIÁ =====
            ["DanhGiaDuAn"] = new[]
            {
                Permissions.DanhGiaDuAn.Xem,
                Permissions.DanhGiaDuAn.DanhGia,
                Permissions.DanhGiaDuAn.Sua,
                Permissions.DanhGiaDuAn.Duyet
            },

            ["DanhGiaNhanVien"] = new[]
            {
                Permissions.DanhGiaNhanVien.Xem,
                Permissions.DanhGiaNhanVien.DanhGia,
                Permissions.DanhGiaNhanVien.Sua,
                Permissions.DanhGiaNhanVien.Duyet
            },

            // ===== KHÁC =====
            ["Chat"] = new[]
            {
                Permissions.Chat.Xem,
                Permissions.Chat.Gui
            },

            ["NhatKy"] = new[]
            {
                Permissions.NhatKy.Xem
            }
        };

        var manHinhHienCo = await dbContext.DanhMucManHinh.ToListAsync();
        var mapManHinh = manHinhHienCo
            .Where(x => !string.IsNullOrWhiteSpace(x.TenManHinh))
            .ToDictionary(x => x.TenManHinh!, x => x.MaManHinh, StringComparer.OrdinalIgnoreCase);

        foreach (var tenManHinh in cauHinh.Keys)
        {
            if (mapManHinh.ContainsKey(tenManHinh))
            {
                continue;
            }

            var manHinhMoi = new DanhMucManHinh
            {
                TenManHinh = tenManHinh,
                MoTaManHinh = $"Màn hình {permissionProvider.GetPermissionDefinition(cauHinh[tenManHinh].First(), tenManHinh).ScreenDisplayName}"
            };
            dbContext.DanhMucManHinh.Add(manHinhMoi);
            await dbContext.SaveChangesAsync();
            mapManHinh[tenManHinh] = manHinhMoi.MaManHinh;
        }

        var quyenHienCo = await dbContext.DanhMucQuyen
            .Where(x => !string.IsNullOrWhiteSpace(x.TenDanhMucQuyen))
            .Select(x => x.TenDanhMucQuyen!)
            .ToListAsync();

        foreach (var item in cauHinh)
        {
            var maManHinh = mapManHinh[item.Key];
            foreach (var tenQuyen in item.Value)
            {
                if (quyenHienCo.Contains(tenQuyen, StringComparer.OrdinalIgnoreCase))
                {
                    continue;
                }

                dbContext.DanhMucQuyen.Add(new DanhMucQuyen
                {
                    MaManHinh = maManHinh,
                    TenDanhMucQuyen = tenQuyen,
                    MoTaDanhMucQuyen = permissionProvider.GetPermissionDefinition(tenQuyen, item.Key).Description
                });
            }
        }

        await dbContext.SaveChangesAsync();

        var manHinhCanCapNhat = await dbContext.DanhMucManHinh.ToListAsync();
        foreach (var manHinh in manHinhCanCapNhat.Where(x => !string.IsNullOrWhiteSpace(x.TenManHinh)))
        {
            if (!cauHinh.TryGetValue(manHinh.TenManHinh!, out var permissions) || permissions.Length == 0)
            {
                continue;
            }

            var screenDisplayName = permissionProvider
                .GetPermissionDefinition(permissions[0], manHinh.TenManHinh)
                .ScreenDisplayName;
            manHinh.MoTaManHinh = $"Màn hình {screenDisplayName}";
        }

        var quyenCanCapNhat = await (from permission in dbContext.DanhMucQuyen
                                     join screen in dbContext.DanhMucManHinh
                                         on permission.MaManHinh equals screen.MaManHinh
                                     select new
                                     {
                                         Permission = permission,
                                         ScreenKey = screen.TenManHinh
                                     }).ToListAsync();

        foreach (var item in quyenCanCapNhat)
        {
            if (string.IsNullOrWhiteSpace(item.Permission.TenDanhMucQuyen))
            {
                continue;
            }

            item.Permission.MoTaDanhMucQuyen = permissionProvider
                .GetPermissionDefinition(item.Permission.TenDanhMucQuyen, item.ScreenKey)
                .Description;
        }

        await dbContext.SaveChangesAsync();
    }

    private static async Task ChuyenThamChieuChucDanhAsync(
        QuanLyDuAnDbContext dbContext,
        int maChucDanhCu,
        int maChucDanhChuan)
    {
        var nguoiDungCanChuyen = await dbContext.NguoiDung
            .Where(x => x.MaChucDanh == maChucDanhCu)
            .ToListAsync();

        foreach (var nguoiDung in nguoiDungCanChuyen)
        {
            nguoiDung.MaChucDanh = maChucDanhChuan;
        }
    }

    private static async Task ChuyenThamChieuLoaiDuAnAsync(
        QuanLyDuAnDbContext dbContext,
        int maLoaiDuAnCu,
        int maLoaiDuAnChuan)
    {
        var duAnCanChuyen = await dbContext.DuAn
            .Where(x => x.MaLoaiDuAn == maLoaiDuAnCu)
            .ToListAsync();

        foreach (var duAn in duAnCanChuyen)
        {
            duAn.MaLoaiDuAn = maLoaiDuAnChuan;
        }
    }

    private static async Task ChuyenThamChieuMucDoUuTienAsync(
        QuanLyDuAnDbContext dbContext,
        int maMucDoCu,
        int maMucDoChuan)
    {
        var congViecCanChuyen = await dbContext.CongViec
            .Where(x => x.MaMucDo == maMucDoCu)
            .ToListAsync();
        foreach (var congViec in congViecCanChuyen)
        {
            congViec.MaMucDo = maMucDoChuan;
        }

        var deXuatCongViecCanChuyen = await dbContext.DeXuatCongViec
            .Where(x => x.MaMucDo == maMucDoCu)
            .ToListAsync();
        foreach (var deXuatCongViec in deXuatCongViecCanChuyen)
        {
            deXuatCongViec.MaMucDo = maMucDoChuan;
        }
    }

    private static async Task ChuyenThamChieuTieuChiDanhGiaAsync(
        QuanLyDuAnDbContext dbContext,
        int maTieuChiCu,
        int maTieuChiChuan)
    {
        var chiTietDanhGiaDuAnCanChuyen = await dbContext.CtDanhGiaDuAn
            .Where(x => x.MaTieuChi == maTieuChiCu)
            .ToListAsync();
        foreach (var chiTietDanhGiaDuAn in chiTietDanhGiaDuAnCanChuyen)
        {
            chiTietDanhGiaDuAn.MaTieuChi = maTieuChiChuan;
        }

        var chiTietDanhGiaNhanVienCanChuyen = await dbContext.CtDanhGiaNhanVien
            .Where(x => x.MaTieuChi == maTieuChiCu)
            .ToListAsync();
        foreach (var chiTietDanhGiaNhanVien in chiTietDanhGiaNhanVienCanChuyen)
        {
            chiTietDanhGiaNhanVien.MaTieuChi = maTieuChiChuan;
        }
    }

    private static async Task ChuyenThamChieuNguyenNhanAsync(
        QuanLyDuAnDbContext dbContext,
        int maNguyenNhanCu,
        int maNguyenNhanChuan)
    {
        var datasetCanChuyen = await dbContext.AiDataset
            .Where(x => x.MaDMNguyenNhan == maNguyenNhanCu)
            .ToListAsync();
        foreach (var dataset in datasetCanChuyen)
        {
            dataset.MaDMNguyenNhan = maNguyenNhanChuan;
        }

        var ketQuaCanChuyen = await dbContext.AiKetQua
            .Where(x => x.MaDMNguyenNhan == maNguyenNhanCu)
            .ToListAsync();
        foreach (var ketQua in ketQuaCanChuyen)
        {
            ketQua.MaDMNguyenNhan = maNguyenNhanChuan;
        }

        var nguyenNhanCanChuyen = await dbContext.AiNguyenNhan
            .Where(x => x.MaDMNguyenNhan == maNguyenNhanCu)
            .ToListAsync();
        foreach (var nguyenNhan in nguyenNhanCanChuyen)
        {
            nguyenNhan.MaDMNguyenNhan = maNguyenNhanChuan;
        }
    }

    private sealed class TieuChiDanhGiaComparer : IEqualityComparer<(string ten, string loai)>
    {
        public bool Equals((string ten, string loai) x, (string ten, string loai) y)
        {
            return string.Equals(x.ten, y.ten, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.loai, y.loai, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode((string ten, string loai) obj)
        {
            return HashCode.Combine(
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.ten),
                StringComparer.OrdinalIgnoreCase.GetHashCode(obj.loai));
        }
    }

    private static async Task TaoNguoiDungMacDinhAsync(
        QuanLyDuAnDbContext dbContext,
        PasswordHasher<Aspnetusers> passwordHasher,
        string tenDangNhap,
        string hoTen,
        string roleId)
    {
        var chucDanhAdmin = await dbContext.ChucDanh
            .OrderBy(x => x.MaChucDanh)
            .FirstOrDefaultAsync(x => x.TenChucDanh == "Quản trị viên");

        if (chucDanhAdmin is null)
        {
            chucDanhAdmin = new ChucDanh
            {
                TenChucDanh = "Quản trị viên",
                MoTaChucDanh = "Chức danh mặc định cho tài khoản admin"
            };
            dbContext.ChucDanh.Add(chucDanhAdmin);
            await dbContext.SaveChangesAsync();
        }

        var userId = Guid.NewGuid().ToString("N");

        var nguoiDung = new NguoiDung
        {
            MaChucDanh = chucDanhAdmin.MaChucDanh,
            HoTenNguoiDung = hoTen,
            IsDeleted = false,
            Id = null!
        };

        dbContext.NguoiDung.Add(nguoiDung);
        await dbContext.SaveChangesAsync();

        var user = new Aspnetusers
        {
            Id = userId,
            MaNguoiDung = nguoiDung.MaNguoiDung,
            UserName = tenDangNhap,
            NormalizedUserName = tenDangNhap.ToUpperInvariant(),
            Email = $"{tenDangNhap}@local.app",
            NormalizedEmail = $"{tenDangNhap}@LOCAL.APP",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString("N"),
            ConcurrencyStamp = Guid.NewGuid().ToString("N"),
            PhoneNumberConfirmed = false,
            TwoFactorEnabled = false,
            LockoutEnabled = false,
            AccessFailedCount = 0
        };
        user.PasswordHash = passwordHasher.HashPassword(user, "111111");

        dbContext.Aspnetusers.Add(user);

        dbContext.Aspnetuserroles.Add(new Aspnetuserroles
        {
            Asp_Id = userId,
            Id = roleId
        });

        nguoiDung.Id = userId;

        await dbContext.SaveChangesAsync();
    }
}

