# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file
COPY KitDistributionAPI/KitDistributionAPI.csproj KitDistributionAPI/
RUN dotnet restore KitDistributionAPI/KitDistributionAPI.csproj

# Copy everything else
COPY . .
WORKDIR /src/KitDistributionAPI
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080

ENTRYPOINT ["dotnet", "KitDistributionAPI.dll"]
