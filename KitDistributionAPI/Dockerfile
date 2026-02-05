# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copy csproj explicitly
COPY KitDistributionAPI.csproj ./
RUN dotnet restore KitDistributionAPI.csproj

# copy rest of source
COPY . .
RUN dotnet publish KitDistributionAPI.csproj -c Release -o out

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/out .

ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "KitDistributionAPI.dll"]
