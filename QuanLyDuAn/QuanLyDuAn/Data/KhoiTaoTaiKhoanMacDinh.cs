using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Models.Entities;

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

        var taiKhoanAdmin = await dbContext.Aspnetusers
            .FirstOrDefaultAsync(x => x.NormalizedUserName == "ADMIN");

        if (taiKhoanAdmin is null)
        {
            var passwordHasher = new PasswordHasher<Aspnetusers>();
            await TaoNguoiDungMacDinhAsync(dbContext, passwordHasher, "admin", "Quan tri he thong", roleAdmin.Id);
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

            // ===== CHAT (tu? ch?n) =====
            Permissions.Chat.Xem,
            Permissions.Chat.Gui,

            // AI 
            Permissions.AI.Dataset,
            Permissions.AI.Train,
            Permissions.AI.DuDoan,
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
            Permissions.PhanCong.Xem,
            Permissions.PhanCong.ThucHien,

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

            // Danh gia
            Permissions.DanhGiaDuAn.Xem,
            Permissions.DanhGiaDuAn.DanhGia,
            Permissions.DanhGiaDuAn.Sua,
            Permissions.DanhGiaNhanVien.Xem,
            Permissions.DanhGiaNhanVien.DanhGia,
            Permissions.DanhGiaNhanVien.Sua,

            //AI
            Permissions.AI.DuDoan,
            Permissions.AI.Dashboard,

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

            // Chi tiết công việc
            Permissions.ChiTietCongViec.Xem,
            Permissions.ChiTietCongViec.Them,
            Permissions.ChiTietCongViec.Sua,
            Permissions.ChiTietCongViec.Xoa,

            // Tiến độ
            Permissions.TienDo.CapNhat,

            // Chat
            Permissions.Chat.Xem,
            Permissions.Chat.Gui,

            // Xem thống kê đơn giản
            Permissions.ThongKe.Xem,
            Permissions.ThongKe.XuatFile,

            // Xem đánh giá
            Permissions.DanhGiaNhanVien.Xem
        });
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
            (ten: "Quan tri vien", moTa: "Quan tri he thong va phan quyen"),
            (ten: "Quan ly du an", moTa: "Dieu phoi va duyet nghiep vu du an"),
            (ten: "Developer", moTa: "Lap trinh vien tham gia phat trien"),
            (ten: "Tester", moTa: "Kiem thu va dam bao chat luong"),
            (ten: "Business Analyst", moTa: "Phan tich nghiep vu")
        };

        var hienCo = await dbContext.ChucDanh
            .AsNoTracking()
            .Where(x => x.TenChucDanh != null)
            .Select(x => x.TenChucDanh!)
            .ToListAsync();

        foreach (var item in mau)
        {
            if (hienCo.Contains(item.ten, StringComparer.OrdinalIgnoreCase))
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
            (ten: "Phat trien phan mem", moTa: "Du an xay dung he thong phan mem"),
            (ten: "Bao tri nang cap", moTa: "Du an bao tri va nang cap he thong"),
            (ten: "Nghien cuu AI", moTa: "Du an tap trung nghien cuu ve AI")
        };

        var hienCo = await dbContext.LoaiDuAn
            .AsNoTracking()
            .Where(x => x.TenLoai != null)
            .Select(x => x.TenLoai!)
            .ToListAsync();

        foreach (var item in mau)
        {
            if (hienCo.Contains(item.ten, StringComparer.OrdinalIgnoreCase))
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
            (ten: "Thap", moTa: "Muc uu tien thap"),
            (ten: "Trung binh", moTa: "Muc uu tien trung binh"),
            (ten: "Cao", moTa: "Muc uu tien cao"),
            (ten: "Khan cap", moTa: "Xu ly ngay lap tuc")
        };

        var hienCo = await dbContext.MucDoUuTien
            .AsNoTracking()
            .Where(x => x.TenMucDo != null)
            .Select(x => x.TenMucDo!)
            .ToListAsync();

        foreach (var item in mau)
        {
            if (hienCo.Contains(item.ten, StringComparer.OrdinalIgnoreCase))
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
            (ten: "Tien do", diem: 10d, moTa: "Danh gia theo muc do dap ung tien do", loai: "NhanVien"),
            (ten: "Chat luong", diem: 10d, moTa: "Danh gia chat luong dau ra", loai: "NhanVien"),
            (ten: "Hop tac", diem: 10d, moTa: "Danh gia kha nang lam viec nhom", loai: "NhanVien"),
            (ten: "Hieu qua ngan sach", diem: 10d, moTa: "Danh gia su dung ngan sach cua du an", loai: "DuAn")
        };

        var hienCo = await dbContext.TieuChiDanhGia
            .AsNoTracking()
            .Where(x => x.TenTieuChi != null)
            .Select(x => x.TenTieuChi!)
            .ToListAsync();

        foreach (var item in mau)
        {
            if (hienCo.Contains(item.ten, StringComparer.OrdinalIgnoreCase))
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
        var mau = new[]
        {
            "Cham phe duyet",
            "Thieu nhan su",
            "Thay doi yeu cau lien tuc",
            "Vuot ngan sach",
            "Rui ro ky thuat"
        };

        var hienCo = await dbContext.DmNguyenNhan
            .AsNoTracking()
            .Where(x => x.TenNguyenNhan != null)
            .Select(x => x.TenNguyenNhan!)
            .ToListAsync();

        foreach (var ten in mau)
        {
            if (hienCo.Contains(ten, StringComparer.OrdinalIgnoreCase))
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
        var cauHinh = new Dictionary<string, string[]>
        {
            // ===== DASHBOARD =====
            ["Dashboard"] = new[]
            {
                Permissions.ThongKe.Xem
            },

            // ===== HE THONG =====
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

            // ===== DU AN =====
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

            // ===== CONG VIEC =====
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

            ["PhanCong"] = new[]
            {
                Permissions.PhanCong.Xem,
                Permissions.PhanCong.ThucHien
            },

            ["TienDo"] = new[]
            {
                Permissions.TienDo.Xem,
                Permissions.TienDo.CapNhat
            },

            // ===== TAI CHINH =====
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
                Permissions.AI.DuDoan
            },

            ["AIDashboard"] = new[]
            {
                Permissions.AI.Dashboard
            },

            // ===== DANH GIA =====
            ["DanhGiaDuAn"] = new[]
            {
                Permissions.DanhGiaDuAn.Xem,
                Permissions.DanhGiaDuAn.DanhGia,
                Permissions.DanhGiaDuAn.Sua
            },

            ["DanhGiaNhanVien"] = new[]
            {
                Permissions.DanhGiaNhanVien.Xem,
                Permissions.DanhGiaNhanVien.DanhGia,
                Permissions.DanhGiaNhanVien.Sua
            },

            // ===== KHAC =====
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
                MoTaManHinh = $"Danh muc man hinh {tenManHinh}"
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
                    MoTaDanhMucQuyen = $"Quyen {tenQuyen}"
                });
            }
        }

        await dbContext.SaveChangesAsync();
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
            .FirstOrDefaultAsync(x => x.TenChucDanh == "Quan tri vien");

        if (chucDanhAdmin is null)
        {
            chucDanhAdmin = new ChucDanh
            {
                TenChucDanh = "Quan tri vien",
                MoTaChucDanh = "Chuc danh mac dinh cho tai khoan admin"
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
        user.PasswordHash = passwordHasher.HashPassword(user, "123456");

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
