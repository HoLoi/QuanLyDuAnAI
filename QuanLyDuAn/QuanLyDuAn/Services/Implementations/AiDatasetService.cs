using System.Globalization;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Constants;
using QuanLyDuAn.Data;
using QuanLyDuAn.Models.Entities;
using QuanLyDuAn.Services.Interfaces;
using QuanLyDuAn.ViewModels.Ai;

namespace QuanLyDuAn.Services.Implementations
{
    public class AiDatasetService : IAiDatasetService
    {
        private const int MinReasonTrainRows = 30;
        private const int MinReasonClasses = 2;
        private const int MinReasonRowsPerClass = 5;
        private const double TyLeCongViecTreCanhBao = 30d;

        private readonly QuanLyDuAnDbContext _context;
        private readonly ILogger<AiDatasetService> _logger;

        public AiDatasetService(QuanLyDuAnDbContext context, ILogger<AiDatasetService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AiDatasetPageViewModel> KhoiTaoTrangDatasetAsync(
            bool dangThieuDataset,
            bool baoGomBaoCaoChatLuong,
            CancellationToken cancellationToken = default)
        {
            var vm = new AiDatasetPageViewModel
            {
                DangThieuDataset = dangThieuDataset
            };

            var duAnRows = await _context.DuAn
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.TenDuAn)
                .Select(x => new
                {
                    x.MaDuAn,
                    x.TenDuAn
                })
                .ToListAsync(cancellationToken);
            vm.DanhSachDuAn = duAnRows
                .Select(x => new AiDatasetProjectOptionViewModel
                {
                    MaDuAn = x.MaDuAn,
                    TenDuAn = string.IsNullOrWhiteSpace(x.TenDuAn) ? $"Dự án {x.MaDuAn}" : x.TenDuAn
                })
                .OrderBy(x => x.TenDuAn)
                .ToList();

            vm.TongSoDongDataset = await _context.AiDataset.CountAsync(cancellationToken);
            vm.SoDongDuLabel = await _context.AiDataset.CountAsync(
                x => x.LaDuAnTre == true && x.MaDMNguyenNhan.HasValue,
                cancellationToken);
            vm.SoDongThieuLabel = vm.TongSoDongDataset - vm.SoDongDuLabel;

            if (baoGomBaoCaoChatLuong)
            {
                vm.ChatLuongDatasetDb = await KiemTraChatLuongDatasetAsync(cancellationToken);
                vm.DaKiemTraChatLuongDataset = true;
                if (!vm.ChatLuongDatasetDb.DuDieuKienTrain && vm.ChatLuongDatasetDb.LyDoKhongDat.Count > 0)
                {
                    vm.CanhBao = "Dataset hiện có nhưng chưa đủ điều kiện train model nguyên nhân.";
                }
            }

            if (vm.TongSoDongDataset == 0)
            {
                vm.CanhBao ??= "Chưa có dữ liệu AI_DATASET. Vui lòng tổng hợp dataset trước.";
            }

            return vm;
        }

        public async Task<AiDatasetTongHopResultViewModel> TongHopDatasetAsync(CancellationToken cancellationToken = default)
        {
            var trangThaiChoPhep = LayTrangThaiChoPhepTongHopAi();
            var projectIds = await _context.DuAn
                .Where(x =>
                    x.IsDeleted != true
                    && trangThaiChoPhep.Contains(x.TrangThaiDuAn ?? string.Empty))
                .Select(x => x.MaDuAn)
                .ToListAsync(cancellationToken);

            if (projectIds.Count == 0)
            {
                return new AiDatasetTongHopResultViewModel
                {
                    ThongBao = ["Không có dự án hợp lệ để tổng hợp AI_DATASET."]
                };
            }

            return await TongHopNoiBoAsync(projectIds, cancellationToken);
        }

        public async Task<AiDatasetTongHopResultViewModel> TongHopDatasetChoDuAnAsync(int maDuAn, CancellationToken cancellationToken = default)
        {
            if (maDuAn <= 0)
            {
                return new AiDatasetTongHopResultViewModel
                {
                    SoDuAnBoQua = 1,
                    ThongBao = ["Mã dự án không hợp lệ."]
                };
            }

            var duAn = await _context.DuAn
                .Where(x => x.IsDeleted != true && x.MaDuAn == maDuAn)
                .Select(x => new
                {
                    x.MaDuAn,
                    x.TrangThaiDuAn
                })
                .FirstOrDefaultAsync(cancellationToken);
            if (duAn is null)
            {
                return new AiDatasetTongHopResultViewModel
                {
                    SoDuAnBoQua = 1,
                    ThongBao = [$"Không tìm thấy dự án #{maDuAn}."]
                };
            }

            if (!LaTrangThaiDuAnChoPhepTongHopAi(duAn.TrangThaiDuAn))
            {
                return new AiDatasetTongHopResultViewModel
                {
                    SoDuAnBoQua = 1,
                    ThongBao = [$"Dự án #{maDuAn} chưa ở trạng thái Hoàn thành hoặc Lưu trữ nên không thể tổng hợp AI_DATASET."]
                };
            }

            return await TongHopNoiBoAsync([maDuAn], cancellationToken);
        }

        public async Task<AiProjectFeatureSnapshotViewModel?> BuildFeatureSnapshotAsync(int maDuAn, CancellationToken cancellationToken = default)
        {
            if (maDuAn <= 0)
            {
                return null;
            }

            var snapshots = await BuildFeatureSnapshotsAsync([maDuAn], chiNhanTrangThaiChinhThuc: false, cancellationToken);
            return snapshots.FirstOrDefault();
        }

