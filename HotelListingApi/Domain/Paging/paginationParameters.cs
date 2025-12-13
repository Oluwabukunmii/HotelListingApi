using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.Domain.Paging
{
    public class paginationParameters
    {
        private const int MaxPageSize = 50;
        private int pageSize = 10;

        [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, MaxPageSize, ErrorMessage = "Page size must be between 1 and 50")]
        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string? SortBy { get; set; } = null;  // e.g. sort by Name, Price, etc.
        public string SortOrder { get; set; } = "asc"; // asc or desc
    }
}
