using BlazorEcommerce.Server.Services.CartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [AllowAnonymous]
        [HttpPost("products")]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = await _cartService.GetCartProducts(cartItems);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> StoreCartItems(List<CartItem> cartItems)
        {
            var result = await _cartService.StoreCartItems(cartItems);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet("count")]
        public async Task<ActionResult<ServiceResponse<int>>> GetCartItemsCount()
        {
            var result = await _cartService.GetCartItemsCount();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CartProductResponse>>>> GetDbCartProducts()
        {
            var result = await _cartService.GetDbCartProducts();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPost("add")]
        public async Task<ActionResult<ServiceResponse<bool>>> AddToCart(CartItem cartItems)
        {
            var result = await _cartService.AddToCart(cartItems);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("update-quantity")]
        public async Task<ActionResult<ServiceResponse<bool>>> UpdateQuantity(CartItem cartItems)
        {
            var result = await _cartService.UpdateQuantity(cartItems);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("{productId}/{productTypeId}")]
        public async Task<ActionResult<ServiceResponse<bool>>> RemoveItemFromCart(int productId, int productTypeId)
        {
            var result = await _cartService.RemoveItemFromCart(productId, productTypeId);
            if (!result.Success)
            {
                return NotFound(result);
            }
            return Ok(result);
        }
    }
}