        public async Task<AiDatasetQualitySummaryViewModel> KiemTraChatLuongDatasetAsync(CancellationToken cancellationToken = default)
        {
            var rows = await _context.AiDataset
                .Select(x => new
                {
                    x.LaDuAnTre,
                    x.MaDMNguyenNhan,
                    x.SoNhanVienDuAn,
                    x.TongSoCongViec,
                    x.SoCongViecTre,
                    x.TyLeCongViecTre,
                    x.ChiPhiDuKien,
                    x.ChiPhiThucTe,
                    x.ChenhLechChiPhi,
                    x.SoLanThayDoiNhanSu,
                    x.SoLanThayDoiQuanLy,
                    x.SoNgayTreTienDo,
                    x.SoDeXuatCongViecChoDuyet,
                    x.SoDeXuatCongViecBiTuChoi,
                    x.ThoiGianDuyetCongViecTrungBinh,
                    x.SoDeXuatNganSachChoDuyet,
                    x.SoDeXuatNganSachBiTuChoi,
                    x.ThoiGianDuyetNganSachTrungBinh,
                    x.SoBaoCaoTienDoChoDuyet,
                    x.SoBaoCaoTienDoBiTuChoi,
                    x.SoBaoCaoTienDoYeuCauBoSung,
                    x.TyLeBaoCaoTienDoBiTuChoi,
                    x.SoLanCapNhatTienDo,
                    x.SoNgayChamCapNhatTienDo
                })
                .ToListAsync(cancellationToken);

            var tongSoDong = rows.Count;
            var soMauDuAnTre = rows.Count(x => x.LaDuAnTre == true);
            var soMauKhongTre = rows.Count(x => x.LaDuAnTre == false);
            var soDongCoNguyenNhan = rows.Count(x => x.MaDMNguyenNhan.HasValue);
            var soDongDuLabel = rows.Count(x => x.LaDuAnTre == true && x.MaDMNguyenNhan.HasValue);
            var soDongThieuLabel = tongSoDong - soDongDuLabel;

            var soDongHopLeTrain = rows.Count(x =>
                x.LaDuAnTre == true
                && x.MaDMNguyenNhan.HasValue
                && x.SoNhanVienDuAn.HasValue
                && x.TongSoCongViec.HasValue
                && x.SoCongViecTre.HasValue
                && x.TyLeCongViecTre.HasValue
                && x.ChiPhiDuKien.HasValue
                && x.ChiPhiThucTe.HasValue
                && x.ChenhLechChiPhi.HasValue
                && x.SoLanThayDoiNhanSu.HasValue
                && x.SoLanThayDoiQuanLy.HasValue
                && x.SoNgayTreTienDo.HasValue
                && x.SoDeXuatCongViecChoDuyet.HasValue
                && x.SoDeXuatCongViecBiTuChoi.HasValue
                && x.ThoiGianDuyetCongViecTrungBinh.HasValue
                && x.SoDeXuatNganSachChoDuyet.HasValue
                && x.SoDeXuatNganSachBiTuChoi.HasValue
                && x.ThoiGianDuyetNganSachTrungBinh.HasValue
                && x.SoBaoCaoTienDoChoDuyet.HasValue
                && x.SoBaoCaoTienDoBiTuChoi.HasValue
                && x.SoBaoCaoTienDoYeuCauBoSung.HasValue
                && x.TyLeBaoCaoTienDoBiTuChoi.HasValue
                && x.SoLanCapNhatTienDo.HasValue
                && x.SoNgayChamCapNhatTienDo.HasValue);

            var phanBoTheoNguyenNhan = rows
                .Where(x => x.LaDuAnTre == true && x.MaDMNguyenNhan.HasValue)
                .GroupBy(x => x.MaDMNguyenNhan!.Value)
                .ToDictionary(x => x.Key, x => x.Count());

            var lyDoKhongDat = new List<string>();
            if (tongSoDong == 0)
            {
                lyDoKhongDat.Add("Dataset đang rỗng.");
            }

            if (soDongHopLeTrain < MinReasonTrainRows)
            {
                lyDoKhongDat.Add($"Số dòng hợp lệ train nguyên nhân là {soDongHopLeTrain}, nhỏ hơn {MinReasonTrainRows}.");
            }

            if (phanBoTheoNguyenNhan.Count < MinReasonClasses)
            {
                lyDoKhongDat.Add($"Cần ít nhất {MinReasonClasses} lớp nguyên nhân khác nhau.");
            }

            if (phanBoTheoNguyenNhan.Count > 0 && phanBoTheoNguyenNhan.Values.Min() < MinReasonRowsPerClass)
            {
                lyDoKhongDat.Add($"Mỗi nguyên nhân cần tối thiểu {MinReasonRowsPerClass} dòng.");
            }

            return new AiDatasetQualitySummaryViewModel
            {
                TongSoDong = tongSoDong,
                SoDongDuLabel = soDongDuLabel,
                SoDongThieuLabel = soDongThieuLabel,
                SoDongHopLeTrain = soDongHopLeTrain,
                SoMauDuAnTre = soMauDuAnTre,
                SoMauKhongTre = soMauKhongTre,
                SoDongCoNguyenNhan = soDongCoNguyenNhan,
                MinTrainRows = MinReasonTrainRows,
                DuDieuKienTrain = lyDoKhongDat.Count == 0,
                PhanBoTheoNguyenNhan = phanBoTheoNguyenNhan.ToDictionary(x => x.Key.ToString(CultureInfo.InvariantCulture), x => x.Value),
                LyDoKhongDat = lyDoKhongDat,
                GhiChuChatLuongDuLieu = []
            };
        }

