using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Repositorys.DTOs.CategoryDTO;
using Repositorys.PaginatedList;
using Services.IService;

namespace Lab_Orchid_SE161629_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Category>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(BaseResponseModel<IEnumerable<Category>>.OkResponseModel(categories));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseModel<Category>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<Category>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound(BaseResponseModel<Category>.NotFoundResponseModel(null, new { message = "Category not found" }));

            return Ok(BaseResponseModel<Category>.OkResponseModel(category));
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(BaseResponseModel<PaginatedList<Category>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategoriesPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var categories = await _categoryService.GetCategoriesPaginatedAsync(pageNumber, pageSize);
            return Ok(BaseResponseModel<PaginatedList<Category>>.OkResponseModel(categories));
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseModel<Category>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseModel<Category>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO categoryDTO)
        {
            try
            {
                var category = await _categoryService.CreateCategoryAsync(categoryDTO);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.CategoryId },
                    BaseResponseModel<Category>.OkResponseModel(category));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<Category>.BadRequestResponseModel(null, new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO categoryDTO)
        {
            try
            {
                var result = await _categoryService.UpdateCategoryAsync(id, categoryDTO);
                if (!result)
                    return NotFound(BaseResponseModel<string>.NotFoundResponseModel(null, new { message = "Category not found" }));

                return Ok(BaseResponseModel<string>.OkResponseModel(null, new { message = "Category updated successfully" }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var result = await _categoryService.DeleteCategoryAsync(id);
                if (!result)
                    return NotFound(BaseResponseModel<string>.NotFoundResponseModel(null, new { message = "Category not found" }));

                return Ok(BaseResponseModel<string>.OkResponseModel(null, new { message = "Category deleted successfully" }));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<string>.BadRequestResponseModel(null, new { message = ex.Message }));
            }
        }
    }
}