using System.Linq.Dynamic.Core;
using HotelListingApi.Domain.Paging;
using Microsoft.EntityFrameworkCore;


namespace HotelListingApi.Domain.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PaginationResult<T>> ToPaginationResultAsync<T>(
           this IQueryable<T> source,
           paginationParameters paginationParameters, Dictionary<string, string> allowedSortColumns,
             string defaultSortColumn) 
        {
            if (paginationParameters == null)
                throw new ArgumentNullException(nameof(paginationParameters));

            // Ensure valid pagination values
            var pageNumber = paginationParameters.PageNumber < 1 ? 1 : paginationParameters.PageNumber;
            var pageSize = paginationParameters.PageSize < 1 ? 10 : paginationParameters.PageSize;

            // Optional dynamic ordering
            if (!string.IsNullOrWhiteSpace(paginationParameters.SortBy))
            {
                var sort = $"{paginationParameters.SortBy} {paginationParameters.SortOrder}";
                source = source.OrderBy(sort);
            }

            var totalCount = await source.CountAsync(); //Counts all items in the query before pagination.

            var items = await source
                .Skip((pageNumber - 1) * pageSize)  //Purpose: It tells the query how many items to skip from the start.
                .Take(pageSize) //Purpose: It tells the query how many items to fetch after skipping.
                .ToListAsync();

            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var metadata = new PaginationMetaData
            {
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages,
                HasNext = pageNumber < totalPages,
                HasPrevious = pageNumber > 1
            };

            return new PaginationResult<T>
            {
                Data = items,
                Metadata = metadata
            };
        }
    }
}
