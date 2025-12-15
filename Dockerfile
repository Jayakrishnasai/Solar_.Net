# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files
COPY SolarSystem.sln ./
COPY src/SolarSystem.Shared/SolarSystem.Shared.csproj src/SolarSystem.Shared/
COPY src/SolarSystem.Api/SolarSystem.Api/SolarSystem.Api.csproj src/SolarSystem.Api/SolarSystem.Api/
COPY src/SolarSystem.Api/SolarSystem.Api.Client/SolarSystem.Api.Client.csproj src/SolarSystem.Api/SolarSystem.Api.Client/

# Restore dependencies
RUN dotnet restore SolarSystem.sln

# Copy all source files
COPY src/ src/

# Build and publish
WORKDIR /src/src/SolarSystem.Api/SolarSystem.Api
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "SolarSystem.Api.dll"]
