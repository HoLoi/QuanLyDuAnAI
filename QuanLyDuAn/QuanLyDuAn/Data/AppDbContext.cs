using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Models;

namespace QuanLyDuAn.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<AiModel> AiModels => Set<AiModel>();
        public DbSet<VaiTro> VaiTros => Set<VaiTro>();
        public DbSet<TrangThai> TrangThais => Set<TrangThai>();
        public DbSet<LoaiDuAn> LoaiDuAns => Set<LoaiDuAn>();
        public DbSet<MucDoUuTien> MucDoUuTiens => Set<MucDoUuTien>();
        public DbSet<NhomNhanVien> NhomNhanViens => Set<NhomNhanVien>();
        public DbSet<DanhMucManHinh> DanhMucManHinhs => Set<DanhMucManHinh>();
        public DbSet<NhanVien> NhanViens => Set<NhanVien>();
        public DbSet<DuAn> DuAns => Set<DuAn>();
        public DbSet<DanhMucCongViec> DanhMucCongViecs => Set<DanhMucCongViec>();
        public DbSet<CongViec> CongViecs => Set<CongViec>();
        public DbSet<KpiCongViec> KpiCongViecs => Set<KpiCongViec>();
        public DbSet<TienDoCongViec> TienDoCongViecs => Set<TienDoCongViec>();
        public DbSet<FileTienDoCongViec> FileTienDoCongViecs => Set<FileTienDoCongViec>();
        public DbSet<FileCongViec> FileCongViecs => Set<FileCongViec>();
        public DbSet<FileDuAn> FileDuAns => Set<FileDuAn>();
        public DbSet<ChiPhi> ChiPhis => Set<ChiPhi>();
        public DbSet<CtChiPhi> CtChiPhis => Set<CtChiPhi>();
        public DbSet<CtCongViec> CtCongViecs => Set<CtCongViec>();
        public DbSet<NhatKyChiPhi> NhatKyChiPhis => Set<NhatKyChiPhi>();
        public DbSet<NhatKyNhanSu> NhatKyNhanSus => Set<NhatKyNhanSu>();
        public DbSet<DanhGiaDuAn> DanhGiaDuAns => Set<DanhGiaDuAn>();
        public DbSet<CtDanhGiaDuAn> CtDanhGiaDuAns => Set<CtDanhGiaDuAn>();
        public DbSet<DanhGiaNhanVien> DanhGiaNhanViens => Set<DanhGiaNhanVien>();
        public DbSet<CtDanhGiaNhanVien> CtDanhGiaNhanViens => Set<CtDanhGiaNhanVien>();
        public DbSet<CtKpiCongViec> CtKpiCongViecs => Set<CtKpiCongViec>();
        public DbSet<PhanCongCongViec> PhanCongCongViecs => Set<PhanCongCongViec>();
        public DbSet<ThanhVienDuAn> ThanhVienDuAns => Set<ThanhVienDuAn>();
        public DbSet<PhongChat> PhongChats => Set<PhongChat>();
        public DbSet<ThanhVienPhongChat> ThanhVienPhongChats => Set<ThanhVienPhongChat>();
        public DbSet<TinNhan> TinNhans => Set<TinNhan>();
        public DbSet<DanhMucChucNang> DanhMucChucNangs => Set<DanhMucChucNang>();
        public DbSet<PhanQuyen> PhanQuyens => Set<PhanQuyen>();
        public DbSet<ThanhVienNhom> ThanhVienNhoms => Set<ThanhVienNhom>();
        public DbSet<AiDataset> AiDatasets => Set<AiDataset>();
        public DbSet<AiKetQua> AiKetQuas => Set<AiKetQua>();
        public DbSet<AiLichSuDuDoan> AiLichSuDuDoans => Set<AiLichSuDuDoan>();
        public DbSet<AiNguyenNhan> AiNguyenNhans => Set<AiNguyenNhan>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AiModel>().ToTable("AI_MODEL").HasKey(x => x.MaModel);
            modelBuilder.Entity<VaiTro>().ToTable("VAI_TRO").HasKey(x => x.MaVaiTro);
            modelBuilder.Entity<TrangThai>().ToTable("TRANG_THAI").HasKey(x => x.MaTrangThai);
            modelBuilder.Entity<LoaiDuAn>().ToTable("LOAI_DU_AN").HasKey(x => x.MaLoaiDuAn);
            modelBuilder.Entity<MucDoUuTien>().ToTable("MUC_DO_UU_TIEN").HasKey(x => x.MaMucDo);
            modelBuilder.Entity<NhomNhanVien>().ToTable("NHOM_NHAN_VIEN").HasKey(x => x.MaNhom);
            modelBuilder.Entity<DanhMucManHinh>().ToTable("DANH_MUC_MAN_HINH").HasKey(x => x.MaManHinh);

            modelBuilder.Entity<NhanVien>(entity =>
            {
                entity.ToTable("NHAN_VIEN");
                entity.HasKey(x => x.MaNhanVien);
                entity.Property(x => x.UserId).HasColumnName("Id");
                entity.Property(x => x.Cccd).HasColumnName("CCCD");

                entity.HasIndex(x => x.UserId)
                    .IsUnique()
                    .HasFilter("[Id] IS NOT NULL");

                entity.HasOne(x => x.User)
                    .WithOne(x => x.NhanVien)
                    .HasForeignKey<NhanVien>(x => x.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<VaiTro>()
                    .WithMany()
                    .HasForeignKey(x => x.MaVaiTro)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DuAn>(entity =>
            {
                entity.ToTable("DU_AN");
                entity.HasKey(x => x.MaDuAn);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<TrangThai>().WithMany().HasForeignKey(x => x.MaTrangThai).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<LoaiDuAn>().WithMany().HasForeignKey(x => x.MaLoaiDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DanhMucCongViec>(entity =>
            {
                entity.ToTable("DANH_MUC_CONG_VIEC");
                entity.HasKey(x => x.MaDanhMucCv);
                entity.Property(x => x.MaDanhMucCv).HasColumnName("MaDanhMucCV");
                entity.Property(x => x.TenDanhMucCv).HasColumnName("TenDanhMucCV");
                entity.Property(x => x.MoTaDanhMucCv).HasColumnName("MoTaDanhMucCV");
                entity.Property(x => x.NgayTaoDmcv).HasColumnName("NgayTaoDMCV");
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CongViec>(entity =>
            {
                entity.ToTable("CONG_VIEC");
                entity.HasKey(x => x.MaCongViec);
                entity.Property(x => x.MaDanhMucCv).HasColumnName("MaDanhMucCV");
                entity.Property(x => x.NgayKetThucCvDuKien).HasColumnName("NgayKetThucCVDuKien");
                entity.Property(x => x.NgayKetThucCvThucTe).HasColumnName("NgayKetThucCVThucTe");
                entity.HasOne<DanhMucCongViec>().WithMany().HasForeignKey(x => x.MaDanhMucCv).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<TrangThai>().WithMany().HasForeignKey(x => x.MaTrangThai).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<MucDoUuTien>().WithMany().HasForeignKey(x => x.MaMucDo).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<KpiCongViec>(entity =>
            {
                entity.ToTable("KPI_CONG_VIEC");
                entity.HasKey(x => x.MaKpi);
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TienDoCongViec>(entity =>
            {
                entity.ToTable("TIEN_DO_CONG_VIEC");
                entity.HasKey(x => x.MaTienDo);
                entity.Property(x => x.ThoiGianCapNhat).HasDefaultValueSql("GETDATE()");
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FileTienDoCongViec>(entity =>
            {
                entity.ToTable("FILE_TIEN_DO_CONG_VIEC");
                entity.HasKey(x => x.MaFileTdcv);
                entity.Property(x => x.MaFileTdcv).HasColumnName("MaFileTDCV");
                entity.Property(x => x.TenFileTdcv).HasColumnName("TenFileTDCV");
                entity.Property(x => x.DuongDanFileTdcv).HasColumnName("DuongDanFileTDCV");
                entity.HasOne<TienDoCongViec>().WithMany().HasForeignKey(x => x.MaTienDo).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FileCongViec>(entity =>
            {
                entity.ToTable("FILE_CONG_VIEC");
                entity.HasKey(x => x.MaFileCv);
                entity.Property(x => x.MaFileCv).HasColumnName("MaFileCV");
                entity.Property(x => x.TenFileCv).HasColumnName("TenFileCV");
                entity.Property(x => x.DuongDanFileCv).HasColumnName("DuongDanFileCV");
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FileDuAn>(entity =>
            {
                entity.ToTable("FILE_DU_AN");
                entity.HasKey(x => x.MaFileDa);
                entity.Property(x => x.MaFileDa).HasColumnName("MaFileDA");
                entity.Property(x => x.TenFileDa).HasColumnName("TenFileDA");
                entity.Property(x => x.DuongDanFileDa).HasColumnName("DuongDanFileDA");
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ChiPhi>(entity =>
            {
                entity.ToTable("CHI_PHI");
                entity.HasKey(x => x.MaChiPhi);
                entity.Property(x => x.NgayCapNhatCp).HasColumnName("NgayCapNhatCP");
                entity.Property(x => x.NganSach).HasColumnType("decimal(18,2)");
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CtChiPhi>(entity =>
            {
                entity.ToTable("CT_CHI_PHI");
                entity.HasKey(x => x.MaChiTietCp);
                entity.Property(x => x.MaChiTietCp).HasColumnName("MaChiTietCP");
                entity.Property(x => x.SoTienDaChi).HasColumnType("decimal(18,2)");
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<ChiPhi>().WithMany().HasForeignKey(x => x.MaChiPhi).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CtCongViec>(entity =>
            {
                entity.ToTable("CT_CONG_VIEC");
                entity.HasKey(x => x.MaChiTietCv);
                entity.Property(x => x.MaChiTietCv).HasColumnName("MaChiTietCV");
                entity.Property(x => x.NoiDungChiTietCv).HasColumnName("NoiDungChiTietCV");
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NhatKyChiPhi>(entity =>
            {
                entity.ToTable("NHAT_KY_CHI_PHI");
                entity.HasKey(x => x.MaNhatKyCp);
                entity.Property(x => x.MaNhatKyCp).HasColumnName("MaNhatKyCP");
                entity.Property(x => x.SoTienNkcp).HasColumnName("SoTienNKCP");
                entity.Property(x => x.SoTienNkcp).HasColumnType("decimal(18,2)");
                entity.Property(x => x.HanhDongNkcp).HasColumnName("HanhDongNKCP");
                entity.Property(x => x.ThoiGianNkcp).HasColumnName("ThoiGianNKCP").HasDefaultValueSql("GETDATE()");
                entity.HasOne<ChiPhi>().WithMany().HasForeignKey(x => x.MaChiPhi).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<NhatKyNhanSu>(entity =>
            {
                entity.ToTable("NHAT_KY_NHAN_SU");
                entity.HasKey(x => x.MaNhatKyNs);
                entity.Property(x => x.MaNhatKyNs).HasColumnName("MaNhatKyNS");
                entity.Property(x => x.HanhDongNkns).HasColumnName("HanhDongNKNS");
                entity.Property(x => x.ThoiGianNkns).HasColumnName("ThoiGianNKNS").HasDefaultValueSql("GETDATE()");
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DanhGiaDuAn>(entity =>
            {
                entity.ToTable("DANH_GIA_DU_AN");
                entity.HasKey(x => x.MaDanhGiaDuAn);
                entity.Property(x => x.DiemTongDanhGiaDa).HasColumnName("DiemTongDanhGiaDA");
                entity.Property(x => x.NgayDanhGiaDa).HasColumnName("NgayDanhGiaDA");
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CtDanhGiaDuAn>(entity =>
            {
                entity.ToTable("CT_DANH_GIA_DU_AN");
                entity.HasKey(x => x.MaChiTietDgda);
                entity.Property(x => x.MaChiTietDgda).HasColumnName("MaChiTietDGDA");
                entity.Property(x => x.DiemDanhGiaDa).HasColumnName("DiemDanhGiaDA");
                entity.HasOne<DanhGiaDuAn>().WithMany().HasForeignKey(x => x.MaDanhGiaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DanhGiaNhanVien>(entity =>
            {
                entity.ToTable("DANH_GIA_NHAN_VIEN");
                entity.HasKey(x => x.MaDanhGiaNhanVien);
                entity.Property(x => x.MaNguoiDanhGia).HasColumnName("MaNguoiDanhGia");
                entity.Property(x => x.DiemTongDanhGiaNv).HasColumnName("DiemTongDanhGiaNV");
                entity.Property(x => x.NgayDanhGiaNv).HasColumnName("NgayDanhGiaNV");
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNguoiDanhGia).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CtDanhGiaNhanVien>(entity =>
            {
                entity.ToTable("CT_DANH_GIA_NHAN_VIEN");
                entity.HasKey(x => x.MaChiTietDgnv);
                entity.Property(x => x.MaChiTietDgnv).HasColumnName("MaChiTietDGNV");
                entity.Property(x => x.DiemDanhGiaNv).HasColumnName("DiemDanhGiaNV");
                entity.HasOne<KpiCongViec>().WithMany().HasForeignKey(x => x.MaKpi).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<DanhGiaNhanVien>().WithMany().HasForeignKey(x => x.MaDanhGiaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CtKpiCongViec>(entity =>
            {
                entity.ToTable("CT_KPI_CONG_VIEC");
                entity.HasKey(x => x.MaChiTietKpicv);
                entity.Property(x => x.MaChiTietKpicv).HasColumnName("MaChiTietKPICV");
                entity.HasOne<KpiCongViec>().WithMany().HasForeignKey(x => x.MaKpi).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PhanCongCongViec>(entity =>
            {
                entity.ToTable("PHAN_CONG_CONG_VIEC");
                entity.HasKey(x => new { x.MaNhanVien, x.MaCongViec });
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ThanhVienDuAn>(entity =>
            {
                entity.ToTable("THANH_VIEN_DU_AN");
                entity.HasKey(x => new { x.MaDuAn, x.MaNhanVien });
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PhongChat>(entity =>
            {
                entity.ToTable("PHONG_CHAT");
                entity.HasKey(x => x.MaPhongChat);
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ThanhVienPhongChat>(entity =>
            {
                entity.ToTable("THANH_VIEN_PHONG_CHAT");
                entity.HasKey(x => new { x.MaPhongChat, x.MaNhanVien });
                entity.HasOne<PhongChat>().WithMany().HasForeignKey(x => x.MaPhongChat).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<TinNhan>(entity =>
            {
                entity.ToTable("TIN_NHAN");
                entity.HasKey(x => x.MaTinNhan);
                entity.Property(x => x.ThoiGianGui).HasDefaultValueSql("GETDATE()");
                entity.HasOne<PhongChat>().WithMany().HasForeignKey(x => x.MaPhongChat).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<DanhMucChucNang>(entity =>
            {
                entity.ToTable("DANH_MUC_CHUC_NANG");
                entity.HasKey(x => x.MaChucNang);
                entity.HasOne<DanhMucManHinh>().WithMany().HasForeignKey(x => x.MaManHinh).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<PhanQuyen>(entity =>
            {
                entity.ToTable("PHAN_QUYEN");
                entity.HasKey(x => new { x.MaNhom, x.MaChucNang });
                entity.HasOne<NhomNhanVien>().WithMany().HasForeignKey(x => x.MaNhom).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<DanhMucChucNang>().WithMany().HasForeignKey(x => x.MaChucNang).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ThanhVienNhom>(entity =>
            {
                entity.ToTable("THANH_VIEN_NHOM");
                entity.HasKey(x => new { x.MaNhom, x.MaNhanVien });
                entity.HasOne<NhomNhanVien>().WithMany().HasForeignKey(x => x.MaNhom).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<NhanVien>().WithMany().HasForeignKey(x => x.MaNhanVien).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AiDataset>(entity =>
            {
                entity.ToTable("AI_DATASET");
                entity.HasKey(x => x.MaData);
                entity.Property(x => x.ChiPhiThucTe).HasColumnType("decimal(18,2)");
                entity.Property(x => x.ChiPhiDuKien).HasColumnType("decimal(18,2)");
                entity.Property(x => x.TrangThaiCuoi).HasMaxLength(50);
                entity.Property(x => x.NguyenNhanChinh).HasMaxLength(100);
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AiKetQua>(entity =>
            {
                entity.ToTable("AI_KET_QUA");
                entity.HasKey(x => x.MaAiKetQua);
                entity.HasOne<AiModel>().WithMany().HasForeignKey(x => x.MaModel).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<DuAn>().WithMany().HasForeignKey(x => x.MaDuAn).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AiLichSuDuDoan>(entity =>
            {
                entity.ToTable("AI_LICH_SU_DU_DOAN");
                entity.HasKey(x => x.MaAiLichSuDuDoan);
                entity.Property(x => x.ThoiGianDuDoan).HasDefaultValueSql("GETDATE()");
                entity.HasOne<AiModel>().WithMany().HasForeignKey(x => x.MaModel).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<AiNguyenNhan>(entity =>
            {
                entity.ToTable("AI_NGUYEN_NHAN");
                entity.HasKey(x => x.MaNguyenNhan);
                entity.HasOne<CongViec>().WithMany().HasForeignKey(x => x.MaCongViec).OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
