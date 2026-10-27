# BlazorEcommerce

Hosted Blazor WebAssembly e-commerce demo (.NET 6) with an ASP.NET Core API and SQL Server via EF Core.

## Run with Docker (one command)

Requirements: Docker with Compose v2.

```bash
docker compose up --build -d
```

That builds the app image, starts SQL Server 2022, waits for it to be healthy, applies the
EF Core migrations (including seed products/categories), and starts the site.

- App / UI: http://localhost:8080
- Swagger: http://localhost:8080/swagger
- SQL Server: `localhost,14333` (user `sa`, password `YourStrong!Passw0rd`)

Useful commands:

```bash
docker compose logs -f app        # follow app logs (migrations, requests)
docker compose ps                 # health of db + app
docker compose down               # stop (keeps database volume)
docker compose down -v            # stop and delete the database
```

### Configuration

All settings have defaults in `docker-compose.yml`. To override, copy `.env.example` to `.env`
and edit it:

| Variable                 | Default                                  | Purpose                        |
| ------------------------ | ---------------------------------------- | ------------------------------ |
| `MSSQL_SA_PASSWORD`      | `YourStrong!Passw0rd`                    | SQL Server `sa` password       |
| `JWT_TOKEN_KEY`          | `change-me-to-a-long-random-secret-key`  | Signing key for JWT auth       |
| `APP_PORT`               | `8080`                                   | Host port for the web app      |
| `DB_PORT`                | `14333`                                  | Host port for SQL Server       |
| `ASPNETCORE_ENVIRONMENT` | `Production`                             | Set `Development` for WASM debugging |

## Run locally without Docker

Needs the .NET 6 SDK and a reachable SQL Server. Set `ConnectionStrings:DefaultConnection`
in `Server/appsettings.json` (or via environment variable `ConnectionStrings__DefaultConnection`), then:

```bash
dotnet run --project Server
```

Migrations are applied automatically on startup.
