using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProduct()
        {
            var result = await _productService.GetProductsAsync();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("{productId}")]
        public async Task<ActionResult<ServiceResponse<Product>>> GetProduct(int productId)
        {
            var result = await _productService.GetProductAsync(productId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("category/{categoryUrl}")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetProductsByCategory(string categoryUrl)
        {
            var result = await _productService.GetProductsByCategory(categoryUrl);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("search/{searchText}/{page}")]
        public async Task<ActionResult<ServiceResponse<ProductSearchResult>>> SearchProducts(string searchText, int page)
        {
            if (string.IsNullOrWhiteSpace(searchText))
            {
                return BadRequest(new ServiceResponse<ProductSearchResult>
                {
                    Success = false,
                    Message = "Search text must not be empty."
                });
            }

            var result = await _productService.SearchPeoducts(searchText, page);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("searchsuggestions/{searchText}")]
        public async Task<ActionResult<ServiceResponse<List<string>>>> GetProductsSearchSuggestions(string searchText)
        {
            var result = await _productService.GetProductsSearchSuggestions(searchText);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("featured")]
        public async Task<ActionResult<ServiceResponse<List<Product>>>> GetFeaturedProducts()
        {
            var result = await _productService.GetFeaturedProducts();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}