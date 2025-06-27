using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositorys.DTOs.OrchidDTO;
using Repositorys.IRepositories;
using Repositorys.PaginatedList;
using Services.IService;

namespace Services.Service
{
    public class OrchidService : IOrchidService
    {
        private readonly IUOW _unitOfWork;
        private readonly IGenericRepository<Orchid> _orchidRepository;
        private readonly IGenericRepository<Category> _categoryRepository;

        public OrchidService(IUOW unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _orchidRepository = _unitOfWork.GetRepository<Orchid>();
            _categoryRepository = _unitOfWork.GetRepository<Category>();
        }

        public async Task<IEnumerable<Orchid>> GetAllOrchidsAsync()
        {
            return await _orchidRepository.Entities
                .Include(o => o.Category)
                .ToListAsync();
        }

        public async Task<Orchid?> GetOrchidByIdAsync(int id)
        {
            return await _orchidRepository.Entities
                .Include(o => o.Category)
                .FirstOrDefaultAsync(o => o.OrchidId == id);
        }

        public async Task<PaginatedList<Orchid>> GetOrchidsPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _orchidRepository.Entities
                .Include(o => o.Category)
                .OrderBy(o => o.OrchidName);

            return await _orchidRepository.GetPagging(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Orchid>> GetOrchidsByCategoryAsync(int categoryId)
        {
            return await _orchidRepository.Entities
                .Include(o => o.Category)
                .Where(o => o.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Orchid> CreateOrchidAsync(CreateOrchidDTO orchidDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(orchidDTO.CategoryId)
                ?? throw new InvalidOperationException("Category not found");

            var orchid = new Orchid
            {
                OrchidId = await GenerateNewId(),
                OrchidName = orchidDTO.OrchidName,
                OrchidDescription = orchidDTO.OrchidDescription ?? "",
                Price = orchidDTO.Price,
                IsNatural = orchidDTO.IsNatural,
                OrchidUrl = orchidDTO.OrchidUrl ?? "",
                CategoryId = orchidDTO.CategoryId
            };

            await _orchidRepository.InsertAsync(orchid);
            await _unitOfWork.SaveAsync();

            return orchid;
        }

        public async Task<bool> UpdateOrchidAsync(int id, UpdateOrchidDTO orchidDTO)
        {
            var orchid = await _orchidRepository.GetByIdAsync(id);
            if (orchid == null) return false;

            if (!await _categoryRepository.Entities.AnyAsync(c => c.CategoryId == orchidDTO.CategoryId))
                throw new InvalidOperationException("Category not found");

            orchid.OrchidName = orchidDTO.OrchidName;
            orchid.OrchidDescription = orchidDTO.OrchidDescription ?? "";
            orchid.Price = orchidDTO.Price;
            orchid.IsNatural = orchidDTO.IsNatural;
            orchid.OrchidUrl = orchidDTO.OrchidUrl ?? "";
            orchid.CategoryId = orchidDTO.CategoryId;

            _orchidRepository.Update(orchid);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteOrchidAsync(int id)
        {
            var orchid = await _orchidRepository.GetByIdAsync(id);
            if (orchid == null) return false;

            _orchidRepository.Delete(orchid);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<Orchid>> SearchOrchidsAsync(string searchTerm)
        {
            return await _orchidRepository.Entities
                .Include(o => o.Category)
                .Where(o => o.OrchidName.Contains(searchTerm) ||
                           o.OrchidDescription.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<PaginatedList<Orchid>> FilterOrchidsAsync(
            string? searchTerm = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? isNatural = null,
            int? categoryId = null,
            string? sortBy = null,
            bool ascending = true,
            int pageNumber = 1,
            int pageSize = 10)
        {
            // Start with a base query that includes the category
            var query = _orchidRepository.Entities
                .Include(o => o.Category)
                .AsQueryable();

            // Apply search filter (case-insensitive)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(o => 
                    EF.Functions.Like(o.OrchidName.ToLower(), $"%{searchTerm}%") || 
                    EF.Functions.Like(o.OrchidDescription.ToLower(), $"%{searchTerm}%") ||
                    EF.Functions.Like(o.Category.CategoryName.ToLower(), $"%{searchTerm}%"));
            }

            // Apply price range filter
            if (minPrice.HasValue)
                query = query.Where(o => o.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                query = query.Where(o => o.Price <= maxPrice.Value);

            // Apply type filter
            if (isNatural.HasValue)
                query = query.Where(o => o.IsNatural == isNatural.Value);

            // Apply category filter
            if (categoryId.HasValue)
                query = query.Where(o => o.CategoryId == categoryId.Value);

            // Apply sorting with multiple options
            query = sortBy?.ToLower() switch
            {
                "name" => ascending 
                    ? query.OrderBy(o => o.OrchidName) 
                    : query.OrderByDescending(o => o.OrchidName),
                
                "price" => ascending 
                    ? query.OrderBy(o => o.Price) 
                    : query.OrderByDescending(o => o.Price),
                
                "category" => ascending 
                    ? query.OrderBy(o => o.Category.CategoryName).ThenBy(o => o.OrchidName)
                    : query.OrderByDescending(o => o.Category.CategoryName).ThenBy(o => o.OrchidName),
                
                "natural" => ascending 
                    ? query.OrderBy(o => o.IsNatural).ThenBy(o => o.OrchidName)
                    : query.OrderByDescending(o => o.IsNatural).ThenBy(o => o.OrchidName),
                
                _ => query.OrderBy(o => o.OrchidName)
            };

            // Return paginated results
            return await _orchidRepository.GetPagging(query, pageNumber, pageSize);
        }

        public async Task<IEnumerable<Orchid>> GetOrchidsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _orchidRepository.Entities
                .Include(o => o.Category)
                .Where(o => o.Price >= minPrice && o.Price <= maxPrice)
                .OrderBy(o => o.Price)
                .ToListAsync();
        }

        public async Task<IEnumerable<Orchid>> GetOrchidsByTypeAsync(bool isNatural)
        {
            return await _orchidRepository.Entities
                .Include(o => o.Category)
                .Where(o => o.IsNatural == isNatural)
                .OrderBy(o => o.OrchidName)
                .ToListAsync();
        }

        public async Task<Dictionary<string, int>> GetOrchidsCountByCategory()
        {
            return await _orchidRepository.Entities
                .Include(o => o.Category)
                .GroupBy(o => o.Category.CategoryName)
                .ToDictionaryAsync(
                    g => g.Key ?? "Uncategorized",
                    g => g.Count()
                );
        }

        public async Task<Dictionary<decimal, int>> GetOrchidsPriceDistribution(int numberOfRanges = 5)
        {
            var orchids = await _orchidRepository.Entities.ToListAsync();
            if (!orchids.Any())
                return new Dictionary<decimal, int>();

            var minPrice = orchids.Min(o => o.Price);
            var maxPrice = orchids.Max(o => o.Price);
            var rangeSize = (maxPrice - minPrice) / numberOfRanges;

            var distribution = new Dictionary<decimal, int>();
            for (int i = 0; i < numberOfRanges; i++)
            {
                var rangeStart = minPrice + (i * rangeSize);
                var rangeEnd = rangeStart + rangeSize;
                var count = orchids.Count(o => o.Price >= rangeStart && o.Price < (i == numberOfRanges - 1 ? rangeEnd + 1 : rangeEnd));
                distribution.Add(rangeStart, count);
            }

            return distribution;
        }

        private async Task<int> GenerateNewId()
        {
            var maxId = await _orchidRepository.Entities
                .MaxAsync(o => (int?)o.OrchidId) ?? 0;
            return maxId + 1;
        }
    }
}