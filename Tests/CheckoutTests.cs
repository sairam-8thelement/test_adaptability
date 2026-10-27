using BlazorEcommerce.Server.Services.AuthService;
using BlazorEcommerce.Server.Services.CartService;
using BlazorEcommerce.Server.Services.OrderService;

namespace BlazorEcommerce.Tests
{
    public class CheckoutTests
    {
        private const int UserId = 99;

        [Fact]
        public async Task PlaceOrder_WithItemsInCart_CreatesOrderAndEmptiesCart()
        {
            // Arrange
            var context = TestDb.NewContext();

            context.CartItems.Add(new CartItem
            {
                UserId = UserId,
                ProductId = 1,
                ProductTypeId = 2,
                Quantity = 2
            });
            await context.SaveChangesAsync();

            var authMock = new Mock<IAuthService>();
            authMock.Setup(a => a.GetUserId()).Returns(UserId);

            var cartProducts = new List<CartProductResponse>
            {
                new CartProductResponse
                {
                    ProductId = 1,
                    ProductTypeId = 2,
                    Title = "The Hitchhiker's Guide to the Galaxy",
                    ProductType = "Paperback",
                    Price = 9.99m,
                    Quantity = 2
                }
            };

            var cartMock = new Mock<ICartService>();
            cartMock.Setup(c => c.GetDbCartProducts()).ReturnsAsync(new ServiceResponse<List<CartProductResponse>>
            {
                Data = cartProducts
            });

            var orderService = new OrderService(context, cartMock.Object, authMock.Object);

            // Act
            var placeOrderResponse = await orderService.PlaceOrder();

            // Assert - checkout succeeded
            Assert.True(placeOrderResponse.Success);
            Assert.True(placeOrderResponse.Data);

            // Assert - order persisted atomically with correct totals and items
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.UserId == UserId);

            Assert.NotNull(order);
            Assert.Equal(19.98m, order!.TotalPrice);
            Assert.NotNull(order.OrderItems);
            Assert.Single(order.OrderItems);

            var orderItem = order.OrderItems[0];
            Assert.Equal(1, orderItem.ProductId);
            Assert.Equal(2, orderItem.ProductTypeId);
            Assert.Equal(2, orderItem.Quantity);
            Assert.Equal(19.98m, orderItem.TotalPrice);

            // Assert - cart items for the user were cleared in the same save
            var remainingCartItems = await context.CartItems.Where(ci => ci.UserId == UserId).ToListAsync();
            Assert.Empty(remainingCartItems);
        }
    }
}