        public Task<List<AiDatasetRowViewModel>> LayDatasetHopLeDeTrainAsync(CancellationToken cancellationToken = default)
            => LayDatasetNguyenNhanHopLeDeTrainAsync(cancellationToken);

        public async Task<AiReasonTrainingQualitySummaryViewModel> KiemTraChatLuongDatasetNguyenNhanAsync(CancellationToken cancellationToken = default)
        {
            var rows = await LayDatasetNguyenNhanHopLeDeTrainAsync(cancellationToken);
            var tongSoDong = rows.Count;
            var phanBo = rows
                .Where(x => x.MaDMNguyenNhan.HasValue)
                .GroupBy(x => x.MaDMNguyenNhan!.Value)
                .ToDictionary(x => x.Key, x => x.Count());

            var soLoai = phanBo.Count;
            var minRows = soLoai > 0 ? phanBo.Values.Min() : 0;
            var maxRows = soLoai > 0 ? phanBo.Values.Max() : 0;
            var imbalanceRatio = minRows > 0 ? Math.Round((double)maxRows / minRows, 4) : 0d;

            var lyDoKhongDat = new List<string>();
            if (tongSoDong < MinReasonTrainRows)
            {
                lyDoKhongDat.Add($"Số dòng hợp lệ train nguyên nhân là {tongSoDong}, nhỏ hơn {MinReasonTrainRows}.");
            }

            if (soLoai < MinReasonClasses)
            {
                lyDoKhongDat.Add($"Số loại nguyên nhân hiện có là {soLoai}, nhỏ hơn {MinReasonClasses}.");
            }

            if (soLoai > 0 && minRows < MinReasonRowsPerClass)
            {
                lyDoKhongDat.Add($"Mỗi nguyên nhân cần tối thiểu {MinReasonRowsPerClass} dòng, hiện nhỏ nhất là {minRows}.");
            }

            var ghiChu = new List<string>();
            if (imbalanceRatio >= 3d)
            {
                ghiChu.Add($"Dataset bị lệch lớp (imbalance ratio={imbalanceRatio:0.##}).");
            }

            return new AiReasonTrainingQualitySummaryViewModel
            {
                TongSoDongJoin = tongSoDong,
                SoDongHopLeTrain = tongSoDong,
                SoLoaiNguyenNhan = soLoai,
                MinTrainRows = MinReasonTrainRows,
                MinLoaiNguyenNhan = MinReasonClasses,
                MinDongMoiLoai = MinReasonRowsPerClass,
                DuDieuKienTrain = lyDoKhongDat.Count == 0,
                PhanBoTheoNguyenNhan = phanBo.ToDictionary(x => x.Key.ToString(CultureInfo.InvariantCulture), x => x.Value),
                ImbalanceRatio = imbalanceRatio,
                LyDoKhongDat = lyDoKhongDat,
                GhiChuChatLuongDuLieu = ghiChu
            };
        }

        public async Task<List<AiDatasetRowViewModel>> LayDatasetNguyenNhanHopLeDeTrainAsync(CancellationToken cancellationToken = default)
        {
            var rows = await _context.AiDataset
                .Where(x => x.LaDuAnTre == true && x.MaDMNguyenNhan.HasValue)
                .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaData)
                .Select(x => new AiDatasetRowViewModel
                {
                    MaDuAn = x.MaDuAn.ToString(CultureInfo.InvariantCulture),
                    SoNhanVienDuAn = x.SoNhanVienDuAn,
                    TongSoCongViec = x.TongSoCongViec,
                    SoCongViecTre = x.SoCongViecTre,
                    TyLeCongViecTre = x.TyLeCongViecTre,
                    ChiPhiDuKien = (double?)x.ChiPhiDuKien,
                    ChiPhiThucTe = (double?)x.ChiPhiThucTe,
                    ChenhLechChiPhi = (double?)x.ChenhLechChiPhi,
                    SoLanThayDoiNhanSu = x.SoLanThayDoiNhanSu,
                    SoLanThayDoiQuanLy = x.SoLanThayDoiQuanLy,
                    SoNgayTreTienDo = x.SoNgayTreTienDo,
                    SoDeXuatCongViecChoDuyet = x.SoDeXuatCongViecChoDuyet,
                    SoDeXuatCongViecBiTuChoi = x.SoDeXuatCongViecBiTuChoi,
                    ThoiGianDuyetCongViecTrungBinh = x.ThoiGianDuyetCongViecTrungBinh,
                    SoDeXuatNganSachChoDuyet = x.SoDeXuatNganSachChoDuyet,
                    SoDeXuatNganSachBiTuChoi = x.SoDeXuatNganSachBiTuChoi,
                    ThoiGianDuyetNganSachTrungBinh = x.ThoiGianDuyetNganSachTrungBinh,
                    SoBaoCaoTienDoChoDuyet = x.SoBaoCaoTienDoChoDuyet,
                    SoBaoCaoTienDoBiTuChoi = x.SoBaoCaoTienDoBiTuChoi,
                    SoBaoCaoTienDoYeuCauBoSung = x.SoBaoCaoTienDoYeuCauBoSung,
                    TyLeBaoCaoTienDoBiTuChoi = x.TyLeBaoCaoTienDoBiTuChoi,
                    SoLanCapNhatTienDo = x.SoLanCapNhatTienDo,
                    SoNgayChamCapNhatTienDo = x.SoNgayChamCapNhatTienDo,
                    LaDuAnTre = 1,
                    MaDMNguyenNhan = x.MaDMNguyenNhan,
                    NgayTongHop = x.NgayTongHop,
                    GhiChuDataset = x.GhiChuDataset
                })
                .ToListAsync(cancellationToken);

