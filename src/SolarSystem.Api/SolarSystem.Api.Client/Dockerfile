FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj files and restore
COPY SolarSystem.Api.Client.csproj ./
COPY ../../SolarSystem.Shared/SolarSystem.Shared.csproj ../../SolarSystem.Shared/
RUN dotnet restore

# copy everything else and build
COPY ../../SolarSystem.Shared/ ./SolarSystem.Shared/
RUN dotnet publish -c Release -o /app/publish

# runtime stage using nginx to serve static files
FROM nginx:alpine AS runtime
COPY --from=build /app/publish/wwwroot /usr/share/nginx/html
EXPOSE 80
