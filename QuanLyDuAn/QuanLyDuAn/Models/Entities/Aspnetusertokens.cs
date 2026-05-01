namespace QuanLyDuAn.Models.Entities;

public partial class Aspnetusertokens
{
    public string Id { get; set; } = null!;
    public string LoginProvider { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Value { get; set; }
}
