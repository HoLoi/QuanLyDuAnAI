namespace QuanLyDuAn.Models.Entities;

public partial class Aspnetuserlogins
{
    public string LoginProvider { get; set; } = null!;
    public string ProviderKey { get; set; } = null!;
    public string Id { get; set; } = null!;
    public string? ProviderDisplayName { get; set; }
}
