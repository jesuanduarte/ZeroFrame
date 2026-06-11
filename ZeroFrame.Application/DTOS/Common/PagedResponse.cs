namespace ZeroFrame.Application.DTOS.Common
{
    // Resposta padronizada para listagens paginadas mantendo os DTOs atuais dentro de Items.
    public class PagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        public static PagedResponse<T> Create(List<T> items, int totalItems, PaginationParams paginationParams)
        {
            var totalPages = (int)Math.Ceiling(totalItems / (double)paginationParams.PageSize);

            return new PagedResponse<T>
            {
                Items = items,
                TotalItems = totalItems,
                PageNumber = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalPages = totalPages,
                HasPreviousPage = paginationParams.PageNumber > 1,
                HasNextPage = paginationParams.PageNumber < totalPages
            };
        }
    }
}
