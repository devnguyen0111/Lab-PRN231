using Microsoft.EntityFrameworkCore;
using Models.Models;
using Repositorys.DTOs.CategoryDTO;
using Repositorys.IRepositories;
using Repositorys.PaginatedList;
using Services.IService;

namespace Services.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly IUOW _unitOfWork;
        private readonly IGenericRepository<Category> _categoryRepository;

        public CategoryService(IUOW unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = _unitOfWork.GetRepository<Category>();
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.Entities
                .Include(c => c.Orchids)
                .ToListAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.Entities
                .Include(c => c.Orchids)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<PaginatedList<Category>> GetCategoriesPaginatedAsync(int pageNumber, int pageSize)
        {
            var query = _categoryRepository.Entities
                .Include(c => c.Orchids)
                .OrderBy(c => c.CategoryName);

            return await _categoryRepository.GetPagging(query, pageNumber, pageSize);
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDTO categoryDTO)
        {
            if (await _categoryRepository.Entities.AnyAsync(c => c.CategoryName == categoryDTO.CategoryName))
                throw new InvalidOperationException("Category name already exists");

            var category = new Category
            {
                CategoryId = await GenerateNewId(),
                CategoryName = categoryDTO.CategoryName
            };

            await _categoryRepository.InsertAsync(category);
            await _unitOfWork.SaveAsync();

            return category;
        }

        public async Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDTO categoryDTO)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            if (await _categoryRepository.Entities.AnyAsync(c =>
                c.CategoryName == categoryDTO.CategoryName && c.CategoryId != id))
                throw new InvalidOperationException("Category name already exists");

            category.CategoryName = categoryDTO.CategoryName;

            _categoryRepository.Update(category);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            if (await HasOrchidsAsync(id))
                throw new InvalidOperationException("Cannot delete category with associated orchids");

            _categoryRepository.Delete(category);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> HasOrchidsAsync(int id)
        {
            var category = await _categoryRepository.Entities
                .Include(c => c.Orchids)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            return category?.Orchids.Any() ?? false;
        }

        private async Task<int> GenerateNewId()
        {
            var maxId = await _categoryRepository.Entities
                .MaxAsync(c => (int?)c.CategoryId) ?? 0;
            return maxId + 1;
        }
    }
}