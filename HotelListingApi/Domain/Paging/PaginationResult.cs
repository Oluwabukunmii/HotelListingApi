namespace HotelListingApi.Domain.Paging
{
    public class PaginationResult<T>
    {
      
            public IEnumerable<T> Data { get; set; } = [];
            public PaginationMetaData Metadata { get; set; } = new();
        
    }
}
