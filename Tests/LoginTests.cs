using BlazorEcommerce.Server.Services.AuthService;
using Microsoft.AspNetCore.Http;

namespace BlazorEcommerce.Tests
{
    public class LoginTests
    {
        [Fact]
        public async Task Login_WithValidCredentials_ReturnsAuthenticatedResult()
        {
            // Arrange
            var context = TestDb.NewContext();
            var authService = new AuthService(context, TestDb.NewConfig(), Mock.Of<IHttpContextAccessor>());

            const string email = "login-test@example.com";
            const string password = "SuperSecret123!";

            var registerResponse = await authService.Register(new User { Email = email }, password);
            Assert.True(registerResponse.Success);

            // Act
            var loginResponse = await authService.Login(email, password);

            // Assert
            Assert.True(loginResponse.Success);
            Assert.False(string.IsNullOrWhiteSpace(loginResponse.Data));

            var segments = loginResponse.Data!.Split('.');
            Assert.Equal(3, segments.Length);
        }
    }
}