            return rows.Where(HasDuFeature).ToList();
        }

        private async Task<AiDatasetTongHopResultViewModel> TongHopNoiBoAsync(List<int> projectIds, CancellationToken cancellationToken)
        {
            var result = new AiDatasetTongHopResultViewModel
            {
                TongSoDuAnXuLy = projectIds.Count
            };

            if (projectIds.Count == 0)
            {
                result.ThongBao.Add("Không có dự án để tổng hợp.");
                return result;
            }

            var snapshots = await BuildFeatureSnapshotsAsync(projectIds, chiNhanTrangThaiChinhThuc: true, cancellationToken);
            var snapshotMap = snapshots.ToDictionary(x => x.MaDuAn);
            var duAnSet = projectIds.ToHashSet();
            var datasetRows = await _context.AiDataset
                .Where(x => duAnSet.Contains(x.MaDuAn))
                .OrderByDescending(x => x.NgayTongHop ?? DateTime.MinValue)
                .ThenByDescending(x => x.MaData)
                .ToListAsync(cancellationToken);
            var datasetMap = datasetRows.GroupBy(x => x.MaDuAn).ToDictionary(x => x.Key, x => x.First());

            foreach (var maDuAn in projectIds)
            {
                if (!snapshotMap.TryGetValue(maDuAn, out var snapshot))
                {
                    result.SoDuAnBoQua++;
                    continue;
                }

                if (datasetMap.TryGetValue(maDuAn, out var existing))
                {
                    GanSnapshotVaoDataset(existing, snapshot);
                    result.SoDuAnCapNhat++;
                    continue;
                }

                var created = new AiDataset();
                GanSnapshotVaoDataset(created, snapshot);
                _context.AiDataset.Add(created);
                result.SoDuAnTaoMoi++;
            }

            await _context.SaveChangesAsync(cancellationToken);
            result.ThongBao.Add("Đã tổng hợp dữ liệu nghiệp vụ vào AI_DATASET.");
            _logger.LogInformation(
                "Tong hop AI_DATASET xong. Xu ly {Tong}, tao moi {TaoMoi}, cap nhat {CapNhat}.",
                result.TongSoDuAnXuLy,
                result.SoDuAnTaoMoi,
                result.SoDuAnCapNhat);
            return result;
        }

