using BlazorEcommerce.Server.Services.ProductService;

namespace BlazorEcommerce.Tests
{
    public class ProductCatalogTests
    {
        [Fact]
        public async Task SearchPeoducts_ReturnsMatchingProduct()
        {
            // Arrange
            using var context = TestDb.NewContext();
            context.Products.Add(new Product
            {
                Id = 9001,
                Title = "Zorblatt Chronicles",
                Description = "A nonsense token product used to make search tests deterministic.",
                ImageUrl = "https://example.com/zorblatt.jpg",
                CategoryId = 1
            });
            await context.SaveChangesAsync();

            var productService = new ProductService(context);

            // Act
            var result = await productService.SearchPeoducts("Zorblatt", 1);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data!.CurrentPage);
            Assert.Equal(1, result.Data.Pages);
            Assert.Single(result.Data.Products);
            Assert.Equal(9001, result.Data.Products.Single().Id);
        }

        [Fact]
        public async Task GetProductsByCategory_ReturnsProductsInCategory()
        {
            // Arrange
            using var context = TestDb.NewContext();
            var productService = new ProductService(context);

            // Act
            var result = await productService.GetProductsByCategory("books");

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data!);
            Assert.All(result.Data, product => Assert.Equal(1, product.CategoryId));
            Assert.Contains(result.Data, product => product.Title == "Ready Player One");
        }
    }
}
