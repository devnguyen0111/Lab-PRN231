using Models.Models;
using Repositorys.DTOs.CategoryDTO;
using Repositorys.PaginatedList;

namespace Services.IService
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<PaginatedList<Category>> GetCategoriesPaginatedAsync(int pageNumber, int pageSize);
        Task<Category> CreateCategoryAsync(CreateCategoryDTO categoryDTO);
        Task<bool> UpdateCategoryAsync(int id, UpdateCategoryDTO categoryDTO);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> HasOrchidsAsync(int id);
    }
}