        private async Task<List<AiProjectFeatureSnapshotViewModel>> BuildFeatureSnapshotsAsync(
            List<int> projectIds,
            bool chiNhanTrangThaiChinhThuc,
            CancellationToken cancellationToken)
        {
            if (projectIds.Count == 0)
            {
                return [];
            }

            var approvedStatuses = TrangThai.GetCommonStatusVariants(TrangThai.DaDuyet);
            var choDuyetStatuses = TrangThai.GetCommonStatusVariants(TrangThai.ChoDuyet);
            var tuChoiStatuses = TrangThai.GetCommonStatusVariants(TrangThai.TuChoi);
            var yeuCauBoSungStatuses = TrangThai.GetCommonStatusVariants(TrangThai.YeuCauBoSung);
            var duAnSet = projectIds.ToHashSet();
            var homNay = DateTime.Today;
            var thoiDiemTongHop = DateTime.Now;

            var soNhanVienTheoDuAn = await _context.NhanVienDuAn
                .Where(x => duAnSet.Contains(x.MaDuAn))
                .GroupBy(x => x.MaDuAn)
                .Select(x => new { x.Key, SoNhanVien = x.Select(v => v.MaNguoiDung).Distinct().Count() })
                .ToDictionaryAsync(x => x.Key, x => x.SoNhanVien, cancellationToken);

            var duAnSnapshots = await _context.DuAn
                .Where(x => duAnSet.Contains(x.MaDuAn) && x.IsDeleted != true)
                .Select(x => new { x.MaDuAn, x.TrangThaiDuAn, x.NgayKetThucDuAn, x.NgayHoanThanhThucTeDuAn, x.PhanTramHoanThanh })
                .ToDictionaryAsync(x => x.MaDuAn, x => x, cancellationToken);

            var congViecSnapshots = await (
                from cv in _context.CongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where cv.IsDeleted != true
                      && dm.IsDeleted != true
                      && duAnSet.Contains(dm.MaDuAn)
                select new
                {
                    dm.MaDuAn,
                    cv.TrangThaiCongViec,
                    cv.NgayKetThucCVDuKien,
                    cv.NgayKetThucCVThucTe
                }).ToListAsync(cancellationToken);

            var nganSachDuKienTheoDuAn = await _context.NganSach
                .Where(x =>
                    x.IsDeleted != true
                    && duAnSet.Contains(x.MaDuAn)
                    && x.IsActive == true
                    && approvedStatuses.Contains(x.TrangThaiNganSach ?? string.Empty))
                .GroupBy(x => x.MaDuAn)
                .Select(x => new { x.Key, TongTien = x.Sum(v => v.SoTienNganSach ?? 0m) })
                .ToDictionaryAsync(x => x.Key, x => x.TongTien, cancellationToken);

            var chiPhiTheoDuAn = await (
                from cp in _context.ChiPhi
                join ns in _context.NganSach on cp.MaNganSach equals ns.MaNganSach
                where cp.IsDeleted != true && ns.IsDeleted != true && duAnSet.Contains(ns.MaDuAn)
                group cp by ns.MaDuAn
                into g
                select new { g.Key, TongChiPhi = g.Sum(v => v.SoTienDaChi ?? 0m) }
            ).ToDictionaryAsync(x => x.Key, x => x.TongChiPhi, cancellationToken);

            var nhatKyPhuTrachTheoDuAn = await _context.NhatKyPhuTrachDuAn
                .Where(x => duAnSet.Contains(x.MaDuAn))
                .Select(x => new { x.MaDuAn, x.NkHanhDongPTDA })
                .ToListAsync(cancellationToken);

            var soLanThayDoiNhanSuTheoDuAn = nhatKyPhuTrachTheoDuAn
                .Where(x => LaHanhDongThayDoiNhanSu(x.NkHanhDongPTDA))
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(x => x.Key, x => x.Count());

            var yeuCauDoiQuanLyTheoDuAn = await _context.YeuCauDoiQuanLy
                .Where(x => x.IsDeleted != true && duAnSet.Contains(x.MaDuAn))
                .Select(x => new
                {
                    x.MaDuAn,
                    x.TrangThaiYeuCauDoiQuanLy,
                    x.NgayDuyetYeuCauDoiQuanLy,
                    x.MaQuanLyHienTai,
                    x.MaQuanLyDeXuat
                })
                .ToListAsync(cancellationToken);

            var soLanDoiQuanLyTheoDuAn = yeuCauDoiQuanLyTheoDuAn
                .Where(x =>
                    x.NgayDuyetYeuCauDoiQuanLy.HasValue
                    && x.MaQuanLyHienTai != x.MaQuanLyDeXuat
                    && approvedStatuses.Contains(x.TrangThaiYeuCauDoiQuanLy ?? string.Empty))
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(x => x.Key, x => x.Count());

            var deXuatCongViecTheoDuAn = await _context.DeXuatCongViec
                .Where(x => x.IsDeleted != true && duAnSet.Contains(x.MaDuAn))
                .Select(x => new
                {
                    x.MaDuAn,
                    x.TrangThaiCongViecDeXuat,
                    x.NgayDeXuatCongViec,
                    x.NgayDuyetDeXuatCongViec
                })
                .ToListAsync(cancellationToken);
            var deXuatCongViecMap = deXuatCongViecTheoDuAn
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(x => x.Key, x => x.ToList());

            var deXuatNganSachTheoDuAn = await _context.DeXuatNganSach
                .Where(x => x.IsDeleted != true && duAnSet.Contains(x.MaDuAn))
                .Select(x => new
                {
                    x.MaDuAn,
                    x.TrangThaiDeXuat,
                    x.NgayDeXuat,
                    x.NgayDuyet
                })
                .ToListAsync(cancellationToken);
            var deXuatNganSachMap = deXuatNganSachTheoDuAn
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(x => x.Key, x => x.ToList());

            var baoCaoTienDoTheoDuAn = await (
                from td in _context.TienDoCongViec
                join ct in _context.CtCongViec on td.MaChiTietCV equals ct.MaChiTietCV
                join cv in _context.CongViec on ct.MaCongViec equals cv.MaCongViec
                join dm in _context.DanhMucCongViec on cv.MaDanhMucCV equals dm.MaDanhMucCV
                where ct.IsDeleted != true
                      && cv.IsDeleted != true
                      && dm.IsDeleted != true
                      && duAnSet.Contains(dm.MaDuAn)
                select new
                {
                    dm.MaDuAn,
                    td.TrangThaiTienDo,
                    td.ThoiGianCapNhat
                }).ToListAsync(cancellationToken);
            var baoCaoTienDoMap = baoCaoTienDoTheoDuAn
                .GroupBy(x => x.MaDuAn)
                .ToDictionary(x => x.Key, x => x.ToList());

            var lyDoDaXacNhanTheoDuAn = await _context.AiNguyenNhan
                .Where(x => x.IsDeleted != true && duAnSet.Contains(x.MaDuAn))
                .GroupBy(x => x.MaDuAn)
                .Select(g => g.OrderByDescending(x => x.NgayXacNhan ?? DateTime.MinValue).ThenByDescending(x => x.MaAINguyenNhan).First())
                .ToDictionaryAsync(x => x.MaDuAn, x => x.MaDMNguyenNhan, cancellationToken);

            var snapshots = new List<AiProjectFeatureSnapshotViewModel>();
            foreach (var maDuAn in projectIds)
            {
                if (!duAnSnapshots.TryGetValue(maDuAn, out var duAnSnapshot))
                {
                    continue;
                }

                if (chiNhanTrangThaiChinhThuc && !LaTrangThaiDuAnChoPhepTongHopAi(duAnSnapshot.TrangThaiDuAn))
                {
                    continue;
                }

                var congViecs = congViecSnapshots.Where(x => x.MaDuAn == maDuAn).ToList();
                var tongSoCongViec = congViecs.Count;
                var soCongViecTre = congViecs.Count(x =>
                {
                    if (!x.NgayKetThucCVDuKien.HasValue)
                    {
                        return false;
                    }

                    var ngayDuKien = x.NgayKetThucCVDuKien.Value.Date;
                    if (TrangThai.LaHoanThanhCongViec(x.TrangThaiCongViec))
                    {
                        return x.NgayKetThucCVThucTe.HasValue && x.NgayKetThucCVThucTe.Value.Date > ngayDuKien;
                    }

                    return homNay > ngayDuKien;
                });

                var tyLeCongViecTre = tongSoCongViec > 0
                    ? Math.Round((double)soCongViecTre * 100d / tongSoCongViec, 2)
                    : 0d;

                var duAnDaHoanThanhCoNgayThucTe = TrangThai.LaHoanThanhCongViec(duAnSnapshot.TrangThaiDuAn)
                    && duAnSnapshot.NgayKetThucDuAn.HasValue
                    && duAnSnapshot.NgayHoanThanhThucTeDuAn.HasValue;

                var soNgayTreTienDo = duAnDaHoanThanhCoNgayThucTe
                    ? Math.Max(0, (duAnSnapshot.NgayHoanThanhThucTeDuAn!.Value.Date - duAnSnapshot.NgayKetThucDuAn!.Value.Date).Days)
                    : congViecs
                        .Where(x => x.NgayKetThucCVDuKien.HasValue)
                        .Select(x =>
                        {
                            var mocSoSanh = TrangThai.LaHoanThanhCongViec(x.TrangThaiCongViec)
                                ? (x.NgayKetThucCVThucTe?.Date ?? x.NgayKetThucCVDuKien!.Value.Date)
                                : homNay;
                            return Math.Max(0, (mocSoSanh - x.NgayKetThucCVDuKien!.Value.Date).Days);
                        })
                        .DefaultIfEmpty(0)
                        .Max();

                deXuatCongViecMap.TryGetValue(maDuAn, out var deXuatCongViecCuaDuAn);
                deXuatCongViecCuaDuAn ??= [];
                var soDeXuatCongViecChoDuyet = deXuatCongViecCuaDuAn.Count(x =>
                    choDuyetStatuses.Contains(x.TrangThaiCongViecDeXuat ?? string.Empty));
                var soDeXuatCongViecBiTuChoi = deXuatCongViecCuaDuAn.Count(x =>
                    tuChoiStatuses.Contains(x.TrangThaiCongViecDeXuat ?? string.Empty));
                var thoiGianDuyetCongViecTrungBinh = Math.Round(deXuatCongViecCuaDuAn
                    .Where(x => x.NgayDeXuatCongViec.HasValue && x.NgayDuyetDeXuatCongViec.HasValue)
                    .Select(x => Math.Max(0d, (x.NgayDuyetDeXuatCongViec!.Value - x.NgayDeXuatCongViec!.Value).TotalDays))
                    .DefaultIfEmpty(0d)
                    .Average(), 2);

                deXuatNganSachMap.TryGetValue(maDuAn, out var deXuatNganSachCuaDuAn);
                deXuatNganSachCuaDuAn ??= [];
                var soDeXuatNganSachChoDuyet = deXuatNganSachCuaDuAn.Count(x =>
                    choDuyetStatuses.Contains(x.TrangThaiDeXuat ?? string.Empty));
                var soDeXuatNganSachBiTuChoi = deXuatNganSachCuaDuAn.Count(x =>
                    tuChoiStatuses.Contains(x.TrangThaiDeXuat ?? string.Empty));
                var thoiGianDuyetNganSachTrungBinh = Math.Round(deXuatNganSachCuaDuAn
                    .Where(x => x.NgayDeXuat.HasValue && x.NgayDuyet.HasValue)
                    .Select(x => Math.Max(0d, (x.NgayDuyet!.Value - x.NgayDeXuat!.Value).TotalDays))
                    .DefaultIfEmpty(0d)
                    .Average(), 2);

                baoCaoTienDoMap.TryGetValue(maDuAn, out var baoCaoTienDoCuaDuAn);
                baoCaoTienDoCuaDuAn ??= [];
                var soLanCapNhatTienDo = baoCaoTienDoCuaDuAn.Count;
                var soBaoCaoTienDoChoDuyet = baoCaoTienDoCuaDuAn.Count(x =>
                    choDuyetStatuses.Contains(x.TrangThaiTienDo ?? string.Empty));
                var soBaoCaoTienDoBiTuChoi = baoCaoTienDoCuaDuAn.Count(x =>
                    tuChoiStatuses.Contains(x.TrangThaiTienDo ?? string.Empty));
                var soBaoCaoTienDoYeuCauBoSung = baoCaoTienDoCuaDuAn.Count(x =>
                    yeuCauBoSungStatuses.Contains(x.TrangThaiTienDo ?? string.Empty));
                var tyLeBaoCaoTienDoBiTuChoi = soLanCapNhatTienDo > 0
                    ? Math.Round((double)soBaoCaoTienDoBiTuChoi * 100d / soLanCapNhatTienDo, 2)
                    : 0d;
                var mocChamCapNhatTienDo = duAnSnapshot.NgayKetThucDuAn?.Date ?? homNay;
                var lanCapNhatTienDoGanNhat = baoCaoTienDoCuaDuAn
                    .Where(x => x.ThoiGianCapNhat.HasValue)
                    .Select(x => x.ThoiGianCapNhat!.Value.Date)
                    .DefaultIfEmpty(mocChamCapNhatTienDo)
                    .Max();
                var soNgayChamCapNhatTienDo = Math.Max(0, (mocChamCapNhatTienDo - lanCapNhatTienDoGanNhat).Days);

                var laDuAnTre = XacDinhDuAnTre(
                    duAnSnapshot.TrangThaiDuAn,
                    duAnSnapshot.PhanTramHoanThanh,
                    duAnSnapshot.NgayKetThucDuAn,
                    duAnSnapshot.NgayHoanThanhThucTeDuAn,
                    homNay,
                    soCongViecTre,
                    tyLeCongViecTre,
                    soNgayTreTienDo);

                var maDmNguyenNhan = laDuAnTre && lyDoDaXacNhanTheoDuAn.TryGetValue(maDuAn, out var maLyDoXacNhan)
                    ? maLyDoXacNhan
                    : (int?)null;

                var chiPhiDuKien = nganSachDuKienTheoDuAn.GetValueOrDefault(maDuAn, 0m);
                var chiPhiThucTe = chiPhiTheoDuAn.GetValueOrDefault(maDuAn, 0m);
                snapshots.Add(new AiProjectFeatureSnapshotViewModel
                {
                    MaDuAn = maDuAn,
                    TrangThaiDuAn = duAnSnapshot.TrangThaiDuAn,
                    PhanTramHoanThanh = duAnSnapshot.PhanTramHoanThanh,
                    NgayKetThucDuAn = duAnSnapshot.NgayKetThucDuAn,
                    NgayHoanThanhThucTeDuAn = duAnSnapshot.NgayHoanThanhThucTeDuAn,
                    ThoiDiemTongHop = thoiDiemTongHop,
                    LaDuAnTre = laDuAnTre,
                    MaDMNguyenNhan = maDmNguyenNhan,
                    SoNhanVienDuAn = soNhanVienTheoDuAn.GetValueOrDefault(maDuAn, 0),
                    TongSoCongViec = tongSoCongViec,
                    SoCongViecTre = soCongViecTre,
                    TyLeCongViecTre = tyLeCongViecTre,
                    ChiPhiDuKien = chiPhiDuKien,
                    ChiPhiThucTe = chiPhiThucTe,
                    ChenhLechChiPhi = chiPhiThucTe - chiPhiDuKien,
                    SoLanThayDoiNhanSu = soLanThayDoiNhanSuTheoDuAn.GetValueOrDefault(maDuAn, 0),
                    SoLanThayDoiQuanLy = soLanDoiQuanLyTheoDuAn.GetValueOrDefault(maDuAn, 0),
                    SoNgayTreTienDo = soNgayTreTienDo,
                    SoDeXuatCongViecChoDuyet = soDeXuatCongViecChoDuyet,
                    SoDeXuatCongViecBiTuChoi = soDeXuatCongViecBiTuChoi,
                    ThoiGianDuyetCongViecTrungBinh = thoiGianDuyetCongViecTrungBinh,
                    SoDeXuatNganSachChoDuyet = soDeXuatNganSachChoDuyet,
                    SoDeXuatNganSachBiTuChoi = soDeXuatNganSachBiTuChoi,
                    ThoiGianDuyetNganSachTrungBinh = thoiGianDuyetNganSachTrungBinh,
                    SoBaoCaoTienDoChoDuyet = soBaoCaoTienDoChoDuyet,
                    SoBaoCaoTienDoBiTuChoi = soBaoCaoTienDoBiTuChoi,
                    SoBaoCaoTienDoYeuCauBoSung = soBaoCaoTienDoYeuCauBoSung,
                    TyLeBaoCaoTienDoBiTuChoi = tyLeBaoCaoTienDoBiTuChoi,
                    SoLanCapNhatTienDo = soLanCapNhatTienDo,
                    SoNgayChamCapNhatTienDo = soNgayChamCapNhatTienDo
                });
            }

            return snapshots;
        }

