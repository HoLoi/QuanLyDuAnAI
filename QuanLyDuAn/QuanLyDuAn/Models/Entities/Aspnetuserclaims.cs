namespace QuanLyDuAn.Models.Entities;

public partial class Aspnetuserclaims
{
    public int Id { get; set; }
    public string Asp_Id { get; set; } = null!;
    public string? ClaimType { get; set; }
    public string? ClaimValue { get; set; }
}
