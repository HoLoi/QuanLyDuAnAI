using Microsoft.EntityFrameworkCore;
using QuanLyDuAn.Models.Entities;

namespace QuanLyDuAn.Data;

public partial class QuanLyDuAnDbContext : DbContext
{
    public QuanLyDuAnDbContext(DbContextOptions<QuanLyDuAnDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AiDataset> AiDataset { get; set; } = null!;
    public virtual DbSet<AiKetQua> AiKetQua { get; set; } = null!;
    public virtual DbSet<AiModel> AiModel { get; set; } = null!;
    public virtual DbSet<AiNguyenNhan> AiNguyenNhan { get; set; } = null!;
    public virtual DbSet<Aspnetroleclaims> Aspnetroleclaims { get; set; } = null!;
    public virtual DbSet<Aspnetroles> Aspnetroles { get; set; } = null!;
    public virtual DbSet<Aspnetuserclaims> Aspnetuserclaims { get; set; } = null!;
    public virtual DbSet<Aspnetuserlogins> Aspnetuserlogins { get; set; } = null!;
    public virtual DbSet<Aspnetuserroles> Aspnetuserroles { get; set; } = null!;
    public virtual DbSet<Aspnetusertokens> Aspnetusertokens { get; set; } = null!;
    public virtual DbSet<Aspnetusers> Aspnetusers { get; set; } = null!;
    public virtual DbSet<ChiPhi> ChiPhi { get; set; } = null!;
    public virtual DbSet<ChucDanh> ChucDanh { get; set; } = null!;
    public virtual DbSet<CongViec> CongViec { get; set; } = null!;
    public virtual DbSet<CtCongViec> CtCongViec { get; set; } = null!;
    public virtual DbSet<CtDanhGiaDuAn> CtDanhGiaDuAn { get; set; } = null!;
    public virtual DbSet<CtDanhGiaNhanVien> CtDanhGiaNhanVien { get; set; } = null!;
    public virtual DbSet<DanhGiaDuAn> DanhGiaDuAn { get; set; } = null!;
    public virtual DbSet<DanhGiaNhanVien> DanhGiaNhanVien { get; set; } = null!;
    public virtual DbSet<DanhMucCongViec> DanhMucCongViec { get; set; } = null!;
    public virtual DbSet<DanhMucManHinh> DanhMucManHinh { get; set; } = null!;
    public virtual DbSet<DanhMucQuyen> DanhMucQuyen { get; set; } = null!;
    public virtual DbSet<DeXuatCongViec> DeXuatCongViec { get; set; } = null!;
    public virtual DbSet<DeXuatNganSach> DeXuatNganSach { get; set; } = null!;
    public virtual DbSet<DmNguyenNhan> DmNguyenNhan { get; set; } = null!;
    public virtual DbSet<DuAn> DuAn { get; set; } = null!;
    public virtual DbSet<FileCongViec> FileCongViec { get; set; } = null!;
    public virtual DbSet<FileDuAn> FileDuAn { get; set; } = null!;
    public virtual DbSet<FileTienDoCongViec> FileTienDoCongViec { get; set; } = null!;
    public virtual DbSet<LoaiDuAn> LoaiDuAn { get; set; } = null!;
    public virtual DbSet<MucDoUuTien> MucDoUuTien { get; set; } = null!;
    public virtual DbSet<NganSach> NganSach { get; set; } = null!;
    public virtual DbSet<NguoiDung> NguoiDung { get; set; } = null!;
    public virtual DbSet<NhanVienDuAn> NhanVienDuAn { get; set; } = null!;
    public virtual DbSet<NhanVienTeam> NhanVienTeam { get; set; } = null!;
    public virtual DbSet<NhatKyChiPhi> NhatKyChiPhi { get; set; } = null!;
    public virtual DbSet<NhatKyDuAn> NhatKyDuAn { get; set; } = null!;
    public virtual DbSet<NhatKyNganSach> NhatKyNganSach { get; set; } = null!;
    public virtual DbSet<NhatKyPhanCongCongViec> NhatKyPhanCongCongViec { get; set; } = null!;
    public virtual DbSet<NhatKyPhuTrachDuAn> NhatKyPhuTrachDuAn { get; set; } = null!;
    public virtual DbSet<NhatKyQuanLyDuAn> NhatKyQuanLyDuAn { get; set; } = null!;
    public virtual DbSet<PhanCongCongViec> PhanCongCongViec { get; set; } = null!;
    public virtual DbSet<PhongChat> PhongChat { get; set; } = null!;
    public virtual DbSet<Team> Team { get; set; } = null!;
    public virtual DbSet<TeamDuAn> TeamDuAn { get; set; } = null!;
    public virtual DbSet<ThanhVienPhongChat> ThanhVienPhongChat { get; set; } = null!;
    public virtual DbSet<TienDoCongViec> TienDoCongViec { get; set; } = null!;
    public virtual DbSet<TieuChiDanhGia> TieuChiDanhGia { get; set; } = null!;
    public virtual DbSet<TinNhan> TinNhan { get; set; } = null!;
    public virtual DbSet<YeuCauDoiQuanLy> YeuCauDoiQuanLy { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiDataset>(entity =>
        {
            entity.ToTable("AI_DATASET");
            entity.HasKey(e => e.MaData);
            entity.Property(e => e.ChiPhiDuKien).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ChiPhiThucTe).HasColumnType("decimal(18,2)");
            entity.Property(e => e.ChenhLechChiPhi).HasColumnType("decimal(18,2)");
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_AI_DATAS_DU LIEU_DU_AN");
        });
        modelBuilder.Entity<AiKetQua>(entity =>
        {
            entity.ToTable("AI_KET_QUA");
            entity.HasKey(e => e.MaAiKetQua);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_AI_KET_Q_DU LIEU_DU_AN");
            entity.HasOne<AiDataset>()
                .WithMany()
                .HasForeignKey(d => d.MaData)
                .HasConstraintName("FK_AI_KET_Q_DUA VAO_AI_DATAS");
            entity.HasOne<AiModel>()
                .WithMany()
                .HasForeignKey(d => d.MaModel)
                .HasConstraintName("FK_AI_KET_Q_KET QUA_AI_MODEL");
            entity.HasOne<DmNguyenNhan>()
                .WithMany()
                .HasForeignKey(d => d.MaDMNguyenNhan)
                .HasConstraintName("FK_AI_KET_Q_THUOC_DM_NGUYE");
        });
        modelBuilder.Entity<AiModel>(entity =>
        {
            entity.ToTable("AI_MODEL");
            entity.HasKey(e => e.MaModel);
            entity.Property(e => e.TenModel).HasMaxLength(255);
            entity.Property(e => e.MoTaModel).HasMaxLength(255);
        });
        modelBuilder.Entity<AiNguyenNhan>(entity =>
        {
            entity.ToTable("AI_NGUYEN_NHAN");
            entity.HasKey(e => e.MaAINguyenNhan);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_AI_NGUYE_THUOC_DU_AN");
            entity.HasOne<DmNguyenNhan>()
                .WithMany()
                .HasForeignKey(d => d.MaDMNguyenNhan)
                .HasConstraintName("FK_AI_NGUYE_THUOC_DM_NGUYE");
        });
        modelBuilder.Entity<Aspnetroleclaims>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Asp_Id).HasMaxLength(128);
            entity.HasOne<Aspnetroles>()
                .WithMany()
                .HasForeignKey(d => d.Asp_Id)
                .HasConstraintName("FK_ASPNETRO_FK_ASPNET_ASPNETRO");
            entity.HasOne<DanhMucQuyen>()
                .WithMany()
                .HasForeignKey(d => d.MaDanhMucQuyen)
                .HasConstraintName("FK_ASPNETRO_THUOC_DANH_MUC");
        });
        modelBuilder.Entity<Aspnetroles>(entity =>
        {
            entity.ToTable("AspNetRoles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });
        modelBuilder.Entity<Aspnetuserclaims>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Asp_Id).HasMaxLength(128);
            entity.HasOne<Aspnetusers>()
                .WithMany()
                .HasForeignKey(d => d.Asp_Id)
                .HasConstraintName("FK_ASPNETUSERCLAIMS_ASPNETUSERS");
        });
        modelBuilder.Entity<Aspnetuserlogins>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.ProviderKey).HasMaxLength(128);
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.HasOne<Aspnetusers>()
                .WithMany()
                .HasForeignKey(d => d.Id)
                .HasConstraintName("FK_ASPNETUSERLOGINS_ASPNETUSERS");
        });
        modelBuilder.Entity<Aspnetuserroles>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
            entity.HasKey(e => new { e.Asp_Id, e.Id });
            entity.Property(e => e.Asp_Id).HasMaxLength(128);
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.HasOne<Aspnetusers>()
                .WithMany()
                .HasForeignKey(d => d.Asp_Id)
                .HasConstraintName("FK_ASPNETUSERROLES_ASPNETUSERS");
            entity.HasOne<Aspnetroles>()
                .WithMany()
                .HasForeignKey(d => d.Id)
                .HasConstraintName("FK_ASPNETUS_FK_ASPNET_ASPNETRO");
        });
        modelBuilder.Entity<Aspnetusertokens>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
            entity.HasKey(e => new { e.Id, e.LoginProvider, e.Name });
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.LoginProvider).HasMaxLength(128);
            entity.Property(e => e.Name).HasMaxLength(128);
            entity.HasOne<Aspnetusers>()
                .WithMany()
                .HasForeignKey(d => d.Id)
                .HasConstraintName("FK_ASPNETUSERTOKENS_ASPNETUSERS");
        });
        modelBuilder.Entity<Aspnetusers>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.UserName).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_ASPNETUS_CO TK_NGUOI_DU");
        });
        modelBuilder.Entity<ChiPhi>(entity =>
        {
            entity.ToTable("CHI_PHI");
            entity.HasKey(e => e.MaChiPhi);
            entity.Property(e => e.SoTienDaChi).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TrangThaiChiPhi).HasMaxLength(50);
            entity.HasOne<NganSach>()
                .WithMany()
                .HasForeignKey(d => d.MaNganSach)
                .HasConstraintName("FK_CHI_PHI_CHI_NGAN_SAC");
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_CHI_PHI_CHI CHO_CONG_VIE");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_CHI_PHI_DELETED_BY");
        });
        modelBuilder.Entity<ChucDanh>(entity =>
        {
            entity.ToTable("CHUC_DANH");
            entity.HasKey(e => e.MaChucDanh);
            entity.Property(e => e.TenChucDanh).HasMaxLength(255);
            entity.Property(e => e.MoTaChucDanh).HasMaxLength(255);
        });
        modelBuilder.Entity<CongViec>(entity =>
        {
            entity.ToTable("CONG_VIEC");
            entity.HasKey(e => e.MaCongViec);
            entity.Property(e => e.TenCongViec).HasMaxLength(255);
            entity.Property(e => e.TrangThaiCongViec).HasMaxLength(50);
            entity.HasOne<DanhMucCongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaDanhMucCV)
                .HasConstraintName("FK_CONG_VIE_CO_DANH_MUC");
            entity.HasOne<MucDoUuTien>()
                .WithMany()
                .HasForeignKey(d => d.MaMucDo)
                .HasConstraintName("FK_CONG_VIE_CO_MUC_DO_U");
            entity.HasOne<DeXuatCongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaDeXuatCV)
                .HasConstraintName("FK_CONG_VIE_THUOC_DE_XUAT_");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_CONG_VIEC_DELETED_BY");
        });
        modelBuilder.Entity<CtCongViec>(entity =>
        {
            entity.ToTable("CT_CONG_VIEC");
            entity.HasKey(e => e.MaChiTietCV);
            entity.Property(e => e.TrangThaiCTCV).HasMaxLength(50);
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_CT_CONG__CO_CONG_VIE");
        });
        modelBuilder.Entity<CtDanhGiaDuAn>(entity =>
        {
            entity.ToTable("CT_DANH_GIA_DU_AN");
            entity.HasKey(e => e.MaChiTietDGDA);
            entity.Property(e => e.TieuChi).HasMaxLength(50);
            entity.HasOne<DanhGiaDuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDanhGiaDuAn)
                .HasConstraintName("FK_CT_DGDA_CO_DANH_GIA");
        });
        modelBuilder.Entity<CtDanhGiaNhanVien>(entity =>
        {
            entity.ToTable("CT_DANH_GIA_NHAN_VIEN");
            entity.HasKey(e => e.MaChiTietDGNV);
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_CT_DANH__CO_CONG_VIE");
            entity.HasOne<DanhGiaNhanVien>()
                .WithMany()
                .HasForeignKey(d => d.MaDanhGiaNhanVien)
                .HasConstraintName("FK_CT_DGNV_CO_DANH_GIA");
        });
        modelBuilder.Entity<DanhGiaDuAn>(entity =>
        {
            entity.ToTable("DANH_GIA_DU_AN");
            entity.HasKey(e => e.MaDanhGiaDuAn);
            entity.Property(e => e.NhanXetTongDuAn).HasMaxLength(255);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_DGDA_DANH_GIA_NGUOI_DU");
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_DANH_GIA_DUOC DANH_DU_AN");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_DGDA_DELETED_BY");
        });
        modelBuilder.Entity<DanhGiaNhanVien>(entity =>
        {
            entity.ToTable("DANH_GIA_NHAN_VIEN");
            entity.HasKey(e => e.MaDanhGiaNhanVien);
            entity.Property(e => e.XepLoai).HasMaxLength(50);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDanhGia)
                .HasConstraintName("FK_DGNV_DANH_GIA_NGUOI_DU");
            entity.HasOne<TieuChiDanhGia>()
                .WithMany()
                .HasForeignKey(d => d.MaTieuChi)
                .HasConstraintName("FK_DANH_GIA_DUA VAO_TIEU_CHI");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_DANH_GIA_DUOC_NGUOI_DU");
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_DANH_GIA_TRONG_DU_AN");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_DGNV_DELETED_BY");
        });
        modelBuilder.Entity<DanhMucCongViec>(entity =>
        {
            entity.ToTable("DANH_MUC_CONG_VIEC");
            entity.HasKey(e => e.MaDanhMucCV);
            entity.Property(e => e.TenDanhMucCV).HasMaxLength(255);
            entity.Property(e => e.MoTaDanhMucCV).HasMaxLength(255);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_DANH_MUC_GOM_DU_AN");
        });
        modelBuilder.Entity<DanhMucManHinh>(entity =>
        {
            entity.ToTable("DANH_MUC_MAN_HINH");
            entity.HasKey(e => e.MaManHinh);
            entity.Property(e => e.TenManHinh).HasMaxLength(255);
            entity.Property(e => e.MoTaManHinh).HasMaxLength(255);
        });
        modelBuilder.Entity<DanhMucQuyen>(entity =>
        {
            entity.ToTable("DANH_MUC_QUYEN");
            entity.HasKey(e => e.MaDanhMucQuyen);
            entity.Property(e => e.TenDanhMucQuyen).HasMaxLength(255);
            entity.Property(e => e.MoTaDanhMucQuyen).HasMaxLength(255);
            entity.HasOne<DanhMucManHinh>()
                .WithMany()
                .HasForeignKey(d => d.MaManHinh)
                .HasConstraintName("FK_DANH_MUC_CO_DANH_MUC");
        });
        modelBuilder.Entity<DeXuatCongViec>(entity =>
        {
            entity.ToTable("DE_XUAT_CONG_VIEC");
            entity.HasKey(e => e.MaDeXuatCV);
            entity.Property(e => e.TenCongViecDeXuat).HasMaxLength(255);
            entity.Property(e => e.ChiPhiDeXuat).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TrangThaiCongViecDeXuat).HasMaxLength(50);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_DE_XUAT__CO_DU_AN");
            entity.HasOne<DanhMucCongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaDanhMucCV)
                .HasConstraintName("FK_DE_XUAT__CO_DANH_MUC");
            entity.HasOne<MucDoUuTien>()
                .WithMany()
                .HasForeignKey(d => d.MaMucDo)
                .HasConstraintName("FK_DE_XUAT__CO_MUC_DO_U");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDeXuat)
                .HasConstraintName("FK_DE_XUAT__DE XUAT_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDuyet)
                .HasConstraintName("FK_DE_XUAT__DUYET_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_DE_XUAT_CV_DELETED_BY");
        });
        modelBuilder.Entity<DeXuatNganSach>(entity =>
        {
            entity.ToTable("DE_XUAT_NGAN_SACH");
            entity.HasKey(e => e.MaDeXuatNS);
            entity.Property(e => e.NganSachCu).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NganSachDeXuat).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TrangThaiDeXuat).HasMaxLength(50);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_DE_XUAT_NS_CO_DU_AN");
            entity.HasOne<NganSach>()
                .WithMany()
                .HasForeignKey(d => d.MaNganSachCu)
                .HasConstraintName("FK_DE_XUAT_NS_THUOC_NGAN_SACH");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDeXuat)
                .HasConstraintName("FK_DE_XUAT_NS_DE_XUAT_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDuyet)
                .HasConstraintName("FK_DE_XUAT_NS_DUYET_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_DE_XUAT_NS_DELETED_BY");
        });
        modelBuilder.Entity<DmNguyenNhan>(entity =>
        {
            entity.ToTable("DM_NGUYEN_NHAN");
            entity.HasKey(e => e.MaDMNguyenNhan);
            entity.Property(e => e.TenNguyenNhan).HasMaxLength(255);
        });
        modelBuilder.Entity<DuAn>(entity =>
        {
            entity.ToTable("DU_AN");
            entity.HasKey(e => e.MaDuAn);
            entity.Property(e => e.TenDuAn).HasMaxLength(255);
            entity.Property(e => e.TrangThaiDuAn).HasMaxLength(50);
            entity.Property(e => e.GhiChuDuAn).HasMaxLength(255);
            entity.HasOne<LoaiDuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaLoaiDuAn)
                .HasConstraintName("FK_DU_AN_CO_LOAI_DU_");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_DU_AN_QUAN LY_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_DU_AN_DELETED_BY");
        });
        modelBuilder.Entity<FileCongViec>(entity =>
        {
            entity.ToTable("FILE_CONG_VIEC");
            entity.HasKey(e => e.MaFileCV);
            entity.Property(e => e.TenFileCV).HasMaxLength(255);
            entity.Property(e => e.DuongDanFileCV).HasMaxLength(500);
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_FILE_CON_CO_CONG_VIE");
        });
        modelBuilder.Entity<FileDuAn>(entity =>
        {
            entity.ToTable("FILE_DU_AN");
            entity.HasKey(e => e.MaFileDA);
            entity.Property(e => e.TenFileDA).HasMaxLength(255);
            entity.Property(e => e.DuongDanFileDA).HasMaxLength(500);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_FILE_DU__CO_DU_AN");
        });
        modelBuilder.Entity<FileTienDoCongViec>(entity =>
        {
            entity.ToTable("FILE_TIEN_DO_CONG_VIEC");
            entity.HasKey(e => e.MaFileTDCV);
            entity.Property(e => e.TenFileTDCV).HasMaxLength(255);
            entity.Property(e => e.DuongDanFileTDCV).HasMaxLength(500);
            entity.HasOne<TienDoCongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaTienDo)
                .HasConstraintName("FK_FILE_TIE_CO_TIEN_DO_");
        });
        modelBuilder.Entity<LoaiDuAn>(entity =>
        {
            entity.ToTable("LOAI_DU_AN");
            entity.HasKey(e => e.MaLoaiDuAn);
            entity.Property(e => e.TenLoai).HasMaxLength(255);
            entity.Property(e => e.MoTaLoaiDuAn).HasMaxLength(255);
        });
        modelBuilder.Entity<MucDoUuTien>(entity =>
        {
            entity.ToTable("MUC_DO_UU_TIEN");
            entity.HasKey(e => e.MaMucDo);
            entity.Property(e => e.TenMucDo).HasMaxLength(100);
            entity.Property(e => e.MoTaMucDo).HasMaxLength(255);
        });
        modelBuilder.Entity<NganSach>(entity =>
        {
            entity.ToTable("NGAN_SACH");
            entity.HasKey(e => e.MaNganSach);
            entity.Property(e => e.SoTienNganSach).HasColumnName("NganSach").HasColumnType("decimal(18,2)");
            entity.Property(e => e.MoTaNganSach).HasMaxLength(255);
            entity.Property(e => e.TrangThaiNganSach).HasMaxLength(50);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_NGAN_SAC_CO_DU_AN");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDeXuat)
                .HasConstraintName("FK_NGAN_SAC_DE XUAT_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDuyet)
                .HasConstraintName("FK_NGAN_SAC_DUYET_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_NGAN_SACH_DELETED_BY");
        });
        modelBuilder.Entity<NguoiDung>(entity =>
        {
            entity.ToTable("NGUOI_DUNG");
            entity.HasKey(e => e.MaNguoiDung);
            entity.Property(e => e.Id).HasMaxLength(128);
            entity.Property(e => e.HoTenNguoiDung).HasMaxLength(255);
            entity.Property(e => e.DiaChiNguoiDung).HasMaxLength(255);
            entity.Property(e => e.SdtNguoiDung).HasMaxLength(20);
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.HasOne<ChucDanh>()
                .WithMany()
                .HasForeignKey(d => d.MaChucDanh)
                .HasConstraintName("FK_NGUOI_DU_CO_CHUC_DAN");
            entity.HasOne<Aspnetusers>()
                .WithMany()
                .HasForeignKey(d => d.Id)
                .HasConstraintName("FK_NGUOI_DU_CO TK_ASPNETUS");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_NGUOI_DUNG_DELETED_BY");
        });
        modelBuilder.Entity<NhanVienDuAn>(entity =>
        {
            entity.ToTable("NHAN_VIEN_DU_AN");
            entity.HasKey(e => new { e.MaDuAn, e.MaNguoiDung });
            entity.Property(e => e.VaiTroTrongDuAn).HasMaxLength(50);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_NHAN_VIE_NHAN_VIEN_DU_AN");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_NVDA_NGUOI_DUNG");
        });
        modelBuilder.Entity<NhanVienTeam>(entity =>
        {
            entity.ToTable("NHAN_VIEN_TEAM");
            entity.HasKey(e => new { e.MaNguoiDung, e.MaTeam });
            entity.Property(e => e.VaiTroTrongTeam).HasMaxLength(100);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_NVT_NGUOI_DUNG");
            entity.HasOne<Team>()
                .WithMany()
                .HasForeignKey(d => d.MaTeam)
                .HasConstraintName("FK_NHAN_VIE_NHAN_VIEN_TEAM");
        });
        modelBuilder.Entity<NhatKyChiPhi>(entity =>
        {
            entity.ToTable("NHAT_KY_CHI_PHI");
            entity.HasKey(e => e.MaNhatKyCP);
            entity.Property(e => e.NkSoTienDaChi).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NkTrangThaiChiPhi).HasMaxLength(50);
            entity.Property(e => e.HanhDongNKCP).HasMaxLength(255);
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_NKCP_CO_CONG_VIEC");
            entity.HasOne<ChiPhi>()
                .WithMany()
                .HasForeignKey(d => d.MaChiPhi)
                .HasConstraintName("FK_NHAT_KY__GI_CHI_PHI");
        });
        modelBuilder.Entity<NhatKyDuAn>(entity =>
        {
            entity.ToTable("NHAT_KY_DU_AN");
            entity.HasKey(e => e.MaNhatKyTeamDA);
            entity.Property(e => e.HanhDongNKDA).HasMaxLength(255);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_NKDA_CO_DU_AN");
            entity.HasOne<Team>()
                .WithMany()
                .HasForeignKey(d => d.MaTeam)
                .HasConstraintName("FK_NHAT_KY__CO_TEAM");
        });
        modelBuilder.Entity<NhatKyNganSach>(entity =>
        {
            entity.ToTable("NHAT_KY_NGAN_SACH");
            entity.HasKey(e => e.MaNhatKyNS);
            entity.Property(e => e.SoTienNKNS).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NganSachTruoc).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NganSachSau).HasColumnType("decimal(18,2)");
            entity.Property(e => e.NkTrangThaiNganSach).HasMaxLength(50);
            entity.Property(e => e.HanhDongNKNS).HasMaxLength(255);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_NKNS_CO_DU_AN");
            entity.HasOne<NganSach>()
                .WithMany()
                .HasForeignKey(d => d.MaNganSach)
                .HasConstraintName("FK_NHAT_KY__GHI_NGAN_SAC");
        });
        modelBuilder.Entity<NhatKyPhanCongCongViec>(entity =>
        {
            entity.ToTable("NHAT_KY_PHAN_CONG_CONG_VIEC");
            entity.HasKey(e => e.MaNhatKyPCCV);
            entity.Property(e => e.HanhDongPCCV).HasMaxLength(255);
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_NKPCCV_CO_CONG_VIEC");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_NHAT_KY__DUOC PHAN_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungGhi)
                .HasConstraintName("FK_NKPCCV_GHI_NGUOI_DU");
        });
        modelBuilder.Entity<NhatKyPhuTrachDuAn>(entity =>
        {
            entity.ToTable("NHAT_KY_PHU_TRACH_DU_AN");
            entity.HasKey(e => e.MaNhatKyPTDA);
            entity.Property(e => e.NkHanhDongPTDA).HasMaxLength(255);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_NKPTDA_CO_DU_AN");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_NHAT_KY__DUOC GHI_NGUOI_DU");
        });
        modelBuilder.Entity<NhatKyQuanLyDuAn>(entity =>
        {
            entity.ToTable("NHAT_KY_QUAN_LY_DU_AN");
            entity.HasKey(e => e.MaNhatKyQLDA);
            entity.Property(e => e.NkHanhDongQLDA).HasMaxLength(255);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_NKQLDA_GHI_NGUOI_DU");
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_NHAT_KY__TRONG_DU_AN");
        });
        modelBuilder.Entity<PhanCongCongViec>(entity =>
        {
            entity.ToTable("PHAN_CONG_CONG_VIEC");
            entity.HasKey(e => new { e.MaNguoiDung, e.MaCongViec });
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_PHAN_CON_PHAN_CONG_CONG_VIE");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_PHAN_CON_PHAN_CONG_NGUOI_DU");
        });
        modelBuilder.Entity<PhongChat>(entity =>
        {
            entity.ToTable("PHONG_CHAT");
            entity.HasKey(e => e.MaPhongChat);
            entity.Property(e => e.TenPhong).HasMaxLength(255);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_PHONG_CH_CO_DU_AN");
        });
        modelBuilder.Entity<Team>(entity =>
        {
            entity.ToTable("TEAM");
            entity.HasKey(e => e.MaTeam);
            entity.Property(e => e.TenTeam).HasMaxLength(255);
            entity.Property(e => e.MoTaTeam).HasMaxLength(255);
            entity.Property(e => e.TrangThaiTeam).HasMaxLength(50);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_TEAM_DELETED_BY");
        });
        modelBuilder.Entity<TeamDuAn>(entity =>
        {
            entity.ToTable("TEAM_DU_AN");
            entity.HasKey(e => new { e.MaTeam, e.MaDuAn });
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_TEAM_DU__TEAM_DU_A_DU_AN");
            entity.HasOne<Team>()
                .WithMany()
                .HasForeignKey(d => d.MaTeam)
                .HasConstraintName("FK_TEAM_DU__TEAM_DU_A_TEAM");
        });
        modelBuilder.Entity<ThanhVienPhongChat>(entity =>
        {
            entity.ToTable("THANH_VIEN_PHONG_CHAT");
            entity.HasKey(e => new { e.MaPhongChat, e.MaNguoiDung });
            entity.Property(e => e.VaiTroTrongPhongChat).HasMaxLength(50);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_THANH_VI_THANH_VIE_NGUOI_DU");
            entity.HasOne<PhongChat>()
                .WithMany()
                .HasForeignKey(d => d.MaPhongChat)
                .HasConstraintName("FK_THANH_VI_THANH_VIE_PHONG_CH");
        });
        modelBuilder.Entity<TienDoCongViec>(entity =>
        {
            entity.ToTable("TIEN_DO_CONG_VIEC");
            entity.HasKey(e => e.MaTienDo);
            entity.Property(e => e.GhiChuTienDo).HasMaxLength(255);
            entity.Property(e => e.TrangThaiTienDo).HasMaxLength(50);
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_TIEN_DO__BAO CAO_NGUOI_DU");
            entity.HasOne<CongViec>()
                .WithMany()
                .HasForeignKey(d => d.MaCongViec)
                .HasConstraintName("FK_TIEN_DO__DUOC CAP _CONG_VIE");
        });
        modelBuilder.Entity<TieuChiDanhGia>(entity =>
        {
            entity.ToTable("TIEU_CHI_DANH_GIA");
            entity.HasKey(e => e.MaTieuChi);
            entity.Property(e => e.TenTieuChi).HasMaxLength(255);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.LoaiTieuChi).HasMaxLength(255);
            entity.Property(e => e.TrangThaiTieuChi).HasMaxLength(50);
        });
        modelBuilder.Entity<TinNhan>(entity =>
        {
            entity.ToTable("TIN_NHAN");
            entity.HasKey(e => e.MaTinNhan);
            entity.HasOne<PhongChat>()
                .WithMany()
                .HasForeignKey(d => d.MaPhongChat)
                .HasConstraintName("FK_TIN_NHAN_NAM TRONG_PHONG_CH");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDung)
                .HasConstraintName("FK_TIN_NHAN_NHAN TIN_NGUOI_DU");
        });
        modelBuilder.Entity<YeuCauDoiQuanLy>(entity =>
        {
            entity.ToTable("YEU_CAU_DOI_QUAN_LY");
            entity.HasKey(e => e.MaYeuCauDoiQuanLy);
            entity.Property(e => e.TrangThaiYeuCauDoiQuanLy).HasMaxLength(255);
            entity.HasOne<DuAn>()
                .WithMany()
                .HasForeignKey(d => d.MaDuAn)
                .HasConstraintName("FK_YEU_CAU__CO_DU_AN");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaQuanLyHienTai)
                .HasConstraintName("FK_YEU_CAU__DE XUAT_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaQuanLyDeXuat)
                .HasConstraintName("FK_YEU_CAU__DUOC DE X_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.MaNguoiDungDuyet)
                .HasConstraintName("FK_YEU_CAU__DUYET_NGUOI_DU");
            entity.HasOne<NguoiDung>()
                .WithMany()
                .HasForeignKey(d => d.DeletedBy)
                .HasConstraintName("FK_YEU_CAU_QL_DELETED_BY");
        });

        // SQL Server does not allow multiple cascade paths. The source SQL schema
        // also does not define cascading deletes, so enforce NoAction globally.
        foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        {
            foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
        }

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