        private static void GanSnapshotVaoDataset(AiDataset dataset, AiProjectFeatureSnapshotViewModel snapshot)
        {
            dataset.MaDuAn = snapshot.MaDuAn;
            dataset.SoNhanVienDuAn = snapshot.SoNhanVienDuAn;
            dataset.TongSoCongViec = snapshot.TongSoCongViec;
            dataset.SoCongViecTre = snapshot.SoCongViecTre;
            dataset.TyLeCongViecTre = snapshot.TyLeCongViecTre;
            dataset.ChiPhiDuKien = snapshot.ChiPhiDuKien;
            dataset.ChiPhiThucTe = snapshot.ChiPhiThucTe;
            dataset.ChenhLechChiPhi = snapshot.ChenhLechChiPhi;
            dataset.SoLanThayDoiNhanSu = snapshot.SoLanThayDoiNhanSu;
            dataset.SoLanThayDoiQuanLy = snapshot.SoLanThayDoiQuanLy;
            dataset.SoNgayTreTienDo = snapshot.SoNgayTreTienDo;
            dataset.SoDeXuatCongViecChoDuyet = snapshot.SoDeXuatCongViecChoDuyet;
            dataset.SoDeXuatCongViecBiTuChoi = snapshot.SoDeXuatCongViecBiTuChoi;
            dataset.ThoiGianDuyetCongViecTrungBinh = snapshot.ThoiGianDuyetCongViecTrungBinh;
            dataset.SoDeXuatNganSachChoDuyet = snapshot.SoDeXuatNganSachChoDuyet;
            dataset.SoDeXuatNganSachBiTuChoi = snapshot.SoDeXuatNganSachBiTuChoi;
            dataset.ThoiGianDuyetNganSachTrungBinh = snapshot.ThoiGianDuyetNganSachTrungBinh;
            dataset.SoBaoCaoTienDoChoDuyet = snapshot.SoBaoCaoTienDoChoDuyet;
            dataset.SoBaoCaoTienDoBiTuChoi = snapshot.SoBaoCaoTienDoBiTuChoi;
            dataset.SoBaoCaoTienDoYeuCauBoSung = snapshot.SoBaoCaoTienDoYeuCauBoSung;
            dataset.TyLeBaoCaoTienDoBiTuChoi = snapshot.TyLeBaoCaoTienDoBiTuChoi;
            dataset.SoLanCapNhatTienDo = snapshot.SoLanCapNhatTienDo;
            dataset.SoNgayChamCapNhatTienDo = snapshot.SoNgayChamCapNhatTienDo;
            dataset.LaDuAnTre = snapshot.LaDuAnTre;
            dataset.MaDMNguyenNhan = snapshot.MaDMNguyenNhan;
            dataset.NgayTongHop = snapshot.ThoiDiemTongHop;
            dataset.GhiChuDataset = "Tổng hợp tự động từ dữ liệu nghiệp vụ.";
        }

