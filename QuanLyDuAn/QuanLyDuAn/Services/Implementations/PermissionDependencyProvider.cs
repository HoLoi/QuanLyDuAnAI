using QuanLyDuAn.Constants;
using QuanLyDuAn.Services.Interfaces;

namespace QuanLyDuAn.Services.Implementations;

public class PermissionDependencyProvider : IPermissionDependencyProvider
{
    private readonly IReadOnlyList<PermissionDefinition> _definitions;
    private readonly Dictionary<string, PermissionDefinition> _definitionByPermission;
    private readonly Dictionary<string, string> _parentByChild;
    private readonly Dictionary<string, HashSet<string>> _requiredByRole;
    private readonly Dictionary<string, HashSet<string>> _deniedByRole;

    public PermissionDependencyProvider()
    {
        _definitions = BuildDefinitions();
        _definitionByPermission = _definitions
            .GroupBy(x => x.PermissionName, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);
        _parentByChild = _definitions
            .Where(x => !string.IsNullOrWhiteSpace(x.ParentPermission))
            .ToDictionary(x => x.PermissionName, x => x.ParentPermission!, StringComparer.OrdinalIgnoreCase);

        _requiredByRole = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Admin"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                Permissions.PhanQuyen.Xem,
                Permissions.PhanQuyen.Luu
            }
        };

        _deniedByRole = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["Employee"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                Permissions.PhanQuyen.Luu,
                Permissions.DuyetNganSach.Duyet,
                Permissions.DuyetDeXuatCongViec.Duyet,
                Permissions.DuyetYeuCauDoiQuanLy.Duyet,
                Permissions.DanhGiaDuAn.Duyet,
                Permissions.DanhGiaNhanVien.Duyet,
                Permissions.TienDo.Duyet,
                Permissions.AI.Dataset,
                Permissions.AI.Train,
                Permissions.AI.XacNhan
            },
            ["Manager"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                Permissions.PhanQuyen.Luu,
                Permissions.AI.Dataset,
                Permissions.AI.Train
            },
            ["Admin"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                Permissions.Chat.Gui,
                Permissions.TienDo.CapNhat,
                Permissions.TienDo.Duyet
            }
        };
    }

    public IReadOnlyList<PermissionDefinition> GetPermissionDefinitions() => _definitions;

    public PermissionDefinition GetPermissionDefinition(string permissionName, string? screenKey = null)
    {
        if (_definitionByPermission.TryGetValue(permissionName, out var definition))
        {
            return definition;
        }

        var fallbackScreenKey = string.IsNullOrWhiteSpace(screenKey)
            ? GetScreenKeyFromPermission(permissionName)
            : screenKey.Trim();
        var suffix = GetPermissionSuffix(permissionName);
        return new PermissionDefinition(
            permissionName,
            GetSuffixDisplayName(suffix),
            $"Quyền {GetSuffixDisplayName(suffix).ToLowerInvariant()} trên màn hình {fallbackScreenKey}.",
            fallbackScreenKey,
            GetScreenDisplayName(fallbackScreenKey),
            "Khac",
            "Khác",
            null,
            99,
            99,
            99);
    }

    public string? GetParentPermission(string permissionName)
        => _parentByChild.TryGetValue(permissionName, out var parent) ? parent : null;

    public IReadOnlyCollection<string> GetRequiredPermissionsForRole(string roleName)
        => _requiredByRole.TryGetValue(roleName, out var values)
            ? values
            : Array.Empty<string>();

    public IReadOnlyCollection<string> GetDeniedPermissionsForRole(string roleName)
        => _deniedByRole.TryGetValue(roleName, out var values)
            ? values
            : Array.Empty<string>();

    public HashSet<string> NormalizeDependencies(IEnumerable<string> permissionNames)
    {
        var normalized = permissionNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var changed = true;
        while (changed)
        {
            changed = false;
            foreach (var permissionName in normalized.ToList())
            {
                if (_parentByChild.TryGetValue(permissionName, out var parent)
                    && normalized.Add(parent))
                {
                    changed = true;
                }
            }
        }

        return normalized;
    }

    public List<string> ValidateDependencies(IEnumerable<string> permissionNames)
    {
        var selected = permissionNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();

        foreach (var child in selected)
        {
            if (!_parentByChild.TryGetValue(child, out var parent) || selected.Contains(parent))
            {
                continue;
            }

            var childDisplay = GetPermissionDefinition(child).DisplayName;
            var parentDisplay = GetPermissionDefinition(parent).DisplayName;
            errors.Add($"Quyền {childDisplay} cần quyền {parentDisplay}.");
        }

        return errors;
    }

    public List<string> ValidateRoleConstraints(string roleName, IEnumerable<string> permissionNames)
    {
        var selected = permissionNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var errors = new List<string>();

        foreach (var required in GetRequiredPermissionsForRole(roleName))
        {
            if (!selected.Contains(required))
            {
                errors.Add($"Role {roleName} bắt buộc giữ quyền {GetPermissionDefinition(required).DisplayName}.");
            }
        }

        foreach (var denied in GetDeniedPermissionsForRole(roleName))
        {
            if (selected.Contains(denied))
            {
                errors.Add($"Role {roleName} không được cấp quyền {GetPermissionDefinition(denied).DisplayName}.");
            }
        }

        return errors;
    }

    private static IReadOnlyList<PermissionDefinition> BuildDefinitions()
    {
        var definitions = new List<PermissionDefinition>();

        Add(definitions, "HeThong", "Hệ thống", 1, "NhanSu", "Nhân sự", 1, Permissions.NhanSu.Xem, "Xem", null, 1);
        Add(definitions, "HeThong", "Hệ thống", 1, "NhanSu", "Nhân sự", 1, Permissions.NhanSu.Them, "Thêm", Permissions.NhanSu.Xem, 2);
        Add(definitions, "HeThong", "Hệ thống", 1, "NhanSu", "Nhân sự", 1, Permissions.NhanSu.Sua, "Sửa", Permissions.NhanSu.Xem, 3);
        Add(definitions, "HeThong", "Hệ thống", 1, "NhanSu", "Nhân sự", 1, Permissions.NhanSu.Xoa, "Xóa", Permissions.NhanSu.Xem, 4);
        Add(definitions, "HeThong", "Hệ thống", 1, "NhanSu", "Nhân sự", 1, Permissions.NhanSu.Khoa, "Khóa", Permissions.NhanSu.Xem, 5);
        Add(definitions, "HeThong", "Hệ thống", 1, "NhanSu", "Nhân sự", 1, Permissions.NhanSu.MoKhoa, "Mở khóa", Permissions.NhanSu.Xem, 6);

        AddCrud(definitions, "HeThong", "Hệ thống", 1, "ChucDanh", "Chức danh", 2, Permissions.ChucDanh.Xem, Permissions.ChucDanh.Them, Permissions.ChucDanh.Sua, Permissions.ChucDanh.Xoa);
        Add(definitions, "HeThong", "Hệ thống", 1, "PhanQuyen", "Phân quyền", 3, Permissions.PhanQuyen.Xem, "Xem", null, 1);
        Add(definitions, "HeThong", "Hệ thống", 1, "PhanQuyen", "Phân quyền", 3, Permissions.PhanQuyen.Luu, "Lưu cấu hình", Permissions.PhanQuyen.Xem, 2);
        AddCrud(definitions, "HeThong", "Hệ thống", 1, "Nhom", "Nhóm", 4, Permissions.Nhom.Xem, Permissions.Nhom.Them, Permissions.Nhom.Sua, Permissions.Nhom.Xoa);
        Add(definitions, "HeThong", "Hệ thống", 1, "ThanhVienNhom", "Thành viên nhóm", 5, Permissions.ThanhVienNhom.Xem, "Xem", null, 1);
        Add(definitions, "HeThong", "Hệ thống", 1, "ThanhVienNhom", "Thành viên nhóm", 5, Permissions.ThanhVienNhom.Them, "Thêm", Permissions.ThanhVienNhom.Xem, 2);
        Add(definitions, "HeThong", "Hệ thống", 1, "ThanhVienNhom", "Thành viên nhóm", 5, Permissions.ThanhVienNhom.Xoa, "Xóa", Permissions.ThanhVienNhom.Xem, 3);

        AddCrud(definitions, "DuAn", "Dự án", 2, "DuAn", "Dự án", 1, Permissions.DuAn.Xem, Permissions.DuAn.Them, Permissions.DuAn.Sua, Permissions.DuAn.Xoa);
        Add(definitions, "DuAn", "Dự án", 2, "YeuCauDoiQuanLy", "Yêu cầu đổi quản lý", 2, Permissions.YeuCauDoiQuanLy.Xem, "Xem", null, 1);
        Add(definitions, "DuAn", "Dự án", 2, "YeuCauDoiQuanLy", "Yêu cầu đổi quản lý", 2, Permissions.YeuCauDoiQuanLy.Them, "Thêm yêu cầu", Permissions.YeuCauDoiQuanLy.Xem, 2);
        Add(definitions, "DuAn", "Dự án", 2, "DuyetYeuCauDoiQuanLy", "Duyệt yêu cầu đổi quản lý", 3, Permissions.DuyetYeuCauDoiQuanLy.Xem, "Xem", null, 1);
        Add(definitions, "DuAn", "Dự án", 2, "DuyetYeuCauDoiQuanLy", "Duyệt yêu cầu đổi quản lý", 3, Permissions.DuyetYeuCauDoiQuanLy.Duyet, "Duyệt", Permissions.DuyetYeuCauDoiQuanLy.Xem, 2);
        Add(definitions, "DuAn", "Dự án", 2, "TeamDuAn", "Team dự án", 4, Permissions.TeamDuAn.Xem, "Xem", null, 1);
        Add(definitions, "DuAn", "Dự án", 2, "TeamDuAn", "Team dự án", 4, Permissions.TeamDuAn.Them, "Thêm", Permissions.TeamDuAn.Xem, 2);
        Add(definitions, "DuAn", "Dự án", 2, "TeamDuAn", "Team dự án", 4, Permissions.TeamDuAn.Xoa, "Xóa", Permissions.TeamDuAn.Xem, 3);
        Add(definitions, "DuAn", "Dự án", 2, "ThanhVienDuAn", "Thành viên dự án", 5, Permissions.ThanhVienDuAn.Xem, "Xem", null, 1);
        Add(definitions, "DuAn", "Dự án", 2, "ThanhVienDuAn", "Thành viên dự án", 5, Permissions.ThanhVienDuAn.Them, "Thêm", Permissions.ThanhVienDuAn.Xem, 2);
        Add(definitions, "DuAn", "Dự án", 2, "ThanhVienDuAn", "Thành viên dự án", 5, Permissions.ThanhVienDuAn.Xoa, "Xóa", Permissions.ThanhVienDuAn.Xem, 3);

        Add(definitions, "CongViec", "Công việc", 3, "DanhMucCongViec", "Danh mục công việc", 1, Permissions.DanhMucCongViec.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "DanhMucCongViec", "Danh mục công việc", 1, Permissions.DanhMucCongViec.Them, "Thêm", Permissions.DanhMucCongViec.Xem, 2);
        Add(definitions, "CongViec", "Công việc", 3, "DanhMucCongViec", "Danh mục công việc", 1, Permissions.DanhMucCongViec.Sua, "Sửa", Permissions.DanhMucCongViec.Xem, 3);
        Add(definitions, "CongViec", "Công việc", 3, "DanhMucCongViec", "Danh mục công việc", 1, Permissions.DanhMucCongViec.Xoa, "Xóa", Permissions.DanhMucCongViec.Xem, 4);
        Add(definitions, "CongViec", "Công việc", 3, "CongViec", "Công việc", 2, Permissions.CongViec.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "ChiTietCongViec", "Chi tiết công việc", 3, Permissions.ChiTietCongViec.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "ChiTietCongViec", "Chi tiết công việc", 3, Permissions.ChiTietCongViec.Them, "Thêm", Permissions.ChiTietCongViec.Xem, 2);
        Add(definitions, "CongViec", "Công việc", 3, "ChiTietCongViec", "Chi tiết công việc", 3, Permissions.ChiTietCongViec.Sua, "Sửa", Permissions.ChiTietCongViec.Xem, 3);
        Add(definitions, "CongViec", "Công việc", 3, "ChiTietCongViec", "Chi tiết công việc", 3, Permissions.ChiTietCongViec.Xoa, "Xóa", Permissions.ChiTietCongViec.Xem, 4);
        Add(definitions, "CongViec", "Công việc", 3, "PhanCongCongViec", "Phân công công việc", 4, Permissions.PhanCongCongViec.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "PhanCongCongViec", "Phân công công việc", 4, Permissions.PhanCongCongViec.ThucHien, "Thực hiện", Permissions.PhanCongCongViec.Xem, 2);
        Add(definitions, "CongViec", "Công việc", 3, "PhanCongChiTietCongViec", "Phân công chi tiết công việc", 5, Permissions.PhanCongChiTietCongViec.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "PhanCongChiTietCongViec", "Phân công chi tiết công việc", 5, Permissions.PhanCongChiTietCongViec.ThucHien, "Thực hiện", Permissions.PhanCongChiTietCongViec.Xem, 2);
        Add(definitions, "CongViec", "Công việc", 3, "DeXuatCongViec", "Đề xuất công việc", 6, Permissions.DeXuatCongViec.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "DeXuatCongViec", "Đề xuất công việc", 6, Permissions.DeXuatCongViec.Them, "Thêm", Permissions.DeXuatCongViec.Xem, 2);
        Add(definitions, "CongViec", "Công việc", 3, "DuyetDeXuatCongViec", "Duyệt đề xuất công việc", 7, Permissions.DuyetDeXuatCongViec.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "DuyetDeXuatCongViec", "Duyệt đề xuất công việc", 7, Permissions.DuyetDeXuatCongViec.Duyet, "Duyệt", Permissions.DuyetDeXuatCongViec.Xem, 2);
        Add(definitions, "CongViec", "Công việc", 3, "TienDo", "Tiến độ", 8, Permissions.TienDo.Xem, "Xem", null, 1);
        Add(definitions, "CongViec", "Công việc", 3, "TienDo", "Tiến độ", 8, Permissions.TienDo.CapNhat, "Cập nhật", Permissions.TienDo.Xem, 2);
        Add(definitions, "CongViec", "Công việc", 3, "TienDo", "Tiến độ", 8, Permissions.TienDo.Duyet, "Duyệt", Permissions.TienDo.Xem, 3);

        Add(definitions, "TaiChinh", "Tài chính", 4, "DeXuatNganSach", "Đề xuất ngân sách", 1, Permissions.DeXuatNganSach.Xem, "Xem", null, 1);
        Add(definitions, "TaiChinh", "Tài chính", 4, "DeXuatNganSach", "Đề xuất ngân sách", 1, Permissions.DeXuatNganSach.Them, "Thêm", Permissions.DeXuatNganSach.Xem, 2);
        Add(definitions, "TaiChinh", "Tài chính", 4, "DuyetNganSach", "Duyệt ngân sách", 2, Permissions.DuyetNganSach.Xem, "Xem", null, 1);
        Add(definitions, "TaiChinh", "Tài chính", 4, "DuyetNganSach", "Duyệt ngân sách", 2, Permissions.DuyetNganSach.Duyet, "Duyệt", Permissions.DuyetNganSach.Xem, 2);
        Add(definitions, "TaiChinh", "Tài chính", 4, "NganSach", "Ngân sách", 3, Permissions.NganSach.Xem, "Xem", null, 1);
        Add(definitions, "TaiChinh", "Tài chính", 4, "ChiPhi", "Chi phí", 4, Permissions.ChiPhi.Xem, "Xem", null, 1);

        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaDuAn", "Đánh giá dự án", 1, Permissions.DanhGiaDuAn.Xem, "Xem", null, 1);
        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaDuAn", "Đánh giá dự án", 1, Permissions.DanhGiaDuAn.DanhGia, "Đánh giá", Permissions.DanhGiaDuAn.Xem, 2);
        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaDuAn", "Đánh giá dự án", 1, Permissions.DanhGiaDuAn.Sua, "Sửa", Permissions.DanhGiaDuAn.Xem, 3);
        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaDuAn", "Đánh giá dự án", 1, Permissions.DanhGiaDuAn.Duyet, "Duyệt", Permissions.DanhGiaDuAn.Xem, 4);
        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaNhanVien", "Đánh giá nhân viên", 2, Permissions.DanhGiaNhanVien.Xem, "Xem", null, 1);
        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaNhanVien", "Đánh giá nhân viên", 2, Permissions.DanhGiaNhanVien.DanhGia, "Đánh giá", Permissions.DanhGiaNhanVien.Xem, 2);
        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaNhanVien", "Đánh giá nhân viên", 2, Permissions.DanhGiaNhanVien.Sua, "Sửa", Permissions.DanhGiaNhanVien.Xem, 3);
        Add(definitions, "DanhGia", "Đánh giá", 5, "DanhGiaNhanVien", "Đánh giá nhân viên", 2, Permissions.DanhGiaNhanVien.Duyet, "Duyệt", Permissions.DanhGiaNhanVien.Xem, 4);

        Add(definitions, "Chat", "Chat", 6, "Chat", "Chat dự án", 1, Permissions.Chat.Xem, "Xem", null, 1);
        Add(definitions, "Chat", "Chat", 6, "Chat", "Chat dự án", 1, Permissions.Chat.Gui, "Gửi", Permissions.Chat.Xem, 2);

        Add(definitions, "AI", "AI", 7, "AI", "AI", 1, Permissions.AI.Xem, "Xem AI", null, 1);
        Add(definitions, "AI", "AI", 7, "AIDashboard", "Tổng quan AI", 2, Permissions.AI.Dashboard, "Xem tổng quan AI", Permissions.AI.Xem, 1);
        Add(definitions, "AI", "AI", 7, "AIDataset", "Dữ liệu AI", 3, Permissions.AI.Dataset, "Quản lý dữ liệu AI", Permissions.AI.Xem, 1);
        Add(definitions, "AI", "AI", 7, "AITrain", "Huấn luyện AI", 4, Permissions.AI.Train, "Huấn luyện AI", Permissions.AI.Xem, 1);
        Add(definitions, "AI", "AI", 7, "AIPredict", "Phân tích nguyên nhân trễ", 5, Permissions.AI.PhanTichNguyenNhan, "Phân tích", Permissions.AI.Xem, 1);
        Add(definitions, "AI", "AI", 7, "AIXacNhan", "Xác nhận nguyên nhân trễ", 6, Permissions.AI.XacNhan, "Xác nhận", Permissions.AI.Xem, 1);

        Add(definitions, "BaoCao", "Báo cáo", 8, "Dashboard", "Thống kê", 1, Permissions.ThongKe.Xem, "Xem", null, 1);
        Add(definitions, "BaoCao", "Báo cáo", 8, "Dashboard", "Thống kê", 1, Permissions.ThongKe.XuatFile, "Xuất file", Permissions.ThongKe.Xem, 2);
        Add(definitions, "BaoCao", "Báo cáo", 8, "NhatKy", "Nhật ký", 2, Permissions.NhatKy.Xem, "Xem", null, 1);

        return definitions;
    }

    private static void AddCrud(
        List<PermissionDefinition> definitions,
        string groupKey,
        string groupName,
        int groupOrder,
        string screenKey,
        string screenDisplayName,
        int screenOrder,
        string xem,
        string them,
        string sua,
        string xoa)
    {
        Add(definitions, groupKey, groupName, groupOrder, screenKey, screenDisplayName, screenOrder, xem, "Xem", null, 1);
        Add(definitions, groupKey, groupName, groupOrder, screenKey, screenDisplayName, screenOrder, them, "Thêm", xem, 2);
        Add(definitions, groupKey, groupName, groupOrder, screenKey, screenDisplayName, screenOrder, sua, "Sửa", xem, 3);
        Add(definitions, groupKey, groupName, groupOrder, screenKey, screenDisplayName, screenOrder, xoa, "Xóa", xem, 4);
    }

    private static void Add(
        List<PermissionDefinition> definitions,
        string groupKey,
        string groupName,
        int groupOrder,
        string screenKey,
        string screenDisplayName,
        int screenOrder,
        string permissionName,
        string displayName,
        string? parentPermission,
        int permissionOrder)
    {
        var description = parentPermission is null
            ? $"Cho phép {displayName.ToLowerInvariant()} màn hình {screenDisplayName}."
            : $"Cho phép {displayName.ToLowerInvariant()} trên màn hình {screenDisplayName}. Cần quyền Xem.";

        definitions.Add(new PermissionDefinition(
            permissionName,
            displayName,
            description,
            screenKey,
            screenDisplayName,
            groupKey,
            groupName,
            parentPermission,
            groupOrder,
            screenOrder,
            permissionOrder));
    }

    private static string GetScreenKeyFromPermission(string permissionName)
    {
        var dotIndex = permissionName.IndexOf('.');
        return dotIndex > 0 ? permissionName[..dotIndex] : permissionName;
    }

    private static string GetPermissionSuffix(string permissionName)
    {
        var dotIndex = permissionName.LastIndexOf('.');
        return dotIndex >= 0 && dotIndex < permissionName.Length - 1
            ? permissionName[(dotIndex + 1)..]
            : permissionName;
    }

    private static string GetSuffixDisplayName(string suffix)
        => suffix switch
        {
            "Xem" => "Xem",
            "Them" => "Thêm",
            "Sua" => "Sửa",
            "Xoa" => "Xóa",
            "Duyet" => "Duyệt",
            "Luu" => "Lưu cấu hình",
            "CapNhat" => "Cập nhật",
            "Gui" => "Gửi",
            "XacNhan" => "Xác nhận",
            "Khoa" => "Khóa",
            "MoKhoa" => "Mở khóa",
            "ThucHien" => "Thực hiện",
            "DanhGia" => "Đánh giá",
            "XuatFile" => "Xuất file",
            "Dataset" => "Quản lý dữ liệu AI",
            "Train" => "Huấn luyện AI",
            "PhanTichNguyenNhan" => "Phân tích nguyên nhân trễ",
            "Dashboard" => "Xem tổng quan AI",
            _ => suffix
        };

    private static string GetScreenDisplayName(string screenKey)
        => screenKey switch
        {
            "Dashboard" => "Thống kê",
            "AIDashboard" => "Tổng quan AI",
            "AIDataset" => "Dữ liệu AI",
            "AITrain" => "Huấn luyện AI",
            "AIPredict" => "Phân tích nguyên nhân trễ",
            "AIXacNhan" => "Xác nhận nguyên nhân trễ",
            "NhanSu" => "Nhân sự",
            "ChucDanh" => "Chức danh",
            "PhanQuyen" => "Phân quyền",
            "Nhom" => "Nhóm",
            "ThanhVienNhom" => "Thành viên nhóm",
            "DuAn" => "Dự án",
            "YeuCauDoiQuanLy" => "Yêu cầu đổi quản lý",
            "DuyetYeuCauDoiQuanLy" => "Duyệt yêu cầu đổi quản lý",
            "TeamDuAn" => "Team dự án",
            "ThanhVienDuAn" => "Thành viên dự án",
            "DanhMucCongViec" => "Danh mục công việc",
            "CongViec" => "Công việc",
            "ChiTietCongViec" => "Chi tiết công việc",
            "PhanCongCongViec" => "Phân công công việc",
            "PhanCongChiTietCongViec" => "Phân công chi tiết công việc",
            "DeXuatCongViec" => "Đề xuất công việc",
            "DuyetDeXuatCongViec" => "Duyệt đề xuất công việc",
            "TienDo" => "Tiến độ",
            "DeXuatNganSach" => "Đề xuất ngân sách",
            "DuyetNganSach" => "Duyệt ngân sách",
            "NganSach" => "Ngân sách",
            "ChiPhi" => "Chi phí",
            "DanhGiaDuAn" => "Đánh giá dự án",
            "DanhGiaNhanVien" => "Đánh giá nhân viên",
            "Chat" => "Chat dự án",
            "NhatKy" => "Nhật ký",
            "AI" => "AI",
            _ => screenKey
        };
}
