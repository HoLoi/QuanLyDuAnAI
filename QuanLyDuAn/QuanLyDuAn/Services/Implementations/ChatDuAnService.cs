using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.ChatDuAn;

namespace QuanLyDuAn.Services.Implementations
{
    public class ChatDuAnService : IChatDuAnService
    {
        private const int GioiHanTinNhan = 200;
        private const int DoDaiTinNhanToiDa = 2000;

        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatDuAnService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> DamBaoPhongChatDuAnAsync(int maDuAn)
        {
            var duAn = await _context.DuAn
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                throw new Exception("Không tìm thấy dự án hoặc dự án đã bị xóa.");
            }

            var maPhongChat = await DamBaoPhongChatNoiBoAsync(duAn);
            await DongBoThanhVienPhongChatAsync(maDuAn);

            return maPhongChat;
        }

        public async Task DongBoThanhVienPhongChatAsync(int maDuAn)
        {
            var duAn = await _context.DuAn
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                throw new Exception("Không tìm thấy dự án hoặc dự án đã bị xóa.");
            }

            var maPhongChat = await DamBaoPhongChatNoiBoAsync(duAn);
            var roleByUser = await LayVaiTroTheoNguoiDungAsync(duAn);

            if (roleByUser.Count == 0)
            {
                await XoaToanBoThanhVienPhongChatAsync(maPhongChat);
                return;
            }

            var validUserIds = await _context.NguoiDung
                .AsNoTracking()
                .Where(x => roleByUser.Keys.Contains(x.MaNguoiDung) && x.IsDeleted != true)
                .Select(x => x.MaNguoiDung)
                .ToListAsync();

            var validUserSet = validUserIds.ToHashSet();
            var expectedRoles = roleByUser
                .Where(x => validUserSet.Contains(x.Key))
                .ToDictionary(x => x.Key, x => x.Value);

            if (expectedRoles.Count == 0)
            {
                await XoaToanBoThanhVienPhongChatAsync(maPhongChat);
                return;
            }

            var adminIds = await LayTapNguoiDungAdminAsync(expectedRoles.Keys.ToList());
            foreach (var adminId in adminIds)
            {
                expectedRoles.Remove(adminId);
            }

            if (expectedRoles.Count == 0)
            {
                await XoaToanBoThanhVienPhongChatAsync(maPhongChat);
                return;
            }

            var currentMembers = await _context.ThanhVienPhongChat
                .Where(x => x.MaPhongChat == maPhongChat)
                .ToListAsync();

            var daThayDoi = false;

            foreach (var userId in expectedRoles.Keys)
            {
                var expectedRole = expectedRoles[userId];
                var existing = currentMembers.FirstOrDefault(x => x.MaNguoiDung == userId);

                if (existing == null)
                {
                    _context.ThanhVienPhongChat.Add(new ThanhVienPhongChat
                    {
                        MaPhongChat = maPhongChat,
                        MaNguoiDung = userId,
                        VaiTroTrongPhongChat = expectedRole
                    });
                    daThayDoi = true;
                    continue;
                }

                if (!string.Equals(existing.VaiTroTrongPhongChat, expectedRole, StringComparison.OrdinalIgnoreCase))
                {
                    existing.VaiTroTrongPhongChat = expectedRole;
                    daThayDoi = true;
                }
            }

            var duplicateMembers = currentMembers
                .GroupBy(x => x.MaNguoiDung)
                .SelectMany(x => x.Skip(1))
                .ToList();

            if (duplicateMembers.Count > 0)
            {
                _context.ThanhVienPhongChat.RemoveRange(duplicateMembers);
                daThayDoi = true;
            }

            var obsoleteMembers = currentMembers
                .Where(x => !expectedRoles.ContainsKey(x.MaNguoiDung))
                .ToList();

            if (obsoleteMembers.Count > 0)
            {
                _context.ThanhVienPhongChat.RemoveRange(obsoleteMembers);
                daThayDoi = true;
            }

