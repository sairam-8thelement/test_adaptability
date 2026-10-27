# syntax=docker/dockerfile:1

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Restore first so NuGet packages are cached between source changes.
COPY BlazorEcommerce.sln ./
COPY Server/BlazorEcommerce.Server.csproj Server/
COPY Client/BlazorEcommerce.Client.csproj Client/
COPY Shared/BlazorEcommerce.Shared.csproj Shared/
RUN dotnet restore Server/BlazorEcommerce.Server.csproj

# Publish the hosted server (this also builds and bundles the Blazor WASM client).
COPY . .
RUN dotnet publish Server/BlazorEcommerce.Server.csproj -c Release -o /app/publish

# ---------- Runtime stage ----------
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BlazorEcommerce.Server.dll"]
