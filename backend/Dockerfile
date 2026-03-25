# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy solution and project files
COPY Eventia.Backend.sln .
COPY src/Eventia.API/Eventia.API.csproj ./src/Eventia.API/
COPY src/Eventia.Application/Eventia.Application.csproj ./src/Eventia.Application/
COPY src/Eventia.Domain/Eventia.Domain.csproj ./src/Eventia.Domain/
COPY src/Eventia.Infrastructure/Eventia.Infrastructure.csproj ./src/Eventia.Infrastructure/

# Restore dependencies
RUN dotnet restore

# Copy all source and build
COPY . .
RUN dotnet publish src/Eventia.API/Eventia.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Environment variables
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "Eventia.API.dll"]
