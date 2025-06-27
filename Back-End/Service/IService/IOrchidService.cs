using Models.Models;
using Repositorys.DTOs.OrchidDTO;
using Repositorys.PaginatedList;

namespace Services.IService
{
    public interface IOrchidService
    {
        Task<IEnumerable<Orchid>> GetAllOrchidsAsync();
        Task<Orchid?> GetOrchidByIdAsync(int id);
        Task<PaginatedList<Orchid>> GetOrchidsPaginatedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<Orchid>> GetOrchidsByCategoryAsync(int categoryId);
        Task<Orchid> CreateOrchidAsync(CreateOrchidDTO orchidDTO);
        Task<bool> UpdateOrchidAsync(int id, UpdateOrchidDTO orchidDTO);
        Task<bool> DeleteOrchidAsync(int id);
        Task<IEnumerable<Orchid>> SearchOrchidsAsync(string searchTerm);

        // New filter and search methods
        Task<PaginatedList<Orchid>> FilterOrchidsAsync(
            string? searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isNatural = null,
            int? categoryId = null,
            string? sortBy = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10);

        Task<IEnumerable<Orchid>> GetOrchidsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Orchid>> GetOrchidsByTypeAsync(bool isNatural);
        Task<Dictionary<string, int>> GetOrchidsCountByCategory();
        Task<Dictionary<decimal, int>> GetOrchidsPriceDistribution(int numberOfRanges = 5);
    }
}