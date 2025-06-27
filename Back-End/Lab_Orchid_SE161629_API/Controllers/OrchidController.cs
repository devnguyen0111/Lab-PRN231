using DataAccess.ResponseModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models.Models;
using Repositorys.DTOs.OrchidDTO;
using Repositorys.PaginatedList;
using Services.IService;
using System.ComponentModel.DataAnnotations;

namespace Lab_Orchid_SE161629_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrchidController : ControllerBase
    {
        private readonly IOrchidService _orchidService;

        public OrchidController(IOrchidService orchidService)
        {
            _orchidService = orchidService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Orchid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllOrchids()
        {
            var orchids = await _orchidService.GetAllOrchidsAsync();
            return Ok(BaseResponseModel<IEnumerable<Orchid>>.OkResponseModel(orchids));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BaseResponseModel<Orchid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<Orchid>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrchidById(int id)
        {
            var orchid = await _orchidService.GetOrchidByIdAsync(id);
            if (orchid == null)
                return NotFound(BaseResponseModel<Orchid>.NotFoundResponseModel(null, new { message = "Orchid not found" }));

            return Ok(BaseResponseModel<Orchid>.OkResponseModel(orchid));
        }

        [HttpGet("paged")]
        [ProducesResponseType(typeof(BaseResponseModel<PaginatedList<Orchid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrchidsPaginated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var orchids = await _orchidService.GetOrchidsPaginatedAsync(pageNumber, pageSize);
            return Ok(BaseResponseModel<PaginatedList<Orchid>>.OkResponseModel(orchids));
        }

        [HttpGet("category/{categoryId}")]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Orchid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrchidsByCategory(int categoryId)
        {
            var orchids = await _orchidService.GetOrchidsByCategoryAsync(categoryId);
            return Ok(BaseResponseModel<IEnumerable<Orchid>>.OkResponseModel(orchids));
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(BaseResponseModel<Orchid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponseModel<Orchid>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrchid([FromBody] CreateOrchidDTO orchidDTO)
        {
            try
            {
                var orchid = await _orchidService.CreateOrchidAsync(orchidDTO);
                return CreatedAtAction(nameof(GetOrchidById), new { id = orchid.OrchidId },
                    BaseResponseModel<Orchid>.OkResponseModel(orchid));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(BaseResponseModel<Orchid>.BadRequestResponseModel(null, new { message = ex.Message }));
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BaseResponseModel<string>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateOrchid(int id, [FromBody] UpdateOrchidDTO orchidDTO)
        {
            try
            {
                var result = await _orchidService.UpdateOrchidAsync(id, orchidDTO);
                if (!result)
                    return NotFound(BaseResponseModel<string>.NotFoundResponseModel(null, new { message = "Orchid not found" }));

                return Ok(BaseResponseModel<string>.OkResponseModel(null, new { message = "Orchid updated successfully" }));
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
        public async Task<IActionResult> DeleteOrchid(int id)
        {
            var result = await _orchidService.DeleteOrchidAsync(id);
            if (!result)
                return NotFound(BaseResponseModel<string>.NotFoundResponseModel(null, new { message = "Orchid not found" }));

            return Ok(BaseResponseModel<string>.OkResponseModel(null, new { message = "Orchid deleted successfully" }));
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Orchid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchOrchids([FromQuery] string searchTerm)
        {
            var orchids = await _orchidService.SearchOrchidsAsync(searchTerm);
            return Ok(BaseResponseModel<IEnumerable<Orchid>>.OkResponseModel(orchids));
        }

        /// <summary>
        /// Get orchids with advanced filtering, searching, and sorting capabilities
        /// </summary>
        /// <param name="searchTerm">Search in name, description, and category name (optional)</param>
        /// <param name="minPrice">Minimum price filter (optional)</param>
        /// <param name="maxPrice">Maximum price filter (optional)</param>
        /// <param name="isNatural">Filter by natural/artificial type (optional)</param>
        /// <param name="categoryId">Filter by category ID (optional)</param>
        /// <param name="sortBy">Sort by: name, price, category, natural (optional, defaults to name)</param>
        /// <param name="ascending">Sort direction (optional, defaults to true)</param>
        /// <param name="pageNumber">Page number (optional, defaults to 1)</param>
        /// <param name="pageSize">Items per page (optional, defaults to 10)</param>
        /// <returns>Paginated list of filtered orchids</returns>
        [HttpGet("filter")]
        [ProducesResponseType(typeof(BaseResponseModel<PaginatedList<Orchid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> FilterOrchids(
            [FromQuery] string? searchTerm = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] bool? isNatural = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true,
            [FromQuery][Range(1, int.MaxValue)] int pageNumber = 1,
            [FromQuery][Range(1, 100)] int pageSize = 10)
        {
            var orchids = await _orchidService.FilterOrchidsAsync(
                searchTerm, minPrice, maxPrice, isNatural, categoryId,
                sortBy, ascending, pageNumber, pageSize);

            return Ok(BaseResponseModel<PaginatedList<Orchid>>.OkResponseModel(orchids));
        }

        [HttpGet("price-range")]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Orchid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrchidsByPriceRange([FromQuery] decimal minPrice, [FromQuery] decimal maxPrice)
        {
            var orchids = await _orchidService.GetOrchidsByPriceRangeAsync(minPrice, maxPrice);
            return Ok(BaseResponseModel<IEnumerable<Orchid>>.OkResponseModel(orchids));
        }

        [HttpGet("by-type/{isNatural}")]
        [ProducesResponseType(typeof(BaseResponseModel<IEnumerable<Orchid>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrchidsByType(bool isNatural)
        {
            var orchids = await _orchidService.GetOrchidsByTypeAsync(isNatural);
            return Ok(BaseResponseModel<IEnumerable<Orchid>>.OkResponseModel(orchids));
        }

        [HttpGet("category-distribution")]
        [ProducesResponseType(typeof(BaseResponseModel<Dictionary<string, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrchidsCountByCategory()
        {
            var distribution = await _orchidService.GetOrchidsCountByCategory();
            return Ok(BaseResponseModel<Dictionary<string, int>>.OkResponseModel(distribution));
        }

        [HttpGet("price-distribution")]
        [ProducesResponseType(typeof(BaseResponseModel<Dictionary<decimal, int>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOrchidsPriceDistribution([FromQuery] int numberOfRanges = 5)
        {
            var distribution = await _orchidService.GetOrchidsPriceDistribution(numberOfRanges);
            return Ok(BaseResponseModel<Dictionary<decimal, int>>.OkResponseModel(distribution));
        }
    }
}