namespace ZeroFrame.Application.DTOS.Common
{
    // Parametros recebidos via query string e normalizados para evitar paginas invalidas ou muito grandes.
    public class PaginationParams
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;
        public const int MaxPageSize = 50;

        private int _pageNumber = DefaultPageNumber;
        private int _pageSize = DefaultPageSize;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? DefaultPageNumber : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1
                ? DefaultPageSize
                : Math.Min(value, MaxPageSize);
        }

        public int Skip => (PageNumber - 1) * PageSize;
    }
}