            if (daThayDoi)
            {
                await _context.SaveChangesAsync();
            }
        }

        public async Task<ChatDuAnPageViewModel> GetPageAsync(int? maDuAn, string? tuKhoa)
        {
            KiemTraQuyenChat(Permissions.Chat.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            var dsMaDuAnTheoScope = await LayDanhSachMaDuAnTheoScopeAsync(currentUserId);
            if (dsMaDuAnTheoScope.Count == 0)
            {
                return new ChatDuAnPageViewModel
                {
                    TuKhoa = tuKhoa,
                    ThongBaoTrangThai = "Bạn chưa tham gia dự án nào có phòng chat."
                };
            }

            var danhSachPhong = await GetPhongChatDuocThamGiaAsync(tuKhoa);
            if (danhSachPhong.Count == 0)
            {
                return new ChatDuAnPageViewModel
                {
                    TuKhoa = tuKhoa,
                    ThongBaoTrangThai = "Bạn chưa tham gia dự án nào có phòng chat."
                };
            }

            ChatDuAnPhongItemViewModel? phongDangChon;

            if (maDuAn.HasValue)
            {
                var coQuyen = await CoQuyenVaoDuAnAsync(maDuAn.Value, currentUserId);
                if (!coQuyen)
                {
                    throw new Exception("Bạn không có quyền truy cập phòng chat của dự án này.");
                }

                phongDangChon = danhSachPhong.FirstOrDefault(x => x.MaDuAn == maDuAn.Value);
                if (phongDangChon == null)
                {
                    var maPhong = await DamBaoPhongChatDuAnAsync(maDuAn.Value);
                    danhSachPhong = await GetPhongChatDuocThamGiaAsync(tuKhoa);
                    phongDangChon = danhSachPhong.FirstOrDefault(x => x.MaPhongChat == maPhong);
                }
            }
            else
            {
                phongDangChon = danhSachPhong.First();
            }

            if (phongDangChon == null)
            {
                return new ChatDuAnPageViewModel
                {
                    TuKhoa = tuKhoa,
                    DanhSachPhong = danhSachPhong,
                    ThongBaoTrangThai = "Bạn chưa tham gia dự án nào có phòng chat."
                };
            }

            var danhSachTinNhan = await GetTinNhanAsync(phongDangChon.MaPhongChat);

            foreach (var phong in danhSachPhong)
            {
                phong.DangChon = phong.MaPhongChat == phongDangChon.MaPhongChat;
            }

            var coTheGui = await CoTheGuiTinNhanAsync(phongDangChon.MaPhongChat, currentUserId, roleFlags);

            return new ChatDuAnPageViewModel
            {
                DanhSachPhong = danhSachPhong,
                PhongDangChon = phongDangChon,
                DanhSachTinNhan = danhSachTinNhan,
                TuKhoa = tuKhoa,
                MaDuAnDangChon = phongDangChon.MaDuAn,
                MaPhongChatDangChon = phongDangChon.MaPhongChat,
                CoTheGuiTinNhan = coTheGui,
                Form = new ChatDuAnGuiTinNhanViewModel
                {
                    MaDuAn = phongDangChon.MaDuAn,
                    MaPhongChat = phongDangChon.MaPhongChat
                }
            };
        }

        public async Task GuiTinNhanAsync(ChatDuAnGuiTinNhanViewModel form)
        {
            KiemTraQuyenChat(Permissions.Chat.Gui);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            if (form.MaPhongChat <= 0)
            {
                throw new Exception("Phòng chat không hợp lệ.");
            }

            var noiDung = (form.NoiDungTinNhan ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(noiDung))
            {
                throw new Exception("Vui lòng nhập nội dung tin nhắn.");
            }

            if (noiDung.Length > DoDaiTinNhanToiDa)
            {
                throw new Exception($"Tin nhắn tối đa {DoDaiTinNhanToiDa} ký tự.");
            }

            var thongTinPhong = await (
                from pc in _context.PhongChat
                join da in _context.DuAn on pc.MaDuAn equals da.MaDuAn
                where pc.MaPhongChat == form.MaPhongChat
                      && pc.IsDeleted != true
                      && da.IsDeleted != true
                select new
                {
                    pc.MaPhongChat,
                    pc.MaDuAn,
                    da.TrangThaiDuAn
                }).FirstOrDefaultAsync();

            if (thongTinPhong == null)
            {
                throw new Exception("Không tìm thấy phòng chat.");
            }

            var coQuyenPhongChat = await CoQuyenVaoPhongChatAsync(thongTinPhong.MaPhongChat, currentUserId);
            if (!coQuyenPhongChat)
            {
                throw new Exception("Bạn không có quyền gửi tin nhắn trong phòng chat này.");
            }

            if (TrangThai.EqualsValue(thongTinPhong.TrangThaiDuAn, TrangThai.DaHuy)
                || TrangThai.EqualsValue(thongTinPhong.TrangThaiDuAn, TrangThai.LuuTru))
            {
                throw new Exception("Dự án đã đóng, không thể gửi tin nhắn.");
            }

            await DongBoThanhVienPhongChatAsync(thongTinPhong.MaDuAn);

            var laThanhVienHopLe = await LaThanhVienPhongChatAsync(thongTinPhong.MaPhongChat, currentUserId);
            if (!laThanhVienHopLe)
            {
                throw new Exception("Bạn không còn thuộc phạm vi dự án để gửi tin nhắn.");
            }

            _context.TinNhan.Add(new TinNhan
            {
                MaPhongChat = thongTinPhong.MaPhongChat,
                MaNguoiDung = currentUserId,
                NoiDungTinNhan = noiDung,
                ThoiGianGui = DateTime.Now,
                IsDeleted = false
            });

            await _context.SaveChangesAsync();
        }

        public async Task<List<ChatDuAnPhongItemViewModel>> GetPhongChatDuocThamGiaAsync(string? tuKhoa)
        {
            KiemTraQuyenChat(Permissions.Chat.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            var dsMaDuAn = await LayDanhSachMaDuAnTheoScopeAsync(currentUserId);
            if (dsMaDuAn.Count == 0)
            {
                return new List<ChatDuAnPhongItemViewModel>();
            }

            foreach (var maDuAn in dsMaDuAn)
            {
                await DamBaoPhongChatDuAnAsync(maDuAn);
            }

            var query =
                from pc in _context.PhongChat
                join da in _context.DuAn on pc.MaDuAn equals da.MaDuAn
                where pc.IsDeleted != true
                      && da.IsDeleted != true
                      && dsMaDuAn.Contains(da.MaDuAn)
                select new
                {
                    pc.MaPhongChat,
                    pc.MaDuAn,
                    pc.TenPhong,
                    da.TenDuAn,
                    da.TrangThaiDuAn
                };

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    (x.TenPhong ?? string.Empty).ToLower().Contains(keyword)
                    || (x.TenDuAn ?? string.Empty).ToLower().Contains(keyword));
            }

            var roomRows = await query
                .OrderBy(x => x.TenDuAn)
                .ThenBy(x => x.MaPhongChat)
                .ToListAsync();

            if (roomRows.Count == 0)
            {
                return new List<ChatDuAnPhongItemViewModel>();
            }

            var roomIds = roomRows.Select(x => x.MaPhongChat).Distinct().ToList();

            var soThanhVienByPhong = await _context.ThanhVienPhongChat
                .Where(x => roomIds.Contains(x.MaPhongChat))
                .GroupBy(x => x.MaPhongChat)
                .Select(x => new
                {
                    MaPhongChat = x.Key,
                    SoLuong = x.Select(v => v.MaNguoiDung).Distinct().Count()
                }).ToDictionaryAsync(x => x.MaPhongChat, x => x.SoLuong);

            var soTinNhanByPhong = await _context.TinNhan
                .Where(x => roomIds.Contains(x.MaPhongChat) && x.IsDeleted != true)
                .GroupBy(x => x.MaPhongChat)
                .Select(x => new
                {
                    MaPhongChat = x.Key,
                    SoLuong = x.Count()
                }).ToDictionaryAsync(x => x.MaPhongChat, x => x.SoLuong);

            var tinMoiNhatRows = await _context.TinNhan
                .Where(x => roomIds.Contains(x.MaPhongChat) && x.IsDeleted != true)
                .OrderByDescending(x => x.ThoiGianGui ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaTinNhan)
                .Select(x => new
                {
                    x.MaPhongChat,
                    x.NoiDungTinNhan,
                    x.ThoiGianGui
                })
                .ToListAsync();

            var tinMoiNhatByPhong = tinMoiNhatRows
                .GroupBy(x => x.MaPhongChat)
                .ToDictionary(x => x.Key, x => x.First());

            return roomRows.Select(x =>
            {
                tinMoiNhatByPhong.TryGetValue(x.MaPhongChat, out var tinMoiNhat);
                soThanhVienByPhong.TryGetValue(x.MaPhongChat, out var soThanhVien);
                soTinNhanByPhong.TryGetValue(x.MaPhongChat, out var soTinNhan);

                return new ChatDuAnPhongItemViewModel
                {
                    MaPhongChat = x.MaPhongChat,
                    MaDuAn = x.MaDuAn,
                    TenPhong = string.IsNullOrWhiteSpace(x.TenPhong) ? $"Chat - {x.TenDuAn}" : x.TenPhong!,
                    TenDuAn = x.TenDuAn ?? $"Dự án {x.MaDuAn}",
                    TrangThaiDuAn = TrangThai.ToDisplay(x.TrangThaiDuAn),
                    SoThanhVien = soThanhVien,
                    SoTinNhan = soTinNhan,
                    TinNhanMoiNhat = tinMoiNhat?.NoiDungTinNhan,
                    ThoiGianTinNhanMoiNhat = tinMoiNhat?.ThoiGianGui
                };
            }).ToList();
        }

        public async Task<List<ChatDuAnTinNhanItemViewModel>> GetTinNhanAsync(int maPhongChat)
        {
            KiemTraQuyenChat(Permissions.Chat.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            if (maPhongChat <= 0)
            {
                throw new Exception("Phòng chat không hợp lệ.");
            }

            var roomInfo = await _context.PhongChat
                .Where(x => x.MaPhongChat == maPhongChat && x.IsDeleted != true)
                .Select(x => new
                {
                    x.MaPhongChat,
                    x.MaDuAn
                }).FirstOrDefaultAsync();

            if (roomInfo == null)
            {
                throw new Exception("Không tìm thấy phòng chat.");
            }

            var coQuyen = await CoQuyenVaoPhongChatAsync(maPhongChat, currentUserId);
            if (!coQuyen)
            {
                throw new Exception("Bạn không có quyền xem phòng chat này.");
            }

            await DongBoThanhVienPhongChatAsync(roomInfo.MaDuAn);

            var tinNhan = await (
                from tn in _context.TinNhan
                join nd in _context.NguoiDung on tn.MaNguoiDung equals nd.MaNguoiDung
                where tn.MaPhongChat == maPhongChat
                      && tn.IsDeleted != true
                      && nd.IsDeleted != true
                orderby (tn.ThoiGianGui ?? DateTime.MinValue) descending, tn.MaTinNhan descending
                select new ChatDuAnTinNhanItemViewModel
                {
                    MaTinNhan = tn.MaTinNhan,
                    MaPhongChat = tn.MaPhongChat,
                    MaNguoiDung = tn.MaNguoiDung,
                    HoTenNguoiDung = nd.HoTenNguoiDung ?? $"Nhân viên {nd.MaNguoiDung}",
                    AnhDaiDien = nd.AnhDaiDien,
                    NoiDungTinNhan = tn.NoiDungTinNhan ?? string.Empty,
                    ThoiGianGui = tn.ThoiGianGui,
                    LaTinNhanCuaToi = tn.MaNguoiDung == currentUserId
                }).Take(GioiHanTinNhan).ToListAsync();

            tinNhan.Reverse();
            return tinNhan;
        }

        private async Task<int> DamBaoPhongChatNoiBoAsync(DuAn duAn)
        {
            var phong = await _context.PhongChat
                .Where(x => x.MaDuAn == duAn.MaDuAn && x.IsDeleted != true)
                .OrderBy(x => x.MaPhongChat)
                .FirstOrDefaultAsync();

            if (phong != null)
            {
                return phong.MaPhongChat;
            }

            phong = new PhongChat
            {
                MaDuAn = duAn.MaDuAn,
                TenPhong = $"Chat - {duAn.TenDuAn}",
                IsDeleted = false
            };

            _context.PhongChat.Add(phong);
            await _context.SaveChangesAsync();
            return phong.MaPhongChat;
        }

        private async Task XoaToanBoThanhVienPhongChatAsync(int maPhongChat)
        {
            var currentMembers = await _context.ThanhVienPhongChat
                .Where(x => x.MaPhongChat == maPhongChat)
                .ToListAsync();

            if (currentMembers.Count == 0)
            {
                return;
            }

            _context.ThanhVienPhongChat.RemoveRange(currentMembers);
            await _context.SaveChangesAsync();
        }

        private async Task<Dictionary<int, string>> LayVaiTroTheoNguoiDungAsync(DuAn duAn)
        {
            var roleByUser = new Dictionary<int, string>();

            if (duAn.MaNguoiDung > 0)
            {
                GanVaiTroCaoHon(roleByUser, duAn.MaNguoiDung, "QuanLy");
            }

            var thanhVienDuAn = await _context.NhanVienDuAn
                .Where(x => x.MaDuAn == duAn.MaDuAn)
                .Select(x => new
                {
                    x.MaNguoiDung,
                    x.VaiTroTrongDuAn
                })
                .ToListAsync();

            foreach (var item in thanhVienDuAn)
            {
                var role = TrangThai.EqualsValue(item.VaiTroTrongDuAn, TrangThai.VaiTroLeader)
                    ? "Leader"
                    : "ThanhVien";
                GanVaiTroCaoHon(roleByUser, item.MaNguoiDung, role);
            }

            return roleByUser;
        }

        private async Task<HashSet<int>> LayTapNguoiDungAdminAsync(List<int> maNguoiDungList)
        {
            if (maNguoiDungList.Count == 0)
            {
                return new HashSet<int>();
            }

            var adminIds = await (
                from nd in _context.NguoiDung
                join asp in _context.Aspnetusers on nd.Id equals asp.Id
                join ur in _context.Aspnetuserroles on asp.Id equals ur.Asp_Id
                join role in _context.Aspnetroles on ur.Id equals role.Id
                where maNguoiDungList.Contains(nd.MaNguoiDung)
                      && ((role.NormalizedName ?? role.Name) ?? string.Empty).ToUpper() == "ADMIN"
                select nd.MaNguoiDung
            ).Distinct().ToListAsync();

            return adminIds.ToHashSet();
        }

        private static void GanVaiTroCaoHon(IDictionary<int, string> roleByUser, int maNguoiDung, string vaiTroMoi)
        {
            if (!roleByUser.TryGetValue(maNguoiDung, out var vaiTroCu))
            {
                roleByUser[maNguoiDung] = vaiTroMoi;
                return;
            }

            if (TinhDiemVaiTro(vaiTroMoi) > TinhDiemVaiTro(vaiTroCu))
            {
                roleByUser[maNguoiDung] = vaiTroMoi;
            }
        }

        private static int TinhDiemVaiTro(string? vaiTro)
        {
            if (string.Equals(vaiTro, "QuanLy", StringComparison.OrdinalIgnoreCase))
            {
                return 3;
            }

            if (string.Equals(vaiTro, "Leader", StringComparison.OrdinalIgnoreCase))
            {
                return 2;
            }

            return 1;
        }

        private void KiemTraQuyenChat(string tenQuyen)
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
            {
                throw new Exception("Bạn chưa đăng nhập.");
            }

            var coQuyen = user.Claims.Any(claim =>
                !string.IsNullOrWhiteSpace(claim.Value)
                && string.Equals(claim.Value.Trim(), tenQuyen, StringComparison.OrdinalIgnoreCase));

            if (!coQuyen)
            {
                throw new Exception("Bạn không có quyền truy cập chức năng chat dự án.");
            }
        }

        private async Task<int> GetCurrentUserIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Không xác định được người dùng hiện tại.");
            }

            var maNguoiDung = await _context.Aspnetusers
                .Where(x => x.Id == aspUserId)
                .Select(x => x.MaNguoiDung)
                .FirstOrDefaultAsync();

            if (maNguoiDung <= 0)
            {
                throw new Exception("Không xác định được nhân sự tương ứng của người dùng hiện tại.");
            }

            return maNguoiDung;
        }

        private async Task<(bool IsAdmin, bool IsManager, bool IsEmployee)> GetCurrentUserRoleFlagsAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            var aspUserId = user?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrWhiteSpace(aspUserId))
            {
                throw new Exception("Không xác định được người dùng hiện tại.");
            }

            var roleNames = await (
                from ur in _context.Aspnetuserroles
                join r in _context.Aspnetroles on ur.Id equals r.Id
                where ur.Asp_Id == aspUserId
                select (r.NormalizedName ?? r.Name) ?? string.Empty
            ).ToListAsync();

            var normalizedRoles = roleNames
                .Select(x => x.Trim().ToUpperInvariant())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToHashSet();

            return (normalizedRoles.Contains("ADMIN"), normalizedRoles.Contains("MANAGER"), normalizedRoles.Contains("EMPLOYEE"));
        }

        private static void KiemTraKhongChoAdminChatDuAn((bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
            {
                throw new Exception("Quản trị viên không tham gia trao đổi nghiệp vụ dự án.");
            }
        }

        private async Task<bool> CoQuyenVaoDuAnAsync(int maDuAn, int currentUserId)
        {
            if (await LaAdminAsync(currentUserId))
            {
                return false;
            }

            var duAn = await _context.DuAn
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MaDuAn == maDuAn && x.IsDeleted != true);

            if (duAn == null)
            {
                return false;
            }

            if (duAn.MaNguoiDung == currentUserId)
            {
                return true;
            }

            var thamGiaTrucTiep = await _context.NhanVienDuAn
                .AnyAsync(x => x.MaDuAn == maDuAn && x.MaNguoiDung == currentUserId);

            if (thamGiaTrucTiep)
            {
                return true;
            }

            return false;
        }

        private async Task<bool> CoQuyenVaoPhongChatAsync(int maPhongChat, int currentUserId)
        {
            var maDuAn = await _context.PhongChat
                .Where(x => x.MaPhongChat == maPhongChat && x.IsDeleted != true)
                .Select(x => (int?)x.MaDuAn)
                .FirstOrDefaultAsync();

            if (!maDuAn.HasValue)
            {
                return false;
            }

            return await CoQuyenVaoDuAnAsync(maDuAn.Value, currentUserId);
        }

        private async Task<List<int>> LayDanhSachMaDuAnTheoScopeAsync(int currentUserId)
        {
            var managedProjects = await _context.DuAn
                .Where(x => x.IsDeleted != true && x.MaNguoiDung == currentUserId)
                .Select(x => x.MaDuAn)
                .ToListAsync();

            var directProjects = await (
                from nvda in _context.NhanVienDuAn
                join da in _context.DuAn on nvda.MaDuAn equals da.MaDuAn
                where nvda.MaNguoiDung == currentUserId && da.IsDeleted != true
                select da.MaDuAn
            ).ToListAsync();

            return managedProjects
                .Concat(directProjects)
                .Distinct()
                .OrderBy(x => x)
                .ToList();
        }

        private async Task<bool> LaThanhVienPhongChatAsync(int maPhongChat, int maNguoiDung)
        {
            return await _context.ThanhVienPhongChat
                .AnyAsync(x => x.MaPhongChat == maPhongChat && x.MaNguoiDung == maNguoiDung);
        }

        private async Task<bool> LaAdminAsync(int maNguoiDung)
        {
            return await (
                from nd in _context.NguoiDung
                join asp in _context.Aspnetusers on nd.Id equals asp.Id
                join ur in _context.Aspnetuserroles on asp.Id equals ur.Asp_Id
                join role in _context.Aspnetroles on ur.Id equals role.Id
                where nd.MaNguoiDung == maNguoiDung
                      && ((role.NormalizedName ?? role.Name) ?? string.Empty).ToUpper() == "ADMIN"
                select nd.MaNguoiDung
            ).AnyAsync();
        }

        private async Task<bool> CoTheGuiTinNhanAsync(int maPhongChat, int currentUserId, (bool IsAdmin, bool IsManager, bool IsEmployee) roleFlags)
        {
            if (roleFlags.IsAdmin)
            {
                return false;
            }

            var room = await _context.PhongChat
                .AsNoTracking()
                .Where(x => x.MaPhongChat == maPhongChat && x.IsDeleted != true)
                .Select(x => new
                {
                    x.MaPhongChat,
                    x.MaDuAn
                })
                .FirstOrDefaultAsync();

            if (room == null)
            {
                return false;
            }

            var coQuyenDuAn = await CoQuyenVaoDuAnAsync(room.MaDuAn, currentUserId);
            if (!coQuyenDuAn)
            {
                return false;
            }

            var duAnHopLe = await _context.DuAn
                .AsNoTracking()
                .Where(x => x.MaDuAn == room.MaDuAn && x.IsDeleted != true)
                .Select(x => x.TrangThaiDuAn)
                .FirstOrDefaultAsync();

            if (TrangThai.EqualsValue(duAnHopLe, TrangThai.DaHuy)
                || TrangThai.EqualsValue(duAnHopLe, TrangThai.LuuTru))
            {
                return false;
            }

            return await LaThanhVienPhongChatAsync(maPhongChat, currentUserId);
        }
    }
}
