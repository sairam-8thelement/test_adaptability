using Microsoft.Extensions.Configuration;

namespace BlazorEcommerce.Tests
{
    public static class TestDb
    {
        public const string JwtKey = "ThisIsATestOnlyJwtSigningKeyThatIsAtLeast64CharactersLongForHS512";

        public static DataContext NewContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new DataContext(options);
            context.Database.EnsureCreated();
            return context;
        }

        public static IConfiguration NewConfig()
        {
            return new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["AppSettings:Token"] = JwtKey
                })
                .Build();
        }
    }
}
