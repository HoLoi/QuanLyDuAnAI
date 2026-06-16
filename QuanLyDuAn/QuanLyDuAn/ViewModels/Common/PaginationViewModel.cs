namespace QuanLyDuAn.ViewModels.Common
{
    public class PaginationViewModel
    {
        public static readonly int[] AllowedPageSizes = { 10, 20, 50, 100 };
        public const int DefaultPageSize = 20;

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = DefaultPageSize;
        public int TotalItems { get; set; }

        public int TotalPages =>
            PageSize <= 0
                ? 0
                : (int)Math.Ceiling(TotalItems / (double)PageSize);

        public int FromItem =>
            TotalItems == 0
                ? 0
                : ((PageNumber - 1) * PageSize) + 1;

        public int ToItem =>
            TotalItems == 0
                ? 0
                : Math.Min(PageNumber * PageSize, TotalItems);

        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public int Skip => (PageNumber - 1) * PageSize;

        public static int NormalizePageSize(int pageSize)
        {
            return AllowedPageSizes.Contains(pageSize) ? pageSize : DefaultPageSize;
        }

        public static int NormalizePageNumber(int pageNumber)
        {
            return pageNumber < 1 ? 1 : pageNumber;
        }

        public static PaginationViewModel Create(int pageNumber, int pageSize, int totalItems)
        {
            var normalizedPageSize = NormalizePageSize(pageSize);
            var normalizedPageNumber = NormalizePageNumber(pageNumber);
            var totalPages = normalizedPageSize <= 0
                ? 0
                : (int)Math.Ceiling(totalItems / (double)normalizedPageSize);

            if (totalPages > 0 && normalizedPageNumber > totalPages)
            {
                normalizedPageNumber = totalPages;
            }

            return new PaginationViewModel
            {
                PageNumber = normalizedPageNumber,
                PageSize = normalizedPageSize,
                TotalItems = Math.Max(totalItems, 0)
            };
        }

        public PaginationViewModel WithTotalItems(int totalItems)
        {
            return Create(PageNumber, PageSize, totalItems);
        }
    }
}
