using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuanLyDuAn.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AI_MODEL",
                columns: table => new
                {
                    MaModel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenModel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SoLuongDuLieu = table.Column<int>(type: "int", nullable: true),
                    DoChinhXac = table.Column<double>(type: "float", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTaModel = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_MODEL", x => x.MaModel);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CHUC_DANH",
                columns: table => new
                {
                    MaChucDanh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChucDanh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaChucDanh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHUC_DANH", x => x.MaChucDanh);
                });

            migrationBuilder.CreateTable(
                name: "DANH_MUC_MAN_HINH",
                columns: table => new
                {
                    MaManHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenManHinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaManHinh = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_MUC_MAN_HINH", x => x.MaManHinh);
                });

            migrationBuilder.CreateTable(
                name: "DM_NGUYEN_NHAN",
                columns: table => new
                {
                    MaDMNguyenNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNguyenNhan = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DM_NGUYEN_NHAN", x => x.MaDMNguyenNhan);
                });

            migrationBuilder.CreateTable(
                name: "LOAI_DU_AN",
                columns: table => new
                {
                    MaLoaiDuAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaLoaiDuAn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOAI_DU_AN", x => x.MaLoaiDuAn);
                });

            migrationBuilder.CreateTable(
                name: "MUC_DO_UU_TIEN",
                columns: table => new
                {
                    MaMucDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenMucDo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MoTaMucDo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUC_DO_UU_TIEN", x => x.MaMucDo);
                });

            migrationBuilder.CreateTable(
                name: "TIEU_CHI_DANH_GIA",
                columns: table => new
                {
                    MaTieuChi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTieuChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DiemTieuChi = table.Column<double>(type: "float", nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    LoaiTieuChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TrangThaiTieuChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIEU_CHI_DANH_GIA", x => x.MaTieuChi);
                });

            migrationBuilder.CreateTable(
                name: "DANH_MUC_QUYEN",
                columns: table => new
                {
                    MaDanhMucQuyen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaManHinh = table.Column<int>(type: "int", nullable: false),
                    TenDanhMucQuyen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaDanhMucQuyen = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_MUC_QUYEN", x => x.MaDanhMucQuyen);
                    table.ForeignKey(
                        name: "FK_DANH_MUC_CO_DANH_MUC",
                        column: x => x.MaManHinh,
                        principalTable: "DANH_MUC_MAN_HINH",
                        principalColumn: "MaManHinh");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asp_Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    MaDanhMucQuyen = table.Column<int>(type: "int", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ASPNETRO_FK_ASPNET_ASPNETRO",
                        column: x => x.Asp_Id,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ASPNETRO_THUOC_DANH_MUC",
                        column: x => x.MaDanhMucQuyen,
                        principalTable: "DANH_MUC_QUYEN",
                        principalColumn: "MaDanhMucQuyen");
                });

            migrationBuilder.CreateTable(
                name: "AI_DATASET",
                columns: table => new
                {
                    MaData = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    SoNhanVienDuAn = table.Column<int>(type: "int", nullable: true),
                    TongSoCongViec = table.Column<int>(type: "int", nullable: true),
                    SoCongViecTre = table.Column<int>(type: "int", nullable: true),
                    TyLeCongViecTre = table.Column<double>(type: "float", nullable: true),
                    ChiPhiDuKien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChiPhiThucTe = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChenhLechChiPhi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TongGioLam = table.Column<double>(type: "float", nullable: true),
                    SoLanThayDoiNhanSu = table.Column<int>(type: "int", nullable: true),
                    SoLanThayDoiQuanLy = table.Column<int>(type: "int", nullable: true),
                    SoNgayTreTienDo = table.Column<int>(type: "int", nullable: true),
                    IsTre = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_DATASET", x => x.MaData);
                });

            migrationBuilder.CreateTable(
                name: "AI_KET_QUA",
                columns: table => new
                {
                    MaAiKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDMNguyenNhan = table.Column<int>(type: "int", nullable: false),
                    MaModel = table.Column<int>(type: "int", nullable: false),
                    MaData = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    DoTinCayKetQua = table.Column<double>(type: "float", nullable: true),
                    ThoiGianDuDoanKetQua = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_KET_QUA", x => x.MaAiKetQua);
                    table.ForeignKey(
                        name: "FK_AI_KET_Q_DUA VAO_AI_DATAS",
                        column: x => x.MaData,
                        principalTable: "AI_DATASET",
                        principalColumn: "MaData");
                    table.ForeignKey(
                        name: "FK_AI_KET_Q_KET QUA_AI_MODEL",
                        column: x => x.MaModel,
                        principalTable: "AI_MODEL",
                        principalColumn: "MaModel");
                    table.ForeignKey(
                        name: "FK_AI_KET_Q_THUOC_DM_NGUYE",
                        column: x => x.MaDMNguyenNhan,
                        principalTable: "DM_NGUYEN_NHAN",
                        principalColumn: "MaDMNguyenNhan");
                });

            migrationBuilder.CreateTable(
                name: "AI_NGUYEN_NHAN",
                columns: table => new
                {
                    MaAINguyenNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaDMNguyenNhan = table.Column<int>(type: "int", nullable: false),
                    DoTinCay = table.Column<double>(type: "float", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_NGUYEN_NHAN", x => x.MaAINguyenNhan);
                    table.ForeignKey(
                        name: "FK_AI_NGUYE_THUOC_DM_NGUYE",
                        column: x => x.MaDMNguyenNhan,
                        principalTable: "DM_NGUYEN_NHAN",
                        principalColumn: "MaDMNguyenNhan");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Asp_Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    Asp_Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.Asp_Id, x.Id });
                    table.ForeignKey(
                        name: "FK_ASPNETUS_FK_ASPNET_ASPNETRO",
                        column: x => x.Id,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.Id, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_ASPNETUSERTOKENS_ASPNETUSERS",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NGUOI_DUNG",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChucDanh = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    HoTenNguoiDung = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DiaChiNguoiDung = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SdtNguoiDung = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGUOI_DUNG", x => x.MaNguoiDung);
                    table.ForeignKey(
                        name: "FK_NGUOI_DUNG_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_NGUOI_DU_CO TK_ASPNETUS",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NGUOI_DU_CO_CHUC_DAN",
                        column: x => x.MaChucDanh,
                        principalTable: "CHUC_DANH",
                        principalColumn: "MaChucDanh");
                });

            migrationBuilder.CreateTable(
                name: "DU_AN",
                columns: table => new
                {
                    MaDuAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaLoaiDuAn = table.Column<int>(type: "int", nullable: false),
                    TenDuAn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTaoDuAn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayBatDauDuAn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucDuAn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhanTramHoanThanh = table.Column<int>(type: "int", nullable: true),
                    TrangThaiDuAn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    GhiChuDuAn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DU_AN", x => x.MaDuAn);
                    table.ForeignKey(
                        name: "FK_DU_AN_CO_LOAI_DU_",
                        column: x => x.MaLoaiDuAn,
                        principalTable: "LOAI_DU_AN",
                        principalColumn: "MaLoaiDuAn");
                    table.ForeignKey(
                        name: "FK_DU_AN_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DU_AN_QUAN LY_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "TEAM",
                columns: table => new
                {
                    MaTeam = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTeam = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaTeam = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayLapTeam = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiTeam = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEAM", x => x.MaTeam);
                    table.ForeignKey(
                        name: "FK_TEAM_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "DANH_GIA_DU_AN",
                columns: table => new
                {
                    MaDanhGiaDuAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    DiemTongDanhGiaDA = table.Column<int>(type: "int", nullable: true),
                    NhanXetTongDuAn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayDanhGiaDA = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_GIA_DU_AN", x => x.MaDanhGiaDuAn);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_DUOC DANH_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_DGDA_DANH_GIA_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DGDA_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "DANH_GIA_NHAN_VIEN",
                columns: table => new
                {
                    MaDanhGiaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaTieuChi = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDungDanhGia = table.Column<int>(type: "int", nullable: false),
                    DiemTongDanhGiaNV = table.Column<int>(type: "int", nullable: true),
                    NgayDanhGiaNV = table.Column<DateTime>(type: "datetime2", nullable: true),
                    XepLoai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_GIA_NHAN_VIEN", x => x.MaDanhGiaNhanVien);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_DUA VAO_TIEU_CHI",
                        column: x => x.MaTieuChi,
                        principalTable: "TIEU_CHI_DANH_GIA",
                        principalColumn: "MaTieuChi");
                    table.ForeignKey(
                        name: "FK_DANH_GIA_DUOC_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DANH_GIA_TRONG_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_DGNV_DANH_GIA_NGUOI_DU",
                        column: x => x.MaNguoiDungDanhGia,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DGNV_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "DANH_MUC_CONG_VIEC",
                columns: table => new
                {
                    MaDanhMucCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    TenDanhMucCV = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaDanhMucCV = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTaoDMCV = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_MUC_CONG_VIEC", x => x.MaDanhMucCV);
                    table.ForeignKey(
                        name: "FK_DANH_MUC_GOM_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                });

            migrationBuilder.CreateTable(
                name: "FILE_DU_AN",
                columns: table => new
                {
                    MaFileDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    TenFileDA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DuongDanFileDA = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_DU_AN", x => x.MaFileDA);
                    table.ForeignKey(
                        name: "FK_FILE_DU__CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                });

            migrationBuilder.CreateTable(
                name: "NGAN_SACH",
                columns: table => new
                {
                    MaNganSach = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDungDuyet = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDungDeXuat = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    NganSach = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    MoTaNganSach = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayCapNhatNganSach = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayDuyetNganSach = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiNganSach = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NGAN_SACH", x => x.MaNganSach);
                    table.ForeignKey(
                        name: "FK_NGAN_SACH_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_NGAN_SAC_CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_NGAN_SAC_DE XUAT_NGUOI_DU",
                        column: x => x.MaNguoiDungDeXuat,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_NGAN_SAC_DUYET_NGUOI_DU",
                        column: x => x.MaNguoiDungDuyet,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "NHAN_VIEN_DU_AN",
                columns: table => new
                {
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NgayThamGiaDuAn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VaiTroTrongDuAn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAN_VIEN_DU_AN", x => new { x.MaDuAn, x.MaNguoiDung });
                    table.ForeignKey(
                        name: "FK_NHAN_VIE_NHAN_VIEN_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_NVDA_NGUOI_DUNG",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_PHU_TRACH_DU_AN",
                columns: table => new
                {
                    MaNhatKyPTDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    NkHanhDongPTDA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NkThoiGianPTDA = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_PHU_TRACH_DU_AN", x => x.MaNhatKyPTDA);
                    table.ForeignKey(
                        name: "FK_NHAT_KY__DUOC GHI_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_NKPTDA_CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_QUAN_LY_DU_AN",
                columns: table => new
                {
                    MaNhatKyQLDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NkHanhDongQLDA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NkThoiGianQLDA = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QLDATuNgay = table.Column<DateTime>(type: "datetime2", nullable: true),
                    QLDADenNgay = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_QUAN_LY_DU_AN", x => x.MaNhatKyQLDA);
                    table.ForeignKey(
                        name: "FK_NHAT_KY__TRONG_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_NKQLDA_GHI_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "PHONG_CHAT",
                columns: table => new
                {
                    MaPhongChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    TenPhong = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHONG_CHAT", x => x.MaPhongChat);
                    table.ForeignKey(
                        name: "FK_PHONG_CH_CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                });

            migrationBuilder.CreateTable(
                name: "YEU_CAU_DOI_QUAN_LY",
                columns: table => new
                {
                    MaYeuCauDoiQuanLy = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaQuanLyDeXuat = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDungDuyet = table.Column<int>(type: "int", nullable: true),
                    MaQuanLyHienTai = table.Column<int>(type: "int", nullable: false),
                    TrangThaiYeuCauDoiQuanLy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTaoYeuCauDoiQuanLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayDuyetYeuCauDoiQuanLy = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YEU_CAU_DOI_QUAN_LY", x => x.MaYeuCauDoiQuanLy);
                    table.ForeignKey(
                        name: "FK_YEU_CAU_QL_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_YEU_CAU__CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_YEU_CAU__DE XUAT_NGUOI_DU",
                        column: x => x.MaQuanLyHienTai,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_YEU_CAU__DUOC DE X_NGUOI_DU",
                        column: x => x.MaQuanLyDeXuat,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_YEU_CAU__DUYET_NGUOI_DU",
                        column: x => x.MaNguoiDungDuyet,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "NHAN_VIEN_TEAM",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaTeam = table.Column<int>(type: "int", nullable: false),
                    VaiTroTrongTeam = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    NgayThamGiaTeam = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsLeader = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAN_VIEN_TEAM", x => new { x.MaNguoiDung, x.MaTeam });
                    table.ForeignKey(
                        name: "FK_NHAN_VIE_NHAN_VIEN_TEAM",
                        column: x => x.MaTeam,
                        principalTable: "TEAM",
                        principalColumn: "MaTeam");
                    table.ForeignKey(
                        name: "FK_NVT_NGUOI_DUNG",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_DU_AN",
                columns: table => new
                {
                    MaNhatKyTeamDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTeam = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    TeamCuPhuTrach = table.Column<int>(type: "int", nullable: true),
                    TeamMoiPhuTrach = table.Column<int>(type: "int", nullable: true),
                    HanhDongNKDA = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGianNKDA = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_DU_AN", x => x.MaNhatKyTeamDA);
                    table.ForeignKey(
                        name: "FK_NHAT_KY__CO_TEAM",
                        column: x => x.MaTeam,
                        principalTable: "TEAM",
                        principalColumn: "MaTeam");
                    table.ForeignKey(
                        name: "FK_NKDA_CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                });

            migrationBuilder.CreateTable(
                name: "TEAM_DU_AN",
                columns: table => new
                {
                    MaTeam = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    NgayTeamThamGiaDA = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TEAM_DU_AN", x => new { x.MaTeam, x.MaDuAn });
                    table.ForeignKey(
                        name: "FK_TEAM_DU__TEAM_DU_A_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_TEAM_DU__TEAM_DU_A_TEAM",
                        column: x => x.MaTeam,
                        principalTable: "TEAM",
                        principalColumn: "MaTeam");
                });

            migrationBuilder.CreateTable(
                name: "CT_DANH_GIA_DU_AN",
                columns: table => new
                {
                    MaChiTietDGDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDanhGiaDuAn = table.Column<int>(type: "int", nullable: false),
                    NhanXetDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TieuChi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DiemDanhGiaDA = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DANH_GIA_DU_AN", x => x.MaChiTietDGDA);
                    table.ForeignKey(
                        name: "FK_CT_DGDA_CO_DANH_GIA",
                        column: x => x.MaDanhGiaDuAn,
                        principalTable: "DANH_GIA_DU_AN",
                        principalColumn: "MaDanhGiaDuAn");
                });

            migrationBuilder.CreateTable(
                name: "DE_XUAT_CONG_VIEC",
                columns: table => new
                {
                    MaDeXuatCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaDanhMucCV = table.Column<int>(type: "int", nullable: false),
                    MaMucDo = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDungDeXuat = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDungDuyet = table.Column<int>(type: "int", nullable: true),
                    TenCongViecDeXuat = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaCongViecDeXuat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChiPhiDeXuat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayBatDauCongViecDeXuat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucCVDeXuatDuKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayDeXuatCongViec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayDuyetDeXuatCongViec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiCongViecDeXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_XUAT_CONG_VIEC", x => x.MaDeXuatCV);
                    table.ForeignKey(
                        name: "FK_DE_XUAT_CV_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DE_XUAT__CO_DANH_MUC",
                        column: x => x.MaDanhMucCV,
                        principalTable: "DANH_MUC_CONG_VIEC",
                        principalColumn: "MaDanhMucCV");
                    table.ForeignKey(
                        name: "FK_DE_XUAT__CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_DE_XUAT__CO_MUC_DO_U",
                        column: x => x.MaMucDo,
                        principalTable: "MUC_DO_UU_TIEN",
                        principalColumn: "MaMucDo");
                    table.ForeignKey(
                        name: "FK_DE_XUAT__DE XUAT_NGUOI_DU",
                        column: x => x.MaNguoiDungDeXuat,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DE_XUAT__DUYET_NGUOI_DU",
                        column: x => x.MaNguoiDungDuyet,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "DE_XUAT_NGAN_SACH",
                columns: table => new
                {
                    MaDeXuatNS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNganSachCu = table.Column<int>(type: "int", nullable: true),
                    NganSachCu = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NganSachDeXuat = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    LyDoDeXuat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaNguoiDungDeXuat = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDungDuyet = table.Column<int>(type: "int", nullable: true),
                    NgayDeXuat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiDeXuat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DE_XUAT_NGAN_SACH", x => x.MaDeXuatNS);
                    table.ForeignKey(
                        name: "FK_DE_XUAT_NS_CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                    table.ForeignKey(
                        name: "FK_DE_XUAT_NS_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DE_XUAT_NS_DE_XUAT_NGUOI_DU",
                        column: x => x.MaNguoiDungDeXuat,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DE_XUAT_NS_DUYET_NGUOI_DU",
                        column: x => x.MaNguoiDungDuyet,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_DE_XUAT_NS_THUOC_NGAN_SACH",
                        column: x => x.MaNganSachCu,
                        principalTable: "NGAN_SACH",
                        principalColumn: "MaNganSach");
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_NGAN_SACH",
                columns: table => new
                {
                    MaNhatKyNS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNganSach = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    SoTienNKNS = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NganSachTruoc = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NganSachSau = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NkNgayCapNhatNS = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NkTrangThaiNganSach = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HanhDongNKNS = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGianNKNS = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_NGAN_SACH", x => x.MaNhatKyNS);
                    table.ForeignKey(
                        name: "FK_NHAT_KY__GHI_NGAN_SAC",
                        column: x => x.MaNganSach,
                        principalTable: "NGAN_SACH",
                        principalColumn: "MaNganSach");
                    table.ForeignKey(
                        name: "FK_NKNS_CO_DU_AN",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn");
                });

            migrationBuilder.CreateTable(
                name: "THANH_VIEN_PHONG_CHAT",
                columns: table => new
                {
                    MaPhongChat = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    VaiTroTrongPhongChat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THANH_VIEN_PHONG_CHAT", x => new { x.MaPhongChat, x.MaNguoiDung });
                    table.ForeignKey(
                        name: "FK_THANH_VI_THANH_VIE_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_THANH_VI_THANH_VIE_PHONG_CH",
                        column: x => x.MaPhongChat,
                        principalTable: "PHONG_CHAT",
                        principalColumn: "MaPhongChat");
                });

            migrationBuilder.CreateTable(
                name: "TIN_NHAN",
                columns: table => new
                {
                    MaTinNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhongChat = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    NoiDungTinNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIN_NHAN", x => x.MaTinNhan);
                    table.ForeignKey(
                        name: "FK_TIN_NHAN_NAM TRONG_PHONG_CH",
                        column: x => x.MaPhongChat,
                        principalTable: "PHONG_CHAT",
                        principalColumn: "MaPhongChat");
                    table.ForeignKey(
                        name: "FK_TIN_NHAN_NHAN TIN_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "CONG_VIEC",
                columns: table => new
                {
                    MaCongViec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDeXuatCV = table.Column<int>(type: "int", nullable: true),
                    MaDanhMucCV = table.Column<int>(type: "int", nullable: false),
                    MaMucDo = table.Column<int>(type: "int", nullable: false),
                    TenCongViec = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    MoTaCongViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBatDauCongViec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucCVDuKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucCVThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTaoCongViec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiCongViec = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONG_VIEC", x => x.MaCongViec);
                    table.ForeignKey(
                        name: "FK_CONG_VIEC_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_CONG_VIE_CO_DANH_MUC",
                        column: x => x.MaDanhMucCV,
                        principalTable: "DANH_MUC_CONG_VIEC",
                        principalColumn: "MaDanhMucCV");
                    table.ForeignKey(
                        name: "FK_CONG_VIE_CO_MUC_DO_U",
                        column: x => x.MaMucDo,
                        principalTable: "MUC_DO_UU_TIEN",
                        principalColumn: "MaMucDo");
                    table.ForeignKey(
                        name: "FK_CONG_VIE_THUOC_DE_XUAT_",
                        column: x => x.MaDeXuatCV,
                        principalTable: "DE_XUAT_CONG_VIEC",
                        principalColumn: "MaDeXuatCV");
                });

            migrationBuilder.CreateTable(
                name: "CHI_PHI",
                columns: table => new
                {
                    MaChiPhi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaNganSach = table.Column<int>(type: "int", nullable: false),
                    NoiDungChiPhi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoTienDaChi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayChi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiChiPhi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHI_PHI", x => x.MaChiPhi);
                    table.ForeignKey(
                        name: "FK_CHI_PHI_CHI CHO_CONG_VIE",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                    table.ForeignKey(
                        name: "FK_CHI_PHI_CHI_NGAN_SAC",
                        column: x => x.MaNganSach,
                        principalTable: "NGAN_SACH",
                        principalColumn: "MaNganSach");
                    table.ForeignKey(
                        name: "FK_CHI_PHI_DELETED_BY",
                        column: x => x.DeletedBy,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "CT_CONG_VIEC",
                columns: table => new
                {
                    MaChiTietCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    NoiDungChiTietCV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTaoCTCV = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayBaoCaoCTCV = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhanTramHoanThanhCTCV = table.Column<double>(type: "float", nullable: true),
                    TrangThaiCTCV = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_CONG_VIEC", x => x.MaChiTietCV);
                    table.ForeignKey(
                        name: "FK_CT_CONG__CO_CONG_VIE",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                });

            migrationBuilder.CreateTable(
                name: "CT_DANH_GIA_NHAN_VIEN",
                columns: table => new
                {
                    MaChiTietDGNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDanhGiaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    NoiDungDanhGiaNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemDanhGiaNV = table.Column<int>(type: "int", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DANH_GIA_NHAN_VIEN", x => x.MaChiTietDGNV);
                    table.ForeignKey(
                        name: "FK_CT_DANH__CO_CONG_VIE",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                    table.ForeignKey(
                        name: "FK_CT_DGNV_CO_DANH_GIA",
                        column: x => x.MaDanhGiaNhanVien,
                        principalTable: "DANH_GIA_NHAN_VIEN",
                        principalColumn: "MaDanhGiaNhanVien");
                });

            migrationBuilder.CreateTable(
                name: "FILE_CONG_VIEC",
                columns: table => new
                {
                    MaFileCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    TenFileCV = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DuongDanFileCV = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_CONG_VIEC", x => x.MaFileCV);
                    table.ForeignKey(
                        name: "FK_FILE_CON_CO_CONG_VIE",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_PHAN_CONG_CONG_VIEC",
                columns: table => new
                {
                    MaNhatKyPCCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDungGhi = table.Column<int>(type: "int", nullable: false),
                    HanhDongPCCV = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGianPCCV = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_PHAN_CONG_CONG_VIEC", x => x.MaNhatKyPCCV);
                    table.ForeignKey(
                        name: "FK_NHAT_KY__DUOC PHAN_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_NKPCCV_CO_CONG_VIEC",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                    table.ForeignKey(
                        name: "FK_NKPCCV_GHI_NGUOI_DU",
                        column: x => x.MaNguoiDungGhi,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "PHAN_CONG_CONG_VIEC",
                columns: table => new
                {
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    NgayGiaoViec = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHAN_CONG_CONG_VIEC", x => new { x.MaNguoiDung, x.MaCongViec });
                    table.ForeignKey(
                        name: "FK_PHAN_CON_PHAN_CONG_CONG_VIE",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                    table.ForeignKey(
                        name: "FK_PHAN_CON_PHAN_CONG_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                });

            migrationBuilder.CreateTable(
                name: "TIEN_DO_CONG_VIEC",
                columns: table => new
                {
                    MaTienDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDung = table.Column<int>(type: "int", nullable: false),
                    PhanTram = table.Column<int>(type: "int", nullable: true),
                    GhiChuTienDo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGianCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiTienDo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIEN_DO_CONG_VIEC", x => x.MaTienDo);
                    table.ForeignKey(
                        name: "FK_TIEN_DO__BAO CAO_NGUOI_DU",
                        column: x => x.MaNguoiDung,
                        principalTable: "NGUOI_DUNG",
                        principalColumn: "MaNguoiDung");
                    table.ForeignKey(
                        name: "FK_TIEN_DO__DUOC CAP _CONG_VIE",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_CHI_PHI",
                columns: table => new
                {
                    MaNhatKyCP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaChiPhi = table.Column<int>(type: "int", nullable: false),
                    NkSoTienDaChi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NkNgayChi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NkTrangThaiChiPhi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HanhDongNKCP = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ThoiGianNKCP = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_CHI_PHI", x => x.MaNhatKyCP);
                    table.ForeignKey(
                        name: "FK_NHAT_KY__GI_CHI_PHI",
                        column: x => x.MaChiPhi,
                        principalTable: "CHI_PHI",
                        principalColumn: "MaChiPhi");
                    table.ForeignKey(
                        name: "FK_NKCP_CO_CONG_VIEC",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec");
                });

            migrationBuilder.CreateTable(
                name: "FILE_TIEN_DO_CONG_VIEC",
                columns: table => new
                {
                    MaFileTDCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTienDo = table.Column<int>(type: "int", nullable: false),
                    TenFileTDCV = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DuongDanFileTDCV = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_TIEN_DO_CONG_VIEC", x => x.MaFileTDCV);
                    table.ForeignKey(
                        name: "FK_FILE_TIE_CO_TIEN_DO_",
                        column: x => x.MaTienDo,
                        principalTable: "TIEN_DO_CONG_VIEC",
                        principalColumn: "MaTienDo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AI_DATASET_MaDuAn",
                table: "AI_DATASET",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_AI_KET_QUA_MaData",
                table: "AI_KET_QUA",
                column: "MaData");

            migrationBuilder.CreateIndex(
                name: "IX_AI_KET_QUA_MaDMNguyenNhan",
                table: "AI_KET_QUA",
                column: "MaDMNguyenNhan");

            migrationBuilder.CreateIndex(
                name: "IX_AI_KET_QUA_MaDuAn",
                table: "AI_KET_QUA",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_AI_KET_QUA_MaModel",
                table: "AI_KET_QUA",
                column: "MaModel");

            migrationBuilder.CreateIndex(
                name: "IX_AI_NGUYEN_NHAN_MaDMNguyenNhan",
                table: "AI_NGUYEN_NHAN",
                column: "MaDMNguyenNhan");

            migrationBuilder.CreateIndex(
                name: "IX_AI_NGUYEN_NHAN_MaDuAn",
                table: "AI_NGUYEN_NHAN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_Asp_Id",
                table: "AspNetRoleClaims",
                column: "Asp_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_MaDanhMucQuyen",
                table: "AspNetRoleClaims",
                column: "MaDanhMucQuyen");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_Asp_Id",
                table: "AspNetUserClaims",
                column: "Asp_Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_Id",
                table: "AspNetUserLogins",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_Id",
                table: "AspNetUserRoles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_MaNguoiDung",
                table: "AspNetUsers",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_CHI_PHI_DeletedBy",
                table: "CHI_PHI",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CHI_PHI_MaCongViec",
                table: "CHI_PHI",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_CHI_PHI_MaNganSach",
                table: "CHI_PHI",
                column: "MaNganSach");

            migrationBuilder.CreateIndex(
                name: "IX_CONG_VIEC_DeletedBy",
                table: "CONG_VIEC",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_CONG_VIEC_MaDanhMucCV",
                table: "CONG_VIEC",
                column: "MaDanhMucCV");

            migrationBuilder.CreateIndex(
                name: "IX_CONG_VIEC_MaDeXuatCV",
                table: "CONG_VIEC",
                column: "MaDeXuatCV");

            migrationBuilder.CreateIndex(
                name: "IX_CONG_VIEC_MaMucDo",
                table: "CONG_VIEC",
                column: "MaMucDo");

            migrationBuilder.CreateIndex(
                name: "IX_CT_CONG_VIEC_MaCongViec",
                table: "CT_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DANH_GIA_DU_AN_MaDanhGiaDuAn",
                table: "CT_DANH_GIA_DU_AN",
                column: "MaDanhGiaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DANH_GIA_NHAN_VIEN_MaCongViec",
                table: "CT_DANH_GIA_NHAN_VIEN",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DANH_GIA_NHAN_VIEN_MaDanhGiaNhanVien",
                table: "CT_DANH_GIA_NHAN_VIEN",
                column: "MaDanhGiaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_DU_AN_DeletedBy",
                table: "DANH_GIA_DU_AN",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_DU_AN_MaDuAn",
                table: "DANH_GIA_DU_AN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_DU_AN_MaNguoiDung",
                table: "DANH_GIA_DU_AN",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_DeletedBy",
                table: "DANH_GIA_NHAN_VIEN",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_MaDuAn",
                table: "DANH_GIA_NHAN_VIEN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_MaNguoiDung",
                table: "DANH_GIA_NHAN_VIEN",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_MaNguoiDungDanhGia",
                table: "DANH_GIA_NHAN_VIEN",
                column: "MaNguoiDungDanhGia");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_MaTieuChi",
                table: "DANH_GIA_NHAN_VIEN",
                column: "MaTieuChi");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_MUC_CONG_VIEC_MaDuAn",
                table: "DANH_MUC_CONG_VIEC",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_MUC_QUYEN_MaManHinh",
                table: "DANH_MUC_QUYEN",
                column: "MaManHinh");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_CONG_VIEC_DeletedBy",
                table: "DE_XUAT_CONG_VIEC",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_CONG_VIEC_MaDanhMucCV",
                table: "DE_XUAT_CONG_VIEC",
                column: "MaDanhMucCV");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_CONG_VIEC_MaDuAn",
                table: "DE_XUAT_CONG_VIEC",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_CONG_VIEC_MaMucDo",
                table: "DE_XUAT_CONG_VIEC",
                column: "MaMucDo");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_CONG_VIEC_MaNguoiDungDeXuat",
                table: "DE_XUAT_CONG_VIEC",
                column: "MaNguoiDungDeXuat");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_CONG_VIEC_MaNguoiDungDuyet",
                table: "DE_XUAT_CONG_VIEC",
                column: "MaNguoiDungDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_NGAN_SACH_DeletedBy",
                table: "DE_XUAT_NGAN_SACH",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_NGAN_SACH_MaDuAn",
                table: "DE_XUAT_NGAN_SACH",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_NGAN_SACH_MaNganSachCu",
                table: "DE_XUAT_NGAN_SACH",
                column: "MaNganSachCu");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_NGAN_SACH_MaNguoiDungDeXuat",
                table: "DE_XUAT_NGAN_SACH",
                column: "MaNguoiDungDeXuat");

            migrationBuilder.CreateIndex(
                name: "IX_DE_XUAT_NGAN_SACH_MaNguoiDungDuyet",
                table: "DE_XUAT_NGAN_SACH",
                column: "MaNguoiDungDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_DU_AN_DeletedBy",
                table: "DU_AN",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DU_AN_MaLoaiDuAn",
                table: "DU_AN",
                column: "MaLoaiDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DU_AN_MaNguoiDung",
                table: "DU_AN",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_FILE_CONG_VIEC_MaCongViec",
                table: "FILE_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_FILE_DU_AN_MaDuAn",
                table: "FILE_DU_AN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_FILE_TIEN_DO_CONG_VIEC_MaTienDo",
                table: "FILE_TIEN_DO_CONG_VIEC",
                column: "MaTienDo");

            migrationBuilder.CreateIndex(
                name: "IX_NGAN_SACH_DeletedBy",
                table: "NGAN_SACH",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NGAN_SACH_MaDuAn",
                table: "NGAN_SACH",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_NGAN_SACH_MaNguoiDungDeXuat",
                table: "NGAN_SACH",
                column: "MaNguoiDungDeXuat");

            migrationBuilder.CreateIndex(
                name: "IX_NGAN_SACH_MaNguoiDungDuyet",
                table: "NGAN_SACH",
                column: "MaNguoiDungDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_NGUOI_DUNG_DeletedBy",
                table: "NGUOI_DUNG",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_NGUOI_DUNG_Id",
                table: "NGUOI_DUNG",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_NGUOI_DUNG_MaChucDanh",
                table: "NGUOI_DUNG",
                column: "MaChucDanh");

            migrationBuilder.CreateIndex(
                name: "IX_NHAN_VIEN_DU_AN_MaNguoiDung",
                table: "NHAN_VIEN_DU_AN",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_NHAN_VIEN_TEAM_MaTeam",
                table: "NHAN_VIEN_TEAM",
                column: "MaTeam");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_CHI_PHI_MaChiPhi",
                table: "NHAT_KY_CHI_PHI",
                column: "MaChiPhi");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_CHI_PHI_MaCongViec",
                table: "NHAT_KY_CHI_PHI",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_DU_AN_MaDuAn",
                table: "NHAT_KY_DU_AN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_DU_AN_MaTeam",
                table: "NHAT_KY_DU_AN",
                column: "MaTeam");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_NGAN_SACH_MaDuAn",
                table: "NHAT_KY_NGAN_SACH",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_NGAN_SACH_MaNganSach",
                table: "NHAT_KY_NGAN_SACH",
                column: "MaNganSach");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_PHAN_CONG_CONG_VIEC_MaCongViec",
                table: "NHAT_KY_PHAN_CONG_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_PHAN_CONG_CONG_VIEC_MaNguoiDung",
                table: "NHAT_KY_PHAN_CONG_CONG_VIEC",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_PHAN_CONG_CONG_VIEC_MaNguoiDungGhi",
                table: "NHAT_KY_PHAN_CONG_CONG_VIEC",
                column: "MaNguoiDungGhi");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_PHU_TRACH_DU_AN_MaDuAn",
                table: "NHAT_KY_PHU_TRACH_DU_AN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_PHU_TRACH_DU_AN_MaNguoiDung",
                table: "NHAT_KY_PHU_TRACH_DU_AN",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_QUAN_LY_DU_AN_MaDuAn",
                table: "NHAT_KY_QUAN_LY_DU_AN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_QUAN_LY_DU_AN_MaNguoiDung",
                table: "NHAT_KY_QUAN_LY_DU_AN",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_PHAN_CONG_CONG_VIEC_MaCongViec",
                table: "PHAN_CONG_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_PHONG_CHAT_MaDuAn",
                table: "PHONG_CHAT",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_TEAM_DeletedBy",
                table: "TEAM",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TEAM_DU_AN_MaDuAn",
                table: "TEAM_DU_AN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_THANH_VIEN_PHONG_CHAT_MaNguoiDung",
                table: "THANH_VIEN_PHONG_CHAT",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_TIEN_DO_CONG_VIEC_MaCongViec",
                table: "TIEN_DO_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_TIEN_DO_CONG_VIEC_MaNguoiDung",
                table: "TIEN_DO_CONG_VIEC",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_TIN_NHAN_MaNguoiDung",
                table: "TIN_NHAN",
                column: "MaNguoiDung");

            migrationBuilder.CreateIndex(
                name: "IX_TIN_NHAN_MaPhongChat",
                table: "TIN_NHAN",
                column: "MaPhongChat");

            migrationBuilder.CreateIndex(
                name: "IX_YEU_CAU_DOI_QUAN_LY_DeletedBy",
                table: "YEU_CAU_DOI_QUAN_LY",
                column: "DeletedBy");

            migrationBuilder.CreateIndex(
                name: "IX_YEU_CAU_DOI_QUAN_LY_MaDuAn",
                table: "YEU_CAU_DOI_QUAN_LY",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_YEU_CAU_DOI_QUAN_LY_MaNguoiDungDuyet",
                table: "YEU_CAU_DOI_QUAN_LY",
                column: "MaNguoiDungDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_YEU_CAU_DOI_QUAN_LY_MaQuanLyDeXuat",
                table: "YEU_CAU_DOI_QUAN_LY",
                column: "MaQuanLyDeXuat");

            migrationBuilder.CreateIndex(
                name: "IX_YEU_CAU_DOI_QUAN_LY_MaQuanLyHienTai",
                table: "YEU_CAU_DOI_QUAN_LY",
                column: "MaQuanLyHienTai");

            migrationBuilder.AddForeignKey(
                name: "FK_AI_DATAS_DU LIEU_DU_AN",
                table: "AI_DATASET",
                column: "MaDuAn",
                principalTable: "DU_AN",
                principalColumn: "MaDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_AI_KET_Q_DU LIEU_DU_AN",
                table: "AI_KET_QUA",
                column: "MaDuAn",
                principalTable: "DU_AN",
                principalColumn: "MaDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_AI_NGUYE_THUOC_DU_AN",
                table: "AI_NGUYEN_NHAN",
                column: "MaDuAn",
                principalTable: "DU_AN",
                principalColumn: "MaDuAn");

            migrationBuilder.AddForeignKey(
                name: "FK_ASPNETUSERCLAIMS_ASPNETUSERS",
                table: "AspNetUserClaims",
                column: "Asp_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ASPNETUSERLOGINS_ASPNETUSERS",
                table: "AspNetUserLogins",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ASPNETUSERROLES_ASPNETUSERS",
                table: "AspNetUserRoles",
                column: "Asp_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ASPNETUS_CO TK_NGUOI_DU",
                table: "AspNetUsers",
                column: "MaNguoiDung",
                principalTable: "NGUOI_DUNG",
                principalColumn: "MaNguoiDung");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NGUOI_DU_CO TK_ASPNETUS",
                table: "NGUOI_DUNG");

            migrationBuilder.DropTable(
                name: "AI_KET_QUA");

            migrationBuilder.DropTable(
                name: "AI_NGUYEN_NHAN");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CT_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "CT_DANH_GIA_DU_AN");

            migrationBuilder.DropTable(
                name: "CT_DANH_GIA_NHAN_VIEN");

            migrationBuilder.DropTable(
                name: "DE_XUAT_NGAN_SACH");

            migrationBuilder.DropTable(
                name: "FILE_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "FILE_DU_AN");

            migrationBuilder.DropTable(
                name: "FILE_TIEN_DO_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "NHAN_VIEN_DU_AN");

            migrationBuilder.DropTable(
                name: "NHAN_VIEN_TEAM");

            migrationBuilder.DropTable(
                name: "NHAT_KY_CHI_PHI");

            migrationBuilder.DropTable(
                name: "NHAT_KY_DU_AN");

            migrationBuilder.DropTable(
                name: "NHAT_KY_NGAN_SACH");

            migrationBuilder.DropTable(
                name: "NHAT_KY_PHAN_CONG_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "NHAT_KY_PHU_TRACH_DU_AN");

            migrationBuilder.DropTable(
                name: "NHAT_KY_QUAN_LY_DU_AN");

            migrationBuilder.DropTable(
                name: "PHAN_CONG_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "TEAM_DU_AN");

            migrationBuilder.DropTable(
                name: "THANH_VIEN_PHONG_CHAT");

            migrationBuilder.DropTable(
                name: "TIN_NHAN");

            migrationBuilder.DropTable(
                name: "YEU_CAU_DOI_QUAN_LY");

            migrationBuilder.DropTable(
                name: "AI_DATASET");

            migrationBuilder.DropTable(
                name: "AI_MODEL");

            migrationBuilder.DropTable(
                name: "DM_NGUYEN_NHAN");

            migrationBuilder.DropTable(
                name: "DANH_MUC_QUYEN");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DANH_GIA_DU_AN");

            migrationBuilder.DropTable(
                name: "DANH_GIA_NHAN_VIEN");

            migrationBuilder.DropTable(
                name: "TIEN_DO_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "CHI_PHI");

            migrationBuilder.DropTable(
                name: "TEAM");

            migrationBuilder.DropTable(
                name: "PHONG_CHAT");

            migrationBuilder.DropTable(
                name: "DANH_MUC_MAN_HINH");

            migrationBuilder.DropTable(
                name: "TIEU_CHI_DANH_GIA");

            migrationBuilder.DropTable(
                name: "CONG_VIEC");

            migrationBuilder.DropTable(
                name: "NGAN_SACH");

            migrationBuilder.DropTable(
                name: "DE_XUAT_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "DANH_MUC_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "MUC_DO_UU_TIEN");

            migrationBuilder.DropTable(
                name: "DU_AN");

            migrationBuilder.DropTable(
                name: "LOAI_DU_AN");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "NGUOI_DUNG");

            migrationBuilder.DropTable(
                name: "CHUC_DANH");
        }
    }
}