        private static string[] LayTrangThaiChoPhepTongHopAi()
        {
            return
            [
                ..TrangThai.GetCommonStatusVariants(TrangThai.HoanThanh),
                ..TrangThai.GetCommonStatusVariants(TrangThai.LuuTru)
            ];
        }

        private static bool LaTrangThaiDuAnChoPhepTongHopAi(string? trangThai)
        {
            var trangThaiChoPhep = LayTrangThaiChoPhepTongHopAi();
            return trangThaiChoPhep.Any(x => TrangThai.EqualsValue(trangThai, x));
        }

        private static bool LaHanhDongThayDoiNhanSu(string? hanhDong)
        {
            var normalized = TrangThai.Normalize(hanhDong).Replace(" ", string.Empty);
            if (string.IsNullOrWhiteSpace(normalized))
            {
                return false;
            }

            return normalized.Contains("themnhansu")
                || normalized.Contains("themnhanvien")
                || normalized.Contains("themthanhvien")
                || normalized.Contains("xoanhansu")
                || normalized.Contains("xoanhanvien")
                || normalized.Contains("xoathanhvien")
                || normalized.Contains("gonhanvien")
                || normalized.Contains("gonhansu")
                || normalized.Contains("bochon")
                || normalized.Contains("capnhatvaitrophutrach")
                || normalized.Contains("thaydoiphutrach")
                || normalized.Contains("doiphutrach")
                || normalized.Contains("dieuchuyennhansu")
                || normalized.Contains("ganthanhvien");
        }

