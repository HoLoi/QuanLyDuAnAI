namespace QuanLyDuAn.ViewModels.Common
{
    public class PagedResultViewModel<T>
    {
        public List<T> Items { get; set; } = new();
        public PaginationViewModel Pagination { get; set; } = new();
    }
}
