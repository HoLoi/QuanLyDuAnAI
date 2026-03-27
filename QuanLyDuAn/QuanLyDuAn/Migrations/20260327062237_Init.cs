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
                    TenModel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoChinhXac = table.Column<double>(type: "float", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MoTaModel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_MODEL", x => x.MaModel);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
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
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DANH_MUC_MAN_HINH",
                columns: table => new
                {
                    MaManHinh = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenManHinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaManHinh = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_MUC_MAN_HINH", x => x.MaManHinh);
                });

            migrationBuilder.CreateTable(
                name: "LOAI_DU_AN",
                columns: table => new
                {
                    MaLoaiDuAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenLoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaLoaiDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    TenMucDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaMucDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MUC_DO_UU_TIEN", x => x.MaMucDo);
                });

            migrationBuilder.CreateTable(
                name: "NHOM_NHAN_VIEN",
                columns: table => new
                {
                    MaNhom = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenNhom = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaNhom = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHOM_NHAN_VIEN", x => x.MaNhom);
                });

            migrationBuilder.CreateTable(
                name: "TRANG_THAI",
                columns: table => new
                {
                    MaTrangThai = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenTrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiTrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaTrangThai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TRANG_THAI", x => x.MaTrangThai);
                });

            migrationBuilder.CreateTable(
                name: "VAI_TRO",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VAI_TRO", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DANH_MUC_CHUC_NANG",
                columns: table => new
                {
                    MaChucNang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaManHinh = table.Column<int>(type: "int", nullable: false),
                    TenChucNang = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaChucNang = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_MUC_CHUC_NANG", x => x.MaChucNang);
                    table.ForeignKey(
                        name: "FK_DANH_MUC_CHUC_NANG_DANH_MUC_MAN_HINH_MaManHinh",
                        column: x => x.MaManHinh,
                        principalTable: "DANH_MUC_MAN_HINH",
                        principalColumn: "MaManHinh",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NHAN_VIEN",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CCCD = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAN_VIEN", x => x.MaNhanVien);
                    table.ForeignKey(
                        name: "FK_NHAN_VIEN_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NHAN_VIEN_VAI_TRO_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VAI_TRO",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PHAN_QUYEN",
                columns: table => new
                {
                    MaNhom = table.Column<int>(type: "int", nullable: false),
                    MaChucNang = table.Column<int>(type: "int", nullable: false),
                    CoQuyen = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHAN_QUYEN", x => new { x.MaNhom, x.MaChucNang });
                    table.ForeignKey(
                        name: "FK_PHAN_QUYEN_DANH_MUC_CHUC_NANG_MaChucNang",
                        column: x => x.MaChucNang,
                        principalTable: "DANH_MUC_CHUC_NANG",
                        principalColumn: "MaChucNang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PHAN_QUYEN_NHOM_NHAN_VIEN_MaNhom",
                        column: x => x.MaNhom,
                        principalTable: "NHOM_NHAN_VIEN",
                        principalColumn: "MaNhom",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DU_AN",
                columns: table => new
                {
                    MaDuAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaTrangThai = table.Column<int>(type: "int", nullable: false),
                    MaLoaiDuAn = table.Column<int>(type: "int", nullable: false),
                    TenDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBatDauDuAn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucDuAn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PhanTramHoanThanh = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DU_AN", x => x.MaDuAn);
                    table.ForeignKey(
                        name: "FK_DU_AN_LOAI_DU_AN_MaLoaiDuAn",
                        column: x => x.MaLoaiDuAn,
                        principalTable: "LOAI_DU_AN",
                        principalColumn: "MaLoaiDuAn",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DU_AN_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DU_AN_TRANG_THAI_MaTrangThai",
                        column: x => x.MaTrangThai,
                        principalTable: "TRANG_THAI",
                        principalColumn: "MaTrangThai",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THANH_VIEN_NHOM",
                columns: table => new
                {
                    MaNhom = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    VaiTroTrongNhom = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THANH_VIEN_NHOM", x => new { x.MaNhom, x.MaNhanVien });
                    table.ForeignKey(
                        name: "FK_THANH_VIEN_NHOM_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THANH_VIEN_NHOM_NHOM_NHAN_VIEN_MaNhom",
                        column: x => x.MaNhom,
                        principalTable: "NHOM_NHAN_VIEN",
                        principalColumn: "MaNhom",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AI_KET_QUA",
                columns: table => new
                {
                    MaAiKetQua = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaModel = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    NguyenNhanDuDoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DoTinCayKetQua = table.Column<double>(type: "float", nullable: true),
                    ThoiGianDuDoanKetQua = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_KET_QUA", x => x.MaAiKetQua);
                    table.ForeignKey(
                        name: "FK_AI_KET_QUA_AI_MODEL_MaModel",
                        column: x => x.MaModel,
                        principalTable: "AI_MODEL",
                        principalColumn: "MaModel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AI_KET_QUA_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CHI_PHI",
                columns: table => new
                {
                    MaChiPhi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    NganSach = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    MoTaChiPhi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayCapNhatCP = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TrangThaiChiPhi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHI_PHI", x => x.MaChiPhi);
                    table.ForeignKey(
                        name: "FK_CHI_PHI_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CHI_PHI_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DANH_GIA_DU_AN",
                columns: table => new
                {
                    MaDanhGiaDuAn = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    DiemTongDanhGiaDA = table.Column<int>(type: "int", nullable: true),
                    NhanXetTongDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDanhGiaDA = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_GIA_DU_AN", x => x.MaDanhGiaDuAn);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_DU_AN_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_DU_AN_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DANH_GIA_NHAN_VIEN",
                columns: table => new
                {
                    MaDanhGiaNhanVien = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNguoiDanhGia = table.Column<int>(type: "int", nullable: false),
                    DiemTongDanhGiaNV = table.Column<int>(type: "int", nullable: true),
                    NgayDanhGiaNV = table.Column<DateTime>(type: "datetime2", nullable: true),
                    XepLoai = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_GIA_NHAN_VIEN", x => x.MaDanhGiaNhanVien);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_NHAN_VIEN_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_NHAN_VIEN_NHAN_VIEN_MaNguoiDanhGia",
                        column: x => x.MaNguoiDanhGia,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DANH_GIA_NHAN_VIEN_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DANH_MUC_CONG_VIEC",
                columns: table => new
                {
                    MaDanhMucCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    TenDanhMucCV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaDanhMucCV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTaoDMCV = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DANH_MUC_CONG_VIEC", x => x.MaDanhMucCV);
                    table.ForeignKey(
                        name: "FK_DANH_MUC_CONG_VIEC_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FILE_DU_AN",
                columns: table => new
                {
                    MaFileDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    TenFileDA = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuongDanFileDA = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_DU_AN", x => x.MaFileDA);
                    table.ForeignKey(
                        name: "FK_FILE_DU_AN_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PHONG_CHAT",
                columns: table => new
                {
                    MaPhongChat = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    TenPhong = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHONG_CHAT", x => x.MaPhongChat);
                    table.ForeignKey(
                        name: "FK_PHONG_CHAT_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THANH_VIEN_DU_AN",
                columns: table => new
                {
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    VaiTroTrongDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayThamGiaDuAn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THANH_VIEN_DU_AN", x => new { x.MaDuAn, x.MaNhanVien });
                    table.ForeignKey(
                        name: "FK_THANH_VIEN_DU_AN_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THANH_VIEN_DU_AN_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CT_DANH_GIA_DU_AN",
                columns: table => new
                {
                    MaChiTietDGDA = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDanhGiaDuAn = table.Column<int>(type: "int", nullable: false),
                    NhanXetDuAn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TieuChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemDanhGiaDA = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DANH_GIA_DU_AN", x => x.MaChiTietDGDA);
                    table.ForeignKey(
                        name: "FK_CT_DANH_GIA_DU_AN_DANH_GIA_DU_AN_MaDanhGiaDuAn",
                        column: x => x.MaDanhGiaDuAn,
                        principalTable: "DANH_GIA_DU_AN",
                        principalColumn: "MaDanhGiaDuAn",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CONG_VIEC",
                columns: table => new
                {
                    MaCongViec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDanhMucCV = table.Column<int>(type: "int", nullable: false),
                    MaTrangThai = table.Column<int>(type: "int", nullable: false),
                    MaMucDo = table.Column<int>(type: "int", nullable: false),
                    TenCongViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaCongViec = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBatDauCongViec = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucCVDuKien = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThucCVThucTe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTaoCongViec = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CONG_VIEC", x => x.MaCongViec);
                    table.ForeignKey(
                        name: "FK_CONG_VIEC_DANH_MUC_CONG_VIEC_MaDanhMucCV",
                        column: x => x.MaDanhMucCV,
                        principalTable: "DANH_MUC_CONG_VIEC",
                        principalColumn: "MaDanhMucCV",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONG_VIEC_MUC_DO_UU_TIEN_MaMucDo",
                        column: x => x.MaMucDo,
                        principalTable: "MUC_DO_UU_TIEN",
                        principalColumn: "MaMucDo",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CONG_VIEC_TRANG_THAI_MaTrangThai",
                        column: x => x.MaTrangThai,
                        principalTable: "TRANG_THAI",
                        principalColumn: "MaTrangThai",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "THANH_VIEN_PHONG_CHAT",
                columns: table => new
                {
                    MaPhongChat = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    VaiTroTrongPhongChat = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_THANH_VIEN_PHONG_CHAT", x => new { x.MaPhongChat, x.MaNhanVien });
                    table.ForeignKey(
                        name: "FK_THANH_VIEN_PHONG_CHAT_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_THANH_VIEN_PHONG_CHAT_PHONG_CHAT_MaPhongChat",
                        column: x => x.MaPhongChat,
                        principalTable: "PHONG_CHAT",
                        principalColumn: "MaPhongChat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TIN_NHAN",
                columns: table => new
                {
                    MaTinNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhongChat = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NoiDungTinNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIN_NHAN", x => x.MaTinNhan);
                    table.ForeignKey(
                        name: "FK_TIN_NHAN_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TIN_NHAN_PHONG_CHAT_MaPhongChat",
                        column: x => x.MaPhongChat,
                        principalTable: "PHONG_CHAT",
                        principalColumn: "MaPhongChat",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AI_DATASET",
                columns: table => new
                {
                    MaData = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    SoNhanVienDuAn = table.Column<int>(type: "int", nullable: true),
                    ChiPhiThucTe = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ChiPhiDuKien = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TongGioLam = table.Column<double>(type: "float", nullable: true),
                    SoNgayTreTienDo = table.Column<int>(type: "int", nullable: true),
                    SoLanThayDoiNhanSu = table.Column<int>(type: "int", nullable: true),
                    SoNgayDuKienCV = table.Column<int>(type: "int", nullable: true),
                    SoNgayThucTeCV = table.Column<int>(type: "int", nullable: true),
                    SoLanDelay = table.Column<int>(type: "int", nullable: true),
                    TrangThaiCuoi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    NguyenNhanChinh = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_DATASET", x => x.MaData);
                    table.ForeignKey(
                        name: "FK_AI_DATASET_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AI_DATASET_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AI_LICH_SU_DU_DOAN",
                columns: table => new
                {
                    MaAiLichSuDuDoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaModel = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    KetQuaDuDoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianDuDoan = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    DoTinCay = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_LICH_SU_DU_DOAN", x => x.MaAiLichSuDuDoan);
                    table.ForeignKey(
                        name: "FK_AI_LICH_SU_DU_DOAN_AI_MODEL_MaModel",
                        column: x => x.MaModel,
                        principalTable: "AI_MODEL",
                        principalColumn: "MaModel",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AI_LICH_SU_DU_DOAN_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AI_NGUYEN_NHAN",
                columns: table => new
                {
                    MaNguyenNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    LoaiNguyenNhan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaNguyenNhan = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AI_NGUYEN_NHAN", x => x.MaNguyenNhan);
                    table.ForeignKey(
                        name: "FK_AI_NGUYEN_NHAN_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CT_CHI_PHI",
                columns: table => new
                {
                    MaChiTietCP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaChiPhi = table.Column<int>(type: "int", nullable: false),
                    NoiDungChiPhi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoTienDaChi = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NgayChi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LoaiChiPhi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_CHI_PHI", x => x.MaChiTietCP);
                    table.ForeignKey(
                        name: "FK_CT_CHI_PHI_CHI_PHI_MaChiPhi",
                        column: x => x.MaChiPhi,
                        principalTable: "CHI_PHI",
                        principalColumn: "MaChiPhi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CT_CHI_PHI_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CT_CONG_VIEC",
                columns: table => new
                {
                    MaChiTietCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    NoiDungChiTietCV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_CONG_VIEC", x => x.MaChiTietCV);
                    table.ForeignKey(
                        name: "FK_CT_CONG_VIEC_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FILE_CONG_VIEC",
                columns: table => new
                {
                    MaFileCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    TenFileCV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuongDanFileCV = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_CONG_VIEC", x => x.MaFileCV);
                    table.ForeignKey(
                        name: "FK_FILE_CONG_VIEC_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KPI_CONG_VIEC",
                columns: table => new
                {
                    MaKpi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    TenKpi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MoTaKpi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemTongKpi = table.Column<int>(type: "int", nullable: true),
                    LoaiKpi = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KPI_CONG_VIEC", x => x.MaKpi);
                    table.ForeignKey(
                        name: "FK_KPI_CONG_VIEC_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_CHI_PHI",
                columns: table => new
                {
                    MaNhatKyCP = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChiPhi = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    SoTienNKCP = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    HanhDongNKCP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianNKCP = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_CHI_PHI", x => x.MaNhatKyCP);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_CHI_PHI_CHI_PHI_MaChiPhi",
                        column: x => x.MaChiPhi,
                        principalTable: "CHI_PHI",
                        principalColumn: "MaChiPhi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_CHI_PHI_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_CHI_PHI_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NHAT_KY_NHAN_SU",
                columns: table => new
                {
                    MaNhatKyNS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaDuAn = table.Column<int>(type: "int", nullable: false),
                    HanhDongNKNS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianNKNS = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NHAT_KY_NHAN_SU", x => x.MaNhatKyNS);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_NHAN_SU_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_NHAN_SU_DU_AN_MaDuAn",
                        column: x => x.MaDuAn,
                        principalTable: "DU_AN",
                        principalColumn: "MaDuAn",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NHAT_KY_NHAN_SU_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PHAN_CONG_CONG_VIEC",
                columns: table => new
                {
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayGiaoViec = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PHAN_CONG_CONG_VIEC", x => new { x.MaNhanVien, x.MaCongViec });
                    table.ForeignKey(
                        name: "FK_PHAN_CONG_CONG_VIEC_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PHAN_CONG_CONG_VIEC_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TIEN_DO_CONG_VIEC",
                columns: table => new
                {
                    MaTienDo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCongViec = table.Column<int>(type: "int", nullable: false),
                    MaNhanVien = table.Column<int>(type: "int", nullable: false),
                    PhanTram = table.Column<int>(type: "int", nullable: true),
                    GhiChuTienDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "GETDATE()"),
                    TrangThaiTienDo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TIEN_DO_CONG_VIEC", x => x.MaTienDo);
                    table.ForeignKey(
                        name: "FK_TIEN_DO_CONG_VIEC_CONG_VIEC_MaCongViec",
                        column: x => x.MaCongViec,
                        principalTable: "CONG_VIEC",
                        principalColumn: "MaCongViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TIEN_DO_CONG_VIEC_NHAN_VIEN_MaNhanVien",
                        column: x => x.MaNhanVien,
                        principalTable: "NHAN_VIEN",
                        principalColumn: "MaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CT_DANH_GIA_NHAN_VIEN",
                columns: table => new
                {
                    MaChiTietDGNV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKpi = table.Column<int>(type: "int", nullable: false),
                    MaDanhGiaNhanVien = table.Column<int>(type: "int", nullable: false),
                    NoiDungDanhGiaNhanVien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemDanhGiaNV = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_DANH_GIA_NHAN_VIEN", x => x.MaChiTietDGNV);
                    table.ForeignKey(
                        name: "FK_CT_DANH_GIA_NHAN_VIEN_DANH_GIA_NHAN_VIEN_MaDanhGiaNhanVien",
                        column: x => x.MaDanhGiaNhanVien,
                        principalTable: "DANH_GIA_NHAN_VIEN",
                        principalColumn: "MaDanhGiaNhanVien",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CT_DANH_GIA_NHAN_VIEN_KPI_CONG_VIEC_MaKpi",
                        column: x => x.MaKpi,
                        principalTable: "KPI_CONG_VIEC",
                        principalColumn: "MaKpi",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CT_KPI_CONG_VIEC",
                columns: table => new
                {
                    MaChiTietKPICV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaKpi = table.Column<int>(type: "int", nullable: false),
                    TinhTrangKpi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiemKpi = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CT_KPI_CONG_VIEC", x => x.MaChiTietKPICV);
                    table.ForeignKey(
                        name: "FK_CT_KPI_CONG_VIEC_KPI_CONG_VIEC_MaKpi",
                        column: x => x.MaKpi,
                        principalTable: "KPI_CONG_VIEC",
                        principalColumn: "MaKpi",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FILE_TIEN_DO_CONG_VIEC",
                columns: table => new
                {
                    MaFileTDCV = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaTienDo = table.Column<int>(type: "int", nullable: false),
                    TenFileTDCV = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuongDanFileTDCV = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FILE_TIEN_DO_CONG_VIEC", x => x.MaFileTDCV);
                    table.ForeignKey(
                        name: "FK_FILE_TIEN_DO_CONG_VIEC_TIEN_DO_CONG_VIEC_MaTienDo",
                        column: x => x.MaTienDo,
                        principalTable: "TIEN_DO_CONG_VIEC",
                        principalColumn: "MaTienDo",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AI_DATASET_MaCongViec",
                table: "AI_DATASET",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_AI_DATASET_MaDuAn",
                table: "AI_DATASET",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_AI_KET_QUA_MaDuAn",
                table: "AI_KET_QUA",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_AI_KET_QUA_MaModel",
                table: "AI_KET_QUA",
                column: "MaModel");

            migrationBuilder.CreateIndex(
                name: "IX_AI_LICH_SU_DU_DOAN_MaCongViec",
                table: "AI_LICH_SU_DU_DOAN",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_AI_LICH_SU_DU_DOAN_MaModel",
                table: "AI_LICH_SU_DU_DOAN",
                column: "MaModel");

            migrationBuilder.CreateIndex(
                name: "IX_AI_NGUYEN_NHAN_MaCongViec",
                table: "AI_NGUYEN_NHAN",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CHI_PHI_MaDuAn",
                table: "CHI_PHI",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_CHI_PHI_MaNhanVien",
                table: "CHI_PHI",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_CONG_VIEC_MaDanhMucCV",
                table: "CONG_VIEC",
                column: "MaDanhMucCV");

            migrationBuilder.CreateIndex(
                name: "IX_CONG_VIEC_MaMucDo",
                table: "CONG_VIEC",
                column: "MaMucDo");

            migrationBuilder.CreateIndex(
                name: "IX_CONG_VIEC_MaTrangThai",
                table: "CONG_VIEC",
                column: "MaTrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_CT_CHI_PHI_MaChiPhi",
                table: "CT_CHI_PHI",
                column: "MaChiPhi");

            migrationBuilder.CreateIndex(
                name: "IX_CT_CHI_PHI_MaCongViec",
                table: "CT_CHI_PHI",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_CT_CONG_VIEC_MaCongViec",
                table: "CT_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DANH_GIA_DU_AN_MaDanhGiaDuAn",
                table: "CT_DANH_GIA_DU_AN",
                column: "MaDanhGiaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DANH_GIA_NHAN_VIEN_MaDanhGiaNhanVien",
                table: "CT_DANH_GIA_NHAN_VIEN",
                column: "MaDanhGiaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_CT_DANH_GIA_NHAN_VIEN_MaKpi",
                table: "CT_DANH_GIA_NHAN_VIEN",
                column: "MaKpi");

            migrationBuilder.CreateIndex(
                name: "IX_CT_KPI_CONG_VIEC_MaKpi",
                table: "CT_KPI_CONG_VIEC",
                column: "MaKpi");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_DU_AN_MaDuAn",
                table: "DANH_GIA_DU_AN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_DU_AN_MaNhanVien",
                table: "DANH_GIA_DU_AN",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_MaDuAn",
                table: "DANH_GIA_NHAN_VIEN",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_MaNguoiDanhGia",
                table: "DANH_GIA_NHAN_VIEN",
                column: "MaNguoiDanhGia");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_GIA_NHAN_VIEN_MaNhanVien",
                table: "DANH_GIA_NHAN_VIEN",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_MUC_CHUC_NANG_MaManHinh",
                table: "DANH_MUC_CHUC_NANG",
                column: "MaManHinh");

            migrationBuilder.CreateIndex(
                name: "IX_DANH_MUC_CONG_VIEC_MaDuAn",
                table: "DANH_MUC_CONG_VIEC",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DU_AN_MaLoaiDuAn",
                table: "DU_AN",
                column: "MaLoaiDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_DU_AN_MaNhanVien",
                table: "DU_AN",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_DU_AN_MaTrangThai",
                table: "DU_AN",
                column: "MaTrangThai");

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
                name: "IX_KPI_CONG_VIEC_MaCongViec",
                table: "KPI_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_NHAN_VIEN_Id",
                table: "NHAN_VIEN",
                column: "Id",
                unique: true,
                filter: "[Id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_NHAN_VIEN_MaVaiTro",
                table: "NHAN_VIEN",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_CHI_PHI_MaChiPhi",
                table: "NHAT_KY_CHI_PHI",
                column: "MaChiPhi");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_CHI_PHI_MaCongViec",
                table: "NHAT_KY_CHI_PHI",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_CHI_PHI_MaNhanVien",
                table: "NHAT_KY_CHI_PHI",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_NHAN_SU_MaCongViec",
                table: "NHAT_KY_NHAN_SU",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_NHAN_SU_MaDuAn",
                table: "NHAT_KY_NHAN_SU",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_NHAT_KY_NHAN_SU_MaNhanVien",
                table: "NHAT_KY_NHAN_SU",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_PHAN_CONG_CONG_VIEC_MaCongViec",
                table: "PHAN_CONG_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_PHAN_QUYEN_MaChucNang",
                table: "PHAN_QUYEN",
                column: "MaChucNang");

            migrationBuilder.CreateIndex(
                name: "IX_PHONG_CHAT_MaDuAn",
                table: "PHONG_CHAT",
                column: "MaDuAn");

            migrationBuilder.CreateIndex(
                name: "IX_THANH_VIEN_DU_AN_MaNhanVien",
                table: "THANH_VIEN_DU_AN",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_THANH_VIEN_NHOM_MaNhanVien",
                table: "THANH_VIEN_NHOM",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_THANH_VIEN_PHONG_CHAT_MaNhanVien",
                table: "THANH_VIEN_PHONG_CHAT",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TIEN_DO_CONG_VIEC_MaCongViec",
                table: "TIEN_DO_CONG_VIEC",
                column: "MaCongViec");

            migrationBuilder.CreateIndex(
                name: "IX_TIEN_DO_CONG_VIEC_MaNhanVien",
                table: "TIEN_DO_CONG_VIEC",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TIN_NHAN_MaNhanVien",
                table: "TIN_NHAN",
                column: "MaNhanVien");

            migrationBuilder.CreateIndex(
                name: "IX_TIN_NHAN_MaPhongChat",
                table: "TIN_NHAN",
                column: "MaPhongChat");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AI_DATASET");

            migrationBuilder.DropTable(
                name: "AI_KET_QUA");

            migrationBuilder.DropTable(
                name: "AI_LICH_SU_DU_DOAN");

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
                name: "CT_CHI_PHI");

            migrationBuilder.DropTable(
                name: "CT_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "CT_DANH_GIA_DU_AN");

            migrationBuilder.DropTable(
                name: "CT_DANH_GIA_NHAN_VIEN");

            migrationBuilder.DropTable(
                name: "CT_KPI_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "FILE_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "FILE_DU_AN");

            migrationBuilder.DropTable(
                name: "FILE_TIEN_DO_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "NHAT_KY_CHI_PHI");

            migrationBuilder.DropTable(
                name: "NHAT_KY_NHAN_SU");

            migrationBuilder.DropTable(
                name: "PHAN_CONG_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "PHAN_QUYEN");

            migrationBuilder.DropTable(
                name: "THANH_VIEN_DU_AN");

            migrationBuilder.DropTable(
                name: "THANH_VIEN_NHOM");

            migrationBuilder.DropTable(
                name: "THANH_VIEN_PHONG_CHAT");

            migrationBuilder.DropTable(
                name: "TIN_NHAN");

            migrationBuilder.DropTable(
                name: "AI_MODEL");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "DANH_GIA_DU_AN");

            migrationBuilder.DropTable(
                name: "DANH_GIA_NHAN_VIEN");

            migrationBuilder.DropTable(
                name: "KPI_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "TIEN_DO_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "CHI_PHI");

            migrationBuilder.DropTable(
                name: "DANH_MUC_CHUC_NANG");

            migrationBuilder.DropTable(
                name: "NHOM_NHAN_VIEN");

            migrationBuilder.DropTable(
                name: "PHONG_CHAT");

            migrationBuilder.DropTable(
                name: "CONG_VIEC");

            migrationBuilder.DropTable(
                name: "DANH_MUC_MAN_HINH");

            migrationBuilder.DropTable(
                name: "DANH_MUC_CONG_VIEC");

            migrationBuilder.DropTable(
                name: "MUC_DO_UU_TIEN");

            migrationBuilder.DropTable(
                name: "DU_AN");

            migrationBuilder.DropTable(
                name: "LOAI_DU_AN");

            migrationBuilder.DropTable(
                name: "NHAN_VIEN");

            migrationBuilder.DropTable(
                name: "TRANG_THAI");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "VAI_TRO");
        }
    }
}
