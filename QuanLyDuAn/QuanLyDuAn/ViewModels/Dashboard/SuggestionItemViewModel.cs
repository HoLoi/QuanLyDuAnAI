namespace QuanLyDuAn.ViewModels.Dashboard
{
    public class SuggestionItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Priority { get; set; } = "Thấp";
        public string? ActionText { get; set; }
        public string? ActionUrl { get; set; }
    }
}
