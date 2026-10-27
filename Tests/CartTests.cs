using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Services.CartService;

namespace BlazorEcommerce.Tests
{
    public class CartTests
    {
        private const int UserId = 42;

        [Fact]
        public async Task AddToCart_ThenGetCartProducts_ReflectsAddedItem()
        {
            // Arrange
            var context = TestDb.NewContext();

            var authMock = new Mock<IAuthService>();
            authMock.Setup(a => a.GetUserId()).Returns(UserId);

            var cartService = new CartService(context, authMock.Object);

            // Act - add an item that has a seeded ProductVariant + ProductType
            var addResponse = await cartService.AddToCart(new CartItem
            {
                ProductId = 1,
                ProductTypeId = 2,
                Quantity = 2
            });

            // Assert - add succeeded
            Assert.True(addResponse.Success);
            Assert.True(addResponse.Data);

            // Act - read back via the user-scoped query
            var cartResponse = await cartService.GetDbCartProducts();

            // Assert - item is reflected in the cart with resolved product/variant data
            Assert.True(cartResponse.Success);
            Assert.NotNull(cartResponse.Data);
            Assert.Single(cartResponse.Data!);

            var cartProduct = cartResponse.Data![0];
            Assert.Equal(1, cartProduct.ProductId);
            Assert.Equal(2, cartProduct.ProductTypeId);
            Assert.Equal(2, cartProduct.Quantity);
            Assert.False(string.IsNullOrWhiteSpace(cartProduct.Title));
            Assert.False(string.IsNullOrWhiteSpace(cartProduct.ProductType));
        }
    }
}
