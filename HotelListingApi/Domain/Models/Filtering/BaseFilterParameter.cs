namespace HotelListingApi.Domain.Models.Filtering
{
    public class BaseFilterParameter
    {
        public string? Search { get; set; }
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;

    }
}
