namespace QuanLyDuAn.Models.Entities;

public partial class Aspnetroleclaims
{
    public int Id { get; set; }
    public string Asp_Id { get; set; } = null!;
    public int MaDanhMucQuyen { get; set; }
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}
