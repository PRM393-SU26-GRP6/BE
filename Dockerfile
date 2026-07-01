# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CourtManager.APIs/CourtManager.APIs.csproj", "CourtManager.APIs/"]
COPY ["CourtManager.Application/CourtManager.Application.csproj", "CourtManager.Application/"]
COPY ["CourtManager.Domain/CourtManager.Domain.csproj", "CourtManager.Domain/"]
COPY ["CourtManager.Infrastructure/CourtManager.Infrastructure.csproj", "CourtManager.Infrastructure/"]
RUN dotnet restore "./CourtManager.APIs/CourtManager.APIs.csproj"
COPY . .
WORKDIR "/src/CourtManager.APIs"
RUN dotnet build "./CourtManager.APIs.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CourtManager.APIs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CourtManager.APIs.dll"]
