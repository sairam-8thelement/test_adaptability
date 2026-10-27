namespace BlazorEcommerce.Client.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _http;

        public ProductService(HttpClient http)
        {
            _http = http;
        }

        public event Action ProductsCahanged;

        public List<Product> Products { get; set; } = new List<Product>();
        public string Message { get; set; } = "Loading products...";
        public int CurrentPage { get; set; } = 1;
        public int PageCount { get; set; } = 0;
        public string LastSearchText { get; set; } = string.Empty;

        public async Task<ServiceResponse<Product>> GetProduct(int productId)
        {
            var httpResponse = await _http.GetAsync($"api/product/{productId}");
            var result = await TryReadServiceResponse<Product>(httpResponse);
            if (result != null)
            {
                return result;
            }

            return new ServiceResponse<Product>
            {
                Success = false,
                Message = "Sorry, but this product does not exist."
            };
        }

        public async Task GetProducts(string? categoryUrl = null)
        {
            var httpResponse = categoryUrl == null ?
             await _http.GetAsync("api/product/featured") :
             await _http.GetAsync($"api/product/category/{categoryUrl}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var result = await TryReadServiceResponse<List<Product>>(httpResponse);
                if (result != null && result.Data != null)
                    Products = result.Data;

                CurrentPage = 1;
                PageCount = 0;
                if (Products.Count == 0)
                {
                    Message = "No products found";
                }
            }
            else
            {
                Products = new List<Product>();
                Message = "No products found";
                CurrentPage = 1;
                PageCount = 0;
            }

            ProductsCahanged.Invoke();
        }

        public async Task SearchProducts(string searchText, int page)
        {
            LastSearchText = searchText;
            var httpResponse = await _http.GetAsync($"api/product/search/{searchText}/{page}");

            if (httpResponse.IsSuccessStatusCode)
            {
                var result = await TryReadServiceResponse<ProductSearchResult>(httpResponse);
                if (result != null && result.Data != null)
                {
                    Products = result.Data.Products;
                    CurrentPage = result.Data.CurrentPage;
                    PageCount = result.Data.Pages;
                }
                if (Products.Count == 0) Message = "No Products found.";
            }
            else
            {
                var errorResult = await TryReadServiceResponse<ProductSearchResult>(httpResponse);
                Products = new List<Product>();
                PageCount = 0;
                Message = errorResult != null && !string.IsNullOrEmpty(errorResult.Message)
                    ? errorResult.Message
                    : "No Products found.";
            }

            ProductsCahanged.Invoke();
        }

        public async Task<List<string>> GetProductSearchSuggestions(string searchText)
        {
            var httpResponse = await _http.GetAsync($"api/product/searchsuggestions/{searchText}");

            if (!httpResponse.IsSuccessStatusCode)
            {
                return new List<string>();
            }

            var result = await TryReadServiceResponse<List<string>>(httpResponse);
            return result?.Data ?? new List<string>();
        }

        private static async Task<ServiceResponse<T>?> TryReadServiceResponse<T>(HttpResponseMessage httpResponse)
        {
            try
            {
                return await httpResponse.Content.ReadFromJsonAsync<ServiceResponse<T>>();
            }
            catch
            {
                return null;
            }
        }
    }
}