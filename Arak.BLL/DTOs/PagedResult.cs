namespace Arak.BLL.DTOs
{
    /// <summary>
    /// Paged query result wrapper — generic container for paginated API responses.
    /// Matches BACKEND.md §3 pagination contract.
    /// Frontend reads: response.data.data (array) + response.data.total (count).
    /// Also includes "items" alias for json-server v1 compatibility.
    /// </summary>
    public class PagedResult<T>
    {
        public IEnumerable<T> Data  { get; init; } = [];

        /// <summary>Total number of records matching the filter.</summary>
        public int Total { get; init; }

        /// <summary>
        /// Alias for Total — json-server v1 convention.
        /// Frontend reads response.data.items or response.data.total.
        /// </summary>
        public int Items => Total;

        public int Page  { get; init; }
        public int PageSize { get; init; }
    }
}
