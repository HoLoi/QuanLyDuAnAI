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
        private const int SoPhongTaiBanDau = 20;
        private const int SoPhongToiDaMoiLanTai = 50;
        private const int SoTinNhanTaiBanDau = 30;
        private const int SoTinNhanToiDaMoiLanTai = 50;
        private const int DoDaiTinNhanToiDa = 2000;

        private readonly QuanLyDuAnDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IChatRealtimePublisher _realtimePublisher;
        private readonly ILogger<ChatDuAnService> _logger;
        public ChatDuAnService(
            QuanLyDuAnDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IChatRealtimePublisher realtimePublisher,
            ILogger<ChatDuAnService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _realtimePublisher = realtimePublisher;
            _logger = logger;
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

            var phongBatch = await GetPhongChatBatchNoAuthAsync(
                currentUserId,
                tuKhoa,
                null,
                SoPhongTaiBanDau);

            ChatDuAnPhongItemViewModel? phongDangChon = null;
            if (maDuAn.HasValue)
            {
                if (!await CoQuyenVaoDuAnAsync(maDuAn.Value, currentUserId))
                {
                    throw new Exception("Bạn không có quyền truy cập phòng chat của dự án này.");
                }

                phongDangChon = phongBatch.DanhSachPhong.FirstOrDefault(x => x.MaDuAn == maDuAn.Value);
                if (phongDangChon == null)
                {
                    phongDangChon = await GetPhongItemTheoDuAnAsync(maDuAn.Value);
                    if (phongDangChon == null)
                    {
                        var maPhong = await DamBaoPhongChatDuAnAsync(maDuAn.Value);
                        phongDangChon = await GetPhongItemTheoMaPhongAsync(maPhong);
                    }
                }
            }
            else
            {
                phongDangChon = phongBatch.DanhSachPhong.FirstOrDefault();
            }

            if (phongDangChon == null)
            {
                return new ChatDuAnPageViewModel
                {
                    TuKhoa = tuKhoa,
                    PhongBatch = phongBatch,
                    DanhSachPhong = phongBatch.DanhSachPhong,
                    ThongBaoTrangThai = "Bạn chưa tham gia dự án nào có phòng chat."
                };
            }

            foreach (var phong in phongBatch.DanhSachPhong)
            {
                phong.DangChon = phong.MaPhongChat == phongDangChon.MaPhongChat;
            }

            var tinNhanBatch = await GetTinNhanBatchNoAuthAsync(
                phongDangChon.MaPhongChat,
                currentUserId,
                null,
                null,
                SoTinNhanTaiBanDau);
            var coTheGui = await CoTheGuiTinNhanAsync(phongDangChon.MaPhongChat, currentUserId, roleFlags);

            return TaoPageViewModel(phongDangChon, phongBatch, tinNhanBatch, tuKhoa, coTheGui);
        }

        public async Task<ChatDuAnPageViewModel> GetPhongContentAsync(int maPhongChat)
        {
            KiemTraQuyenChat(Permissions.Chat.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            var phong = await GetPhongItemTheoMaPhongAsync(maPhongChat);
            if (phong == null || !await CoQuyenVaoPhongChatAsync(maPhongChat, currentUserId))
            {
                throw new Exception("Bạn không có quyền xem phòng chat này.");
            }

            if (!await LaThanhVienPhongChatAsync(maPhongChat, currentUserId))
            {
                throw new Exception("Bạn không còn là thành viên của phòng chat này.");
            }

            var tinNhanBatch = await GetTinNhanBatchNoAuthAsync(
                maPhongChat,
                currentUserId,
                null,
                null,
                SoTinNhanTaiBanDau);
            var coTheGui = await CoTheGuiTinNhanAsync(maPhongChat, currentUserId, roleFlags);
            var phongBatch = new ChatDuAnPhongBatchViewModel
            {
                DanhSachPhong = new List<ChatDuAnPhongItemViewModel> { phong }
            };

            return TaoPageViewModel(phong, phongBatch, tinNhanBatch, null, coTheGui);
        }

        public async Task<ChatDuAnTinNhanItemViewModel> GuiTinNhanAsync(ChatDuAnGuiTinNhanViewModel form)
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
                    da.TenDuAn,
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

            var tinNhanMoi = new TinNhan
            {
                MaPhongChat = thongTinPhong.MaPhongChat,
                MaNguoiDung = currentUserId,
                NoiDungTinNhan = noiDung,
                ThoiGianGui = DateTime.Now,
                IsDeleted = false
            };

            _context.TinNhan.Add(tinNhanMoi);
            await _context.SaveChangesAsync();

            var nguoiGui = await _context.NguoiDung
                .AsNoTracking()
                .Where(x => x.MaNguoiDung == currentUserId && x.IsDeleted != true)
                .Select(x => new
                {
                    x.HoTenNguoiDung,
                    x.AnhDaiDien
                })
                .FirstOrDefaultAsync();

            var realtimeMessage = new ChatRealtimeMessageDto
            {
                MaTinNhan = tinNhanMoi.MaTinNhan,
                MaPhongChat = tinNhanMoi.MaPhongChat,
                MaDuAn = thongTinPhong.MaDuAn,
                MaNguoiDung = currentUserId,
                TenNguoiGui = nguoiGui?.HoTenNguoiDung ?? $"Nhân viên {currentUserId}",
                AvatarUrl = nguoiGui?.AnhDaiDien,
                NoiDungTinNhan = noiDung,
                ThoiGianGui = tinNhanMoi.ThoiGianGui ?? DateTime.Now,
                TenDuAn = thongTinPhong.TenDuAn ?? $"Dự án {thongTinPhong.MaDuAn}"
            };

            try
            {
                var recipientIds = await LayIdentityUserIdNhanRealtimeAsync(
                    thongTinPhong.MaPhongChat,
                    thongTinPhong.MaDuAn);
                await _realtimePublisher.PublishMessageCreatedAsync(recipientIds, realtimeMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Chuẩn bị phát realtime thất bại cho tin {MessageId}, phòng {RoomId}. Tin đã được lưu.",
                    tinNhanMoi.MaTinNhan,
                    tinNhanMoi.MaPhongChat);
            }

            return new ChatDuAnTinNhanItemViewModel
            {
                MaTinNhan = tinNhanMoi.MaTinNhan,
                MaPhongChat = tinNhanMoi.MaPhongChat,
                MaNguoiDung = currentUserId,
                HoTenNguoiDung = nguoiGui?.HoTenNguoiDung ?? $"Nhân viên {currentUserId}",
                AnhDaiDien = nguoiGui?.AnhDaiDien,
                NoiDungTinNhan = noiDung,
                ThoiGianGui = tinNhanMoi.ThoiGianGui,
                LaTinNhanCuaToi = true
            };
        }

        public async Task<ChatDuAnTinNhanMoiBatchViewModel> GetTinNhanMoiAsync(
            int maPhongChat,
            int afterMessageId,
            int pageSize = SoTinNhanToiDaMoiLanTai)
        {
            KiemTraQuyenChat(Permissions.Chat.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            if (maPhongChat <= 0 || afterMessageId < 0)
            {
                throw new Exception("Tham số đồng bộ tin nhắn không hợp lệ.");
            }

            pageSize = Math.Clamp(pageSize, 1, SoTinNhanToiDaMoiLanTai);
            var room = await _context.PhongChat
                .AsNoTracking()
                .Where(x => x.MaPhongChat == maPhongChat && x.IsDeleted != true)
                .Select(x => new { x.MaPhongChat, x.MaDuAn })
                .FirstOrDefaultAsync();

            if (room == null
                || !await CoQuyenVaoDuAnAsync(room.MaDuAn, currentUserId)
                || !await LaThanhVienPhongChatAsync(maPhongChat, currentUserId))
            {
                throw new Exception("Bạn không có quyền đồng bộ tin nhắn của phòng chat này.");
            }

            var rows = await (
                from tn in _context.TinNhan.AsNoTracking()
                join nd in _context.NguoiDung.AsNoTracking() on tn.MaNguoiDung equals nd.MaNguoiDung
                from da in _context.DuAn.AsNoTracking()
                where tn.MaPhongChat == maPhongChat
                      && da.MaDuAn == room.MaDuAn
                      && tn.MaTinNhan > afterMessageId
                      && tn.IsDeleted != true
                      && nd.IsDeleted != true
                      && da.IsDeleted != true
                orderby tn.MaTinNhan
                select new ChatRealtimeMessageDto
                {
                    MaTinNhan = tn.MaTinNhan,
                    MaPhongChat = tn.MaPhongChat,
                    MaDuAn = da.MaDuAn,
                    MaNguoiDung = tn.MaNguoiDung,
                    TenNguoiGui = nd.HoTenNguoiDung ?? $"Nhân viên {tn.MaNguoiDung}",
                    AvatarUrl = nd.AnhDaiDien,
                    NoiDungTinNhan = tn.NoiDungTinNhan ?? string.Empty,
                    ThoiGianGui = tn.ThoiGianGui ?? DateTime.MinValue,
                    TenDuAn = da.TenDuAn ?? $"Dự án {da.MaDuAn}"
                })
                .Take(pageSize + 1)
                .ToListAsync();

            var hasMore = rows.Count > pageSize;
            if (hasMore)
            {
                rows.RemoveAt(rows.Count - 1);
            }

            return new ChatDuAnTinNhanMoiBatchViewModel
            {
                DanhSachTinNhan = rows,
                HasMore = hasMore
            };
        }

        public async Task<List<ChatDuAnPhongItemViewModel>> GetPhongChatDuocThamGiaAsync(string? tuKhoa)
        {
            var batch = await GetPhongChatBatchAsync(tuKhoa, pageSize: SoPhongToiDaMoiLanTai);
            return batch.DanhSachPhong;
        }

        public async Task<ChatDuAnPhongBatchViewModel> GetPhongChatBatchAsync(
            string? tuKhoa,
            int? lastRoomId = null,
            int pageSize = SoPhongTaiBanDau)
        {
            KiemTraQuyenChat(Permissions.Chat.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            return await GetPhongChatBatchNoAuthAsync(
                currentUserId,
                tuKhoa,
                lastRoomId,
                pageSize);
        }

        public async Task<List<ChatDuAnTinNhanItemViewModel>> GetTinNhanAsync(int maPhongChat)
        {
            var batch = await GetTinNhanBatchAsync(maPhongChat, pageSize: SoTinNhanToiDaMoiLanTai);
            return batch.DanhSachTinNhan;
        }

        public async Task<ChatDuAnTinNhanBatchViewModel> GetTinNhanBatchAsync(
            int maPhongChat,
            DateTime? cursorThoiGianGui = null,
            int? cursorMaTinNhan = null,
            int pageSize = SoTinNhanTaiBanDau)
        {
            KiemTraQuyenChat(Permissions.Chat.Xem);

            var currentUserId = await GetCurrentUserIdAsync();
            var roleFlags = await GetCurrentUserRoleFlagsAsync();
            KiemTraKhongChoAdminChatDuAn(roleFlags);

            return await GetTinNhanBatchNoAuthAsync(
                maPhongChat,
                currentUserId,
                cursorThoiGianGui,
                cursorMaTinNhan,
                pageSize);
        }

        private async Task<ChatDuAnPhongBatchViewModel> GetPhongChatBatchNoAuthAsync(
            int currentUserId,
            string? tuKhoa,
            int? lastRoomId,
            int pageSize)
        {
            pageSize = Math.Clamp(pageSize, 1, SoPhongToiDaMoiLanTai);
            var dsMaDuAn = await LayDanhSachMaDuAnTheoScopeAsync(currentUserId);
            if (dsMaDuAn.Count == 0)
            {
                return new ChatDuAnPhongBatchViewModel { TuKhoa = tuKhoa };
            }

            var query =
                from pc in _context.PhongChat.AsNoTracking()
                join da in _context.DuAn.AsNoTracking() on pc.MaDuAn equals da.MaDuAn
                where pc.IsDeleted != true
                      && da.IsDeleted != true
                      && dsMaDuAn.Contains(da.MaDuAn)
                select new
                {
                    pc.MaPhongChat,
                    pc.MaDuAn,
                    pc.TenPhong,
                    da.TenDuAn,
                    da.TrangThaiDuAn,
                    TinNhanMoiNhat = _context.TinNhan
                        .Where(tn => tn.MaPhongChat == pc.MaPhongChat && tn.IsDeleted != true)
                        .OrderByDescending(tn => tn.ThoiGianGui ?? DateTime.MinValue)
                        .ThenByDescending(tn => tn.MaTinNhan)
                        .Select(tn => tn.NoiDungTinNhan)
                        .FirstOrDefault(),
                    ThoiGianTinNhanMoiNhat = _context.TinNhan
                        .Where(tn => tn.MaPhongChat == pc.MaPhongChat && tn.IsDeleted != true)
                        .OrderByDescending(tn => tn.ThoiGianGui ?? DateTime.MinValue)
                        .ThenByDescending(tn => tn.MaTinNhan)
                        .Select(tn => tn.ThoiGianGui)
                        .FirstOrDefault(),
                    SoThanhVien = _context.ThanhVienPhongChat
                        .Count(tv => tv.MaPhongChat == pc.MaPhongChat),
                    SoTinNhan = _context.TinNhan
                        .Count(tn => tn.MaPhongChat == pc.MaPhongChat && tn.IsDeleted != true)
                };

            if (!string.IsNullOrWhiteSpace(tuKhoa))
            {
                var keyword = tuKhoa.Trim().ToLower();
                query = query.Where(x =>
                    (x.TenPhong ?? string.Empty).ToLower().Contains(keyword)
                    || (x.TenDuAn ?? string.Empty).ToLower().Contains(keyword));
            }

            if (lastRoomId.HasValue)
            {
                query = query.Where(x => x.MaPhongChat < lastRoomId.Value);
            }

            var rows = await query
                .OrderByDescending(x => x.MaPhongChat)
                .Take(pageSize + 1)
                .ToListAsync();

            var hasMore = rows.Count > pageSize;
            if (hasMore)
            {
                rows.RemoveAt(rows.Count - 1);
            }

            var items = rows.Select(x => new ChatDuAnPhongItemViewModel
            {
                MaPhongChat = x.MaPhongChat,
                MaDuAn = x.MaDuAn,
                TenPhong = string.IsNullOrWhiteSpace(x.TenPhong) ? $"Chat - {x.TenDuAn}" : x.TenPhong!,
                TenDuAn = x.TenDuAn ?? $"Dự án {x.MaDuAn}",
                TrangThaiDuAn = TrangThai.ToDisplay(x.TrangThaiDuAn),
                SoThanhVien = x.SoThanhVien,
                SoTinNhan = x.SoTinNhan,
                TinNhanMoiNhat = x.TinNhanMoiNhat,
                ThoiGianTinNhanMoiNhat = x.ThoiGianTinNhanMoiNhat
            }).ToList();

            var last = items.LastOrDefault();
            return new ChatDuAnPhongBatchViewModel
            {
                DanhSachPhong = items,
                HasMore = hasMore,
                NextRoomId = last?.MaPhongChat,
                TuKhoa = tuKhoa
            };
        }

        private async Task<ChatDuAnTinNhanBatchViewModel> GetTinNhanBatchNoAuthAsync(
            int maPhongChat,
            int currentUserId,
            DateTime? cursorThoiGianGui,
            int? cursorMaTinNhan,
            int pageSize)
        {
            if (maPhongChat <= 0)
            {
                throw new Exception("Phòng chat không hợp lệ.");
            }

            pageSize = Math.Clamp(pageSize, 1, SoTinNhanToiDaMoiLanTai);

            var roomExists = await _context.PhongChat
                .AsNoTracking()
                .AnyAsync(x => x.MaPhongChat == maPhongChat && x.IsDeleted != true);
            if (!roomExists)
            {
                throw new Exception("Không tìm thấy phòng chat.");
            }

            if (!await CoQuyenVaoPhongChatAsync(maPhongChat, currentUserId))
            {
                throw new Exception("Bạn không có quyền xem phòng chat này.");
            }

            if (!await LaThanhVienPhongChatAsync(maPhongChat, currentUserId))
            {
                throw new Exception("Bạn không còn là thành viên của phòng chat này.");
            }

            var query =
                from tn in _context.TinNhan.AsNoTracking()
                join nd in _context.NguoiDung.AsNoTracking() on tn.MaNguoiDung equals nd.MaNguoiDung
                where tn.MaPhongChat == maPhongChat
                      && tn.IsDeleted != true
                      && nd.IsDeleted != true
                select new
                {
                    TinNhan = tn,
                    nd.HoTenNguoiDung,
                    nd.AnhDaiDien
                };

            if (cursorMaTinNhan.HasValue)
            {
                if (!cursorThoiGianGui.HasValue)
                {
                    throw new Exception("Cursor tin nhắn không hợp lệ.");
                }

                var cursorTime = cursorThoiGianGui.Value;
                var cursorMessageId = cursorMaTinNhan.Value;
                query = query.Where(x =>
                    (x.TinNhan.ThoiGianGui ?? DateTime.MinValue) < cursorTime
                    || ((x.TinNhan.ThoiGianGui ?? DateTime.MinValue) == cursorTime
                        && x.TinNhan.MaTinNhan < cursorMessageId));
            }

            var rows = await query
                .OrderByDescending(x => x.TinNhan.ThoiGianGui ?? DateTime.MinValue)
                .ThenByDescending(x => x.TinNhan.MaTinNhan)
                .Take(pageSize + 1)
                .Select(x => new
                {
                    MaTinNhan = x.TinNhan.MaTinNhan,
                    MaPhongChat = x.TinNhan.MaPhongChat,
                    MaNguoiDung = x.TinNhan.MaNguoiDung,
                    x.HoTenNguoiDung,
                    AnhDaiDien = x.AnhDaiDien,
                    NoiDungTinNhan = x.TinNhan.NoiDungTinNhan ?? string.Empty,
                    ThoiGianGui = x.TinNhan.ThoiGianGui
                })
                .ToListAsync();

            var hasMore = rows.Count > pageSize;
            if (hasMore)
            {
                rows.RemoveAt(rows.Count - 1);
            }

            var items = rows.Select(x => new ChatDuAnTinNhanItemViewModel
            {
                MaTinNhan = x.MaTinNhan,
                MaPhongChat = x.MaPhongChat,
                MaNguoiDung = x.MaNguoiDung,
                HoTenNguoiDung = x.HoTenNguoiDung ?? $"Nhân viên {x.MaNguoiDung}",
                AnhDaiDien = x.AnhDaiDien,
                NoiDungTinNhan = x.NoiDungTinNhan,
                ThoiGianGui = x.ThoiGianGui,
                LaTinNhanCuaToi = x.MaNguoiDung == currentUserId
            }).ToList();
            items.Reverse();
            var oldest = items.FirstOrDefault();

            return new ChatDuAnTinNhanBatchViewModel
            {
                MaPhongChat = maPhongChat,
                DanhSachTinNhan = items,
                HasMore = hasMore,
                CursorThoiGianGui = oldest?.ThoiGianGui ?? (oldest == null ? null : DateTime.MinValue),
                CursorMaTinNhan = oldest?.MaTinNhan
            };
        }

        private async Task<ChatDuAnPhongItemViewModel?> GetPhongItemTheoDuAnAsync(int maDuAn)
        {
            var maPhongChat = await _context.PhongChat
                .AsNoTracking()
                .Where(x => x.MaDuAn == maDuAn && x.IsDeleted != true)
                .OrderBy(x => x.MaPhongChat)
                .Select(x => (int?)x.MaPhongChat)
                .FirstOrDefaultAsync();

            return maPhongChat.HasValue
                ? await GetPhongItemTheoMaPhongAsync(maPhongChat.Value)
                : null;
        }

        private async Task<ChatDuAnPhongItemViewModel?> GetPhongItemTheoMaPhongAsync(int maPhongChat)
        {
            var row = await (
                from pc in _context.PhongChat.AsNoTracking()
                join da in _context.DuAn.AsNoTracking() on pc.MaDuAn equals da.MaDuAn
                where pc.MaPhongChat == maPhongChat
                      && pc.IsDeleted != true
                      && da.IsDeleted != true
                select new
                {
                    pc.MaPhongChat,
                    pc.MaDuAn,
                    pc.TenPhong,
                    da.TenDuAn,
                    da.TrangThaiDuAn,
                    SoThanhVien = _context.ThanhVienPhongChat.Count(tv => tv.MaPhongChat == pc.MaPhongChat),
                    SoTinNhan = _context.TinNhan.Count(tn => tn.MaPhongChat == pc.MaPhongChat && tn.IsDeleted != true),
                    TinNhanMoiNhat = _context.TinNhan
                        .Where(tn => tn.MaPhongChat == pc.MaPhongChat && tn.IsDeleted != true)
                        .OrderByDescending(tn => tn.ThoiGianGui ?? DateTime.MinValue)
                        .ThenByDescending(tn => tn.MaTinNhan)
                        .Select(tn => tn.NoiDungTinNhan)
                        .FirstOrDefault(),
                    ThoiGianTinNhanMoiNhat = _context.TinNhan
                        .Where(tn => tn.MaPhongChat == pc.MaPhongChat && tn.IsDeleted != true)
                        .OrderByDescending(tn => tn.ThoiGianGui ?? DateTime.MinValue)
                        .ThenByDescending(tn => tn.MaTinNhan)
                        .Select(tn => tn.ThoiGianGui)
                        .FirstOrDefault()
                }).FirstOrDefaultAsync();

            if (row == null)
            {
                return null;
            }

            return new ChatDuAnPhongItemViewModel
            {
                MaPhongChat = row.MaPhongChat,
                MaDuAn = row.MaDuAn,
                TenPhong = string.IsNullOrWhiteSpace(row.TenPhong) ? $"Chat - {row.TenDuAn}" : row.TenPhong!,
                TenDuAn = row.TenDuAn ?? $"Dự án {row.MaDuAn}",
                TrangThaiDuAn = TrangThai.ToDisplay(row.TrangThaiDuAn),
                SoThanhVien = row.SoThanhVien,
                SoTinNhan = row.SoTinNhan,
                TinNhanMoiNhat = row.TinNhanMoiNhat,
                ThoiGianTinNhanMoiNhat = row.ThoiGianTinNhanMoiNhat
            };
        }

        private static ChatDuAnPageViewModel TaoPageViewModel(
            ChatDuAnPhongItemViewModel phong,
            ChatDuAnPhongBatchViewModel phongBatch,
            ChatDuAnTinNhanBatchViewModel tinNhanBatch,
            string? tuKhoa,
            bool coTheGui)
        {
            phong.DangChon = true;
            return new ChatDuAnPageViewModel
            {
                DanhSachPhong = phongBatch.DanhSachPhong,
                PhongBatch = phongBatch,
                PhongDangChon = phong,
                DanhSachTinNhan = tinNhanBatch.DanhSachTinNhan,
                TinNhanBatch = tinNhanBatch,
                TuKhoa = tuKhoa,
                MaDuAnDangChon = phong.MaDuAn,
                MaPhongChatDangChon = phong.MaPhongChat,
                CoTheGuiTinNhan = coTheGui,
                Form = new ChatDuAnGuiTinNhanViewModel
                {
                    MaDuAn = phong.MaDuAn,
                    MaPhongChat = phong.MaPhongChat
                }
            };
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

        private async Task<List<string>> LayIdentityUserIdNhanRealtimeAsync(
            int maPhongChat,
            int maDuAn)
        {
            var recipients = await (
                from tv in _context.ThanhVienPhongChat.AsNoTracking()
                join nd in _context.NguoiDung.AsNoTracking() on tv.MaNguoiDung equals nd.MaNguoiDung
                join asp in _context.Aspnetusers.AsNoTracking() on nd.MaNguoiDung equals asp.MaNguoiDung
                where tv.MaPhongChat == maPhongChat
                      && nd.IsDeleted != true
                      && (asp.LockoutEnabled == false
                          || !asp.LockoutEnd.HasValue
                          || asp.LockoutEnd.Value <= DateTime.UtcNow)
                      && (_context.DuAn.Any(da =>
                              da.MaDuAn == maDuAn
                              && da.IsDeleted != true
                              && da.MaNguoiDung == nd.MaNguoiDung)
                          || _context.NhanVienDuAn.Any(nv =>
                              nv.MaDuAn == maDuAn
                              && nv.MaNguoiDung == nd.MaNguoiDung))
                      && !_context.Aspnetuserroles.Any(ur =>
                          ur.Asp_Id == asp.Id
                          && _context.Aspnetroles.Any(role =>
                              role.Id == ur.Id
                              && ((role.NormalizedName ?? role.Name) ?? string.Empty).ToUpper() == "ADMIN"))
                      && (_context.Aspnetuserclaims.Any(uc =>
                              uc.Asp_Id == asp.Id
                              && uc.ClaimValue == Permissions.Chat.Xem)
                          || _context.Aspnetuserroles.Any(ur =>
                              ur.Asp_Id == asp.Id
                              && _context.Aspnetroleclaims.Any(rc =>
                                  rc.Asp_Id == ur.Id
                                  && (rc.ClaimValue == Permissions.Chat.Xem
                                      || _context.DanhMucQuyen.Any(q =>
                                          q.MaDanhMucQuyen == rc.MaDanhMucQuyen
                                          && q.TenDanhMucQuyen == Permissions.Chat.Xem)))))
                select asp.Id)
                .Distinct()
                .ToListAsync();

            return recipients;
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