        private static bool HasDuFeature(AiDatasetRowViewModel row)
        {
            return row.SoNhanVienDuAn.HasValue
                && row.TongSoCongViec.HasValue
                && row.SoCongViecTre.HasValue
                && row.TyLeCongViecTre.HasValue
                && row.ChiPhiDuKien.HasValue
                && row.ChiPhiThucTe.HasValue
                && row.ChenhLechChiPhi.HasValue
                && row.SoLanThayDoiNhanSu.HasValue
                && row.SoLanThayDoiQuanLy.HasValue
                && row.SoNgayTreTienDo.HasValue
                && row.SoDeXuatCongViecChoDuyet.HasValue
                && row.SoDeXuatCongViecBiTuChoi.HasValue
                && row.ThoiGianDuyetCongViecTrungBinh.HasValue
                && row.SoDeXuatNganSachChoDuyet.HasValue
                && row.SoDeXuatNganSachBiTuChoi.HasValue
                && row.ThoiGianDuyetNganSachTrungBinh.HasValue
                && row.SoBaoCaoTienDoChoDuyet.HasValue
                && row.SoBaoCaoTienDoBiTuChoi.HasValue
                && row.SoBaoCaoTienDoYeuCauBoSung.HasValue
                && row.TyLeBaoCaoTienDoBiTuChoi.HasValue
                && row.SoLanCapNhatTienDo.HasValue
                && row.SoNgayChamCapNhatTienDo.HasValue
                && row.LaDuAnTre == 1
                && row.MaDMNguyenNhan.HasValue;
        }

        private static bool XacDinhDuAnTre(
            string? trangThaiDuAn,
            int? phanTramHoanThanh,
            DateTime? ngayKetThucDuAn,
            DateTime? ngayHoanThanhThucTeDuAn,
            DateTime homNay,
            int soCongViecTre,
            double tyLeCongViecTre,
            int soNgayTreTienDo)
        {
            var daHoanThanh = TrangThai.LaHoanThanhCongViec(trangThaiDuAn) || (phanTramHoanThanh ?? 0) >= 100;
            if (TrangThai.LaHoanThanhCongViec(trangThaiDuAn)
                && ngayKetThucDuAn.HasValue
                && ngayHoanThanhThucTeDuAn.HasValue)
            {
                return ngayHoanThanhThucTeDuAn.Value.Date > ngayKetThucDuAn.Value.Date;
            }

            var quaHanDuAn = ngayKetThucDuAn.HasValue && homNay > ngayKetThucDuAn.Value.Date && !daHoanThanh;
            return quaHanDuAn
                   || soCongViecTre > 0
                   || tyLeCongViecTre >= TyLeCongViecTreCanhBao
                   || soNgayTreTienDo > 0;
        }
    }
}


